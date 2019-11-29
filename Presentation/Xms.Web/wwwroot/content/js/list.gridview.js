//@ sourceURL=datagridpage.js
var Datagrid_page = function (GridViewModel, gridModel, sumsColumn) {
    if (typeof GridViewModel === 'undefined') { throw new Error('GridViewModel 不能为空'); }
    var grid = this.grid = null;
    this.GridViewModel = GridViewModel;
    var urlConfig = {
        getData: 'rendergridview?entityid=' + Xms.Page.PageContext.entityid + '&onlydata=true&pagesize' + 20 + '&page=' + 1
    }

    var utils = {
        getParamsByArray: function (arr, key, value) {
            var res = [];
            $.each(arr, function (i, n) {
                if (n[key] == value) {
                    res.push(n);
                }
            });
            return res;
        }
    }
    this.init = function () {
        var config = getDataGridConfig(datagridConfig);
        var $grid = $(GridViewModel.sectionid);
        var formular = gridFormular.formularInit($grid);
        connectFormularConfig(config, formular);
        console.log('gridconfig', config);
        grid = $(GridViewModel.sectionid).pqGrid(config);
        this.grid = grid;
        this.bindEvent();
    }

    this.bindEvent = function () {
        grid.on('pqgridrefresh pqgridrefreshrow', function () {
            var $grid = $(this);

            //delete button
            $grid.find("button.delete_btn").button({ icons: { primary: 'ui-icon-close' } })
                .unbind("click")
                .bind("click", function (evt) {
                    if (isEditing($grid)) {
                        return false;
                    }
                    var $tr = $(this).closest("tr"),
                        rowIndx = $grid.pqGrid("getRowIndx", { $tr: $tr }).rowIndx;
                    deleteRow(rowIndx, $grid);
                });
            //edit button
            $grid.find("button.edit_btn").button({ icons: { primary: 'ui-icon-pencil' } })
                .unbind("click")
                .bind("click", function (evt) {
                    if (isEditing($grid)) {
                        return false;
                    }
                    var $tr = $(this).closest("tr"),
                        rowIndx = $grid.pqGrid("getRowIndx", { $tr: $tr }).rowIndx;
                    editRow(rowIndx, $grid);
                    return false;
                });

            //rows which were in edit mode before refresh, put them in edit mode again.
            var rows = $grid.pqGrid("getRowsByClass", { cls: 'pq-row-edit' });
            if (rows.length > 0) {
                var rowIndx = rows[0].rowIndx;
                editRow(rowIndx, $grid);
            }
        });

        grid.on("pqgridcellsave", function (event, ui) {
        });

        grid.on("pqgrideditorbegin", function (event, ui) {
        });
        grid.on("pqgridcellbeforesave", function (event, ui) {
            var dataIndx = ui.dataIndx, rowData = ui.rowData, rowIndx = ui.rowIndx, newVal = ui.newVal, $editor = ui.$editor, colModel = grid.pqGrid("getColModel");
            rowData.isEdited = true;
            var itemArr = gridFormular.formularInit(grid)();
            gridFormular.setFormularAndRelation(itemArr, function (itemArr, tempArr, itemRuler, leftArr, rightRuler) {
                if (~itemArr.indexOf(dataIndx)) {
                    gridFormular.getFormularResult(leftArr, tempArr[0], rightRuler, rowData, dataIndx, newVal, colModel, function (res) {
                        grid.pqGrid("refreshRow", { rowIndx: rowIndx });
                    });
                }
                console.log(itemArr, tempArr, itemRuler, leftArr, rightRuler);
            });
        });
        grid.on("pqgridcellsave", function (event, ui) {
            var dataIndx = ui.dataIndx, rowData = ui.rowData;

            console.log(dataIndx, rowData);
        });
    }

    var bottomsInfo = [
        {
            type: "button",
            label: '保存',
            icon: 'ui-icon-disk',
            listeners: [{
                click: function (a, b) {
                    console.log('listeners', a, grid.pqGrid("getRowData"));
                    var columns = grid.pqGrid("getColModel");
                    var len = grid.find('.pq-grid-table tr').length - 1;
                    var primarykey = '';
                    var datas = [];
                    for (var i = 0; i < len; i++) {
                        var rowdata = grid.pqGrid("getRowData", { rowIndxPage: i });
                        if (rowdata) {
                            datas.push(rowdata);
                        }
                    }

                    datas = $.extend(true, [], datas);
                    console.log(datas);
                    var res = [];
                    $.each(datas, function (key, item) {
                        if (item.isEdited) {
                            var obj = { entityid: GridViewModel.entityid, data: {} };
                            obj.data = item;
                            obj.name = GridViewModel.entityname;
                            obj.relationshipname = GridViewModel.relationshipname;
                            // obj.data.primarykey = grid.opts.primarykey;
                            console.log(GridViewModel.entityname);
                            obj.data[GridViewModel.referencingattributename] = GridViewModel.referencedrecordid;
                            obj.data.primarykey = getPrimaryKey(grid)();
                            obj.data.id = item[obj.data.primarykey];
                            res.push(obj);
                        }
                    });
                    console.log(res);
                    var postData = {
                        child: encodeURIComponent(JSON.stringify(res)),
                        entityname: GridViewModel.entityname,
                        parentid: Xms.Page.PageContext.RecordId
                    }
                    purecms.post('/admin/dataservice/savechilds', postData, function () {
                    }, function (response) {
                        //$(document).trigger('subgrid.save', { data: response });
                        //GridViewModel.rebind(null, function () {
                        //    console.log($(GridViewModel.sectionid).parents('.subgrid:first'))
                        //    if (typeof setSubGridFormular == 'function') {
                        //        setSubGridFormular($(GridViewModel.sectionid).parents('.subgrid:first'), 'gridview');
                        //    }
                        //});
                        console.log(response);
                    });
                    // console.log(grid.pqGrid( "getData"));
                }
            }]
        },
        {
            type: "button",
            label: '新增行',
            icon: 'ui-icon-plus',
            listeners: [{
                click: function (a, b) {
                    var primarykey = getPrimaryKey(grid)();
                    var rowdata = { rowData: {} };
                    rowdata.rowData[primarykey] = Xms.Utility.Guid.NewGuid().ToString('N');
                    grid.pqGrid('addRow', rowdata);
                    // console.log(grid.pqGrid( "getData"));
                }
            }]
        },
        {
            type: "button",
            label: '删除',
            icon: 'ui-icon-plus',
            listeners: [{
                click: function (e, b) {
                    // console.log(grid.pqGrid( "getData"));
                }
            }]
        }
    ];

    var datagridConfig = {
        height: '300',
        width: "100%",
        scrollModel: { autoFit: true },
        showTop: true,
        showBottom: true,
        title: GridViewModel.gridid,
        //collapsible: false,
        rowHt: 30,
        autoRow: false,
        wrap: false,
        hwrap: false,
        resizable: true,
        columnBorders: false,
        trackModel: { on: true }, //to turn on the track changes.
        pageModel: { type: "remote", rPP: GridViewModel.pagesize, page: GridViewModel.page, strRpp: "{0}" },
        showHeader: true,
        roundCorners: true,
        rowBorders: true,
        columnBorders: true,
        selectionModel: { type: 'row' },
        numberCell: { show: false },
        theme: true,
        postRenderInterval: -1, //synchronous post rendering.
        toolbar: {
            items: bottomsInfo
        },
    };
    datagridConfig.colModel = gridModel;

    datagridConfig.dataModel = {
        location: "remote",
        dataType: "JSON",
        method: "POST",
        getUrl: function (opts) {
            console.log('opts', opts);

            var _url = urlConfig.getData;
            if (opts) {
                var pagemodel = opts.pageModel;
                var curpage = pagemodel.curPage;
                _url = $.setUrlParam(_url, 'page', curpage);
            }

            return { url: _url };
        },
        postData: { pagesize: GridViewModel.pagesize, page: GridViewModel.page },
        //url: "/pro/products.php",//for PHP
        getData: function (dataJSON, textStatus, jqXHR) {
            var data = dataJSON.items;
            var res = { curPage: dataJSON.currentpage || 1, totalRecords: dataJSON.totalitems, data: data }
            console.log(res);
            return res;
        }
    }

    function isEditing($grid) {
        var rows = $grid.pqGrid("getRowsByClass", { cls: 'pq-row-edit' });
        if (rows.length > 0) {
            var rowIndx = rows[0].rowIndx;
            $grid.pqGrid("goToPage", { rowIndx: rowIndx });
            //focus on editor if any
            $grid.pqGrid("editFirstCellInRow", { rowIndx: rowIndx });
            return true;
        }
        return false;
    }
    //called by add button in toolbar.
    //function addRow($grid) {
    //    if (isEditing($grid)) {
    //        return false;
    //    }
    //    //append empty row in the first row.
    //    var rowData = { UnitPrice: 0, UnitsInStock: 0, UnitsOnOrder: 0, Discontinued: false, ProductName: "" }; //empty row template
    //    $grid.pqGrid("addRow", { rowIndxPage: 0, rowData: rowData });

    //    var $tr = $grid.pqGrid("getRow", { rowIndxPage: 0 });
    //    if ($tr) {
    //        //simulate click on edit button.
    //        $tr.find("button.edit_btn").click();
    //    }
    //}
    //called by delete button.
    function deleteRow(rowIndx, $grid) {
        $grid.pqGrid("addClass", { rowIndx: rowIndx, cls: 'pq-row-delete' });
        var rowData = $grid.pqGrid("getRowData", { rowIndx: rowIndx });

        purecms.del(rowData[getPrimaryKey(grid)()], '/admin/entity/delete?entityid=' + GridViewModel.entityid, false, function () {
            $grid.pqGrid("deleteRow", { rowIndx: rowIndx });
            grid.pqGrid("refresh");
        });
    }
    //called by edit button.
    function editRow(rowIndx, $grid) {
        //debugger;
        $grid.pqGrid("addClass", { rowIndx: rowIndx, cls: 'pq-row-edit' });
        //$grid.pqGrid("refreshRow", { rowIndx: rowIndx});

        $grid.pqGrid("editFirstCellInRow", { rowIndx: rowIndx });

        ////change edit button to update button and delete to cancel.
        var $tr = $grid.pqGrid("getRow", { rowIndx: rowIndx }),
            $btn = $tr.find("button.edit_btn");
        $btn.button("option", { label: "Update", "icons": { primary: "ui-icon-check" } })
            .unbind("click")
            .click(function (evt) {
                evt.preventDefault();
                return update(rowIndx, $grid);
            });
        $btn.next().button("option", { label: "Cancel", "icons": { primary: "ui-icon-cancel" } })
            .unbind("click")
            .click(function (evt) {
                $grid.pqGrid("quitEditMode")
                    .pqGrid("removeClass", { rowIndx: rowIndx, cls: 'pq-row-edit' })
                    .pqGrid("rollback");
            });
        //$grid.addClass({ rowIndx: rowIndx, cls: 'pq-row-edit' });
        //$grid.refreshRow({rowIndx: rowIndx});

        //$grid.editFirstCellInRow({ rowIndx: rowIndx });
    }
    //called by update button.
    function update(rowIndx, $grid) {
        var grid = $grid.pqGrid('getInstance').grid;
        if (grid.saveEditCell() == false) {
            return false;
        }

        var isValid = grid.isValid({ rowIndx: rowIndx }).valid;
        if (!isValid) {
            return false;
        }

        if (grid.isDirty()) {
            var url,
                rowData = grid.getRowData({ rowIndx: rowIndx }),
                recIndx = grid.option("dataModel.recIndx"),
                type;

            grid.removeClass({ rowIndx: rowIndx, cls: 'pq-row-edit' });

            if (rowData[recIndx] == null) {
                //add record.
                type = 'add';
                url = "/pro/products/add"; //ASP.NET, java
                //url = "/pro/products.php?pq_add=1"; //for PHP
            }
            else {
                //update record.
                type = 'update';
                url = "/pro/products/update"; //ASP.NET, java
                //url = "/pro/products.php?pq_update=1"; //for PHP
            }
            $.ajax($.extend({}, ajaxObj, {
                context: $grid,
                url: url,
                data: rowData,
                success: function (response) {
                    if (type == 'add') {
                        rowData[recIndx] = response.recId;
                    }
                    //debugger;
                    grid.commit({ type: type, rows: [rowData] });
                    grid.refreshRow({ rowIndx: rowIndx });
                }
            }));
        }
        else {
            grid.quitEditMode();
            grid.removeClass({ rowIndx: rowIndx, cls: 'pq-row-edit' });
            grid.refreshRow({ rowIndx: rowIndx });
        }
    }

    function getPrimaryKey(grid) {
        var primarykey = '';
        return function () {
            if (primarykey == '') {
                var columns = grid.pqGrid("getColModel");
                $.each(columns, function (i, n) {
                    if (n.key == true) {
                        primarykey = n.dataIndx;
                        return true;
                    }
                });
                return primarykey;
            } else {
                return primarykey;
            }
        }
    }

    function dataGridFactory($grid) {
        var datagrid = $grid;
        var config = getDataGridConfig(datagridConfig);
        console.log('gridconfig', config);
        var _grid = $(GridViewModel.sectionid).pqGrid(config);
        return _grid;
    }

    function getDataGridConfig(datagridinfo) {
        datagridinfo.colModel = filterColumns(datagridinfo.colModel);
        datagridinfo.colModel.unshift(
            {
                title: "", editable: false, minWidth: 165, sortable: false, render: function (ui) {
                    return "<button type='button' class='edit_btn'>Edit</button>\
                            <button type='button' class='delete_btn'>Delete</button>";
                }
            });
        return datagridinfo;
    }

    function filterColumns(columnconfigs) {
        var res = [];
        console.log('columnconfigs,', columnconfigs)
        $.each(columnconfigs, function (key, item) {
            if (item) {
                var obj = getColumnInfo(item);
                res.push(obj);
            }
        });
        return res
    }
    //处理不同数据类型的
    function getColumnInfo(item) {
        var edittype = 'string';
        var obj = {
            dataIndx: item.name,
            title: item.label,
            dataType: 'string',
            //  editable:item.editable,
            width: item.width || 100,
        }
        if (item.edittype && item.edittype != "") {
            //item.editable = true;
        }
        if (item.metadata && item.metadata.attributetypename == "nvarchar") {
            edittype = "string";
        } else if (item.metadata && item.metadata.attributetypename == "picklist") {
            //console.log("optionset-type", obj.metadata&&obj.metadata.attributetypename);
            edittype = 'picklist';
            obj.editModel = { saveKey: item.name.replace(/name$/, 'id'), keyUpDown: false }
            obj.edittype = edittype;
            setEditorInfo(obj, item);
        } else if (item.metadata && item.metadata.attributetypename == "lookup" || item.metadata && item.metadata.attributetypename == "owner" || item.metadata && item.metadata.attributetypename == "customer") {
            edittype = 'lookup'
            obj.edittype = edittype;
            setEditorInfo(obj, item);
        } else if (item.metadata && (item.metadata.attributetypename == "datetime" || item.metadata.attributetypename == "CreatedOn")) {
            edittype = 'datetime'
        }
        //如果为关联字段，则不可编辑
        if (~obj.dataIndx.indexOf('.')) {
            obj.editable = false;
            obj.isrelation = true;
        }
        if (item.key == true) {
            obj.hidden = true;
            obj.key = true;
        }
        // obj.precision = item.metadata.precision;
        // item.isEdited = false;

        return obj;
    }

    function setEditorInfo(obj, item) {
        if (obj.edittype == 'picklist') {
            var options = $.map(item.metadata.optionset.items, function (n, i) {
                if (item) {
                    return { text: n.name, value: n.value };
                }
            });
            obj.editor = {
                type: //'select'
                    function (ui) {
                        //debugger;
                        var $cell = ui.$cell,
                            rowData = ui.rowData,
                            dataIndx = ui.dataIndx,
                            width = ui.column.width,
                            cls = ui.cls;
                        var dc = $.trim(rowData[dataIndx.replace(/name$/, 'id')]);

                        var $inp = $("<input type='hidden' name='" + dataIndx + "' class='" + cls + " pq-ac-editor' />")
                            .width(width - 6)
                            .appendTo($cell)
                            .val(dc);
                        $inp.picklist({
                            required: $inp.is('.required'),
                            items: item.metadata.optionset.items,
                            isDefault: true,
                            changeHandler: function (e, obj) {
                            }
                        });

                        setTimeout(function () {
                            $inp.next().addClass('pq-editor-focus').focus().on('blur', function () {
                                $inp.trigger('blur')
                            });
                        }, 50);
                    }
                ,
                options: options,
                labelIndx: 'text',
                valueIndx: 'value',

                prepend: {
                },
                getData: function (ui) {
                    var clave = ui.$cell.find("select").val();
                    var rowData = grid.pqGrid("getRowData", { rowIndx: ui.rowIndx });
                    rowData[item.name.replace(/name$/, 'id')] = clave;
                    rowData[item.name] = ui.$cell.find("select option[value='" + clave + "']").text();
                    console.log(rowData)
                    //grid.pqGrid("refreshRow");
                    return ui.$cell.find("select option[value='" + clave + "']").text();
                }
            }
        } else if (obj.edittype == 'lookup') {
            obj.editor = {
                type: //'select'
                    function (ui) {
                        console.log(ui);
                        //debugger;
                        var $cell = ui.$cell,
                            rowData = ui.rowData,
                            dataIndx = ui.dataIndx,
                            rowIndx = ui.rowIndx,
                            width = ui.column.width,
                            cls = ui.cls;
                        var dc = $.trim(rowData[dataIndx.replace(/name$/, '')]);
                        var dctext = $.trim(rowData[dataIndx]);
                        var $in = $("<input type='hidden' name='" + dataIndx + "' id='" + dataIndx + "' class='" + cls + " pq-ac-editor' />")
                            .width(width - 6)
                            .appendTo($cell)
                            .val(dc);
                        $in.attr('data-value', dc);
                        $in.attr('data-text', dctext);
                        var field = dataIndx.replace(/name$/, 'id');

                        //if (!editor.props.$inputext) {
                        var $input = $('<input type="text" class="form-control" id="' + field + '_text" name="' + field + '_text" class="" />');
                        var $value = $('<input type="hidden" class="form-control" id="' + field + '" name="' + field + '" class=""  />');
                        // }
                        $input.appendTo($cell);
                        $value.appendTo($cell);
                        var self = $input;

                        if (!self.prop('id')) {
                            self.prop('id', self.prop('name') + Xms.Utility.Guid.NewGuid().ToString('N'));
                        }
                        //console.log('self',self)
                        self.attr('data-lookup', item.metadata.referencedentityid)
                        var lookupid = item.metadata.referencedentityid;
                        var inputid = self.prop('id');
                        var valueid = dataIndx;
                        //self.after($value);
                        // $input.hide();
                        var valueid = inputid.replace(/_text/, '');
                        if (!$input.attr('id')) {
                            $input.attr('id', valueid);
                        }
                        if (dc != '') {
                            $input.val(dctext);
                            $value.val(dc);
                        }

                        //var parentTr = self.parents('tr:first');
                        var _isRelative = false;
                        ////var queryid = self.attr('data-defaultviewid');
                        //if (!~checkTrInList(_trlist, parentTr)) {
                        //    lookuplist = [];
                        //    _trlist.push(parentTr);
                        //}

                        //if (!~$.inArray(lookupid, lookuplist)) {
                        //    _isRelative = true;
                        //    lookuplist.push(lookupid);
                        //}
                        //console.log(self.prop('name')+'    lookuplist',_isRelative);
                        // self.attr('data-_isRelative', _isRelative);
                        var value = self.val() || '';
                        //if(value && value!=''){
                        //    console.log('valueid',$('#' + valueid))
                        //    console.log('inputid',$('#' + valueid))
                        //}
                        // self.next().prop('id', valueid);
                        var lookupurl = '/admin/entity/RecordsDialog?inputid=' + inputid + '&sortby=CreatedOn&singlemode=true';

                        self.lookup({
                            dialog: function () {
                                var f = $('#' + inputid).attr("data-filter");
                                if (f) f = JSON.parse(decodeURIComponent(f));
                                else f = null;
                                if (self.attr('data-defaultviewid') && self.attr('data-defaultviewid') != '') {
                                    lookupurl = $.setUrlParam(lookupurl, 'queryid', self.attr('data-defaultviewid'));
                                    // lookupurl += '&queryid=' + self.attr('data-defaultviewid');
                                }
                                else {
                                    lookupurl = $.setUrlParam(lookupurl, 'entityid', self.attr('data-lookup'));
                                }
                                if (f) {
                                    purecms.opendialog(lookupurl, 'dataGridselectRecordCallback', { filter: f });
                                } else {
                                    purecms.opendialog(lookupurl, 'dataGridselectRecordCallback');
                                }
                            }
                            , clear: function () {
                                $('#' + inputid).val('');
                                $('#' + valueid).val('');
                                if (self.attr('data-_isRelative') && self.attr('data-_isRelative') == "true") {
                                    var relationData = $('#' + inputid).data().relationData;
                                    if (relationData && relationData.length > 0) {
                                        $.each(relationData, function (key, item) {
                                            var type = item.td.attr('data-type');
                                            // console.log(attrname,type,eval('data.' + attrname));
                                            if (type == 'nvarchar' || type == 'money' || type == 'int') {
                                                item.td.find('input[name=' + GridViewModel.nameprefix + item.attrname + ']').val('');
                                            }
                                            else if (type == 'owner' || type == 'lookup' || type == 'customer' || type == 'picklist' || type == 'state' || type == 'bit' || type == 'status') {
                                                item.td.text('');
                                            } else {
                                                item.td.text('');
                                            }
                                        });
                                    }
                                }
                                if ($('#' + inputid).siblings(".xms-dropdownLink").length > 0) {
                                    $('#' + inputid).siblings(".xms-dropdownLink").remove();
                                }
                                return false;
                            }
                            , isDefaultSearch: true
                            , isShowSearch: true,
                            searchOpts: {
                                id: lookupid
                                , addHandler: function (tar, obj, par) {
                                    purecms.console('lookup', tar)

                                    //console.log(tar)
                                    //  $(par.obj).trigger("lookup.triggerChange");
                                }
                                , delHandler: function (input) {
                                    var tarid = input.attr("id").replace("_text", "");
                                    var tarDom = $("#" + tarid);
                                    var tagContext = $('div[data-sourceattributename="' + tarid + '"]');
                                    tagContext.html('');
                                }
                            }
                        });
                        $('#' + inputid).on('dialog.return', function (e, obj) {
                            // $(this).parents('tr:first').attr('data-edited', true);
                            // console.log(obj);
                            // $input.val(obj.id);
                            // $(this).val(obj.name);
                            //$box.html(obj.name);
                            $in.attr('data-value', obj.id);
                            $in.attr('data-text', obj.name);
                            var that = this;
                        });

                        setTimeout(function () {
                            grid.off('click.grid-lookup-blur').on('click.grid-lookup-blur', function (e) {
                                if ($(e.target).closest($cell).length == 0) {
                                    $in.trigger('blur');
                                    grid.off('click.grid-lookup-blur')
                                }
                            });
                        }, 50);

                        $('#' + inputid).bind('change', function () {
                            var $this = $(this);
                            var inputid = $(this).prop('id');
                            var valueid = inputid.replace(/_text/, '');
                            var v = $('#' + valueid).val();
                            var entityid = $(this).attr('data-lookup');
                            var relationData = $this.data().relationData = [];
                            var _isRelative = true;
                            //带出关联字段的数据
                            if (ui.column.isrelation == true) {
                                _isRelative = true;
                            }
                            //console.log(entityid, v, valueid);
                            if (v && v != '') {
                                //purecms.getjson('/admin/dataservice/RetriveReferencedRecord', {entityid: entityid, value:v, allcolumns:true}, function(response){
                                var params = {
                                    type: entityid + v + 'true',
                                    data: { entityid: entityid, value: v, allcolumns: true }
                                }
                                console.log('_isRelative', _isRelative);
                                if (_isRelative == true) {
                                    purecms.pageCache('renderGridView', '/admin/dataservice/RetriveReferencedRecord', params, function (response) {
                                        var data = response.content;
                                        console.log('response', data);
                                        for (var i in rowData) {
                                            if (rowData.hasOwnProperty(i)) {
                                                var objitem = rowData[i];
                                                var colModel = grid.pqGrid('getColModel');
                                                var ifield = dataIndx.replace(/name$/, '');
                                                if (~i.indexOf(ifield) && ~i.indexOf('.')) {
                                                    var attrs = i.split('.');
                                                    var col = utils.getParamsByArray(colModel, 'dataIndx', i);
                                                    if (col.length > 0) {
                                                        var type = col.edittype;
                                                        if ((objitem !== '' && objitem !== null && objitem !== undefined)) {
                                                            // console.log(attrname,type,eval('data.' + attrname));
                                                            if (type == 'nvarchar' || type == 'money' || type == 'int') {
                                                                rowData[i] = data[attrs[1]];
                                                                // $td.find('input[name=' + GridViewModel.nameprefix + ifield + ']').val(data[attrname]);
                                                            }
                                                            else if (type == 'owner' || type == 'lookup' || type == 'customer' || type == 'picklist' || type == 'state' || type == 'bit' || type == 'status') {
                                                                //var _field = i.replace(/name$/, 'id');
                                                                rowData[i] = data[ifield + 'name']
                                                                // rowData[i] = data[dataIndx];
                                                                // rowData[_field] = data[_field];
                                                            } else {
                                                                rowData[i] = data[attrs[1]];
                                                                //rowData[ifield] = data[ifield];
                                                            }
                                                            grid.pqGrid("refreshRow", { rowIndx: rowIndx });
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        //var $row = $this.parents('tr:first');
                                        //$row.find('td[data-name]').each(function (i, n) {
                                        //    var $td = $(this);
                                        //    if ($('#' + valueid).parents('td').is($td)) return true;

                                        //    var attrname = $td.attr('data-name') ? $td.attr('data-name').toLowerCase() : '';
                                        //    if (!attrname) return true;
                                        //    var type = $td.attr('data-type');
                                        //    if ($td.attr('data-entityname').toLowerCase() != GridViewModel.entityname.toLowerCase()) {//关联记录带出字段内容
                                        //        relationData.push({ td: $td, attrname: attrname });
                                        //        if ((data[attrname] !== '' && data[attrname] !== null && data[attrname] !== undefined)) {
                                        //            // console.log(attrname,type,eval('data.' + attrname));
                                        //            if (type == 'nvarchar' || type == 'money' || type == 'int') {
                                        //            }
                                        //            else if (type == 'owner' || type == 'lookup' || type == 'customer' || type == 'picklist' || type == 'state' || type == 'bit' || type == 'status') {
                                        //            } else {
                                        //            }

                                        //        }
                                        //    }
                                        //});
                                        // $this.trigger('dialog.relationReturn', { row: $row, data: data });
                                    });
                                }
                                // });
                            }
                        });

                        var attrName = self.attr('data-name') ? self.attr('data-name').toLowerCase() : '';
                    }
                ,
                labelIndx: 'text',
                valueIndx: 'value',

                prepend: {
                },
                getData: function (ui) {
                    var clave = ui.$cell.find("input.pq-ac-editor").attr('data-value');
                    var text = ui.$cell.find("input.pq-ac-editor").attr('data-text');
                    var rowData = grid.pqGrid("getRowData", { rowIndx: ui.rowIndx });
                    if (clave) {
                        rowData[item.name.replace(/name$/, '')] = clave;
                        rowData[item.name] = text;
                        console.log(rowData)
                        //grid.pqGrid("refreshRow");
                    }
                    return text;
                }
            }
        }

        window.formular_utils = utils
    }

    function connectFormularConfig(config, formularlist) {
        if (formularlist.length > 0) {
            $.each(formularlist, function (key, item) {
                if (item.type == 'formular') {
                    var isLeft = false;
                    if (item.expression.indexOf('$$$') > -1) {
                        var itemArr = item.expression.split('$$$');
                    } else {
                        var itemArr = JSON.parse(item.expression);
                    }
                    if (itemArr[1] == '=') {
                        itemArr = itemArr.reverse();
                        isLeft = false;
                    }
                    var itemRuler = itemArr.join('');
                    var tempArr = itemRuler.split('=');
                    var leftArr = getLeftFormular(itemArr, isLeft);//获取等号左边的等式
                    var rightRuler = itemArr[itemArr.length - 1];
                }
            });
        }
    }
}

    ; (function () {
        function getLeftFormular(items, isLeft) {//isLeft   需计算的字段是否在字段左边
            var arr = [];
            $.each(items, function (i, n) {
                if (n == "=") {
                    return false;
                } else {
                    if (!checkFormularRuler(n)) {
                        arr.push(n);
                    }
                }
            });
            if (isLeft) {
                arr = arr.reverse();
            }
            return arr;
        }
        function checkFormularRuler(nn) {
            var temp = nn;
            nn = {}; nn.key = temp;
            return nn.key == "+" || nn.key == "-" || nn.key == "*" || nn.key == "/" || nn.key == "=" || nn.key == "(" || nn.key == ")";
        }

        function getFormularResult(leftArr, leftStr, rights, context, dataIndx, newVal, colModel, callback) {//左边的字段，左边的等式
            $.each(leftArr, function (key, item) {
                if (dataIndx == item) {
                    var itemname = dataIndx;
                    var itemVal = newVal;
                } else {
                    var itemname = item;
                    var itemObj = context[itemname.toLowerCase()] || 0;
                    var itemVal = 0;
                    if (itemObj !== 0) {
                        itemVal = itemObj != '' ? (itemObj + '').replace(/\,/g, '') : '';
                    }
                }

                var reg = new RegExp(itemname.toLowerCase());
                leftStr = leftStr.toLowerCase().replace(reg, itemVal);
            });
            var res = 0;
            try {
                res = eval(leftStr);//计算公式
                var rightobj = context[rights.toLowerCase()];
                var col = formular_utils.getParamsByArray(colModel, 'dataIndx', rights);
                if (col.length > 0) {
                    var type = col[0].edittype;
                    //var precision = col[0].precision;
                    // if (type == 'money' && res && res != '') {
                    //     res = (res * 1).toFixed(precision || 2);
                    // }
                }

                context[rights.toLowerCase()] = res;
                console.log(context);
                callback && callback(rightobj);
            } catch (e) {
                // console.log(e);
            }
        }
        //获取计算公式
        function formularInit(grid) {
            var self = this;
            var formular = null;
            return function () {
                if (!formular) {
                    formular = this.formulars = getAndBindFormularInfo(grid);
                }
                return formular;
            }
        }

        function setFormularAndRelation(formularlist, callback) {
            if (formularlist.length > 0) {
                $.each(formularlist, function (key, item) {
                    if (item.type == 'formular') {
                        var isLeft = false;
                        if (item.expression.indexOf('$$$') > -1) {
                            var itemArr = item.expression.split('$$$');
                        } else {
                            var itemArr = JSON.parse(item.expression);
                        }
                        if (itemArr[1] == '=') {
                            itemArr = itemArr.reverse();
                            isLeft = false;
                        }
                        var itemRuler = itemArr.join('');
                        var tempArr = itemRuler.split('=');
                        var leftArr = getLeftFormular(itemArr, isLeft);//获取等号左边的等式
                        var rightRuler = itemArr[itemArr.length - 1];//等式右边的字段名
                        callback && callback(itemArr, tempArr, itemRuler, leftArr, rightRuler);
                        //var $rightRdom = $this.find('input[data-name="' + rightRuler.toLowerCase() + '"][data-isrelated="False"]');
                        //console.log('$rightRdom', $rightRdom.length);
                        //if ($rightRdom.length > 0) {
                        //    $rightRdom.prop('readonly', true);
                        //}

                        //$.each(leftArr, function (ii, nn) {
                        //    if (!checkFormularRuler(nn)) {
                        //        if (theme == 'jqgrid') {
                        //        } else {
                        //            var input = $this.find('input[data-name="' + nn.toLowerCase() + '"][data-isrelated="False"]');
                        //            input.each(function () {
                        //                var that = $(this);
                        //                var context = that.parents('tr:first');
                        //                that.on('change', function () {//绑定字段方法
                        //                    console.log('change')
                        //                    getFormularResult(leftArr, tempArr[0], rightRuler, context);//处理等式
                        //                    console.log(parTr, leftArr, tempArr[0], rightRuler, context)
                        //                    var parTr = $(this).parents('tr');
                        //                    var itemRightDom = parTr.find('input[data-name="' + rightRuler.toLowerCase() + '"][data-isrelated="False"]');
                        //                    if (itemRightDom.length > 0) {
                        //                        itemRightDom.trigger('change');
                        //                    }
                        //                });
                        //                context.find('button[name=editRowBtn]').unbind('gridview.editRow').bind('gridview.editRow', function () {
                        //                    setSubGridFormular($this)
                        //                });
                        //                context.find('button[name=saveRowBtn]').unbind('gridview.saveRow').bind('gridview.saveRow', function () {
                        //                    setSubGridFormular($this)
                        //                });
                        //                context.find('button[name=cancelRowBtn]').unbind('gridview.cancelRow').bind('gridview.cancelRow', function () {
                        //                    setSubGridFormular($this)
                        //                });
                        //            });

                        //        }
                        //    }
                        //});
                    } else {//关联字段
                        //var input = $('input.lookup[data-name="' + item.Name.toLowerCase() + '"][data-isrelated="False"]');
                        //if (input.length == 0) return false;
                        //console.log('item.Expression', JSON.parse(item.Expression));
                        //var itemArr = JSON.parse(item.Expression);
                        //if (input.length > 0) {
                        //    input.on('change', function () {
                        //        var inputid = $(this).attr('id');
                        //        var hiddenDom = $('#' + inputid.replace('_text', ''));
                        //        var changeEntityid = $(this).attr("data-lookup");;
                        //        var changeValue = hiddenDom.val();
                        //        var parentDom = $(this).parents('tr:first');
                        //        var that = $(this);
                        //        //console.log('hiddenDom',hiddenDom)
                        //        setlabelsToTarget(changeEntityid, changeValue, function (data) {
                        //            $.each(itemArr, function (ii, nn) {
                        //                if (!nn) return true;
                        //                var ntemp = nn.split('§');
                        //                var tarv = ntemp[0];
                        //                var sourv = ntemp[1];
                        //                var $context = $('input[data-name="' + tarv.toLowerCase() + '"][data-isrelated="True"]', parentDom);
                        //                $context.val('');
                        //                var list = data.content;

                        //                var type = sourv.toLowerCase();
                        //                var controltype = $context.attr('data-type') || "nvarchar";
                        //                //console.log($context);
                        //                // console.log(type)
                        //                //console.log("type"+controltype,type);
                        //                // console.log("setexts"+controltype,list);
                        //                if (!list || !list[type]) return true;
                        //                //html.push('<span class="label-tag" data-id="'+list.id+'">'+list[type]+'</span>');
                        //                if ($context.is(":disabled")) { return true; }

                        //                if (controltype == "lookup" || controltype == "owner" || controltype == "customer") {
                        //                    // console.log("lookup:",list);
                        //                    $context = $('input.lookup[data-name="' + nn.toLowerCase() + '"][data-isrelated="True"]', parentDom);
                        //                    $contextid = $context.attr('id').replace('_text', '');
                        //                    var $hidden = $('#' + $contextid, parentDom);
                        //                    $context.val(list[type + "name"]);
                        //                    $context.attr("data-id", list[type]);
                        //                    $hidden.val(list[type + "name"]);
                        //                    $hidden.attr("data-id", list[type]);
                        //                } else if (controltype == "state") {
                        //                    $context.parent().find("input[type='radio']").prop("checked", false);
                        //                    $context.parent().find("input[type='radio'][value='" + list[type] + "']").prop("checked", true);
                        //                    $context.val(list[type])
                        //                } else if (controltype == "picklist") {
                        //                    $context.siblings("select>option[value='" + list[type] + "']").prop("selected", true);
                        //                    $context.val(list[type])
                        //                } else {
                        //                    $context.val(list[type]);
                        //                }

                        //                $context.trigger('change');
                        //            })
                        //        });
                        //    });
                        //}
                    }
                });
            }
        }

        function getAndBindFormularInfo(grid, theme) {
            var $this = $(grid).parents('.subgrid:first');
            var ruler = $this.attr("data-formular");
            if (!ruler || ruler == "") { return false; }
            // console.log(decodeURIComponent(ruler));
            var rulerObj = JSON.parse(decodeURIComponent(ruler).toLowerCase());
            var formularlist = rulerObj;//getSubGridEntityList(rulerObj);//获取值计算的列表
            return formularlist;
        }
        window.gridFormular = {
            getFormularResult: getFormularResult,
            formularInit: formularInit,
            setFormularAndRelation: setFormularAndRelation
        }
        //window.formularInit = formularInit;
    })();