import { Observable } from 'rxjs';
import { publishReplay, refCount } from 'rxjs/operators';

export class HttpCache {
    constructor($storage) {
        this.$storage = $storage;
    }
    /**
   * 缓存请求数据
   *
   * @param {*} cb 请求的回调
   * @param {object} options 缓存选项
   * @param {string} options.cacheKey 缓存用的key
   * @param {string} options.cacheType 缓存用的类型不区分大小写(l===local===存localStorage;
   *                                      s===session===sessionStorage;
   *                                      m===memory===memoryStorage;
   *                                      ms===memorysession===memoryStorage + sessionStorage;
   *                                      ml===memorylocal===memoryStorage + localStorage)
   * @param {boolean} options.needSend 缓存里取的是否需要发请求
   * @param {Function} filtersCacheData 过滤缓存数据
   * @returns {Observable<object>}
   * @memberof HttpCache
   */
    cacheData(cb, options, filtersCacheData) {
        let self = this;
        let $storage = self.$storage;
        let res;
        let cacheKey = options && options.cacheKey;
        if ($storage instanceof Object) {
            if (cacheKey) {
                res = $storage.getMemory(cacheKey, false, {
                    storageKey: 'temp_http'
                });
            }

            if (!res) {
                res = Observable.create((observer) => {
                    let memoryCacheData;
                    if (options && options.needSend) { // 获取本地数据后还需要发请求的，查看内存是否有数据
                        memoryCacheData = self.getNotNeedSendData(options);
                    }
                    let cacheData = memoryCacheData || self.getCacheData(options);

                    // 返回缓存数据
                    if (cacheKey && cacheData) {
                        cacheData._status = 304;
                        observer.next({
                            data: cacheData,
                            status: 304,
                            statusText: 'OK'
                        });
                    }
                    if (!cacheData || (!memoryCacheData && options && options.needSend)) {
                        // 获取接口数据
                        if (cb instanceof Function) {
                            // 缓存接口请求
                            cacheKey && $storage.setMemory(cacheKey, res, {
                                storageKey: 'temp_http'
                            });

                            // 发送请求
                            cb().subscribe(back => {
                                observer.next(back);

                                let _tempCacheData = filtersCacheData(back); // 过滤数据
                                if (typeof _tempCacheData !== 'undefined') {
                                    // 缓存接口数据
                                    self.setCacheData(options, _tempCacheData);
                                }

                                // 如果缓存接口请求则删除
                                cacheKey && $storage.removeMemory(cacheKey, {
                                    storageKey: 'temp_http'
                                });
                            }, err => {
                                observer.error(err);

                                // 如果缓存接口请求则删除
                                cacheKey && $storage.removeMemory(cacheKey, {
                                    storageKey: 'temp_http'
                                });
                            }, () => {
                                observer.complete();
                            });
                        } else {
                            observer.complete();
                        }
                    } else {
                        observer.complete();
                    }
                }).pipe(publishReplay(), refCount());
            }
        } else {
            if (cb instanceof Function) {
                res = cb();
            }
        }

        return res;
    }
    /**
   * 获取内存+本地份数据的内存数据
   *
   * @param {*} options 缓存选项
   * @param {string} options.cacheKey 缓存用的key
   * @param {string} options.cacheType 缓存用的类型不区分大小写(l===local===存localStorage;
   *                                      s===session===sessionStorage;
   *                                      m===memory===memoryStorage;
   *                                      ms===memorysession===memoryStorage + sessionStorage;
   *                                      ml===memorylocal===memoryStorage + localStorage)
   * @returns {object}
   * @memberof HttpCache
   */
    getNotNeedSendData(options) {
        let $storage = this.$storage;
        let res;
        let cacheKey = options && options.cacheKey;
        cacheKey && filterCacheType(options && options.cacheType, (back) => {
            if (back && back.type === 'gm') {
                res = $storage.getMemory(cacheKey, false);
            }
        });
        return res;
    }
    /**
   * 获取缓存数据
   *
   * @param {object} options 缓存选项
   * @param {string} options.cacheKey 缓存用的key
   * @param {string} options.cacheType 缓存用的类型不区分大小写(l===local===存localStorage;
   *                                      s===session===sessionStorage;
   *                                      m===memory===memoryStorage;
   *                                      ms===memorysession===memoryStorage + sessionStorage;
   *                                      ml===memorylocal===memoryStorage + localStorage)
   * @returns {object}
   * @memberof HttpCache
   */
    getCacheData(options) {
        let $storage = this.$storage;
        let res;
        let cacheKey = options && options.cacheKey;
        cacheKey && filterCacheType(options && options.cacheType, (back) => {
            switch (back && back.type) {
                case 'l':
                    res = $storage.getLocal(cacheKey, false);
                    break;
                case 's':
                    res = $storage.getSession(cacheKey, false);
                    break;
                case 'm':
                    res = $storage.getMemory(cacheKey, false);
                    break;
            }
        });
        return res;
    }

    /**
   *设置缓存数据
    *
    * @param {object} options 缓存选项
    * @param {string} options.cacheKey 缓存用的key
    * @param {string} options.cacheType 缓存用的类型不区分大小写(l===local===存localStorage;
    *                                      s===session===sessionStorage;
    *                                      m===memory===memoryStorage;
    *                                      ms===memorysession===memoryStorage + sessionStorage;
    *                                      ml===memorylocal===memoryStorage + localStorage)
    * @param {string} options.timeout 超时时间
    * @param {any} data 缓存的数据
    * @memberof HttpCache
    */
    setCacheData(options, data) {
        let $storage = this.$storage;
        let cacheKey = options && options.cacheKey;
        cacheKey && filterCacheType(options && options.cacheType, (back) => {
            switch (back && back.type) {
                case 'l':
                    $storage.setLocal(cacheKey, data, Object.assign({
                        isSetTime: true
                    }, options));
                    break;
                case 's':
                    $storage.setSession(cacheKey, data, Object.assign({
                        isSetTime: true
                    }, options));
                    break;
                case 'm':
                case 'gm':
                    $storage.setMemory(cacheKey, data, Object.assign({
                        isSetTime: true
                    }, options));
                    break;
            }
        });
    }
}
/**
 * 过滤类型
 *
 * @param {string} cacheType 缓存类型
 * @param {Function} cb 类型回调，参数options.type:l为localStorage；s为sessionStorage；m为memoryStorage；gm为group memoryStorage
 */
function filterCacheType(cacheType, cb) {
    cacheType = ((cacheType || 'm') + '').toLowerCase();
    if (cb instanceof Function) {
        switch (cacheType) {
            case 'l':
            case 'ml':
            case 'memorylocal':
            case 'local':
                cb({type: 'l'});
                break;
            case 's':
            case 'ms':
            case 'memorysession':
            case 'session':
                cb({type: 's'});
                break;
            case 'm':
            case 'memory':
                cb({type: 'm'});
                break;
        }
        switch (cacheType) { // 因为获取数据的时候要分开，所以不能和m\memory放一起
            case 'ml':
            case 'memorylocal':
            case 'ms':
            case 'memorysession':
                cb({type: 'gm'});
                break;
        }
    }
}
