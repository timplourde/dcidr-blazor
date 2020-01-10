using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dcidr.Model
{
    public static class ResultGenerator
    {
        public static List<Result> GenerateResults(Decision decision)
        {
            // WARNING: all preconditions should be validated

            // add inverse criteria comparisons
            var critComps = decision.CriteriaComparisons.ToList();
            var inverseCritComps = critComps.Select(cc =>
            {
                var inverseCc = new CriteriaComparison(cc.CriteriaTwo, cc.CriteriaOne);
                inverseCc.SetWeight(GetInverseWeight(cc.Weight.Value));
                return inverseCc;
            }).ToList();
            critComps.AddRange(inverseCritComps);

            var totalCritCompWeight = critComps.Sum(cc => GetWeightFactor(cc.Weight.Value));
            
            // build RDVs for each criterion
            var criterionRdvs = critComps.GroupBy(c => c.CriteriaOne).Select(g =>
                new
                {
                    Criteria = g.Key,
                    Rdv = g.Sum(cc => GetWeightFactor(cc.Weight.Value)) / totalCritCompWeight
                }).ToDictionary(d=>d.Criteria, d=>d.Rdv);

            // key is criteria, value is option - RDV
            var optionRdvsByCriterion = new Dictionary<string, Dictionary<string, decimal>>();
            foreach(var crit in decision.Criteria.Items)
            {
                var ocs = decision.OptionComparisons.Where(oc => oc.Criterion == crit).ToList();
                // add inverses
                var inverseOcs = ocs.Select(cc =>
                {
                    var inverseOc = new OptionComparison(crit, cc.OptionTwo, cc.OptionOne);
                    inverseOc.SetWeight(GetInverseWeight(cc.Weight.Value));
                    return inverseOc;
                }).ToList();
                ocs.AddRange(inverseOcs);

                var totalWeight = ocs.Sum(oc => GetWeightFactor(oc.Weight.Value));

                var optionRdvs = ocs.GroupBy(c => c.OptionOne).Select(g =>
                   new
                   {
                       Option = g.Key,
                       Rdv = g.Sum(cc => GetWeightFactor(cc.Weight.Value)) / totalWeight
                   }).ToDictionary(d => d.Option, d => d.Rdv);

                optionRdvsByCriterion.Add(crit, optionRdvs);
            }

            var results = new List<Result>();
           

            return new List<Result>();
        }

        private static decimal GetWeightFactor(Weight weight)
        {
            switch (weight)
            {
                case Weight.MuchLessThan:
                    return 0.1m;
                case Weight.LessThan:
                    return 0.2m;
                case Weight.AboutEqualTo:
                    return 1m;
                case Weight.GreaterThan:
                    return 5m;
                case Weight.MuchGreaterThan:
                    return 10m;
                default:
                    throw new ArgumentException(nameof(weight));
            }
        }

        private static Weight GetInverseWeight(Weight weight)
        {
            switch (weight)
            {
                case Weight.MuchLessThan:
                    return Weight.MuchGreaterThan;
                case Weight.LessThan:
                    return Weight.GreaterThan;
                case Weight.AboutEqualTo:
                    return Weight.AboutEqualTo;
                case Weight.GreaterThan:
                    return Weight.LessThan;
                case Weight.MuchGreaterThan:
                    return Weight.MuchLessThan;
                default:
                    throw new ArgumentException(nameof(weight));
            }
        }
    }
}
