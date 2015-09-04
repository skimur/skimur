//$(function() {
//    // the post up/down voting logic
//    $(".post").each(function (index, element) {

//        var $postVoting = $(".post-voting", element);
//        var postId = $(element).data("post-id");

//        $(".up", $postVoting).click(function () {
//            // the user wants to upvote a post!

//            if (!skimurui.isLoggedIn) {
//                skimurui.login.display("You must be logged in to vote.");
//                return;
//            }

//            if ($postVoting.hasClass("vote-processing")) return;
//            if ($postVoting.hasClass("voted-up")) {
//                // the user already upvoted it, let's just remove the vote
//                $postVoting.addClass("vote-processing");
//                skimur.unvotePost(postId, function (result) {
//                    $postVoting.removeClass("vote-processing");
//                    if (result.success) {
//                        var votes = $(".votes", $postVoting);
//                        votes.html(+votes.html() - 1);
//                        $postVoting.removeClass("voted-up").removeClass("voted-down");
//                    } else {
//                        skimur.displayError(result.error);
//                    }
//                });
//            } else {
//                // the user hasn't casted an upvote, so lets add it
//                $postVoting.addClass("vote-processing");
//                skimur.upvotePost(postId, function (result) {
//                    $postVoting.removeClass("vote-processing");
//                    if (result.success) {
//                        var votes = $(".votes", $postVoting);
//                        votes.html(+votes.html() + 1 + ($postVoting.hasClass("voted-down") ? 1 : 0));
//                        $postVoting.addClass("voted-up").removeClass("voted-down");
//                    } else {
//                        skimur.displayError(result.error);
//                    }
//                });
//            }
//        });

//        $(".down", $postVoting).click(function () {

//            if (!skimurui.isLoggedIn) {
//                skimurui.login.display("You must be logged in to vote.");
//                return;
//            }

//            // the user wants to downvote a post!
//            if ($postVoting.hasClass("vote-processing")) return;
//            if ($postVoting.hasClass("voted-down")) {
//                // the user already downvoted it, let's just remove the vote
//                $postVoting.addClass("vote-processing");
//                skimur.unvotePost(postId, function (result) {
//                    $postVoting.removeClass("vote-processing");
//                    if (result.success) {
//                        var votes = $(".votes", $postVoting);
//                        votes.html(+votes.html() + 1);
//                        $postVoting.removeClass("voted-up").removeClass("voted-down");
//                    } else {
//                        skimur.displayError(result.error);
//                    }
//                });
//            } else {
//                // the user hasn't casted a downvote, so lets add it
//                $postVoting.addClass("vote-processing");
//                skimur.downvotePost(postId, function (result) {
//                    $postVoting.removeClass("vote-processing");
//                    if (result.success) {
//                        var votes = $(".votes", $postVoting);
//                        votes.html(+votes.html() - 1 - ($postVoting.hasClass("voted-up") ? 1 : 0));
//                        $postVoting.removeClass("voted-up").addClass("voted-down");
//                    } else {
//                        skimur.displayError(result.error);
//                    }
//                });
//            }
//        });
//    });
//});

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
                    skimur.displayError(result.error);
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
                    skimur.displayError(result.error);
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
                    skimur.displayError(result.error);
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
                    skimur.displayError(result.error);
                }
            });
        }
    };

    return {
        voteUp: voteUp,
        voteDown: voteDown
    };

})();