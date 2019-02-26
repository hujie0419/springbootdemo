const menus = [
    { 
      'key': '1',
      'name': '大众点评美容团购',
      'icon': 'ios-speedometer',
      children: [
        { 'key': 'gpconfig', 'name': '团购配置' },
        { 'key': 'shopconfig', 'name': '门店维护' }
      ]
    },
    { 
      'key': '1',
      'name': '门店同步',
      'icon': 'ios-reverse-camera-outline',
      children: [      
        { 'key': 'shopsync', 'name': '门店同步' },
        { 'key': 'pingansyncconfig', 'name': '平安同步门店配置' },
        { 'key': 'thirdpartycodeconfig', 'name': '三方码配置' }
      ]
    }
  ];

export default menus;
