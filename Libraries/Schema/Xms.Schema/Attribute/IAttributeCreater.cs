namespace Xms.Schema.Attribute
{
    public interface IAttributeCreater
    {
        bool Create(Domain.Attribute entity);

        bool CreateDefaultAttributes(Domain.Entity entity, params string[] defaultAttributeNames);

        bool CreateOwnerAttributes(Domain.Entity entity);

        bool CreateWorkFlowAttributes(Domain.Entity entity);

        bool CreateBusinessFlowAttributes(Domain.Entity entity);
    }
}