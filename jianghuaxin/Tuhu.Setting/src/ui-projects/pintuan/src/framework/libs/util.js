import axios from 'axios';

let util = {
  message: null,
  modal: null
};
util.title = function (title) {
    title = title || '拼团配置系统';
    window.document.title = title;
};

axios.interceptors.request.use(function (config) {
    loadingbar.start();
    // config.baseURL = location.protocol + "//" + location.hostname.replace("yunying", "setting");
    // console.log(config);
    return config;
  }, function (error) {
    // 对请求错误做些什么
    loadingbar.error();
    return Promise.reject(error);
  });

axios.interceptors.response.use((response) => {
    console.log(response);
    var regexp = /Msg[\s|\S]*[-]*.*IsPower/;
    var isPower = regexp.test(response.data);
    if (isPower) {
        util.message.error({
            content: response.data.match(regexp)[0].replace('Msg:', '').replace(",IsPower", ''),
            duration: 10,
            closable: true
        });
    }
    loadingbar.finish();
    return response;
  }, (error) => {
    loadingbar.error();
    util.message.error({
        content: JSON.stringify(error.response.data),
        duration: 10,
        closable: true
    });
  });

util.ajax = axios;

util.axiosInstance = axios.create();

var loadingbar;

util.useModal = function (item) {
  util.modal = item;
}
util.useMessage = function (item) {
  util.message = item;
}
util.useLoadingbar = function (item) {
    loadingbar = item;
}

util.inOf = function (arr, targetArr) {
    let res = true;
    arr.forEach(item => {
        if (targetArr.indexOf(item) < 0) {
            res = false;
        }
    });
    return res;
};

util.oneOf = function (ele, targetArr) {
    if (targetArr.indexOf(ele) >= 0) {
        return true;
    } else {
        return false;
    }
};

util.deepCopy = function (source) {
    var result = JSON.parse(JSON.stringify(source));
    return result;
}

util.array = {};
util.array.select = function (arr, select) {
    var result = [];
    arr.forEach((item, index) => {
        result.push(select(item));
    });

    return result;
}

util.formatTime = function (strTime) {
  var obj = new Date(parseInt(strTime.replace("/Date(", "").replace(")/", ""), 10));
  return obj.Format("yyyy-MM-dd hh:mm:ss");
}

util.formatDate = function (strTime) {
  var obj = new Date(parseInt(strTime.replace("/Date(", "").replace(")/", ""), 10));
  return obj.Format("yyyy-MM-dd");
}

export default util;
