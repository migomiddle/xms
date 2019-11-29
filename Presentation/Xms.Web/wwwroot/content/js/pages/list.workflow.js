//@ sourceURL=page/list.workflow.js
(function ($, root, un) {
    var private_method = {
        removeChilds: function (arr) {
            $.each(arr, function (key, item) {
                var name = item.name;
                var index = item.indexOf(item);
                if (index > -1) {
                    item.list.splice(index, 1);
                    return true;
                }
            });
        }
    }
    var settings = {
        connectorPaintStyle: {
            lineWidth: 3,
            strokeStyle: "#49afcd",
            joinstyle: "round"
        },
        //鼠标经过样式
        connectorHoverStyle: {
            lineWidth: 3,
            strokeStyle: "#da4f49"
        }
    }

    var prefixName = 'node_';
    var _name = 'xmsPointer';
    function mapPoint(name, param) {
        this.name = name;
        //this.title = title;
        this.list = [];
        this.connections = [];
        this.param = $.extend({}, param);
        //this.param.name = param && param.name;
    }
    mapPoint.prototype.add = function (point) {
        var index = this.indexOf(point.name);
        if (index == -1) {
            this.list.push(point);
        }
    }
    mapPoint.prototype.remove = function (point) {
        var index = this.indexOf(point.name);
        if (index > -1) {
            private_method.removeChilds(this.list);
            this.list.splice(index, 1);
        }
    }
    mapPoint.prototype.removeByName = function (name) {
        var index = this.indexOf(name);
        if (index > -1) {
            private_method.removeChilds(this.list);
            this.list.splice(index, 1);
        }
    }
    mapPoint.prototype.indexOf = function (name, type) {
        var index = -1;
        if (!type) {
            for (var i = 0, length = this.list.length; i < length; i++) {
                var item = this.list[i];
                if (item.name == name) {
                    index = i;
                    break;
                }
            }
        } else {
            for (var i = 0, length = this.list.length; i < length; i++) {
                var item = this.list[i];
                if (item[type] == name) {
                    index = i;
                    break;
                }
            }
        }
        return index;
    }
    mapPoint.prototype.getPoint = function (name) {
        var index = this.indexOf(name);;
        if (index > -1) {
            return this.list[index];
        }
    }
    mapPoint.prototype.setParam = function (key, value) {
        this.param[key] = value;
    }
    mapPoint.prototype.setParams = function (params) {
        this.param = $.extend({}, this.param, params);
    }
    mapPoint.prototype.getParam = function (key) {
        return this.param[key];
    }

    var eleConnectionController = new mapPoint('eleConnectionController');
    function eleConnection(name, param) {
        mapPoint.call(this, name, param);
    }
    eleConnection.prototype = $.extend({}, mapPoint.prototype);

    var elePointController = new mapPoint('elePointController');

    var elesetting = {
        x: 0,
        y: 0,
        nodetype: 'normal',
        cnName: '节点'
    }
    //     *jsPlumb.setId(el, newId)设置jsPlumb中的元素id
    //*jsPlumb.setIdChanged(oldId, newId)改变已有jsPlumb中的元素id
    function elePoint(name, opts) {
        this.opts = $.extend({}, elesetting, opts);
        this.x = this.opts.x || 0;
        this.y = this.opts.y || 0;

        //this.point = new mapPoint(+setTimeout(0));
        this.param = {};
        this.param.name = '';
        this.cnName = this.opts.cnName;

        mapPoint.call(this, this.name, this.opts);//继承构造方法
        this.WorkFlowStepId = this.param.WorkFlowStepId ? this.param.WorkFlowStepId : Xms.Utility.Guid.NewGuid().ToString('D');
        this.param.WorkFlowStepId = this.WorkFlowStepId;
        this.nodetype = this.opts.nodetype === 0 ? this.opts.nodetype : this.opts.nodetype || '2';//start , end;
        this.name = this.param.name || this.WorkFlowStepId;
        // this.Name = this.param.Name || '';
        if (this.param.name == "") {
            this.param.name = this.name;
        }
        //this.param.StepOrder = this.name;
        this.oldName = this.name;
        this.Name = this.cnName;
        this.param.NodeName = this.param.NodeName || ''
        this.title = $('<div class="xmsPointer-title">' + (this.cnName) + '</div>');
        //if (this.nodetype == 2 || this.nodetype == 3) {
        //    //节点顶部工具栏
        //    this.toolBar = $('<div class="xmsPointer-toolBar"></div>');
        //    this.tool_name = $('<span>' + this.cnName + '</span>');
        //    this.tool_setting = $('<span><em class="glyphicon glyphicon-cog"></em></span>');
        //    this.tool_delete = $('<span><em class="glyphicon glyphicon-trash"></em></span>');
        //    this.toolBar.append(this.tool_setting, this.tool_delete);

        //    //节点右侧按钮
        //    this.rightShowBtn = $('<div class="xmsPointer-rightShowBtn" style="display:none;"><span class="glyphicon glyphicon-chevron-right"></span></div>');
        //    this.rightMenu = $('<div class="xmsPointer-rightMenu" style="display:none;"></div>');
        //    this.rightMenu_addPoint = $('<span><em class="glyphicon glyphicon-retweet"></em> 添加普通节点</span>');
        //    this.rightMenu_addPoints = $('<span><em class="glyphicon glyphicon-random"></em> 添加分流节点</span>');
        //    this.rightMenu.append(this.rightMenu_addPoint, this.rightMenu_addPoints);
        //}
        this.plumb = null;
        this.box = null;
        this.plumbCtrls = [];
        this._super = elePointController;
        elePointController.add(this);
    }
    elePoint.prototype = $.extend({}, mapPoint.prototype);
    elePoint.prototype.init = function () {
        this.box = $('<div class="xmsPointer-box" data-toggle="tooltip"></div>');
        this.box.data().point = this;
        this.plumb = $('<div class="xmsPointer-plumb" data-toggle="tooltip" data-placement="left" title="拖动图标连线"><span></span></div>');
        //if (this.nodetype == 2 || this.nodetype == 3) {
        //    this.box.append(this.toolBar);
        //    this.box.append(this.rightShowBtn);
        //    this.box.append(this.rightMenu);
        //}
        this.box.append(this.plumb).append(this.title);

        this.setPos();
        return this;
    }
    elePoint.prototype.setPos = function () {
        this.box.css({ 'left': this.x, 'top': this.y });
    }
    elePoint.prototype.loadPlumb = function () {
        var self = this;
        var p = this.box;
        //jsPlumb.draggable(jsPlumb.getSelector(".xmsPointer-box"));
        jsPlumb.draggable(p, {
            stop: function (event, ui) {
                //console.log(ui,event)
                var target = ui.helper;
                var par = self.box.parent();
                self.param.x = ui.position.left;
                self.param.y = ui.position.top;
            }
            , star: function (event, ui) {
            }
            , drag: function (event, ui) {
                // console.log(ui,event)
                var target = ui.helper;
                var par = self.box.parent();
                var bound = {
                    minX: 0,
                    minY: 0,
                    maxX: par.width() - target.outerWidth(),
                    maxY: par.height() - target.outerHeight()
                }

                if (ui.position.left <= bound.minX) {
                    ui.position.left = bound.minX
                }
                if (ui.position.left >= bound.maxX) {
                    ui.position.left = bound.maxX
                }

                if (ui.position.top <= bound.minY) {
                    ui.position.top = bound.minY
                }
                if (ui.position.top >= bound.maxY) {
                    ui.position.top = bound.maxY
                }
            }
        });
        if (this.nodetype == '1') {
            this.box.addClass('mappoint-end point-red');
            this.plumb.children('span').addClass('glyphicon glyphicon-stop');
            jsPlumb.makeTarget(jsPlumb.getSelector('.xmsPointer-box'), {
                dropOptions: { hoverClass: "hover", activeClass: "active" },
                uuid: this.name,
                anchor: "Continuous",
                maxConnections: -1,
                endpoint: ["Dot", { radius: 1, uuid: 'test' }],
                paintStyle: { fillStyle: "#ec912a", radius: 1 },
                hoverPaintStyle: this.connectorHoverStyle,
                beforeDrop: beforeDrop
            });
        } else if (this.nodetype == '3') {
            this.box.addClass('point-blue');
            this.plumb.children('span').addClass('glyphicon glyphicon-random');
            jsPlumb.makeSource(this.plumb, {
                parent: p,
                anchor: "Continuous",
                isTarget: true,
                uuid: this.name,
                endpoint: ["Dot", { radius: 1, uuid: 'test' }],
                connector: ["Flowchart", { stub: [5, 5] }],
                connectorStyle: settings.connectorPaintStyle,
                hoverPaintStyle: settings.connectorHoverStyle,
                dragOptions: {},
                maxConnections: -1
            });
            jsPlumb.makeTarget(jsPlumb.getSelector('.xmsPointer-box'), {
                dropOptions: { hoverClass: "hover", activeClass: "active" },
                anchor: "Continuous",
                uuid: this.name,
                maxConnections: -1,
                endpoint: ["Dot", { radius: 1, uuid: 'test' }],
                paintStyle: { fillStyle: "#ec912a", radius: 1 },
                hoverPaintStyle: this.connectorHoverStyle,
                beforeDrop: beforeDrop
            });
        } else if (this.nodetype == "2") {
            this.box.addClass(' point-blue');
            this.plumb.children('span').addClass('glyphicon glyphicon-retweet');
            jsPlumb.makeSource(this.plumb, {
                parent: p,
                anchor: "Continuous",
                isTarget: true,
                uuid: this.name,
                endpoint: ["Dot", { radius: 1, uuid: 'test' }],
                connector: ["Flowchart", { stub: [5, 5] }],
                connectorStyle: settings.connectorPaintStyle,
                hoverPaintStyle: settings.connectorHoverStyle,
                dragOptions: {},
                maxConnections: -1
            });
            jsPlumb.makeTarget(jsPlumb.getSelector('.xmsPointer-box'), {
                dropOptions: { hoverClass: "hover", activeClass: "active" },
                anchor: "Continuous",
                uuid: this.name,
                maxConnections: -1,
                endpoint: ["Dot", { radius: 1, uuid: 'test' }],
                paintStyle: { fillStyle: "#ec912a", radius: 1 },
                hoverPaintStyle: this.connectorHoverStyle,
                beforeDrop: beforeDrop
            });
        } else if (this.nodetype == '0') {
            this.box.addClass('mappoint-start point-green');
            this.plumb.children('span').addClass('glyphicon glyphicon-play');
            jsPlumb.makeSource(this.plumb, {
                parent: p,
                anchor: "Continuous",
                isTarget: true,
                uuid: this.name,
                endpoint: ["Dot", { radius: 1, uuid: 'test' }],
                connector: ["Flowchart", { stub: [5, 5] }],
                connectorStyle: settings.connectorPaintStyle,
                hoverPaintStyle: settings.connectorHoverStyle,
                dragOptions: {},
                maxConnections: -1
            });
        }
        this.name = this.box.attr('id');
        //jsPlumb.setId(this.box, this.name)
        // console.log('show name:', this.name);
        function beforeDrop(params) {
            //判断如果为开始节点，则不链接
            var sourceId = params.sourceId;
            var targetId = params.targetId;
            if (sourceId == targetId) return false;
            // console.log(params)
            if ($('#' + targetId).hasClass('mappoint-start')) {
                return false;
            }
            if (eleConnectionController.indexOf('elecon____' + sourceId + '____' + targetId) > -1 || eleConnectionController.indexOf('elecon____' + targetId + '____' + sourceId) > -1) {//防止重复链接
                return false;
            }
            var c = jsPlumb.connect({ source: sourceId, target: targetId });
            var con = new eleConnection('elecon____' + sourceId + '____' + targetId, { c: c, targetId: targetId, sourceId: sourceId });
            eleConnectionController.add(con);
            if (!$('#' + sourceId).data().point.param.Conditions) {
                $('#' + sourceId).data().point.param.Conditions = [];
                $('#' + sourceId).data().point.param.Conditions.push({ PrevStepId: getTargetByName(sourceId).old, NextStepId: getTargetByName(targetId).old, Conditions: [] });
            } else {
                var conflag = false;
                $.each($('#' + sourceId).data().point.param.Conditions, function (iii, nnn) {
                    if (nnn.PrevStepId == getTargetByName(sourceId).old && nnn.NextStepId == getTargetByName(targetId).old) {
                        conflag = true;
                        return false;
                    }
                });
                if (!conflag) {
                    $('#' + sourceId).data().point.param.Conditions.push({ PrevStepId: getTargetByName(sourceId).old, NextStepId: getTargetByName(targetId).old, Conditions: [] });
                }
            }

            self.add($('#' + sourceId).data().point);
            return false;
        }
        return this;
    }
    elePoint.prototype.render = function (_context) {
        $(_context).append(this.box);
        this.bindEvent();
        this.loadPlumb();
        return this;
    }

    elePoint.prototype.bindEvent = function (_context) {
        var self = this;
        var that = this.box;
        that.off('dblclick').on('dblclick', function () {
            that.trigger('mappoint.dblclick', { ele: that, _super: self });
        });
    }
    elePoint.prototype.setTitle = function (title) {
        this.title.text(title);
        this.param.cnName = title;
        this.cnName = title;
        this.Name = this.cnName;
        if (this.nodetype == 2 || this.nodetype == 3) {
            this.tool_name.text(title)
        }
    }

    jsPlumb.importDefaults({
        DragOptions: { cursor: 'pointer' },
        EndpointStyle: { fillStyle: '#225588' },
        Endpoint: ["Dot", { radius: 1 }],
        ConnectionOverlays: [
            ["Arrow", { location: 1 }],
            ["Label", {
                location: 0.1,
                id: "label",
                cssClass: "aLabel"
            }]
        ],
        Anchor: 'Continuous',
        ConnectorZIndex: 5,
        HoverPaintStyle: settings.connectorHoverStyle
    });
    if ($.support.msie && $.support.version < '9.0') { //ie9以下，用VML画图
        jsPlumb.setRenderMode(jsPlumb.VML);
    } else { //其他浏览器用SVG
        jsPlumb.setRenderMode(jsPlumb.SVG);
    }

    elePoint.setMaxWrap = function (w, h) {
        settings.maxWidth = w;
        settings.maxHeight = h;
    }
    root.mapPoint = mapPoint;
    root.elePoint = elePoint;
    root.eleConnection = eleConnection;
    root.elePointController = elePointController;
    root.eleConnectionController = eleConnectionController;
})(jQuery, window);

var startPointAdded = false;
var endPointAdded = false;
var pointStepOrder = 0;
var shutPoints = new mapPoint('shutPoints');//保存分流节点

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
    return ele;
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
        });
    }
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

function loadEntities(callback) {
    Xms.Web.GetJson('/api/schema/entity/' + Xms.Page.PageContext.EntityId, null, function (data) {
        if (!data) return;
        $('#EntityId').append('<option data-relationship="' + data.name + '"  value="' + data.entityid + '">' + data.localizedname + '</option>');
        $('#EntityId').trigger('change');
        EntityId = $('#EntityId').val();
        callback && callback();
    });
}
var PAGE_TYPE = 'edit';
var TYPE_SHUNT = "3";
var TYPE_NARMAL = "2";
var TYPE_START = "0";
var TYPE_END = "1";
var EntityId = '';

var model = $('#recordWorkFlowInfos').html(); //测试数据
var namesp = [];
function getTargetByoldName(name) {
    var res = null;
    $.each(namesp, function (key, item) {
        if (item.old == name) {
            res = item;
            return false;
        }
    });
    return res;
}
function getTargetByName(name) {
    var res = null;
    $.each(namesp, function (key, item) {
        if (item.newn == name) {
            res = item;
            return false;
        }
    });
    return res;
}
function resetFlowCnName(entityid, callback, type) {
    if (!type) {
        var params = {
            type: "workflow_" + entityid,
            data: null
        }
        Xms.Web.PageCache('workflow', '/api/schema/attribute?getall=true&entityid=' + entityid + '', params, function (data) {
            callback && callback(data);
        });
    } else {
        var enParam = {
            type: 'referencingentityid' + entityid,
            data: { referencingentityid: entityid }
        }
        Xms.Web.PageCache('workflow', '/api/schema/relationship/GetReferenced/', enParam, function (result) {
            callback && callback(result);
        });
    }
}

function editFilterData(data, isNotFilter) {
    if (isNotFilter) return data;
    $.each(data, function (key, item) {
        item.AllowAssign = item.allowassign;
        item.AllowCancel = item.allowcancel;
        delete item.allowassign;
        delete item.allowcancel;
        item.AuthAttributes = item.authattributes;
        item.Conditions = item.conditions;
        item.Description = item.description;
        item.FormId = item.formid;
        delete item.formid;
        item.ReturnTo = item.returnto;
        item.HandlerIdType = item.handleridtype;
        item.Handlers = item.handlers;
        delete item.handlers;
        delete item.handleridtype;
        item.Name = item.name;
        item.cnName = item.name;
        item.NodeName = item.nodename;
        item.ReturnType = item.returntype;
        item.StepOrder = item.steporder;
        item.WorkFlowStepId = item.workflowstepid;
        item.AttachmentRequired = item.attachmentrequired;
        item.AttachmentExts = item.attachmentexts;
        delete item.attachmentrequired;
        delete item.attachmentexts;
        delete item.name;
    });
    return data;
}
function loadInitFlow(type) {
    if (model == "") {
        var startP = addPoint({ nodetype: TYPE_START, cnName: "开始" }, true);
        var endP = addPoint({ nodetype: TYPE_END, cnName: "结束", x: 700, y: 200 }, true);
        var curP = addPoint({ x: 300, y: 100 });
        connectPoint(startP.box.attr("id"), curP.box.attr("id"), curP.box);
        curP.add($('#' + curP.box.attr("id")).data().point);
        connectPoint(curP.box.attr("id"), endP.box.attr("id"), endP.box);
        endP.add($('#' + endP.box.attr("id")).data().point);
        return false;
    }

    var datas = JSON.parse(model);
    datas = editFilterData(datas, type);
    if (datas.length > 0) {
        var connections = [];
        var points = [];
        //必须先把点画上，再画线
        $.each(datas, function (key, item) {
            if (typeof item == "undefined") { return true }
            item.style = stringTostyle(item.style);
            item.x = item.style ? item.style.x : item.x ? item.x : 100;
            item.y = item.style ? item.style.y : item.y ? item.y : 100;
            var source = addPoint(item);

            console.log('point', source);
            //namesp.push({ old: source.oldName, newn: source.name, point: source });
            if (item.Conditions && item.Conditions != "") {
                console.log(decodeURIComponent(item.Conditions))
                item.Conditions = JSON.parse(decodeURIComponent(item.Conditions));
                source.param.Conditions = item.Conditions;
                $.each(item.Conditions, function (i, n) {
                    connections.push({
                        point: source,
                        sourceid: n.PrevStepId,
                        targetid: n.NextStepId
                    });
                });
            }
        });

        console.log('需要链接的线', connections);
        $.each(connections, function (cckey, item) {
            var tempsour = getTargetByoldName(item.sourceid);
            var tempTar = getTargetByoldName(item.targetid);
            if (!tempsour || !tempTar) { return true; }
            var sourceId = tempsour.newn;
            var targetId = tempTar.newn;
            // console.log(sourceId, targetId);
            var isConnected = eleConnectionController.indexOf('elecon____' + sourceId + '____' + targetId);
            if (isConnected > -1) return true;
            if ($('#' + sourceId).length == 0 || $('#' + targetId).length == 0) return true;
            connectPoint(sourceId, targetId, item.point.box)
            ////console.log($('#' + item2id).data().point);
            item.point.add($('#' + sourceId).data().point);
        });
    }
}

function loadWorkFlowInstances(callback) {
    Xms.Web.GetJson('/api/workflow/instances?entityid=' + Xms.Page.PageContext.EntityId + '&recordid=' + Xms.Page.PageContext.RecordId, {}, function (res) {
        callback && callback(res);
    });
}