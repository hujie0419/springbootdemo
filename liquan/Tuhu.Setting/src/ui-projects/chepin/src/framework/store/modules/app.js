const app = {
    namespaced: true,
    state: {
        menu: {
            menus: [],
            openedmenus: [],
            shrinked: []
        },
        user: {
            name: "",
            permission: []
        }
    },
    mutations: {
        init(state, menus) {
            state.menus = menus;
        }
    }
};

export default app;