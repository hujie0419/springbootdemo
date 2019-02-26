import { Observable } from 'rxjs';
import { publishReplay, refCount } from 'rxjs/operators'; // 'rxjs/add/operator/publishReplay';

import { HttpClientCore } from './HttpClient.core';

export class HttpClient extends HttpClientCore {
  /* eslint-disable no-useless-constructor */
  constructor(option, $env, $storage) {
    super(option, $env, $storage);
  }

  /**
   * 发送请求
   * @param {*} url api url
   * @param {*} option 选项
   * @returns {Observable<object>}
   */
  getData(url, option) {
    let _that = this;
    return Observable.create((observer) => {
      _that.send(url, option).subscribe(back => {
        observer.next(back.data);
      }, err => {
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
