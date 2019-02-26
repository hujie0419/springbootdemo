const menus = [{
    'key': 'baseconfig',
    'name': '保养基础数据配置',
    'icon': 'ios-briefcase',
    children: [{
        'key': 'packageconfig',
        'name': '项目配置'
      },
      {
        'key': 'installtypeconfig',
        'name': '切换服务配置'
      },
      {
        'key': 'levelUpIconConfig',
        'name': '升级购图标配置'
      }
    ]
  },
  {
    'key': 'batteryconfig',
    'name': '蓄电池配置',
      'icon': 'battery-charging',
    children: [{
        'key': 'baoyangbatterycoverarea',
        'name': '保养流程蓄电池品牌覆盖区域配置'
      },
      {
        'key': 'baoyangbatterylevelup',
        'name': '蓄电池升级购配置'
      },
      {
        'key': 'baoyangCouponActivityConfig',
        'name': '蓄电池/加油卡浮动配置'
      },
      {
        'key': 'batterycouponpricedisplay',
        'name': '蓄电池券后价展示配置'
      }
    ]
    },
    {
      'key': 'vehicleconfig',
      'name': '车型配置',
        'icon': 'android-car',
      children: [{
          'key': 'vehiclearticle',
          'name': '车型数据提示'
        }
      ]
      }
];

export default menus;
