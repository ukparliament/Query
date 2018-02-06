using System;
using System.Collections.Generic;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Parsing.Handlers;

namespace FriendlyHierarchyTests
{
    /// <summary>
    /// An RDF Handler which wraps another handler, stripping explicit xsd:string datatypes on object literals
    /// </summary>
    public class StripStringHandler : BaseRdfHandler, IWrappingRdfHandler
    {
        private IRdfHandler _handler;
        private static readonly Uri DataTypeString = new Uri(XmlSpecsHelper.XmlSchemaDataTypeString);

        /// <summary>
        /// Creates a new StripStringHandler
        /// </summary>
        /// <param name="handler">Inner handler to use</param>
        public StripStringHandler(IRdfHandler handler) : base()
        {
            _handler = handler ?? throw new ArgumentNullException("handler");
        }

        /// <summary>
        /// Handles triples by stripping explicit xsd:string datatype on object literals before delegating to inner handler
        /// </summary>
        protected override bool HandleTripleInternal(Triple t)
        {
            if (t.Object is ILiteralNode literal && EqualityHelper.AreUrisEqual(literal.DataType, DataTypeString))
                t = new Triple(
                    t.Subject,
                    t.Predicate,
                    t.Graph.CreateLiteralNode(literal.Value));

            return _handler.HandleTriple(t);
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
        /// Delegates base Uri handling to inner handler
        /// </summary>
        protected override bool HandleBaseUriInternal(Uri baseUri) => _handler.HandleBaseUri(baseUri);

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