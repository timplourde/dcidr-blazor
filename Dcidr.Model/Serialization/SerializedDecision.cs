using System;
using System.Collections.Generic;
using System.Text;

namespace Dcidr.Model.Serialization
{
    public class SerializedDecision
    {
        public string Id { get; set; }
        public DateTime DateCreatedUtc { get; set; }
        public string Name { get; set; }
        public string[] Criteria { get; set; }
        public string[] Options { get; set; }
        public SerializedCriteriaComparison[] CriteriaComparisons { get; set; }
        public SerializedOptionComparison[] OptionComparisons { get; set; }
    }
}
