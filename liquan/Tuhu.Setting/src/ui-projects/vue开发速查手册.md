# Vue 开发速查手册

### iview

#### 表单验证

ivew使用的表单验证插件是https://github.com/yiminghe/async-validator，这个插件存在一些bug

1. number类型数据的form validate

   问题：官方文档上说明需要指定type：number，但是这种情况首次加载会出现错误提示

   解决方案：自定义验证

   ``` javascript
   {
       validator: (rule, value, callback) => {
           if (value === '' || isNaN(value)) {
               callback(new Error('请输入途虎门店数字ID'));
           } else {
               callback();
           }
       },
       trigger: "blur"
   }
   ```



#### Select

1. Number类型的默认选中

   问题：如果value是number类型，那么默认选中会失效

   解决方案：option使用数据源加载

   ```javascript
   <Select v-model="value">
       <Option v-for="item in select_data" :value="item.value" :key="item.value">{{ 			item.text }}</Option>
   </Select>
   ```

   ​

### 常用代码

```javascript
params: {
	...this.search_data, 
	pageIndex: this.page.current, 
	pageSize: this.page.pageSize
} 
```



