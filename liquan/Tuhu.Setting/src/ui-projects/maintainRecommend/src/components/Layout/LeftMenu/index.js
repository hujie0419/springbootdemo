/**
 * 注册组件
 *
 * @export
 * @param {*} Vue vue
 */
export default function install(Vue) {
  Vue.component('left-menu', () => import('./LeftMenu.vue'));
}