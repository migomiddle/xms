//@ sourceURL=pages/entity.rendergridview.js
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
    //page Url = /pages/entity.rendergridview.js
    //deps
    var pageWrap_RenderGirdView = {}
    $.extend(pageWrap_RenderGirdView, pageRenderGridViewInfo);
    console.log(pageWrap_RenderGirdView);

    ; (function () {
        function renderDefaultItems(count) {
            Xms.Web.Console('xxxxxxx', GridViewModel.defaultEmptyRows);
            count = count || GridViewModel.defaultEmptyRows || 0;
            var rowcount = $(GridViewModel.sectionid + " .datatable").find("tr:gt(0)").length;//计算当前行
            for (var i = 0; i < count; i++ , rowcount++) {
                var obj = { entityid: GridViewModel.entityid, data: {} };
                var id = Xms.Utility.Guid.NewGuid().ToString();
                obj.data.id = id;
                //referenced recordid
                if (GridViewModel.relationshipname != '' && !obj.data[GridViewModel.referencingattributename]) {
                    obj.data[GridViewModel.referencingattributename] = GridViewModel.referencedrecordid;
                }
                obj.name = GridViewModel.entityname;
                obj.relationshipname = GridViewModel.relationshipname;
                var $newrow = GridViewModel.editrowmodel.clone(true);
                var childRecords = getrecords();
                childRecords.push(obj);
                setrecords(childRecords);

                $newrow.find('input[name=recordid]').val(obj.data.id);
                $newrow.show();
                $(GridViewModel.sectionid + " .datatable").find('tbody').append($newrow);
                $newrow.find('input[name=rownumber]').val(rowcount);
                $newrow.find('td.row-num').text(rowcount);
            }
            //console.log('count',count);
            if (count == 1) {
                return $newrow;
            }
        }
        //return false;
        var GridViewModel = $.extend({}, pageWrap_RenderGirdView,
            {
                ajaxTable: function (count, flag) {
                    var self = $(GridViewModel.sectionid + " .datatable");
                    var containerId = '#' + self.attr('data-ajaxcontainer');
                    self.parent().delegate('a[data-ajax="true"]', 'click', function (e) {
                        e.preventDefault();
                        var url = $(this).attr('href');
                        url = url + (url.indexOf('?') == -1 ? '?' : '&') + '__r=' + new Date().getTime();
                        GridViewModel.rebind(url);
                        return false;
                    });
                    if (!flag) {
                        renderDefaultItems(count);
                    }
                },
                ajaxgrid_reset: function (count, flag) {
                    GridViewModel.ajaxTable(count, flag);
                    GridViewModel.pageUrl = $(GridViewModel.sectionid + " .datatable").attr('data-pageurl');
                    GridViewModel.pag_init();
                    Xms.Web.DataTable($(GridViewModel.sectionid + " .datatable"));
                    //默认第一列为快速查找字段
                    $(GridViewModel.sectionid + ' #fieldDropdown').next().find('a:eq(1)').trigger('click');
                    $(GridViewModel.sectionid + ' button[name=clearBtn]:first').on('click', null, function (e) {
                        $(GridViewModel.sectionid + ' #Q').val('');
                        GridViewModel.q = '';
                        GridViewModel.rebind();
                    });
                    $(GridViewModel.sectionid + ' button[name=searchBtn]').off('click').on('click', null, function (e) {
                        GridViewModel.q = $(GridViewModel.sectionid + ' #Q').val();
                        GridViewModel.qfield = $(GridViewModel.sectionid + ' #QField').val() || '';
                        GridViewModel.rebind();
                    });
                    // var isrunNext = true;
                    $(GridViewModel.sectionid + ' input.quickly-search-input').off('click').on("keydown", function (e) {
                        e = e || window.event;
                        //if(isrunNext == false)return false;
                        //isrunNext = false;
                        if (e.keyCode == 13) {
                            GridViewModel.q = $(GridViewModel.sectionid + ' #Q').val();
                            GridViewModel.qfield = $(GridViewModel.sectionid + ' #QField').val() || '';
                            GridViewModel.rebind(null, function () {
                                //  isrunNext = true;
                            });
                        }
                    });
                    $(GridViewModel.sectionid + ' .datatable').find('input.normal-input').off('keydown').on('keydown', function (e, obj) {
                        e = e || event || window.event;
                        var ecode = e.keyCode;
                        var $this = $(this);
                        var curTr = $this.parents('tr:first');
                        curTr.attr('data-edited', true);
                        if ($this.data().msgbox && $this.data().msgbox.length > 0) {
                            $this.data().msgbox.hide();
                        }
                        if (obj && obj.noGoNext) {
                            riggerNextInputFocus(this, true);
                        } else if (ecode == "13") {
                            triggerNextInputFocus(this);
                        }
                    }).off('change').on('change', function () {
                        var $this = $(this);
                        var curTr = $this.parents('tr:first');
                        curTr.attr('data-edited', true);//火狐下会没有触发keydown事件，所以添加
                        var isChecke = checkEntityType($this);
                        if (isChecke == false) { return false; }
                    });
                    $(GridViewModel.sectionid + ' button[name=createBtn]').off('click').on('click', null, function (e) {
                        GridViewModel.CreateRecord();
                    });
                    if (GridViewModel.referencedrecordid != '') {
                    }
                    else if (GridViewModel.relationshipname != '') {
                        $(GridViewModel.sectionid + ' .toolbar button:not(.btnLocal)').addClass('disabled').prop('disabled', 'disabled');
                    }
                    $(GridViewModel.sectionid + ' button[name=deleteBtn]').off('click').on('click', null, function (e) {
                        GridViewModel.DeleteRecord();
                        $('body').trigger('gridview.delete');
                        $(GridViewModel.sectionid).trigger('gridviewByid.delete');
                        $(GridViewModel.sectionid).trigger('gridviewByid.server.delete');
                        GridViewModel.resetRowNumber();
                        GridViewModel.savetable();
                    });
                    $(GridViewModel.sectionid + ' button[name=addRowBtnLocal]').off('click').on('click', null, function (e) {
                        var newrow = renderDefaultItems(1);
                        GridViewModel.ajaxgrid_reset(1, true);
                        if (typeof setSubGridFormular == 'function') {
                            setSubGridFormular($(GridViewModel.sectionid).parents('.subgrid:first'), 'gridview');
                        }
                        $('body').trigger('gridview.add', { row: newrow });
                        $('body ' + GridViewModel.sectionid).trigger('gridviewByid.add', { row: newrow });
                        $(GridViewModel.sectionid).trigger('gridviewByid.server.add');
                        GridViewModel.resetRowNumber();
                        GridViewModel.savetable();
                    });
                    $(GridViewModel.sectionid + ' button[name=saveBtnLocal]').off('click').on('click', null, function (e) {
                        saveCurrentSubGrid();
                        $('body').trigger('gridview.save');
                        $(GridViewModel.sectionid).trigger('gridviewByid.save');
                        $(GridViewModel.sectionid).trigger('gridviewByid.server.save');
                    });
                    $(GridViewModel.sectionid + ' button[name=freshBtn]').off('click').on('click', null, function (e) {
                        GridViewModel.rebind(null, function () {
                            if (typeof setSubGridFormular !== 'undefined') {
                                setSubGridFormular($(GridViewModel.sectionid).parents('.subgrid:first'), 'gridview');
                            }
                        });

                        $('body').trigger('gridview.rebind');
                        $(GridViewModel.sectionid).trigger('gridviewByid.rebind');
                        $(GridViewModel.sectionid).trigger('gridviewByid.server.rebind');
                    });
                    $(GridViewModel.sectionid + ' button[name=resetRowBtn]').off('click').on('click', null, function (e) {
                        var parTr = $(this).parents('tr:first');
                        GridViewModel.resetRow(parTr);
                        setSubGridFormular($(GridViewModel.sectionid).parents('.subgrid:first'), 'gridview');
                        $('body').trigger('gridview.reset');
                        $(GridViewModel.sectionid).trigger('gridviewByid.reset');
                        $(GridViewModel.sectionid).trigger('gridviewByid.server.reset');
                    });
                    GridViewModel.initedit();
                },
                pag_init: function () {
                    $(GridViewModel.sectionid + ' #page-selection').bootpag({
                        total: $(GridViewModel.sectionid + ' #page-selection').attr('data-total')
                        , maxVisible: 5
                        , page: $(GridViewModel.sectionid + ' #page-selection').attr('data-page')
                        , leaps: true
                        , prev: '&lsaquo;'
                        , next: '&rsaquo;'
                        , firstLastUse: true
                        , first: '&laquo;'
                        , last: '&raquo;'
                        //, wrapClass: ''
                    }).on("page", function (event, num) {
                        event.preventDefault();
                        GridViewModel.page = num;
                        GridViewModel.rebind();
                        return false;
                    });
                    $(GridViewModel.sectionid).off('subpage.resetWidth').on('subpage.resetWidth', settableHeaderWidth).trigger('subpage.resetWidth');
                    console.log('GridViewModel.sectionid', $(GridViewModel.sectionid).parent());
                },
                rebind: function (url, callback) {
                    var url = url || '/entity/rendergridview';
                    url = url + (url.indexOf('?') == -1 ? '?' : '&') + '__r=' + new Date().getTime();
                    //console.log(url);
                    var model = new Object();
                    model.EntityId = GridViewModel.entityid;
                    model.QueryId = GridViewModel.queryid;
                    model.RelationShipName = GridViewModel.relationshipname;
                    model.ReferencedRecordId = GridViewModel.referencedrecordid;
                    model.Filter = GridViewModel.filters;
                    model.Q = $.getUrlParam("q", url) || GridViewModel.q;
                    model.QField = $.getUrlParam("qfield", url) || GridViewModel.qfield;
                    model.SortBy = $.getUrlParam("sortby", url) || GridViewModel.sortby;
                    model.SortDirection = $.getUrlParam("sortdirection", url) || GridViewModel.sortdir;
                    model.Page = $.getUrlParam("page", url) || GridViewModel.page;
                    model.PageSize = $.getUrlParam("pagesize", url) || GridViewModel.pagesize;
                    model.IsEditable = GridViewModel.iseditable;
                    model.defaultEmptyRows = GridViewModel.defaultEmptyRows;
                    model.PagingEnabled = GridViewModel.pagingEnabled;
                    //console.log(model);
                    Xms.Web.LoadPage(url, model, function (response) {
                        Xms.Web.Console(response);
                        //$(GridViewModel.sectionid + ' .gridview').html($(response).find('.gridview').html());
                        $(GridViewModel.sectionid).html($(response).html());
                        GridViewModel.ajaxgrid_reset();
                        callback && callback();

                        $('body').trigger('gridview.subgridRebind');
                        $(GridViewModel.sectionid).trigger('gridviewByid.server.subgridRebind');
                        GridViewModel.resetRowNumber();
                    });
                },
                CreateRecord: function () {
                    Xms.Web.Console(GridViewModel);
                    var url = ORG_SERVERURL + '/entity/create?entityid=' + GridViewModel.entityid + '&relationshipname=' + GridViewModel.relationshipname + '&referencedrecordid=' + GridViewModel.referencedrecordid + '&grid=' + GridViewModel.gridid;
                    //Xms.Web.OpenWindow(url);
                    $('#createModal').modal({
                        keyboard: true
                    });
                    $('#createModal').find('.modal-body').html('<iframe src="' + url + '" frameborder="0" width="100%" height="500"></iframe>');
                    //$('#createModal').find('.modal-content').css('width',$(document).width()/2).css('height',$(document).height()/1.5);
                    $('#createModal').find('.modal-title').html('<span class="glyphicon glyphicon-file"></span> ' + GridViewModel.entityloclaizedname + ' - ' + (typeof LOC_NEWRECORD == 'undefined' ? '新建' : LOC_NEWRECORD));
                },
                DeleteRecord: function () {
                    var target = $(GridViewModel.sectionid + ' .datatable');
                    var id = Xms.Web.GetTableSelected(target);
                    // console.log(id);
                    //var parRow = $(this).parents('tr:first');
                    //GridViewModel.removeRow(parRow);
                    //$(this).trigger('gridview.removeRowBtn');//值计算时重新绑定对应的输入框的值
                    //e.preventDefault();
                    Xms.Web.Del(id, '/api/delete?entityid=' + GridViewModel.entityid, false, function () { GridViewModel.rebind('/entity/rendergridview'); });
                },
                initedit: function () {
                    $(GridViewModel.sectionid).find('.picklist:not(:disabled),.bit:not(:disabled),.status:not(:disabled)').each(function (i, n) {
                        var self = $(n);
                        //console.log(self)
                        var items = JSON.parse(decodeURIComponent(self.attr('data-items')));
                        var isdefault = (self.val() && self.val() != '') ? true : false;
                        self.picklist({
                            required: self.is('.required'),
                            items: items,
                            isDefault: isdefault,
                            changeHandler: function (e, obj) {
                                Xms.Web.Console('change', self);
                                self.parents('tr:first').attr('data-edited', true);
                                if (obj && obj.noGoNext) {
                                    triggerNextInputFocus(self, true);
                                } else {
                                    triggerNextInputFocus(self);
                                }
                            }
                        });
                    });

                    function checkTrInList(list, tr) {
                        var index = -1;
                        if (list.length == 0) return index;
                        $.each(list, function (key, item) {
                            if (item.get(0) == tr.get(0)) {
                                index = key;
                                return false;
                            }
                        });
                        return index;
                    }
                    var lookuplist = [];//防止多个相同实体且是引用类型的会给关联的字段多次赋值
                    var _trlist = [];
                    //console.log($(GridViewModel.sectionid + ' .datatable').find('.lookup:not(:disabled)'))
                    $(GridViewModel.sectionid + ' .datatable').find('.lookup:not(:disabled)').each(function (i, n) {
                        var self = $(n);
                        if (!self.prop('id')) {
                            self.prop('id', self.prop('name') + Xms.Utility.Guid.NewGuid().ToString('N'));
                        }
                        //console.log('self',self)
                        var lookupid = self.attr('data-lookup');
                        var inputid = self.prop('id');
                        var valueid = inputid.replace(/_text/, '');
                        var parentTr = self.parents('tr:first');
                        var _isRelative = false;
                        //var queryid = self.attr('data-defaultviewid');
                        if (!~checkTrInList(_trlist, parentTr)) {
                            lookuplist = [];
                            _trlist.push(parentTr);
                        }

                        if (!~$.inArray(lookupid, lookuplist)) {
                            _isRelative = true;
                            lookuplist.push(lookupid);
                        }
                        //console.log(self.prop('name')+'    lookuplist',_isRelative);
                        self.attr('data-_isRelative', _isRelative);
                        var value = self.val() || '';
                        //if(value && value!=''){
                        //    console.log('valueid',$('#' + valueid))
                        //    console.log('inputid',$('#' + valueid))
                        //}
                        self.next().prop('id', valueid);
                        var lookupurl = '/entity/RecordsDialog?inputid=' + inputid + '&sortby=CreatedOn&singlemode=true';

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
                                    Xms.Web.OpenDialog(lookupurl, 'selectRecordCallback', { filter: f });
                                } else {
                                    Xms.Web.OpenDialog(lookupurl, 'selectRecordCallback');
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
                                    Xms.Web.Console('lookup', tar)

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
                            if (obj && obj.noGoNext) {
                                triggerNextInputFocus($('#' + inputid), true);
                            } else {
                                triggerNextInputFocus($('#' + inputid));
                            }
                            $(this).parents('tr:first').attr('data-edited', true);
                        });
                        $('#' + inputid).bind('change', function () {
                            var $this = $(this);
                            var inputid = $(this).prop('id');
                            var valueid = inputid.replace(/_text/, '');
                            var v = $('#' + valueid).val();
                            var entityid = $(this).attr('data-lookup');
                            var relationData = $this.data().relationData = [];
                            if (v && v != '') {
                                var params = {
                                    type: entityid + v + 'true',
                                    data: { entityid: entityid, value: v, allcolumns: true }
                                }
                                console.log('_isRelative', _isRelative);
                                if (self.attr('data-_isRelative') && self.attr('data-_isRelative') == "true") {
                                    Xms.Web.PageCache('renderGridView', '/api/data/Retrieve/ReferencedRecord/' + params.data.entityid + '/' + params.data.value + '/' + params.data.allcolumns, params, function (response) {
                                        var data = response.content;
                                        var $row = $this.parents('tr:first');
                                        $row.find('td[data-name]').each(function (i, n) {
                                            var $td = $(this);
                                            if ($('#' + valueid).parents('td').is($td)) return true;

                                            var attrname = $td.attr('data-name') ? $td.attr('data-name').toLowerCase() : '';
                                            if (!attrname) return true;
                                            var type = $td.attr('data-type');
                                            if ($td.attr('data-entityname').toLowerCase() != GridViewModel.entityname.toLowerCase()) {//关联记录带出字段内容
                                                relationData.push({ td: $td, attrname: attrname });
                                                if ((data[attrname] !== '' && data[attrname] !== null && data[attrname] !== undefined)) {
                                                    // console.log(attrname,type,eval('data.' + attrname));
                                                    if (type == 'nvarchar' || type == 'money' || type == 'int') {
                                                        $td.find('input[name=' + GridViewModel.nameprefix + attrname + ']').val(data[attrname]);
                                                    }
                                                    else if (type == 'owner' || type == 'lookup' || type == 'customer' || type == 'picklist' || type == 'state' || type == 'bit' || type == 'status') {
                                                        $td.text(data[attrname + 'name']);
                                                    } else {
                                                        $td.text(data[attrname]);
                                                    }
                                                }
                                            }
                                        });
                                        $this.trigger('dialog.relationReturn', { row: $row, data: data });
                                    });
                                }
                                // });
                            }
                        });

                        var attrName = self.attr('data-name') ? self.attr('data-name').toLowerCase() : '';
                        //通过引用实体添加时，则填充引用字段值
                        if (GridViewModel.relationshipname != '' && GridViewModel.referencedrecordid != '' && GridViewModel.relationshipname.toLowerCase().indexOf(attrName) > 0) {
                            var data = { name: GridViewModel.relationshipname };
                            if (self.attr('data-name')) {
                                if (GridViewModel.relationshipmeta.referencingattributename.toLowerCase() == attrName) {
                                    var lookupid = GridViewModel.relationshipmeta.referencedentityid;
                                    var params = {
                                        type: lookupid + GridViewModel.referencedrecordid,
                                        data: { entityid: lookupid, value: GridViewModel.referencedrecordid }
                                    }
                                    Xms.Web.PageCache('renderGridView', '/api/data/Retrieve/ReferencedRecord/' + params.data.entityid + '/' + params.data.value, params, function (response) {
                                        var obj = { id: response.content.id, name: response.content.name };
                                        $('#' + inputid).val(obj.name);
                                        $('#' + valueid).val(obj.id);
                                        $('#' + inputid).trigger('change');
                                        $('#' + inputid).parents('.input-group').find('button').prop('disabled', 'disabled');
                                        $("#" + inputid).trigger("searchDialog", { isDefault: true });
                                    });
                                    //  });
                                }
                            }
                        }
                        if (GridViewModel.relationshipname != '' && GridViewModel.referencedrecordid == '' && GridViewModel.relationshipname.toLowerCase().indexOf(attrName) > 0) {
                            $('#' + inputid).parents('.input-group').find('button').prop('disabled', 'disabled');
                        }
                    });

                    $(GridViewModel.sectionid + ' .datatable button[name=saveRowBtn]').off('click').on('click', null, function (e) {
                        Xms.Web.Console('saverow');
                        GridViewModel.saverow($(this).parents('tr:first'));
                        $(this).trigger('gridview.saveRow');//值计算时重新绑定对应的输入框的值
                        $(GridViewModel.sectionid).trigger('gridview.control.saveRow');
                        e.preventDefault();
                    });
                    $(GridViewModel.sectionid + ' .datatable button[name=editRowBtn]').off('click').on('click', null, function (e) {
                        Xms.Web.Console('editrow');
                        GridViewModel.editrow($(this).parents('tr:first'));
                        $(this).trigger('gridview.editRow');//值计算时重新绑定对应的输入框的值
                        $(GridViewModel.sectionid).trigger('gridview.control.editRow');
                        e.preventDefault();
                        GridViewModel.resetRowNumber();
                        GridViewModel.savetable();
                    });
                    $(GridViewModel.sectionid + ' .datatable button[name=removeRowBtn]').off('click').on('click', null, function (e) {
                        var parRow = $(this).parents('tr:first');
                        GridViewModel.removeRow(parRow);
                        $(this).trigger('gridview.removeRowBtn');//值计算时重新绑定对应的输入框的值
                        $(GridViewModel.sectionid).trigger('gridview.control.removeRowBtn');
                        e.preventDefault();
                        GridViewModel.resetRowNumber();
                        GridViewModel.savetable();
                    });
                    $(GridViewModel.sectionid + ' .datatable button[name=removeRowBtnLocal]').off('click').on('click', null, function (e) {
                        var parRow = $(this).parents('tr:first');
                        GridViewModel.removeRowBtnLocal(parRow);
                        $(this).trigger('gridview.removeRowBtnLocal');//值计算时重新绑定对应的输入框的值
                        $(GridViewModel.sectionid).trigger('gridview.control.removeRowBtnLocal');
                        e.preventDefault();
                        GridViewModel.resetRowNumber();
                        GridViewModel.savetable();
                    });
                    $(GridViewModel.sectionid + ' .datatable button[name=cancelRowBtn]').off('click').on('click', null, function (e) {
                        Xms.Web.Console('canceledit');
                        if (GridViewModel.createmode == 'local') {
                            var $row = $(this).parents('tr:first');
                            var id = $row.find('input[name=recordid]').val();
                            GridViewModel.removerecord(id);
                            $row.remove();
                        }
                        else {
                            $(GridViewModel.sectionid + " .datatable").find('tbody>tr.editrow').remove();
                            GridViewModel.editrowmodel.find('button[name=cancelRowBtn]').addClass('hide');
                            $(GridViewModel.sectionid + " .datatable").find('tbody').prepend(GridViewModel.editrowmodel);
                            $(GridViewModel.sectionid + " .datatable").find('tbody>tr').removeClass('hide');
                        }
                        $(this).trigger('gridview.cancelRow');//值计算时重新绑定对应的输入框的值
                        $(GridViewModel.sectionid).trigger('gridview.control.cancelRow');
                        e.preventDefault();
                        GridViewModel.resetRowNumber();
                        GridViewModel.savetable();
                    });
                    // $('.tableheaderResize').tableHdResize();
                    // console.log($('.datepicker:not(disabled)',GridViewModel.sectionid + " .datatable"))
                    $('.datepicker:not(:disabled)', GridViewModel.sectionid + " .datatable").each(function () {
                        var format = $(this).attr('data-format') || 'yyyy-MM-dd HH:mm:ss';
                        var tempformat = format;
                        var dataname = $(this).attr('data-name');
                        var value = $(this).val();
                        if (value != '') {
                            $(this).val(new Date(value).format(format))
                        }
                        Xms.Web.Console(dataname, format);
                        format = format.replace("yyyy", "Y").replace("dd", "d").replace("hh", "h").replace("mm", "i").replace('MM', "m").replace('ss', "s").replace('HH', "H").replace('h', "H");

                        if (tempformat.indexOf("hh:mm") > -1) {
                            $(this).datetimepicker({
                                language: "en"
                                , step: 15
                                , format: format
                                , scrollInput: !1
                                , scrollMonth: false
                            }).on('change', function (e, obj) {
                                var curTr = $(this).parents('tr:first');
                                curTr.attr('data-edited', true);
                                if (obj && obj.noGoNext) {
                                    triggerNextInputFocus($(this), true);
                                } else {
                                    triggerNextInputFocus($(this))
                                }
                            })
                        } else {
                            $(this).datetimepicker({
                                language: "en"
                                , timepicker: false
                                , format: format
                                , scrollInput: !1
                                , scrollMonth: false
                            }).on('change', function (e, obj) {
                                var curTr = $(this).parents('tr:first');
                                curTr.attr('data-edited', true);
                                if (obj && obj.noGoNext) {
                                    triggerNextInputFocus($(this), true);
                                } else {
                                    triggerNextInputFocus($(this))
                                }
                            })
                        }
                    });
                    if (typeof setSubGridFormular == 'function') {
                        setSubGridFormular($(GridViewModel.sectionid).parent());//添加值计算
                    }
                    console.log('$(GridViewModel.sectionid)', $(GridViewModel.sectionid));
                    $(GridViewModel.sectionid).trigger('gridviewId.loaded', { GridViewModel: GridViewModel });

                    var _parid = $(GridViewModel.sectionid).parents('.subgrid:first').attr('id');

                    if (_parid) {
                        console.log('_parid', $('#' + _parid));
                        $('#' + _parid).trigger('gridview.loaded', { GridViewModel: GridViewModel });
                        $('body').trigger('gridview.' + _parid + '.loaded', { GridViewModel: GridViewModel });
                    }
                }
            });

        var pageCache = { lookup: [] };
        GridViewModel.editrowmodel = $(GridViewModel.sectionid + " .datatable").find('tbody>tr.editrow').clone(true);
        GridViewModel.ajaxgrid_reset();

        //row model
        GridViewModel.getrowmodel = function (row) {
            row.find('[name]').each(function (j, cell) {
                var control = $(cell);
                var val = '', name = control.attr('name');
                if (control.is('input,select,textarea')) {
                    val = control.val();
                }
                else {
                    val = control.text();
                }
                val = encodeURIComponent(val);
                //console.log('row.' + name +'="'+val+'"');
                eval('row.' + name + '="' + val + '"');
            });
            return row;
        }

        GridViewModel.resetRowNumber = function (row) {
            var table = $(GridViewModel.sectionid + " .datatable");
            console.log("table.find('tr:gt(1)')", table.find('tr:gt(1)'));
            table.find('tr:gt(1)').each(function (key, item) {
                var index = key + 1;
                var _page = GridViewModel.page;
                var _page_size = GridViewModel.pagesize;
                index = ((_page - 1) * _page_size) + index;
                $(this).find('.row-num').text(index);
                $(this).find('input[name="rownumber"]').val(index);
            });
        }
        GridViewModel.editable = function () {
            //editable
            if (!GridViewModel.iseditable) {
                return;
            }
            var table = $(GridViewModel.sectionid + " .datatable");
            var editRow = $('<tr class="editrow"></tr>');
            table.find('thead>tr>th').each(function (i, n) {
                var $this = $(n);
                var cell = $('<td></td>');
                if ($this.attr('data-name')) {
                    var type = $this.attr('data-type');
                    var name = $this.attr('data-name');
                    cell.attr('data-type', type);
                    var defaultValue = '';
                    var control = $('<input type="text" class="form-control input-sm ' + type + '" name="' + name + '" data-type="' + type + '" />');
                    if (type == 'lookup') {
                        control.attr('data-lookup', $this.attr('data-lookup'));
                        control.prop('name', name + '_text');
                        cell.append($('<input type="hidden" name="' + name + '" data-type="' + type + '" />'));
                    }
                    else if (type == 'int' || type == 'float' || type == 'money') {
                        defaultValue = 0;
                        control.addClass('text-right');
                        cell.addClass('text-right');
                    }
                    else if (type == 'datetime') {
                        if (name == 'createdon') {
                            defaultValue = new Date().Format('yyyy-MM-dd hh:mm:ss');
                            control.prop('disabled', 'disabled');
                        }
                    }
                    control.val(defaultValue);
                    control.attr('data-value', defaultValue);
                    //row config
                    var config = $.grep(GridViewModel.gridconfig, function (c, i) {
                        return c.name == name;
                    });
                    if (config && config[0]) {
                        if (!config[0].editable) {
                            control.prop('readonly', 'readonly');
                        }
                        if (config[0].bind) {
                            control.attr('data-bind', config[0].bind);
                        }
                        if (config[0].exp) {
                            control.attr('data-exp', config[0].exp);
                        }
                        if (config[0].onchange) {
                            control.attr('data-change-func', config[0].onchange.func);
                            control.attr('data-change-target', config[0].onchange.target);
                            control.bind('change', function () {
                                console.log('change');
                                if ($(this).attr('data-change-func') == 'update') {
                                    var $row = $(this).parents('tr:first');
                                    var target = $row.find('[name=' + $(this).attr('data-change-target') + ']');
                                    console.log(target.attr('data-exp'));
                                    var row = GridViewModel.getrowmodel($row);
                                    var result = eval(target.attr('data-exp'));
                                    console.log(result);
                                    target.val(result);
                                }
                            });
                        }
                    }

                    cell.append(control);
                }
                if ($this.attr('data-operation')) {
                    cell.append('<button type="button" class="btn btn-link btn-xs"><span class="glyphicon glyphicon-saved"></span></button>');
                }
                editRow.append(cell);
            });
            $(GridViewModel.sectionid + " .datatable").find('tbody>tr:first').before(editRow);
        }
        GridViewModel.sumtable = function () {
            if (!GridViewModel.iseditable) {
                return;
            }
            //sum
            var sumRow = $('<tr class="sumrow"></tr>');
            $(GridViewModel.sectionid + " .datatable").find('thead>tr>th').each(function (i, n) {
                var $this = $(n);
                var cell = $('<td></td>');
                var total = 0;
                if ($this.attr('data-name')) {
                    var type = $this.attr('data-type');
                    var name = $this.attr('data-name');
                    var index = $this.index();
                    if (type == 'int' || type == 'float' || type == 'money') {
                        $(GridViewModel.sectionid + " .datatable").find('tbody>tr>td').each(function (ii, nn) {
                            if ($(nn).index() == index) {
                                total += $(nn).val(); console.log(index, $(nn).val());
                            }
                        });
                        cell.addClass('text-right');
                    }
                    cell.text(total);
                }
                sumRow.append(cell);
            });
            if ($(GridViewModel.sectionid + " .datatable").find('.sumrow').length > 0) {
                $(GridViewModel.sectionid + " .datatable").find('.sumrow').replaceWith(sumRow);
            } else {
                $(GridViewModel.sectionid + " .datatable").find('tbody>tr:last').after(sumRow);
            }
        }

        GridViewModel.getTableData = function (type) {
            var table = $(GridViewModel.sectionid + " .datatable");
            var rows = table.find('tbody>tr:gt(0)');
            console.log(rows);
            var flag = true;
            $(GridViewModel.sectionid).find('input[name=allsubgirddata]').val('');
            var res = [];
            rows.each(function () {
                var $row = $(this);
                var obj = { entityid: GridViewModel.entityid, data: {} };
                var id = $row.find('input[name="recordid"]').val();
                var primarykey = $row.find('input[name="recordid"]').attr('data-primarykey');
                var isvalid = true;
                $row.find('td[data-name]').each(function (i, n) {
                    var $td = $(this);

                    var attrname = $td.attr('data-name');
                    //console.log('attrname',attrname);
                    if (!attrname) return true;
                    if (typeof $td.attr('data-entityname') == 'string' && $td.attr('data-entityname').toLowerCase() == GridViewModel.entityname.toLowerCase()) {
                        //console.log(GridViewModel.nameprefix+attrname);
                        var type = $td.attr('data-type');
                        var $input = $td.find('input[name="' + GridViewModel.nameprefix + attrname + '"]');
                        var value = $input.val();
                        //console.log($input, value);
                        if (value == '' && $td.attr('data-isrequired')) {
                        } else {
                            if (type == 'float' || type == 'money') {
                                value = value.replace(/\,/g, '');
                            }
                        }
                        obj.data[attrname] = value;
                    }
                });
                if (primarykey) {
                    obj.data.primarykey = primarykey;
                }
                //console.log('isvalid',isvalid);
                if (!isvalid) { flag = false; return false; }
                obj.data.id = id;

                if (GridViewModel.relationshipname != '' && !obj.data[GridViewModel.referencingattributename]) {
                    obj.data[GridViewModel.referencingattributename] = GridViewModel.referencedrecordid;
                }
                // console.log(obj);
                obj.name = GridViewModel.entityname;
                obj.relationshipname = GridViewModel.relationshipname;
                res.push(obj);
            });
            return res;
            var resultValue = encodeURIComponent(JSON.stringify(res));
            $(GridViewModel.sectionid).find('input[name=allsubgirddata]').val(resultValue);
        }

        GridViewModel.savetable = function (type) {
            var table = $(GridViewModel.sectionid + " .datatable");
            var rows = table.find('tr[data-edited="true"]');
            var flag = true;
            $(GridViewModel.sectionid).find('input[name=tempdata]').val('')
            rows.each(function () {
                var $row = $(this);
                var obj = { entityid: GridViewModel.entityid, data: {} };
                var id = $row.find('input[name="recordid"]').val();
                var primarykey = $row.find('input[name="recordid"]').attr('data-primarykey');
                var rownumber = $row.find('input[name="rownumber"]').val();
                var isvalid = true;
                $row.find('td[data-name]').each(function (i, n) {
                    var $td = $(this);

                    var attrname = $td.attr('data-name');
                    //console.log('attrname',attrname);
                    if (!attrname) return true;

                    if (typeof $td.attr('data-entityname') == 'string' && $td.attr('data-entityname').toLowerCase() == GridViewModel.entityname.toLowerCase()) {
                        //console.log(GridViewModel.nameprefix+attrname);
                        var type = $td.attr('data-type');
                        var $input = $td.find('input[name="' + GridViewModel.nameprefix + attrname + '"]');
                        var value = $input.val();
                        //console.log($input, value);
                        if (value == '' && $td.attr('data-isrequired')) {
                            //referenced record
                            if (GridViewModel.createmode == 'local' && GridViewModel.relationshipname != '' && attrname.toLowerCase() == GridViewModel.referencingattributename.toLowerCase()) {
                                // isvalid = true;
                            }
                            else {
                                Xms.Web.Toptip('"' + $td.attr('data-localizedname') + '" ' + LOC_VALIDATION_REQUIRED_FIELD);
                                if ($input.is('.hide')) {
                                    $($input.attr('data-instance')).get(0).focus();
                                }
                                isvalid = false;
                                return false;
                            }
                        }
                        if (type == 'float' || type == 'money') {
                            value = value.replace(/\,/g, '');
                        }
                        obj.data[attrname] = value;
                    }
                });
                if (primarykey) {
                    obj.data.primarykey = primarykey;
                }
                //console.log('isvalid',isvalid);
                if (!isvalid) { flag = false; return false; }
                obj.data.id = id;

                if (GridViewModel.relationshipname != '' && !obj.data[GridViewModel.referencingattributename]) {
                    obj.data[GridViewModel.referencingattributename] = GridViewModel.referencedrecordid;
                }
                // console.log(obj);
                obj.name = GridViewModel.entityname;
                obj.relationshipname = GridViewModel.relationshipname;
                obj.rownumber = rownumber;
                if (obj.data.id) {
                    //console.log('obj.data.id',obj.data.id)
                    GridViewModel.removerecord(obj.data.id);
                    // console.log(GridViewModel.getrecords())
                    GridViewModel.addrecord(obj);
                }
            });
            //console.log('flag',flag);
            if (flag == false) {
                // Xms.Web.Alert(false,'请填写相关信息');
                return false;
            }
            //只提交填写过的行
            if (!$(GridViewModel.sectionid).find('input[name=tempdata]').val() || $(GridViewModel.sectionid).find('input[name=tempdata]').val() == "") {
                var childArr = [];
                $(GridViewModel.sectionid).find('input[name="resdata"]').val('');
                return true;
            } else {
                var childArr = JSON.parse(decodeURIComponent($(GridViewModel.sectionid).find('input[name=tempdata]').val()));
            }
            console.log('只提交填写过的行', childArr);
            var res = $.grep(childArr, function (obj, key) {
                var flag = false;
                //console.log(obj)
                rows.each(function () {
                    var recordid = $(this).find('input[name="recordid"]').val();
                    //console.log('xxxx',obj.data.id,recordid)
                    if (obj.data.id == recordid) {
                        if (obj.data.primarykey) {
                            obj.data[obj.data.primarykey.toLowerCase()] = obj.data.id;
                        }
                        delete obj.data.id;
                        flag = true;
                        return false;
                    }
                });
                return flag;
            });
            var resultValue = encodeURIComponent(JSON.stringify(res));
            console.log('每个单据体里的数据', resultValue);
            $(GridViewModel.sectionid).find('input[name="resdata"]').val(resultValue);
            return true;
        }
        GridViewModel.saverow = function ($row) {
            console.log('saverow');
            //var $row = $(GridViewModel.sectionid + " #datatable").find('tbody>tr.editrow');
            var obj = { entityid: GridViewModel.entityid, data: {} };
            var id = $row.find('input[name=recordid]').val();
            var isvalid = true;
            var flag = true;
            $row.find('td[data-name]').each(function (i, n) {
                var $td = $(this);
                var attrname = $td.attr('data-name');
                if (!attrname) return true;
                var type = $td.attr('data-type');
                if ($td.attr('data-entityname').toLowerCase() == GridViewModel.entityname.toLowerCase()) {
                    //console.log(GridViewModel.nameprefix+attrname);
                    var $input = $td.find('input[name="' + GridViewModel.nameprefix + attrname + '"]');
                    var value = $input.val();
                    //console.log($input, value);
                    if (value == '' && $td.attr('data-isrequired')) {
                        //referenced record
                        if (GridViewModel.createmode == 'local' && GridViewModel.relationshipname != '' && attrname.toLowerCase() == GridViewModel.referencingattributename.toLowerCase()) {
                            isvalid = true;
                        }
                        else {
                            Xms.Web.Toptip('"' + $td.attr('data-localizedname') + '" ' + LOC_VALIDATION_REQUIRED_FIELD);
                            if ($input.is('.hide')) {
                                $($input.attr('data-instance')).get(0).focus();
                            }
                            isvalid = false;
                            return false;
                        }
                    }
                    obj.data[attrname] = value;
                }
            });
            if (!isvalid) return isvalid;
            obj.data.id = id;
            //referenced recordid
            if (GridViewModel.relationshipname != '' && !obj.data[GridViewModel.referencingattributename]) {
                obj.data[GridViewModel.referencingattributename] = GridViewModel.referencedrecordid;
            }
            console.log(obj);
            obj.name = GridViewModel.entityname;
            obj.relationshipname = GridViewModel.relationshipname;
            if (GridViewModel.createmode == 'local') {
                console.log('local add');
                var $newrow = GridViewModel.editrowmodel.clone(true);
                $row.find('button[name=cancelRowBtn]').removeClass('hide');
                //$row.find('td[data-isrequired="required"]').each(function(i, n){
                //    $(n).find('input').addClass('required');
                //});
                if (obj.data.id) {
                    GridViewModel.removerecord(obj.data.id);
                    GridViewModel.addrecord(obj);
                }
                else {
                    obj.data.id = Xms.Utility.Guid.NewGuid().ToString();
                    $row.find('input[name=recordid]').val(obj.data.id);
                    $(GridViewModel.sectionid + " .datatable").find('tbody').append($row);
                    $(GridViewModel.sectionid + " .datatable").find('tbody').prepend($newrow);
                    GridViewModel.addrecord(obj);
                }
            }
            else {
                obj.data = JSON.stringify(obj.data);
                if (id) {
                    Xms.Web.Post('/api/data/update', obj, false, function (response) {
                        Xms.Web.Toptip(response.content);
                        if (response.IsSuccess) GridViewModel.rebind();
                    });
                }
                else {
                    Xms.Web.Post('/api/data/create', obj, false, function (response) {
                        Xms.Web.Toptip(response.content);
                        if (response.IsSuccess) GridViewModel.rebind();
                    });
                }
            }
            return isvalid;
        }
        GridViewModel.editrow = function ($row) {
            //remove all edit row
            $(GridViewModel.sectionid + " .datatable").find('tbody>tr.editrow').remove();
            $(GridViewModel.sectionid + " .datatable").find('tbody>tr').removeClass('hide');
            var $editrow = GridViewModel.editrowmodel.clone(true);
            //get record
            var id = $row.find('input[name=recordid]').val();
            $editrow.find('input[name=recordid]').val(id);
            Xms.Web.GetJson('/api/data/retrieve/' + GridViewModel.entityname + '/' + id, null, function (response) {
                var data = response.content;
                console.log(data);
                $editrow.find('td[data-name]').each(function (i, n) {
                    var $td = $(this);
                    var attrname = $td.attr('data-name').toLowerCase();
                    if (!attrname) return true;
                    var type = $td.attr('data-type').toLowerCase();
                    if ($td.attr('data-entityname').toLowerCase() == GridViewModel.entityname.toLowerCase()) {
                        var $input = $td.find('input[name="' + GridViewModel.nameprefix + attrname + '"]');
                        if (type == 'bit' || type == 'picklist' || type == 'state') {
                            $input.val(data[attrname]);
                            $td.find($input.attr('data-instance')).attr('data-value', data[attrname]);
                            Xms.Web.SelectedValue($td.find($input.attr('data-instance')), data[attrname]);
                        }
                        else if (type == 'lookup' || type == 'owner' || type == 'customer') {
                            $input.val(data[attrname]);
                            $td.find('input[name=' + $input.prop('name') + '_text]').val(data[attrname + 'name']);
                        }
                        else if (type == 'nvarchar' || type == 'money' || type == 'int') {
                            $input.val(data[attrname]);
                        }
                        $input.attr('data-value', data[attrname]);
                    }
                    if (GridViewModel.relationshipname != '' && GridViewModel.relationshipname.toLowerCase().indexOf(attrname) > 0) {
                        $td.find('button').prop('disabled', 'disabled');
                    }
                });
                $editrow.find('button[name=cancelRowBtn]').removeClass('hide');
                $row.addClass('hide').after($editrow);
            });
            //$row.preventDefault();
        }
        GridViewModel.removeRow = function ($row) {
            var id = $row.find('input[name="recordid"]').val();
            Xms.Web.Del(id, '/api/data/delete?entityid=' + GridViewModel.entityid, false, function () { GridViewModel.rebind('/entity/rendergridview'); });
        }
        GridViewModel.removeRowBtnLocal = function ($row) {
            $row.remove();
        }

        GridViewModel.getrecords = function () {
            var childData = $(GridViewModel.sectionid).find('input[name=tempdata]').val();
            var childRecords = [];
            if (childData && childData != '') {
                childRecords = JSON.parse(decodeURIComponent(childData));
            }
            //console.log(childRecords);
            return childRecords;
        }
        GridViewModel.addrecord = function (obj) {
            var childRecords = GridViewModel.getrecords();
            childRecords.push(obj);
            GridViewModel.setrecords(childRecords);
        }
        GridViewModel.removerecord = function (id) {
            var childRecords = GridViewModel.getrecords();
            childRecords = $.grep(childRecords, function (n, i) {
                var d = n.data;
                return d.id != id;
            });
            console.log(childRecords);
            GridViewModel.setrecords(childRecords);
        }
        GridViewModel.setrecords = function (childRecords) {
            $(GridViewModel.sectionid).find('input[name=tempdata]').val(encodeURIComponent(JSON.stringify(childRecords)));
        }
        function getrecords() {
            var childData = $(GridViewModel.sectionid).find('input[name=tempdata]').val();
            var childRecords = [];
            if (childData && childData != '') {
                childRecords = JSON.parse(decodeURIComponent(childData));
            }
            //console.log(childRecords);
            return childRecords;
        }

        function setrecords(childRecords) {
            $(GridViewModel.sectionid).find('input[name=tempdata]').val(encodeURIComponent(JSON.stringify(childRecords)));
        }
        //GridViewModel.editable();
        //GridViewModel.sumtable();

        function getcheckFunc(input, type) {
            var $input = $(input);
            var stype = type || $input.attr('data-controltype');
            if (stype == 'nvarchar') {
                var format = $input.attr('data-dataformat');
                stype = (format && format != "") ? format : stype;
            }
            var errmsg = '输入格式不正确';
            if (stype == 'float') {
                stype = 'float';
                errmsg = '输入格式不正确';
            } else if (stype == 'email') {
                stype = 'email';
                errmsg = '邮箱格式不正确';
            } else if (stype == 'url') {
                stype = 'url';
                errmsg = '链接格式不正确';
            } else if (stype == 'int') {
                stype = 'integer';
                errmsg = '请输入整数';
            } else if (stype == 'money') {
                stype = 'money';
                errmsg = '输入格式不正确';
            } else if (stype == 'isnull') {
                stype = 'isnull';
                errmsg = '不能为空';
            } else if (stype == 'datetime') {
            }
            var checkfun = Xms.Web.ValidData(stype);
            if (!checkfun) return false;
            if ($input.data().msgbox) {
                $input.data().msgbox.hide();
            }
            return { func: checkfun, msg: errmsg, type: stype };
        }

        function checkEntityType(input) {
            var $input = $(input);
            var tdParent = $input.parents('td:first');
            var value = $input.val();
            var checkobj;
            Xms.Web.Console('data-isrequired', tdParent.attr('data-isrequired'))
            if (tdParent.attr('data-isrequired') == "required" && value == "") {
                if (!input.prop('disabled')) {
                    checkobj = getcheckFunc(input, 'isnull');
                }
            } else {
                if (!input.prop('disabled')) {
                    checkobj = getcheckFunc(input);
                }
            }
            Xms.Web.Console('checkobj', checkobj)
            if (!checkobj) return true;

            var checkfun = checkobj.func;
            var msg = checkobj.msg;
            var type = checkobj.type;
            if (type == 'float' || type == "money") {
                value = value.replace(/\,/g, '');
            }
            var flag = checkfun(value);
            if (value == "") {
                flag = !flag;
            }
            if (!flag) {
                showInErrorMsg($input, msg);
            }
            return flag;
        }

        function showInErrorMsg($input, msg) {
            var msgbox = null;
            $input.parent().css('position', "relative");
            if ($input.data().msgbox && $input.data().msgbox.length > 0) {
                msgbox = $input.data().msgbox;
                msgbox.show();
            } else {
                msgbox = $('<div class="inputErrorMsgBox"><em class="glyphicon glyphicon-exclamation-sign"></em><span class="inputErrorMsg"></span></div>');
                msgbox.appendTo($input.parent());
                $input.data().msgbox = msgbox
            }
            var bound = {
                x: $input.position().left,
                y: $input.position().top,
                w: $input.outerWidth(),
                h: $input.outerHeight()
            }
            msgbox.find('.inputErrorMsg').text(msg);
            msgbox.css({ "top": bound.y - bound.h, "left": bound.x });
        }

        function hideInErrorMsg($input) {
            if ($input.length > 0) {
                msgbox = $('.inputErrorMsgBox');
                msgbox.hide();
            }
        }

        function triggerNextInputFocus(cur) {
            if (Xms.Page && !Xms.Page.subgridIsTriggerNext) return false;
            var $this = $(cur);
            if ($this.length == 0) return false;
            var value = $this.val();
            if ($this.hasClass('isvalidata')) {
                var isChecke = checkEntityType($this);
                if (isChecke == false) return false;
            }
            var curTd = $this.parents('td:first');
            var curTr = curTd.parent();

            var nextTd = curTd.next();
            var nextTr = curTr.next();
            // console.log('triggerNext',curTd);
            if (nextTd.length > 0) {
                //
                var trigInput = nextTd.find('input[data-isedit="true"]');
                var type = trigInput.attr('data-type');
                if (type == 'lookup' || type == 'owner') {
                    var lookupbtn = trigInput.prev().find('button[name="lookupBtn"]');
                    if (lookupbtn.length > 0 && !lookupbtn.prop('disabled')) {
                        if ($('#entityRecordsModal').length > 0) {
                            $('body').removeClass('modal-open');
                            setTimeout(function () {
                                lookupbtn.focus();
                                lookupbtn.trigger('click');
                            }, 500);
                        } else {
                            lookupbtn.focus();
                            lookupbtn.trigger('click');
                        }
                        return false;
                    } else {
                        triggerNextInputFocus(nextTd.find('input'));
                        return false;
                    }
                } else if (type == 'picklist') {
                    var selector = trigInput.siblings('select');
                    if (selector.length > 0 && !selector.prop('disabled')) {
                        Xms.Web.Console('selector', selector)
                        setTimeout(function () { selector.trigger('focus'); }, 0);
                    } else {
                        triggerNextInputFocus(nextTd.find('input'));
                    }
                } else {
                    if (trigInput.length > 0 && !trigInput.prop('disabled')) {
                        setTimeout(function () {
                            trigInput.focus();
                            trigInput.get(0).select();
                        }, 0);
                    } else {
                        triggerNextInputFocus(nextTd.find('input'));
                    }
                }
            } else {
                if (nextTr) {
                    triggerNextInputFocus(nextTr.find('input:first'));
                }
            }
        }
        function settableheaderWidth() {
            var editFormTable = $(GridViewModel.sectionid + " .datatable");
            var ths = editFormTable.find('th.tableHeaderItem');
            var widths = 0;
            ths.each(function (key, item) {
                var _w = $(item).attr('data-width') * 1;
                widths += _w;
            });
            $(GridViewModel.sectionid + ' .tableHeaderWidth').val(widths);
        }
        function settableHeaderWidth() {
            var datatable = $(GridViewModel.sectionid + " .datatable");
            settableheaderWidth();
            var tableW = $(GridViewModel.sectionid + ' .tableHeaderWidth').val() * 1;
            var wrapW = $(GridViewModel.sectionid + " .tableReWidth").eq(0).parents('.panel-collapse:first').width();
            if (tableW >= wrapW) {
                datatable.width(tableW)
            } else {
                datatable.width('100%');
            }
        }
        function checkSubGridData() {
            var inputs = $(GridViewModel.sectionid).find('tr[data-edited="true"] input.isvalidata');
            Xms.Web.Console('inputs', inputs);
            var flag = true;
            inputs.each(function () {
                var $input = $(this);
                var value = $input.val();
                var format = $input.attr('data-dataformat');
                var type = $input.attr('data-type');
                var $td = $input.parents('td:first');
                var isrequired = $td.attr('data-isrequired');
                Xms.Web.Console('isChecke', isrequired, type)
                if (((type == "nvarchar" && !!format) || type == "float" || type == "int" || type == "money")) {
                    var isChecke = checkEntityType($input);
                    if (isChecke == false) {
                        flag = false;
                        return false;
                    }
                } else if (isrequired == "required") {
                    if (type == 'lookup' || type == 'owner' || type == "customer") {
                        var hiddenid = $input.attr('id').replace('_text', '');
                        var $hidden = $('#' + hiddenid);
                        if ($hidden.length > 0) {
                            var isChecke = checkEntityType($hidden, 'isnull');
                            if (isChecke == false) {
                                flag = false;
                                return false;
                            }
                        }
                    } else {
                        var isChecke = checkEntityType($input, 'isnull');
                        if (isChecke == false) {
                            flag = false;
                            return false;
                        }
                    }
                }
            });
            return flag;
        }

        function saveCurrentSubGrid() {
            var check = checkSubGridData();
            if (!check) return false;
            var flag = true;
            sfl = GridViewModel.savetable();
            if (sfl == false) {
                flag = false;
            }
            if (flag) {
                var res = $(GridViewModel.sectionid).find('input[name="resdata"]').val();
                console.log('提交单据体的数据', res);
                Xms.Web.Post('/api/data/save/savechilds', {
                    child: res,
                    entityname: GridViewModel.entityname,
                    parentid: Xms.Page.PageContext.RecordId
                }, function () { }, function (response) {
                    $(document).trigger('subgrid.save', { data: response });
                    GridViewModel.rebind(null, function () {
                        console.log($(GridViewModel.sectionid).parents('.subgrid:first'))
                        if (typeof setSubGridFormular == 'function') {
                            setSubGridFormular($(GridViewModel.sectionid).parents('.subgrid:first'), 'gridview');
                        }
                    });
                    console.log(response);
                });
            }
            return flag;
        }

        function assignSubGrid(data, auto, isCover) {
            if (!data || !$.isArray(data)) { console.log('data必须为数组格式', data); return false; }
            if (data.length == 0) { console.log('data', data); return false; }
            var currentSubgird = $(GridViewModel.sectionid), _table = currentSubgird.find('table.datatable:first');
            var thead_th = _table.children('thead').find('tr>th');
            var noEditTr = _table.children('tbody').find('tr:gt(0):first');
            var relationshipname = GridViewModel.relationshipname;
            Xms.Web.Console('testConsole', data);
            $.each(data, function (key, item) {
                while (noEditTr) {
                    Xms.Web.Console('iscover', isCover && noEditTr.attr('data-edited'));
                    if (noEditTr.attr('data-isinit')) {
                        noEditTr = noEditTr.next();
                    } else if (isCover && noEditTr.attr('data-edited')) {
                        noEditTr = noEditTr.next();
                    } else {
                        break;
                    }
                }
                console.log('noEditTr.length', noEditTr.length);
                if (noEditTr.length == 0) {//如果没有新的行，就新建一行
                    noEditTr = renderDefaultItems(1);
                    GridViewModel.ajaxgrid_reset(1, true);
                    if (typeof setSubGridFormular == 'function') {
                        setSubGridFormular($(GridViewModel.sectionid).parents('.subgrid:first'), 'gridview');
                    }
                    Xms.Web.Console('noEditTr', noEditTr);
                }
                for (var i in item) {
                    if (item.hasOwnProperty(i)) {
                        Xms.Web.Console('noEditTr', noEditTr);
                        noEditTr.find('td').each(function (ii, nn) {
                            var $nn = $(nn);
                            var index = $nn.index();
                            var attrname = thead_th.eq(index).attr('data-name');
                            var itemAttr = i;
                            if (relationshipname && relationshipname != '') {
                                attrname = relationshipname + '_' + attrname;
                                if (i.indexOf(relationshipname) == -1) {//防止在有关联的时候传进来的数据不带关联名字
                                    itemAttr = relationshipname + '_' + i;
                                }
                            }
                            var _input = $nn.children('input');
                            //Xms.Web.Console('attrname',attrname,_input);
                            if (attrname && _input) {
                                var _inputName = _input.attr('name');
                                if (attrname.toLowerCase() == itemAttr) {
                                    Xms.Page.setValueByContext(noEditTr, _inputName, item[i], item, relationshipname);
                                }
                            }
                        });
                        noEditTr.attr('data-edited', true);
                    }
                }
                noEditTr = noEditTr.next();
            });
            if (auto) {
                if (formSaveSubGrid()) {
                    Save();
                }
            }
        }

        function setAttributeState(name, state, callback) {
            if (!name) return false;
            var currentSubgird = $(GridViewModel.sectionid), _table = currentSubgird.find('table.datatable:first');
            var thead_th = _table.children('thead').find('tr>th');
            var noEditTr = _table.children('tbody').find('tr:gt(0)');
            var res = [];
            noEditTr.each(function () {
                var $td = $(this).find('td[data-name="' + name + '"]');
                if ($td.length > 0) {
                    Xms.Web.Console('$td', $td);
                    $td.find('input,button,select').prop('disabled', state || false);
                    res.push($td);
                }
            });
            callback && callback(currentSubgird, res);
        }
        function attribueBindEvent(name, event, callback, isreplace) {
            if (!name) return false;
            var currentSubgird = $(GridViewModel.sectionid), _table = currentSubgird.find('table.datatable:first');
            var thead_th = _table.children('thead').find('tr>th');
            var noEditTr = _table.children('tbody').find('tr:gt(0)');
            var res = [];
            Xms.Web.Console('noEditTr', noEditTr);
            noEditTr.each(function () {
                var $_input = $(this).find('input[data-name="' + name + '"]');
                var _index = $(this).index() - 1;
                if ($_input.length > 0) {
                    if (isreplace) {
                        $_input.off(event).on(event, function (e) {
                            var subgridData = GridViewModel.getTableData();
                            var trData = subgridData;
                            Xms.Web.Console('subgridData', subgridData);
                            var opts = { input: $_input, tr: $(this), trIndex: _index, trData: trData, tbody: noEditTr, subgrid: currentSubgird };
                            callback && callback.call(this, e, opts);
                        });
                    } else {
                        $_input.on(event, function (e) {
                            var subgridData = GridViewModel.getTableData();
                            var trData = subgridData;
                            Xms.Web.Console('subgridData', subgridData);
                            var opts = { input: $_input, tr: $(this), trIndex: _index, trData: trData, tbody: noEditTr, subgrid: currentSubgird };
                            callback && callback.call(this, e, opts);
                        });
                    }
                }
            });
        }
        function getSubGridIsEdit(name) {
            if (!name) return false;
            var currentSubgird = $(GridViewModel.sectionid), _table = currentSubgird.find('table.datatable:first');
            var thead_th = _table.children('thead').find('tr>th');
            var noEditTr = _table.children('tbody').find('tr:gt(0)');
            var res = [];
            var isedit = false;
            noEditTr.each(function () {
                var isedited = $(this).attr('data-isedited');
                Xms.Web.Console('inputIsEdit', isedited, $(this));
                if (isedited && isedited == true) {
                    isedit = true;
                    return false;
                }
            });
            return isedit;
        }

        if (typeof window.GridViewModel == 'undefined') {
            window.GridViewModel = {};
        }
        window.GridViewModel[GridViewModel.sectionid] = GridViewModel;
        window.checkSubGridData = checkSubGridData;
        if (typeof window.GridViewModelObject === 'undefined') {
            window.GridViewModelObject = {};
        }
        window.GridViewModelObject[GridViewModel.gridid] = {};
        window.GridViewModelObject[GridViewModel.gridid].gridModel = GridViewModel;
        window.GridViewModelObject[GridViewModel.gridid].renderDefaultItems = renderDefaultItems;
        window.GridViewModelObject[GridViewModel.gridid].assignSubGrid = assignSubGrid;
        window.GridViewModelObject[GridViewModel.gridid].setAttributeState = setAttributeState;
        window.GridViewModelObject[GridViewModel.gridid].attribueBindEvent = attribueBindEvent;
        window.GridViewModelObject[GridViewModel.gridid].getSubGridIsEdit = getSubGridIsEdit;
    })();

    return pageWrap_RenderGirdView;
});