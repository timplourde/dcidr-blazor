using Dcidr.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;

namespace Dcidr.Excel.Tests
{
    [TestClass]
    public class ExcelExporterTests
    {
        [TestMethod]
        public void CanGenerateExcelFile()
        {
            var bytes = ExcelExporter.GenerateExcel(GivenADecision());
            File.WriteAllBytes("file.xlsx", bytes);
        }

        private Decision GivenADecision()
        {
            var d = new Decision();
            d.Options.Add("o1");
            d.Options.Add("o2");
            d.Criteria.Add("c1");
            d.Criteria.Add("c2");
            d.Criteria.Add("c3");

            foreach (var oc in d.OptionComparisons)
            {
                oc.SetWeight(Weight.GreaterThan);
            }

            d.CriteriaComparisons.First(c => c.CriterionOne == "c1" && c.CriterionTwo == "c2").SetWeight(Weight.GreaterThan);
            d.CriteriaComparisons.First(c => c.CriterionOne == "c1" && c.CriterionTwo == "c3").SetWeight(Weight.MuchGreaterThan);
            d.CriteriaComparisons.First(c => c.CriterionOne == "c2" && c.CriterionTwo == "c3").SetWeight(Weight.MuchLessThan);
            return d;
        }
    }
}
