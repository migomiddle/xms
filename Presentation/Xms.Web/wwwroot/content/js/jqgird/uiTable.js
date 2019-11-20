; (function () {
    "use strict"

    function afterRestoreCell(rowid, value, iRow, iCol) {
        triggerNextCellEdit(rowid, value, iRow, iCol)
    }
    function afterSaveCell(rowid, name, value, iRow, iCol) {
        triggerNextCellEdit(rowid, value, iRow, iCol)
    }

    function triggerNextCellEdit(rowid, value, iRow, iCol, noCheckEvent, notCheckNextCell) {
        var e = window.event;
        console.log('next');
        var datagridlist = $(".datagridlist");
        if ((e && e.keyCode == "13") || noCheckEvent) {
            var rowDom = $("#" + rowid);
            $(".datepicker").remove();
            var _Cell = rowDom.children("td").eq(iCol + 1);
            console.log("_Cell", _Cell);
            if (_Cell.length > 0 && !notCheckNextCell) {
                var isEdit = rowDom.parents(".ui-jqgrid-view:first").find(".ui-jqgrid-labels>th").eq(iCol + 1).attr("data-isedit");
                console.log("isEdit", isEdit);
                if (isEdit == "true") {
                    setTimeout(function () { _Cell.trigger("click") });
                } else {
                    triggerNextCellEdit(rowid, value, iRow, iCol + 1, noCheckEvent)
                }
            } else {
                var rowNext = rowDom.next();
                if (rowDom.length > 0) {
                    var isEdit = rowDom.parents(".ui-jqgrid-view:first").find(".ui-jqgrid-labels>th").eq(0).attr("data-isedit");
                    var rowid = rowNext.attr("id");
                    if (isEdit == "true") {
                        setTimeout(function () { _Cell.trigger("click") });
                    } else {
                        triggerNextCellEdit(rowid, value, iRow, 1, noCheckEvent)
                    }
                }
            }
        }
    }

    //自定义 编辑框
    var customEdit = {
        lookup: {
            getElement: function (value, editOptions) {
                //console.log(value);
                console.log("editOptions", editOptions);
                var extendOpts = editOptions.custom_extend;
                var res = $("<div />");
                var html = [];
                //html.push('<div class="input-group">');
                html.push('<input class="form-control customSearch lookup" id="' + extendOpts.name + '" name="' + extendOpts.name + '" value="' + value + '" />');
                //html.push('<span class="input-group-btn"><button class="btn btn-default customBtn" type="button">...</button></span>');
                //html.push('</div>');
                res.html(html.join(""));
                res.find("button.customBtn").on("click", function () {
                    var input = res.find("input.customSearch");
                    editOptions.editCustomCallback && editOptions.editCustomCallback(this, input, res);
                });
                res.find('.customSearch').lookup({
                    dialog: function (obj, callback) {
                        var lookupurl = '/entity/RecordsDialog?getall=true&inputid=' + extendOpts.name + '&entityid=' + extendOpts.referencedentityid;
                        Xms.Web.OpenDialog(lookupurl, null, null, function (data) {
                            //console.log(res.find("input.customSearch"))
                            res.find("input.customSearch").bind("dialog.return", function (e, result) {
                                //console.log("dialogreturn", result);
                                res.find('.customSearch').val(result.name).attr("data-id", result.id);
                                var rowid = res.parents("tr:first").attr("id");
                                console.log(rowid);
                                var value = "";
                                var iRow = res.parents("tr:first").index() - 1;
                                var iCol = res.parents("td:first").index() - 1;
                                //res.parents('.ui-jqgrid-btable:first').jqGrid("saveCell", iRow, iCol);
                                setTimeout(function () { triggerNextCellEdit(rowid, value, iRow, iCol + 1, true) }, 250);
                            });
                        });
                    }
                    , clear: function () {
                    }
                })
                return res;
            },
            setValue: function (elem, oper, value) {
                console.log(elem);
                console.log(oper);
                console.log(value);
                if (oper == "set") {
                    $(elem).text(value);
                }
                if (oper == "get") {
                    //console.log($(elem).find("input.customSearch").val())
                    return $(elem).find("input.customSearch").val();
                }
            }
        },
        picklist: {
            getElement: function (value, editOptions) {
                var extendOpts = editOptions.custom_extend;
                var res = $("<div />");
                var html = [];
                //html.push('<div class="input-group">');
                html.push('<input class="form-control customSearch picklist"  type="text" id="' + extendOpts.name + '" name="' + extendOpts.name + '" value="' + value + '"  data-attributeid="" data-controltype="picklist" data-type="int" data-items="' + encodeURIComponent(JSON.stringify(extendOpts.optionset.items)) + '" value="" data-onlylabel="" data-localizedname="客户类型"  />');
                //html.push('<span class="input-group-btn"><button class="btn btn-default customBtn" type="button">...</button></span>');
                //html.push('</div>');
                res.html(html.join(""));
                res.find("button.customBtn").on("click", function () {
                    var input = res.find("input.customSearch");
                    editOptions.editCustomCallback && editOptions.editCustomCallback(this, input, res);
                });
                res.find('.customSearch').lookup({
                    dialog: function (obj, callback) {
                        var lookupurl = '/entity/RecordsDialog?singlemode=true&inputid=' + extendOpts.name + '&entityid=' + extendOpts.referencedentityid;
                        Xms.Web.OpenDialog(lookupurl, null, null, function (data) {
                            console.log(res.find("input.customSearch"))
                            res.find("input.customSearch").bind("dialog.return", function (e, result) {
                                console.log("dialogreturn", result);
                                res.find('.customSearch').val(result.name).attr("data-id", result.id);
                            });
                        });
                    }
                    , clear: function () {
                    }
                })
                return res;
            },
            setValue: function (elem, oper, value) {
                if (oper == "set") {
                    $(elem).text(value);
                }
                if (oper == "get") {
                    return $(elem).find("input.customSearch").val();
                }
            }
        }
    }

    //配置信息
    var setting = {
        dataFilter: null,
        ModelFilter: null,
        data: null,//数据源,
        colNames: [],
        colModel: null,//数据配置
        dataType: "local",//local本地数据,
        url: null,
        gettype: "get",
        navPagerEleId: null,//工具栏ID
        navPagerSetting: { edit: false, add: false, del: false, find: true, refresh: true },//配置工具栏
        //width:800,
        height: null,
        isAdd: true,
        isDel: true,
        isEdit: true,
        rowEdit: true,
        cellEdit: false,
        afterEditCell: function (id, name, val, iRow, iCol) {
            afterEditCell(id, name, val, iRow, iCol);
        },
        afterSaveCell: function (rowid, name, val, iRow, iCol) {
            afterSaveCell(rowid, name, val, iRow, iCol);
        },
        afterRestoreCell: function (rowid, value, iRow, iCol) {
            afterRestoreCell(rowid, value, iRow, iCol);
        },

        rowNum: 20,
        getDataOpts: {},
        saveSubmit: null,
        saveCallback: null,
        colSums: [],//需要统计的列的key,
        renderedCB: null,
        gridComplete: function (obj) {
        },
        gridSetting: {// jqgrid  的配置信息   会覆盖上面配置的信息
            footerrow: false,//是否显示统计数据的行  ,如果为true，请设置上面的colSums
        }
    }
    $.extend($.fn.jqGrid, {
        setData: function (data) {
            this[0].p.data = data;
            return true;
        }
    });
    $.fn.xmsJqGrid = function (options) {
        var opts = $.extend({}, setting, options);

        return this.each(function () {
            var $this = $(this), self = this;
            var data = opts.data, colModel = opts.colModel;
            /*if(opts.dataFilter){
                data = opts.dataFilter(data);
            }*/

            if (opts.ModelFilter) {
                colModel = opts.ModelFilter(colModel);
                console.log(colModel);
            }

            //对colModel 编辑是添加自定义编辑方式
            $.each(opts.colModel, function (key, obj) {
                if (obj.editCustom) {
                    obj["editoptions"] = {
                        custom_value: customEdit[obj.editCustom]["setValue"],
                        custom_element: customEdit[obj.editCustom]["getElement"],
                        custom_extend: obj.extend,
                        editCustomCallback: obj.editCustomCallback
                    }
                }
            });

            if (opts.cellEdit) {
                opts.rowEdit = false;
                opts.cellsubmit = 'clientArray';
            }
            var gridSetting = {
                datatype: opts.dataType,
                url: opts.url,
                bindKeys: { onEnter: true },
                colModel: colModel,
                //viewrecords: true, // show the current page, data rang and total records on the toolbar
                loadonce: false,
                /*serializeGridData:function(data){
                    //console.log(data);
                    if(opts.dataFilter){
                        return opts.dataFilter(data);
                    }else{
                        return data;
                    }
                },*/
                cellsubmit: opts.cellsubmit,
                cellEdit: opts.cellEdit,
                afterEditCell: opts.afterEditCell,
                afterSaveCell: opts.afterSaveCell,
                afterRestoreCell: opts.afterRestoreCell,
                beforeProcessing: function (data) {
                    if (opts.dataFilter) {
                        opts.dataFilter(data);
                    }
                    //data.rows.splice(3,1);
                },
                //   onSelectRow: editRowAfter, // the javascript function to call on row click. will ues to to put the row in edit mode
                //width: opts.width,
                height: opts.height,
                rowNum: opts.rowNum,
                pager: opts.navPagerEleId,

                userDataOnFooter: true,
                //altRows : true,
                gridComplete: function () {
                    if (opts.rowEdit || opts.cellEdit) {
                        var ids = $this.jqGrid('getDataIDs');
                        for (var i = 0; i < ids.length; i++) {
                            var id = ids[i];
                            var DeleteBtn = "";
                            var addbtn = '', delbtn = '', editbtn = '';
                            if (opts.isAdd) {
                                addbtn = "<a href='javascript:;' class='xmsGrid-add'  title='添加' data-id='" + id + "' >+</a> ";
                            }
                            if (opts.isDel) {
                                delbtn = "<a href='javascript:;' class='xmsGrid-remove' data-id='" + id + "' title='删除' ><em class='glyphicon glyphicon-remove'></em></a> ";
                            }
                            if (opts.isEdit) {
                                editbtn = "<a href='javascript:;' title='编辑' class='xmsGrid-edit'  data-id='" + id + "' ><em class='glyphicon glyphicon-edit'></em></a>";
                            }
                            var editBtn = addbtn + delbtn + editbtn + "<input type='hidden' class='xmsGridRowId' value='" + id + "'>";
                            $this.jqGrid('setRowData', ids[i], { Edit: editBtn, Delete: DeleteBtn });
                        }
                        $(".xmsGrid-add").off("click").on("click", function () {
                            var id = $(this).attr("data-id");
                            addRow(id);
                        });
                        $(".xmsGrid-remove").off("click").on("click", function () {
                            var id = $(this).attr("data-id");
                            delRow(id, this);
                        });
                        $(".xmsGrid-edit").off("click").on("click", function () {
                            var id = $(this).attr("data-id");
                            editRow(id);
                        });
                    }
                    var $grid = $this;
                    if (opts.colSums.length > 0) {
                        var temp = { "Edit": "合计：" };
                        $.each(opts.colSums, function (obj, key) {
                            var colSum = $grid.jqGrid('getCol', key, false, 'sum');
                            temp[key] = colSum;
                        });
                        $grid.jqGrid('footerData', 'set', temp);
                    }
                    opts.renderedCB && opts.renderedCB($this);
                }
            };
            /*if(opts.data){
                gridSetting.data = data;
            }*/

            gridSetting = $.extend({}, gridSetting, opts.gridSetting);
            console.log("gridSetting", gridSetting)
            $this.jqGrid(gridSetting);
            /*  if(opts.url){
                  opts.getDataOpts = $.extend({},{url:opts.url},opts.getDataOpts);
                  fetchGridData();
              }

              //
              function fetchGridData() {
                  var gridArrayData = [];
                  // show loading message
                //  $this[0].grid.beginReq();

                  $.ajax(opts.getDataOpts).done(function(result){
                      var data = result;
                      if(opts.dataFilter){
                          data =   opts.dataFilter(result)
                      }
                      // set the new data
                      $this
                          .jqGrid('clearGridData')
                          .jqGrid('setGridParam', {datatype:'local', page:1, data: data}).trigger("reloadGrid", [{ page: 1}]);
                  });
              }*/

            $this.jqGrid('navGrid', opts.navPagerEleId, opts.navPagerSetting);

            $this.on("xmsGrid.reload", function (e, opts) {
                //$this.jqGrid('clearGridData')
                $this.trigger('reloadGrid', opts);
            });
            $this.on("xmsGrid.addRow", function (e) {
                addRow();
            });
            $this.on("xmsGrid.delRow", function (e, opts) {
                //delRow(opts);
            });
            $this.on("xmsGrid.editRow", function (e, opts) {
                // editRow(opts);
            });
            $this.on("xmsGrid.searchGrid", function (e, opts) {
                $this.jqGrid('searchGrid', opts);
            });
            $this.on("xmsGrid.dbclickRow", function () {
            });

            //保存数据
            $(opts.saveSubmit).on("click", function () {
                var data = $this.jqGrid("getGridParam");
                opts.saveCallback && opts.saveCallback(data.data, data, $this);
                console.log(data.data);
            });
            //编辑
            var lastSelection;
            function editRow(id) {
                var grid = $this;
                var e = window.event;
                if (e && e.stopPropagation) {
                    e.stopPropagation();
                }

                if (id) {
                    grid.jqGrid('restoreRow', lastSelection);
                    grid.jqGrid('editRow', id, {
                        keys: true, focusField: 4, oneditfunc: function (rowid) {
                            console.log(rowid);
                        },
                        url: 'clientArray',
                        aftersavefunc: 'clientArray',
                        succesfunc: function (response) {
                            alert("save success");
                            return true;
                        },
                        errorfunc: function (rowid, res) {
                            console.log(rowid);
                            console.log(res);
                        }
                    });
                    $this.trigger("xmsGrid.editRow", { id: id, data: grid.jqGrid('getRowData', id) });
                    lastSelection = id;
                }
            }

            function editRowAfter(id) {
                var e = window.event;
                var target = e && (e.target || e.srcElement);
                if (e && e.stopPropagation) {
                    e.stopPropagation();
                }

                if ($(target).closest("button,.modal-dialog").length > 0) {
                    return false;
                }
                var grid = $this;
                // $this.trigger("xmsGrid.CheckEditRow", { id: id, data: grid.jqGrid('getRowData', id) });
                if (id && id != lastSelection) {
                    if (lastSelection) {
                        grid.jqGrid('saveRow', lastSelection, false);
                    } else if (newrowid && id && id != newrowid) {
                        grid.jqGrid('saveRow', newrowid, false);
                        grid.jqGrid('editRow', id, false);
                    }
                } else {
                    if (id) {
                        if ($(target).length > 0) {
                            var index = $(target).index();
                            grid.jqGrid('editCell', id, index, false);
                        } else {
                            grid.jqGrid('editRow', id, false);
                        }
                    }
                }
                lastSelection = id;
                //$this.trigger("xmsGrid.editRowEnd", { id: id, data: grid.jqGrid('getRowData', id) });
            }

            //删除
            function delRow(selectedId, obj) {
                var e = window.event;
                if (e && e.stopPropagation) {
                    e.stopPropagation();
                }
                if (confirm("确定要删除该行？")) {
                    $this.jqGrid("delRowData", selectedId);
                    $this.trigger("xmsGrid.delRow", { id: selectedId, btn: $(obj) });
                }
            }

            //add
            //grid添加新的一行
            var newrowid;
            function addRow() {
                var grid = $this;
                var e = window.event;
                if (e && e.stopPropagation) {
                    e.stopPropagation();
                }
                var selectedId = grid.jqGrid("getGridParam", "selrow");
                var ids = grid.jqGrid('getDataIDs');
                console.log(ids);
                //获得当前最大行号（数据编号）
                var type = 'int';
                if (ids.length > 0) {
                    if (isNaN(ids[0])) {
                        type = 'string';
                    }
                }
                if (type == 'int') {
                    var rowid = Math.max.apply(Math, ids);
                } else {
                    var rowid = ids[ids.length - 1];
                }
                //获得新添加行的行号（数据编号）
                newrowid = rowid + setTimeout(0);
                var dataRow = {
                    id: "",
                    valid: "",
                    zoneID: '',
                    factorPG: '',
                    factorQG: '',
                    factorPL: '',
                    factorQL: '',
                    caseID: ''
                };

                //将新添加的行插入到最后一行
                grid.jqGrid("addRowData", newrowid, dataRow, "last");
                //设置grid单元格不可编辑
                // grid.setGridParam({cellEdit:false});
                //设置grid单元格可编辑
                // grid.jqGrid('editRow', newrowid, false);
            }
        });
    }
})();