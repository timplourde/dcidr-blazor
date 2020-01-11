using System;
using System.Collections.Generic;
using System.Text;

namespace Dcidr.Model.Serialization
{
    public class SerializedOptionComparison
    {
        public string Criterion { get; set; }
        public string OptionOne { get; set; }
        public string OptionTwo { get; set; }
        public Weight? Weight { get; set; }
    }
}
