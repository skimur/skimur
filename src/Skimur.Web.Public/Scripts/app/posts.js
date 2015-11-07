; skimurui.posts = (function () {

    var getPost = function (element) {
        return $(element).closest(".post");
    };

    var cancel = function (element) {

        var $post = getPost(element);

        // hide any content that may be staged (editing/banning/etc).
        var $staging = $post.find("> .disc-body .disc-staging").addClass("hidden").empty();

        return {
            post: $post,
            staging: $staging
        }
    };

    var voteUp = function (element) {

        if (!skimurui.login.checkLoggedIn("You must be logged in to vote."))
            return;

        var $post = getPost(element);
        var $voting = $(".disc-voting", $post);

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
        var $voting = $(".disc-voting", $post);

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
                skimur.approvePost($post.data("post-id"), function (result) {
                    if (result.success) {
                        skimurui.displaySuccess("The post has been approved.");
                        $post.removeClass("removed").addClass("approved");
                        $(".verdict", $post).removeClass("removed").addClass("approved");
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
                        $post.removeClass("approved").addClass("removed");
                        $(".verdict", $post).removeClass("approved").addClass("removed");
                    } else {
                        skimurui.displayError(result.error);
                    }
                });
            }
        });
    };

    var report = function (element) {

        if (!skimurui.login.checkLoggedIn("You must be logged in to report."))
            return;

        var post = cancel(element);

        var $form = skimurui.buildReportForm().appendTo(post.staging);

        $(".report", $form).click(function (e) {
            skimur.reportPost(post.post.data("post-id"), $("input[type='radio']:checked", $form).val(), $("input[type='text']", $form).val(), function (result) {
                console.log(result);
                if (result.success) {
                    skimurui.displaySuccess("The post has been reported.");
                    cancel(element);
                } else {
                    skimurui.displayError(result.error);
                }
            });
        });

        $(".cancel", $form).click(function (e) {
            cancel(element);
        });

        post.staging.removeClass("hidden");
    };

    var toggleReports = function(element) {
        var $post = getPost(element);
        var $reports = $(".disc-reports", $post);
        if ($reports.hasClass("hidden")) {
            $reports.removeClass("hidden");
        } else {
            $reports.addClass("hidden");
        }
    };

    var clearReports = function(element) {
        var $post = getPost(element);
        skimur.clearReportsForPost($post.data("post-id"), function(result) {
            if (result.success) {
                $(".disc-reports, .disc-options .reports, .disc-options .clear-reports", $post).remove();

            } else {
                skimurui.displayError(result.error);
            }
        });
    }

    var ignoreReports = function(element) {
        var $post = getPost(element);
        skimur.ignoreReportsForPost($post.data("post-id"), function(result) {
            if (result.success) {
                $post.removeClass("reports-unignored").addClass("reports-ignored");
            } else {
                skimurui.displayError(result.error);
            }
        });
    }

    var unignoreReports = function (element) {
        var $post = getPost(element);
        skimur.unignoreReportsForPost($post.data("post-id"), function (result) {
            if (result.success) {
                $post.removeClass("reports-ignored").addClass("reports-unignored");
            } else {
                skimurui.displayError(result.error);
            }
        });
    }

    var startEdit = function(element) {
        var post = cancel(element);
        var $textArea = $("<textarea />")
            .appendTo(post.staging)
            .val(post.post.find("> .disc-body .disc-content-unformatted").val());

        $textArea.markdown({ iconlibrary: "fa", width: "form-group" });

        var $buttonsContainer = $("<div />").appendTo(post.staging);

        var saveButton = $("<button class='btn btn-primary'>Save</button>")
            .appendTo($buttonsContainer)
            .click(function (e) {
                e.preventDefault();
                saveButton.buttonStartLoading();
                skimur.editPost(post.post.data("post-id"), $textArea.val(), function (result) {
                    if (result.success) {
                        cancel(element);
                        post.post.replaceWith($(result.html));
                    } else {
                        saveButton.buttonStopLoading();
                        skimurui.displayError(result.error);
                    }
                });
            });

        $buttonsContainer.append("&nbsp;&nbsp;&nbsp;");

        $("<button class='btn btn-default'>Cancel</button>")
            .appendTo($buttonsContainer)
            .click(function (e) {
                e.preventDefault();
                cancel(this);
            });

        post.staging.removeClass("hidden");
        $textArea.focus();
    };

    var toggleNsfw = function(element) {
        var $post = getPost(element);
        skimurui.confirmWarning("Are you sure?", "Yes!", function(confirmResult) {
            if (confirmResult.confirmed) {
                var nsfw = $post.hasClass("nsfw");
                skimur.togglePostNsfw($post.data("post-id"), !nsfw, function(result) {
                    if (result.success) {
                        if (nsfw) {
                            $post.removeClass("nsfw");
                            $("> .disc-body", $post).find(".disc-options > li.nsfw").remove();
                        } else {
                            $post.addClass("nsfw");
                            $("> .disc-body", $post).find(".disc-options > li.nsfw").remove();
                            $("> .disc-body", $post).find(".disc-options").prepend('<li class="nsfw"><span class="nsfw">NSFW</span><li>');
                        }
                    } else {
                        skimurui.displayError(result.error);
                    }
                });
            }
        });
    }

    var deletePost = function(element) {
        var $post = getPost(element);

        skimurui.confirmDelete(function (result) {
            if (result.confirmed) {
                skimur.deletePost($post.data("post-id"), null, function (deleteResult) {
                    if (deleteResult.success) {
                        $("> .disc-body", $post).find(".disc-options > li:not(.comments)").remove();
                        skimurui.displaySuccess("The post has been deleted.");
                    } else {
                        skimurui.displayError(deleteResult.error);
                    }
                });
            }
        });
    }

    var toggleSticky = function(element) {
        var $post = getPost(element);
        skimurui.confirmWarning("Are you sure?", "Yes!", function (confirmResult) {
            if (confirmResult.confirmed) {
                var sticky = $post.hasClass("sticky");
                skimur.toggleSticky($post.data("post-id"), !sticky, function (result) {
                    if (result.success) {
                        if (sticky) {
                            $post.removeClass("sticky");
                            $("> .disc-body", $post).find(".disc-heading > .sticky").remove();
                        } else {
                            $post.addClass("sticky");
                            $("> .disc-body", $post).find(".disc-heading > .sticky").remove();
                            $("> .disc-body", $post).find(".disc-heading").prepend('<span class="sticky">Sticky</span>');
                        }
                    } else {
                        skimurui.displayError(result.error);
                    }
                });
            }
        });
    }

    var expando = function(element) {
        var $post = getPost(element);

        var $expandoModal = $("#expando-modal");

        if ($expandoModal.length === 0) {
            $expandoModal = $("<div id='expando-modal' class='modal fade expando-modal'>" +
                "<div class='modal-dialog'>" +
                        "<div class='modal-content'>" +
                            "<div class='modal-body'>" +
                            "</div>" +
                            "<div class='modal-footer'>" +
                            "</div>" +
                        "</div>" +
                    "</div>" +
                "</div>");
            $expandoModal.appendTo($("body"));
            $expandoModal.modal({ show: false });
        }

        var $content = $(".modal-body", $expandoModal).empty();
        var $footer = $(".modal-footer", $expandoModal).empty();

        $("<iframe />").attr("src", "http://localhost:45191/").appendTo($content);

        $expandoModal.modal("show");
    };

    return {
        voteUp: voteUp,
        voteDown: voteDown,
        approve: approve,
        remove: remove,
        report: report,
        toggleReports: toggleReports,
        clearReports: clearReports,
        ignoreReports: ignoreReports,
        unignoreReports: unignoreReports,
        startEdit: startEdit,
        delete: deletePost,
        toggleNsfw: toggleNsfw,
        toggleSticky: toggleSticky,
        expando: expando
    };

})();