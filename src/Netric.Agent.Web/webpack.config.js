module.exports = {
    entry: "./client/entry.jsx",
    output: {
        path: __dirname,
        filename: "bundle.js"
    },
    module: {
        loaders: [
            {test : /\.jsx?/,loader : 'babel'}
        ]
    }
};
