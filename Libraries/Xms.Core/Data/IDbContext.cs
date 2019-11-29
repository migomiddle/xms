namespace Xms.Core.Data
{
    public interface IDbContext
    {
        bool TransactionCancelled { get; set; }

        void BeginTransaction();

        void CompleteTransaction();

        void RollBackTransaction();
    }
}