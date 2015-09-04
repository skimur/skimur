; skimurui = (function () {

    var displayError = function (message) {
        $.notify(message, {
            type: "danger",
            placement: {
                align: "center"
            }
        });
    };

    var displaySuccess = function (message) {
        $.notify(message, {
            type: "success",
            placement: {
                align: "center"
            }
        });
    };

    var popupError = function (title, message) {
        swal({
            title: title,
            text: message,
            type: "error",
            confirmButtonText: "Ok"
        });
    };

    var popupSuccess = function (title, message) {
        swal({
            title: title,
            text: message,
            type: "success",
            confirmButtonText: "Ok"
        });
    };

    var confirm = function (type, title, buttonText, callback) {
        swal({
            title: "Are you sure?",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: buttonText,
            cancelButtonText: "Cancel",
            closeOnConfirm: true,
            closeOnCancel: true
        },
        function (isConfirm) {
            callback({
                confirmed: isConfirm
            });
        });
    }

    var confirmError = function (title, buttonText, callback) {
        confirm("error", title, buttonText, callback);
    }

    var confirmWarning = function (title, buttonText, callback) {
        confirm("warning", title, buttonText, callback);
    }

    var confirmInfo = function (title, buttonText, callback) {
        confirm("info", title, buttonText, callback);
    }

    var confirmSuccess = function (title, buttonText, callback) {
        confirm("success", title, buttonText, callback);
    }

    var confirmDelete = function (callback) {
        confirmWarning("Are you sure?", "Yes, delete it!", callback);
    }

    return {
        displayError: displayError,
        displaySuccess: displaySuccess,
        popupError: popupError,
        popupSuccess: popupSuccess,
        confirm: confirm,
        confirmError: confirmError,
        confirmWarning: confirmWarning,
        confirmInfo: confirmInfo,
        confirmSuccess: confirmSuccess,
        confirmDelete: confirmDelete
    };

})();