<script>
/* eslint-disable max-lines */
import NewActivityExtend from '../newActivityExtend//NewActivityExtend';
import * as newActivityApi from '../../commons/apis/newActivity/newActivityApi';
import { getChildList, getDicList, getNavConfig } from '../config/popUp.config';
import { floatingUpsConfig, getStaticFloating, getfloatingUpsJumpConfig } from '../config/floatingUps.config';
export default {
    extends: NewActivityExtend,
    data() {
        return {
            formUpdata: null,
            formDataCache: this.$$form.initFormCache() // 缓存表单数据
        };
    },
    methods: {
        /**
         * 添加导航
         */
        addNavSet() {
            let _that = this;
            _that.getActivityInfo(_that.Activityid).subscribe(baseData => {
                _that.title = '添加导航';
                let navConfig = getNavConfig();
                _that.openPop({
                    className: null,
                    formList: navConfig,
                    endCallback: (data) => {
                        if (!data || data.type === 'cancel' || data.type === 'close') {
                            return;
                        }
                        _that.$http.post(newActivityApi.EDITNAVIGATIONSET, {
                            apiServer: 'apiServer',
                            data: Object.assign({
                                ActivityId: _that.Activityid || ''
                            }, data)
                        }).subscribe((res) => {
                            let _res = res&&res.data;
                            if (_that.$filterResponseCode(_res)) {
                                _that.$$saveMsg('添加成功!', {type: 'success'});
                            }
                        });
                    },
                    initCallback: (formModel, formList) => {
                        let _data = baseData && baseData.data;
                        let daultData = _data && _data.NavigationSettingInfo;
                        if (daultData) {
                            let data;
                            if (typeof daultData.IsNavigation === 'boolean') {
                                data = data || {};
                                data.IsNavigation = daultData.IsNavigation;
                            }
                            if (daultData.NavigationColorStart && daultData.NavigationColorEnd) {
                                data = data || {};
                                data.NavigationColorStart = daultData.NavigationColorStart;
                                data.NavigationColorEnd = daultData.NavigationColorEnd;
                            }

                            data && formModel.setValue(data);
                        }
                        formList = setNavList('IsNavigation', formList, formModel);
                    },
                    onsetChange: (con, formModel, formList) => {
                        let conName = con.controlConfig && con.controlConfig.controlName;
                        return setNavList(conName, formList, formModel);
                    },
                    valueCheck: (data) => {
                        if (data.IsNavigation) {
                            if (data.NavigationColorStart && !data.NavigationColorEnd) {
                                _that.$$errorMsg('请选择结束颜色');
                                return false;
                            } else if (!data.NavigationColorStart && data.NavigationColorEnd) {
                                _that.$$errorMsg('请选择开始颜色');
                                return false;
                            }
                        }
                        return true;
                    }
                });
                /**
                 * 设置添加导航
                 * @param {string} conName 表单项名称
                 * @param {Array} formList 表单项名称
                 * @param {formGroup} formModel 表单项名称
                 * @returns {Array}
                 */
                function setNavList(conName, formList, formModel) {
                    let val = formModel.value;
                    if (conName === 'IsNavigation') {
                        if (val.IsNavigation) {
                            if (formList.length < 2) {
                                let list = getNavConfig().slice(1);
                                formList = formList.concat(list);
                                _that.formUpdata = _that.$$form.initFormData(list);
                                formModel.merge(_that.formUpdata);
                            }
                        } else {
                            if (formList.length > 1) {
                                let delList = formList.splice(1);
                                delList.forEach(item => {
                                    formModel.removeItem(item.controlName);
                                });
                            }
                        }
                        return formList;
                    }
                }
            });
        },
        /**
         * 添加模块
         */
        addModule() {
            this.$http.post(newActivityApi.GETDICLIST, {
                apiServer: 'apiServer',
                isLoading: true,
                data: {
                    DicType: ''
                }
            }).subscribe((baseData) => {
                let moduleData = baseData && baseData.data;
                let moduleList = moduleData && moduleData.PrimaryModuleList;
                this.title = '添加模块';
                this.openPop({
                    className: null,
                    formList: getDicList(moduleList),
                    endCallback: data => {
                        if (!data || data.type === 'cancel' || data.type === 'close') {
                            return;
                        }
                        let _primary=splitModule(data.PrimaryModule);
                        let _secondary=splitModule(data.SecondaryModule);
                        let _data = {
                            ActivityId: this.Activityid || '',
                            PrimaryModuleTypeCode: _primary[0] || '',
                            PrimaryModuleTypeName: _primary[1] || '',
                            SecondaryModuleTypeCode: _secondary[0] || '',
                            SecondaryModuleTypeName: _secondary[1] || ''
                        };
                        this.$http.post(newActivityApi.ADDMODULEINFO, {
                            apiServer: 'apiServer',
                            isLoading: true,
                            data: Object.assign({}, _data)
                        }).subscribe((res) => {
                            let _res = res&&res.data;
                            let _rModuleId = _res&& _res.ModuleId;
                            if (this.$filterResponseCode(res)) {
                                this.isRefresh = true;
                                if (_rModuleId) {
                                    this.ModuleId = _rModuleId;
                                    setTimeout(() => {
                                        let goData = Object.assign({}, _data, {
                                            ModuleId: this.ModuleId
                                        });
                                        this.$emit('updateTable');
                                        this.$emit('toTabs', goData);
                                        // this.goNewTab(goData);
                                    }, 300);
                                    this.$$saveMsg('添加成功!', {type: 'success'});
                                } else {
                                    this.$$errorMsg('添加失败', {type: 'error'});
                                }
                            } else {
                                this.$$errorMsg('添加失败', {type: 'error'});
                            }
                        });
                    },
                    onsetChange: (con, formModel, formList) => {
                        let store = formModel.value;
                        if (con.controlConfig.controlName === 'PrimaryModule') {
                            let selectedCode = con.value && con.value.split('/')[0];
                            formList = getDicList(moduleList, selectedCode);
                            formList = formList.splice(0, formList.length);
                            setTimeout(() => {
                                formModel.get('SecondaryModule').setValue(formList[0].formControl[1].list[0].value);
                            }, 20);
                        }
                        return formList;
                    }
                });
            });
            /**
             * 拆分TypeCode和TypeName
             * @param {string} str 分割的字符串
             * @returns {Array}
             */
            function splitModule(str) {
                str = (str || '') +'';
                return str.split('/');
            }
        },
        /**
         * 修改顶部适配栏
         * @param {Boolean} isEdit 是否为修改
         */
        openColumn(isEdit) {
            this.getActivityInfo(this.Activityid).subscribe(data => {
                let _res = data && data.data;
                this.title = '修改顶部适配栏';
                let defaultData = _res.TopPanelSettingInfo || {};
                let groupSelect = this.formModel.get('groupSelect');
                this.openPop({
                    className: 'modifyFit',
                    formList: getChildList(5),
                    endCallback: (back) => { // 取消或保存后的回调
                        if (!back || back.type === 'cancel' || back.type === 'close') {
                            let val = groupSelect.value.filter((item) => {
                                let res = true;
                                if (!isEdit || !item) {
                                    res = (item.select != 'column');
                                }
                                return res;
                            });
                            groupSelect.setValue(val || []);
                            return;
                        }
                        this.$http.post(newActivityApi.SAVEADAPTCOLUMNINFO, {
                            apiServer: 'apiServer',
                            isLoading: true,
                            data: Object.assign({}, back, {
                                ActivityId: this.Activityid,
                                AdapterLabel: back.AdapterLabel || '1',
                                IsDividingLine: back._IsDividingLine.select,
                                DividingLineColor: back._IsDividingLine.value || ''
                            })
                        }).subscribe((res) => {
                            let _res = res && res.data;
                            if (this.$filterResponseCode(_res)) {
                                this.$$saveMsg('添加成功!', {type: 'success'});
                            }
                        });
                    },
                    initCallback: (formModel) => { // fromModel初始化
                        let res;
                        if (defaultData && (typeof defaultData.IsDividingLine !== 'undefined' &&
                                            defaultData.IsDividingLine !== '' &&
                                            defaultData.IsDividingLine !== null)) {
                            res = {
                                _IsDividingLine: {
                                    select: defaultData.IsDividingLine,
                                    value: (defaultData.DividingLineColor || '#FFFFFF') + ''
                                }
                            };
                            defaultData && formModel.setValue(Object.assign({}, defaultData, res));
                        }
                    }
                });
            });
        },
        columnModify(data) {
            if (data.extendConfig.id === 'columnModify') {
                this.openColumn(true);
            }
        },
        /**
         * 添加悬浮窗
         */
        openWindowSet() {
            let _that = this;
            let formDataCache = _that.formDataCache;
            _that.getActivityInfo(_that.Activityid).subscribe(baseData => {
                baseData = baseData && baseData.data;
                _that.title = '添加悬浮窗';
                let baseConfig = floatingUpsConfig() || [];
                let picConfig = getStaticFloating() || [];
                let jumpConfig = getfloatingUpsJumpConfig('PopupWin') || []; // 跳转配置
                let _formList = baseConfig.concat(picConfig).concat(jumpConfig);
                _that.openPop({
                    className: 'modifyFit',
                    formList: _formList,
                    endCallback: back => {
                        if (!back || back.type === 'cancel' || back.type === 'close') {
                            return;
                        }
                        _that.$http.post(newActivityApi.EDITFLOATINGINFO, {
                            apiServer: 'apiServer',
                            isLoading: true,
                            data: Object.assign({}, back, {
                                ActivityId: _that.Activityid || ''
                            })
                        }).subscribe((res) => {
                        });
                    },
                    initCallback: (formModel, formList) => {
                        // 设置添加悬浮窗默认数据
                        let defaultData = baseData.FloatingSettingInfo;
                        if (defaultData) {
                            defaultData.FloatingUpsType && cacheUpsType(defaultData);
                            defaultData.FloatingUpsJump && cacheUpsJump(defaultData);
                        }
                        let val = formModel.value;
                        let _defaultdata = formDataCache.getCache([defaultData], {key: defaultData.FloatingUpsType});
                        let _defaultdata1 = formDataCache.getCache([defaultData], {key: defaultData.FloatingUpsJump});
                        if (_defaultdata || _defaultdata1) {
                            formModel.setValue(Object.assign({}, _defaultdata && _defaultdata[0], _defaultdata1 && _defaultdata1[0]));
                        }
                        formList = setUpsTypeList('FloatingUpsType', formList, formModel);
                        formList = setUpsTypeList('FloatingUpsJump', formList, formModel);
                        return formList;
                    },
                    onsetChange: (con, formModel, formList) => { // 设置变化选项
                        formList = setUpsTypeList(con.controlConfig.controlName, formList, formModel);
                        return formList;
                    }
                });
            });
            /**
             * 设置添加导航
             * @param {string} conName 表单项名称
             * @param {Array} formList 表单项名称
             * @param {formGroup} formModel 表单
             * @returns {Array}
             */
            function setUpsTypeList (conName, formList, formModel) {
                let delList = [];
                let val = formModel.value;
                if (conName === 'FloatingUpsType') { // 悬浮窗类型发生改变的时候
                    switch (val.FloatingUpsType) {
                        case 'NoFloating': // 无悬浮窗
                            if (formList.length > 1) {
                                // reset = true;
                                delList = formList.splice(1);
                                removeList(delList, formModel);
                            }
                            break;
                        case 'StaticFloating': // 有悬浮窗
                            if (formList.length <= 1) {
                                // reset = true;
                                let addList = getStaticFloating();
                                formList = formList.concat(addList);
                                _that.formUpdata = _that.$$form.initFormData(addList);
                                formModel.merge(_that.formUpdata);
                                let defaultdata = formDataCache.getCache([val], {key: val.FloatingUpsType});
                                if (defaultdata) {
                                    formModel.setValue(defaultdata[0]);
                                }
                                let _val = formModel.value;
                                formList = setFloatingUpsJump(_val.FloatingUpsJump, formModel, formList);
                            }
                            break;
                    }
                } else { // 保存悬浮窗对应数据
                    cacheUpsType(val);

                    val = formModel.value;
                    if (conName === 'FloatingUpsJump') { // 设置悬浮窗跳转
                        formList = setFloatingUpsJump(val.FloatingUpsJump, formModel, formList);
                    } else { // 保存悬浮窗跳转数据
                        cacheUpsJump(val);
                    }
                }
                return formList;
            }

            /**
             * 从formModel移除选项
             * @param {Array} list 列表
             * @param {formGroup} formModel 表单
             */
            function removeList(list, formModel) {
                if (list && list.length >0) {
                    list.forEach(child => {
                        formModel.removeItem(child.controlName);
                    });
                }
            }
            /**
             * 设置悬浮窗跳转
             * @param {string} type 跳转类型
             * @param {formGroup} formModel 表单
             * @param {Array} formList 表单配置列表
             * @returns {Array} formList
             */
            function setFloatingUpsJump(type, formModel, formList) {
                let items;
                let val = formModel.value;
                // reset = true;
                if (formList.length > 3) {
                    let delList = formList.splice(3);
                    removeList(delList, formModel);
                }
                let list = getfloatingUpsJumpConfig(type);
                formList = formList.concat(list);
                _that.formUpdata = _that.$$form.initFormData(list);
                formModel.merge(_that.formUpdata);
                let defaultData = formDataCache.getCache([formModel.value], {key: val.FloatingUpsJump});
                defaultData && formModel.setValue(defaultData[0]);
                return formList;
            }
            /**
             * 保存悬浮窗对应数据
             * @param {Object} val 数据
                         */
            function cacheUpsType(val) {
                val.FloatingUpsType && formDataCache.setCache([{
                    FloatingUpsImgUrl: val.FloatingUpsImgUrl,
                    FloatingUpsJump: val.FloatingUpsJump,
                    FloatingUpsType: val.FloatingUpsType
                }], {key: val.FloatingUpsType});
            }
            /**
             * 保存悬浮窗对应数据
             * @param {Object} val 数据
                         */
            function cacheUpsJump(val) {
                let cacheData;
                switch (val.FloatingUpsJump) {
                    case 'PopupWin':
                        cacheData = {
                            PopUpsImgUrl: val.PopUpsImgUrl,
                            FloatingUpsJump: val.FloatingUpsJump
                        };
                        break;
                    case 'Link':
                        cacheData = {
                            MobileLink: val.MobileLink,
                            AppletsLink: val.AppletsLink,
                            QuickAppLink: val.QuickAppLink,
                            AppLink: val.AppLink,
                            FloatingUpsJump: val.FloatingUpsJump
                        };
                        break;
                }
                if (cacheData) {
                    formDataCache.setCache([cacheData], {key: val.FloatingUpsJump});
                }
            }
        }
    }
};
</script>
