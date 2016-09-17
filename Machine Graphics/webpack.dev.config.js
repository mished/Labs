const path = require('path')
module.exports = {
  entry: './src/app.js',
  debug: true,
  devtool: '#inline-source-map',
  output: {
    path: path.resolve(__dirname, 'dev-build'),
    filename: 'bundle.js'
  },
  module: {
    loaders: [
      {
        test: /\.js$/,
        exclude: /node_modules/,
        loader: 'babel-loader'
      },
      {
        test: /\.css$/,
        exclude: /node_modules/,
        loader: 'style-loader!css-loader'
      }
    ]
  }
}
