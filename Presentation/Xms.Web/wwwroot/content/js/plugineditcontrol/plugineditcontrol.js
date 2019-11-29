(function ($) {
    $.pluginEditControl = function (args) {
        return new PluginEditControl(args);
    };
    PluginEditControl = function (args) {
        this._args = args
        this.id = Xms.Utility.Guid.NewGuid().ToString();
        this.template = [
            '<form id="form' + this.id + '">',
            '<div id="' + this.id + '" class="plugincontent">',
            '<ul class="nav nav-tabs" role="tablist">',
            '</ul>',
            '<div class="tab-content">',
            '</div>',
            '</div>',
            '</form>'
        ];
        this.args = {
            container: $('body'),
            data: [],
            beforehandData: {
                entityPlugins: [],
                queryViews: [],
                systemForms: []
            }
        };
        $.extend(this.args, args || {});
        this.listMethod = {
            entity: { type: 0, loadMethod: this.loadEntities, tag: this },
            workflow: { type: 1, loadMethod: this.loadFlowWorks, tag: this }
        };
        this.plugInType = [
            { id: 'Entity' + this.id, name: 'Entity', localizeName: '实体', instances: 'Xms.Plugin.Abstractions.IEntityPlugin', listMethod: this.listMethod.entity, primaryKey: 'entityid', typeCode: 0 },
            { id: 'List' + this.id, name: 'List', localizeName: '视图', instances: 'Xms.Plugin.Abstractions.IPlugin`2', listMethod: this.listMethod.entity, loadChildMethod: this.loadQueryViews, primaryKey: 'queryviewid', typeCode: 1 },
            { id: 'Form' + this.id, name: 'Form', localizeName: '表单', instances: 'Xms.Plugin.Abstractions.IPlugin`2', listMethod: this.listMethod.entity, loadChildMethod: this.loadSystemForms, primaryKey: 'systemformid', typeCode: 2 },
            { id: 'WorkFlow' + this.id, name: 'WorkFlow', localizeName: '流程', instances: 'Xms.Plugin.Abstractions.IPlugin`2', listMethod: this.listMethod.workflow, primaryKey: 'workflowid', typeCode: 3 }
        ];
        this.container = $(this.args.container);
        this.data = { Plugins: this.args.data, Entities: null, QueryViews: null, FlowWorks: null, SystemForms: null, BeforehandData: this.args.beforehandData };
        this.tabItems = [];
        this.control = this;
        this.beforehandControl = { PluginCollapse: [] };
        this.loadTabTotalCount = 0;
        this.loadTabOkCount = 0;
        this._create();
    }
    PluginEditControl.prototype = {
        _create: function () {
            var _this = this;
            _this.initBeforehandData();
            _this.el = $(_this.template.join('\n'));
            _this.navContainer = _this.el.find('.nav');
            _this.tabContentContainer = _this.el.find('.tab-content');
            _this.loadTab(_this.plugInType);
            _this.container.append(_this.el);
            $('a:first', _this.navContainer).tab('show');
        },
        refresh: function (data) {
            this._args.data = data;
            this.container.empty();
            return new PluginEditControl(this._args);
        },
        loadTab: function (data) {
            var _this = this;
            this.loadTabTotalCount = data.length;
            $(data).each(function (i, e) {
                _this.addTab(e);
            });
        },
        addTab: function (data) {
            var _this = this;
            var _navItem = new PluginEditNavItem(data);
            data.data = this.filterData(data);
            data.beforehandData = this.filterBeforehandData(data);
            var _tabContentItem = new PluginEditTabContentItem(this, data)
            this.navContainer.append(_navItem.el);
            this.tabContentContainer.append(_tabContentItem.el);
            var _item = { nav: _navItem, tab: _tabContentItem };
            this.tabItems.push(_item);
            var _sign = this.tabItems.indexOf(_item);

            data.listMethod.loadMethod(_sign, function (__sign, _data) {
                _this.tabItems[__sign].tab.loadPlugin(_data);
                _this.loadTabOkCount++;
                if (_this.loadTabTotalCount == _this.loadTabOkCount)
                    _this.handleLoadEnd();
            });
        },
        filterData: function (data) {
            var _fdata = [];
            $(this.data.Plugins).each(function (i, e) {
                if (e.plugininstances != null) {
                    $(e.plugininstances).each(function (_i, _e) {
                        if (_e.name == data.instances)
                            _fdata.push(e);
                    });
                }
            });
            return _fdata;
        },
        filterBeforehandData: function (data) {
            if (data.typeCode == 1)
                return this.data.BeforehandData.queryViews;
            else if (data.typeCode == 2)
                return this.data.BeforehandData.systemForms;
            else return;
        },
        request: function (id, operation, data) {
            var _this = this.tag || this;
            _this.requestList = _this.requestList || {}
            if (!_this.requestList.hasOwnProperty(id)) {
                _this.requestList[id] = [];
                operation(id, data);
            }
            _this.requestList[id].push(data);
        },
        loadEntities: function (sign, callback) {
            var _this = this.tag || this;
            var _rid = 'loadEntities';
            if (_this.data.Entities) {
                callback(sign, _this.data.Entities);
            } else {
                _this.request(_rid, function (rId, rData) {
                    $.ajaxSetup({ sign: rData.sign });
                    Xms.Schema.GetEntities({ getall: true }, function (data) {
                        if (!data || data.length == 0) return;
                        _this.data.Entities = data;
                        $(_this.requestList[rId]).each(function (i, e) {
                            rData.callback(e.sign, _this.data.Entities);
                        });
                    });
                }, { sign: sign, callback: callback });
            }
        },
        loadFlowWorks: function (sign, callback) {
            var _this = this.tag || this;
            if (_this.data.FlowWorks) {
                callback(sign, _this.data.FlowWorks);
            } else {
                $.ajaxSetup({ sign: sign });
                Xms.Schema.GetFlowWorks(null, function (data) {
                    if (data) {
                        _this.data.FlowWorks = data.items;
                        callback(this.sign, _this.data.FlowWorks);
                    }
                });
            }
        },
        loadQueryViews: function (entityid, callback) {
            var _this = this.control;

            if (_this.data.QueryViews && _this.data.QueryViews[entityid]) {
                callback(_this.data.QueryViews[entityid]);
            } else {
                Xms.Schema.GetQueryViews(entityid, function (data) {
                    if (data) {
                        _this.data.QueryViews = _this.data.QueryViews || {};
                        _this.data.QueryViews[entityid] = data.items;
                        callback(_this.data.QueryViews[entityid]);
                    }
                });
            }
        },
        loadSystemForms: function (entityid, callback) {
            var _this = this.control;
            if (_this.data.SystemForms && _this.data.SystemForms[entityid]) {
                callback(_this.data.SystemForms[entityid]);
            } else {
                Xms.Schema.GetSystemForms(entityid, function (data) {
                    if (data) {
                        _this.data.SystemForms = _this.data.SystemForms || {};
                        _this.data.SystemForms[entityid] = data.items;
                        callback(_this.data.SystemForms[entityid]);
                    }
                });
            }
        },
        serializeData: function () {
            var _jsons = { DeleteEntityPluginIds: null, EntityPlugins: [] };
            var _data = this.el.serializeFormJSON();
            console.log(_data)
            for (var key in _data) {
                if (key != 'DeleteEntityPluginIds') {
                    var _arr = key.split('_');
                    for (var i = 0; i < _data[_arr[0] + '_' + _arr[1] + '_EventName'].length; i++) {
                        var _json = {
                            EntityPluginId: _data[_arr[0] + '_' + _arr[1] + '_EntityPluginId'],
                            EntityId: _data[_arr[0] + '_' + _arr[1] + '_EntityId'],
                            TypeCode: _data[_arr[0] + '_' + _arr[1] + '_TypeCode'],
                            AssemblyName: _data[_arr[0] + '_' + _arr[1] + '_AssemblyName'],
                            ClassName: _data[_arr[0] + '_' + _arr[1] + '_ClassName'],
                            EventName: _data[_arr[0] + '_' + _arr[1] + '_EventName'][i]
                        };
                        if (_json.EntityPluginId == '')
                            _json.EntityPluginId = Xms.Utility.Guid.EmptyGuid.ToString();
                        _jsons.EntityPlugins.push(_json);
                    }
                    delete _data[_arr[0] + '_' + _arr[1] + '_EntityPluginId'];
                    delete _data[_arr[0] + '_' + _arr[1] + '_EntityId'];
                    delete _data[_arr[0] + '_' + _arr[1] + '_TypeCode'];
                    delete _data[_arr[0] + '_' + _arr[1] + '_AssemblyName'];
                    delete _data[_arr[0] + '_' + _arr[1] + '_ClassName'];
                    delete _data[_arr[0] + '_' + _arr[1] + '_EventName'];
                } else
                    _jsons.DeleteEntityPluginIds = [].concat(_data[key]);
            }

            return _jsons;
        },
        initBeforehandData: function () {
            var _this = this;
            for (var key in _this.data.BeforehandData.queryViews) {
                _this.data.QueryViews = _this.data.QueryViews || {};
                _this.data.QueryViews[key] = _this.data.BeforehandData.queryViews[key];
            }
            for (var key in _this.data.BeforehandData.systemForms) {
                _this.data.SystemForms = _this.data.SystemForms || {};
                _this.data.SystemForms[key] = _this.data.BeforehandData.systemForms[key];
            }
        },
        handleBeforehandControl: function () {
            var _this = this;
            $(_this.beforehandControl.PluginCollapse).each(function (i, e) {
                e.collapse('show');
            });
            $(_this.data.BeforehandData.entityPlugins).each(function (i, e) {
                var _input = $('input[name="' + e.classname + '_' + e.entityid + '_EventName' + '"][value="' + e.eventname + '"]');
                _input.parent().find('[name="' + e.classname + '_' + e.entityid + '_EntityPluginId' + '"]').val(e.entitypluginid);
                _input.parent().find('[name="DeleteEntityPluginIds"]').val(e.entitypluginid);
                _input.click();
            });
        },
        handleLoadEnd: function () {
            var _this = this;
            _this.handleBeforehandControl();
        }
    };
    PluginEditNavItem = function (args) {
        this.template = ['<li role="presentation"><a href="#' + args.id + '" aria-controls="' + args.id + '" role="tab" data-toggle="tab">' + args.localizeName + '</a></li>'];
        $.extend(this.args, args || {});
        this._create();
    }
    PluginEditNavItem.prototype = {
        _create: function () {
            this.el = $(this.template.join('\n'));
        }
    };
    PluginEditTabContentItem = function (control, args) {
        this.template = [
            '<div role="tabpanel" class="tab-pane" id="' + args.id + '">',
            '<div class="row m-0">',
            '<div class="col-sm-3 pluginnavs"></div>',
            '<div class="col-sm-9 plugincontents tab-content"></div>',
            '</div>',
            '</div>'
        ];
        this.templatePluginNav = ['<div class="pluginnav"><a class="btn btn-default" role="tab" data-toggle="tab"><div class="pluginnavicon"></div><span></span></a></div>'];
        this.templatePluginContent = [
            '<div role="tabpanel" class="tab-pane">',
            '<div class="plugincontent-table-thead" >',
            '<table class="table table-bordered table-thead">',
            '<thead><tr>',
            '<th class="table-thead-th1">',
            '<div class="input-group input-group-sm">',
            '<span class= "input-group-addon" >',
            '实体',
            '</span >',
            '<input type="text" class="form-control search-input" placeholder="搜索" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">',
            '<ul class="dropdown-menu">',
            '<li><a href="javascript:void(0);">请输入</a></li>',
            '</ul>',
            '</div>',
            '</th>',
            '<th>',
            '<div class="input-group input-group-sm">',
            '<span class= "input-group-addon" >',
            '事件',
            '</span >',
            '<div class="input-group-event-thead">',
            '</div>',
            '</div>',
            '</th>',
            '</tr></thead>',
            '</table>',
            '</div>',
            '<div class="plugincontent-table-container" >',
            '<table class="table table-bordered">',
            '<tbody class="entity-list">',
            '</tbody>',
            '</table>',
            '</div>',
            '</div>'
        ];
        this.templatePlugin = [
            '<tr>',
            '<td class="table-thead-td1">',
            '<div class="input-group input-group-sm">',
            '<span class= "input-group-addon">',
            '</span>',
            '<span class="entity-content" >',
            '<span class="entity"  data-toggle="collapse"  aria-expanded="false" aria-controls="collapseExample" >',
            '<span class="entity-text"></span>',
            '<span class="caret"></span>',
            '</span>',
            '</span>',
            '<div class="collapse" >',
            '<ul class="collapse-content">',
            '</ul>',
            '<div>',
            '</div>',
            '</div>',
            '</div>',
            '</td>',
            '<td class="event">',
            '<div class="input-group input-group-sm input-group-event">',
            '<span class= "input-group-addon">',
            '</span>',
            '<div class="input-event-all">',
            '<input class="deleteeventname" type="hidden" name="DeleteEntityPluginIds" disabled="disabled" />',
            '<input class="noteventname" type="hidden" name="EntityPluginId" disabled="disabled" />',
            '<input class="noteventname" type="hidden" name="EntityId" disabled="disabled" />',
            '<input class="noteventname" type="hidden" name="TypeCode" disabled="disabled"/>',
            '<input class="noteventname" type="hidden" name="AssemblyName" disabled="disabled" />',
            '<input class="noteventname" type="hidden" name="ClassName" disabled="disabled"/>',
            '<input class="eventname" type="checkbox" value="create"/>创建',
            '<input class="eventname" type="checkbox" value="update"/>修改',
            '<input class="eventname" type="checkbox" value="delete"/>删除',
            '<input class="eventname" type="checkbox" value="share"/>共享',
            '<input class="eventname" type="checkbox" value="assign"/>分派',
            '</div>',
            '</div>',
            '</td > ',
            '</tr>'
        ];
        this.templateCollapseItem = [
            '<li>',
            '<span class="collapse-item-text"></span>',
            '</li>'
        ];
        this.templateEventItem = [
            '<div class="input-event-item">',
            '<input class="deleteeventname" type="hidden" name="DeleteEntityPluginIds" disabled="disabled" />',
            '<input class="noteventname" type="hidden" name="EntityPluginId" disabled="disabled" />',
            '<input class="noteventname" type="hidden" name="EntityId" disabled="disabled" />',
            '<input class="noteventname" type="hidden" name="TypeCode" disabled="disabled"/>',
            '<input class="noteventname" type="hidden" name="AssemblyName" disabled="disabled" />',
            '<input class="noteventname" type="hidden" name="ClassName" disabled="disabled"/>',
            '<input class="eventname" type="checkbox" name="EventName" value="create"/>创建',
            '<input class="eventname" type="checkbox" name="EventName" value="update"/>修改',
            '<input class="eventname" type="checkbox" name="EventName" value="delete"/>删除',
            '<input class="eventname"  type="checkbox" name="EventName" value="share"/>共享',
            '<input class="eventname"  type="checkbox" name="EventName" value="assign"/>分派',
            '</div>'
        ];
        this.args = {
            data: []
        };
        $.extend(this.args, args || {});
        this.data = this.args.data;
        this.control = control;
        this.beforehandData = this.args.beforehandData;
        this.loadMethod = this.args.loadChildMethod;
        this.listType = this.args.listMethod.type;

        this.primaryKey = this.args.primaryKey;
        this.typeCode = this.args.typeCode;
        this._create();
    }
    PluginEditTabContentItem.prototype = {
        _create: function () {
            this.el = $(this.template.join('\n'));
            this.pluginNavs = this.el.find('.pluginnavs');
            this.pluginContents = this.el.find('.plugincontents');
            this.loadPluginNav(this.data);
            this.pluginContentTableContainer = this.el.find('.plugincontent-table-container');
            this.entityLists = this.el.find('.entity-list');
            this.pluginNavs.find('.pluginnav:first').addClass('active');
            this.pluginContents.find('.tab-pane:first').addClass('active');
            this.setSize();
        },
        getNewPluginNav: function (data, id) {
            var _temp = $(this.templatePluginNav.join('\n'));
            _temp.find('a').attr({ 'href': '#' + id, 'aria-controls': id });
            _temp.on('click', function () {
                $(this).addClass('active').siblings().removeClass('active');
            });
            _temp.find('span').text(data.plugin.name + '(' + data.plugin.namespace + ')');
            return _temp
        },
        getNewPluginContent: function (data, id) {
            var _this = this;
            var _temp = $(this.templatePluginContent.join('\n'));

            var _searchInput = _temp.find('.search-input');
            _searchInput.attr({ 'id': "search" + id });
            var _dropdownMenu = _temp.find('.dropdown-menu');
            _dropdownMenu.attr({ 'aria-labelledby': "search" + id });
            _searchInput.on('keyup', function () {
                _dropdownMenu.empty();
                var __val = $(this).val();
                if (__val && __val != "") {
                    var __data = _this.entityLists.find('.entity-text:contains(' + __val + ')');
                    if (__data.length == 0) {
                        _dropdownMenu.append('<li><a href="javascript:void(0);">请输入</a></li>');
                    } else {
                        __data.each(function (i, e) {
                            var _menu = $('<li><a href="#' + $(e).attr("id") + '">' + $(e).text() + '</a></li>');
                            _menu.on('click', 'a', function () {
                                $(this).parents('.dropdown-menu:first').prev().val($(this).text());
                                var _entityText = $($(this).attr("href"));
                                $('tr', _entityText.parents('.entity-list')).removeClass('active');
                                $(_entityText).parents('tr:first').addClass('active');
                            });
                            _dropdownMenu.append(_menu);
                        });
                    }
                } else
                    _dropdownMenu.append('<li><a href="javascript:void(0);">请输入</a></li>');
            });
            var _entityList = _temp.find('.entity-list');
            _entityList.data("plugin", data.plugin);
            _temp.attr({ 'id': id });
            return _temp
        },
        getNewPlugin: function (data, pluginData) {
            var _this = this;
            var _entityId = data.entityid;
            var _guid = Xms.Utility.Guid.NewGuid().ToString();
            var _id = "entityCollapse" + _guid;
            var _idText = "entityText" + _guid;
            var _temp = $(this.templatePlugin.join('\n'));
            var _entity = _temp.find('.entity');

            var _attr = {}
            var _text = "";
            var _entityText = _temp.find('.entity-text');
            if (_this.listType == this.control.listMethod.entity.type) {
                _attr = { 'data-relationship': data.name, 'value': data.entityid, 'href': '#' + _id };
                _text = data.localizedname;
            } else if (_this.listType == this.control.listMethod.workflow.type) {
                _attr = { 'data-relationship': data.name, 'value': data.workflowid, 'href': '#' + _id };
                _text = data.name;
            }
            _entity.attr(_attr);
            _entityText.text(_text).attr({ 'id': _idText });

            if (_this.loadMethod) {
                var _collapse = _temp.find('.collapse');
                var _collapseContent = _temp.find('.collapse-content');
                var _groupEvent = _temp.find('.input-group-event');
                var _eventCheckboxAll = _temp.find('.input-event-all input');
                _eventCheckboxAll.on('click', function () {
                    $(this).parent().nextAll().find('input[value="' + $(this).attr("value") + '"]').prop('checked', $(this).is(':checked'));
                    if ($(this).parent().nextAll().find('.eventname:checked').length > 0)
                        $(this).parent().nextAll().find('.noteventname').removeAttr('disabled');
                    else
                        $(this).parent().nextAll().find('.noteventname').attr('disabled', 'disabled');
                });
                _collapse.attr({ 'id': _id });

                _collapse.on('show.bs.collapse', function () {
                    var _isLoad = $(this).data('isLoad');
                    if (!_isLoad) {
                        $(this).data('isLoad', true);
                        _this.loadMethod(_entityId, function (_data) {
                            if (_data.length == 0) {
                                _collapseContent.append('<li style="text-indent:32px;">无数据</li>')
                            } else {
                                $(_data).each(function (i, e) {
                                    _collapseContent.append(_this.getNewCollapseItem(e, _this.primaryKey));
                                    _groupEvent.append(_this.getNewEventItem(e, _this.primaryKey, _this.typeCode, pluginData))
                                });
                            }
                        });
                    }
                    $(this).parents('td:first').next().find('.input-event-item').show(200);
                });
                _collapse.on('hide.bs.collapse', function () {
                    $(this).parents('td:first').next().find('.input-event-item').hide(200);
                });
                if (_this.isBeforehandData(_entityId))
                    _this.control.beforehandControl.PluginCollapse.push(_collapse);
            } else {
                var _eventCheckboxAll = _temp.find('.input-event-all input[type="checkbox"]');
                _eventCheckboxAll.attr({ name: "EventName" });
                var _inputs = _temp.find('.input-event-all input');
                _this.initInput(_inputs, data[_this.primaryKey], _this.typeCode, pluginData);
                _eventCheckboxAll.on('click', function () {
                    if ($(this).parent().find('.eventname:checked').length > 0)
                        $(this).parent().find('.noteventname').removeAttr('disabled');
                    else
                        $(this).parent().find('.noteventname').attr('disabled', 'disabled');
                    var deleteInput = $(this).siblings('.deleteeventname');
                    if (deleteInput.val() != null) {
                        if ($(this).is(':checked')) {
                            deleteInput.attr('disabled', 'disabled');
                        } else {
                            deleteInput.removeAttr('disabled');
                        }
                    }
                });
                _temp.find('.entity-checkbox-all').hide();
                _temp.find('.collapse').hide();
                _temp.find('.caret').hide();
            }
            return _temp
        },
        getNewCollapseItem: function (data, primaryKey) {
            var _temp = $(this.templateCollapseItem.join('\n'));
            var _text = _temp.find('.collapse-item-text');
            var _input = _temp.find('input');
            _input.attr({ name: data[primaryKey] });
            _input.val(data.entityid);

            _text.text(data.name);
            return _temp
        },
        getNewEventItem: function (data, primaryKey, typeCode, pluginData) {
            var _temp = $(this.templateEventItem.join('\n'));
            var _inputs = _temp.find('input');
            this.initInput(_inputs, data[primaryKey], typeCode, pluginData);

            _temp.find('.eventname').on('change', function () {
                if ($(this).parent().find('.eventname:checked').length > 0)
                    $(this).parent().find('.noteventname').removeAttr('disabled');
                else
                    $(this).parent().find('.noteventname').attr('disabled', 'disabled');
                var eventGroup = $(this).parents('.input-group-event:first');
                var val = $(this).val();
                if (eventGroup.find('.input-event-item .eventname[value="' + val + '"]').length == eventGroup.find('.input-event-item  .eventname[value="' + val + '"]:checked').length) {
                    eventGroup.find('.input-event-all .eventname[value="' + val + '"]').prop('checked', true);
                } else {
                    eventGroup.find('.input-event-all .eventname[value="' + val + '"]').prop('checked', false);
                }
                var deleteInput = $(this).siblings('.deleteeventname');
                if (deleteInput.val() != null) {
                    if ($(this).is(':checked')) {
                        deleteInput.attr('disabled', 'disabled');
                    } else {
                        deleteInput.removeAttr('disabled');
                    }
                }
            });
            return _temp;
        },

        loadPluginNav: function (data) {
            var _this = this;
            $(data).each(function (i, e) {
                _this.addPluginNav(e);
            });
        },
        addPluginNav: function (data) {
            var id = "pluginItem" + Xms.Utility.Guid.NewGuid().ToString();
            this.pluginNavs.append(this.getNewPluginNav(data, id));
            this.pluginContents.append(this.getNewPluginContent(data, id));
        },
        loadPlugin: function (data) {
            var _this = this;

            $(data).each(function (i, e) {
                _this.addPlugin(e);
            });
        },
        addPlugin: function (data) {
            var _this = this;
            $(this.entityLists).each(function () {
                $(this).append(_this.getNewPlugin(data, $(this).data('plugin')));
            });
        },
        initInput: function (inputs, id, typeCode, pluginData) {
            $(inputs).each(function () {
                var _name = $(this).attr('name');
                if (_name == 'DeleteEntityPluginIds')
                    return true;
                if (_name == 'EntityId')
                    $(this).val(id);
                if (_name == 'TypeCode')
                    $(this).val(typeCode);
                if (_name == 'AssemblyName')
                    $(this).val(pluginData.assembly.assemblyname);
                if (_name == 'ClassName')
                    $(this).val(pluginData.namespace + '.' + pluginData.name);
                $(this).attr({ name: pluginData.namespace + '.' + pluginData.name + '_' + id + '_' + _name });
            });
        },
        getBeforehandData: function (entityid) {
            if (this.beforehandData && this.beforehandData.hasOwnProperty(entityid))
                return this.beforehandData[entityid];
            else
                return null;
        },
        isBeforehandData: function (entityid) {
            if (this.beforehandData)
                return this.beforehandData.hasOwnProperty(entityid);
            else
                return false;
        },
        getMaxSize: function () {
            return { width: $(window).width(), height: $(window).height() };
        },
        setSize: function () {
            var size = this.getMaxSize();
            this.pluginNavs.css({ "height": (size.height - 245) + "px" });
            this.pluginContents.css({ "height": (size.height - 245) + "px" });
            this.pluginContentTableContainer.css({ "height": (size.height - 245 - 34.5) + "px" });
        },
    };
}(window.jQuery));