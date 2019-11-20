using System;
using System.Collections.Generic;
using Xms.Core.Data;

namespace Xms.Business.FormStateRule
{
    public interface ISystemFormStatusSetter
    {
        bool IsDisabled(List<Guid> rulesId, Entity data);
    }
}