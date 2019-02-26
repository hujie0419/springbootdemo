var merge = require('webpack-merge')
var prodEnv = require('./prod.env')

module.exports = merge(prodEnv, {
  NODE_ENV: '"development"',
  is_debug: true,
  login_url: '"https://www.tuhu.cn/login.html"',
  logout_url:'"https://www.tuhu.cn/logout.html"',
  login_tuhuscm_ticket: '"cZlwPVd9NbxlCAt3uKHYaKbtfAzrEZFKMRQqwv7QrCQ8CiErpBQkLakyHadDV8xzplPdQdIx22mEbb4SQSDoTMRsfB5JKJGeDUGsBBYLl2PsdidzLFkJBZaVgx7nBcxPh02zO6qnu6niX/+DFo9tMNh7zQnECOHgh4msWzOdY5g="'
})
