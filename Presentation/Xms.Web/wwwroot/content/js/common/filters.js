//闭包执行一个立即定义的匿名函数
!function (factory) {
    //factory是一个函数，下面的koExports就是他的参数

    // Support three module loading scenarios
    if (typeof require === 'function' && typeof exports === 'object' && typeof module === 'object') {
        // [1] CommonJS/Node.js
        // [1] 支持在module.exports.abc,或者直接exports.abc
        var target = module['exports'] || exports; // module.exports is for Node.js
        factory(target);
    } else if (typeof define === 'function' && define['amd']) {
        // [2] AMD anonymous module
        // [2] AMD 规范
        //define(['exports'],function(exports){
        //    exports.abc = function(){}
        //});
        define(['jquery'], factory);
    } else {
        // [3] No module loader (plain <script> tag) - put directly in global namespace
        factory(window['jQuery']);
    }
}(function ($) {
    function XmsFilter(filter) {
        filter = filter || {};
        this.FilterOperator = filter.operator || 0;
        this.Conditions = filter.Conditions || [];
        this.Filters = filter.Filters || [];
    }
    XmsFilter.prototype.setOperator = function (operator) {
        this.FilterOperator = operator;
    }
    XmsFilter.prototype.addFilter = function (filter, repeat) {
        if (!repeat) {
            this.Filters.push(filter);
        }
    }
    //只能检查第一层，没做递归检测
    XmsFilter.prototype._checkByConditions = function (objdata) {
        var self = this, flag = false;
        if (this.FilterOperator == 0) {//如果是并
            flag = true;
            if (this.Conditions.length > 0) {
                $.each(this.Conditions, function (i, n) {
                    var operator = n.operator;//操作符
                    var operatorname = Xms.Fetch.getFilterName(operator);//获取操作符名字
                    var operatorFunc = Xms.ExtFilter.FilterHandler[operatorname];//获取操作符对比函数;

                    if (typeof operatorFunc === 'function') {
                        var res = operatorFunc(n.values, objdata[n.attributename]);
                        if (!res) {
                            flag = false;
                        }
                    }
                });
            }
        } else {//如果是或
            if (this.Conditions.length > 0) {
                $.each(this.Conditions, function (i, n) {
                    var operator = n.operator;//操作符
                    var operatorname = Xms.Fetch.getFilterName(operator);//获取操作符名字
                    var operatorFunc = Xms.ExtFilter.FilterHandler[operatorname];//获取操作符对比函数;

                    if (typeof operatorFunc === 'function') {
                        var res = operatorFunc(n.values, objdata[n.attributename]);
                        if (res) {
                            flag = true;
                            return false;
                        }
                    }
                });
            }
        }
        return flag;
    }
    XmsFilter.prototype.filterByData = function (objdata) {
        if (typeof Xms != 'undefined' && Xms.Fetch) {
            return this._checkByConditions(objdata);
        } else {
            console.error('请加载fetch.js');
        }
        return false;
    }
    XmsFilter.prototype.addCondition = function (condition, cover) {
        if (!cover) {
            this.Conditions.push(condition);
        } else {
            var index = this.indexOfCondition(condition.AttributeName)
            if (~index) {
                this.Conditions[index] = condition;
            } else {
                this.Conditions.push(condition);
            }
        }
    }

    XmsFilter.prototype.indexOfCondition = function (AttributeName) {
        var index = -1;
        $.each(this.Conditions, function (i, n) {
            if (n.AttributeName == AttributeName) {
                index = i;
                return false;
            }
        });
        return index;
    }

    XmsFilter.prototype.findCondition = function (AttributeName) {
        var index = [];
        $.each(this.Conditions, function (i, n) {
            if (n.AttributeName == AttributeName) {
                index.push(i);
                return true;
            }
        });
        return index;
    }
    //只删除当前FILER里的CONDITION
    XmsFilter.prototype.removeCondition = function (AttributeName) {
        var index = this.findCondition(AttributeName)
        var self = this;
        if (index && index.length > 0) {
            self.Conditions = $.grep(self.Conditions, function (i, n) {
                var flag = true;
                $.each(index, function (key, item) {
                    if (item == n) {
                        flag = false;
                        return false;
                    }
                });
                return flag
            });
        }
    }
    //递归删除所有的CONDITION
    XmsFilter.prototype.removeAllCondition = function (AttributeName) {
        var self = this;
        var removeFiltersIndex = [];
        $.each(this.Filters, function (i, n) {
            if (n.Conditions.length > 0) {
                n.removeCondition(AttributeName);
            }
            if (n.Filters.length > 0) {
                var __condition = n.removeAllCondition(AttributeName);
            }
            if (n.Filters.length == 0 && n.Conditions.length == 0) {
                removeFiltersIndex.push(i);
            }
        });
        self.Filters = $.grep(self.Filters, function (i, n) {
            var flag = true;
            $.each(removeFiltersIndex, function (key, item) {
                if (item == n) {
                    flag = false;
                    return false;
                }
            });
            return flag
        });
        console.log(self.Filters);
    }

    //获取当前Filter的对应得condition
    XmsFilter.prototype.getCondition = function (AttributeName) {
        var conditions = [];
        $.each(this.Conditions, function (i, n) {
            if (n.AttributeName == AttributeName) {
                conditions.push(n);
            }
        });
        return conditions;
    }
    //查找出所有的condition
    XmsFilter.prototype.getAllCondition = function (AttributeName) {
        var conditions = [];
        $.each(this.Filters, function (i, n) {
            var _condition = n.getCondition(AttributeName);
            if (_condition.length > 0) {
                conditions = conditions.concat(_condition);
            }
            if (n.Filters.length > 0) {
                var __condition = n.getAllCondition(AttributeName);
                if (__condition.length > 0) {
                    conditions = conditions.concat(__condition);
                }
            }
        });
        return conditions;
    }
    XmsFilter.prototype.getFilterInfo = function () {
        var res = {};
        res.FilterOperator = this.FilterOperator || 0;
        res.Conditions = this.Conditions;
        res.Filters = [];
        $.each(this.Filters, function (i, n) {
            res.Filters.push(n.getFilterInfo());
        });
        return res;
    }
    XmsFilter.prototype.clearAll = function () {
        this.FilterOperator = 0;
        this.Conditions = [];
        this.Filters = [];
    }
    function XmsCondition(AttributeName, Operator, Values) {
        this.AttributeName = AttributeName || '';
        this.Operator = Operator || 0;
        this.Values = Values || [];
    }

    XmsFilter.changeFiltersToXmsFilter = function (filters) {
        var xmsfilter = new XmsFilter();
        if (filters.Conditions && filters.Conditions.length > 0) {
            xmsfilter.Conditions = filters.Conditions;
        }
        xmsfilter.FilterOperator = filters.FilterOperator;
        if (filters.Filters && filters.Filters.length > 0) {
            $.each(filters.Filters, function (i, n) {
                xmsfilter.Filters.push(XmsFilter.changeFiltersToXmsFilter(n));
            });
        }
        return xmsfilter;
    }
    window.XmsFilter = XmsFilter;
    window.XmsCondition = XmsCondition;
    return {
        XmsFilter: XmsFilter,
        XmsCondition: XmsCondition
    }
});

(function () {
    if (typeof (Xms) == "undefined") { Xms = { __namespace: true }; }
    Xms.ExtFilter = function () { };

    Xms.ExtFilter.FilterHandlerLabel = {}
    //val1是过滤条件里的值，val2是传进来需要对比的值
    Xms.ExtFilter.FilterHandler = {
        Equal: function (val1, val2) {
            return val1[0] == val2;
        },
        NotEqual: function (val1, val2) {
            return val1[0] != val2;
        },
        GreaterThan: function (val1, val2) {
            return val1[0] < val2;
        },
        LessThan: function (val1, val2) {
            return val1[0] > val2;
        },
        GreaterEqual: function (val1, val2) {
            return val1[0] <= val2;
        },
        LessEqual: function (val1, val2) {
            return val1[0] >= val2;
        },
        Like: function (val1, val2) {
            return val2.indexOf(val1[0]) != -1;
        },
        NotLike: function (val1, val2) {
            return val2.indexOf(val1[0]) == -1;
        },
        In: function (val1, val2) {
            return val2.indexOf(val1[0]) != -1;
        },
        NotIn: function (val1, val2) {
            return val2.indexOf(val1[0]) == -1;
        },
        Between: function (val1, val2) {
            return val2 < val1[1] && val2 > val1[0];
        },
        NotBetween: function (val1, val2) {
            return val2 > val1[1] || val2 < val1[0];
        },
        Null: function (val1, val2) {
            return val2.indexOf(val1[0]) == -1;
        },
        NotNull: function (val1, val2) {
            return val2.indexOf(val1[0]) != -1;
        },
        Yesterday: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('d', -1)
                return val2time.DateDiff('d', checkday) == 0;
            }
        },
        Today: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date()
                return val2time.DateDiff('d', checkday) == 0;
            }
        },
        Tomorrow: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('d', 1)
                return val2time.DateDiff('d', checkday) == 0;
            }
        },
        Last7Days: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('d', -7)
                return val2time.DateDiff('d', checkday) < 0;
            }
        },
        Next7Days: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('d', 7)
                return val2time.DateDiff('d', checkday) > 0;
            }
        },
        LastWeek: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('w', -1)
                return val2time.DateDiff('w', checkday) < 0;
            }
        },
        ThisWeek: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('w', 0)
                return val2time.DateDiff('w', checkday) == 0;
            }
        },
        NextWeek: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('w', 1)
                return val2time.DateDiff('w', checkday) > 0;
            }
        },
        LastMonth: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('m', -1)
                return val2time.DateDiff('m', checkday) < 0;
            }
        },
        ThisMonth: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('m', 0)
                return val2time.DateDiff('m', checkday) == 0;
            }
        },
        NextMonth: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('m', 1)
                return val2time.DateDiff('m', checkday) > 0;
            }
        },
        On: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date(val1).DateAdd('d', 0)
                return val2time.DateDiff('d', checkday) == 0;
            }
        },
        OnOrBefore: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date(val1).DateAdd('d', 0)
                return val2time.DateDiff('d', checkday) <= 0;
            }
        },
        OnOrAfter: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date(val1).DateAdd('d', 0)
                return val2time.DateDiff('d', checkday) >= 0;
            }
        },
        Before: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date(val1).DateAdd('d', 0)
                return val2time.DateDiff('d', checkday) < 0;
            }
        },
        After: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date(val1).DateAdd('d', 0)
                return val2time.DateDiff('d', checkday) > 0;
            }
        },
        LastYear: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('y', 0)
                return val2time.DateDiff('y', checkday) < 0;
            }
        },
        ThisYear: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('y', 0)
                return val2time.DateDiff('y', checkday) == 0;
            }
        },
        NextYear: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('y', 0)
                return val2time.DateDiff('y', checkday) > 0;
            }
        },
        LastXHours: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('h', 0)
                return val2time.DateDiff('h', checkday) < 0;
            }
        },
        NextXHours: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('h', 0)
                return val2time.DateDiff('h', checkday) > 0;
            }
        },
        LastXDays: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('d', 0)
                return val2time.DateDiff('d', checkday) < 0;
            }
        },
        NextXDays: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('d', 0)
                return val2time.DateDiff('d', checkday) > 0;
            }
        },
        LastXWeeks: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('w', 0)
                return val2time.DateDiff('w', checkday) < 0;
            }
        },
        NextXWeeks: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('w', 0)
                return val2time.DateDiff('w', checkday) > 0;
            }
        },
        LastXMonths: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('m', 0)
                return val2time.DateDiff('m', checkday) < 0;
            }
        },
        NextXMonths: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('m', 0)
                return val2time.DateDiff('m', checkday) > 0;
            }
        },
        LastXYears: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('y', 0)
                return val2time.DateDiff('y', checkday) < 0;
            }
        },
        NextXYears: function (val1, val2) {
            if (val2 && val2 != "") {
                var val2time = new Date(val2);
                var checkday = new Date().DateAdd('y', 0)
                return val2time.DateDiff('y', checkday) > 0;
            }
        },
        EqualUserId: function (val1, val2) {
            if (typeof CURRENT_USER != 'undefined') {
                if (val2 && val2 != "") {
                    return CURRENT_USER.systemuserid == val2;
                }
            }
        },
        NotEqualUserId: function (val1, val2) {
            if (typeof CURRENT_USER != 'undefined') {
                if (val2 && val2 != "") {
                    return CURRENT_USER.systemuserid != val2;
                }
            }
        },
        EqualBusinessId: function (val1, val2) {
            if (typeof CURRENT_USER != 'undefined') {
                if (val2 && val2 != "") {
                    return CURRENT_USER.businessunitid == val2;
                }
            }
        },
        NotEqualBusinessId: function (val1, val2) {
            if (typeof CURRENT_USER != 'undefined') {
                if (val2 && val2 != "") {
                    return CURRENT_USER.businessunitid != val2;
                }
            }
        },
        ChildOf: 47,
        Mask: 48,
        NotMask: 49,
        MasksSelect: 50,
        Contains: 51,
        DoesNotContain: 52,
        EqualUserLanguage: 53,
        NotOn: 54,
        OlderThanXMonths: 55,
        BeginsWith: 56,
        DoesNotBeginWith: 57,
        EndsWith: 58,
        DoesNotEndWith: 59,
        ThisFiscalYear: 60,
        ThisFiscalPeriod: 61,
        NextFiscalYear: 62,
        NextFiscalPeriod: 63,
        LastFiscalYear: 64,
        LastFiscalPeriod: 65,
        LastXFiscalYears: 66,
        LastXFiscalPeriods: 67,
        NextXFiscalYears: 68,
        NextXFiscalPeriods: 69,
        InFiscalYear: 70,
        InFiscalPeriod: 71,
        InFiscalPeriodAndYear: 72,
        InOrBeforeFiscalPeriodAndYear: 73,
        InOrAfterFiscalPeriodAndYear: 74,
        EqualUserTeams: 75,
        EqualOrganizationId: 76,
        NotEqualOrganizationId: 77,
        OnOrBeforeToday: 78,
        OnOrAfterToday: 79,
        BeforeToday: 80,
        AfterToday: 81,
        OlderThanXYears: 82,
        OlderThanXDays: 83,
        AfterXYears: 84,
        AfterXMonths: 85,
        AfterXDays: 86
    };
    //models

    ////类型比较符
    //Xms.ExtFilter.FilterHandler.CommonOperators = [];
    //Xms.ExtFilter.FilterHandler.CommonOperators.push(["Equal", Xms.ExtFilter.FilterHandler.Equal, Xms.ExtFilter.FilterHandlerLabel.Equal]);
    //Xms.ExtFilter.FilterHandler.CommonOperators.push(["NotEqual", Xms.ExtFilter.FilterHandler.NotEqual, Xms.ExtFilter.FilterHandlerLabel.NotEqual]);
    //Xms.ExtFilter.FilterHandler.CommonOperators.push(["NotNull", Xms.ExtFilter.FilterHandler.NotNull, Xms.ExtFilter.FilterHandlerLabel.NotNull]);
    //Xms.ExtFilter.FilterHandler.CommonOperators.push(["Null", Xms.ExtFilter.FilterHandler.Null, Xms.ExtFilter.FilterHandlerLabel.Null]);

    //Xms.ExtFilter.FilterHandler.StringOperators = Xms.ExtFilter.FilterHandler.CommonOperators.concat();
    //Xms.ExtFilter.FilterHandler.StringOperators.push(["Like", Xms.ExtFilter.FilterHandler.Like, Xms.ExtFilter.FilterHandlerLabel.Like]);
    //Xms.ExtFilter.FilterHandler.StringOperators.push(["NotLike", Xms.ExtFilter.FilterHandler.NotLike, Xms.ExtFilter.FilterHandlerLabel.NotLike]);
    //Xms.ExtFilter.FilterHandler.StringOperators.push(["BeginsWith", Xms.ExtFilter.FilterHandler.BeginsWith, Xms.ExtFilter.FilterHandlerLabel.BeginsWith]);
    //Xms.ExtFilter.FilterHandler.StringOperators.push(["DoesNotBeginWith", Xms.ExtFilter.FilterHandler.DoesNotBeginWith, Xms.ExtFilter.FilterHandlerLabel.DoesNotBeginWith]);
    //Xms.ExtFilter.FilterHandler.StringOperators.push(["EndsWith", Xms.ExtFilter.FilterHandler.EndsWith, Xms.ExtFilter.FilterHandlerLabel.EndsWith]);
    //Xms.ExtFilter.FilterHandler.StringOperators.push(["DoesNotEndWith", Xms.ExtFilter.FilterHandler.DoesNotEndWith, Xms.ExtFilter.FilterHandlerLabel.DoesNotEndWith]);

    //Xms.ExtFilter.FilterHandler.NumberOperators = Xms.ExtFilter.FilterHandler.CommonOperators.concat();
    //Xms.ExtFilter.FilterHandler.NumberOperators.push(["GreaterThan", Xms.ExtFilter.FilterHandler.GreaterThan, Xms.ExtFilter.FilterHandlerLabel.GreaterThan]);
    //Xms.ExtFilter.FilterHandler.NumberOperators.push(["LessThan", Xms.ExtFilter.FilterHandler.LessThan, Xms.ExtFilter.FilterHandlerLabel.LessThan]);
    //Xms.ExtFilter.FilterHandler.NumberOperators.push(["GreaterEqual", Xms.ExtFilter.FilterHandler.GreaterEqual, Xms.ExtFilter.FilterHandlerLabel.GreaterEqual]);
    //Xms.ExtFilter.FilterHandler.NumberOperators.push(["LessEqual", Xms.ExtFilter.FilterHandler.LessEqual, Xms.ExtFilter.FilterHandlerLabel.LessEqual]);

    //Xms.ExtFilter.FilterHandler.DateTimeOperators = Xms.ExtFilter.FilterHandler.CommonOperators.concat();
    ////Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["GreaterThan", Xms.ExtFilter.FilterHandler.GreaterThan, Xms.ExtFilter.FilterHandlerLabel.GreaterThan]);
    ////Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["LessThan", Xms.ExtFilter.FilterHandler.LessThan, Xms.ExtFilter.FilterHandlerLabel.LessThan]);
    ////Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["GreaterEqual", Xms.ExtFilter.FilterHandler.GreaterEqual, Xms.ExtFilter.FilterHandlerLabel.GreaterEqual]);
    ////Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["LessEqual", Xms.ExtFilter.FilterHandler.LessEqual, Xms.ExtFilter.FilterHandlerLabel.LessEqual]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["On", Xms.ExtFilter.FilterHandler.On, Xms.ExtFilter.FilterHandlerLabel.On]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["Last7Days", Xms.ExtFilter.FilterHandler.Last7Days, Xms.ExtFilter.FilterHandlerLabel.Last7Days]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["LastWeek", Xms.ExtFilter.FilterHandler.LastWeek, Xms.ExtFilter.FilterHandlerLabel.LastWeek]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["LastMonth", Xms.ExtFilter.FilterHandler.LastMonth, Xms.ExtFilter.FilterHandlerLabel.LastMonth]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["LastYear", Xms.ExtFilter.FilterHandler.LastYear, Xms.ExtFilter.FilterHandlerLabel.LastYear]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["LastXHours", Xms.ExtFilter.FilterHandler.LastXHours, Xms.ExtFilter.FilterHandlerLabel.LastXHours]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["LastXDays", Xms.ExtFilter.FilterHandler.LastXDays, Xms.ExtFilter.FilterHandlerLabel.LastXDays]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["LastXWeeks", Xms.ExtFilter.FilterHandler.LastXWeeks, Xms.ExtFilter.FilterHandlerLabel.LastXWeeks]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["LastXMonths", Xms.ExtFilter.FilterHandler.LastXMonths, Xms.ExtFilter.FilterHandlerLabel.LastXMonths]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["LastXYears", Xms.ExtFilter.FilterHandler.LastXYears, Xms.ExtFilter.FilterHandlerLabel.LastXYears]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["Next7Days", Xms.ExtFilter.FilterHandler.Next7Days, Xms.ExtFilter.FilterHandlerLabel.Next7Days]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["NextWeek", Xms.ExtFilter.FilterHandler.NextWeek, Xms.ExtFilter.FilterHandlerLabel.NextWeek]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["NextMonth", Xms.ExtFilter.FilterHandler.NextMonth, Xms.ExtFilter.FilterHandlerLabel.NextMonth]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["NextYear", Xms.ExtFilter.FilterHandler.NextYear, Xms.ExtFilter.FilterHandlerLabel.NextYear]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["NextXHours", Xms.ExtFilter.FilterHandler.NextXHours, Xms.ExtFilter.FilterHandlerLabel.NextXHours]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["NextXDays", Xms.ExtFilter.FilterHandler.NextXDays, Xms.ExtFilter.FilterHandlerLabel.NextXDays]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["NextXWeeks", Xms.ExtFilter.FilterHandler.NextXWeeks, Xms.ExtFilter.FilterHandlerLabel.NextXWeeks]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["NextXMonths", Xms.ExtFilter.FilterHandler.NextXMonths, Xms.ExtFilter.FilterHandlerLabel.NextXMonths]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["NextXYears", Xms.ExtFilter.FilterHandler.NextXYears, Xms.ExtFilter.FilterHandlerLabel.NextXYears]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["OlderThanXYears", Xms.ExtFilter.FilterHandler.OlderThanXYears, Xms.ExtFilter.FilterHandlerLabel.OlderThanXYears]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["OlderThanXMonths", Xms.ExtFilter.FilterHandler.OlderThanXMonths, Xms.ExtFilter.FilterHandlerLabel.OlderThanXMonths]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["OlderThanXDays", Xms.ExtFilter.FilterHandler.OlderThanXDays, Xms.ExtFilter.FilterHandlerLabel.OlderThanXDays]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["AfterXYears", Xms.ExtFilter.FilterHandler.AfterXYears, Xms.ExtFilter.FilterHandlerLabel.AfterXYears]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["AfterXMonths", Xms.ExtFilter.FilterHandler.AfterXMonths, Xms.ExtFilter.FilterHandlerLabel.AfterXMonths]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["AfterXDays", Xms.ExtFilter.FilterHandler.AfterXDays, Xms.ExtFilter.FilterHandlerLabel.AfterXDays]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["OnOrBefore", Xms.ExtFilter.FilterHandler.OnOrBefore, Xms.ExtFilter.FilterHandlerLabel.OnOrBefore]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["OnOrAfter", Xms.ExtFilter.FilterHandler.OnOrAfter, Xms.ExtFilter.FilterHandlerLabel.OnOrAfter]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["Before", Xms.ExtFilter.FilterHandler.Before, Xms.ExtFilter.FilterHandlerLabel.Before]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["After", Xms.ExtFilter.FilterHandler.After, Xms.ExtFilter.FilterHandlerLabel.After]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["ThisWeek", Xms.ExtFilter.FilterHandler.ThisWeek, Xms.ExtFilter.FilterHandlerLabel.ThisWeek]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["ThisMonth", Xms.ExtFilter.FilterHandler.ThisMonth, Xms.ExtFilter.FilterHandlerLabel.ThisMonth]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["ThisYear", Xms.ExtFilter.FilterHandler.ThisYear, Xms.ExtFilter.FilterHandlerLabel.ThisYear]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["Today", Xms.ExtFilter.FilterHandler.Today, Xms.ExtFilter.FilterHandlerLabel.Today]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["Tomorrow", Xms.ExtFilter.FilterHandler.Tomorrow, Xms.ExtFilter.FilterHandlerLabel.Tomorrow]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["Yesterday", Xms.ExtFilter.FilterHandler.Yesterday, Xms.ExtFilter.FilterHandlerLabel.Yesterday]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["OnOrBeforeToday", Xms.ExtFilter.FilterHandler.OnOrBeforeToday, Xms.ExtFilter.FilterHandlerLabel.OnOrBeforeToday]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["OnOrAfterToday", Xms.ExtFilter.FilterHandler.OnOrAfterToday, Xms.ExtFilter.FilterHandlerLabel.OnOrAfterToday]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["BeforeToday", Xms.ExtFilter.FilterHandler.BeforeToday, Xms.ExtFilter.FilterHandlerLabel.BeforeToday]);
    //Xms.ExtFilter.FilterHandler.DateTimeOperators.push(["AfterToday", Xms.ExtFilter.FilterHandler.AfterToday, Xms.ExtFilter.FilterHandlerLabel.AfterToday]);

    //Xms.ExtFilter.FilterHandler.LookUpOperators = Xms.ExtFilter.FilterHandler.CommonOperators.concat();
    //Xms.ExtFilter.FilterHandler.LookUpOperators.push(["Like", Xms.ExtFilter.FilterHandler.Like, Xms.ExtFilter.FilterHandlerLabel.Like]);
    //Xms.ExtFilter.FilterHandler.LookUpOperators.push(["NotLike", Xms.ExtFilter.FilterHandler.NotLike, Xms.ExtFilter.FilterHandlerLabel.NotLike]);
    //Xms.ExtFilter.FilterHandler.LookUpOperators.push(["BeginsWith", Xms.ExtFilter.FilterHandler.BeginsWith, Xms.ExtFilter.FilterHandlerLabel.BeginsWith]);
    //Xms.ExtFilter.FilterHandler.LookUpOperators.push(["DoesNotBeginWith", Xms.ExtFilter.FilterHandler.DoesNotBeginWith, Xms.ExtFilter.FilterHandlerLabel.DoesNotBeginWith]);
    //Xms.ExtFilter.FilterHandler.LookUpOperators.push(["EndsWith", Xms.ExtFilter.FilterHandler.EndsWith, Xms.ExtFilter.FilterHandlerLabel.EndsWith]);
    //Xms.ExtFilter.FilterHandler.LookUpOperators.push(["DoesNotEndWith", Xms.ExtFilter.FilterHandler.DoesNotEndWith, Xms.ExtFilter.FilterHandlerLabel.DoesNotEndWith]);

    //Xms.ExtFilter.FilterHandler.OwnerOperators = Xms.ExtFilter.FilterHandler.CommonOperators.concat();
    //Xms.ExtFilter.FilterHandler.OwnerOperators.push(["Like", Xms.ExtFilter.FilterHandler.Like, Xms.ExtFilter.FilterHandlerLabel.Like]);
    //Xms.ExtFilter.FilterHandler.OwnerOperators.push(["NotLike", Xms.ExtFilter.FilterHandler.NotLike, Xms.ExtFilter.FilterHandlerLabel.NotLike]);
    //Xms.ExtFilter.FilterHandler.OwnerOperators.push(["BeginsWith", Xms.ExtFilter.FilterHandler.BeginsWith, Xms.ExtFilter.FilterHandlerLabel.BeginsWith]);
    //Xms.ExtFilter.FilterHandler.OwnerOperators.push(["DoesNotBeginWith", Xms.ExtFilter.FilterHandler.DoesNotBeginWith, Xms.ExtFilter.FilterHandlerLabel.DoesNotBeginWith]);
    //Xms.ExtFilter.FilterHandler.OwnerOperators.push(["EndsWith", Xms.ExtFilter.FilterHandler.EndsWith, Xms.ExtFilter.FilterHandlerLabel.EndsWith]);
    //Xms.ExtFilter.FilterHandler.OwnerOperators.push(["DoesNotEndWith", Xms.ExtFilter.FilterHandler.DoesNotEndWith, Xms.ExtFilter.FilterHandlerLabel.DoesNotEndWith]);
    //Xms.ExtFilter.FilterHandler.OwnerOperators.push(["EqualUserId", Xms.ExtFilter.FilterHandler.EqualUserId, Xms.ExtFilter.FilterHandlerLabel.EqualUserId]);
    //Xms.ExtFilter.FilterHandler.OwnerOperators.push(["NotEqualUserId", Xms.ExtFilter.FilterHandler.NotEqualUserId, Xms.ExtFilter.FilterHandlerLabel.NotEqualUserId]);

    //Xms.ExtFilter.FilterHandler.SystemUserOperators = Xms.ExtFilter.FilterHandler.OwnerOperators.concat();

    //Xms.ExtFilter.FilterHandler.BusinessUnitOperators = Xms.ExtFilter.FilterHandler.LookUpOperators.concat();

    //Xms.ExtFilter.FilterHandler.BusinessUnitOperators.push(["EqualBusinessId", Xms.ExtFilter.FilterHandler.EqualBusinessId, Xms.ExtFilter.FilterHandlerLabel.EqualBusinessId]);
    //Xms.ExtFilter.FilterHandler.BusinessUnitOperators.push(["NotEqualBusinessId", Xms.ExtFilter.FilterHandler.NotEqualBusinessId, Xms.ExtFilter.FilterHandlerLabel.NotEqualBusinessId]);

    //Xms.ExtFilter.FilterHandler.OrganizationOperators = Xms.ExtFilter.FilterHandler.LookUpOperators.concat();
    //Xms.ExtFilter.FilterHandler.OrganizationOperators.push(["EqualOrganizationId", Xms.ExtFilter.FilterHandler.EqualOrganizationId, Xms.ExtFilter.FilterHandlerLabel.EqualOrganizationId]);
    //Xms.ExtFilter.FilterHandler.OrganizationOperators.push(["NotEqualOrganizationId", Xms.ExtFilter.FilterHandler.NotEqualOrganizationId, Xms.ExtFilter.FilterHandlerLabel.NotEqualOrganizationId]);

    //Xms.ExtFilter.FilterHandler.PickListOperators = Xms.ExtFilter.FilterHandler.CommonOperators.concat();
    //Xms.ExtFilter.FilterHandler.PickListOperators.push(["In", Xms.ExtFilter.FilterHandler.In, Xms.ExtFilter.FilterHandlerLabel.In]);
    //Xms.ExtFilter.FilterHandler.PickListOperators.push(["NotIn", Xms.ExtFilter.FilterHandler.NotIn, Xms.ExtFilter.FilterHandlerLabel.NotIn]);

    //Xms.ExtFilter.FilterHandlers = [];
    //Xms.ExtFilter.FilterHandlers["nvarchar"] = Xms.ExtFilter.FilterHandler.StringOperators.concat();
    //Xms.ExtFilter.FilterHandlers["datetime"] = Xms.ExtFilter.FilterHandler.DateTimeOperators.concat();
    //Xms.ExtFilter.FilterHandlers["lookup"] = Xms.ExtFilter.FilterHandler.LookUpOperators.concat();
    //Xms.ExtFilter.FilterHandlers["owner"] = Xms.ExtFilter.FilterHandler.OwnerOperators.concat();
    //Xms.ExtFilter.FilterHandlers["picklist"] = Xms.ExtFilter.FilterHandler.PickListOperators.concat();
    //Xms.ExtFilter.FilterHandlers["bit"] = Xms.ExtFilter.FilterHandler.PickListOperators.concat();
    //Xms.ExtFilter.FilterHandlers["int"] = Xms.ExtFilter.FilterHandler.NumberOperators.concat();
    //Xms.ExtFilter.FilterHandlers["money"] = Xms.ExtFilter.FilterHandler.NumberOperators.concat();
    //Xms.ExtFilter.FilterHandlers["float"] = Xms.ExtFilter.FilterHandler.NumberOperators.concat();
    //Xms.ExtFilter.FilterHandlers["decimal"] = Xms.ExtFilter.FilterHandler.NumberOperators.concat();
    //Xms.ExtFilter.FilterHandlers["state"] = Xms.ExtFilter.FilterHandler.PickListOperators.concat();
    //Xms.ExtFilter.FilterHandlers["businessunit"] = Xms.ExtFilter.FilterHandler.OwnerOperators.concat();
    //Xms.ExtFilter.FilterHandlers["systemuser"] = Xms.ExtFilter.FilterHandler.SystemUserOperators.concat();
    //Xms.ExtFilter.FilterHandlers["organization"] = Xms.ExtFilter.FilterHandler.OrganizationOperators.concat();
})();