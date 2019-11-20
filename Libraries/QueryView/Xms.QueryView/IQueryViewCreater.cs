namespace Xms.QueryView
{
    public interface IQueryViewCreater
    {
        bool Create(Domain.QueryView entity);

        bool CreateDefaultView(Schema.Domain.Entity entity);
    }
}