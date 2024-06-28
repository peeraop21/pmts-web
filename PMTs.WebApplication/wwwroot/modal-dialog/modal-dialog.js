function modalDialog(style, message) {
    var icon;
    var title;

    switch (style) {
        case "defaultEn": icon = "info"; title = "Info"; break;
        case "successEn": icon = "success"; title = "Success"; break;
        case "errorEn": icon = "error"; title = "Error"; break;
        case "warningEn": icon = "warning"; title = "Warning"; break;

        case "defaultTh": icon = "info"; title = "ข้อมูล"; break;
        case "successTh": icon = "success"; title = "สำเร็จ"; break;
        case "errorTh": icon = "error"; title = "ผิดพลาด"; break;
        case "warningTh": icon = "warning"; title = "คำเตือน"; break;

    }

    

    ToastError(icon, message,title);

    style = null;
    message = null;
    title = null;
    //$('body').append(modal);
    //$('body').append(btnShowDialog);
    //$('.btnShowModalDialog').click();
    
}
function ToastError(icons, message, title) {
    $.toast({
        heading: title,
        text: message,
        position: 'top-right',
        loaderBg: '#ff6849',
        icon: icons,
        hideAfter: 3000,
        stack: 6
    });
}
function CloseModalDialog() {
    $(".modalDialog").remove();
    $(".modal-backdrop").remove();
    $(".btnShowModalDialog").remove();
    $("body").css("overflow-y", "scroll");
}