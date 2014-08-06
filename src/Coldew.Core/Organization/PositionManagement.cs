using System;
using System.Collections.Generic;
using System.Linq;
using Coldew.Api.Organization.Exceptions;
using System.Collections.ObjectModel;


using Coldew.Api.Organization;


namespace Coldew.Core.Organization
{
    public class PositionManagement
    {

        private Dictionary<string, Position> _positionByIdDictionary;

        public PositionManagement(OrganizationManagement orgMnger)
        {
            this._orgMnger = orgMnger;
            this._positions = new List<Position>();
            this._positionByIdDictionary = new Dictionary<string, Position>();
        }

        private bool _loaded;
        OrganizationManagement _orgMnger;

        List<Position> _positions;
        List<Position> _Positions
        {
            get
            {
                return _positions;
            }
        }

        /// <summary>
        /// 创建用户之前
        /// </summary>
        public event TEventHandler<PositionManagement, PositionCreateInfo> Creating;

        /// <summary>
        /// 创建用户之后
        /// </summary>
        public event TEventHandler<PositionManagement, Position> Created;

        /// <summary>
        /// 删除用户之前
        /// </summary>
        public event TEventHandler<PositionManagement, DeleteEventArgs<Position>> Deleting;

        /// <summary>
        /// 删除用户之后
        /// </summary>
        public event TEventHandler<PositionManagement, DeleteEventArgs<Position>> Deleted;

        private object _updateLockObject = new object();

        public Position Create(User operationUser, PositionCreateInfo createInfo, OrganizationType positionType)
        {
            lock (this._updateLockObject)
            {
                if (operationUser == null)
                {
                    throw new ArgumentNullException("operationUser");
                }
                if (string.IsNullOrWhiteSpace(createInfo.Name))
                {
                    throw new OrganizationException("createInfo.Name不能为空!");
                }

                List<Position> positions = this._Positions;
                Position parent = this.GetPositionById(createInfo.ParentId);
                if (parent != null)
                {
                    Position child = parent.Children.FirstOrDefault(x => x.Name == createInfo.Name);
                    if (child != null)
                    {
                        throw new PositionNameReapeatException();
                    }
                }

                CreatedEventArgs<PositionCreateInfo, Position> args = new CreatedEventArgs<PositionCreateInfo, Position>
                {
                    CreateInfo = createInfo,
                    Operator = operationUser
                };
                if (this.Creating != null)
                {
                    this.Creating(this, createInfo);
                }

                Position position = new Position(Guid.NewGuid().ToString(), createInfo.Name, createInfo.ParentId, createInfo.Remark, this._orgMnger);

                List<Position> tempPositions = positions.ToList();
                tempPositions.Add(position);
                this._positions = tempPositions;

                this.IndexPosition();
                if (this.Created != null)
                {
                    this.Created(this, position);
                }
                return position;
            }
        }

        /// <summary>
        /// 创建职位
        /// </summary>
        /// <param name="position">职位</param>
        /// <returns>职位</returns>
        public Position Create(User operationUser, PositionCreateInfo createInfo)
        {
            return this.Create(operationUser, createInfo, OrganizationType.Position);
        }

        
        /// <summary>
        /// 删除职位
        /// </summary>
        /// <param name="positionId">职位ID</param>
        public void Delete(User operationUser, string positionId)
        {
            if (operationUser == null)
            {
                throw new ArgumentNullException("operationUser");
            }

            Position position = this.GetPositionById(positionId);
            if (position != null)
            {
                if (position.Users.Count > 0)
                {
                    throw new PositionHasUserDeleteException();
                }
                if (position.SelfChildrenHasUser())
                {
                    throw new PositionHasUserDeleteException();
                }
                lock (_updateLockObject)
                {
                    DeleteEventArgs<Position> args = new DeleteEventArgs<Position>();
                    args.DeleteObject = position;
                    args.Operator = operationUser;
                    foreach (Position child in position.Children)
                    {
                        if (child.PositionType != OrganizationType.ManagerPosition)
                        {
                            this.Delete(operationUser, child.ID);
                        }
                    }
                    foreach (Department managerDepartment in position.SelfManagerialDepartments)
                    {
                        this._orgMnger.DepartmentManager.Delete(operationUser, managerDepartment.ID);
                    }
                    if (Deleting != null)
                    {
                        this.Deleting(this, args);
                    }

                    List<Position> tempPositions = this._Positions.ToList();
                    tempPositions.Remove(position);
                    this._positions = tempPositions;

                    this.IndexPosition();
                    if (this.Deleted != null)
                    {
                        this.Deleted(this, args);
                    }
                }
            }
        }

        public Position TopPosition
        {
            get
            {
                
                return (from p in this._Positions
                        where p.Parent == null
                        select p).FirstOrDefault();
            }
        }

        public Position GetPositionById(string positionId)
        {
            if (string.IsNullOrEmpty(positionId))
            {
                return null;
            }
            try
            {
                return this._positionByIdDictionary[positionId];
            }
            catch (KeyNotFoundException)
            {

            }
            return null;
        }

        public Position GetPositionByName(string name)
        {

            return this._Positions.Find(x => x.Name == name);
        }

        public List<Position> SearchName(string name)
        {
            return this._Positions.Where(x => x.Name.IndexOf(name, StringComparison.InvariantCultureIgnoreCase) > -1).ToList();
        }

        private void IndexPosition()
        {
            this._positionByIdDictionary = this._positions.ToDictionary(x => x.ID);
        }

        /// <summary>
        /// 获取用户的所属职位列表
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns>职位列表</returns>
        public ReadOnlyCollection<Position> Positions
        {
            get
            {
                
                return this._Positions.AsReadOnly();
            }
        }

        public event TEventHandler<PositionManagement, List<Position>> Added;

        public virtual void AddPosition(List<Position> positions)
        {
            this._positions.AddRange(positions);
            this.IndexPosition();
            if (this.Added != null)
            {
                this.Added(this, positions);
            }
        }
    }
}
