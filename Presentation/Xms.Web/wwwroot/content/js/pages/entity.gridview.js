//@ sourceURL=page/entity.gridview.js
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
    //this page url  /content//js/pages/entity.gridview.js
    /*
    * deps
    *  /common/filters,
    *  /pages/entity.list.js
     *  /common/charts.js
     */
    var insertChart = common_charts.insertChart;
    var GetChartList = common_charts.GetChartList;

    //page init
    var chartid = "", queryid = "";
    var pageWrap_Gridview = {
        init: function () {
            $(function () {
                // 过滤判断页面宽度
                if (parent != window) {
                    resizeWin();
                }

                //page_Common_Info只可以在页面定义
                if (typeof page_Common_Info !== 'undefined') {
                    //添加面包屑
                    if ($('.breadcrumb > li:not(.pull-right)').length <= 1) {
                        $('.breadcrumb').append('<li><a href="' + page_Common_Info.breadcrumb_url + '">' + page_Common_Info.breadcrumb_preName + '</a></li>');
                        $('.breadcrumb').append('<li><a href="' + ORG_SERVERURL + '/entity/list?queryviewid=' + page_Common_Info.queryId + '">' + page_Common_Info.queryName + '</a></li>');
                        $('title').prepend(page_Common_Info.queryName + ' - ');
                    }
                }

                if ($('#QField').val() != '')
                    $('#qfield-selector').find('a[data-name="' + $('#QField').val() + '"]').trigger('click');
                else
                    //默认第一列为快速查找字段
                    $('#fieldDropdown').next().find('a:eq(1)').trigger('click');

                GetChartList();
                $(".picklist").each(function (i, n) {
                    var $this = $(n);
                    var type = $this.attr("data-name");
                    try {
                        var items = JSON.parse(decodeURIComponent($this.attr('data-items')));
                        $(this).picklist({
                            items: items
                        });
                    } catch (e) {
                        console.log('picklist', decodeURIComponent($this.attr('data-items')), e);
                    }
                });

                $(".xms-formDropDown").xmsFormDrop({
                    noHidePlace: ".modal,.datepicker,#listAlignTop",
                    Event: "click",
                    Selecter: ".btn-group"
                });
                if (transitionEnd) {
                    $("#entityCreateSection").bind(transitionEnd, function (e) {
                        if ($("body").hasClass("rightIframe-open")) {
                            $("#entityCreateSection").removeClass("end");
                        } else {
                            $("#entityCreateIframe").attr("src", '');
                            $("#entityCreateSection").addClass("end");
                        }
                    });
                }
                //添加列统计信息
                aggFildeName();

                pageWrap_Gridview.bindEvent();
            });
        }
        , bindEvent: function () {
            $('.btn-prevent').off('click').on('click', function (event) {
                event.stopPropagation();
                $(this).next('ul').toggle();
            });

            $('#ChartList').off('change').on('change', function () {
                chartid = $(this).find('option:selected').val();
                queryid = Xms.Page.PageContext.QueryId;
                var groupsInserModal = $('#groupsInserModal');
                if (!groupsInserModal.data().groupsCtrl || !groupsInserModal.data().groupsCtrl[chartid]) {
                    if (chartid && queryid) {
                        var $silderRightCrumb = $('.silder-right-crumb');
                        $silderRightCrumb.empty();
                        // filters = new Xms.Fetch.FilterExpression();
                        // pageFilter.submitFilter();
                        rebind();
                    }
                    renderChart(chartid, queryid);
                } else {
                    groupsInserModal.data().groupsCtrl[chartid] = [];
                    groupsInserModal.data().groups[chartid] = [];
                    if (chartid && queryid) {
                        pageFilter.emptyGroupFilters();
                        //renderChartCrumb();
                        var $silderRightCrumb = $('.silder-right-crumb');
                        $silderRightCrumb.empty();
                        // filters = new Xms.Fetch.FilterExpression();
                        // pageFilter.submitFilter();
                        rebind();
                    }
                    renderChart(chartid, queryid);
                }
            });

            $('#qfield-selector').off('click').on('click', 'a', function () {
                $('#QField').val($(this).attr('data-name'));
                $('#fieldDropdown').find('span:first').text($(this).text());
            });
            $(".xms-slider-ctrl").off('click').on("click", function () {
                var groupsInserModal = $('#groupsInserModal');
                var groupsCtrl = groupsInserModal.data().groupsCtrl;
                var groups = groupsInserModal.data().groups;
                var $this = $(this), active = $this.attr("active");
                console.log(active)
                if (active == "1") {
                    $this.attr("active", "2");
                    $(".xms-table-section").css("right", 35);
                    $(".menu-md.menu-right").css({ "width": 0, "border": "none" });
                    $("#listchartWinBtn").attr("active", "2");
                    $this.find(".glyphicon").removeClass("glyphicon-arrow-right");
                } else {
                    var fixheight = 100;
                    $('.menu-right').height($('#xms-table-section').find('.panel-default').height() - fixheight + 'px')
                    $(".xms-table-section").css("right", 35);
                    $this.attr("active", "1");
                    //重置之前的钻取数据
                    if (chartid && queryid) {
                        pageFilter.emptyGroupFilters();
                        groupsCtrl = [];
                        groups = [];

                        groupsCtrl.push({
                            chartid: chartid,
                            queryid: queryid,
                            filters: new Xms.Fetch.FilterExpression()
                        })
                        renderChart(chartid, queryid, { 'width': '100%', 'height': '300px' });
                        // renderChartCrumb();
                        pageFilter.submitFilter();
                    }
                    $(".menu-md.menu-right").css({ "width": 367, "border": "1px solid #ccc" });
                    $(".xms-fixed-slider").find(".glyphicon").addClass("glyphicon-arrow-right");
                }
                groupsInserModal.removeClass('active');
            });

            $("#listchartWinBtn").off('click').on("click", function () {
                var $this = $(this), active = $this.attr("active");
                if (active == "1") {
                    $(".xms-table-section").css("right", 35);
                    $this.attr("active", "2");
                    $(".menu-md.menu-wrap").css("width", 367);
                    renderChart(chartid, queryid, { 'width': '100%', 'height': '300px' });
                } else {
                    $(".xms-table-section").css("right", 35);
                    $this.attr("active", "1");
                    $(".menu-md.menu-wrap").css("width", 600);
                    renderChart(chartid, queryid, { 'width': '600px', 'height': '300px' });
                }
            });

            $(".entityCreateSection-close").click(function () {
                closeRecordIframe();
            });

            $('.searchLookup').keyup(function (e) {
                var _this = $(this)
                if (e.keyCode == 68) {
                    _this.attr('data-value', '');
                }
                page_common_formSearcher.SearchTip(this, _this.attr('data-referencedentityid'));
            });

            $('#listAlignStyle>.btn').off('click').on('click', function alignStyle() {
                var $this = $(this), type = $this.attr('data-type');
                $this.siblings('.btn').removeClass('active').end().addClass('active');

                var viewid = $('#viewSelector').attr('data-value');
                var url = '';
                if (type == 'top') {
                    $('.kanban-filter').removeClass('hide');
                    $('.filter-section').addClass('hide');
                    $('.date-filter-section').addClass('hide');
                    $('#kanbanview').addClass('hide');
                    $('.xms-fixed-slider').addClass('hide');
                    $('#xms-gridview-section').addClass('hide');
                    $('#kanbanSearch').addClass('in');
                    if ($('#xms-table-section').length > 0) { $('#xms-table-section').empty(); }
                } else {
                    $('.kanban-filter').addClass('hide');
                    $('.filter-section').removeClass('hide');
                    $('.date-filter-section').removeClass('hide');
                    $('.xms-fixed-slider').addClass('show');
                    $('#xms-gridview-section').addClass('show');
                    url = '/entity/gridview?queryviewid=' + viewid;
                    var enabledviewselector = $.getUrlParam('isenabledviewselector', location.href);
                    if (enabledviewselector) {
                        url += '&isenabledviewselector=false';
                    }
                    pageWrap_list.loadData(url);
                }
            });
            $('#searchForm').off().on('keyup', function (e) {
                if (e.keyCode == '13') {
                    searchByConditionEnter();
                }
            });

            $(".lookup").siblings("span").find(".ctrl-search").click(function () {
                var $this = $(this);
                var type = $this.attr("data-name");
                var inreferencedentityid = $this.parents("span:first").siblings("input").attr("data-referencedentityid");
                var inputid = $this.parents("span:first").siblings("input").attr("id");
                var $input = $this.parents("span:first").siblings("input");
                if (!inputid) {
                    inputid = "lookup_" + new Date() * 1;
                    $input.attr("id", inputid);
                }
                var lookupurl = '/entity/RecordsDialog?entityid=' + inreferencedentityid + '&singlemode=true&inputid=' + inputid;
                if ($this.attr('data-defaultviewid')) {
                    lookupurl = $.setUrlParam(lookupurl, 'queryid', $this.attr('data-defaultviewid'));
                }
                Xms.Web.OpenDialog(lookupurl, "selectRecordCallback", null, function () {
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
                page_common_formSearcher.SearchTip(input);
            });
        }
    }

    function resizeWin() {
        if (parent != window) {
            if ($(parent.document.body).length > 0 && $(parent.document.body).width() <= 768) {
                $('.custom-input-hide').hide();
            } else {
                $('.custom-input-hide').show();
            }
        }
    }
    function renderChart(chartid, queryid, opts, postData) {
        insertChart(chartid, queryid, opts, postData, function (dHtml) {
            $('#viewCharts').html(dHtml);
        });
    }

    function aggFildeName() {
        var _AggregateTypeList = { '_1': '合计：', '_2': '平均值：', '_3': '最大值：', '_4': '最小值：' };
        var $AggregateFields = $('#AggregateFields');
        var _val = $AggregateFields.val(), tempArr = [];
        if (_val && _val != "" && _val != "null") {
            tempArr = JSON.parse(_val);
            $.each(tempArr, function (key, item) {
                var _name = item.attributename.toLowerCase();
                if (_name) {
                    var $aggtypename = $('.agg-type-name[data-name="' + _name + '"]');
                    // console.log(item);
                    if ($aggtypename.length > 0) {
                        $aggtypename.text(_AggregateTypeList['_' + item.aggregatetype]);
                    }
                }
            });
        }
    }

    function selectRecordCallback(result, inputid) {
        console.log(result);
        var input = $('#' + inputid);
        var rName = [];
        var rId = [];
        for (var i = 0; i < result.length; i++) {
            rName.push(result[i].name);
            rId.push(result[i].id);
        }
        console.log(rName);
        input.val(rName.join(','));
        input.attr('data-value', rId.join(','));
        input.parents('.container-fluid').find('.colconfirm').click();
        setLookUpState(input);
    }
    function setLookUpState(obj) {
        var val = $(obj).val();
        //if (val == "") return true;
        if ($(obj).siblings(".xms-dropdownLink").length > 0) {
            $(obj).siblings(".xms-dropdownLink").remove();
        }
        var tar = $("#" + $(obj).attr("id").replace("_text", ""));
        var id = tar.attr("data-value");
        var values = tar.val();
        if (!~id.indexOf(',')) {
            var alink = $('<a target="_blank" href="' + ORG_SERVERURL + '/entity/create?entityid=' + $(obj).attr("data-entityid") + '&recordid=' + id + '" class="xms-dropdownLink" title="' + $(obj).val() + '"><span class="glyphicon glyphicon-list-alt"></span> <span class="xms-drlinki" data-id="" data-value="">' + $(obj).val() + '</span></a>');
        } else {
            var arrids = id.split(',');
            var arrvalues = values.split(',');
            var alink = $('<a target="_blank" href="' + ORG_SERVERURL + '/entity/create?entityid=' + $(obj).attr("data-entityid") + '&recordid=' + arrids[0] + '" class="xms-dropdownLink" title="' + $(obj).val() + '"><span class="glyphicon glyphicon-list-alt"></span> <span class="xms-drlinki" data-id="" data-value="">' + $(obj).val() + '</span></a>');
        }
        $(obj).parent().append(alink);
        $(obj).css({ "color": "#fff" });
        console.log(alink);
        var width = $(obj).width();
        var lenW = alink.find(".xms-drlinki").width();
        if (lenW > width) {
            lenW = width - 30;
        }
        alink.css("width", lenW + 20);
        $(obj).focus();
    }

    function closeRecordIframe() {
        entityIframe('hide');
        if (typeof entityCreateIframe !== 'undefined') {
            try {
                var _iframe = entityCreateIframe.window;//document.frames('entityCreateIframe');
                var unbindFun = _iframe.unBindBeforeUnload;
                if (unbindFun && $.isFunction(unbindFun)) {
                    unbindFun();
                }
            } catch (e) { }
        }
    }

    function entityIframe(type, url) {
        var top = 65;//$("#entityCreateIframe").offset().top;
        var height = $(window).height() - top;
        $("#entityCreateIframe").height(height - 20);
        $("#entityCreateSection").height(height);
        if (type == 'show') {
            //  $("#entityCreateSection").show();
            $("body").addClass("rightIframe-open").removeClass("rightIframe-close");
            $("#entityCreateIframe").attr("src", url);
        } else {
            $("body").addClass("rightIframe-close").removeClass("rightIframe-open");
            // $("#entityCreateSection").hide()
        }
    }

    function DayQuery(e) {
        var $this = $(e);
        if ($this.is('.active')) {
            pageFilter.removeFilter('createdon');
            $this.removeClass("active").focusout();
            return;
        }
        $this.siblings().removeClass("active").end().addClass("active");
        var setDate = $(e).attr('data-day');
        var filter = new Xms.Fetch.FilterExpression();
        var condition = new Xms.Fetch.ConditionExpression();
        filter.FilterOperator = Xms.Fetch.LogicalOperator.And;
        condition.AttributeName = 'createdon';
        condition.Operator = Xms.Fetch.ConditionOperator.GreaterEqual;
        condition.Values.push(setDate);
        filter.Conditions.push(condition);

        console.log(gridview_filters)
        pageFilter.addFilter('createdon', filter, true);
        gridview_filters.removeAllCondition('createdon');
        gridview_filters.addFilter(new XmsFilter(filter));
        $('.datagrid-view').cDatagrid('refreshDataAndView');
        $('.datagrid-view').xmsDatagrid('refreshDataAndView');
        if ($('.menu-right').width() > 100) {
            var chartid = $('#ChartList').find('option:selected').val();
            var queryid = $('#QueryId').val();
            renderChart(chartid, queryid, { 'width': '100%', 'height': '300px' });
        }
    }
    function EditRecord(newWindow, obj) {
        var event = event || window.event || (arguments && arguments.callee && arguments.callee.caller && arguments.callee.caller.arguments[0]);
        var target = $('#datatable');
        if (event) {
            Xms.Web.SelectingRow(event.target, false, true);
        } else {
            Xms.Web.SelectingRow(obj, false, true);
        }
        var id = Xms.Web.GetTableSelected(target);
        if ((!id || id.length == 0) && Xms.Web.GetSelectingRowRecordId) {
            id = Xms.Web.GetSelectingRowRecordId(event.target);
        }
        var url = ORG_SERVERURL + '/entity/edit?entityid=' + Xms.Page.PageContext.EntityId + '&recordid=' + id;
        if (newWindow) {
            entityIframe('show', url);
        }
        else {
            location.href = url;
        }
    }

    window.renderChart = renderChart;

    window.entityIframe = entityIframe;
    window.pageWrap_Gridview = pageWrap_Gridview;
    window.selectRecordCallback = selectRecordCallback;
    window.EditRecord = EditRecord;
    window.DayQuery = DayQuery;
    var page_modules = pageWrap_Gridview;
    return page_modules;
});