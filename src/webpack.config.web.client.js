var webpack = require('webpack');
var path = require('path');
var ExtractTextPlugin = require("extract-text-webpack-plugin");
var extractCSS = new ExtractTextPlugin('styles.css');

module.exports = {
  entry: [
    'whatwg-fetch',
    'babel-polyfill',
    'bootstrap-loader',
    './client/web/client'
  ],
  module: {
    loaders: [
      { test: /\.jsx?$/, loaders: ['babel-loader'] },
      { test: /\.css$/, loader: extractCSS.extract('style', 'css?modules') },
      { test: /\.scss$/, loader: extractCSS.extract('style', 'css?modules!sass') },
      { test: /\.(woff2?|ttf|eot|svg)(\?v=[0-9]\.[0-9]\.[0-9])?$/, loader: 'file' },
      { test: /\.(jpeg|jpeg|gif|png|tiff)(\?v=[0-9]\.[0-9]\.[0-9])?$/, loader: 'file' }
    ]
  },
  output: {
    path: path.join(__dirname, 'server', 'src', 'Skimur.Web', 'wwwroot', 'dist'),
    filename: 'client.js',
    libraryTarget: 'this',
    publicPath: '/dist/'
  },
  plugins: [
    extractCSS
  ],
  resolve: {
    extensions: ['', '.js', '.jsx'],
    modulesDirectories: [
      path.join(__dirname, 'node_modules'),
      path.join(__dirname, 'client', 'common'),
      path.join(__dirname, 'client', 'web')
    ]
  },
  devtool: 'source-map'
};
