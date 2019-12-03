//@ sourceURL=page/customizehome.js
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
    //deps userpagesetting.js
    // var getUserAndPageSetting = getUserAndPageSetting;

    var pageSettingName = 'customizehome';
    var userSetting = {
        key: 'customizehome_quicklist'//快捷方式
    }
    var _settings = getUserAndPageSetting(pageSettingName, userSetting.key)
    var userSettingCtrl = _settings.userSettingCtrl;
    var pageSettingCtrl = _settings.pageSettingCtrl;
    var userSettingDefaults = {
        shortcutList: []
    }

    var customhome = new function () {
        var self = this;
        this.shortcutDatas = [];
        this.solutionid = '';
        this.quickLinkList = [];
        this.init = function () {
            $(function () {//用户已经设置过快捷方式
                var datas = userSettingCtrl.getDatasBykey(pageSettingName);//获取当前页面用户信息

                self.loadDefault();

                self.loadSystemInfo();
            })
        }
        this.loadParentDatas = function () {
            var datas = Xms.Web.callParentMethod('xms_navGetAllDatas');
            this.solutionid = Xms.Web.callParentMethod('getPageSolutionId');
            return datas;
        }
        //加载初始化设置
        this.loadDefault = function (datas) {
            var listdatas = pageSettingCtrl.getDatasBykey(userSetting.key)//获取快捷方式列表
            if (listdatas && listdatas.length == 0 && userSettingDefaults.shortcutList.length == 0) {
                var _datas = this.loadParentDatas();
                _datas = this.filterparentData(_datas);
                //加载快捷方式
                if (_datas) {
                    this.addShortCutData(_datas)
                    this.loadShortCutList();
                }
            } else {
                if (listdatas.length > 0) {
                    this.addShortCutData(listdatas[0].value)
                    this.loadShortCutList();
                }
            }
        }
        this.filterparentData = function (datas) {
            if (datas && datas.length >0) {
                var res = $.extend([], datas[0]);
                if (this.solutionid) {
                    $.each(res.children, function (i, n) {
                        n.solutionid = self.solutionid;
                        n.remove = true;
                    })
                }
                console.log(datas);
                return res.children;
            }
        }
        this.loadpage = function (datas) {
        }
        this.addShortDataAndRender = function (datas, datas1) {
            if (datas.nodeType && datas1) {
                datas = datas1;
            }
            self.addShortCutData(datas);
            self.loadShortCutList();
            pageSettingCtrl.add({ key: userSetting.key, value: self.shortcutDatas }, true);
            userSettingCtrl.addSetting(pageSettingName, pageSettingCtrl.getDatas(), null, true);
            userSettingCtrl.saveByRemote();
        }
        this.addShortCutData = function (datas) {
            $.each(datas, function (i, n) {
                if ($.indexBykeyValue(self.shortcutDatas, 'id', n.id) == -1) {
                    self.shortcutDatas.push(n);
                }
            });
        }
        //快捷方式部分
        //初始化
        this.loadShortCutList = function () {
            var datas = self.shortcutDatas;
            console.log(datas);
            var $shortcutList = $('#shortcutList');
            var listhtmls = [];
            var contenthtmls = [];
            var maxcount = 8;
            $shortcutList.children().eq(0).empty();
            $.each(datas, function (i, n) {
                var count = i % maxcount;
                var step = i / maxcount >> 0;
                var data = self.addShortCutList(n, step);
                if (count == 0) {
                    if (step === 0) {
                        contenthtmls.push('<div class="item active"><ul class=" pt-1">');
                    } else {
                        contenthtmls.push('<div class="item"><ul class=" pt-1">');
                    }
                }
                contenthtmls.push(data.content);
                if (count == maxcount - 1) {
                    contenthtmls.push('</ul></div>');
                }
                if (count == 0) {
                    $shortcutList.children().eq(0).append(data.list);
                }
            });

            $shortcutList.children().eq(1).html(contenthtmls.join(''));
            this.bindShortCutList();
        }
        this.addShortCutList = function (data, count) {
            var icon = data.icon || getTypeIcon(data.type);
            if (icon && icon != '' && icon.indexOf('<') == -1) {
                icon = '<em class="' + icon + '"></em>';
            }
            return {
                list: ' <li data-target="#shortcutList" data-slide-to="' + count + '" class=" ' + (count === 0 ? 'active' : '') + '"></li>',
                content: '<li class="col-sm-3">' +
                    '<a href="javascript:;" class="shortcut-item" data-id="' + data.id + '">' +
                    (!data.remove ? '<div class="item-remove" data-id="' + data.id + '">×</div>' : '') +
                    '<i class="item-icon"><span>' + icon + '</span></i>' +
                    '<p class="item-text"> ' + (data.name || data.label) + '</p>' +
                    '</a >' +
                    '</li >'
            };
        }
        this.removeShortCutList = function ($item, datas) {
            var index = $.indexBykeyValue(self.shortcutDatas, 'id', datas.id);
            $item.remove();
            self.shortcutDatas.splice(index, 1);
            pageSettingCtrl.add({ key: userSetting.key, value: self.shortcutDatas }, true);
            userSettingCtrl.addSetting(pageSettingName, pageSettingCtrl.getDatas(), null, true);
            userSettingCtrl.saveByRemote();
        }
        this.bindShortCutList = function (datas) {
            var $shortcutList = $('#shortcutList');
            $shortcutList.on('click', '.shortcut-item', function (e) {
                var $this = $(this);
                var id = $this.attr('data-id');
                if (self.shortcutDatas && self.shortcutDatas.length > 0) {
                    var index = $.indexBykeyValue(self.shortcutDatas, 'id', id);
                    if (index != -1) {
                        Xms.Web.callParentMethod('triggerLink', self.shortcutDatas[index])
                        //triggerLink();
                    }
                }
            })
            $shortcutList.on('click', '.item-remove', function (e) {
                e.stopPropagation();
                var $this = $(this);
                var id = $this.attr('data-id');
                if (self.shortcutDatas && self.shortcutDatas.length > 0) {
                    var data = $.queryBykeyValue(self.shortcutDatas, 'id', id);
                    if (data.length > 0) {
                        self.removeShortCutList($this.parents('li:first'), data[0]);
                    }
                }
            })
        }
        this.loadSystemInfo = function () {
            Xms.Web.Get(ORG_SERVERURL + '/api/serverhostmanage/getSystemInfomation', function (res) {
                console.log(res);
                var datas = res.content;
                for (var i in datas) {
                    if (datas.hasOwnProperty(i)) {
                        var item = datas[i];
                        if (i == 'osdescription') {
                            if (item && item != '') {
                                item = getSystemShortName(item);
                            }
                        } else if (i == 'diskinfos') {
                            item = renderDiskInfo(datas);
                        } else if (i == 'tickcount') {
                            item = getSystemTickcount(item);
                        } else if (i == 'cpucounter') {
                            item = getSystemCpucount(item);
                        } else if (i == 'cpuname') {
                            item = getSystemCpuname(item);
                        }
                        var $inner = $('#system_' + i);
                        if ($inner.length > 0) {
                            $inner.children(':first').html(item)
                            if (i != 'diskinfos') {
                                $inner.attr('title', item);
                            }
                        }
                    }
                }
            }, null, false, false, null);
        }
    }

    var addShortDataAndRender = customhome.addShortDataAndRender;
    function getSystemCpuname(mss) {
        var res = '';
        var index = mss.indexOf('CPU');
        if (index == -1) {
            index = mss.indexOf('cpu');
        }
        if (index != -1) {
            res = mss.substring(0, index);
        }
        return res;
    }
    function getSystemCpucount(mss) {
        return parseInt(mss) + '%';
        // return (data / 1000).toFixed(0);
    }
    function getSystemTickcount(mss) {
        var hours = parseInt((mss % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        var minutes = parseInt((mss % (1000 * 60 * 60)) / (1000 * 60));
        var seconds = parseInt((mss % (1000 * 60)) / 1000);
        return hours + '小时' + minutes + '分' + seconds + '秒';
        // return (data / 1000).toFixed(0);
    }

    function renderDiskInfo(datas) {
        var htmls = [];
        var classnames = ['progress-bar progress-bar-success', 'progress-bar progress-bar-info', 'progress-bar progress-bar-warning', 'progress-bar progress-bar-danger', 'progress-bar progress-bar-success', 'progress-bar progress-bar-info', 'progress-bar progress-bar-warning', 'progress-bar progress-bar-danger']
        $.each(datas.diskinfos, function (i, n) {
            var percent = ((n.totalsize - n.totalfreespace) / n.totalsize).toFixed(2);
            htmls.push(['<div class="progress">',
                '<div class="' + classnames[i] + '" role = "progressbar" aria - valuenow="40" aria - valuemin="0" aria - valuemax="100" style = "width: ' + (percent * 100) + '%" >',
                '<span class="glyphicon glyphicon-hdd ml-1 pull-left"></span> ' + n.name + ' ' + ((percent * 100) >> 0) + '%',
                '</div >',
                '</div >'
            ].join(''));
        });
        return htmls.join('')
    }

    function getSystemShortName(name) {
        var res = '';
        if (name.indexOf('Microsoft') != -1) {
            if (name.indexOf('Windows 10') != -1) {
                res = 'Windows 10';
            } else if (name.indexOf('Windows 7') != -1) {
                res = 'Windows 7';
            } else if (name.indexOf('Windows 2008') != -1) {
                res = 'Windows 2008';
            } else if (name.indexOf('Windows 2012') != -1) {
                res = 'Windows 2012';
            } else if (name.indexOf('Windows 2018') != -1) {
                res = 'Windows 2018';
            }
        } else if (name.indexOf('Linux') != -1) {
            res = 'Linux';
        }
        return res;
    }

    var getSystemInfo = {
        getSystemShortName: getSystemShortName,
        getSystemTickcount: getSystemTickcount,
        renderDiskInfo: renderDiskInfo,
    }
    var customizeNav = {
        load: function (url, node) {
            if (node.solutionid) {
                url += url.indexOf('?') > 0 ? '&solutionid=' + node.solutionid : '?solutionid=' + node.solutionid;
                url = ORG_SERVERURL + url;
                location.href = url;
            }
        },
        loadComponents: function (node) {
            this.load('/customize/solution/components', node);
        },
        loadEntities: function (node) {
            this.load('/customize/entity/index', node);
        },
        loadForms: function (entityid, node) {
            this.load('/customize/systemform/index?entityid=' + entityid, node);
        },
        loadAttributes: function (entityid, node) {
            this.load('/customize/attribute/index?entityid=' + entityid, node);
        },
        loadViews: function (entityid, node) {
            this.load('/customize/queryview/index?entityid=' + entityid, node);
        },
        loadOptionSets: function (node) {
            this.load('/customize/OptionSet/index', node);
        },
        loadRibbonButtons: function (entityid, node) {
            this.load('/customize/ribbonbutton/index?entityid=' + entityid, node);
        },
        loadDuplicateRules: function (entityid, node) {
            this.load('/customize/duplicaterule/index?entityid=' + entityid, node);
        },
        loadEntityMaps: function (entityid, node) {
            this.load('/customize/entitymap/index?entityid=' + entityid, node);
        },
        loadRelations: function (entityid, type, node) {
            this.load('/customize/relationship/index?entityid=' + entityid + '&type=' + type, node);
        },
        loadFilterRules: function (entityid, node) {
            this.load('/customize/filterrule/index?entityid=' + entityid, node);
        },
        loadCharts: function (entityid, node) {
            this.load('/customize/chart/index?entityid=' + entityid, node);
        },
        loadDashboards: function (node) {
            this.load('/customize/dashboard/index', node);
        },
        loadReports: function (node) {
            this.load('/customize/report/index', node);
        },
        loadWebResources: function (node) {
            this.load('/customize/webresource/index', node);
        },
        loadWorkflows: function (node) {
            this.load('/customize/flow/index', node);
        },
        loadAutoNumberRules: function (node) {
            this.load('/customize/serialnumber/index', node);
        },
        loadPlugins: function (node) {
            this.load('/customize/entityplugin/index', node);
        },
        loadFieldPermissions: function (node) {
            this.load('/FieldSecurity/index', node);
        },
        loadLabels: function (node) {
            this.load('/customize/localizedlabel/index', node);
        }
    }
    function triggerLink(node) {
        var type = node.type;
        // console.log(type, node);
        var hashlink = "/" + type + ":" + node.link;
        if (type == 'solution') {
            customizeNav.load('/customize/solution/editsolution?id=' + solutionid, node);
        }
        else if (type == 'component') {
            customizeNav.loadComponents(node);
        }
        else if (type == 'entity') {
            customizeNav.load('/customize/entity/editentity?id=' + node.link, node);
            if (!node) return false;
            $('#customTabNav').iframeLinks('addLink', { id: node.id, name: node.label, other: '(' + node._parentname + ')', src: ORG_SERVERURL + '/customize/entity/editentity?id=' + node.link });
        }
        else if (type == 'form') {
            customizeNav.loadForms(node.link, node);
        }
        else if (type == 'view') {
            customizeNav.loadViews(node.link, node);
        }
        else if (type == 'attribute') {
            customizeNav.loadAttributes(node.link, node);
        }
        else if (type == 'ribbonbutton') {
            customizeNav.loadRibbonButtons(node.link, node);
        }
        else if (type == 'duplicaterules') {
            customizeNav.loadDuplicateRules(node.link, node);
        }
        else if (type == 'entitymap') {
            customizeNav.loadEntityMaps(node.link, node);
        }
        else if (type == 'onetomore') {
            customizeNav.loadRelations(node.link, 1, node);
        }
        else if (type == 'moretoone') {
            customizeNav.loadRelations(node.link, 2, node);
        }
        else if (type == 'moretomore') {
            customizeNav.loadRelations(node.link, 3, node);
        }
        else if (type == 'filterrules') {
            customizeNav.loadFilterRules(node.link, node);
        }
        else if (type == 'chart') {
            customizeNav.loadCharts(node.link, node);
        }
        else if (type == 'optionset') {
            customizeNav.loadOptionSets(node);
        }
        else if (type == 'fieldpermission') {
            customizeNav.loadFieldPermissions(node);
        }
        else if (type == 'dashboard') {
            customizeNav.loadDashboards(node);
        }
        else if (type == 'report') {
            customizeNav.loadReports(node);
        }
        else if (type == 'webresource') {
            customizeNav.loadWebResources(node);
        }
        else if (type == 'workflow') {
            customizeNav.loadWorkflows(node);
        }
        else if (type == 'autonumber') {
            customizeNav.loadAutoNumberRules(node);
        }
        else if (type == 'plugin') {
            customizeNav.loadPlugins(node);
        }
        else if (type == 'labels') {
            customizeNav.loadLabels(node);
        }
        else {
            customizeNav.loadEntities(node);
        }
        var hash = location.hash;
        //clearHashLink();
        location.hash = hashlink;
    }
    function getTypeIcon(type) {
        var node = { type: type };
        if (node.type == 'solution')
            return "glyphicon glyphicon-info-sign";
        else if (node.type == 'component')
            return "glyphicon glyphicon-th-large";
        else if (node.type == 'entity')
            return "glyphicon glyphicon-book";
        else if (node.type == 'view')
            return '<i class="layui-icon layui-icon-layer">&#xe638;</i>';
        else if (node.type == 'form')
            return '<i class="layui-icon layui-icon-table">&#xe63c;</i>';
        else if (node.type == 'attribute')
            return "glyphicon glyphicon-file";
        else if (node.type == 'ribbonbutton')
            return "glyphicon glyphicon-flash";
        else if (node.type == 'duplicaterules')
            return "glyphicon glyphicon-repeat";
        else if (node.type == 'filterrules')
            return "glyphicon glyphicon-alert";
        else if (node.type == 'entitymap')
            return "glyphicon glyphicon-transfer";
        else if (node.type == 'onetomore')
            return "glyphicon glyphicon-leaf";
        else if (node.type == 'moretoone')
            return "glyphicon glyphicon-leaf";
        else if (node.type == 'moretomore')
            return "glyphicon glyphicon-leaf";
        else if (node.type == 'chart')
            return "glyphicon glyphicon-stats";
        else if (node.type == 'optionset')
            return '<i class="layui-icon">&#xe62a;</i>';
        else if (node.type == 'fieldpermission')
            return "glyphicon glyphicon-lock";
        else if (node.type == 'dashboard')
            return "glyphicon glyphicon-dashboard";
        else if (node.type == 'report')
            return '<i class="layui-icon">&#xe62c;</i>';
        else if (node.type == 'webresource')
            return "glyphicon glyphicon-globe";
        else if (node.type == 'workflow')
            return "glyphicon glyphicon-random";
        else if (node.type == 'autonumber')
            return "glyphicon glyphicon-font";
        else if (node.type == 'plugin')
            return "glyphicon glyphicon-inbox";
        else if (node.type == 'labels')
            return "glyphicon glyphicon-tags";
        else
            return "glyphicon glyphicon-folder-close";
    }
    window.customhomeCtrl = customhome;
    window.addShortDataAndRender = addShortDataAndRender;
    customhome.init();
});