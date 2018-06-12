var path = require('path')
var webpack = require('webpack')
var ExtractTextPlugin = require("extract-text-webpack-plugin")
var version = '1.0.3'

module.exports = {
  devtool: false,
  entry: {
    ['bundle.'+ version] : ['./index'],
    ['PrePolyfill.' + version] : ['./PrePolyfill']
  },
  output: {
    path: path.join(__dirname, 'dist'),
    filename: '[name].min.js',
    publicPath: 'https://res.tuhu.org/react/find/dist/',
    libraryTarget: 'var'
  },
  externals: {
    'react': 'React',
    'react-dom': 'React.__SECRET_DOM_DO_NOT_USE_OR_YOU_WILL_BE_FIRED'
  },
  plugins: [
    new ExtractTextPlugin("[name].min.css"),
    new webpack.optimize.UglifyJsPlugin({
      compress: {
        warnings: false
      },
      sourceMap: false,
      mangle: false
    })
  ],
  module: {
    loaders: [
      {
        test: /\.js$/,
        loader: 'babel',
        query:{
          presets:['es2015','react'],
          plugins:['transform-class-properties']
        },
        exclude: /node_modules/,
        include: __dirname
      },{
        test: /\.css?$/,
        loader: ExtractTextPlugin.extract( 'style', 'css' ),
        include: __dirname
      },{
        test: /.*\.(gif|png|jpe?g|svg)$/i,
        loaders: [
          'url-loader?limit=8192&name=[path][name].[ext]',
          'image-webpack'
        ]
      },{
        test: /\.js$/,
        loader: 'eslint-loader',
        exclude: /node_modules|lib/
      }
    ]
  },
  eslint: {
    configFile: '../../.eslintrc'
  }
}
