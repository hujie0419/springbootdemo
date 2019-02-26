import axios from 'axios';
import { Observable } from 'rxjs';
import { publishReplay, refCount } from 'rxjs/operators'; // 'rxjs/add/operator/publishReplay';

import { HttpCache } from './HttpCache';

export class HttpClientCore {
  constructor(option, $env, $storage) {
    this.$httpCache = new HttpCache($storage);

    this.axios = axios.create(Object.assign({
      withCredentials: true,
      headers: { 'Content-type': 'application/json' }
    }, option));

    this.$env = $env;
  }
  /**
     * 发送请求
     * @param {*} url api url
     * @param {*} options 选项
     * @returns {Observable<object>}
     */
  send(url, options) {
    options = options || {};
    let self = this;
    let _url = self.getApiUrl(options && options.apiServer, url);
    let cacheOptions = self.getCacheOption(url, options);
    options.cacheData && delete options.cacheData;

    // 删除选项中的apiServer的key
    delete options.apiServer;

    // 发送请求
    let res;
    let _axios = Observable.create((observer) => {
      self.axios(_url, options).then(back => {
        observer.next(back);
        observer.complete();
      }, err => {
        observer.error(err);
        observer.complete();
      });
    }).pipe(publishReplay(), refCount());

    // 缓存
    res = self.cacheData(_axios, cacheOptions);

    return res;
  }

  /**
   * 获取api链接
   * @param {*} apiServer api服务器key
   * @param {*} api api
   * @returns {string}
   */
  getApiUrl(apiServer, api) {
    let server = '';
    let res = api || '';
    if (!/^\s*(https?:|\/\/)/.test(res)) { // 没有加协议的才可以拼接
      apiServer = (apiServer || 'apiServer') + '';
      if (apiServer) {
        server = this.$env[apiServer] || '';
      }

      res = server + res + '';
    }
    return res;
  }

  /**
   * 获取缓存参数
   *
   * @param {String} url 请求地址
   * @param {Object} options 请求选项
   * @returns {Object|undefined}
   * @memberof HttpClient
   */
  getCacheOption(url, options) {
    let res;
    // 设置缓存
    let cacheData = options && options.cacheData;
    if (url && cacheData) {
      res = {
        cacheKey: url
      };
      if (cacheData instanceof Object) {
        res = Object.assign(res, cacheData);
      }
    }
    return res;
  }
  /**
   * 设置数据缓存
   *
   * @param {Observable<object>} obs 请求的Observable
   * @param {object} options 缓存选项
   * @returns {Observable<object>}
   * @memberof HttpClient
   */
  cacheData(obs, options) {
    let self = this;
    let res;
    // 缓存数据
    if (self.$httpCache instanceof Object) {
      res = self.$httpCache.cacheData(() => {
        return obs;
      }, options, back => { // 过滤缓存数据（返回数据不为undefined的数据则会缓存）
        let _tempData = back && back.data;
        if (_tempData instanceof Object) {
          // 缓存接口数据
          if (_tempData.success === true || _tempData.status === true || _tempData.Code + '' === '1') {
            return _tempData;
          }
        }
      });
    } else {
      res = obs;
    }
    return res;
  }
}