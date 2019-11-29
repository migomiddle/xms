/*var calculationJson = {
	trigger:[
		{selector:'#discountpercentage,#discountamount',event:'click change'},
		{selector:'#totaltax',event:'keyup'}
	],
	formula:[
		{operator:'',field:'discountpercentage'},
		{operator:'+',field:'discountamount'},
		{operator:'+',constant:'15'},
		{operator:'+',formula:[
			{operator:'',field:'totaltax'},
			{operator:'+',field:'freightamount'}
		]},
	],
	resultField:['billto_contactname']
};*/
//计算公式
function Formula() { }
Formula.prototype = {
    //json解析成公式
    formulaTransform: function (formula) {
        var formulaStr = '';
        for (var i = 0; i < formula.length; i++) {
            var formulaEle = formula[i];
            var eleStr = '';
            if (formulaEle.field) {
                eleStr = 'getFieldValue("' + formulaEle.field + '")';
            } if (formulaEle.constant) {
                eleStr = parseFloat(formulaEle.constant);
            } else if (formulaEle.formula) {
                eleStr = '(' + this.formulaTransform(formulaEle.formula) + ')';
            }

            formulaStr = formulaEle.operator ? formulaStr + formulaEle.operator + eleStr : eleStr;
        }
        return formulaStr;
    },
    //调用公式的方法
    transfer: function (calculationJson) {
        var formulaTransformStr = this.formulaTransform(calculationJson.formula);
        //封装执行运算
        var operation = function () {
            //console.log('formulaResult', formulaResult);
            var formulaResult = eval(formulaTransformStr);
            if (!isNaN(formulaResult) && /^[0-9]*\.[0-9]{3,9}/.test(formulaResult + '')) {
                formulaResult = formulaResult.toFixed(2);
            } else if (isNaN(formulaResult) || formulaResult == Infinity) {
                formulaResult = '';
            }
            for (var i = 0; i < calculationJson.resultField.length; i++) {
                Xms.Page.getAttribute(calculationJson.resultField[i]).setValue(formulaResult);
            }
        }
        //触发运算
        for (var i = 0; i < calculationJson.trigger.length; i++) {
            $('body').on(calculationJson.trigger[i].event, calculationJson.trigger[i].selector, operation);
        }
    }
}
//取字段值
function getFieldValue(field) {
    return Xms.Page.getAttribute(field).getValue() ? parseFloat(Xms.Page.getAttribute(field).getValue().replace(/,/g, '')) : 0;
}

//表单配公式规则
function formFormula() {
    //重置操作控件
    var resetControls = function () {
        $('#formulaPOperator .btn,#formulaPField,#formulaPConstant').attr('disabled', 'disabled');
        $('#formulaPOperator .btn').removeClass('active');
        $('#formulaPConstant').val('');
    }
    //点击添加公式元素
    $('#formulaPAdd a').click(function () {
        var $formulaPGroup = $('#formulaPGroup .formulaP-item-group.active').length > 0 ? $('#formulaPGroup .formulaP-item-group.active').children('.group-main') : $('#formulaPGroup');
        var operator = $formulaPGroup.children('.formulaP-item').length > 0 ? '<font>+</font>' : '<font></font>';
        var html = '';
        switch ($(this).attr('data-type')) {
            case 'field':
                html = '<div class="formulaP-item" data-type="field" data-field="">' + operator + '<span>未设置</span><em>×</em></div>';
                break;
            case 'constant':
                html = '<div class="formulaP-item" data-type="constant">' + operator + '<span>0</span><em>×</em></div>';
                break;
            case 'formula':
                html = '<div class="formulaP-item formulaP-item-group" data-type="formula">' + operator + '（<div class="group-main"></div>）<em>×</em></div>';
                break;
            default:
                break;
        }
        $formulaPGroup.append(html);
    });
    //删除公式元素
    $('#formulaPGroup').on('click', '.formulaP-item > em', function (event) {
        event.stopPropagation();
        $(this).parent().remove();
        resetControls();
    });
    //选中公式元素
    $('#formulaPGroup').on('click', '.formulaP-item', function (event) {
        event.stopPropagation();
        $('#formulaPGroup .formulaP-item').not(this).removeClass('active');
        $(this).toggleClass('active');
        resetControls();
        //解锁可操作控件
        if ($(this).hasClass('active')) {
            $('#formulaPOperator .btn[data-key="' + $(this).children('font').text() + '"]').addClass('active');
            $('#formulaPOperator .btn').removeAttr('disabled');
            switch ($(this).attr('data-type')) {
                case 'field':
                    $('#formulaPField').removeAttr('disabled').val($(this).attr('data-field'));
                    break;
                case 'constant':
                    $('#formulaPConstant').removeAttr('disabled').val($(this).children('span').text());
                    break;
                default:
                    break;
            }
        }
    });
    //点击运算符
    $('#formulaPOperator').on('click', '.btn', function () {
        $(this).addClass('active').siblings().removeClass('active');
        $('#formulaPGroup .formulaP-item.active > font').text($(this).attr('data-key'));
    });
    //更改常数
    $('#formulaPConstant').on('change keyup', function () {
        $('#formulaPGroup .formulaP-item.active > span').text($(this).val() ? $(this).val() : 0);
    });
    //更改字段
    $('#formulaPField').change(function () {
        $('#formulaPGroup .formulaP-item.active').attr('data-field', $(this).val());
        $('#formulaPGroup .formulaP-item.active > span').text($(this).find('option:selected').text());
    });
}

//加载公式可选字段
function loadAttributesFormula(attributes) {
    //单据头公式字段
    $('#formulaPField').html('');
    if (attributes) {
        $.each(attributes, function (i, n) {
            if (n.attributetypename == 'int' || n.attributetypename == 'float' || n.attributetypename == 'money') {
                var _html = '<option value="' + n.name.toLowerCase() + '">' + n.localizedname + '</option>';
                $('#formulaPField').append(_html);
            }
        });
    }
}
//json解析成html
function formulaTransformHtml(formula) {
    var formulaHtml = '';
    for (var i = 0; i < formula.length; i++) {
        var formulaEle = formula[i];
        var eleHtml = '';
        if (formulaEle.field) {
            eleHtml = '<div class="formulaP-item" data-type="field" data-field="' + formulaEle.field + '"><font>' + formulaEle.operator + '</font><span>' + formulaEle.fieldText + '</span><em>×</em></div>';
        } if (formulaEle.constant) {
            eleHtml = '<div class="formulaP-item" data-type="constant"><font>' + formulaEle.operator + '</font><span>' + formulaEle.constant + '</span><em>×</em></div>';
        } else if (formulaEle.formula) {
            eleHtml = '<div class="formulaP-item formulaP-item-group" data-type="formula"><font>' + formulaEle.operator + '</font>（<div class="group-main">' + formulaTransformHtml(formulaEle.formula) + '</div>）<em>×</em></div>';
        }

        formulaHtml += eleHtml;
    }
    return formulaHtml;
}
//html解析成json
function formulaTransformJson(itemGroup) {
    var formulaPJson = [];
    $(itemGroup).children('.formulaP-item').each(function () {
        var formulaPItem = { operator: $(this).children('font').text() };
        switch ($(this).attr('data-type')) {
            case 'field':
                if ($(this).attr('data-field')) {
                    formulaPItem.field = $(this).attr('data-field');
                    formulaPItem.fieldText = $(this).children('span').text();
                    break;
                } else {
                    return;
                }

            case 'constant':
                formulaPItem.constant = $(this).children('span').text();
                break;
            case 'formula':
                formulaPItem.formula = formulaTransformJson($(this).children('.group-main'));
                break;
            default:
                break;
        }

        formulaPJson.push(formulaPItem);
    });
    return formulaPJson;
}
//显示公式规则
function showFormulaRule() {
    $('#formulaPOperator .btn,#formulaPField,#formulaPConstant').attr('disabled', 'disabled');
    $('#formulaPConstant').val('');
    $('#formulaPGroup').html('');
    if ($('.selected').attr('data-formulajson')) {
        var formulaJson = JSON.parse($('.selected').attr('data-formulajson'));
        $('#formulaPGroup').html(formulaTransformHtml(formulaJson.formula));
    }
}
//点击确定保存公式
function formulaSave() {
    var selected = $('.selected');
    var calculationJson = { resultField: [selected.attr('data-name').toLowerCase()] };
    calculationJson.formula = formulaTransformJson($('#formulaPGroup'));
    //公式触发器
    var triggerSelector = [];
    $('#formulaPGroup').find('.formulaP-item').each(function () {
        if ($(this).attr('data-type') == 'field') {
            triggerSelector.push('#' + $(this).attr('data-field'));
        }
    });
    calculationJson.trigger = [{ selector: triggerSelector.join(','), event: 'keyup change' }];
    if (calculationJson.formula.length == 0) {
        selected.removeAttr('data-formulajson');
    } else {
        selected.attr('data-formulajson', JSON.stringify(calculationJson));
    }
}