using Dcidr.Model.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Dcidr.Model.Tests
{
    [TestClass]
    public class DecisionArchiveTests
    {
        private DecisionArchive GivenAnArchive()
        {
            return new DecisionArchive()
            {
                Decisions = new[]
                {
                    new DecisionArchiveItem()
                    {
                        Id = "a",
                        DateCreatedUtc = DateTime.UtcNow.AddDays(-1),
                        Name = "a"
                    },
                    new DecisionArchiveItem()
                    {
                        Id = "b",
                        DateCreatedUtc = DateTime.UtcNow.AddDays(-2),
                        Name = "b"
                    }
                }
            };
        }
        
        [TestMethod]
        public void CanUpsert_Add()
        {
            var archive = GivenAnArchive();
            archive.Upsert(new DecisionArchiveItem
            {
                Id = "c",
                DateCreatedUtc = DateTime.UtcNow,
                Name = "c"
            });

            Assert.AreEqual(3, archive.Decisions.Length);
            Assert.AreEqual("c", archive.Decisions.First().Id);
        }

        [TestMethod]
        public void CanUpsert_Update()
        {
            var archive = GivenAnArchive();
            archive.Upsert(new DecisionArchiveItem
            {
                Id = "a",
                DateCreatedUtc = DateTime.UtcNow.AddDays(-1),
                Name = "a"
            });

            Assert.AreEqual(2, archive.Decisions.Length);
            Assert.AreEqual("a", archive.Decisions.First().Id);
        }

        [TestMethod]
        public void CanRemove()
        {
            var archive = GivenAnArchive();
            archive.Remove(new DecisionArchiveItem
            {
                Id = "a",
                DateCreatedUtc = DateTime.UtcNow.AddDays(-1),
                Name = "a"
            });

            Assert.AreEqual(1, archive.Decisions.Length);
            Assert.AreEqual("b", archive.Decisions.First().Id);
        }
    }
}
