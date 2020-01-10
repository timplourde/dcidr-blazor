using System;
using System.Collections.Generic;
using System.Text;

namespace Dcidr.Model
{
    public class CriteriaComparison
    {
        public CriteriaComparison(string criteriaOne, string criteriaTwo)
        {
            CriteriaOne = criteriaOne;
            CriteriaTwo = criteriaTwo;
        }

        public string CriteriaOne { get; }
        public string CriteriaTwo { get; }
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
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CriteriaOne);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CriteriaTwo);
            return hashCode;
        }
    }
}
