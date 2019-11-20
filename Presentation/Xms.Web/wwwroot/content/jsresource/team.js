function setMembershipFilter() {
    var tempobj = { "FilterOperator": 0, "Conditions": [{ "AttributeName": "lk_tms_systemuserid.teamid", "Operator": 0, "Values": [Xms.Page.PageContext.RecordId] }], "Filters": [] }
    Xms.Page.getControl('users').setfilter(tempobj).refresh();
}
//添加团队成员
function AddTeamMembership() {
    Xms.Web.OpenDialog('/entity/RecordsDialog?entityname=systemuser&singlemode=false&inputid=team', 'AddTeamMembership_Callback');
}
function AddTeamMembership_Callback(result, inputid) {
    console.log(result, inputid);
    var ids = [];
    $(result).each(function (i, n) {
        ids.push(n.id);
    });
    var obj = { teamid: Xms.Page.PageContext.RecordId, userid: ids };
    Xms.Web.Post('/api/team/addmembers', obj, false, function (response) {
        Xms.Web.Toptip(response.content);
        if (response.IsSuccess) location.reload();
    });
}
//移除团队成员
function RemoveTeamMembership() {
    $('.subgrid[id=users] .datatable')
    Xms.Web.Post('/api/team/removemembers', obj, false, function (response) {
        Xms.Web.Toptip(response.content);
        if (response.IsSuccess) location.reload();
    });
}

//分派安全角色
function AssignRolesToTeam() {
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
    Xms.Web.OpenDialog('/security/RolesDialog?singlemode=false', 'AssignRolesToTeamCallback', { teamid: id })
}
function AssignRolesToTeamCallback(data, model) {
    console.log(data, model);
    var ids = [];
    $(data).each(function (i, n) {
        ids.push(n.id);
    });
    Xms.Web.Post('/security/AssignRolesToTeam', { teamid: model.teamid, roleid: ids }, false, function (response) {
        Xms.Web.Toast(response.IsSuccess, response.Content);
    });
}