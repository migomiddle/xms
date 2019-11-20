using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Schema
{
    /// <summary>
    /// 架构元数据模块服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Schema.Data.IMetadataProvider, Schema.Data.MetadataProvider>();
            services.AddScoped<Schema.IMetadataService, Schema.MetadataService>();
            services.AddScoped<Schema.Entity.IEntityCreater, Schema.Entity.EntityCreater>();
            services.AddScoped<Schema.Entity.IEntityUpdater, Schema.Entity.EntityUpdater>();
            services.AddScoped<Schema.Entity.IEntityFinder, Schema.Entity.EntityFinder>();
            services.AddScoped<Schema.Entity.IEntityDeleter, Schema.Entity.EntityDeleter>();
            services.AddScoped<Schema.Attribute.IDefaultAttributeProvider, Schema.Attribute.DefaultAttributeProvider>();
            services.AddScoped<Schema.Attribute.IAttributeCreater, Schema.Attribute.AttributeCreater>();
            services.AddScoped<Schema.Attribute.IAttributeDeleter, Schema.Attribute.AttributeDeleter>();
            services.AddScoped<Schema.Attribute.IAttributeFinder, Schema.Attribute.AttributeFinder>();
            services.AddScoped<Schema.Attribute.IAttributeUpdater, Schema.Attribute.AttributeUpdater>();
            services.AddScoped<Schema.Attribute.IAttributeImporter, Schema.Attribute.AttributeImporter>();
            services.AddScoped<Schema.Attribute.IAttributeDependency, Schema.Attribute.AttributeDependency>();
            services.AddScoped<Schema.RelationShip.IRelationShipCreater, Schema.RelationShip.RelationShipCreater>();
            services.AddScoped<Schema.RelationShip.IRelationShipUpdater, Schema.RelationShip.RelationShipUpdater>();
            services.AddScoped<Schema.RelationShip.IRelationShipDeleter, Schema.RelationShip.RelationShipDeleter>();
            services.AddScoped<Schema.RelationShip.IRelationShipFinder, Schema.RelationShip.RelationShipFinder>();
            services.AddScoped<Schema.OptionSet.IOptionSetCreater, Schema.OptionSet.OptionSetCreater>();
            services.AddScoped<Schema.OptionSet.IOptionSetUpdater, Schema.OptionSet.OptionSetUpdater>();
            services.AddScoped<Schema.OptionSet.IOptionSetDeleter, Schema.OptionSet.OptionSetDeleter>();
            services.AddScoped<Schema.OptionSet.IOptionSetFinder, Schema.OptionSet.OptionSetFinder>();
            services.AddScoped<Schema.OptionSet.IOptionSetDetailCreater, Schema.OptionSet.OptionSetDetailCreater>();
            services.AddScoped<Schema.OptionSet.IOptionSetDetailUpdater, Schema.OptionSet.OptionSetDetailUpdater>();
            services.AddScoped<Schema.OptionSet.IOptionSetDetailDeleter, Schema.OptionSet.OptionSetDetailDeleter>();
            services.AddScoped<Schema.OptionSet.IOptionSetDetailFinder, Schema.OptionSet.OptionSetDetailFinder>();
            services.AddScoped<Schema.StringMap.IStringMapCreater, Schema.StringMap.StringMapCreater>();
            services.AddScoped<Schema.StringMap.IStringMapUpdater, Schema.StringMap.StringMapUpdater>();
            services.AddScoped<Schema.StringMap.IStringMapFinder, Schema.StringMap.StringMapFinder>();
            services.AddScoped<Schema.StringMap.IStringMapDeleter, Schema.StringMap.StringMapDeleter>();
        }
    }
}