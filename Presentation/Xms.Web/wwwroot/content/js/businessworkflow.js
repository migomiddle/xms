function loadEntities(callback) {
    Xms.Web.GetJson('/api/schema/entity', {}, function (data) {
        if (!data || data.content.length == 0) return;
        console.log(data.content)
        $(data.content).each(function (i, n) {
            if (n.businessflowenabled) {
                $('#EntitySel').append('<option data-relationship="' + n.name + '"  value="' + n.entityid + '">' + n.localizedname + '</option>');
            }
        });
        $('#EntitySel').trigger('change');
        EntityId = $('#EntitySel').val();
        callback && callback();
    });
}
$(function () {
    var datasUrl = {
        attributes: "/api/schema/attribute?entityid="
        , entityById: "/api/schema/relationship/GetReferenced/"//获取关联的实体,
        , allEntity: '/api/schema/entity' //获取所有实体
    }

    function getEntityRelation(entityid, callback) {
        var data = {
            type: 'relation' + entityid,
            data: null
        }
        Xms.Web.PageCache('businessflow', datasUrl.entityById + entityid, data, function (data) {
            if (typeof (callback) == "function") {
                callback.call(this, data);
            }
        }, true);
    }
    function getAllEntity(callback, isall) {
        var data = {
            type: 'allentity',
            data: null
        }
        var getall = isall ? "?getall=true" : "";
        Xms.Web.PageCache('businessflow', datasUrl.allEntity + getall, data, function (alldatas) {
            if (typeof (callback) == "function") {
                callback.call(this, alldatas);
            }
        }, true);
    }
    function getAttributeByEntityId(entityid, callback, isall) {
        var data = {
            type: 'attribute' + entityid,
            data: null
        }
        var getall = isall ? "&getall=true" : "";
        Xms.Web.PageCache('businessflow', datasUrl.attributes + entityid + getall, data, function (alldatas) {
            if (typeof (callback) == "function") {
                callback.call(this, alldatas);
            }
        }, true);
    }
    function getEntityData(datas, sour, value, tar) {//获取sour属性等于value的 tar属性的值
        var res = null;
        $.each(datas, function (key, item) {
            if (item[sour].toLowerCase() == value.toLowerCase()) {
                res = item[tar];
                return false;
            }
        });
        return res;
    }

    $('.fold').click(function () {
        var $this = $(this);
        if ($this.data('status') === false) {//折叠
            $this.html('展开<i class="icon-fold"></i>').data('status', true);
        } else {//展开
            $this.html('折叠<i class="icon-fold-down"></i>').data('status', false);
        }
        $('.work-flow').stop().slideToggle();
    });

    //添加关联实体
    $('.addEntityRelationBtn').on('click', function () {
        var wrap = $('#entityMenu');
        var items = wrap.children('li.menu-item');
        gApp.entityMenulist.removeAll();
        console.log(items)
        if (items.length == 0) {
            getAllEntity(function (datas) {
                if (datas.content && datas.content.items) {
                    var options = [];
                    var crumbs = [];

                    if (!datas.content.items || datas.content.items.length == 0) { return false; }
                    console.log('datas.content', datas.content);
                    //console.log('item.entityname', item.entityname);
                    $.each(datas.content.items, function (key, item) {
                        //if (item.businessflowenabled) {
                        gApp.entityMenulist.addItem(
                            item.name,
                            item.entityid,
                            item.localizedname
                        )
                        //  }
                    });
                } else {
                }
            }, true);
        } else {
            var last = items.last();
            var entityid = last.children().attr('data-id');
            //  alert(entityid);
            getEntityRelation(entityid, function (datas) {
                console.log(datas.content);
                if (datas && datas.content) {
                    var options = [];
                    var crumbs = [];
                    console.log('datas.content', datas.content);
                    if (!datas.content || datas.content.length == 0) { return false; }
                    $.each(datas.content, function (key, item) {
                        //  if (item.businessflowenabled) {
                        gApp.entityMenulist.addItem(
                            item.referencingentityname,
                            item.referencingentityid,
                            item.referencingentitylocalizedname,
                            item.name
                        )
                        //   }
                    });
                }
            }, true);
        }
    });

    $('#entityMenu').on('click', 'li.menu-item', function () {
        var $this = $(this);
        $this.siblings('li.menu-item').removeClass('active').end().addClass('active');
    });

    $('button[type="submit"]').off('click').on('click', function (e) {
        if (e.preventDefault) { e.preventDefault() }
        if (!saveCurrentFlow()) return false;
        //var postData = {
        //    name: $('#Name').val(),
        //    StepData: $('#StepData').val(),
        //    entityid: $('#EntityId').val(),
        //    Description: $('#Description').val()
        //}
        var postData = $(this).parents('form').serializeFormJSON();
        console.log('需要提交的数据', postData);
        console.log('需要提交的 JSON 数据', JSON.stringify(postData));
        var url = '/customize/flow/CreateBusinessFlow';
        if (PAGE_TYPE == 'EDIT') {
            url = '/customize/flow/EditBusinessFlow';
            postData.WorkFlowId = $('#WorkFlowId').val();
        }
        Xms.Web.Post(url, postData, false, function (res) {
            if (PAGE_TYPE == 'CREATE') {
                if (res.IsSuccess) {
                    Xms.Web.Alert(true, res.Content, function () {
                        location.href = ORG_SERVERURL + '/customize/flow/editbusinessflow?id=' + res.Extra.id;
                    });
                } else {
                    Xms.Web.Alert(false, res.Content);
                }
            } else {
                if (res.IsSuccess) {
                    Xms.Web.Alert(true, res.Content);
                } else {
                    Xms.Web.Alert(false, res.Content);
                }
            }
        }, null, null, false);
        //$('form').submit();
        return false;
    });

    function baseModel(name, key, value, rel) {
        var self = this;
        this.key = ko.observable(key);
        this.name = ko.observable(name);
        this.value = ko.observable(value);
        this.relationName = ko.observable(rel || '');
        this.options = ko.observableArray([]);
        this.addStep = function (obj, e) {
            e = e || window.event;
            //console.log(e);
            var target = e.target || e.srcElement;
            var $this = $(target);
            console.log($this);
            var entityid = $this.attr('data-id');
            var entityname = $this.text();

            getAttributeByEntityId(entityid, function (res) {
                console.log(res);
                if (!res || !res.content) return false;
                self.options.push({ optionVal: '请选择', optionId: '' });
                $.each(res.content, function (key, item) {
                    self.options.push({ optionVal: item.localizedname, optionId: item.attributeid });
                });
            }, true);
            gApp.entityList.addEntity(self.name, this.value, this.key, this.relationName);
        }
    }

    function entityMenu() {
        this.list = ko.observableArray([]);
        this.addItem = function (name, key, value, rel) {
            this.list.push(new baseModel(name, key, value, rel));
        }
        this.removeAll = function () {
            this.list.removeAll();
        }
        this.remove = function () {
            if (this.list().length > 1) {
                this.list.shift();
            }
        }
    }

    //步骤
    function StepModel(attributename, displayname, isrequired, attrvalue) {
        //  console.log('attributename', attributename);
        var self = this;
        this.attrname = ko.observable(attributename || '字段名');
        this.attrvalue = ko.observable(attrvalue);
        this.isrequired = isrequired || false;
        this.displayname = ko.observable(displayname || '步骤名');
        this.options = ko.observableArray([]);
        this.isAttrDefault = true;
        this._super = null;
        this.getOptions = function () {
            var entityid = '';
            if (typeof self._super._super.entityid() === 'function') {
                entityid = self._super._super.entityid()();
            } else {
                entityid = self._super._super.entityid();
            }
            getAttributeByEntityId(entityid, function (res) {
                // console.log(res.content);
                self.options.push({ optionText: '请选择', optionValue: '' });
                $.each(res.content, function (key, item) {
                    self.options.push({ optionText: item.localizedname, optionValue: item.name });
                });
            }, true)
        }

        this.nameMouseEnter = function (obj, e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            var $target = $(target);
            var $input = $target.siblings('input');
            if ($input.length > 0) {
                $input.removeClass('c-hide');
                $target.hide();
            }
        }
        this.nameMouseOut = function (obj, e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            var $target = $(target);
            var $span = $target.siblings('span');
            var val = $target.val();
            // console.log(val);
            if ($span.length > 0) {
                $span.show();
                $target.addClass('c-hide');
                if (val == '') {
                    val = '步骤名';
                }
                self.displayname(val);
            }
        }

        this.attrMouseEnter = function (obj, e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            var $target = $(target);
            var $input = $target.siblings('select');
            if ($input.length > 0) {
                $input.removeClass('c-hide');
                $target.hide();
            }
        }
        this.attrChange = function (obj, e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            var $target = $(target);
            var $span = $target.siblings('span');
            var name = $target.find('option:selected').text();
            var value = $target.val();
            //console.log(name);
            if ($span.length > 0) {
                if (!name || name == "" || name == "请选择") {
                    name = '字段名';
                }
                self.attrname(name);
                self.attrvalue(value);
                self.displayname(name);
                self.isAttrDefault = false;
            }
            // console.log('self.isAttrDefault',self.isAttrDefault)
        }
        this.attrMouseOut = function (obj, e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            var $target = $(target);
            var $span = $target.siblings('span');
            var name = $target.find('option:selected').text();
            var val = $target.val();
            // console.log(val);
            if ($span.length > 0) {
                $span.show();
                $target.addClass('c-hide');
                if (!val || val == "") {
                    val = '字段名';
                }

                //  self.attrname(val);
            }
        }
        this.changeIsrequire = function (obj, e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            if (e.stopPropagation) { e.stopPropagation(); }
            var $target = $(target);
            if (self.isrequired() == true) {
                self.isrequired(false);
            } else {
                self.isrequired(true);
            }
            return false;
        }
    }

    //表格行
    function itemModel(name, stagetype, parid, processstageid) {
        var self = this;
        this.name = ko.observable(name || '阶段');
        this.stagetype = ko.observable(stagetype || '阶段类型');
        this.processstageid = processstageid || null;
        this.entityid = parid; // 对应面包屑的实体ID
        this._super = null;
        this.isShow = ko.observable(false); //控制要显示的实体的数据
        this.steps = ko.observableArray([]);
        this.nameMouseEnter = function (obj, e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            var $target = $(target);
            var $input = $target.siblings('input');
            if ($input.length > 0) {
                $input.removeClass('c-hide');
                $target.hide();
            }
        }
        this.nameMouseOut = function (obj, e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            var $target = $(target);
            var $span = $target.siblings('span');
            var val = $target.val();
            console.log(val);
            if ($span.length > 0) {
                if (val == '') {
                    val = '阶段';
                }
                self.name(val);
                $span.show();
                $target.addClass('c-hide');
            }
        }
        this.addStep = function (attributename, displayname, isrequired, attrvalue) {
            console.log('itemModel', attributename)
            var step = new StepModel(attributename, displayname, isrequired, attrvalue);
            step._super = this;
            step.getOptions();
            this.steps.push(step);
            return step;
        }
        this.removeAll = function () {
            this.steps.removeAll();
        }
        this.stageHandler = function (obj, e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            if (e.stopPropagation) { e.stopPropagation(); }
            var $target = $(target);
            obj._super.showItem(obj);
            return true;
        }

        this.delStep = function (obj, e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            if (e.stopPropagation) { e.stopPropagation(); }

            if (self.steps().length == 1) return false;

            self.steps.remove(this);
        }
    }

    function EntityModel(name, entityname, id, rel) {
        var self = this;
        this.entityname = ko.observable(entityname || '');
        //console.log('name', name());
        this.entityvalue = ko.observable(name || '');
        this.entityid = ko.observable(id || '');
        this.relationshipname = ko.observable(rel || '');//关系名称
        this.isShow = ko.observable(false); //控制要显示的实体的数据
        this._super = null;//获取列表的方法，用于显示隐藏对应实体数据
        this.last = null;
        this.items = ko.observableArray([]);
        this.addItems = function (name, stagetype) { //添加阶段
            var parid = $('li.menu-item.active', '#entityMenu').attr('data-id');
            parid = parid || this.getEntityId();
            //parid = gApp.entityList().list;
            var item = new itemModel(name, stagetype, parid);
            this.last = item
            //var len = this.items().length;
            //item.isShow(true);

            //if (len >= 1) { this.isDelete(true); }
            this.items.push(item);
            item._super = this;
            if (item.steps().length == 0) {
                item.addStep('', '', '', '');
            }
            this.showItem(item);
            return item;
            //$('tr.step-item').off('click').on('click', function () {
            //    $(this).siblings('tr').removeClass('active').end().addClass('active');
            //});
        }
        this.addEditItems = function (name, stagetype, processstageid) { //添加阶段
            var parid = $('li.menu-item.active', '#entityMenu').attr('data-id');
            parid = parid || this.getEntityId();
            var item = new itemModel(name, stagetype, parid, processstageid);

            this.last = item
            var len = this.items().length;
            //item.isShow(true);

            this.items.push(item);
            item._super = this;
            this.showItem(item);
            return item;
            //$('tr.step-item').off('click').on('click', function () {
            //    $(this).siblings('tr').removeClass('active').end().addClass('active');
            //});
        }
        this.addStep = function (attributename, displayname, isrequired, attrvalue) {
            if (!this.last) return false;
            // console.log(attributename, displayname, isrequire)
            this.last.addStep(attributename, displayname, isrequired, attrvalue);
        }
        this.addLastStep = function (attributename, displayname, isrequired, attrvalue) {
            if (!this.last) return false;
            //console.log(attributename, displayname, isrequire)
            this.last.addStep(attributename, displayname, isrequired, attrvalue);
        }
        this.delStage = function (obj, e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            if (e.stopPropagation) { e.stopPropagation(); }
            if (self.items().length == 1) return false;

            self.items.remove(this);
            self.showItem(self.items()[0]);
        }
        //console.log(this.entityid()())
        this.entityHandle = function (obj, e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            var $target = $(target);
            if (typeof this.entityid() === 'function') {
                self._super.showEntity(this.entityid()());
            } else {
                self._super.showEntity(this.entityid());
            }
        }
        this.activeStage = function (obj, e) {
            //console.log(obj)
            this.last = obj;
        }
        this.getIndex = function (item) {
        }
        this.moveUp = function (obj, e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            if (e.stopPropagation) { e.stopPropagation(); }
            if (!self.last) return false;
            var index = self.items.indexOf(self.last);
            if (index == 0) return false;
            var del = self.items.splice(index - 1, 1);
            console.log('del())', del);
            self.items.splice(index, 0, del[0]);
        }
        this.moveDown = function (obj, e) {
            e = e || window.event;
            var target = e.target || e.srcElement;
            if (e.stopPropagation) { e.stopPropagation(); }
            if (!self.last) return false;

            var index = self.items.indexOf(self.last);
            var length = self.items().length;
            if (index >= length - 1) return false;
            var del = self.items.splice(index + 1, 1);
            console.log('del())', del);
            self.items.splice(index, 0, del[0]);
        }

        this.showItem = function (item) {
            $.each(this.items(), function (key, e) {
                e.isShow(false);
            });
            item.isShow(true);
            this.last = item
        }
        this.getEntityId = function () {
            var entityid;
            if (typeof this.entityid() === 'function') {
                entityid = this.entityid()();
            } else {
                entityid = this.entityid();
            }
            return entityid;
        }
    }

    function entityList() {
        var self = this;
        this.list = ko.observableArray([]);//对应面包屑列表
        this.isDelete = ko.observable(false);

        this.addEntity = function (name, entityname, id, rel) { //添加面包屑
            var entity = new EntityModel(name, entityname, id, rel);
            var len = this.list().length;
            if (len == 0) { entity.isShow(true); }
            if (len >= 1) { this.isDelete(true); }
            entity._super = this;
            this.list.push(entity);
            if (typeof entity.entityid() === 'function') {
                this.showEntity(entity.entityid()())
            } else {
                this.showEntity(entity.entityid())
            }

            return entity;
        }

        this.showEntity = function (id) {
            $.each(this.list(), function (key, item) {
                var entityid = '';
                if (typeof item.entityid() === 'function') {
                    entityid = item.entityid()();
                } else {
                    entityid = item.entityid();
                }
                // console.log('entityid == id', entityid == id)
                if (entityid == id) {
                    item.isShow(true);
                } else {
                    item.isShow(false);
                }
            });
        }
        this.remove = function () {
            var len = this.list().length;
            // console.log(len)
            if (len > 1) {
                this.list.pop();
                var reslen = this.list().length;
                var entityid = '';
                if (typeof this.list()[reslen - 1].entityid() === 'function') {
                    entityid = this.list()[reslen - 1].entityid()();
                } else {
                    entityid = this.list()[reslen - 1].entityid();
                }
                this.showEntity(entityid);
                if (reslen <= 1) {
                    this.isDelete(false);
                }
            }
        }
        this.removeAll = function () {
            this.list.removeAll();
        }
    }

    function saveCurrentFlow() {
        var flowName = $('#Name').val();
        var flowdesc = $('#Description').val();
        var entityid = $('#EntityId').val();
        if (flowName == '') {
            Xms.Web.Alert(false, '请先输入流程名字');
            return false;
        }
        //if (flowdesc == '') {
        //    Xms.Web.Alert(false, '请先输入流程描述');
        //    return false;
        //}
        var result = [];
        var datas = gApp.entityList.list();

        datas = ko.toJS(datas);
        if (datas.length == 0) {
            Xms.Web.Alert(false, '请先添加实体');
            return false;
        }
        var stepcount = 0;
        var checkflag = true;//检查阶段的填写
        var checkStageMsg = '';

        var checkStep = true;
        var checkStepMsg = '';
        $.each(datas, function (i, n) {
            var entity = {
                entityid: n.entityid,
                relationshipname: n.relationshipname
            }
            var checkstage = 0;
            $.each(n.items, function (ii, nn) {
                checkstage++;
                var stage = {
                    entityid: entity.entityid,
                    processstageid: nn.processstageid,
                    relationshipname: entity.relationshipname,
                    Name: nn.name,
                    StageOrder: ++stepcount,
                    steps: [],
                    Steps: ''
                }
                var stepflag = true;
                $.each(nn.steps, function (iii, nnn) {
                    var step = {
                        attributename: nnn.attrvalue,
                        displayname: nnn.displayname,
                        isrequired: nnn.isrequired
                    }
                    if (step.isrequired == true) {
                        if (step.attributename == "") {
                            stepflag = false;
                            checkStepMsg = '请选择 实体 ' + n.entityname + ', 阶段 ' + stage.Name + ' 的字段名';
                            return false;
                        }
                    }
                    if (nnn.isAttrDefault == true) {
                        checkStepMsg = '请选择 实体 ' + n.entityname + ', 阶段 ' + stage.Name + ' 的字段名';
                        stepflag = false;
                        return false;
                    }
                    stage.steps.push(step);
                });
                if (stepflag == false) {
                    checkStep = false;
                    return false;
                }
                stage.Steps = JSON.stringify(stage.steps);
                delete stage.steps;
                result.push(stage);
            });
            if (checkstage == 0) {
                checkStageMsg = '请先添加 实体 ' + n.entityname + ' 的阶段'
                checkflag = false;
                return false;
            }
            if (!checkStep) {
                checkflag = false;
                return false;
            }
            //entity.Steps = JSON.stringify(entity.steps);
        });
        if (!checkflag) {
            if (checkStepMsg != '') {
                Xms.Web.Alert(false, checkStepMsg);
                return false;
            }
            Xms.Web.Alert(false, checkStageMsg);
            return false;
        }

        //提交数据

        //var postData = {
        //    name: $('#Name').val(),
        //    StepData: result
        //}
        //console.log('需要提交的数据', postData);
        //console.log('需要提交的 JSON 数据', JSON.stringify(postData));
        //Xms.Web.Post('createworkflow', postData, false, function (res) {
        //    console.log(res);
        //},null,null,false);
        $stepDom.val(JSON.stringify(result));
        return true;
        //return false;
        // $('form:first').submit();
    }

    //加载数据
    var $stepDom = $('#StepData');
    var gApp = window.gApp = {};

    //是编辑还是新建
    if ($stepDom.val() == "") {//新建
        gApp.entityMenulist = new entityMenu(); // 添加实体时的菜单
        gApp.entityList = new entityList(); // 已添加的实体
        ko.applyBindings(gApp, document.getElementById('bussinessWrap'));
        //[{
        //    "processstageid": "1f0eb520-1e61-4ba5-a918-54b6a88a3f84",
        //    "name": "商机流程",
        //    "category": 0,
        //    "entityid": "48303fb7-2a1d-4c43-922e-d641ffb51815",
        //    "relationshipname": "",
        //    "workflowid": "11b241b9-544d-4fb4-9b11-2fdf2fdc3cfe",
        //    "stageorder": 1,
        //    "steps": "[{\"attributename\":\"SaleStage\",\"displayname\":\"销售阶段\",\"isrequired\":false},{\"attributename\":\"BudgetAmount\",\"displayname\":\"预算金额\",\"isrequired\":false}]"
        //}]
    } else {
        getAllEntity(initPage, true);//编辑时加载数据
    }

    function dataHandler(datas, callback) {
        var result = [];
        var count = 0;
        var length = 0;
        datas.sort(function (a, b) {
            return a.stageorder - b.stageorder;
        });
        // console.table(datas);
        $.each(datas, function (i, n) {
            if (typeof result[n.entityid] === 'undefined') {
                result[n.entityid] = [];
            }
            result[n.entityid].relationshipname = n.relationshipname;
            result[n.entityid].push(n);
        });

        for (var i in result) {
            if (result.hasOwnProperty(i)) {
                length++;
                var n = result[i];
                var temp = [];
                $.each(n, function (ii, nn) {
                    var _name = encodeURI(nn.name);
                    if (typeof temp[_name] === 'undefined') {
                        temp[_name] = [];
                    }
                    temp[_name].push(nn);
                    temp.relationshipname = nn.relationshipname;
                });
                result[i] = temp;
            }
        };

        return result;
    }

    function initPage() {
        gApp.entityMenulist = new entityMenu(); // 添加实体时的菜单
        gApp.entityList = new entityList(); // 已添加的实体
        ko.applyBindings(gApp, document.getElementById('bussinessWrap'));
        var editDatas = JSON.parse($stepDom.val());
        var datas = dataHandler(editDatas);
        //  console.log(datas);
        var entitydatas = Xms.Web.PageCacheData['businessflow']['allentity'].data.content;
        var count = 0;
        var ischange = false;
        var currentEntityid = '';
        console.log('datas', datas)
        for (var i in datas) {
            count++;
            if (datas.hasOwnProperty(i) && typeof datas[i] !== 'function') {
                (function (jj, _count) {
                    var n = datas[jj];
                    var rel = n.relationshipname || '';

                    if (rel == "") {//第一个实体的数据
                        var name = getEntityData(entitydatas, 'entityid', jj, 'name');

                        var key = jj;
                        var value = getEntityData(entitydatas, 'entityid', jj, 'localizedname');
                        var entity = gApp.entityList.addEntity(name, value, key, rel);
                        ; (function (k, kentityid) {
                            currentEntityid = kentityid;
                            getAttributeByEntityId(kentityid, function () {
                                var stagesData = Xms.Web.PageCacheData['businessflow']['attribute' + kentityid].data.content;
                                for (var ii in k) {
                                    if (k.hasOwnProperty(ii) && ii != 'relationshipname') {
                                        var nn = k[ii];
                                        $.each(nn, function (iii, nnn) {
                                            var stagename = nnn.name;
                                            var stagetype = nnn.stagetype || '';
                                            var processstageid = nnn.processstageid;
                                            console.log('processstageid', processstageid);
                                            var stage = entity.addEditItems(stagename, stagetype, processstageid);

                                            var stepStr = nnn.steps;
                                            var stepArr = JSON.parse(stepStr);
                                            $.each(stepArr, function (key, item) {
                                                var attrname = getEntityData(stagesData, 'name', item.attributename, 'localizedname');
                                                var displayname = item.displayname;
                                                var isrequired = item.isrequired;
                                                var attrvalue = item.attributename;
                                                var step = stage.addStep(attrname, displayname, isrequired, attrvalue);
                                                step.isAttrDefault = false;
                                            });
                                        });
                                    }
                                };
                            }, true);
                        })(n, i);
                    } else {//后面关联的实体的数据
                        var curentityid = currentEntityid
                        getEntityRelation(curentityid, function () {
                            var relationdatas = Xms.Web.PageCacheData['businessflow']['relation' + curentityid].data.content;
                            currentEntityid = jj;
                            var name = getEntityData(relationdatas, 'referencingentityid', jj, 'referencingentityname');//relationdatas[0]['referencingentityname'];
                            var key = jj;
                            var value = getEntityData(relationdatas, 'referencingentityid', jj, 'referencingentitylocalizedname'); //relationdatas[0]['referencingentitylocalizedname']//getEntityData(relationdatas, 'referencingentityid', jj, 'referencingentitylocalizedname');
                            var entity = gApp.entityList.addEntity(name, value, key, rel);
                            ; (function (k, kentityid) {
                                getAttributeByEntityId(jj, function () {
                                    var stagesData = Xms.Web.PageCacheData['businessflow']['attribute' + jj].data.content;
                                    for (var ii in k) {
                                        if (k.hasOwnProperty(ii) && ii != 'relationshipname') {
                                            var nn = k[ii];
                                            $.each(nn, function (iii, nnn) {
                                                var stagename = nnn.name;
                                                var stagetype = nnn.stagetype || '';
                                                var processstageid = nnn.processstageid;
                                                console.log('processstageid', processstageid);
                                                var stage = entity.addEditItems(stagename, stagetype, processstageid);
                                                var stepStr = nnn.steps;
                                                var stepArr = JSON.parse(stepStr);
                                                $.each(stepArr, function (key, item) {
                                                    Xms.Web.PageCacheData['businessflow']['attribute' + jj].data.content;
                                                    var attrname = getEntityData(stagesData, 'name', item.attributename, 'localizedname');
                                                    var displayname = item.displayname;
                                                    var isrequired = item.isrequired;
                                                    var attrvalue = item.attributename;
                                                    setTimeout(function () {
                                                        var step = stage.addStep(attrname, displayname, isrequired, attrvalue);
                                                        step.isAttrDefault = false;
                                                    }, 10);
                                                });
                                            });
                                        }
                                    };
                                }, true);
                                //  }, 50);
                            })(n, i);
                        });
                    }
                })(i, count);
            }
        };
        $('.step-fields').css('width', ($(document).width() - 70) + 'px');
        $(window).on('resize', function () {
            $('.step-fields').css('width', ($(document).width() - 70) + 'px');
        })
    }

    function entitychange() {
        $('#EntitySel').on('change', function () {
            Xms.Web.Confirm('更改实体', '更换实体会清除之前的数据，是否更改？', function () {
                gApp.entityList.removeAll();
                gApp.entityMenulist.removeAll();
                var name = $('#EntitySel').find('option:selected').attr('data-relationship');
                var value = $('#EntitySel').find('option:selected').text();
                var key = $('#EntitySel').find('option:selected').val();
                var dEntity = gApp.entityList.addEntity(name, value, key);
                var dItems = dEntity.addItems();
                entitychange();
            });
        })
    }

    if (PAGE_TYPE == "CREATE") {
        loadEntities(function () {
            var name = $('#EntitySel').find('option:selected').attr('data-relationship');
            var value = $('#EntitySel').find('option:selected').text();
            var key = $('#EntitySel').find('option:selected').val();

            var dEntity = gApp.entityList.addEntity(name, value, key);
            console.log(gApp.entityList)
            var dItems = dEntity.addItems();
            entitychange();
            setTimeout(function () {
                Xms.Web.fullByContext($(window), $('#collapseTwo'), -120);
                $('.step-fields').css('width', ($(document).width() - 70) + 'px');
                $(window).on('resize', function () {
                    $('.step-fields').css('width', ($(document).width() - 70) + 'px');
                })
            }, 50)
        });
    } else {
        loadEntities(function () {
            Xms.Web.SelectedValue($('#EntitySel'), $('#EntitySel').attr('data-value'));
            entitychange();
            setTimeout(function () {
                Xms.Web.fullByContext($(window), $('#collapseTwo'), -120)
            }, 50)
        })
    }

    window.saveCurrentFlow = saveCurrentFlow;
});