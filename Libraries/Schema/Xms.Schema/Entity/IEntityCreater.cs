namespace Xms.Schema.Entity
{
    public interface IEntityCreater
    {
        bool Create(Domain.Entity entity, bool makeAllDefaultAttributes = true);

        bool Create(Domain.Entity entity, params string[] defaultAttributeNames);
    }
}