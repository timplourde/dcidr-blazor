using System;
using System.Collections.Generic;
using System.Text;

namespace Dcidr.Model.Serialization
{
    public class DecisionArchive
    {
        public DecisionArchiveItem[] Decisions { get; set; }
    }

    public class DecisionArchiveItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreatedUtc { get; set; }
    }
}
