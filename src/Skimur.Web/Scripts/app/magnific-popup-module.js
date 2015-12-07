(function () {

    var skimurIframeNs = 'skimuriframe';
    var emptyPage = '//about:blank';

    var fixIframeBugs = function (mfp, isShowing) {
        if (mfp.currTemplate[skimurIframeNs]) {
            var el = mfp.currTemplate[skimurIframeNs].find('iframe');
            if (el.length) {
                // reset src after the popup is closed to avoid "video keeps playing after popup is closed" bug
                if (!isShowing) {
                    el[0].src = emptyPage;
                }

                // IE8 black screen bug fix
                if (mfp.isIE8) {
                    el.css('display', isShowing ? 'block' : 'none');
                }
            }
        }
    };

    $.magnificPopup.registerModule(skimurIframeNs, {

        options: {
            markup: '<div class="mfp-iframe-scaler">' +
                        '<div class="mfp-close"></div>' +
                        '<iframe class="mfp-iframe" src="//about:blank" frameborder="0" allowfullscreen></iframe>' +
                    '</div>',

            srcAction: 'iframe_src'
        },

        proto: {
            initSkimuriframe: function () {

                var mfp = this;

                mfp.types.push(skimurIframeNs);

                mfp.ev.on("mfpBeforeChange.mfp", function(e, prevType, newType) {
                    if (prevType !== newType) {
                        if (prevType === skimurIframeNs) {
                            fixIframeBugs(mfp); // iframe if removed
                        } else if (newType === skimurIframeNs) {
                            fixIframeBugs(mfp, true); // iframe is showing
                        }
                    } 
                });

                mfp.ev.on("mfpBeforeAppend.mfp", function () {
                    mfp.container.removeClass(function (index, css) {
                        return (css.match(/(^|\s)mfp-provider-\S+/g) || []).join(' ');
                    });
                    if (mfp.currTemplate[skimurIframeNs]) {
                        mfp.container.addClass("mfp-provider-" + mfp.currItem.data.provider.toLowerCase());
                    }
                });

                mfp.ev.on("mfpClose." + skimurIframeNs + ".mfp", function () {
                    fixIframeBugs(mfp);
                });
            },

            getSkimuriframe: function (item, template) {

                var mfp = this;

                var embedSrc = item.src;
                var iframeSt = mfp.st.iframe;

                var dataObj = {};
                if (iframeSt.srcAction) {
                    dataObj[iframeSt.srcAction] = embedSrc;
                }
                mfp._parseMarkup(template, dataObj, item);

                mfp.updateStatus('ready');

                return template;
            }
        }
    });

})();