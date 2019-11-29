using Xms.Context;
using Xms.Identity;
using Xms.Organization;

namespace Xms.Business.SerialNumber
{
    /// <summary>
    /// 用户变量替换器
    /// </summary>
    public class UserVariableReplacer : IVariableReplacer
    {
        private readonly IAppContext _appContext;
        private readonly ICurrentUser _currentUser;
        private readonly IBusinessUnitService _businessUnitService;

        public UserVariableReplacer(IAppContext appContext
            , IBusinessUnitService businessUnitService)
        {
            _appContext = appContext;
            _currentUser = _appContext.GetFeature<ICurrentUser>();
            _businessUnitService = businessUnitService;
        }

        public string Replace(string text)
        {
            //当前部门编号
            if (text.IndexOf("{businessunit.number}") >= 0)
            {
                text = text.Replace("{businessunit.number}", _businessUnitService.FindById(_currentUser.BusinessUnitId).UnitNumber);
            }
            //上级部门编号
            if (text.IndexOf("{businessunit.parent.number}") >= 0)
            {
                var currentBusinessUnit = _businessUnitService.FindById(_currentUser.BusinessUnitId);
                if (currentBusinessUnit.ParentBusinessUnitId.HasValue)
                {
                    var parentBusinessUnit = _businessUnitService.FindById(currentBusinessUnit.ParentBusinessUnitId.Value);
                    text = text.Replace("{businessunit.parent.number}", parentBusinessUnit.UnitNumber);
                }
            }
            //组织唯一名称
            text = text.Replace("{org.uniquename}", _currentUser.OrgInfo.UniqueName);
            return text;
        }
    }
}