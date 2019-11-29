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
    var filters = new Xms.Fetch.FilterExpression();
    var columnFilters = [];
    var pageFilter = {
        setFilterCallback: setFilterCallback,
        filterColumnNull: filterColumnNull,
        addFilter: addFilter,
        removeFilter: removeFilter,
        submitFilter: submitFilter,
        addFilterItem: addFilterItem,
        customizeFilter: customizeFilter,
        bindColumnFilterStatus: bindColumnFilterStatus,
        emptyGroupFilters: emptyGroupFilters
        , getFilters: function () {
            return filters;
        }
        , getColumnFilters: function () {
            return columnFilters;
        }
        , clearFilters: function () {
            filters = new Xms.Fetch.FilterExpression();
        }
        , clearColumnFilters: function () {
            columnFilters = [];
        }
    }
    function sendFilter(callback) {
        //submitFilter(callback);
        //   $('.datagrid-view').cDatagrid('refreshDataAndView')
    }
    function submitFilter(callback) {
        $('.datagrid-view').cDatagrid('refreshDataAndView')
        //var model = new Object();
        //model.QueryViewId = $('#QueryViewId').val();//$.getUrlParam('queryid');
        //model.Filter = filters;
        //Xms.Web.LoadPage(pageUrl, model, function (response) {
        //    $('#gridview').html($(response).find('#gridview').html());
        //  //  ajaxgrid_reset();
        //    callback && callback();
        //    $('#gridview').trigger('gridview.loaded');
        //    //如果设置已勾选可不受分页影响

        //});
    }
    function addFilterItem(name, _filter) {
        var obj;
        $(columnFilters).each(function (ii, nn) {
            console.log(name)
            if (nn[0] == name) {
                obj = nn[1];
                return;
            }
        });
        if (obj != null) {
            columnFilters = $.grep(columnFilters, function (nn, i) {
                return nn[1] != obj;
            });
            filters.Filters = $.grep(filters.Filters, function (nn, i) {
                return nn != obj;
            });
        }
        var nf = [name, _filter];
        columnFilters.push(nf);
        filters.Filters.push(_filter);
        console.log('addFilterItem', filters.Filters);
    }
    function removeFilter(name, isNotSubmit) {
        var obj;
        $(columnFilters).each(function (ii, nn) {
            if (nn[0] == name || nn[0] == name.toLowerCase()) {
                obj = nn[1];
                return;
            }
        });
        columnFilters = $.grep(columnFilters, function (nn, i) {
            return nn[1] != obj;
        });
        filters.Filters = $.grep(filters.Filters, function (nn, i) {
            return nn != obj;
        });
        if (!isNotSubmit) {
            sendFilter();
        }
    }
    function addFilter(name, filter, isNotSubmit) {
        var obj;
        $(columnFilters).each(function (ii, nn) {
            if (nn[0] == name) {
                obj = nn[1];
                return;
            }
        });
        if (obj != null) {
            columnFilters = $.grep(columnFilters, function (nn, i) {
                return nn[1] != obj;
            });
            filters.Filters = $.grep(filters.Filters, function (nn, i) {
                return nn != obj;
            });
        }
        var nf = [name, filter];
        columnFilters.push(nf);
        // console.log('addFilter.function', filters);

        filters.Filters.push(filter);
        if (!isNotSubmit) {
            sendFilter();
        }
    }
    function customizeFilter(name, dataType, _target) {
        var f = new Xms.Fetch.FilterExpression();
        $(columnFilters).each(function (i, n) {
            if (n[0] == name) {
                f.FilterOperator = n[1].FilterOperator;
                $(n[1].Conditions).each(function (ii, nn) {
                    f.Conditions.push(nn);
                });
            }
        });
        var data = {};
        var $target = $(_target);
        if ((dataType == 'lookup' || dataType == 'customer' || dataType == 'owner') && ~name.indexOf('.')) {//如果为关联字段
            var relationname = name.split('.')[0];
            var referecingentityid = $target.parents('th:first').attr('data-referencedentityid');
            referecingentityid = referecingentityid || $target.attr('data-referencedentityid');
            var referecedentityid = '';
            if (!referecingentityid) return false;
            Xms.Web.GetJson('/api/schema/relationship/GetReferencing/' + referecingentityid + '', null, function (data) {
                var repdatas = data.content;
                console.log('customizeFilter', repdatas);
                $.each(repdatas, function (i, n) {
                    if (n.name.toLowerCase() == relationname.toLowerCase()) {
                        referecedentityid = n.referencedentityid;
                        return false;
                    }
                });
                if (referecedentityid) {
                    data.entityid = referecedentityid;
                    data.field = name;
                    data.datatype = dataType;
                    data.filter = gridview_filters.getFilterInfo();
                    Xms.Web.OpenDialog('/filter/filterdialog', 'pageFilter.setFilterCallback', data, null, null, true);
                }
            });
        } else {
            data.entityid = Xms.Page.PageContext.EntityId;
            data.field = name;
            data.datatype = dataType;
            data.filter = gridview_filters.getFilterInfo();
            Xms.Web.OpenDialog('/filter/filterdialog', 'pageFilter.setFilterCallback', data, null, null, true);
        }
    }
    function bindColumnFilterStatus() {
        $('#datatable').find('thead th[data-name]').find('.glyphicon-filter').remove();
        $(columnFilters).each(function (i, n) {
            var name = n[0];
            var filter = n[1];
            var column = $('#datatable').find('thead th[data-name="' + name + '"]');
            column.attr('data-filter', JSON.stringify(filter));
            column.find('div:first').width(column.find('div:first').width() + 20);
            var flagEl = $('<span class="glyphicon glyphicon-filter"></span>');
            column.find('.dropdown-toggle').find('.glyphicon-filter').remove();
            column.find('.dropdown-toggle').prepend(flagEl);
            $(filter.Conditions).each(function (ii, nn) {
                if (nn.Operator == Xms.Fetch.ConditionOperator.Null) {
                    column.find('a[data-operator=null]').parent().addClass('bg-warning').end().attr('style', 'color:red;');
                }
                if (nn.Operator == Xms.Fetch.ConditionOperator.NotNull) {
                    column.find('a[data-operator=notnull]').parent().addClass('bg-warning').end().attr('style', 'color:red;');
                }
            });
            column.find('.dropdown-menu a:first').removeClass('disabled').bind('click', function () {
                pageFilter.removeFilter(name);
                gridview_filters.removeAllCondition(name);
                $('.datagrid-view').cDatagrid('refreshDataAndView');
                $('.datagrid-view').xmsDatagrid('refreshDataAndView');
            });
        });
    }

    function filterColumnNull(name, isnull) {
        var filter = new Xms.Fetch.FilterExpression();
        var condition = new Xms.Fetch.ConditionExpression();
        filter.FilterOperator = Xms.Fetch.LogicalOperator.And;
        condition.AttributeName = name;
        condition.Operator = isnull ? Xms.Fetch.ConditionOperator.Null : Xms.Fetch.ConditionOperator.NotNull;
        filter.Conditions.push(condition);
        addFilter(name, filter, true);
        gridview_filters.removeAllCondition(name);
        gridview_filters.addFilter(new XmsFilter(filter));
        $('.datagrid-view').cDatagrid('refreshDataAndView');
        $('.datagrid-view').xmsDatagrid('refreshDataAndView');
    }
    function setFilterCallback(name, filter) {
        //console.log(name, filter);
        if (filter && filter.Conditions.length > 0) {
            addFilter(name, filter, true);
            gridview_filters.removeAllCondition(name);
            gridview_filters.addFilter(new XmsFilter(filter));
            $('.datagrid-view').cDatagrid('refreshDataAndView');
            $('.datagrid-view').xmsDatagrid('refreshDataAndView');
        }
    }

    //删除图表添加进去的过滤条件
    function emptyGroupFilters(index) {
        var groupsInserModal = $('#groupsInserModal');
        var chartid = $('#ChartList').find('option:selected').val();
        if (!chartid) return false;
        if (!groupsInserModal.data().groupsCtrl) return false;
        var groupsCtrl = groupsInserModal.data().groupsCtrl[chartid];

        if (groupsCtrl.length > 0) {
            if (index) {
                $.each(groupsCtrl, function (key, item) {
                    if (key > index) {
                        if (item.attributeName) {
                            pageFilter.removeFilter(item.attributeName.toLowerCase(), true);
                            gridview_filters.removeAllCondition(item.attributeName.toLowerCase());
                        }
                    }
                });
            } else {
                $.each(groupsCtrl, function (key, item) {
                    if (item.attributeName) {
                        pageFilter.removeFilter(item.attributeName.toLowerCase(), true);
                        gridview_filters.removeAllCondition(item.attributeName.toLowerCase());
                    }
                });
            }
        }
        $('.datagrid-view').cDatagrid('refreshDataAndView');
        // $('.datagrid-view').xmsDatagrid('refreshDataAndView');
    }

    window.pageFilter = pageFilter;
    return pageFilter
});