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
using Coldew.Core.DataManager;
using Coldew.Core;
using Coldew.Core.DataProviders;
using Coldew.Core.UI;

namespace Coldew.NnitTest
{
    [TestFixture]
    public class DataManagerTester 
    {
        ColdewManager _coldewManager;
        ColdewDataManager _coldewDataManager;
        OrganizationDataManager _orgDataManager;
        OrganizationManagement _orgManager;
        public DataManagerTester()
        {
            this.CreateManager();
        }

        private void CreateManager()
        {
            this._coldewManager = new ColdewManager();
            this._orgDataManager = new OrganizationDataManager(this._coldewManager);
            this._coldewDataManager = new ColdewDataManager(this._coldewManager);
            this._orgManager = this._coldewManager.OrgManager;
        }

        #region org data manager test

        [Test]
        public void UserDataManagerTest()
        {
            User user = this.CreateUserTest();
            this.ChangeUserTest(user);
            this.DeleteUserTest(user);

            //reload test
            user = this.CreateUserTest();
            this.CreateManager();
            user = this._orgManager.UserManager.GetUserById(user.ID);
            Assert.IsNotNull(user);
            this.ChangeUserTest(user);
            this.DeleteUserTest(user);
        }

        private User CreateUserTest()
        {
            Position position = this._orgManager.PositionManager.Create(this._orgManager.System, new PositionCreateInfo { Name = Guid.NewGuid().ToString() });
            UserCreateInfo createInfo = new UserCreateInfo { Account = Guid.NewGuid().ToString(), Password = "123456", Name = Guid.NewGuid().ToString(), MainPositionId = position.ID };
            User user = this._orgManager.UserManager.Create(this._orgManager.System, createInfo);
            this.AssertUser(createInfo, user);

            //load form db
            List<User> users = this._orgDataManager.UserDataManager.DataProvider.Select();
            User u1_db = users.Find(x => x.ID == user.ID);
            this.AssertUser(createInfo, u1_db);

            return user;
        }

        private void ChangeUserTest(User user)
        {
            //change test
            UserChangeInfo changeInfo = new UserChangeInfo(user.MapUserInfo());
            changeInfo.Name = Guid.NewGuid().ToString();
            user.Change(this._orgManager.System, changeInfo);
            user = this._orgManager.UserManager.GetUserById(user.ID);
            this.AssertUser(changeInfo, user);

            //load form db
            List<User> users = this._orgDataManager.UserDataManager.DataProvider.Select();
            User u1_db = users.Find(x => x.ID == user.ID);
            this.AssertUser(changeInfo, u1_db);
        }

        private void DeleteUserTest(User user)
        {
            //delete test
            string u1Id = user.ID;
            this._orgManager.UserManager.Delete(this._orgManager.System, u1Id);
            user = this._orgManager.UserManager.GetUserById(u1Id);
            Assert.IsNull(user);
            //load form db
            List<User> users = this._orgDataManager.UserDataManager.DataProvider.Select();
            User u1_db = users.Find(x => x.ID == user.ID);
            Assert.IsNull(u1_db);
        }

        private void AssertUser(UserCreateInfo createInfo, User user)
        {
            Assert.AreEqual(createInfo.Account, user.Account);
            Assert.AreEqual(createInfo.Name, user.Name);
            Assert.AreEqual(createInfo.MainPositionId, user.MainPosition.ID);
            Assert.AreEqual(createInfo.Password, user.Password);
        }

        private void AssertUser(UserChangeInfo changeInfo, User user)
        {
            Assert.AreEqual(changeInfo.Name, user.Name);
        }

        [Test]
        public void PositionDataManagerTest()
        {
            Position position = this.CreatePositionTest();
            this.ChangePositionTest(position);
            this.DeletePositionTest(position);

            //reload test
            position = this.CreatePositionTest();
            this.CreateManager();
            position = this._orgManager.PositionManager.GetPositionById(position.ID);
            Assert.IsNotNull(position);
            this.ChangePositionTest(position);
            this.DeletePositionTest(position);
        }

        private Position CreatePositionTest()
        {
            //create test
            PositionCreateInfo p1CreateInfo = new PositionCreateInfo { Name = Guid.NewGuid().ToString() };
            Position position = this._orgManager.PositionManager.Create(this._orgManager.System, p1CreateInfo);
            this.AssertPosition(p1CreateInfo, position);
            //load form db
            List<Position> positions = this._orgDataManager.PositionDataManager.DataProvider.Select();
            Position p1_db = positions.Find(x => x.ID == position.ID);
            this.AssertPosition(p1CreateInfo, p1_db);
            return position;
        }

        private void ChangePositionTest(Position position)
        {
            //change test
            PositionChangeInfo changeInfo = new PositionChangeInfo(position.MapPositionInfo());
            changeInfo.Name = Guid.NewGuid().ToString();
            position.Change(this._orgManager.System, changeInfo);
            this.AssertPosition(changeInfo, position);
            //add user test
            Position p_temp = this._orgManager.PositionManager.Create(this._orgManager.System, new PositionCreateInfo { Name = Guid.NewGuid().ToString() });
            User user_temp = this._orgManager.UserManager.Create(this._orgManager.System, new UserCreateInfo { Account = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString(), MainPositionId = p_temp.ID, Password = "123456" });
            position.AddUser(this._orgManager.System, user_temp);
            Assert.IsTrue(position.Contains(user_temp));
            //load form db
            List<Position> positions = this._orgDataManager.PositionDataManager.DataProvider.Select();
            this._orgDataManager.PositionDataManager.DataProvider.LoadUsers(positions);
            Position p_db = positions.Find(x => x.ID == position.ID);
            this.AssertPosition(changeInfo, p_db);
            Assert.IsTrue(p_db.Contains(user_temp));
            //remove user test
            position.RemoveUser(this._orgManager.System, user_temp);
            Assert.IsFalse(position.Contains(user_temp));
            //load form db
            positions = this._orgDataManager.PositionDataManager.DataProvider.Select();
            this._orgDataManager.PositionDataManager.DataProvider.LoadUsers(positions);
            p_db = positions.Find(x => x.ID == position.ID);
            Assert.IsFalse(position.Contains(user_temp));
        }

        private void DeletePositionTest(Position position)
        {
            //delete test
            string p1Id = position.ID;
            this._orgManager.PositionManager.Delete(this._orgManager.System, p1Id);
            position = this._orgManager.PositionManager.GetPositionById(p1Id);
            Assert.IsNull(position);
            //load form db
            List<Position> positions = this._orgDataManager.PositionDataManager.DataProvider.Select();
            Position p1_db = positions.Find(x => x.ID == p1Id);
            Assert.IsNull(p1_db);
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
        public void DepartmentDataManagerTest()
        {
            Department department = this.CreateDepartmentTest();
            this.ChangeDepartmentTest(department);
            this.DeleteDepartmentTest(department);

            //reload test
            department = this.CreateDepartmentTest();
            this.CreateManager();
            department = this._orgManager.DepartmentManager.GetDepartmentById(department.ID);
            Assert.IsNotNull(department);
            this.ChangeDepartmentTest(department);
            this.DeleteDepartmentTest(department);
        }

        private Department CreateDepartmentTest()
        {
            //create test
            DepartmentCreateInfo createInfo = new DepartmentCreateInfo { Name = Guid.NewGuid().ToString(), ManagerPositionInfo = new PositionCreateInfo { Name = Guid.NewGuid().ToString() } };
            Department department = this._orgManager.DepartmentManager.Create(this._orgManager.System, createInfo);
            this.AssertDepartment(createInfo, department);
            //load form db
            List<Department> departments = this._orgDataManager.DepartmentDataManager.DataProvider.Select();
            Department d1_db = departments.Find(x => x.ID == department.ID);
            this.AssertDepartment(createInfo, d1_db);
            return department;
        }

        private void ChangeDepartmentTest(Department department)
        {
            //change test
            DepartmentChangeInfo changeInfo = new DepartmentChangeInfo(department.MapDepartmentInfo());
            changeInfo.Name = Guid.NewGuid().ToString();
            department.Change(this._orgManager.System, changeInfo);
            this.AssertDepartment(changeInfo, department);
            //load form db
            List<Department> departments = this._orgDataManager.DepartmentDataManager.DataProvider.Select();
            Department d1_db = departments.Find(x => x.ID == department.ID);
            this.AssertDepartment(changeInfo, d1_db);
        }

        private void DeleteDepartmentTest(Department department)
        {
            //delete test
            string d1Id = department.ID;
            this._orgManager.DepartmentManager.Delete(this._orgManager.System, d1Id);
            department = this._orgManager.DepartmentManager.GetDepartmentById(d1Id);
            Assert.IsNull(department);
            //load form db
            List<Department> departments = this._orgDataManager.DepartmentDataManager.DataProvider.Select();
            Department d1_db = departments.Find(x => x.ID == d1Id);
            Assert.IsNull(d1_db);
        }

        private void AssertDepartment(DepartmentCreateInfo createInfo, Department department)
        {
            Assert.AreEqual(createInfo.Name, department.Name);
            this.AssertPosition(createInfo.ManagerPositionInfo, department.ManagerPosition);
        }

        private void AssertDepartment(DepartmentChangeInfo changeInfo, Department department)
        {
            Assert.AreEqual(changeInfo.Name, department.Name);
        }

        [Test]
        public void GroupDataManagerTest()
        {
            Group group = this.CreateGroupTest();
            this.ChangeGroupTest(group);
            this.DeleteGroupTest(group);

            //reload test
            group = this.CreateGroupTest();
            this.CreateManager();
            group = this._orgManager.GroupManager.GetGroupById(group.ID);
            Assert.IsNotNull(group);
            this.ChangeGroupTest(group);
            this.DeleteGroupTest(group);
        }

        private Group CreateGroupTest()
        {
            //create test
            GroupCreateInfo createInfo = new GroupCreateInfo { Name = Guid.NewGuid().ToString() };
            Group group = this._orgManager.GroupManager.Create(this._orgManager.System, createInfo);
            this.AssertGroup(createInfo, group);
            //load form db
            List<Group> groups = this._orgDataManager.GroupDataManager.DataProvider.Select();
            Group g1_db = groups.Find(x => x.ID == group.ID);
            this.AssertGroup(createInfo, g1_db);
            return group;
        }

        private void ChangeGroupTest(Group group)
        {
            //change test
            GroupChangeInfo changeInfo = new GroupChangeInfo(group.MapGroupInfo());
            changeInfo.Name = Guid.NewGuid().ToString();
            group.Change(this._orgManager.System, changeInfo);
            this.AssertGroup(changeInfo, group);
            //add member
            Position position = this._orgManager.PositionManager.Create(this._orgManager.System, new PositionCreateInfo{ Name = Guid.NewGuid().ToString() });
            group.AddMember(this._orgManager.System, position);
            Assert.IsTrue(group.Contains(position));
            //load form db
            List<Group> groups = this._orgDataManager.GroupDataManager.DataProvider.Select();
            this._orgDataManager.GroupDataManager.DataProvider.LoadMembers(groups);
            Group g1_db = groups.Find(x => x.ID == group.ID);
            this.AssertGroup(changeInfo, g1_db);
            Assert.IsTrue(g1_db.Contains(position));
        }

        private void DeleteGroupTest(Group group)
        {
            //delete test
            string g1Id = group.ID;
            this._orgManager.GroupManager.Delete(this._orgManager.System, g1Id);
            group = this._orgManager.GroupManager.GetGroupById(g1Id);
            Assert.IsNull(group);
            //load form db
            List<Group> groups = this._orgDataManager.GroupDataManager.DataProvider.Select();
            Group g1_db = groups.Find(x => x.ID == g1Id);
            Assert.IsNull(g1_db);
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
        public void FormDataManagerTest()
        {
            ColdewObjectCreateInfo objectCreateInfo = new ColdewObjectCreateInfo { Code = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(objectCreateInfo);
            cobject.CreateStringField(new StringFieldCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            Form form = this.CreateFormTest(cobject);

            this.CreateManager();
            cobject = this._coldewManager.ObjectManager.GetObjectById(cobject.ID);
            form = cobject.FormManager.GetFormById(form.ID);
            Assert.IsNotNull(form);
        }

        private Form CreateFormTest(ColdewObject cobject)
        {
            //create test
            List<Control> controls = cobject.GetFields().Select(x => new Input(x) as Control).ToList();
            FormCreateInfo createInfo = new FormCreateInfo { Code = Guid.NewGuid().ToString(), Title = Guid.NewGuid().ToString(), Controls = controls };
            Form form = cobject.FormManager.Create(createInfo);
            this.AssertForm(createInfo, form);
            Assert.IsTrue(form.Contains(controls[0]));
            //load form db
            FormDataProvider dataProvider = new FormDataProvider(cobject);
            List<Form> forms = dataProvider.Select();
            dataProvider.LoadControls(forms);
            Form form_db = forms.Find(x => x.ID == form.ID);
            this.AssertForm(createInfo, form_db);
            Assert.IsTrue(form_db.Children[0] is Input);
            return form;
        }

        private void AssertForm(FormCreateInfo createInfo, Form form)
        {
            Assert.AreEqual(createInfo.Code, form.Code);
            Assert.AreEqual(createInfo.Title, form.Title);
            Assert.AreEqual(createInfo.Controls.Count, form.Children.Count);
        }

        [Test]
        public void GridViewDataManagerTest()
        {
            ColdewObjectCreateInfo objectCreateInfo = new ColdewObjectCreateInfo { Code = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(objectCreateInfo);
            cobject.CreateStringField(new StringFieldCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            GridView gridView = this.CreateGridViewTest(cobject);
            this.ChangeGridViewTest(gridView);
            this.DeleteGridViewTest(gridView);

            //reload test
            gridView = this.CreateGridViewTest(cobject);
            this.CreateManager();
            gridView = cobject.GridViewManager.GetGridView(gridView.ID);
            Assert.IsNotNull(gridView);
            this.ChangeGridViewTest(gridView);
            this.DeleteGridViewTest(gridView);
        }

        private GridView CreateGridViewTest(ColdewObject cobject)
        {
            //create test
            List<Field> fields = cobject.GetFields();
            List<GridViewColumn> columns = fields.Select(x => new GridViewColumn(x)).ToList();
            GridViewCreateInfo createInfo = new GridViewCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), columns, this._orgManager.System, fields[0]);
            GridView gridView = cobject.GridViewManager.Create(createInfo);
            this.AssertGridView(createInfo, gridView);
            //load form db
            GridViewDataProvider dataProvider = new GridViewDataProvider(cobject);
            List<GridView> gridViews = dataProvider.Select();
            GridView gridView_db = gridViews.Find(x => x.ID == gridView.ID);
            this.AssertGridView(createInfo, gridView_db);
            return gridView;
        }

        private void ChangeGridViewTest(GridView gridView)
        {
            //change test
            GridViewChangeInfo changeInfo = new GridViewChangeInfo(gridView);
            changeInfo.Name = Guid.NewGuid().ToString();
            gridView.Change(changeInfo);
            this.AssertGridView(changeInfo, gridView);
            //load form db
            GridViewDataProvider dataProvider = new GridViewDataProvider(gridView.ColdewObject);
            List<GridView> gridViews = dataProvider.Select();
            GridView gridView_db = gridViews.Find(x => x.ID == gridView.ID);
            this.AssertGridView(changeInfo, gridView_db);
        }

        private void DeleteGridViewTest(GridView gridView)
        {
            ColdewObject cobject = gridView.ColdewObject;
            //delete test
            string gridViewId = gridView.ID;
            gridView.Delete();
            gridView = gridView.ColdewObject.GridViewManager.GetGridView(gridViewId);
            Assert.IsNull(gridView);
            //load form db
            GridViewDataProvider dataProvider = new GridViewDataProvider(cobject);
            List<GridView> gridViews = dataProvider.Select();
            GridView gridView_db = gridViews.Find(x => x.ID == gridViewId);
            Assert.IsNull(gridView_db);
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
        public void MetadataDataManagerTest()
        {
            ColdewObjectCreateInfo objectCreateInfo = new ColdewObjectCreateInfo { Code = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(objectCreateInfo);
            cobject.CreateStringField(new StringFieldCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            Metadata metadata = this.CreateMetadataTest(cobject);
            this.ChangeMetadataTest(metadata);
            this.DeleteMetadataTest(metadata);

            //reload test
            metadata = this.CreateMetadataTest(cobject);
            this.CreateManager();
            metadata = cobject.MetadataManager.GetById(metadata.ID);
            Assert.IsNotNull(metadata);
            this.ChangeMetadataTest(metadata);
            this.DeleteMetadataTest(metadata);
        }

        private Metadata CreateMetadataTest(ColdewObject cobject)
        {
            //create test
            List<Field> fields = cobject.GetFields();
            JObject jobject = new JObject();
            jobject.Add(fields[0].Code, Guid.NewGuid().ToString());
            MetadataValueDictionary value = new MetadataValueDictionary(cobject, jobject);
            MetadataCreateInfo createInfo = new MetadataCreateInfo { Creator = this._orgManager.System, Value = value };
            Metadata metadata = cobject.MetadataManager.Create(createInfo);
            this.AssertMetadata(createInfo, metadata);
            //load form db
            MetadataDataProvider dataProvider = new MetadataDataProvider(cobject);
            List<Metadata> metadatas = dataProvider.Select();
            Metadata metadata_db = metadatas.Find(x => x.ID == metadata.ID);
            this.AssertMetadata(createInfo, metadata_db);
            return metadata;
        }

        private void ChangeMetadataTest(Metadata metadata)
        {
            //change test
            List<Field> fields = metadata.ColdewObject.GetFields();
            JObject jobject = new JObject();
            jobject.Add(fields[0].Code, Guid.NewGuid().ToString());
            MetadataValueDictionary value = new MetadataValueDictionary(metadata.ColdewObject, jobject);
            MetadataChangeInfo changeInfo = new MetadataChangeInfo { Operator = this._orgManager.System, Value = value };
            metadata.SetValue(changeInfo);
            this.AssertMetadata(changeInfo, metadata);
            //load form db
            MetadataDataProvider dataProvider = new MetadataDataProvider(metadata.ColdewObject);
            List<Metadata> metadatas = dataProvider.Select();
            Metadata metadata_db = metadatas.Find(x => x.ID == metadata.ID);
            this.AssertMetadata(changeInfo, metadata_db);
        }

        private void DeleteMetadataTest(Metadata metadata)
        {
            ColdewObject cobject = metadata.ColdewObject;
            //delete test
            string metadataId = metadata.ID;
            metadata.Delete(this._orgManager.System);
            metadata = metadata.ColdewObject.MetadataManager.GetById(metadataId);
            Assert.IsNull(metadata);
            //load form db
            MetadataDataProvider dataProvider = new MetadataDataProvider(cobject);
            List<Metadata> metadatas = dataProvider.Select();
            Metadata metadata_db = metadatas.Find(x => x.ID == metadataId);
            Assert.IsNull(metadata_db);
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
        public void ObjectDataManagerTest()
        {
            ColdewObject cobject = this.CreateObjectTest();
            this.ChangeObjectTest(cobject);

            //reload test
            cobject = this.CreateObjectTest();
            this.CreateManager();
            cobject = this._coldewManager.ObjectManager.GetObjectById(cobject.ID);
            Assert.IsNotNull(cobject);
            this.ChangeObjectTest(cobject);
        }

        private ColdewObject CreateObjectTest()
        {
            //create test
            ColdewObjectCreateInfo createInfo = new ColdewObjectCreateInfo { Code = Guid.NewGuid().ToString(), Name = Guid.NewGuid().ToString() };
            ColdewObject cobject = this._coldewManager.ObjectManager.Create(createInfo);
            this.AssertObject(createInfo, cobject);

            StringFieldCreateInfo stringFieldCreateInfo = new StringFieldCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Field stringField = cobject.CreateStringField(stringFieldCreateInfo);
            this.AssertField(stringFieldCreateInfo, stringField);
            //load form db
            ObjectDataProvider dataProvider = new ObjectDataProvider(this._coldewManager.ObjectManager);
            List<ColdewObject> objects = dataProvider.Select();
            ColdewObject object_db = objects.Find(x => x.ID == cobject.ID);
            this.AssertObject(createInfo, object_db);

            stringField = object_db.GetFieldById(stringField.ID);
            this.AssertField(stringFieldCreateInfo, stringField);
            return cobject;
        }

        private void ChangeObjectTest(ColdewObject cobject)
        {
            //change test
            StringFieldCreateInfo stringFieldCreateInfo = new StringFieldCreateInfo(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
            Field nameField = cobject.CreateStringField(stringFieldCreateInfo);
            cobject.SetNameField(nameField);
            Assert.AreEqual(cobject.NameField, nameField);
            //load form db
            ObjectDataProvider dataProvider = new ObjectDataProvider(this._coldewManager.ObjectManager);
            List<ColdewObject> objects = dataProvider.Select();
            ColdewObject object_db = objects.Find(x => x.ID == cobject.ID);
            nameField = object_db.GetFieldById(nameField.ID);
            Assert.AreEqual(object_db.NameField, nameField);
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
