using System;
using Demo.Foundation.ProcessingEngine.Facets;
using Demo.Foundation.ProcessingEngine.Models;
using Sitecore.Analytics;
using Sitecore.Analytics.XConnect.Facets;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Demo.Foundation.ProcessingEngine.Conditionals
{
    public class PersonalizeCluster<T> : StringOperatorCondition<T> where T : RuleContext
    {
        public int Number { get; set; }

        protected override bool Execute(T ruleContext)
        {
            if (!Tracker.Current.IsActive || Tracker.Current.Contact == null) return false;

            var xConnectFacets = Tracker.Current.Contact.GetFacet<IXConnectFacets>("XConnectFacets");
            if (xConnectFacets==null || xConnectFacets.Facets == null ||
                !xConnectFacets.Facets.ContainsKey(RfmContactFacet.DefaultFacetKey)) return false;

            RfmContactFacet facet = xConnectFacets.Facets[RfmContactFacet.DefaultFacetKey] as RfmContactFacet;

            if (facet == null) return false;

            return facet.Cluster == Number;
        }
    }
}
