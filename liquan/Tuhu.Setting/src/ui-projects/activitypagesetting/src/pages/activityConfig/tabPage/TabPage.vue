<script>
import { Observable } from 'rxjs';
import { publishReplay, refCount } from 'rxjs/operators';
import apis from '../commons/apis/apis.js';

export default {
    // inject: ['$$tabs', '$$form'],
    inject: ['$$Popup'],
    props: {
        tagOption: {
            type: Object
        }
    },
    methods: {
        /**
         * 基础模块配置信息查询接口
         * @param {Object} options 查询参数（默认取data）
         * @return {Observable}
         */
        getModuleData(options) {
            let _data = Object.assign({}, this.tagOption && this.tagOption.data, options);

            return Observable.create((observer) => {
                if (_data && _data.ActivityId && _data.ModuleId) {
                    this.$http.post(apis.GetActivityModuleBasis, {
                        data: Object.assign({
                            ActivityId: _data.ActivityId,
                            ModuleId: _data.ModuleId
                        }, options)
                    }).subscribe(back => {
                        observer.next(back);
                        observer.complete();
                    }, err => {
                        observer.error(err);
                        observer.complete();
                    });
                } else {
                    observer.error('请传入ActivityId和ModuleId');
                }
            }).pipe(publishReplay(), refCount());
        },
        /**
         * 页面ID过滤器
         * @param {Object} item 当前页签项数据
         * @param {string|number} tabKey 当前名称
         * @return {boolean}
         */
        filterPageId(item, tabKey) {
            return item.name === (tabKey + (item.pageId || ''));
        }
        /**
         * 刷新数据
         */
        // refresh () {
        //     this.refreshData();
        // }
    }
};
</script>
