const menus = [
    { 
      'key': '1',
      'name': '保养活动',
      'icon': 'ios-speedometer',
      children: [
        { 'key': 'demo', 'name': '配置' },
        { 'key': '1-2', 'name': '报表' }
      ]
    },
    { 
      'key': '2',
      'name': '大客户保养',
      'icon': 'ios-calendar',
      children: [
        { 'key': '2-1', 'name': '套餐配置' },
        { 'key': '2-2', 'name': '发码配置' }
      ]
    },
    { 
      'key': 'jobgroup',
      'name': '优先级配置',
      'icon': 'ios-briefcase',
      children: [
        { 'key': 'jobgrouplist', 'name': '通用配置' },
        { 'key': '3-2', 'name': '特殊车型配置' },
        { 'key': '3-3', 'name': '监控' }
      ]
    }
  ];

export default menus;
