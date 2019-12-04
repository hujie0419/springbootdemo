import axios from 'axios';

class UtilLib {
    constructor (message, loadingbar) {
        this.message = message;
        this.loadingbar = loadingbar;
        this.ajax = axios;
        this.array = new ArrayExtension();
    }

    initialize () {
        this.ajax.interceptors.request.use((config) => {
            this.loadingbar.start();
            // config.baseURL = location.protocol + "//" + location.hostname.replace("yunying", "setting");
            // console.log(config);
            return config;
          }, (error) => {
            // 对请求错误做些什么
            this.loadingbar.error();
            return Promise.reject(error);
          });
        
        this.ajax.interceptors.response.use((response) => {
            console.log(response);
            var regexp = /Msg[\s|\S]*[-]*.*IsPower/;
            var isPower = regexp.test(response.data);
            if (isPower) {
                this.message.error({
                    content: response.data.match(regexp)[0].replace('Msg:', '').replace(",IsPower", ''),
                    duration: 10,
                    closable: true
                });
            }
            this.loadingbar.finish();
            return response;
        }, (error) => {
            this.loadingbar.error();
            this.message.error({
                content: JSON.stringify(error.response.data),
                duration: 10,
                closable: true
            });
        });
    }

    setTitile (title) {
        title = title || '';
        window.document.title = title;
    }

    getAxiosInstance () {
        return axios.create();
    }

    deepCopy (source) {
        var result = JSON.parse(JSON.stringify(source));
        return result; 
    }
}

class ArrayExtension {
    select (arr, select) {
        var result = [];
        arr.forEach((item, index) => {
            result.push(select(item));
        });

        return result;
    }
}

export default UtilLib;
