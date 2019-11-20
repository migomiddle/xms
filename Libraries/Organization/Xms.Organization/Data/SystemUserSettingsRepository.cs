using PetaPoco;
using System;
using Xms.Core.Data;
using Xms.Data;
using Xms.Organization.Domain;

namespace Xms.Organization.Data
{
    /// <summary>
    /// 用户参数配置仓储
    /// </summary>
    public class SystemUserSettingsRepository : ISystemUserSettingsRepository
    {
        private readonly DataRepositoryBase<UserSettings> _repository;

        public SystemUserSettingsRepository(IDbContext dbContext)
        {
            _repository = new DataRepositoryBase<UserSettings>(dbContext);
        }

        public UserSettings FindById(Guid id)
        {
            return _repository.ExecuteFind(Sql.Builder.Append("select * from systemusersettings where systemusersettingsid=@0", id));
        }
    }
}