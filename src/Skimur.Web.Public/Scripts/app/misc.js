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

$(function() {
    if (!skimurui.isLoggedIn) {
        $(".submit-link, .submit-post").click(function(e) {
            e.preventDefault();
            skimurui.login.display("You must be logged in to submit things.");
        });
    }
});