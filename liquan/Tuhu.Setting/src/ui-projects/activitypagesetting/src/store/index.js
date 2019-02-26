import Vue from 'vue';
import Vuex from 'vuex';
import * as actions from './actions';
import * as getters from './getters';
// import common from './modules/common';
import param from './modules/param';
import {userInfoStateFactory} from './modules/userInfo';

Vue.use(Vuex);

export default new Vuex.Store({
    actions,
    getters,
    modules: {
    // common,
        param,
        userInfo: userInfoStateFactory()
    },
    strict: process.env.NODE_ENV !== 'production'
});
