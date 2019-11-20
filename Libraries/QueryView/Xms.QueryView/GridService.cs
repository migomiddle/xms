using System.Collections.Generic;
using Xms.Context;
using Xms.Infrastructure.Utility;
using Xms.QueryView.Abstractions.Component;

namespace Xms.QueryView
{
    /// <summary>
    /// 列表组件服务
    /// </summary>
    public class GridService : IGridService
    {
        private readonly IAppContext _appContext;

        public GridService(IAppContext appContext)
        {
            _appContext = appContext;
        }

        public GridDescriptor Build(Domain.QueryView view, List<Schema.Domain.Entity> entities, List<Schema.Domain.Attribute> attributes)
        {
            var gridBuilder = new GridBuilder(view);
            GridDescriptor grid = gridBuilder.Grid;
            int i = 0;
            foreach (var cell in grid.Rows[0].Cells)
            {
                if (cell.Label.IsEmpty())
                {
                    if (cell.EntityName.IsCaseInsensitiveEqual(view.EntityName))//主实体
                    {
                        var attr = attributes.Find(x => x.Name.IsCaseInsensitiveEqual(cell.Name) && x.EntityName.IsCaseInsensitiveEqual(view.EntityName));
                        if (attr != null)
                        {
                            grid.Rows[0].Cells[i].Label = attr.LocalizedName;
                        }
                    }
                    else //关联实体
                    {
                        //当列为关联实体字段时，列名加上关联实体名
                        var temp = cell.Name.SplitSafe(".");
                        if (temp.Length > 1)
                        {
                            var attr = attributes.Find(x => x.Name.IsCaseInsensitiveEqual(temp[1]) && x.EntityName.IsCaseInsensitiveEqual(cell.EntityName));
                            if (attr != null)
                            {
                                grid.Rows[0].Cells[i].Label = attr.LocalizedName + "(" + attr.EntityLocalizedName + ")";
                            }
                        }
                    }
                }
                i++;
            }

            return grid;
        }
    }
}