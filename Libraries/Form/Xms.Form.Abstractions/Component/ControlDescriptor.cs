using Newtonsoft.Json.Converters;
using System;

namespace Xms.Form.Abstractions.Component
{
    public class ControlDescriptor
    {
        public string Name { get; set; }
        public string EntityName { get; set; }
        private FormControlType _controlType;

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        public FormControlType ControlType
        {
            get
            {
                return _controlType;
            }
            set
            {
                _controlType = value;
                if (_controlType == FormControlType.Label || _controlType == FormControlType.Standard)
                {
                    Parameters = new ExtendPropertyParameters();
                }
                else if (_controlType == FormControlType.SubGrid)
                {
                    Parameters = new SubGridParameters();
                }
                else if (_controlType == FormControlType.Lookup)
                {
                    Parameters = new LookUpParameters();
                }
                else if (_controlType == FormControlType.IFrame)
                {
                    Parameters = new IFrameParameters();
                }
                else if (_controlType == FormControlType.Chart)
                {
                    Parameters = new ChartParameters();
                }
                else if (_controlType == FormControlType.Grid)
                {
                    Parameters = new GridParameters();
                }
                else if (_controlType == FormControlType.Report)
                {
                    Parameters = new ReportParameters();
                }
                else if (_controlType == FormControlType.FreeText)
                {
                    Parameters = new FreeTextparameters();
                }
            }
        }

        public bool ReadOnly { get; set; } = false;

        public ControlParameters Parameters { get; set; }

        public string Formula { get; set; }

        //[Newtonsoft.Json.JsonIgnore]

        public Schema.Domain.Attribute AttributeMetadata { get; set; }
    }

    public class ControlParameters { }

    public class ExtendPropertyParameters : ControlParameters
    {
        public string EntityName { get; set; }
        public string AttributeName { get; set; }
        public string SourceAttributeName { get; set; }
        public string SourceAttributeType { get; set; }
    }

    public class SubGridParameters : ControlParameters
    {
        public string ViewId { get; set; }
        public int PageSize { get; set; } = 5;
        public string TargetEntityName { get; set; }
        public string RelationshipName { get; set; }
        public bool Editable { get; set; } = true;
        public bool PagingEnabled { get; set; } = true;
        public int DefaultEmptyRows { get; set; } = 5;
        public FieldEvent[] FieldEvents { get; set; }
    }

    public class FieldEvent
    {
        public string Name { get; set; }
        public string Expression { get; set; }

        public string Type { get; set; }
    }

    public class LookUpParameters : ExtendPropertyParameters
    {
        public bool DefaultViewReadOnly { get; set; }
        public bool ViewPickerReadOnly { get; set; }
        public bool DisableViewPicker { get; set; }
        public string DefaultViewId { get; set; }

        //..........filter parameters.......//
        public string FilterRelationshipName { get; set; }

        public string DependentAttributeName { get; set; }
        public string DependentAttributeType { get; set; }
        public bool AllowFilterOff { get; set; }
    }

    public class IFrameParameters : ControlParameters
    {
        public string Url { get; set; }
        public bool Border { get; set; }

        public string Scroll { get; set; }
        public bool AllowCrossDomain { get; set; }
        public int RowSize { get; set; }
    }

    public class FreeTextparameters : ControlParameters
    {
        public string Content { get; set; }
    }

    public class ChartParameters : ControlParameters
    {
        public bool EnableChartPicker { get; set; }
        public bool EnableViewPicker { get; set; }
        public Guid ViewId { get; set; }
        public Guid ChartId { get; set; }
    }

    public class GridParameters : ControlParameters
    {
        public bool EnableQuickFind { get; set; }
        public bool EnableViewPicker { get; set; }
        public Guid ViewId { get; set; }
        public int RecordsPerPage { get; set; }
    }

    public class ReportParameters : ControlParameters
    {
        public Guid ReportId { get; set; }
        public bool EnableFilter { get; set; }
    }
}