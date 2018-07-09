const menus = [
  {
    'key': '1',
    'name': '价格配置',
    'icon': 'ios-speedometer',
    children: [
      { 'key': 'seckillindex', 'name': '天天秒杀' }
    ]
  },
  {
    'key': '2',
    'name': '世界杯竞猜游戏配置',
    children: [
      { 'key': 'Question', 'name': '答题配置' },
      { 'key': 'Prize', 'name': '兑换物配置' }
    ]
  },
  {
    'key': '3',
    'name': '黑名单配置',
    'icon': 'ios-speedometer',
    children: [
      { 'key': 'PintuanBlockList', 'name': '拼团黑名单配置' }
    ]
  }
];

export default menus;
