using Dcidr.Model.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Dcidr.Model.Tests
{
    [TestClass]
    public class SeralizationTests
    {
        [TestMethod]
        public void CanDeserialize()
        {
            var serializedDecision = new SerializedDecision
            {
                Id = "foo",
                Name = "bar",
                DateCreatedUtc = new System.DateTime(),
                Options = new[] {"o1", "o2"},
                Criteria = new[] {"c1", "c2"},
                CriteriaComparisons = new []
                {
                    new SerializedCriteriaComparison
                    {
                        CriterionOne = "c1",
                        CriterionTwo = "c2",
                        Weight = Weight.GreaterThan
                    }
                },
                OptionComparisons = new []
                {
                    new SerializedOptionComparison
                    {
                        Criterion = "c1",
                        OptionOne = "o1",
                        OptionTwo = "o2",
                        Weight = Weight.MuchGreaterThan
                    },
                    new SerializedOptionComparison
                    {
                        Criterion = "c2",
                        OptionOne = "o1",
                        OptionTwo = "o2",
                        Weight = Weight.MuchLessThan
                    }
                }
            };

            var d = Decision.Deserialize(serializedDecision);

            Assert.AreEqual("foo", d.Id);
            Assert.AreEqual("bar", d.Name);
            Assert.AreEqual(serializedDecision.DateCreatedUtc, d.DateCreatedUtc); 
            CollectionAssert.AreEqual(new[] { "c1", "c2" }, d.Criteria.Items.ToArray());
            CollectionAssert.AreEqual(new[] { "o1", "o2" }, d.Options.Items.ToArray());
            d.CriteriaComparisons.Single(cc => cc.CriterionOne == "c1"
                && cc.CriterionTwo == "c2"
                && cc.Weight == Weight.GreaterThan);
            d.OptionComparisons.Single(cc => cc.Criterion == "c1"
                && cc.OptionOne == "o1"
                && cc.OptionTwo == "o2"
                && cc.Weight == Weight.MuchGreaterThan);
            d.OptionComparisons.Single(cc => cc.Criterion == "c2"
               && cc.OptionOne == "o1"
               && cc.OptionTwo == "o2"
               && cc.Weight == Weight.MuchLessThan);

            Assert.AreEqual(2, d.Results.Count);
            var actualResults = d.Results.Select(r => (r.Option, r.Score.ToString("N4"))).ToArray();
            CollectionAssert.AreEqual(new[]{
                    ("o1", "0.9524"),
                    ("o2", "0.0476")
                }, actualResults);
        }

        [TestMethod]
        public void CanSerialize()
        {
            var d = new Decision();
            d.Name = "bar";
            d.Criteria.Add("c1");
            d.Criteria.Add("c2");
            d.Options.Add("o1");
            d.Options.Add("o2");
            foreach(var oc in d.OptionComparisons)
            {
                oc.SetWeight(Weight.GreaterThan);
            }
            foreach(var cc in d.CriteriaComparisons)
            {
                cc.SetWeight(Weight.LessThan);
            }

            var sd = d.Serialize();

            Assert.AreEqual(d.Id, sd.Id);
            Assert.AreEqual(d.Name, sd.Name);
            Assert.AreEqual(d.DateCreatedUtc, sd.DateCreatedUtc);
            CollectionAssert.AreEqual(new[]
            {
                "c1",
                "c2"
            }, sd.Criteria);
            
            CollectionAssert.AreEqual(new[]
            {
                "o1",
                "o2"
            }, sd.Options);

            CollectionAssert.AreEqual(new[]
            {
                ("c1", "o1", "o2", Weight.GreaterThan),
                ("c2", "o1", "o2", Weight.GreaterThan),
            }, sd.OptionComparisons.Select(oc => (oc.Criterion, oc.OptionOne, oc.OptionTwo, oc.Weight.Value)).ToArray());

            CollectionAssert.AreEqual(new[]
            {
                ("c1", "c2", Weight.LessThan)
            }, sd.CriteriaComparisons.Select(cc => (cc.CriterionOne, cc.CriterionTwo, cc.Weight.Value)).ToArray());
        }
    }
}
