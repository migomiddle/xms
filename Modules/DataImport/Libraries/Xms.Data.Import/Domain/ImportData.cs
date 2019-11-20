using PetaPoco;
using System;

namespace Xms.Data.Import.Domain
{
    [TableName("ImportData")]
    [PrimaryKey("ImportDataId", AutoIncrement = false)]
    public class ImportData
    {
        public Guid ImportDataId { get; set; } = Guid.NewGuid();
        public Guid RecordId { get; set; }
        public string Data { get; set; }
        public int LineNumber { get; set; }
        public Guid ImportFileId { get; set; }
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public int ErrorType { get; set; }
    }
}