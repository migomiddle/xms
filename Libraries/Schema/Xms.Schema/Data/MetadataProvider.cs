using PetaPoco;
using System.Collections.Generic;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;

namespace Xms.Schema.Data
{
    /// <summary>
    /// 数据库架构操作
    /// </summary>
    public class MetadataProvider : IMetadataProvider
    {
        private readonly Database _database;
        private readonly IDbContext _dbContext;

        public MetadataProvider(IDbContext dbContext)
        {
            _dbContext = dbContext;
            _database = _dbContext as DbContext;
        }

        #region 列

        public void AddColumn(params Domain.Attribute[] attributes)
        {
            Guard.NotEmpty(attributes, nameof(attributes));
            Sql s = Sql.Builder;
            foreach (var attr in attributes)
            {
                if (!this.ColumnExists(attr.EntityName, attr.Name))
                {
                    s.Append(string.Format("ALTER TABLE [{0}] ADD [{1}] {2};", attr.EntityName, attr.Name, attr.GetDbType()));
                }
            }
            if (s.SQL.IsNotEmpty())
            {
                _database.Execute(s);
            }
        }

        public void AlterColumn(params Domain.Attribute[] attributes)
        {
            Guard.NotEmpty(attributes, nameof(attributes));
            Sql s = Sql.Builder;
            foreach (var attr in attributes)
            {
                if (this.ColumnExists(attr.EntityName, attr.Name))
                {
                    _database.Execute(string.Format("ALTER TABLE [{0}] ALTER COLUMN [{1}] {2} NULL", attr.EntityName, attr.Name, attr.GetDbType()));
                }
            }
            if (s.SQL.IsNotEmpty())
            {
                _database.Execute(s);
            }
        }

        public IEnumerable<dynamic> GetTableColumns(string table)
        {
            return _database.Query<dynamic>("select sc.name,st.name as typename from syscolumns sc,systypes st where sc.xtype=st.xtype and sc.id=Object_Id(@0)", table);
        }

        public bool ColumnExists(string table, string column)
        {
            return _database.ExecuteScalar<int>("select count(1) from syscolumns where id=Object_Id(@0) and name=@1", table, column) > 0;
        }

        public void DropColumn(params Domain.Attribute[] attributes)
        {
            Guard.NotEmpty(attributes, nameof(attributes));
            Sql dropSql = Sql.Builder;
            foreach (var attr in attributes)
            {
                if (this.ColumnExists(attr.EntityName, attr.Name))
                {
                    dropSql.Append(string.Format("ALTER TABLE [{0}] DROP COLUMN [{1}]", attr.EntityName, attr.Name, attr.GetDbType()));
                }
            }
            if (dropSql.SQL.IsNotEmpty())
            {
                _database.Execute(dropSql);
            }
        }

        #endregion 列

        #region 表

        public void AlterView(Domain.Entity entity, List<Domain.Attribute> attributes, List<Domain.RelationShip> relationShips)
        {
            Sql dropSql = Sql.Builder.Append(string.Format("IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[{0}View]') AND OBJECTPROPERTY(id, N'IsView') = 1) ", entity.Name))
                .Append(string.Format("DROP VIEW [dbo].[{0}View]", entity.Name));
            Sql createSql = Sql.Builder.Append(string.Format("CREATE VIEW [{0}View] AS", entity.Name))
            .Append("SELECT");//.Append("GO");

            List<string> attrNames = new List<string>();
            foreach (var item in attributes)
            {
                attrNames.Add(string.Format("[{0}].[{1}]", entity.Name, item.Name));
            }

            foreach (var item in relationShips)
            {
                if (attributes.Exists(n => n.AttributeId == item.ReferencingAttributeId))
                {
                    attrNames.Add(string.Format("[{0}].[Name] AS {1}Name", item.Name, item.ReferencingAttributeName));
                }
            }

            createSql.Append(string.Join(",\n", attrNames));
            //tables
            createSql.Append(string.Format("FROM [{0}] WITH(NOLOCK)", entity.Name));
            foreach (var item in relationShips)
            {
                createSql.Append(string.Format("LEFT JOIN [{1}] AS [{3}] WITH(NOLOCK) ON [{2}].[{0}]=[{3}].[{4}]", item.ReferencingAttributeName, item.ReferencedEntityName, entity.Name, item.Name, item.ReferencedAttributeName));
            }
            _database.Execute(dropSql);
            _database.Execute(createSql);
        }

        public void CreateTable(Domain.Entity entity, List<Domain.Attribute> defaultAttributes)
        {
            if (this.TableExists(entity.Name))
            {
                return;
            }
            Sql s = Sql.Builder.Append(string.Format("CREATE TABLE [dbo].[{0}]", entity.Name))
                .Append("(");
            //.Append(string.Format("[{0}Id] [uniqueidentifier] NOT NULL,", entity.Name))
            //.Append("[Name] [nvarchar](300) NULL,")
            //.Append("[VersionNumber] [timestamp] NOT NULL,")
            //.Append("[CreatedOn] [datetime] NOT NULL,")
            //.Append("[CreatedBy] [uniqueidentifier] NOT NULL,")
            //.Append("[ModifiedOn] [datetime] NULL,")
            //.Append("[ModifiedBy] [uniqueidentifier] NULL,")
            //.Append("[StateCode] [int] NOT NULL,")
            //.Append("[StatusCode] [int] NOT NULL,");
            foreach (var attribute in defaultAttributes)
            {
                s.Append("[{0}] {1} {2} NULL,".FormatWith(attribute.Name, attribute.GetDbType(), attribute.IsNullable ? "" : "NOT"));
            }
            List<string> idxColumns = new List<string>();//需要创建索引的字段
            if (defaultAttributes.Exists(x => x.Name.IsCaseInsensitiveEqual("StateCode")))
            {
                idxColumns.Add("StateCode");
            }
            if (defaultAttributes.Exists(x => x.Name.IsCaseInsensitiveEqual("CreatedOn")))
            {
                idxColumns.Add("CreatedOn");
            }
            if (entity.EntityMask == EntityMaskEnum.User)
            {
                //s.Append("[OwnerId] [uniqueidentifier] NOT NULL,")
                //.Append("[OwnerIdType] [int] NOT NULL,")
                //.Append("[OwningBusinessUnit] [uniqueidentifier] NULL,");
                if (defaultAttributes.Exists(x => x.Name.IsCaseInsensitiveEqual("OwnerId")))
                {
                    if (!defaultAttributes.Exists(x => x.Name.IsCaseInsensitiveEqual("OwnerIdType")))
                    {
                        s.Append("[OwnerIdType] [int] NOT NULL,");
                    }
                    idxColumns.Add("OwnerId");
                }
            }
            if (defaultAttributes.Exists(x => x.Name.IsCaseInsensitiveEqual("OrganizationId")))
            {
                //s.Append("[OrganizationId] [uniqueidentifier] NOT NULL,");
                idxColumns.Add("OrganizationId");
            }
            //if (entity.WorkFlowEnabled)
            //{
            //    s.Append("[WorkFlowId] [uniqueidentifier] NULL,");
            //    s.Append("[ProcessState] [int] NULL default(-1),");
            //}
            //if (entity.BusinessFlowEnabled)
            //{
            //    s.Append("[StageId] [uniqueidentifier] NULL,");
            //}
            //主键
            s.Append(string.Format("CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED", entity.Name))
            .Append("(")
            .Append(string.Format("[{0}Id] ASC", entity.Name))
            .Append(")WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON[PRIMARY]")
            .Append(") ON [PRIMARY]");
            //创建外键约束
            if (defaultAttributes.Exists(x => x.Name.IsCaseInsensitiveEqual("CreatedBy")))
            {
                s.Append(string.Format("ALTER TABLE [dbo].[{0}]  WITH CHECK ADD  CONSTRAINT [FK_{0}_SystemUser_CreatedBy] FOREIGN KEY([CreatedBy])", entity.Name));
                s.Append("REFERENCES[dbo].[SystemUser]([SystemUserId])");
                s.Append(string.Format("ALTER TABLE [dbo].[{0}] CHECK CONSTRAINT [FK_{0}_SystemUser_CreatedBy]", entity.Name));
            }
            if (entity.EntityMask == EntityMaskEnum.User)
            {
                if (defaultAttributes.Exists(x => x.Name.IsCaseInsensitiveEqual("OwningBusinessUnit")))
                {
                    s.Append(string.Format("ALTER TABLE [dbo].[{0}]  WITH NOCHECK ADD  CONSTRAINT [FK_{0}_BusinessUnit_OwningBusinessUnit] FOREIGN KEY([OwningBusinessUnit])", entity.Name));
                    s.Append("REFERENCES[dbo].[BusinessUnit]([BusinessUnitId])");
                    s.Append(string.Format("ALTER TABLE [dbo].[{0}] CHECK CONSTRAINT [FK_{0}_BusinessUnit_OwningBusinessUnit]", entity.Name));
                }
            }
            if (defaultAttributes.Exists(x => x.Name.IsCaseInsensitiveEqual("OrganizationId")))
            {
                s.Append(string.Format("ALTER TABLE [dbo].[{0}]  WITH NOCHECK ADD  CONSTRAINT [FK_{0}_Organization_OrganizationId] FOREIGN KEY([OrganizationId])", entity.Name));
                s.Append("REFERENCES[dbo].[Organization]([OrganizationId])");
                s.Append(string.Format("ALTER TABLE [dbo].[{0}] CHECK CONSTRAINT [FK_{0}_Organization_OrganizationId]", entity.Name));
            }

            //启动流程时建立约束
            if (entity.WorkFlowEnabled)
            {
                if (defaultAttributes.Exists(x => x.Name.IsCaseInsensitiveEqual("WorkFlowId")))
                {
                    s.Append(string.Format("ALTER TABLE [dbo].[{0}]  WITH NOCHECK ADD  CONSTRAINT [FK_{0}_WorkFlow_WorkFlowId] FOREIGN KEY([WorkFlowId])", entity.Name));
                    s.Append("REFERENCES[dbo].[WorkFlow]([WorkFlowId])");
                    s.Append(string.Format("ALTER TABLE [dbo].[{0}] CHECK CONSTRAINT [FK_{0}_WorkFlow_WorkFlowId]", entity.Name));
                }
            }
            //创建默认字段索引
            if (idxColumns.NotEmpty())
            {
                s.Append(string.Format("CREATE INDEX ndx_core ON {0}({1}) WITH (FILLFACTOR = 80)", entity.Name, string.Join(",", idxColumns)));
            }

            _database.Execute(s);
        }

        public bool TableExists(string table)
        {
            return _database.ExecuteScalar<int>("select count(1) from sysobjects where xtype='U' and name = @0", table) > 0;
        }

        public void DropTable(Domain.Entity entity)
        {
            Sql dropSql = Sql.Builder.Append(string.Format("IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[{0}]')) ", entity.Name))
                .Append(string.Format("DROP TABLE [dbo].[{0}]", entity.Name));
            _database.Execute(dropSql);
        }

        public void DropView(Domain.Entity entity)
        {
            Sql dropSql = Sql.Builder.Append(string.Format("IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[{0}View]') AND OBJECTPROPERTY(id, N'IsView') = 1) ", entity.Name))
                .Append(string.Format("DROP VIEW [dbo].[{0}View]", entity.Name));
            _database.Execute(dropSql);
        }

        #endregion 表
    }
}