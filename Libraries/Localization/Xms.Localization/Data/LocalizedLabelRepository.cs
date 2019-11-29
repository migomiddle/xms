using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Localization.Domain;

namespace Xms.Localization.Data
{
    /// <summary>
    /// 本地化标签仓储
    /// </summary>
    public class LocalizedLabelRepository : DefaultRepository<LocalizedLabel>, ILocalizedLabelRepository
    {
        public LocalizedLabelRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool DeleteByObject(Guid objectId, params string[] name)
        {
            Sql s = Sql.Builder.Append("delete localizedlabel where objectid=@0", objectId);
            if (name.NotEmpty())
            {
                s.Append("and objectcolumnname in(@0)", string.Join(",", name));
            }
            return _repository.Execute(s) > 0;
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool DeleteByObject(IEnumerable<Guid> objectId)
        {
            Sql s = Sql.Builder.Append("delete localizedlabel where objectid in(@0)", string.Join(",", objectId));
            return _repository.Execute(s) > 0;
        }

        public List<dynamic> Export(Guid solutionId, LanguageCode baseLanguageId, IEnumerable<Language> languages)
        {
            Sql s = Sql.Builder.Append("SELECT label.ObjectId,label.ObjectColumnName,label.LabelTypeCode");
            foreach (var item in languages)
            {
                s.Append(string.Format(@",(CASE LanguageId WHEN {0} THEN label.Label END) AS '{0}'", item.UniqueId));
            }
            s.Append("FROM " + TableName + " AS label")
                .Append("WHERE label.SolutionId=@0", solutionId)
                .Append("GROUP BY label.ObjectId,label.ObjectColumnName,label.LabelTypeCode,label.LanguageId,label.Label")
                .Append("ORDER BY label.LabelTypeCode,label.ObjectId");
            return new DataRepositoryBase<dynamic>(DbContext).ExecuteQuery(s);
        }

        public string GetLocalizedLabelXml(Guid solutionId)
        {
            var result = new DataRepositoryBase<object>(DbContext).Find(@"select convert(xml,(select a.ComponentState,a.Label,a.LabelTypeCode,a.LanguageId,a.LocalizedLabelId,a.ObjectColumnName,a.ObjectId,a.SolutionId
            from LocalizedLabel a
            FOR XML PATH('LocalizedLabel'),ROOT('LocalizedLabels')))");
            if (result != null)
            {
                var data = (result as IDictionary<string, object>).ToList();
                if (data.NotEmpty() && data[0].Value != null)
                {
                    return data[0].Value.ToString();
                }
            }
            return string.Empty;
        }

        #endregion implements
    }
}