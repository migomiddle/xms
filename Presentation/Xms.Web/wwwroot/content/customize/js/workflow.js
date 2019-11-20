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
        if (this.nodetype == 2 || this.nodetype == 3) {
            //节点顶部工具栏
            this.toolBar = $('<div class="xmsPointer-toolBar"></div>');
            this.tool_name = $('<span>' + this.cnName + '</span>');
            this.tool_setting = $('<span><em class="glyphicon glyphicon-cog"></em></span>');
            this.tool_delete = $('<span><em class="glyphicon glyphicon-trash"></em></span>');
            this.toolBar.append(this.tool_setting, this.tool_delete);

            //节点右侧按钮
            this.rightShowBtn = $('<div class="xmsPointer-rightShowBtn" style="display:none;"><span class="glyphicon glyphicon-chevron-right"></span></div>');
            this.rightMenu = $('<div class="xmsPointer-rightMenu" style="display:none;"></div>');
            this.rightMenu_addPoint = $('<span><em class="glyphicon glyphicon-retweet"></em> 添加普通节点</span>');
            this.rightMenu_addPoints = $('<span><em class="glyphicon glyphicon-random"></em> 添加分流节点</span>');
            this.rightMenu.append(this.rightMenu_addPoint, this.rightMenu_addPoints);
        }
        this.plumb = null;
        this.box = null;
        this.plumbCtrls = [];
        this._super = elePointController;
        elePointController.add(this);
    }
    elePoint.prototype = $.extend({}, mapPoint.prototype);
    elePoint.prototype.init = function () {
        this.box = $('<div class="xmsPointer-box"></div>');
        this.box.data().point = this;
        this.plumb = $('<div class="xmsPointer-plumb" data-toggle="tooltip" data-placement="left" title="拖动图标连线"><span></span></div>');
        if (this.nodetype == 2 || this.nodetype == 3) {
            this.box.append(this.toolBar);
            this.box.append(this.rightShowBtn);
            this.box.append(this.rightMenu);
        }
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
            c.bind("dblclick", function () {
                var sourcePoint = elePointController.getPoint(sourceId);
                var targetPoint = elePointController.getPoint(targetId);
                con.datas = targetPoint;
                p.trigger('connection.dblclick', { ele: p, _super: $('#' + sourceId).data().point, point: sourcePoint, source: sourceId, target: targetId, connection: c });
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
                                    p.trigger('connection.delete', { ele: p, _super: $('#' + sourceId).data().point, point: $('#' + sourceId).data().point, source: sourceId, target: targetId, connection: c });
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
            console.log($('#' + sourceId).data().point);
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
        //if (this.nodetype == 2 || this.nodetype==3) {
        //    this.toolBar = $('<div class="xmsPointer-toolBar"></div>');
        //    this.tool_name = $('<span>' + this.cnName + '</span>');
        //    this.tool_setting = $('<span><em class="glyphicon glyphicon-cog"></em></span>');
        //    this.tool_delete = $('<span><em class="glyphicon glyphicon-trash"></em></span>');
        //    this.toolBar.append(this.tool_name, this.tool_setting, this.tool_delete);
        //}

        if (this.nodetype == 2 || this.nodetype == 3) {
            this.tool_setting.off('click').on('click', function () {
                that.trigger('mappoint.dblclick', { ele: that, _super: self });
            });
            this.tool_delete.off('click').on('click', function () {
                that.trigger('mappoint.delete', { ele: that, _super: self });
            });
            that.hover(function () {
                self.rightShowBtn.show();
            }, function () {
                self.rightShowBtn.hide();
                self.rightMenu.hide();
            });
            self.rightShowBtn.on('click', function () {
                self.rightMenu.show();
            });
            self.rightMenu_addPoint.off('click').on('click', function () {
                addPointFun(null, self);
            });
            self.rightMenu_addPoints.off('click').on('click', function () {
                //addPointFun({ nodetype: '3' }, self)
                openAddPointsModal(self);
            });
        }
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