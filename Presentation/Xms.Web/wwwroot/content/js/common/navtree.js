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
    if (typeof Xms === 'undefined') {
        throw Error('Xms不能为空， 请先引用xms.js文件 ');
    }

    var common_navtree = {
        renderGridViewTree: function (item, entityname, parent, attrname, sortby, data) {
            treeFilterGridView(item, entityname, parent, attrname, sortby, data);
        }
    }

    function getTreeData(opts, callback) {
        var url = opts.url;
        var data = opts.data;
        console.log(url, data);
        Xms.Web.GetJson(url, data, function (response) {
            console.log('response', response);
            callback && callback(response, parent);
        }, null, null, 'post');
    }

    function handlerTreeData(response, entityname, parent) {
        var datas = [], resDatas = [];
        if (response.content && response.content.length > 0) {
            datas = response.content;
        } else if (response.content.items && response.content.items.length > 0) {
            datas = response.content.items;
        }
        entityname = entityname.toLowerCase();
        parent = parent.toLowerCase();
        $.each(datas, function (key, item) {
            var itemdata = {
                "id": item[entityname + 'id'],
                "text": item['name'],
                "parentid": item[parent],
                "value": item[parent]
            }
            resDatas.push(itemdata);
        });
        return resDatas;
    }

    function getTreeTools(opts, xmsTree, context) {
        var defsbtns = [{ 'title': '全部展开', handler: 'expandAll' }, { 'title': '全部缩回', handler: 'closeAll' }];
        var _tools = $.extend([], [], opts.btns || {});//已忽略添加按钮
        var $toolbar = $('<div class="xms-tree-toolbar"></div>');
        $.each(_tools, function (key, item) {
            var $item = $('<div class="xms-tree-toolbtn btn btn-xs btn-default">' + item.title + '</div>');
            $item.off().on('click', function () {
                xmsTree.xjqTree(item.handler);
            });
            $toolbar.append($item);
        });
        return $toolbar;
    }

    /*
     @opts  { entityname: entityname, parent: parentname, attrname: attrname, itemClick: itemClick, sortby: sortby, isDefaultClick: isDefaultClick, treesort: treesort, ordertype: ordertype };
     @loadTreeAfter
    */
    function _renderTree(opts, loadTreeAfter, preRender, rendered, setPostOpts) {
        var url = '/api/data/Retrieve/Multiple';
        preRender && preRender();
        if (!opts.entityname || !opts.parent || !opts.attrname) {
            throw Error("entityname,parent,attrname 不能为空");
        }
        console.log(opts);
        var entityname = opts.entityname.toLowerCase();
        var parent = opts.parent.toLowerCase();
        var attrname = opts.attrname.toLowerCase();
        var sortby = opts.sortby || 'name';
        var sorttype = opts.ordertype || "Ascending";
        var treesort = opts.treesort;
        var isDefaultClick = typeof opts.isDefaultClick == 'undefined' ? true : opts.isDefaultClick;
        var fil = opts.filter || [];
        var filter = { "Conditions": fil };
        var order = [{ "AttributeName": treesort, "OrderType": sorttype }];
        if (treesort) {
            var queryObj = { EntityName: entityname, Orders: order, Criteria: filter, ColumnSet: { allcolumns: true } };
        } else {
            var queryObj = { EntityName: entityname, Criteria: filter, ColumnSet: { allcolumns: true } };
        }
        var postdata = { "query": queryObj, "isAll": true };
        setPostOpts && setPostOpts(postdata);
        console.log('postdata', postdata);
        postdata = JSON.stringify(postdata);
        var resD = {};
        resD.url = url;
        // resD.sortby = sortby;
        resD.data = postdata;
        var xmsTree = null;
        getTreeData(resD, function (response) {
            //左边树导航
            var data = handlerTreeData(response, entityname, parent);
            console.log('handlerTreeData', JSON.stringify(data));
            var source =
            {
                datatype: "json",
                datafields: [
                    { name: 'id' },
                    { name: 'parentid' },
                    { name: 'text' },
                    { name: 'value' }
                ],
                id: 'id',
                localdata: data
            };
            var dataAdapter = new dataApax(source);
            dataAdapter.dataBind();
            console.log(dataAdapter.getRecords())
            var records = dataAdapter.getRecordsHierarchy('id', 'parentid', 'items', [{ name: 'text', map: 'label' }]);
            console.log('tree.records', records);
            xmsTree = $('#xms-table-navtree');
            var offsetHeight = 0;
            xmsTree.xjqTree({ source: records, width: '150px', height: $('#xms-gridview-section').outerHeight() - offsetHeight, theme: 'bootstrap' });
            var close = $('<div class="xms-navtree-close">x</div>');
            var _tools = getTreeTools(opts, xmsTree);
            if (_tools.children().length > 0) {
                xmsTree.prepend(_tools);
            }
            //设置tools高度
            //var toolsH = _tools.outerHeight();
            //xmsTree.css('padding-top', toolsH);
            xmsTree.append(close);
            var currentFilterName = '';
            close.on('click', function () {
                $('#xms-gridview-section').removeClass('xms-table-showtree');
                if (currentFilterName != '') {
                    pageFilter.removeFilter(currentFilterName);
                }
            });
            xmsTree.off().on('itemClick', function (event, obj) {
                var target = obj.target;
                var id = $(target).attr('data-id');
                console.log(id);
                var item = xmsTree.xjqTree('getItem', id);
                console.log(value);
                var value = item.value;
                if (isDefaultClick) {
                    currentFilterName = attrname;
                    treeFilterGridView(item, entityname, parent, attrname, sortby, data);
                }
                opts.itemClick && opts.itemClick(item, entityname, parent, attrname, sortby);
                console.log('itemValue', value);
            });
            console.log(loadTreeAfter);
            loadTreeAfter && loadTreeAfter(xmsTree);
            if (typeof pageUrl !== 'undefined') {
                //  pageUrl = $.setUrlParam(pageUrl, 'sortby', sortby);
                // pageUrl = $.setUrlParam(pageUrl, 'sortdirection', 0);
            }
            pageFilter.submitFilter();
        });
    }

    function renderLeftTree(opts, loadTreeAfter) {
        _renderTree(opts, loadTreeAfter, function () {
            $('#gridview').css('z-index', 0);//防止树被遮挡,无法点击
            $('#xms-gridview-section').addClass('xms-table-showtree');
        }, function () {
        });
    }

    function getChildsData(data, id) {
        var maxData = 20;
        var count = 0;
        var res = [];
        //console.log(id);
        $.each(data, function (key, item) {
            if (res.length >= maxData) {
                return false;
            }
            if (item.parentid == id) {
                res.push(item);
                //getInerChilds(item);
            }
        });
        console.log(res);
        console.table(data);
        while (res.length < maxData && count < 10) {
            count++;

            $.each(res, function (key, item) {
                if (res.length >= maxData) {
                    return false;
                }
                $.each(data, function (i, n) {
                    if (res.length >= maxData) {
                        return false;
                    }
                    if (n.parentid === item.id && !checkInRes(n, res)) {
                        res.push(n);
                    }
                });
            });
        }
        return res;
    }

    function checkInRes(item, res) {
        var flag = false;
        $.each(res, function (i, n) {
            if (item.id == n.id) {
                flag = true;
                return false;
            }
        });
        return flag;
    }

    function treeFilterGridView(item, entityname, parent, attrname, sortby, data) {
        //console.table(data);
        var filterdata = getChildsData(data, item.id);
        var filter = new Xms.Fetch.FilterExpression();
        var condition = new Xms.Fetch.ConditionExpression();
        filter.FilterOperator = Xms.Fetch.LogicalOperator.Or;
        condition.AttributeName = attrname;
        condition.Operator = Xms.Fetch.ConditionOperator.In;
        condition.Values[0] = item.id;
        console.table(filterdata)
        if (filterdata.length > 0) {
            $.each(filterdata, function (key, item) {
                condition.Values.push(item.id);
            });
        }
        filter.Conditions.push(condition);
        pageFilter.addFilter(attrname, filter);
        gridview_filters.removeAllCondition(attrname);
        gridview_filters.addFilter(new XmsFilter(filter));
        if (typeof pageUrl !== 'undefined') {
            // pageUrl = $.setUrlParam(pageUrl, 'sortby', sortby);
            //  pageUrl = $.setUrlParam(pageUrl, 'sortdirection', 0);
        }
        pageFilter.submitFilter();
    }
    window.renderLeftTree = renderLeftTree;
    window.common_navtree = common_navtree;
    return common_navtree
});