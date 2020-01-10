using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Dcidr.Model.Tests
{
    [TestClass]
    public class DecisionTests
    {
        
        [TestMethod]
        public void HasEnoughOptions_Works()
        {
            var d = new Decision();
            d.Options.Add("opt a");
            Assert.IsFalse(d.HasEnoughOptions);
            d.Options.Add("opt b");
            Assert.IsTrue(d.HasEnoughOptions);
        }

        [TestMethod]
        public void HasEnoughCriteria_Works()
        {
            var d = new Decision();
            d.Criteria.Add("crit a");
            Assert.IsFalse(d.HasEnoughCriteria);
            d.Criteria.Add("crit b");
            Assert.IsTrue(d.HasEnoughCriteria);
        }

        [TestMethod]
        public void AllCriteriaComparisonsHaveWeights_Works()
        {
            var d = new Decision();
            d.Criteria.Add("crit a");
            d.Criteria.Add("crit b");
            Assert.IsFalse(d.AllCriteriaComparisonsHaveWeights);
            foreach (var cc in d.CriteriaComparisons)
            {
                cc.SetWeight(Weight.AboutEqualTo);
            }
            Assert.IsTrue(d.AllCriteriaComparisonsHaveWeights);
        }

        [TestMethod]
        public void AllOptionComparisonsHaveWeights_Works()
        {
            var d = new Decision();
            d.Criteria.Add("crit a");
            d.Criteria.Add("crit b");
            d.Options.Add("opt x");
            d.Options.Add("opt y");
            Assert.IsFalse(d.AllOptionComparisonsHaveWeights);
            foreach (var oc in d.OptionComparisons)
            {
                oc.SetWeight(Weight.AboutEqualTo);
            }
            Assert.IsTrue(d.AllOptionComparisonsHaveWeights);
        }

        [TestMethod]
        public void AddOption_AddsOptionCompaisons()
        {
            var d = new Decision();
            d.Criteria.Add("crit-x");
            d.Criteria.Add("crit-y");
            d.Options.Add("opt-a");
            d.Options.Add("opt-b");

            Assert.AreEqual(2, d.OptionComparisons.Count);
            d.Options.Add("opt-c");
            Assert.AreEqual(6, d.OptionComparisons.Count);
        }

        [TestMethod]
        public void RemoveOption_RemovesOptionComparisons()
        {
            var d = new Decision();
            d.Criteria.Add("crit-x");
            d.Criteria.Add("crit-y");
            d.Options.Add("opt-a");
            d.Options.Add("opt-b");
            d.Options.Add("opt-c");
            d.Options.Remove("opt-c");
            Assert.AreEqual(2, d.OptionComparisons.Count);
        }

        [TestMethod]
        public void AddCriteria_AddsOptionComparisons_AndCriteriaComparisons()
        {
            var d = new Decision();
            d.Options.Add("opt-a");
            d.Options.Add("opt-b");

            d.Criteria.Add("crit-x");
            Assert.AreEqual(1, d.OptionComparisons.Count);
            Assert.AreEqual(0, d.CriteriaComparisons.Count);

            d.Criteria.Add("crit-y");
            Assert.AreEqual(2, d.OptionComparisons.Count);
            Assert.AreEqual(1, d.CriteriaComparisons.Count);

            d.Criteria.Add("crit-z");
            Assert.AreEqual(3, d.OptionComparisons.Count);
            Assert.AreEqual(3, d.CriteriaComparisons.Count);
        }

        [TestMethod]
        public void RemoveCriteria_RemovesOptionComparisons_AndCriteriaComparisons()
        {
            var d = new Decision();
            d.Options.Add("opt-a");
            d.Options.Add("opt-b");
            d.Criteria.Add("crit-x");
            d.Criteria.Add("crit-y");
            Assert.AreEqual(2, d.OptionComparisons.Count);
            Assert.AreEqual(1, d.CriteriaComparisons.Count);

            d.Criteria.Remove("crit-z");
            Assert.AreEqual(2, d.OptionComparisons.Count);
            Assert.AreEqual(1, d.CriteriaComparisons.Count);
        }

        [TestMethod]
        public void SettingAllWeights_CreatesAReport()
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

            d.CriteriaComparisons.First(c => c.CriteriaOne == "c1" && c.CriteriaTwo == "c2").SetWeight(Weight.GreaterThan);
            d.CriteriaComparisons.First(c => c.CriteriaOne == "c1" && c.CriteriaTwo == "c3").SetWeight(Weight.MuchGreaterThan);
            d.CriteriaComparisons.First(c => c.CriteriaOne == "c2" && c.CriteriaTwo == "c3").SetWeight(Weight.MuchLessThan);

            Assert.IsTrue(d.ResultPrerequisitesMet);

            Assert.AreEqual(2, d.Results.Count());
            var winner = d.Results.First();
            Assert.AreEqual("o1", winner.Option);
            Assert.AreEqual("0.9615", winner.Score.ToString("N4"));

            var secondPlace = d.Results.ElementAt(1);
            Assert.AreEqual("o2", secondPlace.Option);
            Assert.AreEqual("0.0385", secondPlace.Score.ToString("N4"));
        }
    }
}
