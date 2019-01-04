// MIT License
//
// Copyright (c) 2019 UK Parliament
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using VDS.RDF;
using VDS.RDF.Parsing.Handlers;

namespace FriendlyHierarchyTests
{
    /// <summary>
    /// An RDF Handler which wraps another handler, stripping explicit xsd:string datatypes on object literals
    /// </summary>
    public class UriMappingHandler : BaseRdfHandler, IWrappingRdfHandler
    {
        private readonly IDictionary<Uri, Uri> _mapping;
        private readonly IRdfHandler _handler;

        /// <summary>
        /// Creates a new UriMappingGraphHandler
        /// </summary>
        /// <param name="handler">Inner handler to use</param>
        public UriMappingHandler(IRdfHandler handler, INodeFactory factory, IDictionary<Uri, Uri> mapping) : base(factory)
        {
            _handler = handler ?? throw new ArgumentNullException("handler");
            _mapping = mapping ?? throw new ArgumentNullException("handler");
        }

        protected override bool HandleBaseUriInternal(Uri baseUri)
        {
            return base.HandleBaseUriInternal(this.Convert(baseUri));
        }

        public override IUriNode CreateUriNode(Uri uri)
        {
            return base.CreateUriNode(this.Convert(uri));
        }

        // TODO: This is naive
        private Uri Convert(Uri original)
        {
            foreach (var item in this._mapping)
            {
                var incorrectBase = item.Key;

                if (incorrectBase.IsBaseOf(original))
                {
                    var correctBase = item.Value;
                    var relative = incorrectBase.MakeRelativeUri(original);
                    var corrected = new Uri(correctBase, relative);

                    return corrected;
                }
            }

            return original;
        }

        #region Delegate to inner handler

        /// <summary>
        /// Gets the handler wrapped by this handler
        /// </summary>
        public IEnumerable<IRdfHandler> InnerHandlers => _handler.AsEnumerable();

        /// <summary>
        /// Starts inner handler
        /// </summary>
        protected override void StartRdfInternal() => _handler.StartRdf();

        /// <summary>
        /// Ends inner handler
        /// </summary>
        protected override void EndRdfInternal(bool ok) => _handler.EndRdf(ok);

        /// <summary>
        /// Delegates triple handling to inner handler
        /// </summary>
        protected override bool HandleTripleInternal(Triple t) => _handler.HandleTriple(t);

        /// <summary>
        /// Delegates namespace handling to inner handler
        /// </summary>
        protected override bool HandleNamespaceInternal(string prefix, Uri namespaceUri) => _handler.HandleNamespace(prefix, namespaceUri);

        /// <summary>
        /// Gets whether inner handler accepts all triples
        /// </summary>
        public override bool AcceptsAll => _handler.AcceptsAll;

        #endregion
    }
}