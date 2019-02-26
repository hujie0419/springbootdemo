import Icon from './Icon';
import Layout from './Layout';

const options = {
  Icon,
  Layout
};

options.install = (Vue) => {
  for (let component in options) {
    const componentInstaller = options[component];

    if (componentInstaller && component !== 'install') {
      Vue.use(componentInstaller);
    }
  }
};

export default options;