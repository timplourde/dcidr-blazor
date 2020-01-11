using System;
using System.Collections.Generic;
using System.Text;

namespace Dcidr.Model.Serialization
{
    public class SerializedCriteriaComparison
    {
        public string CriteriaOne { get; set; }
        public string CriteriaTwo { get; set; }
        public Weight? Weight { get; set; }
    }
}
