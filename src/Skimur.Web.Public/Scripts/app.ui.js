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
            skimur.subscribe(subName, function(data) {
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

$(function() {

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
                skimur.unvotePost(postSlug, function(result) {
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
                skimur.upvotePost(postSlug, function(result) {
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
    $.fn.comment = function() {

        var initializeHtml = function(string ) {
            
        }

        return {
            buildHtml: buildHtml,
            initializeHtml: initializeHtml
        };

    };

});