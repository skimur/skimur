; skimurui.messages = (function () {

    var getMessage = function (element) {
        return $(element).closest(".message");
    };

    var cancel = function (element) {

        var $message = getMessage(element);

        // hide any content that may be staged.
        var $staging = $message.find("> .message-body .message-staging").addClass("hidden").empty();

        return {
            message: $message,
            staging: $staging
        }
    };

    var toggleExpand = function (element) {
        var $message = getMessage(element);
        if ($message.hasClass("collapsed")) {
            $message.removeClass("collapsed");
            $(element).html("[–]");
        } else {
            $message.addClass("collapsed");
            $(element).html("[+]");
        }
    };

    var startReply = function (element) {

        var message = cancel(element);
        var $textArea = $("<textarea />").appendTo(message.staging);

        $textArea.markdown({ iconlibrary: "fa", width: "form-group" }).addClass("form-control");

        var $buttonsContainer = $("<div />").appendTo(message.staging);

        $("<a href='javascript:void(0);' class='btn btn-primary'>Save</a>")
            .appendTo($buttonsContainer)
            .click(function (e) {
                e.preventDefault();
                skimur.replyToMessage(message.message.data("message-id"), $textArea.val(), function (result) {
                    cancel(element);
                    if (result.success) {
                        skimurui.displaySuccess("Reply sent.");
                    } else {
                        skimurui.displayError(result.error);
                    }
                });
            });

        $buttonsContainer.append("&nbsp;&nbsp;&nbsp;");

        $("<a href='javascript:void(0);' class='btn btn-default'>Cancel</a>")
            .appendTo($buttonsContainer)
            .click(function (e) {
                e.preventDefault();
                cancel(this);
            });

        message.staging.removeClass("hidden");
        $textArea.focus();
    };

    return {
        toggleExpand: toggleExpand,
        startReply: startReply
    };

})();