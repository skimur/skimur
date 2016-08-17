var path = require('path');

module.exports = {
  entry: [
    './electron/app'
  ],
  module: {
    loaders: [{
      test: /\.jsx?$/,
      loaders: ['babel-loader'],
      exclude: /node_modules/
    }]
  },
  output: {
    path: path.join(__dirname, 'electron', 'dist'),
    filename: 'app.js',
    libraryTarget: 'commonjs2'
  },
  resolve: {
    extensions: ['', '.js', '.jsx']
  },
  target: 'electron-renderer'
};
