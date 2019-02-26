import moment from 'moment';

var Mixin = {
  methods: {
    translateYesOrNo(row, column) {
      return row[column.property] ? '是' : '否';
    },
    formatDate(row, column) {
      return this._formatDate(row[column.property]);
    },
    formatDatetimeWithoutSecond(row, column) {
      return this._formatDatetimeWithoutSecond(row[column.property]);
    },
    formatDatetime(row, column) {
      return this._formatDatetime(row[column.property]);
    },
    formatTime(row, column) {
      return this._formatTime(row[column.property]);
    },
    formatTimeWithoutSecond(row, column) {
      return this._formatTimeWithoutSecond(row[column.property]);
    },
    formatDatetimeWithPattern(row, column, pattern) {
      return this._formatDatetimeWithPattern(row[column.property], pattern);
    },
    formatStatus(row, column, key) {
      return this._formatStatus(row[column.property], key);
    },
    formatParseFloat(string) {
      return parseFloat(string);
    },
    // 非tab枚举值格式化
    _translateYesOrNo(value) {
      switch (value) {
        case 0:
          return '否';
        case 1:
          return '是';
        case 'N':
          return '否';
        case 'Y':
          return '是';
        default:
          return '';
      }
    },
    _formatDate(value) {
      return value == null ? '' : moment(value).format('YYYY-MM-DD');
    },
    _formatDatetimeWithoutSecond(value) {
      return value == null ? '' : moment(value).format('YYYY-MM-DD HH:mm');
    },
    _formatDatetime(value) {
      return value == null ? '' : moment(value).format('YYYY-MM-DD HH:mm:ss');
    },
    _formatTime(value) {
      return value == null ? '' : moment(value).format('HH:mm:ss');
    },
    _formatTimeWithoutSecond(value) {
      return value == null ? '' : moment(value).format('HH:mm');
    },
    _formatDatetimeWithPattern(value, pattern) {
      return value == null ? '' : moment(value).format(pattern);
    },
    _formatStatus(value, key) {
      const routeStatus = JSON.parse(localStorage.getItem('parameters'))[key];
      return value == null ? '' : routeStatus[value];
    },

    accAdd(arg1, arg2) {
      // js 加法计算 调用：accAdd(arg1,arg2) 返回值：arg1加arg2的精确结果
      var r1, r2, m;
      try {
        r1 = arg1.toString().split('.')[1].length;
      } catch (e) {
        r1 = 0;
      }
      try {
        r2 = arg2.toString().split('.')[1].length;
      } catch (e) {
        r2 = 0;
      }
      m = Math.pow(10, Math.max(r1, r2));
      return ((arg1 * m + arg2 * m) / m).toFixed(2);
    },
    subtr(arg1, arg2) {
      // js 减法计算 subtr(arg1,arg2) 返回值：arg1减arg2的精确结果
      var r1, r2, m, n;
      try {
        r1 = arg1.toString().split('.')[1].length;
      } catch (e) {
        r1 = 0;
      }
      try {
        r2 = arg2.toString().split('.')[1].length;
      } catch (e) {
        r2 = 0;
      }
      m = Math.pow(10, Math.max(r1, r2));
      n = (r1 >= r2) ? r1 : r2;
      return ((arg1 * m - arg2 * m) / m).toFixed(2);
    },
    accMul(arg1, arg2) {
      // js 乘法函数 调用：accMul(arg1,arg2) 返回值：arg1乘以arg2的精确结果
      var m = 0,
        s1 = arg1 ? arg1.toString() : '',
        s2 = arg2 ? arg2.toString() : '';
      try {
        m += s1.split('.')[1].length;
      } catch (e) {}
      try {
        m += s2.split('.')[1].length;
      } catch (e) {}
      return Number(s1.replace('.', '')) * Number(s2.replace('.', '')) / Math.pow(10, m);
    },
    formatCurrency(row, column) {
      let number = row[column.property];
      if (number == null || isNaN(number)) {
        return '0.00';
      }
      let numberString = '' + number.toFixed(2);
      let numeric = this.formatNumeric(numberString.split('.')[0]);

      return numeric + '.' + numberString.split('.')[1];
    },
    formatNumber(row, column) {
      let number = row[column.property];
      if (isNaN(number)) {
        return '0';
      }
      let numberString = '' + number;
      if (numberString.indexOf('.') != -1) {
        return this.formatNumeric(numberString.split('.')[0]) + '.' + numberString.split('.')[1];
      } else {
        return this.formatNumeric(number);
      }
    },
    formatPercentage(row, column) {
      let number = row[column.property];
      if (isNaN(number)) {
        return '0';
      }
      let numberString = '' + number * 100;
      if (numberString.indexOf('.') != -1) {
        return this.formatNumeric(numberString.split('.')[0]) + '.' + numberString.split('.')[1];
      } else {
        return this.formatNumeric(number * 100);
      }
    },
    formatNumeric(number) {
      let result = [],
        counter = 0;
      number = (number || 0).toString().split('');
      for (let i = number.length - 1; i >= 0; i--) {
        counter++;
        result.unshift(number[i]);
        if (!(counter % 3) && i != 0) {
          result.unshift(',');
        }
      }
      return result.join('');
    },
    isEmpty(value) {
      var type;
      if (value == null) { // 等同于 value === undefined || value === null
        return true;
      }
      type = Object.prototype.toString.call(value).slice(8, -1);
      switch (type) {
        case 'String':
          return !$.trim(value);
        case 'Array':
          return !value.length;
        case 'Object':
          return $.isEmptyObject(value); // 普通对象使用 for...in 判断，有 key 即为 false
        default:
          return false; // 其他对象均视作非空
      }
    },
    getUrlKey(paras) {
      var url = location.href;
      var paraString = url.substring(url.indexOf('?') + 1, url.length).split('&');
      var paraObj = {};
      var j;
      /* eslint-disable-next-line no-cond-assign */
      for (var i = 0; j = paraString[i]; i++) {
        paraObj[j.substring(0, j.indexOf('=')).toLowerCase()] = j.substring(j.indexOf('=') + 1, j.length);
      }
      var returnValue = paraObj[paras.toLowerCase()];
      if (typeof (returnValue) == 'undefined') {
        return '';
      } else {
        return returnValue;
      }
    },
    multiLevelJump(lable, name, url) {
      let tab = {
        label: lable,
        name: name,
        url: url,
        isActive: false,
        isEnable: true
      };
      this.$store.dispatch('addToTab', tab);
      this.$store.dispatch('setTabActive', tab);
      this.$store.dispatch('setTabViewName', [tab.name, tab.name]);
    }
  }
};
export default Mixin;
