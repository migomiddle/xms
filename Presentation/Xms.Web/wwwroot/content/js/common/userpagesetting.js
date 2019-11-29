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
    var Gmethods = new function () {
        var self = this;
        this.usersettings = new UserSettings();
        this.ownerid = CURRENT_USER.systemuserid;
        this.localstorageName = 'xms_userinfo_' + this.ownerid;
        this.infos = [];
        this.addToLocalStorage = function () {
            value = JSON.stringify(this.usersettings.getDatas());
            localStorage.setItem(this.localstorageName, value);
        }
        this.getByLocalStorage = function () {
            var value = localStorage.getItem(this.localstorageName);
            if (value && value != '') {
                var datas = JSON.parse(value);
                if (datas && datas.length > 0) {
                    $.each(datas, function (i, n) {
                        self.addSetting(n.key, n.value, n.id);
                    });
                }
                return self.getDatas();
            }
        }

        this.getDatas = function () {
            return this.usersettings.getDatas();
        }
        this.getDatasBykey = function (key) {
            return this.usersettings.getDatasBykey(key);
        }
        this.batchInList = function (list) {
            var self = this;
            $.each(list, function (i, n) {
                if (n) {
                    n.key = n.name;
                    n.value = n.value;
                    self.usersettings.add(n);
                }
            });
        }
        this.changeToPostData = function () {
            var list = self.usersettings.getDatas();
            $.each(list, function (i, n) {
                n.ownerid = self.ownerid;
                n.value = encodeURIComponent(JSON.stringify(n.value));
                n.name = n.key;
            });
            return list;
        }
        this.saveByRemote = function () {
            var datas = this.changeToPostData();
            console.log(datas);
            $.each(datas, function (i, n) {
                Xms.Web.Post(ORG_SERVERURL + '/api/userpersonalization/set', n, false, function (res) {
                    console.log(res);
                }, null, false, false, null);
            });
        }
        this.addSetting = function (key, value, id, iscover) {
            self.usersettings.add({ id: id, key: key, value: value }, iscover);
            return this;
        }
        this.removeSettingByKey = function (key) {
            self.usersettings.remove(key);
            return this;
        }
        this.getSettings = function () {
            return self.usersettings.ToString();
        }
    }

    function UserSettings() {
        this.list = [];
        this.pageinfo = {};
        this.dirthChecker = new xmsDirtyChecker();
    }
    UserSettings.prototype.getDatas = function () {
        return this.list;
    }
    UserSettings.prototype.getDatasBykey = function (key) {
        return $.queryBykeyValue(this.list, 'key', key);
    }
    UserSettings.prototype.add = function (setting, isCover) {
        var index = $.indexBykeyValue(this.list, 'key', setting.key);
        if (index == -1) {
            var _setting = new UserSetting(setting);
            this.list.push(_setting);
            this.dirthChecker.addWatch(_setting.key, _setting.value);
        } else if (isCover) {
            this.list[index] = new UserSetting(setting);
            this.dirthChecker.addWatch(this.list[index].key, this.list[index].value);
        }
    }
    UserSettings.prototype.remove = function (key) {
        var index = $.indexBykeyValue(this.list, 'key', key);
        if (index != -1) {
            this.splice(index, 1);
        }
    }
    UserSettings.prototype.ToString = function () {
        return JSON.stringify(this.list);
    }
    function UserSetting(setting) {
        this.id = setting.id || Xms.Utility.Guid.NewGuid().ToString();
        this.key = setting.key;
        this.value = setting.value;
        this.list = [];
        this.isInited = false;
    }
    UserSetting.prototype.init = function () {
        var self = this;
        if (this.value != '') {
            try {
                var decoddata = decodeURIComponent(this.value);
                var objdata = JSON.parse(decoddata);
                if (objdata && objdata.length > 0) {
                    $.each(objdata, function () {
                        self.list.push(this);
                    });
                    this.isInited = true;
                }
            } catch (e) {
                console.error(e);
            }
        }
    }
    UserSetting.prototype.getDataBykey = function (value, key) {
        var self = this;
        key = key || 'key';
        if (this.isInited) {
            return $.queryBykeyValue(this.list, key, value);
        }
        return []
    }
    UserSetting.prototype.getDatas = function () {
        return this.list
    }

    //配置页面具体位置的用户操作信息
    function pageSetting(name) {
        UserSettings.call(this, arguments);
        this.name = name;
        this.ischanged = false;
    }
    $.extend(pageSetting.prototype, UserSettings.prototype, {
        constructor: pageSetting,
        //把页面字符串信息转为这里的信息
        changeData: function (str) {
            var self = this;
            if (str != '') {
                try {
                    var decoddata = decodeURIComponent(str);
                    var objdata = JSON.parse(decoddata);
                    if (objdata && objdata.length > 0) {
                        $.each(objdata, function () {
                            self.list.push(this);
                        });
                        this.ischanged = true;
                    }
                } catch (e) {
                    console.error(e);
                }
            }
        }
    });

    //获取用户和页面的某一区域配置信息，_pageSettingName:当天页面的用户配置信息名字 ， funSettingName：页面某一块区域的配置信息名字
    function getUserAndPageSetting(_pageSettingName, funSettingName) {
        var userSettingName = _pageSettingName;
        var userSettingCtrl = XmsUserSetting;
        var pageSettingName = funSettingName;
        var pageSettingCtrl = new XmsPageSetting(pageSettingName);
        if (CURRENT_USER.userpersonalizations != '') {
            CURRENT_USER.userpersonalizations = CURRENT_USER.userpersonalizations.replace(/&quot;/g, "\"");
        }
        userSettingCtrl.batchInList(JSON.parse((CURRENT_USER.userpersonalizations)));
        var userSetting = {
            key: pageSettingName
        }
        var userSettingInfos = userSettingCtrl.getDatasBykey(userSettingName);
        if (userSettingInfos && userSettingInfos.length > 0) {
            pageSettingCtrl.changeData(userSettingInfos[0].value);
        }
        return {
            userSettingCtrl: userSettingCtrl,
            pageSettingCtrl: pageSettingCtrl
        }
    }
    window.getUserAndPageSetting = getUserAndPageSetting;
    window.XmsPageSetting = pageSetting;
    window.XmsUserSetting = Gmethods;
});