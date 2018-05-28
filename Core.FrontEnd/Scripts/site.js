var Util = {
    _month :['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    dateTimeFromJson:function(val) {
        var d = new Date(parseInt(val.substr(6)));
        var date = d.getDate() + " " + Util._month[d.getMonth()] + ", " + d.getFullYear();
        var time = d.toLocaleTimeString().toLowerCase();
        return date + ' at ' + time;
    },
    isValidEmail: function(mail) 
    {
        if (/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/.test(mail)) {
            return true;
        }
        return false;
    },
    showWaiting:function() {
        jQuery('#modal-waiting').modal('show', { backdrop: 'static', keyboard: false });
    },
    hideWaiting:function() {
        jQuery('#modal-waiting').modal('hide');
    },
    getQueryStringValue: function(name, url) {
        if (!url) url = window.location.href;
        name = name.replace(/[\[\]]/g, "\\$&");
        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';
        return decodeURIComponent(results[2].replace(/\+/g, " "));
    }
}

var ShoppingCart = {
    _id: null,
    _functionReloadList:null,
    init: function () {
        __systemHandles["ShoppingCart"] = function(jsonData) {
            if (jsonData.Id == ShoppingCart._id) {
                switch(jsonData.ActionType) {
                    case "PreCalculateShoppingCart":
                        if (jsonData.MessageBox != '') {
                            toastr.info(jsonData.Message);
                        }
                        
                        break;
                    case "RefreshShoppingCart":
                        ShoppingCart.refresh();
                        if (ShoppingCart._functionReloadList) {
                            ShoppingCart._functionReloadList();
                        }
                        break;
                }
            }
        };
        ShoppingCart.refresh();
    }
    ,
    refresh:function() {
        $.post('/ShoppingCart/GetMiniCart', {})
            .done(function (data) {
                if (data.Ok) {
                    ShoppingCart._id = data.Data.Id;

                    $('#shopping-cart-mini').show();
                    $('#shopping-cart-mini-quantity').text(data.Data.TotalQuantity);

                    $('.current-cat-quanity').text(data.Data.TotalQuantity);

                    //Cookies.set("shoppingcartid", data.Data.Id);
                } else {
                    $('#shopping-cart-mini').hide();
                }
            }).fail(function () {
                $('#shopping-cart-mini').hide();
            });
    }
    , addProduct: function (productId, quantity, title,callSuccessCallBack) {
        var cfr = confirm('Do you want to ADD product: '+title);
        if (!cfr) return;
        var data= {
            productId:productId,
            quantity:quantity
        }
        $.post('/ShoppingCart/AddProductToShoppingCart', data)
            .done(function (data) {
                if (data.Ok) {
                    $('#shopping-cart-mini').show();
                    $('#shopping-cart-mini-quantity').text(data.Data.TotalQuantity);
                    toastr.info(data.Message);
                    //ShoppingCart.refresh();
                    if (callSuccessCallBack) {
                        callSuccessCallBack();
                    }
                } else {
                    $('#shopping-cart-mini').hide();
                }
            }).fail(function () {
                $('#shopping-cart-mini').hide();
            });
    }
    , removeProduct: function (productId, quantity, title, callSuccessCallBack) {
        var cfr = confirm('Do you want to REMOVE product: ' + title);
        if (!cfr) return;
        var data = {
            productId: productId,
            quantity: quantity
        }
        $.post('/ShoppingCart/RemoveProductFromShoppingCart', data)
            .done(function (data) {
                if (data.Ok) {
                    $('#shopping-cart-mini').show();
                    $('#shopping-cart-mini-quantity').text(data.Data.TotalQuantity);
                    //ShoppingCart.refresh();
                    toastr.info(data.Message);
                    if (callSuccessCallBack) {
                        callSuccessCallBack();
                    }
                } else {
                    $('#shopping-cart-mini').hide();
                }
            }).fail(function () {
                $('#shopping-cart-mini').hide();
            });
    }
    , removeAllProduct: function (callSuccessCallBack) {
        var cfr = confirm('Do you want to REMOVE ALL');
        if (!cfr) return;
        var data = {
        
        }
        $.post('/ShoppingCart/RemoveAllProductFromShoppingCart', data)
            .done(function (data) {
                if (data.Ok) {
                    $('#shopping-cart-mini').show();
                    $('#shopping-cart-mini-quantity').text(data.Data.TotalQuantity);
                    //ShoppingCart.refresh();
                    toastr.info(data.Message);
                    if (callSuccessCallBack) {
                        callSuccessCallBack();
                    }
                } else {
                    $('#shopping-cart-mini').hide();
                }
            }).fail(function () {
                $('#shopping-cart-mini').hide();
            });
    },
    gotoCart:function() {
        var data = {

        }

        Util.showWaiting();

        $.post('/ShoppingCart/PreCalculate', data)
            .done(function (data) {
                if (data.Ok) {
                    window.location = '/ShoppingCart/Index';
                } else {
                    Util.hideWaiting();
                    alert('Error. Can not proccess');
                }
            }).fail(function () {
                Util.hideWaiting();
                alert('Error. Can not proccess');
            });
    },
    createNew:function() {
        var cfr = confirm('Do you want to CREATE new shopping cart?');
        if (!cfr) return;
        Util.showWaiting();
        var data = {

        }
        $.post('/ShoppingCart/CreateNewShoppingCart', data)
            .done(function (data) {
                if (data.Ok) {
                  
                    ShoppingCart.refresh();
                    window.location.reload(true);
                } else {
                    $('#shopping-cart-mini').hide();
                }
            }).fail(function () {
                $('#shopping-cart-mini').hide();
            });
    }
}
