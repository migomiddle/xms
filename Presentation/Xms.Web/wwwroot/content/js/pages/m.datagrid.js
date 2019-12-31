//@ sourceURL=pages/m.datagrid.js
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
    var prefix = '__datagrid__';
    function datagridList() {
        this.list = [];
        this.prefix = prefix;
    }
    datagridList.prototype.add = function (grid) {
        this.list.push(grid);
    }
    datagridList.prototype.indexOf = function (value, key) {
        var index = -1;
        key = key || 'id';
        $.each(this.list, function (i, n) {
            if (this[key] == value) {
                index = i;
                return false;
            }
        });
        return index;
    }
    datagridList.prototype.getGrid = function (gridname) {
        var res = null;
        var self = this;
        $.each(this.list, function (i, n) {
            if (n['id'] == self.prefix + gridname) {
                res = n;
                return false;
            }
        });
        return res;
    }
    datagridList.prototype.getByIndex = function (index) {
        if (this.list[index]) {
            return this.list[index];
        } else {
            return null;
        }
    }
    var entityDatagirdList = new datagridList();
    var datagrid_count = 0;
    var toolbtns = [

        {
            classname: 'btn btn-link btn-xs', label: '插入行', name: 'addRow', enabled: true, icon: 'glyphicon glyphicon-plus', event: function (e, n, self) {
                if (typeof dirtyChecker != 'undefined') {
                    dirtyChecker.isDirty = true;
                }
                var data = { cdatagrid_editer: 'new', datagrid_isEdit: true };
                var flag = self.$wrap.trigger('gridview.prevAddRow', { e: e, n: n, self: self, data: data });
                console.log(flag);
                self.$context.cDatagrid('addRow', data);
                self.$wrap.trigger('gridview.addRow', { e: e, n: n, self: self });
            }, eventtype: 'click'
        }

        , {
            classname: 'btn btn-link btn-xs', label: '删除', name: 'saveGrid', enabled: true, icon: 'glyphicon glyphicon-trash', event: function (e, n, self) {
                self.delDatas();
                self.$wrap.trigger('gridview.deleteRow', { e: e, n: n, self: self });
            }, eventtype: 'click'
        }];
    function entityDatagrid(id, $context, datas) {
        var self = this;
        this.prefix = prefix;
        this.__id = id || datagrid_count++;
        this.id = this.prefix + this.__id;
        this.$context = $context;
        this.$wrap = this.$context.parent();
        this.$plug = null;
        this.postData = {};
        this.localDatas = [];
        this.datas = {};
        if (datas.queryviews) {
            var first = datas.queryviews[0];
            datas.entityId = first.entityid;
            datas.queryId = first.queryviewid;
            datas.aggregateconfig = first.aggregateconfig;
            datas.layoutconfig = first.layoutconfig.toLowerCase();
            datas.buttonsinfo = datas.buttons;
            datas.attributesInfo = JSON.parse(JSON.stringify(datas.attributes).toLowerCase());
            datas.setAttributesShow = datas.attributesInfo = filterAttributes(datas.attributesInfo, datas);
            this.datas = $.extend({}, datas);
            this.datas.freezenIndex = 2;

            if (this.datas.attributesInfo.length > 0) {
                var entityname = '';
                $.each(this.datas.attributesInfo, function (i, n) {
                    if (n.name != '' && n.name.indexOf('.') == -1) {
                        self.datas.entityName = n.entityname;
                    }
                });
            }
        }
        this.$parent = null;
        this.$toolbar = $('<div class="toolbar clearfix"></div>');
        this.$buttons = $('<div class="toolbar-bottons pull-left"></div>');
        this.buttons = toolbtns;
        this.deleteList = [];
        this.$searcher = $('<div class="pull-right toolbar-searcher"></div>');
        var searchhtml = ['<table style="position:relative;">',
            '<tbody><tr>',
            '<td>',
            '<div class="input-group input-group-sm col-sm-4">',
            ' <input class="form-control quickly-search-input valid" name="Q" placeholder="快速查询" style="width:200px;" type="text" value="">',
            '<span class="input-group-btn">',
            '<a class="btn btn-default" name="clearBtn" type="button" title="清空">',
            '  <span class="glyphicon glyphicon-remove-sign"></span>',
            '</a>',
            ' <a class="btn btn-default" name="searchBtn" type="button">',
            '  <span class="glyphicon glyphicon-search"></span>',
            ' </a>',
            '</span>',
            ' </div>',
            ' </td>',
            ' </tr>',
            ' </tbody></table>'];
        this.$searcher.html(searchhtml.join(''));
        this.$parent = this.$context.parent();
        this.$parent.prepend(this.$toolbar);
        this.$toolbar.append(this.$searcher);
        this.$toolbar.append(this.$buttons);
        //  this.addButtons();
        entityDatagirdList.add(this);
        this.loadDatagird();
    }
    entityDatagrid.prototype.setContext = function ($context) {
        this.$context = $context;
    }
    entityDatagrid.prototype.setDatagrdiOpts = function (opts) {
        this.postData = opts;
    }
    entityDatagrid.prototype.extDatagrdiOpts = function (opts) {
        $.extend(this.postData, opts);
    }
    entityDatagrid.prototype.extDatagrdiOpts = function (opts) {
        $.extend(this.postData, opts);
    }
    entityDatagrid.prototype.loadDatagird = function () {
        var self = this;
        this.$plug = loadDataGrid(this.$context, this.datas, null, {
            //bindEvent: function () {
            //   // self.bindEvent();
            //},
            _super: self
        }, self);
        self.addButtons();
        this.$context.data().$plugGrid = this;
        // self._addEmptyRow(self.datas.DefaultEmptyRows);
        return this;
    }
    entityDatagrid.prototype._createButton = function (btninfo) {
        if (btninfo.xms_customize_button) {
            var btn = $(['<a  class="' + btninfo.classname + '" name="' + btninfo.name + '" title="" ' + (btninfo.enabled ? '' : 'disabled') + ' >', '</a>'].join(''));
        } else {
            var btn = $(['<button type="button" class="' + btninfo.classname + '" name="' + btninfo.name + '" title="" ' + (btninfo.enabled ? '' : 'disabled') + ' >', '</button>'].join(''));
        }
        btn.html('<span class="' + btninfo.icon + '"></span> ' + btninfo.label)
        return btn;
    }
    entityDatagrid.prototype._addEmptyRow = function (count) {
        if (this.datas.formState == 'disabled' || this.datas.formState == 'readonly') {
            return true;
        }
        if (this.datas.iseditable == 'false') {
            return true;
        }
        for (var i = 0; i < count; i++) {
            this.$context.cDatagrid('addRow', { cdatagrid_editer: 'new' });
        }
    }
    entityDatagrid.prototype.addButton = function (btninfo) {
        this.$buttons.append(this._createButton(btninfo));
    }
    entityDatagrid.prototype._filterButtonInfoData = function () {
        if (this.datas.buttonsinfo && this.datas.buttonsinfo.length > 0) {
            this.datas.buttonsinfo = $.grep(this.datas.buttonsinfo, function (n, i) {
                if (n.showarea == 4 && n.isenabled && n.isvisibled) {
                    n.classname = n.cssclass;
                    n.name = 'xms_subgrid_button_' + n.ribbonbuttonid;
                    n.eventtype = 'click';
                    n.enabled = true;
                    n.xms_customize_button = true;
                    n.event = function (e, n, self) {
                        var func = new Function(n.jsaction);
                        func.call(this, e, n, self);
                    }
                    return true;
                }
            });
        }
    }
    entityDatagrid.prototype.addButtons = function () {
        var self = this;
        this._filterButtonInfoData();
        this.buttons = $.extend([], toolbtns);
        $.each(this.datas.buttonsinfo, function () {
            self.buttons.push(this);
        });
        if (this.datas.iseditable == "false") {
            this.buttons = [];
        }
        $.each(this.buttons, function () {
            self.addButton(this);
        });
        this.bindButtonEvent();
    }
    entityDatagrid.prototype.bindButtonEvent = function (opts) {
        var self = this;
        $.each(this.buttons, function (i, n) {
            self.$buttons.find('[name="' + n.name + '"]').on(n.eventtype, function (e) {
                n.event.call(this, e, n, self);
            })
        });
    }
    entityDatagrid.prototype.showValidataTips = function (rule, $td) {
        var self = this;
        showInErrorMsg($td, rule.msg);
        function showInErrorMsg($input, msg) {
            var msgbox = null;
            //$input.css('position', "relative");
            if ($input.data().msgbox && $input.data().msgbox.length > 0) {
                msgbox = $input.data().msgbox;
                msgbox.show();
            } else {
                msgbox = $('<div class="inputErrorMsgBox"><em class="glyphicon glyphicon-exclamation-sign"></em><span class="inputErrorMsg"></span></div>');
                msgbox.appendTo(self.$context.find('.pq-grid-cont-inner-freezeleft').next());
                $input.data().msgbox = msgbox
            }
            var freezeW = self.$context.find('.pq-grid-cont-inner-freezeleft').width();
            var bound = {
                x: $input.position().left,
                y: $input.position().top,
                w: $input.outerWidth(),
                h: $input.outerHeight()
            }
            msgbox.find('.inputErrorMsg').text(msg);
            msgbox.css({ "top": bound.y, "left": bound.x - freezeW + bound.w });
        }
    }
    entityDatagrid.prototype.validataCell = function (rowData, attrname, rules, $td, $th) {
        var flag = true;
        var self = this;
        $.each(rules, function (i, n) {
            if (n.type == 'minLen') {
                if (!rowData[attrname] || rowData[attrname].length < n.value) {
                    flag = false;
                    self.showValidataTips(n, $td);
                    return false;
                }
            }
        });
        return flag;
    }
    entityDatagrid.prototype.getAndValidDatas = function () {
        var self = this;
        var flag = true;
        var datas = ''
        $('.inputErrorMsgBox').remove();
        var columns = self.$context.cDatagrid('getOpts');
        var requiredDatas = [];
        if (columns && columns.colModel) {
            var colmodel = columns.colModel;
            console.log(colmodel);
            var $theader = this.$context.find('.pq-grid-header-table tr.pq-grid-title-row>td:gt(1)');
            this.$context.find('.pq-grid-cont-inner:first tr.pq-grid-row').each(function (ii, nn) {
                var isvalid = true;
                console.log($tds);
                var index = $(nn).index();
                var rowData = self.$plug.cDatagrid('getRowData', index - 1);
                if (rowData.datagrid_isEdit) {
                    var $tds = $(this).find('td:gt(1)');
                    $tds.data().msgbox = null;
                    $tds.each(function (i, n) {
                        var tdIndex = $(n).index() - 2;
                        var $th = $theader.eq(tdIndex);
                        var attrname = $th.children().attr('data-fieldname');
                        var rowCol = $.queryBykeyValue(colmodel, 'dataIndx', attrname);
                        if (rowCol && rowCol.length > 0) {
                            if (rowCol[0]._validations && rowCol[0]._validations.length > 0) {
                                var _flag = self.validataCell(rowData, attrname, rowCol[0]._validations, $(n), $th);
                                if (isvalid == true) {
                                    isvalid = _flag
                                }
                                if (isvalid == false) {
                                    requiredDatas.push({ rowData: rowData, attrname: attrname, $obj: $(n), $th: $th, validation: rowCol[0]._validations, rowIndex: ii, label: rowCol[0].title });
                                    //  return false;
                                }
                            }
                        }
                    });
                }
                if (isvalid == false) {
                    flag = false;
                    return false;
                }
                console.log(isvalid)
            });
        } else {
            console.log(columns)
        }

        return { isvalid: flag, datas: this.localDatas, requiredDatas: requiredDatas }
    }
    entityDatagrid.prototype.getEditedDatas = function () {
        return $.grep(this.localDatas, function (n, i) {
            if (n.datagrid_isEdit == true) {
                return n;
            }
        })
    }
    entityDatagrid.prototype.getDatas = function () {
        return this.localDatas
    }
    entityDatagrid.prototype.delDatas = function () {
        var self = this;
        var res = [];
        this.$context.find('.pq-grid-cont-inner:first tr.pq-grid-row').each(function (i, n) {
            var $this = $(this);
            var index = $this.index() - 1;
            var rowData = self.$plug.cDatagrid('getRowData', index);
            var $first = $this.find('input[type="checkbox"]:checked:first');
            if ($first.length > 0) {
                res.push(index);
                if (rowData['cdatagrid_editer'] != 'new') {
                    self.deleteList.push(rowData);
                }
            }
        });
        $.each(res, function () {
            self.$plug.cDatagrid('deleteRow', this);
        });
        if (typeof dirtyChecker != 'undefined') {
            dirtyChecker.isDirty = true;
        }
    }
    entityDatagrid.prototype.saveData = function () {
        var self = this;
        var flag = true;
        this.$context.find('.pq-grid-cont-inner:first tr.pq-grid-row').each(function (i, n) {
            var index = $(n).index();
            var rowData = self.$plug.cDatagrid('getRowData', index - 1);
            if (rowData.datagrid_isEdit) {
                var isvalid = self.$plug.cDatagrid('isValid', index - 1);
                if (isvalid.valid == false) {
                    flag = false;
                    return false;
                }
            }
            console.log(isvalid)
        });
        if (flag == false) {
            return false;
        }
        console.log(this);
    }
    entityDatagrid.prototype.removeRowData = function (index) {
        var self = this;
        self.$plug.cDatagrid('removeRowData', index - 1);
    }
    entityDatagrid.prototype.removeAllData = function () {
        var self = this;
        self.$plug.find('.pq-grid-cont-inner:first').each(function () {
            var dellist = []
            $(this).find('tr.pq-grid-row').each(function () {
                var index = $(this).index();
                var rowData = self.$plug.cDatagrid('getRowData',index-1);
                var id = $(this).find('input[name="recordid"]:first').val();
                if (id && rowData.cdatagrid_editer!='new') {
                    rowData.entitystatus = 3;
                    self.deleteList.push(rowData);
                   
                }
            });
            $(this).find('tr.pq-grid-row').each(function () {
                var index = $(this).index();
                var rowData = self.$plug.cDatagrid('getRowData', 1);
                var id = $(this).find('input[name="recordid"]:first').val();
                if (id) {
                    self.$plug.cDatagrid('deleteRow', 1);
                }
            });
           
        });
        if (typeof dirtyChecker != 'undefined') {
            dirtyChecker.isDirty = true;
        }
        
    }
    entityDatagrid.prototype.addDatas = function (datas) {
        var len = this.localDatas.length, self = this;;
        $.each(datas, function () {
            this.cdatagrid_editer = 'new';
            this.datagrid_isEdit = true;
            self.$plug.cDatagrid('addRow', this);
        });
        if (typeof dirtyChecker != 'undefined') {
            dirtyChecker.isDirty = true;
        }
        //this.$plug.cDatagrid('refresh');
    }
    entityDatagrid.prototype.setDatas = function (datas) {
    }
    entityDatagrid.prototype.bindEvent = function ($context, evt, ui) {
        var self = this;
        $('button[name=removeRowBtn]', self.$parent).off('click').on('click', null, function (e) {
            var parRow = $(this).parents('tr:first'), index = parRow.index();
            var rowData = self.$plug.cDatagrid('getRowData', index - 1);
            var id = parRow.find('input[name="recordid"]:first').val();
            if (id) {
                rowData.entitystatus = 3;
                self.deleteList.push(rowData);
                self.$plug.cDatagrid('deleteRow', index - 1);
            }
            if (typeof dirtyChecker != 'undefined') {
                dirtyChecker.isDirty = true;
            }
            self.$wrap.trigger('gridview.removeRow', { e: e, self: self });
        });
        $('button[name=removeRowBtnLocal]', self.$parent).off('click').on('click', null, function (e) {
            var parRow = $(this).parents('tr:first'), index = parRow.index();
            self.$plug.cDatagrid('deleteRow', index - 1);
            self.$wrap.trigger('gridview.removeLocalRow', { e: e, self: self });
        });
        $('[name=searchBtn]', self.$parent).off('click').on('click', null, function (e) {
            self.$plug.cDatagrid('refreshDataAndView');
        });
        $('[name=clearBtn]', self.$parent).off('click').on('click', null, function (e) {
            self.$searcher.find('input[name="Q"]').val('')
            self.$plug.cDatagrid('refreshDataAndView');
        });
    }
    function filterAttributes(items, datas) {
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
                        if (attrs[1] == item.name.toLowerCase()) {
                            item.name = n.name;
                            item.localizedname = n.label ? n.label : item.label ? item.label : item.localizedname + '(' + item.entitylocalizedname + ')' ? item.localizedname : item.entitylocalizedname//item.label; //+ '(' + item.entitylocalizedname + ')';
                            item.width = n.width;
                            tar = item;
                            n.editable = false;
                            if (datas.formState == 'disabled' || datas.formState == 'readonly') {
                                n.editable = false;
                            }
                            if (datas.iseditable == "false") {
                                n.editable = false;
                            }
                            return false;
                        }
                    } else {
                        if (item.name == n.name.toLowerCase()) {
                            item.width = n.width;
                            item.localizedname = n.label ? n.label : item.label ? item.label : item.localizedname ? item.localizedname : item.entitylocalizedname
                            tar = item;
                            if (item.name == 'createdon' || item.name == 'modifiedon' || item.name == 'modifiedby' || item.name == 'createdby') {
                                n.editable = false;
                            } else {
                                n.editable = true;
                            }
                            if (item.name.replace(/id$/, '') == datas.formEntityName) {
                                n.editable = false;
                            }
                            if (datas.formState == 'disabled' || datas.formState == 'readonly') {
                                n.editable = false;
                            }
                            if (datas.iseditable == "false") {
                                n.editable = false;
                            }
                            return false;
                        }
                    }
                });
                if (tar) {
                    $.extend(n, tar, n);
                }
            });
            return layoutitems;
        } else {
            return items
        }
    }
    function loadDataGrid($context, datas, isDestroy, opts, _super) {
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
                tableW += ((n.width || 100) * 1);
            });
            //  console.log(datatableW, tableW);
            if (datatableW < tableW) {
                isWidthToMax = false;
            }
        }
        //  var firstload = false;
        //datagrid配置项
        var datagridconfig = {
            freezeCtrl: false,
            getDataUrl: function (cdatagrid, opts) {
                var pagesize = cdatagrid.opts.pageModel.rPP

                return ORG_SERVERURL + '/api/data/fetchAndAggregate?entityid=' + datas.entityId + '&queryviewid=' + datas.queryId + '&onlydata=true&pagesize=' + pagesize + '&page=' + cdatagrid.opts.pageModel.page
            },

            selectionModel: { type: 'cell' },
            //  selectionModel: { type: null },
            getColModels: function (grid, opts) {
                return datas.attributesInfo;
                //'/api/schema/queryview/getattributes/' + datas.queryId + '?__r=' + new Date().getTime();
            },
            rowDblClick: function (event, ui) {
            },
            loading: false,
            rowClick: function (event, ui) {
                var $table = $(event.target);
                var $tr = ui.$tr;
                _super.$wrap.trigger('gridview.rowClick', { e: event, $tr: $tr, ui: ui });
            },
            gridrefreshed: function ($grid, self, obj, e) {
                console.log($grid, self, obj, e)
                _super.$wrap.trigger('gridview.refreshed', { grid: $grid, that: self, obj: obj, e: e });
            },
            //  addCheckbox:false,
            checkName: 'recordid',
            headerFilter: true,
            pageModel: { type: "remote", rPP: 10, page: 1, strRpp: "{0}" },
            isWidthToMax: isWidthToMax,
            scrollModel: { autoFit: isWidthToMax },
            filterColModel: function (items) {
                return items;
            },
            columnFilter: function (items) {
                items[0].align = 'center';
                var delCol = {
                    title: " ", dataIndx: 'cdatagrid_editer', edittype: 'cdatagrid_editer', editable: false, minWidth: 40, width: 40, maxWidth: 40, notHeaderFilter: true, sortable: false, render: function (ui) {
                        var datas = ui.rowData;
                        var dataIndx = ui.dataIndx;
                        var column = ui.column;
                        var recordid = datas[dataIndx];
                        if (recordid == 'new') {
                            return '<button type="button" class="btn btn-link btn-xs" name="removeRowBtnLocal"><span class="glyphicon glyphicon-minus-sign"></span></button>';
                        } else {
                            return '<button type="button" class="btn btn-link btn-xs" name="removeRowBtn"><span class="glyphicon glyphicon-remove"></span></button>';
                        }
                    }
                };
                if (datas.formState == 'disabled' || datas.formState == 'readonly') {
                    delCol.hidden = true;
                }
                if (datas.iseditable == 'false') {
                    delCol.hidden = true;
                }
                items.splice(1, 0, delCol);
                if (datas.iseditable == "false") {
                    items.unshift();
                }
                //行事件绑定
                var rowsCommons = layoutconfigObj.rowcommand;
                // console.log(rowsCommons)
                if (rowsCommons && rowsCommons.length > 0) {
                    $.each(items, function (key, item) {
                        item.xmsLineEvent = xmsLineEvent;
                        item.rowsCommons = rowsCommons;
                    });
                }
                return items;
            }
        }

        // 操作列按钮配置
        if (datas.PagingEnabled == 'true') {
            if (datas.pagesize) {
                datagridconfig.pageModel.rPP = datas.pagesize;
            }
        }

        datagridconfig.initAfter = function ($grid) {
            //  datatableItemBtns($('.datatable-itembtn'));

            $grid.$plugGrid.pqGrid("option", "freezeCols", datas.freezenIndex);
            $grid.$plugGrid.pqGrid("refresh");

            $grid.$grid.on('click', '.forzen-ctrl', function () {
                var index = $(this).parents('td:first').index();
                datas.freezenIndex = index + 1;
                $grid.$plugGrid.pqGrid("option", "freezeCols", datas.freezenIndex);
                $grid.$plugGrid.pqGrid("refresh");
                $grid.$grid.find('.forzen-ctrl').removeClass('freeze-ctrl-active');
                $grid.$grid.find('.pq-grid-header-table .pq-grid-title-row td:eq(' + index + ')').find('.forzen-ctrl').addClass('freeze-ctrl-active');
            })

            if (datas.aggInfo) {
            }
        }
        datagridconfig.filterData = function (res, colmodel) {
            if (datas.aggregateconfig && datas.aggregateconfig != '') {
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
                                temp[nn.field] = _AggregateTypeList['_' + flag.aggregatetype] + (agginfo === null ? '' : agginfo);
                                res.fetchdata.items.push(temp);
                            }
                        }
                    });
                }
            }
            return res;
        }

        datagridconfig.refresh = function (evt, ui) {
            var index = datas.freezenIndex - 2;
            if (index < 0) {
                index = 1;
            }
            $context.find('.pq-grid-header-left .forzen-ctrl').eq(index).addClass('freeze-ctrl-active');
            _super.bindEvent($context, evt, ui);
        }

        var firstload = false;
        datagridconfig.extend = function (datagrid) {
            var extobj = {
                isJsonAjax: true,
                afterAjax: function (that, objP, DM, PM, FM,response) {
                    var pageIsEdit = datas.pageIsEdit;
                    var totalRecords = PM.totalRecords;
                    var emtpy = datas.DefaultEmptyRows - totalRecords;
                    if (pageIsEdit && !firstload && emtpy > 0 && response.IsSuccess!==false) {

                        opts._super._addEmptyRow(emtpy);
                        firstload = true;
                    }
                    opts._super.$wrap.trigger('gridview.afterAjax', { $context: $context, that: that, datagridconfig: datagridconfig })
                },
                getDataAfter: function () {
                    datas.gridviewLoaded && datas.gridviewLoaded();
                    opts._super.$wrap.trigger('gridview.getDataAfter')
                },
                beforeAjax: function (grid, objP, DM, PM, FM) {
                    opts._super.$wrap.trigger('gridview.beforeAjax', { grid: grid, objP: objP, DM: DM, PM: PM, FM: FM });
                    var pageIsEdit = datas.pageIsEdit;
                    if (pageIsEdit) {
                        return true;
                    } else {
                        grid.hideLoading();
                        grid.loading = false;
                        setTimeout(function () {
                            DM.data = _super.localDatas;
                            if (_super && _super.localDatas && _super.localDatas.length == 0) {
                                _super._addEmptyRow(datas.DefaultEmptyRows);
                            }
                            // opts._super.refresh();
                        }, 500);
                        datas.gridviewLoaded && datas.gridviewLoaded();
                        return false;
                    }
                },
                getData: function (dataJSON, textStatus, jqXHR) {
                    if (dataJSON.IsSuccess == false) {
                        $context.cDatagrid('disable');
                        opts._super.$toolbar.hide();
                       // Xms.Web.Toast(dataJSON.Content, false, 5000);
                    }
                    try {
                        var resjson = dataJSON.fetchdata
                        var data = resjson.items;
                    } catch (e) {
                        var resjson = {}
                        var data = []
                    }
                    $.each(data, function () {
                        this['cdatagrid_editer'] = 'old';
                    });
                    var curpage = resjson.currentpage
                    if (data.length == 0) {
                        curpage = 1;
                    }
                    console.log(dataJSON)
                    _super.localDatas = data;
                    var res = { curPage: curpage || 1, totalRecords: resjson.totalitems, data: data }
                    datas.oldAggInfo = datas.aggInfo = dataJSON.aggregatedata;
                    opts._super.$wrap.trigger('gridview.getData', { datas: res });
                    // alert(111);
                    return res;
                },

                filterSendData: function (postData, objP, DM, PM, FM, grid) {
                    //console.log('postdata', postData, objP, DM, PM, FM)
                    var pagesize = PM.rPP;

                    var ext = { sortby: objP.dataIndx, sortdirection: objP.dir == 'up' ? '0' : '1', pagesize: pagesize };

                    if (datas.filter) {
                        ext.filter = datas.filter;
                    }
                    var wrapFilter = _super.$wrap.attr('data-filter');
                    if (ext.filter && ext.filter.Filters && wrapFilter && wrapFilter != '') {
                        wrapFilter = JSON.parse(decodeURIComponent(wrapFilter));
                        ext.filter.Filters = [];
                        ext.filter.Filters.push(wrapFilter);
                    }
                    var $searchInput = _super.$searcher.find('input[name="Q"]');
                    if ($searchInput.length > 0 && $searchInput.val() != "") {
                        ext.q = $searchInput.val()
                    }
                    $.extend(postData, ext);
                    return postData;
                }
            }
            if (datas.layoutconfig) {
                var _layconfig = JSON.parse(datas.layoutconfig);
                if (_layconfig.sortcolumns && _layconfig.sortcolumns.length > 0) {
                    extobj.sortIndx = _layconfig.sortcolumns[0].name;
                    extobj.sortDir = _layconfig.sortcolumns[0].sortascending ? 'up' : 'down';
                }
            }
            $.extend(datagrid.opts.dataModel, extobj)
            $('body').trigger('rendergridview.datagridExtend', { datagrid: datagrid, datas: datas });
        }
        //设置表格高度
        var parHeight = 0, height = 350, fixHeight = -30;
        if (parent && parent.window) {
            var offsetT = $context.offset().top;
            var scrolltop = $(window).scrollTop();
            height = parent.window.innerHeight + scrolltop - offsetT - fixHeight;
        } else {
            height = 450 - 80;
        }
        if (height > 300) {
            datagridconfig.height = height * 1;
        }
        datagridconfig.height = 300;
        //  datagridconfig.filter = gridview_filters;
        //  datagridconfig.filter = datas.filter;
        $('body').trigger('rendergridview.setDataGridConfig', { datagridconfig: datagridconfig });
        $context.cDatagrid(datagridconfig)
        return $context;
    }

    var datagridView = new function () {
        this.getGridDatas = function (gridid) {
            var $grid = $('#' + gridid);
            if ($grid.length > 0) {
                // $grid.children().c
            }
        }
    }

    window.datagridView = datagridView;
    window.entityDatagirdList = entityDatagirdList;
    window.entityDatagrid = entityDatagrid;
    window.loadDataGrid = loadDataGrid;

    window.tableGrid = tableGrid;

    function tableGrid($context, extopt) {
        this.$context = $context;
        var entityid;
        var entityname;
        var queryid = extopt.queryid;
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
            fetchconfig: '',
            name: '',
            aggregateconfig: '',
            oldaggregateconfig: '',
            globtnstr: '',
            freezenIndex: 2,
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
            }
        }
        $.extend(datas, extopt);

        this.init = function () {
            this.loadPageInfo();
        }
        this.loadPageInfo = function () {
            this.loadQueryViews();
        }
        this._filterEnableViews = function (datas) {
            return $.grep(datas, function (n, i) {
                if (!n.isdisabled) {
                    return true;
                }
            });
        }
        this.loadQueryViews = function (type, value, callback) {
            var url = ORG_SERVERURL + '/api/schema/queryview/GetViewInfo?';
            if (!type) {
                if (entityname) {
                    url += 'entityname=' + entityname;
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
            var self = this
            Xms.Web.Get(url, function (res) {
                console.log('getbyentityid', JSON.parse(res.Content));
                var jsonres = datas.jsonres = JSON.parse(res.Content);
                datas.queryviews = jsonres.views;

                if (datas.queryviews && datas.queryviews.length > 0) {
                    //只加载启用的
                    datas.queryviews = self._filterEnableViews(datas.queryviews);
                    if (datas.queryviews && datas.queryviews.length == 1) {
                        self.setPageInfo(0, datas.queryviews[0], jsonres);
                        self.loadQueryViewInfo(null, jsonres);
                    } else if (datas.queryviews && datas.queryviews.length > 1) {
                        //设置页面信息,第一次加载时默认第一个为当前的视图
                        var index = self._getDefaultViewKey(datas.queryviews);
                        self.setPageInfo(index, null, jsonres);
                        self.loadQueryViewInfo(null, jsonres);
                    }
                }
            });
        }
        this.filterAttributes = function (items) {
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
        }
        this.loadQueryViewInfo = function (callback, jsonres) {
            var self = this;
            //加载字段数据
            if (datas.attributesInfo) {
                $.each(datas.attributesInfo, function () {
                    if (this.name != '') {
                        this.name = this.name.toLowerCase();
                    }
                });
            }
            datas.setAttributesShow = datas.attributesInfo = self.filterAttributes(datas.attributesInfo);

            //加载列表数据
            self.loadDataTable(this.$context);

            //绑定事件
            // self.bindEvent();
            callback && callback();
        }
        this.setPageInfo = function (key, first, jsonres) {
            if (datas.queryviews) {
                key = key || 0;
                first = first || datas.queryviews[key];
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

                datas.EntityId = datas.entityId;
                datas.QueryId = datas.queryId;
                datas.TargetFormId = first.targetformid;
            }
        }

        this.loadDataTable = function ($context, isDestroy) {
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
                    var pagesize = cdatagrid.opts.pageModel.rPP;

                    return ORG_SERVERURL + '/api/data/fetchAndAggregate?entityid=' + datas.entityId + '&queryviewid=' + datas.queryId + '&onlydata=true&pagesize=' + pagesize + '&page=' + cdatagrid.opts.pageModel.page
                },

                selectionModel: { type: null },
                getColModels: function (grid, opts) {
                    return datas.attributesInfo;
                },

                loading: false,
                addCheckbox: false,
                checkName: 'recordid',
                headerFilter: true,
                pageModel: { type: "remote", rPP: 10, page: 1, strRpp: "{0}" },
                isWidthToMax: isWidthToMax,
                scrollModel: { autoFit: isWidthToMax },
                filterColModel: function (items) {
                    return items;
                }
            }

            //添加统计信息行
            datagridconfig.extend = function (datagrid) {
                var extobj = {
                    isJsonAjax: true,
                    beforeAjax: function () {
                        if (extopt.isNotLoadData) {
                            return false;
                        }
                        return true;
                    },
                    getData: function (dataJSON, textStatus, jqXHR) {
                        var resjson = dataJSON.fetchdata || { currentpage: 1, totalitems: 0 }
                        var data = resjson ? resjson.items : [];
                        console.log(dataJSON)
                        var res = { curPage: resjson.currentpage || 1, totalRecords: resjson.totalitems, data: data }
                        datas.oldAggInfo = datas.aggInfo = dataJSON.aggregatedata;
                        // alert(111);
                        return res;
                    },
                    filterSendData: function (postData, objP, DM, PM, FM) {
                        //console.log('postdata', postData, objP, DM, PM, FM)
                        $.extend(postData, { sortby: objP.dataIndx, sortdirection: objP.dir == 'up' ? '0' : '1', pagesize: PM.rPP });
                        return postData;
                    }
                }
                if (datas.fetchconfig) {
                    var _fetchconfig = JSON.parse(datas.fetchconfig.toLowerCase());
                    if (_fetchconfig.orders && _fetchconfig.orders.length > 0) {
                        extobj.sortIndx = _fetchconfig.orders[0].attributename;
                        extobj.sortDir = _fetchconfig.orders[0].ordertype == 'descending' ? 'up' : 'down';
                    }
                }
                $.extend(datagrid.opts.dataModel, extobj)
            }

            datagridconfig.height = 350;
            $context.cDatagrid(datagridconfig)
        }
    }
});