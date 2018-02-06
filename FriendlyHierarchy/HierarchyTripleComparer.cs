namespace FriendlyHierarchy
{
    using System;
    using System.Linq;
    using VDS.RDF;
    using VDS.RDF.Parsing;

    internal class HierarchyTripleComparer : BaseTripleComparer
    {
        private readonly static Uri rdf_type = new Uri(RdfSpecsHelper.RdfType);

        private readonly static Func<Triple, bool>[] tests = new Func<Triple, bool>[] {
                IsRoot, // subjects that are not objects elsewhere come first
                IsNamedSubject, // named subjects precede blanks
                IsTypeStatement,
                IsLiteralObject,
                IsList, // list roots are lifted
                IsNamedObject, // named objects precede blanks
                IsBlankObject // blank objects precede literals
            };

        public override int Compare(Triple x, Triple y)
        {
            if (x == y)
            {
                return 0;
            }

            // Run tests on statement pairs until you find a conclusive one, then skip the rest
            foreach (var test in HierarchyTripleComparer.tests)
            {
                // Test both statements
                var resultX = test(x);
                var resultY = test(y);

                // Equality implies inconclusive test, we must continue testing
                if (resultX != resultY)
                {
                    // Results are unequal, outcome is 
                    // either (1 - 0) ==  1: x is greater than y
                    //     or (0 - 1) == -1: x is less than y
                    return Convert.ToInt32(resultX) - Convert.ToInt32(resultY);
                }
            }

            // Default outcome is
            // 0: Statements are equal
            return 0;
        }

        private static bool IsRoot(Triple statement)
        {
            return !statement.Graph.GetTriplesWithObject(statement.Subject).Any();
        }

        private static bool IsNamedSubject(Triple statement)
        {
            return statement.Subject is IUriNode;
        }

        private static bool IsTypeStatement(Triple statement)
        {
            return (statement.Predicate as IUriNode).Uri.Equals(HierarchyTripleComparer.rdf_type);
        }

        private static bool IsLiteralObject(Triple statement)
        {
            return statement.Object is ILiteralNode;
        }

        private static bool IsList(Triple statement)
        {
            return
                !(statement.Object is ILiteralNode)
                && statement.Object.IsListRoot(statement.Graph);
        }

        private static bool IsNamedObject(Triple statement)
        {
            return statement.Object is IUriNode;
        }

        private static bool IsBlankObject(Triple statement)
        {
            return statement.Object is IBlankNode;
        }
    }
}
