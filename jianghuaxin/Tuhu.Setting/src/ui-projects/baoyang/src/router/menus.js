const menus = [{ 
      'key': 'baseconfig',
      'name': '保养基础数据配置',
      'icon': 'ios-briefcase',
      children: [
        { 'key': 'packageconfig', 'name': '项目配置' },
        { 'key': 'installtypeconfig', 'name': '切换服务配置' },
        { 'key': 'levelUpIconConfig', 'name': '升级购图标配置' }
      ]
    },
    { 
      'key': 'batteryconfig',
      'name': '蓄电池配置',
      'icon': 'ios-briefcase',
      children: [
        { 'key': 'baoyangbatterycoverarea', 'name': '保养流程蓄电池品牌覆盖区域配置' }
      ]
    }
  ];

export default menus;
