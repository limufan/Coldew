using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Coldew.Api;
using Coldew.Core;
using Coldew.Core.UI;
using Coldew.Data;
using Coldew.Data.UI;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Coldew.NnitTest
{
    public class FormManagerTester :UnitTestBase
    {
        [Test]
        public void Test()
        {
            ColdewObject cobject = this.ColdewManager.ObjectManager.Create(new ColdewObjectCreateInfo{ Code = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString()});
            Field nameField = cobject.CreateStringField(new StringFieldCreateInfo("name", "名称") { Required = true });

            List<Control> controls = new List<Control>();
            controls.Add(new Fieldset("set"));
            Row row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(nameField));

            cobject.FormManager.Create(new FormCreateInfo { Code = "form1", Title = "title", Controls = controls });

            ColdewManager coldewManager = new Core.ColdewManager();
            cobject = coldewManager.ObjectManager.GetObjectById(cobject.ID);
            List<Form> forms = cobject.FormManager.GetForms();
            Form form = forms[0];
            controls = form.Children;
            Assert.IsTrue(controls[0] is Fieldset);
            Assert.IsTrue(controls[1] is Row);
            Assert.IsTrue(((Row)controls[1]).Children[0] is Input);
        }
    }
}
