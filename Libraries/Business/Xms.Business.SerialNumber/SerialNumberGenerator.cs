using System;
using System.Collections.Generic;
using Xms.Business.SerialNumber.Data;

namespace Xms.Business.SerialNumber
{
    /// <summary>
    /// 编号生成器
    /// </summary>
    public class SerialNumberGenerator : ISerialNumberGenerator
    {
        private readonly ISerialNumberRuleRepository _serialNumberRuleRepository;
        private readonly IEnumerable<IVariableReplacer> _variableReplacers;

        public SerialNumberGenerator(ISerialNumberRuleRepository serialNumberRuleRepository
            , IEnumerable<IVariableReplacer> variableReplacers)
        {
            _serialNumberRuleRepository = serialNumberRuleRepository;
            _variableReplacers = variableReplacers;
        }

        public string Generate(Guid ruleid)
        {
            var number = _serialNumberRuleRepository.GetSerialNumber(ruleid);
            foreach (var replacer in _variableReplacers)
            {
                number = replacer.Replace(number);
            }
            return number;
        }
    }
}