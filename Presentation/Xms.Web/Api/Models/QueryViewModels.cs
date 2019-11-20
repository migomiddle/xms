using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Xms.Business.SerialNumber.Domain;
using Xms.Core;
using Xms.DataMapping.Domain;
using Xms.Flow.Abstractions;
using Xms.Flow.Domain;
using Xms.Form.Abstractions;
using Xms.Form.Abstractions.Component;
using Xms.Form.Domain;
using Xms.Logging.DataLog.Domain;
using Xms.QueryView.Abstractions.Component;
using Xms.Schema.Domain;
using Xms.Sdk.Abstractions.Query;
using Xms.Security.Domain;
using Xms.Web.Framework.Paging;

namespace Xms.Web.Models
{
    public class QueryViewLayoutConfigModel
    {
        public List<object> SortColumns { get; set; }
        public List<object> Rows { get; set; }
        public List<Guid> ClientResources { get; set; }
    }
}
