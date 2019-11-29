//Type.registerNamespace("Xms");
if (typeof (Xms) == "undefined") { Xms = { __namespace: true }; }
Xms.Utility = function () { };

//表示全局唯一标识符 (GUID)。
Xms.Utility.Guid = function (g) {
    var arr = new Array(); //存放32位数值的数组

    if (typeof (g) == "string") { //如果构造函数的参数为字符串
        InitByString(arr, g);
    }

    else {
        InitByOther(arr);
    }
    //返回一个值，该值指示 Guid 的两个实例是否表示同一个值。

    this.Equals = function (o) {
        if (o && o.IsGuid) {
            return this.ToString() == o.ToString();
        }

        else {
            return false;
        }
    }

    //Guid对象的标记

    this.IsGuid = function () { }

    //返回 Guid 类的此实例值的 String 表示形式。

    this.ToString = function (format) {
        if (typeof (format) == "string") {
            if (format == "N" || format == "D" || format == "B" || format == "P") {
                return ToStringWithFormat(arr, format);
            }

            else {
                return ToStringWithFormat(arr, "D");
            }
        }

        else {
            return ToStringWithFormat(arr, "D");
        }
    }

    //由字符串加载

    function InitByString(arr, g) {
        g = g.replace(/\{|\(|\)|\}|-/g, "");

        g = g.toLowerCase();

        if (g.length != 32 || g.search(/[^0-9,a-f]/i) != -1) {
            InitByOther(arr);
        }

        else {
            for (var i = 0; i < g.length; i++) {
                arr.push(g[i]);
            }
        }
    }

    //由其他类型加载

    function InitByOther(arr) {
        var i = 32;

        while (i--) {
            arr.push("0");
        }
    }

    /*

    根据所提供的格式说明符，返回此 Guid 实例值的 String 表示形式。

    N  32 位： xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx

    D  由连字符分隔的 32 位数字 xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx

    B  括在大括号中、由连字符分隔的 32 位数字：{xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}

    P  括在圆括号中、由连字符分隔的 32 位数字：(xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)

    */

    function ToStringWithFormat(arr, format) {
        switch (format) {
            case "N":

                return arr.toString().replace(/,/g, "");

            case "D":

                var str = arr.slice(0, 8) + "-" + arr.slice(8, 12) + "-" + arr.slice(12, 16) + "-" + arr.slice(16, 20) + "-" + arr.slice(20, 32);

                str = str.replace(/,/g, "");

                return str;

            case "B":

                var str = ToStringWithFormat(arr, "D");

                str = "{" + str + "}";

                return str;

            case "P":

                var str = ToStringWithFormat(arr, "D");

                str = "(" + str + ")";

                return str;

            default:

                return new Guid();
        }
    }
};
//Guid 类的默认实例，其值保证均为零。
Xms.Utility.Guid.EmptyGuid = new Xms.Utility.Guid();
//初始化 Guid 类的一个新实例。
Xms.Utility.Guid.NewGuid = function () {
    var g = "";
    var i = 32;
    while (i--) {
        g += Math.floor(Math.random() * 16.0).toString(16);
    }
    return new Xms.Utility.Guid(g);
}

Xms.Utility.GetFunction = function (code, argNames) {
    argNames = argNames || [];
    var fn = window, parts = (code || "").split(".");
    while (fn && parts.length) {
        fn = fn[parts.shift()];
    }
    if (typeof (fn) === "function") {
        return fn;
    }
    argNames.push(code);
    return Function.constructor.apply(null, argNames);
}
Xms.Utility.IsArray = function (value) {
    if (typeof Array.isArray === "function") {
        return Array.isArray(value);
    } else {
        return Object.prototype.toString.call(value) === "[object Array]";
    }
}
// 对Date的扩展，将 Date 转化为指定格式的String
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符，
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)
// 例子：
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18
Date.prototype.Format = function (fmt) {
    try {
        var o = {
            "M+": this.getMonth() + 1, //月份
            "d+": this.getDate(), //日
            "h+": this.getHours(), //小时
            "m+": this.getMinutes(), //分
            "s+": this.getSeconds(), //秒
            "q+": Math.floor((this.getMonth() + 3) / 3), //季度
            "S": this.getMilliseconds() //毫秒
        };
        if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o)
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
        return fmt;
    } catch (e) {
        return this;
    }
}