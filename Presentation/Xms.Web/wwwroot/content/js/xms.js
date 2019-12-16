//@ sourceURL=Xms.js
; (function (root) {
    if (typeof (root.Xms) == "undefined") {
        root.Xms = { __namespace: true };
    }
    if (typeof (Xms.Global) == "undefined") {
        Xms.Global = function () { };
    }

    if (typeof (Xms.Page) == "undefined") {
        Xms.Page = function () { };
        Xms.Page = {
            formHelper: $('#editForm')
            , getAttribute: function (name, context) {
                var input = $('input[name="' + name + '"]', context || '#editForm');
                var attr = new Xms.Page.Attribute();
                attr.Target = input;
                attr.Name = name;

                return attr;
            }
            , getControl: function (name) {
                var $this = this.formHelper.find('[name="' + name + '"]');
                var type = $this.attr('data-controltype');
                if (type == 'subgrid') {
                    $this.refresh = function () {
                        var func = getFunction($this.attr('data-refresh'));
                        func.call(this, $this);
                        return $this;
                    };
                    $this.setfilter = function (filter) {
                        $this.attr('data-filter', encodeURIComponent(JSON.stringify(filter)));
                        return $this;
                    };
                    $this.getfilter = function () {
                        var a = $this.attr('data-filter');
                        if (a) {
                            return JSON.parse(decodeURIComponent(a));
                        }
                        return null;
                    };
                    $this.seteditable = function (flag) {
                        $this.attr('data-editable', flag + '');
                        //this.refresh();
                        if (flag) {
                            $this.find('input,select,button,textarea').removeProp('disabled');
                            $this.find('.toolbar').find('.btn').removeProp('disabled');
                        }
                        else {
                            $this.find('input,select,button,textarea').prop('disabled', 'disabled');
                            $this.find('.toolbar').find('.btn').attr('disabled', 'disabled');
                        }
                        return $this;
                    };
                    $this.iseditable = function () {
                        return $this.attr('data-editable');
                    };
                    $this.getRows = function (index) {
                        var res = (typeof index === undefined) ? $this.find('tr.editrow:eq(' + index + ')') : $this.find('tr.editrow');
                        return res;
                    };
                    $this.getCells = function (index) {
                        return $this.find('tr.editrow').find('td:eq(' + index + ')');
                    };
                    $this.getInput = function (name) {
                        return $this.find('input[name="' + name + '"]');
                    }
                    $this.getInputByIndex = function (index) {
                        return $this.getCells(index).find("input[type='text']");
                    }
                    $this.bindChange = function (index, func, bool) {
                        if (bool) {
                            $this.getInputByIndex(index).unbind("change").bind("change", function (e) {
                                func.call(this, e);
                            });
                        } else {
                            $this.getInputByIndex(index).bind("change", function (e) {
                                func.call(this, e);
                            });
                        }
                        return $this;
                    };
                }
                else if (type == 'lookup' || type == 'owner' || type == 'customer') {
                    $this.setfilter = function (filter) {
                        $this.attr('data-filter', encodeURIComponent(JSON.stringify(filter)));
                        return $this;
                    };
                    $this.getfilter = function () {
                        var a = $this.attr('data-filter');
                        if (a) {
                            return JSON.parse(decodeURIComponent(a));
                        }
                        return null;
                    };
                }
                return $this;
            }
            , getValue: function (name) {
                var $this = this.formHelper.find('[name="' + name + '"]');
                var result = $this.val();
                if (result == '') return null;
                var type = $this.attr('data-controltype');
                if (type == 'lookup' || type == 'owner' || type == 'customer') {
                    result = {};
                    result.id = $this.val();
                    result.name = this.formHelper.find('[name="' + name + '_text"]').val();
                }
                return result;
            }
            , setValue: function (name, value) {
                var $this = this.formHelper.find('[name="' + name + '"]');
                var type = $this.attr('data-controltype');
                var dataformat = $this.attr('data-format') || '';
                //console.log(type, dataformat, type == 'ntext' && dataformat == 'email');
                if (type == 'lookup' || type == 'owner' || type == 'customer') {
                    if (value != null) {
                        $this.val(value.id).trigger('change');
                        var textInput = this.formHelper.find('[name="' + name + '_text"]');
                        if (textInput.siblings('.xms-drlinki').length > 0) {
                            textInput.siblings('.xms-drlinki').remove();
                        }
                        textInput.val(value.name).trigger('change');
                        $.fn.xmsSelecteDown.setLookUpState(textInput);
                    }
                    else {
                        $this.val('').trigger('change');
                        this.formHelper.find('[name="' + name + '_text"]').val('').trigger('change');
                        $this.parent().find('.xms-drlinki').parent('a').remove();
                    }
                } else if (type == 'picklist') {
                    $this.next().val(value).trigger('change');
                } else if (type == 'status' || type == 'bit') {
                    var _input = this.formHelper.find('[name="' + name + '"]'),
                        _radios = _input.parent().find('input[type="radio"]');
                    //console.log('radios', _radios);
                    _radios.prop('checked', false);
                    _input.parent().find('input[type="radio"][value="' + value + '"]').prop('checked', true);
                    $this.val(value).trigger('change');
                }
                else if (type == 'ntext' && dataformat == 'email') {
                    console.log(11111111111111);
                    var $_id = $('#' + name);
                    if ($_id.length > 0) {
                        $_id.data().ue.ready(function () { $_id.data().ue.setContent(value) });
                    }
                }
                else {
                    $this.val(value).trigger('change');
                }
            }
            , setRequiredLevel: function (name, level) {
                this.formHelper.find('[name=' + name + ']').addClass(level);
            }
            , setRequired: function (name) {
                this.setRules(name, { required: true });
                var _label = this.formHelper.find('[name=' + name + ']').parents('.form-lists:first').find('label');
                this.formHelper.find('[name=' + name + ']').addClass('required');
                if (_label.find('span').length == 0) {
                    _label.append('<span style="color:red;font-weight:bolder;">*</span>');
                }
            }
            , removeRequired: function (name) {
                this.removeRules(name, 'required');
                var _label = this.formHelper.find('[name=' + name + ']').parents('.form-lists:first').find('label');
                this.formHelper.find('[name=' + name + ']').trigger('blur').removeClass('required');
                if (_label.find('span').length > 0) {
                    _label.find('span').remove();
                }
            }
            , setRules: function (name, rules) {
                this.formHelper.find('[name=' + name + ']').rules('add', rules);
            }
            , setRange: function (name, min, max) {
                this.setRules(name, [min, max]);
            }
            , removeRules: function (name, rules) {
                if (rules) {
                    this.formHelper.find('[name=' + name + ']').rules('remove', rules);
                } else {
                    this.formHelper.find('[name=' + name + ']').rules('remove');
                }
            }
            , setDisabled: function (name, flag) {
                var $this = this.formHelper.find('[name=' + name + ']');
                var type = $this.attr('data-controltype');
                var _format = $this.attr('data-format');
                if (type == 'lookup' || type == 'owner' || type == 'customer') {
                    if (flag) {
                        $this.each(function () {
                            $(this).parent().find('input,button,select,textarea,radio,checkbox').prop('disabled', true);
                        });
                    } else {
                        $this.prev().find('input,button,select,textarea,radio,checkbox').prop('disabled', 'disabled');
                    }
                } else if (type == 'ntext' && _format && _format == 'email') {
                    if ($this.data().ue) {
                        $this.data().ue.ready(function () {
                            //不可编辑
                            $this.data().ue.setDisabled();
                        });
                    }
                }
                else {
                    $this.prop('disabled', 'disabled');
                }
                return $this;
            }
        };
    }
    (function () {
        function GridViewMethod() {
            var self = this;
            //获取行数据
            //gridname:单据体名 arg: TR或TR内的jq对象/第几行/记录id

            this.$grid = null
            this._init = function () {
            }
            this.getRowData = function (gridname, arg) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname);
                if (typeof arg == 'object') { //如果是一个JQ对象，返回当前对象的行数据
                    if (arg.length > 0) {
                        if ($(arg).get(0).nodeName == 'Tr') {
                            var index = $(arg).index() - 1;
                            return self.$datagrid.cDatagrid('getRowData', index);
                        } else {
                            var $tr = $(arg).parents('tr:first');
                            var index = $tr.index() - 1;
                            return $datagrid.cDatagrid('getRowData', index);
                        }
                    }
                } else if (!isNaN(arg)) {//如果是一个数字直接返回目标行的数据
                    return $datagrid.cDatagrid('getRowData', arg);
                } else if (typeof arg === 'string') {//如果是一个字符串，会返回与字符串相等的行记录的数据
                    var $record = self.$datagrid.find('.pq-grid-cont-inner:first').find('input[value="' + arg + '"]');
                    if ($record.length > 0) {
                        var $tr = $(arg).parents('tr:first');
                        var index = $tr.index() - 1;
                        return $datagrid.cDatagrid('getRowData', index);
                    }
                }
                // return this;
            }
            //设置行数据
            //gridname:单据体名 rowindex 从0开始
            this.setRowData = function (gridname, rowIndex, datas, isnotrefresh) {
                this._init(arguments);
                if (this.__type == 'queryview') {
                    datas = rowIndex;
                    rowIndex = gridname;
                }
                var data = this.getRowData(gridname, rowIndex);
                for (var i in datas) {
                    if (datas.hasOwnProperty(i)) {
                        data[i] = datas[i];
                    }
                }
                if (!isnotrefresh) {
                    this.refreshView(gridname);
                }
            }
            //设置某行某字段禁用
            //gridname:单据体名 rowindex 从0开始
            this.setRowAttributeDisabled = function (gridname, rowIndex, attributename, isnotrefresh) {
                var data = {};
                data['__xms_pqgrid_editabled_' + attributename] = false;
                this.setRowData(gridname, rowIndex, data, isnotrefresh)
            }
            //设置某行某字段禁用后重新启用(只能用在上面的方法调用过后)
            //gridname:单据体名 rowindex 从0开始
            this.setRowAttributeEnabled = function (gridname, rowIndex, attributename, isnotrefresh) {
                var data = {};
                data['__xms_pqgrid_editabled_' + attributename] = true;
                this.setRowData(gridname, rowIndex, data, isnotrefresh)
            }
            //gridname:单据体名 设置启用
            this.setEnabled = function (gridname) {
                var $datagrid = this.getGrid(gridname);
                $datagrid.cDatagrid('enable');
            }
            //gridname:单据体名 设置禁用
            this.setDisabled = function (gridname) {
                var $datagrid = this.getGrid(gridname);
                $datagrid.cDatagrid('disable');
            }
            //gridname:单据体名 返回
            this.undo = function (gridname) {
                var $datagrid = this.getGrid(gridname);
                $datagrid.cDatagrid('history', { method: 'undo' });
            }
            //gridname:单据体名 前进
            this.redo = function (gridname) {
                var $datagrid = this.getGrid(gridname);
                $datagrid.cDatagrid('history', { method: 'redo' });
            }
            //设置行数据
            //gridname:单据体名 rowindex 从0开始
            this.setSingle = function (gridname, type) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname);
                type = type || false;
                if (!type) {
                    $datagrid.attr('data-issingle', 1);
                    $datagrid.find('.pq-header-outer .pq-grid-title-row:first input[type="checkbox"]').hide();
                } else {
                    $datagrid.removeAttr('data-issingle');
                    $datagrid.find('.pq-header-outer .pq-grid-title-row:first input[type="checkbox"]').show();
                }
            }
            //设置某行中引用字段打开选择窗口时的过滤条件
            //gridname:单据体名  rowindex 从0开始
            this.setRowAttrnameFilter = function (gridname, rowIndex, attrbutename, filter, isnotrefresh) {
                this._init(arguments);
                var filterinfo = '__xms_' + attrbutename + '_filter';
                var data = {};
                data[filterinfo] = filter;
                this.setRowData(gridname, rowIndex, data, isnotrefresh);
            }
            //设置某一列字段禁用编辑
            //gridname:单据体名  attrbutename 字段名
            this.setAttributeDisabled = function (gridname, attrbutename, isnotrefresh) {
                this.setAttributeState(gridname, attrbutename, false, isnotrefresh)
            }
            //设置某一列字段编辑状态
            //gridname:单据体名  attrbutename 字段名
            this.setAttributeState = function (gridname, attrbutename, type, isnotrefresh) {
                var cmodels = [];
                var $datagrid = this.getGrid(gridname);
                var CM = $datagrid.cDatagrid('getColModel');
                type = type || false;
                cmodels = CM;
                var attrs = [];
                if (!$.isArray(attrbutename)) {
                    attrs.push(attrbutename);
                } else {
                    attrs = attrbutename;
                }
                if (attrbutename && attrbutename.length >= 0) {
                    $.each(cmodels, function (i, n) {
                        $.each(attrs, function (ii, nn) {
                            if (nn == n.dataIndx || nn == n.field) {
                                n.editable = type;
                                return false;
                            }
                        });
                    });
                    if (!isnotrefresh) {
                        this.refreshView(gridname);
                    }
                }
            }
            //设置某一列字段启用编辑
            //gridname:单据体名  attrbutename 字段名
            this.setAttributeEnabled = function (gridname, attrbutename, isnotrefresh) {
                this.setAttributeState(gridname, attrbutename, true, isnotrefresh)
            }
            //获取cdatagrid插件实例
            this.getGrid = function (gridname) {
                if (gridname && gridname != '') {
                    gridname = gridname.toLowerCase();
                }
                var $datagrid = $('#' + gridname).children('.entity-datagrid-wrap');
                return $datagrid;
            }

            //获取pggrid插件实例
            this.getPlugGrid = function (gridname) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname);
                var grid = $datagrid.data().$plugGrid;
                return grid;
            }
            //批量添加数据，从后面增加
            this.addDatas = function (gridname, datas) {
                this._init(arguments);
                if (datas && datas.length > 0) {
                    var grid = this.getPlugGrid(gridname);
                    if (grid) {
                        grid.addDatas(datas);
                    }
                }
                return this;
            }
            //获取勾选过的数据
            this.getSelectedData = function (gridname) {
                this._init(arguments);
                var res = [];
                var $datagrid = this.getGrid(gridname)
                var $trs = $datagrid.find('.pq-grid-cont-inner:first').find('tr.pq-grid-row');
                $trs.each(function () {
                    var $this = $(this);
                    var index = $this.index() - 1;
                    var $first = $this.find('input[type="checkbox"]:checked:first');
                    if ($first.length > 0) {
                        res.push($datagrid.cDatagrid('getRowData', index));
                    }
                });
                console.log(res);
                return res;
            }
            //给单据体各行设置数据
            this.setDataEveryRow = function (gridname,datas) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname)
                var $trs = $datagrid.find('.pq-grid-cont-inner:first').find('tr.pq-grid-row');
                $trs.each(function () {
                    var $this = $(this);
                    var index = $this.index() - 1;
                    self.setRowData(gridname, index, datas, true);
                });
            }
            //只刷新表格不获取数据
            this.refreshView = function (gridname, callback) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname)
                $datagrid.cDatagrid('refresh');
                callback && callback();
                return this;
            }
            //获取数据并刷新表格
            this.refreshDataAndView = function (gridname, callback) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname)
                $datagrid.cDatagrid('refreshDataAndView');
                callback && callback();
                return this;
            }
            //设置整个单据体的过滤条件
            this.setFilter = function (gridname, filter, isRefresh) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname)
                var _filter = encodeURIComponent(JSON.stringify(filter));
                $datagrid.attr('data-filter', _filter).parent().attr('data-filter', _filter);
                $datagrid.on('gridview.refreshed', function (e, opts) {
                    $datagrid.attr('data-filter', filter).children().attr('data-filter', filter);
                });
                if (isRefresh) {
                    self.refreshDataAndView(gridname)
                }
            }
            this.bindRefresh = function (gridname,callback) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname);
                //$datagrid.trigger('datagrid.refresh', { event: event, ui: ui, dataIndx: dataIndx, rowData: rowData, colModel, colModel, grid: self });
                $datagrid.on('datagrid.refresh', callback);
            }
            this.bindAddRow = function (gridname, callback) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname);
               
                $datagrid.on('gridview.addRow', callback);
            }
            this.bindDelRow = function (gridname, callback) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname);

                $datagrid.on('gridview.deleteRow', callback);
            }
            this.bindRemoveRow = function (gridname, callback) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname);

                $datagrid.on('gridview.removeRow', callback);
            }
            this.bindRemoveLocalRow = function (gridname, callback) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname);

                $datagrid.on('gridview.removeLocalRow', callback);
            }
            this.bindCtrl = function (gridname, callback) {
                this.bindRefresh(gridname, callback); console.log(1);
                this.bindAddRow(gridname, callback); console.log(2);
                this.bindDelRow(gridname, callback); console.log(3);
                this.bindRemoveRow(gridname, callback); console.log(4);
                this.bindRemoveLocalRow(gridname, callback); console.log(5);
            }
            this.bindInitAfter = function (gridname, callback) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname);

                $datagrid.on('gridview.getDataAfter', callback);
            }
            this.removeAllData = function (gridname) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname);
                var dellist = [];
                var $trs = $datagrid.parent().data().__entityDatagrid.removeAllData();
            }
            //设置所有TR的属性
            this.setTrsAttr = function (gridname, key, value) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname)
                var $trs = $datagrid.find('.pq-grid-cont-inner').each(function () {
                    $(this).find('tr.pq-grid-row').attr(key, value);
                });
            }
            //添加监听事件
            this.onCellEvent = function (gridname, event, func) {
                this._init(arguments);
                var $datagrid = this.getGrid(gridname)
                $datagrid.on(event, func);
            }
            //字段编辑开始时
            this.onCellEditBegin = function (gridname, func) {
                this.onCellEvent(gridname, 'datagrid.editorbegin', func);
            }
            //字段编辑保存前
            this.onCellEditBeforeSave = function (gridname, func) {
                this.onCellEvent(gridname, 'datagrid.cellbeforesave', func);
            }
        };
        //表单中单据体的操作方法
        if (typeof (Xms.FormGridView) == "undefined") {
            Xms.FormGridView = new GridViewMethod();
        }

        //视图中的操作方法
        if (typeof (Xms.QueryView) == "undefined") {
            var QueryViewMethod = function () {
                var self = this;

                GridViewMethod.call(this, arguments);
                this.$datagrid = $('.datagrid-view');
                this.__type = 'queryview';
                this._init = function (args) {
                    var _args = [].slice.call(args);
                    //if (_args.length > 0) {
                    //    _args.unshift('gridview');
                    //}
                    for (var i = _args.length - 1; i >= 0; i--) {
                        if (_args[i]) {
                            _args[i + 1] = _args[i];
                        }
                    }
                }
                //获取cdatagrid插件实例
                this.getGrid = function (gridname) {
                    var $datagrid = $('.datagrid-view');
                    return $datagrid;
                }

                this.getRowData = function (arg) {
                    if (typeof arg == 'object') { //如果是一个JQ对象，返回当前对象的行数据
                        if (arg.length > 0) {
                            if ($(arg).get(0).nodeName == 'Tr') {
                                var index = $(arg).index() - 1;
                                return self.$datagrid.cDatagrid('getRowData', index);
                            } else {
                                var $tr = $(arg).parents('tr:first');
                                var index = $tr.index() - 1;
                                return self.$datagrid.cDatagrid('getRowData', index);
                            }
                        }
                    } else if (!isNaN(arg)) {//如果是一个数字直接返回目标行的数据
                        return self.$datagrid.cDatagrid('getRowData', arg);
                    } else if (typeof arg === 'string') {//如果是一个字符串，会返回与字符串相等的行记录的数据
                        var $record = self.$datagrid.find('.pq-grid-cont-inner:first').find('input[value="' + arg + '"]');
                        if ($record.length > 0) {
                            var $tr = $(arg).parents('tr:first');
                            var index = $tr.index() - 1;
                            return self.$datagrid.cDatagrid('getRowData', index);
                        }
                    }
                }
                this.setTrsAttr = function (key, value) {
                    var $trs = self.$datagrid.find('.pq-grid-cont-inner').each(function () {
                        $(this).find('tr.pq-grid-row').attr(key, value);
                    });
                }
                this.getSelectedData = function () {
                    var res = [];
                    var $trs = self.$datagrid.find('.pq-grid-cont-inner:first').find('tr.pq-grid-row');
                    $trs.each(function () {
                        var $this = $(this);
                        var index = $this.index() - 1;
                        var $first = $this.find('input[type="checkbox"]:checked:first');
                        if ($first.length > 0) {
                            res.push(self.$datagrid.cDatagrid('getRowData', index));
                        }
                    });
                    console.log(res);
                    return res;
                }
                //只刷新表格不获取数据
                this.refreshView = function (callback) {
                    this.$datagrid.cDatagrid('refresh');
                    callback && callback();
                    return this;
                }
                //获取数据并刷新表格
                this.refreshDataAndView = function (callback) {
                    this.$datagrid.cDatagrid('refreshDataAndView');
                    callback && callback();
                    return this;
                }
                //设置整个单据体的过滤条件
                this.setFilter = function (filter) {
                    var $datagrid = this.getGrid()
                    var _filter = encodeURIComponent(JSON.stringify(filter));
                    $datagrid.attr('data-filter', _filter).parent().attr('data-filter', _filter);
                    $datagrid.on('gridview.refreshed', function (e, opts) {
                        $datagrid.attr('data-filter', filter).children().attr('data-filter', filter);
                    });
                }
            }
            Xms.QueryView = new QueryViewMethod();
        }

        //其他地方的操作datagrid方法
        if (typeof (Xms.DataGridCtrl) == "undefined") {
            var DataGridCtrl = function () {
                var self = this;
                GridViewMethod.call(this, arguments);
                //获取cdatagrid插件实例
                this.getGrid = function (gridname) {
                    if (gridname && gridname != '') {
                        gridname = gridname.toLowerCase();
                    }
                    var $datagrid = $('#' + gridname);
                    return $datagrid;
                }
            }
            Xms.DataGridCtrl = new DataGridCtrl();
        }
    })();

    Xms.Page.data = {
        getId: function () {
            return Xms.Page.PageContext.EntityId;
        }
        , save: function () {
        }
    };
    Xms.Page.Attribute = function () {
        this.Target = null;
        this.Name = '';
        this.Meta = null;
    }
    Xms.Page.Attribute.prototype = {
        getValue: function () {
            return Xms.Page.getValue(this.Name);
        }
        , setValue: function (value) {
            Xms.Page.setValue(this.Name, value);
            return this;
        }
        , setDisabled: function (flag) {
            Xms.Page.setDisabled(this.Name, flag);
            return this;
        }
    }
    if (typeof Xms.Page.PageContext === 'undefined') {
        Xms.Page.PageContext = function () { };
    }
    if (typeof Xms.Page.Form === 'undefined') {
        Xms.Page.Form = function () { };
    }
    Xms.Page.FormState = {
        Create: 'create'
        , Edit: 'edit'
        , ReadOnly: 'readonly'
        , Disabled: 'disabled'
        , Preview: 'preview'
    };
    Xms.Page.FormCurrentState = Xms.Page.Form.Edit;
    //utilities
    Xms.Page.SetNotify = function (state, body) {
        var notify = $('#formNotify');
        notify.find('#formNotifyLabel').html(body);
        if (!state) {
            notify.removeClass('alert-success').addClass('alert-warning');
        }
        else {
            notify.removeClass('alert-warning').addClass('alert-success');
        }
        notify.removeClass('hide');
    };
    Xms.Page.GetFormState = function () {
        return Xms.Page.FormCurrentState;
    }
    Xms.Page.SetFormState = function (state, _context) {
        if (!_context) {
            if (state == Xms.Page.FormState.ReadOnly) {
                this.formHelper.find('input,textarea').prop('readonly', 'readonly');
                this.formHelper.find('select,button').prop('disabled', 'disabled');
                Xms.Page.FormCurrentState = Xms.Page.FormState.ReadOnly;
            }
            else if (state == Xms.Page.FormState.Disabled) {
                this.formHelper.find('input,select,button,textarea').prop('disabled', 'disabled');
                this.formHelper.find('textarea').each(function () {
                    var $this = $(this);
                    var type = $this.attr('data-controltype');
                    var _format = $this.attr('data-format');
                    if (type && type == 'ntext' && _format && _format == 'email') {
                        if ($this.data().ue) {
                            $this.data().ue.ready(function () {
                                //不可编辑
                                $this.data().ue.setDisabled();
                            });
                        }
                    }
                });
                Xms.Page.FormCurrentState = Xms.Page.FormState.Disabled;
            } else if (state == Xms.Page.FormState.Edit) {
                this.formHelper.find('input,textarea').prop('readonly', false);
                this.formHelper.find('select,button').prop('disabled', false);
                this.formHelper.find('input,select,button,textarea').prop('disabled', false);
                this.formHelper.find('a.btn').removeClass('disabled');
                this.formHelper.find('a.upload-file').removeClass('disabled');
                this.formHelper.find('textarea').each(function () {
                    var $this = $(this);
                    var type = $this.attr('data-controltype');
                    var _format = $this.attr('data-format');
                    if (type && type == 'ntext' && _format && _format == 'email') {
                        if ($this.data().ue) {
                            $this.data().ue.ready(function () {
                                //不可编辑
                                $this.data().ue.setEnabled();
                            });
                        }
                    }
                });
                Xms.Page.FormCurrentState = Xms.Page.FormState.Edit;
            }
        } else {
            if (state == Xms.Page.FormState.ReadOnly) {
                this.formHelper.find(_context).find('input,textarea').prop('readonly', 'readonly');
                this.formHelper.find(_context).find('select,button').prop('disabled', 'disabled');
                //this.formHelper.find('#toolbar').find('.btn').attr('disabled', 'disabled');
                Xms.Page.FormCurrentState = Xms.Page.FormState.ReadOnly;
            }
            else if (state == Xms.Page.FormState.Disabled) {
                this.formHelper.find(_context).find('input,select,button,textarea').prop('disabled', 'disabled');
                this.formHelper.find(_context).find('textarea').each(function () {
                    var $this = $(this);
                    var type = $this.attr('data-controltype');
                    var _format = $this.attr('data-format');
                    if (type && type == 'ntext' && _format && _format == 'email') {
                        if ($this.data().ue) {
                            $this.data().ue.ready(function () {
                                //不可编辑
                                $this.data().ue.setDisabled();
                            });
                        }
                    }
                });
                Xms.Page.FormCurrentState = Xms.Page.FormState.Disabled;
                //this.formHelper.find('#toolbar').find('.btn').attr('disabled', 'disabled');
            } else if (state == Xms.Page.FormState.Edit) {
                this.formHelper.find(_context).find('input,textarea').prop('readonly', false);
                this.formHelper.find(_context).find('select,button').prop('disabled', false);
                this.formHelper.find(_context).find('input,select,button,textarea').prop('disabled', false);
                this.formHelper.find(_context).find('textarea').each(function () {
                    var $this = $(this);
                    var type = $this.attr('data-controltype');
                    var _format = $this.attr('data-format');
                    if (type && type == 'ntext' && _format && _format == 'email') {
                        if ($this.data().ue) {
                            $this.data().ue.ready(function () {
                                //不可编辑
                                $this.data().ue.setEnabled();
                            });
                        }
                    }
                });
                Xms.Page.FormCurrentState = Xms.Page.FormState.Edit;
            }
        }
    };
    if (typeof Xms.Data === 'undefined') {
        Xms.Data = function () { };
    }
    Xms.Data.Export = function (queryid, type, filter, includeIndex, exportTitle) {
        Xms.Web.Post('/entity/export', { queryviewid: queryid, exporttype: type, filter: filter, includeIndex: includeIndex, exportTitle: exportTitle }, false, function (response) {
            console.log(response);
            if (response.IsSuccess) {
                location.href = response.Content;
                //window.open(response.Content);
            }
            else {
                console.log(response.Content);
            }
        });
    };
    Xms.Data.Import = function (entityid) {
        Xms.Web.OpenDialog('/dataimport/import?entityid=' + entityid);
    };
    /*
    *       表单提交前执行事件
    *       @param Xms.FormPrevSubmit.add(func)：添加一个需要检查的方法  func返回false，表单会不提交
    *       @param Xms.FormPrevSubmit.check():
    *
    */
    (function () {
        function FormPrevSubmit() {
            this.list = [];
        }
        FormPrevSubmit.prototype.add = function (func) {
            if (typeof func !== 'function') { throw new Error('传入参数不是函数') }
            this.list.push(func);
        }
        FormPrevSubmit.prototype.check = function (func) {
            if (this.list.length == 0) return true;
            var flag = true;
            $.each(this.list, function (key, item) {
                if (item && $.isFunction(item)) {
                    var itemflag = item();
                    if (!itemflag) {
                        flag = false;
                        return false;
                    }
                }
            });
            return flag;
        }
        Xms.FormPrevSubmit = new FormPrevSubmit();
    })();

    /*

    */
    Xms.Page.showFormHideButton = function () {
        var $toolbar = $("#toolbar");
        if ($toolbar.length > 0) {
            $toolbar.showBySize();
        }
    }
    Xms.Page.hideFormHideButton = function () {
        var $toolbar = $("#toolbar");
        if ($toolbar.length > 0) {
            $toolbar.hideBySize();
        }
    }

    /*
    *       设置单据体是否可编辑
    *       @param name：单据体名字
    *       @param data: type  true,false
    *
    */
    Xms.Page.setFormIsEdit = function (name, type, callback) {
        if (typeof renderGridView !== 'undefined') {
            var $subgrid = $('#' + name);
            if ($subgrid.length > 0) {
                $subgrid.attr('data-editable', type || false);
                renderGridView($('#' + name), callback || function () { });
            }
        }
    }

    /*
    *       设置列表页面
    *       @param entityname：实体名字
    *       @param parentname: parentid的字段名
    *       @param attrname:需要添加过滤方法的字段
    *       @param itemClick:  单击节点触发的事件（非必填）
    *       @param sortby:  右边列表需要排序的字段
    *       @param isDefaultClick:  是否使用默认的过滤方法,不填或true时使用默认的点击方法
    *       @param callback:  加载完树后的回调事件，可设置展开树时节点打开方式
    *       @param treesort:  左边要排序的字段
    *       @param ordertype:  排序方式，正序/倒序"Ascending/Descending"
    *       如果只传一个参数是会当做一个对象 ：
            { entityname: entityname, parent: parentname, attrname: attrname, itemClick: itemClick, sortby: sortby, isDefaultClick: isDefaultClick, treesort: treesort, ordertype: ordertype };
    */
    Xms.Page.loadGridViewTree = function (entityname, parentname, attrname, itemClick, sortby, isDefaultClick, callback, treesort, ordertype) {
        if (typeof renderLeftTree === 'function') {
            console.log('Xms.Page.loadGridViewTree', callback);
            var _args;
            if (arguments.length == 1) {
                _args = arguments[0];
                callback = _args.callback;
            } else {
                _args = { entityname: entityname, parent: parentname, attrname: attrname, itemClick: itemClick, sortby: sortby, isDefaultClick: isDefaultClick, treesort: treesort, ordertype: ordertype };
            }
            renderLeftTree(_args, callback);
        } else {
            throw new Error("只能在列表页面中使用该方法");
        }
    }
    /*
    *       设置列表页面
    *       @param entityname：实体名字
    *       @param parentname: parentid的字段名
    *       @param attrname:需要添加过滤方法的字段
    *       @param itemClick:  单击节点触发的事件（非必填）
    *       @param sortby:  右边列表需要排序的字段
    *       @param isDefaultClick:  是否使用默认的过滤方法,不填或true时使用默认的点击方法
    *       @param callback:  加载完树后的回调事件，可设置展开树时节点打开方式
    *       @param treesort:  左边要排序的字段
    *       @param ordertype:  排序方式，正序/倒序"Ascending/Descending"
    *       如果只传一个参数是会当做一个对象 ：
            { entityname: entityname, parent: parentname, attrname: attrname, itemClick: itemClick, sortby: sortby, isDefaultClick: isDefaultClick, treesort: treesort, ordertype: ordertype };
    */
    Xms.Page.loadFlowLine = function (entityname, parentname, attrname, itemClick, sortby, isDefaultClick, callback, treesort, ordertype) {
        if (typeof renderLeftTree === 'function') {
            console.log('Xms.Page.loadGridViewTree', callback);
            var _args;
            if (arguments.length == 1) {
                _args = arguments[0];
                callback = _args.callback;
            } else {
                _args = { entityname: entityname, parent: parentname, attrname: attrname, itemClick: itemClick, sortby: sortby, isDefaultClick: isDefaultClick, treesort: treesort, ordertype: ordertype };
            }
            renderLeftTree(_args, callback);
        } else {
            throw new Error("只能在列表页面中使用该方法");
        }
    }
    Xms.Page.loadFlowLineTotarget = function ($context, entityname, parentname, attrname, itemClick, sortby, isDefaultClick, callback, treesort, ordertype) {
        if (typeof renderLeftTree === 'function') {
            console.log('Xms.Page.loadGridViewTree', callback);
            var _args;
            if (arguments.length == 1) {
                _args = arguments[0];
                callback = _args.callback;
            } else {
                _args = { $context: $context, entityname: entityname, parent: parentname, attrname: attrname, itemClick: itemClick, sortby: sortby, isDefaultClick: isDefaultClick, treesort: treesort, ordertype: ordertype };
            }
            renderLeftTree(_args, callback);
        } else {
            throw new Error("只能在列表页面中使用该方法");
        }
    }
})(window);

; (function (root) {
    Xms.Reg = new function () {
        this.regs = [];
    };
    Xms.Reg.regexps = [];
    Xms.Reg.inserRulesToForm = function ($input, rules) {
        $input.rules('add', rules);
    };
    Xms.Reg.removeRuleForm = function ($input) {
        $input.rules('remove');
    };
    Xms.Reg.addRegExp = function (name, func) {
        Xms.Reg[name] = func;
    }

    //Xms.Reg.isDigit(val);
    Xms.Reg.addRegExp('isDigit', function (num) {
        var _reg = /^-?\d*\.?\d+$/;
        return /^-?\d*\.?\d+$/.test(num);
    });

    Xms.Reg.addRegExp('isEnglish', function (s) {
        return Xms.Web.ValidData('english')(s);
    });
    Xms.Reg.addRegExp('isChinese', function (s) {
        return Xms.Web.ValidData('ischinese')(s);
    });
    Xms.Reg.addRegExp('isEmail', function (s) {
        return Xms.Web.ValidData('email')(s);
    });
    Xms.Reg.addRegExp('isInteger', function (s) {
        return Xms.Web.ValidData('integer')(s);
    });
    Xms.Reg.addRegExp('isDecimal', function (s) {
        return Xms.Web.ValidData('isdecimal')(s);
    });
    Xms.Reg.addRegExp('minLength', function (s, len) {
        return Xms.Web.ValidData('minlength')(s, len);
    });
    Xms.Reg.addRegExp('maxLength', function (s, len) {
        return Xms.Web.ValidData('maxlength')(s, len);
    });
    Xms.Reg.addRegExp('isNull', function (s) {
        return Xms.Web.ValidData('isnull')(s);
    });
    Xms.Reg.addRegExp('isFloat', function (s) {
        return Xms.Web.ValidData('float')(s);
    });
    //严格的日期格式判断
    Xms.Reg.addRegExp('isDate', function (s, _bool) {
        _bool = true;
        if (_bool == true) {
            var objRegExp = /((^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(10|12|0?[13578])([-\/\._])(3[01]|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(11|0?[469])([-\/\._])(30|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))([-\/\._])(0?2)([-\/\._])(2[0-8]|1[0-9]|0?[1-9])$)|(^([2468][048]00)([-\/\._])(0?2)([-\/\._])(29)$)|(^([3579][26]00)([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][0][48])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][0][48])([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][2468][048])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][2468][048])([-\/\._])(0?2)([-\/\._])(29)$)|(^([1][89][13579][26])([-\/\._])(0?2)([-\/\._])(29)$)|(^([2-9][0-9][13579][26])([-\/\._])(0?2)([-\/\._])(29)$))/;
            return objRegExp.test(s);
        }
    });
})(window);