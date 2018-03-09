namespace FriendlyHierarchy
{
    using System.Linq;
    using VDS.RDF;

    public abstract class SpanningAlgorithm
    {
        protected readonly SpanningAlgorithmSettings settings;
        private INode[] multiParents;

        protected IGraph Graph { get; private set; }

        public SpanningAlgorithm(SpanningAlgorithmSettings settings)
        {
            this.settings = settings ?? throw new System.ArgumentNullException(nameof(settings));
        }

        protected void Initialize(IGraph g)
        {
            var orderedStatements = g.Triples.OrderByDescending(statement => statement, new HierarchyTripleComparer());
            this.Graph = new Graph();
            this.Graph.Assert(orderedStatements);

            if (!this.settings.Flat && this.settings.LiftMultiParents)
            {
                this.multiParents =
                    this.Graph
                    .Nodes
                    .Except(this.Graph.Nodes.LiteralNodes())
                    .Where(node => this.Graph.GetTriplesWithObject(node).Count() > 1)
                    .ToArray();
            }
        }

        protected virtual void WriteRdf()
        {
            while (this.Graph.Triples.FirstOrDefault() is Triple triple)
            {
                this.WriteSubject(triple.Subject);
            }
        }

        protected virtual void WriteSubject(INode node)
        {
            this.WriteSubjectId(node);

            var groups = this
                .Graph
                .GetTriplesWithSubject(node)
                .GroupBy(t => t.Predicate as IUriNode);

            foreach (var group in groups)
            {
                this.WritePredicateGroup(group);
            }
        }

        protected virtual void WriteSubjectId(INode node)
        {
        }

        protected virtual void WritePredicateGroup(IGrouping<IUriNode, Triple> statements)
        {
            this.Graph.Retract(statements);

            foreach (var statement in statements)
            {
                this.WritePredicate(statement);
            }
        }

        protected virtual void WritePredicate(Triple statement)
        {
            if (this.settings.UseLists && statement.Object.IsListRoot(this.Graph))
            {
                this.WriteList(statement.Object);
            }
            else
            {
                this.WriteObject(statement.Object);
            }
        }

        protected virtual void WriteList(INode node)
        {
            var items = this.Graph.GetListItems(node);

            var list = this.Graph.GetListAsTriples(node);
            this.Graph.Retract(list);

            foreach (var item in items)
            {
                this.WriteListItem(item);
            }
        }

        protected virtual void WriteListItem(INode node)
        {
            this.WriteObject(node);
        }

        protected virtual void WriteLiteral(ILiteralNode node)
        {
        }

        protected virtual void WriteObject(INode node)
        {
            if (node is ILiteralNode literal)
            {
                this.WriteLiteral(literal);
            }
            else if (this.settings.Flat || this.settings.LiftMultiParents && this.multiParents.Contains(node))
            {
                this.WriteObjectId(node);
            }
            else
            {
                this.WriteSubject(node);
            }
        }

        protected virtual void WriteObjectId(INode node)
        {
        }
    }
}
