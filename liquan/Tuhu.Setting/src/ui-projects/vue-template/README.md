# setting start

## 起步
---

```
cd src/ui-projects
vue init ./vue-template {your folder name}

cd {your folder name}

访问：http://yunying.tuhu.de:{port}/pages/{path}/#/
```

## 使用技术
* vue
* iview
* vue-router
* vuex
* less
* axios

## 文件目录结构
---
* assets：一般存放开发过程中产生的静态资源
* static：存放第三方静态资源
* components：公共组件
* framework：框架文件，修改需要同步修改template
* router：路由和目录
* views：页面
* dist：编译后的目录

## 常用文件说明
---
### /router/index.js
Vue-router的路由文件
使用说明：
1. meta中的title会加载到header的title里，authpath是用来做权限验证的，若没有权限，则跳转到403页面
2. 自定义路由需要写在page404上面，否则404会无效

## /router/menus.js	
左侧菜单配置

## /framework/lib/util.js
包含一些常用方法和扩展方法
1. 封装了axios：
```
Vue.prototype.ajax = util.ajax;
Vue.prototype.util = util;

// 请求拦截器
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

// 响应拦截器
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
```
**使用：**
Vue.ajax 或者 Vue.util.ajax

2. 消息提示：
util.message 对象为 Vue.prototype.$Message [iView - A high quality UI Toolkit based on Vue.js](https://www.iviewui.com/components/message)
**example**：
``` javascript
message.error({
        content: JSON.stringify(error.response.data),
        duration: 10,
        closable: true
    });
```
**使用：**
Vue.message 或者 Vue.util.message

3. simple linq：
select
**使用：**
Vue.array 或者 Vue.util.array

4. 其他：
待补充

## 开发指南：
共用组件写在/src/components里
页面写在/src/views里
样式可以放到/assets里
图片，字体资源放到/static里
import关键字
@代表src目录
修改页面风格：ivew主题在theme.less，header的颜色在app.less，loadingbar颜色在main.js
vue data建议有一定的结构，比如 table：{data, columns}
如需使用其它插件，请联系前端做评审

## Deploy
静 态 目 录 ：  
../../ui-projects/template/dist:/pages/template
NPM Build ：  
/src/ui-projects/template


# vue start

``` shell
npm config set registry https://registry.npm.taobao.org
npm config get registry

npm install --global vue-cli
vue init webpack vuetest

npm install iview --save
npm install less less-loader --save-dev
npm install node-sass sass-loader --save-dev
npm install sass-loader --save-dev

# install dependencies
npm install

# serve with hot reload at localhost:8080
npm run dev

# build for production with minification
npm run build

# build for production and view the bundle analyzer report
npm run build --report
```

## 单文件组件
```
<template>
	// template的顶层需要是div
  <div>
		// 内容
  </div>
</template>

<script>
export default {
  
}
</script>

<style lang="less">

</style>

```

## @ = v-on，v-bind 可以省略
## 对象，数组变更
Vue.js是基于**Object.defineProperty**对对象实现“响应式化”
https://cn.vuejs.org/images/data.png

数组变异方法：
* push()
* pop()
* shift()
* unshift()
* splice()
* sort()
* reverse()
* **vm.$set(vm.items, indexOfItem, newValue)**
* **vm.$remove(item)**

**Vue 不能检测对象属性的添加或删除：**
**Vue.set(object, key, value) 别名：vm.$set**

## 单向数据流
Prop 是单向绑定的：当父组件的属性变化时，将传导给子组件，但是反过来不会。这是为了防止子组件无意间修改了父组件的状态，来避免应用的数据流变得难以理解。

## 组件间通信
**~::禁止使用$ref::~**

常用的组件间通信方式：
1. **bus**
同级组件间通信
``` vue
var bus = new Vue();
bus.$on('setMsg', content => { 
      this.msg = content;
    });
bus.$emit('setMsg', 'Hi Vue!');
```

2. **emit**
 子组件 -> 父组件 -> 子组件的同级组件
``` vue
// parent.vue
<template>
	<div>
  		<child @demoEvent="handleDemoEvent"></test>
	</div>
</template>

methods: {
    handleDemoEvent (yourdata) {}
}

// child.vue
<template>
	<button @click="click"></button>
</template>

methods: {
	click () {
		this.$emit('demoEvent', yourdata);	
	}
}
```

3. **vuex**
全局状态保持，数据可能被多个组件使用到的时候使用，或者存在组件的嵌套

## ajax
使用axios：[使用说明 · Axios 中文说明 · 看云](https://www.kancloud.cn/yunye/axios/234845)

* 从浏览器中创建 XMLHttpRequests
* 从 node.js 创建 http 请求
* 支持 Promise API
* 拦截请求和响应
* 转换请求数据和响应数据
* 取消请求
* 自动转换 JSON 数据
* 客户端支持防御 XSRF
