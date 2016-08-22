var path = require('path');

module.exports = {
  entry: [
    './electron/app'
  ],
  module: {
    loaders: [{
      test: /\.jsx?$/,
      loaders: ['babel-loader']
    }]
  },
  output: {
    path: path.join(__dirname, 'electron', 'dist'),
    filename: 'app.js',
    libraryTarget: 'commonjs2'
  },
  resolve: {
    extensions: ['', '.js', '.jsx'],
    modulesDirectories: [path.join(__dirname, 'client')]
  },
  target: 'electron-renderer'
};
