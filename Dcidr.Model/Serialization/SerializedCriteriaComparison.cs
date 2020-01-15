using System;
using System.Collections.Generic;
using System.Text;

namespace Dcidr.Model.Serialization
{
    public class SerializedCriteriaComparison
    {
        public string CriterionOne { get; set; }
        public string CriterionTwo { get; set; }
        public Weight? Weight { get; set; }
    }
}
