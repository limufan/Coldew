using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Coldew.Api;
using Coldew.Data;
using Coldew.Core;
using Coldew.Core.Organization;

namespace Coldew.Core
{
    public class GridViewColumnMapper
    {
        ColdewObjectManager _objectManager;
        public GridViewColumnMapper(ColdewObjectManager coldewManger)
        {
            this._objectManager = coldewManger;
        }

        public GridViewColumn MapColumn(GridViewColumnModel model)
        {
            dynamic d_model = model;
            return this.MapColumn(d_model);
        }

        private GridViewColumn MapColumn(GridViewFieldColumnModel model)
        {
            Field field = this._objectManager.GetFieldById(model.fieldId);
            return new GridViewFieldColumn(field);
        }

        private GridViewColumn MapColumn(GridViewRelatedColumnModel model)
        {
            Field field = this._objectManager.GetFieldById(model.fieldId);
            Field relatedField = this._objectManager.GetFieldById(model.relatedFieldId);
            return new GridViewRelatedColumn(field, relatedField);
        }

        public GridViewColumnModel MapColumnModel(GridViewColumn column)
        {
            dynamic d_column = column;
            return this.MapColumnModel(d_column);
        }

        private GridViewColumnModel MapColumnModel(GridViewFieldColumn column)
        {
            return new GridViewFieldColumnModel { fieldId = column.Field.ID };
        }

        private GridViewColumnModel MapColumnModel(GridViewRelatedColumn column)
        {
            return new GridViewRelatedColumnModel { fieldId = column.Field.ID, relatedFieldId = column.RelatedField.ID };
        }
    }

    public abstract class GridViewColumn
    {
        public int Width { set; get; }

        public abstract MetadataValue GetValue(Metadata metadata);
    }

    public class GridViewFieldColumn : GridViewColumn
    {
        public GridViewFieldColumn(Field field)
        {
            this.Field = field;
            this.Width = field.GridWidth;
        }

        public Field Field { set; get; }

        public override MetadataValue GetValue(Metadata metadata)
        {
            MetadataValue value = metadata.GetValue(this.Field.Code);
            return value;
        }
    }

    public class GridViewRelatedColumn : GridViewFieldColumn
    {
        public GridViewRelatedColumn(Field field, Field relatedField)
            :base(field)
        {
            this.RelatedField = relatedField;
            this.Width = relatedField.GridWidth;
        }

        public Field RelatedField { set; get; }

        public override MetadataValue GetValue(Metadata metadata)
        {
            return metadata.GetRelatedValue(this.Field, this.RelatedField);
        }
    }
}
