const path = require("path");
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = {
  entry: {
    index: "./src/chat/index.ts",
    makeSession: "./src/makeSession/makeSession.ts",
    form: "./src/form/form.ts",
  },
  output: {
    path: path.resolve(__dirname, "wwwroot/webpack"),
    filename: "[name].js",
    publicPath: "/webpack/",
  },
  resolve: {
    extensions: [".js", ".ts"],
  },
  module: {
    rules: [
      {
        test: /\.ts$/,
        use: "ts-loader",
      },
      {
        test: /\.s?css$/,
        use: [
          // Save the CSS as a separate file to allow caching
          MiniCssExtractPlugin.loader,
          {
            // Translate CSS into CommonJS modules
            loader: "css-loader",
            options: {
              url: true,
            },
          },
          {
            // Run postcss actions
            loader: "postcss-loader",
            options: {
              postcssOptions: {
                plugins: [
                  function () {
                    return [require("autoprefixer")];
                  },
                ],
              },
            },
          },
          {
            // Resolve relative URLs in SCSS files
            loader: "resolve-url-loader",
            options: {
              sourceMap: true,
            },
          },
          {
            loader: "sass-loader",
            options: {
              sourceMap: true,
              sassOptions: {
                outputStyle: "compressed",
              },
            },
          },
        ],
      },
      {
        test: /\.woff($|\?)|\.woff2($|\?)|\.ttf($|\?)|\.eot($|\?)|\.svg($|\?)/i,
        type: "asset/resource",
        generator: {
          //filename: 'fonts/[name]-[hash][ext][query]'
          filename: "fonts/[name][ext][query]",
        },
      },
    ],
  },
  plugins: [
    new CleanWebpackPlugin(),
    new MiniCssExtractPlugin({
      filename: "css/[name].css",
    }),
  ],
};
