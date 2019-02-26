import { Observable } from 'rxjs';
import { publishReplay, refCount } from 'rxjs/operators'; // 'rxjs/add/operator/publishReplay';

import { HttpClientCore } from './HttpClient.core';

export class HttpClient extends HttpClientCore {
    /* eslint-disable no-useless-constructor */
    constructor(option, config, $env, $storage) {
        super(option, config, $env, $storage);
        this.option = option;
    }

    /**
     * 发送请求
     * @param {*} url api url
     * @param {*} option 选项
     * @returns {Observable<object>}
     */
    getData(url, option) {
        let _that = this;

        let isErrorData = false;
        if (option && typeof option.isErrorData !== 'undefined') {
            isErrorData = option.isErrorData;
        } else if (this.option && typeof this.option.isErrorData !== 'undefined') {
            isErrorData = this.option.isErrorData;
        }
        option.isErrorData = isErrorData;
        return Observable.create((observer) => {
            _that.send(url, option).subscribe(back => {
                let data = back.data;
                let isError = false;
                if ((isErrorData || option.isErrorMsg) && data && this.config && (this.config.onServiceError instanceof Function)) {
                    let _data = this.config.onServiceError(data, option);
                    if (typeof data !== 'undefined' && typeof _data === 'undefined') {
                        isError = true;
                    }
                }
                if (!isError || !isErrorData) {
                    observer.next(data);
                }
            }, err => {
                if ((isErrorData || option.isErrorMsg) && _that.config && (_that.config.onHttpError instanceof Function)) {
                    _that.config.onHttpError(err, url);
                }
                observer.error(err);
            }, () => {
                observer.complete();
            });
        }).pipe(publishReplay(), refCount());
    }
    /**
     * get请求
     * @param {*} url api url
     * @param {*} option 选项
     * @returns {Observable<object>}
     */
    get(url, option) {
        return this.getData(url, Object.assign({}, option, {
            method: 'get'
        }));
    }

    /**
     * post请求
     * @param {*} url api url
     * @param {*} option 选项
     * @returns {Observable<object>}
     */
    post(url, option) {
        return this.getData(url, Object.assign({}, option, {
            method: 'post'
        }));
    }
    /**
     * formData方式post请求
     * @param {*} url api url
     * @param {*} option 选项
     * @returns {Observable<object>}
     */
    formData(url, option) {
        let data = option.data;
        if (!(data instanceof FormData) && data instanceof Object) {
            let formData = new FormData();
            for (const key in data) {
                if (data.hasOwnProperty(key)) {
                    const item = data[key];
                    formData.append(key, item);
                }
            }
            option.data = formData;
        }
        return this.getData(url, Object.assign({
            headers: { 'Content-Type': 'multipart/form-data' }
        }, option, {
            method: 'post'
        }));
    }
}

/**
 * 实例化HttpClient
 *
 * @export
 * @param {*} args 参数
 * @returns {HttpClient}
 */
export function httpFactory (...args) {
    return new HttpClient(...args);
}
