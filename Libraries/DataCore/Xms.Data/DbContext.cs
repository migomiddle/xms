using PetaPoco;
using System;
using System.Collections.Generic;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Data.Provider;
using Xms.Infrastructure;

namespace Xms.Data
{
    public class DbContext : PetaPoco.Database, IDbContext
    {
        private readonly Guid Id = Guid.NewGuid();
        public bool TransactionCancelled { get; set; } = false;

        public DbContext(IDataProviderOptions options)
            : base(PetaPoco.DatabaseConfiguration.Build()
                  .UsingDefaultMapper<DomainMapper>()
                  .UsingConnectionString(options.ConnectionString)
                  .UsingProviderName(options.ProviderName)
                  .UsingCommandTimeout(options.CommandTimeOut))
        {
            KeepConnectionAlive = true;

            //IsolationLevel = System.Data.IsolationLevel.ReadUncommitted;
            //CommandExecuted += DbContext_CommandExecuted;
        }

        private void DbContext_CommandExecuted(object sender, DbCommandEventArgs e)
        {
        }

        public void RollBackTransaction()
        {
            TransactionCancelled = true;
            base.AbortTransaction();
        }

        public override bool OnException(Exception e)
        {
            TransactionCancelled = true;
            //base.AbortTransaction();
            var args = new List<string>();
            foreach (var arg in LastArgs)
            {
                if (arg is AnsiString)
                {
                    args.Add((arg as AnsiString).Value);
                }
                else if (arg != null)
                {
                    args.Add(arg.ToString());
                }
            }
            var sql = LastSQL + (LastArgs != null ? ";\n" + string.Join(",", args) : "");
            base.Dispose();
            throw new XmsException(e.Message + ": " + sql, e);
        }
    }
}