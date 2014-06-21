using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using Coldew.Api;
using Coldew.Core;
using Coldew.Core.UI;
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
            ColdewObject cobject = this.ColdewManager.ObjectManager.Create(new ColdewObjectCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), ColdewObjectType.Standard, true));
            Field nameField = cobject.CreateStringField(new StringFieldCreateInfo("name", "名称") { Required = true });

            List<Control> controls = new List<Control>();
            controls.Add(new Fieldset("set"));
            Row row = new Row();
            controls.Add(row);
            row.Children.Add(new Input(nameField));

            cobject.FormManager.Create("form1", "title", controls, null);

            FormDataService formDataService = new FormDataService(this.ColdewManager.ObjectManager, cobject);
            List<Form> forms = formDataService.Load();
            Form form = forms[0];
            controls = form.Controls;
            Assert.IsTrue(controls[0] is Fieldset);
            Assert.IsTrue(controls[1] is Row);
            Assert.IsTrue(((Row)controls[1]).Children[0] is Input);
        }
    }
}
