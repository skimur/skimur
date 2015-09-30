; skimurui.login = (function () {

    var displayModal = function (message, register) {
        // TODO: display message to the user, if given
        $("#login-modal").on("shown.bs.modal", function() {
            $("#login-modal").find("input[type=text],textarea,select").filter(":visible:first").focus();
        }).modal();
        if (register) {
            $("#login-modal a[href='#register-tab']").tab("show");
        } else {
            $("#login-modal a[href='#signin-tab']").tab("show");
        }
    };

    var displayLogin = function(message) {
        displayModal(message, false);
    };

    var displayRegister = function(message) {
        displayModal(message, true);
    };

    var checkLoggedIn = function(message) {
        if (!window.skimurui.isLoggedIn) {
            displayModal("You must be logged in to vote.");
            return false;
        }
        return true;
    };

    return {
        display: displayModal,
        displayLogin: displayLogin,
        displayRegister: displayRegister,
        checkLoggedIn : checkLoggedIn
    };

})();