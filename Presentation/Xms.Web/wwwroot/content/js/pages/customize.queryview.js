//@ sourceURL=pages/customize.queryview.js
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
});

//获取表单列表
function getFormsList(entityid, $context, isRequire, callback) {
    //加载forms列表
    var postParams = {
        type: 'forms' + entityid,
        data: { entityid: entityid,loaddata:true }
    }
    Xms.Web.PageCache('workflow', '/customize/systemform/index', postParams, function (res) {
        var resItems = res.content;
        if (resItems && resItems.items) {
            var _html = [];
            if (isRequire == false) {
                _html.push('<option data-formid="" data-solutionid="" value=""></option>');
            }
            $.each(resItems.items, function (i, n) {
                _html.push('<option data-formid="' + n.systemformid + '" data-solutionid="' + n.solutionid + '" value="' + n.systemformid + '">' + n.name + '</option>');
            });
            $context.html(_html.join(''));
            callback && callback();
        }
    }, true);
}

var AggregateType = { 'datetime': ['Max', 'Min'], 'int': ['Sum', 'Avg', 'Max', 'Min'], 'float': ['Sum', 'Avg', 'Max', 'Min'], 'money': ['Sum', 'Avg', 'Max', 'Min'] };
//打开窗口时获取是否需要统计
function getAggregateConfig(type) {
    var $columnstatistical = $('#columnstatistical');

    var attrtype = AggregateType[type];
    if (!attrtype) {
        $columnstatistical.parent().hide();
        return false;
    }
    $columnstatistical.parent().show();
    $columnstatistical.children().hide();
    $columnstatistical.children('option').each(function (i, n) {
        $.each(attrtype, function (ii, nn) {
            if (nn == n.value) {
                $(n).show();
            }
        });
    });
    $columnstatistical.children('option:first').show();
}
//打开窗口时统计de值
function setAggregateVal(name) {
    var $columnstatistical = $('#columnstatistical'), $AggregateConfig = $('#AggregateConfig');
    var $this = $('.selected:first', '#views');
    var relationship = $this.attr('data-relationship');
    var _value = $AggregateConfig.val(), tempArr = [], res = null;
    if (_value != '') {
        tempArr = JSON.parse(_value);
    }
    if (relationship) {
        name = relationship.toLowerCase() + '_' + name;
    }
    $.each(tempArr, function (key, item) {
        if (item['AttributeName'].toLowerCase() == name.toLowerCase()) {
            res = item['AggregateType'];
            return false
        }
    });
    console.log(res);
    $columnstatistical.children('option:first').prop('selected', true);
    if (res) {
        $columnstatistical.val(res);
    }
}
//关闭窗口时设置统计de值
function setAggregateConfig(name, value) {
    var $AggregateConfig = $('#AggregateConfig'), res = '';
    var $this = $('.selected:first', '#views');
    var relationship = $this.attr('data-relationship');
    _value = $AggregateConfig.val(), tempArr = [];
    var _flag = false;
    if (relationship) {
        name = relationship.toLowerCase() + '_' + name;
    }
    if (_value != '') {
        tempArr = JSON.parse(_value);
        if (value != '') {
            $.each(tempArr, function (key, item) {
                if (item['AttributeName'].toLowerCase() == name.toLowerCase()) {
                    item['AggregateType'] = value;
                    _flag = true;
                    return false
                }
            });
            if (_flag == false) {
                var _obj = {};
                _obj['AttributeName'] = name;
                _obj['AggregateType'] = value;
                tempArr.push(_obj);
            }
        } else {
            var temp = null;
            $.each(tempArr, function (key, item) {
                if (item['AttributeName'].toLowerCase() == name.toLowerCase()) {
                    temp = key;
                    _flag = true;
                    return false
                }
            });
            if (_flag == true) {
                tempArr.splice(temp, 1);
            }
        }
    } else {
        if (value != '') {
            var _obj = {};
            _obj['AttributeName'] = name;
            _obj['AggregateType'] = value;
            tempArr.push(_obj);
        }
    }
    console.log(tempArr);
    res = JSON.stringify(tempArr);
    if (res == '[]') {
        res = '';
    }
    $AggregateConfig.val(res);
}

function renderModityByData(data, type) {
    var _html = [];
    $.each(data, function (i, n) {
        var flag = false;
        n['rolesid'] = n['roleid'];
        _html.push('<option class="list-group-item" data-entityname="type" value="' + n['rolesid'] + '" data-value="' + n['rolesid'] + '">');
        _html.push(n.name);
        _html.push('</option>');
    });
    return _html.join('');
}
function settableheaderWidth() {
    var editFormTable = $("#editFormTable");
    var ths = editFormTable.find('th.tableHeaderItem');
    var widths = 0;
    ths.each(function (key, item) {
        var _w = $(item).attr('data-width') * 1;
        widths += _w;
    });
    $('#tableheaderWidth').val(widths);
}

function resetTableWidth() {
    settableheaderWidth();
    var tableW = $('#tableheaderWidth').val() * 1;
    var wrapW = $('#tableReWidth').width();//
    console.log(tableW, wrapW);
    if (tableW >= wrapW) {
        $("#editFormTable").width(tableW)
    } else {
        $("#editFormTable").width('auto');
    }
}

function resizeTableHead() {
    //

    $("#editFormTable").tableHdResize({
        showTips: true
        , onResizeEnd: function (th) {
            var w = th.outerWidth();
            th.attr("data-width", w);
            $('#columnwidth').val(w);
            resetTableWidth();
            saveGridConfig();
        }
        , resetTableWidth: false
    });
}
//判断字段是否已加入列表
function attributeIsExists(entityName, attrName, relationship) {
    entityName = entityName.toLowerCase();
    attrName = attrName.toLowerCase();
    //是否为关联实体字段
    if (relationship) {//(entityName != QueryObject.EntityName.toLowerCase()) {
        //是否已存在
        var le = null;
        $(QueryObject.LinkEntities).each(function (ii, nn) {
            if (nn.LinkToEntityName.toLowerCase() == entityName) {
                le = nn;
                return false;
            }
        });
        if (le != null) {
            if (~$.inArray(attrName, le.Columns.Columns)) {
                if (relationship == le.EntityAlias) {
                    return true;
                }
            }
        }
    }
    else {
        if (~$.inArray(attrName, QueryObject.ColumnSet.Columns)) {
            return true;
        }
    }
    return false;
}
//加载实体，两个字段同时引用同一实体时？
function loadEntities() {
    var dfd = $.Deferred();
    var entityid = $('#EntityId').val();
    Xms.Schema.GetEntityRelations(entityid, null, function (data) {
        if (!data) return;
        var _htmls = [];
        $(data).each(function (i, n) {
            //
            var _html = '<option data-relationship="' + n.name + '" data-entityid="' + n.entityid + '" data-referencingattributelocalizedname="' + n.referencingattributelocalizedname + '" data-referencingattributename="' + n.referencingattributename + '" value="' + n.referencedentityid + '">' + n.referencingattributelocalizedname + '(' + n.referencedentitylocalizedname + ')' + '</option>';
            _htmls.push(_html);
        });
        var entityhtml = $('#entitydefaultoption').html();
        $('#entities').html(entityhtml + _htmls.join(''));
        // console.log('_html',_html);

        // Xms.Web.SelectedValue($('#entities'), entityid);
        $('#entities').find('option:first').prop('selected', true);
        dfd.resolve();
    });
    return dfd.promise();
}
var rowcommondAttributes = null;
//加载字段
function loadAttributes(callback) {
    var entityid = Xms.Web.SelectedValue($('#entities')) || $('#EntityId').val();
    var relationship = $('#entities').find('option:selected').attr('data-relationship') || '';
    var referencingattributelocalizedname = $('#entities').find('option:selected').attr('data-referencingattributelocalizedname') || '';
    var _html = new Array();
    Xms.Schema.GetAttributesByEntityId(entityid, function (data) {
        if (!data) return;
        console.log('data.content.items', data);
        if (rowcommondAttributes == null) {
            rowcommondAttributes = data;
        }
        var attributename = $('#entities option:selected').attr('data-referencingattributename')
        $(data).each(function (i, n) {
            //console.log(n.entityname, n.name, attributeIsExists(n.entityname, n.name));
            if (attributeIsExists(n.entityname, n.name, relationship) == false) {
                _html.push('<li class="mutil-list-item" data-relationship="' + relationship + '" data-referencingattributelocalizedname="' + referencingattributelocalizedname + '" data-type="' + n.attributetypename + '" data-referencdentityname="' + n.referencedentityname + '" data-name="' + n.name + '" data-entityname="' + n.entityname + '" data-referencdattributename="' + (attributename || '') + '" data-entitylocalizedname="' + n.entitylocalizedname + '" data-referencedentityid="' + n.referencedentityid + '" data-attributesid="' + (n.attributeid || "") + '" data-localizedname="' + n.localizedname + '" data-optionsetid="' + (n.optionsetid || "") + '">');
                _html.push('<span class="glyphicon glyphicon-screenshot"></span>');
                _html.push('<span class="th-label">' + n.localizedname + '</span>');
                _html.push('</li>');
            }
        });
        $('#attributes').html(_html.join(''));
        callback && callback();
    });
}
//加载过滤条件字段选项
function loadFilterAttributes() {
    var entityid = Xms.Web.SelectedValue($('#entities')) || $('#EntityId').val();
    var _mainOptions = new Array(), _relatedOptions = new Array();
    Xms.Schema.GetAttributesByEntityId(entityid, function (data) {
        if (!data) return;
        _mainOptions.push('<optgroup label="字段">');
        console.log(data)
        $(data).each(function (i, n) {
            _mainOptions.push('<option value="' + n.name + '" data-entityid="' + n.entityid + '" data-entityname="' + n.entityname + '" data-type="' + n.attributetypename + '">' + n.localizedname + '</option>');
        });
        _mainOptions.push('</optgroup>');
        Xms.Schema.GetEntityRelations(entityid, null, function (data) {
            if (!data) {
                $('#filternamelist').append($(_mainOptions.join('') + _relatedOptions.join('')));
                $('#filterConditions > tfoot > tr:first').find('select[name=filtername]').append($(_mainOptions.join('') + _relatedOptions.join('')));
                return;
            }
            Relationships = data;
            _relatedOptions.push('<optgroup label="关联">');
            $(data).each(function (ii, nn) {
                _relatedOptions.push('<option value="' + nn.referencedattributename + '" data-entityname="' + nn.referencedentityname + '" data-type="' + nn.referencedattributetypename + '">' + nn.referencingattributelocalizedname + '(' + nn.referencedentitylocalizedname + ')' + '</option>');
            });
            _relatedOptions.push('</optgroup>');
            $('#filternamelist').append($(_mainOptions.join('') + _relatedOptions.join('')));
            $('#filterConditions > tfoot > tr:first').find('select[name=filtername]').append($(_mainOptions.join('') + _relatedOptions.join('')));
        });
    });
}
//加载操作符
function loadOperators(target, type) {
    target.empty();
    var operators = Xms.Fetch.ConditionOperators[type];
    //操作符
    var op = new Array();
    $(operators).each(function (i, n) {
        op.push('<option data-value="' + n[0] + '" value="' + n[1] + '">' + n[2] + '</option>');
    });
    target.append(op.join(''));
}
function getDdlItems(datatype) {
    var items = [];
    if (datatype == 'picklist') {
        items = [{ value: 0, text: '已提交' }, { value: 1, text: '审核中' }];
    } else if (datatype == 'state') {
        items = [{ value: 0, text: '否' }, { value: 1, text: '是' }];
    } else if (datatype == 'bit') {
        items = [{ value: 0, text: '否' }, { value: 1, text: '是' }];
    }
    return items;
}
//字段选择后更改操作符
function onChangeAttribute(e) {
    var $this = $(e);
    var target = $this.parents('tr').find('select[name=filteroperator]');
    var type = $this.find('option:selected').attr('data-type');
    loadOperators(target, type);
    target.change();
}
//列移动排序
function moveColumn(direction) {
    var $this = $("#views").find('.selected');
    if (direction == 'before' && $this.prev().length > 0) {
        $this.insertBefore($this.prev());
    }
    else if (direction == 'after' && $this.next().length > 0) {
        $this.insertAfter($this.next());
    }
    saveGridConfig();
}
//删除列
function removeColumn() {
    var $this = $("#views").find('.selected');
    var index = $this.index();
    console.log(index);
    var delSelf = $("#views").find('th:eq(' + index + ')');
    var delOneSort = false;
    var attrname = $this.attr('data-name');
    var attrrelashipname = $this.attr('data-relationship');
    if ($('#views').find('th').length > 1) {
        if (delSelf.hasClass('shadow')) {
            index = -1;
        }
        //还原左边列表start
        var attrEntities = $('#entities').find('option:selected');
        var FRelationship = attrEntities.attr('data-relationship').toLowerCase();
        var TRelationship = delSelf.attr('data-relationship').toLowerCase();
        for (var i = 0; i < AttributesList.length; i++) {
            if (AttributesList[i].name.toLowerCase() == delSelf.attr('data-name').toLowerCase() && AttributesList[i].relationship.toLowerCase() == TRelationship.toLowerCase()) {
                if (FRelationship == TRelationship) {
                    $(AttributesList[i].html).find('span').removeClass('hide');
                    $('#attributes').append(AttributesList[i].html);
                }
                AttributesList.splice(i, 1);
                break;
            }
        }
        //还原左边列表end
        //排序设置start
        if (typeof delSelf.attr('data-sorttype') != 'undefined' && typeof delSelf.attr('data-sortnum') != 'undefined' && delSelf.attr('data-sortnum') == 'one') {
            delOneSort = true;
        }
        //排序设置end
        delSelf.remove();
        if (!$("#views").parents('table:first').find('tr').find('td:eq(' + index + ')').hasClass('shadow')) {
            $("#views").parents('table:first').find('tr').find('td:eq(' + index + ')').remove();
        }
        saveGridConfig();
        if ($('#views').find('th').length == 1) {
            $("#views").parents('table:first').find('.shadow').removeClass('hide');
        }
        else {
            //如果第一个排序被删除了，默认第一个排序
            if (delOneSort) {
                $('#views').find('th:eq(1)').attr({ 'data-sorttype': 'ascending', 'data-sortnum': 'one' });
            }
        }
    }
    if (attrrelashipname && attrrelashipname != "") {
        attrname = attrrelashipname.toLowerCase() + '_' + attrname;
    }
    setAggregateConfig(attrname, '')
    resetTableWidth();
    saveGridConfig();
    //  addAttributeToInput();
}
//编辑列参数
function editColumnParams() {
    var $this = $("#views").find('.selected');
    var width = $this.attr('data-width');
    var visibled = $this.attr('data-visibled');
    var type = $this.attr('data-type');
    var _name = $this.attr('data-name');
    var attributename = $this.attr('data-localizedname');
    var label = $this.attr('data-label') || $this.attr('data-localizedname');
    //var sorttype = $this.attr('data-sorttype');
    $('#columnModal').find('input[name=columnwidth]').val(width);
    $('#columnModal').find('input[name=columnattributename]').val(attributename);
    $('#columnModal').find('input[name=columnlabel]').val(label).select();
    $('#columnModal').find('input[name=columnvisibled]').prop('checked', visibled);
    getAggregateConfig(type);
    setAggregateVal(_name);
    //Xms.Web.SelectedValue($('#columnModal').find('select[name=sorttype]'), sorttype);
    //$('#columnModal').modal({
    //    keyboard: true
    //})
    $('#columnModal').addClass('in');
    $('#existsButtons').removeClass('in');
}
//保存列参数设置
function saveColumnParams() {
    var $this = $("#views").find('.selected');
    var $columnstatistical = $('#columnstatistical');
    var width = $('#columnModal').find('input[name=columnwidth]').val();
    var visibled = $('#columnModal').find('input[name=columnvisibled]').prop('checked');
    var _name = $this.attr('data-name');
    var type = $this.attr('data-type');
    var label = $('#columnModal').find('input[name=columnlabel]').val();
    var _assValue = $columnstatistical.val();
    //var sorttype = Xms.Web.SelectedValue($('#columnModal').find('select[name=sorttype]'));
    //$this.attr('data-sorttype', sorttype);
    $this.attr('data-width', width);
    $this.attr('data-label', label);
    $this.find('.th-label').text(label);
    $this.attr('data-visibled', visibled);
    $this.attr('style', 'width:' + width + 'px;');
    $('#columnModal').modal('hide');
    $("#views").parents('table').removeClass('table');
    //reset form
    // $('#columnModal').find('input[name=columnwidth]').val('');
    $('#columnModal').find('input[name=columnvisibled]').prop('checked', true);
    Xms.Web.SelectedValue($('#columnModal').find('select[name=sorttype]'), '');
    var attrtype = AggregateType[type];
    if (attrtype) {
        setAggregateConfig(_name, _assValue);
    }

    saveGridConfig();
    $('#columnModal').removeClass('in');
}
//保存列表布局
function saveGridConfig() {
    var $layout = $('#LayoutConfig');
    try {
        if ($layout.val() != "") {
            var grid = JSON.parse($layout.val());
            // if (!grid.SortColumns) {
            //防止新增实体后默认的数据为小写
            if (grid.rows) {
                delete grid.rows;
            }
            grid.Rows = [];
            // }
            //if (!grid.Rows) {
            grid.Rows = [];
            //}
        } else {
            var grid = { SortColumns: [], Rows: [] };
        }
    } catch (e) {
        console.error(e);
    }
    var sortColumn = { Name: '', SortAscending: false };
    var row = { Name: '', Id: '', Cells: [] };
    var cell = { Name: '', EntityName: '', IsHidden: false, IsSortable: false, Width: 0 };
    var sortColumns = new Array(), cells = new Array(), columns = new Array(), orders = new Array();
    QueryObject.LinkEntities = []; // 保存已添加到表格头的字段
    $("#views").find('th:not(.shadow)').each(function (i, n) {
        var column = $(n);
        var name = column.attr('data-name').toLowerCase();
        var entityname = column.attr('data-entityname').toLowerCase();
        var visibled = column.attr('data-visibled');
        var sorttype = column.attr('data-sorttype').toLowerCase();
        var width = column.attr('data-width');
        var label = column.attr('data-label');
        var relationship = column.attr('data-relationship').toLowerCase();
        //关联实体的字段名
        //var referencingattributename = column.attr('data-referencingattributename')? column.attr('data-referencingattributename').toLowerCase():'';

        //是否为关联实体字段
        if (relationship) {//如果为关联实体
            //是否已存在
            var le = null;
            $(QueryObject.LinkEntities).each(function (ii, nn) {
                if (nn.LinkToEntityName.toLowerCase() == entityname && nn.EntityAlias == relationship) {
                    le = nn;
                    return false;
                }
            });
            if (le != null) {
                if ($.inArray(name, le.Columns.Columns) < 0) {//列名不存在
                    le.Columns.Columns.push(name);
                }
                name = le.EntityAlias + '.' + name;//cell's name
            }
            else {
                le = new LinkEntity();//生成关联实体别名
                le.LinkToEntityName = entityname;
                le.EntityAlias = relationship || entityname + '_' + Xms.Utility.Guid.NewGuid().ToString('N');
                le.LinkToAttributeName = entityname + 'id';
                le.LinkFromEntityName = QueryObject.EntityName;
                $(Relationships).each(function (ii, nn) {//from.createdby => to.userid
                    if (nn.name.toLowerCase() == relationship && nn.referencedattributename.toLowerCase() == entityname + 'id') {
                        le.LinkFromAttributeName = nn.referencingattributename.toLowerCase();
                        return false;
                    }
                });
                le.Columns.Columns.push(name);
                QueryObject.LinkEntities.push(le);
                name = le.EntityAlias + '.' + name;//cell's name
            }
        }
        else { columns.push(name); }
        cell = cell.constructor();//js对象为引用类型，所以需要NEW一个对象
        cell.Name = name;
        cell.EntityName = entityname;
        cell.IsHidden = !visibled;
        cell.IsSortable = sorttype != '';
        cell.Width = width;
        cell.Label = label || null;
        cells.push(cell);
        if (sorttype != '') {
            sortColumn = sortColumn.constructor();
            sortColumn.Name = name;
            sortColumn.SortAscending = sorttype == 'ascending';
            sortColumns.push(sortColumn);
            var ord = { AttributeName: name, OrderType: sorttype == 'ascending' ? 0 : 1 };
            orders.push(ord);
        }
    });
    row.Cells = cells;
    grid.SortColumns = sortColumns;
    grid.Rows.push(row);
    var $jslibrary = $('#jslibrary');
    if ($jslibrary.val()) {
        var jslibval = $jslibrary.val();
        var jsarray = jslibval.split(',');
        grid.ClientResources = [];
        $.each(jsarray, function (i,n) {
            grid.ClientResources.push(n);
        });
    } else {
        grid.ClientResources = '';
    }

    //grid.RowCommand = saveRowCommandValue();
    console.log('提交的grid的数据', grid);
    $('#LayoutConfig').val(JSON.stringify(grid));

    QueryObject.ColumnSet.Columns = columns;
    //QueryObject.Orders = orders;
    UpdateQueryObjectOrder();
    console.log(QueryObject.LinkEntities);
    QueryObject.LinkEntities = conforLink(QueryObject.LinkEntities);//合并相关实体
    $('#FetchConfig').val(JSON.stringify(QueryObject));
}
//增加一行过滤条件
function addCondition(e) {
    var newRow = $('#filterConditions > tfoot > tr:first').clone(true).removeClass('hide');
    var fname = Xms.Web.SelectedValue($(e));
    if (fname != null) {
        Xms.Web.SelectedValue(newRow.find('select[name=filtername]'), fname);
    }
    onChangeAttribute(newRow.find('select[name=filtername]').get(0));
    $('#filterConditions > tbody').append(newRow);
    $('#filterConditions > tbody').find('tr:last').find('select[name=filteroperator]').change();
    Xms.Web.SelectedValue($(e), '');
}
//清空过滤条件
function clearConditions() {
    Xms.Web.Confirm('确认', '确定清空所有过滤条件？', function () {
        $('#filterConditions tbody tr:visible').remove();
    });
}
//过滤条件组合
function groupConditions(logical) {
    var rows = $('#filterConditions tbody tr.active');
    if (rows.length < 2) {
        Xms.Web.Alert(false, '请选择2个条件以上');
        return;
    }
    //检测
    //rows.each(function (i, n) {
    //    if (row.is('.filterrow')) {
    //        //如果是组合内，则更改组合符号
    //        if (row.parents('tr.filtergroup').first()) {
    //            row.parents('tr.filtergroup').first().find('td:first').find('.dropdown-menu > li:eq(1)').trigger('click');
    //        }
    //    }
    //    else if (row.is('.filtergroup')) {
    //        removenode.push(row.parents('tr.filterrow'));
    //        var node = $('<tr class="filterrow"><td colspan="4"><table class="table table-condensed"><tbody></tbody></table></td></tr>');
    //        node.find('tbody').append(row);
    //        parent.find('#' + grouplistid).append(node);
    //    }
    //    //如果两个选项跨组合，或跨实体，则取消
    //    //...
    //});
    var container = new Array();
    var grouplistid = 'grouplist_' + Math.round(new Date().getTime() / 1000);
    var insertObj, pos;
    if (rows.first().prev().length == 0) {//第一行
        if (rows.last().next().length > 0) {
            insertObj = rows.last().next();
            pos = 'before';
        }
        else {
            insertObj = rows.first().parent();
            pos = 'append';
        }
    }
    else {
        insertObj = rows.first().prev();
        pos = 'after';
    }
    container.push('<tr class="filterrow"><td colspan="4">');
    container.push('<table class="table table-condensed"><tbody><tr class="filtergroup" data-logical="' + logical + '" data-groupid="' + grouplistid + '">');
    container.push('<td class="bg-info text-primary" style="vertical-align:middle;">');
    container.push('<div class="dropdown"><input type="checkbox" name="filtergroup" />');
    container.push('<a class="dropdown-toggle" data-toggle="dropdown" href="#"><span name="logicaltext">' + (logical == 'and' ? '并且' : '或者') + '</span> <span class="caret"></span></a>');
    container.push('<ul class="dropdown-menu">');
    container.push('<li><a class="btn-link" onclick="javascript:cancelGroup(this,\'' + grouplistid + '\')">取消组合</a></li>');
    container.push('<li><a class="btn-link" onclick="javascript:changeGroupLogical(this)">转换为"' + (logical != 'and' ? '并且' : '或者') + '"组合</a></li>');
    container.push('</ul></div>');
    container.push('</td>');
    container.push('<td colspan="3">');
    container.push('<table class="table table-condensed"><tbody name="grouplist" id="' + grouplistid + '">');
    /********放置条件********/
    container.push('</tbody></table>');
    container.push('</td>');
    container.push('</tr>');
    container.push('</tbody></table>');
    container.push('</td></tr>');
    var parent = $(container.join('\n'));
    var removenode = new Array();
    rows.each(function (i, n) {
        var row = $(n);
        row.removeClass('active').find(':checkbox').prop('checked', false);
        row.find('tr').removeClass('active');
        if (row.is('.filterrow')) {
            parent.find('#' + grouplistid).append(row.clone(true));
            //if (row.parents('tr.filterrow').first()) removenode.push(row.parents('tr.filterrow').first());
            row.remove();
        }
        else if (row.is('.filtergroup')) {
            removenode.push(row.parents('tr.filterrow:first'));
            var node = $('<tr class="filterrow"><td colspan="4"><table class="table table-condensed"><tbody></tbody></table></td></tr>');

            node.find('tbody').empty().append(row.clone(true));
            parent.find('#' + grouplistid).append(node);
            row.remove();
        }
    });
    console.log(pos);
    if (insertObj.is('tr')) {
        if (pos == 'before') insertObj.before(parent);
        else insertObj.after(parent);
        //insertObj.remove();
        $(removenode).each(function (i, n) { $(n).remove(); });
    }
    else {
        insertObj.append(parent);
        //insertObj.find('.filterrow:first').remove();
    }
    ////$(removenode).each(function (i, n) { $(n).remove(); });
}
//设置组合逻辑
function changeGroupLogical(e) {
    var logical = $(e).parents('table').first().find('[data-logical]').attr('data-logical');
    logical = logical == 'and' ? 'or' : 'and';
    $(e).parents('table').first().find('[data-logical]').attr('data-logical', logical)
    $(e).parents('td').first().find('[name=logicaltext]').text(logical == 'and' ? '并且' : '或者');
    $(e).text('转换为"' + (logical != 'and' ? '并且' : '或者') + '"组合');
}
//取消组合
function cancelGroup(e, groupid) {
    var g = $(e).parents('td').first();
    var rows = $('#' + groupid).children();
    $('#filterConditions tr.filtergroup[data-groupid=' + groupid + ']').replaceWith(rows);
    g.remove();
}
//保存过滤条件
function saveFilter() {
    var mainGroup = $('.form-group[data-type="Main"]').children('.andorGroup,.pilot-row').not('[data-relevant=true]');
    QueryObject.Criteria = eachGroup(mainGroup, true);
    var relaveList = $('.form-group[data-type="Main"]').children('.relevant-box');
    var rItem = eachRelevant(relaveList, EntityName);
    for (var j = QueryObject.LinkEntities.length - 1; 0 <= j; j--) {
        if (QueryObject.LinkEntities[j].Columns.Columns.length > 0) {
            QueryObject.LinkEntities[j].LinkCriteria = new Xms.Fetch.FilterExpression(Xms.Fetch.LogicalOperator.And);
            QueryObject.LinkEntities[j].LinkEntities = [];
        } else {
            QueryObject.LinkEntities.splice(j, 1);
        }
    }
    for (var i = 0; i < rItem.length; i++) {
        QueryObject.LinkEntities.push(rItem[i]);
    }
    QueryObject.LinkEntities = conforLink(QueryObject.LinkEntities);//合并相关实体
    console.log("filerResult", QueryObject);
    $('#FetchConfig').val(JSON.stringify(QueryObject));
    $('#filterModal').modal('hide');
}
function conforLink(linkEntity) {
    var oldItem = linkEntity;
    var newItem = [];
    for (var i = oldItem.length - 1; 0 <= i; i--) {
        if (oldItem[i].Columns.Columns.length > 0) {
            newItem.push(oldItem[i]);
            oldItem.splice(i, 1);
        }
    }
    for (var j = 0; j < newItem.length; j++) {
        for (var m = oldItem.length - 1; 0 <= m; m--) {
            if (newItem[j].EntityAlias.toLowerCase() == oldItem[m].EntityAlias.toLowerCase()) {
                newItem[j].LinkCriteria = oldItem[m].LinkCriteria;
                newItem[j].LinkEntities = oldItem[m].LinkEntities;
                oldItem.splice(m, 1);
            }
        }
    }
    for (var k = oldItem.length - 1; 0 <= k; k--) {
        for (var p = 0; p < newItem.length; p++) {
            if (newItem[p].EntityAlias.toLowerCase() == oldItem[k].EntityAlias.toLowerCase()) {
                newItem.splice(p, 1);
            }
        }
        newItem.push(oldItem[k]);
    }
    return newItem;
}

function openFilter() {
    if ($('#pliot-page1').length > 0) {
        $('#filterModal').modal('show');
        var isload = $('#pliot-page1').attr('data-load');
        if (typeof isload == 'undefined') {
            $('#pliot-page1').attr('data-load', false);
            isload = $('#pliot-page1').attr('data-load');
        }
        if (isload == false || isload == 'false') {
            initFilter(FetchConfig, $('#pliot-page1').find('div[data-type="Main"]'));
            $('#pliot-page1').attr('data-load', true);
        }
    }
    else {
        Xms.Web.Alert(false, '请等待加载完毕后再点击');
    }
}
//排序设置start
function openSortModal() {
    if (AttributesList.length == 0) {
        Xms.Web.Alert(false, "没有可排序字段");
        return false;
    }
    var target = $('#sortModal');
    target.modal('show');
    var sort1 = target.find('#sortparm1');
    var sort2 = target.find('#sortparm2');
    var sorttype1 = target.find('input[name="sorttype1"]');
    var sorttype2 = target.find('input[name="sorttype2"]');
    sort1.html('');
    sort2.html('');
    sort2.append('<option value="">选择</option>');
    console.log('AttributesList', AttributesList);
    for (var i = 0; i < AttributesList.length; i++) {
        var _html = '<option data-relationship="' + AttributesList[i].relationship + '" data-name="' + AttributesList[i].name + '">' + AttributesList[i].text + '</option>';
        sort1.append(_html);
        sort2.append(_html);
    }
    var oneChoose = $('#views').find('th[data-sortnum="one"]');
    var twoChoose = $('#views').find('th[data-sortnum="two"]');

    if (oneChoose.length > 0) {
        console.log(oneChoose.attr('data-name'))
        var relationship = oneChoose.attr('data-relationship');
        if (relationship && relationship != "") {
            sort1.find('option[data-relationship="' + oneChoose.attr('data-relationship') + '"][data-name="' + oneChoose.attr('data-name') + '"]').attr('selected', 'selected');
        } else {
            sort1.find('option[data-name="' + oneChoose.attr('data-name') + '"]').attr('selected', 'selected');
        }
        target.find('input[name="sorttype1"][value="' + oneChoose.attr('data-sorttype').toLowerCase() + '"]').attr('checked', 'checked');
    }
    if (twoChoose.length > 0) {
        console.log(twoChoose.attr('data-name'))
        var relationship = oneChoose.attr('data-relationship');
        if (relationship && relationship != "") {
            sort2.find('option[data-relationship="' + twoChoose.attr('data-relationship') + '"][data-name="' + twoChoose.attr('data-name') + '"]').attr('selected', 'selected');
        } else {
            sort2.find('option[data-name="' + twoChoose.attr('data-name') + '"]').attr('selected', 'selected');
        }

        target.find('input[name="sorttype2"][value="' + twoChoose.attr('data-sorttype').toLowerCase() + '"]').attr('checked', 'checked');
    }
    changeSort(sort1);
    changeSort(sort2);
}
function changeSort(e) {
    var target = $('#sortModal');
    //target.find('option').removeClass('hide');
    var choose = $(e).find('option:selected');
    var sibling = $(e).parents('div.form-group').siblings().find('select');
    var cRelationship = choose.attr('data-relationship');
    var cName = choose.attr('data-name');
    sibling.find('option').removeClass('hide');
    sibling.find('option[data-relationship="' + cRelationship + '"][data-name="' + cName + '"]').addClass('hide');
}
function saveSortParams() {
    var target = $('#sortModal');
    var sort1 = target.find('#sortparm1');
    var sort2 = target.find('#sortparm2');
    $('#views').find('th').attr({ 'data-sorttype': '', 'data-sortnum': '' });
    target.find('option:selected').each(function (i, n) {
        var relationship = $(n).attr('data-relationship');
        var name = $(n).attr('data-name');
        var sorttype = $(n).parent('select').parent('div').next().find('input:checked').val();
        var label = $('#views').find('th[data-relationship="' + relationship + '"][data-name="' + name + '"]');
        label.attr('data-sorttype', sorttype);
        if ($(n).parent('select').attr('id') == 'sortparm1') {
            label.attr('data-sortnum', "one");
        }
        else {
            label.attr('data-sortnum', "two");
        }
    });
    saveGridConfig();
    target.modal('hide');
}
function UpdateQueryObjectOrder() {
    var target = $('#views');
    var oneSort = target.find('th[data-sortnum="one"]');
    var twoSort = target.find('th[data-sortnum="two"]');
    var sortColumn = { Name: '', SortAscending: false };
    var orders = new Array();
    var sortColumns = new Array();
    if (oneSort.length > 0) {
        //保存第一个筛选
        var onename = oneSort.attr('data-name');
        var onesorttype = oneSort.attr('data-sorttype');
        var onerelationship = oneSort.attr('data-relationship');
        onesorttype = onesorttype && onesorttype.toLowerCase();
        sortColumn = sortColumn.constructor();
        sortColumn.Name = onename;
        sortColumn.SortAscending = onesorttype == 'ascending';
        sortColumns.push(sortColumn);
        var oneord = '';
        if (onerelationship != '') {
            oneord = { AttributeName: onerelationship + '.' + onename, OrderType: onesorttype == 'ascending' ? 'Ascending' : 'Descending' };
        }
        else {
            oneord = { AttributeName: onename, OrderType: onesorttype == 'ascending' ? 'Ascending' : 'Descending' };
        }
        orders.push(oneord);
        if (twoSort.length > 0) {
            //保存第二个筛选
            var twoname = twoSort.attr('data-name');
            var twosorttype = twoSort.attr('data-sorttype');
            var tworelationship = twoSort.attr('data-relationship');
            tworelationship = tworelationship && tworelationship.toLowerCase();
            sortColumn = sortColumn.constructor();
            sortColumn.Name = twoname;
            sortColumn.SortAscending = twosorttype == 'ascending';
            sortColumns.push(sortColumn);
            var twoord = '';
            if (tworelationship != '') {
                twoord = { AttributeName: tworelationship + '.' + twoname, OrderType: twosorttype == 'ascending' ? 'Ascending' : 'Descending' };
            }
            else {
                twoord = { AttributeName: twoname, OrderType: twosorttype == 'ascending' ? 'Ascending' : 'Descending' };
            }
            orders.push(twoord);
        }
    }
    QueryObject.Orders = orders;
}
//排序设置END
function LinkEntity() {
    var self = new Object();
    self.LinkFromEntityName = '';
    self.LinkFromAttributeName = '';
    self.LinkToEntityName = '';
    self.LinkToAttributeName = '';
    self.LinkCriteria = new Xms.Fetch.FilterExpression(Xms.Fetch.LogicalOperator.And);
    self.JoinOperator = 1;
    self.EntityAlias = '';
    self.FromEntityAlias = '';
    self.Columns = { Columns: [] };
    return self;
}
function FilterExpression() {
    var self = new Object();
    self.FilterOperator = 'and';
    self.Conditions = new Array();
    self.Filters = new Array();
    return self;
}
function ConditionExpression() {
    var self = new Object();
    self.AttributeName = '';
    self.Operator = '';
    self.Values = new Array();
    return self;
}
function Save(type) {
    savetype = type;
    $("#savetype").val(savetype);
    $("#editform").submit();
}
function Save(type) {
    savetype = type;
    if ($('#isShowTab').length > 0) {
        if ($('#isShowTab').prop('checked')) {
            var _rowConfig = $('#LayoutConfig').val();
            var _rowObj = JSON.parse(_rowConfig);
            _rowObj.ExtEntityTabs = getExtEntityInfo();
            $('#LayoutConfig').val(JSON.stringify(_rowObj));
        } else {
            var _rowConfig = $('#LayoutConfig').val();
            var _rowObj = JSON.parse(_rowConfig);
            delete _rowObj.ExtEntityTabs;
            $('#LayoutConfig').val(JSON.stringify(_rowObj));
        }
    }
    $("#savetype").val(savetype);
    $("#editform").submit();
}
function Reset() {
    $("#editform").reset();
}

//过滤条件设置--行事件设置

//禁用输入框
var disabledArr = ['Last7Days', 'LastWeek', 'LastMonth', 'LastYear', 'LastYear', 'NextWeek', 'NextMonth', 'NextYear', 'ThisWeek', 'ThisMonth', 'ThisYear', 'Today', 'Tomorrow', 'Yesterday'];
//显示时间控件
var showDatepicker = ['Equal', 'NotEqual', 'GreaterThan', 'LessThan', 'GreaterEqual', 'LessEqual', 'OnOrAfter', 'OnOrBefore'];
//无需查找按钮
var noFindButton = ['BeginsWith', 'DoesNotBeginWith', 'EndsWith', 'DoesNotEndWith', 'Like', 'NotLike'];
//需要下拉 多选
var moreSelect = [];
//包含和不包含
var includeNull = ['NotNull', 'Null'];

//function initRowCommandSettings(){
//    var $rowcommand = $('#rowcommand');
//    var rowFirst = $rowcommand.find('.well:first');
//    rowFirst.find('.EventType');
//}

function loadFilterOperators(input, type, opts) {
    //操作符
    if (type == 'status') {
        type = 'state';
    }
    var _operators = Xms.Fetch.ConditionOperators[type];
    var op = new Array();
    op.push('<option data-value="" value="">' + LOC_FILTER_CONDITION_OPERATOR_SELECT + '</option>');
    $(_operators).each(function (i, n) {
        op.push('<option data-value="' + n[0] + '" value="' + n[1] + '">' + n[2] + '</option>');
    });
    $(input).html(op.join(''));
}

function addView() {
    var view = $('.well:first', '#rowcommand').clone();
    addAttributeToInput(view);
    $('.well:last', '#rowcommand').after(view);
    $('.well:last', '#rowcommand').show();
    console.log('addView', $('.well:first .judgeView', '#rowcommand'));
    view.find('.colorpicker').spectrum({
        flat: false,
        preferredFormat: 'rgb'
    });
    saveRowCommandValue();
    return view;
}
function removeView(obj) {
    $(obj).parent('.well').remove();
    saveRowCommandValue();
}
function checkAdd(obj, isParent) {
    var ddd = $('.judgeView:first', '#rowcommand>.well:first').clone();
    console.log('dddddd', ddd);
    if (!isParent) {
        $(obj).parents('.judgeBox').find('.judgeView:last').after(ddd);
        $(obj).parents('.judgeBox').find('.addMove:last').removeClass('glyphicon-plus').addClass('glyphicon-minus').attr('onclick', 'removeBox(this)');
    } else {
        $(obj).find('.judgeView:last').after(ddd);
        $(obj).find('.addMove:last').removeClass('glyphicon-plus').addClass('glyphicon-minus').attr('onclick', 'removeBox(this)');
    }
    if ($(obj).parents('.judgeBox').length > 0) {
        $(obj).parents('.judgeBox').css({ 'border': '1px solid #ccc', 'background': '#fff', 'border-radius': '10px' });
    } else {
        $(obj).find('.judgeBox').css({ 'border': '1px solid #ccc', 'background': '#fff', 'border-radius': '10px' });
    }
    console.log('checkAdd', $(obj));
    saveRowCommandValue();
    return ddd;
}
function removeBox(obj) {
    $(obj).parent('.judgeView').remove();
    saveRowCommandValue();
}

//加载字段
function loadRowCommondAttributes(callback) {
    var entityid = $('#EntityId').val();
    var $attrsInput = $('.ziduan', "#rowcommand");
    if (rowcommondAttributes != null) {
        var attrHtmls = [];
        attrHtmls.push('<option value="">请选择</option>');
        $(rowcommondAttributes).each(function (i, n) {
            attrHtmls.push('<option data-relationship="' + n.relationship + '" data-type="' + n.attributetypename + '" data-referencingattributelocalizedname="' + n.referencingattributelocalizedname + '" data-name="' + n.name + '" data-entityname="' + n.entityname + '" data-referencingattributename="' + n.referencingattributename + '" data-referencedentityid="' + n.referencedentityid + '" data-entitylocalizedname="' + n.entitylocalizedname + '" value="' + n.name + '" data-optionsetid="' + (n.optionsetid || "") + '" data-attributesid="' + n.attributesid + '" >' + n.localizedname + '</option>');
        });
        $attrsInput.html(_html.join(''));
        $attrsInput.each(function () {
            var $this = $(this);
            var oldValue = $this.val();
            $this.html(attrHtmls.join(''));
            console.log('addAttributeToInput', attrHtmls);
            $this.val(oldValue);
        });
        saveRowCommandValue();
        callback && callback();
    }
}

function addAttributeToInput(_context) {
    var entityid = $('#EntityId').val();
    _context = _context || "#rowcommand"
    var $attrsInput = $('.ziduan', _context);
    if (rowcommondAttributes != null) {
        var attrHtmls = [];
        attrHtmls.push('<option value="">请选择</option>');
        $(rowcommondAttributes).each(function (i, n) {
            attrHtmls.push('<option data-relationship="' + n.relationship + '" data-type="' + n.attributetypename + '" data-referencingattributelocalizedname="' + n.referencingattributelocalizedname + '" data-name="' + n.name + '" data-entityname="' + n.entityname + '" data-referencingattributename="' + n.referencingattributename + '" data-referencedentityid="' + n.referencedentityid + '" data-entitylocalizedname="' + n.entitylocalizedname + '" value="' + n.name + '" data-optionsetid="' + (n.optionsetid || "") + '" data-attributesid="' + n.attributesid + '" >' + n.localizedname + '</option>');
        });
        $attrsInput.html(attrHtmls.join(''));
        $attrsInput.each(function () {
            var $this = $(this);
            var oldValue = $this.val();
            $this.html(attrHtmls.join(''));
            console.log('addAttributeToInput', attrHtmls);
            $this.val(oldValue);
        });
        saveRowCommandValue();
        //callback && callback();
    }
}

function rowlookupCallback(result, inputid) {
    console.log(result, inputid);
    $('#' + inputid).val(result[0].name);
    $('#' + inputid).attr('data-value', result[0].id);
    saveRowCommandValue();
}

function saveRowCommandValue() {
    var $rowcommand = $("#rowcommand");
    var res = [];
    var flag = true;
    if ($rowcommand.find('.well:gt(0)').length > 0) {
        $rowcommand.find('.well:gt(0)').each(function () {
            var $this = $(this);
            var LogicalOperator = $this.find('.LogicalOperator').val();
            var ActionType = $this.find('.ActionType').val();
            var EventType = $this.find('.EventType').val();
            var actions = {};
            $this.find('.actionSet').each(function (i, n) {
                var _key = $(this).attr('data-key');
                actions[_key] = $(this).find('.ColorSite').children().val();
            });
            var _obj = {
                "LogicalOperator": LogicalOperator,
                "Conditions": [],
                "ActionType": ActionType,
                "Action": actions,
                "EventType": EventType
            }
            var _condition = [];
            console.log($this.find('.judgeBox>.judgeView:gt(0)'));
            $this.find('.judgeBox>.judgeView').each(function (i, n) {
                var $n = $(this);
                var attrname = $n.find('.ziduan').val();
                var attrtype = $n.find('.ziduan>option:selected').attr('data-type');
                var oper = $n.find('.Operator').val();
                var value = [];
                console.log(' $n.find(".ziduan")', attrtype)
                $n.find('.Values').each(function () {
                    if (attrtype == 'lookup' || attrtype == 'customer' || attrtype == 'owner') {
                        value.push($(this).find('input[name="value"]').attr('data-value'));
                    } else {
                        value.push($(this).find('input[name="value"]').val());
                    }
                });
                _condition.push({
                    "AttributeName": attrname,
                    "Operator": oper,
                    "Values": value
                });
                if (attrname == "" || oper == "") {
                    flag = false;
                    return false;
                }
            });
            console.log(_condition);
            _obj.Conditions = _condition;
            res.push(_obj);
        });
    }
    if (flag == true && res.length > 0) {
        var _rowConfig = $('#LayoutConfig').val();
        var _rowObj = JSON.parse(_rowConfig);
        _rowObj.RowCommand = res;
        console.log('提交的grid的数据', res);
        $('#LayoutConfig').val(JSON.stringify(_rowObj));
    } else if (flag == false || res.length == 0) {
        var _rowConfig = $('#LayoutConfig').val();
        if (_rowConfig && _rowConfig != "") {
            var _rowObj = JSON.parse(_rowConfig);
            delete _rowObj.RowCommand;
            console.log('提交的grid的数据', res);
            $('#LayoutConfig').val(JSON.stringify(_rowObj));
        }
    }
    if (flag == true) {
        return res;
    } else {
        return false;
    }
}

function resetFilterCnName(entityid, callback, type) {
    if (!type) {
        var params = {
            type: "queryview" + entityid,
            data: { getall: true, entityid: entityid }
        }
        Xms.Web.PageCache('workflow', '/api/schema/attribute?__r=' + new Date().getTime(), params, function (data) {
            callback && callback(data);
        });
    } else {
        var enParam = {
            type: 'referencingentityid' + entityid,
            data: { referencingentityid: entityid }
        }
        Xms.Web.PageCache('workflow', '/api/schema/relationship/GetReferenced/' + enParam.data.referencingentityid, null, function (result) {
            callback && callback(result);
        });
    }
}

function selectRecordCallback(result, inputid) {
    var names = $.map(result, function (item, key) {
        return item.name;
    });
    var values = $.map(result, function (item, key) {
        return item.id;
    });
    $('#' + inputid).val(names.join(','));
    var valueid = inputid.replace(/_text/, '');
    $('#' + valueid).val(values.join(','));
    $('#' + inputid).trigger('change');
    $('#' + valueid).trigger('change');
    var _LayoutConfig = $('#LayoutConfig').val();
    if (_LayoutConfig != "") {
        var objRowdatas = JSON.parse(_LayoutConfig);
    } else {
        var objRowdatas = {};
    }
    var jslib = $('#jslibrary');
    if (result && result.length > 0) {
        jslib.val(values.join(','));
        objRowdatas.ClientResources = values;
        $('#LayoutConfig').val(JSON.stringify(objRowdatas));
    }
}