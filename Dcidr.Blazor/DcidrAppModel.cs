using Dcidr.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dcidr.Blazor
{
    public class DcidrAppModel
    {
        public Decision Decision { get; set; } = new Decision();

        public void NewDecision()
        {
            Decision = new Decision();
        }
    }
}
