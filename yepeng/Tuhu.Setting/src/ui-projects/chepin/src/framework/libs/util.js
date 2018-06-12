import axios from 'axios';

let util = {

};
util.title = function (title) {
    title = title || '';
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
    loadingbar.finish();
    return response;
  }, (error) => {
    loadingbar.error();
    message.error({
        content: JSON.stringify(error.response.data),
        duration: 10,
        closable: true
    });
  });

util.ajax = axios;

util.axiosInstance = axios.create();

var message;
var loadingbar;
util.useMessage = function (item) {
    message = item;
}
util.useLoadingbar = function (item) {
    loadingbar = item;
}

util.message = message;

util.array = {};
util.array.select = function (arr, select) {
    var result = [];
    arr.forEach((item, index) => {
        result.push(select(item));
    });

    return result;
}

export default util;
