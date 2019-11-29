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
        fetchconfig:'',
        name: '',
        aggregateconfig: '',
        oldaggregateconfig: '',
        globtnstr: '',
        freezenIndex:2,
        gridviewItemBtnstmpl: ''//行按钮
        , inlinebtnlength: 0
        , jslibs: []//视图使用到的Web资源
        , attributesInfo: []
        , setAttributesHide: []//隐藏的字段
        , setAttributesShow: []//显示的字段
        , breadcrumbinfos: {
            entityname: '',
            queryviewname: ''
            ,nonereadfields:[]
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
                } else if (entityid) {
                    url += 'entityid=' + entityid;
                } else if (queryid) {
                    url += 'id=' + queryid;
                }
            } else {
                url += type + '=' + value;
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
            if (entityname) {

            } else if (entityid) {
                Xms.Web.Get(ORG_SERVERURL + '/api/schema/queryview/GetByEntityId?entityid=' + entityid, function (res) {
                    console.log('getbyentityid', JSON.parse(res.Content));
                    datas.queryviews = JSON.parse(res.Content);
                    if ($viewSelector.length > 0) {
                        var isenabled = $viewSelector.attr('data-isenabled');
                        if (isenabled == 1) {
                            if (datas.queryviews && datas.queryviews.length > 0) {
                                //只加载启用的
                                datas.queryviews = pageWrap._filterEnableViews(datas.queryviews);
                                //设置页面信息,第一次加载时默认第一个为当前的视图
                                var index = pageWrap._getDefaultViewKey(datas.queryviews);
                                pageWrap.setPageInfo(index);
                                pageWrap.loadQueryViewInfo();
                            }
                        }
                    }
                });
            } else if (queryid) {
                Xms.Web.Get(ORG_SERVERURL + '/api/schema/queryview/' + queryid, function (res) {
                    console.log('getbyentityid', JSON.parse(res.Content));
                    datas.queryviews = JSON.parse(res.Content);
                    if ($viewSelector.length > 0) {
                        var isenabled = $viewSelector.attr('data-isenabled');
                        if (isenabled == 1) {
                            if (datas.queryviews) {
                                //只加载启用的
                                //  datas.queryviews = pageWrap._filterEnableViews(datas.queryviews);
                                //设置页面信息,第一次加载时默认第一个为当前的视图
                                // var index = pageWrap._getDefaultViewKey(datas.queryviews);
                                pageWrap.setPageInfo(0, datas.queryviews);
                                pageWrap.loadQueryViewInfo();
                            }
                        }
                    }
                });
            }
        },
        loadAttributes: function (callback) {
            // if (!datas.isloadAttribute) {
            //   Xms.Web.GetJson('/api/schema/queryview/getattributes/' + datas.queryId + '?__r=' + new Date().getTime(), null, function (response) {
            // console.log('response', response)
            //  if (response.Content && response.Content != "") {
            //       response.content = JSON.parse(response.Content.toLowerCase());
            //   }
            //  console.log('attributes info', datas.attributesInfo);
            // if (!response.content || response.content.length == 0) {
            //     console.error('没有字段数据');
            //      callback && callback();
            //      return false;
            //  }
            datas.attributesInfo = response.content;
            datas.isloadAttribute = true;
            
            callback && callback();
            //    });
            //  } else {
            //      callback && callback();
            //  }
        },
        loadAggregate: function (callback) {
            if (datas.aggregateconfig && datas.aggregateconfig != '') {

                //  Xms.Web.Post('/api/data/aggregate', { queryViewId: datas.queryId, filter: gridview_filters.getFilterInfo() },false, function (response) {
                // console.log('response', response)
                //  if (response.Content && response.Content != "") {
                //      response.content = JSON.parse(response.Content.toLowerCase());
                //  }
                //   console.log('aggregate info', response.content);
                //   if (!response.content || response.content.length == 0) {
                //       console.log('没有统计数据');
                //        callback && callback();
                //        return false;
                //    }
                //   datas.oldAggInfo = datas.aggInfo = response.content;
                //  pageWrap.isloadAggInfo = true;
                //  callback && callback();
                //   },null,false,false,null);
            }
            //else if ((!datas.aggregateconfig || datas.aggregateconfig == '') && pageWrap.isloadAggInfo) {
            //    datas.aggInfo = '';
            //    callback && callback();
            //} else if (pageWrap.isloadAggInfo && (datas.aggregateconfig && datas.aggregateconfig != '')) {
            //    datas.aggInfo = datas.oldAggInfo;
            //    callback && callback();
            //} else {
            //    callback && callback();
            //}
        },
        filterAttributes: function (items) {
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
            //设置面包屑
            pageWrap.setBreadcrumb();
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
                    datas.setAttributesShow = datas.attributesInfo = pageWrap.filterAttributes(datas.attributesInfo);

                    $('body').trigger('queryview.loadedAttribute', { attributeInfo: datas.attributesInfo });
                    //  = $.extend(true, {}, datas.attributesInfo);
                    console.log(datas.attributesInfo);
                    //加载统计信息
                    //  pageWrap.loadAggregate(function () {
                    //加载列表数据
                    pageWrap.loadDataTable();

                    $('body').trigger('queryview.loaded');

                    if (parent && parent != window) {
                        if (window['bodyInited']) window['bodyInited']();
                    }


                    //   });



                    //加载过滤搜索设置
                    pageWrap.formSearcher();

                    pageWrap.loadkanban();

                    //绑定事件 
                    pageWrap.bindEvent();
                    $('body').trigger('queryview.bindEvented');
                    callback && callback();
                    //  });


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
                    } else if (attrtype == "picklist") {
                        itemshtml.push(' <input type="text" id="' + ctrilId + '" class="form-control colinput input-sm picklist" data-type="' + attrtype + '" data-name="' + attrname + '" name="' + attrname + '" data-items="@Html.UrlEncoder.Encode(itemStr)" />');
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
        setBreadcrumb: function () {
            //添加面包屑
            if ($('.breadcrumb > li:not(.pull-right)').length <= 1) {
                // $('.breadcrumb').append('<li><a href="' + page_Common_Info.breadcrumb_url + '">' + page_Common_Info.breadcrumb_preName + '</a></li>');
                //  $('.breadcrumb').append('<li>' + datas.breadcrumbinfos.queryviewname + '</a></li>');
                //  $('.breadcrumb').append('<li><a href="' + ORG_SERVERURL + '/entity/list?queryviewid=' + page_Common_Info.queryId + '">' + page_Common_Info.queryName + '</a></li>');
                //  $('title').prepend(page_Common_Info.queryName + ' - ');
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
                // datas.isloadAttribute = true;
                if (datas.layoutconfig && datas.layoutconfig != '') {
                    try {
                        var clientresource = JSON.parse(datas.layoutconfig);
                        if (clientresource.clientresources && clientresource.clientresources.length > 0) {
                            //$.each(clientresource.clientresources, function () {
                            //    datas.jslibs.push(this.id);
                            //})
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
                //  var $scripts = $('<script type="text/javascript" src="' + ORG_SERVERURL + '/api/webresource?ids=' + datas.jslibs.join(',') + '" charset="UTF-8"></script>');
                // Xms.Ajax.Get(ORG_SERVERURL + '/api/webresource?ids=' + datas.jslibs.join(','), {}, function (res) {
                if (datas.scripthtml && datas.scripthtml != '') {
                    var $script = $('<script class="websource-script-loaded"></script>');
                    console.log('script is loaded')
                    $script.html('try{\n' + datas.scripthtml + '\n}catch(e){console.error("Web资源代码出错",e)}');
                    $('body').append($script)
                    callback && callback();
                }
                // }, function () {
                //     callback && callback();
                // }, { dataType: 'text' });
                //  $('body').append($scripts)
                // $scripts.get(0).onload = function () {
                //     console.log('script is loaded')
                // }
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
            //加载
            //    Xms.Web.Get(ORG_SERVERURL + '/api/schema/queryview/GetButtons/' + datas.queryId, function (res) {


            //   console.log('GetButtons', res.content);
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
                    datas.gridviewItemBtnstmpl = '<div class="btn-group " style="position:relative;"><div class="btn btn-link dropdown-toggle btn-prevent datatable-itembtn"  aria-expanded="false" style="height: 100%;width:100%;line-height:0.5;text-align:left;padding: 6px 0px;"><span class="caret" style = "top:-3px;" ></span ></div ><ul  class="btn-list  dropdown-menu">' + btnStr + '</ul></div>';
                }
                if (gloBtnStr != '') {
                    datas.globtnstr = '<div class=" margin-bottom" style="position:relative;">' + gloBtnStr + '</div>';
                    $queryviewButtons.html(datas.globtnstr)
                }



            }
            callback && callback();
            //    });
        },
        loadDataTable: function () {
            loadDataTable($('.datagrid-view'), true);
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
                        //  pageWrap.setPageInfo(index, null, datas.jsonres);
                        // pageWrap.loadQueryViewInfo(function () {
                        //  page_common_formSearcher.searchKanban();
                        //      $('#viewSelector').prev('a').find('.selecter-label').text($(self).text());
                        //  });

                    } else {
                        pageWrap.loadQueryViews('id', v);
                        // pageWrap.setPageInfo(index, null, datas.jsonres);
                        // pageWrap.loadQueryViewInfo();
                        // pageWrap_list.loadData('/entity/gridview?queryviewid=' + v + '&IsEnabledViewSelector=' + page_Common_Info.isviewselector + '&IsShowChart=' + page_Common_Info.isshowchart);
                    }
                }
            });
            //$('.datepicker').datepicker({
            //    autoclose: true
            //    , clearBtn: true
            //    , format: "yyyy-mm-dd"
            //    , language: "zh-CN"
            //});

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



            //添加列统计信息
            //  aggFildeName();

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
                            // renderChartCrumb();
                            //   pageFilter.submitFilter();
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
                //$(".xms-formDropDown").trigger("xmsFormDrop.show");
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
                        //排序
                        //  var newCM = $.extend([], true, CM);

                        // $.each(CM, function (ii, nn) {
                        //      newCM.push(nn);
                        //  });

                        //while (CM.length > 2) {
                        //    newCM.push(CM.pop());
                        //}
                        //$.each(sourcedata, function (i,n) {
                        //    $.each(newCM, function (ii, nn) {
                        //        if (nn.dataIndx == n.name) {
                        //            CM.push(nn);
                        //            return false;
                        //        }
                        //    });
                        //});
                        //console.log(CM);
                        //CM = newCM;
                        // $('.datagrid-view').cDatagrid('refreshHeader');
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
            //$('#').on('click', function () {

            //})
        },
        reSetTopStyle: reSetTopStyle,
        //  loadData: loadData
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


    function loadDataTable($context, isDestroy) {

        //统计信息
        var aggInfo = $('#AggregateFields').val();//[{"attributename":"totalamount","aggregatetype":1}]
        var _AggregateTypeList = { '_1': '合计：', '_2': '平均值：', '_3': '最大值：', '_4': '最小值：' };

        if (isDestroy && $context.data().cDatagrid) {
            $context.cDatagrid('destroy');
            $context.data().cDatagrid = null;
        }

        var isWidthToMax = true;
        var layoutconfigObj = '';
        if (datas.layoutconfig && datas.layoutconfig != "") {
            layoutconfigObj = JSON.parse(datas.layoutconfig);
        }
        if (layoutconfigObj) {
            //判断宽度是否需要自适应
            var layoutitems = layoutconfigObj.rows[0].cells;
            var datatableW = $context.width();
            var columnNumW = 30;//序号列宽
            var editWidth = datas.gridviewItemBtnstmpl ? 100 : 0;//操作列默认宽
            var tableW = columnNumW + editWidth;
            $.each(layoutitems, function (i, n) {
                tableW += ((n.Width || 100) * 1);

            });
            //  console.log(datatableW, tableW);
            if (datatableW < tableW) {
                isWidthToMax = false;
            }


        }
        //datagrid配置项
        var datagridconfig = {
            freezeCtrl: false,
            getDataUrl: function (cdatagrid, opts) {
                return ORG_SERVERURL + '/api/data/fetchAndAggregate?entityid=' + datas.entityId + '&queryviewid=' + datas.queryId + '&onlydata=true&pagesize=' + cdatagrid.opts.pageModel.rPP + '&page=' + cdatagrid.opts.pageModel.page
            },

            //  selectionModel: 'cell',
            selectionModel: { type: null },
            getColModels: function (grid, opts) {
                return datas.attributesInfo;
                //'/api/schema/queryview/getattributes/' + datas.queryId + '?__r=' + new Date().getTime();
            },
            rowDblClick: function (event, ui) {
                var id = $(ui.$tr).find('input[name="recordid"]').val();
                var url = ORG_SERVERURL + '/entity/edit?entityid=' + datas.entityId + '&recordid=' + id;
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
                        curInput.trigger('click');
                    }

                }

                function uncheck() {
                    if (checked) {
                        curInput.trigger('click');
                    }
                }

                if (($(event.toElement).hasClass('datatable-itembtn') || $(event.toElement).hasClass('caret') )) {
                    
                    //$table.find('input[type="checkbox"]').each(function(){
                    //    if($(this).prop('checked')){
                    //        $(this).trigger('click');
                    //    }
                    //});
                   
                    check()
                  //  $(event.toElement).find('.caret').trigger('click')
                } else if (curInput.length==0 && ( $(event.currentTarget).hasClass(highline))) {
                    check()
                } else if (curInput.length >0 && $(event.currentTarget).hasClass(highline)) {
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
            checkName:'recordid',
            headerFilter: true,
            pageModel: { type: "remote", rPP: 10, page: 1, strRpp: "{0}" },
            isWidthToMax: isWidthToMax,
            scrollModel: { autoFit: isWidthToMax },
            filterColModel: function (items) {
               
                return items;
                // console.log('layoutitems', layoutitems);
               
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
        var $gridviewItemBtnstmpl = datas.gridviewItemBtnstmpl;
        if ($gridviewItemBtnstmpl) {
            var itemtmpl = $gridviewItemBtnstmpl
            itemtmpl = itemtmpl.replace(/&lt;/g, '<').replace(/&gt;/g, '>');
            datagridconfig.itemsBtnTmpl = itemtmpl;
        }
        $('body').trigger('queryview.itemBtnTmpl', { itemBtnTmpl: datagridconfig.itemBtnTmpl,datagridconfig:datagridconfig });
        var $summary = "";
        datagridconfig.render = function () {
            $summary = $("<div class='pq-grid-summary'  ></div>")
                .prependTo($(".pq-grid-bottom", this));
           

        }
        datagridconfig.initAfter = function ($grid) {
            datatableItemBtns($('.datatable-itembtn'));
            $grid.$plugGrid.pqGrid("option", "freezeCols", datas.freezenIndex);
            $grid.$plugGrid.pqGrid("refresh");
            
            $grid.$grid.on('click','.forzen-ctrl', function () {
                var index = $(this).parents('td:first').index();
                datas.freezenIndex = index+1;
                $grid.$plugGrid.pqGrid("option", "freezeCols", datas.freezenIndex );
                $grid.$plugGrid.pqGrid("refresh");
                $grid.$grid.find('.forzen-ctrl').removeClass('freeze-ctrl-active');
                $grid.$grid.find('.pq-grid-header-table .pq-grid-title-row td:eq(' + index + ')').find('.forzen-ctrl').addClass('freeze-ctrl-active');
            })
            changeTableFilterDragdown($grid.box.next().find('.datatable-filter-wrapBox').find('.dropdown-toggle'))
            if (datas.aggInfo) {

            }
            
        }
        datagridconfig.filterData = function (res,colmodel) {
            if (datas.aggregateconfig && datas.aggregateconfig!='') {
                console.log(datas.aggregateconfig);
                var agginfo = JSON.parse(datas.aggregateconfig.toLowerCase())
                var fetchdata = res.fetchdata;
                datas.aggInfo = res.aggregatedata;
                if (datas.aggInfo) {
                    $.each(colmodel, function (ii, nn) {
                        var flag = null;
                        $.each(datas.aggInfo.data, function (i, n) {
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
                                temp[nn.field] = _AggregateTypeList['_' + flag.aggregatetype] +  (agginfo===null?'':agginfo);
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
            console.log(evt,ui)
            var colmodel = ui.colModel;
            var aggHtmls = [];
            //防止相同条件重复加载
            if (datas.oldFilterInfo != '') {
                if (datas.oldFilterInfo == JSON.stringify(gridview_filters.getFilterInfo())) {
                    loadAggInfo();
                } else {
                   // pageWrap.loadAggregate(function () {
                        datas.oldFilterInfo = JSON.stringify(gridview_filters.getFilterInfo());
                        loadAggInfo();
                  //  });
                }
            } else {
               // pageWrap.loadAggregate(function () {
                    datas.oldFilterInfo = JSON.stringify(gridview_filters.getFilterInfo());
                    loadAggInfo();
              //  });
               
            }
            datatableItemBtns($('.datatable-itembtn'));
            function loadAggInfo() {
                
                if (datas.aggInfo && datas.aggInfo != "") {
                    //var aggObjs = JSON.parse(aggInfo);
                    var $trs = $('<tr></tr>');
                    var $tds = [];
                   // $tds.push('<td class="pq-grid-cell"></td>');
                    $.each(colmodel, function (ii, nn) {
                        var flag = null;
                        $.each(datas.aggInfo.data, function (i, n) {
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
                    $('body').trigger('queryview.aggInfo', { datagridconfig: datagridconfig,htmls:$tds });
                    $trs.html($tds.join(''))
                    $context.find('table.pq-grid-table').append($trs);
                    //var index = $trs.index();
                    //$(evt.target).pqGrid("option", "freezeRows", 1);
                    //if (!datas.isforzenRow || datas.isforzenRow == false) {
                    //    datas.isforzenRow = true;
                    //    $(evt.target).pqGrid("refresh");
                    //    datas.isforzenRow = false;
                    //   // return false;
                    //}
                   // 
                }
            }
            changeTableFilterDragdown($context.find('.datatable-filter-wrapBox').find('.dropdown-toggle'))
            //if (datas.freezenIndex) {
            $context.find('.pq-grid-header-left .forzen-ctrl').eq(datas.freezenIndex - 2).addClass('freeze-ctrl-active');
           // }

            if (datas.nonereadfields && datas.nonereadfields.length > 0) {
                $.each(datas.nonereadfields, function () {
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

                    $('body').trigger('queryview.afterAjax', { $context: $context, that: that, datagridconfig: datagridconfig })
                },

                getData: function (dataJSON, textStatus, jqXHR) {
                    var resjson = dataJSON.fetchdata
                    var data = resjson.items;
                    console.log(dataJSON)
                    var res = { curPage: resjson.currentpage || 1, totalRecords: resjson.totalitems, data: data }
                    datas.oldAggInfo = datas.aggInfo = dataJSON.aggregatedata;
                    // alert(111);
                    return res;
                },
                filterSendData: function (postData, objP, DM, PM, FM) {
                    //console.log('postdata', postData, objP, DM, PM, FM)
                    $.extend(postData, { filter: gridview_filters.getFilterInfo(), sortby: objP.dataIndx, sortdirection: objP.dir == 'up' ? '0' : '1', pagesize: PM.rPP });
                    return postData;
                }
            }
            if (datas.fetchconfig) {
                var _fetchconfig = JSON.parse(datas.fetchconfig.toLowerCase());
                if (_fetchconfig.orders && _fetchconfig.orders.length > 0) {
                    extobj.sortIndx = _fetchconfig.orders[0].attributename;
                    extobj.sortDir = _fetchconfig.orders[0].ordertype =='descending'?'up':'down';
                }
            }
            $.extend(datagrid.opts.dataModel, extobj )
            $('body').trigger('queryview.datagridExtend', { datagrid: datagrid, datas:datas });
        }
        //设置表格高度
        var parHeight = 0, height=400, fixHeight = 220;
        if (parent && parent.window) {
            height = parent.window.innerHeight - fixHeight;
        } else {
            height = window.innerHeight - 50;
        }
        if (height > 400) {
            datagridconfig.height = height;
        }
        datagridconfig.filter = gridview_filters;
        
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
                var clone_offset = $(this).offset();
                var clone_size = { w: _sibling.width(), h: _sibling.height() };
                clone_offset.left = clone_offset.left;
                clone_offset.top = clone_offset.top + 20;
                _sibling_clone.css(clone_offset).css({ 'position': 'absolute', 'opacity': 1 });
                var _rowData = $('.datagrid-view').cDatagrid('getRowData', index);
                _sibling_clone.find('a').data().rowData = _rowData;
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
                if (target.closest(_sibling.parent()).length == 0 && target.closest(itemBtntempDropdown).length==0) {
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
        // console.log('arguments.callee.caller', arguments.callee.caller);

        var model = new Object();
        model.QueryViewId = Xms.Page.PageContext.QueryId
        model.Filter = filters;
        //Xms.Web.LoadPage(pageUrl, model, function (response) {
        //    //console.log(response);
        //    $('#gridview').html($(response).find('#gridview').html());
        //    ajaxgrid_reset();
        //    $('#gridview').trigger('gridview.loaded');
        //    //如果设置已勾选可不受分页影响

        //    //resetRightCharts();
        //});
    }
    function setGridTableH() {
        var gridId = $("#gridview");
        var height = $(".xms-table-section").outerHeight();
        //console.log(height)
        gridId.height(height);
    }

    //function loadData(url, target, callback) {
    //    Xms.Web.Load(url, function (response) {
    //        //console.log(response);
    //        if (typeof response == 'object') {
    //            //$('#content').text(response.Content);
    //            Xms.Web.Toast(response.Content, 'error', false);
    //            return;
    //        }
    //        var $contentEl = target || $('#content');
    //        $contentEl.html(response);
    //        //$("#datatable").ajaxTable();
    //    //    ajaxTable();
    //        //ajaxgrid_reset();
    //        //默认第一列为快速查找字段
    //        $('#fieldDropdown').next().find('a:eq(1)').trigger('click');
    //        entityid = $('#EntityId').val();
    //     //   changeTableHeight();
    //        if (parent && parent != window) {
    //            if (window['bodyInited']) window['bodyInited']();
    //        }
    //      //  callback && callback();
    //      //  $('#gridview').trigger('gridview.inited');
    //      //  pageWrap_Gridview.init();
    //    });
    //}

    //function changeTableHeight() {
    //    setGridTableH();
    //    var parW = Xms.Web.getParentWin();
    //    if (parW != window) {
    //        var offsetT = 40;
    //        var parW_height = $(parW).height();
    //        var thisW = $(window);
    //        var parWOffsetT = parW_height - offsetT;
    //        if ($("#datatable").length == 0) return false;
    //        var dataTableT = $("#datatable").offset().top;
    //        $("#gridview .panel").height(parWOffsetT - dataTableT - 10 - 90);
    //        $("#content,.page-render-wrap,#main").css("margin-bottom", 0);
    //    }
    //    setGridTableH();
    //}

    //function __settableheaderWidth() {
    //    var editFormTable = $("#datatable");
    //    var ths = editFormTable.find('th.tableHeaderItem');
    //    var widths = 0;
    //    ths.each(function (key, item) {
    //        var _w = $(item).attr('data-width') * 1;
    //        widths += _w;
    //    });

    //    $('#tableHeaderWidth').val(widths);
    //}
    //function settableHeaderWidth() {

    //    __settableheaderWidth();
    //    var tableW = $('#tableHeaderWidth').val() * 1;
    //    var wrapW = $('.tableReWidth').eq(0).width();//
    //   // console.log('settableHeaderWidth', tableW, wrapW);
    //    if (tableW >= wrapW) {
    //        $("#datatable").width(tableW)
    //    } else {
    //        $("#datatable").width('100%');
    //    }
    //    $("#datatable").tableHdResize({ resetTableWidth: false, linkTable: '#fozon-table' });
    //    $('.gridview-table-cell', '#datatable').addClass('cell-ellipsis');

    //}

    //function ajaxgrid_reset() {
    //    $('.datepicker').datepicker({
    //        autoclose: true
    //        , clearBtn: true
    //        , format: "yyyy-mm-dd"
    //        , language: "zh-CN"
    //    });
    //    pageUrl = $("#datatable").attr('data-pageurl');
    //    pag_init();
    //    Xms.Web.DataTable($("#datatable"));
    //    $('#searchForm').ajaxSearch('#gridview', function () {
    //        ajaxgrid_reset();
    //        pageFilter.clearColumnFilters();
    //        filters = new Xms.Fetch.FilterExpression();
    //        pageFilter.bindColumnFilterStatus();
    //        $('#gridview').trigger('gridview.clear');
    //    });
    //    setTableFilter();
    //    pageFilter.bindColumnFilterStatus();
    //    //resetRightCharts();
    //    changeTableHeight();
    //    settableHeaderWidth();
    //    if (mutilCheckbox) {
    //        var _values = mutilCheckbox.getValues();
    //        $.each(_values, function (key, item) {
    //            $('input[name="recordid"][value="' + item + '"]', '#gridview').prop('checked', true);
    //        });
    //    }
    //    if (table_fozone == true) {

    //        $('#datatable').forzenTable({
    //            target: $('#xms-table-section')
    //            , colOffset: [0, 23, 0, -20]
    //            , disabled: [0]
    //            , enable: []
    //            // ,fhead:false
    //            , maxLength: 2
    //            , isHide: true
    //            , position: 'left'
    //            , headOffset: [0, 21, 0, 0]

    //        });

    //        $('#datatable').on('forzen.show', function (e, opts) {
    //            // { obj: self, sumWidth: sumWidth }
    //            $('#fozon-wrap').css('padding-left', opts.sumWidth);
    //        });
    //        $('#datatable').on('forzen.hide', function (e, opts) {
    //            // { obj: self, sumWidth: sumWidth }
    //            $('#fozon-wrap').css('padding-left', opts.sumWidth);
    //        });
    //        changeTableFilterDragdown($gridview.find('.datatable-filter-wrap').find('.dropdown-toggle'));

    //        // $('#datatable').forzenTable('_showCol', 1,5);//默认冻结操作列
    //    }
    //    $('#gridview').trigger('gridview.ajaxreset');
    //}
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




    function pag_init() {
        $('#page-selection').bootpag({
            total: $('#page-selection').attr('data-total')
            , maxVisible: 10
            , page: $('#page-selection').attr('data-page')
            , leaps: false
            , prev: '&lsaquo;'
            , next: '&rsaquo;'
            , firstLastUse: true
            , first: '&laquo;'
            , last: '&raquo;'
        }).on("page", function (event, /* page number here */ num) {
            event.preventDefault();
            pageUrl = $.setUrlParam(pageUrl, 'page', num);
            pageFilter.submitFilter();
            
            return false;
        });
    }
    function rebind() {
        gridview_filters.clearAll();
        $('.datagrid-view').cDatagrid('refreshDataAndView');
        $('.datagrid-view').xmsDatagrid('reload');
        //if ($('.entityCreateSection-close').length > 0 && $('.rightIframe-open').length>0) {
        //    $('.entityCreateSection-close').trigger('click');
        //}
       
      //  pageFilter.submitFilter();
    }
    function setTableFilter() {
        var enabledFilter = $('#datatable').attr('data-enabledfilter');
        if (enabledFilter == 'true') {
            $('#datatable').find('thead th[data-type]').each(function (i, n) {
                var self = $(n);
                var dataType = self.attr('data-type');
                var name = self.attr('data-name');
                var width = self.attr('data-width');
                var label = self.attr('data-label');
                var referecingentityid = self.attr('data-referencedentityid');
                var dataText = self.data().dataText;
                var controls = new Array();

                controls.push('<div style="width:' + (width) + 'px;" class="datatable-filter-wrap" title="' + self.attr('data-label') + '">');
                if (!dataText) {
                    self.data().dataText = self.html();
                    controls.push(self.html());
                } else {
                    controls.push(dataText);
                }
                
                controls.push('<div class="btn-group controls pull-right">');
                controls.push('<a class="dropdown-toggle" data-toggle="dropdown" href="#">');
                controls.push('<span class="caret"></span>');
                controls.push('</a>');
                controls.push('<ul class="dropdown-menu">');
                controls.push('<li class="dropdown-header">' + LOC_FILTER + '</li>');
                controls.push('<li><a href="javascript:void(0)" class="disabled"><span class="glyphicon glyphicon-remove-sign"></span> ' + LOC_RESET + '</a></li>');
                controls.push('<li><a href="javascript:void(0)" data-operator="notnull" onclick="pageFilter.filterColumnNull(\'' + name + '\', false)"><span class="glyphicon glyphicon-ok-circle"></span> ' + LOC_FILTER_NOTNULL + '</a></li>');
                controls.push('<li><a href="javascript:void(0)" data-operator="null" onclick="pageFilter.filterColumnNull(\'' + name + '\', true)"><span class="glyphicon glyphicon-ban-circle"></span> ' + LOC_FILTER_NULL + '</a></li>');
                controls.push('<li><a href="javascript:void(0)" data-referencedentityid="' + referecingentityid + '" onclick="pageFilter.customizeFilter(\'' + name + '\', \'' + dataType + '\',this)"><span class="glyphicon glyphicon-pencil"></span> ' + LOC_FILTER_CUSTOMIZE + '</a></li>');
                controls.push('</ul>');
                controls.push('</div>');
                controls.push('</div>');

                self.html(controls.join('\n'));
            });
        }
    }
    //function ajaxTable() {
    //    var self = $("#datatable");
    //    var containerId = '#' + self.data('ajaxcontainer');
    //    var callback = Xms.Utility.GetFunction(self.data('ajaxcallback'));
    //    //$(containerId).parent().undelegate(containerId + ' a[data-ajax="true"]', 'click');
    //    $(containerId).parent().delegate(containerId + ' a[data-ajax="true"]', 'click', function () {
    //        var model = new Object();
    //        var url = $(this).attr('href');
    //       // model = $.urlParamObj(url);
    //      //  url = url + (url.indexOf('?') == -1 ? '?' : '&') + '__r=' + new Date().getTime();
    //        model.QueryViewId = Xms.Page.PageContext.QueryId;
    //        model.Filter = filters;
    //        Xms.Web.LoadPage(url, model, function (response) {
    //            $('#gridview').html($(response).find('#gridview').html());
    //            ajaxgrid_reset();
    //            $('#gridview').trigger('gridview.reload');
    //        });
    //        return false;
    //    });
    //}
    function resetRightCharts() {
        if ($(".xms-slider-ctrl").length > 0) {
            $(".xms-slider-ctrl").attr("active", "2");
            $(".xms-table-section").css({ "right": 35 });
            $(".menu-md.menu-right").css({ "width": 0 });
            $(".xms-slider-ctrl").find(".glyphicon").removeClass("glyphicon-arrow-right");
        }
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
        var setDate = start.format('YYYY-MM-DD');
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


    window.renderChart = renderChart;

    window.entityIframe = entityIframe;
   // window.pageWrap_Gridview = pageWrap_Gridview;
    window.selectRecordCallback = selectRecordCallback;
    window.EditRecord = EditRecord;
    window.DayQuery = DayQuery;

   // window.ajaxgrid_reset = ajaxgrid_reset;
    window.pageWrap_list = pageWrap;
  //  window.__settableheaderWidth = __settableheaderWidth;
   // window.settableHeaderWidth = settableHeaderWidth;
    window.rebind = rebind;
    return pageWrap;
});