var UITree = function () {

    var ajaxTreeSample = function () {

        $("#sidebar-cms-category-tree")
            .on("select_node.jstree", function (e, data) {
                var href = data.node.a_attr.href;
                document.location.href = href;
            })
            .jstree({
                "core": {
                    "themes": {
                        "responsive": true
                    },
                    "check_callback": true,
                    'data': {
                        'url': function (node) {
                            return '/AdminPageSideBar/CmsCategoryTree';
                        }
                        ,
                        'data': function (node) {
                            return { 'id': node.id };
                        }
                    }
                }
                ,
                "types": {
                    "default": {
                        "icon": "fa fa-folder icon-state-warning icon-lg"
                    },
                    "root": {
                        "icon": "fa fa-folder icon-state-warning icon-lg"
                    },
                    "file": {
                        "icon": "fa fa-file icon-state-warning icon-lg"
                    }
                }
                ,
                "plugins": ["types"]
            }).jstree("deselect_all");


        $("#sidebar-ecommerce-category-tree")
            .on("select_node.jstree", function (e, data) {
                var href = data.node.a_attr.href;
                document.location.href = href;
            })
            .jstree({
                "core": {
                    "themes": {
                        "responsive": true
                    },
                    "check_callback": true,
                    'data': {
                        'url': function (node) {
                            return '/AdminPageSideBar/EcommerceCategoryTree';
                        }
                        ,
                        'data': function (node) {
                            return { 'id': node.id };
                        }
                    }
                }
                ,
                "types": {
                    "default": {
                        "icon": "fa fa-folder icon-state-warning icon-lg"
                    },
                    "root": {
                        "icon": "fa fa-folder icon-state-warning icon-lg"
                    },
                    "file": {
                        "icon": "fa fa-file icon-state-warning icon-lg"
                    }
                }
                ,
                "plugins": ["types"]
            }).jstree("deselect_all");
    }

    return {
        init: function () {
            ajaxTreeSample();
            $("#sidebar-category-tree").jstree("deselect_all");
        }
    };

}();

if (App.isAngularJsApp() === false) {
    jQuery(document).ready(function () {
        UITree.init();
    });
}