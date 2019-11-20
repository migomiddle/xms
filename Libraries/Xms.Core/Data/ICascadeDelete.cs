namespace Xms.Core.Data
{
    public interface ICascadeDelete<TParent>
    {
        void CascadeDelete(params TParent[] parent);
    }
}