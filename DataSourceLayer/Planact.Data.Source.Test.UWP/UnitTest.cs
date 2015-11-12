using System;
using System.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Microsoft.Data.Entity;

namespace Planact.Data.Source.Test.UWP
{
    [TestClass]
    public class BasicShootAndForgetTests
    {
        [TestMethod]
        public void CanCreateDatabase()
        {
            using (var db = new MyContext())
            {
                db.Database.EnsureCreated();
            }
        }

        [TestMethod]
        public void CanPersistObject()
        {
            // Arrange
            var rng = new Random();
            var expectedId = 1;
            var expectedValue = rng.NextDouble().ToString();

            // Act
            using (var db = new MyContext())
            {
                var data = new SampleData() { Id = expectedId, Value = expectedValue };
                var record = db.SampleData.FirstOrDefault(d => d.Id == expectedId);
                if(record==null)
                    db.SampleData.Add(data);
                db.SaveChanges();
            }

            // Assert
            using (var db = new MyContext())
            {
                var record = db.SampleData.FirstOrDefault(d => d.Id == expectedId);
                Assert.IsNotNull(record);
                Assert.AreEqual(expectedValue, record.Value);
            }
        }
    }
}
