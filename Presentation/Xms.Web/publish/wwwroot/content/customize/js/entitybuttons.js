/**/
(function ($, root, un) {
    var entityButtons = {};
    var _prefix = 'entityBtn_';
    entityButtons.allbuttons = [];
    entityButtons.buttons = [];//在当前页面的可操作的按钮
    entityButtons.cacheDatas = [];
    entityButtons.datas = [];//已经存在的按钮
    entityButtons.delbuttons = [];//已经删除的按钮
    var setttings = {
        url: '',
        data: {},
        success: null,
        type: 'get',
        isSort: true,//是否根据displayorder排序
        context: $('#formButtons'),
        isDefaultRender: true,//是否使用默认的渲染方式,为false时，customRender必须为函数
        customRender: null
    }
    if ($('#EntityId').length > 0) {
        setttings.url = '/customize/RibbonButton/index?entityid=' + $('#EntityId').val();
    }
    function getBtnId(_id) {
        var _reg = new RegExp(_prefix, 'gm');
        return _id.replace(_reg, '');
    }
    var isLoaded = false;//是否已经从服务器获取按钮信息
    var _renderButtonList = function (datas, context, filter) {
        context = context || setttings.context;
        context.empty();
        //var _html = [];
        if (setttings.isSort == true) {
            datas = datas.sort(function (a, b) {
                return a.displayorder - b.displayorder;
            });
        }
        console.log(datas);
        var _html = $.map(datas, function (item, key) {
            var _id = (_prefix + item.ribbonbuttonid);
            return '<div class="entitybtn-item ' + item.cssclass + '" id="' + _id + '"><span class="' + item.icon + '"></span>' + item.label + '<span class="btn-del glyphicon glyphicon-remove" data-id="' + _id + '"></span></div>';
        });
        entityButtons.datas = $.map(datas, function (item, key) {
            return item.ribbonbuttonid;
        });
        if (entityButtons.datas.length == 0) {
            $('#CustomButtons').val('');
        } else {
            $('#CustomButtons').val(JSON.stringify(entityButtons.datas));
        }
        console.log('entityButtons.datas', entityButtons.datas);
        context.html(_html.join(''));
        bindEvent(context);
        if (entityButtons.delbuttons.length == 0) {
            var $delwrap = $('#delbtnList');
            $delwrap.hide();
        }
    }
    function bindEvent(context) {
        context.find('.btn-del').off().on('click', function () {
            var id = getBtnId($(this).attr('data-id'));

            delDatasbtn(entityButtons.buttons, id);
            $(this).off();
            _renderButtonList(entityButtons.buttons);
        });
    }
    function delDatasbtn(datas, id) {
        var index = getbtnIndex(datas, id);
        var btninfo = getBtnInfo(id)[0];
        addToDelList(btninfo);
        entityButtons.delbuttons.push(btninfo);
        datas.splice(index, 1);
    }
    function getbtnIndex(datas, id) {
        var index = -1;
        $.each(datas, function (key, item) {
            if (item.ribbonbuttonid == id) {
                index = key;
                return false;
            }
        });
        return index;
    }
    function addToDelList(btninfo) {
        var $delwrap = $('#delbtnList');
        $delwrap.show();
        var $dellist = $delwrap.children('ul');
        var $btn = $('<li><a><span class="glyphicon glyphicon-plus"></span>' + btninfo.label + '</a></li>');
        $dellist.append($btn);
        $btn.off().on('click', function () {
            addDatasbtn(entityButtons.buttons, btninfo.ribbonbuttonid);
            _renderButtonList(entityButtons.buttons);
            var _index = getbtnIndex(entityButtons.delbuttons, btninfo.ribbonbuttonid);
            entityButtons.delbuttons.splice(_index, 1);
            if (entityButtons.delbuttons.length == 0) {
                $delwrap.hide();
            }
            $btn.off(); $btn.remove();
        })
    }
    function addDatasbtn(datas, id) {
        datas.push(getBtnInfo(id)[0]);
    }
    function getBtnInfo(id) {
        return $.grep(entityButtons.allbuttons, function (item, key) {
            return item.ribbonbuttonid == id;
        });
    }
    function btnHandler(response) {
        console.log(response);
        if (response) {
            entityButtons.allbuttons = $.extend([], response.content.items);//当前实体表单中的所有按钮
        }
        if (setttings.isDefaultRender == true) {
            if (entityButtons.datas.length > 0) {//自定义保存过的按钮
                var _arr = $.grep(entityButtons.allbuttons, function (item, key) {
                    var _res = $.inArray(item.ribbonbuttonid, entityButtons.datas);
                    if (~_res) {
                        return true;
                    } else {
                        var _index = getbtnIndex(entityButtons.delbuttons, item.ribbonbuttonid);
                        if (!~_index) {
                            delDatasbtn(entityButtons.buttons, item.ribbonbuttonid);
                        }
                    }
                });//返回之前保存过的按钮
                entityButtons.cacheDatas = $.extend([], _arr);//把初始化时的自定义按钮缓存起来
                entityButtons.buttons = $.extend([], _arr);
            } else if (entityButtons.cacheDatas.length > 0) {
                var _arr = $.extend([], entityButtons.cacheDatas);
            } else {
                var _arr = $.extend([], entityButtons.allbuttons);
                entityButtons.cacheDatas = $.extend([], entityButtons.allbuttons);//把初始化时的自定义按钮缓存起来
                entityButtons.buttons = $.extend([], entityButtons.allbuttons);
            }
            _renderButtonList(_arr);
        } else if (setttings.isDefaultRender == false && typeof setttings.customRender === 'function') {
            setttings.customRender(entityButtons.allbuttons);
        }
        setttings.success && setttings.success(entityButtons.allbuttons);
    }
    entityButtons.clearRender = function (context) {
        context = context || setttings.context;
        context.empty();
    }
    entityButtons.getSettings = function (opts) {
        return setttings;
    }
    entityButtons.setSettings = function (opts) {
        setttings = $.extend({}, setttings, opts);
    }
    entityButtons.loadButtons = function () {
        if (setttings.type.toLowerCase() == 'get') {
            if (!isLoaded) {
                Xms.Web.GetJson(setttings.url, { showarea: 1 }, btnHandler, null, false, null, false);
                isLoaded = true;
            } else {
                btnHandler();
            }
        } else {
        }
    }

    root.entityButtons = entityButtons;
})(jQuery, window);