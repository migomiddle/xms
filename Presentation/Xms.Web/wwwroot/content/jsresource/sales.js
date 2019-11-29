var Opportunity = {
    Reopen: function () {
        var obj = { entityid: Xms.Page.PageContext.EntityId, data: {} };
        obj.data.id = Xms.Page.PageContext.RecordId;
        obj.data.statuscode = 1;
        obj.data = JSON.stringify(obj.data);
        Xms.Web.Post('/api/data/update', obj, false, function (response) {
            Xms.Web.Toptip(response.content);
            if (response.IsSuccess) location.reload(true);
        });
    }
    , Close: function (statuscode) {
        var obj = { entityid: Xms.Page.PageContext.EntityId, data: {} };
        obj.data.id = Xms.Page.PageContext.RecordId;
        obj.data.statuscode = statuscode;
        obj.data = JSON.stringify(obj.data);
        Xms.Web.Post('/api/data/update', obj, false, function (response) {
            Xms.Web.Toptip(response.content);
            if (response.IsSuccess) location.reload(true);
        });
    }
};
var SalesOrder = {
    Complete: function () {
        var obj = { entityid: Xms.Page.PageContext.EntityId, data: {} };
        obj.data.id = Xms.Page.PageContext.RecordId;
        obj.name = Xms.Page.PageContext.EntityName;
        obj.data.statuscode = 3;
        obj.data = JSON.stringify(obj.data);
        Xms.Web.Post('/api/data/update', obj, false, function (response) {
            Xms.Web.Toptip(response.content);
            if (response.IsSuccess) location.reload(true);
        });
    }
    , Cancel: function () {
        var obj = { entityid: Xms.Page.PageContext.EntityId, data: {} };
        obj.data.id = Xms.Page.PageContext.RecordId;
        obj.name = Xms.Page.PageContext.EntityName;
        obj.data.statuscode = 5;
        obj.data = JSON.stringify(obj.data);
        Xms.Web.Post('/api/data/update', obj, false, function (response) {
            Xms.Web.Toptip(response.content);
            if (response.IsSuccess) location.reload(true);
        });
    }
    , LookupAddress: function () {
        var attr = Xms.Page.getAttribute('customerid');
        var v = attr.getValue();
        if (v == null) {
            Xms.Web.Toptip(LOC_NOTSPECIFIED_OBJECT + ': ' + attr.Target.attr('data-localizedname'));
            $('#' + attr.Target.prop('id') + '_text').focus();
            return;
        }
        var customerid = v.id;
        var target = $('<div>'
            + '<div class="form-group container-fluid"><label class="col-sm-2">' + LOC_SALESORDER_SHIPTOADDRESS + '</label><div class="input-group input-group-sm">'
            + '<input type="text" id="dialog_shiptoaddressid_text" data-type="lookup" name="dialog_shiptoaddressid_text" class="form-control colinput lookup input-sm" />'
            + '<input type="hidden" id="dialog_shiptoaddressid" data-type="lookup" name="dialog_shiptoaddressid" class="form-control colinput" />'
            + '</div></div>'
            + '<div class="form-group container-fluid"><label class="col-sm-2">' + LOC_SALESORDER_BILLTOADDRESS + '</label><div class="input-group input-group-sm">'
            + '<input type="text" id="dialog_billtoaddressid_text" data-type="lookup" name="dialog_billtoaddressid_text" class="form-control colinput lookup input-sm" />'
            + '<input type="hidden" id="dialog_billtoaddressid" data-type="lookup" name="dialog_billtoaddressid" class="form-control colinput" />'
            + '</div></div>'
            + '</div>');
        target.dialog({
            title: '<span class="glyphicon glyphicon-info-sign"></span> ' + LOC_SALESORDER_LOADCUSTOMERADDRESS
            , onClose: function () {
                $(this).dialog("destroy");
                $(document.body).css("padding-right", 0);
            }
            , buttons: [
                {
                    text: "<span class=\"glyphicon glyphicon-remove\"></span> " + LOC_DIALOG_CLOSE,
                    classed: "btn-default",
                    click: function () {
                        $(this).dialog("destroy");
                        $(document.body).css("padding-right", 0);
                    }
                }
                , {
                    text: "<span class=\"glyphicon glyphicon-ok\"></span> " + LOC_DIALOG_OK,
                    classed: "btn-info",
                    click: function () {
                        var shiptoid = target.find('#dialog_shiptoaddressid').val();
                        if (shiptoid) {
                            Xms.Web.GetJson('/api/data/retrieve/customeraddress/' + shiptoid, null, function (response) {
                                if (response.content) {
                                    Xms.Page.getAttribute('shipto_country').setValue(response.content.country);
                                    Xms.Page.getAttribute('shipto_city').setValue(response.content.city);
                                    Xms.Page.getAttribute('shipto_stateorprovince').setValue(response.content.province);
                                    Xms.Page.getAttribute('shipto_street').setValue(response.content.street);
                                    Xms.Page.getAttribute('shipto_contactname').setValue(response.content.contactidname);
                                    Xms.Page.getAttribute('shipto_telephone').setValue(response.content.telephone1);
                                    Xms.Page.getAttribute('shipto_fax').setValue(response.content.fax);
                                    Xms.Page.getAttribute('shipto_postalcode').setValue(response.content.postalcode);
                                }
                            });
                        }
                        var billtoid = target.find('#dialog_billtoaddressid').val();
                        if (billtoid) {
                            Xms.Web.GetJson('/api/data/retrieve/customeraddress/' + billtoid, null, function (response) {
                                if (response.content) {
                                    Xms.Page.getAttribute('billto_country').setValue(response.content.country);
                                    Xms.Page.getAttribute('billto_city').setValue(response.content.city);
                                    Xms.Page.getAttribute('billto_stateorprovince').setValue(response.content.province);
                                    Xms.Page.getAttribute('billto_street').setValue(response.content.street);
                                    Xms.Page.getAttribute('billto_contactname').setValue(response.content.contactidname);
                                    Xms.Page.getAttribute('billto_telephone').setValue(response.content.telephone1);
                                    Xms.Page.getAttribute('billto_fax').setValue(response.content.fax);
                                    Xms.Page.getAttribute('billto_postalcode').setValue(response.content.postalcode);
                                }
                            });
                        }
                        $(this).dialog("destroy");
                        $(document.body).css("padding-right", 0);
                    }
                }
            ]
        });
        var filter = { "Conditions": [{ "AttributeName": "customerid", "Operator": 0, "Values": [customerid] }] }
        target.find('#dialog_shiptoaddressid_text').lookup({
            disabled: true,
            dialog: function () {
                var data = { filter: filter, entityname: 'customeraddress' };
                Xms.Web.OpenDialog('/entity/RecordsDialog?singlemode=true&inputid=dialog_shiptoaddressid', 'SalesOrder.LookupAddressCallback', data);
            }
            , clear: function () {
                $('#dialog_shiptoaddressid').val('');
                $('#dialog_shiptoaddressid_text').val('');
            }
        });
        target.find('#dialog_billtoaddressid_text').lookup({
            disabled: true,
            dialog: function () {
                var data = { filter: filter, entityname: 'customeraddress' };
                Xms.Web.OpenDialog('/entity/RecordsDialog?singlemode=true&inputid=dialog_billtoaddressid', 'SalesOrder.LookupAddressCallback', { filter: filter });
            }
            , clear: function () {
                $('#dialog_billtoaddressid').val('');
                $('#dialog_billtoaddressid_text').val('');
            }
        });
    }
    , LookupAddressCallback: function (result, inputid) {
        if (!result || result.length == 0) return;
        $('#' + inputid).val(result[0].id);
        $('#' + inputid + '_text').val(result[0].name);
    }
    , GetProducts: function () {
        var target = $('<div><div class="form-group container-fluid"><div class="input-group input-group-sm">'
            + '<input type="text" id="dialog_opportunityid_text" data-type="lookup" name="dialog_opportunityid_text" class="form-control colinput lookup input-sm" />'
            + '<input type="hidden" id="dialog_opportunityid" data-type="lookup" name="dialog_opportunityid" class="form-control colinput" />'
            + '</div></div></div>');
        target.dialog({
            title: '<span class="glyphicon glyphicon-info-sign"></span> ' + LOC_SALESORDER_LOADOPPORTUNITYPRODUCTS
            , onClose: function () {
                $(this).dialog("destroy");
                $(document.body).css("padding-right", 0);
            }
            , buttons: [
                {
                    text: "<span class=\"glyphicon glyphicon-remove\"></span> " + LOC_DIALOG_CLOSE,
                    classed: "btn-default",
                    click: function () {
                        $(this).dialog("destroy");
                        $(document.body).css("padding-right", 0);
                    }
                }
                , {
                    text: "<span class=\"glyphicon glyphicon-ok\"></span> " + LOC_DIALOG_OK,
                    classed: "btn-info",
                    click: function () {
                        var id = $('#dialog_opportunityid').val();
                        if (id) {
                            var filter = { "Conditions": [{ "AttributeName": "opportunityid", "Operator": 0, "Values": [id] }] };
                            var queryObj = { EntityName: 'opportunityproduct', Criteria: filter, ColumnSet: { allcolumns: true } };
                            var data = JSON.stringify({ "query": queryObj, "isAll": true });
                            Xms.Web.GetJson('/api/data/Retrieve/Multiple', data, function (response) {
                                console.log(response);
                                if (response.content && response.content.length > 0) {
                                    var datas = [];
                                    $(response.content).each(function (i, n) {
                                        var obj = {};
                                        obj.productid = n.productid;
                                        obj.quantity = n.quantity;
                                        obj.price = n.price;
                                        obj.amount = n.amount;
                                        obj.discountamount = n.discountamount;
                                        obj.salesorderid = Xms.Page.PageContext.RecordId;
                                        obj.currencyid = n.currencyid;
                                        obj.opportunityproductid = n.opportunityproductid;
                                        datas.push(obj);
                                    });
                                    console.log(datas);
                                    Xms.Web.Post('/api/data/create', { entityname: 'salesorderdetail', data: JSON.stringify(datas) }, false, function (response) {
                                        Xms.Web.Toptip(response.content);
                                        if (response.IsSuccess) Xms.Page.getControl('products').refresh();
                                    });
                                }
                            }, null, null, 'post');
                            $(this).dialog("destroy");
                            $(document.body).css("padding-right", 0);
                        }
                    }
                }
            ]
        });
        target.find('#dialog_opportunityid_text').lookup({
            disabled: true,
            dialog: function () {
                var data = { entityname: 'opportunity' };
                Xms.Web.OpenDialog('/entity/RecordsDialog?singlemode=true&inputid=dialog_opportunityid', 'SalesOrder.GetProductsCallback', data);
            }
            , clear: function () {
                $('#dialog_opportunityid').val('');
                $('#dialog_opportunityid_text').val('');
            }
        });
    }
    , GetProductsCallback: function (result, inputid) {
        if (!result || result.length == 0) return;
        $('#dialog_opportunityid').val(result[0].id);
        $('#dialog_opportunityid_text').val(result[0].name);
    }
    , GetQuoteProducts: function () {
        var target = $('<div><div class="form-group container-fluid"><div class="input-group input-group-sm">'
            + '<input type="text" id="dialog_quoteid_text" data-type="lookup" name="dialog_quoteid_text" class="form-control colinput lookup input-sm" />'
            + '<input type="hidden" id="dialog_quoteid" data-type="lookup" name="dialog_quoteid" class="form-control colinput" />'
            + '</div></div></div>');
        target.dialog({
            title: '<span class="glyphicon glyphicon-info-sign"></span> ' + LOC_SALESORDER_LOADQUOTEPRODUCTS
            , onClose: function () {
                $(this).dialog("destroy");
                $(document.body).css("padding-right", 0);
            }
            , buttons: [
                {
                    text: "<span class=\"glyphicon glyphicon-remove\"></span> " + LOC_DIALOG_CLOSE,
                    classed: "btn-default",
                    click: function () {
                        $(this).dialog("destroy");
                        $(document.body).css("padding-right", 0);
                    }
                }
                , {
                    text: "<span class=\"glyphicon glyphicon-ok\"></span> " + LOC_DIALOG_OK,
                    classed: "btn-info",
                    click: function () {
                        var id = $('#dialog_quoteid').val();
                        if (id) {
                            var filter = { "Conditions": [{ "AttributeName": "quoteid", "Operator": 0, "Values": [id] }] };
                            var queryObj = { EntityName: 'opportunityproduct', Criteria: filter, ColumnSet: { allcolumns: true } };
                            var data = JSON.stringify({ "query": queryObj, "isAll": true });
                            Xms.Web.GetJson('/api/data/Retrieve/Multiple', data, function (response) {
                                console.log(response);
                                if (response.content && response.content.length > 0) {
                                    var datas = [];
                                    $(response.content).each(function (i, n) {
                                        var obj = {};
                                        obj.productid = n.productid;
                                        obj.quantity = n.quantity;
                                        obj.price = n.price;
                                        obj.amount = n.amount;
                                        obj.discountamount = n.discountamount;
                                        obj.salesorderid = Xms.Page.PageContext.RecordId;
                                        obj.currencyid = n.currencyid;
                                        obj.opportunityproductid = n.opportunityproductid;
                                        datas.push(obj);
                                    });
                                    console.log(datas);
                                    Xms.Web.Post('/api/data/create', { entityname: 'salesorderdetail', data: JSON.stringify(datas) }, false, function (response) {
                                        Xms.Web.Toptip(response.content);
                                        if (response.IsSuccess) Xms.Page.getControl('products').refresh();
                                    });
                                }
                            }, null, null, 'post');
                            $(this).dialog("destroy");
                            $(document.body).css("padding-right", 0);
                        }
                    }
                }
            ]
        });
        target.find('#dialog_quoteid_text').lookup({
            disabled: true,
            dialog: function () {
                var data = { entityname: 'opportunity' };
                Xms.Web.OpenDialog('/entity/RecordsDialog?singlemode=true&inputid=dialog_quoteid', 'SalesOrder.GetQuoteProductsCallback', data);
            }
            , clear: function () {
                $('#dialog_quoteid').val('');
                $('#dialog_quoteid_text').val('');
            }
        });
    }
    , GetQuoteProductsCallback: function (result, inputid) {
        if (!result || result.length == 0) return;
        $('#dialog_quoteid').val(result[0].id);
        $('#dialog_quoteid_text').val(result[0].name);
    }
};
var Quote = {
    LookupAddress: function () {
        var attr = Xms.Page.getAttribute('customerid');
        var v = attr.getValue();
        if (v == null) {
            Xms.Web.Toptip(LOC_NOTSPECIFIED_OBJECT + ': ' + attr.Target.attr('data-localizedname'));
            $('#' + attr.Target.prop('id') + '_text').focus();
            return;
        }
        var customerid = v.id;
        var target = $('<div>'
            + '<div class="form-group container-fluid"><label class="col-sm-2">' + LOC_SALESORDER_SHIPTOADDRESS + '</label><div class="input-group input-group-sm">'
            + '<input type="text" id="dialog_shiptoaddressid_text" data-type="lookup" name="dialog_shiptoaddressid_text" class="form-control colinput lookup input-sm" />'
            + '<input type="hidden" id="dialog_shiptoaddressid" data-type="lookup" name="dialog_shiptoaddressid" class="form-control colinput" />'
            + '</div></div>'
            + '<div class="form-group container-fluid"><label class="col-sm-2">' + LOC_SALESORDER_BILLTOADDRESS + '</label><div class="input-group input-group-sm">'
            + '<input type="text" id="dialog_billtoaddressid_text" data-type="lookup" name="dialog_billtoaddressid_text" class="form-control colinput lookup input-sm" />'
            + '<input type="hidden" id="dialog_billtoaddressid" data-type="lookup" name="dialog_billtoaddressid" class="form-control colinput" />'
            + '</div></div>'
            + '</div>');
        target.dialog({
            title: '<span class="glyphicon glyphicon-info-sign"></span> ' + LOC_SALESORDER_LOADCUSTOMERADDRESS
            , onClose: function () {
                $(this).dialog("destroy");
                $(document.body).css("padding-right", 0);
            }
            , buttons: [
                {
                    text: "<span class=\"glyphicon glyphicon-remove\"></span> " + LOC_DIALOG_CLOSE,
                    classed: "btn-default",
                    click: function () {
                        $(this).dialog("destroy");
                        $(document.body).css("padding-right", 0);
                    }
                }
                , {
                    text: "<span class=\"glyphicon glyphicon-ok\"></span> " + LOC_DIALOG_OK,
                    classed: "btn-info",
                    click: function () {
                        var shiptoid = target.find('#dialog_shiptoaddressid').val();
                        if (shiptoid) {
                            Xms.Web.GetJson('/api/data/retrieve/customeraddress/' + shiptoid, null, function (response) {
                                if (response.content) {
                                    Xms.Page.getAttribute('shipto_country').setValue(response.content.country);
                                    Xms.Page.getAttribute('shipto_city').setValue(response.content.city);
                                    Xms.Page.getAttribute('shipto_stateorprovince').setValue(response.content.province);
                                    Xms.Page.getAttribute('shipto_street').setValue(response.content.street);
                                    Xms.Page.getAttribute('shipto_contactname').setValue(response.content.contactidname);
                                    Xms.Page.getAttribute('shipto_telephone').setValue(response.content.telephone1);
                                    Xms.Page.getAttribute('shipto_fax').setValue(response.content.fax);
                                    Xms.Page.getAttribute('shipto_postalcode').setValue(response.content.postalcode);
                                }
                            });
                        }
                        var billtoid = target.find('#dialog_billtoaddressid').val();
                        if (billtoid) {
                            Xms.Web.GetJson('/api/data/retrieve/customeraddress/' + billtoid, null, function (response) {
                                if (response.content) {
                                    Xms.Page.getAttribute('billto_country').setValue(response.content.country);
                                    Xms.Page.getAttribute('billto_city').setValue(response.content.city);
                                    Xms.Page.getAttribute('billto_stateorprovince').setValue(response.content.province);
                                    Xms.Page.getAttribute('billto_street').setValue(response.content.street);
                                    Xms.Page.getAttribute('billto_contactname').setValue(response.content.contactidname);
                                    Xms.Page.getAttribute('billto_telephone').setValue(response.content.telephone1);
                                    Xms.Page.getAttribute('billto_fax').setValue(response.content.fax);
                                    Xms.Page.getAttribute('billto_postalcode').setValue(response.content.postalcode);
                                }
                            });
                        }
                        $(this).dialog("destroy");
                        $(document.body).css("padding-right", 0);
                    }
                }
            ]
        });
        var filter = { "Conditions": [{ "AttributeName": "customerid", "Operator": 0, "Values": [customerid] }] }
        target.find('#dialog_shiptoaddressid_text').lookup({
            disabled: true,
            dialog: function () {
                var data = { filter: filter, entityname: 'customeraddress' };
                Xms.Web.OpenDialog('/entity/RecordsDialog?singlemode=true&inputid=dialog_shiptoaddressid', 'Quote.LookupAddressCallback', data);
            }
            , clear: function () {
                $('#dialog_shiptoaddressid').val('');
                $('#dialog_shiptoaddressid_text').val('');
            }
        });
        target.find('#dialog_billtoaddressid_text').lookup({
            disabled: true,
            dialog: function () {
                var data = { filter: filter, entityname: 'customeraddress' };
                Xms.Web.OpenDialog('/entity/RecordsDialog?singlemode=true&inputid=dialog_billtoaddressid', 'Quote.LookupAddressCallback', data);
            }
            , clear: function () {
                $('#dialog_billtoaddressid').val('');
                $('#dialog_billtoaddressid_text').val('');
            }
        });
    }
    , LookupAddressCallback: function (result, inputid) {
        if (!result || result.length == 0) return;
        $('#' + inputid).val(result[0].id);
        $('#' + inputid + '_text').val(result[0].name);
    }
    , GetProducts: function () {
        var target = $('<div><div class="form-group container-fluid"><div class="input-group input-group-sm">'
            + '<input type="text" id="dialog_opportunityid_text" data-type="lookup" name="dialog_opportunityid_text" class="form-control colinput lookup input-sm" />'
            + '<input type="hidden" id="dialog_opportunityid" data-type="lookup" name="dialog_opportunityid" class="form-control colinput" />'
            + '</div></div></div>');
        target.dialog({
            title: '<span class="glyphicon glyphicon-info-sign"></span> ' + LOC_SALESORDER_LOADOPPORTUNITYPRODUCTS
            , onClose: function () {
                $(this).dialog("destroy");
                $(document.body).css("padding-right", 0);
            }
            , buttons: [
                {
                    text: "<span class=\"glyphicon glyphicon-remove\"></span> " + LOC_DIALOG_CLOSE,
                    classed: "btn-default",
                    click: function () {
                        $(this).dialog("destroy");
                        $(document.body).css("padding-right", 0);
                    }
                }
                , {
                    text: "<span class=\"glyphicon glyphicon-ok\"></span> " + LOC_DIALOG_OK,
                    classed: "btn-info",
                    click: function () {
                        var id = $('#dialog_opportunityid').val();
                        if (id) {
                            var filter = { "Conditions": [{ "AttributeName": "opportunityid", "Operator": 0, "Values": [id] }] };
                            var queryObj = { EntityName: 'opportunityproduct', Criteria: filter, ColumnSet: { allcolumns: true } };
                            var data = JSON.stringify({ "query": queryObj, "isAll": true });
                            Xms.Web.GetJson('/api/data/Retrieve/Multiple', data, function (response) {
                                console.log(response);
                                if (response.content && response.content.length > 0) {
                                    var datas = [];
                                    $(response.content).each(function (i, n) {
                                        var obj = {};
                                        obj.productid = n.productid;
                                        obj.quantity = n.quantity;
                                        obj.price = n.price;
                                        obj.amount = n.amount;
                                        obj.discountamount = n.discountamount;
                                        obj.quoteid = Xms.Page.PageContext.RecordId;
                                        obj.currencyid = n.currencyid;
                                        obj.opportunityproductid = n.opportunityproductid;
                                        datas.push(obj);
                                    });
                                    console.log(datas);
                                    Xms.Web.Post('/api/data/create', { entityname: 'quotedetail', data: JSON.stringify(datas) }, false, function (response) {
                                        Xms.Web.Toptip(response.content);
                                        if (response.IsSuccess) Xms.Page.getControl('details').refresh();
                                    });
                                }
                            }, null, null, 'post');
                            $(this).dialog("destroy");
                            $(document.body).css("padding-right", 0);
                        }
                    }
                }
            ]
        });
        target.find('#dialog_opportunityid_text').lookup({
            disabled: true,
            dialog: function () {
                var data = { entityname: 'opportunity' };
                Xms.Web.OpenDialog('/entity/RecordsDialog?singlemode=true&inputid=dialog_opportunityid', 'SalesOrder.GetProductsCallback', data);
            }
            , clear: function () {
                $('#dialog_opportunityid').val('');
                $('#dialog_opportunityid_text').val('');
            }
        });
    }
    , GetProductsCallback: function (result, inputid) {
        if (!result || result.length == 0) return;
        $('#dialog_opportunityid').val(result[0].id);
        $('#dialog_opportunityid_text').val(result[0].name);
    }
    , Active: function () {
        //先查询
        //...
        var obj = { entityid: Xms.Page.PageContext.EntityId, data: {} };
        obj.data.id = Xms.Page.PageContext.RecordId;
        obj.data.statuscode = 2;
        obj.data = JSON.stringify(obj.data);
        Xms.Web.Post('/api/data/update', obj, false, function (response) {
            Xms.Web.Toptip(response.content);
            if (response.IsSuccess) location.reload(true);
        });
    }
    , Revised: function () {
        var target = $('<div><div class="form-group container-fluid">'
            + '<input type="text" id="dialog_reviseremark" name="reviseremark" class="form-control colinput lookup input-sm" placeholder="修订备注" />'
            + '</div></div>');
        target.dialog({
            title: '<span class="glyphicon glyphicon-info-sign"></span> 修订备注'
            , onClose: function () {
                $(this).dialog("destroy");
                $(document.body).css("padding-right", 0);
            }
            , buttons: [
                {
                    text: "<span class=\"glyphicon glyphicon-remove\"></span> " + LOC_DIALOG_CLOSE,
                    classed: "btn-default",
                    click: function () {
                        $(this).dialog("destroy");
                        $(document.body).css("padding-right", 0);
                    }
                }
                , {
                    text: "<span class=\"glyphicon glyphicon-ok\"></span> " + LOC_DIALOG_OK,
                    classed: "btn-info",
                    click: function () {
                        var obj = { entityid: Xms.Page.PageContext.EntityId, data: {} };
                        obj.data.id = Xms.Page.PageContext.RecordId;
                        obj.data.statuscode = 5;//已修订
                        obj.data.reviseremark = $(this).find('#dialog_reviseremark').val();
                        obj.data = JSON.stringify(obj.data);
                        Xms.Web.Post('/api/data/update', obj, false, function (response) {
                            Xms.Web.Toptip(response.content);
                            if (response.IsSuccess) {
                                Quote.Copy();
                                //location.reload(true);
                            }
                        });
                        $(this).dialog("destroy");
                    }
                }
            ]
        });
    }
    , Copy: function () {
        Xms.Web.Post('/api/data/create/map', { sourceentityname: 'quote', targetentityname: 'quote', sourcerecordid: Xms.Page.PageContext.RecordId }, false, function (response) {
            Xms.Web.Toptip(response.content);
            if (response.IsSuccess) {
                location.href = ORG_SERVERURL + '/entity/create?entityid=' + response.Extra.entityid + '&recordid=' + response.Extra.id;
            }
        });
    }
    , AppendOrder: function () {
        Xms.Web.Post('/api/data/create/map', { sourceentityname: 'quote', targetentityname: 'salesorder', sourcerecordid: Xms.Page.PageContext.RecordId }, false, function (response) {
            Xms.Web.Toptip(response.content);
            if (response.IsSuccess) {
                location.href = ORG_SERVERURL + '/entity/create?entityid=' + response.Extra.entityid + '&recordid=' + response.Extra.id;
            }
        });
    }
    , Close: function () {
    }
};