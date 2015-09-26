module.exports = function() {

  var config = {
    jsFiles: [
      'jquery.js',
      'jquery.validate.js',
      'jquery.validate.unobtrusive.js',
      'jquery.validate.bootstrap.js',
      'modernizr.js',
      'bootstrap.js',
      'respond.js',
      'bootstrap-notify.js',
      'sweet-alert.js',
      'marked.js',
      'markedHelper.js',
      'to-markdown.js',
      'bootstrap-markdown.js',
      'app/api.js',
      'app/ui.js',
      'app/login.js',
      'app/misc.js',
      'app/comments.js',
      'app/posts.js',
      'app/subs.js',
      'app/messages.js',
      'app/moderators.js'
  	],
    useStaticAssets: 'true',
    staticAssetsHost: 'http://static.skimur.com/'
  }

  return config;
};