using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;

namespace Coldew.NnitTest
{
    public class AssertHelper
    {
        public static void AreEqual(object expected, object actual)
        {
            Type expectedType = expected.GetType();
            Type actualType = actual.GetType();
            PropertyInfo[] expectedProperties = expectedType.GetProperties();
            foreach (PropertyInfo expectedProperty in expectedProperties)
            {
                PropertyInfo actualPropertyInfo = actualType.GetProperty(expectedProperty.Name);
                if (actualPropertyInfo != null)
                {
                    object expectedValue = actualPropertyInfo.GetValue(expected, null);
                    object actualValue = actualPropertyInfo.GetValue(actual, null);
                    Assert.AreEqual(expectedValue, actualValue);
                }
            }
        }
    }
}
