; skimurui = (function () {

    var permissionsFlagAll = 1;
    var permissionsFlagAccess = 2;
    var permissionsFlagConfig = 4;
    var permissionsFlagFlair = 8;
    var permissionsFlagMail = 16;
    var permissionsFlagPosts = 32;

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

    var buildReportForm = function () {
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

    var buildModeratorPermissionsString = function (permissions) {
        if ((permissions & permissionsFlagAll) !== 0) {
            return "Full";
        } else if (permissions === 0) {
            return "None";
        } else {
            var list = [];
            if ((permissions & permissionsFlagAccess) !== 0) {
                list.push("Access");
            }
            if ((permissions & permissionsFlagConfig) !== 0) {
                list.push("Config");
            }
            if ((permissions & permissionsFlagFlair) !== 0) {
                list.push("Flair");
            }
            if ((permissions & permissionsFlagMail) !== 0) {
                list.push("Mail");
            }
            if ((permissions & permissionsFlagPosts) !== 0) {
                list.push("Posts");
            }
            return list.join(", ");
        }
    }

    var buildModPermissionsForm = function (currentPermissions) {
        var $form = $(
        "<div class='permissions'>" +
            "<label class='permission full'>" +
                "<input type='checkbox' name='full'> Full" +
            "</label>" +
            "<hr />" +
            "<label class='permission access'>" +
                "<input type='checkbox' name='access'> Access" +
            "</label>" +
            "<label class='permission config'>" +
                "<input type='checkbox' name='config'> Config" +
            "</label>" +
            "<label class='permission flair'>" +
                "<input type='checkbox' name='flair'> Flair" +
            "</label>" +
            "<label class='permission mail'>" +
                "<input type='checkbox' name='mail'> Mail" +
            "</label>" +
            "<label class='permission posts'>" +
                "<input type='checkbox' name='posts'> Posts" +
            "</label>" +
        "</div>");

        $(".permission input", $form).click(function (e) {
            var isChecked = $(this).prop("checked");
            var $permission = $(this).closest(".permission");
            if ($permission.hasClass("full")) {
                var items = $(".access, .config, .flair, .mail, .posts", $form).each(function (index, element) {
                    var $element = $(element);
                    if (isChecked) {
                        $element.addClass("disabled").find("input").attr("disabled", true).attr("checked", false);
                    } else {
                        $element.removeClass("disabled").find("input").attr("disabled", false).attr("checked", false);
                    }
                });
                if (isChecked) {
                    items.addClass("disabled");
                } else {
                    items.removeClass("disabled");
                }
            }
        });

        if (currentPermissions !== -1) {
            // and check our current permissions
            if ((currentPermissions & permissionsFlagAll) !== 0) {
                $(".permission.full input", $form).click();
            } else if (currentPermissions === 0) {
                // leave the form alone!
            } else {
                if ((currentPermissions & permissionsFlagAccess) !== 0) {
                    $(".permission.access input", $form).click();
                }
                if ((currentPermissions & permissionsFlagConfig) !== 0) {
                    $(".permission.config input", $form).click();
                }
                if ((currentPermissions & permissionsFlagFlair) !== 0) {
                    $(".permission.flair input", $form).click();
                }
                if ((currentPermissions & permissionsFlagMail) !== 0) {
                    $(".permission.mail input", $form).click();
                }
                if ((currentPermissions & permissionsFlagPosts) !== 0) {
                    $(".permission.posts input", $form).click();
                }
            }
        }

        return $form;
    };

    var getModPermissionsFromForm = function ($form) {
        if ($(".permission.full input", $form).is(":checked")) {
            return permissionsFlagAll;
        } else {
            var permissions = 0;
            if ($(".permission.access input", $form).is(":checked")) {
                permissions |= permissionsFlagAccess;
            }
            if ($(".permission.config input", $form).is(":checked")) {
                permissions |= permissionsFlagConfig;
            }
            if ($(".permission.flair input", $form).is(":checked")) {
                permissions |= permissionsFlagFlair;
            }
            if ($(".permission.mail input", $form).is(":checked")) {
                permissions |= permissionsFlagMail;
            }
            if ($(".permission.posts input", $form).is(":checked")) {
                permissions |= permissionsFlagPosts;
            }
            return permissions;
        }
    };

    var displayChangePermissionsForm = function (value, callback) {

        var $permissionsModal = $("#permissions-modal");

        if ($permissionsModal.length === 0) {
            $permissionsModal = $("<div id='permissions-modal' class='modal fade permission-modal'>" +
                "<div class='modal-dialog'>" +
                        "<div class='modal-content'>" +
                            "<div class='modal-body'>" +
                            "</div>" +
                            "<div class='modal-footer'>" +
                            "</div>" +
                        "</div>" +
                    "</div>" +
                "</div>");
            $permissionsModal.appendTo($("body"));
            $permissionsModal.modal({ show: false });
        }

        var $content = $(".modal-body", $permissionsModal).empty();
        var $footer = $(".modal-footer", $permissionsModal).empty();

        var $form = skimurui.buildModPermissionsForm(value);
        $form.appendTo($content);

        $("<button type='button' class='btn btn-default'>Cancel</button>").click(function (e) {
            e.preventDefault();
            $permissionsModal.modal("hide");
        }).appendTo($footer);
        $("<button type='button' class='btn btn-primary'>Save</button>").click(function (e) {
            e.preventDefault();
            $permissionsModal.modal("hide");
            callback(skimurui.getModPermissionsFromForm($form));
        }).appendTo($footer);

        $permissionsModal.modal("show");
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
        buildReportForm: buildReportForm,
        buildModPermissionsForm: buildModPermissionsForm,
        getModPermissionsFromForm: getModPermissionsFromForm,
        buildModeratorPermissionsString: buildModeratorPermissionsString,
        displayChangePermissionsForm: displayChangePermissionsForm
    };

})();

$.fn.markdown.defaults.parser = function (val) {
    return markedHelper.compile(val).result;
};