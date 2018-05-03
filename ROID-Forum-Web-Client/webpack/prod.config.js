const path = require('path');
const webpackMerge = require('webpack-merge');
const webpackCommon = require('./common.config');

// webpack plugins
const HtmlWebpackPlugin = require('html-webpack-plugin');
const DefinePlugin = require('webpack/lib/DefinePlugin');
const UglifyJsPlugin = require('webpack/lib/optimize/UglifyJsPlugin');
const CopyWebpackPlugin = require('copy-webpack-plugin');
const CleanWebpackPlugin = require('clean-webpack-plugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');

module.exports = webpackMerge(webpackCommon, {
    bail: true,
    devtool: 'source-map',

    output: {
        path: path.resolve(__dirname, '../dist'),
        filename: '[name]-[hash].min.js',
        sourceMapFilename: '[file]-[hash].map'
    },

    module: {
        rules: [
            {
                test: /\.css$/,
                use: ['style-loader', 'css-loader']
            },
            {
                test: /.(ttf|otf|eot|svg|woff(2)?)(\?[a-z0-9]+)?$/,
                use: [{
                    loader: 'file-loader',
                    options: {
                        name: '[name]-[hash].[ext]',
                        outputPath: './assets/fonts/',    // where the fonts will go
                        publicPath: './'       // override the default path
                    }
                }]
            },
            {
                test: /\.scss$/,
                use: ExtractTextPlugin.extract({
                    fallback: 'style-loader',
                    use: [
                        {
                            loader: 'css-loader',

                            options: {
                                modules: false,
                                minimize: true,
                                sourceMap: true,
                                importLoaders: 2,
                                localIdentName: '[name]-[hash]'
                            }
                        },
                        {
                            loader: 'postcss-loader',
                            options: {
                                config: {
                                    path: path.resolve(__dirname, 'postcss.config.js')
                                },
                                sourceMap: true
                            }
                        },
                        {
                            loader: 'resolve-url-loader',
                        },
                        {
                            loader: 'sass-loader',
                            options: {
                                outputStyle: 'expanded',
                                sourceMap: true,
                                sourceMapContents: false
                            }
                        }
                    ]
                })
            }
        ]

    },

    plugins: [
        new CopyWebpackPlugin([
            {from: path.resolve(__dirname, '../static')}
        ], {
            ignore: ['index.html', 'favicon.ico']
        }),
        new CleanWebpackPlugin(['dist'], {
            root: path.resolve(__dirname, '..'),
            exclude: '.gitignore'
        }),
        new DefinePlugin({
            'process.env': {
                NODE_ENV: '"production"'
            }
        }),
        new ExtractTextPlugin('[name]-[hash].min.css'),
        new UglifyJsPlugin({
            compressor: {
                screw_ie8: false,
                warnings: false
            },
            mangle: {
                screw_ie8: false
            },
            output: {
                comments: false,
                screw_ie8: false
            },
            sourceMap: true
        }),
        new HtmlWebpackPlugin({
            template: './static/index.html',
            inject: 'head'
        })
    ]
});
