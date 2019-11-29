; (function ($, root, un) {
    "use strict"
    var hashchange = "hashchange", DOC = document, documentMode = DOC.documentMode;
    function rconsole(str) {
        console.log(str);
    }
    function getHash(url) {
        url = url || DOC.URL
        return '#' + url.replace(/^[^#]*#?(.*)$/, '$1');
    }

    function setHistory(url) {
        $.history.push(url);
    }

    function loadHistory(url) {
    }

    function hashToObj(str) {
    }

    function pathRegExp(path, opts) {
        var insensitive = opts && opts.caseInsensitiveMatch || false,
            ret = {
                originalPath: path,
                regexp: path
            },
            keys = ret.keys = [];

        path = path
            .replace(/([().])/g, '\\$1')
            .replace(/(\/)?:(\w+)([\?\*])?/g, function (_, slash, key, option) {
                var optional = option === '?' ? option : null;
                var star = option === '*' ? option : null;
                keys.push({ name: key, optional: !!optional });
                slash = slash || '';
                return ''
                    + (optional ? '' : slash)
                    + '(?:'
                    + (optional ? slash : '')
                    + (star && '(.+?)' || '([^/]+)')
                    + (optional || '')
                    + ')'
                    + (optional || '');
            })
            .replace(/([\/$\*])/g, '\\$1');

        ret.regexp = new RegExp('^' + path + '$', insensitive ? 'i' : '');
        return ret;
    }

    function controllerList() {
        var controllers = {}, self = this;
        this.add = function (name, func, data) {
            controllers[name] = func;
            controllers[name].data = data;
        }
        this.trigger = function (name, data) {
            controllers[name](data);
        }
        this.getList = function () {
            return controllers;
        }
    }

    function RouteProvider() {
        var routes = {};
        var controlList = new controllerList();
        this.caseInsensitiveMatch = false;
        this.when = function (path, route) {
            // console.log(route)
            var copy = $.extend({}, route);

            if (route && route.caseInsensitiveMatch) {
                route.caseInsensitiveMatch = this.caseInsensitiveMatch;
            }
            routes[path] = $.extend(copy, path && pathRegExp(path, route));
            var controlName = path;
            if (copy.controller) {
                controlList.add(controlName, route.controller);
            }
            if (path) {
                var redirectPath = (path[path.length - 1] == '/')
                    ? path.substr(0, path.length - 1)
                    : path + '/';

                routes[redirectPath] = $.extend(
                    { redirectTo: path },
                    pathRegExp(redirectPath, copy)
                );
            }
            //console.log(controlList.getList())
            return this;
        }
        this.otherwise = function (params) {
            if (typeof params === 'string') {
                params = { redirectTo: params };
            }
            this.when(null, params);
            return this;
        };
        this.getRoutes = function () {
            return routes;
        }
        this.getLength = function () {
            return routes.length;
        }

        this.run = function () {
            this.redirectUrl();
        }
        this.getUrl = function () {
            return location.href;
        }
        this.redirectUrl = function () {
            var isRedirect = false, rIndex = null;
            var url = location.href;
            var hash = getHash(url).replace("#", "");
            $.each(routes, function (key, item) {
                if (item && item.regexp) {
                    if (item.regexp.test(hash)) {
                        isRedirect = true;
                        rIndex = item;
                        return false;
                    }
                }
            });

            if (isRedirect && rIndex) {
                location.href = url.replace(hash, "") + hash;
                controlList.getList()[rIndex["originalPath"]] && controlList.trigger(rIndex["originalPath"], { reg: rIndex, hash: hash });
            } else {
                var redi = routes[null];
                if (rIndex && rIndex.originalPath) {
                    location.href = url.replace(hash, "") + redi.redirectTo;
                    controlList.getList()[rIndex["originalPath"]] && controlList.trigger(rIndex["originalPath"], {
                        reg: rIndex,
                        hash: hash
                    });
                }
            }
        }
    }

    $.moHistory = {};
    var his = new RouteProvider();
    $.moHistory.setConfig = function (func) {
        return func(his);
    }
    $.moHistory.run = function (func) {
        his.run();
    }
    $.moHistory.getHistory = function () {
        return his;
    }

    //test
    //var test = pathRegExp("test:id");
    // rconsole(test.regexp.test("test/1113"))
})(jQuery, window);