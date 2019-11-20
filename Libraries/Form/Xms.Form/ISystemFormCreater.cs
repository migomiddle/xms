namespace Xms.Form
{
    public interface ISystemFormCreater
    {
        bool Create(Domain.SystemForm entity);

        bool CreateDefaultForm(Schema.Domain.Entity entity);
    }
}