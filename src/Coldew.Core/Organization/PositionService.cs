﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.ObjectModel;
using Coldew.Api.Organization;

namespace Coldew.Core.Organization
{
    public class PositionService : IPositionService
    {
        public PositionService(ColdewManager coldewManager)
        {
            this.OrganizationManager = coldewManager.OrgManager;
        }

        public OrganizationManagement OrganizationManager { set; get; }

        public PositionInfo Create(string operationUserId, PositionCreateInfo createInfo)
        {
            User opUser = this.OrganizationManager.UserManager.GetUserById(operationUserId);
            Position position = this.OrganizationManager.PositionManager.Create(opUser, createInfo, OrganizationType.Position);
            return position.MapPositionInfo();
        }

        public void DeletePositionById(string operationUserId, string positionId)
        {
            User opUser = this.OrganizationManager.UserManager.GetUserById(operationUserId);
            this.OrganizationManager.PositionManager.Delete(opUser, positionId);
        }

        public void ChangePositionInfo(string operationUserId, PositionChangeInfo changeInfo)
        {
            User opUser = this.OrganizationManager.UserManager.GetUserById(operationUserId);
            Position position = this.OrganizationManager.PositionManager.GetPositionById(changeInfo.ID);
            position.Change(opUser, changeInfo);
        }

        public void RemoveUserFromPosition(string operationUserId, string positionId, string userId)
        {
            User opUser = this.OrganizationManager.UserManager.GetUserById(operationUserId);
            User user = this.OrganizationManager.UserManager.GetUserById(userId);
            Position position = this.OrganizationManager.PositionManager.GetPositionById(positionId);
            position.RemoveUser(opUser, user);
        }

        public PositionInfo GetPositionById(string positionId)
        {
            Position position = this.OrganizationManager.PositionManager.GetPositionById(positionId);
            if (position != null)
            {
                return position.MapPositionInfo();
            }
            return null;
        }

        public PositionInfo GetTopPosition()
        {
            return this.OrganizationManager.PositionManager.TopPosition.MapPositionInfo();
        }

        public IList<PositionInfo> GetChildPositions(string positionId)
        {
            Position position = this.OrganizationManager.PositionManager.GetPositionById(positionId);
            return position.Children.Select(x => x.MapPositionInfo()).ToList();
        }

        public IList<PositionInfo> GetPositionsByDepartmentId(string departmentId)
        {
            Department department = this.OrganizationManager.DepartmentManager.GetDepartmentById(departmentId);
            return department.Positions.Select(x => x.MapPositionInfo()).ToList();
        }

        public IList<PositionInfo> GetPositionsOfUser(string userId)
        {
            User user = this.OrganizationManager.UserManager.GetUserById(userId);
            return user.Positions.Select(x => x.MapPositionInfo()).ToList();
        }

        public IList<DepartmentInfo> GetManagerialDepartments(string positionId)
        {
            Position position = this.OrganizationManager.PositionManager.GetPositionById(positionId);
            if (position != null)
            {
                return position.ManagerialDepartments.Select(x => x.MapDepartmentInfo()).ToList();
            }
            return null;
        }

        public void ChangeUserPosition(string operationUserId, string userId, string positionId, string changePositionId)
        {
            User opUser = this.OrganizationManager.UserManager.GetUserById(operationUserId);
            User user = this.OrganizationManager.UserManager.GetUserById(userId);
            Position position = this.OrganizationManager.PositionManager.GetPositionById(positionId);
            Position changePosition = this.OrganizationManager.PositionManager.GetPositionById(changePositionId);
            position.RemoveUser(opUser, user);
            changePosition.AddUser(opUser, user);
        }

        public PositionInfo GetPositionByName(string name)
        {
            Position position = this.OrganizationManager.PositionManager.GetPositionByName(name);
            if (position != null)
            {
                return position.MapPositionInfo();
            }
            return null;
        }


        public IList<PositionInfo> GetAllPositions()
        {
            return this.OrganizationManager.PositionManager.Positions
                .Select(x => x.MapPositionInfo())
                .ToList();
        }

        public void AddUserToPosition(string operationUserId, string positionId, string userId)
        {
            User opUser = this.OrganizationManager.UserManager.GetUserById(operationUserId);
            User user = this.OrganizationManager.UserManager.GetUserById(userId);
            Position position = this.OrganizationManager.PositionManager.GetPositionById(positionId);
            position.AddUser(opUser, user);
        }

        public IList<PositionInfo> SearchByName(string name)
        {
            List<Position> posInfoList = this.OrganizationManager.PositionManager.SearchName(name);
            var result = new List<PositionInfo>();
            if (posInfoList != null && posInfoList.Count > 0)
                result.AddRange(from posInfo in posInfoList where posInfoList != null select posInfo.MapPositionInfo());
            return result;
        }
    }
}