; (function ($) {
    "use strict"
    var defaults = {
        offsetWidth: 0,
        showCount: 5,
        leftCtrl: $(".xms-sb-ctrlLeft"),
        rightCtrl: $(".xms-sb-ctrlRight"),
        delAllCtrl: $(".xms-sb-ctrlDelAll"),
        delItemFunc: null
    }
    function getItemsWidth(items) {
        var sum = 0;
        if (items.length && items.length > 0) {
            items.each(function (n, t) {
                sum += $(t).outerWidth();
            });
            return sum;
        }
    }
    function targetDisabled(target, type) {
        $(target).prop("disabled", type);
    }
    $.fn.swipeItems = function (opts) {
        opts = $.extend({}, defaults, opts);
        return this.each(function () {
            var self = this, $this = $(self), items = $this.children(), itemsLength = items.length, winSize = { width: $(window).width(), height: $(window).height() };
            var offset = $this.offset();
            var wrapWidth = winSize.width - offset.left, ctrlMargin = 0, index = 0, end = 0, start = 0;
            if (opts.offsetWidth > 0) {
                wrapWidth = winSize.width - opts.offsetWidth;
            }
            if (opts.startIndex) {
                index = opts.startIndex;
            }
            /* var itemsWidth = getItemsWidth(items);
             if(itemsWidth>wrapWidth){
                 opts.leftCtrl.hide();
                 opts.rightCtrl.hide();
             }else{
                 opts.leftCtrl.hide();
                 opts.rightCtrl.hide();
             }*/
            //init
            $this.children("a:gt(" + (index + opts.showCount - 1) * 1 + ")").hide();
            opts.leftCtrl.addClass("disabled");
            //
            var isPressLeft = false;
            function pressLeft() {
                if (index <= 0) {
                    index = 0;

                    $this.children("a:gt(" + (index + opts.showCount - 1) * 1 + ")").hide();
                    isPressRight = true;
                    isPressLeft = false;
                    opts.leftCtrl.addClass("disabled");
                    opts.rightCtrl.removeClass("disabled");
                } else {
                    $this.children("a:lt(" + index + ")").hide();
                    $this.children("a:gt(" + (index + opts.showCount - 1) * 1 + ")").hide();
                    isPressRight = true;
                    isPressLeft = true;
                    opts.leftCtrl.removeClass("disabled");
                    opts.rightCtrl.removeClass("disabled");
                }
            }
            var isPressRight = true;
            function pressRight() {
                if (index + opts.showCount >= itemsLength) {
                    index = itemsLength - opts.showCount;
                    opts.rightCtrl.addClass("disabled");
                    opts.leftCtrl.removeClass("disabled");
                    isPressRight = false;
                    isPressLeft = true;
                    $this.children("a:lt(" + index + ")").hide();
                } else {
                    opts.rightCtrl.removeClass("disabled");
                    opts.leftCtrl.removeClass("disabled");
                    isPressLeft = true;
                    isPressRight = true;
                    $this.children("a:lt(" + index + ")").hide();
                    $this.children("a:gt(" + (index + opts.showCount - 1) * 1 + ")").hide();
                }
            }

            opts.leftCtrl.bind("click", function () {
                if (isPressLeft == false) return false;
                index--;
                $this.children("a").show();
                pressLeft();
            });
            opts.rightCtrl.bind("click", function () {
                if (isPressRight == false) return false;
                index++;
                $this.children("a").show();
                pressRight();
            });
            opts.delAllCtrl.bind("click", function () {
                console.log($this.children("a"));
                $this.children("a").each(function () {
                    console.log(this);
                    opts.delItemFunc && opts.delItemFunc(this);
                });
            });
        });
    }
})(jQuery);

; (function ($, root) {
    "use strict"
    var navTmplate = '';

    $.fn.mainNavRender = function (opts) {
        this.each(function () {
            var self = this;
            Xms.Web.GetJson("/user/userprivilegestree", false, function (data) {
                var toggleTreeTmpl = '{{if children && children.length>0}} {{each children}} {{if this.children}}<li class="dropdown dropdown-hover"><a class="dropdown-toggle navbar-link  nav-side-alink" target="${navGetTarget(this)}" onclick="" data-href="${navGetLink(this)}" data-id="${$value.id}" data-name="${$value.label}"><span class="${navGetIcon(this)}" title="${$value.label}"></span><span class="caret"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></a><div class="dropdown-menu dropdown-hover-level"><ul class="dropdown-toggle">{{if this.children}} {{each this.children}} {{if this.children}} {{if this.url}} {{if this.smallicon}}<li><a class="nav-side-alink" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}" data-name="${$value.label}" target="${navGetTarget(this)}"><span class="${navGetIcon(this)}" title="${$value.label}"></span><span class="gy-dot"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></a> {{if this.url}} <span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span> {{/if}}</li>{{else}}<li><a class="nav-side-alink" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}" data-name="${$value.label}" target="${navGetTarget(this)}"><span class="${navGetIcon(this)}" title="${$value.label}"></span><span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></a> {{if this.url}} <span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span>{{/if}}</li>{{/if}} {{else}} {{if this.smallicon}}<li class="dropdown-header"><span class="gy-dot"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></li>{{else}}<li class="dropdown-prev"><a class="dropright-toggle navbar-link nav-side-alink" data-toggle="dropright" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}" data-name="${$value.label}" target="${navGetTarget(this)}"><span class="${navGetIcon(this)}" title="${$value.label}"></span><span class="caret"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></a><ul class="dropdown-toggle dropdown-menu dropdown-hover-level">{{each this.children}} {{if this.children}}<li class="nav-childhead"><a class="nav-side-alink" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}" data-name="${$value.label}" target="${navGetTarget(this)}"><span class="${navGetIcon(this)}" title="${$value.label}"></span><span class="caret"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></a> {{if this.url}}<span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span> {{/if}}<ul class="dropdown-toggle dropdown-menu dropdown-hover-level">{{each this.children}}<li><a class="dropright-toggle navbar-link nav-nochild" data-toggle="dropright" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}" data-name="${$value.label}" target="${navGetTarget(this)}"><span class="gy-dot"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></a> {{if this.url}} <span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span> {{/if}}</li>{{/each}}</ul></li>{{else}}<li class="no-childhead"><a  class="nav-side-alink" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}" data-name="${$value.label}" target="${navGetTarget(this)}"><span class="${navGetIcon(this)}" title="${$value.label}"></span><span class="gy-dot"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></a> {{if this.url}}<span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span> {{/if}}</li>{{/if}} {{/each}}</ul></li>{{/if}} {{/if}} {{else}} {{if this.url}} {{if this.smallicon}}<li class="nosubmav"><a class="nav-side-alink" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}" data-name="${$value.label}" target="${navGetTarget(this)}"><span class="${navGetIcon(this)}" title="${$value.label}"></span><span class="gy-dot"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></a> {{if this.url}} <span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span> {{/if}}</li>{{else}}<li class="nosubmav"><a class="nav-side-alink" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}" data-name="${$value.label}" target="${navGetTarget(this)}"><span class="${navGetIcon(this)}" title="${$value.label}"></span><span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></a> {{if this.url}} <span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span> {{/if}}</li>{{/if}} {{else}} {{if this.smallicon}}<li class="dropdown-header"><span class="gy-dot"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></li>{{else}}<li class="dropdown-header dropdown-prev"><span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></li>{{/if}} {{/if}} {{/if}} {{/each}} {{/if}}</ul></div></li>{{else}}<li><a class="nav-side-alink" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}" data-name="${$value.label}" target="${navGetTarget(this)}" class="navbar-link nav-nochild"><span class="${navGetIcon(this)}" title="${$value.label}"></span></span><span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></a> {{if this.url}}<span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span>{{/if}}</li>{{/if}} {{/each}} {{/if}}<ul></ul>';

                var defaultTreeHtml = '{{if children && children.length>0}} {{each children}} {{if this.children}}<li class="dropdown dropdown-hover"><a class="dropdown-toggle navbar-link nav-side-alink" target="${navGetTarget(this)}" onclick="" data-href="${navGetLink(this)}" data-id="${$value.id}" data-name="${$value.label}"><span class="${navGetIcon(this)}" title="${$value.label}"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span> <span class="caret"></span></a><div class="dropdown-menu dropdown-hover-level"><ul class="">{{if this.children}} {{each this.children}} {{if this.children}} {{if this.url}} {{if this.smallicon}}<li><a class="nav-side-alink" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}"  data-name="${$value.label}" target="${navGetTarget(this)}" ><span class="${navGetIcon(this)}" title="${$value.label}"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span> </a> {{if this.url}}<span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span>{{/if}}</li>{{else}}<li><a data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}" class="nav-side-alink"  data-name="${$value.label}" target="${navGetTarget(this)}" >${$value.label}</a> {{if this.url}}<span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span>{{/if}}</li>{{/if}} {{else}} {{if this.smallicon}}<li class="dropdown-header"><span class="${navGetIcon(this)}" title="${$value.label}"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span> </li>{{else}}<li class="dropdown-header">${$value.label}</li>{{/if}} {{/if}} {{each this.children}} {{if this.children}}<li class="dropdown dropdown-hover"><a class="dropright-toggle navbar-link nav-side-alink" data-toggle="dropright" data-href="${navGetLink(this)}" onclick="takeToBottom(this)"  data-id="${$value.id}" data-name="${$value.label}" target="${navGetTarget(this)}" ><span class="${navGetIcon(this)}" title="${$value.label}"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span><span class="caret trans"></span></a><ul class="dropdown-menu dropdown-hover-level">{{each this.children}}<li><a class="dropright-toggle navbar-link nav-side-alink" data-toggle="dropright" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}"  data-name="${$value.label}" target="${navGetTarget(this)}" ><span class="${navGetIcon(this)}" title="${$value.label}"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></a>  {{if this.url}}<span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span>{{/if}}</li>{{/each}}</ul></li>{{else}}<li><a class="nav-side-alink" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}"  data-name="${$value.label}" target="${navGetTarget(this)}" ><span class="${navGetIcon(this)}" title="${$value.label}"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></a> {{if this.url}}<span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span>{{/if}}</li>{{/if}} {{/each}} {{else}} {{if this.url}} {{if this.smallicon}}<li><a class="nav-side-alink" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}"  data-name="${$value.label}" target="${navGetTarget(this)}" ><span class="${navGetIcon(this)}" title="${$value.label}"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span> </a> {{if this.url}}<span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span>{{/if}}</li>{{else}}<li><a class="nav-side-alink" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}"  data-name="${$value.label}" target="${navGetTarget(this)}" >${$value.label}</a> {{if this.url}}<span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span>{{/if}}</li>{{/if}} {{else}} {{if this.smallicon}}<li class="dropdown-header"><span class="${navGetIcon(this)}" title="${$value.label}"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span> </li>{{else}}<li class="dropdown-header">${$value.label}</li>{{/if}} {{/if}} {{/if}} {{/each}} {{/if}}</ul></div></li>{{else}}<li><a class="nav-side-alink" data-href="${navGetLink(this)}" onclick="takeToBottom(this)" data-id="${$value.id}"  data-name="${$value.label}" target="${navGetTarget(this)}"  class="navbar-link"><span class="${navGetIcon(this)}" title="${$value.label}"></span> <span class="nav-subtitle" title="${$value.label}" >${$value.label}</span></a> {{if this.url}}<span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span>{{/if}}</li>{{/if}} {{/each}} {{/if}}<ul></ul>';
                if ($(".toggle-tree").length > 0) {
                    //    var navtmpl = $.template("navtmpl", toggleTreeTmpl);
                    renderToggleTree(data.content, $(self))
                } else {
                    var navtmpl = $.template("navtmpl", defaultTreeHtml);
                }
                //data.content = JSON.parse(data.Content);
                //console.log(data.content);
                // $.tmpl("navtmpl", data.content).appendTo($(self));
                //设置链接为主页
                $('.navsethomepage', '#side-menu').on('click', function (e) {
                    setHomePage(this);
                });
                function setHomePage(obj) {
                    var _url = $(obj).prev('a').attr('data-href');
                    if (_url == "" || !_url) { return false; }
                    Xms.Web.Post("/user/SetUserHomePage?homepage=" + _url, {}, function (response) {
                        if (response.IsSuccess) {
                            Xms.Web.Alert('设置成功');
                        }
                    }, null, null, true, true, { jsonajax: true });
                }
                opts.callback && opts.callback(self);
            });
        });
    }

    function renderToggleTree(datas, $context) {
        // console.log(datas);
        var res = renderToggleLeaf(datas[0].children);
        //console.log(res);
        $context.append($(res.join('')))
    }

    function renderToggleLeaf(datas, res) {
        res = res || [];
        $.each(datas, function (i, n) {
            var hasChildren = n.children && n.children.length > 0;
            res.push('<li class="dropdown dropdown-hover">');
            if (hasChildren) {
                res.push('<a class="dropdown-toggle navbar-link  nav-side-alink" target="' + navGetTarget(n) + '" onclick="" data-href="' + navGetLink(n) + '" data-id="' + n.id + '" data-name="' + n.label + '"><span class="' + navGetIcon(n) + '" title="' + n.label + '"></span>' + navGetCaret(n) + ' <span class="nav-subtitle" title="' + n.label + '">' + n.label + '</span></a>');
            } else {
                res.push('<a class="dropdown-toggle navbar-link  nav-side-alink" target="' + navGetTarget(n) + '" onclick="takeToBottom(this)" data-href="' + navGetLink(n) + '" data-id="' + n.id + '" data-name="' + n.label + '"><span class="' + navGetIcon(n) + '" title="' + n.label + '"></span>' + navGetCaret(n) + ' <span class="nav-subtitle" title="' + n.label + '">' + n.label + '</span></a><span class="navsethomepage glyphicon glyphicon-home" title="设为首页"></span>');
            }

            if (hasChildren) {
                res.push('<div class="dropdown-menu dropdown-hover-level"><ul class="dropdown-toggle">')
                renderToggleLeaf(n.children, res);
                res.push('</div>')
            }
            res.push('</li>');
        });
        return res;
    }

    function navGetCaret(link) {
        if (link && link.children && link.children.length > 0) return '<span class="caret"></span>';
        if (link && (!link.children || link.children.length == 0)) return '';
    }

    function navGetLink(link) {
        // console.log('navGetLink', link);
        if (link && link.url) return (link.url.indexOf('http:') == 0 || link.url.indexOf('https:') == 0) ? link.url : (~link.url.indexOf(ORG_SERVERURL)) ? link.url : ORG_SERVERURL + link.url;//link.url /*+ (link.url.indexOf('?') > 0 ? '&isiframe=1' : '?isiframe=1')*/;
        if (link && !link.url) return "javascript:void(0)";
    }
    function navSetHomePage(link) {
        if (link && link.url) return '<span class="navsethomepage glyphicon glyphicon-home"></span>'; /*+ (link.url.indexOf('?') > 0 ? '&isiframe=1' : '?isiframe=1')*/;
        if (link && !link.url) return '';
    }
    function navGetIcon(icon) {
        if (icon && icon.smallicon) return icon.smallicon;
        if (icon && !icon.smallicon) return "glyphicon glyphicon-list";
    }
    function navGetTarget(link) {
        if (link && link.opentarget) return link.opentarget;
        if (link && !link.opentarget) return "pageViewName";
    }
    root.navGetLink = navGetLink;
    root.navSetHomePage = navSetHomePage;
    root.navGetIcon = navGetIcon;
    root.navGetTarget = navGetTarget;
})(jQuery, window);