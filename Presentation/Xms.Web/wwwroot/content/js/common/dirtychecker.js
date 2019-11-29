//@ sourceURL=common/dirtychecker.js
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
    //deps xms.jquery.js  xmsDirtyChecker

    function setFormDirtyValue() {
        var $attributechanged = $('#attributechanged');
        //console.log(dirtyChecker.dirtyList);
        var dirtyAttrs = [];
        if (dirtyChecker.dirtyList.length > 0) {
            for (var i = 0, len = dirtyChecker.dirtyList.length; i < len; i++) {
                var item = dirtyChecker.dirtyList[i];
                var str = '';
                if (typeof item.key === 'string') {
                    str = item.key.replace('_text', "");
                    if (~item.key.indexOf('bp_')) {
                        str = item.key.replace('bp_', "");
                    }//防止添加进业务流程的字段名
                }
                dirtyAttrs.push(str);
            }
            dirtyAttrs = dirtyAttrs.unique();//防止添加进业务流程的字段名,去重
            $attributechanged.val(dirtyAttrs.join(','));
        }
    }

    function bindCheckerEvent(datas) {
        $.each(["keyup", "blur", "change"], function (i, event) {
            event = event || window.event;
            $.each(datas, function (key, item) {
                var tar = $("#" + item.key);
                if (tar.length == 0) {
                    tar = $('#' + item.key.toLowerCase());
                }
                var type = tar.attr("data-controltype");
                if (type == "picklist") {
                    var displaystyle = tar.attr('data-displaystyle');
                    if (displaystyle && displaystyle == 'radio') {
                        var tarSel = tar.siblings('label');
                    } else {
                        var tarSel = tar.next();
                    }

                    tarSel.bind(event, function () {
                        var id = tar.attr("id");
                        var value = tar.val();
                        dirtyChecker.setValue(item.key, value);
                        dirtyChecker.checkWatchs(function () {
                            bindBeforeUnload();
                        });
                    });
                } else if (type == "state" || type == "bit") {
                    var tarSel = tar.parent().find("input[type='radio']");
                    tarSel.bind(event, function () {
                        var id = tar.attr("id");
                        var value = tar.val();
                        dirtyChecker.setValue(item.key, value);
                        dirtyChecker.checkWatchs(function () {
                            bindBeforeUnload();
                        });
                    });
                } else {
                    tar.bind(event, function () {
                        var id = this.id;
                        var value = this.value;
                        dirtyChecker.setValue(item.key, value);
                        dirtyChecker.checkWatchs(function () {
                            bindBeforeUnload();
                        });
                    });
                }
            });
        });
    }

    var dirtyChecker = new xmsDirtyChecker();
    dirtyChecker.bindCheckerEvent = bindCheckerEvent;
    dirtyChecker.setFormDirtyValue = setFormDirtyValue;

    window.dirtyChecker = dirtyChecker;
    return dirtyChecker;
});