using System;
using System.Linq.Expressions;
using Demo.Foundation.ProcessingEngine.Facets;
using Sitecore.Framework.Rules;
using Sitecore.XConnect;
using Sitecore.XConnect.Segmentation.Predicates;

namespace Demo.Foundation.ProcessingEngine.Conditionals
{
    public class ClusterMatch : ICondition, IContactSearchQueryFactory
    {
        public NumericOperationType Comparison { get; set; }

        public int Number { get; set; }
        public bool Evaluate(IRuleExecutionContext context)
        {
            var contact = context.Fact<Contact>();
            var facet = contact.GetFacet<RfmContactFacet>(RfmContactFacet.DefaultFacetKey);
            if (facet == null) return false;
            int cluster = facet.Cluster;

            return Comparison.Evaluate(cluster, Number);
        }

        public Expression<Func<Contact, bool>> CreateContactSearchQuery(IContactSearchQueryContext context)
        {
            return contact => Comparison.Evaluate(contact.GetFacet<RfmContactFacet>(RfmContactFacet.DefaultFacetKey).Cluster, Number);
        }
    }
}
