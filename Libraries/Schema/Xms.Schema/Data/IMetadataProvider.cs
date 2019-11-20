using System.Collections.Generic;

namespace Xms.Schema.Data
{
    public interface IMetadataProvider
    {
        void AddColumn(params Domain.Attribute[] attributes);

        void AlterColumn(params Domain.Attribute[] attributes);

        void AlterView(Domain.Entity entity, List<Domain.Attribute> attributes, List<Domain.RelationShip> relationShips);

        void CreateTable(Domain.Entity entity, List<Domain.Attribute> defaultAttributes);

        void DropColumn(params Domain.Attribute[] attributes);

        void DropTable(Domain.Entity entity);

        void DropView(Domain.Entity entity);

        bool TableExists(string table);

        bool ColumnExists(string table, string column);

        IEnumerable<dynamic> GetTableColumns(string table);
    }
}