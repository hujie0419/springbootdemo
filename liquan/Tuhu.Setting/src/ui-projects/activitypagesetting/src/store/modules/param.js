import Vue from 'vue';
import * as types from '../mutation-types';

const state = {
    editOrderId: '',
    againOrderId: ''
};

const getters = {
    editOrderId: state => state.editOrderId,
    againOrderId: state => state.againOrderId
};

const actions = {
    setParam ({commit}, [key, value]) {
        commit(types.SET_PARAM, [key, value]);
    }
};

const mutations = {
    [types.SET_PARAM] (state, [key, value]) {
        Vue.set(state, key, value);
    }
};

export default {
    state,
    getters,
    actions,
    mutations
};
