using System;
using System.Collections.Generic;
using System.Text;

namespace Dcidr.Model
{
    public class OptionComparison
    {
        public OptionComparison(string criterion, string optionOne, string optionTwo)
        {
            Criterion = criterion;
            OptionOne = optionOne;
            OptionTwo = optionTwo;
        }

        public string Criterion { get; }
        public string OptionOne { get; }
        public string OptionTwo { get; }
        public Weight? Weight { get; private set; }
        public event EventHandler OnWeightChange;

        public void SetWeight(Weight w)
        {
            Weight = w;
            OnWeightChange?.Invoke(this, new EventArgs());
        }

        public override int GetHashCode()
        {
            var hashCode = 772646710;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Criterion);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(OptionOne);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(OptionTwo);
            return hashCode;
        }
    }
}
