//********************filter********************
Vue.filter('formatDate', function (value) {
    if (value) {
        var time = value instanceof Date ? value : eval('new ' + value.toString().replace(/\//g, ""));
        var year = time.getFullYear();
        var day = time.getDate();
        var month = time.getMonth() + 1;
        var hours = time.getHours();
        var minutes = time.getMinutes();
        var seconds = time.getSeconds();
        var func = function (value, number) {
            var str = value.toString();
            while (str.length < number) {
                str = "0" + str;
            }
            return str;
        }
        if (year == 1) {
            return "";
        } else {
            return func(year, 4) + '-' + func(month, 2) + '-' + func(day, 2) + ' ' +
                func(hours, 2) + ":" + func(minutes, 2) + ":" + func(seconds, 2);
        }
    }
});
//********************component********************
//需要jquery select2
Vue.component('select2', {
    props: {
        options: {
            type: Array,
            default: []
        },
        value: {
            type: String | Array,
            default: "",
        },
        multiple: {
            type: Boolean,
            default: false,
        },
        styles: {
            type: Object,
            default: function () {
                return { width: '100%' };
            }
        },
        width: {
            type: Number | String,
            default: "100%",
        },
        language: {
            type: String,
            default: "zh-CN"
        },
        closeonselect: {
            type: Boolean,
            default: false
        }
    },
    template: '<select :style="styles"><slot></slot></select>',
    mounted: function () {
        var vm = this;
        $(this.$el).select2({
            data: this.options,
            width: this.width,
            multiple: this.multiple,
            language: this.language,
            closeOnSelect: this.closeonselect,
        });
        $(this.$el).select2('val', this.value);
        $(this.$el)
            .on('select2:select', function () {
                vm.value = $(vm.$el).select2('val');
            })
            .on('select2:unselect', function () {
                vm.value = $(vm.$el).select2('val');
            })
    },
    watch: {
        value: function (value) {
            var vm = this;
            $(this.$el).select2('val', value);
            vm.$emit('input', vm.value)
        },
        options: function (options) {
            $(this.$el).empty().select2({ data: options })
        }
    },
    destroyed: function () {
        $(this.$el).off().select2('destroy')
    }
});
//需要artDialog
Vue.component('artdialog', {
    props: {
        title: {
            type: String,
            default: ""
        },
        width: {
            type: Number | String,
            default: 600
        },
        height: {
            type: Number | String,
            default: 300
        },
        fixed: {
            type: Boolean,
            default: true
        },
        buttons: Array | String,
        modal: {
            type: Boolean,
            default: true,
        }
    },
    template: '<div><slot></slot></div>',
    created: function () {
        var vm = this;
        vm.buttons = vm.buttons || [
            {
                value: "保存",
                callback: function () {
                    vm.$emit('save');
                    return false;
                }
            }
        ];
    },
    data: function () {
        return { dialog: {} };
    },
    methods: {
        close: function () {
            if (this.dialog instanceof dialog) {
                this.dialog.close().remove();
            }
            this.$emit('close');
        }
    },
    mounted: function () {
        var vm = this;
        vm.dialog = dialog({
            title: this.title,
            width: this.width,
            height: this.height,
            fixed: this.fixed,
            button: this.buttons,
            content: this.$el,
            cancelValue: "取消",
            cancelDisplay: true,
            cancel: function () {
                vm.$emit('close');
                this.close().remove();
            }
        });

        if (vm.modal) {
            vm.dialog.showModal();
        }
        else {
            vm.dialog.show();
        }
    },
    destroyed: function () {
        if (this.dialog instanceof dialog) {
            this.dialog.close().remove();
        }
        this.$emit('close');
    }
});

//
Vue.component('pager', {
    props: {
        sizes: {
            type: Array,
            default: [5, 10, 20, 50, 100, 200]
        },
        total: {
            type: Number,
            default: 0,
        },
        value: Object,
    },
    template: '<div>\
    <slot></slot>\
    <div style="margin-top:10px;">\
        <span>每页显示:\
            <select v-model="size">\
                <option v-for="size in sizes" v-bind:value="size">{{size}}</option>\
            </select>条</span>\
        <span>\
            <button type="button" v-on:click="click(\'first\')" v-bind:disabled="index<=1">首页</button>\
            <button type="button" v-on:click="click(\'prev\')" v-bind:disabled="index<=1">上一页</button>\
            <button type="button" v-on:click="click(\'next\')" v-bind:disabled="index>=pages">下一页</button>\
            <button type="button" v-on:click="click(\'last\')" v-bind:disabled="index>=pages">尾页</button>\
        </span>\
        <span style="float:right">\
            <span style="display:inline-block;">当前第\
                <span style="color:#ff0000">{{index}}</span>页/共\
                <span style="color:#03b206">{{pages}}</span>页&nbsp;&nbsp;转到\
                <input style="width:30px" type="text" v-model="input">页</span>\
            <button type="button" v-on:click="click(\'jump\')" v-bind:disabled="input<=0 || input>=pages">确定</button>\
        </span>\
    </div>\
</div>',
    data: function () {
        return {
            input: 0,
            pages: 0,
            size: 10,
            index: 1,
        };
    },
    created: function () {
        var vm = this;
        if (vm.value) {
            vm.size = vm.value.size;
            vm.index = vm.value.index;
        } else {
            vm.value = { size: vm.size, index: vm.index };
        }
        this.sizes = (this.sizes || []).sort(function (x, y) { return x - y });
    },
    watch: {
        total: function () {
            var vm = this;
            if (vm.total <= 0) {
                this.pages = 0;
                return;
            }
            vm.pages = Math.ceil(vm.total / vm.size);
        },
        input: function () {
            var input = this.input;
            if (typeof input !== 'number') {
                input = parseInt(input);
                if (isNaN(input)) {
                    input = 0;
                }
            }
            this.input = input;
        },
        size: function () {
            var vm = this;
            var size = this.size;
            if (typeof size !== 'number') {
                size = parseInt(size);
                if (isNaN(size)) {
                    size = 10;
                }
            }

            var min = vm.sizes[0] || 10, max = vm.sizes[vm.sizes.length - 1] || 10;
            if (size < min) {
                size = min;
            }
            if (size > max) {
                size = max;
            }

            this.size = size;
            this.value.size = this.size;
            vm.pages = Math.ceil(vm.total / vm.size);
        },
        index: function () {
            var vm = this;
            var index = vm.index;
            if (index < 1) {
                index = 1;
            }
            if (vm.pages > 0 && index > vm.pages) {
                index = vm.pages;
            }
            vm.value.index = index;
            vm.index = index;
        },
        value: {
            handler: function (val, oldVal) {
                var vm = this;
                vm.size = val.size;
                vm.index = val.index;
                vm.$emit('click', vm.value);
                vm.$emit('flip', vm.value);
            },
            deep: true
        },
        
    },
    methods: {
        click: function (type) {
            var vm = this;
            var index = vm.index;
            switch (type) {
                case 'first':
                    index = 1;
                    break;
                case 'last':
                    index = vm.pages <= 0 ? 1 : vm.pages;
                    break;
                case 'next':
                    index += 1;
                    break;
                case 'prev':
                    index -= 1;
                    break;
                case 'jump':
                    index = this.input;
                    break;
            }
            vm.index = index;
        }
    }
});