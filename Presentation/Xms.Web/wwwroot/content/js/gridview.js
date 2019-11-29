function setLookUpState(obj) {
    var val = $(obj).val();
    //if (val == "") return true;
    if ($(obj).siblings(".xms-dropdownLink").length > 0) {
        $(obj).siblings(".xms-dropdownLink").remove();
    }
    var tar = $("#" + $(obj).attr("id").replace("_text", ""));
    var id = tar.attr("data-value");
    var alink = $('<a target="_blank" href="/entity/create?entityid=' + $(obj).attr("data-entityid") + '&recordid=' + id + '" class="xms-dropdownLink" title="' + $(obj).val() + '"><span class="glyphicon glyphicon-list-alt"></span> <span class="xms-drlinki" data-id="" data-value="">' + $(obj).val() + '</span></a>');
    $(obj).css({ "color": "#fff" });
    $(obj).parent().append(alink);
    console.log(alink);
    var width = $(obj).width();
    var lenW = alink.find(".xms-drlinki").width();
    if (lenW > width) {
        lenW = width - 30;
    }
    alink.css("width", lenW + 20);
    $(obj).focus();
}
var imme = xmsImmediate(500);//获取缓冲函数
function SearchTip(e, attrid) {
    imme(function () {
        var listTarget = $(e);
        var thTarget = $(e).parents('th');
        if ($(e).siblings(".xms-dropdownLink").length > 0) {
            $(e).siblings(".xms-dropdownLink").remove();
            $(e).css("color", "#555");
        }
        var searcher = listTarget.data().searcher;
        console.log(listTarget.data().searcher);
        if (!listTarget.data().searcher) {
            searcher = new xmsSearcher(listTarget, {
                setParentPos: false,
                addHandler: function (obj, id, par, width) {
                    var tar = $("#" + $(par.obj).attr("id").replace("_text", ""));
                    if ($(par.obj).siblings(".xms-dropdownLink").length > 0) {
                        $(par.obj).siblings(".xms-dropdownLink").remove();
                    }
                    if (tar.length > 0) {
                        tar.val($(obj).children('.xms-dropdownSearch-value').text());
                        tar.attr("data-value", id);
                    }
                    var alink = $('<a target="_blank" href="/entity/create?entityid=' + listTarget.attr("data-entityid") + '&recordid=' + id + '" class="xms-dropdownLink" title="' + par.obj.val() + '"><span class="glyphicon glyphicon-list-alt"></span> <span class="xms-drlinki" data-id="" data-value="">' + par.obj.val() + '</span></a>');
                    $(par.obj).css({ "color": "#fff" });

                    $(par.opts.parent).append(alink);
                    var lenW = alink.find(".xms-drlinki").width();
                    if (lenW > width) {
                        lenW = width - 30;
                    }
                    alink.css("width", lenW + 20);

                    $(par.obj).focus();
                }
            });
            listTarget.data().searcher = searcher;
        }
        searcher = listTarget.data().searcher;
        searcher.ele.addClass("open");

        Xms.Web.Get('/api/schema/entity/' + attrid, function (data) {
            if (data.StatusName == 'success') {
                var _value = listTarget.val();
                var refname = data.content.name;
                var filter = { "filteroperator": 0, "conditions": [], "filters": [{ "filteroperator": 1, "filters": [], "conditions": [{ "attributename": "name", "operator": 6, "values": [_value] }] }] }
                var QueryObject = { EntityName: refname, Criteria: filter/*new Xms.Fetch.FilterExpression()*/, ColumnSet: { AllColumns: true }, PageInfo: { PageNumber: 1, PageSize: 15 } };
                // var QueryObject = { EntityName: refname, Criteria: filter, ColumnSet: { AllColumns: true }, PageInfo: { PageNumber: 1, PageSize: 10 } };
                Xms.Web.Post('/api/data/Retrieve/Multiple', { 'query': QueryObject }, false, function (data2) {
                    searcher.show();
                    var datas = [];
                    data2.Content = JSON.parse(data2.Content.replace("\n", ""));
                    console.log(data2.Content)
                    for (var i = 0, len = data2.Content.items.length; i < len; i++) {
                        data2.Content.items[i]['title'] = data2.Content.items[i].name;
                        data2.Content.items[i]['id'] = data2.Content.items[i][refname.toLowerCase() + 'id'];
                    }
                    searcher.removeAll();
                    for (var i = 0, len = data2.Content.items.length; i < len; i++) {
                        searcher.addData(data2.Content.items[i]);
                    }
                    searcher.render();
                    var val = $(e).val();
                    // MacthSearch(e);
                }, false, false, false);
            }
        });
    });
    // MacthSearch(e);
}
function MacthSearch(e) {
    var val = $(e).val();
    var valL = val.toLowerCase();
    $(e).siblings('.xms-dropdownSearch-box').find('.xms-dropdownSearch-item').each(function (i, n) {
        var itemVal = $(n).find('.xms-dropdownSearch-value').text();
        var itemValL = itemVal.toLowerCase();
        if (itemValL.indexOf(valL) != -1) {
            $(n).show();
        } else {
            $(n).hide();
        }
    });
}
function DayQuery(e) {
    var $this = $(e);
    if ($this.is('.active')) {
        removeFilter('createdon');
        $this.removeClass("active").focusout();
        return;
    }
    $this.siblings().removeClass("active").end().addClass("active");
    var setDate = $(e).attr('data-day');
    var filter = new Xms.Fetch.FilterExpression();
    var condition = new Xms.Fetch.ConditionExpression();
    filter.FilterOperator = Xms.Fetch.LogicalOperator.And;
    condition.AttributeName = 'createdon';
    condition.Operator = Xms.Fetch.ConditionOperator.GreaterEqual;
    condition.Values.push(setDate);
    filter.Conditions.push(condition);
    addFilter('createdon', filter);
}
function EditRecord(newWindow) {
    var event = typeof event != 'undefined' ? event : window.event;
    var target = $('#datatable');
    Xms.Web.SelectingRow(event.target, false, true);
    var id = Xms.Web.GetTableSelected(target);
    var url = '/entity/edit?entityid=' + Xms.Page.PageContext.EntityId + '&recordid=' + id;
    if (newWindow) {
        entityIframe('show', url);
    }
    else {
        location.href = url;
    }
}

function clearSearchFiler() {
    clearFilters(); $('#searchForm button[name=resetBtn]').trigger('click'); closeSearchC();
}

function addFilterItem(name, _filter) {
    var obj;
    $(columnFilters).each(function (ii, nn) {
        console.log(name)
        if (nn[0] == name) {
            obj = nn[1];
            return;
        }
    });
    if (obj != null) {
        columnFilters = $.grep(columnFilters, function (nn, i) {
            return nn[1] != obj;
        });
        filters.Filters = $.grep(filters.Filters, function (nn, i) {
            return nn != obj;
        });
    }
    var nf = [name, _filter];
    columnFilters.push(nf);
    filters.Filters.push(_filter);
}
function searchByCondition(isfilterForm) {
    var searchFormSearch = $("#searchFormSearch");
    searchFormSearch.find('.seacher-row').each(function (key) {
        var filter = new Xms.Fetch.FilterExpression();
        filter.FilterOperator = Xms.Fetch.LogicalOperator.And;
        var filtername = $(this).attr('data-name');
        var value = $(this).find('input.form-control').val();
        var filtetype = $(this).attr('data-type');
        var obj = null;
        //删除之前的过滤条件
        if (filters.Filters && filters.Filters.length > 0) {
            filters.Filters = $.grep(filters.Filters, function (nn, i) {
                return nn.Conditions[0].AttributeName != filtername;
            });
        }
        //添加过滤条件
        if (filtetype == 'picklist' || filtetype == 'state' || filtetype == 'bit') {
            inList = $(this).find('.colinput');
            var condition = new Xms.Fetch.ConditionExpression();
            condition.AttributeName = filtername;
            condition.Operator = Xms.Fetch.ConditionOperator.Equal;
            inList.each(function (i, n) {
                var keywork = $(n).attr("data-value") || "";
                condition.Values.push(keywork);
            });
            filter.Conditions.push(condition);
        } else if (filtetype == 'int' || filtetype == 'decimal' || filtetype == 'float' || filtetype == 'money') {
            inList = $(this).find('input.colinput');
            inList.each(function (i, n) {
                if (keywork != '') {
                    var condition = new Xms.Fetch.ConditionExpression();
                    condition.AttributeName = filtername;
                    if (i == 0) {
                        condition.Operator = Xms.Fetch.ConditionOperator.GreaterEqual;
                    } else {
                        condition.Operator = Xms.Fetch.ConditionOperator.LessEqual;
                    }
                    var keywork = $(n).val();
                    condition.Values.push(keywork);
                    // console.log(condition)
                    filter.Conditions.push(condition);
                }
            });
        }
        else if (filtetype == 'datetime') {
            inList = $(this).find('input.colinput');
            inList.each(function (i, n) {
                if (keywork != '') {
                    var condition = new Xms.Fetch.ConditionExpression();
                    condition.AttributeName = filtername;
                    var keywork = $(n).val();
                    if (i == 0) {
                        condition.Operator = Xms.Fetch.ConditionOperator.GreaterEqual;
                    } else {
                        condition.Operator = Xms.Fetch.ConditionOperator.LessEqual;
                        if (!isfilterForm) {
                            keywork = new Date(keywork).DateAdd('d', 1).format('yyyy-MM-dd');
                        } else {
                            keywork = new Date(keywork).format('yyyy-MM-dd');
                            keywork += ' 23:59:59';
                        }
                    }
                    condition.Values.push(keywork);
                    // console.log(condition)
                    filter.Conditions.push(condition);
                }
            });
        }
        else {
            inList = $(this).find('.colinput');
            var condition = new Xms.Fetch.ConditionExpression();
            condition.AttributeName = filtername;
            if (filtetype == 'nvarchar') {
                condition.Operator = Xms.Fetch.ConditionOperator.Like;
            } else {
                condition.Operator = Xms.Fetch.ConditionOperator.Equal;
            }
            console.log('filtetype.' + filtername, filtetype);
            inList.each(function (i, n) {
                if (filtetype == 'lookup' || filtetype == 'owner' || filtetype == 'customer' || filtetype == 'primarykey') {
                    var keywork = $(n).attr('data-value') || "";
                } else {
                    var keywork = $(n).val();
                }
                condition.Values.push(keywork);
            });
            filter.Conditions.push(condition);
        }
        if (filter.Conditions[0]
            && typeof filter.Conditions[0].Values === "object" && filter.Conditions[0].Values.length
            && filter.Conditions[0].Values[0].replace("\s", "") != ""
            || (filter.Conditions[0].Values[1] && filter.Conditions[0].Values[1].replace("\s", "") != "")
        ) {
            addFilterItem(filtername, filter);
        }
    });
    $(".xms-formDropDown").trigger("xmsFormDrop.close");
    pageUrl = $.setUrlParam(pageUrl, 'page', 1);
    setItemFirst($("#searchFormSearch"));

    //if (filters.Filters.length > 0) {
    submitFilter();
    // }
}
function setItemFirst($context) {
    var cusSearch = $context;
    var _parent = cusSearch.parents('.xms-formDropDown:first');
    var items = cusSearch.find(".xms-formDropDown-Item");
    var value = [];
    items.each(function () {
        //var val = $(this).find("input.colinput").val();
        //var itemtype = $(this).attr("data-type");
        //if (itemtype == "picklist" || itemtype == 'state' || itemtype == 'bit') {
        //    val = $(this).find("select>option:selected").text();
        //}
        //if (!val || val == "") {
        //    return true;
        //}
        var type = $(this).attr("data-type");
        var label = $(this).find("label").text();
        var str = label + ":", flag = true;
        if (type == "datetime" || type == "int" || type == 'decimal' || type == 'float' || type == 'money') {
            var startT = $(this).find("input:first");
            var endT = $(this).find("input:last");
            if (startT.val().replace("\s", "") != "" && endT.val().replace("\s", "") != "") {
                str += startT.val();
                str += "-";
                str += endT.val();
            } else if (startT.val().replace("\s", "") != "" && endT.val().replace("\s", "") == "") {
                str += startT.val();
            } else if (startT.val().replace("\s", "") == "" && endT.val().replace("\s", "") != "") {
                str += endT.val();
            }
            else {
                flag = false;
            }
        } else if (type == "picklist" || type == 'state' || type == 'bit') {
            if ($(this).find("select>option:selected").text()) {
                str += $(this).find("select>option:selected").text();
            }
            else {
                flag = false;
            }
        } else {
            if ($(this).find("input.colinput").val()) {
                str += $(this).find("input.colinput").val();
            }
            else {
                flag = false;
            }
        }
        if (flag == true) value.push(str);
    });
    if (!value || value.length == 0) {
        $(".xms-formDownInput", _parent).html('<span class="glyphicon glyphicon-filter"></span>').prop('title', "");
        return false;
    }
    console.log(value);
    $(".xms-formDownInput", _parent).html(value.join(";")).prop('title', value.join(";"));
}

function closeSearchC() {
    $(".xms-formDropDown").trigger("xmsFormDrop.close");
}
function clearFilters() {
    $('#Q').val('');
    $('#searchForm').submit();
    var _parent = $("#searchFormSearch").parents('.xms-formDropDown:first');
    $(".xms-formDownInput", _parent).text('<span class="glyphicon glyphicon-filter"></span>').prop('title', '');
    $('#dayfilters').find('button').removeClass("active");
    $("#searchFormSearch").find("a.xms-dropdownLink").remove();
}
function searchKanban() {
    var viewid = $('#viewSelector').attr('data-value');
    var kanbanSearch = $("#kanbanSearch");
    var aggregateField = Xms.Web.SelectedValue($('#aggregateField')),
        groupField = Xms.Web.SelectedValue($('#groupField'));
    if (!aggregateField || !groupField) {
        Xms.Web.Toast('请指定统计字段和分组字段', false);
        return;
    }
    url = '/entity/kanbanview?queryid=' + viewid + '&aggregatefield=' + aggregateField + '&groupfield=' + groupField;
    loadData(url, $('#kanbanview'), function () {
        $('#kanbanview').removeClass('hide');
        console.log($('#kanbanview').html());
        //alert(123);
        reSetTopStyle();
        setItemFirst(kanbanSearch);
    });
}
function closeKanban() {
    $(".xms-formDropDown").trigger("xmsFormDrop.close");
}