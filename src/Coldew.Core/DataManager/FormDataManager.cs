using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Core.DataProviders;
using Coldew.Core.UI;

namespace Coldew.Core.DataManager
{
    public class FormDataManager
    {
        internal FormDataProvider DataProvider { private set; get; }
        ColdewObject _cobject;
        public FormDataManager(ColdewObject cobject)
        {
            this._cobject = cobject;
            this.DataProvider = new FormDataProvider(cobject);
            cobject.FormManager.Created += FormManager_Created;
            this.Load();
        }

        void FormManager_Created(FormManager formManager, Form form)
        {
            this.DataProvider.Insert(form);
        }

        private void BindEvent(Form form)
        {
            form.Modified += form_Modified;
        }

        void form_Modified(Form form)
        {
            this.DataProvider.Update(form);
        }

        void Load()
        {
            List<Form> forms = this.DataProvider.Select();
            this._cobject.FormManager.AddForms(forms);
            foreach (Form form in forms)
            {
                this.BindEvent(form);
            }
        }

        public void LoadFormControls()
        {
            this.DataProvider.LoadControls(this._cobject.FormManager.GetForms());
        }
    }
}
