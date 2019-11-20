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
        , setRange: function (name, min,max) {
            this.setRules(name, [min,max]);
        }
        , removeRules: function (name, rules) {
            if (rules) {
                this.formHelper.find('[name=' + name + ']').rules('remove',rules);
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

            } else if (type == 'ntext' && _format && _format=='email') {
                if ($this.data().ue) {
                    $this.data().ue.setDisabled();
                }
            }
            else {
                $this.prop('disabled', 'disabled');
            }
            return $this;
        }
        };
    }
    if (typeof (Xms.QueryView) == "undefined") {
        Xms.QueryView = new function () {
            var self = this;
            this.$datagrid = $('.datagrid-view');
            //arg: TR或TR内的jq对象/第几行/记录id
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
                } else if(typeof arg==='string'){//如果是一个字符串，会返回与字符串相等的行记录的数据
                    var $record = self.$datagrid.find('.pq-grid-cont-inner:first').find('input[value="' + arg + '"]');
                    if ($record.length > 0) {
                        var $tr = $(arg).parents('tr:first');
                        var index = $tr.index() - 1;
                        return self.$datagrid.cDatagrid('getRowData', index);
                    }
                }
            }

            this.getSelectedData = function () {
                var res = [];
                var $trs = self.$datagrid.find('.pq-grid-cont-inner:first').find('tr.pq-grid-row');
                $trs.each(function () {
                    var $this = $(this);
                    var index = $this.index()-1;
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
        };
    }

    if (typeof (Xms.FormGridView) == "undefined") {
        Xms.FormGridView = new function () {
            var self = this;
            //gridname:单据体名 arg: TR或TR内的jq对象/第几行/记录id
            this.getRowData = function (gridname, arg) {
                var $datagrid = $('#' + gridname).children('.entity-datagrid-wrap');
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
                return this;
            }
            this.getGrid = function (gridname) {
                var $datagrid = $('#' + gridname).children('.entity-datagrid-wrap');
                return $datagrid;
            }
            this.getPlugGrid = function (gridname) {
                var $datagrid = this.getGrid(gridname);
                var grid = $datagrid.data().$plugGrid;
                return grid;
            }
            //批量添加数据
            this.addDatas = function (gridname,datas) {
                if (datas && datas.length > 0) {
                    var grid = this.getPlugGrid(gridname);
                    if (grid) {
                        grid.addDatas(datas);
                    }
                }
                return this;
            }
            this.getSelectedData = function (gridname) {
                var res = [];
                var $datagrid = $('#' + gridname).children('.entity-datagrid-wrap');
                var $trs = $datagrid.find('.pq-grid-cont-inner:first').find('tr.pq-grid-row');
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
            this.refreshView = function (gridname, callback) {
                var $datagrid = $('#' + gridname).children('.entity-datagrid-wrap');
                $datagrid.cDatagrid('refresh');
                callback && callback();
                return this;
            }
            //获取数据并刷新表格
            this.refreshDataAndView = function (gridname, callback) {
                var $datagrid = $('#' + gridname).children('.entity-datagrid-wrap');
                $datagrid.cDatagrid('refreshDataAndView');
                callback && callback();
                return this;
            }
        };
    }

    //在列表页中查找对应位置的值
    Xms.Page.getValueByGridView = function(attrname,recordid,context){
        context = context || $('#gridview');
        var res;
        if(attrname && recordid){
            var $tr,index;
            var $ths = context.find('thead th');
            if($ths.length>0){
                $ths.each(function(key,item){
                    var _name = $(this).attr('data-name');
                    if(_name==attrname){
                        index = key;
                        return false;
                    }
                });
            }
            var $record = context.find('input[name="recordid"][value="'+recordid+'"]');
            if($record.length>0 && index){
                $tr = $record.parents('tr:first');
                var $td = $tr.find('td:eq('+index+')');
                if($td.find('a').length>0){
                    res = $td.find('a').text();
                }else if($td.find('div.gridview-table-cell').length>0){
                    res = $td.find('div.gridview-table-cell').text();
                }
            }
        }
        return res;
    }
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
                //this.formHelper.find('#toolbar').find('.btn').attr('disabled', 'disabled');
            }
            else if (state == Xms.Page.FormState.Disabled) {
                this.formHelper.find('input,select,button,textarea').prop('disabled', 'disabled');
                this.formHelper.find('textarea').each(function () {
                    var $this = $(this);
                    var type = $this.attr('data-controltype');
                    var _format = $this.attr('data-format');
                    if (type && type == 'ntext' && _format && _format == 'email') {
                        if ($this.data().ue) {
                            $this.data().ue.setDisabled();
                        }
                    }
                });
                Xms.Page.FormCurrentState = Xms.Page.FormState.Disabled;
                //this.formHelper.find('#toolbar').find('.btn').attr('disabled', 'disabled');
            } else if (state == Xms.Page.FormState.Edit) {
                this.formHelper.find('input,textarea').prop('readonly', false);
                this.formHelper.find('select,button').prop('disabled', false);
                this.formHelper.find('input,select,button,textarea').prop('disabled', false);
                this.formHelper.find('textarea').each(function () {
                    var $this = $(this);
                    var type = $this.attr('data-controltype');
                    var _format = $this.attr('data-format');
                    if (type && type == 'ntext' && _format && _format == 'email') {
                        if ($this.data().ue) {
                            $this.data().ue.setEnabled();
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
                            $this.data().ue.setDisabled();
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
                            $this.data().ue.setEnabled();
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
        if ($toolbar.length>0){
            $toolbar.showBySize();
        }
    }
    Xms.Page.hideFormHideButton = function () {
        var $toolbar = $("#toolbar");
        if ($toolbar.length > 0) {
            $toolbar.hideBySize();
        }
    }

    Xms.Page.subgridIsTriggerNext = true;//单据体中的form元素改变后是否跳到下一个位置
    /*
    *       在单据体中插入数据
    *       @param name：单据体名字
    *       @param data: 插入的数据
    *       @param auto: 是否插入后自动保存
    */
    Xms.Page.assignSubGrid = function (name, data, auto, isNoCover) {
        if (GridViewModelObject) {
            if (!GridViewModelObject[name]) {
                throw new Error('单据体名字输入错误');
            }
            GridViewModelObject[name].assignSubGrid(data, auto, isNoCover);
        }
    }
    /*
    *       给单据体中的某个字段添加数据
    */
    Xms.Page.setValueByContext = function ($context, name, value, datas, relationshipname) {
        Xms.Web.Console($context, name, value, datas, relationshipname)
        var $this = $context.find('[name="' + name + '"]');
        var type = $this.attr('data-controltype');
        var dataformat = $this.attr('data-format') || '';
        Xms.Web.Console('$input', $this);
        Xms.Web.Console('inputype', type);
        if (type == 'lookup' || type == 'owner' || type == 'customer') {
            if (value != null) {
                var temp = value;
                var attrName = name;
                if (relationshipname && relationshipname != '') {
                    attrName = name.replace(relationshipname + '_', "");
                }
                console.log('setValueByContext', temp, name, attrName, datas);
                console.log(datas[attrName + 'name'], attrName + 'name')
                if (datas && typeof value === "string") {

                    temp = {
                        id: value,
                        name: datas[attrName + 'name'] || datas[ 'name']
                    }
                }
                console.log(temp);
                $this.val(temp.id).trigger('change', { noGoNext: true });
                var _id = $this.attr('id');
                var textInput = $context.find('[name="' + name + '_text"]');
                textInput.val(temp.name).trigger('change', { noGoNext: true });
                $.fn.xmsSelecteDown.setLookUpState(textInput);
            }
            else {
                $this.val('').trigger('change');
                $context.find('[name="' + name + '_text"]').siblings('.xms-dropdownLink').remove();
                $context.find('[name="' + name + '_text"]').val('').trigger('change');
            }
        } else if (type == 'status' || type == 'picklist' || type == 'state') {
            $this.val(value).trigger('change', { noGoNext: true });

            var next = $this.next('select');
            // console.log(next.find('option[value="' + value + '"]'), value);
            if (next.length > 0) {
                next.find('option[value="' + value + '"]').prop('selected', true);
                next.trigger('change', { noGoNext: true });
            }
            //$context.find('[name="' + name + '"][value="' + value + '"]').prop('checked', true);
        } else if (type == 'ntext' && dataformat == 'email') {
            var $_id = $('#' + name);
            if ($_id.length > 0) {
                $_id.data().ue.ready(function () { $_id.data().ue.setContent(value) });
            }
        }
        else {
            $this.val(value).trigger('change', { noGoNext: true });
        }
        Xms.Web.Console('setValueEnd');
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
    *       设置单据体某字段列是否可编辑
    *       @param name：单据体名字
    *       @param attrname:字段名
    *       @param state:   true,false
    *       
    */
    Xms.Page.setSubGridAttribueState = function (name, attrname, state, callback) {
        if (GridViewModelObject) {
            if (!GridViewModelObject[name]) {
                throw new Error('单据体名字输入错误');
            }
            GridViewModelObject[name].setAttributeState(attrname, state, callback);
        }
    }


    /*
    *       设置单据体某字段列添加事件
    *       @param name：单据体名字
    *       @param attrname:字段名
    *       @param event:  
    *       
    */
    Xms.Page.attribueBindEvent = function (name, attrname, event, callback, isreplace) {
        if (GridViewModelObject) {
            if (!GridViewModelObject[name]) {
                throw new Error('单据体名字输入错误');
            }
            GridViewModelObject[name].attribueBindEvent(attrname, event, callback, isreplace);
        }
    }
    /*
    *       判断单据体是否编辑过
    *       @param name：单据体名字
    *       
    */
    Xms.Page.getSubGridIsEdit = function (name) {
        if (GridViewModelObject) {
            if (!GridViewModelObject[name]) {
                throw new Error('单据体名字输入错误');
            }
            return GridViewModelObject[name].getSubGridIsEdit();
        }
        return true;
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
    Xms.Page.loadGridViewTree = function (entityname, parentname, attrname, itemClick, sortby, isDefaultClick, callback,treesort,ordertype) {
        if (typeof renderLeftTree === 'function') {
            console.log('Xms.Page.loadGridViewTree', callback);
            var _args;
            if (arguments.length == 1) {
                _args = arguments[0];
                callback = _args.callback;
            }else{
                _args = { entityname: entityname, parent: parentname, attrname: attrname, itemClick: itemClick, sortby: sortby, isDefaultClick: isDefaultClick, treesort: treesort, ordertype: ordertype };
                
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
    Xms.Reg.addRegExp('minLength', function (s,len) {
        return Xms.Web.ValidData('minlength')(s,len);
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