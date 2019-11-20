using PetaPoco;
using System;

namespace Xms.Data.Import.Domain
{
    [TableName("ImportFile")]
    [PrimaryKey("ImportFileId", AutoIncrement = false)]
    public class ImportFile
    {
        public Guid ImportFileId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public bool IsFirstRowHeader { get; set; }
        public string Content { get; set; }
        public int SuccessCount { get; set; }
        public Guid RecordsOwnerId { get; set; }
        public int DuplicateDetection { get; set; }
        public int FailureCount { get; set; }
        public string TargetEntityName { get; set; }
        public string FieldDelimiterCode { get; set; }
        public string HeaderRow { get; set; }
        public int TotalCount { get; set; }
        public int Size { get; set; }
        public Guid ImportMapId { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}