//@ sourceURL=entity.js
//记录操作相关
function CreateRecord(newWindow, parameters) {
    var url = location.href;
    if (url.indexOf('entity/create') > 0) {
        url = $.setUrlParam(url, 'entityid', Xms.Page.PageContext.EntityId);
        url = $.setUrlParam(url, 'formid', Xms.Page.PageContext.TargetFormId);
        url = $.setUrlParam(url, 'recordid', null);
        newWindow = false;
    } else {
        url = ORG_SERVERURL + '/entity/create?entityid=' + Xms.Page.PageContext.EntityId + (Xms.Page.PageContext.TargetFormId ? '&formid=' + Xms.Page.PageContext.TargetFormId : '');
    }
    if (parameters) url += (url.indexOf('?') > 0 ? '&' + parameters : '?' + parameters);
    if (newWindow) {
        //Xms.Web.OpenWindow(url);
        entityIframe('show', url);
    }
    else {
        location.href = url;
    }
}
function EditRecord(newWindow) {
    var event = event || window.event || arguments.callee.caller.arguments[0];
    var target = $('#datatable');
    Xms.Web.SelectingRow(event.target, false, true);
    var id = Xms.Web.GetTableSelected(target);
    var url = '/entity/edit?entityid=' + Xms.Page.PageContext.EntityId + '&recordid=' + id;
    if (newWindow) {
        Xms.Web.OpenWindow(ORG_SERVERURL + url);
    }
    else {
        location.href = ORG_SERVERURL + url;
    }
}
function CopyRecord() {
    var url = ORG_SERVERURL + '/entity/create?entityid=' + Xms.Page.PageContext.EntityId + '&copyid=' + Xms.Page.PageContext.RecordId;
    location.href = url;
}
function DeleteRecord() {
    var event = event || window.event || arguments.callee.caller.arguments[0];
    var target = $('#datatable');
    Xms.Web.SelectingRow(event.target, false, true);
    var id = Xms.Web.GetTableSelected(target);
    Xms.Web.Del(id, '/api/data/delete?entityid=' + Xms.Page.PageContext.EntityId, false, function (response) {
        if (response.IsSuccess) {
            getFunction(target.attr('data-refresh'))();
            return;
        }
    });
}
function DeleteOneRecord() {
    var id = Xms.Page.PageContext.RecordId;
    if (!id) {
        Xms.Web.Toast(LOC_SAVERECORD_FIRST, false);
        return;
    }
    Xms.Web.Del([id], '/api/data/delete?entityid=' + Xms.Page.PageContext.EntityId, false, function (response) {
        console.log(response);
        if (response.IsSuccess) {
            Xms.Web.Event.publish('refresh');
            Xms.Web.CloseWindow();
        }
    });
}
function Save(callback) {
    var event = event || window.event || arguments.callee.caller.arguments[0];
    callback && callback();
    if (typeof formSaveSubGrid == 'function') {
        if (formSaveSubGrid()) {
            if (event && event.target) {
                $(event.target).parents('form:first').trigger('submit');
            } else {
                $('form:first').trigger('submit');
            }
        }
    } else {
        $(event.target).parents('form:first').trigger('submit');
    }
}
function SaveAndNew() {
    var event = event || window.event || arguments.callee.caller.arguments[0];
    if (typeof formSaveSubGrid == 'function') {
        if (formSaveSubGrid()) {
            _formSaveAction = Xms.FormSaveAction.saveAndNew;
            if (event && event.target) {
                $(event.target).parents('form:first').trigger('submit');
            } else {
                $('form:first').trigger('submit');
            }
        }
    } else {
        _formSaveAction = Xms.FormSaveAction.saveAndNew;
        $(event.target).parents('form:first').trigger('submit');
    }
}
//状态更改
function SetRecordState(state) {
    var id = [];
    if (Xms.Page && Xms.Page.PageContext && Xms.Page.PageContext.RecordId) {
        id.push(Xms.Page.PageContext.RecordId);
    }
    else {
        var target = $('#datatable');
        id = Xms.Web.GetTableSelected(target);
    }
    if (!id || id.length == 0) {
        Xms.Web.Toast(LOC_NOTSPECIFIED_RECORD, false);
        return;
    }
    var data = {};
    data.entityid = Xms.Page.PageContext.EntityId;
    data.recordid = id;
    data.state = state;
    Xms.Web.OpenDialog('/entity/setrecordstate', null, data);
}
function SetRecordStatus(status) {
    var obj = { entityid: Xms.Page.PageContext.EntityId, data: {} };
    obj.data.id = Xms.Page.PageContext.RecordId;
    obj.name = Xms.Page.PageContext.EntityName;
    obj.data.statuscode = status;
    obj.data = JSON.stringify(obj.data);
    Xms.Web.Post('/api/data/update', obj, false, function (response) {
        Xms.Web.Toptip(response.content);
        if (response.IsSuccess) {
            Xms.Web.Event.publish('refresh');
            location.reload(true);
        }
    });
}
//共享
function Sharing(entityid, objectid) {
    if (!objectid) {
        Xms.Web.Toast(LOC_NOTSPECIFIED_RECORD, false);
        return;
    }
    if (!entityid) {
        Xms.Web.Alert(false, LOC_NOTSPECIFIED_OBJECTTYPE);
        return;
    }
    var data = { entityid: entityid, objectid: objectid };
    Xms.Web.OpenDialog('/entity/sharing', null, data);
}
function Shared(objectid, target) {
    if (!objectid) {
        Xms.Web.Alert(false, LOC_NOTSPECIFIED_RECORD);
        return;
    }
    if (!target || target.length == 0) {
        Xms.Web.Alert(false, LOC_NOTSPECIFIED_OBJECT);
        return;
    }
    var data = { objectid: objectid, target: target };
    Xms.Web.Post('/api/data/share', data, false, function (response) {
        if (response.IsSuccess) {
            Xms.Web.Toptip(response.Content);
            return;
        }
        Xms.Web.Alert(false, response.Content);
    });
}
//分派
function Assigning(entityid, objectid) {
    if (!objectid) {
        objectid = Xms.Web.GetTableSelected($('#datatable'));
    }
    if (!objectid || objectid.length == 0) {
        Xms.Web.Toast(LOC_NOTSPECIFIED_RECORD, false);
        return;
    }
    if (!entityid) {
        Xms.Web.Toast(LOC_NOTSPECIFIED_OBJECTTYPE, false);
        return;
    }
    if (Object.prototype.toString.call(objectid) != '[object Array]') {
        objectid = [objectid];
    }
    var data = { entityid: entityid, objectid: objectid };
    Xms.Web.OpenDialog('/entity/assigning', null, data);
}
//合并
function MergeRecords() {
    var target = $('#datatable');
    var id = Xms.Web.GetTableSelected(target);
    if (!id || id.length == 0) {
        Xms.Web.Toast(LOC_NOTSPECIFIED_RECORD, false);
        return;
    }
    if (id.length != 2) {
        Xms.Web.Toast(LOC_SPECIFIED_TWORECORDS, false);
        return;
    }
    Xms.Web.OpenDialog('/entity/merge?entityid=' + Xms.Page.PageContext.EntityId + '&recordid1=' + id[0] + '&recordid2=' + id[1]);
}
//下推单据
function AppendRecord(entityid, recordid) {
    if (!recordid) {
        Xms.Web.Alert(false, LOC_NOTSPECIFIED_RECORD);
        return;
    }
    if (!entityid) {
        Xms.Web.Alert(false, LOC_NOTSPECIFIED_OBJECTTYPE);
        return;
    }
    Xms.Web.OpenDialog('/entity/appendrecord?entityid=' + entityid + '&recordid=' + recordid);
}
//列表相关
function GetRowData($row) {
    var target = $row.parents('table:first');
    var data = new Array();
    target.find('thead>tr>th[data-name]').each(function (i, n) {
        var self = $(n);
        var dataCell = $row.find('td:eq(' + self.index() + ')');
        data[i] = dataCell.text().trim();
    });
    return data;
}
//工作流相关
function StartWorkFlow(entityid, recordid, callback) {
    if (!entityid || !recordid) {
        Xms.Web.Alert(false, LOC_SAVERECORD_FIRST);
        return;
    }
    Xms.Web.OpenDialog('/flow/startworkflow?entityid=' + entityid + '&recordid=' + recordid, callback);
}
function WorkFlowProcessing(entityid, recordid, callback) {
    if (!entityid || !recordid) {
        Xms.Web.Alert(false, LOC_SAVERECORD_FIRST);
        return;
    }
    Xms.Web.OpenDialog('/flow/workflowprocessing?entityid=' + entityid + '&recordid=' + recordid, null, null, callback);
}
function WorkFlowProcessDetail(entityid, recordid, callback) {
    if (!entityid || !recordid) {
        Xms.Web.Alert(false, LOC_SAVERECORD_FIRST);
        return;
    }
    Xms.Web.OpenDialog('/flow/WorkFlowInstanceDetail?entityid=' + entityid + '&recordid=' + recordid, callback);
}
function WorkFlowCancel(entityid, recordid, callback) {
    if (!entityid || !recordid) {
        Xms.Web.Alert(false, LOC_SAVERECORD_FIRST);
        return;
    }
    Xms.Web.Confirm(LOC_CONFIRM_OPERATION_TITLE, '确定要撤消吗？',
        function () {
            Xms.Web.Post('/api/workflow/cancel?entityid=' + entityid + '&recordid=' + recordid, null, false, function (response) {
                console.log('callback', callback);
                if (response.IsSuccess) {
                    Xms.Web.Toptip(response.Content, true);
                    Xms.Web.Event.publish('refresh');
                    if (typeof (callback) == "function") callback(response);
                    else location.reload(true);
                    return;
                }
                Xms.Web.Alert(false, response.Content);
            })
        }
    );
}
function changeNoticeReaded() {
    var target = $('#datatable');
    var id = Xms.Web.GetTableSelected(target);
    $(id).each(function (i) {
        var obj = { entityid: Xms.Page.PageContext.EntityId, data: {} };
        obj.data.id = this;
        obj.data.isread = 1;
        obj.data = JSON.stringify(obj.data);
        Xms.Web.Post('/api/data/update', obj, false, function (response) {
            if (i == id.length - 1) {
                Xms.Web.Event.publish("noticeChange");
                rebind();
            }
        }, false, false, true);
    });
}