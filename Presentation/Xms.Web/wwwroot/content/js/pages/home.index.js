define(['jquery'], function ($) {
    "use strict"
    var iframeList = [];
    var pageWrap = {
        init: function () {
            pageWrap.setHistory();
            pageWrap.pageNavTree();
            pageWrap.sliderLeft(unbindSliderHover, bindSliderHover);
            pageWrap.dropDown();
            pageWrap.tooltip();
            pageWrap.subnavigater($(".xms-subn-items"), {
                showCount: 7,
                delItemFunc: function (obj) {
                    console.log(obj)
                    removeIframe($(obj).children('span'));
                }
            });
            pageWrap.realTimeNotice();
            //左侧导航伸缩
            $(window).resize(function () {
                winLH();
                setIframeHeight()
            })
            winLH();
            pageWrap.userTag();
            pageWrap.fullScreen();
            pageWrap.navSearch();
            pageWrap.ctrlClose();
            pageWrap.showAbout();
            pageWrap.changeTheme();
        },
        realTimeNotice: function () {
            //实时显示消息通知
            var notification = CURRENT_USER.enablednotification;
            if (notification) {
                var loops = Xms.Web.ToastNotice();
                loops.loopStart();
            }
        },
        sliderLeft: function (opened, closed) {
            $("#xms-sideBarbtn").click(function () {
                var $this = $(this), body = $(document.body), hasclass = body.hasClass("sideBar-open"), target = $this.attr("data-target");
                if (hasclass) {
                    body.addClass("sideBar-close").removeClass("sideBar-open");
                    $('.toggle-tree').children('li.dropdown').removeClass('active');
                    $("i", $this).addClass("layui-icon-spread-left").removeClass('layui-icon-shrink-right');
                    closed && closed($this);
                } else {
                    body.addClass("sideBar-open").removeClass("sideBar-close");
                    $("i", $this).addClass("layui-icon-shrink-right").removeClass('layui-icon-spread-left');
                    opened && opened($this);
                }
            })
        },
        dropDown: function () {
            $(".xms-dropdown").bind("click", function () {
                var $this = $(this), parent = $this.parent().parent(), isopen = parent.hasClass("xms-dropdown-open");
                if (isopen) {
                    parent.removeClass("xms-dropdown-open");
                } else {
                    parent.addClass("xms-dropdown-open");
                }
            });
        },
        createTooltip: function () {
            //添加右侧提示信息事件
            $(".xms-tooltip-ctrl").bind("click", function (e) {
                var winSize = {};
                winSize.height = $(window).height();
                winSize.width = $(window).width();

                var target = $(this);
                var dist = target.attr("data-target");
                dist = $(dist);
                var isShow = dist.is(":hidden");
                if (isShow) {
                    dist.show()
                } else {
                    dist.hide();
                }
                if (winSize.width < 768) {
                    return false;
                }
                var offset = target.offset(), targetL = offset.left, targetT = offset.top, targetH = target.outerHeight(), targetW = target.outerWidth();

                var scrollTop = $(window).scrollTop();
                var boxH = dist.height();
                var arrow = dist.children(".xms-tooltip-arrow");
                var distTop = (targetT + (targetH / 2)) - boxH / 2;
                dist.css("top", distTop);
                arrow.css("top", targetT + (targetH / 2) - distTop - 10);
                if (boxH > scrollTop + winSize.height) {
                    dist.css("top", dist.offset().top - (scrollTop + winSize.height - boxH));
                }
                dist.css("left", targetL + targetW - 14);

                if (e.preventDefault) {
                    e.preventDefault();
                } else {
                    return false;
                }
            });
            $(".xms-tooltip .xms-tooltip-close").bind("click", function () {
                $(this).parents(".xms-tooltip").hide();
            });
        },
        tooltip: function () {
            pageWrap.createTooltip(this);
        },
        subnavigater: function (ele, opts) {
            $(ele).swipeItems(opts);
        },
        setHistory: function () {//添加路由配置信息
            $.moHistory.setConfig(function (con) {
                //配置页面跳转
                con
                    .when("/mainIndex", {
                        controller: function (data) {
                            //  console.log(data);
                        }
                    })
                    .when("/redirectTo:lid", {
                        controller: function (data) {
                            console.log(data);
                            var reg = data.reg.regexp;
                            var hash = data.hash;
                            var lid = hash.split(":");
                            var objDom = $("a[data-id='" + lid[1] + "']");
                            var url = objDom.attr("data-href") || "";
                            if (url == "") { return false; }
                            createIframe("iframe" + lid[1], url);
                            objDom.attr("data-iframe", "iframe" + lid[1]);
                            objDom.attr("data-iframeid", lid[1]);
                            takeToBottom(objDom);
                            showIframe(objDom);
                        }
                    })
                    .otherwise("/mainIndex");
            });
        },
        pageNavTree: function () {//加载页面导航和事件绑定
            $('.view_left').on('mouseenter', '.dropdown-hover', function () {
                if ($('.toggle-tree').length > 0) return true;
                var winH = $(window).height();
                var $box = $(this).find('.dropdown-hover-level');
                var $menu = $(this).find('.dropdown-hover-level').children('ul');
                var _offsetTop = 8;
                var boxH = $(this).find('.dropdown-hover-level').height() + _offsetTop;
                var offH = $(this).offset().top;
                var $window = $(window);
                var winSize = { h: $window.height(), w: $window.width() };

                if ((boxH + offH + _offsetTop > winH) && (boxH < winH)) {
                    var nue = -(boxH + offH + _offsetTop - winH);
                    $box.css('top', nue);
                    $box.css('bottom', 'auto');
                } else if ((boxH + offH + _offsetTop > winH) && (boxH > winH)) {
                    $box.css('top', -offH - _offsetTop + 40);
                }
                if (boxH > winH) {
                    var scrolltip = $('<div class="scrolltips">向下滚动</div>');
                    $box.append(scrolltip);
                    scrolltip.css('top', winSize.h - 60);
                    var step = 10;
                    var ofsetH = boxH - winH - _offsetTop + 40;
                    $box.css('height', winSize.h - 40);
                    $menu.css({ 'height': winSize.h - 40, 'position': 'absolute', width: '100%' });
                    //console.log('leftnavscroll', winSize.h, offsetTop, menuHeight);
                    $menu.mousewheel(scrollMenu);
                }
            }
            )
            $('.xms-subn-items').on('click', 'a', function () {
                $(this).addClass('xms-subn-items-bgcol').siblings().removeClass('xms-subn-items-bgcol');
                $('#xms-subn-home').removeClass('xms-subn-items-bgcol');
                ctrlCloseBtnHandle();
                var _id = $(this).attr('data-iframeid');
                if ($('.toggle-tree').length > 0 && _id) {
                    $('.toggle-tree').find('.nav-side-alink').parent().removeClass('toggle-active');
                    var subnavActive = $('.toggle-tree').find('.nav-side-alink[data-id="' + _id + '"]');
                    subnavActive.parent().addClass('toggle-active');
                    if (subnavActive.parents('.dropdown-hover').is(':hidden')) {
                        subnavActive.parents('.dropdown-hover').addClass('active');
                        subnavActive.parents('.dropdown-hover:first').removeClass('active');
                    }
                }
            })
            $('#xms-subn-home').on('click', function () {
                $(this).addClass('xms-subn-items-bgcol');
                $('a', '.xms-subn-items').removeClass('xms-subn-items-bgcol');
                ctrlCloseBtnHandle();
            })
            $('#side-menu').mainNavRender({
                callback: function (obj) {
                    console.log($.moHistory.getHistory().getRoutes());
                    $.moHistory.run();
                    if ($("a.nav-side-alink", ".toggle-tree").length == 0) {
                        bindSideMenuScoll();
                    } else {
                        $("a.nav-side-alink", ".toggle-tree").parents('.sidebar:first').addClass('toggle-tree-wrap');
                        $("a.nav-side-alink", ".toggle-tree").off('click').on('click', function (e) {
                            e = e || window.event;
                            var self = $(this);
                            var data_href = self.attr('data-href');
                            if (self.siblings(".dropdown-menu:first").is(':hidden')) {
                                var $par = self.parent();
                                $par.siblings('li').removeClass('active');
                                self.parent().addClass('active');
                                if ($('.sideBar-close').length > 0) {
                                    $('body').removeClass('sideBar-close').addClass('sideBar-open')
                                    $('.left-menus-ctrl').removeClass('layui-icon-spread-left').addClass('layui-icon-shrink-right')
                                }
                            } else {
                                self.parent().removeClass('active');
                            }
                            if (data_href) {
                                takeToBottom(this);
                            }
                            e.stopPropagation && e.stopPropagation();
                        })
                    }
                }
            });
            var toggleTreeTop = 0, step = 5, _n = 3.5;
            $('.toggle-tree').on('mousewheel', function (e) {
                // console.log(e.deltaY);
                //  console.log($(this).offset().top)
                var offset = $(this).offset().top;
                var height = $(this).outerHeight();
                var _type = 1;
                if (e.deltaY > 0) {
                    _type = 1;
                } else {
                    _type = -1;
                }
                toggleTreeTop = toggleTreeTop + (step * _type) * _n;
                var wrapSize = $(window).height() - 50;
                if (wrapSize < height) {
                    if (toggleTreeTop < wrapSize - height) {
                        toggleTreeTop = wrapSize - height;
                    } else if (toggleTreeTop > 0) {
                        toggleTreeTop = 0;
                    }
                    $(this).css({ 'position': "relative", 'top': toggleTreeTop++ })
                } else {
                    toggleTreeTop = 0;
                    $(this).css({ 'position': "relative", 'top': toggleTreeTop })
                }
            })
        },
        userTag: function () {
            $(".main-user-tag").on('hide.bs.dropdown', function () {
                if ($(".main-user-tag").attr("close") == "true") {
                    $.cookie('user-tag', $("#main-user-tag-data").val());
                    $(".main-user-tag").attr("close", "false");
                } else
                    return false;
            });
            $(".main-user-tag").on('show.bs.dropdown', function () {
                $("#main-user-tag-data").val($.cookie('user-tag')).focus();
            });
            $(".main-user-tag").on('shown.bs.dropdown', function () {
                $("#main-user-tag-data").focus();
            });
            $(".close", ".main-user-tag").click(function () {
                $(".main-user-tag").attr("close", "true");
            });
        },
        fullScreen: function () {
            $(".main-full-screen").click(function () {
                if ($("span", this).hasClass("layui-icon-screen-restore")) {
                    exitFullScreen();
                    $("span", this).removeClass("layui-icon-screen-restore").addClass("layui-icon-screen-full");
                } else {
                    fullScreen();
                    $("span", this).removeClass("layui-icon-screen-full").addClass("layui-icon-screen-restore");
                }
            });
        },
        navSearch: function () {
            $("#nav-search-tips").append("<li class='nav-search-tips-item'><a href='javascript:;'>请输入关键字...</a></li>");
            $("#nav-search-input").keyup(function () {
                $("#nav-search-dropdown").addClass('open');
                $("#nav-search-tips").empty();
                if ($(this).val() || '') {
                    $("#nav-search-tips").append("<li class='nav-search-tips-item'><a href='javascript:;'>无相关记录...</a></li>");
                    $(".nav-subtitle:contains(" + $(this).val() + ")", "#side-menu").each(function () {
                        $(".nav-search-tips-item:first", "#nav-search-tips").hide();
                        var text = "";
                        $(this).parents("li").each(function () {
                            if (text == "")
                                text += $(this).children("a").attr("data-name");
                            else
                                text = $(this).children("a").attr("data-name") + "-" + text;
                        });
                        var tipsItem = $("<li class='nav-search-tips-item'><a href='javascript:;'data-text='" + $(this).text() + "' data-id='" + $(this).parent("a").attr("data-id") + "'>" + text + "</a></li>");
                        $("a", tipsItem).click(function () {
                            $(".nav-side-alink[data-id='" + $(this).attr("data-id") + "']", "#side-menu").click();
                            $("#nav-search-input").val($(this).attr("data-text"));
                        });
                        $("#nav-search-tips").append(tipsItem);
                    });
                } else {
                    $("#nav-search-tips").append("<li class='nav-search-tips-item'><a href='javascript:;'>请输入关键字...</a></li>");
                }
            });
        },
        ctrlClose: function () {
            $(".dropdown-menu li a", "#xms-sb-ctrlClose").each(function () {
                $(this).click(function () {
                    var $this = $(".xms-subn-items-bgcol", ".xms-subn-items");
                    var index = $this.index();
                    switch ($(this).attr("data-type")) {
                        case "0":
                            removeIframe($this.children('span'));
                            break;
                        case "1":
                            removeIframes($("a:lt(" + index + ") span", ".xms-subn-items"));
                            break;
                        case "2":
                            removeIframes($("a:gt(" + index + ") span", ".xms-subn-items"));
                            break;
                        case "3":
                            removeIframes($("a:not(.xms-subn-items-bgcol) span", ".xms-subn-items"));
                            break;
                        case "4":
                            removeIframes($("a span", ".xms-subn-items"));
                            break;
                        default:
                            break;
                    }
                    ctrlCloseBtnHandle();
                });
            });
        },
        showAbout: function () {
            var layer = window.layer;
            if (layer == null) {
                layui.use('layer', function () {
                    layer = window.layer = layui.layer;
                });
            }

            $("a", "#main-about").click(function () {
                layer.open({
                    type: 1
                    , title: ['版本信息', 'text-align:left;']
                    , area: ['300px', ($(window).height() - 40) + "px"]
                    , offset: ['40px', ($(window).width() - 300) + "px"]
                    , id: 'layerAbout'
                    , content: $('#main-about-content').removeClass("hide")
                    , shade: 0 //显示遮罩
                    , move: false
                    , yes: function () {
                        layer.closeAll();
                    }
                    , end: function () {
                        $('#main-about-content').addClass("hide");
                    }
                });
            });
        },
        currentTheme: '',
        changeTheme: function () {
            var layer = window.layer;
            if (layer == null) {
                layui.use('layer', function () {
                    layer = window.layer = layui.layer;
                });
            }
            $(".main-theme").click(function () {
                var theme = $.cookie('theme') || "default";
                $('#main-theme-content ul li[data-alias="' + theme + '"]').addClass("theme-this").siblings().removeClass("theme-this");
                layer.open({
                    type: 1
                    , title: ['主题方案', 'text-align:left;']
                    , area: ['350px', ($(window).height() - 40) + "px"]
                    , offset: ['40px', ($(window).width() - 350) + "px"]
                    , id: 'layerTheme'
                    , content: $('#main-theme-content').removeClass("hide")
                    , shade: 0 //显示遮罩
                    , move: false
                    , end: function () {
                        $('#main-theme-content').addClass("hide");
                    }
                });
            });
            $("#main-theme-content ul li").click(function () {
                $(this).addClass("theme-this").siblings().removeClass("theme-this");
                $("#themeLink").attr('href', '/content/css/theme/' + $(this).attr("data-alias") + '.css');
                pageWrap.currentTheme = $(this).attr("data-alias");
                $.cookie('theme', $(this).attr("data-alias"), { path: '/' });
                $('#page-content').children().each(function () {
                    var iframeid = this.id;
                    if (iframeid) {
                        Xms.Web.callChildMethod(iframeid, 'changeTheme', pageWrap.currentTheme);
                    }
                });
            });
        }
    }
    //导航过高时滚动处理
    function scrollMenu(e, delta) {
        e = e || window.event;
        var target = e.target || e.srcElement;
        delta = e.deltaY;
        //console.log(e.originalEvent.deltaY);
        var $this = $(this);
        // if ($(target).closest($wrap).length > 0) {
        var dir = delta > 0 ? 'Up' : 'Down';
        if (dir == 'Up') {
            var menuTop = $menu.position().top;
            step = Math.sqrt(($menu.width() * $menu.width()) + (ofsetH * ofsetH)) / 6;
            console.log(step);
            var runstep = menuTop + step;
            if (runstep >= 0) {
                runstep = 0;
                scrolltip.text('向下滚动').show().css('top', winSize.h - 60);
            } else {
                scrolltip.hide();
            }
            $menu.css('top', runstep);
        } else {
            var menuTop = $menu.position().top;
            step = Math.sqrt(($menu.width() * $menu.width()) + (ofsetH * ofsetH)) / 6;
            var runstep = menuTop - step;
            if (runstep <= -ofsetH) {
                runstep = -ofsetH;
                scrolltip.text('向上滚动').show().css('top', 0);
            } else {
                scrolltip.hide();
            }
            $menu.css('top', runstep);
        }
    }

    //导航过高处理
    function bindSideMenuScoll() {
        var $wrap = $('#sidebarMenuWrap');
        var $menu = $('#side-menu');
        var $window = $(window);
        var winSize = { h: $window.height(), w: $window.width() };
        var menuHeight = $menu.outerHeight();
        var offsetTop = 40;//与顶部的距离
        // $wrap.css('position', 'relative');
        var step = 10;
        var ofsetH = offsetTop + menuHeight - winSize.h;
        if (winSize.h < offsetTop + menuHeight) {
            $menu.css({ 'height': menuHeight, 'position': 'absolute', width: '100%' });
            $('body').css('overflow', 'hidden');
            $menu.css({ 'height': winSize.h });
            $menu.addClass('scrollingDown').removeClass('scrollingUp');
            var scrolltip = $('<div class="scrolltips">向下滚动</div>');
            $wrap.append(scrolltip);
            scrolltip.css('top', winSize.h - 60);
            //console.log('leftnavscroll', winSize.h, offsetTop, menuHeight);
            $menu.mousewheel(function scrollMenu(e, delta) {
                e = e || window.event;
                var target = e.target || e.srcElement;
                delta = e.deltaY;
                var $this = $(this);
                var dir = delta > 0 ? 'Up' : 'Down';
                if ($(target).closest('.dropdown-hover-level').length > 0) {
                    return false;
                } else {
                    if (dir == 'Up') {
                        var menuTop = $menu.position().top;
                        step = Math.sqrt(($menu.width() * $menu.width()) + (ofsetH * ofsetH)) / 6;
                        console.log(step);
                        var runstep = menuTop + step;
                        if (runstep >= 0) {
                            runstep = 0;
                            $menu.addClass('scrollingDown').removeClass('scrollingUp');
                            scrolltip.text('向下滚动').show().css('top', winSize.h - 60);
                        } else {
                            scrolltip.hide();
                        }
                        $menu.css('top', runstep);
                    } else {
                        var menuTop = $menu.position().top;
                        step = Math.sqrt(($menu.width() * $menu.width()) + (ofsetH * ofsetH)) / 6;
                        var runstep = menuTop - step;
                        if (runstep <= -ofsetH) {
                            runstep = -ofsetH;
                            $menu.addClass('scrollingUp').removeClass('scrollingDown');
                            scrolltip.text('向上滚动').show().css('top', 0);
                        } else {
                            scrolltip.hide();
                        }
                        $menu.css('top', runstep);
                    }
                }
            });
        }
    }
    //左侧菜单伸缩
    function winLH() {
        var win = $('.xms-header').width();
        if (win >= 751) {
            $(".view_left").show();
            $(".view_left").height($(document).height() - 40 + 'px');
        }
        if (win < 751) {
            $(".view_left").height('100%');
            $(".view_left").css('display', 'none');
        }
    }
    function bindSliderHover() { }
    function unbindSliderHover() {
        var slider = $('#xms-sideBarbtn');
        slider.unbind("mouseenter");
    }

    //添加到下方快捷访问的标签
    function takeToBottom(e, linkname, linkhref, linkid, linktarget) {
        var linkName = linkname || $(e).attr("data-name");
        var linkHref = linkhref || $(e).attr("data-href");
        var linkId = linkid || $(e).attr("data-id");
        var linkTarget = linktarget || $(e).attr('target');
        var $toggletree = $('.toggle-tree');
        var offset = $toggletree.offset().top;
        var height = $toggletree.outerHeight();
        var toggleTreeTop = $toggletree.scrollTop();
        var wrapSize = $(window).height() - 50;
        if (wrapSize > height) {
            $toggletree.trigger('mousewheel')
        }
        if (linkHref == 'javascript:void(0)') return false;
        var id = "iframe" + linkId;
        var iframeInfo = {
            iframeId: linkId,
            link: linkHref
        };
        if (!linkTarget || linkTarget == 'pageViewName') {
            $('.xms-subn-items').find('a').each(function (i, n) {
                var xmsName = $(n).attr('data-name');
                if (linkName == xmsName) {
                    $(n).remove();
                    var iframeId = $(n).attr('data-iframe');
                    $('#page-content').find('#' + iframeId).remove();
                }
            });

            $('.ifrmae-list').hide();
            var tempIframe = createIframe(id, linkHref, linkTarget);

            changeHashRouter(iframeInfo);
            if (linkHref != 'javascript:void(0)') {
                $('.xms-subn-items-bgcol').removeClass('xms-subn-items-bgcol');
                var tabBtn = $('<a target="pageViewName" data-iframe="' + id + '" data-iframeid="' + id.replace('iframe', '') + '" data-name="' + linkName + '" onclick="showIframe(this)" class="btn btn-default xms-subn-items-bgcol" href="javascript:void(0)">' + linkName + ' <span class="glyphicon glyphicon-remove" onclick="removeIframe(this)"></span></a>');
                $('.xms-subn-items').prepend(tabBtn);
                tabBtn.trigger('click');
            }
        } else {
            openNavTargetType(linkTarget, iframeInfo);
        }
        $(".xms-sb-ctrlLeft").trigger('click');
        return tempIframe;
    }
    function openNavTargetType(type, iframeInfo) {
        type = type || '';
        if (!~type.indexOf('_')) {
            type = '_' + type;
        }
        Xms.Web.OpenWindow(iframeInfo.link, type);
    }
    function toggleBottom(e) {
        $(e).next('ul').hide();
    }
    function mouseBottom(e) {
        $(e).parents('li:first').mouseenter(function () {
            $(e).next().show();
        })
        $(e).parents('li:first').mouseleave(function () {
            $(e).next().hide();
        })
    }

    function createIframe(id, url, targetname) {
        var offsetTop = $('#page-content').offset().top;
        var $window = $(window);
        var winsize = { w: $window.width(), h: $window.height() };
        var iframeH = winsize.h - offsetTop;
        targetname = targetname || id;
        if (iframeH < 300) {
            iframeH = 300;
        }
        if ($('#' + id).length > 0) {
            var iframe = $(id);
        } else {
            iframeList.push(id);
            var iframe = $('<iframe width="100%" src="' + url + '" id="' + id + '" name="' + targetname + '" class="ifrmae-list " frameborder="0" onload="Javascript:SetWinHeight(this)" style=" height:' + iframeH + 'px;" scrolling="auto"></iframe>"');
            iframe.prependTo('#page-content');
        }

        return iframe;
    }
    function setIframeHeight() {
        var offsetTop = $('#page-content').offset().top;
        var $window = $(window);
        var winsize = { w: $window.width(), h: $window.height() };
        var iframeH = winsize.h - offsetTop;
        $('#page-content').find('iframe').height(iframeH)
    }
    function showIframe(e) {
        $('.ifrmae-list').hide().removeClass('active-iframe');
        var iframeId = $(e).attr('data-iframe');

        $('#page-content').find('#' + iframeId).show().addClass('active-iframe');
    }
    function removeIframe(e) {
        e = e || window.event;
        var iframeId = $(e).parent('a').attr('data-iframe');

        var parents = $(e).parent().parent();
        var alen = parents.children('a').length;
        var aindex = $(e).parent('a').index();
        $(e).parent('a').remove();
        $('#page-content').find('#' + iframeId).remove();
        if ($('#page-content').find('iframe:visible').length <= 0) {
            // $('#page-content').find('iframe:eq(0)').show();
            if (parents.find('a').length > 0) {
                setTimeout(function () {
                    parents.find('a:last').click();
                })
            } else {
                setTimeout(function () {
                    $("#xms-subn-home").click();
                })
            }
        } else {
            if (aindex == alen - 1) {
                setTimeout(function () {
                    parents.children('a').eq(aindex - 1).click();
                })
            }
        }

        e.stopPropagation && e.stopPropagation();
    }
    function removeIframes(e) {
        $(e).each(function () {
            removeIframe(this);
        });
    }
    function setpageContentHeight() {
        var pageContent = $('#page-content');
        var height = 400;
        var pageTop = pageContent.offset().top;
        var windowH = $(window).height();
        height = windowH - pageTop;
        // console.log(height)
        // pageContent.height(height);
        return height;
    }
    function SetWinHeight(obj) {
        var contentH = setpageContentHeight();
        $("#page-content>iframe").css({ "min-height": contentH, "height": contentH });
        $('body').trigger('extendIframe.onloaded', { iframe: obj });
        //getIframeHeight(contentH);
    }
    function getIframeHeight(maxHeight) {
        var _iframe = $("#page-content").children("iframe:visible");
        var iframeH = _iframe.contents().find("html").height();
        if (iframeH < maxHeight) {
            $("#page-content>iframe").css("min-height", maxHeight - 5);
        } else {
            $("#page-content>iframe").css("min-height", iframeH);
        }
    }
    function changeHashRouter(info) {
        location.hash = "/redirectTo:" + info.iframeId;
    }
    function fullScreen() {
        var elem = document.body;
        if (elem.webkitRequestFullScreen) {
            elem.webkitRequestFullScreen();
        } else if (elem.mozRequestFullScreen) {
            elem.mozRequestFullScreen();
        } else if (elem.requestFullScreen) {
            elem.requestFullscreen();
        } else {
            Xms.Web.Toast("浏览器不支持全屏API或已被禁用", false);
        }
    }
    function exitFullScreen() {
        var elem = document;
        if (elem.webkitCancelFullScreen) {
            elem.webkitCancelFullScreen();
        } else if (elem.mozCancelFullScreen) {
            elem.mozCancelFullScreen();
        } else if (elem.cancelFullScreen) {
            elem.cancelFullScreen();
        } else if (elem.exitFullscreen) {
            elem.exitFullscreen();
        } else {
            Xms.Web.Toast("浏览器不支持全屏API或已被禁用", false);
        }
    }
    function ctrlCloseBtnHandle() {
        if ($("#xms-subn-home").hasClass("xms-subn-items-bgcol")) {
            $(".dropdown-menu li", "#xms-sb-ctrlClose").hide();
            $(".dropdown-menu li:eq(3)", "#xms-sb-ctrlClose").show();
        } else {
            $(".dropdown-menu li", "#xms-sb-ctrlClose").show();
            var len = $('.xms-subn-items a').length;
            var $this = $('.xms-subn-items .xms-subn-items-bgcol');
            if (len == 1) {
                $(".dropdown-menu li:gt(0)", "#xms-sb-ctrlClose").hide();
            } else if ($this.index() == 0) {
                $(".dropdown-menu li:eq(1)", "#xms-sb-ctrlClose").hide();
            } else if ($this.index() == len - 1) {
                $(".dropdown-menu li:eq(2)", "#xms-sb-ctrlClose").hide();
            }
        }
    }
    window.mouseBottom = mouseBottom;
    window.toggleBottom = toggleBottom;
    window.takeToBottom = takeToBottom;
    window.getIframeHeight = getIframeHeight;
    window.SetWinHeight = SetWinHeight;
    window.setpageContentHeight = setpageContentHeight;
    window.removeIframe = removeIframe;
    window.showIframe = showIframe;
    window.createIframe = createIframe;
    window.pageWrap = pageWrap;
    return pageWrap;
});