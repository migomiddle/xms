function getFunction(code, argNames) {
    argNames = argNames || [];
    var fn = window, parts = (code || "").split(".");
    while (fn && parts.length) {
        fn = fn[parts.shift()];
    }
    if (typeof (fn) === "function") {
        return fn;
    }
    argNames.push(code);
    return Function.constructor.apply(null, argNames);
}
if (typeof Array.prototype.unique === "undefined") {//数组去重
    Array.prototype.unique = function () {
        var res = [];
        var json = {};
        for (var i = 0; i < this.length; i++) {
            if (!json[this[i]]) {
                res.push(this[i]);
                json[this[i]] = 1;
            }
        }
        return res;
    }
}
if (typeof Date.prototype.format === "undefined") {
    Date.prototype.format = function (format) {
        var o = {
            "M+": this.getMonth() + 1, //month
            "d+": this.getDate(), //day
            "h+": this.getHours(), //hour
            "m+": this.getMinutes(), //minute
            "s+": this.getSeconds(), //second
            "q+": Math.floor((this.getMonth() + 3) / 3), //quarter
            "S": this.getMilliseconds() //millisecond
        }

        if (/(y+)/.test(format)) {
            // console.log(this)
            format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        }

        for (var k in o) {
            if (new RegExp("(" + k + ")").test(format)) {
                format = format.replace(RegExp.$1, RegExp.$1.length == 1 ? o[k] : ("00" + o[k]).substr(("" + o[k]).length));
            }
        }
        return format;
    }
}
if (typeof event === 'undefined') {
    var event = window.event;
}
if (typeof Date.prototype.DateAdd === "undefined") {
    //+---------------------------------------------------

    //| 日期计算

    //+---------------------------------------------------
    Date.prototype.DateAdd = function (strInterval, Number) {
        var dtTmp = this;

        switch (strInterval) {
            case 's': return new Date(Date.parse(dtTmp) + (1000 * Number));

            case 'n': return new Date(Date.parse(dtTmp) + (60000 * Number));

            case 'h': return new Date(Date.parse(dtTmp) + (3600000 * Number));

            case 'd': return new Date(Date.parse(dtTmp) + (86400000 * Number));

            case 'w': return new Date(Date.parse(dtTmp) + ((86400000 * 7) * Number));

            case 'q': return new Date(dtTmp.getFullYear(), (dtTmp.getMonth()) + Number * 3, dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());

            case 'm': return new Date(dtTmp.getFullYear(), (dtTmp.getMonth()) + Number, dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());

            case 'y': return new Date((dtTmp.getFullYear() + Number), dtTmp.getMonth(), dtTmp.getDate(), dtTmp.getHours(), dtTmp.getMinutes(), dtTmp.getSeconds());
        }
    }
}
if (typeof Date.prototype.DateDiff === "undefined") {
    //+---------------------------------------------------

    //| 比较日期差 dtEnd 格式为日期型或者 有效日期格式字符串

    //+---------------------------------------------------

    Date.prototype.DateDiff = function (strInterval, dtEnd) {
        var dtStart = this;

        if (typeof dtEnd == 'string')//如果是字符串转换为日期型

        {
            dtEnd = StringToDate(dtEnd);
        }

        switch (strInterval) {
            case 's': return parseInt((dtEnd - dtStart) / 1000);

            case 'n': return parseInt((dtEnd - dtStart) / 60000);

            case 'h': return parseInt((dtEnd - dtStart) / 3600000);

            case 'd': return parseInt((dtEnd - dtStart) / 86400000);

            case 'w': return parseInt((dtEnd - dtStart) / (86400000 * 7));

            case 'm': return (dtEnd.getMonth() + 1) + ((dtEnd.getFullYear() - dtStart.getFullYear()) * 12) - (dtStart.getMonth() + 1);

            case 'y': return dtEnd.getFullYear() - dtStart.getFullYear();
        }
    }
}

(function ($) {
    $.extend({
        queryBykeyValue: function (list, key, value, isFirst) {
            var res = [];
            if (!isFirst) {
                res = $.grep(list, function (n, i) {
                    return n[key] == value;
                })
            } else {
                $.each(list, function (i, n) {
                    if (n[key] == value) {
                        res.push(n);
                        return false;
                    }
                })
            }
            return res;
        },
        indexBykeyValue: function (list, key, value) {
            var index = -1;

            $.each(list, function (i, n) {
                if (n[key] == value) {
                    index = i;
                    return false;
                }
            })

            return index;
        },
        addClassToElement: function ($context, type, styles) {
            $context = $context || $('body');
            type = type || 'append';
            if (styles) {
                var $style = $('<style></style>');
                $style.html(styles);
                $context[type]($style);
            }
        },
        changeobjtoStyle: function (obj, classname, pre) {
            var labelstyles = [];
            if (!obj) { return '' };
            pre = (typeof pre !== 'undefined') ? pre : ' .';
            classname && labelstyles.push(pre + classname + ' {');
            for (var i in obj) {
                if (obj.hasOwnProperty(i)) {
                    var item = obj[i];
                    if (!isNaN(item)) {
                        labelstyles.push(i + ':' + item + 'px');
                    } else {
                        labelstyles.push(i + ':' + item);
                    }

                    labelstyles.push('; ')
                }
            }
            classname && labelstyles.push('} ')
            return labelstyles.join('');
        },
        getArrayByKey: function (arr, key, callback) {
            if (!callback) {
                return $.map(arr, function (n, i) {
                    return n[key];
                });
            } else {
                return $.map(arr, callback);
            }
        },
        deboundsEvent: function (timeout) {
            var isRun = false;
            return function (callback) {
                if (isRun == true) return false;
                isRun = true;
                callback && callback();
                setTimeout(function () {
                    isRun = false;
                }, timeout);
            }
        },
        objToUrl: function (param, key, encode) {
            if (param == null) return '';
            var paramStr = '';
            var t = typeof (param);
            if (t == 'string' || t == 'number' || t == 'boolean') {
                paramStr += '&' + key + '=' + ((encode == null || encode) ? encodeURIComponent(param) : param);
            } else {
                for (var i in param) {
                    var k = key == null ? i : key + (param instanceof Array ? '[' + i + ']' : '.' + i)
                    paramStr += $.objToUrl(param[i], k, encode)
                }
            }
            return paramStr;
        },
        fullHeight: function ($obj, offsetH) {
            offsetH = offsetH || 0;
            if ($obj && $obj.length > 0) {
                var winHeight = $(window).height();
                var offset = $obj.offset();
                $obj.css('height', winHeight - offset.top + offsetH);
            }
        }
    })
})(jQuery);

(function ($) {
    $.getUrlParam = function (name, url) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        var r = url ? url.match(reg) : window.location.search.substr(1).match(reg);
        if (r != null) return unescape(r[2]); return null;
    }
    $.setUrlParam = function (oldUrl, paramName, replaceWith) {
        oldUrl = oldUrl.toLowerCase();
        paramName = paramName.toLowerCase();

        if ($.hasParameter(oldUrl, paramName)) {
            var re = eval('/[\\&\\?]+(' + paramName + '=)([^&]*)/gi');
            var parReg = new RegExp("[\\?]" + paramName + "=");
            //console.log(replaceWith==null)
            if (replaceWith == null) {
                if (parReg.test(oldUrl)) {
                    replaceWith = "?";
                    re = eval('/[\\&\\?]+(' + paramName + '=)([^&]*)&+/gi');
                } else {
                    replaceWith = "";
                }
            } else {
                replaceWith = (parReg.test(oldUrl) ? "?" + paramName + '=' + replaceWith : "&" + paramName + '=' + replaceWith);
            }
            var nUrl = oldUrl.replace(re, replaceWith);
            return nUrl;
        }
        if (oldUrl.indexOf('?') > 0) {
            return oldUrl += '&' + paramName + '=' + replaceWith;
        }
        else {
            return oldUrl += '?' + paramName + '=' + replaceWith;
        }
    }
    $.hasParameter = function (url, name) {
        var reg = new RegExp("[\&\?]" + name + "=([^&]*)(&|$)");
        return reg.test(url);
    }
    $.urlParamObj = function (url) {
        if (url.indexOf('?') != -1) {
            var arr1 = url.split("?");
            var params = arr1[1].split("&");
            var obj = {};//声明对象
            for (var i = 0; i < params.length; i++) {
                var param = params[i].split("=");
                obj[param[0]] = param[1];//为对象赋值
            }
            return obj;
        } else {
            return {};
        }
    }
})(jQuery);
//克隆节点
// Textarea and select clone() bug workaround | Spencer Tipping
// Licensed under the terms of the MIT source code license

// Motivation.
// jQuery's clone() method works in most cases, but it fails to copy the value of textareas and select elements. This patch replaces jQuery's clone() method with a wrapper that fills in the
// values after the fact.

// An interesting error case submitted by Piotr Przybyl: If two <select> options had the same value, the clone() method would select the wrong one in the cloned box. The fix, suggested by Piotr
// and implemented here, is to use the selectedIndex property on the <select> box itself rather than relying on jQuery's value-based val().

(function (original) {
    jQuery.fn.clone = function () {
        var result = original.apply(this, arguments),
            my_textareas = this.find('textarea').add(this.filter('textarea')),
            result_textareas = result.find('textarea').add(result.filter('textarea')),
            my_selects = this.find('select').add(this.filter('select')),
            result_selects = result.find('select').add(result.filter('select'));

        for (var i = 0, l = my_textareas.length; i < l; ++i) $(result_textareas[i]).val($(my_textareas[i]).val());
        for (var i = 0, l = my_selects.length; i < l; ++i) {
            for (var j = 0, m = my_selects[i].options.length; j < m; ++j) {
                if (my_selects[i].options[j].selected === true) {
                    result_selects[i].options[j].selected = true;
                }
            }
        }
        return result;
    };
})(jQuery.fn.clone);

//ajax
(function ($) {
    //ajax查询
    $.fn.ajaxSearch = function (targetId, onsuccess, onerror) {
        var self = $(this);
        self.ajaxForm({
            success: function (response) {
                $(targetId).html($(response).find(targetId).html());
                if (typeof (onsuccess) == "function") {//成功回调方法
                    onsuccess.call(this, response);
                }
            },
            error: function (XmlHttpRequest, textStatus, errorThrown) {
                console.log(XmlHttpRequest);
                $.messager.popup(XmlHttpRequest);
                if (typeof (onerror) == "function") {//失败回调方法
                    onerror.call(this, XmlHttpRequest);
                }
            }
        });
    }
    //ajax加载数据列表
    $.fn.ajaxTable = function () {
        //$('table[data-ajax="true"],span[data-ajax="true"]')
        //$(selector).each(function () {
        var self = $(this);
        var containerId = '#' + self.data('ajaxcontainer');
        var callback = getFunction(self.data('ajaxcallback'));

        $(containerId).parent().delegate(containerId + ' a[data-ajax="true"]', 'click', function () {
            //console.log(new Date(),$(this).attr('href'));
            $(containerId).ajaxLoad($(this).attr('href'), containerId, callback);
            return false;
        });
        //})
    }
    $.fn.ajaxLoad = function (url, containerId, callback) {
        url = url + (url.indexOf('?') == -1 ? '?' : '&') + '__r=' + new Date().getTime();

        $('<div/>').load(url + ' ' + containerId, function (data, status, xhr) {
            //console.log(data,$(this).html());
            $(containerId).replaceWith($(this).html());
            if (typeof (callback) === 'function') {
                callback.apply(this, arguments);
            }
        });
        return this;
    }
})(jQuery);

//controls
(function ($) {
    $.lookup = {
        defaults: {
            disabled: true
            , dialog: function () { }
            , clear: function () { }
            , bindEvent: null,
            isShowClear: true,
            isShowSearch: false,
            searchOpts: {},
            dialogCallBack: null,
            isDefaultSearch: false,
            preDialog: null,
            btnDisabled: false
        }
    };
    $.fn.lookup = function (options) {
        var options = $.extend({}, $.lookup.defaults, options);
        //console.log(options);
        return this.each(function () {
            var self = $(this);
            if (self.attr('data-lookupinit')) {
                //return false;
                var sParent = self.parents('.typeahead__container');
                sParent.find('button[name=lookupBtn]').off().on('click', function () { options.preDialog && getFunction(options.preDialog()); getFunction(options.dialog()); });
                sParent.delegate('button[name=clearBtn]', 'click', function () { getFunction(options.clear()); });
            } else {
                self.addClass('typeahead');

                self.attr('data-lookupinit', true);
                var el = new Array();
                el.push('<div class="typeahead__container">');
                el.push('<div class="typeahead__field input-group">');
                el.push('<span class="typeahead__query">');
                //el.push('<span id="' + _cell.Control.Name + '_text"></span>');
                el.push('</span>');
                el.push('<span class="input-group-btn">');
                if (options.isShowClear) {
                    el.push('<button type="button" name="clearBtn" class="btn btn-default btn-sm" title="clear">');
                    el.push('<span class="glyphicon glyphicon-remove-sign"></span>');
                    el.push('</button>');
                }
                el.push('<button type="button" name="lookupBtn" class="btn btn-default btn-sm" title="find">');
                el.push('<span class="glyphicon glyphicon-search"></span>');
                el.push('</button>');
                el.push('</span>');
                el.push('</div>');
                el.push('</div>');
                var o = $(el.join(''));
                var selfClone = self.clone();
                o.find('.typeahead__query').append(selfClone);

                if (options.disabled) {
                    selfClone.prop('disabled', true);
                }
                else {
                    selfClone.prop('disabled', false);
                }
                //console.log(options);
                if (options.btnDisabled) {
                    o.find("button[name='clearBtn']").prop('disabled', true);
                    o.find("button[name='lookupBtn']").prop('disabled', true);
                } else {
                    o.find("button[name='clearBtn']").prop('disabled', false);
                    o.find("button[name='lookupBtn']").prop('disabled', false);
                }

                o.find('button[name=lookupBtn]').off().on('click', function () {
                    getFunction(options.dialog(selfClone, function () {
                        if (options.isShowSearch) {
                            //$(selfClone).trigger("searchDialog", {});
                        }
                        //options.searchOpts.dialogCallBack && options.searchOpts.dialogCallBack();
                    }));
                });
                // console.log(options.searchOpts)
                o.delegate('button[name=clearBtn]', 'click', function () { getFunction(options.clear()); $(selfClone).trigger('lookup.clear'); });
                self.replaceWith(o);
                if (options.isShowSearch && typeof $.fn.xmsSelecteDown !== "undefined") {
                    $(selfClone).xmsSelecteDown(options.searchOpts);
                }
                if (options.isDefaultSearch && options.isShowSearch) {
                    $(selfClone).trigger("searchDialog", { isDefault: true });
                }
                if (options.bindEvent) {
                    options.bindEvent(selfClone);
                }
            }
        });
    };
    $.picklist = {
        defaults: {
            displaytype: 'select'
            , required: false,
            isDefault: false
            , items: [],
            changeHandler: null,
            isReadonly: false,
            multi: null,
            placeholder: ''
        }
    };
    $.fn.picklist = function (options) {
        var options = $.extend({}, $.picklist.defaults, options);
        return this.each(function () {
            var self = $(this);
            if (self.attr('data-picklistinit')) { return; }
            var el = new Array();
            var rqClass = options.required ? ' required' : '';
            var value = self.val();
            var o = null;
            var timeSt = new Date() * 1;
            var flagDefault = false;
            var _thisid = 'picklist' + timeSt;
            if (options.displaytype == 'select') {
                o = $('<select class="form-control input-sm' + rqClass + '" id="' + _thisid + '" onchange="$(this).prev().val($(this).val());$(this).prev().attr(\'data-value\',$(this).val())">');
                if (options.required == false) {
                    el.push('<option value=""' + (value == "" ? " selected" : "") + '>' + options.placeholder + '</option>');
                    // flagDefault = true;
                }
                $(options.items).each(function (i, n) {
                    var v = n.value || n.Value;
                    if (n.value == 0) {
                        v = 0;
                    }
                    var text = n.text || n.Text || n.Name || n.name || n.Label || n.label || n.Key || n.key || '';
                    var selected = value == "" ? (n.IsSelected ? " selected" : "") : (v == value ? ' selected' : '');
                    if (options.required == true && flagDefault == true) {
                        flagDefault = true;
                        el.push('<option value="' + v + '" selected>' + text + '</option>');
                    } else if (selected != "") {
                        el.push('<option value="' + v + '" selected>' + text + '</option>');
                    } else {
                        el.push('<option value="' + v + '">' + text + '</option>');
                    }
                });
                el.push('</select>');
                o.html(el.join(''));
                if (options.isDefault) {
                    o.find('option[value="' + self.val() + '"]').prop("selected", true);
                }
                if (options.isReadonly) {
                    o.prop("disabled", true);
                }
                // if (options.required || options.isDefault) {
                //  self.val(o.find('option:selected').val());
                //  }
                if (options.changeHandler) {
                    o.off('change').on('change', function (e, obj) {
                        options.changeHandler.call(this, e, obj);
                        self.trigger('picklist.change', { target: o });
                    });
                }
                o.on('change', function (e, obj) {
                    self.trigger('change');
                });
                if (options.multi) {
                    if ($.fn.SumoSelect) {
                        o.attr('multiple', 'multiple');
                        if (value != "") {
                            if (value.indexOf(',') > -1) {
                                var temp = value.split(',');
                                $.each(temp, function (key, item) {
                                    o.find('option[value="' + item + '"]').prop("selected", true);
                                });
                            } else {
                                o.find('option[value="' + value + '"]').prop("selected", true);
                            }
                        }
                        setTimeout(function () {
                            o.SumoSelect(options.multi);
                            o.on('sumo.ok', function () {
                                self.val(o.val());
                                self.attr('data-value', o.val());
                            })
                        }, 10);
                    }
                }
                self.trigger("picklist.getTarget", { target: o, picker: self });
            }
            else if (options.displaytype == 'checkbox') {
                var name = self.prop('name');
                $(options.items).each(function (i, n) {
                    var v = n.value || n.Value;
                    var text = n.text || n.Text || n.Name || n.name || n.Label || n.label || n.Key || n.key;
                    var selected = v == value ? ' checked' : '';
                    el.push('<label class="checkbox-inline"><input type="checkbox" data-sourceid="' + (self.attr("id") || _thisid) + '" name="' + (name + new Date().getTime()) + '" value="' + v + '"' + selected + '>' + text + '</label>');
                });
                o = $(el.join(''));
                o.find('input').bind('click', function () {
                    var flag = $(this).prop('checked');
                    if (flag) {
                        self.val($(this).val());
                    }
                });
                if (options.required) {
                    self.val(o.find('input:checked').val());
                }
            }
            else if (options.displaytype == 'radio') {
                var name = self.prop('name');
                var inputname = name + new Date().getTime();
                console.log('name', value)
                $(options.items).each(function (i, n) {
                    var v = n.value || n.Value;
                    var text = n.text || n.Text || n.Name || n.name || n.Label || n.label || n.Key || n.key;
                    var selected = v == value ? ' checked' : '';

                    console.log(i)
                    if (!options.itemTmpl) {
                        el.push('<label class="checkbox-inline"><input type="radio" data-sourceid="' + (self.attr("id") || _thisid) + '" name="' + inputname + '" value="' + v + '"' + selected + '>' + text + '</label>');
                    } else {
                        el.push(options.itemTmpl(self, _thisid, name, v, selected, text, i));
                    }
                });
                o = $(el.join(''));
                o.find('input').bind('click', function () {
                    var flag = $(this).prop('checked');
                    if (flag) {
                        self.val($(this).val()).trigger('change');
                    }
                }).on('change', function () {
                    self.trigger('change');
                })
                if (options.required) {
                    self.val(o.find('input:checked').val());
                }
            }
            if (self.prop('readonly')) {
                o.prop('readonly', 'readonly');
                o.find('input').prop('readonly', 'readonly');
            }
            if (self.prop('disabled')) {
                o.prop('disabled', 'disabled');
                o.find('input').prop('disabled', 'disabled');
            }
            self.attr('data-picklistinit', true);
            self.after(o);
            if (options.displaytype == 'select') {
                self.attr('data-value', $(o).find('option:selected').val());
                self.val($(o).find('option:selected').val());
            }
            self.addClass('hide');
            self.attr('data-instance', '#' + o.prop('id'));
            self.parent().prev().find('label').prop('for', o.prop('id'));
            return o;
        });
    };
})(jQuery);
(function (original) {
    jQuery.fn.text = function (val) {
        var s = $(this), isset = (val != undefined), v = val || null;
        if (s && s.length > 0) {
            if (isset) {
                s.get(0).innerHTML = v;
                return s;
            }
            else {
                return s.get(0).innerHTML;
            }
        }
    };
})(jQuery.fn.text);

; (function ($, root) {
    "use strict"
    var setting = {
        event: "click",
        position: "absolute",
        openCb: null,
        closeCb: null
    }

    $.fn.sliderRight = function (opts) {
        opts = $.extend({}, setting, opts);
        var bodyDom = $(document.body);
        return this.each(function () {
            var $this = $(this), target = $this.attr("data-target");
            $this.on(opts.event, function () {
                bodyDom.toggleClass("menu-open");
                if (bodyDom.hasClass("menu-open")) {
                    opts.openCb && opts.openCb($this);
                } else {
                    opts.closeCb && opts.closeCb($this);
                }
            });
        });
    }
})(jQuery, window);

; (function ($, root) {
    "use strict"
    function getChartsData(data, opts) {
        console.log('chartConfig', opts);
        var type = '', title = '', res = {};
        // data = { "title": "客户类别分析图", "subtitle": "所有客户", "legend": ["客户"], "xaxis": { "type": "category", "data": ["潜在客户", "意向客户", "目标客户", "签约客户", "准客户"] }, "series": [{ "name": "客户", "itemcolor": "", "type": "funnel", "data": ["48374", "8", "521", "11279", "1"], "itemStyle": { "normal": { "color": "" } } }] };
        // data = { "title": "hddg", "subtitle": "客户", "legend": ["客户"], "xaxis": { "type": "category", "data": ["2016-12", "2017-01", "2017-02", "2017-03", "2017-04", "2017-05", "2017-07", "2017-08", "2017-09", "2017-10", "2017-11"] }, "series": [{ "name": "客户", "itemcolor": "", "type": "gauge", "data": ["3", "3", "4", "3", "1", "1", "1", "5", "11", "1", "1"], "itemStyle": { "normal": { "color": "" } } }] };
        data.xaxis.axisLabel = {
            interval: 0,
            rotate: 45
        };
        data.series[0].itemStyle = {
            normal: {
                label: {
                    show: true,
                    position: 'top'
                }
            }
        }
        console.log('rpecharttype(data):', data);
        type = getChartType(data);
        console.log('getChartType(data):', data);
        res = setChart[type](data, opts);

        return res;
    }
    function getChartType(data) {
        if (!data) return false;
        var res = "";
        if (data.series) {
            var seriesLength = data.series.length;
            if (seriesLength == 1) {
                res = data.series[0].type;
            } else if (seriesLength >= 2) {
                if (data.series[0].type == data.series[1].type) {
                    res = data.series[0].type;
                } else {
                    res = "mix";
                }
            }
        }
        return res;
    }
    var colorList = ['#ff7f50', '#87cefa', '#da70d6', '#32cd32', '#6495ed',
        '#ff69b4', '#ba55d3', '#cd5c5c', '#ffa500', '#40e0d0',
        '#1e90ff', '#ff6347', '#7b68ee', '#00fa9a', '#ffd700',
        '#6699FF', '#ff6666', '#3cb371', '#b8860b', '#30e0e0',
        '#ff7f50', '#87cefa', '#da70d6', '#32cd32', '#6495ed',
        '#ff69b4', '#ba55d3', '#cd5c5c', '#ffa500', '#40e0d0',
        '#1e90ff', '#ff6347', '#7b68ee', '#00fa9a', '#ffd700',];
    function getRandom(min, max) {
        return Math.floor(Math.random() * max) + min;
    }
    var setChart = {};
    setChart["bar"] = function (data, opts) {
        var res = {
            //backgroundColor: '#2c343c',
            itemStyle: { normal: { color: colorList[getRandom(0, 20)] } },
            title: {
                text: data.title,
                subtext: data.subtitle || ""
            },
            tooltip: {},
            toolbox: {
                show: true,
                feature: {
                    myTool1: {
                        show: true,
                        title: '查看列表',
                        icon: 'path:M134.095238 256C147.560466 256 158.476191 246.448741 158.476191 234.666667 158.476191 222.884592 147.560466 213.333333 134.095238 213.333333L36.571428 213.333333C23.1062 213.333333 12.190476 222.884592 12.190476 234.666667 12.190476 246.448741 23.1062 256 36.571428 256L134.095238 256Z,path:M134.095238 533.333333C147.560466 533.333333 158.476191 523.782074 158.476191 512 158.476191 500.217926 147.560466 490.666667 134.095238 490.666667L36.571428 490.666667C23.1062 490.666667 12.190476 500.217926 12.190476 512 12.190476 523.782074 23.1062 533.333333 36.571428 533.333333L134.095238 533.333333Z,path:M134.095238 810.666667C147.560466 810.666667 158.476191 801.115407 158.476191 789.333333 158.476191 777.55126 147.560466 768 134.095238 768L36.571428 768C23.1062 768 12.190476 777.55126 12.190476 789.333333 12.190476 801.115407 23.1062 810.666667 36.571428 810.666667L134.095238 810.666667Z,path:M987.428572 256C1000.893801 256 1011.809523 246.448741 1011.809523 234.666667 1011.809523 222.884592 1000.893801 213.333333 987.428572 213.333333L280.380951 213.333333C266.915725 213.333333 256 222.884592 256 234.666667 256 246.448741 266.915725 256 280.380951 256L987.428572 256Z,path:M993.52381 533.333333C1006.989037 533.333333 1017.904762 523.782074 1017.904762 512 1017.904762 500.217926 1006.989037 490.666667 993.52381 490.666667L286.47619 490.666667C273.010963 490.666667 262.095238 500.217926 262.095238 512 262.095238 523.782074 273.010963 533.333333 286.47619 533.333333L993.52381 533.333333Z,path:M996.571428 810.666667C1010.036657 810.666667 1020.952382 801.115407 1020.952382 789.333333 1020.952382 777.55126 1010.036657 768 996.571428 768L289.52381 768C276.058581 768 265.142857 777.55126 265.142857 789.333333 265.142857 801.115407 276.058581 810.666667 289.52381 810.666667L996.571428 810.666667Z',
                        onclick: function (a, b, d, event) {
                            console.log(event, d)
                            var target = event.event.target;
                            if (target) {
                                var par = $(target).parents('.xms-chart');
                                var queryid = par.attr('data-queryid');
                                location.href = ORG_SERVERURL + '/entity/list?queryviewid=' + queryid;//xms没有entityname会有问题
                            }
                        }
                    },
                    // dataView: { readOnly: false },
                    restore: {},
                    saveAsImage: {}
                },
                right: 30
            },
            legend: {
                orient: 'vertical',
                x: 'right',
                y: 'center',
                data: data.legend
            },
            xAxis: data.xaxis,
            yAxis: data.yaxis || {},
            series: data.series
        }

        $.each(res.series, function (key, item) {
            item.type = item.type.toLowerCase();
        });

        res.xAxis.type = res.xAxis.type.toLowerCase();
        return res;
    }
    setChart["hbar"] = function (data, opts) {
        var res = {
            //backgroundColor: '#2c343c',
            itemStyle: { normal: { color: colorList[getRandom(0, 20)] } },
            title: {
                text: data.title,
                subtext: data.subtitle || ""
            },
            tooltip: {},
            toolbox: {
                show: true,
                feature: {
                    myTool1: {
                        show: true,
                        title: '查看列表',
                        icon: 'path:M134.095238 256C147.560466 256 158.476191 246.448741 158.476191 234.666667 158.476191 222.884592 147.560466 213.333333 134.095238 213.333333L36.571428 213.333333C23.1062 213.333333 12.190476 222.884592 12.190476 234.666667 12.190476 246.448741 23.1062 256 36.571428 256L134.095238 256Z,path:M134.095238 533.333333C147.560466 533.333333 158.476191 523.782074 158.476191 512 158.476191 500.217926 147.560466 490.666667 134.095238 490.666667L36.571428 490.666667C23.1062 490.666667 12.190476 500.217926 12.190476 512 12.190476 523.782074 23.1062 533.333333 36.571428 533.333333L134.095238 533.333333Z,path:M134.095238 810.666667C147.560466 810.666667 158.476191 801.115407 158.476191 789.333333 158.476191 777.55126 147.560466 768 134.095238 768L36.571428 768C23.1062 768 12.190476 777.55126 12.190476 789.333333 12.190476 801.115407 23.1062 810.666667 36.571428 810.666667L134.095238 810.666667Z,path:M987.428572 256C1000.893801 256 1011.809523 246.448741 1011.809523 234.666667 1011.809523 222.884592 1000.893801 213.333333 987.428572 213.333333L280.380951 213.333333C266.915725 213.333333 256 222.884592 256 234.666667 256 246.448741 266.915725 256 280.380951 256L987.428572 256Z,path:M993.52381 533.333333C1006.989037 533.333333 1017.904762 523.782074 1017.904762 512 1017.904762 500.217926 1006.989037 490.666667 993.52381 490.666667L286.47619 490.666667C273.010963 490.666667 262.095238 500.217926 262.095238 512 262.095238 523.782074 273.010963 533.333333 286.47619 533.333333L993.52381 533.333333Z,path:M996.571428 810.666667C1010.036657 810.666667 1020.952382 801.115407 1020.952382 789.333333 1020.952382 777.55126 1010.036657 768 996.571428 768L289.52381 768C276.058581 768 265.142857 777.55126 265.142857 789.333333 265.142857 801.115407 276.058581 810.666667 289.52381 810.666667L996.571428 810.666667Z',
                        onclick: function (a, b, d, event) {
                            console.log(event, d)
                            var target = event.event.target;
                            if (target) {
                                var par = $(target).parents('.xms-chart');
                                var queryid = par.attr('data-queryid');
                                location.href = ORG_SERVERURL + '/entity/list?queryviewid=' + queryid;
                            }
                        }
                    },
                    //dataView: { readOnly: false },
                    restore: {},
                    saveAsImage: {}
                },
                right: 30
            },
            legend: {
                orient: 'vertical',
                x: 'right',
                y: 'center',
                data: data.legend
            },
            xAxis: {
                type: 'value',
                boundaryGap: [0, 0.01],
                position: 'right'
            },
            yAxis: data.xaxis || {},
            series: data.series
        }
        console.log('series', res.series);
        var labelRight = {
            normal: {
                position: 'right'
            }
        };
        $.each(res.series, function (key, item) {
            if (item.type == "bar" || item.type == "hbar") {
                item.type = "bar";
            } else if (item.type == "Line") {
                item.type = "line";
            }
            $.each(item.data, function (i, n) {
                var obj = {};
                if (!obj.label) {
                    obj.label = {};
                }
                obj.value = n;
                obj.label = labelRight;
                item.data[i] = obj;
            });
            //排序用到，由于echart是由下到上画数据条，所以需要倒序排
            if (opts.fetch && opts.fetch.length > 0 && opts.fetch[0].topdirection != "") {
                item.data.reverse();
            }
            // item.data.reverse();
        });
        res.yAxis.type = res.yAxis.type.toLowerCase();
        return res;
    }
    setChart["line"] = function (data, opts) {
        var res = {
            //backgroundColor: '#2c343c',
            itemStyle: { normal: { color: colorList[getRandom(0, 20)] } },
            title: {
                text: data.title,
                subtext: data.subtitle || ""
            },
            tooltip: {},
            toolbox: {
                show: true,
                feature: {
                    myTool1: {
                        show: true,
                        title: '查看列表',
                        icon: 'path:M134.095238 256C147.560466 256 158.476191 246.448741 158.476191 234.666667 158.476191 222.884592 147.560466 213.333333 134.095238 213.333333L36.571428 213.333333C23.1062 213.333333 12.190476 222.884592 12.190476 234.666667 12.190476 246.448741 23.1062 256 36.571428 256L134.095238 256Z,path:M134.095238 533.333333C147.560466 533.333333 158.476191 523.782074 158.476191 512 158.476191 500.217926 147.560466 490.666667 134.095238 490.666667L36.571428 490.666667C23.1062 490.666667 12.190476 500.217926 12.190476 512 12.190476 523.782074 23.1062 533.333333 36.571428 533.333333L134.095238 533.333333Z,path:M134.095238 810.666667C147.560466 810.666667 158.476191 801.115407 158.476191 789.333333 158.476191 777.55126 147.560466 768 134.095238 768L36.571428 768C23.1062 768 12.190476 777.55126 12.190476 789.333333 12.190476 801.115407 23.1062 810.666667 36.571428 810.666667L134.095238 810.666667Z,path:M987.428572 256C1000.893801 256 1011.809523 246.448741 1011.809523 234.666667 1011.809523 222.884592 1000.893801 213.333333 987.428572 213.333333L280.380951 213.333333C266.915725 213.333333 256 222.884592 256 234.666667 256 246.448741 266.915725 256 280.380951 256L987.428572 256Z,path:M993.52381 533.333333C1006.989037 533.333333 1017.904762 523.782074 1017.904762 512 1017.904762 500.217926 1006.989037 490.666667 993.52381 490.666667L286.47619 490.666667C273.010963 490.666667 262.095238 500.217926 262.095238 512 262.095238 523.782074 273.010963 533.333333 286.47619 533.333333L993.52381 533.333333Z,path:M996.571428 810.666667C1010.036657 810.666667 1020.952382 801.115407 1020.952382 789.333333 1020.952382 777.55126 1010.036657 768 996.571428 768L289.52381 768C276.058581 768 265.142857 777.55126 265.142857 789.333333 265.142857 801.115407 276.058581 810.666667 289.52381 810.666667L996.571428 810.666667Z',
                        onclick: function (a, b, d, event) {
                            console.log(event, d)
                            var target = event.event.target;
                            if (target) {
                                var par = $(target).parents('.xms-chart');
                                var queryid = par.attr('data-queryid');
                                location.href = ORG_SERVERURL + '/entity/list?queryviewid=' + queryid;
                            }
                        }
                    },
                    // dataView: { readOnly: false },
                    restore: {},
                    saveAsImage: {}
                },
                right: 30
            },
            legend: {
                orient: 'vertical',
                x: 'right',
                y: 'center',
                data: data.legend
            },
            xAxis: data.xaxis,
            yAxis: data.yaxis || {},
            series: data.series
        }
        $.each(res.series, function (key, item) {
            item.type = item.type.toLowerCase();
        });
        res.xAxis.type = res.xAxis.type.toLowerCase();
        return res;
    }
    setChart["mix"] = function (data, opts) {
        var res = {
            //backgroundColor: '#2c343c',
            itemStyle: { normal: { color: colorList[getRandom(0, 20)] } },
            title: {
                text: data.title,
                subtext: data.subtitle || ""
            },
            tooltip: {},
            toolbox: {
                show: true,
                feature: {
                    myTool1: {
                        show: true,
                        title: '查看列表',
                        icon: 'path:M134.095238 256C147.560466 256 158.476191 246.448741 158.476191 234.666667 158.476191 222.884592 147.560466 213.333333 134.095238 213.333333L36.571428 213.333333C23.1062 213.333333 12.190476 222.884592 12.190476 234.666667 12.190476 246.448741 23.1062 256 36.571428 256L134.095238 256Z,path:M134.095238 533.333333C147.560466 533.333333 158.476191 523.782074 158.476191 512 158.476191 500.217926 147.560466 490.666667 134.095238 490.666667L36.571428 490.666667C23.1062 490.666667 12.190476 500.217926 12.190476 512 12.190476 523.782074 23.1062 533.333333 36.571428 533.333333L134.095238 533.333333Z,path:M134.095238 810.666667C147.560466 810.666667 158.476191 801.115407 158.476191 789.333333 158.476191 777.55126 147.560466 768 134.095238 768L36.571428 768C23.1062 768 12.190476 777.55126 12.190476 789.333333 12.190476 801.115407 23.1062 810.666667 36.571428 810.666667L134.095238 810.666667Z,path:M987.428572 256C1000.893801 256 1011.809523 246.448741 1011.809523 234.666667 1011.809523 222.884592 1000.893801 213.333333 987.428572 213.333333L280.380951 213.333333C266.915725 213.333333 256 222.884592 256 234.666667 256 246.448741 266.915725 256 280.380951 256L987.428572 256Z,path:M993.52381 533.333333C1006.989037 533.333333 1017.904762 523.782074 1017.904762 512 1017.904762 500.217926 1006.989037 490.666667 993.52381 490.666667L286.47619 490.666667C273.010963 490.666667 262.095238 500.217926 262.095238 512 262.095238 523.782074 273.010963 533.333333 286.47619 533.333333L993.52381 533.333333Z,path:M996.571428 810.666667C1010.036657 810.666667 1020.952382 801.115407 1020.952382 789.333333 1020.952382 777.55126 1010.036657 768 996.571428 768L289.52381 768C276.058581 768 265.142857 777.55126 265.142857 789.333333 265.142857 801.115407 276.058581 810.666667 289.52381 810.666667L996.571428 810.666667Z',
                        onclick: function (a, b, d, event) {
                            console.log(event, d)
                            var target = event.event.target;
                            if (target) {
                                var par = $(target).parents('.xms-chart');
                                var queryid = par.attr('data-queryid');
                                location.href = ORG_SERVERURL + '/entity/list?queryviewid=' + queryid;
                            }
                        }
                    },
                    // dataView: { readOnly: false },
                    restore: {},
                    saveAsImage: {}
                },
                right: 30
            },
            legend: {
                orient: 'vertical',
                x: 'right',
                y: 'center',
                data: data.legend
            },
            xAxis: data.xaxis,
            yAxis: data.yaxis || {},
            series: data.series
        }
        $.each(res.series, function (key, item) {
            item.type = item.type.toLowerCase();
        });
        res.xAxis.type = res.xAxis.type.toLowerCase();
        return res;
    }
    setChart["pie"] = function (data, opts) {
        var res = {
            //backgroundColor: '#2c343c',
            itemStyle: {
                normal: {
                    color: function (params) {
                        return colorList[params.dataIndex]
                    }
                }
            },
            title: {
                text: data.title,
                subtext: data.subtitle || ""
            },
            tooltip: {},
            toolbox: {
                show: true,
                feature: {
                    myTool1: {
                        show: true,
                        title: '查看列表',
                        icon: 'path:M134.095238 256C147.560466 256 158.476191 246.448741 158.476191 234.666667 158.476191 222.884592 147.560466 213.333333 134.095238 213.333333L36.571428 213.333333C23.1062 213.333333 12.190476 222.884592 12.190476 234.666667 12.190476 246.448741 23.1062 256 36.571428 256L134.095238 256Z,path:M134.095238 533.333333C147.560466 533.333333 158.476191 523.782074 158.476191 512 158.476191 500.217926 147.560466 490.666667 134.095238 490.666667L36.571428 490.666667C23.1062 490.666667 12.190476 500.217926 12.190476 512 12.190476 523.782074 23.1062 533.333333 36.571428 533.333333L134.095238 533.333333Z,path:M134.095238 810.666667C147.560466 810.666667 158.476191 801.115407 158.476191 789.333333 158.476191 777.55126 147.560466 768 134.095238 768L36.571428 768C23.1062 768 12.190476 777.55126 12.190476 789.333333 12.190476 801.115407 23.1062 810.666667 36.571428 810.666667L134.095238 810.666667Z,path:M987.428572 256C1000.893801 256 1011.809523 246.448741 1011.809523 234.666667 1011.809523 222.884592 1000.893801 213.333333 987.428572 213.333333L280.380951 213.333333C266.915725 213.333333 256 222.884592 256 234.666667 256 246.448741 266.915725 256 280.380951 256L987.428572 256Z,path:M993.52381 533.333333C1006.989037 533.333333 1017.904762 523.782074 1017.904762 512 1017.904762 500.217926 1006.989037 490.666667 993.52381 490.666667L286.47619 490.666667C273.010963 490.666667 262.095238 500.217926 262.095238 512 262.095238 523.782074 273.010963 533.333333 286.47619 533.333333L993.52381 533.333333Z,path:M996.571428 810.666667C1010.036657 810.666667 1020.952382 801.115407 1020.952382 789.333333 1020.952382 777.55126 1010.036657 768 996.571428 768L289.52381 768C276.058581 768 265.142857 777.55126 265.142857 789.333333 265.142857 801.115407 276.058581 810.666667 289.52381 810.666667L996.571428 810.666667Z',
                        onclick: function (a, b, d, event) {
                            console.log(event, d)
                            var target = event.event.target;
                            if (target) {
                                var par = $(target).parents('.xms-chart');
                                var queryid = par.attr('data-queryid');
                                location.href = ORG_SERVERURL + '/entity/list?queryviewid=' + queryid;
                            }
                        }
                    },
                    //dataView: {
                    //    onClick: function (a,b,c,d) {
                    //        console.log(a, b, c, d);
                    //    }
                    //    //readOnly: false
                    //},
                    restore: {},
                    saveAsImage: {}
                },
                right: 30
            },
            legend: {
                orient: 'vertical',
                x: 'right',
                y: 'center',
                data: data.legend
            },
            series: data.series
        }
        var series = res.series[0];
        series.type = series.type.toLowerCase();
        series.label = {
            normal: { position: 'inside', show: true, formatter: '{b}：{c}' }
        }
        //$.extend(series.label, {}, {position:'inside',formatter:'{b}:{c}'})
        $.each(series.data, function (key, item) {
            //console.log(item)
            var obj = {};
            obj.value = item;
            if (data.xaxis.data[key]) {
                obj.name = data.xaxis.data[key];
            } else {
                obj.name = "";
            }
            series.data[key] = obj;
        });
        return res;
    }
    setChart["funnel"] = function (data, opts) {
        var res = {
            itemStyle: { normal: { color: '#31b0d5' } },
            title: {
                text: data.title,
                subtext: data.subtitle || ""
            },
            tooltip: {
                trigger: 'item',
                formatter: "{a} <br />{b} : {c}%"
            },
            toolbox: {
                feature: {
                    myTool1: {
                        show: true,
                        title: '查看列表',
                        icon: 'path:M134.095238 256C147.560466 256 158.476191 246.448741 158.476191 234.666667 158.476191 222.884592 147.560466 213.333333 134.095238 213.333333L36.571428 213.333333C23.1062 213.333333 12.190476 222.884592 12.190476 234.666667 12.190476 246.448741 23.1062 256 36.571428 256L134.095238 256Z,path:M134.095238 533.333333C147.560466 533.333333 158.476191 523.782074 158.476191 512 158.476191 500.217926 147.560466 490.666667 134.095238 490.666667L36.571428 490.666667C23.1062 490.666667 12.190476 500.217926 12.190476 512 12.190476 523.782074 23.1062 533.333333 36.571428 533.333333L134.095238 533.333333Z,path:M134.095238 810.666667C147.560466 810.666667 158.476191 801.115407 158.476191 789.333333 158.476191 777.55126 147.560466 768 134.095238 768L36.571428 768C23.1062 768 12.190476 777.55126 12.190476 789.333333 12.190476 801.115407 23.1062 810.666667 36.571428 810.666667L134.095238 810.666667Z,path:M987.428572 256C1000.893801 256 1011.809523 246.448741 1011.809523 234.666667 1011.809523 222.884592 1000.893801 213.333333 987.428572 213.333333L280.380951 213.333333C266.915725 213.333333 256 222.884592 256 234.666667 256 246.448741 266.915725 256 280.380951 256L987.428572 256Z,path:M993.52381 533.333333C1006.989037 533.333333 1017.904762 523.782074 1017.904762 512 1017.904762 500.217926 1006.989037 490.666667 993.52381 490.666667L286.47619 490.666667C273.010963 490.666667 262.095238 500.217926 262.095238 512 262.095238 523.782074 273.010963 533.333333 286.47619 533.333333L993.52381 533.333333Z,path:M996.571428 810.666667C1010.036657 810.666667 1020.952382 801.115407 1020.952382 789.333333 1020.952382 777.55126 1010.036657 768 996.571428 768L289.52381 768C276.058581 768 265.142857 777.55126 265.142857 789.333333 265.142857 801.115407 276.058581 810.666667 289.52381 810.666667L996.571428 810.666667Z',
                        onclick: function (a, b, d, event) {
                            console.log(event, d)
                            var target = event.event.target;
                            if (target) {
                                var par = $(target).parents('.xms-chart');
                                var queryid = par.attr('data-queryid');
                                location.href = ORG_SERVERURL + '/entity/list?queryviewid=' + queryid;
                            }
                        }
                    },
                    // dataView: { readOnly: false },
                    restore: {},
                    saveAsImage: {}
                },
                right: 30
            },
            legend: {
                orient: 'vertical',
                x: 'right',
                y: 'center',
                data: data.legend
            },
            calculable: false,
            noDataLoadingOption: {
                text: data.title + '\n' + data.subtitle,
                effect: 'bubble',
                effectOption: {
                    effect: {
                        n: 100 //气泡个数为100
                    }
                },
                textStyle: {
                    fontSize: 16
                }
            },
            series: [
                {
                    name: '漏斗图',
                    type: 'funnel',
                    left: '15%',
                    top: 60,
                    //x2: 80,
                    bottom: 60,
                    width: '70%',
                    // height: {totalHeight} - y - y2,
                    min: 0,
                    max: 100,
                    minSize: '0%',
                    maxSize: '100%',
                    sort: 'none',
                    gap: 2,
                    label: {
                        normal: {
                            show: true,
                            position: 'left'
                            , color: '#000',
                            textStyle: {
                                fontSize: 12
                                , color: '#000'
                            }
                        },
                        emphasis: {
                            textStyle: {
                                color: '#000'
                                , fontSize: 20
                            }
                        }
                    },
                    labelLine: {
                        normal: {
                            length: 10,
                            lineStyle: {
                                width: 1,
                                type: 'solid'
                            }
                        }
                    },
                    itemStyle: {
                        normal: {
                            borderColor: '#fff',
                            borderWidth: 1,
                            color: function (params) {
                                return colorList[params.dataIndex]
                            }
                        },
                        color: '#000',
                    },
                    data: data.series[0].data
                }
            ]
        }
        var series = res.series[0];
        series.type = series.type.toLowerCase();
        var data_min = Math.min.apply(null, series.data);
        var data_max = Math.max.apply(null, series.data);
        var data_sum = 0;
        var data_count = series.data.length;

        $.each(series.data, function (key, item) {
            data_sum += item * 1;
        });
        console.log('series.max', data_sum);
        series.max = data_sum;
        var step = (data_sum / data_count).toFixed(2);
        var valuecount = 0;
        for (var key = data_count - 1; key >= 0; key--) {
            var item = series.data[key];
            var obj = {};
            valuecount += step * 1;
            console.log('valuecount', valuecount);
            obj.value = valuecount;//(item / data_sum).toFixed(2) * 100;
            if (obj.name = data.xaxis.data[key]) {
                obj.name = obj.name = data.xaxis.data[key] + ' : ' + item;
            } else {
                obj.name = "";
            }
            series.data[key] = obj;
        }
        console.log('series.data', series.data);
        return res;
    }
    setChart["gauge"] = function (data, opts) {
        console.log('setChart["gauge"]', data);
        //return false;
        var res = {
            title: {
                text: data.title,
                subtext: data.subtitle || ""
            },
            tooltip: {},
            toolbox: {
                show: true,
                feature: {
                    myTool1: {
                        show: true,
                        title: '查看列表',
                        icon: 'path:M134.095238 256C147.560466 256 158.476191 246.448741 158.476191 234.666667 158.476191 222.884592 147.560466 213.333333 134.095238 213.333333L36.571428 213.333333C23.1062 213.333333 12.190476 222.884592 12.190476 234.666667 12.190476 246.448741 23.1062 256 36.571428 256L134.095238 256Z,path:M134.095238 533.333333C147.560466 533.333333 158.476191 523.782074 158.476191 512 158.476191 500.217926 147.560466 490.666667 134.095238 490.666667L36.571428 490.666667C23.1062 490.666667 12.190476 500.217926 12.190476 512 12.190476 523.782074 23.1062 533.333333 36.571428 533.333333L134.095238 533.333333Z,path:M134.095238 810.666667C147.560466 810.666667 158.476191 801.115407 158.476191 789.333333 158.476191 777.55126 147.560466 768 134.095238 768L36.571428 768C23.1062 768 12.190476 777.55126 12.190476 789.333333 12.190476 801.115407 23.1062 810.666667 36.571428 810.666667L134.095238 810.666667Z,path:M987.428572 256C1000.893801 256 1011.809523 246.448741 1011.809523 234.666667 1011.809523 222.884592 1000.893801 213.333333 987.428572 213.333333L280.380951 213.333333C266.915725 213.333333 256 222.884592 256 234.666667 256 246.448741 266.915725 256 280.380951 256L987.428572 256Z,path:M993.52381 533.333333C1006.989037 533.333333 1017.904762 523.782074 1017.904762 512 1017.904762 500.217926 1006.989037 490.666667 993.52381 490.666667L286.47619 490.666667C273.010963 490.666667 262.095238 500.217926 262.095238 512 262.095238 523.782074 273.010963 533.333333 286.47619 533.333333L993.52381 533.333333Z,path:M996.571428 810.666667C1010.036657 810.666667 1020.952382 801.115407 1020.952382 789.333333 1020.952382 777.55126 1010.036657 768 996.571428 768L289.52381 768C276.058581 768 265.142857 777.55126 265.142857 789.333333 265.142857 801.115407 276.058581 810.666667 289.52381 810.666667L996.571428 810.666667Z',
                        onclick: function (a, b, d, event) {
                            //console.log(event, d)
                            var target = event.event.target;
                            if (target) {
                                var par = $(target).parents('.xms-chart');
                                var queryid = par.attr('data-queryid');
                                location.href = ORG_SERVERURL + '/entity/list?queryviewid=' + queryid;
                            }
                        }
                    },
                    // dataView: { readOnly: false },
                    restore: {},
                    saveAsImage: {}
                },
                right: 30
            },

            series: [
                {
                    name: data.title,
                    type: 'gauge',
                    min: 0,
                    max: 100,
                    sort: 'none',
                    detail: { formatter: '{value}', show: true },
                    //data: data.xaxis.data,
                    axisLabel: {
                        formatter: function (v) {
                            return v.toFixed(2);
                        }
                    },
                }
            ]
        }
        // return false;
        var series = res.series[0];
        series.type = series.type.toLowerCase();
        var data_max = 0;//Math.max.apply(null, series.data);
        var data_sum = 0;
        var data_count = data.xaxis.data.length;

        var obj = {};
        for (var key = 0; key <= data_count; key++) {
            var item = data.series[0].data[key];

            if (data.xaxis.data[key]) {
                console.log('data_sum', item, data.xaxis.data[key]);
                data_max += item * 1;
                data_sum += data.xaxis.data[key] * 1;
            } else {
                obj.name = "";
            }
        }
        series.max = data_max;
        obj.name = data.series[0].name + '\n' + data_max;
        obj.value = data_sum;
        series.data = [];
        series.data[0] = obj;
        if (series.data.length > 0) {
            series.data = [series.data[0]];
            data.xaxis.data = [data.xaxis.data[0]];
        }
        // console.log('series.data', series.data);
        return res;
    }

    root.xmsGetChartsData = getChartsData;
})(jQuery, window);

(function ($, root) {
    "use strict"

    function Data(id, value) {
        this.id = id;
        this.value = value;
    }
    function Searcher() {
        this.list = [];
        this.length = 0;
    }
    Searcher.prototype.add = function (data) {
        this.list.push(data);
        this.length++;
        return this;
    }
    Searcher.prototype.indexOfById = function (id) {
        var index = -1;
        $.each(this.list, function (key, item) {
            if (item.id == id) {
                index = key;
                return false;
            }
        });
        return index;
    }
    Searcher.prototype.removeById = function (id) {
        var index = this.indexOfById(id);
        if (~index) {
            this.list.splice(index, 1);
        }
    }
    Searcher.prototype.removeAll = function (data) {
        this.list = [];
        this.length = 0;
        return this;
    }
    Searcher.prototype.getList = function (data) {
        return this.list;
    }
    root.xmsSearcherData = Searcher;
})(jQuery, window);

; (function ($, root) {
    "use strict"
    var xmsSearcherData = root.xmsSearcherData;
    var setting = {
        listTmpl: '<div class="xms-dropdownSearch-item" data-id="${id}" title="${title}"> <span class="glyphicon glyphicon-list-alt"></span><span class="xms-dropdownSearch-value">${title}</span></div>',
        mutileTmpl: '<div class="xms-dropdownSearch-item" data-id="${id}" title="${title}"> <input type="checkbox" name="xmsSearcherCheckbox" class="xms-dropdownSearch-checkbox" value="${id}" /> <span class="xms-dropdownSearch-value">${title}</span></div>',
        mutileItemTmpl: '<span class="xms-dropdown-addeditem" data-id="${id}">${title}<em>x</em></span>',
        addHandler: function () { },
        searchHandler: function () { },
        prevClick: function () { },
        getData: function () { },
        eventTarget: null,
        parent: null,
        offsetLeft: 0,
        offsetTop: 0,
        setParentPos: true,
        addHtml: false,
        isHideText: true,
        mutile: false,//多选功能配置
        offsetWidth: 70
    }
    function searcher(obj, opts) {
        this.opts = $.extend({}, setting, opts);
        var $thisPar = $(obj);
        var $thisEle = $('<div class="input-group xms-dropdownSearch-box"></div>');
        var _html = '';
        var mutileHtml = '';
        //添加多选功能
        this.opts.mutile == true ? (mutileHtml += '<div class="xms-dropdown-mutile"><div class="xms-dropdown-mutileAccept">确定</div><div class="xms-dropdownadded-list"></div></div>') : (mutileHtml += '');
        _html += '<div class="xms-dropdownSearch-list">' + mutileHtml + '<div class="xms-dropdownSearch-items"></div><div class="xms-dropdownSearch-info clearfix"></div></div>'
        $thisEle.html(_html);
        if ($thisPar.parent().length > 0) {
            this.opts.parent = $thisPar.parent();
            if (this.opts.setParentPos) {
                this.opts.parent.css("position", "relative");
            }
        }
        $(this.opts.parent).append($thisEle);
        var left = $thisPar.position().left;
        var top = $thisPar.position().top;
        $thisEle.css({ "position": "absolute", "left": left + this.opts.offsetLeft, "top": top + this.opts.offsetTop });
        var $this = $thisPar.children(".xms-dropdown-search"),
            $parent = $thisPar.parent(),
            $items = $thisEle.children(".xms-dropdownSearch-items"),
            $list = $thisEle.children(".xms-dropdownSearch-list");
        $list.css("width", $thisPar.outerWidth() + this.opts.offsetWidth);
        this.addeds = $thisEle.find(".xms-dropdownadded-list"),
            this.obj = $thisPar;
        this.list = new xmsSearcherData();//可选择列表数据
        this.addedList = new xmsSearcherData();//已选择的列表数据
        this.ele = $thisEle;
        this.temp = null;
        $.template("xmsSearcherTmpl", this.opts.listTmpl);
        $.template("mutileSearcherTmpl", this.opts.mutileTmpl);
        $.template("mutileItemSearcherTmpl", this.opts.mutileItemTmpl);
        this.listDom = $parent.find(".xms-dropdownSearch-items");
        this.countShow = $parent.find(".xms-dropdownSearch-countNum");
        this.xmsSearcherTmpl = $.template("xmsSearcherTmpl", opts.listTmpl);
        this.mutileSearcherTmpl = $.template("mutileSearcherTmpl", opts.mutileTmpl);
        this.itemsDom = null;
        this.addeditemsDom = null;
    }
    $.extend(searcher.prototype, {
        addData: function (data) {
            this.list.add(data);
        },
        render: function (preCb, nextCb) {
            preCb && preCb(this);
            var searchlist = this.list.getList();
            if (this.opts.mutile) {
                this.itemsDom = $.tmpl("mutileSearcherTmpl", searchlist);
            } else {
                this.itemsDom = $.tmpl("xmsSearcherTmpl", searchlist);
            }
            this.countShow.text(searchlist.length);
            this.listDom.html("");
            this.listDom.append(this.itemsDom);
            if (this.opts.isHideText) {
                this.hideOverText();
            }
            nextCb && nextCb(this);
            this.bindEvent();
        },
        renderAddeds: function (preCb, nextCb) {
            //  preCb && preCb(this);
            var searchlist = this.addedList.getList();
            this.addeditemsDom = $.tmpl("mutileItemSearcherTmpl", searchlist);
            this.countShow.text(searchlist.length);
            this.addeds.html("");
            this.addeds.append(this.addeditemsDom);

            // nextCb && nextCb(this);
            //  this.bindEvent();
        },
        hideOverText: function () {
            var items = this.listDom.children(".xms-dropdownSearch-item");
            var itemsW = this.listDom.outerWidth();
            items.each(function () {
                var it = $(this).children(".xms-dropdownSearch-value");
                var text = it.text();
                var iw = (itemsW * 0.6 - 4);
                it.width(iw);
            });
        },
        removeAll: function () {
            this.list.removeAll();
        },
        show: function () {
            var $this = this.ele
            $this.addClass("open");
        },
        hide: function () {
            var $this = this.ele
            $this.removeClass("open");
        },
        getIsShow: function () {
            var $this = this.ele;
            return $this.hasClass("open");
        },
        bindEvent: function () {
            var $this = this.ele, $parent = $this.parent(), self = this, btn = this.obj;
            if (this.opts.eventTarget) {
                btn = $(this.opts.eventTarget);
            }
            btn.off("click").on("click", function (e) {
                e = e || window.event;
                if (e.stopPropagation) { //如果提供了事件对象，则这是一个非IE浏览器
                    e.stopPropagation();
                } else {
                    //兼容IE的方式来取消事件冒泡
                    window.event.cancelBubble = true;
                }
                self.opts.prevClick && self.opts.prevClick(this);
                if ($this.hasClass("open")) {
                    $this.removeClass("open");
                } else {
                    $this.addClass("open");
                }
            });
            if (this.opts.mutile) {
                this.ele.find('.xms-dropdown-mutileAccept').off('click').on('click', function () {
                    var checklist = $('input.xms-dropdownSearch-checkbox:checked');
                    if (checklist.length > 0) {
                        var res = [];
                        checklist.each(function (key, item) {
                            res.push(item.value);
                        });
                        self.obj.val(res.join(','));
                        self.obj.attr("data-id", res.join(','));
                    }
                });
                this.itemsDom.find('input').on("change", function () {
                    // self.listDom.empty();
                    if (self.temp) {
                        self.temp.remove();
                    }
                    var clone = $(this).clone();
                    self.temp = clone;
                    if (self.opts.addHtml) {
                        // $this.append(clone);
                    }

                    var width = self.obj.width();
                    // $this.removeClass("open");
                    //给输入框赋值
                    // self.obj.val(clone.children(".xms-dropdownSearch-value").text());
                    // self.obj.attr("data-id", clone.attr("data-id"));
                    // self.opts.addHandler && self.opts.addHandler(this, clone.attr("data-id"), self, width);
                });
            } else {
                this.itemsDom.off("click").on("click", function () {
                    // self.listDom.empty();
                    if (self.temp) {
                        self.temp.remove();
                    }
                    var clone = $(this).clone();
                    self.temp = clone;
                    if (self.opts.addHtml) {
                        $this.append(clone);
                    }
                    var width = self.obj.width();
                    $this.removeClass("open");
                    //给输入框赋值
                    self.obj.val(clone.children(".xms-dropdownSearch-value").text());
                    self.obj.attr("data-id", clone.attr("data-id"));
                    self.opts.addHandler && self.opts.addHandler(this, clone.attr("data-id"), self, width);
                });
            }

            //this.obj.off("keydown").on("keydown", function (e) {
            //    var key = e.key;
            //    console.log(key);
            //    if (key == "ArrowDown") {
            //        var showlist = self.listDom.children("xms-dropdownSearch-item:not(:hidden)");
            //        console.log(showlist);

            //    } else if (key == "ArrowUp") {
            //    }
            //});

            $(document).on("click", function (e) {
                var target = e.target;
                if ($(target).closest($this).length == 0) {
                    $this.removeClass("open");
                }
            });
        }
    });
    root.xmsSearcher = searcher;
})(jQuery, window);

; (function ($, root) {
    "use strict"
    function Node(id, pid, title, layer) {
        this.nodes = [];
        this.pid = pid;
        this.id = id;
        this.title = title;
        this.layer = layer;
    }
    dTree.getFolderItems = function (list, name, key) {
        var res = $.map(list, function (item) {
            return $.inArray(name, item[key]) > -1 ? item : null;
        });
        return res;
    }

    dTree.hasContent = function (folder, value) {
        return folder[value] && folder[value].length;
    }

    function dTree() {
        this.nodes = [];
        this.root = null;
        this.pidList = [];
        this._layer = 0;
    }
    dTree.prototype.addLeaf = function (id, pid, title, debug) {
        if ($.inArray(pid, this.pidList) == -1) {
            this.pidList.push(pid);
            this._layer++;
        }
        if (this.root == null) {
            this.root = new Node(id, pid, title, this._layer);
        } else {
            var findNode = this.searchLeaf(pid);
            if (findNode) {
                findNode.nodes.push(new Node(id, pid, title, this._layer));
            }
        }
        if (debug) {
            this.nodes.push(new Node(id, pid, title))
        }
    }

    dTree.prototype.render = function (node, callback) {
        if (node) {
            if (node.nodes && node.nodes.length > 0) {
                for (var i = 0, len = node.nodes.length; i < len; i++) {
                    this.render(node, function () {
                        callback(node);
                    });
                }
            }
        }
    }
    dTree.prototype.searchLeaf = function (id) {
        if (this.root == null) return false;
        if (this.root.id == id) { return this.root; }
        var test = searchNode(this.root.nodes, function (item) {
            if (item.id == id) {
                return true;
            }
        });
        return test;
    }

    function searchNode(nodes, callback) {
        var len = nodes.length;
        if (len == 0) return false;
        var res = null;
        for (var i = 0; i < len; i++) {
            var item = nodes[i];
            var cb = callback(item);
            if (cb == true) {
                res = item;
            } else {
                if (item.nodes.length > 0) {
                    res = searchNode(item.nodes, callback)
                }
            }
        }
        return res;
    }
    root.dTree = dTree;
})(jQuery, window);

(function ($, root) {
    "use strict"
    var defaults = {
        changeHandler: null,
        trueHandler: null,
        falseHandler: null,
        target: null,
        resultType: 'boolean'
    }
    $.fn.xmsCheckbox = function (opts) {
        opts = $.extend({}, defaults, opts);
        return this.each(function () {
            var self = this, $this = $(self), _parent = $this.parent();
            var box = $('<div class="xms-checkbox"></div>');
            var line = $('<span class="xms-checkbox-line"></span>');
            var circle = $('<span class="xms-checkbox-circle"></span>');
            box.append(line).append(circle).appendTo(_parent);
            $this.hide();
            var isActive = $this.prop("checked");
            if (isActive == true) {
                box.addClass("checked");
            }
            circle.bind("click", function (e) {
                if (box.hasClass("checked")) {
                    box.removeClass("checked");
                    $this.prop("checked", true);
                    opts.resultType == 'boolean' ? $this.val(false) : $this.val(0);
                    opts.falseHandler && opts.falseHandler($this, $this.prop("checked"), box);
                } else {
                    box.addClass("checked");
                    $this.prop("checked", false);
                    opts.resultType == 'boolean' ? $this.val(true) : $this.val(1);
                    opts.trueHandler && opts.trueHandler($this, $this.prop("checked"), box);
                }
                opts.changeHandler && opts.changeHandler($this, $this.prop("checked"), box);
            });
            $this.bind('xmscheckbox.checked', function () {
                box.addClass("checked");
                $this.prop("checked", false);
                opts.resultType == 'boolean' ? $this.val(true) : $this.val(1);
                opts.trueHandler && opts.trueHandler($this, $this.prop("checked"), box);
                opts.changeHandler && opts.changeHandler($this, $this.prop("checked"), box);
            }).bind('xmscheckbox.nochecked', function () {
                box.removeClass("checked");
                $this.prop("checked", true);
                opts.resultType == 'boolean' ? $this.val(false) : $this.val(0);
                opts.falseHandler && opts.falseHandler($this, $this.prop("checked"), box);
                opts.changeHandler && opts.changeHandler($this, $this.prop("checked"), box);
            })
        });
    }
})(jQuery, window);

(function ($, root) {
    "use strict"
    var defaults = {
        selecter: ".tab",
        item: "a.collapse-title",
        content: "div.panel-collapse",
        type: "append",
        alwayLoad: false,
        clickHandler: null
    }
    function formTab(obj, opts) {
        var self = this, $this = $(obj);
        self.selecter = $this.find(opts.selecter);
        var timestrap = new Date() * 1 + new Date().toString(16);
        self.list = $('<ul id="myTab_' + timestrap + '" class="nav nav-tabs" role="tablist"></ul>');
        self.context = $('<div id="myTabContent_' + timestrap + '" class="tab-content"></div>');
        var _html = [];
        var type = $this.attr("data-type") || opts.type;
        self.selecter.each(function (key, item) {
            var items = $(this).find(opts.item);
            var contents = $(this).find(opts.content);
            var active = ""
            if (key == 0) {
                active = "active";
                contents.addClass("panel-collapse collapse active");
            } else {
                contents.addClass("panel-collapse collapse").removeClass("in");
            }
            _html.push('<li role="presentation" id="tab_hd_' + contents.attr("id") + '" class="formtab-item ' + active + '"><a href="#' + contents.attr("id") + '"  role="tab" data-toggle="tab"  aria-expanded="true">' + items.children("span").remove().end().text() + '</a></li>');
            self.context.append(contents);
        });
        self.selecter.hide();
        self.list.html(_html.join(""));
        self.selecter.eq(0)[type](self.list);
        self.list.after(self.context);
        //  console.log(list.find('li.formtab-item:gt(0)'))
        self.list.find('li.formtab-item').on('click', function () {
            var _id = $(this).children('a').attr('href');
            var $this = $(this);
            var $id = $(_id);
            if ($id.length > 0) {
                var subgrid = $id.find('.subgrid');
                var _table = subgrid.children('.grid');
                if (!_table.attr('isresetwidth')) {
                    setTimeout(function () {
                        _table.trigger('subpage.resetWidth');
                        _table.attr('isresetwidth', 1);
                        opts.clickHandler && opts.clickHandler($this, $id);
                    }, 50);
                }
            }
        });
        if (typeof _form !== 'undefined') {//防止页面没有引用JS时没有触发事件
            if (_form.Events != null) {//
                $('body').on('eventAllLoaded', function () {//页面有引用js时必须等待JS加载完后在执行后面相关代码
                    self.list.find('li.formtab-item:first').trigger('click');
                });
            } else {
                self.list.find('li.formtab-item:first').trigger('click');
            }
        } else {
            self.list.find('li.formtab-item:first').trigger('click');
        }
    }
    formTab.prototype.init = function () {
    }
    formTab.prototype.hideByIndex = function (num) {
        this.list.find('li.formtab-item').eq(num).hide();
        this.context.find('.panel-collapse').eq(num).hide();
    }
    formTab.prototype.showByIndex = function (num) {
        var _li = this.list.find('li.formtab-item').eq(num);
        _li.show();
        _li.children('a').trigger('click');
        this.context.find('.panel-collapse').eq(num).show();
    }
    formTab.prototype.hideByDomId = function (domId) {
        $('#tab_hd_tab_' + domId).hide();
        $('#tab_' + domId).hide();
    }
    var createfirst = false;
    $.fn.formTab = function (opts, _config) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                createfirst = true;
                var _tree = new formTab(this, opts);
                $this.data().formTab = _tree;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().formTab) {
                    // console.log(_config);
                    if (!$this.data().formTab[opts]) throw new Error('没有这个方法');
                    res = $this.data().formTab[opts](_config);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

(function ($, root, undefined) {
    "use strict"
    function watch(key, value) {
        this.key = key;
        this.value = value;
    }

    function checker(isUpdate) {
        this.watchs = [];
        this.isDirty = false;
        this.isUpdate = isUpdate || false;
        this.oldWatchs = [];
        this.dirtyList = [];
    }
    checker.prototype.addWatch = function (key, value) {
        this.watchs.push(new watch(key, value));
        this.oldWatchs.push(new watch(key, value));
    }
    checker.prototype.checkWatchs = function (callback) {
        var self = this;
        var flag = false;
        $.each(this.watchs, function (key, item) {
            $.each(self.oldWatchs, function (ii, obj) {
                if (item.key == obj.key) {
                    // console.log(item.value,obj.value)
                    if (item.value != obj.value) {
                        self.$apply(item, obj);
                        if (self.dirtyList.length > 0) {
                            var dirty = $.grep(self.dirtyList, function (iii, nnn) {
                                if (iii.key == obj.key) {
                                    return true;
                                }
                            });
                            //console.log(self.dirtyList);
                            if (dirty.length == 0) {
                                self.dirtyList.push(item);
                            }
                        } else {
                            self.dirtyList.push(item);
                        }

                        self.isDirty = true;
                        flag = true;
                        callback && callback();
                        $(document.body).trigger('xmsChecker.dirty', { obj: obj, item: item, checker: self });
                    }
                }
            });
        });
        if (flag == false) {
            self.isDirty = false;
        }
    }
    checker.prototype.$apply = function (item) {
        if (this.isUpdate) {
            // obj.value = item.value;
        }
    }
    checker.prototype.setValue = function (key, value) {
        //console.log(key)
        $.each(this.watchs, function (i, item) {
            if (item.key == key) {
                item.value = value;
                return false;
            }
        });
    }
    root.xmsDirtyChecker = checker;
})(jQuery, window);

(function (root) {
    "use strict"
    function _setIframeHeight() {
        var notTopWindow = window.self == window.top;
        var parent = null
        if (!notTopWindow) {
            parent = window.parent;
            if (parent['_parentSetHeight']) {
                return parent['_parentSetHeight'](window.self);
            }
        }
    }
    function getIframeHeight() {
        if ($("#formIframeContent").length > 0) {
            var iframeH = $("#formIframeContent").contents().find("html").height();
            //console.log(iframeH);
            $("#formIframeContent").height(iframeH);
        }
    }
    function _parentSetHeight(_win) {
        if (!_win) return false;
        var _this = window.self, iframes = $("iframe.formIframeContent");
        if (iframes.length > 0) {
            var height = iframes.contents().find("html").height();
            iframes.css("min-height", height);
            return height;
        }
    }
    root._setIframeHeight = _setIframeHeight;
    root._parentSetHeight = _parentSetHeight;
    root.getIframeHeight = getIframeHeight;
})(window);

; (function ($, root) {
    "use strict"
    var style = (document.documentElement || document.body).style;
    var transitionEnd = (function () {
        var transEndEventNames = {
            WebkitTransition: 'webkitTransitionEnd',
            MozTransition: 'transitionend',
            OTransition: 'oTransitionEnd otransitionend',
            transition: 'transitionend'
        }
        for (var name in transEndEventNames) {
            if (typeof style[name] === "string") {
                return transEndEventNames[name]
            }
        }
    })();
    root.transitionEnd = transitionEnd;
})(jQuery, window);

; (function ($, root) {
    "use strict"
    var settings = {
        tClass: ".xms-formDropDown-List",//silbings
        tItem: ".xms-formDropDown-Item",
        getSelecter: 'div[data-type="nvarchar"]',
        input: ".colinput",
        Clone: false,
        Event: "click",
        Selecter: ".xms-formDownCtrl",
        viewer: ".xms-formDownInput",
        noHidePlace: null
    }
    $.fn.xmsFormDrop = function (opts) {
        var options = $.extend({}, settings, opts);
        this.each(function () {
            var $this = $(this);
            var target = $this.find(options.tClass);
            var items = target.find(options.tItem), itemFirst = items.eq(0);

            if (options.getSelecter) {
                //console.log(target.find(options.getSelecter))
                if (target.find(options.getSelecter).length > 0) {
                    itemFirst = target.find(options.getSelecter).eq(0);
                }
            }
            if (options.Clone) {
                var clone = itemFirst.clone();
                $this.append(clone);

                clone.find(options.input).bind("click", function () {
                    var hasC = target.hasClass("in");
                    if (hasC) {
                        target.removeClass("in");
                    } else {
                        target.addClass("in");
                    }
                });
            } else {
                if (options.Selecter) {
                    //console.log($this.find(options.Selecter))
                    if (options.Event == "click") {
                        $this.find(options.Selecter).bind("click", function () {
                            var hasC = target.hasClass("in");
                            if (hasC) {
                                target.removeClass("in");
                            } else {
                                target.addClass("in");
                            }
                        });
                    } else if (options.Event == "mouseenter") {
                        $this.mouseover(function () {
                            target.addClass("in");
                        })
                        //    .mouseout(function (e) {
                        //    e = e || window.event;
                        //    var targetE = e.srcElement || e.target;
                        //    if ($(".modal:visible").length > 0 || $(".datepicker:visible").length > 0) {
                        //    } else {
                        //        target.removeClass("in");
                        //    }
                        //});
                    }
                } else {
                    if (options.Event == "click") {
                        $this.bind("click", function () {
                            var hasC = target.hasClass("in");
                            if (hasC) {
                                target.removeClass("in");
                            } else {
                                target.addClass("in");
                            }
                        });
                    } else if (option.Event == "mouseenter") {
                        $this.mouseenter(function () {
                            target.addClass("in");
                        }).mouseout(function () {
                            target.removeClass("in");
                        });
                    }
                }
            }
            $(document).bind("click", function (e) {
                e = e || window.event;
                var targetE = e.srcElement || e.target;
                //console.log($(targetE).closest(target))
                if ($(targetE).closest($this).length == 0 && $(targetE).closest(target).length == 0
                    && (options.noHidePlace && $(targetE).closest(options.noHidePlace).length == 0)
                ) {
                    target.removeClass("in");
                }
            });
            $this.bind("xmsFormDrop.close", function () {
                target.removeClass("in");
            });
            $this.bind("xmsFormDrop.show", function () {
                target.addClass("in");
            });
            $this.trigger("xmsFormDrop.itemFirst", { first: itemFirst, obj: $this, target: target });
        });
    }
})(jQuery, window);

; (function ($, root) {
    "use strict"
    var settings = {
        ctrlBtn: null,
        ctrlBtnText: null,
        itemClass: '.hideBySizeItem',
        find: 'children',
        type: 'count',   //count,width,collospe
        count: 10,
        width: "100%",
        colsOpts: null
    }

    function showHideLimit(list, start, end, type) {
        list.each(function (key, obj) {
            if (start <= key && key <= end) {
                $(this)[type]();
            }
        });
    }
    function getLimitList(list, start, end) {
        var res = [];
        list.each(function (key, obj) {
            if (start <= key && key <= end) {
                res.push($(this));
            }
        });
        return res;
    }
    $.fn.showBySize = function (opts) {
        this.each(function () {
            var $this = $(this);
            if ($this.find('.limitList-box').length > 0) {
                var insertBoxlist = $this.find('.limitList-box').find('.limitList-box-list>a');
                insertBoxlist.each(function () {
                    $this.append(this);
                });
                $this.find('.limitList-box').remove();
                var height = $this.outerHeight();
                $this.parents('#form-section').css('margin-top', height);
            }
        });
    }
    $.fn.hideBySize = function (opts) {
        var opts = $.extend({}, settings, opts);

        this.each(function () {
            var $this = $(this);
            if (opts.ctrlBtn) {
                var ctrlBtn = $this[opts.find](opts.ctrlBtn);
            } else {
                var ctrlBtn = $('<span class="hideBySizeBtn">' + (opts.ctrlBtnText || "...") + '</span>');
            }
            ctrlBtn.hide().attr("data-isHide", true);
            var list = $this[opts.find](opts.itemClass);
            if (opts.type == 'count' && opts.count != '') {
                $this.append(ctrlBtn);
                if (list.length > opts.count) {
                    ctrlBtn.show().attr("data-isHide", true);
                    //showHideLimit(list, opts.count , list.length - 1, "hide");
                }
                var limitList = getLimitList(list, opts.count, list.length - 1);
                var limitBox = $("<div class='limitList-box'></div>");
                var limitBoxList = $("<div class='limitList-box-list'></div>");
                $this.append(limitBox);
                limitBox.append(ctrlBtn).append(limitBoxList);
                limitBoxList.hide();
                $.each(limitList, function (key, obj) {
                    limitBoxList.append(obj);
                });
                ctrlBtn.bind("click", function () {
                    if ($(this).attr("data-isHide") == "true") {
                        limitBoxList.show();
                        $(this).attr("data-isHide", false);
                    } else {
                        limitBoxList.hide();
                        $(this).attr("data-isHide", true);
                    }
                });
                $(document).bind("click", function (e) {
                    e = e || window.event;
                    var target = e.srcElement || e.target;
                    if ($(target).closest(limitBox).length == 0) {
                        limitBoxList.hide();
                        ctrlBtn.attr("data-isHide", true);
                    }
                });
            } else if (opts.type == 'width' && opts.width != '') {
                $this.append(ctrlBtn);
                var $thisW = (opts.width && opts.width != "100%") ? opts.width : $this.width() * 1 - 20;
                var liTemp = list.eq(0);
                var liW = liTemp.width() * 1 + 3;
                var next = liTemp;
                var flag = false;
                while (liTemp.length > 0) {
                    liW += liTemp.outerWidth() + 3;
                    if (liW > $thisW) {
                        flag = true;
                        break;
                    }
                    liTemp = liTemp.next(opts.itemClass);
                }
                if (flag) {
                    var count = liTemp.index();
                    ctrlBtn.show().attr("data-isHide", true);
                    showHideLimit(list, count, list.length - 1, "hide");

                    ctrlBtn.bind("click", function () {
                        if ($(this).attr("data-isHide") == "true") {
                            showHideLimit(list, count, list.length - 1, "show");
                            $(this).attr("data-isHide", false);
                        } else {
                            showHideLimit(list, count, list.length - 1, "hide");
                            $(this).attr("data-isHide", true);
                        }
                    });
                }
            } else if (opts.type == 'collospe' && opts.colsOpts != null) {
                var $thisW = (opts.width && opts.width != "100%") ? opts.width : $this.width() * 1 - 20;
                var liTemp = list.eq(0);
                var liW = liTemp.width() * 1 + 3;
                var next = liTemp;
                var flag = false;
                while (liTemp.length > 0) {
                    liW += liTemp.outerWidth() + 3;
                    if (liW > $thisW) {
                        flag = true;
                        break;
                    }
                    liTemp = liTemp.next(opts.itemClass);
                }
                if (flag) {
                    var count = liTemp.index();
                    ctrlBtn.show().attr("data-isHide", true);
                    var limitList = getLimitList(list, count, list.length - 1);
                    var limitBox = $("<div class='limitList-box'></div>");
                    var limitBoxList = $("<div class='limitList-box-list'></div>");
                    $this.append(limitBox);
                    limitBox.append(ctrlBtn).append(limitBoxList);
                    limitBoxList.hide();
                    $.each(limitList, function (key, obj) {
                        limitBoxList.append(obj);
                    });
                    ctrlBtn.bind("click", function () {
                        if ($(this).attr("data-isHide") == "true") {
                            limitBoxList.show();
                            $(this).attr("data-isHide", false);
                        } else {
                            limitBoxList.hide();
                            $(this).attr("data-isHide", true);
                        }
                    });
                    $(document).bind("click", function (e) {
                        e = e || window.event;
                        var target = e.srcElement || e.target;
                        if ($(target).closest(limitBox).length == 0) {
                            limitBoxList.hide();
                            ctrlBtn.attr("data-isHide", true);
                        }
                    });
                }
            }
        });
    }
})(jQuery, window);

; (function ($, root) {
    "use strict"
    var xmsSearcher = root.xmsSearcher;
    function setLookUpState(obj, type, datat) {//datat需要显示的内容
        if (!datat) {
            var val = $(obj).val();
        } else {
            var val = $(obj).attr(datat);
        }
        //console.log('settimeout', setTimeout(0));
        if ($(obj).siblings(".xms-dropdownLink").length > 0) {
            $(obj).siblings(".xms-dropdownLink").remove();
        }
        if (!type) {
            if (val == "") return true;
        }

        var tar = $("#" + $(obj).attr("id").replace("_text", ""));
        var id = tar.val();
        var alink = $('<a target="_blank" href="' + ORG_SERVERURL + '/entity/create?entityid=' + $(obj).attr("data-lookup") + '&recordid=' + id + '" class="xms-dropdownLink" title="' + val + '"><span class="glyphicon glyphicon-list-alt"></span> <span class="xms-drlinki" data-id="" data-value="">' + val + '</span></a>');
        $(obj).css({ "color": "#fff" });
        if ($(obj).is(":disabled")) {
            $(obj).css({ "color": "#eee" });
        }
        $(obj).parent().append(alink);
        var width = $(obj).width();
        //console.log(width);
        var lenW = alink.find(".xms-drlinki").width();
        if (width < 30 && $(obj).parents('td').length > 0) {
            var thwidth = $(obj).parents('td');
            if (thwidth) {
                width = thwidth - 40;
            }
        }

        //console.log(lenW , width)
        if (lenW + 10 > width) {
            lenW = width - 10;
        } else {
            lenW = lenW + 20;
        }
        alink.css("width", lenW);
        $(obj).trigger("insertByLookup", { tar: obj });
        //$(obj).blur();
        $(obj).focus();
    }

    function MacthSearch(e) {
        var val = $(e).val();
        var valL = val.toLowerCase();
        var $listwrap = $(e).siblings('.xms-dropdownSearch-box');
        var $list = $listwrap.find('.xms-dropdownSearch-item');
        if ($list.length > 0) {
            $list.each(function (i, n) {
                var itemVal = $(n).find('.xms-dropdownSearch-value').text();
                var itemValL = itemVal.toLowerCase();
                if (itemValL.indexOf(valL) != -1) {
                    $(n).show();
                } else {
                    $(n).hide();
                }
            });
            $listwrap.show();
        } else {
            $listwrap.hide();
        }
    }

    function searchFormCtrl(obj, opts) {
        var self = this;
        this.input = obj;
        this.opts = opts;
        if (opts.url && typeof opts.url === "function") {
            this.url = opts.url();
        } else {
            this.url = opts.url;
        }
        this.isLoaded = false;
        this.goLoad = true;
        $(self.input).bind("searchDialog", function (e, obj) {
            //console.log('searchDialog.event',obj);
            if (obj.isDefault) {
                setLookUpState(self.input);
            } else {
                setLookUpState(self.input, "insert");
            }
        });
        this.searcher = new xmsSearcher(obj, {
            setParentPos: false,
            addHandler: function (obj, id, par, width) {
                var tar = $("#" + $(par.obj).attr("id").replace("_text", ""));
                if ($(par.obj).siblings(".xms-dropdownLink").length > 0) {
                    $(par.obj).siblings(".xms-dropdownLink").remove();
                }
                if (tar.length > 0) {
                    tar.val(id);
                }

                var alink = $('<a target="_blank" href="' + ORG_SERVERURL + '/entity/create?entityid=' + $(par.obj).attr("data-lookup") + '&recordid=' + id + '" class="xms-dropdownLink" title="' + par.obj.val() + '"><span class="glyphicon glyphicon-list-alt"></span> <span class="xms-drlinki" data-id="" data-value="">' + par.obj.val() + '</span></a>');
                $(par.obj).css({ "color": "#fff" });
                $(par.obj).val($(obj).children('.xms-dropdownSearch-value').text());
                $(par.opts.parent).append(alink);
                //console.log(width);
                var lenW = alink.find(".xms-drlinki").width();
                console.log(lenW, width)
                if (lenW > width - 10) {
                    lenW = width;
                } else {
                    lenW = lenW + 20;
                }
                alink.css("width", lenW);
                $(par.obj).focus();
                self.opts.addHandler && self.opts.addHandler(tar, obj, par);
                //self.opts.DialogaddHandler && self.opts.DialogaddHandler(self.input);
            }
        });
        this.bindEvent();
    }

    searchFormCtrl.prototype.getDatas = function (id) {
        var self = this;
        var url = self.url;

        url = url.replace("{{id}}", id);
        if (self.opts.setUrl && typeof self.opts.setUrl === "function") {
            var reUrl = self.opts.setUrl();
            if (reUrl && reUrl != "") {
                url = reUrl;
            }
        }
        Xms.Web.GetJson(url, null, function (data) {
            var _value = self.input.val();
            if (data.StatusName == 'success') {
                var refname = data.content.name;
                var inputFilter = self.input.attr('data-filter');
                var useInputFilter = true;
                try {
                    if (inputFilter && inputFilter != "") {
                        inputFilter = JSON.parse(inputFilter);
                    } else {
                        useInputFilter = false;
                    }
                } catch (err) {
                    useInputFilter = false;
                }
                if (useInputFilter) {
                    var filter = inputFilter;
                } else {
                    var filter = { "filteroperator": 0, "conditions": [], "filters": [{ "filteroperator": 1, "filters": [], "conditions": [{ "attributename": "name", "operator": 6, "values": [_value] }] }, { "filteroperator": 1, "filters": [], "conditions": [{ "attributename": "statecode", "operator": 0, "values": [1] }] }] }
                }
                var QueryObject = { EntityName: refname, Criteria: filter/*new Xms.Fetch.FilterExpression()*/, ColumnSet: { AllColumns: true }, PageInfo: { PageNumber: 1, PageSize: 15 } };
                if (self.opts.filters) {
                    QueryObject = self.opts.filters;
                }
                console.log('QueryObject', QueryObject);
                Xms.Web.Post(self.opts.subUrl, { 'query': QueryObject }, false, function (data2) {
                    self.searcher.show();
                    var datas = [];
                    data2.Content = JSON.parse(data2.Content.replace("\n", ""));
                    if (typeof self.opts.dataFilter == 'function') {
                        data2.Content = self.opts.dataFilter(data2.Content);
                    }
                    for (var i = 0, len = data2.Content.items.length; i < len; i++) {
                        data2.Content.items[i]['title'] = data2.Content.items[i].name;
                        data2.Content.items[i]['id'] = data2.Content.items[i][refname.toLowerCase() + 'id'];
                    }
                    self.searcher.removeAll();
                    for (var i = 0, len = data2.Content.items.length; i < len; i++) {
                        self.searcher.addData(data2.Content.items[i]);
                    }
                    self.searcher.render();
                    self.isLoaded = true;
                    self.goLoad = true;
                    var val = $(self.input).val();
                    MacthSearch(self.input);
                }, false, false, false);
            }
        });
    }
    searchFormCtrl.prototype.bindEvent = function () {
        var self = this;

        var imme = xmsImmediate(500);//获取缓冲函数
        self.input.bind("keyup", function () {
            self.isLoaded = false;
            console.log('trigger.lookup.keyup', this);
            imme(function () {
                if (self.isLoaded) {
                    if ($(self.input).siblings(".xms-dropdownLink").length > 0) {
                        self.opts.delHandler && self.opts.delHandler(self.input, $(self.input).siblings(".xms-dropdownLink"));
                        $(self.input).siblings(".xms-dropdownLink").remove();
                        $(self.input).css({ "color": "#555", "opacity": "1" });
                    }
                    //if (!self.searcher.getIsShow()) {
                    self.searcher.show();
                    var hiddenValue = $(self.input).attr('id').replace("_text", "");
                    var hiddenValueDom = $("#" + hiddenValue);
                    if (hiddenValueDom.length > 0) {
                        //hiddenValueDom.val();
                        //hiddenValueDom.trigger('change');
                    }
                    MacthSearch(self.input);
                    // }
                } else {
                    if ($(self.input).siblings(".xms-dropdownLink").length > 0) {
                        self.opts.delHandler && self.opts.delHandler(self.input, $(self.input).siblings(".xms-dropdownLink"));
                        $(self.input).siblings(".xms-dropdownLink").remove();
                        $(self.input).css({ "color": "#555", "opacity": "1" });
                    }
                    if (self.goLoad == false) return false;
                    self.goLoad = false;
                    self.getDatas(self.opts.id);
                }
            });
        });
    }
    var setting = {
        url: "/api/schema/entity/{{id}}",
        subUrl: '/api/data/Retrieve/Multiple',
        data: null,
        addToInputed: null,
        target: null,
        dataFilter: null,
        id: null,
        addHandler: null,
        delHandler: null,
        DialogaddHandler: null
    }

    $.fn.xmsSelecteDown = function (opts) {
        var options = $.extend({}, setting, opts);
        this.each(function () {
            var searchF = new searchFormCtrl($(this), options);
        });
    }
    $.fn.xmsSelecteDown.setLookUpState = setLookUpState;
})(jQuery, window);

(function ($, root, undefined) {
    "use strict";

    var defaults = {
        size: "lg",
        auto: false // 有图片的时候是否显示缩略图在下方
    }

    $.fn.imgShow = function (opts) {
        if (typeof opts === 'string') {
        } else {
            opts = $.extend({}, defaults, opts);

            var imgModal = '<div class="modal-dialog modal-' + opts.size + '"><div class="modal-content"><div class="modal-header"><button type="button" class="close" data-dismiss="modal" aria-hidden="true">×</button></div><div class="modal-body"><div class="imgShow" style="align:center"></div></div></div>';
            var imgModalDom = $('<div class="modal fade " id="imgShowModal" tabindex="-1" role="dialog" aria-labelledby="entityRecordsModalLabel" aria-hidden="true"></div>');
            imgModalDom.html(imgModal);
            if ($("#imgShowModal").length == 0) {
                $(document.body).append(imgModalDom);
            } else {
                imgModalDom = $("#imgShowModal");
            }
            var isModalShow = false;
            return this.each(function () {
                var $this = $(this);
                var winSize = { w: $(window).width(), h: $(window).height() };
                var _name = $this.attr('data-name');
                var slpic = $('<div class="imgShow-small"></div>');
                if (_record && typeof _record[_name] !== 'undefined') {
                    var url = $this.attr('data-imgurl');
                    if (url == "") {
                        var filter = { "Conditions": [{ "AttributeName": Xms.Page.PageContext.EntityName.toLowerCase() + 'id', "Operator": 0, "Values": [Xms.Page.PageContext.RecordId] }] };
                        var queryObj = { EntityName: Xms.Page.PageContext.EntityName.toLowerCase(), Criteria: filter, ColumnSet: { Columns: [_name] } };
                        var data = JSON.stringify({ "query": queryObj, "isAll": true });
                        Xms.Web.GetJson('/api/data/Retrieve/Multiple', data, function (response) {
                            console.log(response)
                            if (response && response.content && response.content.length > 0) {
                                var res = response.content[0];
                                if (!res[_name]) return false;
                                var url = res[_name];
                                $this.siblings('input').val(url).css('text-indent', -999);
                                var isimg = Xms.Web.isImg(url);
                                if (!isimg) {
                                    $this.text('点击下载');
                                } else {
                                    $this.text('点击查看');
                                }
                                $this.bind("click", function () {
                                    var imgW = '100%';
                                    var imgH = 'auto';
                                    if (!isimg) {
                                        var substr = url.substr(-6);
                                        var arr = substr.split('.');
                                        Xms.Web.OpenWindow(url, null, ' download="' + _name + '.' + arr[1] + '"');
                                        return false;
                                    }
                                    if (opts.auto) {
                                        slpic.html('<img src="' + url + '" />');
                                        $this.append(slpic);
                                    }
                                    var img = $('<img src="' + url + '"/>');
                                    imgModalDom.find(".imgShow").html(img);
                                    // img.onload = function () {
                                    if (img.height() > winSize.h) {
                                        imgH = winSize.h * 0.9;
                                        imgW = 'auto';
                                    }
                                    img.css({ "width": imgW, "height": imgH });
                                    imgModalDom.modal('show');
                                });
                            }
                        }, null, null, 'post');
                    }
                }
            });
        }
    }
})(jQuery, window);

(function () {
    "use strict"
    function readFile(obj, txshow, value, opts) {
        var file = obj.files[0];
        //判断是否是图片类型
        if (!/image\/\w+/.test(file.type)) {
            alert("只能选择图片");
            return false;
        }
        var reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onload = function (e) {
            txshow.attr('data-imgurl', this.result);
            txshow.text(opts.uploadedText);
            value.val(this.result);
            opts.uploadEnd && opts.uploadEnd(txshow, value, this.result);
        }
        reader.onabort = function (e) {
            Xms.Web.Alert(false, '上传中断，请尝试重新上传');
        }
        reader.onerror = function (e) {
            Xms.Web.Alert(false, '上传出错，' + e.message);
        }
        reader.onloadend = function (e) {
        }
    }
    function uploadFile(obj, txshow, value, opts, e) {
        //var _form = $('<form class="hide" action="' + ORG_SERVERURL +'/file/Create" method="post" enctype="multipart/form-data"></form>');
        //var $entityid = $('<input type="hidden" name="entityid" value="' + Xms.Page.PageContext.EntityId + '" />');
        //var $objectid = $('<input type="hidden" name="objectid" value="' + Xms.Page.PageContext.RecordId + '" />');
        //var $file = $('<input type="file" onchange="$_fileuploadEndCallback(this);" name="attachment" >');
        //$('body').append(_form);
        //_form.append($entityid).append($objectid).append($file);
        //var $_fileuploadEndCallback = function () {
        //    console.log('uploadform');
        //    setTimeout(function () {
        //        _form.ajaxForm(function (response) {
        //            if (response.IsSuccess) {
        //                var src = response.Extra.filepath;
        //               // src = location.protocol + '\/\/' + location.host + src;
        //                txshow.attr('data-imgurl', src);
        //                txshow.text(opts.uploadedText);
        //                value.val(src);
        //                opts.uploadEnd && opts.uploadEnd(txshow, value, src);
        //                $file.off();
        //                _form.remove();
        //                return;
        //            }
        //            Xms.Web.Alert(false, response.Content);
        //            $file.off();
        //            _form.remove();
        //        }).submit();
        //    }, 0);
        //}
        //window.$_fileuploadEndCallback = $_fileuploadEndCallback;
        //$(obj).on('click', function (e) {
        //    e.preventDefault();
        //    e.stopPropagation();
        //})
        setTimeout(function () {
            $(obj).trigger('click');
        }, 500);
    }
    var settings = {
        uploadEnd: null,
        uploadedText: '点击查看'
    }

    $.fn.uploadTo64 = function (opts) {
        opts = $.extend({}, settings, opts);
        return this.each(function () {
            var input = this;
            var txshow = $(this).parent().siblings("span");
            var _id = $(input).attr('id');
            _id = _id.toLowerCase();
            // var value = $(this).parent().parent().find("input.uploadinput").css("color","#fff");
            if (typeof (FileReader) === 'undefined') {
                txshow.val("抱歉，你的浏览器不支持 FileReader，请使用现代浏览器操作！");
                input.prop('disabled', true);
            } else {
                $(this).change(function (e, opts) {
                    //获取文件对象
                    var file = this.files[0];
                    var filetype = file.type;
                    //使用fileReader对文件对象进行操作
                    var is_img = Xms.Web.isImg(filetype);
                    var reader = new FileReader();
                    if (!is_img) {
                        //将文件读取为arrayBuffer
                        reader.readAsArrayBuffer(file);
                        reader.onload = function () {
                            console.log(reader.result);
                            var $text = $('#___file_upload_' + _id);
                            $text.val(file.name);
                            $text.css('text-indent', 0);
                            $text.parents('.upload-file-box:first').find('.upload-file-input').text('');
                            if (typeof dirtyChecker != 'undefined') {
                                dirtyChecker.isDirty = true;
                            }
                            console.log(reader);
                        }
                    } else {
                        //用于图片显示不需要传入后台，reader.result的结果是base64编码数据，直接放入img的src中即可
                        reader.readAsDataURL(file);
                        reader.onload = function () {
                            var $text = $('#___file_upload_' + _id);
                            $text.val(file.name);
                            $text.css('text-indent', 0);
                            $text.parents('.upload-file-box:first').find('.upload-file-input').text('');
                            if (typeof dirtyChecker != 'undefined') {
                                dirtyChecker.isDirty = true;
                            }
                            console.log(reader);
                        }
                        console.log(e, opts)
                    }
                    /*reader.readAsBinaryString(file);
                    reader.onload = function(){
                        console.log(reader.result);
                    }
                    */
                })
            }
        });
    }
})();

(function ($, root, undefined) {
    "use strict"
    //创建控制节点
    function createHandler() {
        var handler = document.createElement('div');
        handler.className = "tableResize-item tableResize-ctrl";
        return handler;
    }
    function dragable(type) {
        var self = this;
        var mouse = { x: 0, y: 0 };
        var starPosition = {};

        type = type || "position";
        var movePixel = 0;

        this.ele.on("mousedown", function mousedown(e) {
            e = e || window.event;
            if (e && e.stopPropagation) {
                // 因此它支持W3C的stopPropagation()方法
                e.stopPropagation();
            } else {
                //否则，我们需要使用IE的方式来取消事件冒泡
                window.event.cancelBubble = true;
            }
            var target = e.target || e.srcElement;

            var that = this;
            var isDown = false;
            if (self._super.tipBox && self._super.tipBox.length > 0) {
                self._super.tipBox.show();
            }
            starPosition.left = self.ele.offset().left;
            starPosition.x = e.pageX;
            starPosition.y = e.pageY;
            var tableH = self._super.table.height();
            if (tableH > 350) {
                tableH = 350;
            }
            self.ele.height(tableH);
            if (self._super.tipBox && self._super.tipBox.length > 0) {
                self._super.tipBox.removeClass('hide');
            }
            self._super.lineHanderUi.height(tableH);
            movePixel = starPosition.x;
            var pos = {
                x: self.ele[type]().left,
                y: self.ele[type]().top
            };
            var offsetLeft = starPosition.x - pos.x;
            var offsetTop = starPosition.y - pos.y;
            var contrloffsetL = starPosition.x - self.ele.offset().left;
            var contrloffsetT = starPosition.y - self.ele.offset().top;
            var controlTop = self.ele.offset().top;
            self.ele.addClass("active");
            self._super.lineHanderUi.addClass("active");
            //防止出现蓝色选中区域
            document.unselectable = "on";
            document.onselectstart = function () {
                return false;
            }
            $(document).on("mousemove", mousemove);
            $(document).on("mouseup", mouseup);
            function mousemove(e) {
                mouse.x = e.pageX;
                mouse.y = e.pageY;
                self._super.opts.onResize && self._super.opts.onResize();
                var fixw = mouse.x - movePixel;
                var thw = self.par.outerWidth() + fixw;
                if (thw >= self._super.opts.minW) {
                    self.ele.css({
                        "left": mouse.x - offsetLeft,
                    });
                    self._super.lineHanderUi.css({
                        "left": mouse.x - contrloffsetL,
                        "top": controlTop
                    });
                    if (self._super.tipBox && self._super.tipBox.length > 0) {
                        self._super.tipBox.css({
                            "left": mouse.x + 10,
                            "top": mouse.y,
                        });
                        self._super.tipBox.text(thw);
                    }
                }
            }
            function mouseup(e) {
                mouse.x = e.pageX;
                mouse.y = e.pageY;
                movePixel = mouse.x - starPosition.x;
                var thw = self.par.width() + movePixel;
                if (thw >= self._super.opts.minW) {
                    var tablew = self._super.table.width() + movePixel;
                    self._super.table.width(tablew);
                    self.par.width(thw);
                    var toLinkIndex = getThIndex(self._super.table, self.par);
                    if (self._super.opts.linkTable != '' && $(self._super.opts.linkTable).length > 0) {
                        $(self._super.opts.linkTable).find('>thead>tr>th').eq(toLinkIndex).width(thw);
                    }
                } else {
                    var tablew = self._super.table.width() + (self.ele.offset().left - starPosition.left);
                    self._super.table.width(tablew);
                    self.par.width(self.par.width() + (self.ele.offset().left - starPosition.left));
                    var toLinkIndex = getThIndex(self._super.table, self.par);
                    if (self._super.opts.linkTable != '' && $(self._super.opts.linkTable).length > 0) {
                        $(self._super.opts.linkTable).find('>thead>tr>th').eq(toLinkIndex).width(self.par.width() + (self.ele.offset().left - starPosition.left));
                    }
                }
                var thIndex = self.par.index();

                self._super.table.find(">tbody>tr>td:eq(" + thIndex + ")").width(thw);
                self._super.opts.onResizeEnd && self._super.opts.onResizeEnd(self.par, self._super.table);
                $(document).off("mouseup", mouseup);
                $(document).off("mousemove", mousemove);
                self.ele.height(self._super.opts.defaultH);
                self._super.lineHanderUi.height(self._super.opts.defaultH);
                if (self._super.tipBox && self._super.tipBox.length > 0) {
                    self._super.tipBox.hide();
                }
                self.ele.removeClass("active");
                self._super.lineHanderUi.removeClass("active");
                document.unselectable = "off";
                document.onselectstart = function () {
                    return true;
                }
            }
        });

        function getThIndex(table, th) {
            var index = -1;
            table.find('>thead>tr>th').each(function (key) {
                if (this == th.get(0)) {
                    index = key;
                    return false;
                }
            });
            return index;
        }
    }

    //创建节点对象
    function thHanderCtrl(ele, par, _super) {
        this.par = $(par);
        this.ele = $(ele);
        this._super = _super;
        this.offset = { x: 0, y: 0 };
        this.size = { w: 0, h: 0 };
        this.init();
    }
    thHanderCtrl.prototype.init = function () {
        this.par.append(this.ele);
        this.dragInit();
    }
    thHanderCtrl.prototype.dragInit = function () {
        dragable.call(this);
    }
    thHanderCtrl.prototype.move = function (x, y) {
        this.offset = {
            x: x,
            y: y
        }
        this.ele.css({
            "top": y,
            "left": x
        });
    }

    function tableHdResize(obj, opts) {
        var self = this;
        this.table = $(obj);
        this.opts = opts;
        this.lineHanderUi = null;
        if (self.opts.showTips) {
            self.tipBox = $('<span class="resizeTable-tips hide"></span>');
            $('body').append(self.tipBox);
        }
        this.thead = this.table.find("thead:first");
        if (this.thead.length > 0) {
            this.thlist = this.thead.children('tr:first').children('th:not("' + opts.fixedClass + '")');
        } else {
            this.thlist = this.table.children('tr:first').children('th:not("' + opts.fixedClass + '")');
        }
        this.init();
    }
    tableHdResize.prototype.init = function () {
        var self = this;
        var tablew = 0;
        this.thlist.each(function (key, obj) {
            var item = $(this);
            var hander = createHandler();
            var attrWidth = item.attr('data-width');
            var width = attrWidth || item.width();
            tablew += width;
            item.width(width);
            item.removeAttr("width");
            var handerCtrl = new thHanderCtrl(hander, item, self);
        }).css({ "position": "relative" });
        this.lineHanderUi = $('<div class="tableResize-handler"></div>');
        $('body').append(this.lineHanderUi);
        if (this.opts.resetTableWidth) {
            this.table.width('100%');
        }
        this.table.css("table-layout", "fixed");
    }

    var settings = {
        fixedClass: ".hide",
        onResize: null,
        onResizeEnd: null,
        defaultH: 50,
        linkTable: '',
        resetTableWidth: true,
        showTips: false,
        minW: 50
    }
    $.fn.tableHdResize = function (opts) {
        opts = $.extend({}, settings, opts);
        this.each(function () {
            var table = new tableHdResize(this, opts);
        });
    }
})(jQuery, window);

; (function ($, root) {
    "use strict"
    function valiExpList() {
        this.list = [];
        this.length = 0;
    }
    valiExpList.prototype.push = function (name, exp) {
        this.list.push(new valiExp(name, exp));
    }

    function valiExp(name, exp) {
        this.name = name;
        if (!exp) return false;
        if (typeof exp === "function") {
            this.exp = exp();
        } else {
            this.exp = exp;
        }
    }

    function setValidata() {
    }
    function checkRequre(obj) {
    }
    var settings = {}
    $.fn.setValidata = function (opts) {
        opts = $.extend({}, settings, opts);
        return this.each(function () {
            var $this = $(this), $parent = $this.parent();
            var currentStyle = document.defaultView.getComputedStyle($parent.get(0), null) || $parent.get(0).currentStyle;
            var position = currentStyle.position;
            if (position != "absolute" || position != "relative") {
                $this.css("position", "relative");
            }
            var isrequire = $this.attr("data-isrequire");
            if (isrequire == "true") {
                checkRequre(this);
            }
        });
    }
})(jQuery, window);

; (function ($, root) {
    "use strict"
    function resolutionFormula(formula) {
    }
    function checkFormula(formula) {
        var flag = true;

        return flag;
    }
    function rule(key, value, _super) {
        this.key = key;
        this.value = value;
        this._super = _super;
        this.ele = $('<span class="btn btn-default" data-key="' + this.key + '">' + this.value + '</div>');
    }
    function Formula(target, context) {
        if (!target) { return false; }
        this.formulaRule = [];
        this.entitys = [];
        this.result = null;
        this.formulaTypes = ["+", "-", "*", "/", "(", ")", "="];
        this.formulaTypeBtns = [];
        this.modal = null;
        this.target = target;
        this.context = context;
        this.typeRender = null;
        this.entitysRender = null;
        this.init();
    }
    Formula.prototype.init = function () {
        this.modal = $('<div class="modal fade" id="subGridModal" tabindex="-1" role="dialog" data-isnew="false" aria-labelledby="subGridModalLabel" aria-hidden="true" style="display: block;">');
        this.modal.html('<div class="modal-dialog"><div class="modal-content"><div class="modal-header"></div><div class="modal-body"></div></div></div>');
        this.typeRender = $('<div class="formula-typerender col-sm-7" />');
        this.entitysRender = $('<div class="formula-entitysRender col-sm-5" />')
        this.context.append(this.modal);
        this.renderTypes();
    }
    Formula.prototype.renderTypes = function () {
        var self = this;
        $.each(this.formulaTypes, function (key, obj) {
            var btn = new rule(key, obj);
            self.formulaTypeBtns.push(btn);
            self.typeRender.append(btn.ele, self);
        });
    }
    Formula.prototype.renderEntitys = function (entitys) {
        var self = this;
        if (!this.entitys && entitys) {
            this.entitysRender.append('<select></select>');
            var _html = [];
            $.each(this.formulaTypes, function (key, obj) {
                _html.push('')
            });
        }
    }
    Formula.prototype.getEntitys = function (entitys) {
        this.entitys = entitys;
    }
    Formula.prototype.modalShow = function () {
        this.modal.addClass("in");
    }
    Formula.prototype.modalHide = function () {
        this.modal.removeClass("in");
    }
    //root.xmsFormula = {
    //    checkFormula: checkFormula,
    //    resolutionFormula: resolutionFormula
    //}
})(jQuery, window);

(function (root, $) {
    function dragable(type) {
        var self = this;
        var mouse = { x: 0, y: 0 };
        var starPosition = {};

        type = type || "position";
        var movePixel = 0;

        this.ele.on("mousedown", function (e) {
            e = e || window.event;
            if (e && e.stopPropagation) {
                // 因此它支持W3C的stopPropagation()方法
                e.stopPropagation();
            } else {
                //否则，我们需要使用IE的方式来取消事件冒泡
                window.event.cancelBubble = true;
            }
            var target = e.target || e.srcElement;

            var that = this;
            var isDown = false;
            self.ele.css({ "position": "absolute" });
            starPosition.x = e.pageX;
            starPosition.y = e.pageY;
            var pos = {
                x: self.ele[type]().left,
                y: self.ele[type]().top
            };
            var offsetLeft = starPosition.x - pos.x;
            var offsetTop = starPosition.y - pos.y;
            console.log(offsetLeft, offsetTop)

            //防止出现蓝色选中区域
            document.unselectable = "on";
            document.onselectstart = function () {
                return false;
            }
            $(document).on("mousemove", function (e) {
                mouse.x = e.pageX;
                mouse.y = e.pageY;
                self.ele.css({
                    "left": mouse.x - offsetLeft,
                    "top": mouse.y - offsetTop,
                });
            });
            $(document).on("mouseup", function (e) {
                $(document).off("mouseup");
                $(document).off("mousemove");
                //self.ele.css({
                //    "left": 'auto',
                //    "top": 'auto',
                //});
            });
        });
    }
    function xmsList(key, value) {
        this.list = [];
        this.length = 0;
    }
    xmsList.prototype.add = function (item) {
        this.list.push(item);
        this.length++;
    }
    xmsList.prototype.remove = function (key) {
        var index = this.indexOf(key);
        if (index == -1) return false;
        this.list.splice(index, 1);
    }
    xmsList.prototype.indexOf = function (item) {
        var index = -1;
        if (typeof item === "string") {
            $.each(this.list, function (key, obj) {
                if (item == obj.id) {
                    index = key;
                    return false;
                }
            });
        } else {
            $.each(this.list, function (key, obj) {
                if (item.id == obj.id) {
                    index = key;
                    return false;
                }
            });
        }
        return index;
    }
    function xmsFormula(key, value, type, context, _super, noadd) {
        this.key = key;
        this.value = value;
        this.type = type;
        this.context = context;
        this._super = _super;
        this.id = "formulaId_" + setTimeout(0);
        this.ele = $('<div class="formula-item-ele" id="' + this.id + '" data-type="' + this.type + '" />');
        this.close = $('<div class="formula-item-close">x</div>');
        this.text = $('<span class="formula-item-text">' + this.value + '</span>');
        if (!noadd) {
            this.ele.append(this.text).append(this.close);
            this.context.append(this.ele);
            this.bindEvent();
            // this.dragable();
        }
    }
    xmsFormula.prototype.bindEvent = function () {
        var self = this;
        this.close.on("click", function () {
            self.remove();
        });
    }
    xmsFormula.prototype.dragable = function () {
        dragable.call(this);
    }
    xmsFormula.prototype.remove = function () {
        this._super.removeItem(this.id);
    }
    function xmsFormulaList(context, _super, id) {
        this.context = context;
        this._super = _super;
        this.list = [];
        this.length = 0;
        this.viewid = '';
        this.id = id || "formulaList_" + setTimeout(0);
        this.ele = $('<div class="formula-list-item clearfix" id="' + this.id + '" />');
        this.close = $('<div class="formula-item-close">x</div>');
        this.ele.append(this.text);//.append(this.close);
        //this.context.append(this.ele);
    }
    xmsFormulaList.prototype = $.extend({}, xmsList.prototype, xmsFormulaList.prototype);
    xmsFormulaList.prototype.addItem = function (key, value, type, noadd) {
        var item = new xmsFormula(key, value, type, this.ele, this, noadd);
        this.add(item);
        //this.resetPos();
        return item;
    }
    xmsFormulaList.prototype.resetPos = function () {
        if (this.length == 0) return false;
        var offsetL = 10;
        $.each(this.list, function (key, item) {
            var w = item.ele.outerWidth();

            if (w < 24) w = 24;
            var pos = key * (w + offsetL);
            console.log(pos);
            item.ele.css({
                "left": pos
            })
        });
    }
    xmsFormulaList.prototype.removeItem = function (id) {
        var index = this.indexOf(id);
        if (index > -1) {
            this.list[index].ele.remove();
            this.remove(id);
        }
    }
    xmsFormulaList.prototype.getRulerRan = function () {
        var res = '';
        $.each(this.list, function (key, item) {
            if (item.type != 'entity') {
                var kk = item.key;
            } else {
                var kk = Math.floor(Math.random()) * 30 + 1;
            }
            res += kk;
        });
        return res;
    }
    xmsFormulaList.prototype.checkHasItem = function (name) {
        var flag = false;
        $.each(this.list, function (key, item) {
            if (name == item.key) {
                flag = true;
                return false;
            }
        });
        return flag;
    }
    xmsFormulaList.prototype.bindEvent = function () {
        var self = this;
        //this.close.on("click", function () {
        //    self._super.removeList(self.id);
        //});
        this.ele.on('click', function () {
            $(this).siblings().removeClass("active").end().addClass("active");
        });
    }
    function xmsFormulaWrap(ele, id) {
        this.id = id || "formulaWrap_" + setTimeout(0);
        this.ele = ele || null;
        this.list = [];
        this.length = 0;
    }
    xmsFormulaWrap.prototype = $.extend({}, xmsList.prototype, xmsFormulaWrap.prototype);
    xmsFormulaWrap.prototype.addList = function (id) {
        var list = new xmsFormulaList(this.ele, this, id);
        this.add(list);
        return list;
    }
    xmsFormulaWrap.prototype.removeList = function (id) {
        var index = this.indexOf(id);
        if (index > -1) {
            this.list[index].ele.remove();
            this.remove(id);
        }
    }
    xmsFormulaWrap.prototype.clear = function (id) {
        this.list = [];
        this.length = 0;
    }
    xmsFormulaWrap.prototype.render = function (list, viewid) {
        var self = this;
        self.ele.empty();
        if (list.length == 0) return false;
        $.each(list, function (key, obj) {
            if (viewid && viewid == obj.viewid) {
                obj.bindEvent();
                self.ele.append(obj.ele);
                $.each(obj.list, function (i, n) {
                    n.bindEvent();
                });
            } else {
                obj.bindEvent();
                self.ele.append(obj.ele);
                $.each(obj.list, function (i, n) {
                    n.bindEvent();
                });
            }
        });
    }
    xmsFormulaWrap.prototype.getList = function (id) {
        var res = [];
        $.each(this.list, function (key, obj) {
            if (id == obj.id) {
                res.push(obj);
                return false;
            }
        });
        return res;
    }
    xmsFormulaWrap.prototype.checkByOtherList = function (list, value) {//list是不需要查找的列表
        var res = [];
        var flag = false;
        if (this.list.length == 0) return false;
        $.each(this.list, function (key, obj) {
            if (list.id != obj.id) {
                $.each(obj.list, function (i, n) {
                    if ((value == n.id || value == n.key) && n.type == "entity") {
                        flag = true;
                        return false;
                    }
                });
            }
        });
        return flag;
    }
    xmsFormulaWrap.prototype.get = function (id) {
        var res = [];
        $.each(this.list, function (key, obj) {
            $.each(obj.list, function (i, n) {
                if ((id == n.id || id == n.key) && n.type == "entity") {
                    res.push(obj);
                    return false;
                }
            });
        });
        return res;
    }
    xmsFormulaWrap.prototype.getResult = function (id) {
        var res = [];
        $.each(this.list, function (key, obj) {
            var arrtemp = [];
            var name = obj.id;
            var type = 'formular';
            $.each(obj.list, function (i, n) {
                var objtemp = {};
                objtemp.key = n.key;
                objtemp.name = n.value;
                arrtemp.push(objtemp.key);
            });
            if (arrtemp.join('$$$') != '') {
                res.push({ name: name, expression: arrtemp.join('$$$'), type: type });
            }
        });
        return res;
    }
    xmsFormulaWrap.prototype.toJson = function (id) {
        var res = [];
        $.each(this.list, function (key, obj) {
            var arrtemp = [];
            $.each(obj.list, function (i, n) {
                var objtemp = {};
                objtemp.key = n.key;
                objtemp.name = n.value;
                arrtemp.push(objtemp);
            });
            res.push(arrtemp);
        });
        return JSON.stringify(res);
    }
    function xmsFormulaWrapList(ele) {
        this.id = "formulaWrapList_" + new Date().toString(16);
        this.ele = ele;
        this.list = [];
        this.length = 0;
    }
    xmsFormulaWrapList.prototype = $.extend({}, xmsList.prototype, xmsFormulaWrapList.prototype);
    xmsFormulaWrapList.prototype.get = function (id) {
        var index = this.indexOf(id);
        if (index > -1) {
            return this.list[index];
        }
    }
    root.xmsFormHandler = {
        xmsFormulaWrapList: xmsFormulaWrapList,
        xmsFormulaList: xmsFormulaList,
        xmsFormulaWrap: xmsFormulaWrap
    }
})(window, jQuery);

(function (root, $, un) {
    //itemClass,itemValue,itemTitle,itemOther
    var settings = {
        datas: [{ value: '1', title: '1111', other: '' }],
        itemClass: 'xms-autoc-item',
        itemTmpl: '<li class="{{itemClass}}" value="{{itemValue}}">{{itemOther}}{{itemTitle}}</li>',
        dataFilter: null,
        addEvent: null,
        overflowPar: null
    }
    $.fn.xmsAutoComplete = function (opts) {
        opts = $.extend({}, settings, opts);
        var datas = opts.datas;
        if (opts.dataFilter) {
            datas = opts.dataFilter(opts.datas);
        }
        this.each(function () {
            if (!$.isArray(datas)) return false;
            var $this = $(this);
            var $wrap = $('<div class="xms-autoc-wrap"></div>');
            var $ul = $('<ul class="xms-autoc-ul"></ul>');
            $wrap.append($ul);
            var _lis = [];
            $.each(datas, function (key, item) {
                var html = ''
                html = opts.itemTmpl.replace(/{{itemClass}}/g, opts.itemClass);
                html = html.replace(/{{itemValue}}/gi, item.value || '');
                html = html.replace(/{{itemOther}}/gi, item.other || '');
                html = html.replace(/{{itemTitle}}/gi, item.title || '');
                _lis.push(html);
            });
            var ttop = $this.offset().top;
            var tleft = $this.offset().left;
            var tW = $this.outerWidth();
            var tH = $this.outerHeight();
            if (opts.offsetL) {
                tleft += opts.offsetL;
            }

            if (opts.overflowPar && opts.overflowPar.length > 0) {
                var scrollheight = opts.overflowPar.get(0).scrollHeight;

                if (scrollheight > opts.overflowPar.height()) {
                    opts.overflowPar.on('scroll', function () {
                        $wrap.css({ "position": "absolute", "top": ttop + tH - opts.overflowPar.scrollTop(), "left": tleft, "width": tW });
                    })
                }
            }
            $wrap.hide();
            $wrap.css({ "position": "absolute", "top": ttop + tH, "left": tleft, "width": tW });

            $('body').append($wrap);

            $ul.html(_lis.join(''));
            $this.on('focus', function () {
                $wrap.find('li').removeClass('active');
                var value = $this.val();
                if (value != "") {
                    $wrap.find('li[value="' + value + '"]').addClass('active');
                }
                $wrap.show();
            });
            $ul.children('li').on('click', function () {
                $this.val($(this).attr('value'));
                $wrap.hide();
                $this.trigger('xmsAutoItem.click');
            });
            $(document).on('click', function (e) {
                e = e || window.event;
                var target = e.target || e.srcElement;
                if ($(target).closest($wrap).length == 0 && $(target).closest($this).length == 0) {
                    $wrap.hide();
                }
            });
            opts.addEvent && opts.addEvent.apply(this, $wrap);
        });
    }
})(window, jQuery);

; (function (root, $, un) {
    //itemClass,itemValue,itemTitle,itemOther

    function getList(datas, opts, $ul, $this, $wrap) {
        var _lis = [];
        if (datas && datas.length > 0) {
            $.each(datas, function (key, item) {
                var html = ''
                $.each(opts.replaceList, function (i, n) {
                    var reg = new RegExp('{{' + n.key + '}}', 'g');
                    if (i == 0) {
                        html = opts.itemTmpl.replace(reg, opts.itemClass || '');
                    } else {
                        html = html.replace(reg, item[n['value']] || '');
                    }
                });
                _lis.push(html);
            });
        }
        $ul.html(_lis.join(''));
    }

    function autoSearcher(obj, opts, datas) {
        this.box = $(obj);

        this.opts = opts;
        this.datas = datas;
        this.vInput = $('<input class="xms-mutilauto-input form-control"  placeholder="' + (this.opts.inputPlaceHolder || '') + '"  type="text" />');
        if (opts.loadDefaultValue) {
            console.log(this.box.val())
            this.vInput.val(this.box.val());
        }
        this.wrap = $('<div class="xms-mutilauto-wrap"></div>');
        this.wrap.attr('data-isactive', false);
        this.search = $('<div class="xms-mutilauto-search"></div>');
        this.searchType = $('<input type="hidden" data-type="keyword" value="" />');
        this.value = '';
        this.ul = $('<ul class="xms-mutilauto-ul"></ul>');
        this._parent = $(opts.context) || $('body');
        this.box.after(this.wrap);
        this.wrap.append(this.vInput);
        if (opts.searchType) {
            this.wrap.append(this.search);
            this.search.append(this.searchType);
            this.searchType.searchType(opts.searchTypeOpt);
        }
        this.wrap.append(this.ul);
        //this._parent.append(this.wrap);
        this.box.hide();
        this.renderList(datas);
        this.setPos();
        this.bindEvent();
        this.box.attr('data-autosearch', 1);
    }
    autoSearcher.prototype.setPos = function () {
        var self = this;
        //计算位置
        var ttop = this.box.offset().top;
        var tleft = this.box.offset().left;
        var tW = this.box.outerWidth();
        var tH = this.box.outerHeight();
        this.ul.hide();
        //this.ul.css({ "position": "absolute", "top": ttop + tH, "left": tleft, "width": tW });
    }
    autoSearcher.prototype.renderList = function (datas) {
        this.datas = datas;
        getList(datas, this.opts, this.ul, this.box, this.wrap);//生成下拉选项
        this.value = this.ul.find('li:first').attr('value');
    }
    autoSearcher.prototype.filterData = function (datas) {
        var self = this;
        var value = self.vInput.val();
        if (value != "") {
            if (this.ul.find('li:contains("' + value + '")').length > 0) {
                this.ul.find('li').hide();
                this.ul.find('li:contains("' + value + '")').show();
            } else {
                this.ul.find('li').show();
            }
        } else {
            this.ul.find('li').show();
        }
    }
    autoSearcher.prototype.bindEvent = function (datas) {
        var self = this;
        this.ul.children('li').off('click').on('click', function () {
            if (self.opts.defaultSubmit) {
                self.box.val($(this).attr('value'));
                self.ul.hide();
                self.vInput.val($(this).text());
                self.value = $(this).attr('value');
                self.wrap.attr('data-isactive', false);
                self.box.trigger('xmsAutoItem.click');
                self.opts.submithandler && self.opts.submithandler(self, $(this));
            } else {
                self.opts.submithandler && self.opts.submithandler.call(this, self, $(this));
            }
        });

        //绑定事件
        this.vInput.off('click').on('click', function () {
            var active = self.wrap.attr('data-isactive');
            if (active == 'false') {
                self.ul.find('li').removeClass('active');
                var value = self.box.val();
                if (value != "") {
                    self.wrap.find('li[value="' + value + '"]').addClass('active');
                }
                self.filterData();
                self.ul.show();
                self.wrap.attr('data-isactive', true);
            } else {
                self.ul.hide();
                self.wrap.attr('data-isactive', false);
            }
        });
        this.vInput.off('keyup').on('keyup', function (e) {
            e = e || window.event;
            self.filterData();
            self.ul.find('li').removeClass('active');
            self.box.val('');
            self.value = '';
        });
        this.vInput.on('change', function () {
            self.box.val(this.value);
        })
        function documentbind(e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            if ($(target).closest(self.wrap).length == 0 && $(target).closest(self.vInput).length == 0) {
                self.ul.hide();
                self.wrap.attr('data-isactive', false);
            }
        }
        $(document).off('click', documentbind);
        $(document).on('click', documentbind);

        self.opts.addEvent && self.opts.addEvent.apply(self.box, self.wrap);
    }

    var settings = {
        datas: [{ value: '1', title: '1111', other: '' }],
        getType: 'local',//ajax
        ajax: null,
        replaceList: [{ key: 'itemClass', value: 'itemClass' }, { key: 'itemValue', value: 'value' }, { key: 'itemOther', value: 'other' }, { key: 'itemTitle', value: 'title' }],
        itemClass: 'xms-autoc-item',
        itemTmpl: '<li class="{{itemClass}}" value="{{itemValue}}">{{itemOther}}{{itemTitle}}</li>',
        dataFilter: null,
        addEvent: null,
        searchType: null,
        searchTypeOpt: null,
        defaultSubmit: true,
        submithandler: null,
        loadDefaultValue: false
    }
    $.fn.mutilAutoComplete = function (opts) {
        var args = [].slice.call(arguments);
        if (args.length == 1 && typeof opts == 'object') {//返回实例
            opts = $.extend({}, settings, opts);
            return this.each(function () {
                var _self = this;
                if ($(_self).attr('data-autosearch') == 1) {
                    return true;
                }
                var datas = opts.datas;
                if (opts.getType == 'local') {
                    if (opts.dataFilter) {
                        datas = opts.dataFilter(opts.datas);
                    }
                    $(self).data().plugObj = new autoSearcher(_self, opts, datas)
                } else {
                    if (opts.ajax) {
                        Xms.Web.GetJson(opts.ajax.url, opts.ajax.data, function (data) {
                            if (typeof data == "string") {
                                data = JSON.parse(data);
                            }
                            if (opts.dataFilter) {
                                datas = opts.dataFilter(data);
                            }
                            $(self).data().plugObj = new autoSearcher(_self, opts, datas);
                        });
                    }
                }
            });
        } else {//调用实例方法
            return this.each(function () {
                var $this = $(this);
                if ($this.data().plugObj) {
                    if (args.length == 1) {
                        $this.data().plugObj[opts] && $this.data().plugObj[opts].call(this);
                    } else {
                        $this.data().plugObj[args[0]] && $this.data().plugObj[args[0]].call(this, args[1]);
                    }
                }
            });
        }
    }
})(window, jQuery);

//实体下拉选择列表
; (function (root, $, un) {
    function getList(datas, opts, $ul, $this, $wrap, $listWrap, $groupselecter, groupdatas) {
        var _lis = [];
        var groups = [];
        var groupshtmls = [];
        if (datas && datas.length > 0) {
            $.each(datas, function (key, item) {
                var html = ''
                if (groupdatas.content && groupdatas.content.length > 0) {
                    $.each(groupdatas.content, function (i, n) {
                        if ($.inArray(n, groups) == -1) {
                            groups.push(n);
                            $groupselecter.append('<option value="' + n.entitygroupid + '">' + n.name + '</option>');
                        }
                    });
                }
                $.each(opts.replaceList, function (i, n) {
                    var reg = new RegExp('{{' + n.key + '}}', 'g');
                    if (i == 0) {
                        html = opts.itemTmpl.replace(reg, opts.itemClass || '');
                    } else {
                        var val = item[n['value']] || '';
                        if (val != '' && val.indexOf("[") != -1) {
                            val = encodeURIComponent(item[n['value']])
                        }
                        html = html.replace(reg, val || '');
                    }
                });
                _lis.push(html);
            });
        }
        $ul.html(_lis.join(''));
    }

    function autoSearcher(obj, opts, datas, groupdatas) {
        this.box = $(obj);

        this.opts = opts;
        this.datas = datas;
        this.groupdatas = groupdatas;
        var disabled = this.opts.disabled ? ' disabled ' : '';
        this.vInput = $('<input class="xms-mutilauto-input form-control"  ' + disabled + ' placeholder="选择实体"  type="text" />');
        if (opts.loadDefaultValue) {
            console.log(this.box.val())
            this.vInput.val(this.box.val());
        }

        this.wrap = $('<div class="xms-mutilauto-wrap xms-mutilauto-entitywrap"></div>');
        this.inputWrap = $('<div class="xms-mutilauto-inputwrap input-group"><div class="input-group-btn" data-action="remove"><span class="btn btn-default" ' + disabled + ' ><em class=" glyphicon glyphicon-remove"></em></span></div></div>')
        this.wrap.attr('data-isactive', false);
        this.search = $('<div class="xms-mutilauto-search"></div>');
        this.searchType = $('<input type="hidden" ' + disabled + ' placeholder="' + (this.opts.inputPlaceHolder || '') + '" data-type="keyword" value="" />');
        this.listWrap = $('<div class="xms-mutilauto-listwrap"></div>');
        this.selecterWrap = $('<div class="input-group xms-mutilauto-selecterwrap"><span class="input-group-addon"><em class="glyphicon glyphicon-filter"></em></span></div>')
        this.groupSelecter = $('<select class="form-control" ' + disabled + '><option value="-1">实体分组</option></select>');
        this.value = '';
        this.ul = $('<ul class="xms-mutilauto-ul"></ul>');
        this._parent = $(opts.context) || $('body');
        this.box.after(this.wrap);
        this.wrap.append(this.inputWrap);
        this.inputWrap.prepend(this.vInput);
        if (opts.searchType) {
            this.wrap.append(this.search);
            this.search.append(this.searchType);
            this.searchType.searchType(opts.searchTypeOpt);
        }
        this.selecterWrap.append(this.groupSelecter);
        this.listWrap.append(this.selecterWrap).append(this.ul);
        this.wrap.append(this.listWrap);

        //this._parent.append(this.wrap);
        this.box.hide();
        this.renderList(datas);
        this.setPos();
        if (!this.opts.disabled) {
            this.bindEvent();
        }
        this.box.attr('data-autosearch', 1);
    }
    autoSearcher.prototype.setPos = function () {
        var self = this;
        //计算位置
        var ttop = this.box.offset().top;
        var tleft = this.box.offset().left;
        var tW = this.box.outerWidth();
        var tH = this.box.outerHeight();
        this.listWrap.hide();
        //this.ul.css({ "position": "absolute", "top": ttop + tH, "left": tleft, "width": tW });
    }
    autoSearcher.prototype.renderList = function (datas) {
        this.datas = datas;
        getList(datas, this.opts, this.ul, this.box, this.wrap, this.listWrap, this.groupSelecter, this.groupdatas);//生成下拉选项
        this.value = this.ul.find('li:first').attr('value');
        this.opts.rendered && this.opts.rendered(this, this.box, this.wrap);
    }
    autoSearcher.prototype.filterData = function (datas) {
        var self = this;
        var value = self.vInput.val();
        var groupid = self.groupSelecter.val();
        if (value != "" || groupid != "-1") {
            if (this.ul.find('li:contains("' + value + '")').length > 0) {
                this.ul.find('li').hide();
                if (value != '' && groupid == "-1") {
                    this.ul.find('li:contains("' + value + '")').show();
                } else if (value == '' && groupid != "-1") {
                    this.ul.find('li').each(function () {
                        if ($(this).attr('data-groupid').indexOf(groupid) != -1) {
                            $(this).show();
                        }
                    });
                } else if (value != '' && groupid != "-1") {
                    this.ul.find('li').each(function () {
                        if ($(this).text().indexOf(value) != -1 && $(this).attr('data-groupid').indexOf(groupid) != -1) {
                            $(this).show();
                        }
                    });
                }
            } else {
                this.ul.find('li').show();
            }
        } else {
            this.ul.find('li').show();
        }
    }
    autoSearcher.prototype.bindEvent = function (datas) {
        var self = this;
        this.ul.children('li').off('click').on('click', function () {
            if (self.opts.defaultSubmit) {
                self.box.val($(this).attr('value'));
                self.listWrap.hide();
                self.vInput.val($(this).text());
                self.value = $(this).attr('value');
                self.wrap.attr('data-isactive', false);
                self.box.trigger('xmsAutoItem.click');
                self.opts.submithandler && self.opts.submithandler(self, $(this));
            } else {
                self.opts.submithandler && self.opts.submithandler.call(this, self, $(this));
            }
        });
        this.wrap.find('[data-action="remove"]').off('click').on('click', function (e) {
            self.box.val('');
            self.vInput.val('');
            self.value = '';
            self.filterData();
            self.wrap.attr('data-isactive', false);
            self.ul.find('li').removeClass('active');
            self.opts.removehandler && self.opts.removehandler.call(this, self, $(this));
        });
        this.groupSelecter.off('change').on('change', function (e) {
            var val = $(this).val();
            self.filterData();
            self.listWrap.show();
            self.wrap.attr('data-isactive', true);
        });
        //绑定事件
        this.vInput.off('click').on('click', function () {
            var active = self.wrap.attr('data-isactive');
            if (active == 'false') {
                self.ul.find('li').removeClass('active');
                var value = self.box.val();
                if (value != "") {
                    self.wrap.find('li[value="' + value + '"]').addClass('active');
                }
                self.filterData();
                self.listWrap.show();
                self.wrap.attr('data-isactive', true);
            } else {
                self.listWrap.hide();
                self.wrap.attr('data-isactive', false);
            }
        });
        this.vInput.off('keyup').on('keyup', function (e) {
            e = e || window.event;
            self.filterData();
            self.ul.find('li').removeClass('active');
            self.box.val('');
            self.value = '';
        });
        this.vInput.on('change', function () {
            self.box.val(this.value);
        })
        function documentbind(e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            if ($(target).closest(self.wrap).length == 0 && $(target).closest(self.vInput).length == 0) {
                self.listWrap.hide();
                self.wrap.attr('data-isactive', false);
            }
        }
        $(document).off('click', documentbind);
        $(document).on('click', documentbind);

        self.opts.addEvent && self.opts.addEvent.apply(self.box, self.wrap);
    }

    var settings = {
        // datas: [{ value: '1', title: '1111', other: '' }],
        getType: 'ajax',//ajax
        ajax: {
            url: "/api/schema/entity",
            data: { getall: true },
        },
        replaceList: [{ key: 'itemClass', value: 'itemClass' }, { key: 'itemValue', value: 'entityid' }, { key: 'groupid', value: 'entitygroups' }, { key: 'itemOther', value: 'other' }, { key: 'itemTitle', value: 'localizedname' }],
        itemClass: 'xms-autoc-item',
        itemTmpl: '<li class="{{itemClass}}" data-groupid="{{groupid}}" value="{{itemValue}}">{{itemOther}}{{itemTitle}}</li>',
        dataFilter: function (data) {
            return data.content;
        },
        addEvent: null,
        searchType: null,
        searchTypeOpt: null,
        defaultSubmit: true,
        submithandler: null,
        loadDefaultValue: false
    }
    $.fn.entitySelector = function (opts) {
        var args = [].slice.call(arguments);
        if (args.length == 1 && typeof opts == 'object') {//返回实例
            opts = $.extend({}, settings, opts);
            return this.each(function () {
                var _self = this;
                if ($(_self).attr('data-autosearch') == 1) {
                    return true;
                }
                var datas = opts.datas;
                if (opts.getType == 'local') {
                    if (opts.dataFilter) {
                        datas = opts.dataFilter(opts.datas);
                    }
                    $(self).data().entitySelector = new autoSearcher(_self, opts, datas, groupdatas)
                } else {
                    if (opts.ajax) {
                        Xms.Web.GetJson(opts.ajax.url, opts.ajax.data, function (data) {
                            if (typeof data == "string") {
                                data = JSON.parse(data);
                            }
                            console.log('entitys', data);
                            if (opts.dataFilter) {
                                datas = opts.dataFilter(data);
                            }
                            Xms.Ajax.GetJson('/api/data/retrieve/all/entitygroup/name/name:asc', null, function (response) {
                                var groupdatas = Xms.Web.GetAjaxResult(response);
                                $(self).data().entitySelector = new autoSearcher(_self, opts, datas, groupdatas);
                            });
                        });
                    }
                }
            });
        } else {//调用实例方法
            return this.each(function () {
                var $this = $(this);
                if ($this.data().entitySelector) {
                    if (args.length == 1) {
                        $this.data().entitySelector[opts] && $this.data().entitySelector[opts].call(this);
                    } else {
                        $this.data().entitySelector[args[0]] && $this.data().entitySelector[args[0]].call(this, args[1]);
                    }
                }
            });
        }
    }
})(window, jQuery);

(function (root) {
    function xmsImmediate(delta) {
        var timeoutID = null;
        var safe = true;
        return function (action) {
            if (safe) {
                action && action();
                safe = false;
            } else {
                return false;
            }
            clearTimeout(timeoutID);
            timeoutID = setTimeout(function () {
                safe = true;
            }, delta);
        };
    }
    root.xmsImmediate = xmsImmediate;
})(window);
(function ($, root, un) {
})(jQuery, window);

(function ($, root, un) {
    function getItem(datas, id, childname) {
        var res = null;
        for (var i = 0, len = datas.length; i < len; i++) {
            var item = datas[i];
            //console.log()
            if (item.id == id) {
                res = item;
                break;
            } else {
                if (item[childname].length > 0) {
                    res = getItem(item[childname], id, childname);
                    if (res) {
                        break;
                    }
                }
            }
        }

        return res;
    }
    function dataApax(source, opts) {
        this._source = source;
        this.opts = opts;
        this._datas = [];
        this.oldrecords = [];
        this.records = [];
        this.filter = null;
        this.recordsCount = 0;
    }
    dataApax.prototype.dataBind = function () {
        var self = this, _s = this._source, _datafields;
        if (_s) {
            //解析datafields
            _datafields = _s.datafields || _s.dataFields;
            this.filter = _s.filter || null;
            this._datas = _s.localdatas || _s.localdata;
            //把需要的字段存放到 records里，做后续处理
            if (this._datas) {
                this.recordsCount = this._datas.length;
                for (var i = 0, len = this.recordsCount; i < len; i++) {
                    var item = this._datas[i];
                    var _obj = {};
                    for (var j = 0, jlen = _datafields.length; j < jlen; j++) {
                        var jitem = _datafields[j];
                        if (!jitem['name'] || jitem['name'] == un) {
                            throw new Error('必须要有name');
                        }
                        //检查值类型是否正确
                        if (jitem['type'] && core.isString(jitem['type'])) {
                        }
                        _obj[jitem['name']] = item[jitem['name']];
                    }
                    this.records.push(_obj);
                }
            }
            this.oldrecords = this.records;
        }
    }
    dataApax.prototype.getRecords = function () {
        return this.records;
    }

    //构建树结构数据
    //返回一个结构数组从一组平面数据。方法有4个参数,最后2是可选的。第一个参数是该领域的id。第二个参数代表父字段的id。这些参数应该指向一个有效datafield从数据源。可选的第三个参数指定的名称“孩子”集合。最后一个参数指定了数据源之间的映射字段和自定义数据字段。
    //var records = getRecordsHierarchy('id', 'parentid', 'items', [{ name: 'text', map: 'label' }]);
    dataApax.prototype.getRecordsHierarchy = function (id, parentid, childsname, maps) {
        var _records = [];
        if (!id || !parentid) throw new Error();
        if (this.recordsCount == 0) return _records;
        var temps = $.extend([], this.records);
        var tempsLen = temps.length;
        childsname = childsname || 'children';
        console.log('temps', temps)
        var __layer = 1;
        // return false;
        function toTreeData(data) {
            var tree = [];
            // console.log(data)
            //构建树根节点
            for (var i = 0; i < data.length; i++) {
                if (!data[i][parentid] || data[i][parentid] == '') {
                    data[i][childsname] = [];
                    if (maps) {
                        for (var k = 0, klen = maps.length; k < klen; k++) {
                            data[i][maps[k]['map']] = data[i][maps[k]['name']] || '';
                        }
                    }
                    data[i].__layer = __layer;
                    tree.push(data[i]);
                    data.splice(i, 1);
                    i--;
                }
            }
            var j = 0;
            // console.log(tree);
            var step = 0, maxStep = 4000;//防止死循环
            //  return false;
            while (data.length > 0) {
                step++;
                var finded = null;
                if (data[j][parentid] == data[j][id]) {
                    data.splice(j, 1);
                    j--;
                    continue;
                }
                if (tree.length > 0) {
                    finded = getItem(tree, data[j][parentid], childsname);
                }

                if (finded) {
                    // __layer++;
                    data[j][childsname] = [];
                    if (maps) {
                        for (var k = 0, klen = maps.length; k < klen; k++) {
                            data[j][maps[k]['map']] = data[j][maps[k]['name']] || '';
                        }
                    }
                    console.log(finded.__layer, finded);
                    if (finded.__layer) {
                        data[j].__layer = finded.__layer + 1;
                    }
                    finded[childsname].push(data[j]);
                    data.splice(j, 1);
                }
                j++;
                if (j > data.length - 1) {
                    j = 0;
                }
                if (maxStep < step) {
                    break;
                }
            }
            //console.log(data)
            return tree;
        }
        return toTreeData(temps);
    }

    dataApax.prototype.getAggregatedData = function (_settings) {
    }

    /*
    var source =
    {
        localdata: data,
        datatype: "array",
        datafields:
        [
            { name: 'firstname', type: 'string' },
            { name: 'lastname', type: 'string' },
            { name: 'productname', type: 'string' },
            { name: 'available', type: 'bool' },
            { name: 'quantity', type: 'number' },
            { name: 'price', type: 'number' }
        ],
        id:'id'
    };
    var aggregate = dataAdapter.getAggregatedData([{ name: 'price', aggregates: ['min', 'max', 'sum', 'avg'], formatStrings: ['c2', 'c2', 'c2', 'c2']}]);
    var avg = aggregate.price.avg;
    var min = aggregate.price.min;
    var max = aggregate.price.max;
    var sum = aggregate.price.sum;
    */

    root.dataApax = dataApax;
})(jQuery, window);

(function ($, root, un) {
    var settings = {
        source: [],
        width: 150,
        height: 500,
        theme: 'bootstrap',
        isEmpty: true,
        type: 'append',//插入到页面的方式
        customHtml: null
    }
    var createfirst = true;
    var layerlist = ['first', 'second', 'third', 'forth', 'fifth'];
    var layercount = 0;
    function createTreeHtml(datas) {
        var html = [];
        if (createfirst) {
            html.push('<ul class="xjqtree-root xjqtree-list xjqtree-layer-' + layerlist[layercount] + '">');
        } else {
            html.push('<ul class="xjqtree-list xjqtree-layer-' + layerlist[layercount] + '"">');
        }
        layercount++;
        createfirst = false;
        for (var i = 0, len = datas.length; i < len; i++) {
            var item = datas[i];
            html.push('<li id="xjqtree_' + item.id + '" data-parentid="' + (item.parentid || "") + '" data-id="' + item.id + '" class="xjqtree-item" role="">');
            if (item.items.length > 0) {
                html.push('<span class="xjqtree-cret  glyphicon glyphicon-triangle-right"></span>');
            }
            html.push('<div class="xjqtree-item-label" data-parentid="' + (item.parentid || "") + '" data-id="' + item.id + '">' + item.label + '</div>');
            if (item.items.length > 0) {
                html.push(createTreeHtml(item.items))
            }
            html.push('</li>');
        }
        html.push('</ul>');

        return html.join('');
    }

    function getItem(datas, id, childname) {
        childname = childname || 'items';
        var res = null;
        for (var i = 0, len = datas.length; i < len; i++) {
            var item = datas[i];
            //console.log()
            if (item.id == id) {
                res = item;
                break;
            } else {
                if (item[childname].length > 0) {
                    res = getItem(item[childname], id, childname);
                    if (res) break;
                }
            }
        }

        return res;
    }

    function qTree(obj, opts) {
        this.box = $(obj);
        this.opts = opts;
        this.box.css({ 'height': opts.height, 'width': opts.width })
            .addClass('xjqtree-wrap');
        this.source = opts.source;
        this.panel = $('<div class="xjq-panel"></div>');
        this.treeBox = $('<div class="xjq-tree-box"></div>');
        this.panel.append(this.treeBox);
        if (opts.customHtml) {
            var html = opts.customHtml(this.source, opts.customHtml);
        } else {
            var html = createTreeHtml(this.source);
        }
        this.treeBox.html(html);
        this.render();
        this.bindEvent();
    }
    qTree.prototype.render = function () {
        this.box.data().xjqTree = null;
        if (this.opts.isEmpty) {
            this.box.empty()[this.opts.type](this.panel);
        } else {
            this.box[this.opts.type](this.panel);
        }
    }
    qTree.prototype.getItem = function (id) {
        var res = getItem(this.source, id);
        console.log('getItem', this.source, res);
        return res;
    }
    qTree.prototype.hide = function (id) {
        this.panel.hide();
    }
    qTree.prototype.show = function (id) {
        this.panel.show();
    }
    qTree.prototype.setStyle = function (styles) {
        this.panel.css(styles);
    }
    qTree.prototype.expandAll = function () {
        this.panel.find('.xjqtree-list>.xjqtree-item').addClass('tree-open');
    }
    qTree.prototype.closeAll = function () {
        this.panel.find('.xjqtree-list>.xjqtree-item').removeClass('tree-open');
    }
    qTree.prototype.bindEvent = function () {
        var self = this;
        this.panel.off('click');
        this.panel.on('click', '.xjqtree-cret', function (e) {
            var $this = $(this);
            var _par = $this.parent();
            if (_par.hasClass('tree-open')) {
                _par.removeClass('tree-open')
            } else {
                _par.addClass('tree-open')
            }
        });
        this.panel.on('click', '.xjqtree-item-label', function (e) {
            //  console.log(e);
            self.box.trigger('itemClick', e);
            //self.trigger('itemClick', e);
        });
    }

    $.fn.xjqTree = function (opts, _config) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, settings, opts);
            this.each(function () {
                var $this = $(this);
                createfirst = true;
                var _tree = new qTree(this, opts);
                $this.data().xjqTree = _tree;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().xjqTree) {
                    // console.log(_config);
                    if (!$this.data().xjqTree[opts]) throw new Error('没有这个方法');
                    res = $this.data().xjqTree[opts](_config);
                    return false;
                }
            });
            return res;
        }
    }
    function createYearList(start, end, format) {
        format = format || '{year}年';
        var $optionset = $('#optionset-picklist').find('tbody');

        for (start; start <= end; start++) {
            var _clone = $optionset.find('tr:first').clone();
            $optionset.append(_clone);
            _clone.find('input[name=optionsetname]').val(format.replace(/{year}/g, start));
            _clone.find('input[name=optionsetvalue]').val(start);
        }
        var _clone1 = $optionset.find('tr:first').clone();
        _clone1.remove();
    }
})(jQuery, window);

(function ($, root, un) {
    function mutilCheckbox() {
        this.values = [];
        var _id = new Date() * 1 + (Math.random() * 100);
        this.id = 'mutilCrossPage_' + _id.toString(16);
        this.dom = $('<input type="hidden" value="" id="' + this.id + '">');
        $('body').append(this.dom);
    }
    mutilCheckbox.prototype.add = function (value) {
        var index = $.inArray(value, this.values);
        console.log(index);
        if (!~index) {
            this.values.push(value);
        }
    }
    mutilCheckbox.prototype.del = function (value) {
        var index = $.inArray(value, this.values);
        console.log(index);
        if (~index) {
            this.values.splice(index, 1);
        }
    }
    mutilCheckbox.prototype.getValues = function (value) {
        return this.values;
    }
    mutilCheckbox.prototype.getValuesTostring = function (type) {
        return this.values.join(type);
    }
    mutilCheckbox.prototype.getValueByDom = function (type) {
        return this.dom.val();
    }
    mutilCheckbox.prototype.setValueByDom = function (type) {
        return this.dom.val(this.getValuesTostring());
    }
    root.xmsMutilCheckbox = mutilCheckbox;
})(jQuery, window);

(function ($, root, un) {
    var defaults = {
    }
    function ToAreaDom(obj, opts) {
        this.dom = $(obj);
        this.opts = opts;
        this.areaDom = $('<textarea class="form-control input-sm"></textarea>');
        this.dom.after(this.areaDom);
        this.areaDom.val(this.dom.val());
        this.dom.hide();
        this.init();
    }
    ToAreaDom.prototype.init = function () {
        var self = this;
        this.areaDom.on('keyup', function () {
            var _val = this.value;
            //console.log(_val);
            self.dom.val(_val);
            self.dom.trigger('change');
        });
    }
    $.fn.inputToTextArea = function (opts, _config) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                createfirst = true;
                var _tree = new ToAreaDom(this, opts);
                $this.data().ToAreaDom = _tree;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().ToAreaDom) {
                    // console.log(_config);
                    if (!$this.data().ToAreaDom[opts]) throw new Error('没有这个方法');
                    res = $this.data().ToAreaDom[opts](_config);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

(function ($, root, un) {
    var defaults = {
        datas: [],
        datamap: [{ key: 'label', value: 'label' }],
        other: { otherHtml: '', otherDatamap: [] },
        itemTemplate: '<li class="flowline-item flowline-{type}" data-order="{order}"><span class="flowline-circle"></span><div class="flowline-label">{label}</div><div class="flowline-info clearfix"><p>处理者</p><p>状态</p><p>意见/说明</p><p>附件</p><p>处理时间</p>{children}</div></li>'
    }
    function flowLine(obj, opts) {
        this.dom = $(obj);
        this.opts = opts;
        this.wrap = $('<div class="flowline-wrap"></div>');
        this.list = $('<ul class="flowline-list"></ul>');

        this.dom.append(this.wrap.append(this.list));
    }
    flowLine.prototype.stepOther = function (type) {
        type = type || 'append'
        var self = this, _child = this.opts.other.otherHtml;
        var _temp = _child;
        $.each(this.opts.other.otherDatamap, function (i, n) {
            var _reg = new RegExp('{' + n.key + '}', 'gmi');
            _temp = _temp.replace(_reg, n.value);
            _child = _child.replace(_reg, n.value);
        });
        this.wrap[type](_temp);
    }
    flowLine.prototype.createsteplist = function () {
        var self = this;
        var datas = this.opts.datas;
        var tmpl = this.opts.itemTemplate;
        var _html = $.map(datas, function (item, key) {
            if (item.length > 0) {
                var _temp = tmpl;
                var itemlen = item.length - 1;
                $.each(item, function (ii, nn) {
                    var _child = '<p><span></span>&nbsp;<span>{handler}</span></p><p><span>&nbsp;</span><span>{state}</span></p><p><span>&nbsp;</span><span>{desc}</span></p><p><span></span>&nbsp;<span>{attach}</span></p><p><span>&nbsp;</span><span>{date}</span></p>';
                    $.each(self.opts.datamap, function (i, n) {
                        var _reg = new RegExp('{' + n.key + '}', 'gmi');
                        _temp = _temp.replace(_reg, nn[n.value]);
                        _child = _child.replace(_reg, nn[n.value]);
                    });
                    if (itemlen > ii) {
                        _temp = _temp.replace(/{children}/gmi, _child + '{children}');
                    } else {
                        _temp = _temp.replace(/{children}/gmi, _child);
                    }
                });
                return _temp;
            }
        });
        console.log(_html);
        this.list.html(_html.join(''));
    }
    $.fn.flowLine = function (opts, _config) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _tree = new flowLine(this, opts);
                $this.data().flowLine = _tree;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().flowLine) {
                    // console.log(_config);
                    if (!$this.data().flowLine[opts]) throw new Error('没有这个方法');
                    res = $this.data().flowLine[opts](_config);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

(function ($, root, un) {
    var defaults = {
        minWidth: '0',
        minHeight: '0',
        maxWidth: '1000',
        maxHeight: '1000',
        offsetWidth: '0',
        offsetHeight: '0',
        isActive: [1, 1, 1, 1],//左上，右上，右下，左下
        start: null,
        drag: null,
        stop: null
    }
    var _pos = {
        'lefttop': { 'position': 'absolute', 'left': 0, top: 0, 'cursor': 'nwse-resize' },
        'leftbottom': { 'position': 'absolute', 'left': 0, bottom: 0, 'cursor': 'nesw-resize', 'top': 'auto' },
        'righttop': { 'position': 'absolute', 'right': 0, top: 0, 'cursor': 'nesw-resize' },
        'rightbottom': { 'position': 'absolute', 'right': 0, bottom: 0, 'cursor': 'nwse-resize', 'top': 'auto', left: 'auto' }
    }
    function ResizeBox(obj, opts) {
        this.dom = $(obj);
        this.opts = opts;
        this.leftTop = $('<div class="resizebox-lefttop resizebox-control"></div>');
        this.leftBottom = $('<div class="resizebox-leftbottom resizebox-control"></div>');
        this.rightTop = $('<div class="resizebox-righttop resizebox-control"></div>');
        this.rightBottom = $('<div class="resizebox-rightbottom resizebox-control"></div>');
        this.opts.isActive[0] && this.dom.append(this.leftTop);
        this.opts.isActive[3] && this.dom.append(this.leftBottom);
        this.opts.isActive[2] && this.dom.append(this.rightBottom);
        this.opts.isActive[1] && this.dom.append(this.rightTop);
        this.leftTop.css(_pos['lefttop']);
        this.leftBottom.css(_pos['leftbottom']);
        this.rightTop.css(_pos['righttop']);
        this.rightBottom.css(_pos['rightbottom']);
        this.init();
        this.bindEvent();
    }

    function resizeMove() {
    }
    function resizeEnd() {
    }
    ResizeBox.prototype.init = function () {
        var self = this;
        var dom_offset = self.dom.offset();
        var dom_size = { w: self.dom.width(), h: self.dom.height() };
        self.dom.css({ 'left': dom_offset.left, 'top': dom_offset.top, 'margin': 0, 'position': 'absolute' });
    }
    ResizeBox.prototype.bindEvent = function () {
        var self = this;
        console.log('ResizeBox', $.fn.draggable);
        if ($.fn.draggable) {
            var dom_offset = { left: 0, top: 0 }; self.dom.offset();
            var dom_size = { w: self.dom.width(), h: self.dom.height() };
            this.opts.isActive[0] && this.leftTop.draggable({
                start: function (event, ui) {
                    console.log(event, ui);
                    var sour = ui.helper, target = $(event.target);
                    dom_offset = self.dom.offset();
                    dom_size = { w: self.dom.width(), h: self.dom.height() };
                },
                drag: function (event, ui) {
                    var sour = ui.helper, target = $(event.target);

                    var offset = ui.offset;

                    var tempOffsetX = offset.left - dom_offset.left;
                    var tempOffsetY = offset.top - dom_offset.top;
                    console.log(event, ui, tempOffsetX, tempOffsetY, dom_offset, dom_size);
                    self.dom.css({ 'left': dom_offset.left + tempOffsetX, 'top': dom_offset.top + tempOffsetY, 'width': dom_size.w + (-tempOffsetX), 'height': dom_size.h + (bnmtempOffsetY) })
                    // console.log(event, ui.offset);
                }
                , stop: function (event, ui) {
                    var sour = ui.helper, target = $(event.target);
                    sour.css(_pos['lefttop'])
                }
            });
            this.opts.isActive[3] && this.leftBottom.draggable({
                start: function (event, ui) {
                    console.log(event, ui);
                    var sour = ui.helper, target = $(event.target);
                    dom_offset = self.dom.offset();
                    dom_size = { w: self.dom.width(), h: self.dom.height() };
                },
                drag: function (event, ui) {
                    var sour = ui.helper, target = $(event.target);

                    var offset = ui.offset;

                    var tempOffsetX = offset.left - dom_offset.left;
                    var tempOffsetY = offset.top - dom_offset.top;
                    console.log(event, ui, tempOffsetX, 'tempOffsetY:' + tempOffsetY, dom_offset, dom_size);
                    self.dom.css({ 'left': dom_offset.left + tempOffsetX, 'top': dom_offset.top + (tempOffsetY - dom_size.h), 'width': dom_size.w + (-(tempOffsetX)), 'height': dom_size.h + (tempOffsetY - dom_size.h) });
                    self.opts.drag && self.opts.drag.call(self, 'righttop', event, ui, tempOffsetX, tempOffsetY);
                    // console.log(event, ui.offset);
                }
                , stop: function (event, ui) {
                    var sour = ui.helper, target = $(event.target);
                    sour.css(_pos['leftbottom'])
                }
            });
            this.opts.isActive[1] && this.rightTop.draggable({
                start: function (event, ui) {
                    console.log(event, ui);
                    var sour = ui.helper, target = $(event.target);
                    dom_offset = self.dom.offset();
                    dom_size = { w: self.dom.width(), h: self.dom.height() };
                },
                drag: function (event, ui) {
                    var sour = ui.helper, target = $(event.target);

                    var offset = ui.offset;

                    var tempOffsetX = offset.left - dom_offset.left;
                    var tempOffsetY = offset.top - dom_offset.top;
                    console.log(event, ui, tempOffsetX, tempOffsetY, dom_offset, dom_size);
                    self.dom.css({ 'left': tempOffsetX, 'top': tempOffsetY, 'width': dom_size.w + (-tempOffsetX), 'height': dom_size.h + (-tempOffsetY) });

                    // console.log(event, ui.offset);
                }
                , stop: function (event, ui) {
                    var sour = ui.helper, target = $(event.target);
                    sour.css(_pos['lefttop'])
                }
            });
            this.opts.isActive[2] && this.rightBottom.draggable({
                start: function (event, ui) {
                    console.log(event, ui);
                    var sour = ui.helper, target = $(event.target);
                    dom_offset = self.dom.offset();
                    dom_size = { w: self.dom.width(), h: self.dom.height() };
                    self.opts.start && self.opts.start('rightbottom', event, ui);
                },
                drag: function (event, ui) {
                    var sour = ui.helper, target = $(event.target);

                    var offset = ui.offset;

                    var tempOffsetX = offset.left - dom_offset.left;
                    var tempOffsetY = offset.top - dom_offset.top;
                    console.log(event, offset, tempOffsetX, tempOffsetY, dom_offset, dom_size);
                    self.dom.css({ 'width': tempOffsetX, 'height': tempOffsetY });
                    self.opts.drag && self.opts.drag.call(self, 'rightbottom', event, ui, tempOffsetX, tempOffsetY);
                    // console.log(event, ui.offset);
                }
                , stop: function (event, ui) {
                    var sour = ui.helper, target = $(event.target);
                    sour.css(_pos['rightbottom']);
                    self.opts.stop && self.opts.stop('rightbottom', event, ui);
                }
            });
        }
    }
    $.fn.insertResizeBox = function (opts, _config) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _box = new ResizeBox(this, opts);
                $this.data().ResizeBox = _box;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().ResizeBox) {
                    // console.log(_config);
                    if (!$this.data().ResizeBox[opts]) throw new Error('没有这个方法');
                    res = $this.data().ResizeBox[opts](_config);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

; (function ($, root, un) {
    var defaults = {
        target: null,
        confirm: null,
        styles: ''
    }

    function itemData(attrname, ext) {
        this.attrname = attrname;
        this.ext = ext;
    }
    function getItemHtml(data) {
        if (data.ext.attrtype == 'lookup' || data.ext.attrtype == 'customer' || data.ext.attrtype == 'owner' || data.ext.attrtype == 'primarykey') {
            return '';
        } else if (data.ext.attrname == "statecode" || data.ext.attrname.toLowerCase() == "statecode") {
            return '';
        }
        var html = [];
        html.push('<label class="col-sm-4 text-right" for="' + data.ext.attrname + '">' + data.ext.attrlabel + '</label>');
        html.push('<div class="col-sm-8">');
        var isrelated = data.ext.attrname.indexOf(".") > 0;
        var k = isrelated ? data.ext.attrname.split('.')[1] : data.ext.attrname.toLowerCase();
        var ctrlId = isrelated ? data.ext.attrname.replace(".", "_") : data.ext.attrname;
        if (data.ext.attrtype == 'picklist' || data.ext.attrtype == 'bit' || data.ext.attrtype == 'status' || data.ext.attrtype == 'state') {
            html.push('<input type="text" id="' + ctrlId + '" class="form-control colinput input-sm picklist" data-type="' + data.ext.attrtype + '" data-name="' + data.ext.attrname + '" name="' + data.ext.attrname + '" data-items=\'' + data.ext.attropts + '\' />');
        } else if (data.ext.attrtype == 'datetime') {
            html.push('<div class="form-group"><input type="text" style="width:88px; float:none;margin-left: 15px;" id="' + ctrlId + '" class="form-control colinput input-sm datepicker" data-type="' + data.ext.attrtype + '" autocomplete="off" name="' + data.ext.attrname + '" /><span style="width:10px;">-</span><input type="text" style="width:87px; float:none;" autocomplete="off" class="form-control colinput input-sm datepicker" name="' + data.ext.attrname + '" data-type="' + data.ext.attrtype + '" /></div>');
        } else if (data.ext.attrtype == 'lookup' || data.ext.attrtype == 'customer' || data.ext.attrtype == 'owner' || data.ext.attrtype == 'primarykey') {
            //html.push('<div class="input-group input-group-sm"><input type="text" id="' + ctrlId + '" data-type="lookup" data-entityid="' + data.ext.attrentityid + '" data-referencedentityid="' + data.ext.attrreferencedentityid + '" name="' + data.ext.attrname + '" class="form-control colinput lookup searchLookup" /><span class="input-group-btn"><button type="button" name="clearBtn" class="btn btn-default ctrl-del" title="find" style="border-radius:0;"><span class="glyphicon glyphicon-remove-sign"></span></button><button type="button" name="lookupBtn" class="btn btn-default ctrl-search" title="find" style="border-top-left-radius: 0;border-bottom-left-radius: 0;"><span class="glyphicon glyphicon-search"></span></button></span></div>');
        } else if (data.ext.attrtype == 'int' || data.ext.attrtype == 'float' || data.ext.attrtype == 'decimal' || data.ext.attrtype == 'money') {
            html.push('<div class="form-group"><input type="text" style="width:80px; float:none;margin-left: 15px;" id="' + ctrlId + '" class="form-control colinput input-sm" data-type="' + data.ext.attrtype + '" style="width:90px;" name="' + data.ext.attrname + '" value="" /><span style="width:10px;">-</span><input type="text" style="width:80px; float:none;" class="form-control colinput input-sm" name="' + data.ext.attrname + '" value="" data-type="' + data.ext.attrtype + '" /></div>')
        } else {
            html.push('<input type="text" id="' + ctrlId + '" class="form-control colinput input-sm" name="' + data.ext.attrname + '" data-type="' + data.ext.attrname + '" value="" />');
        }
        html.push('</div>');
        return html.join('');
    }

    function tableFiler(obj, opts) {
        this.$obj = $(obj);
        this.opts = opts;
        this.$table = $(this.opts.table);
        this.filterDatas = [];
        this.$theader = this.$table.find('thead th:gt(1)');
        if (this.opts.styles != "") {
            this.$obj.addClass(this.opts.styles);
        }
        this.init();
    }
    tableFiler.prototype.getTableHeaderData = function () {
        var self = this;
        this.$theader.each(function (key, item) {
            var $item = $(item);
            var attrname = $item.attr('data-name');
            var attrtype = $item.attr('data-type');
            var attropts = $item.attr('data-opts');
            var attrlabel = $item.attr('data-label');
            var attrentityid = $item.attr('data-entityid');
            var attrreferencedentityid = $item.attr('data-referencedentityid');
            var ext = {
                attrname: attrname,
                attrtype: attrtype,
                attropts: attropts,
                attrlabel: attrlabel,
                attrentityid: attrentityid,
                attrreferencedentityid: attrreferencedentityid
            }
            var _data = new itemData(attrname, ext);
            self.filterDatas.push(_data);
        });
    }
    tableFiler.prototype.render = function () {
        var self = this;
        var _html = [];
        _html.push('<div class="btn-group"><div class="btn btn-default btn-sm xms-formDownInput " style="width:85px;border-radius:4px;" title=""><span class="glyphicon glyphicon-filter"></span></div><span class="caret" style="position: absolute;right: 10px;top: 50%;z-index:9;"></span></div>');
        _html.push('<div class="xms-formDropDown-List container-fluid" id="searchFormSearch" style="width:336px; padding:10px;"><div style="max-height:350px;overflow-x:hidden;overflow:auto;">')
        $.each(self.filterDatas, function (key, item) {
            _html.push('<div class="row seacher-row xms-formDropDown-Item" data-name="' + item.ext.attrname + '" data-type="' + item.ext.attrtype + '">');
            _html.push(getItemHtml(item));
            _html.push('</div>');
        });
        _html.push('</div>');
        _html.push('<div class="row text-center ctrl-btns"><div class="col-sm-12"><div class="btn btn-default btn-sm " data-role="cancel">取消</div><div class="btn btn-default btn-sm" data-role="clear">清除</div><div class="btn btn-info btn-sm" id="searchFilterListBtn" data-role="confirm">确定</div><button type="reset" name="resetBtn" class="hide"></button></div></div>');
        _html.push('</div>');
        //console.log(_html);
        self.$obj.html(_html.join(''));
    }
    tableFiler.prototype.bindEvent = function () {
        var self = this;
        $(".picklist", self.$obj).each(function (i, n) {
            var $this = $(n);
            var type = $this.attr("data-name");
            try {
                var itemsstrs = $this.attr('data-items')
                var items = JSON.parse(decodeURIComponent(itemsstrs));
                $(this).picklist({
                    items: items
                });
            } catch (err) {
                //throw new Error(err);
                console.error('选项集数据有问题！' + $this.attr('data-items'));
            }
        });
        self.$obj.xmsFormDrop({
            Event: "click",
            Selecter: ".btn-group"
        });
        $(".lookup", self.$obj).siblings("span").find(".ctrl-search").click(function () {
            var $this = $(this);
            var type = $this.attr("data-name");
            var inreferencedentityid = $this.parents("span:first").siblings("input").attr("data-referencedentityid");
            var inputid = $this.parents("span:first").siblings("input").attr("id");
            var $input = $this.parents("span:first").siblings("input");
            if (!inputid) {
                inputid = "lookup_" + new Date() * 1;
                $input.attr("id", inputid);
            }
            var lookupurl = '/entity/RecordsDialog?entityid=' + inreferencedentityid + '&singlemode=false&inputid=' + inputid;
            if ($input.attr('data-customerurl') && $input.attr('data-customerurl') != '') {
                lookupurl = $input.attr('data-customerurl');
            }
            $(this).trigger("dialog.return", function (e, result) {
                //console.log(result);
                var input = $this
                var rName = [];
                var rId = [];
                for (var i = 0; i < result.length; i++) {
                    rName.push(result[i].name);
                    rId.push(result[i].id);
                }
                //console.log(rName);
                input.val(rName.join(','));
                input.attr('data-value', rId.join(','));
            });
            Xms.Web.OpenDialog(lookupurl, null, null, function () {
                var _value = $input.val();
                var $dialogInput = $('#entityRecordsModal').find('#Q');
                var $dialogSearch = $('#entityRecordsModal').find('button[name="searchBtn"]');
                //console.log('lookup_value', _value);
                if (_value != '') {
                    var data_value = $input.attr('data-value');
                    //console.log('data-value', data_value);
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
            //$(".xms-formDropDown").trigger("xmsFormDrop.show");
        });

        $(".lookup").siblings("span").find(".ctrl-del").click(function () {
            var $this = $(this);
            var input = $this.parents("span:first").siblings("input");
            var rowPar = $this.parents('.input-group:first');
            input.attr("data-value", "").val("").css('color', '#555');
            if ($("a.xms-dropdownLink", rowPar).length > 0) {
                $("a.xms-dropdownLink", rowPar).remove();
            }
            console.log($(".xms-dropdownSearch-box"))
            $(".xms-dropdownSearch-box").removeClass("open");
            page_common_formSearcher && page_common_formSearcher.SearchTip(input);
        });
        if ($.fn.datepicker) {
            $('.datepicker', self.$obj).datepicker({
                autoclose: true
                , clearBtn: true
                , format: "yyyy-mm-dd"
                , language: "zh-CN"
            });
        }
        $('.ctrl-btns .btn[data-role="cancel"]', self.$obj).on('click', function () {
            var $this = $(this);
            $(self.$obj).trigger("xmsFormDrop.close");
        });
        $('.ctrl-btns .btn[data-role="confirm"]', self.$obj).on('click', function () {
            var $this = $(this);
            var searchFormSearch = self.$obj;
            var _filters = [];
            searchFormSearch.find('.seacher-row').each(function (key) {
                var filter = new Xms.Fetch.FilterExpression();
                filter.FilterOperator = Xms.Fetch.LogicalOperator.And;
                var filtername = $(this).attr('data-name');
                var value = $(this).find('input.form-control').val();
                var filtetype = $(this).attr('data-type');
                var obj = null;

                //添加过滤条件
                if (filtetype == 'picklist' || filtetype == 'state' || filtetype == 'bit') {
                    inList = $(this).find('.colinput');
                    var condition = new Xms.Fetch.ConditionExpression();
                    condition.AttributeName = filtername;
                    condition.Operator = Xms.Fetch.ConditionOperator.Equal;
                    inList.each(function (i, n) {
                        var keywork = $(n).attr("data-value") || "";
                        condition.Values.push(keywork);
                    });
                    filter.Conditions.push(condition);
                } else if (filtetype == 'int' || filtetype == 'decimal' || filtetype == 'float' || filtetype == 'money') {
                    inList = $(this).find('input.colinput');
                    inList.each(function (i, n) {
                        if (keywork != '') {
                            var condition = new Xms.Fetch.ConditionExpression();
                            condition.AttributeName = filtername;
                            if (i == 0) {
                                condition.Operator = Xms.Fetch.ConditionOperator.GreaterEqual;
                            } else {
                                condition.Operator = Xms.Fetch.ConditionOperator.LessEqual;
                            }
                            var keywork = $(n).val();
                            condition.Values.push(keywork);
                            // console.log(condition)
                            filter.Conditions.push(condition);
                        }
                    });
                }
                else if (filtetype == 'datetime') {
                    inList = $(this).find('input.colinput');
                    inList.each(function (i, n) {
                        if (keywork && keywork != '') {
                            var condition = new Xms.Fetch.ConditionExpression();
                            condition.AttributeName = filtername;
                            var keywork = $(n).val();
                            if (i == 0) {
                                condition.Operator = Xms.Fetch.ConditionOperator.GreaterEqual;
                            } else {
                                condition.Operator = Xms.Fetch.ConditionOperator.LessEqual;
                                //if (!isfilterForm) {
                                //      keywork = new Date(keywork).DateAdd('d', 1).format('yyyy-MM-dd');
                                //  } else {
                                keywork = new Date(keywork).format('yyyy-MM-dd');
                                keywork += ' 23:59:59';
                                // }
                            }
                            condition.Values.push(keywork);
                            // console.log(condition)
                            filter.Conditions.push(condition);
                        }
                    });
                }
                else {
                    inList = $(this).find('.colinput');

                    //console.log('filtetype.' + filtername, filtetype);
                    if (filtetype == 'lookup' || filtetype == 'owner' || filtetype == 'customer' || filtetype == 'primarykey') {
                        inList.each(function (i, n) {
                            var condition = new Xms.Fetch.ConditionExpression();
                            condition.AttributeName = filtername;
                            // console.log($(n).attr('data-value') == "" && $(n).val() != "");
                            if ((!$(n).attr('data-value') || $(n).attr('data-value') == "") && $(n).val() != "") {
                                condition.Operator = Xms.Fetch.ConditionOperator.Like;
                                condition.AttributeName = filtername + 'name';
                                var keywork = $(n).val();
                            } else {
                                var keywork = $(n).attr('data-value') || "";
                            }

                            if (~keywork.indexOf(",")) {
                                var tempkeyword = keywork.split(',');
                                //console.log('condition.' + filtername, tempkeyword);
                                $.each(tempkeyword, function (key, item) {
                                    var _itemCondition = $.extend(true, {}, condition);
                                    _itemCondition.Values.push(item);
                                    filter.Conditions.push(_itemCondition);
                                    // console.log('condition.' + filtername, _itemCondition);
                                });
                                filter.FilterOperator = 1;
                            } else {
                                condition.Values.push(keywork);
                                filter.Conditions.push(condition);
                            }
                        });
                    } else {
                        var condition = new Xms.Fetch.ConditionExpression();
                        condition.AttributeName = filtername;
                        if (filtetype == 'nvarchar') {
                            condition.Operator = Xms.Fetch.ConditionOperator.Like;
                        } else {
                            condition.Operator = Xms.Fetch.ConditionOperator.Equal;
                        }
                        inList.each(function (i, n) {
                            var keywork = $(n).val();
                            condition.Values.push(keywork);
                        });

                        filter.Conditions.push(condition);
                    }
                }
                _filters.push(filter);
            });
            //console.log(_filters);
            self.opts.confirm && self.opts.confirm(self, _filters);
            $(self.$obj).trigger("xmsFormDrop.close");
        });
        $('.ctrl-btns .btn[data-role="clear"]', self.$obj).on('click', function () {
            var $this = $(this);
            var filters = new Xms.Fetch.FilterExpression();
            self.opts.clear && self.opts.clear(self, filters);
            $(self.$obj).trigger("xmsFormDrop.close");
        });
    }
    tableFiler.prototype.init = function () {
        this.getTableHeaderData();
        this.render();
        this.bindEvent();
    }
    $.fn.tableFiler = function (opts, _config, _more) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _box = new tableFiler(this, opts);
                $this.data().tableFiler = _box;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().tableFiler) {
                    if (!$this.data().tableFiler[opts]) throw new Error('没有这个方法');
                    res = $this.data().tableFiler[opts](_config, _more);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

; (function ($, root, un) {
    var defaults = {
        className: 'event-bind',
        context: 'body'
    };
    var eventlist = ['click'];
    function pageBindEvent(opts) {
        if (!(this instanceof pageBindEvent)) {
            return new pageBindEvent(opts);
        }
        this.opts = $.extend({}, opts, defaults);
        this.init();
    }
    pageBindEvent.prototype.init = function () {
        this.bindEvent();
    }
    pageBindEvent.prototype.bindEvent = function () {
        $(this.opts.context).on('click.pageBindEvnet', '.' + this.opts.className, function (e) {
            var $this = $(this), _eventname = $this.attr('data-eventname'), _datas = $this.attr('data-eventdatas');
            if (_eventname) {
                try {
                    var _events = _eventname.split('.');
                    var _len = _events;
                    if (_len.length == 2) {
                        var _obj = _events[0], _evt = _events[1];
                        if (window[_obj] && typeof window[_obj][_evt] == 'function') {
                            window[_obj][_evt].call(this, e, _datas);
                        } else {
                            console.log('/------------------------/');
                            console.log('pageBindEvent: 没有找到对象' + _obj);
                            console.log('/------------------------/');
                        }
                    }
                } catch (e) {
                    console.error(e);
                }
            }
        });
    }
    pageBindEvent.prototype.unbindEvent = function () {
    }

    window.pageCustomBindEvent = pageBindEvent;
})(jQuery, window);

//需要在所有页面直接执行的方法
$(function () {
    //$(document).bind("ajaxQueueEmpty", function () {
    //_setIframeHeight();
    //});

    /*初始化页面事件绑定方法
     * 可通过在DOM元素中添加 event-bind样式名
     根据data-eventname的值执行对应的方法  data-eventname="page_common_formSearcher.clearFilters"
     根据data-eventdatas的值可以给方法添加在DOM中需要传递给该方法的数据，可为空
     */
    pageCustomBindEvent();
});

//序列化表单数据为JSON对象
(function () {
    var rcheckableType = (/^(?:checkbox|radio)$/i);
    var
        rbracket = /\[\]$/,
        rCRLF = /\r?\n/g,
        rsubmitterTypes = /^(?:submit|button|image|reset|file)$/i,
        rsubmittable = /^(?:input|select|textarea|keygen)/i;

    jQuery.fn.extend({
        serializeFormJSON: function (ishiddendata) {//ishiddendata可用于在返回的数据中 表单元素含有data-hiddendata这个属性 ，然后增加一个对象来存放数据,例如表单页面中保存的数据
            var o = {};
            var a = this.customSerializeArray();
            if (!ishiddendata) {
                $.each(a, function () {
                    //如果为复选框或者下拉，可多选的FORM元素
                    if (~this.name.indexOf('[')) {//如果NAME是以数组来命名的话，需要用正则进行处理
                        if (~this.name.indexOf(']\.')) {
                            var arrNameTest = /^([\w\.\_\-]+)(\[([0-9]+)\])\.(\w+)/g;
                            var nameArr = arrNameTest.exec(this.name);
                            if (nameArr && nameArr.length > 1) {
                                var name = nameArr[1];
                                var index = nameArr[3];
                                var __key = nameArr[4]
                                if (__key) {
                                    if (o[name]) {
                                        if (!o[name].push) {
                                            o[name] = [o[name]];
                                        }
                                        if (!o[name][index]) {
                                            o[name][index] = {};
                                        }
                                        o[name][index][__key] = this.value || '';
                                    } else {
                                        o[name] = [];
                                        if (!o[name][index]) {
                                            o[name][index] = {};
                                        }
                                        o[name][index][__key] = (this.value || '');
                                    }
                                }
                            }
                        } else {
                            var arrNameTest = /^([\w\.\_\-]+)(\[([0-9]+)\])$/g;
                            var nameArr = arrNameTest.exec(this.name);
                            if (nameArr && nameArr.length > 1) {
                                var name = nameArr[1];
                                var index = nameArr[3];
                                if (o[name]) {
                                    if (!o[name].push) {
                                        o[name] = [o[name]];
                                    }
                                    o[name][index] = this.value || '';
                                } else {
                                    o[name] = [];
                                    o[name][index] = (this.value || '');
                                }
                            }
                        }
                    } else {//如果为普通命名方式，则直接保存在对应的数组里
                        if (~this.type.indexOf('select')) {//如果为下拉框
                            if (~this.type.indexOf('multiple')) {//如果为多选下拉框
                                if (o[this.name]) {
                                    if (!o[this.name].push) {
                                        o[this.name] = [o[this.name]];
                                    }
                                    o[this.name].push(this.value || '');
                                } else {
                                    o[this.name] = [];
                                    o[this.name].push(this.value || '');
                                }
                            } else {
                                if (o[this.name]) {
                                    if (!o[this.name].push) {
                                        o[this.name] = [o[this.name]];
                                    }
                                    o[this.name].push(this.value || '');
                                } else {
                                    o[this.name] = this.value || '';
                                }
                            }
                        } else if (this.type == 'checkbox') {
                            if (o[this.name]) {
                                if (!o[this.name].push) {
                                    o[this.name] = [o[this.name]];
                                }
                                o[this.name].push(this.value || '');
                            } else {
                                o[this.name] = [];
                                o[this.name].push(this.value || '');
                            }
                        } else {//如果是普通元素，单选或者输入框等
                            if (o[this.name]) {
                                if (!o[this.name].push) {
                                    o[this.name] = [o[this.name]];
                                }
                                o[this.name].push(this.value || '');
                            } else {
                                o[this.name] = this.value || '';
                            }
                        }
                    }
                });
                return o;
            } else {
                var otemp = o[ishiddendata] = {};
                $.each(a, function () {
                    var o_data = otemp;
                    if (this.ishiddendata) {//如果不需要放到新的data 里头的数据
                        o_data = o;
                    }
                    //如果为复选框或者下拉，可多选的FORM元素
                    if (~this.name.indexOf('[')) {//如果NAME是以数组来命名的话，需要用正则进行处理
                        if (~this.name.indexOf(']\.')) {
                            var arrNameTest = /^([\w\.\_\-]+)(\[([0-9]+)\])/g;
                            var nameArr = arrNameTest.exec(this.name);
                            if (nameArr && nameArr.length > 1) {
                                var name = nameArr[1];
                                var index = nameArr[3];
                                if (o_data[name]) {
                                    if (!o_data[name].push) {
                                        o_data[name] = [o_data[name]];
                                    }
                                    o_data[name][index] = this.value || '';
                                } else {
                                    o_data[name] = [];
                                    o_data[name][index] = (this.value || '');
                                }
                            }
                        } else {
                            var arrNameTest = /^([\w\.\_\-]+)(\[([0-9]+)\])$/g;
                            var nameArr = arrNameTest.exec(this.name);
                            if (nameArr && nameArr.length > 1) {
                                var name = nameArr[1];
                                var index = nameArr[3];
                                if (o_data[name]) {
                                    if (!o_data[name].push) {
                                        o_data[name] = [o_data[name]];
                                    }
                                    o_data[name][index] = this.value || '';
                                } else {
                                    o_data[name] = [];
                                    o_data[name][index] = (this.value || '');
                                }
                            }
                        }
                    } else {//如果为普通命名方式，则直接保存在对应的数组里
                        if (~this.type.indexOf('select')) {//如果为下拉框
                            if (~this.type.indexOf('multiple')) {//如果为多选下拉框
                                if (o_data[this.name]) {
                                    if (!o_data[this.name].push) {
                                        o_data[this.name] = [o_data[this.name]];
                                    }
                                    o_data[this.name].push(this.value || '');
                                } else {
                                    o_data[this.name] = [];
                                    o_data[this.name].push(this.value || '');
                                }
                            } else {
                                if (o_data[this.name]) {
                                    if (!o_data[this.name].push) {
                                        o_data[this.name] = [o_data[this.name]];
                                    }
                                    o_data[this.name].push(this.value || '');
                                } else {
                                    o_data[this.name] = this.value || '';
                                }
                            }
                        } else if (this.type == 'checkbox') {
                            if (o_data[this.name]) {
                                if (!o_data[this.name].push) {
                                    o_data[this.name] = [o_data[this.name]];
                                }
                                o_data[this.name].push(this.value || '');
                            } else {
                                o_data[this.name] = [];
                                o_data[this.name].push(this.value || '');
                            }
                        } else {//如果是普通元素，单选或者输入框等
                            if (o_data[this.name]) {
                                if (!o_data[this.name].push) {
                                    o_data[this.name] = [o_data[this.name]];
                                }
                                o_data[this.name].push(this.value || '');
                            } else {
                                o_data[this.name] = this.value || '';
                            }
                        }
                    }
                });
                o[ishiddendata] = JSON.stringify(otemp);
                return o;
            }
        },
        customSerializeArray: function () {
            return this.map(function () {
                // Can add propHook for "elements" to filter or add form elements
                var elements = jQuery.prop(this, "elements");
                return elements ? jQuery.makeArray(elements) : this;
            })
                .filter(function () {
                    var type = this.type;

                    // Use .is( ":disabled" ) so that fieldset[disabled] works
                    return this.name && !jQuery(this).is(":disabled") &&
                        rsubmittable.test(this.nodeName) && !rsubmitterTypes.test(type) &&
                        (this.checked || !rcheckableType.test(type));
                })
                .map(function (i, elem) {
                    var val = jQuery(this).val();
                    var multiple = jQuery(this).attr('multiple') ? true : false;
                    var ishiddendata = jQuery(this).attr('data-hiddendata') ? true : false;
                    if (val == null) {
                        return null;
                    }

                    if (Array.isArray(val)) {
                        return jQuery.map(val, function (val) {
                            return { name: elem.name, value: val.replace(rCRLF, "\r\n"), type: elem.type, multiple: multiple, ishiddendata: ishiddendata };
                        });
                    }

                    return { name: elem.name, value: val.replace(rCRLF, "\r\n"), type: elem.type, multiple: multiple, ishiddendata: ishiddendata };
                }).get();
        }
    });
})();

; (function ($, root, un) {
    var defaults = {
        columnConfigs: [],
        getDataUrl: function () { return ''; }
    }

    function xmsDatagrid(obj, opts) {
        this.$obj = $(obj);
        this.opts = opts;

        this.init();
    }

    xmsDatagrid.prototype.init = function () {
        var self = this;
        var datagridconfig = {
            //获取数据的方法
            getDataUrl: self.opts.getDataUrl,
            headerFilter: true,
            pageModel: { type: "remote", rPP: 10, page: 1, strRpp: "{0}" },
            //itemsBtnTmpl: itemtmpl,
            filterColModel: function (opts) { opts.colModel = self.opts.columnConfigs; return opts; },//配置列类型的过滤方法
            rowDblClick: function (event, ui) {
                var $tr = ui.$tr;
                $tr.find('input[name="recordid"]').prop('checked', true);
            },
            columnFilter: function (items) {//如果是从后台获取的数据，可在此处给列添加更多的选项
                return items;
            }
        }
        datagridconfig.extend = function (datagrid) {
            //因为需要修改为GET获取数据，要重新配置列排序信息
            var filter = function (postData, objP) {
                $.extend(postData, { sortby: objP.dataIndx, sortdirection: objP.dir == 'up' ? '0' : '1' });
                return postData;
            }

            //修改为GET方式获取数据，配置分页信息
            $.extend(datagrid.opts.dataModel, {
                filterSendData: filter,
                method: 'GET',
                // dataType: 'json',
                getData: function (dataJSON, textStatus, jqXHR) {
                    if (typeof dataJSON.Content == 'string') {
                        dataContent = JSON.parse(dataJSON.Content);
                    }
                    var data = dataContent.items;
                    var res = { curPage: dataContent.currentpage || 1, totalRecords: dataContent.totalitems, data: data }
                    console.log(res);
                    return res;
                }
            })
        }
        //  console.log(itemtmpl);
        this.$obj.cDatagrid(datagridconfig);
    }
    $.fn.xmsDatagrid = function (opts, _config, _more) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _box = new xmsDatagrid(this, opts);
                $this.data().xmsDatagrid = _box;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().xmsDatagrid) {
                    if (!$this.data().xmsDatagrid[opts]) throw new Error('没有这个方法');
                    res = $this.data().xmsDatagrid[opts](_config, _more);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

(function ($, root, un) {
    var defaults = {
    }

    function xmsPosDropdown(obj, opts) {
        this.box = $(obj);
        this.opts = opts;
        this.list = [];
        this.tempDropdown = null;
        this.isloaded = false;
        this.init();
    }

    xmsPosDropdown.prototype.init = function () {
        this.bindEvent();
    }
    xmsPosDropdown.prototype.isloaded = function () {
        return this.isloaded;
    }
    xmsPosDropdown.prototype.destory = function () {
        this.$context.find('.datatable-filter-wrap').find('.dropdown-toggle').off('click');
        $(document).off('click.custom.toggle');
        if (this.tempDropdown) {
            this.tempDropdown.remove();
        }
    }
    xmsPosDropdown.prototype.bindEvent = function () {
        var self = this;
        this.box.find('.dropdown-toggle').off('click').on('click', function () {
            var _sibling = $(this).siblings('.dropdown-menu');
            var isShow = _sibling.is(':hidden');
            _sibling.css('opacity', 0);
            if (self.tempDropdown) { self.tempDropdown.remove(); }
            if (isShow) {
                var _sibling_clone = _sibling.clone(true);
                var clone_offset = $(this).offset();
                var clone_size = { w: _sibling.width(), h: _sibling.height() };
                clone_offset.left = clone_offset.left - 150;
                clone_offset.top = clone_offset.top + 20;
                _sibling_clone.css(clone_offset).css({ 'position': 'absolute', 'opacity': 1 });
                $('body').append(_sibling_clone);
                _sibling_clone.show();
                self.tempDropdown = _sibling_clone;
            } else {
                if (self.tempDropdown) {
                    self.tempDropdown.remove();
                }
            }
            $(document).on('click.custom.toggle', function (e) {
                var target = $(e.target || e.srcElement);
                if (target.closest(_sibling.parent()).length == 0) {
                    if (self.tempDropdown) {
                        self.tempDropdown.remove();
                    }
                }
            })
        })

        this.isloaded = true;
    }

    $.fn.xmsPosDropdown = function (opts, _config, _more) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _box = new xmsPosDropdown(this, opts);
                $this.data().xmsPosDropdown = _box;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().xmsPosDropdown) {
                    if (!$this.data().xmsPosDropdown[opts]) throw new Error('没有这个方法');
                    res = $this.data().xmsPosDropdown[opts](_config, _more);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

(function ($, root, un) {
    var defaults = {
        isMaxHeight: true,
        getDataUrl: function () {
            return '';
        },
        headerFilter: false,
        pageModel: { type: "remote", rPP: 10, page: 1, strRpp: "{0}" },
        //itemsBtnTmpl: itemtmpl,
        rowDblClick: function (event, ui) {
            var rowdata = ui.rowData;
            if ($(ui.$tr).find('a.btn>.glyphicon-edit').length > 0) {
                $(ui.$tr).find('a.btn>.glyphicon-edit').trigger('click');
            }
        },
        columnConfigs: {},
        baseUrl: '',
        columnFilter: function (items) {//如果是从后台获取的数据，可在此处给列添加更多的选项
            return items;
        },

        gridrefresh: function ($grid, grid, dom) {//刷新时设置下拉菜单为绝对定位，防止被遮盖
            $grid.find('.datatable-filter-wrap').xmsPosDropdown();
        },
        context: null
    }

    function xmsDataTable(obj, opts) {
        this.box = $(obj);
        this.opts = opts;
        this.$grid = null;

        this.setOpts();
        this.init();
    }
    xmsDataTable.prototype.setOpts = function (opts) {
        var self = this;
        var querycreatedon = $.queryBykeyValue(self.opts.columnConfigs, 'dataIndx', 'createdon');
        var extobj = {};
        if (querycreatedon.length > 0) {
            extobj.sortIndx = 'createdon';
            extobj.sortDir = 'down';
        } else {
            var queryname = $.queryBykeyValue(self.opts.columnConfigs, 'dataIndx', 'name');
            if (queryname.length > 0) {
                var extobj = {};
                extobj.sortIndx = 'name';
                extobj.sortDir = 'down';
            } else if (self.opts.columnConfigs.length > 2) {
                var extobj = {};
                extobj.sortIndx = self.opts.columnConfigs[2].dataIndx;
                extobj.sortDir = 'down';
            }
        }
        if (this.opts.searchForm) {
            this.opts.getDataUrl = function () {
                var formdata = self.opts.searchForm.serialize();
                var url = self.opts.baseUrl
                if (!~url.indexOf('?')) {
                    url = url + '?';
                }
                //if (extobj.sortIndx) {
                // url += '&sortby=' + extobj.sortIndx + '&sortdirection=' + (extobj.sortDir == 'down' ? 0 : 1);
                // }
                return url + formdata;
            }
        }
        this.opts.extend = function (datagrid) {//扩展基础信息
            //因为需要修改为GET获取数据，要重新配置列排序信息
            var filter = function (postData, objP, DM, PM, FM) {
                var sortindx = objP.dataIndx || 'createdon' || 'name' || objP.dataIndx;
                var post = { sortby: sortindx, sortdirection: objP.dir == 'up' ? '0' : '1', pagesize: PM.rPP };
                if (self.opts.filters) {
                    post.filter = self.opts.filters.getFilterInfo()
                }
                $.extend(postData, post);
                return postData;
            }

            self.opts.method = self.opts.method || 'GET'
            //修改为GET方式获取数据，配置分页信息
            $.extend(datagrid.opts.dataModel, {
                isJsonAjax: true,
                filterSendData: filter,
                method: self.opts.method,

                error: function () {
                    console.log(1111)
                    setTimeout(function () {
                        self.box.cDatagrid("refresh");
                    }, 150);
                },
                // dataType: 'json',
                getData: function (dataJSON, textStatus, jqXHR) {
                    if (typeof dataJSON.Content == 'string') {
                        dataContent = JSON.parse(dataJSON.Content);
                    }
                    self.opts.datasFilter && self.opts.datasFilter(dataContent);
                    console.log(dataJSON);
                    var data = dataContent.items;
                    var res = { curPage: dataContent.currentpage || dataContent.page || 1, totalRecords: dataContent.totalitems, data: data }
                    console.log(res);
                    return res;
                }
            });
            var querycreatedon = $.queryBykeyValue(self.opts.columnConfigs, 'dataIndx', 'createdon');
            if (querycreatedon.length > 0) {
                var extobj = {};
                extobj.sortIndx = 'createdon';
                extobj.sortDir = 'down';
                $.extend(datagrid.opts.dataModel, extobj)
            } else {
                var queryname = $.queryBykeyValue(self.opts.columnConfigs, 'dataIndx', 'name');
                if (queryname.length > 0) {
                    var extobj = {};
                    extobj.sortIndx = 'name';
                    extobj.sortDir = 'down';
                    $.extend(datagrid.opts.dataModel, extobj)
                } else if (self.opts.columnConfigs.length > 2) {
                    var extobj = {};
                    extobj.sortIndx = self.opts.columnConfigs[2].dataIndx;
                    extobj.sortDir = 'down';
                    $.extend(datagrid.opts.dataModel, extobj)
                }
            }
        }
    }
    xmsDataTable.prototype.init = function () {
        var self = this;
        this.opts.colModel = self.opts.columnConfigs;
        //this.opts.filterColModel = function (opts) {
        //    opts.colModel = self.opts.columnConfigs;

        //    return opts;
        //}
        //配置列类型的过滤方法
        this.opts.offsetHeight = this.opts.offsetHeight || 0;
        if (!this.opts.height && this.opts.isMaxHeight) {
            var height = 300, fixHeight = 135;
            if (parent && parent.window) {
                var pHeight = parent.window.innerHeight - fixHeight;
                var boxTop = this.box.offset().top;
                this.opts.height = pHeight - boxTop + this.opts.offsetHeight;
            } else {
                var pHeight = $(window).height();
                var boxTop = this.box.offset().top;
                this.opts.height = pHeight - boxTop + this.opts.offsetHeight;
            }
        }

        this.opts.OptsHandler && this.opts.OptsHandler(this.opts);
        this.$grid = this.box.cDatagrid(this.opts);
        this.bindEvent();
    }
    xmsDataTable.prototype.isloaded = function () {
    }
    xmsDataTable.prototype.destory = function () {
    }
    xmsDataTable.prototype.getReload = function () {
    }
    xmsDataTable.prototype.postReload = function () {
    }

    xmsDataTable.prototype.bindEvent = function () {
        var self = this;

        if (this.opts.filters && this.opts.searchForm && this.opts.searchForm.length > 0) {
            this.opts.searchForm.submit(function (e) {
                e.preventDefault && e.preventDefault();
                self.box.cDatagrid('refreshDataAndView');
                return false;
            });
        }

        if (this.opts.context) {
            var target = this.opts.context;
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
                                if (typeof refreshMethod !== "undefined") {
                                    eval(refreshMethod);
                                }
                                self.box.cDatagrid('refreshDataAndView');
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
                var actiondata = $(this).attr('data-actiondata');
                if (actiondata && actiondata.length > 0) {
                    try {
                        actiondata = JSON.parse(actiondata);
                    } catch (err) {
                        actiondata = {};
                        console.error('actiondata 数据错误');
                    }
                } else {
                    actiondata = {};
                }
                var datas = Xms.Web.GetTableSelected(target);
                if (datas.length == 0) {
                    Xms.Web.Toast(LOC_NOTSPECIFIED_RECORD, false);
                } else if (action == "") {
                    Xms.Web.Toast(LOC_URL_EMPTY, false);
                }
                else {
                    var model = { recordid: datas };
                    //var urlaction = $.objToUrl(actiondata);
                    //if (urlaction) {
                    //    if (action.indexOf('?') == -1) {
                    //        action += '?' + urlaction.replace(/^&/,'');
                    //    } else {
                    //        action += urlaction;
                    //    }
                    //}
                    // $.extend(model, actiondata);
                    var urlObj = $.urlParamObj(action);
                    $.extend(model, urlObj);
                    var url = action.substring(0, action.indexOf('?'));
                    Xms.Web.Post(url, model, false, function (response) {
                        if (typeof refreshMethod !== "undefined") {
                            eval(refreshMethod);
                        }
                        Xms.Web.Toast(response.Content, true);
                        self.box.cDatagrid('refreshDataAndView');
                    }, null, false, false, { jsonajax: true });
                }
            });
        }
    }

    $.fn.xmsDataTable = function (opts, _config, _more) {
        if (typeof PAGEDEFAULT_PAGESIZE != 'undefined') {
            defaults.pageModel.rPP = PAGEDEFAULT_PAGESIZE * 1 ? (PAGEDEFAULT_PAGESIZE * 1) : 10;
        }
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _box = new xmsDataTable(this, opts);
                $this.data().xmsDataTable = _box;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().xmsDataTable) {
                    if (!$this.data().xmsDataTable[opts]) throw new Error('没有这个方法');
                    res = $this.data().xmsDataTable[opts](_config, _more);
                    return false;
                }
            });
            return res;
        }
    }

    function xmsAjaxSearchTable(obj, opts) {
        this.box = $(obj);
        this.opts = opts;
        this.init();
    }
    xmsAjaxSearchTable.prototype.init = function () {
    }

    $.fn.xmsAjaxSearchTable = function (opts, _config, _more) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _box = new xmsAjaxSearchTable(this, opts);
                $this.data().xmsAjaxSearchTable = _box;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().xmsAjaxSearchTable) {
                    if (!$this.data().xmsAjaxSearchTable[opts]) throw new Error('没有这个方法');
                    res = $this.data().xmsAjaxSearchTable[opts](_config, _more);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

(function () {
    var defaults = {
        leftContext: '',
        rightContext: '',
        sourceDatas: null,
        key: 'name',
        label: 'localizedname',
        leftCtrl: '',
        rightCtr: '',
        upCtrl: '',
        downCtrl: '',
        itemClass: 'xmsmutil-selector-item'
    }
    defaults.tmpl = '<div class="' + defaults.itemClass + '" data-name="{{key}}">{{label}}</div>'
    function xmsMutilSelector(obj, opts) {
        this.box = $(obj);
        this.opts = opts;
        this.leftContext = this.opts.leftContext;
        this.rightContext = this.opts.rightContext;
        this.sourceDatas = this.opts.sourceDatas;
        this.tmpl = this.opts.tmpl;
        this.tmpl = this.tmpl.replace(/{key}/g, this.opts.key);
        this.tmpl = this.tmpl.replace(/{label}/g, this.opts.label);
        this.currentAttrbute = null;
        this.targetDatas = [];
        this.init();
    }
    xmsMutilSelector.prototype.init = function () {
        this.createList();
        this.bindEvent();
    }
    xmsMutilSelector.prototype.refresh = function () {
        this.sourceDatas = this.opts.sourceDatas;
        this.targetDatas = [];
        this.createList();
    }
    xmsMutilSelector.prototype.xxxxx = function (datas) {
    }
    xmsMutilSelector.prototype._getListHtml = function (datas) {
        var htmls = [];
        var self = this;
        var tmpl = this.tmpl;
        $.each(datas, function (i, n) {
            var keyReg = new RegExp('\{' + self.opts.key + '\}', 'g')
            var _tmpl = tmpl.replace(keyReg, n[self.opts.key]);
            var labelReg = new RegExp('\{' + self.opts.label + '\}', 'g')
            _tmpl = _tmpl.replace(labelReg, n[self.opts.label]);
            htmls.push(_tmpl)
        });
        return htmls.join('');
    }
    xmsMutilSelector.prototype.createList = function (datas) {
        this.leftContext.html('').html(this._getListHtml(this.targetDatas));
        this.rightContext.html('').html(this._getListHtml(this.sourceDatas));
    }
    xmsMutilSelector.prototype.getSourceDatas = function (datas) {
        return this.sourceDatas;
    }

    xmsMutilSelector.prototype.getTargetDatas = function (datas) {
        return this.targetDatas;
    }
    xmsMutilSelector.prototype.destroy = function (datas) {
        this.leftContext.off();
        this.rightContext.off();
        this.leftContext.html('')
        this.rightContext.html('')
    }
    xmsMutilSelector.prototype._getDataIndex = function (datas, key, value) {
        var index = -1;
        $.each(datas, function (i, n) {
            if (n[key] == value) {
                index = i;
                return false;
            }
        });
        return index;
    }
    xmsMutilSelector.prototype.bindEvent = function (datas) {
        var self = this;
        this.leftContext.on('click', '.' + this.opts.itemClass, function (e) {
            self.currentAttrbute = $(e.target);
            self.box.find('.' + self.opts.itemClass).removeClass('active');
            self.currentAttrbute.addClass('active');
        })

        this.rightContext.on('click', '.' + this.opts.itemClass, function (e) {
            self.currentAttrbute = $(e.target);
            self.box.find('.' + self.opts.itemClass).removeClass('active');
            self.currentAttrbute.addClass('active');
        })

        this.opts.upCtrl && this.opts.upCtrl.on('click', function (e) {
            if (self.currentAttrbute && self.currentAttrbute.length > 0) {
                moveUp(self.currentAttrbute)
            }
        })

        this.opts.downCtrl && this.opts.downCtrl.on('click', function (e) {
            if (self.currentAttrbute && self.currentAttrbute.length > 0) {
                moveDown(self.currentAttrbute)
            }
        })

        function moveUp(target) {
            var $target = target;
            var name = $target.attr('data-name');
            var index = self._getDataIndex(self.sourceDatas, self.opts.key, name);
            if (~index) {
                //操作数据
                if ($target.prev().length > 0) {
                    var currentAttr = self.sourceDatas.slice(index, index + 1);
                    self.sourceDatas.splice(index, 1);
                    self.sourceDatas.splice(index - 1, 0, currentAttr[0])
                    $target.prev().insertAfter($target);
                }
            }
            console.log(self.sourceDatas);
        }

        function moveDown(target) {
            var $target = target;
            var name = $target.attr('data-name');
            var index = self._getDataIndex(self.sourceDatas, self.opts.key, name);
            if (~index) {
                //操作数据
                if ($target.next().length > 0) {
                    var currentAttr = self.sourceDatas.slice(index, index + 1);
                    self.sourceDatas.splice(index, 1);
                    self.sourceDatas.splice(index + 1, 0, currentAttr[0])
                    $target.next().insertBefore($target);
                }
            }
            console.log(self.sourceDatas);
        }

        this.opts.leftCtrl && this.opts.leftCtrl.on('click', function (e) {
            if (self.currentAttrbute && self.currentAttrbute.length > 0) {
                moveToRight(e, self.currentAttrbute)
            }
        })

        this.opts.rightCtr && this.opts.rightCtr.on('click', function (e) {
            if (self.currentAttrbute && self.currentAttrbute.length > 0) {
                moveToLeft(e, self.currentAttrbute)
            }
        })

        function moveToRight(e, target) {
            console.log(e, target);

            var $target = target || $(e.target);
            var name = $target.attr('data-name');
            var index = self._getDataIndex(self.targetDatas, self.opts.key, name);
            if (~index) {
                //操作数据
                var currentAttr = self.targetDatas.slice(index, index + 1);
                self.sourceDatas = self.sourceDatas.concat(currentAttr);
                self.targetDatas.splice(index, 1);

                //操作界面
                self.rightContext.append($target);
            }
            console.log(self.targetDatas);
            self.opts.leftDblclick && self.opts.leftDblclick.call(this, e, self);
        }

        function moveToLeft(e, target) {
            console.log(e, target);

            var $target = target || $(e.target);
            var name = $target.attr('data-name');
            var index = self._getDataIndex(self.sourceDatas, self.opts.key, name);
            if (~index) {
                //操作数据
                var currentAttr = self.sourceDatas.slice(index, index + 1);
                self.targetDatas = self.targetDatas.concat(currentAttr);
                self.sourceDatas.splice(index, 1);

                //操作界面
                self.leftContext.append($target);
            }
            console.log(self.sourceDatas)
            self.opts.rightDblclick && self.opts.rightDblclick.call(this, e, self);
        }

        this.leftContext.on('dblclick', '.' + this.opts.itemClass, moveToRight)

        this.rightContext.on('dblclick', '.' + this.opts.itemClass, moveToLeft)
    }
    $.fn.xmsMutilSelector = function (opts, _config, _more) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _box = new xmsMutilSelector(this, opts);
                $this.data().xmsMutilSelector = _box;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().xmsMutilSelector) {
                    if (!$this.data().xmsMutilSelector[opts]) throw new Error('没有这个方法');
                    res = $this.data().xmsMutilSelector[opts](_config, _more);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

(function () {
    var defaults = {
        width: 'auto',
        height: 'auto',
        context: 'body',
        src: '',
        initAfter: null
    }

    function xmsDialogContent(obj, opts) {
        this.box = $(obj);
        this.opts = opts;
        this.$wrap = $('<div class="xmsDialogContent-wrap">')
        this.$iframe = $('<iframe width="' + this.opts.height + '" src="' + this.opts.src + '"  class="xmsDialogContent-iframe" name="xmsDialogContent" frameborder="0" scrolling="auto" height="' + this.opts.height + '" style="height:100%;"></iframe>')
        this.$close = $('<div class="xmsDialogContent-close">x</div>');
        this.$wrap.append(this.$close).append(this.$iframe);
        $(this.opts.context).append(this.$wrap);
        this.init();
    }
    xmsDialogContent.prototype.init = function () {
        this.bindEvent();
        this.opts.initAfter && this.opts.initAfter(this);
    }

    xmsDialogContent.prototype.destroy = function (datas) {
        this.$wrap.remove();
        this.$close.off();
    }
    xmsDialogContent.prototype.show = function (datas) {
        $(this.opts.context).addClass('xmsDialogContent-active');
        this.$iframe.height(this.$wrap.height()).width('100%');
    }
    xmsDialogContent.prototype.hide = function (datas) {
        $(this.opts.context).removeClass('xmsDialogContent-active')
    }
    xmsDialogContent.prototype.changeUrl = function (url) {
        this.$iframe.attr('src', url);
    }
    xmsDialogContent.prototype.bindEvent = function (datas) {
        var self = this;
        this.$close.on('click', function () {
            self.hide();
        })
    }
    $.fn.xmsDialogContent = function (opts, _config, _more) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _box = new xmsDialogContent(this, opts);
                $this.data().xmsDialogContent = _box;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().xmsDialogContent) {
                    if (!$this.data().xmsDialogContent[opts]) throw new Error('没有这个方法');
                    res = $this.data().xmsDialogContent[opts](_config, _more);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

(function () {
    var defaults = {
        initAfter: null,
        isLeftRangle: false,
        callback: function () { }
    }

    var locale = {
        "format": "MM/DD/YYYY",
        "separator": " - ",
        "applyLabel": "确定",
        "cancelLabel": "取消",
        "fromLabel": "从",
        "toLabel": "至",
        "customRangeLabel": "自定义",
        "weekLabel": "周",
        "daysOfWeek": [
            "日",
            "一",
            "二",
            "三",
            "四",
            "五",
            "六"
        ],
        "monthNames": [
            "一月",
            "二月",
            "三月",
            "四月",
            "五月",
            "六月",
            "七月",
            "八月",
            "九月",
            "十月",
            "十一月",
            "十二月"
        ],
        "firstDay": 1
    };
    var configs = {
        "singleDatePicker": true,
        "showWeekNumbers": true,
        "showISOWeekNumbers": true,
        "showDropdowns": true,
        //ranges: ranges,
        "startDate": new Date().format('MM/dd/yyyy'),
        // "endDate": "08/15/2019",
        // "minDate": "4334345"
    }

    function xmsDateRangePicker(obj, opts) {
        this.box = $(obj);
        opts = $.extend({}, configs, opts);
        this.opts = opts;
        var ranges = this.opts.ranges || {
            '默认': [moment().subtract(10, 'years'), moment()],
            '今天': [moment(), moment().add(1, 'days')],
            '昨天': [moment().subtract(1, 'days'), moment()],
            '近7天': [moment().subtract(6, 'days'), moment()],
            '近1个月': [moment().subtract(1, 'months'), moment()],
            '近3个月': [moment().subtract(3, 'months'), moment()],
            '近半年': [moment().subtract(6, 'months'), moment()],
            '近一年': [moment().subtract(1, 'year'), moment()]
        }
        if (this.opts.isLeftRangle) {
            this.opts.ranges = ranges
        }
        this.opts.locale = locale;

        this.init();
    }
    xmsDateRangePicker.prototype.init = function () {
        if (!$.fn.daterangepicker) { console.error('daterangepicker not defined') }
        this.box.daterangepicker(this.opts, this.opts.callback)
        this.bindEvent();
        this.opts.initAfter && this.opts.initAfter(this);
    }

    xmsDateRangePicker.prototype.destroy = function (datas) {
    }

    xmsDateRangePicker.prototype.bindEvent = function (datas) {
        var self = this;
    }
    $.fn.xmsDateRangePicker = function (opts, _config, _more) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _box = new xmsDateRangePicker(this, opts);
                $this.data().xmsDateRangePicker = _box;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().xmsDateRangePicker) {
                    if (!$this.data().xmsDateRangePicker[opts]) throw new Error('没有这个方法');
                    res = $this.data().xmsDateRangePicker[opts](_config, _more);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

(function () {
    var defaults = {
        initAfter: null,
        starttime: null,
        endtime: null,
        isLeftRangle: true,
        callback: function () { }
    }

    var locale = {
        "format": "MM/DD/YYYY",
        "separator": " - ",
        "applyLabel": "确定",
        "cancelLabel": "取消",
        "fromLabel": "从",
        "toLabel": "至",
        "customRangeLabel": "自定义",
        "weekLabel": "周",
        "daysOfWeek": [
            "日",
            "一",
            "二",
            "三",
            "四",
            "五",
            "六"
        ],
        "monthNames": [
            "一月",
            "二月",
            "三月",
            "四月",
            "五月",
            "六月",
            "七月",
            "八月",
            "九月",
            "十月",
            "十一月",
            "十二月"
        ],
        "firstDay": 1
    };
    var configs = {
        "singleDatePicker": false,
        "showWeekNumbers": true,
        "showISOWeekNumbers": true,
        "showDropdowns": true,
        "alwaysShowCalendars": true,
        "autoUpdateInput": false,
        "opens": "right",
        "inputTmpl": ''
        // "startDate": new Date().format('MM/dd/yyyy'),
        //  "endDate": "08/15/2019",
        // "minDate": "4334345"
    }

    function xmsMutilDateRangePicker(obj, opts) {
        if (!$.fn.daterangepicker) { console.error('daterangepicker not defined') }
        this.box = $(obj);
        opts = $.extend({}, configs, opts);
        this.opts = opts;
        var ranges = this.opts.ranges || {
            '默认': [moment().subtract(10, 'years'), moment()],
            '今天': [moment(), moment().add(1, 'days')],
            '昨天': [moment().subtract(1, 'days'), moment()],
            '近7天': [moment().subtract(6, 'days'), moment()],
            '近1个月': [moment().subtract(1, 'months'), moment()],
            '近3个月': [moment().subtract(3, 'months'), moment()],
            '近半年': [moment().subtract(6, 'months'), moment()],
            '近一年': [moment().subtract(1, 'year'), moment()]
        }
        if (this.opts.isLeftRangle) {
            this.opts.ranges = ranges
        }
        this.$input = $((this.opts.inputTmpl || '<input type="text" class="form-control input-sm" value="" />'));
        this.$start = this.opts.starttime || this.box.find('input[name="starttime"]');
        this.$end = this.opts.endtime || this.box.find('input[name="endtime"]');
        this.$start.hide();
        this.$end.hide();
        this.opts.locale = locale;

        this.init();
    }
    xmsMutilDateRangePicker.prototype.init = function () {
        this.box.append(this.$input);
        var self = this;
        this.$input.daterangepicker(this.opts, function (a, b, c, d) {
            self.opts.isDefaultCallback && self.defaultCallback(a, b, c, d);
            self.opts.callback.call(this, a, b, c, d)
        });
        this.$input.on('cancel.daterangepicker', function (ev, picker) {
            $(this).val('');
            self.$start.val('');
            self.$end.val('');
        });
        this.bindEvent();
        this.opts.initAfter && this.opts.initAfter(this);
    }
    xmsMutilDateRangePicker.prototype.defaultCallback = function (a, b, c, d) {
        var self = this;
        self.$input.val(a.format('YYYY-MM-DD') + '-' + b.format('YYYY-MM-DD'))
        self.$start.val(a.format('YYYY-MM-DD'));
        self.$end.val(b.format('YYYY-MM-DD'));
    }
    xmsMutilDateRangePicker.prototype.destroy = function (datas) {
    }

    xmsMutilDateRangePicker.prototype.bindEvent = function (datas) {
        var self = this;
        self.$input.on('keyup', function () {
            $(this).val('');
            self.$start.val('');
            self.$end.val('');
        })
    }
    $.fn.xmsMutilDateRangePicker = function (opts, _config, _more) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _box = new xmsMutilDateRangePicker(this, opts);
                $this.data().xmsMutilDateRangePicker = _box;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().xmsMutilDateRangePicker) {
                    if (!$this.data().xmsMutilDateRangePicker[opts]) throw new Error('没有这个方法');
                    res = $this.data().xmsMutilDateRangePicker[opts](_config, _more);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

(function ($, root) {
    "use strict"
    var defaults = {
        selecter: ".tab",
        item: "a.collapse-title",
        content: "div.panel-collapse",
        type: "append",
        datas: [],
        mapkey: { id: 'queryviewid', name: 'entityname', other: 'referencingattributename', more: 'tabname' },
        autoload: true,
        alwayLoad: false,
        clickHandler: null
    }
    function asyncTabs(obj, opts) {
        var self = this, $this = $(obj);
        self.opts = opts;
        var timestrap = new Date() * 1 + new Date().toString(16);
        self.list = $('<ul id="asynctabs_' + timestrap + '" class="nav nav-tabs" role="tablist"></ul>');
        self.context = $('<div id="asynctabsContent_' + timestrap + '" class="tab-content"></div>');
        var _html = [];
        var type = $this.attr("data-type") || opts.type;
        $.each(opts.datas, function (key, item) {
            if (item && item[opts.mapkey.id]) {
                var html = '<li role="presentation" id="tab_hd_{{id}}" data-id="{{id}}" data-other="{{other}}" class="asynctabs-item "><a href="#{{id}}"  role="tab" data-toggle="tab"  aria-expanded="true">{{name}}</a></li>';
                html = html.replace(/{{id}}/g, item[opts.mapkey.id]);
                html = html.replace(/{{name}}/g, item[opts.mapkey.name] || item[opts.mapkey.more]);
                html = html.replace(/{{other}}/g, item[opts.mapkey.other]);
                self.context.append('<div class="tab-pane fade in" id="{{id}}"></div>'.replace(/{{id}}/g, item[opts.mapkey.id]));
                _html.push(html);
            }
        });
        self.list.html(_html.join(""));
        $this.append(self.list).append(self.context);
        //  console.log(list.find('li.formtab-item:gt(0)'))

        self.list.find('li.asynctabs-item').on('click', function () {
            var _id = $(this).children('a').attr('href');
            var $this = $(this);
            var $id = $(_id);
            if ($id.length > 0) {
                setTimeout(function () {
                    opts.clickHandler && opts.clickHandler($this, $id);
                }, 50);
            }
        });
        if (self.opts.autoload) {
            setTimeout(function () {
                self.list.find('li.asynctabs-item:first a').trigger('click');
            })
        }
    }
    asyncTabs.prototype.init = function () {
    }
    asyncTabs.prototype.hideByIndex = function (num) {
        this.list.find('li.asynctabs-item').eq(num).hide();
        this.context.find('.panel-collapse').eq(num).hide();
    }
    asyncTabs.prototype.showByIndex = function (num) {
        var _li = this.list.find('li.asynctabs-item').eq(num);
        _li.show();
        _li.children('a').trigger('click');
        this.context.find('.panel-collapse').eq(num).show();
    }
    asyncTabs.prototype.hideByDomId = function (domId) {
        $('#tab_hd_tab_' + domId).hide();
        $('#tab_' + domId).hide();
    }
    var createfirst = false;
    $.fn.asyncTabs = function (opts, _config) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                createfirst = true;
                var _tree = new asyncTabs(this, opts);
                $this.data().asyncTabs = _tree;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().asyncTabs) {
                    // console.log(_config);
                    if (!$this.data().asyncTabs[opts]) throw new Error('没有这个方法');
                    res = $this.data().asyncTabs[opts](_config);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

(function ($, root) {
    "use strict"
    var defaults = {
        keymaps: { key: 'id', name: 'value' }
        , tmpl: '<div><label><input type="{select}" value="{key}" /> {name}</label></div>'
        , linkTmpl: '<div data-id="{key}"><label>{name}</label><span class="" data-id="{key}" title="删除">x</span></div>'
        , context: 'body'
        , singleSelect: false
        , getColDatas: null
        , getDatas: null
    }
    var prefix = 'mutil_lookup_';
    var count = 0;
    function mutilLookup(obj, opts) {
        var self = this, $this = $(obj);
        this.__id = count++;
        this.id = prefix + this.__id;
        self.opts = opts;
        this.$box = $this.hide();
        this.$context = $('body');
        this.$wrap = $('<div class="' + prefix + 'wrap input-group" id="' + prefix + '_id_' + count + '"></div>');
        this.$links = $('<div class="' + prefix + 'links"></div>');;
        this.$list = $('<div class="' + prefix + 'list" id="' + prefix + '_list_id_' + count + '"></div>');;
        this.$listWrap = $('<div class="' + prefix + 'list-wrap"></div>');;
        this.$listWrap.append(this.$list);
        this.$input = $('<input type="text" class="form-control input-sm" />');
        this.$hidden = $('<input type="hidden" value="" />');
        this.$buttons = $('<span class="' + prefix + 'buttons input-group-btn"></span>');
        this.$searcher = $('<div class="' + prefix + 'searcher input-group "><input type="text" class="form-control input-sm" /><span class="input-group-btn"><button type="button" class="btn btn-default btn-sm" title="clear" name="clearsearch"><span class="glyphicon glyphicon-remove-sign"></span></button><button type="button" class="btn btn-default btn-sm" name="findsearch" title="find"><span class="glyphicon glyphicon-search"></span></button></span></div>');
        this.$box.parent().append(this.$buttons).css('display', 'table-row').addClass('input-group');
        this.q = '';
        this.filter = null;
        if (typeof XmsFilter != 'undefined') {
            this.filter = new XmsFilter();
        }
        this.isShow = false;
        this.colDatas = [];
        this.settingDatas = [];
        this.datas = [];//
        this.selecteds = [];//已勾选的数据
        console.log('mutillookup', this);
        this.init();
    }
    mutilLookup.prototype.init = function () {
        // this.$box.parent().prepend(this.$input)
        this.$input.insertAfter(this.$box)
        this.$context.append(this.$wrap);
        this.$wrap.hide()
            // .append(this.$input)
            //.append(this.$buttons)
            .append(this.$searcher)
            .append(this.$links)
            .append(this.$listWrap)
            .append(this.$hidden);
        //.append(this.$box);
        this.createWrap();
        this.setPos();
        this.bindEvent();
    }
    mutilLookup.prototype.setPos = function () {
        var offset = this.$input.offset();
        this.$wrap.css('position', 'absolute');
        this.$wrap.css({
            left: offset.left,
            top: offset.top + this.$box.outerHeight(),
            'z-index': 100
        })
    }
    mutilLookup.prototype.addLink = function (id) {
        var data = $.queryBykeyValue(this.datas, this.settingDatas.entityName + 'id', id);
        var index = $.indexBykeyValue(this.selecteds, this.settingDatas.entityName + 'id', id);
        if (index == -1 && data && data.length > 0) {
            var $link = $('<span data-id="' + id + '">' + data[0].name + ' <em class="mutil-link-remove">x</em></span>');
            this.$links.append($link);

            this.selecteds.push(data[0]);
        }
    }
    mutilLookup.prototype.removeLink = function (id, obj, e) {
        var index = $.indexBykeyValue(this.selecteds, this.settingDatas.entityName + 'id', id);
        if (~index) {
            var data = this.selecteds[index];
            if (data) {
                this.$links.find('span[data-id="' + id + '"]').remove();
                this.selecteds.splice(index, 1);
                // this.$links.append('<span>' + data[0].name + '</span>');
            }
        }
    }

    mutilLookup.prototype.bindEvent = function () {
        var self = this;
        this.$box.parent().on('click', '[name="mutilselectbtn"]', function (e) {
            self.show();
            self._getDatas();
        });
        this.$box.parent().on('click', '[name="mutilclearbtn"]', function (e) {
            self.hide();
            self.clear();
        });
        this.$list.on('datatable.rowClick', function (e, opts) {
            if (opts.ui.rowData) {
                if (opts.type == true) {
                    self.addLink(opts.ui.rowData[self.settingDatas.entityName + 'id']);
                } else {
                    self.removeLink(opts.ui.rowData[self.settingDatas.entityName + 'id']);
                }
            }
        });
        this.$list.on('datatable.headerCheckboxCheck', function (e, opts) {
            if (opts.ui.rowData) {
                if (opts.type == true) {
                    self.addAllLink(opts.ui.rowData[self.settingDatas.entityName + 'id']);
                } else {
                    self.removeAllLink(opts.ui.rowData[self.settingDatas.entityName + 'id']);
                }
            }
        });
        //datatable.headerCheckboxCheck
        this.$links.on('click', '.mutil-link-remove', function (e) {
            var id = $(this).parent().attr('data-id');
            self.removeLink(id, this, e);
        });
        this.$searcher.find('button[name="clearsearch"]').on('click', function () {
            self.$searcher.find('input[type="text"]').val('');
            self.$list.cDatagrid('refreshDataAndView');
        })
        this.$searcher.find('button[name="findsearch"]').on('click', function () {
            self.$list.cDatagrid('refreshDataAndView');
        })
    }
    mutilLookup.prototype.clear = function () {
        var self = this;
        //this.$box.parent().off();
        //this.$list.off();
        self.$list.cDatagrid('destroy');
    }
    mutilLookup.prototype.createWrap = function () {
        var self = this;
        this._createButtons();
        // var url = this.opts.getColDatas && this.opts.getColDatas();
    }
    mutilLookup.prototype.createElement = function () {
        var self = this;
        if (self.settingDatas) {
            self.createLinks();
            self.createList();
        }
    }
    mutilLookup.prototype._getDatas = function (callback) {
        var self = this;
        var url = ORG_SERVERURL + '/api/schema/queryview/GetViewInfo?';
        var entityid = '7c4490c7-d179-4baa-b54a-da26ba5c1b84' //self.$box.attr('data-entityid');
        var queryid = self.$box.attr('data-queryid');
        var datas = self.settingDatas;
        if (queryid) {
            url += 'id=' + queryid;
        } else if (entityid) {
            url += 'entityid=' + entityid;
        }
        Xms.Web.Get(url, function (res) {
            //   console.log('getbyentityid', JSON.parse(res.Content));
            var jsonres = datas.jsonres = JSON.parse(res.Content);
            datas.queryviews = jsonres.views;
            self.setDatas();
            self.createElement();
        });
    }
    mutilLookup.prototype.setDatas = function () {
        var datas = this.settingDatas
        var jsonres = datas.jsonres
        if (datas.queryviews) {
            var self = this;
            var index = self._getDefaultViewKey(datas.queryviews);
            var first = datas.queryviews[index];
            datas.entityId = first.entityid;
            datas.entityName = first.entityname;
            datas.queryId = first.queryviewid;
            datas.aggregateconfig = first.aggregateconfig;
            datas.layoutconfig = first.layoutconfig.toLowerCase();
            datas.attributesInfo = JSON.parse(JSON.stringify(jsonres.attributes).toLowerCase());
            datas.setAttributesShow = datas.attributesInfo = filterAttributes(datas.attributesInfo, datas);
            this.settingDatas = $.extend({}, this.settingDatas, datas);
            if (this.settingDatas.attributesInfo.length > 0) {
                $.each(this.settingDatas.attributesInfo, function (i, n) {
                    if (n.name != '' && n.name.indexOf('.') == -1) {
                        self.settingDatas.entityName = n.entityname;
                    }
                });
            }
        }
    }
    // mutilLookup.prototype.getColDatas = function (callback) {}
    mutilLookup.prototype._createButtons = function () {
        this.$buttons.html('<button type="button" name="mutilclearbtn" class="btn btn-default btn-sm" title="clear"><span class="glyphicon glyphicon-remove-sign"></span></button><button type="button" name="mutilselectbtn" class="btn btn-default btn-sm" title="find"><span class="glyphicon glyphicon-search"></span></button>');
    }
    mutilLookup.prototype.createLinks = function () {
    }
    mutilLookup.prototype.createList = function () {
        var self = this;
        var datas = self.settingDatas;
        var datagridconfig = {
            height: 300,
            //获取数据的方法
            getDataUrl: function (cdatagrid, opts) {
                return ORG_SERVERURL + '/api/data/fetchAndAggregate?entityid=' + self.settingDatas.entityId + '&queryviewid=' + self.settingDatas.queryId + '&onlydata=true&pagesize=' + cdatagrid.opts.pageModel.rPP + '&page=' + cdatagrid.opts.pageModel.page
            },
            getColModels: function (grid, opts) {
                return self.settingDatas.attributesInfo;
                //'/api/schema/queryview/getattributes/' + datas.queryId + '?__r=' + new Date().getTime();
            },
            scrollModel: { autoFit: false },
            filterColModel: function (opts) {
                opts.colModel = self.settingDatas.attributesInfo;
                return opts;
            },
        }
        datagridconfig.pageModel = { type: "remote", rPP: 10, page: 1, strRpp: "{0}" }
        datagridconfig.extend = function (datagrid) {
            var extobj = {
                isJsonAjax: true,

                getData: function (dataJSON, textStatus, jqXHR) {
                    var resjson = dataJSON.fetchdata
                    var data = resjson.items;
                    self.datas = data;
                    console.log(dataJSON)
                    var res = { curPage: resjson.currentpage || 1, totalRecords: resjson.totalitems, data: data }

                    return res;
                },
                filterSendData: function (postData, objP, DM, PM, FM) {
                    //console.log('postdata', postData, objP, DM, PM, FM)
                    //filter: gridview_filters.getFilterInfo(),
                    var q = self.$searcher.find('input[type="text"]').val();
                    // if (q != '') {
                    postData.q = q
                    // }
                    $.extend(postData, { sortby: objP.dataIndx, sortdirection: objP.dir == 'up' ? '0' : '1', pagesize: PM.rPP });
                    return postData;
                }
            }
            if (datas.fetchconfig) {
                var _fetchconfig = JSON.parse(datas.fetchconfig.toLowerCase());
                if (_fetchconfig.orders && _fetchconfig.orders.length > 0) {
                    extobj.sortIndx = _fetchconfig.orders[0].attributename;
                    extobj.sortDir = _fetchconfig.orders[0].ordertype == 'descending' ? 'up' : 'down';
                }
            }
            $.extend(datagrid.opts.dataModel, extobj)
        }
        // this.$grid = this.box.cDatagrid(this.opts);
        setTimeout(function () {
            self.$list.cDatagrid(datagridconfig);
        })
    }
    mutilLookup.prototype._getDefaultViewKey = function (datas) {
        var index = 0;
        $.each(datas, function (i, n) {
            if (n.isdefault == true) {
                index = i;
                return false;
            }
        });
        return index;
    }
    mutilLookup.prototype.show = function () {
        this.$wrap.show();
    }
    mutilLookup.prototype.hide = function () {
        this.$wrap.hide();
    }
    function filterAttributes(items, datas) {
        var layoutconfigObj = '';
        if (datas.layoutconfig && datas.layoutconfig != "") {
            layoutconfigObj = JSON.parse(datas.layoutconfig);
        }
        if (layoutconfigObj) {
            var layoutitems = layoutconfigObj.rows[0].cells
            $.each(layoutitems, function (i, n) {//表头数据
                var tar = null;
                $.each(items, function (key, item) {//字段数据
                    if (~n.name.indexOf('.')) {//如果在表头数组
                        var attrs = n.name.split('.');
                        if (attrs[1] == item.name.toLowerCase()) {
                            item.name = n.name;
                            item.localizedname = n.label ? n.label : item.label ? item.label : item.localizedname + '(' + item.entitylocalizedname + ')' ? item.localizedname : item.entitylocalizedname//item.label; //+ '(' + item.entitylocalizedname + ')';
                            item.width = n.width;
                            tar = item;
                            n.editable = false;
                            return false;
                        }
                    } else {
                        if (item.name == n.name.toLowerCase()) {
                            item.width = n.width;
                            item.localizedname = n.label ? n.label : item.label ? item.label : item.localizedname ? item.localizedname : item.entitylocalizedname
                            tar = item;
                            n.editable = false;
                            return false;
                        }
                    }
                });
                if (tar) {
                    $.extend(n, tar, n);
                }
            });
            return layoutitems;
        } else {
            return items
        }
    }
    $.fn.mutilLookup = function (opts, _config) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);

                var _tree = new mutilLookup(this, opts);
                $this.data().mutilLookup = _tree;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().mutilLookup) {
                    // console.log(_config);
                    if (!$this.data().mutilLookup[opts]) throw new Error('没有这个方法');
                    res = $this.data().mutilLookup[opts](_config);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

//面包屑数据操作
(function ($, root, un) {
    function Cramb() {
        this.list = [];
    }
    Cramb.prototype.add = function (data) {
        var index = this.indexOf(data);
        var lastdata = this.list[this.list.length - 1];
        if (!~index) {
            if (!lastdata) {
                this.list.push(data);
            } else if (data.layer == lastdata.layer) {
                this.list.pop();
                this.list.push(data);
            } else if (data.layer >= lastdata.layer) {
                this.list.push(data);
            }
        }
    }
    Cramb.prototype.indexOf = function (data) {
        var index = -1;
        $.each(this.list, function (i, n) {
            if (data.id == this.id) {
                index = i;
                return false;
            }
        });
        return index;
    }
    Cramb.prototype.removeByIndex = function (index) {
        this.list.splice(index, this.list.length - index);
    }
    root.Cramb = Cramb;
})(jQuery, window);

(function ($, root) {
    "use strict"
    var defaults = {
        classname: ''
        , iframeContext: $('body'),
        linksContext: null,
        leftCtrl: null,
        rightCtrl: null,
        $refresh: null,
        offsetH: 50,
        linkHandler: null,
        checkHandler: null
    }
    var prefixname = 'iframelink_'
    function LinkIframe(id, targetname, src, opts, notLoad) {
        var isload = ' data-isloaded="1"';
        if (notLoad) {
            src = '';
            isload = ''
        }
        return $('<iframe width="100%" src="" id="' + prefixname + id + '" data-id="' + id + '"  name="' + prefixname + id + '" class="ifrmae-list ifrmae-list-item" frameborder="0" onload="Javascript:Xms.Web.iFrameHeight(this,' + opts.offsetH + ')"   scrolling="auto"></iframe>"');
    }

    function iFrameHeight($iframe, offsetH) {
        offsetH = offsetH || defaults.offsetH;
        var ifm = $iframe.get(0);
        var subWeb = document.frames ? document.frames["bodyframe"].document : ifm.contentDocument;
        if (ifm != null && subWeb != null) {
            ifm.height = $(window).height() - offsetH;//subWeb.body.scrollHeight;
        }
    }
    function _Iframe_Link(link) {
        this.id = link.id;
        this.name = link.name;
        this.icon = link.icon || '';
        this.src = link.src;
        this.notremove = link.notremove || false;
        this.other = link.other || '';
        this.notLoad = link.notLoad;
    }
    function iframeLinks(obj, opts) {
        this.box = $(obj);
        this.opts = opts;
        this.list = [];
        this.$iframes = this.opts.iframeContext;
        this.$links = this.box;
        this.$leftCtrl = this.opts.leftCtrl;
        this.$rightCtrl = this.opts.rightCtrl;
        this.$refresh = this.opts.$refresh;
        this.$delete = this.opts.$delete;
        this.activeIndex = 0;
        this.pageIndex = 0;
        this.pageTotal = 0;
        this.groups = [];
        this.init();
    }
    iframeLinks.prototype.getDatas = function () {
        return this.list;
    }
    iframeLinks.prototype.init = function () {
        this.bindEvent();
    }
    iframeLinks.prototype.setopts = function (opts) {
        $.extend(this.opts, opts);
    }
    iframeLinks.prototype.resize = function (opts) {
        var self = this;
        var boxWidth = this.box.innerWidth();
        var listWidth = 0;
        this.groups = [];
        var groupsindex = 0;
        this.$links.find('.iframe-link-linkitem').show();
        this.$links.children().each(function (i, n) {
            var hasNext = $(n).next();
            listWidth += $(n).outerWidth();
            if (hasNext.length > 0) {
                var nextWidth = hasNext.outerWidth();
                if (listWidth + nextWidth < boxWidth) {
                    if (!self.groups[groupsindex]) {
                        self.groups[groupsindex] = [];
                    }
                    self.groups[groupsindex].push($(n));
                    $(n).attr('data-groupindex', groupsindex);
                } else {
                    groupsindex++;
                    listWidth = 0;
                }
            } else {
                if (listWidth < boxWidth) {
                    if (!self.groups[groupsindex]) {
                        self.groups[groupsindex] = [];
                    }
                    self.groups[groupsindex].push($(n));
                    $(n).attr('data-groupindex', groupsindex);
                } else {
                    groupsindex++;
                    listWidth = 0;
                }
            }
        });
        console.log(self.groups);
        this.pageTotal = Math.ceil(listWidth / boxWidth);
        if (this.pageTotal > 0 && this.pageIndex == 0) {
            this.pageIndex = 1;
        }
    }
    iframeLinks.prototype.refreshIframe = function (id) {
        var self = this;
        var index = $.indexBykeyValue(this.list, 'id', id);
        
        if (index != -1) {
            var data = this.list[index];
            var _src = data.src;
            if (data.src && data.src.indexOf(ORG_SERVERURL) == -1 && data.src.indexOf('http') == -1) {
                _src = ORG_SERVERURL + data.src;
            }
            $('#' + prefixname + data.id).attr('src', _src);
        }
    }
    iframeLinks.prototype.openByGroups = function (id) {
        var self = this;
        var index = -1;
        var res = null;
        if (self.groups.length > 0) {
            $.each(self.groups, function (i, n) {
                var flag = false;
                if (self.groups[i]) {
                    $.each(self.groups[i], function (ii, nn) {
                        if (nn.attr('data-id') == id) {
                            flag = true;
                            res = nn;
                            return false;
                        }
                    });
                }
                if (flag == true) {
                    index = i;
                    return false;
                }
            });
            self.openGroupsByIndex(index);
        }
        self.pageIndex = index;
        return res;
    }
    iframeLinks.prototype.openGroupsByIndex = function (index) {
        var self = this;
        if (index != -1 && self.groups[index]) {
            this.$links.find('.iframe-link-linkitem').hide();
            $.each(self.groups[index], function (i, n) {
                n.show();
            })
        }
    }
    iframeLinks.prototype.addLink = function (obj, notload) {
        var id = obj.id;
        var name = obj.name;
        var src = obj.src
        var index = $.indexBykeyValue(this.list, 'id', id);
        if (index == -1) {
            var link = new _Iframe_Link(obj);
            this.list.push(link);
            this.addElementLink(link);
            this.addElementIframe(link);
            if (!link.notLoad) {
                this.setIframeSrc(link);
            }
            this.openElementIframe(link);
            this.openElementLink(link);
            this.resize();
            this.openByGroups(link.id);
            //if (!notload) {
            //    this.setIframeSrc(link);
            //}
        } else {
            var data = this.list[index];
            if (data.notLoad) {
                this.setIframeSrc(data);
            }
            this.openElementIframe(data);
            this.openElementLink(data);
            this.openByGroups(data.id);
            //if (!notload) {
            //    this.setIframeSrc(data);
            //}
        }

        this.opts.checkHandler && this.opts.checkHandler.call(this);
    }
    iframeLinks.prototype.removeLink = function (id, obj) {
        var index = $.indexBykeyValue(this.list, 'id', id);
        if (index != -1 && !this.list[index].iframeLinkIsDefault) {
            var link = this.list.splice(index, 1);
            this.removeElementLink(link[0], obj);
            this.removeElementIframe(link[0], obj);
            if (this.list.length > 0) {
                var lastData = this.list[this.list.length - 1];
                this.openElementLink(lastData);
                this.openElementIframe(lastData);
                this.setIframeSrc(lastData);
                this.resize();
                this.openByGroups(lastData.id);
            }
        }
        this.opts.checkHandler && this.opts.checkHandler.call(this);
    }
    iframeLinks.prototype.addElementLink = function (link) {
        var _html = [];
        var close = link.notremove == true ? '' : ' <span class="glyphicon glyphicon-remove iframe-link-linkitemclose" data-id="' + link.id + '"></span>';
        var other = (link.other && link.other != '') ? link.other : '';
        var icon = (link.icon != '' && link.icon.indexOf('<') != -1) ? link.icon : link.icon == '' ? '' : '<span class="' + link.icon + '"></span>';
        if (link.name != '' && link.name.indexOf('<') != '-1') { icon = ''; }
        //在自定义中可添加快捷方式
        var insertQuicklist = '';
        //if (!link.notremove) {
        //    insertQuicklist = '<span class="iframe-link-item-insertq" data-id="' + link.id + '" title="添加到快捷方式"></span>'
        //}
        _html.push('<div class="iframe-link-linkitem btn btn-sm btn-default" data-id="' + link.id + '" title="">' + icon + other + link.name + close + insertQuicklist + '</div>');
        this.$links.append($(_html.join('')));
    }
    iframeLinks.prototype.addElementIframe = function (link) {
        var iframe = LinkIframe(link.id, link.name, link.src, this.opts, link.notLoad);
        this.$iframes.children().hide()
        this.$iframes.append(iframe);
    }
    iframeLinks.prototype.removeElementLink = function (link, obj) {
        $(obj).parent().remove();
    }
    iframeLinks.prototype.setDefault = function (id) {
        var index = $.indexBykeyValue(this.list, 'id', id);
        if (index != -1) {
            this.list[index].iframeLinkIsDefault = true;
        }
    }
    iframeLinks.prototype.removeElementIframe = function (link, obj) {
        $('#' + prefixname + link.id).remove();
    }
    iframeLinks.prototype.openElementLink = function (link, obj) {
        var index = $.indexBykeyValue(this.list, 'id', link.id);
        if (index != -1) {
            this.activeIndex = index;
        }
        this.$links.find('.iframe-link-linkitem').removeClass('active');
        this.$links.find('.iframe-link-linkitem[data-id="' + link.id + '"]').show().addClass('active');
    }
    iframeLinks.prototype.setIframeSrc = function (link) {
        var $iframe = $('#' + prefixname + link.id);
        var islaoded = $iframe.attr('data-isloaded');
        if (!islaoded) {
            var _src = link.src;
            if (link.src && link.src.indexOf(ORG_SERVERURL) == -1 && link.src.indexOf('http') == -1) {
                _src = ORG_SERVERURL + link.src;
            }
            $iframe.attr('src', _src);
            $iframe.attr('data-isloaded', 1);
        }
    }
    iframeLinks.prototype.openElementIframe = function (link) {
        this.$iframes.find('.ifrmae-list-item').hide();
        var $iframe = $('#' + prefixname + link.id);
        $iframe.show();
    }
    iframeLinks.prototype.bindEvent = function (link) {
        var self = this;
        //this.$iframes.on('click', function () {
        //})
        this.$leftCtrl && this.$leftCtrl.on('click', function () {
            self.pageIndex--;
            if (self.pageIndex <= 0) {
                self.pageIndex = 0;
            }
            self.openGroupsByIndex(self.pageIndex);
            self.opts.checkHandler && self.opts.checkHandler.call(self);
        })
        this.$rightCtrl && this.$rightCtrl.on('click', function () {
            self.pageIndex++;
            if (self.pageIndex > self.groups.length - 1) {
                self.pageIndex = self.groups.length - 1;
            }
            self.openGroupsByIndex(self.pageIndex);
            self.opts.checkHandler && self.opts.checkHandler.call(self);
        })
        this.$refresh && this.$refresh.on('click', function (e) {
            var id = self.$links.find('.iframe-link-linkitem.active').attr('data-id');
            self.refreshIframe(id);
            self.opts.checkHandler && self.opts.checkHandler.call(self);
        })
        this.$links.on('click', '.iframe-link-linkitem', function (e) {
            e.stopPropagation();
            var id = $(this).attr('data-id');
            var index = $.indexBykeyValue(self.list, 'id', id);
            self.openElementLink(self.list[index]);
            self.openElementIframe(self.list[index]);
            self.setIframeSrc(self.list[index]);
            self.opts.linkItemHandler && self.opts.linkItemHandler(self.list[index], id);
            self.opts.checkHandler && self.opts.checkHandler.call(self);
        })

        this.$links.on('click', '.iframe-link-linkitemclose', function (e) {
            e.stopPropagation();
            var id = $(this).attr('data-id');
            self.removeLink(id, this);
        })

        this.$links.on('click', '.iframe-link-item-insertq', function (e) {
            e.stopPropagation();
            var id = $(this).attr('data-id');
            var link = $.queryBykeyValue(self.list, 'id', id);
            self.opts.rightTopIconHandle && self.opts.rightTopIconHandle.call(this, e, link, self);
        })

        this.$delete && this.$delete.on('click', function () {
            self.$links.find('.iframe-link-linkitemclose').each(function () {
                var id = $(this).attr('data-id');
                self.removeLink(id, this);
            });
        })
    }
    $.fn.iframeLinks = function (opts, _config) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _tree = new iframeLinks(this, opts);
                $this.data().iframeLinks = _tree;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().iframeLinks) {
                    // console.log(_config);
                    if (!$this.data().iframeLinks[opts]) throw new Error('没有这个方法');
                    res = $this.data().iframeLinks[opts](_config);
                    return false;
                }
            });
            return res;
        }
    }
    $.fn.iframeLinks.iFrameHeight = iFrameHeight;
    $.fn.iframeLinks._Iframe_Link = _Iframe_Link;
})(jQuery, window);

(function ($, root) {
    "use strict"
    var defaults = {
        classname: 'toggle-iframe-right'
        , iframeContext: null
        , defaultSrc: ''
        , offsetH: 0
    }
    var iframeCount = 0;
    var prefixname = 'toggleIframe_'
    function LinkIframe(src, opts) {
        var id = iframeCount++;
        return $('<iframe width="100%" src="' + src + '" id="' + prefixname + id + '" data-id="' + id + '"  name="' + prefixname + id + '" class="toggle-iframe " frameborder="0" onload="Javascript:Xms.Web.iFrameHeight(this,' + opts.offsetH + ')"  scrolling="auto"></iframe>"');
    }
    function toggleIframe(obj, opts) {
        this.box = $(obj);
        this.opts = opts;
        this.state = { active: false };
        this.$wrap = $('<div class="toggle-iframe-wrap ' + this.opts.classname + ' "></div>');
        this.$close = $('<div class="toggle-iframe-close"><span class="glyphicon glyphicon-remove"></span></div>')
        this.$context = this.opts.iframeContext || $('body');
        this.$iframe = LinkIframe(this.opts.defaultSrc, opts);
        this.$context.append(this.$wrap)
        this.$wrap.append(this.$close);
        this.init();
    }
    toggleIframe.prototype.init = function () {
        this.$wrap.append(this.$iframe)
        this.bindEvent();
    }
    toggleIframe.prototype.changeSrc = function (src) {
        this.show();
        this.$iframe.attr('src', src);
    }
    toggleIframe.prototype.show = function (src) {
        this.$wrap.addClass('active')
    }
    toggleIframe.prototype.hide = function (src) {
        this.$wrap.removeClass('active')
    }
    toggleIframe.prototype.bindEvent = function (link) {
        var self = this;
        this.$close.on('click', function () {
            self.hide();
        })
    }
    $.fn.toggleIframe = function (opts, _config) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _tree = new toggleIframe(this, opts);
                $this.data().toggleIframe = _tree;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().toggleIframe) {
                    // console.log(_config);
                    if (!$this.data().toggleIframe[opts]) throw new Error('没有这个方法');
                    res = $this.data().toggleIframe[opts](_config);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

(function ($, root) {
    "use strict"
    var defaults = {
        classname: 'toggle-iframe-right'
        , auto: true,
        event: 1,
        target: null,
        close: false,
        context: null,
        position: 'rel'//abs
        , showHandler: null
    }
    var iframeCount = 0;
    var prefixname = 'toggleTooltip_'

    function toggleTooltip(obj, opts) {
        this.box = $(obj);
        this.opts = opts;
        this.target = this.opts.target;
        this.context = this.opts.context;
        if (!this.target) throw new Error('target not null');
        this.init();
    }
    toggleTooltip.prototype.init = function () {
        // this.$wrap.append(this.$iframe)
        this.setPos();
        this.bindEvent();
    }
    toggleTooltip.prototype.setPos = function () {
        if (this.opts.position == 'abs') {
            var offset = this.box.offset();
        } else {
            var offset = this.box.position();
            this.context.append(this.target);
        }
        var boxSize = { w: this.box.width(), h: this.box.height() };
        this.target.css({
            top: offset.top + boxSize.h,
            left: offset.left
        })
    }
    toggleTooltip.prototype.show = function () {
        this.target.addClass('in')
    }
    toggleTooltip.prototype.hide = function () {
        this.target.removeClass('in')
    }
    toggleTooltip.prototype.bindEvent = function () {
        var self = this;
        this.box.mousemove(function () {
            self.setPos();
            self.show();
            self.opts.showHandler && self.opts.showHandler(self.box, self);
        })
        this.box.mouseout(function () {
            self.hide()
        })
    }
    $.fn.toggleTooltip = function (opts, _config) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _tree = new toggleTooltip(this, opts);
                $this.data().toggleTooltip = _tree;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().toggleTooltip) {
                    // console.log(_config);
                    if (!$this.data().toggleTooltip[opts]) throw new Error('没有这个方法');
                    res = $this.data().toggleTooltip[opts](_config);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);

(function ($, root) {
    "use strict"
    var defaults = {
        classname: 'toasttip'
        , auto: true,
        event: 1,
        target: null,
        close: false,
        context: null,
        position: 'rel'//abs
        , showHandler: null
    }
    var iframeCount = 0;
    var prefixname = 'toasttip_'

    function ToastTip(obj, opts) {
        this.box = $(obj);
        this.opts = opts;
        this.target = this.opts.target;
        this.context = this.opts.context || $('body');
        this.pos = { x: 0, y: 0 };
        if (this.target) {
            this.pos.x = this.target.offset().left;
            this.pos.y = this.target.offset().top;
        };
        this.init();
    }
    ToastTip.prototype.init = function () {
        // this.$wrap.append(this.$iframe)
        this.setPos();
        this.bindEvent();
    }
    ToastTip.prototype.setPos = function () {
        if (this.opts.position == 'abs') {
            var offset = this.box.offset();
        } else {
            var offset = this.box.position();
            this.context.append(this.target);
        }
        var boxSize = { w: this.box.width(), h: this.box.height() };
        this.target.css({
            top: offset.top + boxSize.h,
            left: offset.left
        })
    }
    ToastTip.prototype.show = function (opts) {
        this.target.addClass('in')
    }
    ToastTip.prototype.hide = function () {
        this.target.removeClass('in')
    }
    ToastTip.prototype.bindEvent = function () {
    }
    $.fn.ToastTip = function (opts, _config) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            this.each(function () {
                var $this = $(this);
                var _tree = new ToastTip(this, opts);
                $this.data().ToastTip = _tree;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().ToastTip) {
                    // console.log(_config);
                    if (!$this.data().ToastTip[opts]) throw new Error('没有这个方法');
                    res = $this.data().ToastTip[opts](_config);
                    return false;
                }
            });
            return res;
        }
    }
})(jQuery, window);