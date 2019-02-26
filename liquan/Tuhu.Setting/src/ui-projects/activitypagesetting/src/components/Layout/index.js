import Levelbar from './Levelbar';
import Navbar from './Navbar';
import SideBar from './SideBar';
import AppMain from './AppMain';

const options = {
    Levelbar,
    Navbar,
    SideBar,
    AppMain
};

options.install = (Vue) => {
    for (let component in options) {
        const componentInstaller = options[component];

        if (componentInstaller && component !== 'install') {
            Vue.use(componentInstaller);
        }
    }

    Vue.component('layout', () => import('./Layout.vue'));
};

export default options;
