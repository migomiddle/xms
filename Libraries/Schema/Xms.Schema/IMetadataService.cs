using System.Collections.Generic;

namespace Xms.Schema
{
    public interface IMetadataService
    {
        void AddColumn(Domain.Attribute attr);

        void AlterColumn(Domain.Attribute attr);

        void AlterView(Domain.Entity entity);

        void CreateTable(Domain.Entity entity, List<Domain.Attribute> defaultAttributes);

        void CreateView(Domain.Entity entity);

        void DropColumn(Domain.Attribute attr);

        void DropTable(Domain.Entity entity);

        void DropView(Domain.Entity entity);
    }
}