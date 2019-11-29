using System;

namespace Xms.Web.Framework.Models
{
    public class DeleteModel
    {
        public Guid RecordId { get; set; }
    }

    public class DeleteManyModel
    {
        public Guid[] RecordId { get; set; }
    }

    public class SetRecordStateModel
    {
        public Guid[] RecordId { get; set; }
        public bool IsEnabled { get; set; }
    }
}