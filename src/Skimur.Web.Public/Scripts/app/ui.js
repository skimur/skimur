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
            title: title,
            type: type,
            showCancelButton: true,
            confirmButtonClass: "btn-" + type,
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

    var buildReportForm = function() {
        var $form = $("<div class='report-form'>" +
                "<div class='types'>" +
                    "<div class='btn-group-vertical'>" +
                        "<label class='type spam btn btn-default selected'>" +
                            "<input type='radio' name='type' value='0' checked='checked'> Spam" +
                        "</label>" +
                        "<label class='type vote-manipulation btn btn-default'>" +
                            "<input type='radio' name='type' value='1'> Vote manipulation" +
                        "</label>" +
                        "<label class='type personal-info btn btn-default'>" +
                            "<input type='radio' name='type' value='2'> Personal information" +
                        "</label>" +
                        "<label class='type sexualizing-minors btn btn-default'>" +
                            "<input type='radio' name='type' value='3'> Sexualizing minors" +
                        "</label>" +
                        "<label class='type breaking-skimur btn btn-default'>" +
                            "<input type='radio' name='type' value='4'> Breaking Skimur" +
                        "</label>" +
                        "<label class='type other btn btn-default'>" +
                            "<input type='radio' name='type' value='5'> Other (max 200 characters)" +
                            "<input class='form-control' type='text' disabled />" +
                        "</label>" +
                    "</div>" +
                "</div>" +
                "<div class='buttons'>" +
                    "<a href='javascript:void(0);' class='btn btn-primary report'>Report</a>&nbsp;&nbsp;&nbsp;" +
                    "<a href='javascript:void(0);' class='btn btn-default cancel'>Cancel</a>" +
                "</div>" +
            "</div>");

        $(".type", $form).click(function (e) {
          
            // enable/disable the 'other' box
            if ($(this).hasClass("other")) {
                $form.find(".other .form-control").prop("disabled", false).focus();
            } else {
                $form.find(".other .form-control").prop("disabled", true);
            }

            $(".type.selected", $form).removeClass("selected");
            $(this).addClass("selected");
            $("input", this).prop("checked", true);
        });

        return $form;
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
        confirmDelete: confirmDelete,
        buildReportForm: buildReportForm
    };

})();