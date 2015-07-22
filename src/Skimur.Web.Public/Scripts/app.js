; var skimur = (function () {

    var subscribe = function (subName, callback) {
        $.ajax({
            type: "POST",
            url: "/subscribe",
            data: { subName: subName },
            dataType: "json",
            success: function (data) {
                if (callback)
                    callback(data);
            },
            error: function () {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    var unsubcribe = function (subName, callback) {
        $.ajax({
            type: "POST",
            url: "/unsubscribe",
            data: { subName: subName },
            dataType: "json",
            success: function (data) {
                if (callback)
                    callback(data);
            },
            error: function () {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    var votePost = function (postSlug, voteType, callback) {
        $.ajax({
            type: "POST",
            url: "/votepost",
            data: { postSlug: postSlug, type : voteType },
            dataType: "json",
            success: function (data) {
                if (callback)
                    callback(data);
            },
            error: function () {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    var unvotePost = function(postSlug, callback) {
        $.ajax({
            type: "POST",
            url: "/unvotepost",
            data: { postSlug: postSlug },
            dataType: "json",
            success: function (data) {
                if (callback)
                    callback(data);
            },
            error: function () {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    var voteComment = function (commentId, voteType, callback) {
        $.ajax({
            type: "POST",
            url: "/votecomment",
            data: { commentId: commentId, type: voteType },
            dataType: "json",
            success: function (data) {
                if (callback)
                    callback(data);
            },
            error: function () {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    var unvoteComment = function (commentId, callback) {
        $.ajax({
            type: "POST",
            url: "/unvotecomment",
            data: { commentId: commentId },
            dataType: "json",
            success: function (data) {
                if (callback)
                    callback(data);
            },
            error: function () {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    var createComment = function (postSlug, parentId, body, callback) {
        $.ajax({
            type: "POST",
            url: "/createcomment",
            data: { postSlug: postSlug, parentId: parentId, body: body },
            dataType: "json",
            success: function (data) {
                if (callback)
                    callback(data);
            },
            error: function () {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    var editComment = function (commentId, body, callback) {
        $.ajax({
            type: "POST",
            url: "/editcomment",
            data: { commentId: commentId, body: body },
            dataType: "json",
            success: function (data) {
                if (callback)
                    callback(data);
            },
            error: function () {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    return {
        subscribe: subscribe,
        unsubcribe: unsubcribe,
        votePost: votePost,
        upvotePost: function (postSlug, callback) { votePost(postSlug, 1, callback); },
        downvotePost: function (postSlug, callback) { votePost(postSlug, 0, callback); },
        unvotePost: unvotePost,
        upvoteComment: function (postSlug, callback) { voteComment(postSlug, 1, callback); },
        downvoteComment: function (postSlug, callback) { voteComment(postSlug, 0, callback); },
        unvoteComment: unvoteComment,
        createComment: createComment,
        editComment: editComment
    };

})();

// helper method for capturing the enter key
$.fn.enterKey = function (fnc) {
    return this.each(function () {
        $(this).keypress(function (ev) {
            var keycode = (ev.keyCode ? ev.keyCode : ev.which);
            if (keycode == '13') {
                fnc.call(this, ev);
            }
        });
    });
}