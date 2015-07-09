; var skimur = (function () {

    var subscribe = function (subName, callback) {
        $.ajax({
            type: "POST",
            url: "/subscribe",
            data: { subName: subName },
            dataType: "json",
            success: function (data) {
                if (callback)
                    callback(data);
            },
            error: function () {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    var unsubcribe = function (subName, callback) {
        $.ajax({
            type: "POST",
            url: "/unsubscribe",
            data: { subName: subName },
            dataType: "json",
            success: function (data) {
                if (callback)
                    callback(data);
            },
            error: function () {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    return {
        subscribe: subscribe,
        unsubcribe: unsubcribe
    };

})();

// helper method for capturing the enter key
$.fn.enterKey = function (fnc) {
    return this.each(function () {
        $(this).keypress(function (ev) {
            var keycode = (ev.keyCode ? ev.keyCode : ev.which);
            if (keycode == '13') {
                fnc.call(this, ev);
            }
        });
    });
}