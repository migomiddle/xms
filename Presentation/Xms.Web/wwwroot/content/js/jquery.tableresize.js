/*
Writen by mlcactus, 2014-11-24
这是我封装的一个jquery插件，能够使table的各列可以左右拉伸，从而使宽度变小或变大
用法：$("#table_id").tableresize();
*/
(function ($) {
    $.fn.tableresize = function (options) {
        var defaults = {
            //当table的宽度到达默认最大值时，是否继续增大以至于出现横向滚动条
            resizeTable: true
        };
        var opts = $.extend(defaults, options);

        var _document = $("body");
        //设定user-select样式，防止内容被选中
        var set_user_select = function (jqueryobj, val) {
            jqueryobj.css("-moz-user-select", val).css("-webkit-user-select", val).css("-ms-user-select", val);
        };
        $(this).each(function () {
            if (!$.tableresize) {
                $.tableresize = {};
            }
            var _table = $(this);
            //设定ID
            var id = _table.attr("id") || "tableresize_" + (Math.random() * 100000).toFixed(0).toString();
            var tr = _table.find("tr").first(), ths = tr.children(), _firstth = ths.first();
            //设定临时变量存放对象
            var cobjs = $.tableresize[id] = {};
            cobjs._currentObj = null, cobjs._currentLeft = null;
            ths.mousemove(function (e) {
                var _this = $(this);
                var left = _this.offset().left, top = _this.offset().top, width = _this.outerWidth(), height = _this.outerHeight(), right = left + width, bottom = top + height, pageX = e.pageX, pageY = e.pageY;
                var leftside = !_firstth.is(_this) && Math.abs(left - pageX) <= 5, rightside = Math.abs(right - pageX) <= 5;
                if (cobjs._currentLeft || pageY > top && pageY < bottom && (leftside || rightside)) {
                    _document.css("cursor", "e-resize");
                    set_user_select(_table, "none");
                    if (!cobjs._currentLeft) {
                        if (leftside) {
                            cobjs._currentObj = _this.prev();
                        }
                        else {
                            cobjs._currentObj = _this;
                        }
                    }
                }
                else {
                    _document.css("cursor", "auto");
                    cobjs._currentObj = null;
                }
            });
            ths.mouseout(function (e) {
                if (!cobjs._currentLeft) {
                    cobjs._currentObj = null;
                    _document.css("cursor", "auto");
                    set_user_select(_table, "auto");
                }
            });
            _document.mousedown(function (e) {
                if (cobjs._currentObj) {
                    cobjs._currentLeft = e.pageX;
                }
                else {
                    cobjs._currentLeft = null;
                }
            });
            _document.mouseup(function (e) {
                if (cobjs._currentLeft) {
                    var changeWidth = e.pageX - cobjs._currentLeft;
                    cobjs._currentObj.width(cobjs._currentObj.width() + changeWidth);
                    if (opts.resizeTable) {
                        _table.width(_table.width() + changeWidth);
                    }
                }
                cobjs._currentObj = null;
                cobjs._currentLeft = null;
                _document.css("cursor", "auto");
                set_user_select(_table, "auto");
            });
        });
    };
})(jQuery);