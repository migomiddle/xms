!function (factory) {
    //factory是一个函数，下面的koExports就是他的参数

    // Support three module loading scenarios
    if (typeof require === 'function' && typeof exports === 'object' && typeof module === 'object') {
        // [1] CommonJS/Node.js
        // [1] 支持在module.exports.abc,或者直接exports.abc
        var target = module['exports'] || exports; // module.exports is for Node.js
        factory(target);
    } else if (typeof define === 'function' && define['amd']) {
        // [2] AMD anonymous module
        // [2] AMD 规范
        //define(['exports'],function(exports){
        //    exports.abc = function(){}
        //});
        define(['jquery'], factory);
    } else {
        // [3] No module loader (plain <script> tag) - put directly in global namespace
        factory(window['jQuery']);
    }
}(function ($) {
    "use strict"
    var isBindNoticEvent = false;
    var isSetPost = false;
    var notice_methods = {
        init: function () {
            getNoticeData(function (_html, count, res) { noticeLoaded(_html, count, res) });
        },
        bindEvent: function () {
            var orignalSetItem = localStorage.setItem;
            localStorage.setItem = function (key, newValue) {
                var setItemEvent = new Event("setNoticeChangeEvent");
                setItemEvent.key = key;
                setItemEvent.newValue = newValue;
                window.dispatchEvent(setItemEvent);
                orignalSetItem.apply(this, arguments);
            }
            addEventListener('setNoticeChangeEvent', noticeChangeEvent);
            addEventListener('storage', noticeChangeEvent);
        }
    }

    function noticeLoaded(_html, count, response) {
        if (count > 0) {
            $('#noticeCount').show();
            if (count > 99) {
                count = "" + 99 + '+';
            }
            $('#noticeCount').find('.tooltip-inner').text(count);
            // $('#noticeCenter').find('.noticeList').html(_html);
            $('#noticeCenterBtn').siblings('div').find('.popover-content').html(_html);

            $('#noticeCenterBtn').off().on('click', function () {
                var $this = $(this);
                if ($this.siblings('div').hasClass('in')) {
                    $this.siblings('div').removeClass('in');
                    $this.siblings('div').hide();
                } else {
                    $this.siblings('div').addClass('in').show();

                    getNoticeData(function (_html, count, response) {
                        noticeLoaded(_html, count, response);
                    });
                    if (isSetPost == true) return false;
                    isSetPost = true;
                    var noticeoffsetleft = -150;
                    if ($('.xms-index-ui').length > 0) {
                        noticeoffsetleft = -30;
                    }
                    setDomPosition($('#noticeCenterBtn'), $('#popoverNoticeBox'), { x: noticeoffsetleft });
                }
            });
            $('#mainChangeReaded').off('click').on('click', function (e) {
                e = e || window.event;
                if (e.preventDefault) e.preventDefault();
                Xms.Web.Post('/api/notice/allread', null, false, function (res) {
                    if (res && res.StatusName && res.StatusName == 'success') {
                        Xms.Web.Toast(res.Content, 'success', 1000);
                        noticeLoaded(_html, count, res);
                    }
                }, null, false, false);
            });
            if (isBindNoticEvent == true) return false;
            isBindNoticEvent = true;
            function hideNotice(e) {
                e = e || window.event;
                var target = e.target || e.srcElement;
                if ($(target).closest($('#noticeCenterBtn').parent()).length == 0) {
                    if ($('#noticeCenterBtn').siblings('div').length > 0) {
                        $('#noticeCenterBtn').siblings('div').removeClass('in');
                    }
                    if ($('#noticeCenterBtn').siblings('div').length > 0) {
                        $('#noticeCenterBtn').siblings('div').hide();
                    }
                }
            }
            $(document).off('click', hideNotice).on('click', hideNotice);
        } else {
            $('#noticeCount').hide();
            $('#noticeCenterBtn').siblings('div').find('.popover-content').html('');
        }
    }

    function noticeChangeEvent(e, a, b, c) {
        if (e.key == 'noticeChange') {
            getNoticeData(function (_html, count, response) {
                noticeLoaded(_html, count, response);//重新加载页面右上角的消息
                //以提示框方式提示有消息传入
                if (response.content && response.content.items.length > 0) {
                    var informationListStr = '<ul class="notice-list" style="width:259px;">';
                    var count = response.content.totalitems;
                    for (var i = 0; i < response.content.items.length; i++) {
                        Xms.Web.Notice({
                            title: '您有新的消息!',
                            body: '内容: ' + item.name + '',
                            data: item,
                            onclick: function () {
                                noticeRead(item.noticeid);
                            },
                            icon: '/content/imgs/new_notice.png'
                        });
                    }
                }
            });
        }
    }

    function setDomPosition(_target, _dom, offset) {
        var _targetPos = _target.position();
        var _targetBounds = { w: _target.outerWidth(), h: _target.outerHeight() };
        var _center = { x: _targetPos.left + _targetBounds.w / 2, y: _targetPos.top + 10 + _targetBounds.h / 2 };
        var _domBounds = { w: _dom.outerWidth(), h: _dom.outerHeight() };
        var _domOffset = _dom.offset();
        var winSize = { w: $(window).width(), h: $(window).height() };
        if (_domBounds.w + _domOffset.left > winSize.w) {
            _center.x = winSize.w - (_domBounds.w + _domOffset.left);
        }
        if (offset) {
            _dom.css({ "left": _center.x + (offset.x || 0), "top": _center.y + (offset.y || 0) });
        } else {
            _dom.css({ "left": _center.x, "top": _center.y });
        }
    }

    //消息通知模块
    function getNoticeData(callback) {
        var filter = {};
        filter.Filters = [{ "FilterOperator": Xms.Fetch.LogicalOperator.Or, "Conditions": [{ "AttributeName": "isread", "Operator": Xms.Fetch.ConditionOperator.Equal, "Values": [0] }] }];
        var queryObj = { EntityName: 'Notice', ColumnSet: { allcolumns: true } };
        queryObj.Criteria = filter;
        queryObj.Orders = [{ "AttributeName": "createdon", "OrderType": 1 }];
        var data = JSON.stringify({ "query": queryObj });
        Xms.Web.GetJson('/api/data/Retrieve/Multiple', data, function (response) {
            if (response.content && response.content.items && response.content.items.length > 0) {
                var informationListStr = '<ul class="notice-list" style="width:259px;">';
                var count = response.content.totalitems;
                for (var i = 0; i < response.content.items.length; i++) {
                    var item = response.content.items[i];
                    console.log("item", item);
                    informationListStr += '<li class="notice-list-item" title="' + item.name + '"><a target="_blank" href="javascript:void(0)" onclick="noticeRead(\'' + item.noticeid + '\')">'
                        + '<div><span class="glyphicon glyphicon-envelope"></span> '
                        + '<span>' + item.name + '</span></div>'
                        + '<div>' + item.createdon + '</div></a>'
                        + '</li>';
                    //count++;
                }
                callback && callback(informationListStr + '<li class="notice-list-item"><span style="margin: 10px 15px 10px 0;" class="btn btn-xs btn-primary" id="mainChangeReaded">标记为已读</span><a target="pageViewName" data-name="消息中心" onclick="takeToBottom(this)" data-id="page-moreNotice" title="消息中心" href="javascript:void(0)"  data-href= "' + ORG_SERVERURL + '/entity/list?entityname=notice" style="color:#555;float:right; margin:10px 15px 10px 0; text-align:right;"><div>更多</div></a></li></ul>'
                    + '<script>    '
                    + '    function noticeRead(noticeid) {'
                    + '        Xms.Web.Get(ORG_SERVERURL + "/api/notice/" + noticeid, function (response) {'
                    + '        if (response.IsSuccess) {'
                    + '                if (response.content.linkto) {'
                    + '                    Xms.Web.OpenWindow(ORG_SERVERURL + response.content.linkto);'
                    + '                }'
                    + '                else {'
                    + '                    Xms.Web.OpenWindow(ORG_SERVERURL + "/entity/create?entityname=notice&recordid=" + response.content.noticeid);'
                    + '                }'
                    + '            }'
                    + '        });'
                    + '    } '
                    + '</script > '
                    , count, response);
            }
        }, null, null, 'post');
    }

    function noticeRead(noticeid) {
        Xms.Web.Get(ORG_SERVERURL + "/api/notice/" + noticeid, function (response) {
            if (response.IsSuccess) {
                if (response.content.linkto) {
                    Xms.Web.OpenWindow(response.content.linkto);
                }
                else {
                    Xms.Web.OpenWindow(ORG_SERVERURL + "/entity/create?entityname=notice&recordid=" + response.content.noticeid);
                }
            }
        });
    }

    return notice_methods;
});