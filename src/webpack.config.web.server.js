var path = require('path');
var nodeExternals = require('webpack-node-externals');

module.exports = {
  entry: [
    './client/web/server'
  ],
  module: {
    loaders: [
      { test: /\.jsx?$/, loaders: ['babel-loader'] },
      { test: /\.css$/, loader: 'css/locals?module' },
      { test: /\.scss$/, loader: 'css/locals?module!sass' },
      { test: /\.(woff2?|ttf|eot|svg)(\?v=[0-9]\.[0-9]\.[0-9])?$/, loader: 'file' },
      { test: /\.(jpeg|jpeg|gif|png|tiff)(\?v=[0-9]\.[0-9]\.[0-9])?$/, loader: 'file' }
    ]
  },
  externals: [nodeExternals({
    modulesFromFile: true
  })],
  output: {
    path: path.join(__dirname, 'server', 'src', 'Skimur.Web', 'App'),
    filename: 'server.js',
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
