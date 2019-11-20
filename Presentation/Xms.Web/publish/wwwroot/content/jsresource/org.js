function DeleteBusinessUnit() {
    var event = event || window.event || arguments.callee.caller.arguments[0];
    var target = $('#datatable');
    Xms.Web.SelectingRow(target, false, true);
    var id = Xms.Web.GetTableSelected(target);
    Xms.Web.Del(id, '/org/deletebusinessunit', false, getFunction(target.attr('data-refresh')));
}