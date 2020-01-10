using System;
using System.Collections.Generic;
using System.Linq;

namespace Dcidr.Model
{
    public class Decision
    {
        public Decision()
        {
            Id = Guid.NewGuid().ToString();
            Options = new DecisionItemCollection();
            Options.OnChange += Options_OnChange;
            Criteria = new DecisionItemCollection();
            Criteria.OnChange += Criteria_OnChange;
            CriteriaComparisons = new List<CriteriaComparison>();
            OptionComparisons = new List<OptionComparison>();
            Results = new List<Result>();
        }

        public string Id { get; }
        public DecisionItemCollection Options { get; set; }
        public DecisionItemCollection Criteria { get; set; }
        public List<CriteriaComparison> CriteriaComparisons { get; private set; }
        public List<OptionComparison> OptionComparisons { get; private set; }
        public List<Result> Results { get; private set; }

        public bool HasEnoughOptions => Options.Count >= 2;
        public bool HasEnoughCriteria => Criteria.Count >= 2;
        public bool AllCriteriaComparisonsHaveWeights => CriteriaComparisons.All(c => c.Weight != null);
        public bool AllOptionComparisonsHaveWeights => OptionComparisons.All(c => c.Weight != null);

        private void Options_OnChange(object sender, EventArgs e)
        {
            UpdateOptionComparisons();
            UpdateResults();
        }

        private void Criteria_OnChange(object sender, EventArgs e)
        {
            UpdateOptionComparisons();
            UpdateCriteriaComparisons();
            UpdateResults();
        }

        private void OnWeightChange(object sender, EventArgs e)
        {
            UpdateResults();
        }

        private void UpdateResults()
        {
            if(HasEnoughCriteria && HasEnoughOptions && AllCriteriaComparisonsHaveWeights && AllOptionComparisonsHaveWeights)
            {
                Results = ResultGenerator.GenerateResults(this);
            }
            else
            {
                Results.Clear();
            }
        }

        private List<Result> GenerateResults()
        {
            throw new NotImplementedException();
        }

        private void UpdateOptionComparisons()
        {
            var currentHashCodes = new HashSet<int>(OptionComparisons.Select(c => c.GetHashCode()));
            var proposedHashCodes = new HashSet<int>();
            foreach(var crit in Criteria.Items)
            {
                for (int i = 0; i < Options.Items.Count(); i++)
                {
                    for (int j = i + 1; j < Options.Items.Count(); j++)
                    {
                        var oc = new OptionComparison(crit, Options.Items.ElementAt(i), Options.Items.ElementAt(j));
                        oc.OnWeightChange += OnWeightChange;
                        var hashCode = oc.GetHashCode();
                        proposedHashCodes.Add(hashCode);
                        if (!currentHashCodes.Contains(hashCode))
                        {
                            OptionComparisons.Add(oc);
                        }
                    }
                }
            }
            
            foreach(var exitstingOc in OptionComparisons.ToList())
            {
                if (!proposedHashCodes.Contains(exitstingOc.GetHashCode()))
                {
                    OptionComparisons.Remove(exitstingOc);
                }
            }

            OptionComparisons = OptionComparisons.OrderBy(b => b.Criterion).ToList();
        }

        private void UpdateCriteriaComparisons()
        {
            var currentHashCodes = new HashSet<int>(CriteriaComparisons.Select(c => c.GetHashCode()));
            var proposedHashCodes = new HashSet<int>();
            for (int i = 0; i < Criteria.Items.Count(); i++)
            {
                for (int j = i + 1 ; j < Criteria.Items.Count(); j++)
                {
                    var cc = new CriteriaComparison(Criteria.Items.ElementAt(i), Criteria.Items.ElementAt(j));
                    cc.OnWeightChange += OnWeightChange;
                    var hashCode = cc.GetHashCode();
                    proposedHashCodes.Add(hashCode);
                    if (!currentHashCodes.Contains(hashCode))
                    {
                        CriteriaComparisons.Add(cc);
                    }
                }
            }

            foreach (var exitstingCc in CriteriaComparisons.ToList())
            {
                if (!proposedHashCodes.Contains(exitstingCc.GetHashCode()))
                {
                    CriteriaComparisons.Remove(exitstingCc);
                }
            }

            CriteriaComparisons = CriteriaComparisons.OrderBy(c => c.CriteriaOne).ToList();
        }
    }
}
