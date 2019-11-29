; (function (root, un) {
    function changeToArray(arr) {
        var res = [];
        if (!$.isArray(arr)) {
            if (arr.indexOf(',') > -1) {
                res = arr.split(',')
            } else {
                res = [arr];
            }
        } else {
            res = arr;
        }
        return res;
    }

    function editFilter() {
        var _modal = $('#FilterModal');
        _modal.modal('show');
        var filters = Xms.Fetch.FilterExpression();
        var pocondis = [];
        var $Conditions = $('#Conditions');
        var _conditions = { Conditions: [], LogicalOperator: "and" };//已设置的条件
        if ($Conditions.val() != "") {
            _conditions = JSON.parse($Conditions.val());
        }
        //$.each(_conditions, function (key, n) {
        var logical = _conditions.LogicalOperator;
        $('input[name="connection-logical"][value="' + logical + '"]').prop('checked', true);

        $.each(_conditions.Conditions, function (ii, nn) {
            var CompareAttributeName = null;
            var filtertype = nn.CompareAttributeName ? nn.CompareAttributeName : 'value';
            var _values = nn.Values;
            _values = changeToArray(_values);
            CompareAttributeName = nn.CompareAttributeName || null;
            pocondis.push({
                AttributeName: nn.AttributeName,
                Operator: nn.Operator,
                CompareAttributeName: CompareAttributeName,
                Values: _values
            });
        });
        // });

        filters.Conditions = pocondis;
        var postData = {
            entityid: $('#EntityId').val(),
            filter: filters
        }
        //var filter = '[{"EntityName":"_mappointdckgdmig7","AttributeName":"Address","Operator":"0","Values":"t"}]'
        Xms.Web.Post('/filter/simplefiltersection', postData, false, function (res) {
            //var testdom = $('<div></div>');
            // testdom.html(res);
            $('#FilterModalConditions').html(res);
        }, false, false, false);
    }
    function saveFilter() {
        var _modal = $('#FilterModal');
        var activeCon = $('.condition-item');
        if (activeCon.length == 0) { modal.modal('hide'); return false; }
        var LogicalOperator = $('#connection-logical');
        var res = [];
        var tempobj = {};
        activeCon.each(function (i, n) {
            var item = $(this);
            var attrtype = item.find('.filter-filed-name option:selected').parent().attr('data-type');
            if (attrtype == "filed") {
                var AttributeName = item.find('.filter-filed-name').val();
                var type = item.find('.filter-filed-name option:selected').attr('data-type');
                if (type == 'lookup' || type == 'owner' || type == 'customer') {
                    var Values = item.find('input[name="value"]').attr('data-value');
                } else {
                    var Values = item.find('input[name="value"]').val();
                }
            } else {
                var AttributeName = item.find('.filter-filed-name option:selected').attr('data-name') + '.' + item.find('.filter-filed-name').val();
                var Values = item.find('input[name="value"]').attr('data-value');
            }
            var Operator = item.find('.filter-filed-Operator').val();
            var itemEntityid = item.find('.filter-filed-name option:selected').attr('data-entityid');
            if ((type == 'lookup' || type == 'owner' || type == 'customer') && attrtype == "filed") {
                itemEntityid = item.find('.filter-filed-name option:selected').attr('data-referencedentityid');
            }
            var isfiletype = item.find('.filter-filed-type').prop('disabled');
            if (!isfiletype) {
                var CompareAttributeName = null;
                var filtertype = item.find('.filter-filed-type').val() || "value";
                if (filtertype == "value") {
                    CompareAttributeName = null;
                } else {
                    CompareAttributeName = item.find('.filter-filed-type').val();
                    Values = [];
                }
            } else {
                Values = [];
                CompareAttributeName = null;
            }
            res.push({
                AttributeName: AttributeName,
                Operator: Operator,
                CompareAttributeName: CompareAttributeName,
                Values: Values ? changeToArray(Values) : ''
            });

            console.log('tempobj', res);
        });
        tempobj.Conditions = res;
        tempobj.LogicalOperator = LogicalOperator.val();
        if (res.length > 0) {
            $('#Conditions').val(JSON.stringify(tempobj));
        }
        _modal.modal('hide');
    }
    var businessModel = {};
    businessModel.saveFilter = saveFilter;
    businessModel.editFilter = editFilter;
    root.businessModel = businessModel
})(window);