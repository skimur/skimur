; var skimurui = (function () {

    var toggleSubscription = function (button, subName) {

        var $button = $(button);

        if ($button.hasClass("processing"))
            return;

        if ($button.hasClass("subscribed")) {
            $button.addClass("disabled");
            skimur.unsubcribe(subName, function (data) {
                $button.removeClass("disabled");
                if (data.success) {
                    $button.removeClass("subscribed").addClass("unsubscribed");
                    $button.html("unsubscribed");
                }
            });
        } else if ($button.hasClass("unsubscribed")) {
            $button.addClass("disabled");
            skimur.subscribe(subName, function(data) {
                $button.removeClass("disabled");
                if (data.success) {
                    $button.removeClass("unsubscribed").addClass("subscribed");
                    $button.html("subscribed");
                }
            });
        }
    };

    return {
        toggleSubscription: toggleSubscription
    };

})();