using System.Collections.Generic;
using System.Linq;
using Xms.Core.Data;
using Xms.Infrastructure.Utility;
using Xms.Schema.Extensions;

namespace Xms.Sdk.Client
{
    public static class DataHelper
    {
        public static List<dynamic> WrapOptionName(List<Schema.Domain.Attribute> attributes, List<dynamic> data)
        {
            var attributes_tmp = attributes.Where(n => n.TypeIsBit() || n.TypeIsState() || n.TypeIsPickList()).ToList();
            if (attributes_tmp.IsEmpty())
            {
                return data;
            }
            foreach (var attr in attributes_tmp)
            {
                if (attr.TypeIsPickList() || attr.TypeIsStatus())
                {
                    foreach (var d in data)
                    {
                        var _this = d as IDictionary<string, object>;
                        var line = _this.ToList();
                        var columnName = attr.Name.ToLower();
                        if (_this.ContainsKey(columnName + "name"))
                        {
                            break;
                        }
                        var kv = line.Find(n => n.Key.IsCaseInsensitiveEqual(columnName));
                        if (kv.Value != null && attr.OptionSet != null && attr.OptionSet.Items.NotEmpty())
                        {
                            var o = attr.OptionSet.Items.Find(n => n.Value == int.Parse(kv.Value.ToString()));
                            _this[columnName + "name"] = o != null ? o.Name : string.Empty;
                        }
                        else
                        {
                            _this[columnName + "name"] = string.Empty;
                        }
                    }
                }
                else if (attr.TypeIsState() || attr.TypeIsBit())
                {
                    foreach (var d in data)
                    {
                        var _this = d as IDictionary<string, object>;
                        var line = _this.ToList();
                        var columnName = attr.Name.ToLower();
                        if (_this.ContainsKey(columnName + "name"))
                        {
                            break;
                        }
                        var kv = line.Find(n => n.Key.IsCaseInsensitiveEqual(columnName));
                        if (kv.Value != null && attr.PickLists.NotEmpty())
                        {
                            var bVal = kv.Value.ToString().IsInteger() ? (int.Parse(kv.Value.ToString())) : (bool.Parse(kv.Value.ToString()) ? 1 : 0);
                            var o = attr.PickLists.Find(n => n.Value == bVal);
                            _this[columnName + "name"] = o != null ? o.Name : string.Empty;
                        }
                        else
                        {
                            _this[columnName + "name"] = string.Empty;
                        }
                    }
                }
            }
            return data;
        }

        public static Entity WrapOptionName(List<Schema.Domain.Attribute> attributes, Entity data)
        {
            if (data == null || data.Count == 0)
            {
                return data;
            }
            var attributes_tmp = attributes.Where(n => n.TypeIsBit() || n.TypeIsState() || n.TypeIsPickList()).ToList();
            if (attributes_tmp.IsEmpty())
            {
                return data;
            }
            foreach (var attr in attributes_tmp)
            {
                var columnName = attr.Name.ToLower();
                if (data.ContainsKey(columnName + "name") || !data.ContainsKey(columnName))
                {
                    continue;
                }
                var v = data[columnName];
                if (v == null)
                {
                    data[columnName + "name"] = string.Empty;
                }
                else
                {
                    if (attr.TypeIsPickList() || attr.TypeIsStatus())
                    {
                        if (attr.OptionSet != null && attr.OptionSet.Items.NotEmpty())
                        {
                            var o = attr.OptionSet.Items.Find(n => n.Value == int.Parse(v.ToString()));
                            data[columnName + "name"] = o != null ? o.Name : string.Empty;
                        }
                        else
                        {
                            data[columnName + "name"] = string.Empty;
                        }
                    }
                    else if (attr.TypeIsState() || attr.TypeIsBit())
                    {
                        if (attr.PickLists.NotEmpty())
                        {
                            var bVal = v.ToString().IsInteger() ? (int.Parse(v.ToString())) : (bool.Parse(v.ToString()) ? 1 : 0);
                            var o = attr.PickLists.Find(n => n.Value == bVal);
                            data[columnName + "name"] = o != null ? o.Name : string.Empty;
                        }
                        else
                        {
                            data[columnName + "name"] = string.Empty;
                        }
                    }
                }
            }
            return data;
        }
    }
}