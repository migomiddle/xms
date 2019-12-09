//@ sourceURL=page/flowline.js
(function ($, root, un) {
    
    var prefixName = 'node_';
    var _name = 'xmsPointer';
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
    var elesetting = {
        x: 0,
        y: 0,
        nodetype: 'normal',
        cnName: '节点'
    }
    function getFlowData(opts, callback) {
        var url = opts.url;
        var data = opts.data;
        console.log(url, data);
        Xms.Web.GetJson(url, data, function (response) {
            console.log('response', response);
            callback && callback(response, parent);
        }, null, null, 'post');

    }
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

   
    //     *jsPlumb.setId(el, newId)设置jsPlumb中的元素id
    //*jsPlumb.setIdChanged(oldId, newId)改变已有jsPlumb中的元素id
    function elePoint(name, opts) {
        this.opts = $.extend({}, elesetting, opts);
        this.x = this.opts.x || 0;
        this.y = this.opts.y || 0;
        this.id = this.opts.id;
        //this.point = new mapPoint(+setTimeout(0));
        this.param = {};
        this.param.name = '';
        this.cnName = this.opts.text;
        this.nodetype = 3;
        mapPoint.call(this, this.name, this.opts);//继承构造方法
        this.title = $('<div class="xmsPointer-title">' + (this.cnName) + '</div>');
        this.plumb = null;
        this.box = null;
        this.plumbCtrls = [];
    }
    elePoint.prototype = $.extend({}, mapPoint.prototype);
    elePoint.prototype.init = function () {
        this.wrap = $('<div class="xmsflowline-wrap" data-id="' + this.id + '"></div>');
        this.box = $('<div class="xmsPointer-box xmsPointer-itembox" data-id="' + this.id + '" id="' + this.id + '" data-parentid="'+(this.opts.parentid || '')+'" data-toggle="tooltip"></div>');
        this.box.data().point = this;
        this.plumb = $('<div class="xmsPointer-plumb" data-toggle="tooltip" data-placement="left" title="拖动图标连线"><span></span></div>');
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
        jsPlumb.draggable(jsPlumb.getSelector(".xmsPointer-box"));
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
               // beforeDrop: beforeDrop
            });
        } else if (this.nodetype == '3') {
            this.box.addClass('point-blue');
            this.plumb.children('span').addClass('glyphicon glyphicon-th-large');
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
               // beforeDrop: beforeDrop
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
               // beforeDrop: beforeDrop
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
       
        return this;
    }
    elePoint.prototype.render = function (_context) {
        this.wrap.append(this.box);
        $(_context).append(this.wrap);
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
    function getChildsData(data, id) {
        var maxData = 20;
        var count = 0;
        var res = [];
        //console.log(id);
        $.each(data, function (key, item) {
            if (res.length >= maxData) {
                return false;
            }
            if (item.parentid == id) {
                res.push(item);
                //getInerChilds(item);
            }
        });
        console.log(res);
        console.table(data);
        while (res.length < maxData && count < 10) {
            count++;

            $.each(res, function (key, item) {
                if (res.length >= maxData) {
                    return false;
                }
                $.each(data, function (i, n) {
                    if (res.length >= maxData) {
                        return false;
                    }
                    if (n.parentid === item.id && !checkInRes(n, res)) {
                        res.push(n);
                    }
                });
            });
        }
        return res;
    }

    function checkInRes(item, res) {
        var flag = false;
        $.each(res, function (i, n) {
            if (item.id == n.id) {
                flag = true;
                return false;
            }
        });
        return flag;
    }

    function handlerFlowData(response, entityname, parent) {
        var datas = [], resDatas = [];
        if (response.content && response.content.length > 0) {
            datas = response.content;
        } else if (response.content.items && response.content.items.length > 0) {
            datas = response.content.items;
        }
        entityname = entityname.toLowerCase();
        parent = parent.toLowerCase();
        $.each(datas, function (key, item) {

            var itemdata = {
                "id": item[entityname + 'id'],
                "text": item['name'],
                "parentid": item[parent],
                "value": item[parent]
            }
            resDatas.push(itemdata);
        });
        return resDatas;
    }
    function _renderFlow(opts, loadFlowAfter, preRender, rendered, setPostOpts) {
        var url = '/api/data/Retrieve/Multiple';
        preRender && preRender();
        if (!opts.entityname || !opts.parent || !opts.attrname) {
            throw Error("entityname,parent,attrname 不能为空");
        }
        console.log(opts);
        var entityname = opts.entityname.toLowerCase();
        var parent = opts.parent.toLowerCase();
        var attrname = opts.attrname.toLowerCase();
        var sortby = opts.sortby || 'name';
        var sorttype = opts.ordertype || "Ascending";
        var treesort = opts.treesort;
        var isDefaultClick = typeof opts.isDefaultClick == 'undefined' ? true : opts.isDefaultClick;
        var fil = opts.filter || [];
        var filter = { "Conditions": fil };
        var order = [{ "AttributeName": treesort, "OrderType": sorttype }];
        if (treesort) {
            var queryObj = { EntityName: entityname, Orders: order, Criteria: filter, ColumnSet: { allcolumns: true } };
        } else {
            var queryObj = { EntityName: entityname, Criteria: filter, ColumnSet: { allcolumns: true } };
        }
        var postdata = { "query": queryObj, "isAll": true };
        setPostOpts && setPostOpts(postdata);
        console.log('postdata', postdata);
        postdata = JSON.stringify(postdata);
        var resD = {};
        resD.url = url;
        // resD.sortby = sortby;
        resD.data = postdata;
        getFlowData(resD, function (response) {
            //左边树导航
            var datas = handlerFlowData(response, entityname, parent);
            var source =
            {
                datatype: "json",
                datafields: [
                    { name: 'id' },
                    { name: 'parentid' },
                    { name: 'text' },
                    { name: 'value' }
                ],
                id: 'id',
                localdata: datas
            };
            var dataAdapter = new dataApax(source);
            dataAdapter.dataBind();
            console.log(dataAdapter.getRecords())
            var records = dataAdapter.getRecordsHierarchy('id', 'parentid', 'items', [{ name: 'text', map: 'label' }]);
            console.log('tree.records', records);
            console.log('handlerFlowData', datas);
            filterFlowDatas(records);
            renderFlowLinePoint($('body'), records, records);
            
          //  setFlowLinePost(records, records);
            loadFlowAfter && loadFlowAfter();
        });
    }
    function filterFlowDatas(datas) {
        var gloLen = datas.length;
        $.each(datas, function (i, n) {
            var nlen = n.items.length;
            if (nlen > 0) {
                $.each(n.items, function (ii, nn) {
                    nn.__parent = n;
                    nn.__index = ii;
                });
                filterFlowDatas(n.items);
            }
           
        });
    }
    function getMaxLayer(datas) {
        var layer = 0;
        $.each(datas, function (i, n) {
            layer = n.__layer;
            if (n.items.length > 0) {
                var temp = getMaxLayer(n.items);
                if (temp.__layer > layer) {
                    layer = temp.__layer;
                }
            }
        });
        return layer;
    }
    var posSetting = {
        maxW: $(window).width(),
        rowMax: 3,
        itemW: 160,
        itemMargin: 20,
        itemH:100//上级与下级的距离
    }
    var renderLayout = 0;
    var currentW = 0;
    var currentH = 0;
    var oldW = 0;
    function renderPoint($context,data) {
        var point = new elePoint(n.text, n);
        point.init();
        point.render($context);

        return point;
    }
    function renderFlowLinePoint($context, datas) {
        var gloLen = datas.length;
        function renderToTarget($target, _datas,callback) {
            $.each(_datas, function (i, n) {
                if ($context.find('.xmsflowline-row[data-id="' + n.id + '"]').length == 0) {
                    var $row = $('<div class="xmsflowline-row clearfix" data-id="' + n.id + '" />');
                    $target.append($row)
                } else {
                    var $row = $context.find('.xmsflowline-row[data-id="' + n.id + '"]');
                }
                var nlen = n.items.length;
                var point = new elePoint(n.text, n);
                point.init();
                point.render($row);
                if ($context.find('.xmsflowline-row[data-id="' + n.parentid + '"]').length > 0) {
                    $row.append(point.wrap);
                }
                point.datas = n;
                n.__point = point;
                if (nlen > 0) {
                    renderToTarget($row, n.items);
                }
            });
            callback && callback();
        }
        var $dialog = $('<div class="flowline-dialog active" id="flowline_dialog" />');
        var $dialog_wrap = $('<div class="flowline-dialogwrap" />');
        var $close = $('<div class="flowline-dialogclose"><span class="glyphicon glyphicon-remove"></span></div>');
        var $outputbtn = $('<div class="flowline-dialogtools"><button type="button" class="btn btn-sm btn-warning">导出图片</button></div>')
        $dialog.append($dialog_wrap).append($close).append($outputbtn);
        $context.append($dialog);
        $.each(datas, function (i, n) {
            var $wrap = $('<div clsss="flowline-wrapbox" style="display:inline-block; float:left;"></div>');
            $dialog_wrap.append($wrap);
            //setTimeout(function () { 
                renderToTarget($wrap, [n], function () {
                    
                });
               
            //})
        });
        setTimeout(function () {
            var wrapW = 0;
            $('.flowline-dialogwrap').children().each(function () {
                wrapW = wrapW + $(this).width() + $(this).scrollLeft()||1;
                $(this).attr('data-dwidth', $(this).width());
            });
            $dialog_wrap.css('width', wrapW);
            wrapW = 0;
            $('.flowline-dialogwrap').children().each(function () {
                wrapW = wrapW + $(this).width() + $(this).scrollLeft() || 1;
                $(this).attr('data-dwidth', $(this).width());
            });
            $dialog_wrap.css('width', wrapW);
            connectFlowLine($context);
        }, 10)
        $close.off().on('click', function () {
            $dialog.remove();
            $close.off();
        })
        $outputbtn.off().on('click', function () {
            Xms_FlowLine.outPutImg();
            $outputbtn.off();
        })
        
    }
    function outPutImg() {
        var $flowline_dialog = $('#flowline_dialog');
        if (typeof html2canvas !== 'undefined') {
            //以下是对svg的处理
            var nodesToRecover = [];
            var nodesToRemove = [];
            var svgElem = $flowline_dialog.find('svg');//divReport为需要截取成图片的dom的id
            svgElem.each(function (index, node) {
                var parentNode = node.parentNode;
                var svg = node.outerHTML.trim();

                var canvas = document.createElement('canvas');
                canvg(canvas, svg);
                if (node.style.position) {
                    canvas.style.position += node.style.position;
                    canvas.style.left += node.style.left;
                    canvas.style.top += node.style.top;
                }

                nodesToRecover.push({
                    parent: parentNode,
                    child: node
                });
                parentNode.removeChild(node);

                nodesToRemove.push({
                    parent: parentNode,
                    child: canvas
                });

                parentNode.appendChild(canvas);
            });
        }
        setTimeout(function () {
            html2canvas($flowline_dialog.children().get(0), {
                width: $flowline_dialog.children().outerWidth(),
                height: $flowline_dialog.children().outerHeight()
            }).then(function (canvas) {
                var imgUrl = canvas.toDataURL('image/jpeg');//图片地址
                //  document.body.appendChild(canvas);
                if (window.navigator.msSaveOrOpenBlob) {
                    let bstr = atob(imgUrl.split(",")[1]);
                    let n = bstr.length;
                    let u8arr = new Uint8Array(n);
                    while (n--) {
                        u8arr[n] = bstr.charCodeAt(n);
                    }
                    let blob = new Blob([u8arr]);
                    window.navigator.msSaveOrOpenBlob(blob, "chart-download" + "." + "png");
                } else {
                    // 这里就按照chrome等新版浏览器来处理
                    let a = document.createElement("a");
                    a.href = imgUrl;
                    a.setAttribute("download", "chart-download");
                    a.click();
                }
             //  $('<img>', { src: base64Str }).appendTo($('body'));//直接在原网页显示
            });
        },300)
    }
    function connectFlowLine($wrap) {
        $wrap.find('.xmsPointer-box').each(function (i,n) {
            var $parid = $(n).attr('data-parentid');
            var sourid = $(n).attr('data-id');
            if ($parid) {
                jsPlumb.connect({ source: $parid, target: sourid });
            }
        });
        
    }

    function getDataLayers(datas) {
        var layers =  [];
        function deepLayer(datas) {
            $.each(datas, function (i, n) {
                if (n.__layer && n.__layer > 0) {
                    if (!layers[n.__layer-1]) {
                        layers[n.__layer - 1] = [];
                    }
                    if (n.items.length > 0) {
                        if (n) {
                            layers[n.__layer - 1].push(n);
                        }
                        deepLayer(n.items);
                    } else {
                        if (n) {
                            layers[n.__layer - 1].push(n);
                        }
                    }

                }
            });
        }
         deepLayer(datas);
        console.log(layers);
        return layers;
    }
    function getSuperParent(item, datas) {
        var res = null;
        var checkid = item;
        if (typeof item !== 'string') {
            checkid = item.parentid;
        }
        $.each(datas, function (i, n) {

            if (n.id == checkid) {
                if (!n.parentid) {
                    res = n;
                    return false;
                } else {
                    getSuperParent(item, n.__parent.items);
                }
            } 
        });
        return res;
    }
    function getDatasMaxCountByLayer(datas) {
        var count = 0, layers = [], layer = 0;
       
        layers = getDataLayers(datas);
        console.log(layers);
        $.each(layers, function (i,n) {
            if (n.length > count) {
                count = n.length;
                layer = i;
            }
        });
        return {
            count: count,
            layers: layers,
            layer:layer
        }
    }
    function getDatasMaxWidth(datas) {
        var width = 0;
        var info = getDatasMaxCountByLayer(datas);
        if (info && info.layers.length>0) {
            $.each(info.layers[info.layer], function (i, n) {
                var $target = $('.xmsPointer-box[data-id="' + n.id + '"]');
                if ($target.length > 0) {
                    width += $target.outerWidth() + posSetting.itemMargin;
                }
            });
        }
        return width;
    }
    function setTargetPos($target,x, y) {
        $target.css({
            'top': y,
            'left': x
        });
    }
    function setFlowLinePost(datas,_datas) {
        $.each(datas, function (i, n) {
            var $target = $('.xmsPointer-box[data-id="' + n.id + '"]');
            if ($target.length > 0) {
                if (n.__layer == 1) {
                    var w = getDatasMaxWidth([n]);
                    $target.attr('data-wrapwidth', w);
                    
                    console.log(w);
                }
            }
            var x = 0;
            var y = 0;
            if (n.parentid) {
                var $super = getSuperParent(n.parentid, _datas);
                var _par = n.__parent;
                var curlen = _par.items.length;
                if (curlen > 0) {

                } else {

                }
            } else {

            }
            
            setTargetPos($target, x, y);
            if (n.items.length > 0) {
                setFlowLinePost(n.items,_datas);
            }
        });
        //var itemW = point.box.outerWidth();
        //point.x = ((n.__index || i) * (itemW + posSetting.itemMargin));
        //point.y = n.__layer * posSetting.itemH + currentH;
        //n.__point = point;
        //n.__width = itemW;
        //n.__height = point.y + posSetting.itemH;
        //currentW = point.x;
      //  point.setPos();
    }

    function renderFlowLine(opts, loadFlowAfter) {
        _renderFlow(opts, loadFlowAfter, function () {
            
        }, function () {

        });
    }
    var flowLine = {}
    flowLine.utils = {
        styleToString:function (obj) {
            var res = [];
            for (var i in obj) {
                if (obj.hasOwnProperty(i)) {
                    res.push(i + ':' + obj[i]);
                }
            }
            return res.join(';');
        },
        stringTostyle: function (str) {
            var temp = str.split(';');
            var obj = {};
            $.each(temp, function (i, n) {
                var testr = n.split(':');
                obj[testr[0]] = testr[1] * 1;
            });
            return obj;
        }
        
        
    }
    flowLine.outPutImg = outPutImg;
    flowLine.renderFlowLinePoint = renderFlowLinePoint;
    flowLine.loadInitFlowLine = function ($context, entityname, parentname, attrname, itemClick, sortby, isDefaultClick, callback, treesort, ordertype) {
        var _args;
        if (arguments.length == 1) {
            _args = arguments[0];
            callback = _args.callback;
        } else {
            _args = { $context: $context, entityname: entityname, parent: parentname, attrname: attrname, itemClick: itemClick, sortby: sortby, isDefaultClick: isDefaultClick, treesort: treesort, ordertype: ordertype };

        }
        renderFlowLine(_args, callback);
        
    }

    window.Xms_FlowLine = flowLine;
    //Xms_FlowLine.loadInitFlowLine($('body'),'BusinessUnit','ParentBusinessUnitId','name',null,'name')
})(jQuery, window);




