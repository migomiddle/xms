using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using Xms.Core.Context;
using Xms.Flow.Domain;
using CoreData = Xms.Core.Data;
using SchemaEntity = Xms.Schema.Domain;

namespace Xms.Flow.Core
{
    /// <summary>
    /// 工作流启动上下文
    /// </summary>
    public sealed class WorkFlowStartUpContext
    {
        public IUserContext User { get; set; }
        public Guid ApplicantId { get; set; }
        public Guid ObjectId { get; set; }
        public CoreData.Entity ObjectData { get; set; }
        public string Description { get; set; }
        public WorkFlow WorkFlowMetaData { get; set; }

        public SchemaEntity.Entity EntityMetaData { get; set; }
        public int Attachments { get; set; }
        public List<IFormFile> AttachmentFiles { get; set; }
    }
}