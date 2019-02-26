<template>

   <div>
   <div v-if="log.data.length>0">
        <Table stripe border :columns="log.columns" :data="log.data"></Table>
   </div>

    </div>
</template>
<script>
    export default {
         props: {
            row: Object
        },
      data () {
          return {
log: {
     data: [],
            columns: [{
                title: "操作内容",
                key: "Property",
                align: "center"
              },
              {
                title: "操作前",
                key: "OldValue",
                align: "center"
              },
              {
                title: "操作后",
                key: "NewValue",
                align: "center"
              }
            ]
}
          }
      },
      created () {
            this.ajax
          .post("/salepromotionactivity/GetOperationLogDetailList", {
             FPKID: this.row.PKID
          })
          .then(response => {
           this.log.data = response.data.List;
          });
// 
      },
      methods: {
loadLogDetail (id) {
}
      }
    };
</script>
