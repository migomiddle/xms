var FilterDialog = {
    //禁用输入框
    disabledArr: ['Last7Days', 'LastWeek', 'LastMonth', 'LastYear', 'LastYear', 'NextWeek', 'NextMonth', 'NextYear', 'ThisWeek', 'ThisMonth', 'ThisYear', 'Today', 'Tomorrow', 'Yesterday'],
    //显示时间控件
    showDatepicker: ['Equal', 'NotEqual', 'GreaterThan', 'LessThan', 'GreaterEqual', 'LessEqual', 'OnOrAfter', 'OnOrBefore', 'Before', 'After'],
    //无需查找按钮
    noFindButton: ['BeginsWith', 'DoesNotBeginWith', 'EndsWith', 'DoesNotEndWith', 'Like', 'NotLike'],
    //包含和不包含
    moreSelect: ['In', 'NotIn'],
    //包含和不包含
    includeNull: ['NotNull', 'Null'],
    bindSelected: function (result, inputid) {
        console.log(result, inputid);
        $('#' + inputid).val(result[0].name);
        $('#' + inputid).attr('data-value', result[0].id);
    },
    bindMoreSelected: function (result, inputid) {
        var rName = '';
        var rId = '';
        for (var i = 0; i < result.length; i++) {
            console.log(result, inputid);
            rName += result[i].name;
            rId += result[i].id;
            if (i != result.length - 1) {
                rName += ',';
                rId += ',';
            }
        }
        $('#' + inputid).val(rName);
        $('#' + inputid).attr('data-value', rId);
    },
    digitsValue: function (e) {
        var val = $(e).val();
        var check = true;
        var reg = new RegExp(/^\+?[1-9][0-9]*$/);
        if (!reg.test(val) || val == '') {
            if ($(e).next('label[name="digitsTip"]').length <= 0) {
                $('<label name="digitsTip">请输入整数</label>').insertAfter($(e));
            }
            check = false;
        }
        else {
            $(e).next('label[name="digitsTip"]').remove();
            check = true;
        }
        return check;
    },
    valiExp_float: function (e) {
        var val = $(e).val();
        var check = true;
        //console.log(Xms.Web.ValidData('float')(val))
        if (!Xms.Web.ValidData('float')(val)) {
            if ($(e).next('label[name="digitsTip"]').length <= 0) {
                $('<label name="digitsTip">请输入数字</label>').insertAfter($(e));
            }
            check = false;
        } else {
            $(e).next('label[name="digitsTip"]').remove();
            check = true;
        }
        return check;
    },
    validateInput: function (e, type) {
        var check = true;
        //console.log(FilterDialog['valiExp_' + type])
        if (FilterDialog['valiExp_' + type]) {
            check = FilterDialog['valiExp_' + type](e);
        }
        return check;
    },
    bindInput: function (select, input, datatype, attributeid, referencedentityid, ddlItems, optionsetid, ischange, moreOpts) {
        var FilterDialogModel = {
            attributeid: attributeid,
            operators: Xms.Fetch.ConditionOperators[datatype],
            datatype: datatype
        };
        console.log('bindInput', datatype)

        if (typeof select === "string") {
            var temp = select.split(',');
            var v = temp[0];
            var dataval = temp[1] ? temp[1] : "";
        } else {
            var v = Xms.Web.SelectedValue(select);
            var dataval = select.find('option:selected').attr('data-value');
        }

        var id = datatype + Math.round(new Date().getTime() / 1000);
        //清除一遍start
        input.removeAttr('data-picklistinit');
        var newinput = input.clone();
        input.replaceWith(newinput);//清除一遍end
        //input = select.parents('.pilot-row').find('input[name=value]');//重新获取
        input = newinput;
        console.log(input);
        if (input.siblings('.SumoSelect').length > 0) {
            input.siblings('.SumoSelect').remove();
        }
        input.prop('id', id);
        if (input.parents('.typeahead__container').length > 0 && datatype != 'lookup' && datatype != 'owner' && datatype != 'customer') {
            input.parents('.typeahead__container').replaceWith('<input type="text" class="form-control input-sm" name="value" >');
        }
        if (input.siblings('select').length > 0 && datatype != 'picklist') {
            input.siblings('select').remove();
            input.removeClass('hide');
            input.removeClass('datepicker');
        }
        input.removeClass('hide');
        if (v == Xms.Fetch.ConditionOperator.NotNull || v == Xms.Fetch.ConditionOperator.Null || v == null || v == '' || $.inArray(dataval, FilterDialog.disabledArr) != -1) {
            if (input.parents('.typeahead__container').length > 0) {
                input.parents('.typeahead__container').replaceWith('<input id="' + id + '" type="text" class="form-control input-sm" name="value" disabled="disabled" >');
            } else {
                console.log(datatype != "picklist")
                if (datatype != "picklist") {
                    input.val('').prop('disabled', 'disabled');
                    input.siblings().prop('disabled', 'disabled');
                } else {
                    if (input.siblings('select').length > 0) {
                        input.siblings('select').remove();
                    }
                    if (input.siblings('.SumoSelect').length > 0) {
                        input.siblings('.SumoSelect').remove();
                    }
                    if (ddlItems.length == 0) {
                        ddlItems.push({ value: '', text: '请选择' });
                    }
                    input.picklist({
                        displaytype: 'select'
                        , required: true
                        , items: ddlItems
                    });
                    console.log('filterDialog', input);
                    input.val('').prop('disabled', 'disabled');
                    input.siblings().prop('disabled', 'disabled');
                }
            }
        }
        else if (v != null && v != '') {
            input.removeProp('disabled');
            input.siblings().removeProp('disabled', 'disabled');
            var self = $(input);
            var inputid = self.prop('id');
            var lookupid = referencedentityid;
            var value = self.attr('data-value') || self.val();
            if (ischange) {
                value = '';
            }
            console.log('filterdialog', self);
            //根据字段类型生成输入框
            switch (datatype) {
                case "lookup":
                    if ($.inArray(dataval, FilterDialog.noFindButton) != -1) {
                        if (input.parents('.typeahead__container').length > 0) {
                            input.parents('.typeahead__container').replaceWith('<input type="text" class="form-control input-sm" name="value" >');
                        } else {
                            input.removeProp('disabled');
                        }
                    }
                    else if ($.inArray(dataval, FilterDialog.moreSelect) != -1) {
                        if (value != null && value != '' && value.length > 20) {
                            var param = {
                                type: lookupid + value,
                                data: { entityid: lookupid, value: value }
                            }
                            Xms.Web.PageCache('filterDialog', '/api/data/Retrieve/ReferencedRecord/' + param.data.entityid + '/' + param.data.value, null, function (response) {
                                console.log(response);
                                if (response.content && response.content['name']) {
                                    input.val(response.content['name']);
                                    input.attr('data-value', response.content['id']);
                                }
                                input.lookup({
                                    dialog: function () {
                                        Xms.Web.OpenDialog('/entity/RecordsDialog?entityid=' + referencedentityid + '&singlemode=true&inputid=' + id, 'FilterDialog.bindMoreSelected')
                                    }
                                    ,
                                    clear: function () {
                                        $('#' + id).val('');
                                        $('#' + id).siblings(':input').val('');
                                    },
                                    disabled: false
                                });
                            });
                        } else {
                            $('#' + id).val('');
                            input.lookup({
                                dialog: function () {
                                    Xms.Web.OpenDialog('/entity/RecordsDialog?entityid=' + referencedentityid + '&singlemode=true&inputid=' + id, 'FilterDialog.bindMoreSelected')
                                }
                                ,
                                clear: function () {
                                    $('#' + id).val('');
                                    $('#' + id).siblings(':input').val('');
                                },
                                disabled: false
                            });
                        }
                    }
                    else {
                        if (value != null && value != '' && value.length > 20) {
                            var param = {
                                type: lookupid + value,
                                data: { entityid: lookupid, value: value }
                            }
                            Xms.Web.PageCache('filterDialog', '/api/data/Retrieve/ReferencedRecord/' + param.data.entityid + '/' + param.data.value, null, function (response) {
                                console.log(response);
                                if (response.content && response.content['name']) {
                                    input.val(response.content['name']);
                                    input.attr('data-value', response.content['id']);
                                }
                                input.lookup({
                                    dialog: function () {
                                        Xms.Web.OpenDialog('/entity/RecordsDialog?entityid=' + referencedentityid + '&singlemode=true&inputid=' + id, 'FilterDialog.bindSelected')
                                    }
                                    ,
                                    clear: function () {
                                        $('#' + id).val('');
                                        $('#' + id).siblings(':input').val('');
                                    },
                                    disabled: false
                                });

                                input.prop('disabled', 'disabled');
                            });
                        } else {
                            $('#' + id).val('');
                            input.lookup({
                                dialog: function () {
                                    Xms.Web.OpenDialog('/entity/RecordsDialog?entityid=' + referencedentityid + '&singlemode=true&inputid=' + id, 'FilterDialog.bindSelected')
                                }
                                ,
                                clear: function () {
                                    $('#' + id).val('');
                                    $('#' + id).siblings(':input').val('');
                                },
                                disabled: false
                            });

                            input.prop('disabled', 'disabled');
                        }
                    }
                    break;
                case "owner":
                    if ($.inArray(dataval, FilterDialog.noFindButton) != -1) {
                        if (input.parents('.typeahead__container').length > 0) {
                            input.parents('.typeahead__container').replaceWith('<input type="text" class="form-control input-sm" name="value" >');
                        } else {
                            input.removeProp('disabled');
                        }
                    }
                    else if ($.inArray(dataval, FilterDialog.moreSelect) != -1) {
                        if (value != null && value != '' && value.length > 20) {
                            var param = {
                                type: lookupid + value,
                                data: { entityid: lookupid, value: value }
                            }
                            Xms.Web.PageCache('filterDialog', '/api/data/Retrieve/ReferencedRecord/' + param.data.entityid + '/' + param.data.value, null, function (response) {
                                console.log(response);
                                if (response.content && response.content['name']) {
                                    input.val(response.content['name']);
                                    input.attr('data-value', response.content['id']);
                                }
                                input.lookup({
                                    dialog: function () {
                                        Xms.Web.OpenDialog('/entity/RecordsDialog?entityid=' + referencedentityid + '&singlemode=true&inputid=' + id, 'FilterDialog.bindMoreSelected', null, function () {
                                            input.removeProp('disabled');
                                        })
                                    }
                                    ,
                                    clear: function () {
                                        $('#' + id).val('');
                                        $('#' + id).siblings(':input').val('');
                                    },
                                    disabled: false
                                });
                            });
                        } else {
                            $('#' + id).val('');
                            input.lookup({
                                dialog: function () {
                                    Xms.Web.OpenDialog('/entity/RecordsDialog?entityid=' + referencedentityid + '&singlemode=true&inputid=' + id, 'FilterDialog.bindMoreSelected', null, function () {
                                        input.removeProp('disabled');
                                    })
                                }
                                ,
                                clear: function () {
                                    $('#' + id).val('');
                                    $('#' + id).siblings(':input').val('');
                                },
                                disabled: false
                            });
                        }
                    }
                    else {
                        if (value != null && value != '' && value.length > 20) {
                            var param = {
                                type: lookupid + value,
                                data: { entityid: lookupid, value: value }
                            }
                            Xms.Web.PageCache('filterDialog', '/api/data/Retrieve/ReferencedRecord/' + param.data.entityid + '/' + param.data.value, null, function (response) {
                                console.log(response);
                                if (response.content && response.content['name']) {
                                    input.val(response.content['name']);
                                    input.attr('data-value', response.content['id']);
                                }
                                input.lookup({
                                    dialog: function () {
                                        Xms.Web.OpenDialog('/entity/RecordsDialog?entityid=' + referencedentityid + '&singlemode=true&inputid=' + id, 'FilterDialog.bindSelected', null, function () {
                                            //input.removeProp('disabled');
                                        })
                                    }
                                    ,
                                    clear: function () {
                                        $('#' + id).val('');
                                        $('#' + id).siblings(':input').val('');
                                    },
                                    disabled: false
                                });
                                input.prop('disabled', 'disabled');
                            });
                        } else {
                            $('#' + id).val('');
                            input.lookup({
                                dialog: function () {
                                    Xms.Web.OpenDialog('/entity/RecordsDialog?entityid=' + referencedentityid + '&singlemode=true&inputid=' + id, 'FilterDialog.bindSelected', null, function () {
                                        //input.removeProp('disabled');
                                    })
                                }
                                ,
                                clear: function () {
                                    $('#' + id).val('');
                                    $('#' + id).siblings(':input').val('');
                                },
                                disabled: false
                            });
                            input.prop('disabled', 'disabled');
                        }
                    }
                    break;
                case "customer":
                    if (value != null && value != '' && value.length > 20) {
                        var param = {
                            type: lookupid + value,
                            data: { entityid: lookupid, value: value }
                        }
                        Xms.Web.PageCache('filterDialog', '/api/data/Retrieve/ReferencedRecord/' + param.data.entityid + '/' + param.data.value, null, function (response) {
                            console.log(response);
                            input.val();
                            input.lookup({
                                dialog: function () {
                                    Xms.Web.OpenDialog('/entity/RecordsDialog?entityid=' + referencedentityid + '&singlemode=true&inputid=' + id, 'FilterDialog.bindSelected')
                                }
                                ,
                                clear: function () {
                                    $('#' + id).val('');
                                    $('#' + id).siblings(':input').val('');
                                },
                                disabled: true
                            });
                        });
                    } else {
                        input.lookup({
                            dialog: function () {
                                Xms.Web.OpenDialog('/entity/RecordsDialog?entityid=' + referencedentityid + '&singlemode=true&inputid=' + id, 'FilterDialog.bindSelected')
                            }
                            ,
                            clear: function () {
                                $('#' + id).val('');
                                $('#' + id).siblings(':input').val('');
                            },
                            disabled: true
                        });
                    }
                    break;
                case "picklist":
                    Xms.Ajax.Get(ORG_SERVERURL + '/api/schema/optionset/getitems/' + optionsetid, null, function (data) {
                        console.log('picklist.item', data);
                        data = JSON.parse(data.Content);
                        var ddlItems = [];
                        for (var i = 0; i < data.length; i++) {
                            ddlItems.push({
                                value: data[i].value || '',
                                text: data[i].name || ''
                            });
                        }
                        if (ddlItems.length == 0) {
                            ddlItems.push({ value: '', text: '请选择' });
                        }
                        console.log('bindinput.filterdialog', ischange, moreOpts);
                        //moreOpts可以配置是否开启下拉框多选方式
                        if ($.inArray(dataval, FilterDialog.moreSelect) != -1 && (!moreOpts || !moreOpts.multiSelector)) {
                            if (input.siblings('select').length > 0) {
                                input.siblings('select').remove();
                            }
                            if (input.siblings('.SumoSelect').length > 0) {
                                input.siblings('.SumoSelect').remove();
                            }
                            input.picklist({
                                displaytype: 'select'
                                , required: true
                                , items: ddlItems
                                , multi: {
                                    captionFormat: '选中 {0} 个',
                                    captionFormatAllSelected: '选中 {0} 个!',
                                    placeholder: '',
                                }
                            });
                        }
                        else {
                            if (input.siblings('select').length > 0) {
                                input.siblings('select').remove();
                            }
                            if (input.siblings('.SumoSelect').length > 0) {
                                input.siblings('.SumoSelect').remove();
                            }
                            input.picklist({
                                displaytype: 'select'
                                , required: true
                                , items: ddlItems
                            });
                            //input.siblings('select').removeProp('multiple');
                            //input.siblings('select').removeProp('size');
                        }
                    })
                    break;
                case "state":
                    Xms.Ajax.Get(ORG_SERVERURL + '/api/schema/stringmap/' + FilterDialogModel.attributeid, null, function (data) {
                        data = JSON.parse(data.Content);
                        var ddlItems = [];
                        for (var i = 0; i < data.length; i++) {
                            ddlItems.push({
                                value: data[i].value,
                                text: data[i].name
                            });
                        }
                        input.picklist({
                            displaytype: 'select'
                            , required: true
                            , items: ddlItems
                        });
                    });
                    break;
                case "status":
                    Xms.Ajax.Get(ORG_SERVERURL + '/api/schema/RetriveOptionItems', { 'optionsetid': optionsetid }, function (data) {
                        data = JSON.parse(data.Content);
                        var ddlItems = [];
                        for (var i = 0; i < data.length; i++) {
                            ddlItems.push({
                                value: data[i].value,
                                text: data[i].name
                            });
                        }
                        input.picklist({
                            displaytype: 'select'
                            , required: true
                            , items: ddlItems
                        });
                    });
                    break;
                case "bit":
                    Xms.Ajax.Get(ORG_SERVERURL + '/api/schema/stringmap/' + FilterDialogModel.attributeid, null, function (data) {
                        data = JSON.parse(data.Content);
                        var ddlItems = [];
                        for (var i = 0; i < data.length; i++) {
                            ddlItems.push({
                                value: data[i].value,
                                text: data[i].name
                            });
                        }
                        input.picklist({
                            displaytype: 'select'
                            , required: true
                            , items: ddlItems
                        });
                    });
                    break;
                case "float":
                    //console.log('float', 11111111111111);
                    input.bind("keyup", function () { FilterDialog.validateInput(this, "float"); });
                    break;
                case "money":
                    input.bind('keyup', function () { FilterDialog.digitsValue(this) });
                    break;
                case "int":
                    input.bind('keyup', function () { FilterDialog.digitsValue(this) });
                    break;
                case "datetime":
                    input.unbind('keyup');
                    input.removeAttr('data-num');

                    if ($.inArray(dataval, FilterDialog.showDatepicker) != -1) {
                        input.addClass('datepicker');
                        $('.datepicker').datepicker({
                            autoclose: true
                            , clearBtn: true
                            , format: "yyyy-mm-dd"
                            , language: "zh-CN"
                        });
                    } else {
                        input.removeClass('datepicker');
                        input.attr('data-num', true);
                        input.replaceWith(input.clone().bind('keyup', function () { FilterDialog.digitsValue(this) }));
                    }
                    break;
                default:
                    input.removeProp('disabled');
                    break;
            }
        }
    }
};