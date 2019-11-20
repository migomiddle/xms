using Xms.Infrastructure.Utility;
using Xms.QueryView.Abstractions.Component;

namespace Xms.QueryView
{
    /// <summary>
    /// 列表对象
    /// </summary>
    public class GridBuilder
    {
        private readonly Domain.QueryView _view;

        public GridBuilder(Domain.QueryView view)
        {
            _view = view;
            Grid = Grid.DeserializeFromJson(_view.LayoutConfig);
        }

        public GridDescriptor Grid { get; set; }
    }
}