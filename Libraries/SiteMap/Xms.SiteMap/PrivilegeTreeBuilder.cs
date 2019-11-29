using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Xms.Core.Context;
using Xms.Infrastructure.Utility;
using Xms.SiteMap.Domain;

namespace Xms.SiteMap
{
    /// <summary>
    /// 菜单树结构
    /// </summary>
    public class PrivilegeTreeBuilder : IPrivilegeTreeBuilder
    {
        private readonly IPrivilegeService _privilegeService;

        public PrivilegeTreeBuilder(IPrivilegeService privilegeService)
        {
            _privilegeService = privilegeService;
        }

        public List<Privilege> GetTreePath(string url)
        {
            if (url.IsNotEmpty())
            {
                Predicate<Privilege> filter = (n => n.Url.IsCaseInsensitiveEqual(url));
                return GetTreePathCore(filter);
            }
            return null;
        }

        public List<Privilege> GetTreePath(string areaName, string className, string methodName)
        {
            if (className.IsNotEmpty() && methodName.IsNotEmpty())
            {
                Predicate<Privilege> filter = (n => n.ClassName.IsCaseInsensitiveEqual(className) && n.MethodName.IsCaseInsensitiveEqual(methodName));
                if (areaName.IsNotEmpty())
                {
                    filter += (x => x.SystemName.IsCaseInsensitiveEqual(areaName));
                }
                return GetTreePathCore(filter);
            }
            return null;
        }

        private List<Privilege> GetTreePathCore(Predicate<Privilege> filter)
        {
            List<Privilege> result = new List<Privilege>();
            var all = _privilegeService.AllPrivileges;
            if (all == null)
            {
                return result;
            }
            var current = all.Find(filter);
            if (null != current)
            {
                var flag = current.Level > 1;
                result.Add(current);
                Guid parentid = current.ParentPrivilegeId;
                while (flag)
                {
                    var parent = all.Find(n => n.PrivilegeId == parentid);
                    if (parent != null)
                    {
                        result.Add(parent);
                        parentid = parent.ParentPrivilegeId;
                        if (parent.Level <= 1)
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                }
                result.Reverse();
            }

            return result;
        }

        #region json相关

        public string Build(Func<QueryDescriptor<Privilege>, QueryDescriptor<Privilege>> container, bool nameLower = true)
        {
            List<Privilege> list = _privilegeService.Query(container);

            List<dynamic> dlist = Build(list, Guid.Empty);
            dynamic contact = new ExpandoObject();
            contact.label = "root";
            contact.id = Guid.Empty;
            contact.children = dlist;

            List<dynamic> results = new List<dynamic>();
            results.Add(contact);

            var json = results.SerializeToJson(nameLower);
            return json;
        }

        public List<dynamic> Build(List<Privilege> privilegeList, Guid parentId)
        {
            List<dynamic> dynamicList = new List<dynamic>();
            List<Privilege> childList = privilegeList.Where(n => n.ParentPrivilegeId == parentId).OrderBy(n => n.DisplayOrder).ToList();
            if (childList != null && childList.Count > 0)
            {
                List<dynamic> ddList = new List<dynamic>();
                dynamic contact = new ExpandoObject();
                foreach (var item in childList)
                {
                    contact = new ExpandoObject();
                    contact.label = item.DisplayName;
                    contact.id = item.PrivilegeId;
                    contact.url = item.Url;
                    contact.smallicon = item.SmallIcon;
                    contact.opentarget = item.OpenTarget;
                    if (privilegeList.Find(n => n.ParentPrivilegeId == item.PrivilegeId) != null)
                    {
                        ddList = Build(privilegeList, item.PrivilegeId);
                        if (ddList.Count > 0)
                        {
                            contact.children = ddList;
                            ddList = new List<dynamic>();
                        }
                    }
                    dynamicList.Add(contact);
                }
            }
            return dynamicList;
        }

        #endregion json相关
    }
}