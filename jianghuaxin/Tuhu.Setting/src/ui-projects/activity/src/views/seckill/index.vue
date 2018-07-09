<template>
  <div>
    <div>价格配置> 天天秒杀</div>
    <div>
      <DatePicker type="date"
                  v-model="dt"
                  :options="options1"
                  placeholder="选择日期"
                  format="yyyy-MM-dd"
                  style="width: 200px"></DatePicker>
      <Button type="primary"
              @click="search"
              icon="ios-search">筛选</Button>
    </div>
    <div style="margin-top:18px">
      <Table :data="data"
             :columns="columns"
             :loading="loading"
             border></Table>
      <div style="margin-left:500px;margin-top:10px;overflow: hidden">
        <Tag type="border"
             color="blue">{{FooterDay}}</Tag>

      </div>
    </div>
  </div>
</template>
<script>
export default {
    data () {
        return {
            columns: [
                {
                    title: "场次",
                    key: "Schedule",
                    align: "center",
                    render: (h, params) => {
                        return h('div', [
                            h('span', {

                                style: {
                                    align: 'center'

                                }

                            }, params.row.Schedule)
                        ]);
                    }
                },
                {
                    title: "周一",
                    key: "Day1",
                    align: "center",
                    render: (h, params) => {
                        var class1 = this.getclass(params.row.Day1.Status);
                        var class2 = this.getclass2(params.row.Day1.Status)
                        if (params.row.Day1.Status === 1) {
                            return h('div', [
                                h('Icon', {
                                    props: {
                                        type: 'android-add'
                                    }
                                }),
                                h('strong', {
                                    on: {
                                        click: () => {
                                            this.toedit(params.row.Day1);
                                        }
                                    }
                                }, "新建")
                            ]);
                        } else {
                            if (params.row.Day1.IsDefault) {
                                return h("div",
                                    {
                                        style: class2
                                    }, [
                                        h(
                                            "div", [h('span',
                                                {
                                                    on: {
                                                        click: () => {
                                                            this.toedit(params.row.Day1);
                                                        }
                                                    }
                                                },
                                                params.row.Day1.StrStatus
                                            )]
                                        ),
                                        h(
                                            "div", [h('span',
                                                '默认配置'
                                            )]
                                        )
                                    ])
                            } else {
                                return h("div",
                                    {
                                        style: class1
                                    }, [
                                        h(
                                            "div", [h('span',
                                                {
                                                    on: {
                                                        click: () => {
                                                            this.toedit(params.row.Day1);
                                                        }
                                                    }
                                                },
                                                params.row.Day1.StrStatus
                                            )]
                                        ),
                                        h(
                                            "div", [h('span',
                                                params.row.Day1.Count + '个产品'
                                            )]
                                        )
                                    ])
                            }
                        }
                    }
                },
                {
                    title: "周二",
                    key: "Day2",
                    align: "center",
                    render: (h, params) => {
                        var class1 = this.getclass(params.row.Day2.Status);
                        var class2 = this.getclass2(params.row.Day2.Status);
                        if (params.row.Day2.Status === 1) {
                            return h('div', [
                                h('Icon', {
                                    props: {
                                        type: 'android-add'
                                    }
                                }),
                                h('strong', {
                                    on: {
                                        click: () => {
                                            this.toedit(params.row.Day2);
                                        }
                                    }
                                }, "新建")
                            ]);
                        } else {
                            if (params.row.Day2.IsDefault) {
                                return h("div",
                                    {
                                        style: class2
                                    }, [
                                        h(
                                            "div", [h('span',
                                                {
                                                    on: {
                                                        click: () => {
                                                            this.toedit(params.row.Day2);
                                                        }
                                                    }
                                                },
                                                params.row.Day2.StrStatus
                                            )]
                                        ),
                                        h(
                                            "div", [h('span',
                                                '默认配置'
                                            )]
                                        )
                                    ])
                            } else {
                                return h("div",
                                    {
                                        style: class1
                                    }, [
                                        h(
                                            "div", [h('span',
                                                {
                                                    on: {
                                                        click: () => {
                                                            this.toedit(params.row.Day2);
                                                        }
                                                    }
                                                },
                                                params.row.Day2.StrStatus
                                            )]
                                        ),
                                        h(
                                            "div", [h('span',
                                                params.row.Day2.Count + '个产品'
                                            )]
                                        )
                                    ])
                            }
                        }
                    }
                },
                {
                    title: "周三",
                    key: "Day3",
                    align: "center",
                    render: (h, params) => {
                        var class1 = this.getclass(params.row.Day3.Status);
                        var class2 = this.getclass2(params.row.Day3.Status);
                        if (params.row.Day3.Status === 1) {
                            return h('div', [
                                h('Icon', {
                                    props: {
                                        type: 'android-add'
                                    }
                                }),
                                h('strong', {
                                    on: {
                                        click: () => {
                                            this.toedit(params.row.Day3);
                                        }
                                    }
                                }, "新建")
                            ]);
                        } else {
                            if (params.row.Day3.IsDefault) {
                                return h("div",
                                    {
                                        style: class2
                                    }, [
                                        h(
                                            "div", [h('span',
                                                {
                                                    on: {
                                                        click: () => {
                                                            this.toedit(params.row.Day3);
                                                        }
                                                    }
                                                },
                                                params.row.Day3.StrStatus
                                            )]
                                        ),
                                        h(
                                            "div", [h('span',
                                                '默认配置'
                                            )]
                                        )
                                    ])
                            } else {
                                return h("div",
                                    {
                                        style: class1
                                    }, [
                                        h(
                                            "div", [h('span',
                                                {
                                                    on: {
                                                        click: () => {
                                                            this.toedit(params.row.Day3);
                                                        }
                                                    }
                                                },
                                                params.row.Day3.StrStatus
                                            )]
                                        ),
                                        h(
                                            "div", [h('span',
                                                params.row.Day3.Count + '个产品'
                                            )]
                                        )
                                    ])
                            }
                        }
                    }
                },
                {
                    title: "周四",
                    key: "Day4",
                    align: "center",
                    render: (h, params) => {
                        var class1 = this.getclass(params.row.Day4.Status);
                        var class2 = this.getclass2(params.row.Day4.Status);
                        if (params.row.Day4.Status === 1) {
                            return h('div', [
                                h('Icon', {
                                    props: {
                                        type: 'android-add'
                                    }
                                }),
                                h('strong', {
                                    on: {
                                        click: () => {
                                            this.toedit(params.row.Day4);
                                        }
                                    }
                                }, "新建")
                            ]);
                        } else {
                            if (params.row.Day4.IsDefault) {
                                return h("div",
                                    {
                                        style: class2
                                    }, [
                                        h(
                                            "div", [h('span',
                                                {
                                                    on: {
                                                        click: () => {
                                                            this.toedit(params.row.Day4);
                                                        }
                                                    }
                                                },
                                                params.row.Day4.StrStatus
                                            )]
                                        ),
                                        h(
                                            "div", [h('span',
                                                '默认配置'
                                            )]
                                        )
                                    ])
                            } else {
                                return h("div",
                                    {
                                        style: class1
                                    }, [
                                        h(
                                            "div", [h('span',
                                                {
                                                    on: {
                                                        click: () => {
                                                            this.toedit(params.row.Day4);
                                                        }
                                                    }
                                                },
                                                params.row.Day4.StrStatus
                                            )]
                                        ),
                                        h(
                                            "div", [h('span',
                                                params.row.Day4.Count + '个产品'
                                            )]
                                        )
                                    ])
                            }
                        }
                    }
                }, {
                    title: "周五",
                    key: "Day5",
                    align: "center",
                    render: (h, params) => {
                        var class1 = this.getclass(params.row.Day5.Status);
                        var class2 = this.getclass2(params.row.Day5.Status);
                        if (params.row.Day5.Status === 1) {
                            return h('div', [
                                h('Icon', {
                                    props: {
                                        type: 'android-add'
                                    }
                                }),
                                h('strong', {
                                    on: {
                                        click: () => {
                                            this.toedit(params.row.Day5);
                                        }
                                    }
                                }, "新建")
                            ]);
                        } else {
                            if (params.row.Day5.IsDefault) {
                                return h("div",
                                    {
                                        style: class2
                                    }, [
                                        h(
                                            "div", [h('span',
                                                {
                                                    on: {
                                                        click: () => {
                                                            this.toedit(params.row.Day5);
                                                        }
                                                    }
                                                },
                                                params.row.Day5.StrStatus
                                            )]
                                        ),
                                        h(
                                            "div", [h('span',
                                                '默认配置'
                                            )]
                                        )
                                    ])
                            } else {
                                return h("div",
                                    {
                                        style: class1
                                    }, [
                                        h(
                                            "div", [h('span',
                                                {
                                                    on: {
                                                        click: () => {
                                                            this.toedit(params.row.Day5);
                                                        }
                                                    }
                                                },
                                                params.row.Day5.StrStatus
                                            )]
                                        ),
                                        h(
                                            "div", [h('span',
                                                params.row.Day5.Count + '个产品'
                                            )]
                                        )
                                    ])
                            }
                        }
                    }
                }, {
                    title: "周六",
                    key: "Day6",
                    align: "center",
                    render: (h, params) => {
                        var class1 = this.getclass(params.row.Day6.Status);
                        var class2 = this.getclass2(params.row.Day6.Status);
                        if (params.row.Day6.Status === 1) {
                            return h('div', [
                                h('Icon', {
                                    props: {
                                        type: 'android-add'
                                    }
                                }),
                                h('strong', {
                                    on: {
                                        click: () => {
                                            this.toedit(params.row.Day6);
                                        }
                                    }
                                }, "新建")
                            ]);
                        } else {
                            if (params.row.Day6.IsDefault) {
                                return h("div",
                                    {
                                        style: class2
                                    }, [
                                        h(
                                            "div", [h('span',
                                                {
                                                    on: {
                                                        click: () => {
                                                            this.toedit(params.row.Day6);
                                                        }
                                                    }
                                                },
                                                params.row.Day6.StrStatus
                                            )]
                                        ),
                                        h(
                                            "div", [h('span',
                                                '默认配置'
                                            )]
                                        )
                                    ])
                            } else {
                                return h("div",
                                    {
                                        style: class1
                                    }, [
                                        h(
                                            "div", [h('span',
                                                {
                                                    on: {
                                                        click: () => {
                                                            this.toedit(params.row.Day6);
                                                        }
                                                    }
                                                },
                                                params.row.Day6.StrStatus
                                            )]
                                        ),
                                        h(
                                            "div", [h('span',
                                                params.row.Day6.Count + '个产品'
                                            )]
                                        )
                                    ])
                            }
                        }
                    }
                },
                {
                    title: "周日",
                    key: "Day7",
                    align: "center",
                    render: (h, params) => {
                        var class1 = this.getclass(params.row.Day7.Status);
                        var class2 = this.getclass2(params.row.Day7.Status);
                        if (params.row.Day7.Status === 1) {
                            return h('div', [
                                h('Icon', {
                                    props: {
                                        type: 'android-add'
                                    }
                                }),
                                h('strong', {
                                    on: {
                                        click: () => {
                                            this.toedit(params.row.Day7);
                                        }
                                    }
                                }, "新建")
                            ]);
                        } else {
                            if (params.row.Day7.IsDefault) {
                                return h("div",
                                    {
                                        style: class2
                                    }, [
                                        h(
                                            "div", [h('span',
                                                {
                                                    on: {
                                                        click: () => {
                                                            this.toedit(params.row.Day7);
                                                        }
                                                    }
                                                },
                                                params.row.Day7.StrStatus
                                            )]
                                        ),
                                        h(
                                            "div", [h('span',
                                                '默认配置'
                                            )]
                                        )
                                    ])
                            } else {
                                return h("div",
                                    {
                                        style: class1
                                    }, [
                                        h(
                                            "div", [h('span',
                                                {
                                                    on: {
                                                        click: () => {
                                                            this.toedit(params.row.Day7);
                                                        }
                                                    }
                                                },
                                                params.row.Day7.StrStatus
                                            )]
                                        ),
                                        h(
                                            "div", [h('span',
                                                params.row.Day7.Count + '个产品'
                                            )]
                                        )
                                    ])
                            }
                        }
                    }
                },
                {
                    title: "默认配置",
                    key: "Default",
                    align: "center",
                    render: (h, params) => {
                        var class1 = this.getclass(params.row.Default.Status);
                        if (params.row.Default.Status === 1) {
                            return h('div', [
                                h('Icon', {
                                    props: {
                                        type: 'android-add'
                                    }
                                }),
                                h('strong', {
                                    on: {
                                        click: () => {
                                            this.toedit(params.row.Default);
                                        }
                                    }
                                }, "新建")
                            ]);
                        } else {
                            return h("div",
                                {
                                    style: class1
                                }, [
                                    h(
                                        "div", [h('span',
                                            {
                                                on: {
                                                    click: () => {
                                                        this.toedit(params.row.Default);
                                                    }
                                                }
                                            },
                                            params.row.Default.StrStatus
                                        )]
                                    ),
                                    h(
                                        "div", [h('span',
                                            params.row.Default.Count + '个产品'
                                        )]
                                    )
                                ])
                        }
                    }
                }
            ],
            data: [],
            loading: true,
            page: {
                total: 5,
                current: 1,
                pageSize: 5
            },
            dt: new Date(),
            FooterDay: "",
            options1: {
                shortcuts: [
                    {
                        text: 'Today',
                        value () {
                            return new Date();
                        },
                        onClick: (picker) => {
                            this.$Message.info('Click today');
                        }
                    },
                    {
                        text: 'Yesterday',
                        value () {
                            const date = new Date();
                            date.setTime(date.getTime() - 3600 * 1000 * 24);
                            return date;
                        },
                        onClick: (picker) => {
                            this.$Message.info('Click yesterday');
                        }
                    },
                    {
                        text: 'One week',
                        value () {
                            const date = new Date();
                            date.setTime(date.getTime() - 3600 * 1000 * 24 * 7);
                            return date;
                        },
                        onClick: (picker) => {
                            this.$Message.info('Click a week ago');
                        }
                    }
                ]
            }
        }
    },
    created () {
        this.loadData();
    },
    methods: {
        search () {
            this.loadData();
        },
        loadData () {
            var _this = this;
            let params = {
                dt: _this.dt.toString()
            };
            this.ajax
                .get("/Seckill/GetSeckillList", {
                    params: params
                })
                .then(response => {
                    this.data = response.data.result;
                    this.FooterDay = response.data.FooterDay;
                    this.loading = false
                });
        },
        toedit (day) {
            debugger;
            let routeData = this.$router.resolve({ 'name': 'seckillEdit', query: { activityId: day.ActivityId, schedule: day.Schedule, shortDate: day.ShortDate, isDefault: day.IsDefault, strStatus: day.StrStatus } });
            window.open(routeData.href, '_blank');
        },
        getclass (state) {
            switch (state) {
                case 1:
                    return { "background-color": "white", "color": "black", "text-align": "center", "line-height": "45px" };
                case 2:
                    return { "background-color": "white", "color": "red", "text-align": "center", "line-height": "45px" };
                case 3:
                    return { "background-color": "#00FA9A", "color": "black", "text-align": "center", "line-height": "45px" };
                case 4:
                    return { "background-color": "yellow", "color": "red", "text-align": "center", "line-height": "45px" };
                case 5:
                    return { "background-color": "#00FA9A", "color": "black", "font-weight": "bold", "text-align": "center", "line-height": "45px" }
                case 6:
                    return { "background-color": "#cccccc", "color": "black", "text-align": "center", "line-height": "45px" }
                default:
                    break;
            }
        },
        getclass2 (state) {
            switch (state) {
                case 5:
                    return { "background-color": "#00FA9A", "color": "red", "text-align": "center", "line-height": "45px" }
                case 6:
                    return { "background-color": "#cccccc", "color": "red", "text-align": "center", "line-height": "45px" }
                default:
                    break;
            }
        }
    }
}
</script>
<style>
body .ivu-table-cell {
  /* position: fixed !important; */
  padding-left: 0px;
  padding-right: 0px;
}
</style>
