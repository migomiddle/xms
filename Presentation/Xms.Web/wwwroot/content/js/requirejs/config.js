require.config({
    urlArgs: "v=" + (typeof XMS_PAGER_VERSION === "string" ? XMS_PAGER_VERSION : ''),
    baseUrl: '/content/js/',
    waitSeconds: 10000,
    paths: {
        //lib
        'jquery': 'jquery.min',
        'bootstrap': 'bootstrap.min',
        'jqueryBootstrap': 'jquery.bootstrap.min',
        'mousewheel': 'jquery.mousewheel',
        'jquery-mousewheel': 'jquery.mousewheel',
        //jquery-ui
        'jquery.ui': 'jquery-ui-1.10.3/ui/jquery.ui.core',
        'jquery.ui.widget': 'jquery-ui-1.10.3/ui/jquery.ui.widget',
        'jquery.ui.mouse': 'jquery-ui-1.10.3/ui/jquery.ui.mouse',
        'jquery.ui.draggable': 'jquery-ui-1.10.3/ui/jquery.ui.draggable',

        //xms
        'xms': 'xms',
        'xms.jquery': 'xms.jquery',
        'xms.web': 'xms.web',
        'fetch': 'fetch',
        'xmsMain.plugs': 'xmsmain.plugs',
        'xms-history': 'xms-history',
        'createForm': 'form',
        'renderForm': 'renderform',
        'calculation': 'calculation',

        //jquery-plugs
        'jquery.toast': 'jquery-toast/jquery.toast.min',
        'jquery.tmpl': 'jquery.tmpl',
        'jquery.cookie': 'jquery.cookie',
        'jquery.bootpag': 'jquery.bootpag.min',
        'jquery.form': 'jquery.form',
        'jquery.tableresize': 'jquery.tableresize',
        'bootstrap-datepicker': 'bootstrap-datepicker-1.5.0/js/bootstrap-datepicker.min',
        'bootstrap-datepicker-cn': 'bootstrap-datepicker-1.5.0/locales/bootstrap-datepicker.zh-CN.min',

        'jquery-validate': 'jquery-validate/jquery.validate.min',
        'jquery-validate-zh': 'jquery-validate/localization/messages_zh.min',
        'jquery.dirtyforms': 'jquery.dirtyforms',
        'jquery.printTable': 'jquery.printTable',
        'jquery.datetimepicker.full': 'bootstrap-datetimepicker/jquery.datetimepicker.full'

        //uediter
        , 'ueditor': 'ueditor/ueditor.all.min'
        , 'ueditor.config': 'ueditor/ueditor.config'
        , 'ueditor.addcustomizebutton': 'ueditor/addcustomizebutton'

        //common-modules
        , 'notice': 'common/notice'
        , 'filters': 'common/filters'
        , 'navtree': 'common/navtree'
        , 'charts': 'common/charts'
        , 'formSearcher': 'common/formsearcher'
        , 'selectEntityDialog': 'common/selectentityDialog'
        , 'dirtychecker': 'common/dirtychecker'
        , 'formular': 'common/formular'

        //pages
        , 'home.index': 'pages/home.index'
        , 'entity.list': 'pages/entity.list'
        , 'entity.gridview': 'pages/entity.gridview'
        , 'entity.create': 'pages/entity.create'
    },
    map: {
        '*': {
            'css': './requirejs/css.min' // or whatever the path to require-css is
        }
    },
    //“¿¿µ
    shim: {
        //'bootstrap': {
        //    deps: ['css!../css/a.css']
        //}
        'bootstrap': {
            deps: ['jquery']
        },
        'bootstrap': {
            deps: ['jquery', 'css!/content/css/bootstrap.min.css']
        },
        'jqueryBootstrap': {
            deps: ['jquery', 'bootstrap']
        },
        //jquery-ui
        'jquery.ui': {
            deps: ['jquery']
        },
        'jquery.ui.widget': {
            deps: ['jquery.ui']
        },
        'jquery.ui.mouse': {
            deps: ['jquery.ui.widget']
        },
        'jquery.ui.draggable': {
            deps: ['jquery.ui.mouse', 'jquery.ui.widget']
        },
        //jquery-plugs
        'jquery.toast': {
            deps: ['jquery']
        },
        'jquery.tmpl': {
            deps: ['jquery']
        },
        'jquery.cookie': {
            deps: ['jquery']
        },
        'jquery.bootpag': {
            deps: ['jquery']
        },
        'jquery.form': {
            deps: ['jquery']
        },
        'jquery-validate': {
            deps: ['jquery']
        },
        'jquery.printTable': {
            deps: ['jquery']
        },
        'jquery.tableresize': {
            deps: ['jquery']
        },
        'jquery.dirtyforms': {
            deps: ['jquery']
        },
        'bootstrap-datepicker': {
            deps: ['bootstrap']
        },
        'bootstrap-datepicker-cn': {
            deps: ['jquery', 'bootstrap-datepicker']
        },
        //'jquery.datetimepicker.full': {
        //    deps: ['jquery','jquery-mousewheel']
        //},

        //'mousewheel': {
        //    deps: ['jquery.datetimepicker.full']
        //},
        //xms

        'xms': {
            deps: ['jquery']
        },
        'xms.jquery': {
            deps: ['xms']
        },
        'xms.web': {
            deps: ['xms', 'xms.jquery', 'jquery.ui.draggable']
        },
        'xmsMain.plugs': {
            deps: ['jquery', 'jquery.tmpl']
        },
        'xms-history': {
            deps: ['jquery']
        },
        'fetch': {
            deps: ['xms']
        },
        'createForm': {
            deps: ['xms']
        },
        'renderForm': {
            deps: ['xms']
        },
        'calculation': {
            deps: ['xms']
        },
        //common
        'filters': {
            deps: ['fetch']
        },
        'charts': {
            deps: ['formSearcher']
        },
        //pages
        'entity.list': {
            deps: ['filters', 'xms.web', 'entity.gridview']
        },
        'entity.gridview': {
            deps: ['charts', 'formSearcher', 'xms.jquery', 'xms.web']
        },
        'entity.create': {
            deps: ['formular', 'dirtychecker', 'xms.jquery', 'xms.web', 'createForm', 'renderForm', 'calculation',]
        },
        'home.index': {
            deps: ['jquery']
        }
    },
});