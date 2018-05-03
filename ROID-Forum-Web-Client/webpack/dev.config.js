const path = require('path');
const webpackMerge = require('webpack-merge');
const webpackCommon = require('./common.config');

const proxyRules = require('../proxy/rules');

// webpack plugins
const HtmlWebpackPlugin = require('html-webpack-plugin');
const DefinePlugin = require('webpack/lib/DefinePlugin');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const HotModuleReplacementPlugin = require('webpack/lib/HotModuleReplacementPlugin');

module.exports = webpackMerge(webpackCommon, {
    devtool: 'inline-source-map',

    output: {
        path: path.resolve(__dirname, '../dist'),
        filename: '[name].min.js',
        sourceMapFilename: '[file].map'
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
                        name: '[name].[ext]',
                        outputPath: 'assets/fonts/',    // where the fonts will go
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
                                sourceMap: true,
                                importLoaders: 2,
                                localIdentName: '[name]'
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
                                sourceMapContents: true
                            }
                        }
                    ]
                })
            }
        ]
    },

    plugins: [
        new DefinePlugin({
            'process.env': {
                NODE_ENV: "'development'"
            }
        }),
        new ExtractTextPlugin('[name].min.css'),
        new HotModuleReplacementPlugin(),
        new HtmlWebpackPlugin({
            template: './static/index.html',
            inject: 'head'
        })
    ],

    devServer: {
        host: 'localhost',
        port: 3000,
        contentBase: path.resolve(__dirname, '../static'),
        watchContentBase: true,
        compress: true,
        inline: true,
        historyApiFallback: {
            disableDotRule: true
        },
        watchOptions: {
            ignored: /node_modules/
        },
        overlay: {
            warnings: true,
            errors: true
        },
        proxy: proxyRules
    }
});
