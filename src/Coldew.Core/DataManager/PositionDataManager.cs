using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api.Organization;
using Coldew.Core.DataProviders;
using Coldew.Core.Organization;

namespace Coldew.Core.DataManager
{
    public class PositionDataManager
    {
        public PositionDataProvider DataProvider { private set; get; }
        OrganizationManagement _orgManager;
        public PositionDataManager(OrganizationManagement orgManager)
        {
            this._orgManager = orgManager;
            this.DataProvider = new PositionDataProvider(orgManager);
            orgManager.PositionManager.Created += PositionManager_Created;
            orgManager.PositionManager.Deleted += PositionManager_Deleted;
            this.Load();
        }

        void PositionManager_Deleted(PositionManagement manager, DeleteEventArgs<Position> args)
        {
            this.DataProvider.Delete(args.DeleteObject);
        }

        void PositionManager_Created(PositionManagement manager, Position position)
        {
            this.DataProvider.Insert(position);
            this.BindEvent(position);
        }

        private void BindEvent(Position position)
        {
            position.Changed += Position_Changed;
            position.AddedUser += Position_AddedUser;
            position.RemovedUser += Position_RemovedUser;
        }

        void Position_RemovedUser(Position sender, User args)
        {
            this.DataProvider.Update(sender);
        }

        void Position_AddedUser(Position sender, User args)
        {
            this.DataProvider.Update(sender);
        }

        void Position_Changed(Position position, PositionChangeInfo changeInfo)
        {
            this.DataProvider.Update(position);
        }

        void Load()
        {
            List<Position> positions = this.DataProvider.Select();
            this._orgManager.PositionManager.AddPosition(positions);
            foreach (Position position in positions)
            {
                this.BindEvent(position);
            }
        }

        public void LoadUsers()
        {
            this.DataProvider.LoadUsers(this._orgManager.PositionManager.Positions.ToList());
        }
    }
}
