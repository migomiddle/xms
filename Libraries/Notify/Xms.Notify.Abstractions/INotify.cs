namespace Xms.Notify.Abstractions
{
    public interface INotify
    {
        object Send(NotifyBody body);
    }
}