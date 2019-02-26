import * as types from '../mutation-types';

const state = {
    tabs: [],
    comps: [],
    dashboardActive: true,
    viewNames: {
        entry: 'entry',
        distribution: 'distribution',
        review: 'review',
        project: 'project',
        businessarea: 'businessarea',
        except: 'except',
        schedule: 'schedule',
        receipt: 'receipt',
        plan: 'plan',
        setup: 'setup',
        track: 'track',
        roads: 'roads',
        orders: 'orders',
        number: 'number',
        perform: 'perform',
        our: 'our',
        machine: 'machine',

        forexcept: 'forexcept',
        forereceipt: 'forereceipt',
        foreaccount: 'foreaccount',
        forentry: 'forentry',
        forecheck: 'forecheck',
        forelook: 'forelook',
        foreability: 'foreability',
        foreour: 'foreour',
        foreproject: 'foreproject'
    },
    user: {
        user: 'sean',
        name: '肖林',
        avatar: '',
        introduction: '介绍'
    }
};

const getters = {
    dashboardActive: state => state.dashboardActive,
    allTabs: state => state.tabs,
    allComps: state => state.comps,
    entryViewName: state => state.viewNames.entry,
    distributionViewName: state => state.viewNames.distribution,
    reviewViewName: state => state.viewNames.review,
    accountViewName: state => state.viewNames.account,
    projectViewName: state => state.viewNames.project,
    businessareaViewName: state => state.viewNames.businessarea,
    exceptViewName: state => state.viewNames.except,
    scheduleViewName: state => state.viewNames.schedule,
    receiptViewName: state => state.viewNames.receipt,
    planViewName: state => state.viewNames.plan,
    setupViewName: state => state.viewNames.setup,
    abilityViewName: state => state.viewNames.ability,
    trackViewName: state => state.viewNames.track,
    roadsViewName: state => state.viewNames.roads,
    ordersViewName: state => state.viewNames.orders,
    numberViewName: state => state.viewNames.number,
    performViewName: state => state.viewNames.perform,
    messageViewName: state => state.viewNames.message,
    // accountViewName: state => state.viewNames.account,
    permissionViewName: state => state.viewNames.permission,
    machineViewName: state => state.viewNames.machine,
    forexceptViewName: state => state.viewNames.forexcept,
    forereceiptViewName: state => state.viewNames.forereceipt,
    foreaccountViewName: state => state.viewNames.foreaccount,
    forentryViewName: state => state.viewNames.forentry,
    forelookViewName: state => state.viewNames.forelook,
    forecheckViewName: state => state.viewNames.forecheck,
    foreabilityViewName: state => state.viewNames.foreability,

    avatar: state => state.user.avatar,
    name: state => state.user.name,
    user: state => state.user.user,
    introduction: state => state.user.introduction
};

const actions = {
    addToTab ({commit}, tab) {
        commit(types.ADD_TO_TAB, tab);
    },
    removeFromTab ({commit}, tab) {
        commit(types.REMOVE_FROM_TAB, tab);
    },
    setDashboardActive ({commit}) {
        commit(types.SET_DASHBOARD_ACTIVE);
    },
    setTabActive ({commit}, tab) {
        commit(types.SET_TAB_ACTIVE, tab);
    },
    setTabViewName ({commit}, [bizName, viewName]) {
        commit(types.SET_TAB_VIEW_NAME, [bizName, viewName]);
    },
    setTabLabel ({commit}, [tabName, label]) {
        commit(types.SET_TAB_LABEL, [tabName, label]);
    },
    setTabRefresh({commit}, componentName) {
        commit(types.SET_TAB_REFRESH, componentName);
    }
};

const mutations = {
    [types.ADD_TO_TAB] (state, tab) {
    // Dashboard is a static tab, prevent add and remove
        if (tab.name == 'dashboard') {
            return;
        }
        let t = state.tabs.find((tempTab) => {
            return tempTab.name == tab.name;
        });
        // Add if not exists
        if (!t) {
            state.tabs.push(tab);

            // Refresh component by set needRefresh = true;
            let comp = state.comps.find((comp) => {
                return comp.name == tab.name;
            });
            if (!comp) {
                tab.needRefresh = true;
                state.comps.push(tab);
            } else {
                comp.needRefresh = true;
            }
        } else {
            if (t.label !== tab.label) {
                t.label = tab.label;
            }
        }
    },
    [types.REMOVE_FROM_TAB] (state, tab) {
        if (tab.name == 'dashboard') {
            return;
        }

        // Destroy the component
        let comp = state.comps.find((comp) => {
            return comp.name == tab.name;
        });
        if (comp) {
            comp.needRefresh = false;
        }

        state.tabs.splice(state.tabs.indexOf(tab), 1);
    },
    [types.SET_DASHBOARD_ACTIVE] (state) {
        state.tabs.forEach(function (tabTemp) {
            tabTemp.isActive = false;
        });
        state.dashboardActive = true;
    },
    [types.SET_TAB_ACTIVE] (state, tab) {
        state.dashboardActive = false;
        state.tabs.forEach((item) => {
            if (item.name == tab.name) {
                item.isActive = true;
            } else {
                item.isActive = false;
            }
        });
        state.comps.forEach((item) => {
            if (item.name == tab.name) {
                item.isActive = true;
            } else {
                item.isActive = false;
            }
        });
    },
    [types.SET_TAB_VIEW_NAME] (state, [bizName, viewName]) {
        state.viewNames[bizName] = viewName;
    },
    [types.SET_TAB_LABEL] (state, [tabName, label]) {
        let t = state.tabs.find((tempTab) => {
            return tempTab.name == tabName;
        });
        if (t) {
            t.label = label;
        }
    },
    [types.SET_TAB_REFRESH] (state, componentName) {
        state.comps.map((tempTab, index) => {
            if (tempTab.name == componentName) {
                state.comps.splice(index, 1);
            }
        });
    }
};

export default {
    state,
    getters,
    actions,
    mutations
};
