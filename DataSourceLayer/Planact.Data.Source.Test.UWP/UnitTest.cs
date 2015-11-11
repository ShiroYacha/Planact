using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Planact.Data.Source.Test.UWP
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var db = new MyContext())
            {
                db.Database.EnsureCreated();
            }
        }
    }
}
