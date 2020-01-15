using System;
using System.Collections.Generic;
using System.Text;

namespace Dcidr.Model
{
    public class CriteriaComparison
    {
        public CriteriaComparison(string criterionOne, string criterionTwo)
        {
            CriterionOne = criterionOne;
            CriterionTwo = criterionTwo;
        }

        public string CriterionOne { get; }
        public string CriterionTwo { get; }
        public Weight? Weight { get; private set; }
        public event EventHandler OnWeightChange;

        public void SetWeight(Weight w)
        {
            Weight = w;
            OnWeightChange?.Invoke(this, new EventArgs());
        }

        public override int GetHashCode()
        {
            var hashCode = 240112566;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CriterionOne);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CriterionTwo);
            return hashCode;
        }
    }
}
