// webpack plugins

module.exports = {
  entry: {
    'site': [
      './src/Main.ts'
    ]
  },
  devtool: 'inline-source-map',
  resolve: {
    extensions: ['.tsx', '.ts', '.js', '.scss'],
    modules: ['node_modules']
  },

  module: {
    rules: [
        {
            test: /\.tsx?$/,
            use: 'ts-loader',
            exclude: /node_modules/
        },
      {
        test: /\.json$/,
        loader: 'json-loader'
      },

      {
        test: /\.(jpg|png|gif|wav)$/,
        loader: 'file-loader',
          options: {
              name: 'assets/[name]-[hash].[ext]',
          }
      },

      {
        test: /\.(mp4|webm)$/,
        loader: 'url?limit=10000'
      }

    ]

  }
};
