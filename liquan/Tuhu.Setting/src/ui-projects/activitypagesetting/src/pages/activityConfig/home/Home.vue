<template>
    <div class='general-pages'>
        <div class='general-page-title'>
            <div class='general-page-title-left'>活动列表</div>
            <div class='general-page-title-right'></div>
        </div>
        <div style="margin-bottom: 20px;">
            <el-button size="small" @click="$$tabs.addTab('activityList', {
                    title: 'activityList'
                })" >
                添加activityList
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('newActivity', {
                    title: 'newActivity'
                })" >
                添加newActivity
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('generalPage', {
                    title: 'generalPage',
                    data: {
                        ActivityId: '1234',
                        ModuleId: '1',
                        ModuleTypeCode: '1'
                    }
                })" >
                添加generalPage
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('seperateCar', {
                title: 'SeperateCar',
                    data: {
                        ActivityId: '1234',
                        ModuleId: '1',
                        ModuleTypeCode: '1'
                    }
            })" >
                添加SeperateCar
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('picture', {
                    title: 'picture',
                    data: {
                        ActivityId: '1234',
                        ModuleId: '1',
                        ModuleTypeCode: '1'
                    }
                })" >
                添加picture
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('ProductPage', {
                    title: 'ProductPage',
                    data: {
                        ActivityId: '1234',
                        ModuleId: '1',
                        ModuleTypeCode: '1'
                    }
                })" >
                添加ProductPage
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('goods', {
                title: 'goods',
                data: {
                    ActivityId: '1234',
                    ModuleId: '1',
                    ModuleTypeCode: '1'
                }
            })" >
                添加goods
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('goodsEdit', {
                title: 'goodsEdit'
            })" >
                添加goodsEdit
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('goodsSeckill', {
                    title: 'Seckill',
                    data: {
                        ActivityId: '1234',
                        ModuleId: '1',
                        ModuleTypeCode: '1'
                    }
                })" >
                添加Seckill
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('groupBooking', {
                    title: 'groupBooking',
                    data: {
                        ActivityId: '1234',
                        ModuleId: '1',
                        ModuleTypeCode: '1'
                    }
                })" >
                添加groupBooking
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('sysRec', {
                    title: 'sysRec',
                    data: {
                        ActivityId: '1234',
                        ModuleId: '1',
                        ModuleTypeCode: '1'
                    }
                })" >
                添加sysRec
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('slideConpon', {
                title: 'slideConpon',
                    data: {
                        ActivityId: '1234',
                        ModuleId: '1',
                        ModuleTypeCode: '1'
                    }
            })" >
                添加SlideConpon
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('drawLottery', {
                    title: 'drawLottery',
                    data: {
                        ActivityId: '1234',
                        ModuleId: '1',
                        ModuleTypeCode: '1'
                    }
                })" >
                添加drawLottery
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('maintainPricing', {
                    title: 'maintainPricing',
                    data: {
                        ActivityId: '1234',
                        ModuleId: '1',
                        ModuleTypeCode: '1'
                    }
                })" >
                添加maintainPricing
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('countDown', {
                    title: 'countDown',
                    data: {
                        ActivityId: '1234',
                        ModuleId: '1',
                        ModuleTypeCode: '1'
                    }
                })" >
                添加countDown
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('textLinkPage', {
                    title: 'textLinkPage',
                    data: {
                        ActivityId: '1234',
                        ModuleId: '1'
                    }
                })" >
                添加textLinkPage
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('footTabsPage', {
                    title: 'footTabsPage',
                    data: {
                        ActivityId: '1234',
                        ModuleId: '1'
                    }
                })" >
                添加footTabsPage
            </el-button>
            <el-button size="small" @click="$$tabs.addTab('videoConfigPage', {
                    title: 'videoConfigPage',
                    data: {
                        ActivityId: '1234',
                        ModuleId: '1',
                        ModuleTypeCode: '1'
                    }
                })" >
                添加videoConfigPage
            </el-button>
        </div>
    </div>
</template>

<script>
import TabPage from '../tabPage/TabPage';
export default {
    extends: TabPage,
    mounted() {
        this.$$tabs.addTab('newActivity', {
            title: 'newActivity',
            data: {
                ActivityId: '1234',
                ModuleId: '1',
                ModuleTypeCode: '1'
            }
        });
        console.log('tagOption', this.tagOption);
    }
};
</script>

<style scoped>

</style>
