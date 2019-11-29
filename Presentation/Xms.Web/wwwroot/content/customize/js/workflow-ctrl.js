var startPointAdded = false;
var endPointAdded = false;
var pointStepOrder = 0;
var shutPoints = new mapPoint('shutPoints');//保存分流节点
function pointBindEvent(ele) {
    ele.box.on('mappoint.dblclick', function (e, param) {
        editPoint(ele, param);
        // console.log(param);
    }).on('connection.dblclick', function (e, param) {
        console.log(param);
        /*if(confirm('yes')){
            jsPlumb.detach(param.connection)
        }*/
        editConnection(this, param);
    }).on('connection.delete', function (e, param) {
        console.log(param);
        delConnection(this, param);
    });
    if (ele.param.nodetype && ele.param.nodetype == TYPE_START || ele.param.nodetype == TYPE_END) return false;
    ele.box.on('mappoint.delete', function () {
        Xms.Web.Confirm("提示", '是否要删除该步骤?', function () {
            //删除数据和对应的流转条件
            var sourceid = ele.name;
            //找出链接到自己的流转条件
            var gconns = [];
            $.each(eleConnectionController.list, function (key, item) {
                if (item.name.indexOf(sourceid) > -1) {
                    gconns.push(item);
                }
            });
            console.log(gconns)
            $.each(gconns, function (i, n) {
                var _tarid = n.param.targetId;
                var _sourid = n.param.sourceId;
                var _point_i = elePointController.indexOf(_tarid);
                if (_point_i > -1) {
                    var _opoint = elePointController.list[_point_i];
                    //删除链接出去的线的数据
                    var workflows = $('#' + _tarid).data().point.param.Conditions;
                    if (workflows && workflows.length) {
                        workflows = $.grep(workflows, function (item, key) {
                            if ((item.PrevStepId == getTargetByName(sourceid).old || item.PrevStepId == sourceid) && (item.NextStepId == getTargetByName(_tarid).old || item.NextStepId == _tarid)) {
                                return false;
                            } else {
                                return true;
                            }
                        });
                        $('#' + _tarid).data().point.param.Conditions = workflows;
                        console.log('剩余的线', workflows)
                    }
                    //删除链接过来的线的数据
                    var sourTarget = $('#' + _sourid).data().point.param.Conditions;
                    if (sourTarget && sourTarget.length) {
                        sourTarget = $.grep(sourTarget, function (item, key) {
                            if ((item.NextStepId == getTargetByName(sourceid).old || item.NextStepId == sourceid) && (item.PrevStepId == getTargetByName(_sourid).old || item.PrevStepId == _sourid)) {
                                return false;
                            } else {
                                return true;
                            }
                        });
                        $('#' + _sourid).data().point.param.Conditions = sourTarget;
                    }
                }
                jsPlumb.detach(n.param.c);
                eleConnectionController.removeByName(n.name);
            });

            //删除
            console.log(eleConnectionController)
            ele.box.remove();
            elePointController.remove(ele);
            // console.log(elePointController)
        });
    })
    //ele.box.bind('mousedown', function (e) {
    //    //console.log(e)
    //    if (e.preventDefault) { e.preventDefault(); }
    //    if (e.which == 3) { //右键绑定
    //        $(this).contextMenu('pointMenu', {
    //            bindings: {
    //                "menu_delPoint": function () {
    //                    console.log(elePointController);
    //                    ele.box.trigger('mappoint.delete');
    //                }
    //            }
    //        });

    //    }
    //});
}
function openAddPointsModal(point) {
    $('#addPointCount').data().point = point;
    $('#addPointCount').modal('show');
}
function addPointShuntCounts() {
    var point = $('#addPointCount').data().point;
    var count = $('#pointShuntCounts').val();
    if (isNaN(count)) {
        Xms.Web.Alert(false, '请输入数字');
        return false;
    }
    if (count > 10) {
        Xms.Web.Alert(false, '不能大于10个');
        return false;
    }
    addPointFun({ nodetype: '3' }, point, count);
    $('#addPointCount').modal('hide');
}
function addPointFun(eleparam, lastPoint, count) {
    var wrapSize = { w: $('#workflowWrap').width(), h: $('#workflowWrap').height() };
    var pos = { x: 0, y: 0 };
    var curentPoint = null;
    var hasLastPoint = elePointController.list.length > 2;
    lastPoint = lastPoint || null;

    if (hasLastPoint) {
        if (!lastPoint) {
            lastPoint = elePointController.list[elePointController.list.length - 1];
        }
        pos.x = lastPoint.box.position().left;
        pos.y = lastPoint.box.position().top;
    } else {
        if (!lastPoint) {
            lastPoint = elePointController.list[0];
        }
        pos.x = lastPoint.box.position().left;
        pos.y = lastPoint.box.position().top;
    }
    var curLastP = null;
    if (eleparam && eleparam.nodetype == TYPE_SHUNT) {
        if (lastPoint) {
            if (lastPoint.nodetype == TYPE_SHUNT) {
                //Xms.Web.Alert(false, "请先添加普通加点在添加分流节点");
                // return false;
            }
            curLastP = addShuntPoints(lastPoint, eleparam, wrapSize, count);
            //pos.x = pos.x + 170;
            //if (pos.x + 150 > wrapSize.w) {
            //    pos.x = pos.x - 170;
            //}
            //pos.y = pos.y - 50;
            //eleparam = $.extend({}, { nodetype: TYPE_SHUNT, cnName: "分流节点" }, pos, eleparam);
            //var firstP = addPoint(eleparam);

            //pos.y = pos.y + 100;
            //eleparam = $.extend({}, { nodetype: TYPE_SHUNT, cnName: "分流节点" }, pos);
            //var secondP = addPoint(eleparam);
            //var sourceId = lastPoint.box.attr('id');
            //var targetId_1 = firstP.box.attr('id');
            //var targetId_2 = secondP.box.attr('id');
            //connectPoint(sourceId, targetId_1, firstP.box);
            //connectPoint(sourceId, targetId_2, secondP.box);
        } else {
            Xms.Web.Alert(false, "请先添加节点");
            return false;
        }
    } else {
        if (lastPoint) {
            pos.x = pos.x + 170;
            if (pos.x + 150 > wrapSize.w) {
                pos.x = pos.x - 170;
            }
            pos.y = pos.y + 50;
            eleparam = $.extend({}, { nodetype: TYPE_NARMAL, cnName: "普通节点" }, pos, eleparam);
            curentPoint = addPoint(eleparam);
            var sourceId = lastPoint.box.attr('id');
            var targetId = curentPoint.box.attr('id');
            var box = curentPoint.box;
            connectPoint(sourceId, targetId, box);
            curLastP = curentPoint;
        } else {
            curentPoint = addPoint(eleparam);
            curLastP = curentPoint;
        }
    }

    //获取结束节点
    var _endpoint = elePointController.list[1];
    var curSourceid = curLastP.box.attr('id');
    var curTargetid = _endpoint.box.attr('id');
    connectPoint(curSourceid, curTargetid, curLastP.box);
}
function addShuntPoints(lastPoint, eleparam, wrapSize, count) {
    count = count || 2;
    var pointSize = { w: 150, y: 36 };
    var pointPos = {}
    var _points = [];
    pointPos.x = lastPoint.box.position().left;
    pointPos.y = lastPoint.box.position().top;
    var margin = { left: 5, top: 5, right: 5, bottom: 5 };
    var wrapSizeCount = Math.ceil(wrapSize.w / pointSize.w);
    var ban = Math.ceil(count / 2);
    console.log('ban', ban);
    for (var i = 0; i < count; i++) {
        (function (j) {
            // setTimeout(function () {
            var pos = { x: 0, y: 0 };
            pos.x = pointPos.x - ((j - ban) * pointSize.w);
            console.log('pos.x', pointPos.x - ((j - ban) * pointSize.w));
            pos.y = pointPos.y + 70;
            //pos.x = pos.x + 170;
            console.log(pos.x + 150 > wrapSize.w)
            if (pos.x + 150 > wrapSize.w) {
                pos.x = pos.x - 170;
            }
            if (pos.x < 0) {
                pos.x = 0;
            }
            if (pos.y + 36 > wrapSize.y) {
                pos.y = pos.y - 50;
            }
            if (pos.y < 0) {
                pos.y = 0;
            }
            console.log(pos.x);
            var neleparam = $.extend({}, { nodetype: TYPE_SHUNT, cnName: "分流节点" }, pos, eleparam);

            var firstP = addPoint(neleparam);
            _points.push(firstP);
            var sourceId = lastPoint.box.attr('id');
            var targetId_1 = firstP.box.attr('id');
            connectPoint(sourceId, targetId_1, firstP.box);
            // },100)
        })(i);
    }
    return _points[count - 1];
}

//添加流程
function addPoint(eleparam, nodetype) {
    eleparam = $.extend({}, {
        cnName: '节点',
        StepOrder: '',
        Name: '',
        AuthAttributes: '',
        AllowAssign: true,
        AllowCancel: true,
        ReturnType: '3',
        HandlerIdType: 1,
        Handlers: '',
        Description: '',
        AttachmentRequired: false,
        AttachmentExts: '',
        FormId: null
    }, {
        x: 0,
        y: 0,
        title: '测试',
        nodetype: TYPE_NARMAL
    }, eleparam);

    var ele = new elePoint(null, eleparam).init().render('#workflowWrap');
    if (ele.param.nodetype == TYPE_SHUNT) {
        shutPoints.add(ele);
    }
    if (ele.param.StepOrder == "") {
        ele.param.StepOrder = pointStepOrder;
    }
    if (ele.param.NodeName == "") {
        ele.param.NodeName = 'node_' + pointStepOrder;
    }
    pointStepOrder++;
    // console.log(ele);
    namesp.push({ old: ele.param.WorkFlowStepId, newn: ele.name, point: ele });
    pointBindEvent(ele);
    return ele;
}
//获取表单列表
function getFormsList(callback) {
    var entityid = getEntityId();
    //加载forms列表
    var postParams = {
        type: 'forms' + entityid,
        data: { entityid: entityid }
    }
    Xms.Web.PageCache('workflow', '/customize/systemform/index', postParams, function (res) {
        var $formDom = $('#point-formname');
        var resItems = res.content;
        if (resItems && resItems.items) {
            var _html = [];
            $.each(resItems.items, function (i, n) {
                _html.push('<option data-formid="' + n.systemformid + '" data-solutionid="' + n.solutionid + '">' + n.name + '</option>');
            });
            $formDom.html(_html.join(''));
            callback && callback();
        }
    }, true);
}

function renderPermission(datas) {
    if (!datas || !datas.length) return false;
    var testModel = datas;
    var _html = [], $PermissionDom = $('ul.Permissions-list');
    $.each(testModel, function (key, item) {
        _html.push('<li class="Permissions-list-item" data-permission="' + item.name + '">' +
            '<div class="Permissions-item-con Permissions-item-left">' +
            '<span class="glyphicon glyphicon-star"></span>' + item.localizedname +
            '</div>' +
            '<div class="Permissions-item-con Permissions-item-right">' +
            '<label for="permissions-item-edit1' + key + '"><input type="checkbox" onclick="checkPermissionMask(this)" class="permissions-item-first" id="permissions-item-edit1' + key + '" checked name="permissions-item-check' + key + '" value="1" />查看</label>' +
            '<label for="permissions-item-edit2' + key + '"><input type="checkbox" onclick="checkPermissionMask(this)" class="permissions-item-second" name="permissions-item-check' + key + '" checked id="permissions-item-edit2' + key + '" value="2" />编辑</label>' +
            '</div>' +
            '</li>');
    });
    $PermissionDom.html(_html.join(''));
}
//获取授权字段数据
function getPermissionsData(callback) {
    var entityid = getEntityId();
    Xms.Web.GetJson('/customize/attribute/index?entityid=' + entityid, {}, function (response) {
        console.log('获取授权字段数据', response);
        if (response.content && response.content.items) {
            var datas = [];
            renderPermission(response.content.items);
            callback && callback();
        }
    }, null, null, 'post', true);
}
//获取审核者数据
function getModifyData(callback, filterData, allloaded) {
    var wrap = $('#modify-con-wrap');
    var entitynames = ['systemuser', 'team', 'post', 'job', 'roles'];
    var count = 0;
    for (var i = 0; i < entitynames.length; i++) {
        (function (type) {
            var entityname = entitynames[type];
            var filter = { "Conditions": [{ "AttributeName": entityname == 'roles' ? "statecode" : "statecode", "Operator": 0, "Values": [1] }] }
            var queryObj = { EntityName: entityname, Criteria: filter, ColumnSet: { allcolumns: true } };
            var data = JSON.stringify({ "query": queryObj, "isAll": true });
            Xms.Web.GetJson('/api/data/Retrieve/Multiple', data, function (response) {
                console.log(response);
                if (response.content && response.content.length > 0) {
                    var datas = [];
                    var select = renderModityByData(response.content, type);
                    callback && callback(select);
                    count++;
                    if (count == entitynames.length) {
                        allloaded && allloaded();
                    }
                }
            }, null, null, 'post', true);
        })(i);
    }
}
var entitynames_cn = ['用户', '团队', '岗位', '职位', '角色'];
function renderModityByData(datas, type) {
    if (!datas || !datas.length) return false;
    var _html = [], $modifyDom = $('#modify-tab' + (type));
    var select = $modifyDom.children('ul'), $res = $('#modifyResult');
    var resList = $res.children('li');
    var entitynames = ['systemuser', 'team', 'post', 'job', 'roles'];
    //if (select.attr('isloaded') == 'true' || select.attr('isloaded') == true) return select;
    if (resList.length > 0) {
        $(datas).each(function (i, n) {
            var flag = false;
            if (entitynames[type] == 'roles') {
                n['rolesid'] = n['roleid'];
            }
            resList.each(function (ii, nn) {
                if ($(nn).attr('data-value') == n[entitynames[type] + 'id']) {
                    flag = true;
                    $(nn).text(n.name + ' (' + entitynames_cn[type] + ')');
                }
            });
            if (!flag) {
                _html.push('<li class="list-group-item" data-entityname="' + entitynames[type] + '" data-value="' + n[entitynames[type] + 'id'] + '">');
                _html.push(n.name + ' (' + entitynames_cn[type] + ')');
                _html.push('</li>');
            }
        });
    } else {
        $(datas).each(function (i, n) {
            if (entitynames[type] == 'roles') {
                n['rolesid'] = n['roleid'];
            }
            _html.push('<li class="list-group-item" data-entityname="' + entitynames[type] + '" data-value="' + n[entitynames[type] + 'id'] + '">');
            _html.push(n.name + ' (' + entitynames_cn[type] + ')');
            _html.push('</li>');
        });
    }
    select.html(_html.join(''));
    $modifyDom.append(select);
    // select.attr('isloaded', true);
    return select;
}
//编辑流程
function editPoint(selected, param) {
    var modal = $('#mappointModal');
    modal.modal('show');
    modal.data().param = param;//编辑时传过来的信息
    modal.data().selecter = selected;
    var _super = param._super;
    var params = _super.param;
    var title = $('#point-name');
    var tag = $('#point-flag');
    var retype = $('#point-retype');
    var restep = $('#point-restep');
    var formname = $('#point-formname');
    //var runtarget = $('#point-runtarget');
    var desc = $('#point-desc');

    var allowcancel = $('#point-allowcancel');
    var allowassign = $('#point-allowassign');

    //初始化输入框
    restep.html('');
    title.val('');
    tag.val('');
    // runtarget.val('');
    desc.val('');
    allowcancel.prop('checked', false);
    allowassign.prop('checked', false);

    //--------赋值
    title.val(_super.param.cnName);
    tag.val(params.NodeName);
    //runtarget.val(_super.param.name);
    desc.val(_super.param.Description);
    if (_super.param.AllowAssign) {
        allowassign.prop('checked', true);
    }
    if (_super.param.AllowCancel) {
        allowcancel.prop('checked', true);
    }

    //读取form数据
    getFormsList(function () {
        formname.find('option[data-formid="' + _super.param.FormId + '"]').prop('selected', true);
    });

    /*授权字段 start */
    getPermissionsData(function () {
        var authsmodel = [];
        if (params.AuthAttributes && params.AuthAttributes != '') {
            authsmodel = JSON.parse(params.AuthAttributes);
        }
        //[{ Name: 'aaa', PermissionMask: "1" }, { Name: 'bbb', PermissionMask: "2" }, { Name: 'ccc', PermissionMask: "1" }];
        //设置授权字段
        $.each(authsmodel, function (key, item) {
            var peritem = $('.Permissions-list-item[data-permission="' + item.Name + '"]');
            if (peritem.length > 0) {
                var firs = peritem.find('input.permissions-item-first');
                var secs = peritem.find('input.permissions-item-second');
                firs.prop('checked', true);
                secs.prop('checked', true);
                if (item.PermissionMask == 1) {
                    secs.prop('checked', false);
                } else if (item.PermissionMask == 0) {
                    firs.prop('checked', false);
                    secs.prop('checked', false);
                }
            }
        });
    });
    /*授权字段     end*/
    //$('input[name="handleridtype"]', modal).prop('checked', false);
    //if (params['HandlerIdType'] && params['HandlerIdType'] != '') {
    //    $('input[name="handleridtype"][value="' + params['HandlerIdType'] + '"]', modal).prop('checked', true);
    //}
    if (params['HandlerIdType'] && params['HandlerIdType'] != '') {
        $('#usertypeSeclecter option[value="' + params['HandlerIdType'] + '"]').prop('selected', true);
    }

    //处理者
    //reset
    var $res = $('#modifyResult');
    $res.html('');
    $('#modifyHandles1,#modifyHandles2,#modifyHandles3').empty().removeAttr('isloaded');

    $('#modifyUserType>li').off('click').on('click', function () {
        $(this).siblings('li').removeClass('active').end().addClass('active');
        var targetid = $(this).children('a').attr('href');
        var _target = $(targetid);
        _target.siblings('div').removeClass('active in').addClass('fade').hide().end().addClass('active in').removeClass('fade').show();
    });
    $('#usertypeSeclecter', modal).off('change').on('change', function () {
        if ($(this).val() == 2) {
            $('.modify-section').show();
            if (params['Handlers'] && params['Handlers'] != '') {
                var _html = [];
                var reArr = JSON.parse(params['Handlers']);

                $.each(reArr, function (i, n) {
                    _html.push('<li class="list-group-item" data-entityname="' + n.type + '" data-value="' + n.id + '">');
                    _html.push('</li>');
                });
                $res.html(_html.join(''));
                modifyReSelect();
            }
            getModifyData(function (select) {
                modifySelect.call(select);
            });
        } else {
            $('.modify-section').hide();
        }
    });
    $('#usertypeSeclecter', modal).trigger('change');

    //处理驳回类型和驳回步骤
    retype.off('change').on('change', function () {
        changeReType(this, _super);
    });
    retype.find('option[value="4"]').prop('selected', true);
    if (_super.param.ReturnType) {
        retype.find('option[value="' + _super.param.ReturnType + '"]').prop('selected', true);
        retype.trigger('change');
        if (_super.param.ReturnTo) {
            restep.find('option[data-step="' + _super.param.ReturnTo + '"]').prop('selected', true);
        }
    }
    $('#attach-require').prop('checked', false);
    $('#attach-exts').val('');
    /*
    *附件
    */
    if (_super.param['AttachmentRequired']) {
        $('#attach-require').prop('checked', !!_super.param['AttachmentRequired']);
    }
    if (_super.param['AttachmentExts']) {
        $('#attach-exts').val(_super.param['AttachmentExts']);
    }
}

function modifySelect() {
    var $this = $(this);
    var $res = $('#modifyResult');
    $this.children('li').off('click').on('click', function () {
        var _par = $(this).parents('#modifyResult');
        if (_par.length > 0) {//如果已在结果列表里,则返回原来的列表;
            $this.append($(this));
        } else {
            $res.append($(this));
        }
    });
}

function modifyReSelect() {
    var $res = $('#modifyResult');
    var entitynames = ['systemuser', 'team', 'post', 'job', 'roles'];
    $('#modifyResult').children('li').off('click').on('click', function () {
        var entityname = $(this).attr('data-entityname');
        var type = 0;
        if (entityname == 'team') {
            type = 1;
        } else if (entityname == 'post') {
            type = 2;
        } else if (entityname == 'job') {
            type = 3;
        } else if (entityname == 'roles') {
            type = 4;
        }
        var $par = $('#modifyHandles' + type);
        var testPar = $(this).parents('#modifyHandles' + type);
        if (testPar.length > 0) {//如果已在结果列表里,则返回原来的列表;
            $res.append($(this));
        } else {
            $par.append($(this));
        }
    });
}

function changeReType(obj, _super) {
    var $obj = $(obj);
    var type = $obj.val(), _html = [], datas;
    var $reStep = $('#point-restep');
    datas = getReTypeData(type, _super);
    console.log(datas);
    $.each(datas, function (ii, nn) {
        _html.push('<option value="' + nn.tag + '" data-step="' + nn.steporder + '">' + nn.cnName + '</option>')
    });
    $reStep.html(_html.join(''));
}

//处理驳回步骤
function getReTypeData(type, _super) {
    var res = [];
    var obj_oldname = _super.oldName;
    var items = elePointController.list;
    if (type == 1) {//处理驳回步骤的上一步
        var citems = eleConnectionController.list;
        var tempArr = [];
        $.each(citems, function (key, item) {
            if (item.name.indexOf(_super.name) > 0) {
                tempArr.push(item);
            }
        });
        console.log(tempArr);
        $.each(tempArr, function (ii, nn) {
            $.each(items, function (key, item) {
                if (item.name == nn.param.sourceId && item.oldName != obj_oldname) {
                    res.push({ cnName: item.cnName, tag: item.oldName, steporder: item.param.StepOrder })
                }
            });
        });
    } else if (type == 2) {
        var startPid = $('.mappoint-start').attr('id');
        $.each(items, function (key, item) {
            if (item.name == startPid && item.oldName != obj_oldname) {
                res.push({ cnName: item.cnName, tag: item.oldName, steporder: item.param.StepOrder })
            }
        });
    } else if (type == 3) {
        $.each(items, function (key, item) {
            if (item.oldName == obj_oldname) return true;
            res.push({ cnName: item.cnName, tag: item.oldName, steporder: item.param.StepOrder })
        });
    } else if (type == 4) {
    }

    return res;
}

//授权字段 复选框规则
function checkPermissionMask(obj) {
    var $obj = $(obj);
    var _par = $obj.parents('.Permissions-item-right:first');
    if ($obj.hasClass('permissions-item-first')) {
        if (_par.length > 0) {
            var secc = _par.find('input.permissions-item-second');
            if ($obj.prop('checked')) {
                // secc.prop('checked', true);
            } else {
                secc.prop('checked', false);
            }
        }
    } else if ($obj.hasClass('permissions-item-second')) {
        if (_par.length > 0) {
            var firc = _par.find('input.permissions-item-first');
            if ($obj.prop('checked')) {
                firc.prop('checked', true);
            }
        }
    }
}
function savePoint() {
    var modal = $('#mappointModal');
    var param = modal.data().param;
    var selected = modal.data().selecter;
    var title = $('#point-name');
    //var tag = $('#point-flag');
    var retype = $('#point-retype>option:selected');
    var restep = $('#point-restep');
    var formname = $('#point-formname');
    //  var runtarget = $('#point-runtarget');
    var desc = $('#point-desc');
    var allowcancel = $('#point-allowcancel');
    var allowassign = $('#point-allowassign');

    var titleV = title.val();
    //var tagV = tag.val();
    var retypeV = retype.val();
    var restepV = restep.find('option:selected').attr('data-step');
    var formnameV = formname.find('option:selected').attr('data-formid');
    // var runtargetV = runtarget.val();
    var descV = desc.val();
    var allowcancelV = allowcancel.prop('checked');
    var allowassignV = allowassign.prop('checked');

    var attachRequire = $('#attach-require').prop('checked');
    var attachexts = $('#attach-exts').val();

    if (title.val() == "") {
        Xms.Web.Alert(false, "节点名称不能为空");
        return false;
    }
    //if(attachexts!='' && /^\*\.(\w+)$/g.test(attachexts)){
    //    Xms.Web.Alert(false, "附件类型格式错误");
    //    return false;
    //}

    /*授权字段 start */
    var $permiWrap = $('ul.Permissions-list');
    var $permiList = $permiWrap.children('.Permissions-list-item');
    var permires = [];
    $permiList.each(function (key, item) {
        var $item = $(item);
        var firs = $item.find('input.permissions-item-first');
        var secs = $item.find('input.permissions-item-second');
        var tempobj = {};
        tempobj.Name = $item.attr('data-permission');
        if (!firs.prop('checked') && !secs.prop('checked')) {
            tempobj.PermissionMask = 0;
        } else if (secs.prop('checked')) {
            tempobj.PermissionMask = 2;
            return true;
        } else if (!secs.prop('checked') && firs.prop('checked')) {
            tempobj.PermissionMask = 1;
        }
        permires.push(tempobj);
    });

    /*授权字段 end */

    var handlertype = $('#usertypeSeclecter', modal);//$('input[name="handleridtype"]:checked', modal);
    var handlers = $('#modifyResult').children('li');
    var handlertypeV = handlertype.val();
    var handlersV = [];
    if (handlertypeV == 2 && handlers.length == 0) {
        Xms.Web.Alert(false, '清先添加审核者');
        return false;
    }
    if (handlertypeV == 2 && handlers.length > 0) {
        handlers.each(function () {
            handlersV.push({ type: $(this).attr('data-entityname'), id: $(this).attr('data-value') });
        });
    }
    param._super.setTitle(titleV);
    param._super.setParams({
        cnName: titleV,
        ReturnTo: restepV,
        AuthAttributes: JSON.stringify(permires),
        AllowCancel: allowcancelV,
        AllowAssign: allowassignV,
        ReturnType: retypeV,
        HandlerIdType: handlertypeV,
        Handlers: JSON.stringify(handlersV),//handlersV.join(','),
        Description: descV,
        AttachmentRequired: !!attachRequire,
        AttachmentExts: attachexts,
        FormId: formnameV
    });
    modal.modal('hide');
}

function delConnection(obj, params) {
    var targetId = params.target || params.targetId;
    var sourceId = params.source || params.sourceId;
    /*删除数据  关联数据*/
    if ($('#' + sourceId).data().point) {
        var workflows = $('#' + sourceId).data().point.param.Conditions;
        if (workflows && workflows.length) {
            workflows = $.grep(workflows, function (item, key) {
                if ((item.PrevStepId == getTargetByName(sourceId).old || item.PrevStepId == sourceId) && (item.NextStepId == getTargetByName(targetId).old || item.NextStepId == targetId)) {
                    return false;
                } else {
                    return true;
                }
            });
            $('#' + sourceId).data().point.param.Conditions = workflows;
        }
    }
}

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

function editConnection(selected, param) {
    var modal = $('#connectionModal');
    modal.modal('show');
    modal.data().selecter = selected;
    var selectid = selected.id;
    var cTarget = $('#' + param.target);
    modal.data().param = param;

    /*加载字段*/

    var entityid = getEntityId();
    var filters = Xms.Fetch.FilterExpression();
    var pocondis = [];
    if (param.point.param.Conditions && param.point.param.Conditions.length > 0) {
        $.each(param.point.param.Conditions, function (key, n) {
            if ((n.PrevStepId == getTargetByName(param.source).old || n.PrevStepId == param.source) && (n.NextStepId == getTargetByName(param.target).old || n.NextStepId == param.target)) {//编辑时防止加载已存在的点找不到对应数据
                var logical = n.LogicalOperator;
                $('#connection-logical').find('option[value="' + logical + '"]').prop('selected', true);
                $.each(n.Conditions, function (ii, nn) {
                    var CompareAttributeName = null;
                    var filtertype = nn.CompareAttributeName ? 'attribute' : 'value';
                    var _values = nn.Values;
                    if (filtertype == "value") {
                        CompareAttributeName = null;
                        _values = changeToArray(_values);
                    } else if (filtertype == "attribute") {
                        _values = [];
                        CompareAttributeName = nn.CompareAttributeName;
                    }
                    pocondis.push({
                        //EntityName: n.EntityName,
                        AttributeName: nn.AttributeName,
                        Operator: nn.Operator,
                        CompareAttributeName: CompareAttributeName,
                        Values: _values
                    });
                });
            }
        });
    }
    filters.Conditions = pocondis;
    var postData = {
        entityid: entityid,
        filter: filters
    }
    //var filter = '[{"EntityName":"_mappointdckgdmig7","AttributeName":"Address","Operator":"0","Values":"t"}]'
    Xms.Web.Post('/filter/simplefiltersection', postData, false, function (res) {
        //var testdom = $('<div></div>');
        // testdom.html(res);
        $('#connection-conditions').html(res);
    }, false, false, false);

    /*加载字段  end*/
}

function getEntityId() {
    return $('#EntitySel').length > 0 ? $('#EntitySel').val() : $('#EntityId').val()
}

function saveConnection() {
    var modal = $('#connectionModal');
    var selected = modal.data().selecter;
    var param = modal.data().param;
    var sourceid = param.source;
    var cTarget = $('#' + param.targetId);
    var activeCon = $('.condition-item');
    if (activeCon.length == 0) { modal.modal('hide'); return false; }
    var LogicalOperator = $('#connection-logical');
    var params = param.point.param;
    //去掉之前该源节点上的流转条件数据，防止重复
    var workflowstep = params.Conditions;
    if (!params.Conditions) {
        workflowstep = params.Conditions = [];
    }
    var tempobj = {};
    tempobj.PrevStepId = getTargetByName(sourceid).old;
    tempobj.NextStepId = getTargetByName(param.target).old;
    tempobj.LogicalOperator = LogicalOperator.val();
    tempobj.Conditions = [];
    var flowShowLabel = [];
    var workflowTar = null;
    $.each(params.Conditions, function (key, item) {
        if (item.PrevStepId == getTargetByName(sourceid).old && item.NextStepId == getTargetByName(param.target).old) {
            workflowTar = item;
            return false;
        }
    });
    //前一环节审核者领导
    var flag = true;
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
        //if (!Values || Value=='') {
        //    flag = false;
        //    return false;
        //}
        var CompareAttributeName = null;
        var CompareAttributeNameZn = null;
        var filtertype = item.find('.filter-filed-type').val() || "value";;
        if (filtertype == "value") {
            CompareAttributeName = null;
        } else if (filtertype == "attribute") {
            Values = [];
            CompareAttributeName = item.find('.filter-compare-attrname').val();
            CompareAttributeNameZn = item.find('.filter-compare-attrname option:selected').text();
        }
        tempobj.Conditions.push({
            EntityName: itemEntityid,
            AttributeName: AttributeName,
            Operator: Operator,
            CompareAttributeName: CompareAttributeName,
            Values: Values ? changeToArray(Values) : ''
        });
        var showLabelValue = item.find('input[name="value"]').val();
        if (CompareAttributeNameZn) {
            showLabelValue = CompareAttributeNameZn;
        }
        flowShowLabel.push({
            AttributeName: item.find('.filter-filed-name option:selected').text(),
            Operator: item.find('.filter-filed-Operator option:selected').text(),
            Values: showLabelValue
        });
        console.log('tempobj', tempobj);
    });
    //if (flag == false) {
    //    Xms.Web.Alert(false,'请')
    //}
    if (workflowTar) {
        workflowTar.Conditions = tempobj.Conditions;
        workflowTar.LogicalOperator = LogicalOperator.val();
    } else {
        workflowstep.push(tempobj);
    }
    if (workflowTar.Conditions && workflowTar.Conditions.length > 0 && flowShowLabel.length > 0) {
        var labels = [];
        $.each(flowShowLabel, function (key, item) {
            labels.push(item.AttributeName + " " + item.Operator + ' ' + item.Values)
        });
        param.connection.setLabel(labels.join('<br/>'));
    }
    console.log(param);
    modal.modal('hide');
}

function connectPoint(sourceId, targetId, box) {
    var c = jsPlumb.connect({ source: sourceId, target: targetId });
    var con = new eleConnection('elecon____' + sourceId + '____' + targetId, { c: c, targetId: targetId, sourceId: sourceId })
    eleConnectionController.add(con);
    var $source = $('#' + sourceId), $target = $('#' + targetId);
    var flowShowLabel = [];
    if (!$source.data().point.param.Conditions) {
        $source.data().point.param.Conditions = [];
        $source.data().point.param.Conditions.push({ PrevStepId: getTargetByName(sourceId).old, NextStepId: getTargetByName(targetId).old, Conditions: [] });
    } else {
        var conflag = false;
        $.each($source.data().point.param.Conditions, function (iii, nnn) {
            if (nnn.PrevStepId == getTargetByName(sourceId).old && nnn.NextStepId == getTargetByName(targetId).old) {
                conflag = true;
                if (nnn.Conditions.length > 0) {
                    $.each(nnn.Conditions, function (iiii, nnnn) {
                        var CompareAttributeName = null;
                        var filtertype = nnnn.CompareAttributeName ? 'attribute' : 'value';
                        var _values = nnnn.Values;
                        var CompareAttributeNameZn = '';
                        if (filtertype == "value") {
                            CompareAttributeName = null;
                            _values = changeToArray(_values);
                        } else if (filtertype == "attribute") {
                            _values = [];
                            CompareAttributeName = nnnn.CompareAttributeName;
                            CompareAttributeNameZn = nnnn.CompareAttributeName;
                        }
                        var showLabelValue = changeToArray(nnnn.Values);
                        if (CompareAttributeNameZn) {
                            showLabelValue = CompareAttributeNameZn;
                        }
                        flowShowLabel.push({
                            AttributeName: nnnn.AttributeName,
                            Operator: nnnn.Operator,
                            CompareAttributeName: nnnn.CompareAttributeName,
                            Values: nnnn.Values,
                            entityid: nnnn.EntityName
                        });
                    })
                }
                return false;
            }
        });
        if (!conflag) {
            $source.data().point.param.Conditions.push({ PrevStepId: getTargetByName(sourceId).old, NextStepId: getTargetByName(targetId).old, Conditions: [] });
        }
    }

    // console.log(data);

    if ($source.data().point.param.Conditions && $source.data().point.param.Conditions.length > 0 && flowShowLabel.length > 0) {
        var labels = [];
        $.each(flowShowLabel, function (key, item) {
            var type = false;
            var attrname = item.AttributeName;
            // console.log('flowShowLabel', type);
            var curindex = attrname.indexOf('.');
            if (curindex > -1) {
                type = true;
                attrname = attrname.substr(0, curindex);
                // console.log('xxxxxxxx----attrname', item.Values);
            }

            var compareattrname = item.AttributeName;
            var filtertype = item.CompareAttributeName ? 'attribute' : 'value';
            var _values = item.Values[0]

            resetFlowCnName(getEntityId(), function (data) {
                var param = {
                    type: item.entityid + item.Values[0],
                    data: { entityid: item.entityid, value: item.Values[0] }
                }
                var attrobj = getAttributeobj(data, attrname);
                //console.log('attributeobj', attrobj);
                if ((attrobj.attributetypename == 'lookup' || attrobj.attributetypename == 'owner' || attrobj.attributetypename == 'customer') && filtertype == "value") {
                    Xms.Web.PageCache('workflow', '/api/data/Retrieve/ReferencedRecord/' + param.data.entityid + '/' + param.data.value, param, function (response) {
                        if (response.content && response.content['name']) {
                            labels.push(
                                getAttributeName(data, attrname) + " " +
                                getOperateName(item.Operator) + ' ' +
                                response.content['name']
                            );
                            c.setLabel(labels.join('<br/>'));
                        }
                    });
                } else {
                    if (filtertype == "value") {
                    } else if (filtertype == "attribute") {
                        _values = item.CompareAttributeName;
                        _values = getAttributeName(data, _values)
                    }
                    labels.push(
                        getAttributeName(data, attrname) + " " +
                        getOperateName(item.Operator) + ' ' +
                        _values
                    );
                    c.setLabel(labels.join('<br/>'));
                }
            }, type);
        });
    }

    c.bind("dblclick", function () {
        var targetPoint = elePointController.getPoint(targetId);
        box.trigger('connection.dblclick', { ele: box, _super: $('#' + targetId).data().point, point: $('#' + sourceId).data().point, source: sourceId, target: targetId, connection: c });
    }).bind('mousedown', function (obj, e) {
        console.log(e, obj)
        if (e.preventDefault) { e.preventDefault(); }
        if (e.which == 3) { //右键绑定
            //console.log(c)
            //$(c.canvas).offset();
            $(c.canvas).contextMenu('processMenu', {
                bindings: {
                    "menu_delConnection": function () {
                        Xms.Web.Confirm("提示", '是否要删除该条件?', function () {
                            box.trigger('connection.delete', { ele: box, _super: $('#' + targetId).data().point, point: $('#' + sourceId).data().point, source: sourceId, target: targetId, connection: c });
                            eleConnectionController.removeByName('elecon____' + sourceId + '____' + targetId);
                            eleConnectionController.removeByName('elecon____' + targetId + '____' + sourceId);
                            jsPlumb.detach(c);

                            console.log(eleConnectionController)
                        })

                        // jsPlumb.detach(c);
                    }
                }
            });
        }
    });
}
function getAttributeName(attrs, name) {
    var res = null;
    var arr = [];
    if (attrs.content.length > 0) {
        arr = attrs.content;
    } else {
        arr = attrs.content.items;
    }
    $.each(arr, function (key, item) {
        console.log(item['name'].toLowerCase(), name.toLowerCase())
        if (item['name'].toLowerCase() == name.toLowerCase()) {
            res = item.localizedname || item.referencingattributelocalizedname + '(' + item.referencedentitylocalizedname + ')';
            return false;
        }
    });
    return res;
}
function getAttributeobj(attrs, name) {
    var res = null;
    var arr = [];
    if (attrs.content.length > 0) {
        arr = attrs.content;
    } else {
        arr = attrs.content.items;
    }
    $.each(arr, function (key, item) {
        console.log(item['name'].toLowerCase(), name.toLowerCase())
        if (item['name'].toLowerCase() == name.toLowerCase()) {
            res = item;
            return false;
        }
    });
    return res;
}
function getOperateName(name) {
    var res = null;
    for (var i in Xms.Fetch.ConditionOperator) {
        if (Xms.Fetch.ConditionOperator.hasOwnProperty(i)) {
            if (Xms.Fetch.ConditionOperator[i] == name) {
                res = i;
            }
        }
    }
    return Xms.Fetch.ConditionOperatorLabel[res];
}

function styleToString(obj) {
    var res = [];
    for (var i in obj) {
        if (obj.hasOwnProperty(i)) {
            res.push(i + ':' + obj[i]);
        }
    }
    return res.join(';');
}
function stringTostyle(str) {
    var temp = str.split(';');
    var obj = {};
    $.each(temp, function (i, n) {
        var testr = n.split(':');
        obj[testr[0]] = testr[1] * 1;
    });
    return obj;
}
function saveWorkflow() {
    console.log(elePointController)
    if (elePointController.list.length > 0) {
        var result = [];
        var errors = [];
        var tempRes = $.extend(true, {}, elePointController);
        $.each(tempRes.list, function (key, item) {
            if (item.param.FormId == "") {
                errors.push({
                    item: item,
                    msg: item.cnName + ",请先设置表单"
                });
                return false;
            }
            if (item.param.formid) {
                delete item.param.formid;
            }
            if (item.param.Conditions && item.param.Conditions != '') {
                if (typeof item.param.Conditions === 'string') {
                    item.param.Conditions = JSON.parse(decodeURIComponent(item.param.Conditions));
                }
                //$.each(item.param.Conditions, function (i, n) {
                //    if (item.param.nodetype == TYPE_NARMAL && shutPoints.indexOf(n.NextStepId, 'oldName') > -1) {
                //        if (n.Conditions.length == 0) {
                //            errors.push({
                //                item: item,
                //                msg: item.cnName + ",分流节点必须设置条件，请先设置"
                //            });
                //            return false;
                //        }
                //    }
                //});
            } else {
                item.param.Conditions = "";
            }
            item.param.style = styleToString({//保存样式
                x: item.param.x,
                y: item.param.y
            });
            item.param.Name = item.cnName;
            item.param.nodetype = item.nodetype;
            item.param.WorkFlowStepId = item.WorkFlowStepId;
            result.push(item.param);
        });
        if (errors.length > 0) {
            var msg = [];
            $.each(errors, function (k, kitem) {
                msg.push(kitem.cnName);
            });
            Xms.Web.Alert(false, errors[0].msg);
            return false;
        }
        $.each(tempRes.list, function (key, item) {
            if (item.param.Conditions && item.param.Conditions != '') {
                $.each(item.param.Conditions, function (i, n) {
                    if (item.param.Conditions != "" && typeof item.param.Conditions !== "string") {
                        item.param.Conditions = encodeURIComponent(JSON.stringify(item.param.Conditions));
                    }
                });
            }
        });
        console.log('提交的数据：', result);
        console.log('提交的数据str：', JSON.stringify(result));
        $('#workflowData').val(encodeURIComponent(JSON.stringify(result)));
        return true;
    }
}

function resSetWorkFlow() {
    $.each(elePointController.list, function (iii, nnn) {
        //删除数据和对应的流转条件
        var sourceid = nnn.name;

        //找出链接到自己的流转条件
        var gconns = [];
        $.each(eleConnectionController.list, function (key, item) {
            if (item.name.indexOf(sourceid) > -1) {
                gconns.push(item);
            }
        });
        $.each(gconns, function (i, n) {
            var _tarid = n.param.targetId;
            var _point_i = elePointController.indexOf(_tarid);
            if (_point_i > -1) {
                var _opoint = elePointController.list[_point_i];
                var workflows = $('#' + _tarid).data().point.param.Conditions;
                if (workflows && workflows.length) {
                    workflows = $.grep(workflows, function (item, key) {
                        if ((item.PrevStepId == getTargetByName(sourceid).old || item.PrevStepId == sourceid) && (item.NextStepId == getTargetByName(_tarid).old || item.NextStepId == _tarid)) {
                            return false;
                        } else {
                            return true;
                        }
                    });
                }
            }
            jsPlumb.detach(n.param.c);
            eleConnectionController.removeByName(n.name);
        });
    });
    for (var i = 0, len = elePointController.list.length; i < len; i++) {
        var elei = elePointController.list[0];
        elei.box.remove();
        elePointController.remove(elei);
    }

    startPointAdded = false;
    endPointAdded = false;
    pointStepOrder = 0;
    namesp = [];
    if (typeof PAGE_TYPE !== 'undefined' && PAGE_TYPE == 'new') {
        model = '';
        loadInitFlow();
    } else {
        model = decodeURIComponent($('#workflowData').val());
        loadInitFlow(true);
    }
}