using Demo.Foundation.ProcessingEngine.Facets;
using Sitecore.Analytics;
using Sitecore.Analytics.XConnect.Facets;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Demo.Foundation.ProcessingEngine.Conditionals
{
    public class PersonalizeCluster<T> : IntegerComparisonCondition<T> where T : RuleContext
    {
        protected override bool Execute(T ruleContext)
        {
            if (!Tracker.Current.IsActive || Tracker.Current.Contact == null) return false;

            var xConnectFacets = Tracker.Current.Contact.GetFacet<IXConnectFacets>("XConnectFacets");
            if (xConnectFacets?.Facets == null || !xConnectFacets.Facets.ContainsKey(RfmContactFacet.DefaultFacetKey)) return false;

            if (!(xConnectFacets.Facets[RfmContactFacet.DefaultFacetKey] is RfmContactFacet facet)) return false;

            return Compare(facet.Cluster);
        }
    }
}
