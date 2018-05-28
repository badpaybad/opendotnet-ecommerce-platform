tinymce.init({
    //selector: 'textarea',
    selector: '.text-editor',
    height: 400,
    theme: 'modern',
    force_br_newlines: true,
    force_p_newlines: false,
    forced_root_block: '',
    plugins: 'print preview paste searchreplace autolink directionality code visualblocks visualchars fullscreen image link media template codesample table charmap hr pagebreak nonbreaking anchor toc insertdatetime advlist lists textcolor wordcount spellchecker imagetools media  link contextmenu colorpicker textpattern help',
    menubar: "tools",
    toolbar1: 'formatselect | bold italic strikethrough forecolor backcolor | alignleft aligncenter alignright alignjustify  | numlist bullist outdent indent  | removeformat image link media | fullscreen',
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

function RoxyFileBrowser(field_name, url, type, win) {
    var roxyFileman = '/assets/Roxy_fileman/index.html';
    //if (roxyFileman.indexOf("?") < 0) {
    //    roxyFileman += "?type=" + type;
    //}
    //else {
    //    roxyFileman += "&type=" + type;
    //}
    roxyFileman += '?input=' + field_name + '&value=' + win.document.getElementById(field_name).value;
    if (tinyMCE.activeEditor.settings.language) {
        roxyFileman += '&langCode=' + tinyMCE.activeEditor.settings.language;
    }
    tinyMCE.activeEditor.windowManager.open({
        file: roxyFileman,
        title: 'Roxy Fileman',
        width: 850,
        height: 650,
        resizable: "yes",
        plugins: "media",
        inline: "yes",
        close_previous: "no"
    }, { window: win, input: field_name });
    return false;
}