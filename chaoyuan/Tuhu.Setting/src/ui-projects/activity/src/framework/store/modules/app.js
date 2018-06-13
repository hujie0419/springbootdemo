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
        init (state, menus) {
            console.log(menus);
            state.menus = menus;
        }
    }
};

export default app;
