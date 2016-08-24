var path = require('path');

module.exports = {
  entry: [
    './client/web/client'
  ],
  module: {
    loaders: [{
      test: /\.jsx?$/,
      loaders: ['babel-loader']
    }]
  },
  output: {
    path: path.join(__dirname, 'server', 'src', 'Skimur.Web', 'wwwroot', 'dist'),
    filename: 'client.js',
    libraryTarget: 'this',
    publicPath: '/dist/'
  },
  resolve: {
    extensions: ['', '.js', '.jsx'],
    modulesDirectories: [
      path.join(__dirname, 'node_modules'),
      path.join(__dirname, 'client', 'common'),
      path.join(__dirname, 'client', 'web')
    ]
  }
};
