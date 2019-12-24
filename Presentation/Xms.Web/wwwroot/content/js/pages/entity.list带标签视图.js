//@ sourceURL=page/entity.list.js
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
    //deps xmsMutilCheckbox
    //pageFilter

    var insertChart = common_charts.insertChart;
    var GetChartList = common_charts.GetChartList;

    //page init
    var chartid = "";

    var filters = pageFilter.getFilters();
    var columnFilters = pageFilter.getColumnFilters();
    var entityid;
    var table_fozone = false;//设置列表表头是否冻结；
    var datas = {
        relationshipname: '',
        isloadAttribute: false,
        isloadChart: false,
        isLoadFormSearch: false,
        isloadAggInfo: false,
        aggInfo: '',
        oldFilterInfo: '',
        // oldaggInfo: '',
        oldAggInfo: '',
        queryviews: null,
        entityId: '',
        queryId: '',
        layoutconfig: '',
        fetchconfig: '',
        name: '',
        aggregateconfig: '',
        oldaggregateconfig: '',
        globtnstr: '',
        freezenIndex: 2,
        listDatas: [],
        gridviewItemBtnstmpl: ''//行按钮
        , inlinebtnlength: 0
        , jslibs: []//视图使用到的Web资源
        , attributesInfo: []
        , setAttributesHide: []//隐藏的字段
        , setAttributesShow: []//显示的字段
        , breadcrumbinfos: {
            entityname: '',
            queryviewname: ''
            , nonereadfields: []
            , entitystabs: []
        }
    }
    var pageWrap = {
        init: function () {
            $(function () {
                filters.FilterOperator = Xms.Fetch.LogicalOperator.And;
                // loadData(pageUrl);
                Xms.Web.Event.subscribe('refresh', function (e) {
                    rebind();
                });
                console.log(pageUrl);
                $('#main').css('margin-bottom', 0);
            });
            Xms.Web.Loading();
            Xms.Web.Loader();
            pageWrap.loadPageInfo();
            pageWrap.bindOnceEvent();
        },
        loadPageInfo: function () {
            pageWrap.loadQueryViews();
        },
        _getQueryViews: function (datas) {
            var htmls = [];
            $.each(datas, function (i, n) {
                htmls.push('<li><a href="javascript:void(0)" data-value="' + n.queryviewid + '" title="' + n.name + '">' + n.name + '</a></li>');
            });
            return htmls.join('');
        },
        _getDefaultViewKey: function (datas) {
            var index = 0;
            $.each(datas, function (i, n) {
                if (n.isdefault == true) {
                    index = i;
                    return false;
                }
            });
            return index;
        },
        _filterEnableViews: function (datas) {
            return $.grep(datas, function (n, i) {
                if (!n.isdisabled) {
                    return true;
                }
            });
        },
        createQueryViewSetting: function () {
            var attrs = datas.attributesInfo;
            var $querySettingWrapHide = $('#querySettingWrapHide');
            var $querySettingWrapShow = $('#querySettingWrapShow');
        },
        loadQueryViews: function (type, value, callback) {
            $('body').trigger('queryview.prevLoad');
            //加载
            var $viewSelector = $('#viewSelector');
            var url = ORG_SERVERURL + '/api/schema/queryview/GetViewInfo?';
            if (!type) {
                if (entityname) {
                    url += 'entityname=' + entityname;
                }
                if (!entityid) {
                    entityid = $.getUrlParam('entityid') || '';
                }
                if (entityid) {
                    if (url.indexOf('entityname') != -1) {
                        url += '&'
                    }
                    url += 'entityid=' + entityid;
                }
                if (queryid) {
                    if (url.indexOf('entityname') != -1 || url.indexOf('entityid') != -1) {
                        url += '&';
                    }
                    url += 'id=' + queryid;
                }
            } else {
                url += type + '=' + value;
            }
            var relationship = $.getUrlParam('relationshipname');
            if (relationship) {
                datas.relationshipname = relationship;
            }
            Xms.Web.Get(url, function (res) {
                console.log('getbyentityid', JSON.parse(res.Content));
                var jsonres = datas.jsonres = JSON.parse(res.Content);
                datas.queryviews = jsonres.views;
                console.log(res);
                if ($viewSelector.length > 0) {
                    var isenabled = $viewSelector.attr('data-isenabled');
                    if (isenabled == 1) {
                        if (datas.queryviews && datas.queryviews.length > 0) {
                            //只加载启用的
                            datas.queryviews = pageWrap._filterEnableViews(datas.queryviews);
                            if (datas.queryviews && datas.queryviews.length == 1) {
                                if ($viewSelector.length > 0) {
                                    var isenabled = $viewSelector.attr('data-isenabled');
                                    if (isenabled == 1) {
                                        if (datas.queryviews) {
                                            //只加载启用的
                                            //  datas.queryviews = pageWrap._filterEnableViews(datas.queryviews);
                                            //设置页面信息,第一次加载时默认第一个为当前的视图
                                            // var index = pageWrap._getDefaultViewKey(datas.queryviews);
                                            pageWrap.setPageInfo(0, datas.queryviews[0], jsonres);
                                            pageWrap.loadQueryViewInfo(null, jsonres);
                                        }
                                    }
                                }
                            } else if (datas.queryviews && datas.queryviews.length > 1) {
                                //设置页面信息,第一次加载时默认第一个为当前的视图
                                var index = pageWrap._getDefaultViewKey(datas.queryviews);
                                pageWrap.setPageInfo(index, null, jsonres);
                                pageWrap.loadQueryViewInfo(null, jsonres);
                            }
                        }
                    }
                }
            });
            return false;
        },
        loadSingleQueryview: function (_queryid, $context, callback, _filter) {
            Xms.Web.Get(ORG_SERVERURL + '/api/schema/queryview/GetViewInfo?id=' + _queryid, function (res) {
                console.log('loadSingleQueryview', JSON.parse(res.Content));
                var _datas = {}// $.extend({}, datas);

                var content = JSON.parse(res.Content);
                var view = content.views[0];
                _datas.entityId = view.entityid;
                _datas.queryId = _queryid;
                _datas.layoutconfig = view.layoutconfig.toLowerCase();

                _datas.entitystabs = '';
                _datas.setConfig = function (gridconfig) {
                    gridconfig.refresh = null;
                    gridconfig.height = 300;
                    gridconfig.rowDblClick = null;
                    gridconfig.rowClick = null;
                    gridconfig.refreshDataAndView = null;
                    gridconfig.columnFilter = null;
                    gridconfig.refreshDataAndViewed = null;

                    gridconfig.extend = function (datagrid) {
                        $.extend(datagrid.opts.dataModel, {
                            isJsonAjax: true,

                            filterSendData: function (postData, objP, DM, PM, FM) {
                                //console.log('postdata', postData, objP, DM, PM, FM)
                                var objdata = { sortby: objP.dataIndx, sortdirection: objP.dir == 'up' ? '0' : '1', pagesize: PM.rPP }
                                if (_filter) {
                                    objdata.filter = _filter;
                                }
                                $.extend(postData, objdata);
                                return postData;
                            }
                        })
                    }
                };
                //   pageWrap.loadAttributes(function (res) {
                if (content.attributes) {
                    $.each(content.attributes, function () {
                        if (this.name != '') {
                            this.name = this.name.toLowerCase();
                        }
                        //  Xms.Web.getAttributePlug(this);
                    });
                }
                _datas.attributesInfo = pageWrap.filterAttributes(content.attributes, _datas);
                _datas.isloadAttribute = true;
                _datas.forzenHeight = '400';
                _datas.filter = _filter;
                loadDataTable($context, true, _datas);
                //  }, _datas);

                // callback && callback();
            });
        },
        loadAttributes: function (callback) {
            datas.attributesInfo = response.content;
            datas.isloadAttribute = true;
            callback && callback();
        },
        loadAggregate: function (callback) {
        },
        filterAttributes: function (items, datas) {
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
                            if (attrs[1] == item.name) {
                                item.name = n.name;
                                item.localizedname = n.label ? n.label : item.label ? item.label : item.localizedname + '(' + item.entitylocalizedname + ')' ? item.localizedname : item.entitylocalizedname//item.label; //+ '(' + item.entitylocalizedname + ')';
                                item.width = n.width;
                                tar = item;
                                return false;
                            }
                        } else {
                            if (item.name == n.name) {
                                item.width = n.width;
                                item.localizedname = n.label ? n.label : item.label ? item.label : item.localizedname ? item.localizedname : item.entitylocalizedname
                                tar = item;
                                return false;
                            }
                        }
                    });
                    if (tar) {
                        $.extend(n, tar, n);
                    }
                    n.editable = false;
                });
                return layoutitems;
            } else {
                return items
            }
        },
        loadQueryViewInfo: function (callback, jsonres) {
            $('body').trigger('queryview.loading');
            page_common_formSearcher.clearSearchFiler(null, true);
            gridview_filters.clearAll();

            //加载列表按钮
            pageWrap.loadButtons(function () {
                //加载weB资源
                pageWrap.loadWebSource(function () {
                    //加载字段数据
                    //     pageWrap.loadAttributes(function () {
                    console.log('datas.attributesInfo', $.extend({}, datas.attributesInfo));
                    if (datas.attributesInfo) {
                        $.each(datas.attributesInfo, function () {
                            if (this.name != '') {
                                this.name = this.name.toLowerCase();
                            }
                            //  Xms.Web.getAttributePlug(this);
                        });
                    }
                    datas.setAttributesShow = datas.attributesInfo = pageWrap.filterAttributes(datas.attributesInfo, datas);

                    $('body').trigger('queryview.loadedAttribute', { attributeInfo: datas.attributesInfo });
                    //  = $.extend(true, {}, datas.attributesInfo);
                    console.log(datas.attributesInfo);

                    //加载列表数据
                    pageWrap.loadDataTable();

                    $('body').trigger('queryview.loaded');

                    if (parent && parent != window) {
                        if (window['bodyInited']) window['bodyInited']();
                    }

                    //加载过滤搜索设置
                    pageWrap.formSearcher();

                    pageWrap.loadkanban();

                    //绑定事件
                    pageWrap.bindEvent();
                    $('body').trigger('queryview.bindEvented');
                    callback && callback();
                });
            });
        },
        loadkanban: function () {
            var $aggregateField = $('#aggregateField');
            var $groupField = $('#groupField');
            var agghtmls = $.map(datas.attributesInfo, function (n, i) {
                if (n.attributetypename == "int" || n.attributetypename == "float" || n.attributetypename == "money" || n.attributetypename == "decimal") {
                    return ' <option value="' + n.name + '">' + n.localizedname + '</option>'
                }
            });
            agghtmls.unshift('<option></option>');
            $aggregateField.empty().html(agghtmls.join(''));

            var grouphtmls = $.map(datas.attributesInfo, function (n, i) {
                if (n.attributetypename == "picklist" || n.attributetypename == "status") {
                    return ' <option value="' + n.name + '">' + n.localizedname + '</option>'
                }
            });
            grouphtmls.unshift('<option></option>');
            $groupField.empty().html(grouphtmls.join(''));
        },
        formSearcher: function () {
            var $searchFormSearchItems = $('#searchFormSearchItems');
            if (datas.attributesInfo && datas.attributesInfo.length > 0) {
                var itemshtml = [];
                var isshowQueryDate = false;
                $.each(datas.attributesInfo, function (i, n) {
                    var attrname = n.name;
                    if (!attrname) return true;
                    var isrela = attrname.indexOf('.') != -1;
                    var attrtype = n.attributetypename;
                    var label = n.localizedname;
                    var ctrilId = isrela ? attrname.replace('.', '_') : attrname;
                    var referentityid = n.referencedentityid;
                    var entityid = n.entityid;
                    itemshtml.push('<div class="row seacher-row xms-formDropDown-Item" data-name="' + attrname + '" data-type="' + attrtype + '">');
                    itemshtml.push('<label class="col-sm-4 text-right" for= "' + attrname + '" >' + label + '</label >');
                    itemshtml.push('<div class="col-sm-8">');

                    // if (isrela) {
                    //  } else {
                    if (attrname == 'createdon') {
                        isshowQueryDate = true;//是否显示快捷过滤时间的按钮
                    }
                    if (attrtype == 'datetime') {
                        itemshtml.push('<div class="form-group formrangepicker">');
                        itemshtml.push(' <input type="text" style="width:88px;" id="' + ctrilId + '" class="form-control colinput input-sm " data-type="' + attrtype + '" autocomplete="off" name="' + attrname + '" />');
                        //   itemshtml.push('<span style="width:10px;">-</span>');
                        itemshtml.push('<input type="text" style="width:87px;" autocomplete="off" class="form-control colinput input-sm " name="' + attrname + '" data-type="' + attrtype + '" />');
                        itemshtml.push('</div>');
                    } else if (attrtype == "picklist" || attrtype == "status") {
                        var itesmstr = encodeURIComponent(JSON.stringify(n.optionset.items));
                        itemshtml.push(' <input type="text" id="' + ctrilId + '" class="form-control colinput input-sm picklist" data-type="' + attrtype + '" data-name="' + attrname + '" name="' + attrname + '" data-items="' + itesmstr + '" />');
                    } else if (attrtype == "state" || attrtype == "bit") {
                        var itesmstr = encodeURIComponent(JSON.stringify(n.picklists));
                        itemshtml.push(' <input type="text" id="' + ctrilId + '" class="form-control colinput input-sm picklist" data-type="' + attrtype + '" data-name="' + attrname + '" name="' + attrname + '" data-items="' + itesmstr + '" />');
                    } else if (attrtype == "lookup" || attrtype == "customer" || attrtype == "owner" || attrtype == "primarykey") {
                        itemshtml.push('<div class="input-group input-group-sm">');
                        itemshtml.push('<input type="text" id="' + ctrilId + '" data-type="lookup" data-entityid="' + entityid + '" data-referencedentityid="' + referentityid + '" name="' + attrname + '" class="form-control colinput lookup searchLookup" />');
                        itemshtml.push('<span class="input-group-btn">');
                        itemshtml.push('<button type="button" name="clearBtn" class="btn btn-default ctrl-del" title="find" style="border-radius:0;"><span class="glyphicon glyphicon-remove-sign"></span></button>');
                        itemshtml.push('<button type="button" name="lookupBtn" class="btn btn-default ctrl-search" title="find" style="border-top-left-radius: 0;border-bottom-left-radius: 0;"><span class="glyphicon glyphicon-search"></span></button>');
                        itemshtml.push('</span>');
                        itemshtml.push('</div>');
                    } else if (attrtype == "int" || attrtype == "float" || attrtype == "decimal" || attrtype == "money") {
                        itemshtml.push('<div class="form-group">');
                        itemshtml.push('<input type = "text" style = "width:80px;" id = "' + ctrilId + '" class= "form-control colinput input-sm" data-type="' + attrtype + '" style = "width:90px;" name = "' + attrname + '" value = "" />');
                        itemshtml.push('<span style="width:10px;">-</span>');
                        itemshtml.push('<input type="text" style="width:80px;" class="form-control colinput input-sm" name="' + attrname + '" value="" data-type="' + attrtype + '" />');
                        itemshtml.push('</div >');
                    } else {
                        itemshtml.push('<input type="text" id="' + ctrilId + '" class="form-control colinput input-sm" name="' + attrname + '" data-type="' + attrtype + '" value="" />');
                    }
                    //  }
                    itemshtml.push('</div>');
                    itemshtml.push('</div >');
                });
                if (isshowQueryDate) {
                    $('#queryCalendarBtn').removeClass('hide');
                }
                $searchFormSearchItems.html(itemshtml.join(''));
            }
        },

        setPageInfo: function (key, first, jsonres) {
            var $viewSelector = $('#viewSelector');
            if (datas.queryviews) {
                key = key || 0;
                first = first || datas.queryviews[key];
                if (datas.queryviews.length > 0 && !datas.isloadqueryviews) {
                    $viewSelector.html(pageWrap._getQueryViews(datas.queryviews))
                    datas.isloadqueryviews = true;
                }
                $viewSelector.attr('data-value', first.queryviewid);
                $viewSelector.prev().attr('title', first.name);
                $viewSelector.prev().find('.selecter-label').html(first.name);
                $viewSelector.prev().attr('title', first.name);
                datas.entityId = first.entityid;
                datas.queryId = first.queryviewid;
                datas.aggregateconfig = first.aggregateconfig;
                datas.layoutconfig = first.layoutconfig.toLowerCase();
                datas.fetchconfig = first.fetchconfig.toLowerCase();
                datas.jslibs = [];
                datas.buttonsinfo = jsonres.buttons;
                datas.scripthtml = jsonres.webresources;
                datas.attributesInfo = jsonres.attributes;
                datas.nonereadfields = jsonres.nonereadfields;
                if (datas.layoutconfig && datas.layoutconfig != "") {
                    datas.entitystabs = JSON.parse(datas.layoutconfig).extentitytabs;
                }

                // datas.isloadAttribute = true;
                if (datas.layoutconfig && datas.layoutconfig != '') {
                    try {
                        var clientresource = JSON.parse(datas.layoutconfig);
                        if (clientresource.clientresources && clientresource.clientresources.length > 0) {
                            datas.jslibs = clientresource.clientresources;
                        }
                    } catch (e) {
                        console.error(e);
                    }
                }

                Xms.Page.PageContext.EntityId = datas.entityId;
                Xms.Page.PageContext.QueryId = datas.queryId;
                Xms.Page.PageContext.TargetFormId = first.targetformid;
                datas.breadcrumbinfos.queryviewname = first.name;
                $('#EntityId').val(datas.entityId);
                $('#QueryViewId').val(datas.queryId);
            }
        },
        loadWebSource: function (callback) {
            if (datas.jslibs.length > 0) {
                if (datas.scripthtml && datas.scripthtml != '') {
                    var $script = $('<script class="websource-script-loaded"></script>');
                    console.log('script is loaded')
                    $script.html('try{\n' + datas.scripthtml + '\n}catch(e){console.error("Web资源代码出错",e)}');
                    $('body').append($script)
                    callback && callback();
                }
            } else {
                callback && callback();
            }
        },
        loadButtons: function (callback) {
            var showarea = { glo: 2, inline: 3 };//2,在列表页头部按钮，3列表行内按钮
            //行内按钮
            var btnStr = '';

            //视图上方按钮
            var $queryviewButtons = $('#queryviewButtons');
            var gloBtnStr = '';

            datas.inlinebtnlength = 0;
            if (datas.buttonsinfo && datas.buttonsinfo.length > 0) {
                $.each(datas.buttonsinfo, function (i, n) {
                    if (n.showarea == showarea.glo) {
                        gloBtnStr += '<a class="' + n.cssclass + ' ' + (n.isvisibled ? '' : ' hide ') + '" href="javascript:void(0)" title="' + n.label + '" onclick="' + n.jsaction + '" ' + (n.isenabled ? '' : ' disabled ') + ' ><span class="' + n.icon + '"></span> ' + (n.showlabel ? n.label : '') + '</a> '
                    } else if (n.showarea == showarea.inline/* && n.isenabled*/) {
                        btnStr += "<li> <a class=\"" + n.cssclass + " datagrid-inline-btns\" href=\"javascript: void(0)\" title=\"" + n.label + "\" onclick=\"" + n.jsaction + "\"><span class=\"" + n.icon + "\"></span> " + (n.showlabel ? n.label : '') + "</a></li>";
                        datas.inlinebtnlength++;
                    }
                    if (n.jslibrary) {
                        var lib = n.jslibrary.split(':');
                        datas.jslibs.push(lib[1]);
                    }
                });
                if (btnStr != '') {
                    //datas.gridviewItemBtnstmpl = '<div class="btn-group " style="position:relative;"><div class="btn btn-link dropdown-toggle btn-prevent datatable-itembtn"  aria-expanded="false" style="height: 100%;width:100%;line-height:0.5;text-align:left;padding: 6px 0px;"><span class="caret" style = "top:-3px;" ></span ></div ><ul  class="btn-list  dropdown-menu">' + btnStr + '</ul></div>';
                    datas.gridviewItemBtnstmpl = '<div class="btn-group " style="position:relative;"><div class="btn btn-link dropdown-toggle btn-prevent datatable-itembtn"  aria-expanded="false" style="height: 100%;width:100%;line-height:0.5;text-align:left;padding: 6px 0px;">' + btnStr + '</div ></div>';
                }
                if (gloBtnStr != '') {
                    datas.globtnstr = '<div class=" margin-bottom" style="position:relative;">' + gloBtnStr + '</div>';
                    $queryviewButtons.html(datas.globtnstr)
                }
            }
            callback && callback();
        },
        loadDataTable: function () {
            datas.filter = gridview_filters;
            loadDataTable($('.datagrid-view'), true, datas);
        },
        bindEvent: function () {
            $('#querySettingWrap').xmsMutilSelector({
                leftContext: $('#querySettingWrapHide'),
                rightContext: $('#querySettingWrapShow'),
                sourceDatas: $.extend([], datas.setAttributesShow),
                key: 'name',
                label: 'localizedname',
                leftCtrl: $('#queryviewLeftCtrl'),
                rightCtr: $('#queryviewRightCtrl'),
                upCtrl: $('#queryviewUpCtrl'),
                downCtrl: $('#queryviewDownCtrl'),
                itemClass: 'xmsmutil-selector-item'
            });

            //绑定选择视图下拉框
            $('#viewSelector').off('click').on('click', 'a', function (e) {
                var v = $(this).attr('data-value');
                var self = this;
                var index = $(this).parent().index();
                var $alignType = $('#listAlignStyle').find('.btn.active');
                var aligntype = $alignType.attr('data-type');
                if (v) {
                    $('#viewSelector').attr('data-value', v);
                    if (aligntype == 'top') {
                        pageWrap.loadQueryViews('id', v);
                    } else {
                        pageWrap.loadQueryViews('id', v);
                    }
                }
            });

            $('.formrangepicker').each(function () {
                $(this).xmsMutilDateRangePicker({
                    isDefaultCallback: true,
                    starttime: $(this).children('input:first'),
                    endtime: $(this).children('input:last'),
                    callback: function () {
                        $('.xms-formDropDown-List').addClass('in')
                    }
                })
            })

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

            $('.btn-prevent').off('click').on('click', function (event) {
                event.stopPropagation();
                $(this).next('ul').toggle();
            });

            $('#ChartList').off('change').on('change', function () {
                chartid = $(this).find('option:selected').val();
                queryid = Xms.Page.PageContext.QueryId;
                var groupsInserModal = $('#groupsInserModal');
                $('.changeBig').css('top', 10);
                if (!groupsInserModal.data().groupsCtrl || !groupsInserModal.data().groupsCtrl[chartid]) {
                    if (chartid && queryid) {
                        var $silderRightCrumb = $('.silder-right-crumb');
                        $silderRightCrumb.empty();
                        gridview_filters.clearAll();

                        rebind();
                    }
                    renderChart(chartid, queryid);
                } else {
                    groupsInserModal.data().groupsCtrl[chartid] = [];
                    groupsInserModal.data().groups[chartid] = [];
                    if (chartid && queryid) {
                        pageFilter.emptyGroupFilters();
                        gridview_filters.clearAll();

                        var $silderRightCrumb = $('.silder-right-crumb');
                        $silderRightCrumb.empty();

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
                    $('.menu-right').height($('.data-grid-box').height() - fixheight + 'px')
                    $(".data-grid-box").css("right", 35);
                    $this.attr("active", "1");
                    if (!datas.isloadChart) {
                        GetChartList(function () {
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
                                // pageFilter.submitFilter();
                            }
                        });
                        datas.isloadChart = true;
                    } else {
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
                        }
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

            $('#searchForm').off().on('keyup', function (e) {
                if (e.keyCode == '13') {
                    searchByConditionEnter();
                }
            });

            $(".lookup").siblings("span").find(".ctrl-search").off('click').on('click', function () {
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
            });

            $(".lookup").siblings("span").find(".ctrl-del").off('click').on('click', function () {
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
        },
        bindOnceEvent: function () {
            $(".xms-formDropDown").xmsFormDrop({
                noHidePlace: ".modal,.datepicker,#listAlignTop,.daterangepicker",
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

            $('#queryviewSettingConfirmBtn').off('click').on('click', function () {
                if ($('#querySettingWrap').data().xmsMutilSelector) {
                    var sourcedata = $('#querySettingWrap').xmsMutilSelector('getSourceDatas');
                    var hiddendata = $('#querySettingWrap').xmsMutilSelector('getTargetDatas');
                    console.log(sourcedata);
                    var cmodels = [];
                    if ($('.datagrid-view').data().cDatagrid) {
                        var CM = $('.datagrid-view').cDatagrid('getColModel');
                        cmodels = CM;
                    } else {
                        var CM = $('.datagrid-view').xmsDatagrid('getCols');
                        if (CM.length > 0 && CM[0].length > 0) {
                            cmodels = CM[0];
                        }
                    }

                    if (hiddendata && hiddendata.length >= 0) {
                        $.each(cmodels, function (i, n) {
                            var flag = false;
                            n.hidden = false;
                            n.hide = false;
                            $.each(hiddendata, function (ii, nn) {
                                if (nn.name == n.dataIndx || nn.name == n.field) {
                                    n.hidden = true;
                                    n.hide = true;
                                    return false;
                                }
                            });
                        });

                        if ($('.datagrid-view').data().cDatagrid) {
                            $('.datagrid-view').cDatagrid('refresh');
                        } else {
                            $('.datagrid-view').xmsDatagrid('reflushView');
                        }
                    }
                }
                $('#queryview-settingModal').modal('hide');
            })

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
                    $('#xms-gridview-section').addClass('hide').removeClass('show')
                    $('#kanbanSearch').addClass('in');
                    $('#kanbanview').removeClass('hide');
                    if ($('#xms-table-section').length > 0) { $('#xms-table-section').empty(); }
                } else {
                    $('.kanban-filter').addClass('hide');
                    $('.filter-section').removeClass('hide');
                    $('.date-filter-section').removeClass('hide');
                    $('.xms-fixed-slider').addClass('show');
                    $('#xms-gridview-section').addClass('show').removeClass('hide');
                    $('#kanbanview').addClass('hide');
                    url = '/entity/gridview?queryviewid=' + viewid;
                    var enabledviewselector = $.getUrlParam('isenabledviewselector', location.href);
                    if (enabledviewselector) {
                        url += '&isenabledviewselector=false';
                    }
                    $('.datagrid-view').cDatagrid('refreshDataAndView')
                    $('.datagrid-view').xmsDatagrid('refreshDataAndView');
                    //  pageWrap_list.loadData(url);
                }
            });

            $('body').on('click.resetFilter', 'a.filter-disabled', function (e) {
                gridview_filters.clearAll();
                $('.datagrid-view').cDatagrid('refreshDataAndView')
                $('.datagrid-view').xmsDatagrid('refreshDataAndView');
            });
            $('#queryCalendarBtn').xmsDateRangePicker({
                isLeftRangle: true,
                callback: function (start, end, label) {
                    queryCalandar(start, end, label)
                }
            });
        },
        reSetTopStyle: reSetTopStyle,
    }

    //勾选多选框时是否受到分页和筛选影响
    var mutilCheckbox = null;
    function setCheckboxCrossPage() {
        mutilCheckbox = new xmsMutilCheckbox();

        var $gridview = $('#gridview');
        console.log(mutilCheckbox);
        if (mutilCheckbox) {
            var $records = $gridview.find('table>tbody>tr input[name="recordid"]');

            $('body').on('change', 'input[name="recordid"]', function (e) {
                var checked = $(this).prop('checked');
                if (checked) {//设置为没勾选时
                    mutilCheckbox.add($(this).val());
                } else {
                    mutilCheckbox.del($(this).val());
                }
            });
            $('body').on('change', 'input[name="checkall"]', function (e) {
                var checked = $(this).prop('checked');
                $('input[name="recordid"]', '#gridview').trigger('change');
            });
        }
    }

    function loadDataTable($context, isDestroy, _datas) {
        //统计信息
        var aggInfo = $('#AggregateFields').val();//[{"attributename":"totalamount","aggregatetype":1}]
        var _AggregateTypeList = { '_1': '合计：', '_2': '平均值：', '_3': '最大值：', '_4': '最小值：' };

        if (isDestroy && $context.data().cDatagrid) {
            $context.cDatagrid('destroy');
            $context.data().cDatagrid = null;
        }

        var isWidthToMax = true;
        var layoutconfigObj = '';
        if (_datas.layoutconfig && _datas.layoutconfig != "") {
            layoutconfigObj = JSON.parse(_datas.layoutconfig);
        }
        if (layoutconfigObj) {
            //判断宽度是否需要自适应
            var layoutitems = layoutconfigObj.rows[0].cells;
            var datatableW = $context.width();
            var columnNumW = 30;//序号列宽
            var editWidth = _datas.gridviewItemBtnstmpl ? 100 : 0;//操作列默认宽
            var tableW = columnNumW + editWidth;
            $.each(layoutitems, function (i, n) {
                if (n.Width) {
                    n.width = n.Width;
                }
                tableW += ((n.width || 100) * 1);
            });

            if (datatableW < tableW) {
                isWidthToMax = false;
            }
        }
        //datagrid配置项
        var datagridconfig = {
            freezeCtrl: false,
            isSingle: true,
            getDataUrl: function (cdatagrid, opts) {
                return ORG_SERVERURL + '/api/data/fetchAndAggregate?entityid=' + _datas.entityId + '&queryviewid=' + _datas.queryId + '&onlydata=true&pagesize=' + cdatagrid.opts.pageModel.rPP + '&page=' + cdatagrid.opts.pageModel.page
            },

            selectionModel: { type: null },
            getColModels: function (grid, opts) {
                return _datas.attributesInfo;
            },
            rowDblClick: function (event, ui) {
                var $tr = $(ui.$tr);
                var dblstring = $tr.attr('data-isdblclick');
                if (dblstring == 'false') {
                    return false;
                }
                var id = $(ui.$tr).find('input[name="recordid"]').val();
                var url = ORG_SERVERURL + '/entity/edit?entityid=' + _datas.entityId + '&recordid=' + id;
                entityIframe('show', url);
            },
            loading: false,
            rowClick: function (event, ui) {
                // event.stopPropagation();
                var highline = 'ui-state-highlight';
                var $table = $(event.target);
                var $tr = ui.$tr;
                var checkedval = $tr.find('input[type="checkbox"]').val();//未被冻结的列
                var fozeninput = $(event.target).find('input[value="' + checkedval + '"]:first');
                var hiddeninput = $(event.target).find('input[value="' + checkedval + '"]:last');
                var curInput = fozeninput;

                var checked = curInput.prop('checked');
                if ($(event.toElement).closest('input[type="checkbox"]').length > 0) {
                    return true;
                }
                function check() {
                    if (!checked) {
                        //   ui.rowData.pq_select = true;
                        curInput.trigger('click');
                    }
                }

                function uncheck() {
                    if (checked) {
                        // ui.rowData.pq_select = false;
                        curInput.trigger('click');
                    }
                }

                if (($(event.toElement).hasClass('datatable-itembtn') || $(event.toElement).hasClass('caret'))) {
                    check()
                } else if (curInput.length == 0 && ($(event.currentTarget).hasClass(highline))) {
                    check()
                } else if (curInput.length > 0 && $(event.currentTarget).hasClass(highline)) {
                    uncheck()
                }
                else if (curInput.length == 0) {
                    uncheck()
                } else {
                    if (!$(event.currentTarget).hasClass(highline)) {
                        check()
                    } else {
                        uncheck()
                    }
                }
            },
            checkName: 'recordid',
            headerFilter: true,
            pageModel: { type: "remote", rPP: 10, page: 1, strRpp: "{0}" },
            isWidthToMax: isWidthToMax,
            scrollModel: { autoFit: isWidthToMax },
            filterColModel: function (items) {
                return items;
            },
            columnFilter: function (items) {
                //行事件绑定
                var rowsCommons = layoutconfigObj.rowcommand;
                console.log(rowsCommons)
                if (rowsCommons && rowsCommons.length > 0) {
                    $.each(items, function (key, item) {
                        item.xmsLineEvent = xmsLineEvent;
                        item.rowsCommons = rowsCommons;
                    });
                }
                return items;
            }
        }
        if (typeof PAGEDEFAULT_PAGESIZE != 'undefined') {
            datagridconfig.pageModel.rPP = PAGEDEFAULT_PAGESIZE * 1 ? (PAGEDEFAULT_PAGESIZE * 1) : 10;
        }
        //行事件
        function xmsLineEvent(rowData, rowIndx, CM, row_cls, attr) {
            // console.log(rowData, CM);
            var rowsCommons = [];
            if (CM && CM.length > 0) {
                rowsCommons = CM[0].rowsCommons;
            }
            var res = ["<tr pq-row-indx='", rowIndx, "' class='", row_cls, "' ", attr, " >"].join('');
            $.each(rowsCommons, function (i, n) {
                var __filter = new XmsFilter;
                __filter.FilterOperator = n.logicaloperator == "or" ? 1 : 0;
                __filter.Conditions = n.conditions;
                __filter.Filters.push(__filter);
                var flag = __filter.filterByData(rowData);
                // console.log('filterByData_flag',flag)
                if (flag) {
                    if (n.actiontype == 'setrowbackground') {
                        res = ["<tr pq-row-indx='", rowIndx, , "' class='", row_cls, "' ", attr, ' style="background-color:' + n.action.color + ';" ', " >"].join('');
                    }
                }
            });
            return res
        }

        // 操作列按钮配置
        var $gridviewItemBtnstmpl = _datas.gridviewItemBtnstmpl;
        if ($gridviewItemBtnstmpl) {
            var itemtmpl = $gridviewItemBtnstmpl
            itemtmpl = itemtmpl.replace(/&lt;/g, '<').replace(/&gt;/g, '>');
            datagridconfig.itemsBtnTmpl = itemtmpl;
        }
        $('body').trigger('queryview.itemBtnTmpl', { itemBtnTmpl: datagridconfig.itemBtnTmpl, datagridconfig: datagridconfig });
        var $summary = "";
        datagridconfig.render = function () {
            $summary = $("<div class='pq-grid-summary'  ></div>")
                .prependTo($(".pq-grid-bottom", this));
        }
        datagridconfig.initAfter = function ($grid) {
            datatableItemBtns($('.datatable-itembtn'));
            $grid.$plugGrid.pqGrid("option", "freezeCols", _datas.freezenIndex);
            $grid.$plugGrid.pqGrid("refresh");

            $grid.$grid.on('click', '.forzen-ctrl', function () {
                var index = $(this).parents('td:first').index();
                _datas.freezenIndex = index + 1;
                $grid.$plugGrid.pqGrid("option", "freezeCols", _datas.freezenIndex);
                $grid.$plugGrid.pqGrid("refresh");
                $grid.$grid.find('.forzen-ctrl').removeClass('freeze-ctrl-active');
                $grid.$grid.find('.pq-grid-header-table .pq-grid-title-row td:eq(' + index + ')').find('.forzen-ctrl').addClass('freeze-ctrl-active');
            })
            changeTableFilterDragdown($grid.box.next().find('.datatable-filter-wrapBox').find('.dropdown-toggle'))
            if (_datas.aggInfo) {
            }
        }
        datagridconfig.filterData = function (res, colmodel) {
            if (_datas.aggregateconfig && _datas.aggregateconfig != '') {
                console.log(_datas.aggregateconfig);
                var agginfo = JSON.parse(_datas.aggregateconfig.toLowerCase())
                var fetchdata = res.fetchdata;
                _datas.aggInfo = res.aggregatedata;
                if (_datas.aggInfo) {
                    $.each(colmodel, function (ii, nn) {
                        var flag = null;
                        $.each(_datas.aggInfo.data, function (i, n) {
                            if (nn.field == n.metadata.name.toLowerCase()) {
                                flag = n;
                                return false;
                            }
                        });
                        var temp = {}
                        //  temp[nn.field] = '';
                        if (flag) {
                            if (nn.field != flag.metadata.name.toLowerCase()) {
                            } else {
                                var agginfo = flag.totalamount;
                                if (nn && nn.precision != '' && !isNaN(nn.precision) && (agginfo && !isNaN(agginfo))) {
                                    agginfo = agginfo.toFixed(nn.precision);
                                }
                                //console.log(nn);
                                temp[nn.field] = _AggregateTypeList['_' + flag.aggregatetype] + (agginfo === null ? '' : agginfo);
                                res.fetchdata.items.push(temp);
                            }
                        }
                    });
                }
            }
            return res;
        }
        //添加统计信息行
        datagridconfig.refresh = function (evt, ui) {
            console.log(evt, ui)
            var colmodel = ui.colModel;
            var aggHtmls = [];
            //防止相同条件重复加载
            if (_datas.oldFilterInfo != '') {
                if (_datas.oldFilterInfo == JSON.stringify(gridview_filters.getFilterInfo())) {
                    loadAggInfo();
                } else {
                    _datas.oldFilterInfo = JSON.stringify(gridview_filters.getFilterInfo());
                    loadAggInfo();
                }
            } else {
                _datas.oldFilterInfo = JSON.stringify(gridview_filters.getFilterInfo());
                loadAggInfo();
            }
            datatableItemBtns($('.datatable-itembtn'));
            function loadAggInfo() {
                if (_datas.aggInfo && _datas.aggInfo != "") {
                    var $trs = $('<tr></tr>');
                    var $tds = [];
                    $.each(colmodel, function (ii, nn) {
                        var flag = null;
                        $.each(_datas.aggInfo.data, function (i, n) {
                            if (nn.dataIndx == n.metadata.name.toLowerCase()) {
                                flag = n;
                                return false;
                            }
                        });
                        var temp = {}
                        temp[nn.dataIndx] = '';
                        if (flag) {
                            if (nn.dataIndx != flag.metadata.name.toLowerCase()) {
                                if (nn.dataIndx == 'cdatagrid_editer') {
                                    temp[nn.dataIndx] = ''
                                } else {
                                    temp[nn.dataIndx] = ''
                                }
                            } else {
                                var agginfo = flag[nn.dataIndx];
                                if (nn && nn.precision != '' && !isNaN(nn.precision) && (agginfo && !isNaN(agginfo))) {
                                    agginfo = agginfo.toFixed(nn.precision);
                                }
                                //console.log(nn);
                                temp[nn.dataIndx] = '<div class="aggreBox"><span class="aggreText">' + _AggregateTypeList['_' + flag.aggregatetype] + '</span> ' + agginfo + '</div>'
                            }
                        }
                        $tds.push('<td class="pq-align-right pq-grid-cell">' + temp[nn.dataIndx] + '</td>');
                    });
                    $('body').trigger('queryview.aggInfo', { datagridconfig: datagridconfig, htmls: $tds });
                    $trs.html($tds.join(''))
                    $context.find('table.pq-grid-table').append($trs);
                }
            }
            changeTableFilterDragdown($context.find('.datatable-filter-wrapBox').find('.dropdown-toggle'))
            //if (_datas.freezenIndex) {
            $context.find('.pq-grid-header-left .forzen-ctrl').eq(_datas.freezenIndex - 2).addClass('freeze-ctrl-active');
            // }

            if (_datas.nonereadfields && _datas.nonereadfields.length > 0) {
                $.each(_datas.nonereadfields, function () {
                    var fields = $('.pq-td-div[data-fieldname="' + this + '"]');
                    if (fields.length > 0) {
                        fields.addClass('line-through');
                    }
                });
            }
        }
        datagridconfig.extend = function (datagrid) {
            var extobj = {
                isJsonAjax: true,
                afterAjax: function (that, objP, DM, PM, FM) {
                    // self.afterAjax(self, that, objP, DM, PM, FM);

                    $('body').trigger('queryview.afterAjax', { $context: $context, that: that, datagridconfig: datagridconfig })//安全角色中已使用
                    $context.trigger('rendergridview.afterAjax', { $context: $context, that: that, datagridconfig: datagridconfig });
                },

                getData: function (dataJSON, textStatus, jqXHR) {
                    var resjson = dataJSON.fetchdata || { currentpage: 1, totalitems: 0 };

                    var data = resjson ? resjson.items : [];
                    console.log(dataJSON)
                    _datas.list_datas = data;
                    var res = { curPage: resjson.currentpage || 1, totalRecords: resjson.totalitems, data: data }
                    _datas.oldAggInfo = _datas.aggInfo = dataJSON.aggregatedata;
                    $context.trigger('rendergridview.getData', { _datas: res });
                    // alert(111);
                    return res;
                },
                filterSendData: function (postData, objP, DM, PM, FM) {
                    var filters = _datas.filter;
                    if (filters) {
                        $.extend(postData, { filter: filters.getFilterInfo(), sortby: objP.dataIndx, sortdirection: objP.dir == 'up' ? '0' : '1', pagesize: PM.rPP });
                    } else {
                        $.extend(postData, { sortby: objP.dataIndx, sortdirection: objP.dir == 'up' ? '0' : '1', pagesize: PM.rPP });
                    }

                    $context.trigger('rendergridview.filterSendData', { postData: postData });
                    return postData;
                }
            }
            if (_datas.fetchconfig) {
                var _fetchconfig = JSON.parse(_datas.fetchconfig.toLowerCase());
                if (_fetchconfig.orders && _fetchconfig.orders.length > 0) {
                    extobj.sortIndx = _fetchconfig.orders[0].attributename;
                    extobj.sortDir = _fetchconfig.orders[0].ordertype == 'descending' ? 'up' : 'down';
                }
            }
            $.extend(datagrid.opts.dataModel, extobj)
            $('body').trigger('queryview.datagridExtend', { datagrid: datagrid, _datas: _datas });
        }
        //设置表格高度
        var parHeight = 0, height = 400, fixHeight = 170;
        if (parent && parent.window) {
            height = parent.window.innerHeight - fixHeight;
        } else {
            height = window.innerHeight - 50;
        }
        if (height > 400) {
            datagridconfig.height = height;
        }
        if (_datas.forzenHeight) {
            datagridconfig.height = _datas.forzenHeight;
        }
        datagridconfig.filter = gridview_filters;
        if (_datas.entitystabs != "") {
            datagridconfig.height = 400;
            loadEntityTabs(null);
        }
        $('body').trigger('queryview.setDataGridConfig', { datagridconfig: datagridconfig });
        $context.cDatagrid(datagridconfig)
    }
    var itemBtntempDropdown
    function datatableItemBtns(btns) {
        btns.off('click').on('click', itembtnclick);
        btns.on('itembtnclick', itembtnclick)
        function itembtnclick(e) {
            e.stopPropagation();
            var _sibling = $(this).siblings('.dropdown-menu');
            var isShow = _sibling.is(':hidden');
            _sibling.css('opacity', 0);
            var $tr = $(this).parents('tr:first');
            var $table = $tr.parents('table');
            $table.find('tr').each(function () {
                if (!$(this).is(':hidden')) {
                    var checkbox = $(this).find('input[type="checkbox"]:first')
                    checkbox.prop('checked') && checkbox.trigger('click')
                }
            });
            $tr.trigger('click');
            var index = $(this).parents('tr:first').index() - 1;
            if (itemBtntempDropdown) { itemBtntempDropdown.remove(); }
            if (isShow) {
                var _sibling_clone = _sibling.clone(true);
                _sibling.data()._list_clone = _sibling_clone;
                var clone_offset = $(this).offset();
                var clone_size = { w: _sibling.width(), h: _sibling.height() };
                clone_offset.left = clone_offset.left;
                clone_offset.top = clone_offset.top + 20;
                _sibling_clone.css(clone_offset).css({ 'position': 'absolute', 'opacity': 1 });
                var _rowData = $('.datagrid-view').cDatagrid('getRowData', index);
                _sibling_clone.find('a').data().rowData = _rowData;
                _sibling.find('a').data().rowData = _rowData;
                console.log('row' + index + 'datas:');
                $('body').append(_sibling_clone);
                _sibling_clone.show();
                _sibling.show();
                itemBtntempDropdown = _sibling_clone;
            } else {
                if (itemBtntempDropdown) {
                    itemBtntempDropdown.remove();
                }
                _sibling.hide();
            }

            $(document).on('click.custom.toggle', function (e) {
                var target = $(e.target || e.srcElement);
                if (target.closest(_sibling.parent()).length == 0 && target.closest(itemBtntempDropdown).length == 0) {
                    if (itemBtntempDropdown) {
                        // setTimeout(function () {
                        _sibling.hide();
                        itemBtntempDropdown.remove();
                        // })
                    }
                }
            })
        }
    }

    function submitFilter() {
        var model = new Object();
        model.QueryViewId = Xms.Page.PageContext.QueryId
        model.Filter = filters;
    }

    function changeTableFilterDragdown($context) {
        var $gridview = $('#gridview');
        var tempDropdown = null;
        $context.on('click', function () {
            var _sibling = $(this).siblings('.dropdown-menu');
            var isShow = _sibling.is(':hidden');
            _sibling.css('opacity', 0);
            if (tempDropdown) { tempDropdown.remove(); }
            if (isShow) {
                var _sibling_clone = _sibling.clone(true);
                var clone_offset = $(this).offset();
                var clone_size = { w: _sibling.width(), h: _sibling.height() };
                clone_offset.left = clone_offset.left - 150;
                clone_offset.top = clone_offset.top + 20;
                _sibling_clone.css(clone_offset).css({ 'position': 'absolute', 'opacity': 1 });
                $('body').append(_sibling_clone);
                _sibling_clone.show();
                tempDropdown = _sibling_clone;
            } else {
                if (tempDropdown) {
                    tempDropdown.remove();
                }
            }

            $(document).on('click.custom.toggle', function (e) {
                var target = $(e.target || e.srcElement);
                if (target.closest(_sibling.parent()).length == 0) {
                    if (tempDropdown) {
                        tempDropdown.remove();
                    }
                }
            })
        })
    }

    function closeIframeRebind() {
        rebind(true)
    }
    function rebind(isclose) {
        gridview_filters.clearAll();
        $('.datagrid-view').cDatagrid('refreshDataAndView');
        $('.datagrid-view').xmsDatagrid('reload');
        if (isclose) {
            if ($('.entityCreateSection-close').length > 0 && $('.rightIframe-open').length > 0) {
                $('.entityCreateSection-close').trigger('click');
            }
        }

        //  pageFilter.submitFilter();
    }

    function reSetTopStyle() {
        var $kanbanviewBox = $('#kanbanviewBox');
        var counts = $kanbanviewBox.children('.step-header-group').attr('data-count');
        var step = Math.floor(12 / counts);
        var yuS = 12 % counts;
        var headerItems = $kanbanviewBox.children('.step-header-group').find('.step-header-item');
        var bodyitems = $kanbanviewBox.children('.step-body-group').find('.step-body-item');
        //console.log(step, yuS);
        if (yuS > 0) {
            var leng = headerItems.length;
            headerItems.each(function (key, item) {
                if (key == leng - 1) {
                    var size = 'col-sm-' + (yuS || 2) + ' col-xs-' + (yuS || 2);
                    $(item).addClass(size);
                    bodyitems.eq(key).addClass(size);
                    return true;
                }
                var stsize = 'col-sm-' + (step || 2) + ' col-xs-' + (step || 2);
                $(item).addClass(stsize);
                bodyitems.eq(key).addClass(stsize);
            });
        } else {
            var stsize = 'col-sm-' + (step || 2) + ' col-xs-' + (step || 2);
            headerItems.addClass(stsize);
            bodyitems.addClass(stsize);
        }

        $kanbanviewBox.show();
    }

    function renderChart(chartid, queryid, opts, postData) {
        insertChart(chartid, queryid, opts, postData, function (dHtml) {
            setTimeout(function () {
                //  $('#viewCharts').width($('#viewCharts').parent().innerWidth());
                console.log($('#viewCharts').width());
                //dHtml.css('width',$('#viewCharts').width());
                $('#viewCharts').html(dHtml);
            }, 100)
        });
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
            if (url && url != '' && url.indexOf('\/') == 0) {
                if (!~url.indexOf(ORG_SERVERURL)) {
                    url = ORG_SERVERURL + url;
                }
            }
            $("body").addClass("rightIframe-open").removeClass("rightIframe-close");
            $("#entityCreateIframe").attr("src", url);
        } else {
            $("body").addClass("rightIframe-close").removeClass("rightIframe-open");
            // $("#entityCreateSection").hide()
        }
    }

    function AdvFilterList(filter) {
        gridview_filters.clearAll()
        gridview_filters.addFilter(XmsFilter.changeFiltersToXmsFilter(filter));
        $('.datagrid-view').cDatagrid('refreshDataAndView');
    }
    function AdvancedSearchOpen() {
        window.listAdvancedSearch = window.listAdvancedSearch || $.advancedSearch({
            fields: datas.attributesInfo, operators: Xms.Fetch.ConditionOperators, searchCallback: function (as, filter) {
                AdvFilterList(filter);
            }
        });
        window.listAdvancedSearch.open();
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
        $('.datagrid-view').xmsDatagrid('reload');
        if ($('.menu-right').width() > 100) {
            var chartid = $('#ChartList').find('option:selected').val();
            var queryid = $('#QueryId').val();
            renderChart(chartid, queryid, { 'width': '100%', 'height': '300px' });
        }
    }

    function queryCalandar(start, end, label) {
        console.log(start, end, label);
        var setDate = start.format('YYYY-MM-DD') + ' 00:00:00';
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
        $('.datagrid-view').xmsDatagrid('reload');
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
    $('#dataGridView').on('datatable.rowCheck.rowUnCheck', function (e, opts) {
        //{ type: true, row: fozeninput.parents('tr:first'), recordid: checkedval }
        loadEntityTabs();
    });
    function loadEntityTabs(filters) {
        var $datatable = $('#datatable');
        var $target = $('#entitytabsWrap');
        var recordid = Xms.Web.GetTableSelected($datatable);
        if (recordid.length > 1 || recordid.length == 0) {
            return;
        }
        if ($target.length > 0) {
            $target.remove();
        }

        $datatable.after('<div id="entitytabsWrap" class="entitytabsWrap mt-2"></div>');
        $target = $('#entitytabsWrap');
        var tabinfos = datas.entitystabs;
        console.log('tabinfos', tabinfos)
        if (tabinfos && tabinfos.length > 0) {
            $target.asyncTabs({
                // autoload: false,
                mapkey: { id: 'queryviewid', name: 'tabname', other: 'referencingattributename' },
                datas: tabinfos
                , clickHandler: function ($li, $context) {
                    var queryid = $li.attr('data-id');
                    var attrname = $li.attr('data-other');
                    var recordFilter = null;

                    if (recordid.length == 1) {
                        recordFilter = {}
                        recordFilter.FilterOperator = 0;
                        recordFilter.Conditions = [{ AttributeName: attrname, Operator: '8', Values: recordid }]
                        recordFilter.Filters = [];
                    } else {
                        recordFilter = {}
                        recordFilter.FilterOperator = 0;
                        recordFilter.Conditions = [{ AttributeName: attrname, Operator: '12' }]
                        recordFilter.Filters = [];
                    }
                    if ($li.attr('data-isloaded') == "1") return true;
                    var filter = $li.attr('data-filter') || null;
                    if (filter && filter != "") {
                        filter = JSON.parse(filter);
                    }
                    if (filters) {
                        filter = $.extend({}, filters, (filter || {}));
                    }
                    if (recordFilter) {
                        filter = $.extend({}, recordFilter, filter);
                    }
                    if (filter) {
                        filter = new XmsFilter(filter);
                    }
                    pageWrap.loadSingleQueryview(queryid, $context, function () {
                    }, filter);
                    $li.attr('data-isloaded', 1)
                }
            });
        }
        return $target;
    }

    window.renderChart = renderChart;

    window.entityIframe = entityIframe;
    window.selectRecordCallback = selectRecordCallback;
    window.EditRecord = EditRecord;
    window.DayQuery = DayQuery;
    window.loadEntityTabs = loadEntityTabs;
    window.pageWrap_list = pageWrap;
    window.closeIframeRebind = closeIframeRebind;
    window.rebind = rebind;
    window.advancedSearchOpen = AdvancedSearchOpen;
    window.xmsList_datas = datas;
    return pageWrap;
});