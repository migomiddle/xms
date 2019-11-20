using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;
using Xms.Sdk.Abstractions;
using Xms.Sdk.Query;

namespace Xms.Sdk.Data
{
    /// <summary>
    /// 组织实体数据处理
    /// </summary>
    public class OrganizationDataProvider : IOrganizationDataProvider
    {
        private readonly DataRepositoryBase<dynamic> _repository;

        public OrganizationDataProvider(
            IDbContext dbContext
            )
        {
            _repository = new DataRepositoryBase<dynamic>(dbContext);
        }

        #region transaction

        public void BeginTransaction()
        {
            _repository.BeginTransaction();
        }

        public void CommitTransaction()
        {
            _repository.CompleteTransaction();
        }

        public void RollBackTransaction()
        {
            _repository.RollBackTransaction();
        }

        #endregion transaction

        public Guid Create(Entity entity)
        {
            StringBuilder sql = new StringBuilder();
            entity = UnwrapAttributeValue(entity);

            sql.AppendFormat("INSERT INTO [{0}]", entity.Name);
            sql.Append(" (" + string.Join(",", entity.Keys.Select(x => "[" + x + "]")) + ")");
            sql.Append(" VALUES(");
            List<string> parameters = new List<string>();
            int i = 0;
            foreach (var k in entity.Keys)
            {
                parameters.Add("@" + i);
                i++;
            }
            sql.Append(string.Join(",", parameters));
            sql.Append(")");
            var flag = _repository.Execute(sql.ToString(), entity.Values.ToArray()) > 0;
            if (!flag)
            {
                return Guid.Empty;
            }
            return entity.Id;
        }

        public bool CreateMany(IList<Entity> entities)
        {
            StringBuilder sql = new StringBuilder();
            List<object> args = new List<object>();
            int j = 0;

            for (int i = 0; i < entities.Count; i++)
            {
                entities[i] = UnwrapAttributeValue(entities[i]);

                sql.AppendFormat("INSERT INTO [{0}]", entities[i].Name);
                sql.Append(" (");
                int x = 0;
                foreach (var k in entities[i].Keys)
                {
                    if (x > 0)
                    {
                        sql.Append(",");
                    }

                    sql.Append("[" + k + "]");
                    x++;
                }
                sql.Append(" )");
                sql.Append(" VALUES (");
                List<string> parameters = new List<string>();
                foreach (var k in entities[i].Keys)
                {
                    parameters.Add("@" + j);
                    j++;
                }
                sql.Append(string.Join(",", parameters));
                sql.Append(");");
                args.AddRange(entities[i].Values);
            }
            var flag = _repository.Execute(sql.ToString(), args.ToArray()) > 0;
            return flag;
        }

        public bool Update(Entity entity)
        {
            entity = UnwrapAttributeValue(entity);
            string primaryKey = entity.IdName;
            Guid id = Guid.Parse(entity[primaryKey].ToString());
            entity.Remove(primaryKey);
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(string.Format("UPDATE [{0}]", entity.Name));
            sql.AppendLine("SET");
            List<string> sets = new List<string>();
            int i = 0;
            List<object> values = new List<object>();
            foreach (var k in entity.Keys)
            {
                values.Add(entity[k]);
                sets.Add(string.Format("[{0}]={1}", k, "@" + i));
                i++;
            }
            values.Add(id);
            sql.AppendLine(string.Join(",\n", sets));
            sql.AppendLine(string.Format("WHERE [{0}]=@{1}", primaryKey, i));
            var flag = _repository.Execute(sql.ToString(), values.ToArray()) > 0;
            return flag;
        }

        public bool Update(Entity entity, IQueryResolver queryResolver, bool ignorePermissions = false)
        {
            entity = UnwrapAttributeValue(entity);
            string primaryKey = entity.IdName;
            entity.Remove(primaryKey);
            List<string> sets = new List<string>();
            List<object> values = new List<object>();
            //query string
            //query.ColumnSet.Columns.Clear();
            //query.ColumnSet.Columns.Add(primaryKey);
            var queryString = queryResolver.ToSqlString(false, ignorePermissions);
            values.AddRange(queryResolver.Parameters.Args);

            StringBuilder sql = new StringBuilder();
            sql.AppendLine(string.Format("UPDATE [{0}]", entity.Name));
            sql.AppendLine("SET");
            int i = values.Count;
            foreach (var k in entity.Keys)
            {
                values.Add(entity[k]);
                sets.Add(string.Format("[{0}]={1}", k, "@" + i));
                i++;
            }
            sql.AppendLine(string.Join(",\n", sets));
            sql.AppendLine(string.Format("WHERE [{0}] IN({1})", primaryKey, queryString));
            var flag = _repository.Execute(sql.ToString(), values.ToArray()) > 0;

            return flag;
        }

        public bool Delete(string name, Guid id, string primarykey = "")
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendLine(string.Format("DELETE [{0}]", name));
            sql.AppendLine(string.Format("WHERE [{0}]=@0", primarykey.IfEmpty(name + "Id")));
            return _repository.Execute(sql.ToString(), id) > 0;
        }

        private Entity UnwrapAttributeValue(Entity entity)
        {
            Entity result = new Entity(entity.Name);
            result.IdName = entity.IdName;
            result.Id = entity.Id;
            foreach (var item in entity)
            {
                result.Add(item.Key, item.Value);
            }
            ArrayList keys = new ArrayList(result.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                var k = keys[i].ToString();
                object value = result[k];
                if (value is bool)
                {
                    result[k] = ((bool)value) ? 1 : 0;
                }
                else if (value is EntityReference)
                {
                    result[k] = (value as EntityReference).ReferencedValue;
                }
                else if (value is OptionSetValue)
                {
                    result[k] = (value as OptionSetValue).Value;
                }
                else if (value is Money)
                {
                    result[k] = (value as Money).Value;
                }
                else if (value is OwnerObject)
                {
                    var o = (value as OwnerObject);
                    result[k] = o.OwnerId;
                    result["owneridtype"] = o.OwnerType;
                }
            }
            return result;
        }
    }
}