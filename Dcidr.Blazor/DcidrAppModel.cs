using Dcidr.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dcidr.Blazor
{
    public class DcidrAppModel
    {
        public Decision Decision { get; set; }

        public void NewDecision()
        {
            Decision = new Decision();
            Decision.Options.Add("o1");
            Decision.Options.Add("o2");
            Decision.Criteria.Add("c1");
            Decision.Criteria.Add("c2");
        }
    }
}
