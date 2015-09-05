; skimurui.subs = (function () {

    var toggleSubscription = function (button, subName) {

        if (!skimurui.login.checkLoggedIn("You must be logged in to subscribe."))
            return;

        var $button = $(button);

        if ($button.hasClass("processing"))
            return;

        if ($button.hasClass("subscribed")) {
            $button.addClass("disabled");
            skimur.unsubcribe(subName, function (result) {
                $button.removeClass("disabled");
                if (result.success) {
                    $button.removeClass("subscribed").addClass("unsubscribed");
                    $button.html("unsubscribed");
                } else {
                    skimurui.displayError(result.error);
                }
            });
        } else if ($button.hasClass("unsubscribed")) {
            $button.addClass("disabled");
            skimur.subscribe(subName, function (result) {
                $button.removeClass("disabled");
                if (result.success) {
                    $button.removeClass("unsubscribed").addClass("subscribed");
                    $button.html("subscribed");
                } else {
                    skimurui.displayError(result.error);
                }
            });
        }
    };

    return {
        toggleSubscription: toggleSubscription
    };

})();