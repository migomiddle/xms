//@ sourceURL=cdatagrid.js
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
          //  console.log(context);
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

    function setFormularAndRelation(formularlist, type, callback, event, ui) {
        var dataIndx = ui.dataIndx, rowData = ui.rowData, rowIndx = ui.rowIndx, newVal = ui.newVal, $editor = ui.$editor;
        if (formularlist.length > 0) {
            $.each(formularlist, function (key, item) {
                if (type =='formular' && item.type == 'formular') {
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
                    
                } else if (type == 'append' && item.type == 'append'){//关联字段
                    //$('input.lookup[data-name="grid_edit_' + item.Name.toLowerCase() + '_text"][data-isrelated="False"]');
                    var input = $('input[name="grid_edit_' + item.name + '_text"]');
                    if (input.length == 0) return false;
                    console.log('item.Expression', JSON.parse(item.expression));
                    var itemArr = JSON.parse(item.expression);
                    if (input.length > 0) {
                        input.on('change', function () {
                            var inputid = $(this).attr('id');
                            var hiddenDom = $('#' + inputid.replace('_text', ''));
                            var changeEntityid = $(this).attr("data-lookup");;
                            var changeValue = hiddenDom.val();
                            var parentDom = $(this).parents('tr:first');
                            var that = $(this);
                            //console.log('hiddenDom',hiddenDom)
                            setlabelsToTarget(changeEntityid, changeValue, function (data) {
                                if (!data) return false;
                                $.each(itemArr, function (ii, nn) {
                                    if (!nn) return true;
                                    var ntemp = nn.split('§');
                                    var tarv = ntemp[0];
                                    var sourv = ntemp[1];
                                 //   var $context = $('input[data-name="' + tarv.toLowerCase() + '"][data-isrelated="True"]', parentDom);
                                  //  $context.val('');
                                    data = data.content;

                                    var field = sourv.toLowerCase();
                                  //  var controltype = $context.attr('data-type') || "nvarchar";
                                    //console.log($context);
                                    // console.log(type)
                                    //console.log("type"+controltype,type);
                                    // console.log("setexts"+controltype,list);
                                    if (!data || !data[field]) return true;
                                  //  var data = response.content;
                                    // console.log('response', data);
                                    // for (var i in rowData) {
                                      //  if (rowData.hasOwnProperty(i)) {
                                        //    var objitem = rowData[i];
                                            var colModel = ui.column;
                                           // var ifield = dataIndx.replace(/name$/, '');
                                           
                                            var reattrnamedata = data[field];
                                                //  var col = utils.getParamsByArray(colModel, 'dataIndx', i);
                                                //  if (col.length > 0) {
                                                var type = colModel.edittype;
                                                if ((reattrnamedata !== '' && reattrnamedata !== null && reattrnamedata !== undefined)) {
                                                    // console.log(attrname,type,eval('data.' + attrname));
                                                    if (type == 'nvarchar' || type == 'money' || type == 'int') {
                                                        rowData[field] = reattrnamedata;
                                                        // $td.find('input[name=' + GridViewModel.nameprefix + ifield + ']').val(data[attrname]);
                                                    }
                                                    else if (type == 'owner' || type == 'lookup' || type == 'customer' || type == 'picklist' || type == 'state' || type == 'bit' || type == 'status') {
                                                        //var _field = i.replace(/name$/, 'id');
                                                        rowData[field] = reattrnamedata;
                                                        rowData[field + 'name'] = data[field + 'name'];
                                                        // rowData[i] = data[dataIndx];
                                                        // rowData[_field] = data[_field];
                                                    } else {
                                                        rowData[field] = reattrnamedata;
                                                        //rowData[ifield] = data[ifield];
                                                    }
                                                    $(event.target).pqGrid("refresh");
                                                    //grid.pqGrid("refreshRow", { rowIndx: rowIndx });
                                            //    }
                                                // }
                                            
                                      //  }
                                    }
                                    //html.push('<span class="label-tag" data-id="'+list.id+'">'+list[type]+'</span>');
                                    //if ($context.is(":disabled")) { return true; }

                                    //if (controltype == "lookup" || controltype == "owner" || controltype == "customer") {
                                    //    // console.log("lookup:",list);
                                    //    $context = $('input.lookup[data-name="' + nn.toLowerCase() + '"][data-isrelated="True"]', parentDom);
                                    //    $contextid = $context.attr('id').replace('_text', '');
                                    //    var $hidden = $('#' + $contextid, parentDom);
                                    //    $context.val(list[type + "name"]);
                                    //    $context.attr("data-id", list[type]);
                                    //    $hidden.val(list[type + "name"]);
                                    //    $hidden.attr("data-id", list[type]);
                                    //} else if (controltype == "state") {
                                    //    $context.parent().find("input[type='radio']").prop("checked", false);
                                    //    $context.parent().find("input[type='radio'][value='" + list[type] + "']").prop("checked", true);
                                    //    $context.val(list[type])
                                    //} else if (controltype == "picklist") {
                                    //    $context.siblings("select>option[value='" + list[type] + "']").prop("selected", true);
                                    //    $context.val(list[type])
                                    //} else {
                                    //    $context.val(list[type]);
                                    //}

                                    //$context.trigger('change');
                                })
                            });
                        });
                    }
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



; (function () {
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
    var urlConfig = {
        getData: function (enittyid, queryid, pagesize, page) {
            return 'rendergridview?entityid=' + enittyid + '&QueryViewId' + queryid + '&onlydata=true&pagesize=' + pagesize + '&page=' + page
        }
    }

    function setTableFilter($grid) {
        if (typeof XmsFilter === 'undefined') return false;
        $grid.find('.datatable-filter-list').each(function (i, n) {
            var self = $(n);
            var $td = self.parents('td:first');
            var colInfo = $grid.pqGrid('getHeaderColumnFromTD', $td);
            var colIndex = colInfo.column
            //  console.log('colindex', colInfo);

            var dataType = colIndex.attributetypename;
            //if (colIndex.dataType == 'string') {
            //    dataType = 'nvarchar';
            //  }
            var name = colIndex.name;
            var width = colIndex.width;
            var label = colIndex.title;
            var referecingentityid = colIndex.referencedentityid;
            var dataText = self.data().dataText;
            if (!colIndex.notHeaderFilter) {
                var controls = new Array();

                controls.push('<div  class="datatable-filter-wrapBox" title="' + label + '">');
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
                controls.push('<ul class="dropdown-menu" style="width:' + (width || 100) + 'px;">');
                controls.push('<li class="dropdown-header">' + LOC_FILTER + '</li>');
                controls.push('<li><a href="javascript:void(0)" class="disabled filter-disabled"><span class="glyphicon glyphicon-remove-sign"></span> ' + LOC_RESET + '</a></li>');
                controls.push('<li><a href="javascript:void(0)" data-operator="notnull" onclick="pageFilter.filterColumnNull(\'' + name + '\', false)"><span class="glyphicon glyphicon-ok-circle"></span> ' + LOC_FILTER_NOTNULL + '</a></li>');
                controls.push('<li><a href="javascript:void(0)" data-operator="null" onclick="pageFilter.filterColumnNull(\'' + name + '\', true)"><span class="glyphicon glyphicon-ban-circle"></span> ' + LOC_FILTER_NULL + '</a></li>');
                controls.push('<li><a href="javascript:void(0)" data-referencedentityid="' + referecingentityid + '" onclick="pageFilter.customizeFilter(\'' + name + '\', \'' + dataType + '\',this)"><span class="glyphicon glyphicon-pencil"></span> ' + LOC_FILTER_CUSTOMIZE + '</a></li>');
                controls.push('</ul>');
                controls.push('</div>');
                controls.push('</div>');

                self.html(controls.join('\n'));
            }
        });

    }



    var bottomsInfo = [
        {
            type: "button",
            label: '保存',
            cls: 'btn btn-primary',
            icon: 'ui-icon-disk',
            listeners: [{
                click: function (a, _super, grid) {
                   // console.log('listeners', a, grid.pqGrid("getRowData"));
                 //   var columns = grid.pqGrid("getColModel");
                 //   var len = grid.find('.pq-grid-table tr').length - 1;
                 //   var primarykey = '';
                 //   var datas = [];
                 //   for (var i = 0; i < len; i++) {
                 //       var rowdata = grid.pqGrid("getRowData", { rowIndxPage: i });
                 //       if (rowdata) {
                 //           datas.push(rowdata);
                 //       }
                 //   }

                 //   datas = $.extend(true, [], datas);
                 ////   console.log(datas);
                 //   var res = [];
                 //   $.each(datas, function (key, item) {
                 //       if (item.isEdited) {
                 //           var obj = { entityid: GridViewModel.entityid, data: {} };
                 //           obj.data = item;
                 //           obj.name = GridViewModel.entityname;
                 //           obj.relationshipname = GridViewModel.relationshipname;
                 //           // obj.data.primarykey = grid.opts.primarykey;
                 //         //  console.log(GridViewModel.entityname);
                 //           obj.data[GridViewModel.referencingattributename] = GridViewModel.referencedrecordid;
                 //           obj.data.primarykey = getPrimaryKey(grid)();
                 //           obj.data.id = item[obj.data.primarykey];
                 //           res.push(obj);
                 //       }
                 //   });
                 // //  console.log(res);
                 //   var postData = {
                 //       child: encodeURIComponent(JSON.stringify(res)),
                 //       entityname: GridViewModel.entityname,
                 //       parentid: Xms.Page.PageContext.RecordId
                 //   }
                 //   purecms.post('/admin/dataservice/savechilds', postData, function () {

                 //   }, function (response) {
                 //       //$(document).trigger('subgrid.save', { data: response });
                 //       //GridViewModel.rebind(null, function () {
                 //       //    console.log($(GridViewModel.sectionid).parents('.subgrid:first'))
                 //       //    if (typeof setSubGridFormular == 'function') {
                 //       //        setSubGridFormular($(GridViewModel.sectionid).parents('.subgrid:first'), 'gridview');
                 //       //    }
                 //       //});
                 //   //    console.log(response);
                 //   });
                    // console.log(grid.pqGrid( "getData"));
                }
            }]
        },
        {
            type: "button",
            label: '新增行',
            cls: 'btn',
            icon: 'ui-icon-plus',
            listeners: [{
                click: function (a, _super, grid) {
                    //var primarykey = getPrimaryKey(grid)();
                    //var rowdata = { rowData: {} };
                    //rowdata.rowData[primarykey] = Xms.Utility.Guid.NewGuid().ToString('N');
                    //grid.pqGrid('addRow', rowdata);
                    // console.log(grid.pqGrid( "getData"));
                }
            }]
        },
        {
            type: "button",
            label: '删除',
            cls: 'btn',
            icon: 'ui-icon-plus',
            listeners: [{
                click: function (a, _super, grid) {

                    // console.log(grid.pqGrid( "getData"));
                }
            }]
        }
    ];
    var defaults = {
        entityid: '',
        queryid: '',
        headerFilter: false,
        height: '300',
        width: "auto",
       // scrollModel: { autoFit: true },
        showTop: false,
        showBottom: true,
        //selectionModel: { type: 'row', mode: 'single' },
        title: '',
        //collapsible: false,
        rowHt: 30,
        autoRow: false,
        wrap: false,
        hwrap: false,
       // resizable: true,
        columnBorders: false,
        trackModel: { on: true }, //to turn on the track changes. 
        pageModel: { type: "remote", rPP: 20, page: 1, strRpp: "{0}" },
        showHeader: true,
        roundCorners: true,
        rowBorders: true,
        columnBorders: true,
        selectionModel: { type: 'row' },
        numberCell: { show: false },
        beforeTableView:null,
        //theme: true,
        // filterModel:{ on: false, mode: "OR" },
        postRenderInterval: -1, //synchronous post rendering.
        refreshDataAndView:null,
        //toolbar: {
        //    items: bottomsInfo
        //},
        loading: false,
        rowClick: function (event, ui) {
            var highline1 = 'pg-grid-cell-highlight';
            var highline = 'ui-state-highlight';
            var checkedval = $(event.currentTarget).find('input[type="checkbox"]').val();//未被冻结的列
            var fozeninput = $(event.target).find('input[value="' + checkedval + '"]:first');
            var hiddeninput = $(event.target).find('input[value="' + checkedval + '"]:last');
            var curInput = $(event.toElement).closest('input[type="checkbox"]');
            if (curInput.length > 0) {
                //var checked = curInput.prop('checked');
                //if (checked) {
                //    curInput.prop('checked', false);
                //    hiddeninput.parents('tr:first').removeClass(highline);
                //    fozeninput.parents('tr:first').removeClass(highline);
                //} else {
                //    curInput.prop('checked', true);
                //    hiddeninput.parents('tr:first').addClass(highline);
                //    fozeninput.parents('tr:first').addClass(highline);
                //}
            } else {
                fozeninput.trigger('click')
            }
        },
        addCheckbox: true,
        beforeCheck: function (evt, ui) {
            // console.log(evt.type, ui);
        },
        check: function (event, ui) {
         //   console.log(event, ui);
            event.stopPropagation();
            var source = ui.source;
            var highline = 'ui-state-highlight';
            var $input = $(event.toElement);
            var $grid = $(event.target).find('.pq-grid-cont-outer');
            var source = ui.source;
            if (source == 'header') {

                $grid.find('tbody:first>tr').find('td:eq(0)').find('input').prop('checked', true);
                $grid.find('tbody>tr').addClass(highline);

                $('body').trigger('datatable.headerCheckboxCheck', { event: event, ui: ui })
            } else {
                var highline1 = 'pg-grid-cell-highlight';
                var highline = 'ui-state-highlight';
                var checkedval = $(event.toElement).val();//未被冻结的列
                var fozeninput = $(event.target).find('input[value="' + checkedval + '"]:first');
                var hiddeninput = $(event.target).find('input[value="' + checkedval + '"]:last');
                var curInput = $(event.toElement).closest('input[type="checkbox"]');

                var checked = curInput.prop('checked');
                function check() {
                   
                    //  if ($(event.toElement).attr('name') != 'recordid') {
                    fozeninput.prop('checked', true);
                    //  }
                    hiddeninput.parents('tr:first').addClass(highline);
                    fozeninput.parents('tr:first').addClass(highline);
                    $('body').trigger('queryview.rowClick', { type: true, row: fozeninput.parents('tr:first'), recordid: checkedval })
                }
                check()
                //function uncheck() {
                //    $(event.currentTarget).removeClass(highline);
                //    $(event.currentTarget).removeClass(highline1);
                //    //   if ($(event.toElement).attr('name') != 'recordid') {
                //    fozeninput.prop('checked', false);
                //    //  }
                //    fozeninput.parents('tr:first').removeClass(highline);
                //    hiddeninput.parents('tr:first').removeClass(highline);
                //    fozeninput.parents('tr:first').removeClass(highline1);
                //    hiddeninput.parents('tr:first').removeClass(highline1);
                //    $('body').trigger('queryview.rowClick', { type: false, row: fozeninput.parents('tr:first'), recordid: checkedval })
                //}

               
            }
           
        },
        beforeunCheck: function (event, ui) {
                   // console.log(evt.type, ui);
                },
        unCheck: function (event, ui) {
            event.stopPropagation();
            //console.log(evt.type, ui);
            var source = ui.source;
            var highline = 'ui-state-highlight';
            var $input = $(event.toElement);
            var $grid = $(event.target).find('.pq-grid-cont-outer');
            var source = ui.source;
            if (source == 'header') {
                $grid.find('tbody>tr').find('td:eq(0)').find('input').prop('checked', false);
                $grid.find('tbody>tr').removeClass(highline);
                $('body').trigger('datatable.headerCheckboxUnCheck', { event: event, ui: ui })
            } else {
                var highline = 'ui-state-highlight';
                var checkedval = $(event.toElement).val();//未被冻结的列
                var fozeninput = $(event.target).find('input[value="' + checkedval + '"]:first');
                var hiddeninput = $(event.target).find('input[value="' + checkedval + '"]:last');
                var curInput = $(event.toElement).closest('input[type="checkbox"]');

                var checked = curInput.prop('checked');
                function uncheck() {
                   
                    //   if ($(event.toElement).attr('name') != 'recordid') {
                    fozeninput.prop('checked', false);
                    //  }
                    fozeninput.parents('tr:first').removeClass(highline);
                    hiddeninput.parents('tr:first').removeClass(highline);
                    $('body').trigger('queryview.rowClick', { type: false, row: fozeninput.parents('tr:first'), recordid: checkedval })
                }
                uncheck()
            }
            
        }
    }
    defaults.initAfter = function ($grid) {

        $grid.$plugGrid.pqGrid("option", "freezeCols", 1);
        $grid.$plugGrid.pqGrid("refresh");

    }
    //defaults.selectChange = function (evt, ui) {
    //    console.log('selectChange', ui);
    //    var selected = [],
    //        rows = ui.rows;
    //    if (rows && rows.length) {
    //        for (var i = 0; i < rows.length; i++) {
    //            selected.push(rows[i].rowData.id);
    //        }
    //    }
    //   // $("#select_row_display_div").html("rowId: " + selected);
    //}

    defaults.dataModel = {
        location: "remote",
        sorting: "remote",
        dataType: "JSON",
        method: "POST",
        //  sortIndx: ["name", "createtime"],
        //  sortDir: ["0", "1"],
        filterSendData: function (postData, objP) {
           // $.extend(postData, { filter: gridview_filters.getFilterInfo(), sortby: objP.dataIndx, sortdirection: objP.dir == 'up' ? '0' : '1' });
            $.extend(postData, {  sortby: objP.dataIndx, sortdirection: objP.dir == 'up' ? '0' : '1' });
            return postData;
        },
        contentType: 'application/json; charset=utf-8',
        getUrl: function (opts) {
           // console.log('opts', opts);

            var _url = urlConfig.getData(obj);
            if (opts) {
                var pagemodel = opts.pageModel;
                var curpage = pagemodel.curPage;
                _url = $.setUrlParam(_url, 'page', curpage);
            }

            return { url: _url };
        },
        postData: { pagesize: 20, page: 1 },
        //url: "/pro/products.php",//for PHP
        getData: function (dataJSON, textStatus, jqXHR) {
            var data = dataJSON.items;
            var res = { curPage: dataJSON.currentpage || 1, totalRecords: dataJSON.totalitems, data: data }
            console.log(dataJSON);
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

        //purecms.del(rowData[getPrimaryKey(grid)()], '/admin/entity/delete?entityid=' + GridViewModel.entityid, false, function () {
        //    $grid.pqGrid("deleteRow", { rowIndx: rowIndx });
        //    grid.pqGrid("refresh");
        //});
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
                    if (n.entityname) {
                        primarykey = n.dataIndx;
                        return false;
                    }
                });
                return primarykey.toLowerCase();
            } else {
                return primarykey.toLowerCase();
            }

        }
    }

    function getDataGridConfig(datagridinfo) {
        datagridinfo.colModel = filterColumns(datagridinfo.colModel);

        return datagridinfo;
    }

    function filterColumns(columnconfigs) {
        var res = [];
        //  console.log('columnconfigs,', columnconfigs)
        $.each(columnconfigs, function (key, item) {
            if (item) {
                var obj = getColumnInfo(item, columnconfigs);
                res.push(obj);
            }
        });
        return res
    }
    function queryListByKey(arr, key, value) {
        var res = [];
        res = $.grep(arr, function (n, i) {
            if (n[key] == value) {
                return true;
            }
            return false;
        });
        return res;
    }
    function getPrimaryKeyByColumns(columninfos) {
        var res = null,
            that = this,
            options = this.options;
        $.each(columninfos, function (key, item) {
            if (item.attributetypename == "primarykey") {
                res = item;
                return false;
            }
        });
        return res;
    }
    function getColumnEntityName(columninfos) {
        var res = null,
            that = this,
            options = this.options;
        $.each(columninfos, function (key, item) {
            if (!~item.name.indexOf('.')) {
                res = item.entityname;
                return false;
            }
        });
        return res;
    }
    //处理不同数据类型的
    function getColumnInfo(item, columninfos) {
        var edittype = 'string';
        var obj = {
            dataIndx: item.name,
            title: item.localizedname,
            dataType: 'string',
            //  editable:item.editable,
            width: item.width || 100,
        }
        $.extend(obj, item);
        if (item.edittype && item.edittype != "") {
            //item.editable = true;
        }
        if (item && (item.attributetypename == 'money' || item.attributetypename == 'float')) {
            obj.align = 'right';
            obj.edittype = "money";
            obj.render = function (ui) {
                var datas = ui.rowData;
                var dataIndx = ui.dataIndx;
                var column = ui.column;
                var record = datas[dataIndx];
                if (item && item.precision != "" && !isNaN(item.precision) && record != "" && !isNaN(record)) {
                    record = (record*1).toFixed(item.precision)
                }
                return record;
            }
            setEditorInfo(obj, item);
        }
        if (item && item.attributetypename == "nvarchar") {
            edittype = "string";
            isprimaryfield = item.isprimaryfield;
            if (isprimaryfield == true) {
                obj.render = function (ui, a, b) {
                    var datas = ui.rowData;
                    var dataIndx = ui.dataIndx;
                    var column = ui.column;
                    var entityid = column.entityid;
                    var formid = column.formid ? column.formid : '';
                    var _entityname = getColumnEntityName(columninfos);
                    if (_entityname && dataIndx.indexOf('.')==-1) {
                        _entityname = _entityname.toLowerCase();
                        var recordid = datas[_entityname+'id'];

                        //console.log(ui, a, b);
                        if (datas[dataIndx]) {
                            if (typeof entityIframe=='function') {
                                return '<a class="text-primary" title="' + (datas['name'] || '')+'" href="javascript: entityIframe(\'show\', \'' + ORG_SERVERURL + '/entity/edit?entityid=' + entityid + '&formid' + formid + '=&recordid=' + recordid + '\');"  >' + (datas['name'] || '') + '</a>';
                            } else {
                                return '<a class="text-primary" title="' + (datas['name'] || '') +'" href="' + ORG_SERVERURL + '/entity/edit?entityid=' + entityid + '&formid' + formid + '=&recordid=' + recordid + '" target="_blank" >' + (datas['name'] || '') + '</a>';
                            }
                        } else {
                            return '';
                        }
                    } else {
                        return '<span title="' + datas[dataIndx] + '">' + datas[dataIndx]+'<span>'
                    }
                }
            }
        } else if (item && (item.attributetypename == "picklist"  || item.attributetypename == "status")) {
            //console.log("optionset-type", obj.metadata&&obj.metadata.attributetypename);
            edittype = 'picklist';
            obj.editModel = { saveKey: item.name.replace(/name$/, 'id'), keyUpDown: false }
            obj.edittype = edittype;
            obj.render = function (ui, a, b) {
                var datas = ui.rowData;
                var dataIndx = ui.dataIndx;
                var column = ui.column;
                var entityid = column.referencedentityid;
                var formid = column.formid ? column.formid : '';
                // var keyname = getPrimaryKey()
                var recordid = datas[dataIndx];
                if (item && item.optionset) {
                    var res = queryListByKey(item.optionset.items, 'value', recordid);
                    //  console.log(ui, a, b);
                    if (res.length > 0) {
                        return res[0].name;
                    } else {
                        return '';
                    }
                } else {
                    return '';
                }

            }
            setEditorInfo(obj, item);
        } else if (item && (item.attributetypename == "state" || item.attributetypename =="bit")) {
            edittype = 'picklist';
            obj.editModel = { saveKey: item.name.replace(/name$/, 'id'), keyUpDown: false }
            obj.edittype = edittype;
            obj.render = function (ui, a, b) {
                var datas = ui.rowData;
                var dataIndx = ui.dataIndx;
                var column = ui.column;
                var entityid = column.referencedentityid;
                var formid = column.formid ? column.formid : '';
                // var keyname = getPrimaryKey()
                var recordid = datas[dataIndx];
                if (item && item.picklists) {
                    var res = queryListByKey(item.picklists, 'value', recordid);
                    //  console.log(ui, a, b);
                    if (dataIndx != 'statecode') {
                        if (res.length > 0) {
                            return res[0].name;
                        } else {
                            return '';
                        }
                    } else {
                        if (recordid == 1) {
                            return '<span class="label label-success">启用</span>'
                        } else {
                            return '<span class="label label-default">禁用</span>'
                        }
                    }
                   
                } else {
                    return '';
                }

            }
            setEditorInfo(obj, item);
        } else if (item && (item.attributetypename == "lookup" || item.attributetypename == "owner" || item.attributetypename == "customer")) {
            edittype = 'lookup'
            obj.edittype = edittype;
            if (item.displaystyle == 'link') {
                obj.render = function (ui, a, b) {
                    var datas = ui.rowData;
                    var dataIndx = ui.dataIndx;
                    var column = ui.column;
                    var entityid = column.referencedentityid;
                    var formid = column.formid ? column.formid : '';
                    var recordid = datas[dataIndx];
                    //console.log(ui, a, b);
                    if (datas[dataIndx]) {
                        return '<a class="text-primary" title="' + (datas[dataIndx + 'name'] || '') +'" href="' + ORG_SERVERURL + '/entity/edit?entityid=' + entityid + '&formid' + formid + '=&recordid=' + recordid + '" target="_blank" >' + (datas[dataIndx+'name'] || '') + '</a>';
                    } else {
                        return '';
                    }
                }
            } else {
                obj.render = function (ui, a, b) {
                    var datas = ui.rowData;
                    var dataIndx = ui.dataIndx;
                    var column = ui.column;
                    var entityid = column.referencedentityid;
                    var formid = column.formid ? column.formid : '';
                    var recordid = datas[dataIndx];
                    //console.log(ui, a, b);
                    if (datas[dataIndx]) {
                        return '<span title="' + (datas[dataIndx + 'name'] || '') +'">'+(datas[dataIndx + 'name'] || '')+'</span>';
                    } else {
                        return '';
                    }
                }
            }
            setEditorInfo(obj, item);
        } else if (item && (item.attributetypename == "datetime" || item.attributetypename == "CreatedOn")) {
            edittype = 'datetime'
            obj.edittype = edittype;
            setEditorInfo(obj, item);
        } else if (item && item.attributetypename == "primarykey") {
            edittype = 'primarykey'
            obj.edittype = edittype;
            obj.render = function (ui, a, b) {
                var datas = ui.rowData;
                var dataIndx = ui.dataIndx;
                var column = ui.column;
                var entityid = column.entityid;
                var formid = column.formid ? column.formid : '';
                var recordid = datas[dataIndx];
                //console.log(ui, a, b);
                if (datas[dataIndx]) {
                    return '<a class="text-primary" title="' + (datas['name'] || '') +'" href="' + ORG_SERVERURL + '/entity/edit?entityid=' + entityid + '&formid' + formid + '=&recordid=' + recordid + '" target="_blank" >' + (datas['name']||'') + '</a>';
                } else {
                    return '';
                }
            }
            setEditorInfo(obj, item);
        }
        //如果为关联字段，则不可编辑
        if (obj && obj.dataIndx && ~obj.dataIndx.indexOf('.')) {
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
        if (item.editable == false) { return false;}
        var isrelate = obj.name.indexOf('.') != -1;
        if (obj.editable && obj.edittype == 'picklist') {
            if (item.isrequired == true && !isrelate) {
                obj.validations = [
                    //validation    
                    { type: 'minLen', value: '1', msg: '这是必填项' }
                ]
            }
            if (item.attributetypename == 'state') {
                var options = $.map(item.picklists, function (n, i) {
                    if (item) {
                        return { text: n.name, value: n.value };
                    }
                });
            } else {
                var options = $.map(item.optionset.items, function (n, i) {
                    if (item) {
                        return { text: n.name, value: n.value };
                    }
                });
            }
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

                        var $inp = $("<input type='hidden' name='grid_edit_" + dataIndx + "' class='" + cls + " pq-ac-editor' />")
                            .width(width - 6)
                            .appendTo($cell)
                            .val(dc);
                        $inp.picklist({
                            required: $inp.is('.required'),
                            items: item.optionset.items,
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
                    var rowData = ui.that.getRowData({ rowIndx: ui.rowIndx });
                    rowData[item.name.replace(/name$/, 'id')] = clave;
                    rowData[item.name + 'name'] = ui.$cell.find("select option[value='" + clave + "']").text();
                    // console.log(rowData)
                    //grid.pqGrid("refreshRow");
                    var text = ui.$cell.find("select option[value='" + clave + "']").text();
                    return clave
                }
            }
        } else if (obj.edittype == 'lookup') {
            if (item.isrequired == true && !isrelate) {
                obj.validations = [
                    //validation    
                    { type: 'minLen', value: '1', msg: '这是必填项' }
                ]
            }
            obj.editor = {
                type: //'select'
                    function (ui, even) {
                        //   console.log(ui);
                        //debugger;
                        var $cell = ui.$cell,
                            rowData = ui.rowData,
                            dataIndx = ui.dataIndx,
                            rowIndx = ui.rowIndx,
                            column = ui.column,
                            width = ui.column.width,
                            grid = ui.that.$grid_inner,
                            cls = ui.cls;
                        var dc = $.trim(rowData[dataIndx.replace(/name$/, '')]);
                        var dctext = $.trim(rowData[dataIndx + 'name']);
                        var fieldname = dataIndx.replace(/\./, '_');
                        var $in = $("<input type='hidden' name='grid_edit_" + fieldname + "' id='grid_edit_" + fieldname + "_hidden' class='" + cls + " pq-ac-editor' />")
                            .width(width - 6)
                            .appendTo($cell)
                            .val(dc);
                        $in.attr('data-value', dc);
                        $in.attr('data-text', dctext);
                        var field = dataIndx.replace(/name$/, 'id');

                        //if (!editor.props.$inputext) {

                        var $input = $('<input type="text" class="form-control input-sm" id="grid_edit_' + fieldname + '_text" name="grid_edit_' + fieldname + '_text" class="" />');
                        var $value = $('<input type="hidden" class="form-control " id="grid_edit_' + fieldname + '" name="grid_edit_' + fieldname + '" class=""  />');
                        // }
                        $input.appendTo($cell);
                        $value.appendTo($cell);

                        var self = $input;

                        if (!self.prop('id')) {
                            self.prop('id', self.prop('name') + Xms.Utility.Guid.NewGuid().ToString('N'));
                        }
                        //console.log('self',self)
                        self.attr('data-lookup', item.referencedentityid)
                        self.attr('data-reentityname', item.referencedentityname);
                        var lookupid = item.referencedentityid;
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

                        var lookupurl = ORG_SERVERURL+'/entity/RecordsDialog?inputid=' + inputid + '&sortby=CreatedOn&singlemode=true';
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
                                    Xms.Web.OpenDialog(lookupurl, 'dataGridselectRecordCallback', { filter: f });
                                } else {
                                    Xms.Web.OpenDialog(lookupurl, 'dataGridselectRecordCallback');
                                }
                            }
                            , clear: function () {
                                $('#' + inputid).val('');
                                $('#' + valueid).val('');
                                $('#' + valueid+'_hidden').val('');
                                rowData[item.name.replace(/name$/, '')] = '';
                                rowData[item.name + 'name'] = '';
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
                                    //  purecms.console('lookup', tar)

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
                        $('#' + inputid).focus();
                        $('#' + inputid).on('dialog.return', function (e, obj) {

                            // $(this).parents('tr:first').attr('data-edited', true);
                            // console.log(obj);
                            // $input.val(obj.id);
                            // $(this).val(obj.name);
                            //$box.html(obj.name);
                            $in.attr('data-value', obj.id);
                            $in.next().find('input:first').val(obj.name);
                            $.fn.xmsSelecteDown.setLookUpState($in.next().find('input:first'));
                            $in.attr('data-text', obj.name);
                            $('#' + valueid).val(obj.id)
                            $('#' + inputid).trigger('change');
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
                            var entityname = $(this).attr('data-reentityname');
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
                                // console.log('_isRelative', _isRelative);
                                if (_isRelative == true) {
                                   // Xms.Web.PageCache('renderGridView', '/admin/dataservice/RetriveReferencedRecord', params, function (response) {
                                    var url = '/api/data/retrieve/' + entityname + '/' + v;//'GetReferenced';
                                    Xms.Web.PageCache('renderGridView', url, params, function (response) {
                                        var data = response.content;
                                        // console.log('response', data);
                                        for (var i in rowData) {
                                            if (rowData.hasOwnProperty(i)) {
                                                var objitem = rowData[i];
                                                var colModel = column;
                                                var ifield = dataIndx.replace(/name$/, '');
                                                if (~i.indexOf(ifield) && ~i.indexOf('.')) {
                                                    var attrs = i.split('.');
                                                    var reattrnamedata = data[attrs[1]];
                                                  //  var col = utils.getParamsByArray(colModel, 'dataIndx', i);
                                                  //  if (col.length > 0) {
                                                    var type = colModel.edittype;
                                                    if ((reattrnamedata !== '' && reattrnamedata !== null && reattrnamedata !== undefined)) {
                                                            // console.log(attrname,type,eval('data.' + attrname));
                                                            if (type == 'nvarchar' || type == 'money' || type == 'int') {
                                                                rowData[i] = data[attrs[1]];
                                                                // $td.find('input[name=' + GridViewModel.nameprefix + ifield + ']').val(data[attrname]);
                                                            }
                                                            else if (type == 'owner' || type == 'lookup' || type == 'customer' || type == 'picklist' || type == 'state' || type == 'bit' || type == 'status') {
                                                                //var _field = i.replace(/name$/, 'id');
                                                                rowData[i] = data[attrs[1]];
                                                                rowData[i+'name'] = data[attrs[1]+'name'];
                                                                // rowData[i] = data[dataIndx];
                                                                // rowData[_field] = data[_field];
                                                            } else {
                                                                rowData[i] = data[attrs[1]];
                                                                //rowData[ifield] = data[ifield];
                                                        }
                                                        ui.that.refresh();
                                                            //grid.pqGrid("refreshRow", { rowIndx: rowIndx });
                                                        }
                                                   // }
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
                    var rowData = ui.that.getRowData({ rowIndx: ui.rowIndx });
                    if (clave) {
                        rowData[item.name.replace(/name$/, '')] = clave;
                        rowData[item.name + 'name'] = text;
                        //console.log(rowData)
                        //grid.pqGrid("refreshRow");

                    }
                    return clave;
                }
            }

        } else if (obj.edittype == 'money') {
            if (item.isrequired == true && !isrelate) {
                obj.validations = [
                    //validation    
                    { type: 'minLen', value: '1', msg: '这是必填项' }
                ]
            }
            obj.editor = {
                type: //'select'
                    function (ui) {
                        //debugger;
                        var $cell = ui.$cell,
                            rowData = ui.rowData,
                            dataIndx = ui.dataIndx,
                            width = ui.column.width,
                            cls = ui.cls;
                        var dc = rowData[dataIndx] || '';

                        var $inp = $("<input type='number' name='grid_edit_" + dataIndx + "' class='form-control " + cls + " pq-ac-editor' />")
                            .width(width - 6)
                            .appendTo($cell)
                            .val(dc).focus();
                    }
                ,
                options: options,
                labelIndx: 'text',
                valueIndx: 'value',

                getData: function (ui) {
                    var clave = ui.$cell.find("input").val();
                    return clave
                }
            }

        } else if (obj.edittype == 'datetime') {
            if (item.isrequired == true && !isrelate) {
                obj.validations = [
                    //validation    
                    { type: 'minLen', value: '1', msg: '这是必填项' }
                ]
            }
            obj.editor = {
                type: //'select'
                    function (ui) {
                        //debugger;
                        var $cell = ui.$cell,
                            rowData = ui.rowData,
                            dataIndx = ui.dataIndx,
                            width = ui.column.width,
                            cls = ui.cls;
                        var dc = rowData[dataIndx];

                        var $inp = $("<input type='text' name='grid_edit_" + dataIndx + "' class='form-control " + cls + " pq-ac-editor' />")
                            .width(width - 6)
                            .appendTo($cell)
                            .val(dc).focus();

                        var format = ui.column.format || 'yyyy-MM-dd HH:mm:ss';
                        var tempformat = format;
                        var dataname = dataIndx
                        var value = $(this).val();
                        if (value != '') {
                            $(this).val(new Date(value).format(format))
                        }
                        // Xms.Web.Console(dataname, format);
                        format = format.replace("yyyy", "Y").replace("dd", "d").replace("hh", "h").replace("mm", "i").replace('MM', "m").replace('ss', "s").replace('HH', "H").replace('h', "H");

                        if (tempformat.indexOf("hh:mm") > -1) {

                            $inp.datetimepicker({
                                language: "en"
                                , step: 15
                                , format: format
                                , scrollInput: !1
                                , scrollMonth: false
                            }).on('change', function (e, obj) {
                                //var curTr = $(this).parents('tr:first');
                                //curTr.attr('data-edited', true);
                                //if (obj && obj.noGoNext) {
                                //    triggerNextInputFocus($(this), true);
                                //} else {
                                //    triggerNextInputFocus($(this))
                                //}curTr.attr('data-edited', true);

                            })

                        } else {

                            $inp.datetimepicker({
                                language: "en"
                                , timepicker: false
                                , format: format
                                , scrollInput: !1
                                , scrollMonth: false
                            }).on('change', function (e, obj) {
                                //var curTr = $(this).parents('tr:first');
                                //curTr.attr('data-edited', true);
                                //if (obj && obj.noGoNext) {
                                //    triggerNextInputFocus($(this), true);
                                //} else {
                                //    triggerNextInputFocus($(this))
                                //}
                            })
                        }
                        $inp.focus();
                    }
                ,
                options: options,
                labelIndx: 'text',
                valueIndx: 'value',

                getData: function (ui) {
                    var clave = ui.$cell.find("input").val();
                    return clave
                }
            }

        } else if (obj.edittype == 'nvachart') {
            if (item.isrequired == true && !isrelate) {
                obj.validations = [
                    //validation    
                    { type: 'minLen', value: '1', msg: '这是必填项' }
                ]
            }
            obj.editor = {
                type: //'select'
                    function (ui) {
                        //debugger;
                        var $cell = ui.$cell,
                            rowData = ui.rowData,
                            dataIndx = ui.dataIndx,
                            width = ui.column.width,
                            cls = ui.cls;
                        var dc = rowData[dataIndx];

                        var $inp = $("<input type='text' name='grid_edit_" + dataIndx + "' class='form-control" + cls + " pq-ac-editor' />")
                            .width(width - 6)
                            .appendTo($cell)
                            .val(dc).focus();
                    }
                ,
                options: options,
                labelIndx: 'text',
                valueIndx: 'value',

                getData: function (ui) {
                    var clave = ui.$cell.find("input").val();
                    return clave
                }
            }

        } else if (obj.edittype == 'cdatagrid_editer') {
            obj.editor = {
                type: //'select'
                    function (ui) {
                        return false;
                    }
                ,
                options: options,
                labelIndx: 'text',
                valueIndx: 'value',

                getData: function (ui) {
                   // var clave = ui.$cell.find("input").val();
                   // return clave
                }
            }
        }

        window.formular_utils = utils
    }

    window.dataGridselectRecordCallback = function (res, input) {
       console.log(res,input)
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
    var __prefix = 'cdatagrid_';
    var __count = 0;
    function cDatagrid(obj, opts) {
        this.box = $(obj);
        this.opts = opts;
        this._id = __prefix + __count++;
        this.$grid = null;
        this.$plugGrid = null;
        this.id = "contextmenu" + this._id
        this.$rightClick = $('<div id="' + this.id+'" style="display:none;"></div>');
        $('body').append(this.$rightClick);
        this.init();
    }
    cDatagrid.prototype.createRightClick = function () {
        var contexts = ['<ul class="grid-contextmenu">']
        var contextMenu = this.opts.contextMenu;
        var self = this;
        contexts.push('<li class="grid-contextmenu-fieldname"></li>');
        contexts.push('<li class="grid-contextmenu-copy" id="grid_menu_copy' + this._id + '"><span class="glyphicon glyphicon-copy"></span> 复制单元格</li>');
       // contexts.push('<li class="grid-contextmenu-row" id="grid_menu_row' + this._id + '"><span class="glyphicon glyphicon-ok"></span> 选中行</li>');
       
        if (self.opts.itemsBtnTmpl) {
            var $items = $('<div></div>');
            $items.html(self.opts.itemsBtnTmpl);
            $items.find('li').each(function (i,n) {
                var $this = $(this).children('a');
                var icon = $this.children('span').attr('class');
                var text = $this.get(0).text;
                var event = $this.attr('onclick');
                contexts.push('<li class="grid-contextmenu-edit" onclick="'+event+'" id="grid_menu_edit_customer' + i + '"><span class="' + icon+'"></span> ' + text+'</li>');
            });
           
        }
        if (contextMenu && contextMenu.length>0) {
            $.each(contextMenu, function () {
                contexts.push('<li class="' + this.classname + '" id="' + self._id +this.id +'"><span class="'+(this.icon || '')+'"></span> '+this.name+'</li>');
            });
            
        }
        contexts.push('</ul>');
        this.$rightClick.html(contexts.join(''));
        var events = {
            onContextMenu: function (e) {
                e.stopPropagation();
                var text = self._getTdText(e);
                if (text == '&nbsp;') { text = '' };
                if ($(e.target).find('.datatable-itembtn').length > 0 || $(e.target).closest('.datatable-itembtn').length > 0 || $(e.target).find('input[name="recordid"]').length > 0 || $(e.target).closest('input[name="recordid"]').length > 0) return false;
                self.$rightClick.find('.grid-contextmenu-fieldname').html((text || ' - '));
                var $check = $(e.target).parents('tr:first').find('input[type="checkbox"]');
                if ($check.length > 0) {
                    var val = $check.val();
                    if ($(e.target).parents('.pq-grid-cont-outer:first').find('input[value="' + val + '"]').length > 0) {
                        if (!$(e.target).parents('.pq-grid-cont-outer:first').find('input[value="' + val + '"]:first').prop('checked')) {
                            $(e.target).parents('tr:first').trigger('click')
                        }

                    }
                   
                }
                if ($(e.target).parents('tr:first').find('.glyphicon-edit').length == 0) {
                    self.$rightClick.find('.grid-contextmenu-edit').hide();
                }
                var $tr = $(e.target).parents('tr:first')
                var $table = $tr.parents('.pq-grid-cont-inner');
                if ($table.prev('.pq-grid-cont-inner').length > 0) {
                    $table = $table.prev('.pq-grid-cont-inner');
                }
                $table.find('tr').each(function () {
                    if (!$(this).is(':hidden')) {
                        var checkbox = $(this).find('input[type="checkbox"]:first')
                        checkbox.prop('checked') && checkbox.trigger('click')
                    }
                });
                $tr.trigger('click');
                return true;
            },
            bindings: {
                //"menu_delConnection": function () {
                //    Xms.Web.Confirm("提示", '是否要删除该条件?', function () {
                //        p.trigger('connection.delete', { ele: p, _super: $('#' + sourceId).data().point, point: $('#' + sourceId).data().point, source: sourceId, target: targetId, connection: c });
                //        eleConnectionController.removeByName('elecon____' + sourceId + '____' + targetId);
                //        eleConnectionController.removeByName('elecon____' + targetId + '____' + sourceId);
                //        jsPlumb.detach(c);

                //        console.log(eleConnectionController)
                //    })

                //    // jsPlumb.detach(c);
                //}
            }
        }
        events.bindings["grid_menu_copy" + this._id] = function (e, b, c, that, relaevent) {
            console.log(e, b, c, that, relaevent);

            var text = '';
            if ($(relaevent.target).find('span').length > 0) {
                text = $(relaevent.target).find('span').text();
            } else if ($(relaevent.target).find('a').length > 0) {
                text = $(relaevent.target).find('a').text();
            } else {
                text = $(relaevent.target).text();
            }
            //  console.log(text);
            var $btn = $('<button class="hide" id="clipboard_' + self._id + '" data-clipboard-text="' + text + '">' + text + '</button>')
            $('body').append($btn);
            var clipboard = new ClipboardJS("#clipboard_" + self._id);
            clipboard.on('success', function (e) {
                // console.info('Action:', e.action);
                // console.info('Text:', e.text);
                // console.info('Trigger:', e.trigger);
            });

            clipboard.on('error', function (e) {
                // console.error('Action:', e.action);
                //console.error('Trigger:', e.trigger);
            });
            $btn.trigger('click');
            $btn.remove();
        }
        //events.bindings["grid_menu_row" + this._id] = function (e, b, c, that, relaevent) {
        //    var $tr = $(relaevent.target).parents('tr:first');
        //    $tr.trigger('click')
        //}
        //events.bindings["grid_menu_edit" + this._id] = function (e, b, c, that, relaevent) {
        //    var $tr = $(relaevent.target).parents('tr:first');
        //    $tr.trigger('dblclick')
        //}
        this.box.find('.pq-grid-cont').contextMenu(this.id, events);
    }
    cDatagrid.prototype.renderRightClick = function (contexts, grid) {
       
    }
    cDatagrid.prototype._getTdText = function (relaevent) {
        var text = '';
        if ($(relaevent.target).find('span').length > 0) {
            text = $(relaevent.target).find('span').text();
        } else if ($(relaevent.target).find('a').length > 0) {
            text = $(relaevent.target).find('a').text();
        } else {
            text = $(relaevent.target).text();
        }
        return text;
    }
    cDatagrid.prototype.isValid = function (index) {
        return this.$grid.pqGrid("isValid", { rowIndx: index });
    }
    
    cDatagrid.prototype.bindRightClickEvent = function () {
        var self = this;
        
    }
    cDatagrid.prototype.init = function () {
        var self = this;
        //初始化

        if (this.opts.extend) {
            this.opts.extend(this);
        }

        //设置获取数据配置
        this.opts.dataModel.getUrl = (function () {
            var _entityid = self.opts.entityid;
            var _queryid = self.opts.queryid;
            return function (opts) {
                var _url = self.opts.getDataUrl(self, opts);
                if (opts) {
                    var pagemodel = opts.pageModel;
                    var curpage = pagemodel.curPage;
                    _url = $.setUrlParam(_url, 'page', curpage);
                }
                return { url: _url };
            }
        })();
        if (this.opts.getColModels) {
            var colmodels = this.opts.getColModels(self, this.opts);
                console.log('attributes info', colmodels);
                if (!colmodels || colmodels.length == 0) return false;
                self.opts.colModel = colmodels
                var tempconfig = self.opts.filterColModel && self.opts.filterColModel(self.opts.colModel, self);
                // tempconfig = $.extend(true, self.opts.colModel, tempconfig)
                self.opts.colModel = tempconfig;
                var config = getDataGridConfig(self.opts);
            var _entityname = getColumnEntityName(self.opts.colModel);
            if (_entityname) {
                _entityname=_entityname.toLowerCase();
            }
                //添加复选框列
                if (self.opts.itemsBtnTmpl) {
                    config.colModel.unshift(
                        {
                            title: "操作", dataIndx: 'cdatagrid_editer', editable: false, minWidth: 80, notHeaderFilter: true, sortable: false, render: function (ui) {
                                var datas = ui.rowData;
                                var dataIndx = ui.dataIndx;
                                var column = ui.column;
                                var recordid = datas[dataIndx];
                                if (recordid == 'noshow') {
                                    return '';
                                } else {
                                    return self.opts.itemsBtnTmpl || '';
                                }
                            }
                        });
                }
                //添加操作列
            if (self.opts.addCheckbox) {
                self.opts.checkboxinfo ? config.colModel.unshift(self.opts.checkboxinfo) : config.colModel.unshift({
                    title: "", dataIndx: "recordid", maxWidth: 48, minWidth: 48, align: "center", resizable: false,
                    type: 'checkBoxSelection', cls: 'ui-state-default', sortable: false, editable: false,
                    render: function (ui) {
                        //console.log(primarykey);
                        var datas = ui.rowData;
                        var dataIndx = ui.dataIndx;
                        var column = ui.column;
                        if (_entityname) {
                            _entityname = _entityname.toLowerCase();
                        }
                        var recordid = datas[_entityname + 'id'];
                        return '<input type="checkbox" value="' + recordid + '" name="recordid" class="">'
                    },
                    cb: { header: true, all: true },
                }
                );
            }
                self.opts.colModel = self.opts.columnFilter ? self.opts.columnFilter(config.colModel) : config.colModel;
                
                self.$grid = self.box;
                if (self.opts.isFormular) {
                    var formular = gridFormular.formularInit(self.$grid);
                    connectFormularConfig(config, formular);
                }
                console.log('gridconfig', config);
                self.$plugGrid = self.$grid.pqGrid(config);
                self.bindEvent();
                self.opts.initAfter && self.opts.initAfter(self);
                self.$grid.trigger('xmsDatagrid.initAfter', { grid: self, $grid: self.$grid });
                //self.$plugGrid.find('td').enableSelection();
        } else {
            var tempconfig = self.opts.filterColModel && self.opts.filterColModel(self.opts, self);
            tempconfig = $.extend(true, self.opts, tempconfig)
            var config = getDataGridConfig(tempconfig);
            self.opts.colModel = self.opts.columnFilter ? self.opts.columnFilter(config.colModel) : config.colModel;
            self.$grid = self.box;
            if (self.opts.isFormular) {
                var formular = gridFormular.formularInit(self.$grid);
                connectFormularConfig(config, formular);
            }
           console.log('gridconfig', config);
            self.$plugGrid = self.$grid.pqGrid(config);
            self.bindEvent();
            self.opts.initAfter && self.opts.initAfter(self);
            self.$grid.trigger('xmsDatagrid.initAfter', { grid: self, $grid: self.$grid });
        }
        if ($.fn.contextMenu) {
            self.createRightClick();
        }
    }
    
  //  cDatagrid.prototype.afterAjax = function (callback) {
   //    callback && callback();
   // }
    cDatagrid.prototype.destroy = function (keynames) {
        this.$grid.pqGrid("destroy");
    }
    cDatagrid.prototype.getData = function (keynames) {
        this.$grid.pqGrid("getData", { dataIndx: keynames })
    }
    cDatagrid.prototype.deleteRow = function (index) {
        this.$grid.pqGrid("deleteRow", { rowIndx: index });
    }
    //
    cDatagrid.prototype.triggerGridMethod = function (method,opts) {
        this.$grid.pqGrid(method, opts)
    }
    cDatagrid.prototype.setOpts = function (opts) {
        $.extend(this.opts, opts);
    }
    cDatagrid.prototype.getColModel = function (opts) {
        return this.$grid.pqGrid("getColModel");
    }
    cDatagrid.prototype.refreshHeader = function (opts) {
        return this.$grid.pqGrid("refreshHeader");
    }
    cDatagrid.prototype.refreshColumn = function (opts) {
        return this.$grid.pqGrid("refreshColumn");
    }
    
    cDatagrid.prototype.refresh = function () {
        this.$grid.pqGrid("refresh")
    }
    cDatagrid.prototype.getGrid = function () {
        return this.$plugGrid;
    }
    cDatagrid.prototype.addRow = function (data, key) {
        var self = this;
        var grid = this.$grid;
        var primarykey = getPrimaryKey(grid)();
        data = data || {};
        key = key || 'recordid'
        var rowdata = { rowData: $.extend({},data) };
        rowdata.rowData[key] = Xms.Utility.Guid.NewGuid().ToString('N');
      //  console.log(self.opts.colModel)
        $.each(self.opts.colModel, function (i,n) {
            if (n.dataIndx == 'cdatagrid_editer' || n.dataIndx == key) return true;
            var isrelate = n.dataIndx.indexOf('.')!=-1;
            if (n.attributetypename == 'datetime') {
                if (n.dataIndx == 'createdon' || n.dataIndx == 'modifiedon') {

                    rowdata.rowData[n.dataIndx] = new Date().format('yyyy-MM-dd hh:mm:ss');
                }
            } else if (n.attributetypename == 'owner' || n.dataIndx == 'modifiedby' || n.dataIndx == 'createdby') {
                if (typeof CURRENT_USER !== 'undefined') {
                    rowdata.rowData[n.dataIndx + 'name'] = CURRENT_USER.username;
                    rowdata.rowData[n.dataIndx] = CURRENT_USER.systemuserid;
                }

            } else if (n.attributetypename == 'lookup' && Xms.Page.PageContext.RecordId!="" && _record  && (Xms.Page.PageContext.EntityName && Xms.Page.PageContext.EntityName.toLowerCase() == n.dataIndx.replace(/id$/, ''))) {
                rowdata.rowData[n.dataIndx + 'name'] = _record['name'];
                rowdata.rowData[n.dataIndx] = Xms.Page.PageContext.RecordId;
            }
        });
        grid.pqGrid('addRow', rowdata);
    }
    cDatagrid.prototype.refreshDataAndView = function () {
        this.opts.refreshDataAndView && this.opts.refreshDataAndView(self);
        this.$grid.pqGrid("refreshDataAndView");
        this.opts.refreshDataAndViewed && this.opts.refreshDataAndViewed(self);
    }
    cDatagrid.prototype.getRowData = function (index) {
        
        return this.$grid.pqGrid("getRowData", { rowIndxPage:index});
    }
    cDatagrid.prototype.bindEvent = function () {
        var self = this;
        this.$plugGrid.on('pqgridrefresh pqgridrefreshrow', function () {
            var $grid = $(this);
            if (self.opts.headerFilter) {
               // setTableFilter(self.$grid);
            }
            if (self.opts.gridrefresh) {
                self.opts.gridrefresh(self.$grid, self,this);
            }
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
        this.$plugGrid.on('pqgridrefresh', function () {
            if (self.opts.gridrefreshed) {
                self.opts.gridrefreshed(self.$grid, self, this);
            }
        });
        this.$plugGrid.on("pqgridcellsave", function (event, ui) {

        });

        this.$plugGrid.on("headerCellClick", function (event, ui) {
            self.opts.headerCellClick && self.opts.headerCellClick.call(this, event, ui,self);
        });
        this.$plugGrid.on("pqgrideditorbegin", function (event, ui) {
            var dataIndx = ui.dataIndx, rowData = ui.rowData, rowIndx = ui.rowIndx, newVal = ui.newVal, $editor = ui.$editor, colModel = self.$grid.pqGrid("getColModel");
            rowData.isEdited = true;
            var itemArr = gridFormular.formularInit(self.$grid)();
            gridFormular.setFormularAndRelation(itemArr,'append', function (itemArr, tempArr, itemRuler, leftArr, rightRuler) {
                if (~itemArr.indexOf(dataIndx)) {
                    gridFormular.getFormularResult(leftArr, tempArr[0], rightRuler, rowData, dataIndx, newVal, colModel, function (res) {
                        self.$grid.pqGrid("refresh", { rowIndx: rowIndx });
                    });
                }
                // console.log(itemArr, tempArr, itemRuler, leftArr, rightRuler);
            },event,ui);
            if (typeof dirtyChecker !== 'undefined') {
                dirtyChecker.isDirty = true;
            }
        });
        this.$plugGrid.on("pqgridcellbeforesave", function (event, ui) {
            var dataIndx = ui.dataIndx, rowData = ui.rowData, rowIndx = ui.rowIndx, newVal = ui.newVal, $editor = ui.$editor, colModel = self.$grid.pqGrid("getColModel");
            rowData.isEdited = true;
            var itemArr = gridFormular.formularInit(self.$grid)();
            gridFormular.setFormularAndRelation(itemArr,'formular', function (itemArr, tempArr, itemRuler, leftArr, rightRuler) {
                if (~itemArr.indexOf(dataIndx)) {
                    gridFormular.getFormularResult(leftArr, tempArr[0], rightRuler, rowData, dataIndx, newVal, colModel, function (res) {
                        self.$grid.pqGrid("refresh", { rowIndx: rowIndx });
                    });
                }
               // console.log(itemArr, tempArr, itemRuler, leftArr, rightRuler);
            }, event, ui);
            rowData['datagrid_isEdit'] = true;
        });
        this.$plugGrid.on("pqgridcellsave", function (event, ui) {
            var dataIndx = ui.dataIndx, rowData = ui.rowData;

           // console.log(dataIndx, rowData);
        });
        this.opts.afterBindEvnet && this.opts.afterBindEvnet(this);
    }
    $.fn.cDatagrid = function (opts, _config, _more) {
        if (!(typeof opts == 'string')) {
            opts = $.extend({}, defaults, opts);
            return this.each(function () {
                var $this = $(this);
                var _box = new cDatagrid(this, opts);
                $this.data().cDatagrid = _box;
            });
        } else {
            var res = null;
            this.each(function () {
                var $this = $(this);
                if ($this.data().cDatagrid) {
                    if (!$this.data().cDatagrid[opts]) throw new Error('没有这个方法');
                    res = $this.data().cDatagrid[opts](_config, _more);
                    return false;
                }
            });
            return res;
        }

    }
})();