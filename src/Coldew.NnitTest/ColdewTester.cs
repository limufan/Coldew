﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Coldew.Core.Organization;
using Coldew.Core;
using Coldew.Api;
using Coldew.Core.Permission;
using Coldew.Core.Search;
using Newtonsoft.Json.Linq;

namespace Coldew.NnitTest
{
    public class ColdewTester : UnitTestBase
    {
        public ColdewTester()
        {
            
        }


        [Test]
        public void ColdewObjectTest()
        {
            
        }

        [Test]
        public void PermissionTest()
        {
            ColdewObject cobject = this.ColdewManager.ObjectManager.Create(new ColdewObjectCreateInfo("testObject", "testObject", ColdewObjectType.Standard, true, "名称"));
            Field diquField = cobject.CreateDropdownField(new DropdownFieldCreateInfo("diqu", "地区", new List<string> { "天河区", "番禺区" }));
            Field salesUsersField = cobject.CreateUserField(new UserFieldCreateInfo("userField", "业务员"));

            JObject dictionary = new JObject();
            dictionary.Add(cobject.NameField.Code, "name1");
            dictionary.Add(diquField.Code, "天河区");
            dictionary.Add(salesUsersField.Code, "user5");
            Metadata metadata = cobject.MetadataManager.Create(this.User1, dictionary);

            //enetity permission
            Assert.IsFalse(metadata.CanPreview(this.User2));
            cobject.MetadataPermission.EntityManager.Create(metadata.ID, new MetadataOrgMember(this.User2), MetadataPermissionValue.View);
            Assert.IsTrue(metadata.CanPreview(this.User2));

            //strategy permission
            Assert.IsFalse(metadata.CanPreview(this.User4));
            Assert.IsFalse(metadata.CanPreview(this.User5));
            cobject.MetadataPermission.StrategyManager.Create(new MetadataOrgMember(this.User4), MetadataPermissionValue.View, "{diqu: '天河区'}");
            cobject.MetadataPermission.StrategyManager.Create(new MetadataFieldMember(salesUsersField), MetadataPermissionValue.View, null);
            Assert.IsTrue(metadata.CanPreview(this.User4));
            Assert.IsTrue(metadata.CanPreview(this.User5));

            //object permission
            Assert.IsFalse(cobject.ObjectPermission.HasValue(this.User1, ObjectPermissionValue.View));
            cobject.ObjectPermission.Create(this.User1, ObjectPermissionValue.View);
            Assert.IsTrue(cobject.ObjectPermission.HasValue(this.User1, ObjectPermissionValue.View));

            //field permission
            Assert.IsTrue(cobject.FieldPermission.HasValue(this.User1, FieldPermissionValue.View, salesUsersField));
            Assert.IsTrue(cobject.FieldPermission.HasValue(this.Admin, FieldPermissionValue.View, salesUsersField));
            cobject.FieldPermission.Create(salesUsersField.Code, this.Admin, FieldPermissionValue.All);
            Assert.IsFalse(cobject.FieldPermission.HasValue(this.User1, FieldPermissionValue.View, salesUsersField));
            Assert.IsTrue(cobject.FieldPermission.HasValue(this.Admin, FieldPermissionValue.View, salesUsersField));
        }
    }
}
