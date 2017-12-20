using System;
using System.Collections.Generic;
using System.Text;

namespace NUnit.Framework.Internal.Filters
{
    public class MiscTestClass
    {
        [Test]
        public void LotsOfFail2()
        {
            Assert.IsTrue(false);
        }
    }
}
