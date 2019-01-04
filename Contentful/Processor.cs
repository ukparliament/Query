// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

namespace Contentful
{
    using Contentful.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using VDS.RDF;
    using VDS.RDF.Parsing;

    public class Processor : IDisposable
    {
        public IGraph Graph { get; private set; }

        public Processor(ContentfulClass instance) : this(instance.AsEnumerable()) { }

        public Processor(IEnumerable<ContentfulClass> instance)
        {
            this.Graph = new Graph();

            foreach (ContentfulClass item in instance)
            {
                this.VisitSubject(item);
            }

            this.CleanGraph();
        }

        private void CleanGraph()
        {
            var singleStatementSubjects = this.Graph.Triples.Where(triple => this.Graph.GetTriplesWithSubject(triple.Subject).Count() == 1);
            var statementsOfSingleStatementSubject = singleStatementSubjects.SelectMany(triple => this.Graph.GetTriples(triple.Subject));

            this.Graph.Retract(statementsOfSingleStatementSubject.ToArray());
        }

        private void VisitSubject(ContentfulClass instance)
        {
            var subject = this.GetId(instance);

            this.VisitType(instance, subject);

            this.VisitStatements(instance, subject);
        }

        private INode GetId(ContentfulClass instance)
        {
            var baseUriAttribute = instance.GetType().GetCustomAttribute<BaseUriAttribute>();
            if (baseUriAttribute == null)
            {
                throw new Exception($"BaseUriAttribute missing on {instance.GetType()}");
            }

            var id = instance.Sys.Id;
            if (instance is IUriEntry uriEntry)
            {
                id = uriEntry.Uri;
            }

            return this.Graph.CreateUriNode(new Uri(new Uri(baseUriAttribute.Uri), id));
        }

        private void VisitType(ContentfulClass instance, INode subject)
        {
            var classAttribute = instance.GetType().GetCustomAttribute<ClassAttribute>();
            if (classAttribute != null)
            {
                this.Graph.Assert(
                    subject,
                    this.Graph.CreateUriNode(new Uri(RdfSpecsHelper.RdfType)),
                    this.Graph.CreateUriNode(new Uri(classAttribute.Uri))
                );
            }
        }

        private void VisitStatements(ContentfulClass instance, INode subject)
        {
            var statements = instance
                .GetType()
                .GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(PredicateAttribute)))
                .Select(p => new
                {
                    Predicate = this.Graph.CreateUriNode(new Uri(p.GetCustomAttribute<PredicateAttribute>().Uri)),
                    Object = p.GetValue(instance)
                })
                .Where(field => field.Object != null);

            foreach (var statement in statements)
            {
                if (statement.Object is ContentfulClass contentful)
                {
                    this.VisitObject(subject, statement.Predicate, contentful);
                }
                else if (statement.Object is IEnumerable<ContentfulClass> collection)
                {
                    this.VisitObject(subject, statement.Predicate, collection);
                }
                else
                {
                    this.VisitObject(subject, statement.Predicate, statement.Object.ToString());
                }
            }
        }

        private void VisitObject(INode subject, IUriNode predicate, IEnumerable<ContentfulClass> collection)
        {
            foreach (var item in collection)
            {
                this.VisitObject(subject, predicate, item);
            }
        }

        private void VisitObject(INode subject, IUriNode predicate, ContentfulClass item)
        {
            var @object = this.GetId(item);
            this.Graph.Assert(subject, predicate, @object);

            if (!this.Graph.GetTriplesWithSubject(@object).Any())
            {
                this.VisitSubject(item);
            }
        }

        private void VisitObject(INode subject, IUriNode predicate, string literal)
        {
            this.Graph.Assert(
                subject,
                predicate,
                this.Graph.CreateLiteralNode(literal)
            );
        }

        public void Dispose()
        {
            this.Graph.Dispose();
        }
    }
}
