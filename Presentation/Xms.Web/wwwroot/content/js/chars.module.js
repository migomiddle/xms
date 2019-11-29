(function ($, root, un) {
    "use strict"
    function pChart(title, xAris, yAxis, series, tooltips, legend) {
        this.title = title || { text: "title" };
        this.tooltips = tooltips || {};
        this.legend = {};
        this.legend.data = legend || [];
        this.xAris = xAris;
        this.yAris = yAxis;
        this.series = series;
        this.mychar = null;
    }

    pChart.prototype.render = function (target) {
        this.mychar = echart.init(target);
        myChart.setOption(new ChartsOpts(this.title, this.xAris, this.yAxis, this.series, this.tooltips, this.legend.data));
    }
})(jQuery, window);

(function ($, root, un) {
    "use strict"
    var defaults = {
        maxLength: 5,
        addBtn: null,
        removeBtn: null,
        ctrlClass: "",
        itemClass: ".electrldomItem",
        addHandler: null,
        ctrlHandler: null
    }
    function ElementCtrl(target, opts) {
        opts = $.extend({}, defaults, opts);
        if (!this instanceof ElementCtrl) {
            return new ElementCtrl(target, opts);
        }
        this.target = $(target);
        this.clone = this.target.find(opts.itemClass).clone();
        this.target.find(".itemClass").addClass("electrl-item");
        this.opts = $.extend({}, defaults, opts);
        this.addBtn = this.opts.addBtn;
        this.removeBtn = this.opts.removeBtn;
        this.length = 0;
        this.list = [];
        this.maxLength = this.opts.maxLength;
        this.init();
    }
    ElementCtrl.prototype.init = function () {
        if (this.addBtn.length > 0 && this.removeBtn.length > 0) {
            this.checkShow();
            this.bindEvent();
        }
    }
    ElementCtrl.prototype.checkShow = function () {
        if (this.length >= this.maxLength) {
            this.addBtn.hide();
            this.removeBtn.show();
        } else if (this.length < this.maxLength) {
            if (this.length >= 1) {
                this.addBtn.show();
                this.removeBtn.show();
            } else {
                this.removeBtn.hide();
                this.addBtn.show();
            }
        }
    }
    ElementCtrl.prototype.add = function () {
        if (this.length > this.maxLength) return false;
        var clone = this.clone.clone();
        clone.eleId = "eleid_" + setTimeout(0);
        if (this.opts.ctrlClass !== "") {
            var ctrlItem = $('<div class="col-sm-1 btn">' +
                '<span class="sItemDel"><em class="glyphicon glyphicon-remove"></em></span>' +
                '</div>');
            clone.append(ctrlItem);
        }

        this.list.push(clone);
        //if (this.list.length > 1) {
        //this.list[this.list.length - 1].append(clone);
        // } else {
        this.target.append(clone);
        // }
        this.length++;
        var curent = this.length, self = this;
        this.bindEvent();
        ctrlItem.find(".sItemDel").on("click", function () {
            self.list.splice(curent, 1);
            self.length--;
            clone.remove();
            self.checkShow();
        });
    }
    ElementCtrl.prototype.remove = function (index) {
        if (!index) {
            this.list[this.length - 1].remove();
            this.list.splice(-1, 1);
        } else {
            this.list[index].remove();
            this.list.splice(index, 1);
        }
        this.length--;
        this.bindEvent();
    }
    ElementCtrl.prototype.bindEvent = function () {
        var self = this;
        this.addBtn.off("click").on("click", function () {
            self.add();
            self.opts.addHandler && self.opts.addHandler(self);
        });
        this.removeBtn.off("click").on("click", function () {
            self.remove();
            self.opts.ctrlHandler && self.opts.ctrlHandler(self);
        });
        this.checkShow();
    }

    $.fn.ElementCtrl = function (opts) {
        return this.each(function () {
            new ElementCtrl(this, opts)
        });
    }
})(jQuery, window);

(function ($, root, un) {
    "use strict"
    var slice = [].slice, idPreFix = "idFixed";
    function dataStack(data, element) {
        this.id = idPreFix + setTimeout(0);
        this.data = data;
        this.element = element;
    }
    function dataStackHandle() {
        this.datas = [];
        this.length = 0;
    }
    dataStackHandle.prototype.add = function (data) {
        this.datas.push(data);
        this.length++;
        return this;
    }
    dataStackHandle.prototype.remove = function (data) {
        var index = this.indexOf(data);
        if (index === -1) { return false; }
        this.datas.splice(index, 1);
        this.length--;
        return this;
    }
    dataStackHandle.prototype.indexOf = function (data) {
        var index = -1;
        for (var i = 0, len = this.datas.length; i < len; i++) {
            var item = this.datas[i];
            if (item.id === data.id) {
                index = i;
                break;
            }
        }
        return index;
    }

    root.dataStack = dataStack;
    root.dataStackHandle = dataStackHandle;
})(jQuery, window);

(function ($, root) {
    "use strict"
    var createChartMethod = {
        chartTypeChange: function (obj) {
            var $this = $(obj), value = $this.find("option:selected").val();
            console.log(value)
        },
        saveSubmit: function () {
            var postData = {};
            postData["fetch"] = [];
            $(".yarias-item").each(function () {
                var temp = {};
                temp["attribute"] = $(this).find("input[name='attribute'] option:selected").val();
                temp["type"] = $(this).find("input[name='type'] option:selected").val();
                temp["aggregate"] = $(this).find("input[name='aggregate'] option:selected").val();
                temp["dategrouping"] = $(this).find("input[name='dategrouping'] option:selected").val();
                temp["groupby"] = $(this).find("input[name='groupby'] option:selected").val();
                postData["fetch"].push(temp);
            });

            postData["name"] = $("input[name='Name']").val();
            postData["chartType"] = $("input[name='chartType'] option:selected").val();
        }
    }

    root.createChartMethod = createChartMethod;
})(jQuery, window);

(function ($, root) {
    //表单序列化
    "use strict"
    var defaults = {
        className: ".cc-form-ctrl"
    }
    var method_public = {}, slice = [].slice;
    method_public["get"] = function (obj, opts) {
        var $this = $(obj), len = $this.length, res = {}, fixed = "Series_"
        $this.each(function () {
            var that = this, $that = $(that);
            $(that).find("input").each(function () {
            });
        });
    }
    method_public["init"] = function (opts) {
    }
    $.fn.ccFormSeries = function () {
        var args = slice.call(arguments), argsLen = args.length;
        if (argsLen === 1) {
            return method_public["init"].call(this, args[0]);
        } else if (argsLen === 2) {
            return method_public["get"](this, opts);
        }
    }
})(jQuery, window);

(function ($, root) {
    "use strict"
    var ChartData = function (opts) {
        var tempdataDefault = {
            title: {
                text: "default.Title"
            },
            tooltips: [],
            legend: {
                data: ["收益"]
            },
            xAxis: {
                data: []
            },
            yAxis: {
                data: []
            },
            series: []
        }
        $.extend(this, tempdataDefault, opts);
    }
    ChartData.prototype.toString = function () {
        return JSON.stringify(this);
    }
    ChartData.prototype.filter = function (objs) {
    }

    function chartsDataHandle(data, callback) {
        var temp = data;
        var chartData = new ChartData();
        for (var i = 0, len = temp.fetch.length; i < len; i++) {
            (function (j) {
                var item = temp.fetch[j];
                var postData = {
                    attribute: item.attribute,
                    aggregate: item.aggregate,
                    dategrouping: item.dategrouping,
                    groupby: item.groupby
                }
                //getAriasData(item.type, postData).done(function (data) {
                //    if (item.type === "series") {
                //        chartData.yAxis.data = data;

                //    } else if (item.type === "category") {
                //        chartData.xAxis.data = data;
                //    }
                //    callback && callback(chartData)
                //}, function (data) {
                //测试
                if (item.type === "series") {
                    chartData.yAxis.data = {};
                } else if (item.type === "category") {
                    chartData.xAxis.data = ["05-01", "05-02", "05-03", "05-04", "05-05", "05-06"];
                }
                if (i === len - 1) {
                    callback && callback(chartData);
                }
                //});
            })(i);
        }
    }

    function getAriasData(type, data) {
        var defer = $.Deferred(), url = "";
        if (type === "series") {
            url = "datas/type.json";
        } else if (type === "category") {
            url = "datas/type.json";
        }
        $.ajax({
            url: url,
            data: data,
            type: "get",
            contentType: "application/json; charset=utf-8"
        }).done(function (data) {
            defer.resolve(data);
        });
        return defer.promise();
    }

    //插入图表
    function createChart(target, data, style, type, className) {
        type = type || "append";
        var chartDom = document.createElement("div"), $target = $(target);
        chartDom.className = className;
        style && $(chartDom).css(style);
        $target[type](chartDom);
        var mychart = echarts.init(chartDom);
        mychart.setOption(data);
    }

    function ChartSeries(name, type, data, opts) {
        var self = this;
        this.name = name || "SeriesName";
        this.type = type || "bar";
        this.data = data || [];
        this.opts = opts || {};

        if (this.type === "funnel") {
            var tempObj = {
                left: '10%',
                top: 60,
                //x2: 80,
                bottom: 60,
                width: '80%',
                // height: {totalHeight} - y - y2,
                min: 0,
                max: 100,
                minSize: '0%',
                maxSize: '100%',
                sort: 'descending',
                gap: 2,
                label: {
                    normal: {
                        show: true,
                        position: 'inside'
                    },
                    emphasis: {
                        textStyle: {
                            fontSize: 20
                        }
                    }
                },
                labelLine: {
                    normal: {
                        length: 10,
                        lineStyle: {
                            width: 1,
                            type: 'solid'
                        }
                    }
                },
                itemStyle: {
                    normal: {
                        borderColor: '#fff',
                        borderWidth: 1
                    }
                }
            }
            $.extend(self, tempObj);
        }
    }

    root.ChartData = ChartData;
    root.createChart = createChart;
    root.chartsDataHandle = chartsDataHandle;
    root.ChartSeries = ChartSeries;
})(jQuery, window);

; (function ($) {
    var backCover = $('<div class="backCover"></div>');
    var tempList = [];
    $.fn.jModal = function (type, isremove) {
        if (type === "show") {
            $("body").append(backCover);
            $(this).show().addClass("in");
            tempList.push($(this));
        } else if (type === "hide") {
            $(this).hide().removeClass("in");
            tempList.pop();
            if (isremove) {
                $(this).remove();
            }
            backCover.remove();
        }
    }
    $.fn.jModal.List = tempList;
    $("body").on("click", "[jmodal=modal]", function () {
        var $target = $(this).attr("data-target");
        if (typeof $target === "string") {
            $($target).jModal("show");
        }
    });
    $(".j-modal").on("click", function (e) {
        var $target = $(e.target);
        if ($target.closest(".j-modal-box").length === 0) {
            $.fn.jModal.List[0].jModal("hide");
        }
    });
})(jQuery);