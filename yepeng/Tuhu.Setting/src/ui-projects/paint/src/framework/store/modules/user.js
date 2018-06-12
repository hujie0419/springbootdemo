// import util from "@/framework/libs/util.js"

const user = {
    namespaced: true,
    state: {
        name: "",
        permissions: []
    },
    mutations: {
        logout (state) {
        },
        init (state, userinfo) {
            state.name = userinfo.name;
            state.permissions = userinfo.permissions;
        }
    }
};

export default user;
