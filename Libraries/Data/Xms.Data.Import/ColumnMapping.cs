namespace Xms.Data.Import
{
    public class ColumnMapping
    {
        public string Column { get; set; }
        public Mapping Mapping { get; set; }
        public bool IsUpdatePrimaryField { get; set; }
    }

    public class Mapping
    {
        public string Attribute { get; set; }
        public string NullHandle { get; set; }
        public string LookupName { get; set; }
    }
}