﻿(function ($) {

    $(".shoppingcart a.icon.delete").live("click", function (e) {
        e.preventDefault();

        var shoppingCartItem = ko.dataFor(this);

        if (shoppingCartItem != null) {
            shoppingCartItem.remove();
            return;
        }

        var $button = $(this);
        var $tr = $button.parents("tr:first");
        var $isRemoved = $("input[name$='IsRemoved']", $tr).val("true");
        var $form = $button.parents("form");

        $form.submit();

    });

    var ShoppingCartItem = function (data) {

        this.id = data.id;
        this.title = data.title;
        this.unitPrice = data.unitPrice;
        this.quantity = ko.observable(data.quantity);

        this.total = ko.dependentObservable(function () {
            return this.unitPrice * parseInt(this.quantity());
        }, this);

        this.remove = function () {
            shoppingCart.items.remove(this);
            saveChanges();
        };

        this.quantity.subscribe(function (value) {
            saveChanges();
        });

        this.index = ko.dependentObservable(function () {
            return shoppingCart.items.indexOf(this);
        }, this);
    };

    var shoppingCart = {
        items: ko.observableArray()
    };

    shoppingCart.calculateSubtotal = ko.dependentObservable(function () {
        return $.Enumerable.From(this.items()).Sum(function (x) { return x.total(); });
    }, shoppingCart);

    shoppingCart.itemCount = ko.dependentObservable(function () {
        return $.Enumerable.From(this.items()).Sum(function (x) { return parseInt(x.quantity()); });
    }, shoppingCart);

    shoppingCart.hasItems = ko.dependentObservable(function () { return this.items().length > 0; }, shoppingCart);
    shoppingCart.calculateVat = function () { return this.calculateSubtotal() * 0.19; };
    shoppingCart.calculateTotal = function () { return this.calculateSubtotal() + this.calculateVat(); };

    var saveChanges = function () {
        var data = $.Enumerable.From(shoppingCart.items()).Select(function (x) { return { productId: x.id, quantity: x.quantity() }; }).ToArray();
        var url = $("article.shoppingcart").data("update-shoppingcart-url");
        var config = {
            url: url,
            type: "POST",
            data: data ? JSON.stringify(data) : null,
            dataType: "json",
            contentType: "application/json; charset=utf-8"
        };
        $.ajax(config);
    };

    if ($("article.shoppingcart").length > 0) {
        $.ajaxSetup({ cache: false });
        ko.applyBindings(shoppingCart);
        var dataUrl = $("article.shoppingcart").data("load-shoppingcart-url");

        $("article.shoppingcart tbody").empty();

        $("button[value='Update']").hide();

        $.getJSON(dataUrl, function (data) {
            for (var i = 0; i < data.items.length; i++) {
                var item = data.items[i];
                shoppingCart.items.push(new ShoppingCartItem(item));
            }
        });
    }

})(jQuery);