using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using System.Threading;
using Coldew.Core.Organization;
using Coldew.Api.Organization;
using Coldew.Api.Organization.Exceptions;
using Coldew.Core;
using Coldew.Core.UI;
using Coldew.Data;

namespace Coldew.NnitTest
{
    [TestFixture]
    public class DataProviderTester 
    {
        ColdewManager _coldewManager;
        ColdewDataManager _coldewDataManager;
        OrganizationManagement _orgManager;
        public DataProviderTester()
        {
            this.CreateManager();
        }

        private void CreateManager()
        {
            this._coldewManager = new ColdewManager();
            this._coldewDataManager = new ColdewDataManager(this._coldewManager);
            this._orgManager = this._coldewManager.OrgManager;
        }

        #region org data manager test

        [Test]
        public void UserDataProviderTest()
        {
            // insert test
            Position position = this._orgManager.PositionManager.Create(this._orgManager.System, new PositionCreateInfo { Name = Guid.NewGuid().ToString() });
            this._coldewDataManager.PositionDataProvider.Insert(position);
            UserCreateInfo createInfo = new UserCreateInfo { Account = Guid.NewGuid().ToString(), Password = "123456", Name = Guid.NewGuid().ToString(), MainPositionId = position.ID };
            User user = this._orgManager.UserManager.Create(this._orgManager.System, createInfo);
            this.AssertUser(createInfo, user);
            this._coldewDataManager.UserDataProvider.Insert(user);
            //reload
            this.CreateManager();
            user = this._orgManager.UserManager.GetUserById(user.ID);
            this.AssertUser(createInfo, user);

            // update test
            UserChangeInfo changeInfo = new UserChangeInfo(user.MapUserInfo());
            changeInfo.Name = Guid.NewGuid().ToString();
            user.Change(this._orgManager.System, changeInfo);
            this._coldewDataManager.UserDataProvider.Update(user);
            //reload
            this.CreateManager();
            user = this._orgManager.UserManager.GetUserById(user.ID);
            this.AssertUser(changeInfo, user);

            string u1Id = user.ID;
            this._orgManager.UserManager.Delete(this._orgManager.System, u1Id);
            this._coldewDataManager.UserDataProvider.Delete(user);
            user = this._orgManager.UserManager.GetUserById(u1Id);
            Assert.IsNull(user);
            //reload
            this.CreateManager();
            user = this._orgManager.UserManager.GetUserById(u1Id);
            Assert.IsNull(user);
        }

        private void AssertUser(UserCreateInfo createInfo, User user)
        {
            Assert.AreEqual(createInfo.Account, user.Account);
            Assert.AreEqual(createInfo.Name, user.Name);
            Assert.AreEqual(createInfo.MainPositionId, user.MainPosition.ID);
            Assert.AreEqual(Cryptography.MD5Encode(createInfo.Password), user.Password);
        }

        private void AssertUser(UserChangeInfo changeInfo, User user)
        {
            Assert.AreEqual(changeInfo.Name, user.Name);
        }

        [Test]
        public void PositionDataProviderTest()
        {
            //insert test
            PositionCreateInfo createInfo = new PositionCreateInfo { Name = Guid.NewGuid().ToString() };
            Position position = this._orgManager.PositionManager.Create(this._orgManager.System, createInfo);
            this._coldewDataManager.PositionDataProvider.Insert(position);
            this.AssertPosition(createInfo, position);
            //reload
            this.CreateManager();
            position = this._orgManager.PositionManager.GetPositionById(position.ID);
            this.AssertPosition(createInfo, position);

            //update test
            PositionChangeInfo changeInfo = new PositionChangeInfo(position.MapPositionInfo());
            changeInfo.Name = Guid.NewGuid().ToString();
            position.Change(this._orgManager.System, changeInfo);
            //add user test
            Position temp_position = this._orgManager.PositionManager.Create(this._orgManager.System, createInfo);
            this._coldewDataManager.PositionDataProvider.Insert(temp_position);
            User user_temp = this._orgManager.UserManager.Create(this._orgManager.System, new UserCreateInfo { Account = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString(), MainPositionId = temp_position.ID, Password = "123456" });
            this._coldewDataManager.UserDataProvider.Insert(user_temp);
            position.AddUser(this._orgManager.System, user_temp);
            this.AssertPosition(changeInfo, position);
            Assert.IsTrue(position.Contains(user_temp));
            this._coldewDataManager.PositionDataProvider.Update(position);
            //reload
            this.CreateManager();
            user_temp = this._orgManager.UserManager.GetUserById(user_temp.ID);
            position = this._orgManager.PositionManager.GetPositionById(position.ID);
            this.AssertPosition(changeInfo, position);
            Assert.IsTrue(position.Contains(user_temp));
            //remove user test
            position.RemoveUser(this._orgManager.System, user_temp);
            Assert.IsFalse(position.Contains(user_temp));
            this._coldewDataManager.PositionDataProvider.Update(position);
            //reload
            this.CreateManager();
            user_temp = this._orgManager.UserManager.GetUserById(user_temp.ID);
            position = this._orgManager.PositionManager.GetPositionById(position.ID);
            Assert.IsFalse(position.Contains(user_temp));

            //delete test
            string p1Id = position.ID;
            this._orgManager.PositionManager.Delete(this._orgManager.System, p1Id);
            this._coldewDataManager.PositionDataProvider.Delete(position);
            position = this._orgManager.PositionManager.GetPositionById(p1Id);
            Assert.IsNull(position);
            this.CreateManager();
            position = this._orgManager.PositionManager.GetPositionById(p1Id);
            Assert.IsNull(position);
        }

        private void AssertPosition(PositionCreateInfo createInfo, Position position)
        {
            if (createInfo.ParentId == null)
            {
                createInfo.ParentId = "";
            }
            Assert.AreEqual(createInfo.Name, position.Name);
            Assert.AreEqual(createInfo.ParentId, position.Parent == null ? "" : position.Parent.ID );
        }

        private void AssertPosition(PositionChangeInfo changeInfo, Position position)
        {
            if (changeInfo.ParentId == null)
            {
                changeInfo.ParentId = "";
            }
            Assert.AreEqual(changeInfo.Name, position.Name);
            Assert.AreEqual(changeInfo.ParentId, position.Parent == null ? "" : position.Parent.ID);
        }

        [Test]
        public void DepartmentDataProviderTest()
        {
            //insert test
            Position managerPosition = this._orgManager.PositionManager.Create(this._orgManager.System, new PositionCreateInfo { Name = Guid.NewGuid().ToString() });
            this._coldewDataManager.PositionDataProvider.Insert(managerPosition);
            DepartmentCreateInfo createInfo = new DepartmentCreateInfo { Name = Guid.NewGuid().ToString(), ManagerPosition = managerPosition };
            Department department = this._orgManager.DepartmentManager.Create(this._orgManager.System, createInfo);
            this.AssertDepartment(createInfo, department);
            this._coldewDataManager.DepartmentDataProvider.Insert(department);
            //reload
            this.CreateManager();
            department = this._orgManager.DepartmentManager.GetDepartmentById(department.ID);
            this.AssertDepartment(createInfo, department);

            //update test
            DepartmentChangeInfo changeInfo = new DepartmentChangeInfo(department);
            changeInfo.Name = Guid.NewGuid().ToString();
            department.Change(this._orgManager.System, changeInfo);
            this.AssertDepartment(changeInfo, department);
            this._coldewDataManager.DepartmentDataProvider.Update(department);
            //reload
            this.CreateManager();
            department = this._orgManager.DepartmentManager.GetDepartmentById(department.ID);
            this.AssertDepartment(changeInfo, department);

            //delete test
            string d1Id = department.ID;
            this._orgManager.DepartmentManager.Delete(this._orgManager.System, d1Id);
            this._coldewDataManager.DepartmentDataProvider.Delete(department);
            department = this._orgManager.DepartmentManager.GetDepartmentById(d1Id);
            Assert.IsNull(department);
            //reload
            this.CreateManager();
            department = this._orgManager.DepartmentManager.GetDepartmentById(d1Id);
            Assert.IsNull(department);
        }

        private void AssertDepartment(DepartmentCreateInfo createInfo, Department department)
        {
            Assert.AreEqual(createInfo.Name, department.Name);
        }

        private void AssertDepartment(DepartmentChangeInfo changeInfo, Department department)
        {
            Assert.AreEqual(changeInfo.Name, department.Name);
        }

        [Test]
        public void GroupDataProviderTest()
        {
            //insert test
            GroupCreateInfo createInfo = new GroupCreateInfo { Name = Guid.NewGuid().ToString() };
            Group group = this._orgManager.GroupManager.Create(this._orgManager.System, createInfo);
            this.AssertGroup(createInfo, group);
            this._coldewDataManager.GroupDataProvider.Insert(group);
            //reload
            this.CreateManager();
            group = this._orgManager.GroupManager.GetGroupById(group.ID);
            this.AssertGroup(createInfo, group);

            //update test
            GroupChangeInfo changeInfo = new GroupChangeInfo(group.MapGroupInfo());
            changeInfo.Name = Guid.NewGuid().ToString();
            group.Change(this._orgManager.System, changeInfo);
            this.AssertGroup(changeInfo, group);
            //add member
            Position position = this._orgManager.PositionManager.Create(this._orgManager.System, new PositionCreateInfo { Name = Guid.NewGuid().ToString() });
            this._coldewDataManager.PositionDataProvider.Insert(position);
            group.AddMember(this._orgManager.System, position);
            Assert.IsTrue(group.Contains(position));
            this._coldewDataManager.GroupDataProvider.Update(group);
            //reload
            this.CreateManager();
            position = this._orgManager.PositionManager.GetPositionById(position.ID);
            group = this._orgManager.GroupManager.GetGroupById(group.ID);
            this.AssertGroup(changeInfo, group);
            Assert.IsTrue(group.Contains(position));

            //delete test
            string g1Id = group.ID;
            this._orgManager.GroupManager.Delete(this._orgManager.System, g1Id);
            this._coldewDataManager.GroupDataProvider.Delete(group);
            group = this._orgManager.GroupManager.GetGroupById(g1Id);
            Assert.IsNull(group);
            //load form db
            this.CreateManager();
            group = this._orgManager.GroupManager.GetGroupById(g1Id);
            Assert.IsNull(group);
        }

        private void AssertGroup(GroupCreateInfo createInfo, Group group)
        {
            Assert.AreEqual(createInfo.Name, group.Name);
        }

        private void AssertGroup(GroupChangeInfo changeInfo, Group group)
        {
            Assert.AreEqual(changeInfo.Name, group.Name);
        }

        #endregion

        [Test]
        public void FormDataProviderTest()
        {
            ColdewObjectCreateInfo objectCreateInfo = new ColdewObjectCreateInfo { Code = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(objectCreateInfo);
            cobject.CreateStringField(new StringFieldCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            this._coldewDataManager.ObjectDataProvider.Insert(cobject);

            //insert test
            List<Control> controls = cobject.GetFields().Select(x => new Input(x) as Control).ToList();
            FormCreateInfo createInfo = new FormCreateInfo { Code = Guid.NewGuid().ToString(), Title = Guid.NewGuid().ToString(), Controls = controls };
            Form form = cobject.FormManager.Create(createInfo);
            this.AssertForm(createInfo, form);
            this._coldewDataManager.FormDataProvider.Insert(form);
            //reload
            this.CreateManager();
            form = this._coldewManager.ObjectManager.GetFormById(form.ID);
            this.AssertForm(createInfo, form);
        }

        private void AssertForm(FormCreateInfo createInfo, Form form)
        {
            Assert.AreEqual(createInfo.Code, form.Code);
            Assert.AreEqual(createInfo.Title, form.Title);
            Assert.AreEqual(createInfo.Controls.Count, form.Children.Count);
        }

        [Test]
        public void GridViewDataProviderTest()
        {
            ColdewObjectCreateInfo objectCreateInfo = new ColdewObjectCreateInfo { Code = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(objectCreateInfo);
            cobject.CreateStringField(new StringFieldCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            this._coldewDataManager.ObjectDataProvider.Insert(cobject);

            //create test
            List<Field> fields = cobject.GetFields();
            List<GridColumn> columns = fields.Select(x => new GridColumn(x)).ToList();
            GridViewCreateInfo createInfo = new GridViewCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), columns, this._orgManager.System, fields[0]);
            GridView gridView = cobject.GridViewManager.Create(createInfo);
            this.AssertGridView(createInfo, gridView);
            this._coldewDataManager.GridViewDataProvider.Insert(gridView);
            //reload
            this.CreateManager();
            cobject = this._coldewManager.ObjectManager.GetObjectById(cobject.ID);
            gridView = cobject.GridViewManager.GetGridView(gridView.ID);
            this.AssertGridView(createInfo, gridView);

            //change test
            GridViewChangeInfo changeInfo = new GridViewChangeInfo(gridView);
            changeInfo.Name = Guid.NewGuid().ToString();
            gridView.Change(changeInfo);
            this.AssertGridView(changeInfo, gridView);
            this._coldewDataManager.GridViewDataProvider.Update(gridView);
            //load form db
            this.CreateManager();
            cobject = this._coldewManager.ObjectManager.GetObjectById(cobject.ID);
            gridView = cobject.GridViewManager.GetGridView(gridView.ID);
            this.AssertGridView(changeInfo, gridView);

            //delete test
            string gridViewId = gridView.ID;
            gridView.Delete();
            this._coldewDataManager.GridViewDataProvider.Delete(gridView);
            gridView = gridView.ColdewObject.GridViewManager.GetGridView(gridViewId);
            Assert.IsNull(gridView);
            //reload
            this.CreateManager();
            cobject = this._coldewManager.ObjectManager.GetObjectById(cobject.ID);
            gridView = cobject.GridViewManager.GetGridView(gridViewId);
            Assert.IsNull(gridView);
        }

        private void AssertGridView(GridViewCreateInfo createInfo, GridView gridView)
        {
            Assert.AreEqual(createInfo.Name, gridView.Name);
        }

        private void AssertGridView(GridViewChangeInfo changeInfo, GridView gridView)
        {
            Assert.AreEqual(changeInfo.Name, gridView.Name);
        }

        [Test]
        public void MetadataDataProviderTest()
        {
            ColdewObjectCreateInfo objectCreateInfo = new ColdewObjectCreateInfo { Code = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(objectCreateInfo);
            Field field = cobject.CreateStringField(new StringFieldCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            this._coldewDataManager.ObjectDataProvider.Insert(cobject);

            //create test
            JObject jobject = new JObject();
            jobject.Add(field.Code, Guid.NewGuid().ToString());
            MetadataValueDictionary value = new MetadataValueDictionary(cobject, jobject);
            MetadataCreateInfo createInfo = new MetadataCreateInfo { Creator = this._orgManager.System, Value = value };
            Metadata metadata = cobject.MetadataManager.Create(createInfo);
            this.AssertMetadata(createInfo, metadata);
            this._coldewDataManager.MetadataDataProvider.Insert(metadata);
            //load form db
            this.CreateManager();
            cobject = this._coldewManager.ObjectManager.GetObjectById(cobject.ID);
            metadata = cobject.MetadataManager.GetById(metadata.ID);
            this.AssertMetadata(createInfo, metadata);

            //change test
            jobject = new JObject();
            jobject.Add(field.Code, Guid.NewGuid().ToString());
            value = new MetadataValueDictionary(metadata.ColdewObject, jobject);
            MetadataChangeInfo changeInfo = new MetadataChangeInfo { Operator = this._orgManager.System, Value = value };
            metadata.SetValue(changeInfo);
            this.AssertMetadata(changeInfo, metadata);
            this._coldewDataManager.MetadataDataProvider.Update(metadata);
            //reload
            this.CreateManager();
            cobject = this._coldewManager.ObjectManager.GetObjectById(cobject.ID);
            metadata = cobject.MetadataManager.GetById(metadata.ID);
            this.AssertMetadata(changeInfo, metadata);

            //delete test
            string metadataId = metadata.ID;
            metadata.Delete(this._orgManager.System);
            this._coldewDataManager.MetadataDataProvider.Delete(metadata);
            metadata = metadata.ColdewObject.MetadataManager.GetById(metadataId);
            Assert.IsNull(metadata);
            //reload
            this.CreateManager();
            cobject = this._coldewManager.ObjectManager.GetObjectById(cobject.ID);
            metadata = cobject.MetadataManager.GetById(metadataId);
            Assert.IsNull(metadata);
        }

        private void AssertMetadata(MetadataCreateInfo createInfo, Metadata metadata)
        {
            foreach (MetadataValue value in createInfo.Value.Values)
            {
                MetadataValue metadataValue = metadata.GetValue(value.Field.Code);
                Assert.AreEqual(value.Value, metadataValue.Value); 
            }
        }

        private void AssertMetadata(MetadataChangeInfo changeInfo, Metadata metadata)
        {
            foreach (MetadataValue value in changeInfo.Value.Values)
            {
                MetadataValue metadataValue = metadata.GetValue(value.Field.Code);
                Assert.AreEqual(value.Value, metadataValue.Value);
            }
        }

        [Test]
        public void ObjectDataProviderTest()
        {
            //create test
            ColdewObjectCreateInfo createInfo = new ColdewObjectCreateInfo { Code = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(createInfo);
            this.AssertObject(createInfo, cobject);
            StringFieldCreateInfo stringFieldCreateInfo = new StringFieldCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Field stringField = cobject.CreateStringField(stringFieldCreateInfo);
            this.AssertField(stringFieldCreateInfo, stringField);
            this._coldewDataManager.ObjectDataProvider.Insert(cobject);
            //load form db
            this.CreateManager();
            cobject = this._coldewManager.ObjectManager.GetObjectById(cobject.ID);
            this.AssertObject(createInfo, cobject);
            stringField = cobject.GetFieldById(stringField.ID);
            this.AssertField(stringFieldCreateInfo, stringField);

            //change test
            StringFieldCreateInfo stringFieldCreateInfo1 = new StringFieldCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Field nameField = cobject.CreateStringField(stringFieldCreateInfo1);
            cobject.SetNameField(nameField);
            Assert.AreEqual(cobject.NameField, nameField);
            //load form db
            this.CreateManager();
            cobject = this._coldewManager.ObjectManager.GetObjectById(cobject.ID);
            nameField = cobject.GetFieldById(nameField.ID);
            Assert.AreEqual(cobject.NameField, nameField);
        }

        private void AssertObject(ColdewObjectCreateInfo createInfo, ColdewObject cobject)
        {
            Assert.AreEqual(createInfo.Code, cobject.Code);
            Assert.AreEqual(createInfo.Name, cobject.Name);
        }

        private void AssertField(FieldCreateInfo createInfo, Field field)
        {
            Assert.AreEqual(createInfo.Code, field.Code);
            Assert.AreEqual(createInfo.Name, field.Name);
        }
    }
}
