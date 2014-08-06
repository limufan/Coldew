using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core;
using Coldew.Core.UI;
using NUnit.Framework;

namespace Coldew.NnitTest
{
    public class ColdewUITester
    {
        [Test]
        public void ControlTest()
        {
            ColdewManager coldewManager = new ColdewManager();
            ColdewObject cobject = coldewManager.ObjectManager.Create(new ColdewObjectCreateInfo { Code = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() });
            Field nameField = cobject.CreateStringField(new StringFieldCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()) { Required = true });
            cobject.SetNameField(nameField);
            cobject.CreateDateField(new DateFieldCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()) { Required = true, DefaultValueIsToday = true });
            cobject.CreateUserListField(new UserListFieldCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()) { Required = true });
            
        }
    }
}
