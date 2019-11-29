var Customer = {
    Locked: function () {
        var id = [];
        if (Xms.Page && Xms.Page.PageContext && Xms.Page.PageContext.RecordId) {
            id.push(Xms.Page.PageContext.RecordId);
        }
        else {
            var target = $('#datatable');
            id = Xms.Web.GetTableSelected(target);
        }
        if (!id || id.length == 0) {
            Xms.Web.Toast(LOC_NOTSPECIFIED_RECORD, 'error');
            return;
        }
        var obj = { entityid: Xms.Page.PageContext.EntityId, data: {} };
        obj.data.id = Xms.Page.PageContext.RecordId;
        obj.data.statuscode = 3;
        obj.data = JSON.stringify(obj.data);
        Xms.Web.Post('/api/data/update', obj, false, function (response) {
            Xms.Web.Toptip(response.content);
            if (response.IsSuccess) location.reload(true);
        });
    }
    , UnLocked: function () {
        var id = [];
        if (Xms.Page && Xms.Page.PageContext && Xms.Page.PageContext.RecordId) {
            id.push(Xms.Page.PageContext.RecordId);
        }
        else {
            var target = $('#datatable');
            id = Xms.Web.GetTableSelected(target);
        }
        if (!id || id.length == 0) {
            Xms.Web.Toast(LOC_NOTSPECIFIED_RECORD, 'error');
            return;
        }
        var obj = { entityid: Xms.Page.PageContext.EntityId, data: {} };
        obj.data.id = Xms.Page.PageContext.RecordId;
        obj.data.statuscode = 1;
        obj.data = JSON.stringify(obj.data);
        Xms.Web.Post('/api/data/update', obj, false, function (response) {
            Xms.Web.Toptip(response.content);
            if (response.IsSuccess) location.reload(true);
        });
    }
};
var Lead = {
    TransToCustomer: function () {
        var obj = { entityid: Xms.Page.PageContext.EntityId, data: {} };
        obj.data.id = Xms.Page.PageContext.RecordId;
        obj.data.statuscode = 2;
        obj.data = JSON.stringify(obj.data);
        Xms.Web.Post('/api/data/update', obj, false, function (response) {
            //Xms.Web.Toptip(response.content);
            if (response.IsSuccess) {
                //生成客户及联系人信息
                Xms.Web.Post('/api/data/create/map', { sourceentityname: 'lead', targetentityname: 'customer', sourcerecordid: Xms.Page.PageContext.RecordId }, false, function (response) {
                    Xms.Web.Toptip(response.content);
                    if (response.IsSuccess) {
                        location.href = ORG_SERVERURL + '/entity/create?entityid=' + response.Extra.entityid + '&recordid=' + response.Extra.id;
                    }
                });
            }
            else {
                Xms.Web.Toptip(response.content);
            }
        });
    }
    , Cancel: function () {
        var obj = { entityid: Xms.Page.PageContext.EntityId, data: {} };
        obj.data.id = Xms.Page.PageContext.RecordId;
        obj.data.statuscode = 3;
        obj.data = JSON.stringify(obj.data);
        Xms.Web.Post('/api/data/update', obj, false, function (response) {
            Xms.Web.Toptip(response.content);
            if (response.IsSuccess) location.reload(true);
        });
    }
    , ReActive: function () {
        var obj = { entityid: Xms.Page.PageContext.EntityId, data: {} };
        obj.data.id = Xms.Page.PageContext.RecordId;
        obj.data.statuscode = 1;
        obj.data = JSON.stringify(obj.data);
        Xms.Web.Post('/api/data/update', obj, false, function (response) {
            Xms.Web.Toptip(response.content);
            if (response.IsSuccess) location.reload(true);
        });
    }
};