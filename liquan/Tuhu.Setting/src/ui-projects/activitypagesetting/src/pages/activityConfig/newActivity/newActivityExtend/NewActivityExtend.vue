<script>
import TabPage from '../../tabPage/TabPage';
import { Observable } from 'rxjs';
import { publishReplay, refCount } from 'rxjs/operators';
import { genConfig } from '../../config/tabsMap.config';
import WindowForm from '../../commons/windowForm/WindowForm';
import * as newActivityApi from '../../commons/apis/newActivity/newActivityApi';

export default {
    extends: TabPage,
    props: {
        Activityid: { // 活动ID
            type: String
        }
    },
    data() {
        let data = this.tagOption && this.tagOption.data;
        return {
            myActivityid: (data && data.ActivityId) || '',
            myActivityInfo: null
        };
    },
    watch: {
        Activityid(nowVal) {
            this.myActivityid = nowVal;
        }
    },
    // data() {
    //     return {
    //         myisRefresh: false
    //     };
    // },
    methods: {
        /**
         * 获取活动数据
         * @param {string} activityid 活动ID
         * @returns {Observable}
         */
        getActivityInfo(activityid) {
            if (!activityid) {
                return Observable.create((observer) => { // activityid为空
                    observer.complete();
                });
            }
            return this.$http.post(newActivityApi.GETACTIVITYINFO, {
                apiServer: 'apiServer',
                isErrorData: false,
                isLoading: true,
                data: {
                    ActivityId: activityid || ''
                }
            });
        },
        /**
         * 跳转新导航Tab
         * @param {Object} data 当前导航的数据
         */
        goNewTab(data) {
            let _that = this;
            data = Object.assign({}, data, {ActivityId: _that.myActivityid});
            this.$$tabs.addTab.apply(this, genConfig(data.SecondaryModuleTypeCode, {
                callBackUpdate({tabName = ''}) {
                    // this.myisRefresh = true;
                    _that.initData && _that.initData();
                    // _that.getActivityInfo(_that.Activityid);
                },
                data: data,
                pageId: data.ModuleId || ''
            }));
        },
        initData() {
            this.getActivityInfo(this.myActivityid).subscribe((data) => {
                this.myActivityInfo = data && data.data;
            });
        },
        /**
         * 打开新的浮窗
         * @param {Object} options 浮窗追加的className
         * @param {String} options.className 浮窗追加的className
         * @param {Array} options.formList 表单的配置数据
         * @param {Function} options.endCallback 浮窗的确认或取消的时的回调
         * @param {Function} options.initCallback 表单初始化后的回调
         * @param {Function} options.onsetChange 表单数据改变时的回调（con：改变的无素，formModel：表单模型, formList：表单数据）
         * @param {Function} options.valueCheck 数据校验的方法（返回true则通过校验）
         */
        openPop({className, formList, endCallback, initCallback, onsetChange, valueCheck}) {
            let _that = this;
            this.$$Popup.open(WindowForm, {
                props: {
                    onsetChange: onsetChange || this.onsetChange,
                    onFormInit: initCallback,
                    valueCheck: valueCheck
                },
                data: {
                    formList: formList,
                    title: _that.title
                },
                wrapCla: (className && className + ' form_popup') || 'alignCla form_popup', // 最外层追加的Class名
                // isShowCloseBtn: alignCla ==='fullScreen' ? true : false,
                alignCla: 'centerMiddle', // ''|'bottom'|'top'|'fullScreen'|'centerMiddle',
                transitionCls: 't_scale' // , // ''|'t_toUp'|'t_scale'|'t_toBottom'|'t_toLeft';
                // showFlag: showFlag
            }).then((data) => {
                if (endCallback instanceof Function) {
                    endCallback(data);
                }
            });
        }
    }
};
</script>
