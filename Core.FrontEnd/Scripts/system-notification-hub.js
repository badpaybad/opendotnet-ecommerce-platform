var __systemHandles = [];
__systemHandles['SystemBroadCast'] = function (dataJson) {
    switch (dataJson.type) {
        case "warning":
            toastr.warning(dataJson.message);
            break;
        case 'success':
            toastr.success(dataJson.message);
            break;
        case 'error':
            toastr.error(dataJson.message);
            break;
        default:
            toastr.info(dataJson.message);
            break;
    }
};


var __systemNotificationHub = $.connection.systemNotificationHub;

__systemNotificationHub.client.boardCastMessage = function (msg) {
    __systemHandles[msg.DataType](JSON.parse(msg.DataJson));
}

__systemNotificationHub.client.broadCastMonitorMessage = function (msg) {
    __systemHandles[msg.DataType](JSON.parse(msg.DataJson));
}

__systemNotificationHub.client.shoppingCartMessage = function (msg) {
    __systemHandles[msg.DataType](JSON.parse(msg.DataJson));
}

$.connection.hub.start().done(function () {
    __systemNotificationHub.server.clientConnected();
});

