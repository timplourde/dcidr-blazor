using Dcidr.Model.Serialization;
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
            DateCreatedUtc = DateTime.UtcNow;
            Options = new DecisionItemCollection();
            Options.OnChange += Options_OnChange;
            Criteria = new DecisionItemCollection();
            Criteria.OnChange += Criteria_OnChange;
            CriteriaComparisons = new List<CriteriaComparison>();
            OptionComparisons = new List<OptionComparison>();
            Results = new List<Result>();
        }

        public string Id { get; private set; }
        public DateTime DateCreatedUtc { get; private set; }
        public string Name { get; set; }
        public DecisionItemCollection Options { get; private set; }
        public DecisionItemCollection Criteria { get; private set; }
        public List<CriteriaComparison> CriteriaComparisons { get; private set; }
        public List<OptionComparison> OptionComparisons { get; private set; }
        public List<Result> Results { get; private set; }

        public bool HasEnoughOptions => Options.Count >= 2;
        public bool HasEnoughCriteria => Criteria.Count >= 2;
        public bool AllCriteriaComparisonsHaveWeights => CriteriaComparisons.All(c => c.Weight != null);
        public bool AllOptionComparisonsHaveWeights => OptionComparisons.All(c => c.Weight != null);
        public bool ResultPrerequisitesMet => HasEnoughCriteria && HasEnoughOptions
            && AllCriteriaComparisonsHaveWeights && AllOptionComparisonsHaveWeights;

        public static Decision Deserialize(SerializedDecision serializedDecision)
        {
            var d = new Decision();
            d.Id = serializedDecision.Id;
            d.Name = serializedDecision.Name;
            d.DateCreatedUtc = serializedDecision.DateCreatedUtc;
            if(serializedDecision.Criteria != null)
            {
                foreach (var crit in serializedDecision.Criteria)
                {
                    d.Criteria.Add(crit);
                }
            }
            
            if(serializedDecision.Options != null)
            {
                foreach (var opt in serializedDecision.Options)
                {
                    d.Options.Add(opt);
                }
            }

            if(serializedDecision.OptionComparisons != null)
            {
                foreach(var soc in serializedDecision.OptionComparisons)
                {
                    var oc = d.OptionComparisons.FirstOrDefault(doc => doc.Criterion == soc.Criterion
                        && doc.OptionOne == soc.OptionOne
                        && doc.OptionTwo == soc.OptionTwo);
                    if(oc != null && soc.Weight.HasValue)
                    {
                        oc.SetWeight(soc.Weight.Value);
                    }
                }
            }

            if (serializedDecision.CriteriaComparisons != null)
            {
                foreach (var scc in serializedDecision.CriteriaComparisons)
                {
                    var cc = d.CriteriaComparisons.FirstOrDefault(dcc => dcc.CriterionOne == scc.CriterionOne
                        && dcc.CriterionTwo == scc.CriterionTwo);
                    if (cc != null && scc.Weight.HasValue)
                    {
                        cc.SetWeight(scc.Weight.Value);
                    }
                }
            }

            return d;
        }

        public SerializedDecision Serialize()
        {
            return new SerializedDecision
            {
                Id = Id,
                Name = Name,
                DateCreatedUtc = DateCreatedUtc,
                Options = Options.Items.ToArray(),
                Criteria = Criteria.Items.ToArray(),
                CriteriaComparisons = CriteriaComparisons.Select(cc => new SerializedCriteriaComparison
                {
                    CriterionOne = cc.CriterionOne,
                    CriterionTwo = cc.CriterionTwo,
                    Weight = cc.Weight
                }).ToArray(),
                OptionComparisons = OptionComparisons.Select(oc => new SerializedOptionComparison
                {
                    Criterion = oc.Criterion,
                    OptionOne = oc.OptionOne,
                    OptionTwo = oc.OptionTwo,
                    Weight = oc.Weight
                }).ToArray()
            };
        }

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
            if(ResultPrerequisitesMet)
            {
                Results = ResultGenerator.GenerateResults(this);
            }
            else
            {
                Results.Clear();
            }
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
                    exitstingOc.OnWeightChange -= OnWeightChange;
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
                    exitstingCc.OnWeightChange -= OnWeightChange;
                    CriteriaComparisons.Remove(exitstingCc);
                }
            }

            CriteriaComparisons = CriteriaComparisons.OrderBy(c => c.CriterionOne).ToList();
        }
    }
}
