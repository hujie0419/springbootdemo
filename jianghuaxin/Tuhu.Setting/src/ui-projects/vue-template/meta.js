const path = require('path')
const fs = require('fs')

const {
  sortDependencies,
  installDependencies,
  runLintFix,
  printMessage,
} = require('./utils')
const pkg = require('./package.json')

const templateVersion = pkg.version

const { addTestAnswers } = require('./scenarios')

module.exports = {
  metalsmith: {
    // When running tests for the template, this adds answers for the selected scenario
    before: addTestAnswers
  },
  helpers: {
    if_or(v1, v2, options) {

      if (v1 || v2) {
        return options.fn(this)
      }

      return options.inverse(this)
    },
    template_version() {
      return templateVersion
    },
  },
  
  prompts: {
    name: {
      when: 'isNotTest',
      type: 'string',
      required: true,
      message: 'Project name',
    },
    description: {
      when: 'isNotTest',
      type: 'string',
      required: false,
      message: 'Project description',
      default: 'A Vue.js project',
    },
    author: {
      when: 'isNotTest',
      type: 'string',
      message: 'Author',
    },
    urlprefix: {
      when: 'isNotTest',
      type: 'string',
      message: '设置url前缀',
    },
    webport: {
      when: 'isNotTest',
      type: 'string',
      message: '设置develop端口号',
    },
    webtitle: {
      when: 'isNotTest',
      type: 'string',
      message: '设置站点title，比如途虎运营配置系统',
    }
  },
  complete: function(data, { chalk }) {
    const green = chalk.green

    sortDependencies(data, green)

    const cwd = path.join(process.cwd(), data.inPlace ? '' : data.destDirName)

    installDependencies(cwd, data.autoInstall, green)
      .then(() => {
        return runLintFix(cwd, data, green)
      })
      .then(() => {
        printMessage(data, green)
      })
      .catch(e => {
        console.log(chalk.red('Error:'), e)
      })
  },
}
