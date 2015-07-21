; var skimurui = (function () {

    var toggleSubscription = function (button, subName) {

        var $button = $(button);

        if ($button.hasClass("processing"))
            return;

        if ($button.hasClass("subscribed")) {
            $button.addClass("disabled");
            skimur.unsubcribe(subName, function (data) {
                $button.removeClass("disabled");
                if (data.success) {
                    $button.removeClass("subscribed").addClass("unsubscribed");
                    $button.html("unsubscribed");
                }
            });
        } else if ($button.hasClass("unsubscribed")) {
            $button.addClass("disabled");
            skimur.subscribe(subName, function (data) {
                $button.removeClass("disabled");
                if (data.success) {
                    $button.removeClass("unsubscribed").addClass("subscribed");
                    $button.html("subscribed");
                }
            });
        }
    };

    return {
        toggleSubscription: toggleSubscription
    };

})();

$(function () {

    // the post up/down voting logic
    $(".post").each(function (index, element) {

        var $postVoting = $(".post-voting", element);
        var postSlug = $(element).data("postslug");

        $(".up", $postVoting).click(function () {
            // the user wants to upvote a post!
            if ($postVoting.hasClass("vote-processing")) return;
            if ($postVoting.hasClass("voted-up")) {
                // the user already upvoted it, let's just remove the vote
                $postVoting.addClass("vote-processing");
                skimur.unvotePost(postSlug, function (result) {
                    $postVoting.removeClass("vote-processing");
                    if (result.success) {
                        var votes = $(".votes", $postVoting);
                        votes.html(+votes.html() - 1);
                        $postVoting.removeClass("voted-up").removeClass("voted-down");
                    }
                });
            } else {
                // the user hasn't casted an upvote, so lets add it
                $postVoting.addClass("vote-processing");
                skimur.upvotePost(postSlug, function (result) {
                    $postVoting.removeClass("vote-processing");
                    if (result.success) {
                        var votes = $(".votes", $postVoting);
                        votes.html(+votes.html() + 1 + ($postVoting.hasClass("voted-down") ? 1 : 0));
                        $postVoting.addClass("voted-up").removeClass("voted-down");
                    }
                });
            }
        });

        $(".down", $postVoting).click(function () {
            // the user wants to downvote a post!
            if ($postVoting.hasClass("vote-processing")) return;
            if ($postVoting.hasClass("voted-down")) {
                // the user already downvoted it, let's just remove the vote
                $postVoting.addClass("vote-processing");
                skimur.unvotePost(postSlug, function (result) {
                    $postVoting.removeClass("vote-processing");
                    if (result.success) {
                        var votes = $(".votes", $postVoting);
                        votes.html(+votes.html() + 1);
                        $postVoting.removeClass("voted-up").removeClass("voted-down");
                    }
                });
            } else {
                // the user hasn't casted a downvote, so lets add it
                $postVoting.addClass("vote-processing");
                skimur.downvotePost(postSlug, function (result) {
                    $postVoting.removeClass("vote-processing");
                    if (result.success) {
                        var votes = $(".votes", $postVoting);
                        votes.html(+votes.html() - 1 - ($postVoting.hasClass("voted-up") ? 1 : 0));
                        $postVoting.removeClass("voted-up").addClass("voted-down");
                    }
                });
            }
        });
    });

    // the comment creating
    $.fn.comment = function () {

        return this.each(function () {
            var $comment = $(this);

            var cancel = function () {
                return $comment.find("> .comment-body .comment-staging").addClass("hidden").empty();
            };

            var startReply = function() {
                var $staging = cancel();
                var $textArea = $("<textarea />").appendTo($staging);

                $textArea.markdown({ iconlibrary: "fa" });

                var $buttonsContainer = $("<div class='form-group' />").appendTo($staging);

                $("<a href='javascript:void(0);' class='btn btn-primary'>Save</a>")
                    .appendTo($buttonsContainer)
                    .click(function(e) {
                        e.preventDefault();
                        skimur.createComment($comment.data("post-slug"), $comment.data("comment-id"), $textArea.val(), function(result) {
                            alert(result.success);
                            cancel();
                        });
                    });

                $("<a href='javascript:void(0);' class='btn btn-default'>Cancel</a>")
                    .appendTo($buttonsContainer)
                    .click(function(e) {
                        e.preventDefault();
                        cancel();
                    });

                $staging.removeClass("hidden");
                $textArea.focus();
            };

            var toggleExpand = function($toggleButton) {
                if ($comment.hasClass("collapsed")) {
                    $comment.removeClass("collapsed");
                    $toggleButton.html("[–]");
                } else {
                    $comment.addClass("collapsed");
                    $toggleButton.html("[+]");
                }
            };

            var startEdit = function() {
                var $staging = cancel();
                var $textArea = $("<textarea />")
                    .appendTo($staging)
                    .val($comment.find("> .comment-body .comment-md-unformatted").val());

                $textArea.markdown({ iconlibrary: "fa" });

                var $buttonsContainer = $("<div class='form-group' />").appendTo($staging);

                $("<a href='javascript:void(0);' class='btn btn-primary'>Save</a>")
                    .appendTo($buttonsContainer)
                    .click(function (e) {
                        e.preventDefault();
                        skimur.editComment($comment.data("comment-id"), $textArea.val(), function (result) {
                            alert(result.success);
                            cancel();
                        });
                    });

                $("<a href='javascript:void(0);' class='btn btn-default'>Cancel</a>")
                    .appendTo($buttonsContainer)
                    .click(function (e) {
                        e.preventDefault();
                        cancel();
                    });

                $staging.removeClass("hidden");
                $textArea.focus();
            };

            $("> .comment-body .reply", $comment).click(function (e) {
                e.preventDefault();
                startReply();
            });

            $("> .comment-body .edit", $comment).click(function (e) {
                e.preventDefault();
                startEdit();
            });

            $("> .comment-body .expand", $comment).click(function (e) {
                e.preventDefault();
                toggleExpand($(this));
            });

        });

    };

    $(".comment").comment();

});