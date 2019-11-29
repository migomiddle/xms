(function ($) {
    $.advancedSearch = function (args) {
        return new AdvancedSearch(args);
    };
    $.advancedSearch.logging = true;
    AdvancedSearchFilterItem = function (advancedSearch, args) {
        this.advancedSearch = advancedSearch;
        this.id = Xms.Utility.Guid.NewGuid().ToString();
        this.isSelect = false;
        this.template = [
            '<div class="advancedsearch-filteritem" data-id="' + this.id + '">',
            '<div class="advancedsearch-line hide">',
            '</div>',
            '<div class="input-group">',
            '<span class="input-group-addon"><input type="checkbox" class="advancedsearch-filteritem-select" /></span>',
            '<div class="input-group-btn"><select class="form-control advancedsearch-filteritem-field" style="border-right:none;"><option value="">选择</option></select></div>',
            '<div class="input-group-btn"> <select class="form-control advancedsearch-filteritem-conditionoperator" style="border-right:none;"><option value="">选择</option></select></div>',
            '<div class="input-group-btn advancedsearch-filteritem-value" >',
            '<input class= "form-control" type = "text"  /> ',
            '</div>',
            '<span class="input-group-btn"><button type="button" class="btn btn-default advancedsearch-filteritem-delect"><i class="glyphicon glyphicon-trash"></i></button></span>',
            '</div>',
            '</div>'
        ];
        this.args = {
            fields: [],
            operators: []
        };
        this.type = 2;
        $.extend(this.args, args || {});
        this._create();
    }
    AdvancedSearchFilterItem.prototype = {
        getId: function (e) {
            return this.advancedSearch.getId(e);
        },
        getEl: function () {
            return this.filterItem;
        },
        getParentObjects: function (all) {
            var _group = this;
            var _as = _group.advancedSearch;
            var arr = [];
            $(_group.getParentEls(all)).each(function (i, e) {
                arr.push(_as.getItemObjectById(_group.getId(e)));
            });
            if (all)
                return arr;
            else
                return arr.length > 0 ? arr[0] : null;
        },
        getParentEls: function (all) {
            if (all)
                return this.filterItem.parents(".advancedsearch-filtergroup");
            else
                return this.filterItem.parents(".advancedsearch-filtergroup:first");
        },
        getSiblingEls: function () {
            return this.filterItem.siblings(".advancedsearch-filtergroup,.advancedsearch-filteritem");
        },
        getSiblingObjects: function () {
            var _item = this;
            var arr = [];
            $(_item.getSiblingEls()).each(function (i, e) {
                arr.push(_item.advancedSearch.getItemObjectById(_item.getId(e)));
            });
            return arr;
        },
        getData: function () {
            var _el = this.getEl();
            var fieldEl = _el.find(".advancedsearch-filteritem-field");
            var conditionoperatorEl = _el.find(".advancedsearch-filteritem-conditionoperator");
            var valueEl = _el.find(".advancedsearch-filteritem-value").children();
            return { field: fieldEl.val(), conditionoperator: conditionoperatorEl.val(), value: Xms.Web.getAttributePlugValue(valueEl), type: 2 }
        },
        verification: function () {
            var _data = this.getData();
            if (_data.field == '' || _data.conditionoperator == '') {
                this.getEl().addClass('has-error');
                return false;
            } else {
                this.getEl().removeClass('has-error');
                return true;
            }
        },
        remove: function (event) {
            event = event == null ? true : event;
            if (event) this.trigger("removeItem");
            this.filterItem.remove();
        },
        select: function (event) {
            event = event == null ? true : event;
            if (this.selectCheckbox.is(':checked')) {
                if (!this.isSelect) {
                    this.isSelect = true;
                    this.filterItem.addClass("active");
                    if (event) this.trigger("selectItem");
                }
            } else {
                if (this.isSelect) {
                    this.isSelect = false;
                    this.filterItem.removeClass("active");
                    if (event) this.trigger("selectItem");
                }
            }
        },
        unSelectControl: function (event) {
            this.selectCheckbox.prop('checked', false);
            this.select(event);
        },
        selectControl: function (event) {
            this.selectCheckbox.prop('checked', 'checked');
            this.select(event);
        },
        fieldChange: function () {
            var dataIndex = this.fieldDropdown.get(0).selectedIndex - 1;
            var field = dataIndex < 0 ? null : this.args.fields[dataIndex];
            var dataType = "primarykey" ? 'lookup' : e.attributetypename ? e.attributetypename : "nvarchar";
            var data = Xms.Fetch.ConditionOperators[dataType];
            this._conditionOperatorDropdownInit(data);
            this._valueInputInit(field);
        },
        conditionOperatorChange: function () {
            var condition = this.conditionOperatorDropdown.val();
            switch (condition) {
                case '12':
                case '13':
                    Xms.Web.emptyAttributePlugValue(this.valueInput);
                    Xms.Web.setAttributePlugState(this.valueInput.children(), 'readonly');
                    break;
                default:
                    Xms.Web.setAttributePlugState(this.valueInput.children());
                    break;
            }
            if (condition != '')
                this.getEl().removeClass('has-error');
        },
        trigger: function () {
            this.callListener("on" + arguments[0]);
            return this.advancedSearch.trigger.apply(this, arguments);
        },
        callListener: function (name) {
            name = name.toLowerCase();
            this.advancedSearch.log("looking for listener " + name);
            var listener = window[this.filterItem.data(name)];
            if (listener) {
                this.advancedSearch.log("calling listener " + name);
                var advancedSearch = this.advancedSearch;

                try {
                    var vret = listener(this);
                }
                catch (e) {
                    this.advancedSearch.log("exception calling listener " + name + ": ", e);
                }
            }
            else {
                this.advancedSearch.log("didn't find listener " + name);
            }
        },
        setLineStyle: function (line) {
            this.filterItem.addClass("before-hide");
            this.line.replaceWith(line);
            this.line = line;
        },
        setLastLineStyle: function () {
            this.line.css({ 'height': 0 + 'px', 'top': 0 + 'px' }).removeClass('hide');
        },
        _create: function () {
            this.filterItem = $(this.template.join('\n'));

            this.line = this.filterItem.find('.advancedsearch-line');
            this.selectCheckbox = this.filterItem.find('.advancedsearch-filteritem-select');
            this.fieldDropdown = this.filterItem.find('.advancedsearch-filteritem-field');
            this.conditionOperatorDropdown = this.filterItem.find('.advancedsearch-filteritem-conditionoperator');
            this.valueInput = this.filterItem.find('.advancedsearch-filteritem-value');
            this.delectButton = this.filterItem.find('.advancedsearch-filteritem-delect');

            this.selectCheckbox.click(this, this._handleSelectCheckboxClick);
            this.fieldDropdown.change(this, this._handleFieldDropdownChange);
            this.conditionOperatorDropdown.change(this, this._handleConditionOperatorChange);
            this.delectButton.click(this, this._handleDelectButtonClick);

            this._fieldDropdownInit();

            this._events = {};
        },
        _fieldDropdownInit: function () {
            var _item = this;
            $("option:gt(0)", _item.fieldDropdown).remove();
            $(_item.args.fields).each(function (i, e) {
                _item.fieldDropdown.append('<option value="' + e.name + '" >' + e.localizedname + '</option>')
            });
        },
        _conditionOperatorDropdownInit: function (data) {
            var _item = this;
            $("option:gt(0)", _item.conditionOperatorDropdown).remove();
            $(data).each(function (i, e) {
                _item.conditionOperatorDropdown.append('<option  value="' + e[1] + '">' + e[2] + '</option>')
            });
        },
        _valueInputInit: function (field) {
            this.valueInput.empty();
            var input = $('<input class="form-control" type="text"/>');
            if (field)
                input = Xms.Web.getAttributePlug(field);
            this.valueInput.append(input);
        },
        _onSelectCheckboxClick: function () {
            this.select();
        },
        _onConditionOperatorDropdownChange: function () {
            this.conditionOperatorChange();
        },
        _onFieldDropdownChange: function () {
            this.fieldChange();
        },
        _onDelectButtonClick: function () {
            this.remove();
        },
        _handleSelectCheckboxClick: function (event) {
            var filterItem = event.data;
            filterItem._onSelectCheckboxClick.call(filterItem);
        },
        _handleConditionOperatorChange: function (event) {
            var filterItem = event.data;
            filterItem._onConditionOperatorDropdownChange.call(filterItem);
        },
        _handleFieldDropdownChange: function (event) {
            var filterItem = event.data;
            filterItem._onFieldDropdownChange.call(filterItem);
        },
        _handleDelectButtonClick: function (event) {
            var filterItem = event.data;
            filterItem._onDelectButtonClick.call(filterItem);
        },
    }
    AdvancedSearchFilterGroup = function (advancedSearch, args) {
        this.advancedSearch = advancedSearch;
        this.id = Xms.Utility.Guid.NewGuid().ToString();

        this.template = [
            '<div class="advancedsearch-filtergroup" data-id="' + this.id + '">',
            '<div class="advancedsearch-line">',
            '</div>',
            '<div class="advancedsearch-relation">',
            '<div class="input-group">',
            '<span class="input-group-addon"><input type="checkbox" class="advancedsearch-relation-select"/></span>',
            '<div class="input-group-btn">',
            '<button type="button" class="btn btn-default advancedsearch-relation-dropdown" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false"><span class="advancedsearch-relation-dropdown-condition-text"></span><span class="caret"></span></button>',
            '<ul class="dropdown-menu">',
            '<li><a href="javascript:;" class="advancedsearch-selectgroup">选择组</a></li>',
            '<li><a href="javascript:;" class="advancedsearch-deletegroup">删除组</a></li>',
            '<li><a href="javascript:;" class="advancedsearch-addcondition">添加条件</a></li>',
            '<li><a href="javascript:;" class="advancedsearch-combinationand">组“和”</a></li>',
            '<li><a href="javascript:;" class="advancedsearch-combinationor">组“或”</a></li>',
            '</ul>',
            '</div>',
            '</div>',
            '</div>',

            '<div class="advancedsearch-filteritems">',
            '</div>',
            '</div>'
        ];
        this.args = {
            fields: [],
            operators: [],
            condition: 0
        };

        this.isSelect = true;
        this.type = 1;
        $.extend(this.args, args || {});
        this.condition = this.args.condition;
        this._create();
    }
    AdvancedSearchFilterGroup.prototype = {
        getId: function (e) {
            return this.advancedSearch.getId(e);
        },
        getEl: function () {
            return this.filterGroup;
        },
        getItemObjects: function (all) {
            var _group = this;
            var _as = _group.advancedSearch;
            var arr = [];
            $(_group.getItemEls(all)).each(function (i, e) {
                arr.push(_as.getItemObjectById(_group.getId(e)));
            });
            return arr;
        },
        getChildrenItemObjects: function () {
            var _group = this;
            var _as = _group.advancedSearch;
            var arr = [];
            $(_group.getChildrenEls()).each(function (i, e) {
                arr.push(_as.getItemObjectById(_group.getId(e)));
            });
            return arr;
        },
        getItemEls: function (all) {
            if (all)
                return this.filterItems.find(".advancedsearch-filteritem,.advancedsearch-filtergroup");
            else
                return this.filterItems.find(".advancedsearch-filteritem");
        },
        getChildrenEls: function () {
            return this.filterItems.children(".advancedsearch-filteritem,.advancedsearch-filtergroup");
        },
        getChildrenItemEls: function () {
            return this.filterItems.children(".advancedsearch-filteritem");
        },
        getFirstChildrenEl: function () {
            return this.filterItems.children(":first");
        },
        getLastChildrenEl: function () {
            return this.filterItems.children(":last");
        },
        getParentObjects: function (all) {
            var _group = this;
            var _as = _group.advancedSearch;
            var arr = [];
            $(_group.getParentEls(all)).each(function (i, e) {
                arr.push(_as.getItemObjectById(_group.getId(e)));
            });
            if (all)
                return arr;
            else
                return arr.length > 0 ? arr[0] : null;
        },
        getParentEls: function (all) {
            if (all)
                return this.filterGroup.parents(".advancedsearch-filtergroup");
            else
                return this.filterGroup.parents(".advancedsearch-filtergroup:first");
        },
        getSiblingEls: function () {
            return this.filterGroup.siblings(".advancedsearch-filtergroup,.advancedsearch-filteritem");
        },
        getSiblingObjects: function () {
            var _group = this;
            var _as = _group.advancedSearch;
            var arr = [];
            $(_group.getSiblingEls()).each(function (i, e) {
                arr.push(_as.getItemObjectById(_group.getId(e)));
            });
            return arr;
        },
        remove: function (event) {
            event = event == null ? true : event;
            if (event) this.trigger("removeGroup");
            this.filterGroup.remove();
        },
        select: function (event) {
            event = event == null ? true : event;
            if (this.selectCheckbox.is(':checked')) {
                if (!this.isSelect) {
                    this.isSelect = true;
                    this.filterGroup.addClass("active");
                    if (event) this.trigger("selectGroup");
                }
            } else {
                if (this.isSelect) {
                    this.isSelect = false;
                    this.filterGroup.removeClass("active");
                    if (event) this.trigger("selectGroup");
                }
            }
        },
        combination: function (condition, event) {
            event = event == null ? true : event;
            this.condition = condition;
            if (condition == 0) {
                this.combinationAndButton.hide();
                this.combinationOrButton.show();
                this.relationDropdownConditionText.text("和");
            }
            else if (condition == 1) {
                this.combinationAndButton.show();
                this.combinationOrButton.hide();
                this.relationDropdownConditionText.text("或");
            }
            if (event) this.trigger("combination");
        },
        unSelectControl: function (event) {
            this.selectCheckbox.prop('checked', false);
            this.select(event);
        },
        selectControl: function (event) {
            this.selectCheckbox.prop('checked', 'checked');
            this.select(event);
        },
        removeItemEl: function (el, siobj) {
            var _as = this.advancedSearch;
            if (this.getChildrenEls().length == 2) {
                var iobj = _as.getItemObjectByEl(el);
                siobj = iobj.obj.getSiblingObjects()[0];

                var ipobj = this.getParentObjects();

                if (ipobj) {
                    ipobj.obj.addItemEl(siobj.obj.filterItem);
                    _as.setItemParentObject(siobj.obj, ipobj.obj.id);
                    ipobj.obj.removeItemEl(this.filterGroup, siobj);
                } else {
                    _as.addItemEl(siobj.obj.filterItem);
                    _as.setItemParentObject(siobj.obj, _as.id);
                    this.filterGroup.remove();
                    siobj.obj.setLastLineStyle();
                }
                _as.deleteItemObject(this);
            }
            el.remove();
        },
        addItemEl: function (el) {
            this.filterItems.append(el);
        },
        addCondition: function (els) {
            var _group = this;
            var _as = _group.advancedSearch;
            var obj = null;
            if (!els) {
                obj = new AdvancedSearchFilterItem(_as, {
                    fields: _group.args.fields,
                    operators: _group.args.operators
                })
                _group.addItemEl(obj.filterItem);
                _as.setItemObject(obj, _group.id);
            } else {
                $(els).each(function (i, el) {
                    _group.addItemEl(el);
                    _as.setItemParentObjectEl(el, _group.id);
                });
            }
            _group.setLineStyle();
            _group.unSelectControl(false);
        },
        combinationAnd: function () {
            this.combination(0);
        },
        combinationOr: function () {
            this.combination(1);
        },
        trigger: function () {
            this.callListener("on" + arguments[0]);
            return this.advancedSearch.trigger.apply(this, arguments);
        },
        callListener: function (name) {
            name = name.toLowerCase();
            this.advancedSearch.log("looking for listener " + name);
            var listener = window[this.filterGroup.data(name)];
            if (listener) {
                this.advancedSearch.log("calling listener " + name);
                var advancedSearch = this.advancedSearch;

                try {
                    var vret = listener(this);
                }
                catch (e) {
                    this.advancedSearch.log("exception calling listener " + name + ": ", e);
                }
            }
            else {
                this.advancedSearch.log("didn't find listener " + name);
            }
        },
        setLineStyle: function () {
            var _group = this;
            var _as = _group.advancedSearch;
            var len = _group.getItemEls().length;
            //var height = len * 30 / 2 + 15;
            var fel = _group.getFirstChildrenEl();
            var lel = _group.getLastChildrenEl();
            var fheight = 30;
            var lheight = 30;
            var fobj = _as.getItemObjectByEl(fel).obj;
            var lobj = _as.getItemObjectByEl(lel).obj;
            if (fobj.type == 1) {
                fheight = fobj.getItemEls().length * 30;
                fobj.filterGroup.css({ 'height': fheight + 'px' });
            }
            if (lobj.type == 1) {
                lheight = lobj.getItemEls().length * 30;
                lobj.filterGroup.css({ 'height': lheight + 'px' });
            }
            var height = (len * 30) - (fheight / 2) - (lheight / 2);
            len = fel.height();
            var top = (len * 30 / 2) - ((len * 30 - height) / 2);

            fobj.line.css({ 'height': height + 'px', 'top': top + 'px' }).removeClass('hide');
            lobj.line.css({ 'height': 0 + 'px', 'top': 0 + 'px' }).removeClass('hide');
            $(_group.getChildrenItemEls()).each(function (i, e) {
                $(e).addClass('before-hide');
                _as.getItemObjectByEl(e).obj.line.removeClass('hide');
            });
            var ipobj = _group.getParentObjects();
            if (ipobj && ipobj.obj.type != 0) {
                ipobj.obj.setLineStyle();
            }
        },
        _create: function () {
            this.filterGroup = $(this.template.join('\n'));
            this.line = this.filterGroup.find('.advancedsearch-line');
            this.relation = this.filterGroup.find('.advancedsearch-relation');
            this.selectCheckbox = this.relation.find('.advancedsearch-relation-select');
            this.relationDropdown = this.relation.find('.advancedsearch-relation-dropdown');
            this.relationDropdownConditionText = this.relation.find('.advancedsearch-relation-dropdown-condition-text');
            this.selectGroupButton = this.relation.find('a.advancedsearch-selectgroup');
            this.deleteGroupButton = this.relation.find('a.advancedsearch-deletegroup');
            this.addConditionButton = this.relation.find('a.advancedsearch-addcondition');
            this.combinationAndButton = this.relation.find('a.advancedsearch-combinationand');
            this.combinationOrButton = this.relation.find('a.advancedsearch-combinationor');

            this.filterItems = this.filterGroup.find('.advancedsearch-filteritems');

            this.selectCheckbox.click(this, this._handleSelectCheckboxClick);
            this.selectGroupButton.click(this, this._handleSelectGroupButtonClick);
            this.deleteGroupButton.click(this, this._handleDeleteGroupButtonClick);
            this.addConditionButton.click(this, this._handleAddConditionClick);
            this.combinationAndButton.click(this, this._handleCombinationAndClick);
            this.combinationOrButton.click(this, this._handleCombinationOrClick);
            this._events = {};
            this.combination(this.condition, false);
        },
        _onSelectCheckboxClick: function () {
            this.select()
        },
        _onSelectGroupButtonClick: function () {
            this.selectControl()
        },
        _onDeleteGroupButtonClick: function () {
            this.remove()
        },
        _onAddConditionClick: function () {
            this.addCondition();
        },
        _onCombinationAndClick: function () {
            this.combinationAnd();
        },
        _onCombinationOrClick: function () {
            this.combinationOr();
        },
        _handleSelectCheckboxClick: function (event) {
            var filterGroup = event.data;
            filterGroup._onSelectCheckboxClick.call(filterGroup);
        },
        _handleSelectGroupButtonClick: function (event) {
            var filterGroup = event.data;
            filterGroup._onSelectGroupButtonClick.call(filterGroup);
        },
        _handleDeleteGroupButtonClick: function (event) {
            var filterGroup = event.data;
            filterGroup._onDeleteGroupButtonClick.call(filterGroup);
        },
        _handleAddConditionClick: function (event) {
            var filterGroup = event.data;
            filterGroup._onAddConditionClick.call(filterGroup);
        },
        _handleCombinationAndClick: function (event) {
            var filterGroup = event.data;
            filterGroup._onCombinationAndClick.call(filterGroup);
        },
        _handleCombinationOrClick: function (event) {
            var filterGroup = event.data;
            filterGroup._onCombinationOrClick.call(filterGroup);
        }
    }
    AdvancedSearch = function (args) {
        this.id = Xms.Utility.Guid.NewGuid().ToString();
        /* TEMPLATE */
        this.args = {
            fields: [],
            operators: [],
            size: { width: 800, height: 500 },
            fullScreen: false,
            show: false
        };
        this.template = [
            '<div class="modal fade advancedsearch" id="' + this.id + '">',
            '<div class="modal-dialog advancedsearch-dialog">',
            '<div class="modal-content advancedsearch-content">',
            '<div class="modal-header advancedsearch-header">',
            '<button type="button" class="close advancedsearch-close"  data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>',
            '<button type="button" class="btn btn-link advancedsearch-fullscreen"><i class="layui-icon layui-icon-screen-full"></i></button>',
            '<h4 class="modal-title advancedsearch-title"><i class="layui-icon layui-icon-senior"></i>  高级查找</h4>',
            '</div>',
            '<div class="modal-body advancedsearch-body" style="padding:5px">',
            '<div class="btn-group btn-group-sm advancedsearch-tool-group" role="group" aria-label="...">',
            '<button type="button" class="btn btn-default advancedsearch-addcondition"><i class="glyphicon glyphicon-plus"></i>&nbsp;添加条件</button>',
            '<button type="button" class="btn btn-default advancedsearch-combinationand"><span>&&</span>&nbsp;组“和”</button>',
            '<button type="button" class="btn btn-default advancedsearch-combinationor"><span>||</span>&nbsp;组“或”</button>',
            '<button type="button" class="btn btn-default advancedsearch-search"><i class="glyphicon glyphicon-search"></i>&nbsp;查找</button>',
            '</div>',
            '<div class= "advancedsearch-filtercontainer" style="overflow:auto">',
            '</div>',
            '</div>',
            '</div>',
            '</div>',
            '</div>',
        ];

        this.items = {};
        this.type = 0;

        $.extend(this.args, args || {});
        this.isFullScreen = this.args.fullScreen;
        this.isSuccess = true;
        this._create();
    }
    AdvancedSearch.prototype = {
        log: function () {
            if (!window.console || !$.advancedSearch.logging) { return; }
            var prepend = 'advancedsearch "' + this.el.id + ': ';
            var args = [prepend];
            args.push.apply(args, arguments);
            console.log.apply(console, args);
        },
        getId: function (e) {
            if (e) return $(e).attr('data-id');
            else return this.id;
        },
        getIds: function (data) {
            var _as = this;
            var arr = [];
            $(data).each(function (i, e) {
                arr.push(_as.getId(e));
            });
            return arr;
        },
        hasItemObject: function (id) {
            return this.items.hasOwnProperty(id);
        },
        deleteItemObject: function (obj) {
            var _this = this;
            if (obj.type == 1) {
                $(_this.getItemObjects(obj.id, true)).each(function (i, e) {
                    _this.deleteItemObject(e.obj.id);
                });
            }
            delete _this.items[obj.id];
        },
        setItemObject: function (obj, pid) {
            this.items[obj.id] = { parent: pid, obj: obj };
        },
        getItemObjects: function (id, all) {
            var _this = this;
            if (!id) id = _this.id;
            var arr = [];

            $.each(_this.items, function (i, e) {
                if (e.parent == id) {
                    arr.push(e);
                    if (all) {
                        if (e.obj.type == 1) {
                            arr = arr.concat(e.obj.getItemObjects(e.obj.id, true));
                        }
                    }
                }
            });
            return arr;
        },
        getChildrenItemObjects: function () {
            return this.getItemObjects(this.id);
        },
        getItemObjectById: function (id) {
            return this.items[id];
        },
        getItemObjectByIds: function (ids) {
            var _as = this;
            var arr = [];
            $(ids).each(function (i, e) {
                arr.push(_as.getItemObjectById(e));
            });
            return arr;
        },
        getElById: function (id) {
            return this.getItemObjectById(id).obj.getEl();
        },
        getElByIds: function (ids) {
            var _as = this;
            var arr = [];
            $(ids).each(function (i, e) {
                arr.push(_as.getElById(e));
            });
            return arr;
        },
        getItemObjectByEl: function (el) {
            return this.getItemObjectById(this.getId(el));
        },
        setItemParentObject: function (obj, pid) {
            this.items[obj.id].parent = pid;
        },
        setItemParentObjectEl: function (el, pid) {
            this.setItemParentObject(this.getItemObjectByEl(el).obj, pid);
        },
        getItemParentObject: function (obj) {
            return this.items[this.items[obj.id].parent];
        },
        getItemParentObjectByEl: function (el) {
            return this.getItemParentObject(this.getItemObjectByEl(el).obj);
        },
        getChildrenEls: function (container) {
            container = container == null ? this.filterContainer : container;
            return container.children(".advancedsearch-filteritem,.advancedsearch-filtergroup");
        },
        getChildrenItemEls: function () {
            return this.filterContainer.children(".advancedsearch-filteritem");
        },
        getSelectGroupEls: function (container) {
            container = container == null ? this.filterContainer : container;
            return container.find('.advancedsearch-filtergroup.active');
        },
        getFirstSelectGroupEl: function (container) {
            return this.getSelectGroupEls(container).first();
        },
        getSelectItemEls: function (container) {
            container = container == null ? this.filterContainer : container;
            return container.find('.advancedsearch-filteritem.active');
        },
        getFirstSelectItemEl: function () {
            return this.getSelectItemEls().first();
        },
        group: function (arr, arr1, groups) {
            var _as = this;
            groups = groups == null ? { groups: [], items: arr1 } : groups;
            var group = [];
            if (arr.length > 0) {
                var fobj = _as.getItemObjectById(arr[0]).obj;
                var fgroupEl = fobj.filterGroup;
                var groupItem = { id: arr[0], items: [] };
                $(_as.getSelectItemEls(fgroupEl)).each(function (i, e) {
                    var id = _as.getId(e);
                    var index = arr1.indexOf(id);
                    if (index != -1) {
                        groupItem.items.push(id);
                        arr1.splice(index, 1);
                    }
                });
                group.push(groupItem);
                arr.splice(0, 1);
                $(_as.getSelectGroupEls(fgroupEl)).each(function (i, e) {
                    var id = _as.getId(e);
                    groupItem = { id: id, items: groupItem.items };
                    group.push(groupItem);
                    var index = arr.indexOf(id);
                    if (index != -1) {
                        arr.splice(index, 1);
                    }
                });
                groups.groups.push(group);
                groups.items = arr1;
                if (arr.length > 0) {
                    groups = _as.group(arr, arr1, groups);
                }
            }

            return groups;
        },
        verificationCombination: function (callback) {
            var _as = this;
            var pobj = _as;
            var sItems = _as.getSelectItemEls();
            var sLen = sItems.length;
            if (sLen < 2)
                return false;
            var fsItem = _as.getFirstSelectItemEl();
            var activeSiblingsLen = fsItem.siblings('.active').length;
            if (sLen == activeSiblingsLen + 1) {
                var siblingsLen = fsItem.siblings().length;
                var parent = _as.getItemParentObjectByEl(fsItem);
                if (parent != null) pobj = parent.obj;
                if (pobj.type == 0 || activeSiblingsLen != siblingsLen) {
                    callback && callback(pobj, { groups: [_as.getIds(_as.getSelectItemEls())], items: [] });
                    return pobj;
                }
            } else {
                var sGroups = _as.getSelectGroupEls();
                var sGroupIds = _as.getIds(sGroups);
                var sItemIds = _as.getIds(sItems);
                if (sGroupIds.length > 0) {
                    var data = _as.group(sGroupIds, sItemIds);
                    var gData = data.groups;
                    var iData = data.items;
                    if (gData.length + iData.length > 1) {
                        var fgobj = _as.getItemObjectById(gData[0][0].id);
                        var parent = fgobj.parent;
                        var bool = true;
                        $(gData).each(function (i, e) {
                            if (parent != _as.getItemObjectById(e[0].id).parent) {
                                bool = false;
                                return false;
                            }
                        });
                        if (!bool)
                            return false;
                        else {
                            if (iData.length > 0) {
                                $(iData).each(function (i, e) {
                                    if (parent != _as.getItemObjectById(e).parent) {
                                        bool = false;
                                        return false;
                                    }
                                });
                                if (!bool)
                                    return false;
                            }

                            var piobj = _as.getItemObjectById(parent);
                            var pobj = piobj == null ? this : piobj.obj;
                            callback && callback(pobj, data);
                            return pobj
                        }
                    }
                }
            }
        },
        addItemEl: function (el) {
            this.filterContainer.append(el);
        },
        addCondition: function () {
            var item = new AdvancedSearchFilterItem(this, {
                fields: this.args.fields,
                operators: this.args.operators
            });
            this.filterContainer.append(item.filterItem);
            this.items[item.id] = { parent: this.id, obj: item };
        },
        combination: function (condition) {
            var _as = this;
            _as.verificationCombination(function (_pobj, _data) {
                var group = new AdvancedSearchFilterGroup(_as, {
                    fields: _as.args.fields,
                    operators: _as.args.operators,
                    condition: condition
                });
                var gData = _data.groups;
                var iData = _data.items;
                $(gData).each(function (i, e) {
                    var items = _as.getElByIds(gData.length == 1 && iData.length == 0 ? e : [e[0].id]);
                    group.addCondition(items);
                });
                if (iData.length > 0) {
                    var items = _as.getElByIds(iData);
                    group.addCondition(items);
                }
                _pobj.addItemEl(group.filterGroup);
                _as.setItemObject(group, _pobj.id);
                group.setLineStyle();
                group.selectControl(false);
            });
        },
        combinationAnd: function () {
            this.combination(0);
        },
        combinationOr: function () {
            this.combination(1);
        },
        serialize: function (isVerification, data, filters) {
            var _as = this;
            filters = filters || Xms.Fetch.FilterExpression();
            data = data || this.getData(isVerification);
            $(data).each(function (i, e) {
                if (e.type == 1) {
                    var _filters = Xms.Fetch.FilterExpression(e.condition);
                    filters.Filters.push(_as.serialize(isVerification, e.items, _filters));
                } else if (e.type == 2) {
                    var condition = Xms.Fetch.ConditionExpression();
                    condition.AttributeName = e.field
                    condition.Operator = e.conditionoperator;
                    condition.Values.push(e.value);
                    filters.Conditions.push(condition);
                }
            });
            return filters;
        },
        deserialize: function () { },
        getData: function (isVerification, parent) {
            var _as = this;
            var _arr = [];
            $(_as.getChildrenEls(parent)).each(function (i, e) {
                var obj = _as.getItemObjectByEl(e).obj;
                if (obj.type == 1) {
                    _arr.push({ condition: obj.condition, items: _as.getData(isVerification, obj.filterItems), type: 1 });
                } else if (obj.type == 2) {
                    var _data = obj.getData();
                    if (isVerification) {
                        var _verification = obj.verification();
                        if (_verification == false)
                            _as.setSuccessState(false);
                    }
                    _arr.push(_data);
                }
            });
            return _arr;
        },
        verification: function () {
            var _as = this;
            _as.isSuccess = true;
            var _data = _as.getData(true);
            var _success = _as.isSuccess;
            return { data: _data, success: _success };
        },
        setSuccessState: function (state) {
            this.isSuccess = state == null ? true : state;
        },
        search: function (event) {
            event = event == null ? true : event;
            if (event) this.trigger("search");
        },
        on: function (name, fn) {
            this._events[name] = fn;
            return this;
        },
        trigger: function () {
            this.advancedSearch = this.advancedSearch || this;
            var name = arguments[0];
            var args = Array.prototype.slice.call(arguments);
            args.shift();
            args.unshift(this);
            this.advancedSearch.log("firing event " + name);
            var handler = this._events[name];
            if (handler === undefined && this.advancedSearch !== undefined) {
                handler = this.advancedSearch._events[name];
            }
            var ret = null;

            if (typeof (handler) == "function") {
                this.advancedSearch.log("found event handler, calling " + name);
                try {
                    ret = handler.apply(this, args);
                }
                catch (e) {
                    this.advancedSearch.log("event handler " + name + " had an exception");
                }
            }
            else {
                this.advancedSearch.log("couldn't find an event handler for " + name);
            }
            return ret;
        },
        getMaxSize: function () {
            return { width: $(window).width(), height: $(window).height() };
        },
        getSize: function (size) {
            var _maxSize = this.getMaxSize();
            var _width = size.width;
            var _height = size.height;
            if (_width > _maxSize.width)
                _width = _maxSize.width;
            if (_height > _maxSize.height)
                _height = _maxSize.height;
            return { width: _width, height: _height };
        },
        setSize: function (size) {
            size = this.getSize(size);
            this.dialog.css({ "width": (size.width - 34) + "px", "height": (size.height - 60) + "px" });
            this.content.css({ "width": (size.width - 34) + "px", "height": (size.height - 60) + "px" });
            this.filterContainer.css({ "width": (size.width - 34 - 7) + "px", "height": (size.height - 40 - 65 - 5 - 20 - 2) + "px" });
        },
        fullScreen: function () {
            this.setSize({ width: $(window).width(), height: $(window).height() });
            this.fullScreenButtonIcon.removeClass("layui-icon-screen-full").addClass("layui-icon-screen-restore");
            this.isFullScreen = true;
        },
        restoreScreen: function () {
            this.setSize(this.args.size);
            this.fullScreenButtonIcon.removeClass("layui-icon-screen-restore").addClass("layui-icon-screen-full");
            this.isFullScreen = false;
        },
        close: function () {
            this.modal.modal('hide');
        },
        open: function () {
            this.modal.modal('show');
        },
        _create: function () {
            this.el = $(this.template.join('\n'));
            $('body').append(this.el);
            this.modal = this.el.modal({
                keyboard: this.args.keyboard,
                show: this.args.show,
                backdrop: this.args.backdrop
            });
            this.title = this.modal.find('.advancedsearch-title');

            this.dialog = this.modal.find('.advancedsearch-dialog');
            this.content = this.modal.find('.advancedsearch-content');
            this.header = this.modal.find('.advancedsearch-header');
            this.closeButton = this.modal.find('button.advancedsearch-close');
            this.fullScreenButton = this.modal.find('button.advancedsearch-fullscreen');
            this.fullScreenButtonIcon = this.fullScreenButton.find('i');
            this.body = this.modal.find('.advancedsearch-body');
            this.toolGroup = this.modal.find('.advancedsearch-tool-group');
            this.addConditionButton = this.modal.find('button.advancedsearch-addcondition');
            this.combinationAndButton = this.modal.find('button.advancedsearch-combinationand');
            this.combinationOrButton = this.modal.find('button.advancedsearch-combinationor');
            this.searchButton = this.modal.find('button.advancedsearch-search');

            this.filterContainer = this.modal.find('.advancedsearch-filtercontainer');
            this.fullScreenButton.click(this, this._handleFullScreenClick);
            this.addConditionButton.click(this, this._handleAddConditionClick);
            this.combinationAndButton.click(this, this._handleCombinationAndClick);
            this.combinationOrButton.click(this, this._handleCombinationOrClick);
            this.searchButton.click(this, this._handleSearchClick);

            this._events = {};

            this.on('selectItem', function (e) {
                var piobj = e.getParentObjects();
                if (piobj && piobj.obj.type != 0) {
                    if (e.isSelect && e.filterItem.siblings(':not(.active)').length == 0) {
                        piobj.obj.selectControl(false);
                        piobj.obj.advancedSearch._recursionSelectParentControl(piobj.obj);
                    } else {
                        piobj.obj.unSelectControl(false);
                        $(e.getParentObjects(true)).each(function (i, iobj) {
                            iobj.obj.unSelectControl(false);
                        });
                    }
                }
            });
            this.on('selectGroup', function (e) {
                $(e.getItemObjects(true)).each(function (i, iobj) {
                    if (e.isSelect) {
                        iobj.obj.selectControl(false);
                        iobj.obj.advancedSearch._recursionSelectParentControl(iobj.obj);
                    } else {
                        iobj.obj.unSelectControl(false);
                        $(e.getParentObjects(true)).each(function (i, piobj) {
                            piobj.obj.unSelectControl(false);
                        });
                    }
                });
            });
            this.on('removeItem', function (e) {
                var _as = e.advancedSearch;
                var allPiobj = e.getParentObjects(true);
                var piobj = e.getParentObjects();
                if (piobj && piobj.obj.type == 1) {
                    piobj.obj.removeItemEl(e.filterItem);
                }
                _as.deleteItemObject(e);
                $(allPiobj).each(function (_i, _e) {
                    if (_as.hasItemObject(_e.obj.id)) {
                        if (_e.obj.type == 1)
                            _e.obj.setLineStyle();
                    }
                });
            });
            this.on('removeGroup', function (e) {
                var _as = e.advancedSearch;
                var allPiobj = e.getParentObjects(true);
                var piobj = e.getParentObjects();
                if (piobj && piobj.obj.type == 1) {
                    piobj.obj.removeItemEl(e.filterGroup);
                }
                _as.deleteItemObject(e);
                $(allPiobj).each(function (_i, _e) {
                    if (_as.hasItemObject(_e.obj.id)) {
                        if (_e.obj.type == 1)
                            _e.obj.setLineStyle();
                    }
                });
            });

            this.on('search', function (e) {
                var _as = e.advancedSearch;
                var _data = _as.verification();
                if (_data.success) {
                    var _filters = _as.serialize(false, _data.data);
                    _as.args.searchCallback && _as.args.searchCallback(_as, _filters);
                    _as.close()
                }
            });

            if (this.isFullScreen)
                this.fullScreen();
            else
                this.setSize(this.args.size);
        },
        _recursionSelectParentControl: function (obj) {
            if (obj.getEl().siblings(':not(.active)').length == 0) {
                var piobj = obj.getParentObjects();
                if (piobj) {
                    piobj.obj.selectControl(false);
                    this._recursionSelectParentControl(piobj.obj);
                }
            }
        },
        _onFullScreenClick: function () {
            if (!this.isFullScreen)
                this.fullScreen();
            else
                this.restoreScreen();
        },
        _onAddConditionClick: function () {
            this.addCondition();
        },
        _onCombinationAndClick: function () {
            this.combinationAnd();
        },
        _onCombinationOrClick: function () {
            this.combinationOr();
        },
        _onSearchClick: function () {
            this.search();
        },
        _handleFullScreenClick: function (event) {
            var filter = event.data;
            filter._onFullScreenClick.call(filter);
        },
        _handleAddConditionClick: function (event) {
            var filter = event.data;
            filter._onAddConditionClick.call(filter);
        },
        _handleCombinationAndClick: function (event) {
            var filter = event.data;
            filter._onCombinationAndClick.call(filter);
        },
        _handleCombinationOrClick: function (event) {
            var filter = event.data;
            filter._onCombinationOrClick.call(filter);
        },
        _handleSearchClick: function (event) {
            var filter = event.data;
            filter._onSearchClick.call(filter);
        },
    }
}(window.jQuery));