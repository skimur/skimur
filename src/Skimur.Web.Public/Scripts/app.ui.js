; var skimurui = (function () {

    var toggleSubscription = function (button, subName) {

        var $button = $(button);

        if ($button.hasClass("processing"))
            return;

        if ($button.hasClass("subscribed")) {
            $button.addClass("disabled");
            skimur.unsubcribe(subName, function (result) {
                $button.removeClass("disabled");
                if (result.success) {
                    $button.removeClass("subscribed").addClass("unsubscribed");
                    $button.html("unsubscribed");
                } else {
                    skimur.displayError(result.error);
                }
            });
        } else if ($button.hasClass("unsubscribed")) {
            $button.addClass("disabled");
            skimur.subscribe(subName, function (result) {
                $button.removeClass("disabled");
                if (result.success) {
                    $button.removeClass("unsubscribed").addClass("subscribed");
                    $button.html("subscribed");
                } else {
                    skimur.displayError(result.error);
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
                    } else {
                        skimur.displayError(result.error);
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
                    } else {
                        skimur.displayError(result.error);
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
                    } else {
                        skimur.displayError(result.error);
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
                    } else {
                        skimur.displayError(result.error);
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

            var startReply = function () {
                var $staging = cancel();
                var $textArea = $("<textarea />").appendTo($staging);

                $textArea.markdown({ iconlibrary: "fa", width: "form-group" }).addClass("form-control");

                var $buttonsContainer = $("<div />").appendTo($staging);

                $("<a href='javascript:void(0);' class='btn btn-primary'>Save</a>")
                    .appendTo($buttonsContainer)
                    .click(function (e) {
                        e.preventDefault();
                        skimur.createComment($comment.data("post-slug"), $comment.data("comment-id"), $textArea.val(), function (result) {
                            cancel();
                            if (result.success) {
                                var $newComment = $.buildComment(result);
                                $(".comment-voting", $newComment).addClass("voted-up");
                                $newComment.insertAfter($("> .comment-body", $comment));
                                $newComment.comment();
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
                        cancel();
                    });

                $staging.removeClass("hidden");
                $textArea.focus();
            };

            var toggleExpand = function ($toggleButton) {
                if ($comment.hasClass("collapsed")) {
                    $comment.removeClass("collapsed");
                    $toggleButton.html("[–]");
                } else {
                    $comment.addClass("collapsed");
                    $toggleButton.html("[+]");
                }
            };

            var startEdit = function () {
                var $staging = cancel();
                var $textArea = $("<textarea />")
                    .appendTo($staging)
                    .val($comment.find("> .comment-body .comment-md-unformatted").val());

                $textArea.markdown({ iconlibrary: "fa", width: "form-group" });

                var $buttonsContainer = $("<div />").appendTo($staging);

                $("<a href='javascript:void(0);' class='btn btn-primary'>Save</a>")
                    .appendTo($buttonsContainer)
                    .click(function (e) {
                        e.preventDefault();
                        skimur.editComment($comment.data("comment-id"), $textArea.val(), function (result) {
                            cancel();
                            if (result.success) {
                                $comment.find("> .comment-body .comment-md-unformatted").val(result.body);
                                $comment.find("> .comment-body .comment-md").html(result.bodyFormatted);
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
                        cancel();
                    });

                $staging.removeClass("hidden");
                $textArea.focus();
            };

            var voteUp = function () {

                var $voting = $("> .comment-voting", $comment);

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

            var voteDown = function () {

                var $voting = $("> .comment-voting", $comment);

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

            var $commentVotes = $("> .comment-voting", $comment);

            $(".up", $commentVotes).click(function (e) {
                e.preventDefault();
                voteUp();
            });

            $(".down", $commentVotes).click(function (e) {
                e.preventDefault();
                voteDown();
            });

        });

    };

    $.buildComment = function (comment) {
        var $comment = $(
        "<div class='comment' data-post-slug='" + comment.postSlug + "' data-comment-id='" + comment.commentId + "'>" +
            "<div class='comment-voting'>" +
                "<span class='up'></span>" +
                "<span class='down'></span>" +
            "</div>" +
            "<div class='comment-body'>" +
                "<div class='comment-tagline'>" +
                    "<a href='javascript:void(0)' class='expand'>[–]</a> <a href='/u/" + comment.author + "' class='author'>" + comment.author + "</a> <span class='score'>" + comment.score + " points</span> <time class='timestamp'>" + comment.dateCreatedAgo + "</time>" +
                "</div>" +
                "<div class='comment-md'>" +
                comment.bodyFormatted +
                "</div>" +
                "<textarea class='comment-md-unformatted hidden'>" + comment.body + "</textarea>" +
                       "<ul class='comment-options'>" +
                           "<li class='first'>" +
                               "<a href='javascript:void(0);' class='reply'>reply</a>" +
                           "</li>" +
                           "<li>" +
                               "<a href='javascript:void(0);' class='edit'>edit</a>" +
                           "</li>" +
                       "</ul>" +
                       "<div class='comment-staging hidden'></div>" +
                   "</div>" +
                "<div class='clearfix'></div>" +
            "</div>" +
        "</div>"
        );
        return $comment;
    };

    $(".comment").comment();

});