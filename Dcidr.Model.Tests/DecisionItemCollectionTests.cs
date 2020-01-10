using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Dcidr.Model.Tests
{
    [TestClass]
    public class DecisionItemCollectionTests
    {
        [TestMethod]
        public void Add()
        {
            var sut = new DecisionItemCollection();
            bool handledEvent = false;
            sut.OnChange += (sender, e) =>
            {
                handledEvent = true;
            };

            sut.Add("item");

            Assert.AreEqual(1, sut.Items.Count());
            Assert.IsTrue(handledEvent);
        }

        [TestMethod]
        public void Add_Unique()
        {
            var sut = new DecisionItemCollection();

            sut.Add("item");
            sut.Add("item");

            Assert.AreEqual(1, sut.Items.Count());
        }

        [TestMethod]
        public void Remove()
        {
            var sut = new DecisionItemCollection();
            bool handledEvent = false;

            sut.Add("item");

            sut.OnChange += (sender, e) =>
            {
                handledEvent = true;
            };

            sut.Remove("item");

            Assert.AreEqual(0, sut.Items.Count());
            Assert.IsTrue(handledEvent);
        }
    }
}
