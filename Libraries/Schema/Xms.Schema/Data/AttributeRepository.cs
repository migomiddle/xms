using System;
using System.Collections.Generic;
using Xms.Core.Data;
using Xms.Data;
using Xms.Data.Abstractions;

namespace Xms.Schema.Data
{
    /// <summary>
    /// 字段元数据仓储
    /// </summary>
    public class AttributeRepository : DefaultRepository<Domain.Attribute>, IAttributeRepository
    {
        private readonly IMetadataProvider _metadataProvider;

        public AttributeRepository(IDbContext dbContext, IMetadataProvider metadataProvider) : base(dbContext)
        {
            _metadataProvider = metadataProvider;
        }

        #region implements

        /// <summary>
        /// 创建记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override bool Create(Domain.Attribute entity)
        {
            var result = false;
            using (UnitOfWork.Build(DbContext))
            {
                result = base.Create(entity);
                //新建数据库表字段
                _metadataProvider.AddColumn(entity);
            }
            return result;
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override bool Update(Domain.Attribute entity)
        {
            var result = false;
            var original = FindById(entity.AttributeId);
            using (UnitOfWork.Build(DbContext))
            {
                result = _repository.Update(entity);
                //如果字段长度更改，并且大于原有的长度
                if (entity.MaxLength > original.MaxLength)
                {
                    //更改数据库表字段
                    _metadataProvider.AlterColumn(entity);
                }
            }
            return result;
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Delete(Domain.Attribute entity)
        {
            var flag = false;
            using (UnitOfWork.Build(DbContext))
            {
                flag = base.DeleteById(entity.AttributeId);
                //删除表字段
                _metadataProvider.DropColumn(entity);
            }
            return flag;
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override bool DeleteById(Guid id)
        {
            Domain.Attribute entity = this.FindById(id);
            return Delete(entity);
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public override bool DeleteMany(IEnumerable<Guid> ids)
        {
            var flag = false;
            foreach (var id in ids)
            {
                flag = this.DeleteById(id);
            }
            return flag;
        }

        #endregion implements
    }
}