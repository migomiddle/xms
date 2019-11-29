//@ sourceURL=common/formular.js
//闭包执行一个立即定义的匿名函数
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
    //deps

    //page init
    var page_common_formular = {
        getFormularResult: getFormularResult,
        handleFormula: handleFormula,
        getLeftFormular: getLeftFormular,
        getSubGridEntityList: getSubGridEntityList,
        setSubGridFormular: setSubGridFormular
    }
    function getFormularResult(leftArr, leftStr, rights, context) {//左边的字段，左边的等式
        $.each(leftArr, function (key, item) {
            var itemname = item;
            var itemObj = $("input[data-name='" + itemname.toLowerCase() + "'][data-isrelated='False']", context);
            var itemVal = itemObj.val().replace(/\,/g, '');
            var reg = new RegExp(itemname.toLowerCase());
            leftStr = leftStr.toLowerCase().replace(reg, itemVal);
        });
        var res = 0;
        console.log(leftArr);
        try {
            res = eval(leftStr);//计算公式
            var rightobj = $("input[data-name='" + rights.toLowerCase() + "'][data-isrelated='False']", context);
            var type = rightobj.attr('data-type');
            var precision = rightobj.attr('data-precision') * 1;
            if (type == 'money' && res && res != '') {
                res = (res * 1).toFixed(precision || 2);
            }
            rightobj.val(res);
        } catch (e) {
            // console.log(e);
        }
    }
    function handleFormula(items) {
        var res = '';
        $.each(items, function (i, n) {
            res += n;
        });
        return res;
    }
    function getLeftFormular(items, isLeft) {//isLeft   需计算的字段是否在字段左边
        var arr = [];
        $.each(items, function (i, n) {
            if (n == "=") {
                return false;
            } else {
                if (!checkFormularRuler(n)) {
                    arr.push(n);
                }
            }
        });
        if (isLeft) {
            arr = arr.reverse();
        }
        return arr;
    }
    function checkFormularRuler(nn) {
        var temp = nn;
        nn = {}; nn.key = temp;
        return nn.key == "+" || nn.key == "-" || nn.key == "*" || nn.key == "/" || nn.key == "=" || nn.key == "(" || nn.key == ")";
    }
    function getSubGridEntityList(arr, type) {
        var list = [];
        type = type || 'formular';
        $.each(arr, function (key, item) {
            if (item.Type == type) {
                list.push(item);
            }
        });
        return list;
    }
    function setSubGridFormular(obj, theme) {
        var $this = $(obj);
        var ruler = $this.attr("data-formular");
        if (!ruler || ruler == "") { return false; }
        // console.log(decodeURIComponent(ruler));
        var rulerObj = JSON.parse(decodeURIComponent(ruler));
        var formularlist = rulerObj;//getSubGridEntityList(rulerObj);//获取值计算的列表
        // console.log(rulerObj)
        if (formularlist.length > 0) {
            $.each(formularlist, function (key, item) {
                if (item.Type == 'formular') {
                    var isLeft = false;
                    if (item.Expression.indexOf('$$$') > -1) {
                        var itemArr = item.Expression.split('$$$');
                    } else {
                        var itemArr = JSON.parse(item.Expression);
                    }
                    if (itemArr[1] == '=') {
                        itemArr = itemArr.reverse();
                        isLeft = false;
                    }
                    var itemRuler = itemArr.join('');
                    var tempArr = itemRuler.split('=');
                    var leftArr = getLeftFormular(itemArr, isLeft);//获取等号左边的等式
                    var rightRuler = itemArr[itemArr.length - 1];//等式右边的字段名
                    var $rightRdom = $this.find('input[data-name="' + rightRuler.toLowerCase() + '"][data-isrelated="False"]');
                    console.log('$rightRdom', $rightRdom.length);
                    if ($rightRdom.length > 0) {
                        $rightRdom.prop('readonly', true);
                    }

                    $.each(leftArr, function (ii, nn) {
                        if (!checkFormularRuler(nn)) {
                            if (theme == 'jqgrid') {
                            } else {
                                var input = $this.find('input[data-name="' + nn.toLowerCase() + '"][data-isrelated="False"]');
                                input.each(function () {
                                    var that = $(this);
                                    var context = that.parents('tr:first');
                                    that.on('change', function () {//绑定字段方法
                                        console.log('change')
                                        getFormularResult(leftArr, tempArr[0], rightRuler, context);//处理等式
                                        console.log(parTr, leftArr, tempArr[0], rightRuler, context)
                                        var parTr = $(this).parents('tr');
                                        var itemRightDom = parTr.find('input[data-name="' + rightRuler.toLowerCase() + '"][data-isrelated="False"]');
                                        if (itemRightDom.length > 0) {
                                            itemRightDom.trigger('change');
                                        }
                                    });
                                    context.find('button[name=editRowBtn]').unbind('gridview.editRow').bind('gridview.editRow', function () {
                                        setSubGridFormular($this)
                                    });
                                    context.find('button[name=saveRowBtn]').unbind('gridview.saveRow').bind('gridview.saveRow', function () {
                                        setSubGridFormular($this)
                                    });
                                    context.find('button[name=cancelRowBtn]').unbind('gridview.cancelRow').bind('gridview.cancelRow', function () {
                                        setSubGridFormular($this)
                                    });
                                });
                            }
                        }
                    });
                } else {//关联字段
                    var input = $('input.lookup[data-name="' + item.Name.toLowerCase() + '"][data-isrelated="False"]');
                    if (input.length == 0) return false;
                    console.log('item.Expression', JSON.parse(item.Expression));
                    var itemArr = JSON.parse(item.Expression);
                    if (input.length > 0) {
                        input.on('change', function () {
                            var inputid = $(this).attr('id');
                            var hiddenDom = $('#' + inputid.replace('_text', ''));
                            var changeEntityid = $(this).attr("data-lookup");;
                            var changeValue = hiddenDom.val();
                            var parentDom = $(this).parents('tr:first');
                            var that = $(this);
                            //console.log('hiddenDom',hiddenDom)
                            setlabelsToTarget(changeEntityid, changeValue, function (data) {
                                $.each(itemArr, function (ii, nn) {
                                    if (!nn) return true;
                                    var ntemp = nn.split('§');
                                    var tarv = ntemp[0];
                                    var sourv = ntemp[1];
                                    var $context = $('input[data-name="' + tarv.toLowerCase() + '"][data-isrelated="True"]', parentDom);
                                    $context.val('');
                                    var list = data.content;

                                    var type = sourv.toLowerCase();
                                    var controltype = $context.attr('data-type') || "nvarchar";
                                    //console.log($context);
                                    // console.log(type)
                                    //console.log("type"+controltype,type);
                                    // console.log("setexts"+controltype,list);
                                    if (!list || !list[type]) return true;
                                    //html.push('<span class="label-tag" data-id="'+list.id+'">'+list[type]+'</span>');
                                    if ($context.is(":disabled")) { return true; }

                                    if (controltype == "lookup" || controltype == "owner" || controltype == "customer") {
                                        // console.log("lookup:",list);
                                        $context = $('input.lookup[data-name="' + nn.toLowerCase() + '"][data-isrelated="True"]', parentDom);
                                        $contextid = $context.attr('id').replace('_text', '');
                                        var $hidden = $('#' + $contextid, parentDom);
                                        $context.val(list[type + "name"]);
                                        $context.attr("data-id", list[type]);
                                        $hidden.val(list[type + "name"]);
                                        $hidden.attr("data-id", list[type]);
                                    } else if (controltype == "state") {
                                        $context.parent().find("input[type='radio']").prop("checked", false);
                                        $context.parent().find("input[type='radio'][value='" + list[type] + "']").prop("checked", true);
                                        $context.val(list[type])
                                    } else if (controltype == "picklist") {
                                        $context.siblings("select>option[value='" + list[type] + "']").prop("selected", true);
                                        $context.val(list[type])
                                    } else {
                                        $context.val(list[type]);
                                    }

                                    $context.trigger('change');
                                })
                            });
                        });
                    }
                }
            });
        }
    }

    window.page_common_formular = page_common_formular;
    return page_common_formular;
});