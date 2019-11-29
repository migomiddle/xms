namespace Xms.Schema.Abstractions
{
    public enum CascadeUpdateType
    {
        All = 1,
        Actived = 2,
        Owner = 3,
        None = 4
    }

    public enum CascadeDeleteType
    {
        All = 1,
        RemoveLink = 2,
        Restrict = 3,
        None = 4
    }
}