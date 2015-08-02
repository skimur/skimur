; skimurui.login = (function () {

    var displayModal = function (message) {
        // TODO: display message to the user, if given
        $('#login-modal').modal();
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
        checkLoggedIn : checkLoggedIn
    };

})();