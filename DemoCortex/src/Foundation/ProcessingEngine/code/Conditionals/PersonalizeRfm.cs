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
    public class PersonalizeRfm<T> : StringOperatorCondition<T> where T : RuleContext
    {
        public Guid RfmId { get; set; }

        protected override bool Execute(T ruleContext)
        {
            if (!Tracker.Current.IsActive || Tracker.Current.Contact == null) return false;

            var xConnectFacets = Tracker.Current.Contact.GetFacet<IXConnectFacets>("XConnectFacets");
            if (xConnectFacets == null || xConnectFacets.Facets == null ||
                !xConnectFacets.Facets.ContainsKey(RfmContactFacet.DefaultFacetKey)) return false;

            RfmContactFacet facet = xConnectFacets.Facets[RfmContactFacet.DefaultFacetKey] as RfmContactFacet;

            if (facet == null) return false;

            var rfm = GetRfm();
            if (rfm == null) return false;

            return facet.R == rfm.R && facet.F == rfm.F && facet.M == rfm.M;
        }

        public CustomerBusinessValue GetRfm()
        {
            int r, f, m;
            Item rfmPattern = Database.GetDatabase("master").GetItem(new ID(RfmId));
            if (rfmPattern == null) return null;

            if (int.TryParse(rfmPattern["R"], out r))
            {
                if (int.TryParse(rfmPattern["F"], out f))
                {
                    if (int.TryParse(rfmPattern["M"], out m))
                    {
                        return new CustomerBusinessValue
                        {
                            R = r,
                            F = f,
                            M = m
                        };
                    }
                }
            }

            return null;
        }
    }
}
