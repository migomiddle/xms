/*确定保存时调用的方式*/
function saveAttribute(data) {
    try {
        if (data.status != 1) {
            alert(data.msg);//失败时不关闭 attributeModal 所以用 alert
        } else {
            $("#attributeModal").modal("hide");
            mAlert(data.msg);
            //刷新加载样式，体验不太好 万一未保存设计
            //location.reload();
        }
    } catch (e) {
        alert(data.msg);
    }
}

//-----条件设置--strat----------------
function _id(id) {
    return !id ? null : document.getElementById(id);
}
function trim(str) {
    return (str + '').replace(/(\s+)$/g, '').replace(/^\s+/g, '');
}

function fnCheckExp(text) {
    //检查公式
    if (text.indexOf("(") >= 0) {
        var num1 = text.split("(").length;
        var num2 = text.split(")").length;
        if (num1 != num2) {
            return false;
        }
    }
    return true;
}
/**
 * 增加左括号表达式，会断行
 */
function fnAddLeftParenthesis(id) {
    var oObj = _id('conList_' + id);
    var current = 0;
    if (oObj.options.length > 0) { //检查是否有条件
        for (var i = 0; i < oObj.options.length; i++) {
            if (oObj.options[i].selected) {
                current = oObj.selectedIndex;
                break;
            }
        }
        if (current == 0) {
            current = oObj.options.length - 1;
        }
    } else { //有条件才能添加左括号表达式
        alert("请先添加条件，再选择括号");
        return;
    }
    var sText = oObj.options[current].text, sValue = oObj.options[current].value;
    //已经有条件的话
    if ((trim(sValue).substr(-3, 3) == 'AND') || (trim(sValue).substr(-2, 2) == 'OR')) {
        alert("无法编辑已经存在关系的条件");
        return;
    }
    var sRelation = _id('relation_' + id).value;
    if (sValue.indexOf('(') >= 0) {
        if (!fnCheckExp(sValue)) {
            alert("条件表达式书写错误,请检查括号匹配");
            return;
        } else {
            sValue = sValue + " " + sRelation;
            sText = sText + " " + sRelation;
        }
    } else {
        sValue = sValue + " " + sRelation;
        sText = sText + " " + sRelation;
    }
    oObj.options[current].value = sValue;
    oObj.options[current].text = sText;
    // $('#conList_'+id+' option').eq(current).text(sText)
    $('#conList_' + id).append('<option value="( ">( </option>');

    /* var oMyop = document.createElement('option');
     oMyop.text = "( ";
     var nPos = oObj.options.length;
     oObj.appendChild(oMyop,nPos);*/
}
/**
 * 增加右括号表达式
 */
function fnAddRightParenthesis(id) {
    var oObj = _id('conList_' + id);
    var current = 0;
    if (oObj.options.length > 0) {
        for (var i = 0; i < oObj.options.length; i++) {
            if (oObj.options[i].selected) {
                current = oObj.selectedIndex;
                break;
            }
        }
        if (current == 0) {
            current = oObj.options.length - 1;
        }
    } else {
        alert("请先添加条件，再选择括号");
        return;
    }
    var sText = oObj.options[current].text, sValue = oObj.options[current].value;
    if ((trim(sValue).substr(-3, 3) == 'AND') || (trim(sValue).substr(-2, 2) == 'OR')) {
        alert("无法编辑已经存在关系的条件");
        return;
    }
    if ((trim(sValue).length == 1)) {
        alert("请添加条件");
        return;
    }
    if (!fnCheckExp(sValue)) {
        sValue = sValue + ")";
        sText = sText + ")";
    }
    oObj.options[current].value = sValue;
    oObj.options[current].text = sText;
}
function fnAddConditions(id) {
    var sField = $('#field_' + id).val(), sField_text = $('#field_' + id).find('option:selected').text(), sCon = $('#condition_' + id).val(), sValue = $('#item_value_' + id).val();

    var bAdd = true;
    if (sField !== '' && sCon !== '' && sValue !== '') {
        var oObj = _id('conList_' + id);

        if (oObj.length > 0) {
            var sLength = oObj.options.length;
            var sText = oObj.options[sLength - 1].text;
            if (!fnCheckExp(sText)) {
                bAdd = false;
            }
        }
        if (sValue.indexOf("'") >= 0) {
            alert("值中不能含有'号");
            return;
        }
        var sNewText = "'" + sField + "' " + sCon + " '" + sValue + "'";
        var sNewText_text = "'" + sField_text + "' " + sCon + " '" + sValue + "'";
        for (var i = 0; i < oObj.options.length; i++) {
            if (oObj.options[i].value.indexOf(sNewText) >= 0) {
                alert("条件重复");
                return;
            }
        }

        var sRelation = $('#relation_' + id).val();

        if (bAdd) {
            //var oMyop = document.createElement('option');
            var nPos = oObj.options.length;
            //oMyop.text = sNewText_text;
            // oMyop.value = sNewText;
            //oObj.appendChild(oMyop,nPos);
            $('#conList_' + id).append('<option value="' + sNewText + '">' + sNewText_text + '</option>');
            if (nPos > 0) {
                oObj.options[nPos - 1].text += "  " + sRelation;
                oObj.options[nPos - 1].value += "  " + sRelation;
            }
        } else {
            if (trim(oObj.options[sLength - 1].text).length == 1) {
                oObj.options[sLength - 1].text += sNewText_text;
                oObj.options[sLength - 1].value += sNewText;
            } else {
                oObj.options[sLength - 1].text += " " + sRelation + " " + sNewText_text;
                oObj.options[sLength - 1].value += " " + sRelation + " " + sNewText;
            }
        }
    } else {
        alert("请补充完整条件");
        return;
    }
}
function fnDelCon(id) {
    var oObj = _id('conList_' + id);
    var maxOpt = oObj.options.length;
    if (maxOpt < 0) maxOpt = 0;

    for (var i = 0; i < oObj.options.length; i++) {
        if (oObj.options[i].selected) {
            if ((i + 1) == maxOpt) {
                if (typeof oObj.options[i - 1] !== 'undefined') {
                    oObj.options[i - 1].text = oObj.options[i - 1].text.replace(/(AND|OR)$/, '');
                    oObj.options[i - 1].value = oObj.options[i - 1].value.replace(/(AND|OR)$/, '');
                }
            }
            oObj.removeChild(oObj.options[i]);
            i--;
        }
    }
}
function fnClearCon(id) {
    $('#conList_' + id).html('');
}

//根据基本信息的下一步骤，设置《条件设置》tab的条件列表
function fnSetCondition() {
    if ($("#process_multiple option:selected").length <= 0) {
        $('#tab_attrJudge').hide();
    } else {
        var ids = '';
        $('#ctbody').html('');
        $('#tab_attrJudge').show();
        $("#process_multiple option").each(function () {
            if ($(this).val() > 0 && $(this).attr("selected")) {
                var id = $(this).val(),
                    text = $(this).text(),
                    node = $('#tpl').html();

                var s = node.replace(/\@a/g, id);
                if (id != 0) {
                    text = '<span class="badge badge-inverse">' + id + '</span><br/>' + text;
                }
                s = s.replace(/\@text/g, text);
                s = "<tr>" + s + "<tr>";
                $('#ctbody').append(s);
                ids += id + ',';

                if (_out_condition_data) {
                    $.each(_out_condition_data, function (i, n) {
                        if (i == id && _id('conList_' + i)) {
                            $('#conList_' + i).append(n.condition);
                            $('#process_in_desc_' + i).val(n.condition_desc);
                        }
                    });
                }
                /*
                //flow_id 是流程设计的ID，  process_id 是步骤ID
                if(get_con_url)
                {
                    $.post(get_con_url,{"flow_id":flow_id,"process_id":process_id},function(data){
                        $.each(data,function(i,n){
                            if(i==id && _id('conList_'+i )){
                                $('#conList_'+i).append(n.condition);
                                $('#process_in_desc_'+i).val(n.condition_desc);
                            }
                        })
                    },'json');
                }*/
            }
        });
        if (ids) {
            $("#process_condition").val(ids);
        }
    }
}

//-----条件设置--end----------------

$(function () {
    //TAB
    $('#attributeTab a').click(function (e) {
        e.preventDefault();
        $(this).tab('show');
        if ($(this).attr("href") == '#attrJudge') {
            //加载下一步数据 处理 决策项目
        }
    })

    //步骤类型
    $('input[name="process_type"]').on('click', function () {
        if ($(this).val() == 'is_child') {
            $('#current_flow').hide();
            $('#child_flow').show();
        } else {
            $('#current_flow').show();
            $('#child_flow').hide();
        }
    });
    //返回步骤
    $('input[name="child_after"]').on('click', function () {
        if ($(this).val() == 2) {
            $("#child_back_id").show();
        } else {
            $("#child_back_id").hide();
        }
    });

    //步骤select 2
    $('#process_multiple').multiselect2side({
        selectedPosition: 'left',
        moveOptions: true,
        labelTop: '最顶',
        labelBottom: '最底',
        labelUp: '上移',
        labelDown: '下移',
        labelSort: '排序',
        labelsx: '<i class="icon-ok"></i> 下一步步骤',
        labeldx: '<i class="icon-list"></i> 备选步骤',
        autoSort: false,
        autoSortAvailable: true,
        minSize: 7
    });

    //选人方式
    $("#auto_person_id").on('change', function () {
        var apid = $(this).val();
        if (apid > 0) {
            $('#auto_unlock_id').show();
        } else {
            $('#auto_unlock_id').hide();
        }
        if (apid == 4)//指定用户
        {
            $("#auto_person_4").show();
        } else {
            $("#auto_person_4").hide();
        }
        if (apid == 5)//指定角色
        {
            $("#auto_person_5").show();
        } else {
            $("#auto_person_5").hide();
        }
    });

    /*---------表单字段 start---------*/
    //可写字段
    function write_click(e) {
        var id = $(e).attr('key');
        if (!$(e).attr('disabled')) {
            if ($(e).attr('checked')) {
                $('#secret_' + id).attr({ 'disabled': true, 'checked': false });
            } else {
                $('#secret_' + id).removeAttr('disabled').attr('checked', false);
            }
        }
    }
    //保密字段
    function secret_click(e) {
        var id = $(e).attr('key');

        if (!$(e).attr('disabled')) {
            if ($(e).attr('checked')) {
                $('#write_' + id).attr({ 'disabled': true, 'checked': false });
            } else {
                $('#write_' + id).removeAttr('disabled').attr('checked', false);
            }
        }
    }
    //checkbox全选及反选操作
    function icheck(ac, op) {
        if (ac == 'write') {
            $("input[name='write_fields[]']").each(function () {
                if (this.disabled !== true) {
                    this.checked = op;
                }
                write_click(this);
            })
        } else if (ac == 'secret') {
            $("input[name='secret_fields[]']").each(function () {
                if (this.disabled !== true) {
                    this.checked = op;
                }
                secret_click(this);
            })
        }
    }

    $('#write').click(function () {
        if ($(this).attr('checked')) {
            icheck('write', true);
            $('#secret').attr({ 'disabled': true, 'checked': false });
            $('#check').attr('checked', false).removeAttr('disabled');
        } else {
            icheck('write', false);
            $('#secret').attr('checked', false).removeAttr('disabled');
            $('#check').attr({ 'disabled': true, 'checked': false });
        }
    })
    $('#secret').click(function () {
        if ($(this).attr('checked')) {
            icheck('secret', true)
            $('#write').attr({ 'disabled': true, 'checked': false });
        } else {
            icheck('secret', false);
            $('#write').attr('checked', false).removeAttr('disabled');
        }
    })

    $("input[name='write_fields[]']").click(function () {
        write_click(this);
        $('#write').removeAttr('disabled');
        if ($('#write').attr('checked') == true) {
            $('#write').attr('checked', false)
        }
    })
    $("input[name='secret_fields[]']").click(function () {
        secret_click(this);
        $('#secret').removeAttr('disabled');
        if ($('#secret').attr('checked') == true) {
            $('#secret').attr('checked', false)
        }
    })
    /*---------表单字段 end---------*/

    /*样式*/
    $('.colors li').click(function () {
        var self = $(this);
        if (!self.hasClass('active')) {
            self.siblings().removeClass('active');
        }
        var color = self.attr('org-data') ? self.attr('org-data') : '';

        var parentDiv = self.parents(".colors");
        var orgBind = parentDiv.attr("org-bind");
        if (orgBind == 'style_icon') {
            /*$("#"+orgBind).css({ color:'#fff',background: color });*/
            $("#" + orgBind).val(color);
            $("#style_icon_preview").attr("class", color + " icon-white");
        } else//颜色
        {
            $("#" + orgBind).css({ color: '#fff', background: color });
            $("#" + orgBind).val(color);
        }
        self.addClass('active');
    });

    //表单提交前检测
    $("#flow_attribute").submit(function () {
        //条件检测
        var cond_data = $("#process_condition").val();
        if (cond_data !== '') {
            var pcarr = cond_data.split(',');
            for (var i = 0; i < pcarr.length; i++) {
                if (pcarr[i] !== '') {
                    var obj = _id('conList_' + pcarr[i]);
                    if (obj.length > 0) {
                        var constr = '';
                        for (var j = 0; j < obj.options.length; j++) {
                            constr += obj.options[j].value + '@leipi@';
                            if (!fnCheckExp(constr)) {
                                alert("条件表达式书写错误,请检查括号匹配");
                                $('#condition').click();
                                return false;
                            }
                        }
                        _id('process_in_set_' + pcarr[i]).value = constr;
                    } else {
                        _id('process_in_set_' + pcarr[i]).value = '';
                    }
                }
            }
        }
    });

    //条件设置
    fnSetCondition();
});