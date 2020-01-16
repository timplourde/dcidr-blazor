using System;
using System.Collections.Generic;
using System.Text;

namespace Dcidr.Model
{
    public enum Weight
    {
        MuchLessThan,
        LessThan,
        AboutEqualTo,
        GreaterThan,
        MuchGreaterThan
    }

    public static class WeightExtensions
    {
        public static decimal ToRatio(this Weight weight)
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
    }
}
