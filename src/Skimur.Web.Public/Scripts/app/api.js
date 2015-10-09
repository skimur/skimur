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

    var votePost = function (postId, voteType, callback) {
        $.ajax({
            type: "POST",
            url: "/votepost",
            data: { postId: postId, type: voteType },
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

    var unvotePost = function (postId, callback) {
        $.ajax({
            type: "POST",
            url: "/unvotepost",
            data: { postId: postId },
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

    var createComment = function (postId, parentId, body, callback) {
        $.ajax({
            type: "POST",
            url: "/createcomment",
            data: { postId: postId, parentId: parentId, body: body },
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

    var deleteComment = function(commentId, reason, callback) {
        $.ajax({
            type: "POST",
            url: "/deletecomment",
            data: { commentId: commentId, reason: reason },
            dataType: "json",
            success: function(data) {
                if (callback)
                    callback(data);
            },
            error: function() {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    var moreComments = function (postId, sort, children, depth, callback) {
        $.ajax({
            type: "POST",
            url: "/morecomments",
            data: { postId: postId, sort: sort, children: children, depth: depth },
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

    var approvePost = function (postId, callback) {
        $.ajax({
            type: "POST",
            url: "/posts/approve",
            data: { postId: postId },
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

    var removePost = function (postId, callback) {
        $.ajax({
            type: "POST",
            url: "/posts/remove",
            data: { postId: postId },
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

    var replyToMessage = function (messageId, body, callback) {
        $.ajax({
            type: "POST",
            url: "/messages/reply",
            data: { replyToMessage: messageId, body: body },
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

    var markMessagesAsRead = function (messages, callback) {
        $.ajax({
            type: "POST",
            url: "/messages/markmessagesasread",
            data: { messages: messages },
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
    }

    var markMessagesAsUnread = function (messages, callback) {
        $.ajax({
            type: "POST",
            url: "/messages/markmessagesasunread",
            data: { messages: messages },
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
    }

    var reportPost = function (postId, type, reason, callback) {
        $.ajax({
            type: "POST",
            url: "/reports/reportpost",
            data: { postId: postId, type: type, reason: reason },
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
    }

    var reportComment = function (commentId, type, reason, callback) {
        $.ajax({
            type: "POST",
            url: "/reports/reportcomment",
            data: { commentId: commentId, type: type, reason: reason },
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
    }

    var clearReportsForPost = function (postId, callback) {
        $.ajax({
            type: "POST",
            url: "/reports/clear",
            data: { postId: postId },
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
    }

    var clearReportsForComment = function (commentId, callback) {
        $.ajax({
            type: "POST",
            url: "/reports/clear",
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
    }

    var ignoreReports = function (commentId, postId, callback) {
        $.ajax({
            type: "POST",
            url: "/reports/ignore",
            data: { commentId: commentId, postId: postId },
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
    }

    var unignoreReports = function (commentId, postId, callback) {
        $.ajax({
            type: "POST",
            url: "/reports/unignore",
            data: { commentId: commentId, postId: postId },
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
    }

    var removeModFromSub = function(subName, subId, userId, callback) {
        $.ajax({
            type: "POST",
            url: "/moderators/removemodfromsub",
            data: { subName: subName, subId: subId, userId: userId },
            dataType: "json",
            success: function(data) {
                if (callback)
                    callback(data);
            },
            error: function() {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    var changeModPermissionsForSub = function(subName, subId, userId, permissions, callback) {
        $.ajax({
            type: "POST",
            url: "/moderators/changemodpermissions",
            data: { subName: subName, subId: subId, userId: userId, permissions: permissions },
            dataType: "json",
            success: function(data) {
                if (callback)
                    callback(data);
            },
            error: function() {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    var editPost = function (postId, content, callback) {
        $.ajax({
            type: "POST",
            url: "/editpost",
            data: { postId: postId, content: content },
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

    var deletePost = function (postId, reason, callback) {
        $.ajax({
            type: "POST",
            url: "/deletepost",
            data: { postId: postId, reason: reason },
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

    var togglePostNsfw = function(postId, nsfw, callback) {
        $.ajax({
            type: "POST",
            url: "/posts/togglensfw",
            data: { postId: postId, nsfw: nsfw },
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

    var submissionText = function(subName, subId, callback) {
        $.ajax({
            type: "POST",
            url: "/subs/SubmissionText",
            data: { subName: subName, subId: subId },
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

    var inviteMod = function(subName, subId, userName, userId, permissions) {
        $.ajax({
            type: "POST",
            url: "/moderators/invitemod",
            data: { subName: subName, subId: subId, userName: userName, userId: userId, permissions: permissions },
            dataType: "json",
            success: function(data) {
                if (callback)
                    callback(data);
            },
            error: function() {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    var acceptModInvite = function(subName, subId) {
        $.ajax({
            type: "POST",
            url: "/moderators/acceptinvite",
            data: { subName: subName, subId: subId },
            dataType: "json",
            success: function(data) {
                if (callback)
                    callback(data);
            },
            error: function() {
                if (callback)
                    callback({ success: false, error: "There was an error processing your request." });
            }
        });
    };

    var denyModInvite = function (subName, subId) {
        $.ajax({
            type: "POST",
            url: "/moderators/denyinvite",
            data: { subName: subName, subId: subId },
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

    var removeModInvite = function (subName, subId, userName, userId) {
        $.ajax({
            type: "POST",
            url: "/moderators/removeinvite",
            data: { subName: subName, subId: subId, userName: userName, userId: userId },
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

    var changeModInvite = function (subName, subId, userName, userId, permissions) {
        $.ajax({
            type: "POST",
            url: "/moderators/changeinvite",
            data: { subName: subName, subId: subId, userName: userName, userId: userId, permissions: permissions },
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
        upvotePost: function (postId, callback) { votePost(postId, 1, callback); },
        downvotePost: function (postId, callback) { votePost(postId, 0, callback); },
        unvotePost: unvotePost,
        upvoteComment: function (commentId, callback) { voteComment(commentId, 1, callback); },
        downvoteComment: function (commentId, callback) { voteComment(commentId, 0, callback); },
        unvoteComment: unvoteComment,
        createComment: createComment,
        editComment: editComment,
        deleteComment: deleteComment,
        moreComments: moreComments,
        approvePost: approvePost,
        removePost: removePost,
        replyToMessage: replyToMessage,
        markMessagesAsRead: markMessagesAsRead,
        markMessagesAsUnread: markMessagesAsUnread,
        reportPost: reportPost,
        reportComment: reportComment,
        clearReportsForComment: clearReportsForComment,
        clearReportsForPost: clearReportsForPost,
        ignoreReportsForPost: function (postId, callback) { return ignoreReports(null, postId, callback); },
        ignoreReportsForComment: function (commentId, callback) { return ignoreReports(commentId, null, callback); },
        unignoreReportsForPost: function (postId, callback) { return unignoreReports(null, postId, callback); },
        unignoreReportsForComment: function (commentId, callback) { return unignoreReports(commentId, null, callback); },
        removeModFromSub: removeModFromSub,
        changeModPermissionsForSub: changeModPermissionsForSub,
        editPost: editPost,
        deletePost: deletePost,
        togglePostNsfw: togglePostNsfw,
        submissionText: submissionText,
        inviteMod: inviteMod,
        acceptModInvite: acceptModInvite,
        denyModInvite: denyModInvite,
        removeModInvite: removeModInvite,
        changeModInvite: changeModInvite
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