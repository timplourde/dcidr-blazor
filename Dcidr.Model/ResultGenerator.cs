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
            if (!decision.ResultPrerequisitesMet)
            {
                throw new ArgumentException("Not all prerequisites have been met to calculate results");
            }

            var criteriaRdvs = CalculateCriteriaRdvs(decision);

            var adjustedOptionCriteriaRdvs = CalculateCriteriaAdjustedOptionRdvs(decision, criteriaRdvs);

            return adjustedOptionCriteriaRdvs.GroupBy(x => x.option)
                .Select(g => new Result(g.Key, g.Sum(o => o.adjustedRdv)))
                .OrderByDescending(r=>r.Score)
                .ToList();
        }

        private static Dictionary<string, decimal> CalculateCriteriaRdvs(Decision decision)
        {
            // add inverse criteria comparisons
            var criteriaComparisons = decision.CriteriaComparisons.ToList();
            
            // add inverses
            var inverseCriteriaComparisons = criteriaComparisons.Select(cc =>
            {
                var inverseCc = new CriteriaComparison(cc.CriterionTwo, cc.CriterionOne);
                inverseCc.SetWeight(GetInverseWeight(cc.Weight.Value));
                return inverseCc;
            }).ToList();
            criteriaComparisons.AddRange(inverseCriteriaComparisons);

            var totalCritCompWeight = criteriaComparisons.Sum(cc => GetWeightFactor(cc.Weight.Value));

            // build RDVs for each criterion
            var criteriaRdvs = criteriaComparisons.GroupBy(c => c.CriterionOne).Select(g =>
                new
                {
                    Criteria = g.Key,
                    Rdv = g.Sum(cc => GetWeightFactor(cc.Weight.Value)) / totalCritCompWeight
                }).ToDictionary(d => d.Criteria, d => d.Rdv);

            return criteriaRdvs;
        }

        private static List<(string option, string criteria, decimal adjustedRdv)> CalculateCriteriaAdjustedOptionRdvs(Decision decision, Dictionary<string, decimal> criteriaRdvs)
        {
            var adjustedOptionCriteriaRdvs = new List<(string option, string criteria, decimal adjustedRdv)>();
            foreach (var crit in decision.Criteria.Items)
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

                // calculate the total RDV for each option for this criterion
                var optionRdvs = ocs.GroupBy(c => c.OptionOne).Select(g =>
                   new
                   {
                       Option = g.Key,
                       Rdv = g.Sum(cc => GetWeightFactor(cc.Weight.Value)) / totalWeight
                   }).ToList();

                // record the criteria-RDV-adjusted option total RDV
                foreach (var optionRdv in optionRdvs)
                {
                    var adjustedOptionRdv = criteriaRdvs[crit] * optionRdv.Rdv;
                    adjustedOptionCriteriaRdvs.Add((optionRdv.Option, crit, adjustedOptionRdv));
                }
            }
            return adjustedOptionCriteriaRdvs;
        }

        private static decimal GetWeightFactor(Weight weight) => weight.ToRatio();

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
