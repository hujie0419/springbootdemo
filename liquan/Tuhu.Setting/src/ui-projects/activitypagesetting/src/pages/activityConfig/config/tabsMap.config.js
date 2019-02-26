/**
 * 生成配置参数
 * @param {string} typeCode 类型码
 * @param {Object} options 第二个参数的补充选项
 * @returns {Array}
 */
export function genConfig(typeCode, options) {
    let res = [];
    let params = tabsMap(typeCode);
    if (params.length > 0) {
        params[0] && res.push(params[0]);
        if (params[1] instanceof Object) {
            res.push(Object.assign(params[1], options));
        }
    }
    return res;
}
/**
 * tab映射
 * @param {string} typeCode 类型码
 * @returns {Array} 参数列表
 */
function tabsMap(typeCode) {
    let res = [];
    switch (typeCode) {
        case 'GENERALHEAD':
            res=['generalPage', {
                title: '通用活动页'
            }];
            break;
        case 'SECTIONALHEADER':
            res=['seperateCar', {
                title: '分车型活动页'
            }];
            break;
        case 'IMAGELINKCOLUMNS':
            res=['picture', {
                title: '一行1~4列'
            }];
            break;
        case 'IMAGELINKPRODUCT':
            res=['ProductPage', {
                title: '一图3产品'
            }];
            break;
        case 'ORDINARYPRODUCT':
            res=['goods', {
                title: '普通商品'
            }];
            break;
        case 'SECONDSKILL':
            res=['goodsSeckill', {
                title: '秒杀商品模块'
            }];
            break;
        case 'SPELLGROUP':
            res=['groupBooking', {
                title: '拼团'
            }];
            break;
        case 'SYSRECOMMENDED':
            res=['sysRec', {
                title: '系统推荐'
            }];
            break;
        case 'SLIDINGCOUPON':
            res=['slideConpon', {
                title: '滑动优惠券'
            }];
            break;
        case 'XINDATURN':
            res=['drawLottery', {
                LotteryType: 'NewBigHit',
                title: '新大翻盘'
            }];
            break;
        case 'ERNIE':
            res=['drawLottery', {
                LotteryType: 'Ernie',
                title: '摇奖机'
            }];
            break;
        case 'ANSWERLUCKYDRAW':
            res=['drawLottery', {
                LotteryType: 'AnswerLucky',
                title: '答题抽奖'
            }];
            break;
        case 'ENVELOPEDRAW':
            res=['drawLottery', {
                LotteryType: 'EnvelopeDraw',
                title: '红包抽奖'
            }];
            break;
        case 'MAINTENANCEPRICING':
            res=['maintainPricing', {
                title: '保养定价'
            }];
            break;
        case 'COUNTDOWN':
            res=['countDown', {
                title: '倒计时'
            }];
            break;
        case 'WRITING':
            res=['textLinkPage', {
                title: '轮播文字链'
            }];
            break;
        case 'BOTTOMTAB':
            res=['footTabsPage', {
                title: '底部tab'
            }];
            break;
        case 'VIDEO':
            res=['videoConfigPage', {
                title: '视频'
            }];
            break;
        case 'MAINTENANCEVEHICLE':
            res=['maintenanceVehiclePage', {
                title: '保养分车型'
            }];
            break;
        default:
            break;
    }
    return res;
}
