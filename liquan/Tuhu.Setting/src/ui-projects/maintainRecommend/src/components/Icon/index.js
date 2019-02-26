
/**
 * 注册组件
 *
 * @export
 * @param {*} Vue vue
 */
export default function install(Vue) {
  Vue.component('Icon', () => import('./index.vue'));
}
