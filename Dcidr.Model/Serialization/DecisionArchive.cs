using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dcidr.Model.Serialization
{
    public class DecisionArchive
    {
        public DecisionArchiveItem[] Decisions { get; set; } = new DecisionArchiveItem[] { };

        public void Upsert(DecisionArchiveItem decisionArchiveItem)
        {
            var existingDecision = Decisions.FirstOrDefault(d => d.Id == decisionArchiveItem.Id);
            if(existingDecision != null)
            {
                existingDecision = decisionArchiveItem;
            }
            else
            {
                Decisions = Decisions.Append(decisionArchiveItem).OrderByDescending(d => d.DateCreatedUtc).ToArray();
            }
        }

        public void Remove(DecisionArchiveItem decisionArchiveItem)
        {
            Decisions = Decisions.Where(i => i.Id != decisionArchiveItem.Id).ToArray();
        }
    }

    public class DecisionArchiveItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime DateCreatedUtc { get; set; }
    }
}
