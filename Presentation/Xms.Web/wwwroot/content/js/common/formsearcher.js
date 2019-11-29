//@ sourceURL=common/formSearch.js
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

    //page init
    var page_common_formSearcher = {
        init: function () {
        }
        , SearchTip: SearchTip
        , MacthSearch: MacthSearch
        , clearSearchFiler: clearSearchFiler
        , searchByConditionEnter: searchByConditionEnter
        , searchByCondition: searchByCondition
        , setItemFirst: setItemFirst
        , closeSearchC: closeSearchC
        , clearFilters: clearFilters
        , searchKanban: searchKanban
        , closeKanban: closeKanban
        , closeSearchForm: closeSearchForm
    }
    var imme = xmsImmediate(500);//获取缓冲函数
    function closeSearchForm() {
        searchByCondition(true);
        closeSearchC();
    }
    function SearchTip(e, attrid) {
        $(".xms-dropdownSearch-box").removeClass("open");
        imme(function () {
            var listTarget = $(e);
            var thTarget = $(e).parents('th');
            if ($(e).siblings(".xms-dropdownLink").length > 0) {
                $(e).siblings(".xms-dropdownLink").remove();
                $(e).css("color", "#555");
            }
            var searcher = listTarget.data().searcher;
            console.log(listTarget.data().searcher);
            if (!listTarget.data().searcher) {
                searcher = new xmsSearcher(listTarget, {
                    setParentPos: false,

                    addHandler: function (obj, id, par, width) {
                        var tar = $("#" + $(par.obj).attr("id").replace("_text", ""));
                        if ($(par.obj).siblings(".xms-dropdownLink").length > 0) {
                            $(par.obj).siblings(".xms-dropdownLink").remove();
                        }
                        if (tar.length > 0) {
                            tar.val($(obj).children('.xms-dropdownSearch-value').text());
                            tar.attr("data-value", id);
                        }
                        var alink = $('<a target="_blank" href="' + ORG_SERVERURL + '/entity/create?entityid=' + listTarget.attr("data-entityid") + '&recordid=' + id + '" class="xms-dropdownLink" title="' + par.obj.val() + '"><span class="glyphicon glyphicon-list-alt"></span> <span class="xms-drlinki" data-id="" data-value="">' + par.obj.val() + '</span></a>');
                        $(par.obj).css({ "color": "#fff" });

                        $(par.opts.parent).append(alink);
                        var lenW = alink.find(".xms-drlinki").width();
                        if (lenW > width) {
                            lenW = width - 30;
                        }
                        alink.css("width", lenW + 20);

                        $(par.obj).focus();
                    }
                });
                listTarget.data().searcher = searcher;
            }
            searcher = listTarget.data().searcher;
            searcher.ele.addClass("open");
            if (!attrid) {
                searcher.ele.removeClass("open");
                return false;
            }
            Xms.Web.Get('/api/schema/entity/' + attrid, function (data) {
                if (data.StatusName == 'success') {
                    var _value = listTarget.val();
                    var refname = data.content.name;
                    var filter = { "filteroperator": 0, "conditions": [], "filters": [{ "filteroperator": 1, "filters": [], "conditions": [{ "attributename": "name", "operator": 6, "values": [_value] }] }] }
                    var QueryObject = { EntityName: refname, Criteria: filter/*new Xms.Fetch.FilterExpression()*/, ColumnSet: { AllColumns: true }, PageInfo: { PageNumber: 1, PageSize: 15 } };
                    // var QueryObject = { EntityName: refname, Criteria: filter, ColumnSet: { AllColumns: true }, PageInfo: { PageNumber: 1, PageSize: 10 } };
                    Xms.post('/api/data/Retrieve/Multiple', { 'query': QueryObject }, false, function (data2) {
                        console.log('/api/data/Retrieve/Multiple', data2.Content)
                        try {
                            data2.Content = JSON.parse(data2.Content.replace("\n", ""));
                        } catch (e) {
                            console.log(e, '/api/data/Retrieve/Multiple,error');
                        }
                        data2.Content = {};

                        for (var i = 0, len = data2.Content.items.length; i < len; i++) {
                            data2.Content.items[i]['title'] = data2.Content.items[i].name;
                            data2.Content.items[i]['id'] = data2.Content.items[i][refname.toLowerCase() + 'id'];
                        }
                        searcher.removeAll();
                        for (var i = 0, len = data2.Content.items.length; i < len; i++) {
                            searcher.addData(data2.Content.items[i]);
                        }
                        if (searcher.list.length > 0) {
                            searcher.show();
                        } else {
                            searcher.hide();
                        }
                        searcher.render();
                        // MacthSearch(e);
                    }, false, false, false);
                }
            });
        });
        // MacthSearch(e);
    }
    function MacthSearch(e) {
        var val = $(e).val();
        var valL = val.toLowerCase();
        $(e).siblings('.xms-dropdownSearch-box').find('.xms-dropdownSearch-item').each(function (i, n) {
            var itemVal = $(n).find('.xms-dropdownSearch-value').text();
            var itemValL = itemVal.toLowerCase();
            if (itemValL.indexOf(valL) != -1) {
                $(n).show();
            } else {
                $(n).hide();
            }
        });
    }
    function clearSearchFiler(event, isRefresh) {
        clearFilters();
        $('#searchForm input.lookup').attr('data-value', '').val('');
        $('#searchForm .picklist').attr('data-value', '');
        $('#searchForm button[name=resetBtn]').trigger('click');
        closeSearchC();
        gridview_filters.clearAll();
        if (!isRefresh) {
            $('.datagrid-view').cDatagrid('refreshDataAndView');
            $('.datagrid-view').xmsDatagrid('refreshDataAndView');
        }
        if ($('.menu-right').width() > 100) {
            var chartid = $('#ChartList').find('option:selected').val();
            var queryid = $('#QueryId').val();
            renderChart(chartid, queryid, { 'width': '100%', 'height': '300px' });
        }
    }

    function searchByConditionEnter(e) {
        searchByCondition(true);
        closeSearchC();
    }
    function searchByCondition(isfilterForm) {
        var searchFormSearch = $("#searchFormSearch");
        searchFormSearch.find('.seacher-row').each(function (key) {
            var filter = new Xms.Fetch.FilterExpression();
            filter.FilterOperator = Xms.Fetch.LogicalOperator.And;
            var filtername = $(this).attr('data-name');
            var value = $(this).find('input.form-control').val();
            var filtetype = $(this).attr('data-type');
            var obj = null;
            var inList = null;
            var filters = pageFilter.getFilters();
            //删除之前的过滤条件
            if (filters.Filters && filters.Filters.length > 0) {
                filters.Filters = $.grep(filters.Filters, function (nn, i) {
                    return nn.Conditions[0].AttributeName != filtername;
                });
            }

            if (filtername) {
                gridview_filters.removeAllCondition(filtername);
            }
            // console.log(gridview_filters)
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
                    var keywork = $(n).val();
                    if (keywork != '') {
                        var condition = new Xms.Fetch.ConditionExpression();
                        condition.AttributeName = filtername;

                        if (i == 0) {
                            condition.Operator = Xms.Fetch.ConditionOperator.GreaterEqual;
                            if (isfilterForm) {
                                //  keywork = new Date(keywork).format('yyyy-MM-dd');
                                keywork = keywork + ' 00:00:00';
                            }
                        } else {
                            condition.Operator = Xms.Fetch.ConditionOperator.LessEqual;
                            if (!isfilterForm) {
                                keywork = new Date(keywork).DateAdd('d', 1).format('yyyy-MM-dd');
                            } else {
                                keywork = new Date(keywork).format('yyyy-MM-dd');
                                keywork += ' 23:59:59';
                            }
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
                        //  console.log($(n).attr('data-value') == "" && $(n).val() != "");
                        if ((!$(n).attr('data-value') || $(n).attr('data-value') == "") && $(n).val() != "") {
                            condition.Operator = Xms.Fetch.ConditionOperator.Like;
                            condition.AttributeName = filtername + 'name';
                            var keywork = $(n).val();
                        } else {
                            var keywork = $(n).attr('data-value') || "";
                        }

                        if (~keywork.indexOf(",")) {
                            var tempkeyword = keywork.split(',');
                            console.log('condition.' + filtername, tempkeyword);
                            $.each(tempkeyword, function (key, item) {
                                var _itemCondition = $.extend(true, {}, condition);
                                _itemCondition.Values.push(item);
                                filter.Conditions.push(_itemCondition);
                                console.log('condition.' + filtername, _itemCondition);
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
                    if (filtetype == 'nvarchar' || filtetype == 'ntext') {
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
            if (filter.Conditions[0]
                && typeof filter.Conditions[0].Values === "object" && filter.Conditions[0].Values.length
                && filter.Conditions[0].Values[0].replace("\s", "") != ""
                || (filter.Conditions[0] && filter.Conditions[0].Values[1] && filter.Conditions[0].Values[1].replace("\s", "") != "")
            ) {
                gridview_filters.addFilter(new XmsFilter(filter));
                pageFilter.addFilterItem(filtername, filter);
            }
        });
        $('.datagrid-view').cDatagrid('refreshDataAndView');
        $('.datagrid-view').xmsDatagrid('refreshDataAndView');
        $(".xms-formDropDown").trigger("xmsFormDrop.close");
        pageUrl = $.setUrlParam(pageUrl, 'page', 1);
        setItemFirst($("#searchFormSearch"));

        //if (filters.Filters.length > 0) {
        // pageFilter.submitFilter();
        console.log("$('.menu-right')", $('.menu-right').width())
        if ($('.menu-right').width() > 100) {
            var chartid = $('#ChartList').find('option:selected').val();
            var queryid = $('#QueryId').val();
            renderChart(chartid, queryid, { 'width': '100%', 'height': '300px' });
        }
        // }
    }
    function setItemFirst($context) {
        var cusSearch = $context;
        var _parent = cusSearch.parents('.xms-formDropDown:first');
        var items = cusSearch.find(".xms-formDropDown-Item");
        var value = [];
        items.each(function () {
            //var val = $(this).find("input.colinput").val();
            //var itemtype = $(this).attr("data-type");
            //if (itemtype == "picklist" || itemtype == 'state' || itemtype == 'bit') {
            //    val = $(this).find("select>option:selected").text();
            //}
            //if (!val || val == "") {
            //    return true;
            //}
            var type = $(this).attr("data-type");
            var label = $(this).find("label").text();
            var str = label + ":", flag = true;
            if (type == "datetime" || type == "int" || type == 'decimal' || type == 'float' || type == 'money') {
                var startT = $(this).find("input:first");
                var endT = $(this).find("input:last");
                if (startT.val().replace("\s", "") != "" && endT.val().replace("\s", "") != "") {
                    str += startT.val();
                    str += "-";
                    str += endT.val();
                } else if (startT.val().replace("\s", "") != "" && endT.val().replace("\s", "") == "") {
                    str += startT.val();
                } else if (startT.val().replace("\s", "") == "" && endT.val().replace("\s", "") != "") {
                    str += endT.val();
                }
                else {
                    flag = false;
                }
            } else if (type == "picklist" || type == 'state' || type == 'bit') {
                if ($(this).find("select>option:selected").text()) {
                    str += $(this).find("select>option:selected").text();
                }
                else {
                    flag = false;
                }
            } else {
                if ($(this).find("input.colinput").val()) {
                    str += $(this).find("input.colinput").val();
                }
                else {
                    flag = false;
                }
            }
            if (flag == true) value.push(str);
        });
        if (!value || value.length == 0) {
            $(".xms-formDownInput", _parent).html('<span class="glyphicon glyphicon-filter"></span>').prop('title', "");
            return false;
        }
        console.log(value);
        $(".xms-formDownInput", _parent).html(value.join(";")).prop('title', value.join(";"));
    }

    function closeSearchC() {
        $(".xms-formDropDown").trigger("xmsFormDrop.close");
    }
    function clearFilters() {
        $('#Q').val('');
        pageFilter.clearFilters();
        // pageFilter.submitFilter();
        var _parent = $("#searchFormSearch").parents('.xms-formDropDown:first');
        $(".xms-formDownInput", _parent).text('<span class="glyphicon glyphicon-filter"></span>').prop('title', '');
        $('#dayfilters').find('button').removeClass("active");
        $("#searchFormSearch").find("a.xms-dropdownLink").remove();
    }
    function searchKanban() {
        var viewid = $('#viewSelector').attr('data-value');
        var kanbanSearch = $("#kanbanSearch");
        var aggregateField = Xms.Web.SelectedValue($('#aggregateField')),
            groupField = Xms.Web.SelectedValue($('#groupField'));
        if (!aggregateField || !groupField) {
            Xms.Web.Toast('请指定统计字段和分组字段', false);
            return;
        }
        var url = '/entity/kanbanview?queryid=' + viewid + '&aggregatefield=' + aggregateField + '&groupfield=' + groupField;

        Xms.Web.Load(url, function (response) {
            //console.log(response);
            if (typeof response == 'object') {
                //$('#content').text(response.Content);
                Xms.Web.Toast(response.Content, 'error', false);
                return;
            }
            var $contentEl = $('#kanbanview')
            $contentEl.html(response);
            //默认第一列为快速查找字段
            $('#fieldDropdown').next().find('a:eq(1)').trigger('click');
            var entityid = $('#EntityId').val();
            //  changeTableHeight();
            if (parent && parent != window) {
                if (window['bodyInited']) window['bodyInited']();
            }
            //  $('#gridview').trigger('gridview.inited');
            //  pageWrap_Gridview.init();
            $('#kanbanview').removeClass('hide');
            //alert(123);
            pageWrap_list.reSetTopStyle();
            setItemFirst(kanbanSearch);
        });
        // pageWrap_list.loadData(url, $('#kanbanview'), function () {
        // });
    }
    function closeKanban() {
        $(".xms-formDropDown").trigger("xmsFormDrop.close");
    }

    window.page_common_formSearcher = page_common_formSearcher;
    return page_common_formSearcher;
});