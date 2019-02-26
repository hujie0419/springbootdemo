const menus = [{
        'key': '1',
        'name': '黑名单配置',
        children: [{
            'key': 'PintuanBlockList',
            'name': '拼团黑名单配置'
        }]
    },
    {
        'key': '2',
        'name': '途虎挪车',
        children: [{
            'key': 'MoveCarQRCode',
            'name': '二维码生成记录'
        }]
    },
    {
        'key': '3',
        'name': '意见反馈',
        children: [{
                'key': 'QuestionTypeList',
                'name': '问题类型'
            },
            {
                'key': 'FeedbackList',
                'name': '意见反馈'
            }
        ]
    }, {
        'key': '4',
        'name': '产品限购管理',
        children: [{
            'key': 'tire',
            'name': '轮胎类目'
        }, {
            'key': 'baoyang',
            'name': '保养类目'
        }, {
            'key': 'carproduct',
            'name': '车品类目'
        }, {
            'key': 'meirong',
            'name': '美容类目'
        }]
    }
];

export default menus;