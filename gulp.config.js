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
      'jquery.magnific-popup.js',
      'app/api.js',
      'app/ui.js',
      'app/login.js',
      'app/misc.js',
      'app/comments.js',
      'app/posts.js',
      'app/subs.js',
      'app/messages.js',
      'app/moderators.js',
      'app/magnific-popup-module.js'
  	],
    useStaticAssets: 'true',
    staticAssetsHost: 'https://jjj.skimur.com/',
    rabbitMQHost: '192.168.10.200',
    redisReadWrite: '192.168.10.200:6379',
    redisRead: '192.168.10.200:6379',
    postgres: 'Server=192.168.10.200;Port=5656;User Id=postgres; Password=password; Database=skimur',
    cassandra: '192.168.10.200'
  }

  return config;
};