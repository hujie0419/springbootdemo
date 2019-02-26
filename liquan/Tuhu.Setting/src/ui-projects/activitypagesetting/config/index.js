// see http://vuejs-templates.github.io/webpack for documentation.
var path = require('path');

module.exports = {
    build: {
        env: require('./prod.env'),
        index: path.resolve(__dirname, '../dist/index.html'),
        assetsRoot: path.resolve(__dirname, '../dist'),
        assetsSubDirectory: 'static',
        assetsPublicPath: '/pages/activitypagesetting/',
        productionSourceMap: true,
        // Gzip off by default as many popular static hosts such as
        // Surge or Netlify already gzip all static assets for you.
        // Before setting to `true`, make sure to:
        // npm install --save-dev compression-webpack-plugin
        productionGzip: false,
        productionGzipExtensions: ['js', 'css'],
        // Run the build command with an extra argument to
        // View the bundle analyzer report after build finishes:
        // `npm run build --report`
        // Set to `true` or `false` to always turn it on or off
        bundleAnalyzerReport: process.env.npm_config_report
    },
    local: {
        useEslint: true,
        env: require('./local.env'),
        port: 8080,
        autoOpenBrowser: true,
        assetsSubDirectory: 'static',
        assetsPublicPath: '/',
        domain: '',
        // cookie: {
        //     '.AspNet.Cookies': 'KgMAGerF_y67s2RT25ackTLKmSFAZ3oYepKRqHFFS6rIUN3Yn8NO0h5Xr0kA1BDTHOM4rAohS4sr_3toZx6b02zXqZg3GO1KpuERvmM5TCcUzljU3uxxCXNxvWrWP-JcJMVVLLXUOCVSgLvOHsQI1xoisr4r9diu0ABnml2HtGv-ccioK2E0BVbRgnWx8BbbwvOSkg',
        //     '.TUHUYEWUSiGN': '0EAF09C1142FA5626CDF27B7D4773CDDAD8003ECDE3AE08793C2F3CAAA346112C8B6E3502DDB81B4D1047CF214076227A2A911F98B5A3F39234C6A86B9221DFE556844D20820378C75FD635AC3503E6C506BBEC659FF771219110F40'
        // },
        cookie: {
            '.AspNet.Cookies': '',
            '.TUHUYEWUSiGN': '9C6A9EABEE733DA6AFC6259FA6225959EE0CAC2F6D5055439AAB40F44687DDDB9860E5BBFE0DA2BA1B1807B6670FC513A81227D0208138C29758E677CFC878B2797912A346E55D8C7F25BE14B8222B0B207038AB81B4D819ED4D6578'
        },
        proxyTable: {
            '/work': {
                target: 'https://setting.tuhu.work',
                changeOrigin: true,  //是否跨域
                secure: false,
                pathRewrite: {
                    '^/work': ''
                }
            },
            '/workapp': {
                target: 'https://settingapp.tuhu.work',
                changeOrigin: true,  //是否跨域
                secure: false,
                pathRewrite: {
                    '^/workapp': ''
                }
            },
            '/workdev': {
                target: 'https://settingdev.tuhu.work',
                changeOrigin: true,  //是否跨域
                secure: false,
                pathRewrite: {
                    '^/workdev': ''
                }
            },
            '/api': {
                target: 'http://172.16.20.56:8081',
                changeOrigin: true,  //是否跨域
                secure: false,
                pathRewrite: {
                    '^/api': '/activity'
                }
            }

            // CSS Sourcemaps off by default because relative paths are "buggy"
            // with this option, according to the CSS-Loader README
            // (https://github.com/webpack/css-loader#sourcemaps)
            // In our experience, they generally work as expected,
            // just be aware of this issue when enabling this option.
        },
        cssSourceMap: false
    }
}
