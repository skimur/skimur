; skimurui.moderators = (function () {

    var getModerator = function (element) {
        return $(element).closest(".moderator");
    };

    var changePermissions = function (element) {

        var $moderator = getModerator(element);

        var subName = $moderator.data("sub-name");
        var userId = $moderator.data("user-id");
        var currentPermissions = $moderator.data("permissions");

        if ($moderator.hasClass("invite")) {
            skimurui.displayChangePermissionsForm(currentPermissions, function (newPermissions) {
                skimur.changeModInvite(subName, null, null, userId, newPermissions, function (result) {
                    if (result.success) {
                        $(".permissions", $moderator).text(skimurui.buildModeratorPermissionsString(newPermissions));
                        $moderator.data("permissions", newPermissions);
                    } else {
                        skimurui.displayError(result.error);
                    }
                });
            });
        } else {
            skimurui.displayChangePermissionsForm(currentPermissions, function (newPermissions) {
                skimur.changeModPermissionsForSub(subName, null, userId, newPermissions, function (result) {
                    if (result.success) {
                        $(".permissions", $moderator).text(skimurui.buildModeratorPermissionsString(newPermissions));
                        $moderator.data("permissions", newPermissions);
                    } else {
                        skimurui.displayError(result.error);
                    }
                });
            });
        }
    };

    var removeMod = function (element) {
        var $moderator = getModerator(element);

        if ($moderator.hasClass("invite")) {
            skimurui.confirmWarning("Are you sure?", "Yes, remove invite!", function (confirmResult) {
                if (confirmResult.confirmed) {
                    skimur.removeModInvite($moderator.data("sub-name"), null, null, $moderator.data("user-id"), function (result) {
                        if (result.success) {
                            $moderator.remove();
                        } else {
                            skimurui.displayError(result.error);
                        }
                    });
                }
            });
        } else {
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
        }
    };

    return {
        changePermissions: changePermissions,
        removeMod: removeMod
    };

})();