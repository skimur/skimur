; skimurui.messages = (function () {

    var getMessage = function (element) {
        return $(element).closest(".message");
    };

    var cancel = function (element) {

        var $message = getMessage(element);

        // hide any content that may be staged.
        var $staging = $message.find("> .disc .disc-body .disc-staging").addClass("hidden").empty();

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

    var markAsRead = function(element) {
        var $message = getMessage(element);
        if ($message.hasClass("message-read")) return;

        var messages = [];
        messages.push($message.data("message-id"));

        skimur.markMessagesAsRead(messages, function (result) {
            if (result.success) {
                $message.removeClass("message-unread").addClass("message-read");
            } else {
                skimurui.displayError(result.error);
            }
        });
    }

    var markAsUnread = function(element) {
        var $message = getMessage(element);
        if ($message.hasClass("message-unread")) return;

        var messages = [];
        messages.push($message.data("message-id"));

        skimur.markMessagesAsUnread(messages, function (result) {
            if (result.success) {
                $message.removeClass("message-read").addClass("message-unread");
            } else {
                skimurui.displayError(result.error);
            }
        });
    }

    var messageClicked = function(element) {
        var $message = getMessage(element);
        if ($message.hasClass("message-unread")) {
            markAsRead(element);
        }
    };

    return {
        toggleExpand: toggleExpand,
        startReply: startReply,
        markAsRead : markAsRead,
        markAsUnread: markAsUnread,
        messageClicked: messageClicked
    };

})();