using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Coldew.Core
{
    public class ClassPropertyHelper
    {
        public static void ChangeProperty(object changeInfo, object changeObject)
        {
            Type changeInfoType = changeInfo.GetType();
            Type changeObjectType = changeObject.GetType();
            PropertyInfo[] changeInfoProperties = changeInfoType.GetProperties();
            foreach (PropertyInfo changeInfoProperty in changeInfoProperties)
            {
                PropertyInfo changeObjectPropertyInfo = changeObjectType.GetProperty(changeInfoProperty.Name);
                if (changeObjectPropertyInfo != null && changeObjectPropertyInfo.CanWrite)
                {
                    object changeValue = changeObjectPropertyInfo.GetValue(changeInfo, null);
                    changeObjectPropertyInfo.SetValue(changeObject, changeValue, null);
                }
            }
        }
    }
}
