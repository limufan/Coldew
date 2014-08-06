using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core;
using NUnit.Framework;

namespace Coldew.NnitTest
{
    class C1
    {
        public string p1 { set; get; }
        public DateTime p2 { private set; get; }
        public List<string> p3 { set; get; }
    }

    class C2: C1
    {
        public string p4 { set; get; }
    }

    class C3 : C1
    {
        public string p4 { set; get; }
    }

    public class ClassPropertyHelperTester
    {

        [Test]
        public void TestChangeProperty()
        {
            C1 c1 = new C1();
            c1.p1 = "11";
            c1.p3 = new List<string> {"1", "2" };

            C2 c2 = new C2();
            ClassPropertyHelper.ChangeProperty(c1, c2);
            AssertHelper.AreEqual(c1, c2);
        }
    }
}
