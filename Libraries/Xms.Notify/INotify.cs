namespace Xms.Notify
{
    public interface INotify
    {
        object Send(NotifyBody body);
    }
}