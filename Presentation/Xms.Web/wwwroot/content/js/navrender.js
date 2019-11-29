; (function ($, root) {
    "use strict"
    $(function () {
        Xms.Web.GetJson("/security/userprivilegestree", false, function (data) {
            var navtmpl = $.template("navtmpl", '<ul class="nav navbar-nav navbar-pill">{{if children && children.length>0}} {{each children}} {{if this.children}}<li class="dropdown"><a class="dropdown-toggle navbar-link" data-toggle="dropdown" href="${navGetLink(this)}"><span class="${navGetIcon(this)}"></span> ${$value.label} <span class="caret"></span></a><ul class="dropdown-menu">{{if this.children}} {{each this.children}} {{if this.children}} {{if this.url}} {{if this.smallicon}}<li><a href="${navGetLink(this)}"><span class="${navGetIcon(this)}"></span>${$value.label}</a></li>{{else}}<li><a href="${navGetLink(this)}">${$value.label}</a></li>{{/if}} {{else}} {{if this.smallicon}}<li class="dropdown-header"><span class="${navGetIcon(this)}"></span>${$value.label}</li>{{else}}<li class="dropdown-header">${$value.label}</li>{{/if}} {{/if}} {{each this.children}} {{if this.children}}<li class="dropRight"><a class="dropright-toggle navbar-link" data-toggle="dropright" href="${navGetLink(this)}"><span class="${navGetIcon(this)}"></span> ${$value.label}<span class="caret trans"></span></a><ul class="dropright-menu">{{each this.children}}<li><a class="dropright-toggle navbar-link" data-toggle="dropright" href="${navGetLink(this)}"><span class="${navGetIcon(this)}"></span> ${$value.label}</a></li>{{/each}}</ul></li>{{else}}<li><a href="${navGetLink(this)}"><span class="${navGetIcon(this)}"></span> ${$value.label}</a></li>{{/if}} {{/each}} {{else}} {{if this.url}} {{if this.smallicon}}<li><a href="${navGetLink(this)}"><span class="${navGetIcon(this)}"></span>${$value.label}</a></li>{{else}}<li><a href="${navGetLink(this)}">${$value.label}</a></li>{{/if}} {{else}} {{if this.smallicon}}<li class="dropdown-header"><span class="${navGetIcon(this)}"></span>${$value.label}</li>{{else}}<li class="dropdown-header">${$value.label}</li>{{/if}} {{/if}} {{/if}} {{/each}} {{/if}}</ul></li>{{else}}<li><a href="${navGetLink(this)}" class="navbar-link"><span class="${navGetIcon(this)}"></span> ${$value.label}</a></li>{{/if}} {{/each}} {{/if}}<ul></ul></ul>');
            $.tmpl("navtmpl", data.content).appendTo($("#navbar"));
            console.log(data);
            // $("#navTmpl").tmpl(data.content).appendTo($("#navbar"));
            $('.dropdown-toggle').dropdownHover();
            $(".nav").on("mouseenter", ".dropRight", function () {
                var $this = $(this), target = $this.children(".dropright-menu");
                target.show();
            }).on("mouseleave", ".dropRight", function () {
                var $this = $(this), target = $this.children(".dropright-menu");
                target.hide();
            });
        });
    })
    function navGetLink(link) {
        if (link && link.url) return link.url;
        if (link && !link.url) return "#";
    }
    function navGetIcon(icon) {
        if (icon && icon.smallicon) return icon.smallicon;
        if (icon && !icon.smallicon) return "glyphicon glyphicon-list";
    }
    root.navGetLink = navGetLink;
    root.navGetIcon = navGetIcon;
})(jQuery, window);