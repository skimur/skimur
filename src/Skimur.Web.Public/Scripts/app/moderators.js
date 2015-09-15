; skimurui.moderators = (function () {

    var getModerator = function (element) {
        return $(element).closest(".moderator");
    };

    var changePermissions = function (element) {

        var $moderator = getModerator(element);

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

        var subName = $moderator.data("sub-name");
        var userId = $moderator.data("user-id");
        var currentPermissions = $moderator.data("permissions");

        var $content = $(".modal-body", $permissionsModal).empty();
        var $footer = $(".modal-footer", $permissionsModal).empty();

        var $form = skimurui.buildModPermissionsForm(currentPermissions);
        $form.appendTo($content);

        $("<button type='button' class='btn btn-default'>Cancel</button>").click(function(e) {
            e.preventDefault();
            $permissionsModal.modal("hide");
        }).appendTo($footer);
        $("<button type='button' class='btn btn-primary'>Save</button>").click(function(e) {
            e.preventDefault();
            skimur.changeModPermissionsForSub(subName, null, userId, skimurui.getModPermissionsFromForm($form), function(result) {
                $permissionsModal.modal("hide");
                if (!result.success) {
                    skimurui.displayError(result.error);
                }
            });
        }).appendTo($footer);

        $permissionsModal.modal("show");
    };

    var removeMod = function (element) {
        var $moderator = getModerator(element);
        skimurui.confirmWarning("Are you sure?", "Yes, remove mod!", function (confirmResult) {
            if (confirmResult.confirmed) {
                skimur.removeModFromSub($moderator.data("sub-name"), null, $moderator.data("user-id"), function (result) {
                    if (result.success) {
                        $moderator.remove();
                    } else {
                        skimurui.displayError(result.error);
                    }
                });
            }
        });
    };

    return {
        changePermissions: changePermissions,
        removeMod: removeMod
    };

})();