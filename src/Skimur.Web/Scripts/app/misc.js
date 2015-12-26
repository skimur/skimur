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

// helper method for disabling and enabling buttons
$.fn.buttonStartLoading = function (fnc) {
    return this.each(function () {
        $(this).addClass("loading").prop("disabled", true);
    });
}
$.fn.buttonStopLoading = function (fnc) {
    return this.each(function () {
        $(this).removeClass("loading").prop("disabled", false);
    });
}

$(function() {
    if (!skimurui.isLoggedIn) {
        $(".submit-link, .submit-post").click(function(e) {
            e.preventDefault();
            skimurui.login.display("You must be logged in to submit things.");
        });
    }

    $("#register-link").click(function(e) {
        e.preventDefault();
        skimurui.login.displayRegister();
    });

    $("#signin-link").click(function (e) {
        e.preventDefault();
        skimurui.login.displayLogin();
    });
});