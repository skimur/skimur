; skimurui.posts = (function () {

    var getPost = function (element) {
        return $(element).closest(".post");
    };

    var voteUp = function (element) {

        if (!skimurui.login.checkLoggedIn("You must be logged in to vote."))
            return;

        var $post = getPost(element);
        var $voting = $(".post-voting", $post);

        // the user wants to upvote a post!
        if ($voting.hasClass("vote-processing")) return;
        if ($voting.hasClass("voted-up")) {
            // the user already upvoted it, let's just remove the vote
            $voting.addClass("vote-processing");
            skimur.unvotePost($post.data("post-id"), function (result) {
                $voting.removeClass("vote-processing");
                if (result.success) {
                    var votes = $(".votes", $voting);
                    votes.html(+votes.html() - 1);
                    $voting.removeClass("voted-up").removeClass("voted-down");
                } else {
                    skimurui.displayError(result.error);
                }
            });
        } else {
            // the user hasn't casted an upvote, so lets add it
            $voting.addClass("vote-processing");
            skimur.upvotePost($post.data("post-id"), function (result) {
                $voting.removeClass("vote-processing");
                if (result.success) {
                    var votes = $(".votes", $voting);
                    votes.html(+votes.html() + 1 + ($voting.hasClass("voted-down") ? 1 : 0));
                    $voting.addClass("voted-up").removeClass("voted-down");
                } else {
                    skimurui.displayError(result.error);
                }
            });
        }
    };

    var voteDown = function (element) {

        if (!skimurui.login.checkLoggedIn("You must be logged in to vote."))
            return;

        var $post = getPost(element);
        var $voting = $(".post-voting", $post);

        // the user wants to downvote a post!
        if ($voting.hasClass("vote-processing")) return;
        if ($voting.hasClass("voted-down")) {
            // the user already downvoted it, let's just remove the vote
            $voting.addClass("vote-processing");
            skimur.unvotePost($post.data("post-id"), function (result) {
                $voting.removeClass("vote-processing");
                if (result.success) {
                    var votes = $(".votes", $voting);
                    votes.html(+votes.html() + 1);
                    $voting.removeClass("voted-up").removeClass("voted-down");
                } else {
                    skimurui.displayError(result.error);
                }
            });
        } else {
            // the user hasn't casted a downvote, so lets add it
            $voting.addClass("vote-processing");
            skimur.downvotePost($post.data("post-id"), function (result) {
                $voting.removeClass("vote-processing");
                if (result.success) {
                    var votes = $(".votes", $voting);
                    votes.html(+votes.html() - 1 - ($voting.hasClass("voted-up") ? 1 : 0));
                    $voting.removeClass("voted-up").addClass("voted-down");
                } else {
                    skimurui.displayError(result.error);
                }
            });
        }
    };

    var approve = function(element) {
        var $post = getPost(element);
        skimurui.confirmInfo("Are you sure?", "Yes, approve it!", function (confirmResult) {
            if (confirmResult.confirmed) {
                skimur.removePost($post.data("post-id"), function (result) {
                    if (result.success) {
                        skimurui.displaySuccess("The post has been approved.");
                    } else {
                        skimurui.displayError(result.error);
                    }
                });
            }
        });
    };

    var remove = function (element) {
        var $post = getPost(element);
        skimurui.confirmWarning("Are you sure?", "Yes, remove it!", function(confirmResult) {
            if (confirmResult.confirmed) {
                skimur.removePost($post.data("post-id"), function (result) {
                    if (result.success) {
                        skimurui.displaySuccess("The post has been removed.");
                    } else {
                        skimurui.displayError(result.error);
                    }
                });
            }
        });
    };

    return {
        voteUp: voteUp,
        voteDown: voteDown,
        approve: approve,
        remove: remove
    };

})();