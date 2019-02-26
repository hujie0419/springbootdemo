import * as types from '../mutation-types';
/**
 * 用户信息store
 *
 * @export
 * @returns {object}
 */
export function userInfoStateFactory() {
  const userInfo = {
    actions: {
      userInfo({commit, state}, userinfo) {
        commit(types.USER_INFO, userinfo);
      }
    },
    getters: {
      userInfo(state) {
        // console.log('getters', state.userInfo);
        return state.data;
      }
    },
    state: {
      data: {}
    },
    mutations: {
      [types.USER_INFO](state, userinfo) {
        // console.log('mutations.userinfo', state);
        state.data = userinfo;
      }
    }
  };

  return userInfo;
}