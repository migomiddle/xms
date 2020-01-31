window.onerror = function () {
    $('#loader').hide();
}
if (typeof (Xms) == "undefined") { Xms = { __namespace: true }; }
//Xms.Web = function () { };
Xms.Web = {
    Loader: function (_context) {
        var self = this;
        self.loader = jQuery('<div id="loader" class="ajax-backdrop fade in"><div class="ajax-loading">' + LOC_DATA_LOADING + '</div></div>')
            .appendTo(_context)
            .hide();
        jQuery(document).ajaxStart(function () {
            self.loader.show();
        }).ajaxStop(function () {
            self.loader.hide();
        }).ajaxError(function (a, b, e) {
            self.loader.hide();
            Xms.Web.Toast(LOC_ERROR, false, false);
            console.log(a, b, e);
        });
    }
    , Loading: function () {
        var loader = jQuery('<div id="loader" class="ajax-backdrop fade in"><div class="ajax-loading">' + LOC_DATA_LOADING + '</div></div>')
            .appendTo("body")
            .hide();
        jQuery(document).ajaxStart(function () {
            loader.show();
        }).ajaxStop(function () {
            loader.hide();
        }).ajaxError(function (a, b, e) {
            loader.hide();
            //Xms.Web.Alert(false, '发生了错误');
            //$.messager.popup("出错了");
            Xms.Web.Toast(LOC_ERROR, false, false);
            //Xms.Web.ErrorHandler(a, b, e);

            console.log(a, b, e);
        });
    }
    //error handler

    , ErrorHandler: function (xhr, textStatus, errorThrown) {
        console.log('error', xhr);
        var data = JSON.parse(xhr.responseText);
        var err = data.error ? data.error.message.value : (data.errormessage ? data.errormessage : xhr.responseText);
        var msg = "Error : " +
            xhr.status + ": " +
            xhr.statusText + ": " +
            err;
        //$.messager.popup("出错了");
        Xms.Web.Alert(false, LOC_ERROR + ": " + msg);
    }
    //发送POST请求
    , Post: function (url, data, refresh, onsuccess, onerror, __async, istip, extopts) {
        var args = [].slice.call(arguments);
        if (args.length == 1 && typeof args[0] === 'object') {
            url = args[0].url;
            data = args[0].data;
            onsuccess = args[0].onsuccess;
            onerror = args[0].onerror;
            __async = args[0].async;
            istip = args[0].istip;
            refresh = args[0].refresh;
        }
        if ((!extopts || !extopts.jsonajax) && ~url.indexOf('?')) {
            var model = {};
            model = $.urlParamObj(url);//如果请求方法中带有查询参数，提取出参数放到一个对象中
            url = Xms.Web.getDataUrlApi(url);
            url = url + (url.indexOf('?') == -1 ? '?' : '&') + '__r=' + new Date().getTime();
            data = $.extend({}, model, data);
        }
        var headers = {};
        if (data.__RequestVerificationToken) {
            headers["__RequestVerificationToken"] = data.__RequestVerificationToken;
            delete data['__RequestVerificationToken']
        }
        if (typeof (data) == "object") {
            data = JSON.stringify(data);
        }
        //...
        //是否异步
        var isAsync = true;
        if (typeof __async != 'undefined') isAsync = __async;
        var dtd = $.Deferred();

        if (url.indexOf(ORG_SERVERURL) !== 0) {
            url = ORG_SERVERURL + (url.indexOf('/') == 0 ? "" : "/") + url;
        }
        $.ajax({
            type: "POST",
            url: url,
            data: data,
            //dataType: "json",
            contentType: "application/json; charset=utf-8",
            cache: false,
            headers: headers,
            async: isAsync,
            success: function (response) {
                console.log(url, response);
                //$.messager.popup(response.Content);
                //Xms.Web.Alert(response.IsSuccess, response.Content);
                if (typeof istip == 'undefined' || istip == true) {
                    Xms.Web.Toast(response.Content, response.IsSuccess);
                }
                if (refresh == undefined || refresh == true) {
                    setTimeout("location.reload()", 1000);
                }
                if (typeof (onsuccess) == "function") {
                    onsuccess.call(this, response);//.apply(this, response);
                }
                dtd.resolve(response);
            }
            , error: function (xhr, textStatus, errorThrown) {
                Xms.Web.ErrorHandler(xhr, textStatus, errorThrown);
                if (typeof (onerror) == "function") {
                    onerror.call(this, xhr);
                }
                dtd.reject(xhr, textStatus, errorThrown);
            }
        });
        return dtd.promise();
    },
    changeArrayToUrlParam: function (url, data) {
        function setRepeatUrl(_url, datas, keyname) {
            if (datas && datas.length > 0) {
                var isadd = false
                $.each(datas, function (i, n) {
                    if (~_url.indexOf('?')) {
                        _url = _url + '&' + keyname + '=' + n;
                    } else {
                        if (isadd == false) {
                            isadd = true;
                            _url = _url + '?' + keyname + '=' + n;
                        } else {
                            _url = _url + '&' + keyname + '=' + n;
                        }
                    }
                });
            }
            return _url;
        }
        function deepHandler(data, url) {
            if (data) {
                for (var i in data) {
                    if (data.hasOwnProperty(i)) {
                        if (typeof data[i] == 'object') {
                            if (Object.prototype.toString.call(data[i]) == '[object Array]') {
                                var temp = data[i];
                                var tempname = i;
                                url = setRepeatUrl(url, temp, tempname);
                                delete data[i]
                            } else {
                                //Xms.Web.changeArrayToUrlParam(url,data);
                            }
                        }
                    }
                }
                return url;
            }
        }
        var res = { url: url, data: data };
        var _url = deepHandler(data, url);
        if (url) {
            res.url = _url;
        }
        return res;
    }
    , GetJson: function (url, data, onsuccess, onerror, __async, type, issyn) {
        var args = [].slice.call(arguments);
        if (args.length == 1 && typeof args[0] === 'object') {
            url = args[0].url;
            data = args[0].data;
            onsuccess = args[0].onsuccess;
            onerror = args[0].onerror;
            __async = args[0].async;
            type = args[0].type;
            issyn = args[0].issyn;
        }

        //是否异步
        var isAsync = __async || true;
        if (issyn) {
            isAsync = false;
        }
        type = type || 'GET';
        var dtd = $.Deferred();
        if (url.indexOf(ORG_SERVERURL) !== 0) {
            url = ORG_SERVERURL + (url.indexOf('/') == 0 ? "" : "/") + url;
        }
        if (~url.indexOf('?')) {
            var model = {};
            model = $.urlParamObj(url);//如果请求方法中带有查询参数，提取出参数放到一个对象中
            url = Xms.Web.getDataUrlApi(url);
            url = url + (url.indexOf('?') == -1 ? '?' : '&') + '__r=' + new Date().getTime();
            data = $.extend({}, model, data);
        }
        var res = Xms.Web.changeArrayToUrlParam(url, data);//处理数据中有数据的情况，需要另外处理
        if (res && res.url) {
            url = res.url;
            data = res.data;
        }
        $.ajax({
            type: type,
            url: url,
            data: data,
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            cache: false,
            async: isAsync,
            success: function (response) {
                if (typeof (onsuccess) == "function") {
                    var result = (response.statusName == '404' ? response : Xms.Web.GetAjaxResult(response, args));
                    onsuccess.call(this, result);
                }
                dtd.resolve(response);
            }
            , error: function (xhr, textStatus, errorThrown) {
                Xms.Web.ErrorHandler(xhr, textStatus, errorThrown);
                if (typeof (onerror) == "function") {
                    onerror.call(this, xhr);
                }
                dtd.reject(xhr, textStatus, errorThrown);
            }
        });
        return dtd.promise();
    }
    , SyncGetJson: function (url, data, onsuccess, onerror, type) {
        this.getjson(url, data, onsuccess, onerror, null, type, true);
    }
    , Get: function (url, onsuccess, onerror) {
        var args = [].slice.call(arguments);
        if (args.length == 1 && typeof args[0] === 'object') {
            url = args[0].url;
            onsuccess = args[0].onsuccess;
            onerror = args[0].onerror;
        }
        var dtd = $.Deferred();
        if (url.indexOf(ORG_SERVERURL) !== 0) {
            url = ORG_SERVERURL + (url.indexOf('/') == 0 ? "" : "/") + url;
        }
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            cache: false,
            success: function (response) {
                console.log(url, response);
                if (typeof (onsuccess) == "function") {
                    var result = response;
                    if (typeof (response.Content) != 'undefined') {
                        if (typeof (response.Content) == 'Object' || response.Content.indexOf('{') == 0)
                            result = Xms.Web.GetAjaxResult(response, args);
                    }
                    onsuccess.call(this, result);
                }
                dtd.resolve(response);
            }
            , error: function (xhr, textStatus, errorThrown) {
                Xms.Web.ErrorHandler(xhr, textStatus, errorThrown);
                if (typeof (onerror) == "function") {
                    onerror.call(this, xhr);
                }
                dtd.reject(xhr, textStatus, errorThrown);
            }
        });
        return dtd.promise();
    }
    , AsyncGet: function (url, onsuccess, onerror) {//同步请求
        var args = [].slice.call(arguments);
        if (args.length == 1 && typeof args[0] === 'object') {
            url = args[0].url;
            onsuccess = args[0].onsuccess;
            onerror = args[0].onerror;
        }
        var dtd = $.Deferred();
        if (url.indexOf(ORG_SERVERURL) !== 0) {
            url = ORG_SERVERURL + (url.indexOf('/') == 0 ? "" : "/") + url;
        }
        $.ajax({
            type: "GET",
            url: url,
            contentType: "application/json; charset=utf-8",
            cache: false,
            async: false,
            success: function (response) {
                console.log(url, response);
                if (typeof (onsuccess) == "function") {
                    var result = response;
                    if (typeof (response.Content) != 'undefined') {
                        if (typeof (response.Content) == 'Object' || response.Content.indexOf('{') == 0)
                            result = Xms.Web.GetAjaxResult(response, args);
                    }
                    onsuccess.call(this, result);
                }
                dtd.resolve(response);
            }
            , error: function (xhr, textStatus, errorThrown) {
                Xms.Web.ErrorHandler(xhr, textStatus, errorThrown);
                if (typeof (onerror) == "function") {
                    onerror.call(this, xhr);
                }
                dtd.reject(xhr, textStatus, errorThrown);
            }
        });
        return dtd.promise();
    }
    , Load: function (url, onsuccess, onerror, extopts) {
        var args = [].slice.call(arguments);
        if (args.length == 1 && typeof args[0] === 'object') {
            url = args[0].url;
            onsuccess = args[0].onsuccess;
            onerror = args[0].onerror;
        }
        var data = {};
        var dtd = $.Deferred();
        if (url.indexOf(ORG_SERVERURL) !== 0) {
            url = ORG_SERVERURL + (url.indexOf('/') == 0 ? "" : "/") + url;
        }
        if ((!extopts || !extopts.jsonajax) && ~url.indexOf('?')) {
            var model = {};
            model = $.urlParamObj(url);//如果请求方法中带有查询参数，提取出参数放到一个对象中
            url = Xms.Web.getDataUrlApi(url);
            url = url + (url.indexOf('?') == -1 ? '?' : '&') + '__r=' + new Date().getTime();
            data = $.extend({}, model, data);
        }
        data = JSON.stringify(data);
        $.ajax({
            type: "Post",
            data: data,
            url: url,
            contentType: "application/json; charset=utf-8",
            cache: false,
            success: function (response) {
                if (typeof (onsuccess) == "function") {
                    onsuccess.call(this, response);
                }
                dtd.resolve(response);
            }
            , error: function (xhr, textStatus, errorThrown) {
                Xms.Web.ErrorHandler(xhr, textStatus, errorThrown);
                if (typeof (onerror) == "function") {
                    onerror.call(this, xhr);
                }
                dtd.reject(xhr, textStatus, errorThrown);
            }
        });
        return dtd.promise();
    }
    , LoadPage: function (url, data, onsuccess, onerror, callback, queryUrl) {
        var args = [].slice.call(arguments);
        if (args.length == 1 && typeof args[0] === 'object') {
            url = args[0].url;
            data = args[0].data;
            onsuccess = args[0].onsuccess;
            onerror = args[0].onerror;
            callback = args[0].callback;
        }
        if (!queryUrl && ~url.indexOf('?')) {
            var model = {};
            model = $.urlParamObj(url);//如果请求方法中带有查询参数，提取出参数放到一个对象中
            url = Xms.Web.getDataUrlApi(url);
            url = url + (url.indexOf('?') == -1 ? '?' : '&') + '__r=' + new Date().getTime();
            data = $.extend({}, model, data);
        }
        if (data && typeof (data) == "object") {
            data = JSON.stringify(data);
        }
        var type = data ? "POST" : "GET";
        var dtd = $.Deferred();
        if (url.indexOf(ORG_SERVERURL) !== 0) {
            url = ORG_SERVERURL + (url.indexOf('/') == 0 ? "" : "/") + url;
        }
        $.ajax({
            type: type,
            url: url,
            data: data,
            dataType: "text",
            contentType: "application/json; charset=utf-8",
            cache: false,
            success: function (response) {
                if (typeof (onsuccess) == "function") {
                    onsuccess.call(this, response);
                    callback && callback(response);
                }
                dtd.resolve(response);
            }
            , error: function (xhr, textStatus, errorThrown) {
                Xms.Web.ErrorHandler(xhr, textStatus, errorThrown);
                if (typeof (onerror) == "function") {
                    onerror.call(this, xhr);
                }
                dtd.reject(xhr, textStatus, errorThrown);
            }
        });
        return dtd.promise();
    }
    //获取服务器返回的数据
    , GetAjaxResult: function (response, resource) {
        var obj = {};
        obj.content = null;
        if (response) {
            obj = response;
            if (typeof (response.Content) == 'object') { obj.content = response.Content; }
            else {
                try {
                    if (response.Content && (~response.Content.indexOf('{') || ~response.Content.indexOf('['))) {
                        obj.content = JSON.parse(response.Content);
                    }
                } catch (e) {
                    console.error(e, resource);//方便跟踪错误来源
                    console.error('数据返回错误：' + response.Content);
                    obj.content = response.Content;
                }
            }
        }
        return obj;
    }
    //初始化数据列表
    , DataTable: function (target, opts) {
        var hasRow = target.find('tbody > tr').length > 0;
        var sortby = target.attr("data-sortby");
        var sortdirect = target.attr("data-sortdirection");
        var pageurl = target.attr("data-pageurl") || location.href;
        opts = $.extend({}, {
            checkHandler: null,
            unCheckHandler: null,
            changeHandler: null
        }, opts || {})
        //排序链接
        target.find("thead th[data-name]").each(function (i, n) {
            var self = $(n);
            var url = $.setUrlParam(pageurl, 'sortdirection', sortdirect == 1 ? 0 : 1);
            url = $.setUrlParam(url, 'sortby', self.attr('data-name'));
            if (self.attr('data-label')) {
                if (hasRow) {
                    self.find('a[data-ajax="true"]').remove();
                    self.prepend('<a href="' + (url) + '" data-ajax="true">' + self.attr('data-label') + '</a>');
                }
                else {
                    self.html(self.attr('data-label'));
                }
            }
            else {
                if (hasRow)
                    self.html('<a href="' + (url) + '" data-ajax="true">' + self.text() + '</a>');
                else
                    self.html(self.text());
            }
        });
        if (hasRow == false) {
            target.find('tbody').append('<tr class="bg-warning emptyrow"><td colspan="' + target.find('thead > tr > th').length + '">' + LOC_LIST_NODATA + '</td></tr>');
            target.find('thead > tr > th').find('a').prop('href', 'javascript:void(0)');
            target.find('tfoot').hide();
            return;
        }
        if (target.attr('data-datatableinit') && target.attr('data-datatableinit') == 'true') {
            return;
        }
        target.attr('data-datatableinit', 'true');
        //列表排序样式
        var current = target.find("th[data-name='" + sortby + "']");
        current.addClass("success");
        if (sortdirect == 0) {
            current.find("a[data-ajax]").append('<span class="glyphicon glyphicon-sort-by-attributes"></span>');
        }
        else {
            current.find("a[data-ajax]").append('<span class="glyphicon glyphicon-sort-by-attributes-alt"></span>');
        }
        //刷新按钮
        var refreshMethod = target.attr('data-refresh');
        if (refreshMethod)
            target.find("thead th:last").append('<button type="button" title="' + LOC_REFRESH + '" class="btn btn-default btn-xs pull-right" onclick="' + refreshMethod + '"><span class="glyphicon glyphicon-refresh"></span></button>');
        //全选、反选
        var singlemode = target.attr('data-singlemode') == 'True';
        if (!singlemode) {
            target.find("input[name=checkall]:not(:disabled)").on('click', null, function () {
                var flag = $(this).prop("checked");
                if (flag) {
                    target.find("input[name=recordid]:not(:disabled)").prop("checked", true);
                    //opts.unCheckHandler && opts.unCheckHandler.call(this, flag, target);
                    target.find("tbody > tr").addClass("active");
                }
                else {
                    target.find("input[name=recordid]:not(:disabled)").removeProp("checked");
                    //opts.checkHandler && opts.checkHandler.call(this, flag, target);
                    target.find("tbody > tr").removeClass("active");
                }
                opts.changeHandler && opts.changeHandler(this, 'all', target);
            });
        }
        else {
            target.find("input[name=checkall]").prop('disabled', true);
        }
        //行单击时选中
        var isDblClick = false, fixtime = 300, timeouts = [], fixedCount = 50;
        target.off('click').on('click', "tbody tr", function (e) {
            timeouts.push(new Date() * 1);
            if (timeouts.length >= 2 && timeouts.length < fixedCount) {
                if (timeouts[timeouts.length - 1] - timeouts[timeouts.length - 2] <= fixtime) {
                    isDblClick = true;
                    timeouts = [];
                } else {
                    isDblClick = false;
                }
            } else if (timeouts.length >= fixedCount) {
                isDblClick = false;
                timeouts = [];
            } else {
                isDblClick = false;
            }
            console.log('isDblClick', isDblClick);
            var $target = $(e.target);
            var ischeckbox = $target.is('input') && $target.prop('type') == 'checkbox';
            //if (($target.is('input') || $target.is('select')) && !ischeckbox) {
            //    return;
            //}
            if (($target.is('select')) || ($target.is('input') && !ischeckbox)) {
                return;
            }
            if (!isDblClick) {
                var flag = $(this).find("input[name=recordid]:not(:disabled)").prop("checked");
                if (singlemode) {
                    target.find("input[name=recordid]:not(:disabled)").removeProp("checked");
                    //opts.unCheckHandler && opts.unCheckHandler(this, flag, target);
                    target.find("tbody > tr").removeClass("active");
                }

                if (flag) {
                    if (ischeckbox) {
                        $(this).addClass("active");
                        //opts.checkHandler && opts.checkHandler(this, flag, target);
                        $target.prop("checked", true);
                    }
                    else {
                        $(this).find("input[name=recordid]:not(:disabled)").removeProp("checked");
                        //opts.unCheckHandler && opts.unCheckHandler($(this).find("input[name=recordid]:not(:disabled)"), flag, target);
                        $(this).removeClass("active");
                    }
                }
                else {
                    if (ischeckbox) {
                        $(this).removeClass("active");
                        //opts.unCheckHandler && opts.unCheckHandler(this, flag, target);
                        $target.removeProp("checked");
                    }
                    else {
                        $(this).find("input[name=recordid]:not(:disabled)").prop("checked", true);
                        //opts.checkHandler && opts.checkHandler($(this).find("input[name=recordid]:not(:disabled)"), flag, target);
                        $(this).addClass("active");
                    }
                }
                $(this).trigger('tr.click', { flag: flag, target: $target });
            } else if (isDblClick) {
                var d = $target.is('tr') ? $target : $target.parents('tr');
                if (d.attr('data-dbclick')) {
                    var func = getFunction(d.attr('data-dbclick'));
                    func && func(d, this);
                }
            }
            opts.changeHandler && opts.changeHandler(this, 'item', target);
        });
        //批量删除
        target.find("button[data-role=delete]").click(function () {
            var action = $(this).attr("data-action");
            var datas = Xms.Web.GetTableSelected(target);
            if (datas.length == 0) {
                //$.messager.popup("请选择要删除的记录");
                //Xms.Web.Alert(false, '请选择要删除的记录');
                Xms.Web.Toast(LOC_NOTSPECIFIED_RECORD, false);
            } else if (action == "") {
                //$.messager.popup("提交地址不能为空");
                //Xms.Web.Alert(false, '提交地址不能为空');
                Xms.Web.Toast(LOC_URL_EMPTY, false);
            }
            else {
                confirmtext = "" + ($(this).attr("data-tooltip") ? $(this).attr("data-tooltip") : LOC_CONFIRM_DELETE_TITLE);
                var isconfirm = false;

                var model = { recordid: datas };

                var one = function () {
                    var dfd = $.Deferred();
                    Xms.Web.Confirm(LOC_CONFIRM_DELETE_TITLE, confirmtext, function () {
                        isconfirm = true;
                        dfd.resolve();
                    });
                    return dfd.promise();
                };
                $.when(one()).done(function () {
                    var dfd = $.Deferred();
                    isconfirm = false;
                    Xms.Web.Confirm(LOC_CONFIRM2_DELETE_TITLE, "<strong>" + LOC_CONFIRM2_TITLE + "：</strong>" + confirmtext, function () {
                        isconfirm = true;
                        Xms.Web.Post(action, model, false, function (response) {
                            if (refreshMethod) {
                                eval(refreshMethod);
                            }
                        });
                        dfd.resolve();
                    });
                    return dfd.promise();
                }
                );
            }
        });
        //批量更新
        target.find("[data-role=update]").click(function () {
            var action = $(this).attr("data-action");//批量操作
            var datas = Xms.Web.GetTableSelected(target);
            if (datas.length == 0) {
                Xms.Web.Toast(LOC_NOTSPECIFIED_RECORD, false);
            } else if (action == "") {
                Xms.Web.Toast(LOC_URL_EMPTY, false);
            }
            else {
                var model = { recordid: datas };
                Xms.Web.Post(action, model, false, function (response) {
                    if (refreshMethod) {
                        eval(refreshMethod);
                    }
                });
            }
        });
        return target;
    }
    //设置表格行选中
    , TableSelected: function (container, selector, onselected, unselected) {
        if (selector == undefined) selector = 'tbody tr';
        $(container).off('click').on('click', selector, function (e) {
            var $target = $(e.target);
            var ischeckbox = $target.is('input') && $target.prop('type') == 'checkbox';
            if (!$target.is('td') && !ischeckbox) {//(($target.is('input') || $target.is('select') || $target.is('a')) && !ischeckbox) {
                return;
            }
            var selected = false;
            var checkedflag = $(this).find(":checkbox").prop("checked");
            if (checkedflag) {
                if (ischeckbox) {
                    $(this).addClass("active");
                    selected = true;
                }
                else {
                    $(this).find(":checkbox").removeProp("checked");
                    $(this).removeClass("active");
                }
            }
            else {
                if (ischeckbox) {
                    $(this).removeClass("active");
                }
                else {
                    $(this).find(":checkbox").prop("checked", true);
                    $(this).addClass("active");
                    selected = true;
                }
            }
            if (typeof (onselected) == 'function' && selected == true) {
                onselected($(this));
            }
            if (typeof (unselected) == 'function' && selected == false) {
                unselected($(this));
            }
        });
    }
    //获取列表选中的记录ID
    , GetTableSelected: function (target, tostring) {
        var result = new Array();
        console.log('target.find("input[name=recordid]:checked")', target.find("input[name=recordid]:checked"))
        target.find("input[name=recordid]:checked").each(function (i, n) {
            result.push($(n).val());
        });

        return tostring == true ? JSON.stringify(result) : result;
    }
    //移除列表选中的记录行
    , RemoveTableSelected: function (target) {
        target.find("input[name=recordid]:checked").each(function (i, n) {
            $(n).parents('tr:first').remove();
        });
    }
    //选中一行
    , SelectingRow: function (e, istoggle, unselectother) {
        var row = $(e).parents('tr:first');
        console.log(row);
        if (istoggle) {
            var flag = row.find("input[name=recordid]").prop("checked");
            if (flag) {
                row.removeClass('active');
                row.find("input[name=recordid]").removeProp("checked");
            }
            else {
                row.addClass('active');
                row.find("input[name=recordid]").prop("checked", true);
            }
        }
        else {
            row.addClass('active');
            row.find("input[name=recordid]").prop("checked", true);
        }
        if (unselectother == true) {
            row.siblings().find("input[name=recordid]").removeProp("checked");
        }
    }
    //获取选中行的ID
    , GetSelectingRowRecordId: function (e, istoggle, unselectother) {
        Xms.Web.SelectingRow(e, istoggle, unselectother);
        var row = $(e).parents('tr:first'), res = [];
        var id = row.find("input[name=recordid]").val();
        if (id) {
            res.push(id);
        }
        return res;
    }
    //删除一条记录
    , Del: function (id, action, refresh, onsuccess, onerror, confirmtext, isconfirmagain, confirmext) {
        if (!id || id == '' || id.length == 0) {
            Xms.Web.Toast(LOC_NOTSPECIFIED_RECORD, false);
            return;
        }
        if (!Xms.Utility.IsArray(id)) {
            id = [id];
        }
        if (!confirmtext || confirmtext == undefined || confirmtext == '') confirmtext = LOC_CONFIRM_DELETE_TITLE;
        else confirmtext = LOC_CONFIRM_OPERATION_TITLE + confirmtext;
        var isconfirm = false;

        var one = function () {
            var dfd = $.Deferred();
            Xms.Web.Confirm(LOC_CONFIRM_OPERATION_TITLE, confirmtext, function () {
                isconfirm = true;
                dfd.resolve();
            }, null, confirmext);
            return dfd.promise();
        };
        $.when(one()).done(function () {
            var dfd = $.Deferred();
            if (isconfirmagain != undefined && isconfirmagain == true) {
                isconfirm = false;
                Xms.Web.Confirm(LOC_CONFIRM2_DELETE_TITLE, "<strong>" + LOC_CONFIRM2_TITLE + "：</strong>" + confirmtext, function () {
                    isconfirm = true;
                    var postdata = { recordid: id };
                    if (confirmext && confirmext.postdata) {
                        $.extend(postdata, { recordid: id }, confirmext.postdata);
                    }
                    Xms.Web.Post(action, postdata, refresh, function (res) {
                        console.log(res);
                        if (res && res.Content) {
                            if (res.IsSuccess) {
                                Xms.Web.Toast(res.Content, true, 1000);
                            } else {
                                Xms.Web.Toast(res.Content, false, 1000);
                            }
                            onsuccess && onsuccess(res);
                        }
                        else {
                            $('body').append($(res));
                        }
                    }, onerror, null, false);
                    dfd.resolve();
                });
            }
            else if (isconfirm == true) {
                var postdata = { recordid: id };
                if (confirmext && confirmext.postdata) {
                    $.extend(postdata, { recordid: id }, confirmext.postdata);
                }
                Xms.Web.Post(action, postdata, refresh, function (res) {
                    if (res && res.Content) {
                        if (res.IsSuccess) {
                            Xms.Web.Toast(res.Content, true, 1000);
                        } else {
                            Xms.Web.Toast(res.Content, false, 1000);
                        }
                        onsuccess && onsuccess(res);
                    }
                    else {
                        $('body').append($(res));
                    }
                }, onerror, null, false);
            }
            return dfd.promise();
        }
        );
    }
    //弹出确认框
    //extoptions:{
    //showTip, 为false不显示原先的提示信息
    //content, 要添加进去的内容
    //checkClose, //点击关闭时触发，返回false不关闭窗口
    //checkOk//点击确定时触发，返回false不关闭窗口
    //}
    , Confirm: function (title, text, onOk, onCancel, extoptions) {
        var target = $("#confirmDialog");
        if (target.length > 0) {
            target.remove();
        }
        if (!title || title == undefined || title == '') title = LOC_CONFIRM_OPERATION_TITLE;
        if (!text || text == undefined || text == '') text = LOC_CONFIRM_OPERATION_TITLE;

        var $dialog = $('<div id="confirmDialog" class="hide"></div>');
        var $dialog_header = $('<div class="confirm-dialog-header"></div>');
        var $dialog_content = $('<div class="confirm-dialog-content"></div>');
        var $dialog_footer = $('<div class="confirm-dialog-footer"></div>');
        var $dialog_tips = $('<p class="text-danger"><span class=\"glyphicon glyphicon-info-sign\"></span> <strong>' + text + '</strong></p>')
        $("body").append($dialog);
        $dialog.append($dialog_header).append($dialog_content).append($dialog_footer);
        $dialog_header.append($dialog_tips);
        if (extoptions && extoptions.showTip === false) {
            $dialog_tips.remove();
        }
        if (extoptions && extoptions.content) {
            if (typeof extoptions.content == 'string') {
                $dialog_content.html(extoptions.content);
            } else {
                try {
                    $dialog_content.append(extoptions.content);
                } catch (e) {
                    console.error(e);
                }
            }
        }

        target = $("#confirmDialog");

        target.dialog({
            title: '<span class="glyphicon glyphicon-warning-sign"></span> ' + title
            , onClose: function () {
                if (typeof (onCancel) == "function") {
                    onCancel.call(this);
                }
                $(this).dialog("destroy");
                //target.remove();
                target.parents('.modal').next().remove();
                target.parents('.modal').remove();
                $(document.body).removeClass("modal-open");
            }
            , buttons: [
                {
                    text: "<span class=\"glyphicon glyphicon-remove\"></span> " + LOC_DIALOG_CLOSE,
                    classed: "btn-default",
                    click: function () {
                        if (extoptions && typeof extoptions.checkClose == 'function') {
                            var flag = extoptions.checkClose.call(this, extoptions);
                            if (flag === false) {
                                return false;
                            }
                        }
                        $(this).dialog("destroy");
                        //target.remove();
                        target.parents('.modal').next().remove();
                        target.parents('.modal').remove();
                        $(document.body).removeClass("modal-open");
                        $(document.body).css("padding-right", 0);
                        if (typeof (onCancel) == "function") {
                            onCancel.call(this);
                        }
                    }
                }
                , {
                    text: "<span class=\"glyphicon glyphicon-ok\"></span> " + LOC_DIALOG_OK,
                    classed: "btn-info",
                    click: function () {
                        if (extoptions && typeof extoptions.checkOk == 'function') {
                            var flag = extoptions.checkOk.call(this, extoptions);
                            if (flag === false) {
                                return false;
                            }
                        }
                        $(this).dialog("destroy");
                        //target.remove();
                        target.parents('.modal').next().remove();
                        target.parents('.modal').remove();
                        $(document.body).removeClass("modal-open");
                        $(document.body).css("padding-right", 0);
                        if (typeof (onOk) == "function") {
                            onOk.call(this);
                        }
                    }
                }
            ]
        }).removeClass("hide");
        //dragable
        Xms.Web.Draggable();
    }
    //顶部弹出信息提示
    , Toptip: function (msg) {
        $.messager.popup(msg);
    }
    , Toast: function (msg, state, hideAfter, postion, extendOpts) {
        var hide = typeof (hideAfter) != 'undefined' ? hideAfter : 2000;
        var icon = typeof (state) != 'undefined' ? (state ? 'success' : 'error') : 'info';
        postion = postion || 'mid-center';
        $.toast($.extend({}, {
            text: msg
            , showHideTransition: 'slide'
            , hideAfter: hide
            , position: postion
            , icon: icon
        }, extendOpts));
    }
    , isShoAlertModal: false
    //弹出信息提示框
    , Alert: function (status, text, onOk, onCancel) {
        // if (this instanceof arguments) {
        //   } else {//如果不是用 new alert()的话调用该函数
        if (Xms.Web.isShoAlertModal == true) {
            return false;
        }
        Xms.Web.isShoAlertModal = true;
        Xms.Web.AlertDialog(status, text, onOk, onCancel);
        //  }
    }
    , AlertDialog: function (status, text, onOk, onCancel) {
        var args = [].slice.call(arguments);
        if (args.length == 1 && typeof args[0] === 'object') {
            status = args[0].status;
            text = args[0].text;
            onOk = args[0].onOk;
            onCancel = args[0].onCancel;
        }
        var target = $("#alertDialog");
        var cssName = status == true ? "text-info" : "text-danger";
        var icon = status == true ? "glyphicon glyphicon-ok-sign" : "glyphicon glyphicon-remove-sign";
        target.remove();
        //if (target.length == 0) {
        $("body").append('<div id="alertDialog" class="hide"><p class="' + cssName + '"><span class="' + icon + '"></span> <strong>' + text + '</strong></p></div>');
        target = $("#alertDialog");
        //}

        target.dialog({
            title: "<span class=\"glyphicon glyphicon-info-sign\"></span> <strong>" + (LOC_NOTIFY || '确认提示') + "</strong>"
            , close: function (event, ui) {
            }
            , backdrop: 'static'
            , onClose: function () {
                Xms.Web.isShoAlertModal = false;
                if (typeof (onCancel) == "function") {
                    onCancel.call(this);
                }
                $(this).dialog("destroy");
                $(document.body).removeClass("modal-open");
                //target.remove();
                target.parents('.modal').next().remove();
                target.parents('.modal').remove();
            }
            , buttons: [
                {
                    text: "<span class=\"glyphicon glyphicon-remove\"></span> " + LOC_DIALOG_CLOSE,
                    click: function () {
                        Xms.Web.isShoAlertModal = false;
                        if (typeof (onCancel) == "function") {
                            onCancel.call(this);
                        }
                        $(this).dialog("destroy");
                        //target.remove();
                        target.parents('.modal').next().remove();
                        target.parents('.modal').remove();
                        $(document.body).removeClass("modal-open");
                    }
                }
                , {
                    text: "<span class=\"glyphicon glyphicon-ok\"></span> " + LOC_DIALOG_OK,
                    classed: "btn-info",
                    click: function () {
                        Xms.Web.isShoAlertModal = false;
                        if (typeof (onOk) == "function") {
                            onOk.call(this);
                        }
                        $(this).dialog("destroy");
                        //target.remove();
                        target.parents('.modal').next().remove();
                        target.parents('.modal').remove();
                        $(document.body).removeClass("modal-open");
                    }
                }]
        }).removeClass("hide");
        //dragable
        Xms.Web.Draggable();
    }
    , AlertSuccess: function (text, onOk, onCancel) {
        Xms.Web.Alert(true, text, onOk, onCancel);
    }
    , JsonSubmit: function (target, onsuccess, onerror, extoptions) {
        var data = target.serializeFormJSON((extoptions ? extoptions.ishiddendata : false)), action = target.attr('action'), isTips = target.attr('data-istip'), isformdata = target.attr('data-formdata'), files = [], formdata;
        var postdata = data;
        if (isformdata == 'true') {
            formdata = new FormData();
            target.find('input[type="file"]').each(function (key, item) {
                files.push($(this).attr('name'));
                formdata.append(this.name, this.files[0]);
            });

            for (var i in data) {
                if (data.hasOwnProperty(i)) {
                    var item = data[i];
                    if (item !== "" && i === extoptions.ishiddendata && item.indexOf('___file_upload_') != -1) {
                        item = item.replace(/___file_upload_/g, '');
                    }
                    formdata.append(i, item);
                }
            }
            postdata = formdata;
            extoptions.isformdata = true;
            extoptions._extend = function (opts) {
                $.extend(opts, {
                    contentType: false,
                    processData: false,
                    mimeType: opts.mimeType || false,
                });
            };
        }

        return Xms.Ajax.Post(action, postdata, function (response) {
            //console.log(response);
            if (isTips != '' && isTips == 'true') {
                Xms.Web.Toast(response.Content, response.IsSuccess);
            }
            onsuccess && onsuccess(response);
        }, function (xhr, textStatus, errorThrown) {
            onerror && onerror(xhr, textStatus, errorThrown);
        }, extoptions);
    }

    //表单
    , Form: function (target, onsuccess, onerror, beforesubmit, setting) {
        jQuery.validator.addMethod("isInt", function (value, element) {
            var regu = /^[-]{0,1}[0-9]{1,}$/;
            return this.optional(element) || (regu.test(value));
        }, "请输入数字");
        //表单控件name属性值改为小写
        target.find("input").each(function (i, n) {
            var name = $(n).prop("name");
            $(n).prop("name", name.toLowerCase());
        });
        //表单操作按钮设置
        if (target.find("#form-buttons").length > 0) {
            var formBtns = target.find("#form-buttons").find("button");
            formBtns.each(function (i, n) {
                var shadow = $(n).clone();
                if ($(n).prop("type") == "submit") {
                    shadow.click(function () { target.submit(); });
                }
                else if ($(n).prop("type") == "reset") {
                    shadow.click(function () { target.resetForm(); });
                }
                $("#body-footer-content").append(shadow);
                $("#body-footer-content").append(" ");
            });
            target.find("#form-buttons").addClass("hide");
            $("#body-footer").removeClass("hide");
            $("#renderBody").css("margin-bottom", "80px");
            //$("#content").css("margin-bottom", "80px");
        }
        //表单验证
        //target.find("input[data-val-required]").attr("data-toggle", "tooltip").prop("title", $(this).attr("data-val-required")).addClass("required");
        target.find(".form-group").each(function (i, n) {
            if ($(this).find(".required").length > 0) {
                $(this).find(".control-label").append('<span class="text-danger pull-right">*</span>');
            }
            else if ($(this).find("input[data-val-required]").length > 0) {
                $(this).find(".control-label").append('<span class="text-danger pull-right">*</span>');
                $(this).find("input[data-val-required]").addClass("required");
            }
        });
        function validRuleObj() {
            this.rule = {};
        }
        validRuleObj.prototype.add = function (name, rule) {
            if (typeof this.rule[name] === 'undefined') {
                this.rule[name] = {};
                for (var i in rule) {
                    if (rule.hasOwnProperty(i)) {
                        this.rule[name][i] = rule[i];
                    }
                }
            } else {
                for (var i in rule) {
                    if (rule.hasOwnProperty(i)) {
                        this.rule[name][i] = rule[i];
                    }
                }
            }
        }
        var validrule = new validRuleObj();
        var validRules = new Array();
        var checkreadonly = false;//是否检测带有 readonly
        target.find("input.required:not('.lookup')").each(function (i, n) {
            var name = $(this).prop('name');
            if ($(this).prop('readonly')) {
                $(this).removeClass('required');
            }

            validrule.add(name, { "required": true });
        });

        //添加lookup类型判断
        target.find("input.lookup").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            var id = $(this).attr('id');
            var hiddenName = name.replace('_text', '');
            var hiddenId = id.replace('_text', '');
            //console.log("$('#' + hiddenId).hasClass('.required')", $('#' + hiddenId));
            if ($('#' + hiddenId).hasClass('required') && !$(this).is(':disabled')) {
                // validRules.push('"' + hiddenName + '":{"required":true}');
                validrule.add(hiddenName, { "required": true });
            }
        });
        //console.log("validRules:" + validRules)
        target.find("input[data-val-length-max]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            $(this).prop("maxlength", $(this).attr("data-val-length-max"));
            validrule.add(name, { "maxlength": $(this).prop("maxlength") });
        });
        target.find("input[data-val-length-min]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            $(this).prop("minlength", $(this).attr("data-val-length-min"));
            validrule.add(name, { "minlength": $(this).prop("minlength") });
        });
        target.find("input[data-val-email]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            $(this).prop("type", "email");
            validrule.add(name, { "email": true });
        });
        target.find("input[data-val-url]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            $(this).prop("type", "url");
            validrule.add(name, { "url": true });
        });
        target.find("input[data-val-min]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            validrule.add(name, { "min": $(this).attr("data-val-min") });
        });
        target.find("input[data-val-max]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            validrule.add(name, { "max": $(this).attr("data-val-max") });
        });
        target.find("input[data-range]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            var a = eval($(this).attr("data-range"));
            validrule.add(name, { "range": [a[0], a[1]] });
        });
        target.find("input[data-rangelength]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            var a = eval($(this).attr("data-rangelength"));
            validrule.add(name, { "rangelength": [a[0], a[1]] });
        });
        target.find("input[data-type=email]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            $(this).prop("type", "email");
            validrule.add(name, { "email": true });
        });
        target.find("input[data-type=url]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            $(this).prop("type", "url");
            validrule.add(name, { "url": true });
        });
        target.find("input[data-type=int]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            validrule.add(name, { "isInt": true });
        });
        target.find("input[data-type=float]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            validrule.add(name, { "digits": true });
        });
        target.find("input[data-type=money]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            validrule.add(name, { "digits": true });
        });
        target.find("input[data-type=decimal]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            validrule.add(name, { "digits": true });
        });
        target.find("input[data-type=number],input[data-val-number]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            validrule.add(name, { "number": true });
        });
        target.find("input[data-type=date]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            validrule.add(name, { "date": true });
        });
        target.find("input[data-equalto]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var name = $(this).prop('name');
            validrule.add(name, { "equalTo": $(this).attr("data-equalto") });
        });
        console.log(validRules)
        var customMethod = [];
        target.find("input[data-custom]").each(function (i, n) {
            if ($(this).prop('readonly') && !checkreadonly) {
                return true;
            }
            var $this = $(this);
            var name = $this.attr("data-custom");
            $this.attr(name, true);
            var reg = $this.attr("data-customReg");
            var msg = $this.attr("data-customMsg");
            customMethod.push({
                name: name,
                reg: reg,
                msg: msg
            })
        });
        if (customMethod.length > 0) {
            $.each(customMethod, function (key, obj) {
                //console.log(obj)
                $.validator.addMethod(obj.name, function (value, element) {
                    var reg = new RegExp(obj.reg);
                    return reg.test(value);
                }, obj.msg);
            });
        }
        var rules = validrule.rule;//JSON.parse('{' + validRules.join(',') + '}');
        console.log('rules:', rules)
        var valiSetting = {
            errorClass: "text-warning valid-error"
            //, errorElement: "p"
            , focusCleanup: true
            , errorPlacement: function (error, element) {
                //console.log(error);
                $(element).siblings(".text-warning").remove();
                if ($(element).is(":radio") || $(element).is(":checkbox")) {
                    error.appendTo($(element).parent().parent());
                }
                else {
                    error.appendTo($(element).parent());
                }
                if (setting && setting.parentDom) {
                    setting.parentDom($(element));
                }
                if ($(element).parents(".form-cell-ctrl").length > 0) {
                    $(element).parents(".form-cell-ctrl").addClass("has-error");
                } else {
                    $(element).parents(".form-group").addClass("has-error");
                }
            }
            , success: function (label) {
                if ($(label).parents(".form-cell-ctrl").length > 0) {
                    $(label).parents(".form-cell-ctrl").removeClass("has-error");
                } else {
                    $(label).parents(".form-group").removeClass("has-error");
                }
                $(label).remove();
            }
            , rules: rules
            , ignore: ":disabled,[readonly=readonly]"

            , submitHandler: function (form, ev) {
                var ajaxsubmit = target.attr("data-ajaxsubmit");//是否以AJAX方式提交，默认是
                var jsonAjax = target.attr('data-jsonajax');
                var hiddenData = target.attr('data-hiddendata');//是否把除了hidden以外的数据放到一个新的data对象中
                var formdata = target.attr('data-formdata');
                if (ajaxsubmit != "" && ajaxsubmit == "false") {
                    form.submit();
                }
                else if (jsonAjax != "" && jsonAjax == "true") {
                    console.log('submit', ev);
                    ev.preventDefault && ev.preventDefault();
                    if (!hiddenData) {
                        if (typeof (beforesubmit) == "function") {//提交前方法
                            var issubmit = beforesubmit.call(this);
                            if (issubmit === false) {
                                return false;
                            }
                        }
                        Xms.Web.JsonSubmit($(form), function successHandler(response) {
                            var status = response.IsSuccess;
                            if (typeof (onsuccess) == "function") {//成功回调方法
                                onsuccess.call(this, response);
                            }
                            else {
                                //$.messager.popup(response.Content);
                                Xms.Web.Alert(status, response.Content);
                            }
                            if (target.attr("data-autoreset") && target.attr("data-autoreset").toLowerCase() == "true") {
                                if (status == true) target.resetForm();
                            }
                        }, function errorHandler(xhr, textStatus, errorThrown) {
                            if (typeof (onerror) == "function") {//失败回调方法
                                onerror.call(this, xhr);
                            }
                        }, {//配置contenttype为JSOn
                            'contentType': 'application/json; charset=utf-8'
                        });
                    } else {
                        if (typeof (beforesubmit) == "function") {//提交前方法
                            var issubmit = beforesubmit.call(this);
                            if (issubmit === false) {
                                return false;
                            }
                        }
                        Xms.Web.JsonSubmit($(form), function successHandler(response) {
                            var status = response.IsSuccess;
                            if (typeof (onsuccess) == "function") {//成功回调方法
                                onsuccess.call(this, response);
                            }
                            else {
                                //$.messager.popup(response.Content);
                                Xms.Web.Alert(status, response.Content);
                            }
                            if (target.attr("data-autoreset") && target.attr("data-autoreset").toLowerCase() == "true") {
                                if (status == true) target.resetForm();
                            }
                        }, function errorHandler(xhr, textStatus, errorThrown) {
                            //  Xms.Web.ErrorHandler(xhr, textStatus, errorThrown);
                            if (typeof (onerror) == "function") {//失败回调方法
                                onerror.call(this, xhr);
                            }
                        }, {//配置contenttype为JSOn
                            'contentType': 'application/json; charset=utf-8',
                            ishiddendata: hiddenData || 'data'
                        });
                    }
                    return false;
                }
                else {
                    jQuery(form).ajaxSubmit({
                        type: "post",
                        beforeSubmit: function () {
                            if (typeof (beforesubmit) == "function") {//提交前方法
                                return beforesubmit.call(this);
                            }
                        },
                        success: function (response) {
                            console.log(response);
                            var status = response.IsSuccess;
                            if (typeof (onsuccess) == "function") {//成功回调方法
                                onsuccess.call(this, response);
                            }
                            else {
                                //$.messager.popup(response.Content);
                                Xms.Web.Alert(status, response.Content);
                            }
                            if (target.attr("data-autoreset") && target.attr("data-autoreset").toLowerCase() == "true") {
                                if (status == true) target.resetForm();
                            }
                        },
                        error: function (xhr, textStatus, errorThrown) {
                            Xms.Web.ErrorHandler(xhr, textStatus, errorThrown);
                            if (typeof (onerror) == "function") {//失败回调方法
                                onerror.call(this, xhr);
                            }
                        }
                    });
                }
            }
        };
        valiSetting = $.extend({}, valiSetting, setting || {});
        //console.log(valiSetting)
        target.validate(valiSetting);
        return target;
    }
    //ajax 提交表单
    , AjaxForm: function (target, onOk, onError) {
        target.ajaxForm(function (response) {
            console.log(response);
            //$.messager.popup(response.Content);
            Xms.Web.Alert(true, response.Content, function () { onOk && onOk(response) }, function () { onError && onError(response) });
        });
    }
    //复选框设置为单选
    , SingleCheckbox: function (container, selector, mode, callback, isNotclear) {
        $(container).off('click', selector);
        if (mode == undefined || mode == 'single') {
            if (!isNotclear) {
                $(container).find(selector).prop('checked', false);
            }
            $(container).on('click', selector, function () {
                if ($(this).is(':checked')) {
                    $(container).find(selector).prop('checked', false);
                    $(this).prop('checked', true);
                    callback && callback.call(this, container);
                }
            });
        } else {
            //callback.call(selector, container);
        }
    }
    //获取或设置下拉选项选中的值
    , SelectedValue: function (selector, value) {
        var target = typeof (selector) == 'object' ? selector : $(selector);
        //设置值
        if (value != undefined) {
            if (value == '') value = '""';
            target.find('option').prop('selected', false);
            if (/hh\:mm\:ss$/.test(value)) {//防止时间format类型时报错
                target.find('option').each(function () {
                    if (~$(this).val().indexOf('hh:mm:ss')) {
                        $(this).prop('selected', true);
                    }
                });
            } else if (/^yyyy\/MM\/dd$/.test(value)) {
                target.find('option').each(function () {
                    if (/^yyyy\/MM\/dd$/.test($(this).val())) {
                        $(this).prop('selected', true);
                        return false;
                    }
                });
            } else {
                target.find('option[value=' + value + ']').prop('selected', true);
            }
        }

        else {
            var o = target.find('option:selected');
            if (o != undefined && o.length > 0) {
                return o.val();
            }
            return null;
        }
    }
    , Draggable: function () {
        $(".modal").draggable({
            handle: ".modal-header",
            cursor: 'move',
            refreshPositions: false
        });
    }
    , Dialog: function () {
        callback: []
    }
    , OpenDialog: function (url, callback, data, onopened) {
        var link = url;

        link += (link.indexOf('?') == -1 ? '?' : '&') + '__r=' + new Date().getTime();
        var params = $.urlParamObj(url);
        $('div[data-dialog="' + encodeURIComponent(link) + '"]').remove();
        var container = $('<div data-dialog="' + encodeURIComponent(link) + '"/>');
        $('body').append(container);
        if (!data) data = new Object();
        var _callback = callback ? (typeof (callback) == 'function' ? callback.toString() : callback) : '';
        link += (callback ? '&callback=' + (typeof (callback) == 'function' ? callback.toString() : callback) : '');
        if (!data.callback) {
            data.callback = _callback;
        }
        $.extend(data, params);
        Xms.Web.LoadPage(link, data, function (response) {
            if (response && response.indexOf('{') == 0) {
                var result = JSON.parse(response);
                Xms.Web.Alert(false, result.Content);
                return;
            }
            //防止页面出现多个相同的模态框
            try {
                var tempWrap = $('<div></div>').html(response);
                var __id = tempWrap.find('.modal').attr('id');
                if ($('#' + __id).length > 0) {
                    var par = $('#' + __id).parent();
                    $('#' + __id).remove();
                    par.remove();
                }
            } catch (e) {
            }
            container.html(response);
            if (typeof callback === 'string') {
                container.find('.modal:first').data().OpenDialogCallback = window[callback];
            } else if (container.find('.modal:first').data() && container.find('.modal:first').data().OpenDialogCallback) {
                container.find('.modal:first').data().OpenDialogCallback = callback;
            }
            if ($.fn.insertResizeBox) {
                if (container.find('.modal:first').length > 0) {
                }
            }
            if (typeof (onopened) == 'function') {
                onopened.call(null, response);
                console.log(2);
            }
            //dragable
            Xms.Web.Draggable();
        }, null, null, true);
    }
    , ParseParam: function (param, key) {
        var paramStr = "";
        if (param instanceof String || param instanceof Number || param instanceof Boolean) {
            paramStr += "&" + key + "=" + encodeURIComponent(param);
        } else {
            $.each(param, function (i) {
                var k = key == null ? i : key + (param instanceof Array ? "[" + i + "]" : "." + i);
                paramStr += '&' + parseParam(this, k);
            });
        }
        return paramStr.substr(1);
    }
    , getAttributePlugCallback: function (result, inputid) {
    }
    , AttributePlugCount: 0
    , getAttributePlugValue: function ($plugInput) {
        var res = undefined;
        if ($plugInput && $plugInput.length > 0) {
            var $input = null;
            if ($plugInput.hasClass('xmsplug-forminput')) {
                $input = $plugInput;
            } else {
                $input = $plugInput.find('.xmsplug-forminput:first');
            }
            var attrtype = $input.attr('data-type');
            if (attrtype == 'datetime') {
                res = $input.val();
            } else if (attrtype == "picklist") {
                res = $input.val();
            } else if (attrtype == "lookup" || attrtype == "customer" || attrtype == "owner" || attrtype == "primarykey") {
                res = $input.attr('data-value');
            } else if (attrtype == "int" || attrtype == "float" || attrtype == "decimal" || attrtype == "money") {
                res = $input.val();
            } else {
                res = $input.val();
            }
        }
        return res;
    }
    , emptyAttributePlugValue: function ($plugInput, val, label) {
        Xms.Web.setAttributePlugValue($plugInput, '', '');
    }
    , setAttributePlugValue: function ($plugInput, val, label) {
        if ($plugInput && $plugInput.length > 0) {
            var $input = null;
            if ($plugInput.hasClass('xmsplug-forminput')) {
                $input = $plugInput;
            } else {
                $input = $plugInput.find('.xmsplug-forminput:first');
            }
            var attrtype = $input.attr('data-type');
            if (attrtype == 'datetime') {
                $input.val(val);
            } else if (attrtype == "picklist") {
                $input.val(val);
                $plugInput.find('select>option[value="' + val + '"]').prop('select', true);
            } else if (attrtype == "lookup" || attrtype == "customer" || attrtype == "owner" || attrtype == "primarykey") {
                $input.attr('data-value', val);
                $input.val(label)
            } else if (attrtype == "int" || attrtype == "float" || attrtype == "decimal" || attrtype == "money") {
                $input.val(val);
            } else {
                $input.val(val);
            }
        }
    }
    , setAttributePlugState: function ($plugInput, state) {
        state = state || 'edit';
        if ($plugInput && $plugInput.length > 0) {
            var $input = null;
            if ($plugInput.hasClass('xmsplug-forminput')) {
                var $parent = $plugInput.parent();
                setState($parent, state)
            } else {
                $input = $plugInput.find('.xmsplug-forminput:first');
                setState($plugInput, state)
            }
        }
        function setState($plugInput, state) {
            if (state == 'readonly') {
                $plugInput.find('input,textarea').prop('readonly', true);
                $plugInput.find('button,select').prop('disabled', true);
            } else if (state == 'disabled') {
                $plugInput.find('input,select,button,textarea').prop('disabled', 'disabled');
            } else if (state == 'edit') {
                $plugInput.find('input,select,button,textarea').prop('disabled', false).prop('readonly', false);
            }
        }
    }
    , getAttributePlug: function (n, $target, opts) { //n是字段的信息  object,$target容器
        var attrname = n.name;
        $target = $target || $('body');
        if ($target.length == 0) {
            $target.appendTo($('body'));
        }
        if (!attrname) return true;
        var isrela = attrname.indexOf('.') != -1;
        var attrtype = n.attributetypename;
        var label = n.localizedname;
        var ctrilId = isrela ? attrname.replace('.', '_') : attrname;
        ctrilId = ctrilId + Xms.Web.AttributePlugCount++;
        var referentityid = n.referencedentityid;
        var entityid = n.entityid;

        // if (isrela) {
        var itemshtml = [];
        //  } else {
        if (attrname == 'createdon') {
            isshowQueryDate = true;//是否显示快捷过滤时间的按钮
        }
        var prefix = 'xms_plug_id_';
        ctrilId = prefix + ctrilId;
        if (attrtype == 'datetime') {
            itemshtml.push('<div class="form-group xmsplug-formrangepicker">');
            itemshtml.push(' <input type="text" id="' + ctrilId + '" class="form-control xmsplug-forminput" data-type="' + attrtype + '" autocomplete="off" name="' + attrname + '" />');
            itemshtml.push('</div>');
        } else if (attrtype == "picklist") {
            itemshtml.push(' <input type="text" id="' + ctrilId + '" class="form-control picklist  xmsplug-forminput" data-type="' + attrtype + '" data-name="' + attrname + '" name="' + attrname + '" data-items="' + '' + '" data-optionsetid="' + n.optionsetid +'" />');
        } else if (attrtype == "lookup" || attrtype == "customer" || attrtype == "owner" || attrtype == "primarykey") {
            itemshtml.push('<div class="input-group ">');
            itemshtml.push('<input type="text" id="' + ctrilId + '" data-type="lookup" data-entityid="' + entityid + '" data-referencedentityid="' + referentityid + '" name="' + attrname + '" class="form-control pluglookup searchLookup  xmsplug-forminput" />');
            itemshtml.push('<span class="input-group-btn">');
            itemshtml.push('<button type="button" name="clearBtn" class="btn btn-default ctrl-del" title="delete" style="border-radius:0;"><span class="glyphicon glyphicon-remove-sign"></span></button>');
            itemshtml.push('<button type="button" name="lookupBtn" class="btn btn-default ctrl-search" title="find" style="border-top-left-radius: 0;border-bottom-left-radius: 0;"><span class="glyphicon glyphicon-search"></span></button>');
            itemshtml.push('</span>');
            itemshtml.push('</div>');
        } else if (attrtype == "int" || attrtype == "float" || attrtype == "decimal" || attrtype == "money") {
            itemshtml.push('<div class="form-group">');
            itemshtml.push('<input type = "text"  id = "' + ctrilId + '" class= "form-control  xmsplug-forminput" data-type="' + attrtype + '"  name = "' + attrname + '" value = "" />');

            itemshtml.push('</div >');
        } else {
            itemshtml.push('<input type="text" id="' + ctrilId + '" class="form-control xmsplug-forminput" name="' + attrname + '" data-type="' + attrtype + '" value="" />');
        }
        var $plugInput = null;
        if (itemshtml.length > 0) {
            $plugInput = $(itemshtml.join(''));
        }
        $plugInput.appendTo($target);
        $plugInput.each(function () {
            if ($(this).hasClass('xmsplug-formrangepicker')) {
                $(this).find('input').datepicker({
                    autoclose: true
                    , clearBtn: true
                    , format: "yyyy-mm-dd"
                    , language: "zh-CN"
                });
            }
        })
        if (attrtype == "picklist") {
            $plugInput.each(function (i, n) {
                var $this = $(n);
                var type = $this.attr("data-name");
                var optionsetid = $this.attr('data-optionsetid');
                var url = '/api/schema/optionset/getitems/' + optionsetid+'?__r=' + new Date().getTime();

                Xms.Web.GetJson(url,null, function (res) {
                    console.log('GetOptionsets', res.content)
                    try {
                        var items = res.content
                        $this.picklist({
                            items: items
                        });
                    } catch (e) {
                        console.log('picklist', res, e);
                    }
                });

            });
        }
        $(".pluglookup", $plugInput).each(function () {
            var self = this;
            var $input = $(self);

            var inputid = $input.attr("id");
            if (!inputid) {
                inputid = "lookup_" + new Date() * 1;
                $input.attr("id", inputid);
            }
            $(self).siblings("span").find(".ctrl-search").off('click').on('click', function () {
                var $this = $(this);
                var type = $this.attr("data-name");
                var inreferencedentityid = $this.parents("span:first").siblings("input").attr("data-referencedentityid");

                var lookupurl = '/entity/recordsdialog?entityid=' + inreferencedentityid + '&singlemode=true&inputid=' + inputid;
                if ($this.attr('data-defaultviewid')) {
                    lookupurl = $.setUrlParam(lookupurl, 'queryid', $this.attr('data-defaultviewid'));
                }
                Xms.Web.OpenDialog(lookupurl, "Xms.Web.getAttributePlugCallback", null, function () {
                    var _value = $input.val();
                    var $dialogInput = $('#entityRecordsModal').find('#Q');
                    var $dialogSearch = $('#entityRecordsModal').find('button[name="searchBtn"]');
                    console.log('lookup_value', _value);
                    if (_value != '') {
                        var data_value = $input.attr('data-value');
                        console.log('data-value', data_value);
                        if (data_value && ~data_value.indexOf(',')) {//如果为多选设置已选中的记录
                            try {
                                var arrvalue = data_value.split(',');
                                $.each(arrvalue, function (key, item) {
                                    $('#entityRecordsModal').find('input[name="recordid"][value="' + item + '"]').prop('checked', true);
                                });
                            } catch (e) {
                            }
                        } else {
                            $dialogInput.val(_value);
                            $dialogSearch.trigger('click');
                        }
                    }
                });
            });

            $(self).siblings("span").find(".ctrl-del").off('click').on('click', function () {
                var $this = $(this);
                var input = $this.parents("span:first").siblings("input");
                var rowPar = $this.parents('.input-group:first');
                input.attr("data-value", "").val("").css('color', '#555');
            });

            $("#" + inputid).off("dialog.return").on("dialog.return", function (e, result) {
                if (!opts || !opts.dialogReturn) {
                    var input = $(this).attr('id');
                    var rName = [];
                    var rId = [];
                    var $input = $('#' + input);
                    if (result.length > 0) {
                        for (var i = 0; i < result.length; i++) {
                            rName.push(result[i].name);
                            rId.push(result[i].id);
                        }
                    } else {
                        rName.push(result.name);
                        rId.push(result.id);
                    }
                    console.log(rName);
                    $input.val(rName.join(','));
                    $input.attr('data-value', rId.join(','));
                } else {
                    opts.dialogReturn.call(this, e, result, inputid);
                }
            });
        });

        opts && opts.plugLoaded && opts.plugLoaded($plugInput);

        return $plugInput;
    }
    , CloseDialog: function (target) {
        $(target).parent().next().remove();
        $(target).parent().remove();
        $(document.body).removeClass("modal-open");
    }
    , IsDbClick: function (self) {
        var isDblclick = false;
        firstClick = self.attr("firstclick");
        secondClick = self.attr("secondclick");
        if (!firstClick) {
            self.removeAttr("secondclick");
            self.attr("firstclick", new Date() * 1);
        } else if (firstClick && !secondClick) {
            secondClick = new Date() * 1
            self.attr("secondclick", secondClick);
            if (secondClick * 1 - (firstClick * 1) > 300) {
                self.removeAttr("firstclick");
                self.removeAttr("secondclick");
                Xms.Web.IsDbClick(self);
            } else if (secondClick * 1 - (firstClick * 1) < 300) {
                isDblclick = true;
            }
        } else if (firstClick && secondClick) {
            self.removeAttr("firstclick");
            self.removeAttr("secondclick");
            Xms.Web.IsDbClick(self);
        }
        return isDblclick;
    }
    , OpenWindow: function (url, type, extstr) {
        type = type || '_blank';
        extstr = extstr || '';
        var alinkstr = '<a href="' + url + '" class="hide" target="' + type + '" {extstr} ><span>a</span></a>';
        alinkstr = alinkstr.replace(/{extstr}/, extstr);
        var d = $(alinkstr);
        $('body').append(d);
        d.find('span').trigger('click');
        d.remove();
    }
    , CloseWindow: function () {
        if (window.opener) {
            window.close();
        }
        else {
            var userAgent = navigator.userAgent;
            if (userAgent.indexOf("Firefox") != -1 || userAgent.indexOf("Chrome") != -1) {
                window.location.href = "about:blank";
            } else {
                window.opener = null;
                window.open("", "_self");
                window.close();
            };
        }
    }
    , Event: {
        _event: this,
        subscribes: [],
        isinit: false,
        init: function (type) {
            if (Xms.Web.Event.isinit == true) return;
            Xms.Web.Event.isinit = true;
            var handMessage = function (e) {
                e = e || event || window.event;
                console.log(e);
                if (!type) {
                    $(Xms.Web.Event.subscribes).each(function (i, n) {
                        console.log(n);
                        if (n.key == e.data) {
                            n.handler.call(this, arguments);
                            return true;
                        }
                    });
                } else {
                    $(Xms.Web.Event.subscribes).each(function (i, n) {
                        console.log(n);
                        if (e.data.indexOf(n.key) == 0) {
                            n.handler(e, n);
                            return true;
                        }
                    });
                }
            }
            if (window.addEventListener) {
                window.addEventListener("message", handMessage, false);
            }
            else {
                window.attachEvent("onmessage", handMessage);
            }
        },
        clear: function () {
            this.subscribes = [];
        },
        clearBy: function (str) {
            var flag = -1;
            $(Xms.Web.Event.subscribes).each(function (i, n) {
                console.log(n);
                if (n.key.indexOf(str) == 0) {
                    flag = n;
                    return true;
                }
            });
            if (flag != -1) {
                Xms.Web.Event.subscribes.splice(flag, 1);
            }
        },
        subscribe: function (message, handler, type) {
            Xms.Web.Event.init(type);
            if (!type) {
                var e = { key: message, handler: handler };
            } else {
                var e = {
                    key: message,
                    handler: function (e, n) {
                        handler(e, n);
                    }
                };
            }
            Xms.Web.Event.subscribes.push(e);
        },
        publish: function (message, type) {
            Xms.Web.Event.init(type);
            if (window.opener) {
                window.opener && window.opener.postMessage && window.opener.postMessage(message, "*");
            }
            else {
                if (window.parent) {
                    window.parent && window.parent.postMessage && window.parent.postMessage(message, "*");
                }
                window && window.postMessage && window.postMessage(message, "*");
            }
        }
        , localStorageEvent: new function () {
            var prefix = '_xms_event_';
            function _EVENT(name, handler) {
                this.name = name;
                this.handler = handler;
            }
            var eventlist = {};
            var self = this;
            this.on = function (name, handler) {
                if (!eventlist[prefix + name]) {
                    eventlist[prefix + name] = [];
                }
                var ev = new _EVENT(prefix + name, handler);
                eventlist[prefix + name].push(ev);
            }
            this.trigger = function (name) {
                localStorage.setItem(prefix + name, new Date() * 1);
            }
            this.get = function () {
                return eventlist;
            }
            this.run = function (e, handlers, val) {
                $.each(handlers, function (i, n) {
                    if (n && typeof n.handler == 'function') {
                        n.handler.call(self, e, val);
                    }
                })
            }
            window.addEventListener('storage', function (e) {
                //console.log(e);
                var name = e.key;
                var val = e.newValue;
                if (eventlist[name] && eventlist[name].length > 0) {
                    self.run(e, eventlist[name], val);
                }
            });
        }
    }
    , ValidData: function (type) {
        var vali_Regexp = {
            "english": isEnglish,
            "englishandnumber": isEnglishAndNumber,
            "ischinese": isChinese,
            "qq": isQQ,
            "mobile": isMobile,
            "email": isEmail,
            "integer": isInteger,
            "isip": isIP,
            "isnumber": isNumber,
            "isdecimal": isDecimal,
            "minlength": minLength,
            "maxlength": maxLength,
            "money": isMoney,
            "isnull": isNull,
            "float": isFloat
        }
        function isEnglish(v) {
            var re = new RegExp("^[a-zA-Z\_]+$");
            if (re.test(v)) return true;
            return false;
        }
        function isFunction(func) {
            if (typeof func == "function") {
                return true;
            }
            return false;
        }
        function isNull(val) {
            return val == ""
        }
        function isIP(strIP) {
            if (isNull(strIP)) return false;
            var re = /^(\d+)\.(\d+)\.(\d+)\.(\d+)$/g //匹配IP地址的正则表达式
            if (re.test(strIP)) {
                if (RegExp.$1 < 256 && RegExp.$2 < 256 && RegExp.$3 < 256 && RegExp.$4 < 256) { return true; }
            }
            return false;
        }
        function isEnglishAndNumber(v) {
            var re = new RegExp("^[0-9a-zA-Z\_]+$");
            if (re.test(v)) return true;
            return false;
        }
        function isChinese(v) {
            var re = new RegExp("^[\u4e00-\u9fa5]+$");
            if (re.test(v)) return true;
            return false;
        }
        function maxLength(v, max) {
            var len = v.length;
            if (len <= max) {
                return true;
            } else {
                return false;
            }
        }
        function minLength(v, min) {
            var len = v.length;
            if (len >= max) {
                return true;
            } else {
                return false;
            }
        }
        function isQQ(v) {
            var re = new RegExp("^[1-9][0-9]{4,9}$");
            if (re.test(v)) return true;
            return false;
        }
        function isFloat(str) {
            if (isInteger(str)) return true;
            if (isNaN(str)) return false;
            var re = /^[-]{0,1}(\d+)[\.]+(\d+)$/;
            str = str.replace(/\,/g, "");
            if (re.test(str)) {
                //if (RegExp.$1 == 0 && RegExp.$2 == 0) return false;
                return true;
            } else {
                return false;
            }
        }
        function isMobile(v) {
            var re = new RegExp("^(0|86|17951)?(13[0-9]|15[012356789]|18[0-9]|14[57])[0-9]{8}$");
            if (re.test(v)) return true;
            return false;
        }
        function isEmail(v) {
            var re = new RegExp("^[a-z0-9]+([._\\-]*[a-z0-9])*@([a-z0-9]+[-a-z0-9]*[a-z0-9]+.){1,63}[a-z0-9]+$");
            if (re.test(v)) return true;
            return false;
        }
        function isNumber(v) {
            var re = new RegExp("^[0-9]+$");
            if (re.test(v)) return true;
            return false;
        }
        function isInteger(str) {
            var regu = /^[-]{0,1}[0-9]{1,}$/;
            return regu.test(str);
        }
        function isDecimal(str) {
            if (isInteger(str)) return true;
            var re = /^[-]{0,1}(\d+)[\.]+(\d+)$/;
            if (re.test(str)) {
                if (RegExp.$1 == 0 && RegExp.$2 == 0) return false;
                return true;
            } else {
                return false;
            }
        }
        function isMoney(s) {
            s = s.replace(/\,/g, '');
            //s = s * 1;
            //if (isNaN(s)) return false;
            if (isInteger(s)) return true;
            var regu = /^[-]{0,1}(([1-9]\d*)|0)(\.\d{1,3})?$/;
            //var re = new RegExp(regu);
            // console.log(regu.test(s), s);
            if (regu.test(s)) {
                //if (RegExp.$1 == 0 && RegExp.$2 == 0) return false;
                return true;
            } else {
                return false;
            }
        }
        return vali_Regexp[type] || false;
    }

    , PageCacheConfig: { timeount: 5000, step: 50 }
    , PageCacheData: []//页面异步缓存数据
    , PageCache: function (type, url, param, callback, isSync, actype) {//页面缓存数据获取方法，type值同一个页面一样。
        var cacheFactory;
        if (typeof Xms.Web.PageCacheData[type] === 'undefined') {
            cacheFactory = Xms.Web.PageCacheData[type] = [];
            cacheFactory[param.type] = [];
            cacheFactory[param.type]['state'] = 'start';
            cacheFactory[param.type]['data'] = null;
        } else {
            cacheFactory = Xms.Web.PageCacheData[type];
            if (typeof cacheFactory[param.type] == 'undefined') {
                cacheFactory[param.type] = [];
                cacheFactory[param.type]['state'] = 'start';
                cacheFactory[param.type]['data'] = null;
            }
        }
        //console.log(cacheFactory)
        if (cacheFactory[param.type]['state'] == 'init' || cacheFactory[param.type]['state'] == 'loaded') {
            var starttime = 0;
            var timer = setInterval(function () {
                if (cacheFactory[param.type]['state'] == 'loaded') {
                    if (cacheFactory[param.type]['data'] && cacheFactory[param.type]['data'].content) {
                        callback(cacheFactory[param.type]['data']);
                        clearInterval(timer);
                    }
                }
            }, Xms.Web.PageCacheConfig.step);
        } else if (cacheFactory[param.type]['state'] == 'start' && cacheFactory[param.type]['state'] != 'loaded') {
            console.log(cacheFactory[param.type]['state']);
            cacheFactory[param.type]['state'] = 'init';
            Xms.Web.GetJson(url, param.data, function (data) {
                cacheFactory[param.type]['state'] = 'loaded';
                cacheFactory[param.type]['data'] = data;
                callback(data);
            }, null, isSync || true, actype || null, isSync);
        }
    },

    getDataUrlApi: function (url) {
        var index = url.indexOf('?');
        return url.substr(0, index);
    }
    , Notice: function (opts) {
        if (typeof Notification === 'undefined') { return false; }
        opts = $.extend({}, {
            title: '您有新的消息!',//设置信息标题
            onclick: null,
            onerror: null,
            onclose: null,
            onshow: null,
            dir: 'ltr',//内容文字对齐方式
            lang: '',
            body: '',//设置信息内容
            tag: '',//设置消息框标志，唯一的
            icon: '/content/imgs/new_notice.png',
            renotify: false,//是否替换之前的提示框
            timeout: -1//设置是否自动隐藏,-1为不隐藏,单位毫秒
        }, opts);
        var notification = null;
        var isShow = false;
        Notification.requestPermission(function () {
            if (Notification.permission == "granted") {
                var noticeOpts = {
                    title: opts.title,
                    onclick: opts.onclick,
                    onerror: opts.onerror,
                    onclose: function () { opts.onclose && opts.onclose(); isShow = false; },
                    onshow: function () {
                        opts.onshow && opts.onshow(); isShow = true; opts.timeout != -1 && setTimeout(function () {
                            if (notification) {
                                notification.close();
                            }
                        }, opts.timeout)
                    },
                    dir: opts.dir,
                    lang: opts.lang,
                    body: opts.body,
                    tag: opts.tag,
                    icon: opts.icon,
                    renotify: false//是否替换之前的提示
                }
                if (opts.renotify == true && noticeOpts.tag == '') {
                    noticeOpts.tag = (Math.random() * 100000 >> 0).toString(16);
                }
                notification = new Notification(noticeOpts.title, noticeOpts);
                notification.onclick = function (a, b, c, d) {
                    noticeOpts.onclick && noticeOpts.onclick(a, b, c, d);
                    notification.close();
                };
                notification.onclose = function (a, b, c, d) {
                    noticeOpts.onclose && noticeOpts.onclose(a, b, c, d);
                };
                notification.onshow = function (a, b, c, d) {
                    noticeOpts.onshow && noticeOpts.onshow(a, b, c, d);
                };
                notification.onerror = function (a, b, c, d) {
                    noticeOpts.onerror && noticeOpts.onerror(a, b, c, d);
                    notification.close();
                };
            } else {
                Xms.Web.Alert(false, '您还未开启消息通知，是否重新开启？', function () {
                    Xms.Web.Notice(title, opts)
                })
            }
        });
        return notification;
    }
    , XmsLoops: function (opts) {
        var settings = {
            timeout: 1000
            , loopHandler: null
        }
        function _xmsLoops(opts) {
            this.timer = null;
            this.isRun = true;
            this.opts = $.extend({}, settings, opts);
            this.timeout = this.opts.timeount;
        }
        _xmsLoops.prototype.loopStart = function () {
            var self = this;
            this.timer = setInterval(function () {
                self.opts.loopHandler && self.opts.loopHandler(self);
            }, this.opts.timeout);
            return this;
        }
        _xmsLoops.prototype.loopStop = function () {
            clearInterval(this.timer);
            return this;
        }

        return new _xmsLoops(opts);
    }
    , ToastNoticeObj: null
    , ToastNotice: function () {
        var _oldnoticeId = '';
        var loops = Xms.Web.XmsLoops({
            loopHandler: function (loop) {
                var filter = {};
                filter.Filters = [{ "FilterOperator": Xms.Fetch.LogicalOperator.Or, "Conditions": [{ "AttributeName": "isread", "Operator": Xms.Fetch.ConditionOperator.Equal, "Values": [0] }] }, { "FilterOperator": Xms.Fetch.LogicalOperator.And, "Conditions": [{ "AttributeName": "ownerid", "Operator": Xms.Fetch.ConditionOperator.EqualUserId }] }];
                var queryObj = { EntityName: 'Notice', ColumnSet: { allcolumns: true }, PageInfo: { PageNumber: 1, PageSize: 1 } };
                queryObj.Criteria = filter;
                queryObj.Orders = [{ "AttributeName": "createdon", "OrderType": 1 }];
                var data = JSON.stringify({ "query": queryObj });
                Xms.Web.GetJson('/api/data/Retrieve/Multiple', data, function (response) {
                    if (response.content && response.content.items.length > 0) {
                        var count = response.content.totalitems;
                        for (var i = 0; i < response.content.items.length; i++) {
                            var item = response.content.items[i];
                            if (item.noticeid === _oldnoticeId) { return false; }
                            _oldnoticeId = item.noticeid;
                            Xms.Web.Toast(item.name, 'info', 100000, null, {
                                position: 'bottom-right',
                                clickHandler: function () {
                                    Xms.Web.Get(ORG_SERVERURL + '/api/notice/' + item.noticeid, function (response) {
                                        if (response.IsSuccess) {
                                            if (response.content.linkto) {
                                                Xms.Web.OpenWindow(response.content.linkto);
                                            }
                                            else {
                                                Xms.Web.OpenWindow(ORG_SERVERURL + '/entity/create?entityname=notice&recordid=' + response.content.noticeid);
                                            }
                                        }
                                    });
                                }
                            });
                        }
                    }
                }, null, null, 'post');
            }
        });
        Xms.Web.ToastNoticeObj = loops;
        return loops;
    }
    , Redirect: function (url) {
        if (url.indexOf(ORG_SERVERURL) !== 0) {
            url = ORG_SERVERURL + (url.indexOf('/') == 0 ? "" : "/") + url;
        }
        location.href = url;
    },
    tabCount: 0,
    OpenTab: function (tabname, tabContentLink, tabId) {
        var prefix = "winTab__";
        var tabId = tabId || prefix + (Xms.Web.tabCount++);
        var iframe = Xms.Web.callSuperParentMethod('takeToBottom', tabname, tabContentLink, tabId, 'pageViewName');
    },
    OpenTabByHtml: function (tabname, tabHtml) {
        var prefix = "winTab__";
        var tabId = prefix + (Xms.Web.tabCount++)
    }
    , callChildMethod: function (childid, method, arg1, arg2, arg3, arg4, arg5, arg6) {
        var child = document.getElementById(childid).contentWindow
        if (child && child[method]) {
            child[method](document.body, arg1, arg2, arg3, arg4, arg5, arg6);
        }
    }
    , callSuperParentMethod: function (method, arg1, arg2, arg3, arg4, arg5, arg6) {
        var _par = window.parent;
        while (_par) {
            if (_par.parent != _par) {
                _par = _par.parent;
            } else {
                break;
            }
        }
        if (_par && _par[method]) {
            return _par[method](document.body, arg1, arg2, arg3, arg4, arg5, arg6);
        }
    }
    , callParentMethod: function (method, arg1, arg2, arg3, arg4, arg5, arg6) {
        if (parent && parent[method]) {
            return parent[method](document.body, arg1, arg2, arg3, arg4, arg5, arg6);
        }
    }
    , _callParentMethod: function (method, arg1, arg2, arg3, arg4, arg5, arg6) {
        if (parent && parent[method]) {
            return parent[method](arg1, arg2, arg3, arg4, arg5, arg6);
        }
    }
    , getParentWin: function () {
        return window.parent;
    },
    iFrameHeight: function ($iframe, offsetH) {
        if (typeof $iframe == 'string') {
            var ifm = document.getElementById($iframe);
        } else if (typeof $iframe == 'object') {
            var ifm = $($iframe).get(0);
        }
        var ifname = ifm.name;
        var subWeb = document.frames ? document.frames[ifname].document : ifm.contentDocument;
        var navbarH = offsetH;
        if (ifm != null && subWeb != null) {
            ifm.height = $(window).height() + navbarH;//subWeb.body.scrollHeight;
        }
    }
    , loadScript: function (src, loaded, onLoad, onError, onEnd) {
        var script = document.createElement("script");
        script.onload = function () {
            this.onerror = null;
            this.onload = null;
            document.body.removeChild(this);
            if (onLoad && onLoad.call) {
                onLoad(this.src);
            }
            loaded--;

            if (!loaded) {
                if (onEnd && onEnd.call) {
                    onEnd();
                }
                return;
            }
            loadScript(src, loaded, onLoad, onError, onEnd);
        }
        script.onerror = function () {
            this.onerror = null;
            this.onload = null;
            document.body.removeChild(this);
            if (onError && onError.call) {
                onError(this.src);
            }
            loaded--;
            if (!loaded) {
                if (onEnd && onEnd.call) {
                    onEnd();
                }
                return;
            }
            loadScript(src, loaded, onLoad, onError, onEnd);
        }
        script.src = src.shift();
        document.body.appendChild(script);
    },
    /**
     * 加载脚本
     * @param {Object} cfg 参数
     * @param {Array} cfg.urls 脚本路径数组。['/lib/a.js','/lib/b.js','/lib/c.js']
     * @param {int} cfg.mode 脚本加载方式，默认为串行加载LOADMODE_SEQU
     * @param {Function} cfg.onLoad 单个脚本加载成功事件,可选   回调参数[src]
     * @param {Function} cfg.onEnd 全部脚本加载完成事件,可选
     * @param {Function} cfg.onError 单个脚本加载失败事件,可选
     * @return this
     */
    loadScripts: function (cfg) {
        var LOADMODE_SEQU = 2;
        var loaded = cfg.urls.length;
        var onLoad = cfg.onLoad;
        var onEnd = cfg.onEnd;
        var onError = cfg.onError;
        var d = document;
        var b = document.body;
        var mode = cfg.mode || LOADMODE_SEQU;
        if (mode === LOADMODE_SEQU)
            loadScript(cfg.urls.concat(), loaded, onLoad, onError, onEnd);
        else {
            for (var i = cfg.urls.length; i--;) {
                var s = d.createElement("script");//指定src时，类型必须是javascript或者空，无法加载文本资源
                if (!s.async) s.defer = true;
                s.onload = function () {
                    this.onerror = null;
                    this.onload = null;
                    document.body.removeChild(this);
                    if (onLoad && onLoad.call) {
                        onLoad(this.src);
                    }
                    loaded--;
                    if (!loaded && onEnd && onEnd.call) {
                        onEnd();
                    }
                }
                s.onerror = function () {
                    this.onerror = null;
                    this.onload = null;
                    document.body.removeChild(this);
                    if (onError && onError.call) {
                        onError(this.src);
                    }
                    loaded--;
                    if (!loaded && onEnd && onEnd.call) {
                        onEnd();
                    }
                }
                s.src = cfg.urls[i];
                b.appendChild(s);
            }
        }
        return this;
    }
    , fullByContext: function ($context, $target, offsetH, offsetT, style) {
        offsetH = offsetH || 0;
        offsetT = offsetT || 0;
        style = style || 'min-height'
        var $cH = $context.height();
        var _css = {};
        _css[style] = $cH + offsetH + offsetT;
        $target.css(_css);
    }
    , isImg: function (str) {
        var reg = new RegExp('png|jpeg|jpg|gif|svg', 'gm');
        return reg.test(str)
    }
}

Xms.Ajax = new function () {
    var self = this;
    this.contentTypes = ['application/x-www-form-urlencoded', "application/json; charset=utf-8", "multipart/form-data"];
    this.baseUrl = '';
    this.setDefaults = function (opts) {
        $.extend(defaults, opts);
    }
    this.setBaseUrl = function (baseUrl) {
        self.baseUrl = baseUrl;
    }

    var defaults = {
        type: 'GET',
        //dataType: "json",
        contentType: this.contentTypes[0],
        cache: false,
        async: false,
        beforeRequest: function () { return true; },
        beforeResponse: function () { return true; },
    }
    var optsInfos = {
        get: $.extend({}, defaults, { type: 'GET' }),
        post: $.extend({}, defaults, { type: 'POST', contentType: 'application/json; charset=utf-8', }),
        put: $.extend({}, defaults, { type: 'PUT' }),
        delete: $.extend({}, defaults, { type: 'DELETE' }),
        asyncGet: $.extend({}, defaults, { type: 'GET', async: true }),
        asyncPost: $.extend({}, defaults, { type: 'POST', async: true }),
        getJson: $.extend({}, defaults, { type: 'GET', dataType: 'json' }),
        loadPage: $.extend({}, defaults, { type: 'GET', dataType: 'text' })
    }
    this.getOptsInfo = function (type, url, data, onsuccess, onerror, extOpts) {
        var args = [].slice.call(arguments), opts;
        if (args.length == 2) {//如果只有2个参数，会视为args[1]为对象
            opts = $.extend({}, optsInfos[type], args[1]);
        } else {
            opts = $.extend({}, optsInfos[type], { url: args[1], data: (args[2] || {}), onsuccess: args[3], onerror: (onerror || null) }, extOpts);
        }
        return opts;
    }

    this.Get = function (url, data, onsuccess, onerror, extOpts) {
        var opts = self.getOptsInfo.call(self, 'get', url, data, onsuccess, onerror, extOpts);
        return self.send(opts);
    }
    this.Post = function (url, data, onsuccess, onerror, extOpts) {
        var opts = self.getOptsInfo.call(self, 'post', url, data, onsuccess, onerror, extOpts);
        return self.send(opts);
    }
    this.Put = function (url, data, onsuccess, onerror, extOpts) {
        var opts = self.getOptsInfo.call(self, 'put', url, data, onsuccess, onerror, extOpts);
        return self.send(opts);
    }
    this.Delete = function (url, data, onsuccess, onerror, extOpts) {
        var opts = self.getOptsInfo.call(self, 'delete', url, data, onsuccess, onerror, extOpts);
        return self.send(opts);
    }
    this.GetJson = function (url, data, onsuccess, onerror, extOpts) {
        var opts = self.getOptsInfo.call(self, 'getJson', url, data, onsuccess, onerror, extOpts);
        return self.send(opts);
    }
    this.LoadPage = function (url, data, onsuccess, onerror, extOpts) {
        var opts = self.getOptsInfo.call(self, 'loadPage', url, data, onsuccess, onerror, extOpts);
        if (opts.data) {
            opts.type = 'POST';
        }
        opts.contentType = self.contentTypes[1];
        if (typeof opts.data == 'object') {
            opts.data = JSON.stringify(opts.data);
        }
        return self.send(opts);
    }
    this.AsyncGet = function (url, data, onsuccess, onerror, extOpts) {
        var opts = self.getOptsInfo.call(self, 'asyncGet', url, data, onsuccess, onerror, extOpts);
        return self.send(opts);
    }
    this.AsyncPost = function (url, data, onsuccess, onerror, extOpts) {
        var opts = self.getOptsInfo.call(self, 'asyncPost', url, data, onsuccess, onerror, extOpts);
        return self.send(opts);
    }
    this.GetJson = function (url, data, onsuccess, onerror, extOpts) {
        var opts = self.getOptsInfo.call(self, 'getJson', url, data, onsuccess, onerror, extOpts);
        return self.send(opts);
    }

    this.send = function (opts) {
        var url = opts.url;

        var dtd = $.Deferred();
        if (~url.indexOf('?') && !opts.isformdata) {
            var model = {};
            model = $.urlParamObj(url);//如果请求方法中带有查询参数，提取出参数放到一个对象中
            url = Xms.Web.getDataUrlApi(url);
            url = url + (url.indexOf('?') == -1 ? '?' : '&') + '__r=' + new Date().getTime();
            opts.data = $.extend({}, model, opts.data);
        }
        if (url.indexOf(ORG_SERVERURL) !== 0) {
            url = ORG_SERVERURL + (url.indexOf('/') == 0 ? "" : "/") + url;
        }
        var beforeRequest = opts.beforeRequest();
        if (beforeRequest === false) return false;
        if (opts.contentType && ~opts.contentType.indexOf('json') && !opts.isformdata) {
            opts.data = JSON.stringify(opts.data);
        }
        var postOpts = {
            type: opts.type,
            url: self.baseUrl + url,
            data: opts.data,
            //processData: false,
            //mimeType: opts.mimeType || false,
            dataType: opts.dataType || 'json',
            contentType: opts.contentType,
            cache: opts.cache,
            async: opts.async,
            success: function (response) {
                var beforeResponse = opts.beforeResponse(response);
                if (beforeResponse === false) return false;

                if (typeof (opts.onsuccess) == "function") {
                    var result = response;
                    if (!typeof opts.istip == 'undefined') {
                        Xms.Web.Toast(response.Content, response.IsSuccess);
                    }
                    if (!response.statusName == '404') {
                        result = Xms.Web.GetAjaxResult(response, opts)
                    }
                    opts.onsuccess.call(this, result);
                }
                dtd.resolve(response);
            }
            , error: function (xhr, textStatus, errorThrown) {
                Xms.Web.ErrorHandler(xhr, textStatus, errorThrown);
                if (typeof (opts.onerror) == "function") {
                    opts.onerror.call(this, xhr, textStatus, errorThrown);
                }
                dtd.reject(xhr, textStatus, errorThrown);
            }
        }
        if (opts._extend) {
            opts._extend(postOpts);
        }
        $.ajax(postOpts);
        return dtd.promise();
    }
}