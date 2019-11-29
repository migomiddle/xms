using Xms.Core;
using Xms.Infrastructure.Utility;
using Xms.Plugin.Abstractions;
using Xms.Sdk.Extensions;

namespace Xms.Business.SerialNumber
{
    /// <summary>
    /// 自动编号插件执行服务
    /// </summary>
    public class SerialNumberExecutor : IEntityPlugin
    {
        private readonly ISerialNumberRuleFinder _serialNumberRuleFinder;
        private readonly ISerialNumberGenerator _serialNumberGenerator;

        public SerialNumberExecutor(ISerialNumberRuleFinder serialNumberRuleFinder
            , ISerialNumberGenerator serialNumberGenerator)
        {
            _serialNumberRuleFinder = serialNumberRuleFinder;
            _serialNumberGenerator = serialNumberGenerator;
        }

        public void Execute(PluginExecutionContext context)
        {
            if (context.MessageName == OperationTypeEnum.Create && context.Stage == OperationStage.PreOperation)
            {
                var snr = _serialNumberRuleFinder.FindByEntityId(context.EntityMetadata.EntityId);
                if (snr != null && context.Target.GetStringValue(snr.AttributeName).IsEmpty())
                {
                    context.Target.SetAttributeValue(snr.AttributeName, _serialNumberGenerator.Generate(snr.SerialNumberRuleId));
                }
            }
        }
    }
}