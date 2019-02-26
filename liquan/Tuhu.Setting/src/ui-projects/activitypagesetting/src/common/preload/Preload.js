export class Preload {
  timeout = null;
  constructor(options) {
      this.options = Object.assign({
          routerOptions: {},
          timeout: 2000 // 延时去加载
      }, options);
      this.routerOptions = this.options.routerOptions;
  }
  /**
   *停止加载
   *
   * @memberof Preload
   */
  stopLoad() {
      if (this.timeout!==null) {
          clearTimeout(this.timeout);
      }
  }
  /**
   *开始加载
   *
   * @param {*} route 当前路由
   * @returns {Promise}
   * @memberof Preload
   */
  load(route) {
      let pagelist = [];
      return new Promise((resolve, reject) => {
          let pathList = route && route.meta && route.meta.preload;
          if (pathList && pathList.length > 0) {
              this.stopLoad();
              this.timeout = setTimeout(() => {
                  this.timeout = null;
                  pathList && pathList.forEach(item => { // 获取页面组件
                      pagelist.push(this.getComponents(item));
                  });
                  if (pagelist && pagelist.length > 0) { // 加载页面组件
                      Promise.all(pagelist)
                          .then(list => {
                              cb(list);
                          }, reject);
                  } else {
                      cb([]);
                  }
              }, this.options.timeout);
          } else {
              cb([]);
          }
          /**
       * 加载完成后的回调
       * @param {Array} list 加载的列表
       */
          function cb(list) {
              if ((list && list.length) === (pathList && pathList.length)) {
                  if (route && route.meta && route.meta.preload) {
                      delete route.meta.preload;
                  }
              }
              resolve(list);
          }
      });
  }

  /**
   * 获取预加载的页面路径
   * @param {string} routerPath 页面路径
   * @returns {Promise}
   */
  getComponents(routerPath) {
      let options = this.routerOptions;
      return new Promise((resolve, reject) => {
          if (routerPath) {
              let paths = routerPath.split('/');
              let routerItem = getRouterItem(options, paths);
              try {
                  if (routerItem && routerItem.component instanceof Function) {
                      routerItem.component(resolve);
                  } else {
                      resolve();
                  }
              } catch (error) {
                  reject(error);
              }
          }
      });
  }
}
/**
 * 获取路由项
 *
 * @param {*} obj 路由选项
 * @param {*} list 页面路径字段列表
 * @returns {object}
 */
function getRouterItem(obj, list) {
    let _obj;
    if (obj && list && list.length > 0) {
        let key = list.shift();
        if (key === '') {
            list[0] = '/' + list[0];
            _obj = getRouterItem(obj.routes, list);
        } else {
            _obj = obj.filter(item => {
                if (item.path === key) {
                    return item;
                }
            })[0];

            if (list.length > 0) {
                _obj = getRouterItem(_obj.children, list);
            }
        }
    } else if (list && list.length > 0) {
        _obj = undefined;
    }
    return _obj;
}
