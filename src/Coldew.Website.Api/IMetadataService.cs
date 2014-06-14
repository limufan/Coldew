using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Website.Api.Models;

namespace Coldew.Website.Api
{
    public interface IMetadataService
    {
        MetadataInfo GetMetadataById(string userAccount, string objectId, string id);
        
        MetadataInfo GetMetadataByName(string userAccount, string objectId, string name);
        
        List<MetadataInfo> GetRelatedMetadatas(string userAccount, string relatedObjectId, string objectId, string metadataId, string orderBy);

        string GetGridJson(string objectId, string account, int skipCount, int takeCount, string orderBy, out int totalCount);

        string GetGridJson(string objectId, string account, string orderBy);

        string GetGridJson(string objectId, string gridViewId, string account, string orderBy);
        
        string GetGridJsonBySerach(string objectId, string account, string serachExpressionJson, int skipCount, int takeCount, string orderBy, out int totalCount);

        MetadataGridModel GetMetadataGridModel(string objectId, string gridViewId, string account, string serachExpressionJson, int skipCount, int takeCount, string orderBy);

        string GetGridJsonBySerach(string objectId, string account, string serachExpressionJson, string orderBy);

        string GetGridJsonBySerach(string objectId, string gridViewId, string account, string serachExpressionJson, string orderBy);

        string Create(string objectId, string opUserAccount, string propertyJson);

        void Modify(string objectId, string opUserAccount, string metadataId, string propertyJson);

        void Delete(string objectId, string opUserAccount, List<string> metadataIds);

        void ToggleFavorite(string objectId, string opUserAccount, List<string> metadataIds);

        string GetEditJson(string userAccount, string objectId, string meatadataId);

        string GetMetadatas(string objectCode, string account, string serachExpressionJson, string orderBy);
    }
}
