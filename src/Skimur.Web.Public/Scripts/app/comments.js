; skimurui.comments = (function () {

    var getComment = function (element) {
        return $(element).closest(".comment");
    };

    var cancel = function (element) {

        var $comment = getComment(element);

        // hide any content that may be staged (editing/banning/etc).
        var $staging = $comment.find("> .comment-body .comment-staging").addClass("hidden").empty();

        return {
            comment: $comment,
            staging: $staging
        }
    };

    var voteUp = function (element) {

        if (!skimurui.login.checkLoggedIn("You must be logged in to vote."))
            return;

        var $comment = getComment(element);
        var $voting = $("> .comment-body .comment-voting", $comment);

        // the user wants to upvote a post!
        if ($voting.hasClass("vote-processing")) return;
        if ($voting.hasClass("voted-up")) {
            // the user already upvoted it, let's just remove the vote
            $voting.addClass("vote-processing");
            skimur.unvoteComment($comment.data("comment-id"), function (result) {
                $voting.removeClass("vote-processing");
                if (result.success) {
                    var votes = $(".votes", $voting);
                    votes.html(+votes.html() - 1);
                    $voting.removeClass("voted-up").removeClass("voted-down");
                } else {
                    skimur.displayError(result.error);
                }
            });
        } else {
            // the user hasn't casted an upvote, so lets add it
            $voting.addClass("vote-processing");
            skimur.upvoteComment($comment.data("comment-id"), function (result) {
                $voting.removeClass("vote-processing");
                if (result.success) {
                    var votes = $(".votes", $voting);
                    votes.html(+votes.html() + 1 + ($voting.hasClass("voted-down") ? 1 : 0));
                    $voting.addClass("voted-up").removeClass("voted-down");
                } else {
                    skimur.displayError(result.error);
                }
            });
        }
    };

    var voteDown = function (element) {

        if (!skimurui.login.checkLoggedIn("You must be logged in to vote."))
            return;

        var $comment = getComment(element);
        var $voting = $("> .comment-body .comment-voting", $comment);

        // the user wants to downvote a post!
        if ($voting.hasClass("vote-processing")) return;
        if ($voting.hasClass("voted-down")) {
            // the user already downvoted it, let's just remove the vote
            $voting.addClass("vote-processing");
            skimur.unvoteComment($comment.data("comment-id"), function (result) {
                $voting.removeClass("vote-processing");
                if (result.success) {
                    var votes = $(".votes", $voting);
                    votes.html(+votes.html() + 1);
                    $voting.removeClass("voted-up").removeClass("voted-down");
                } else {
                    skimur.displayError(result.error);
                }
            });
        } else {
            // the user hasn't casted a downvote, so lets add it
            $voting.addClass("vote-processing");
            skimur.downvoteComment($comment.data("comment-id"), function (result) {
                $voting.removeClass("vote-processing");
                if (result.success) {
                    var votes = $(".votes", $voting);
                    votes.html(+votes.html() - 1 - ($voting.hasClass("voted-up") ? 1 : 0));
                    $voting.removeClass("voted-up").addClass("voted-down");
                } else {
                    skimur.displayError(result.error);
                }
            });
        }
    };

    var startReply = function (element) {

        if (!skimurui.login.checkLoggedIn("You must be logged in to comment."))
            return;

        var comment = cancel(element);
        var $textArea = $("<textarea />").appendTo(comment.staging);

        $textArea.markdown({ iconlibrary: "fa", width: "form-group" }).addClass("form-control");

        var $buttonsContainer = $("<div />").appendTo(comment.staging);

        $("<a href='javascript:void(0);' class='btn btn-primary'>Save</a>")
            .appendTo($buttonsContainer)
            .click(function (e) {
                e.preventDefault();
                skimur.createComment(comment.comment.data("post-id"), comment.comment.data("comment-id"), $textArea.val(), function (result) {
                    cancel(element);
                    if (result.success) {
                        $(result.html).insertAfter($("> .comment-body", comment.comment));
                    } else {
                        skimur.displayError(result.error);
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

        comment.staging.removeClass("hidden");
        $textArea.focus();
    };

    var startEdit = function (element) {
        var comment = cancel(element);
        var $textArea = $("<textarea />")
            .appendTo(comment.staging)
            .val(comment.comment.find("> .comment-body .comment-md-unformatted").val());

        $textArea.markdown({ iconlibrary: "fa", width: "form-group" });

        var $buttonsContainer = $("<div />").appendTo(comment.staging);

        $("<a href='javascript:void(0);' class='btn btn-primary'>Save</a>")
            .appendTo($buttonsContainer)
            .click(function (e) {
                e.preventDefault();
                skimur.editComment(comment.comment.data("comment-id"), $textArea.val(), function (result) {
                    cancel(element);
                    if (result.success) {
                        comment.comment.find("> .comment-body").replaceWith($(result.html));
                    } else {
                        skimur.displayError(result.error);
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

        comment.staging.removeClass("hidden");
        $textArea.focus();
    };

    var toggleExpand = function (element) {
        var $comment = getComment(element);
        if ($comment.hasClass("collapsed")) {
            $comment.removeClass("collapsed");
            $(element).html("[–]");
        } else {
            $comment.addClass("collapsed");
            $(element).html("[+]");
        }
    };

    var deleteComment = function (element) {
        var $comment = getComment(element);

        skimur.confirmDelete(function (result) {
            if (result.confirmed) {
                skimur.deleteComment($comment.data("comment-id"), null, function (deleteResult) {
                    if (deleteResult.success) {
                        $("> .comment-body", $comment).find(".delete, .reply, .edit").remove();
                    } else {
                        skimur.displayError(deleteResult.error);
                    }
                });
            }
        });
    };

    var moreChildren = function (element, postId, sort, children, depth) {
        var $comment = getComment(element);
        skimur.moreComments(postId, sort, children, depth, function(result) {
            $comment.after($(result.html)).remove();
        });
    }

    return {
        voteUp: voteUp,
        voteDown: voteDown,
        startReply: startReply,
        startEdit: startEdit,
        toggleExpand: toggleExpand,
        delete: deleteComment,
        moreChildren: moreChildren
    };

})();

$(function () {

    $.fn.comment = function () {
        return this.each(function () {

        });
    };

    $(".comment").comment();

});