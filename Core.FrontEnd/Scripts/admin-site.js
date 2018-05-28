var Util = {
    _month :['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    dateTimeFromJson:function(val) {
        var d = new Date(parseInt(val.substr(6)));
        var month = d.getMonth() + 1;
        var day = d.getDate();
        if (month < 10) month ='0'+ month;
        if (day < 10) day ='0'+ day;
        var date = d.getFullYear() + "-" + month + "-" +day;
        var time = d.toLocaleTimeString().toLowerCase();
        return date + ' ' + time;
    }
    , initTinyMceDomId:function(domId,height) {
        tinymce.init({
            selector: domId,
            height: height,
            theme: 'modern',
            force_br_newlines: true,
            force_p_newlines: false,
            forced_root_block: '',
            plugins: 'print preview paste searchreplace autolink directionality code visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists textcolor wordcount spellchecker imagetools media  link contextmenu colorpicker textpattern help',
            toolbar1: 'formatselect | bold italic strikethrough forecolor backcolor | alignleft aligncenter alignright alignjustify  | numlist bullist outdent indent  | removeformat image link media | fullscreen',
            menubar: "tools",
            image_advtab: true,
            file_browser_callback: RoxyFileBrowser,
            templates: [
                //{ title: 'Test template 1', content: 'Test 1' },
                //{ title: 'Test template 2', content: 'Test 2' }
            ],
            content_css: [
                //'//fonts.googleapis.com/css?family=Lato:300,300i,400,400i',
                '/Content/Site.css'
            ]
        });
    }
    , initTinyMceClassName: function (className, height) {
        tinymce.init({
            selector: className,
            height: height,
            theme: 'modern',
            force_br_newlines: true,
            force_p_newlines: false,
            forced_root_block: '',
            plugins: 'print preview paste searchreplace autolink directionality code visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists textcolor wordcount spellchecker imagetools media  link contextmenu colorpicker textpattern help',
            toolbar1: 'formatselect | bold italic strikethrough forecolor backcolor | alignleft aligncenter alignright alignjustify  | numlist bullist outdent indent  | removeformat image link media | fullscreen',
            menubar: "tools",
            image_advtab: true,
            file_browser_callback: RoxyFileBrowser,
            templates: [
                //{ title: 'Test template 1', content: 'Test 1' },
                //{ title: 'Test template 2', content: 'Test 2' }
            ],
            content_css: [
                //'//fonts.googleapis.com/css?family=Lato:300,300i,400,400i',
                '/Content/Site.css'
            ]
        });
    },
    trimChar: function(string, charToRemove) {
        while(string.charAt(0) == charToRemove) {
            string = string.substring(1);
        }

        while(string.charAt(string.length - 1) == charToRemove) {
            string = string.substring(0, string.length - 1);
        }

        return string;
    },
    showWaiting: function () {
        jQuery('#modal-waiting').modal('show', { backdrop: 'static', keyboard: false });
    },
    hideWaiting: function () {
        jQuery('#modal-waiting').modal('hide');
    },
    replace:function(src, charToReplace, newChar) {
        while (src.indexOf(charToReplace) >= 0) {
            src = src.replace(charToReplace, newChar);
        }
        return src;
    }
}

var Dashboard= {
    init:function() {
        //orderConfirmedCount
        //contactMesageTodayCount
        var data = {};

        $.post('/Admin/AdminDashboard/OrderAndContactMsgCount', data)
            .done(function (data) {
                if (data.Ok) {
                    $('#orderConfirmedCount').text(data.Data["OrderConfirmedCount"]);
                    $('#contactMesageTodayCount').text(data.Data["ContactMessageTodayCount"]);
                } else {
                    $('#orderConfirmedCount').text(0);
                    $('#contactMesageTodayCount').text(0);
                }
            }).fail(function () {
                bootbox.alert({
                    message: "Can not make request, check your internet and try to reload page",
                    backdrop: true
                });
            });
    }
}

Dashboard.init();


