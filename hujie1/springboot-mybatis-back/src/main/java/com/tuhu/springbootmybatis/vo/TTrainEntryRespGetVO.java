package com.tuhu.springbootmybatis.vo;

import lombok.Data;
import lombok.ToString;

import java.util.Date;

/**
 * @author hujie1
 * @date 2020/10/1910:25
 */
@Data
@ToString
public class TTrainEntryRespGetVO {

    //报名人
    private String userName;
    //课程名称
    private String courseName;
    //授课人
    private String lecturer;
    //开课日期
    private Date openingTime;
    //报名时间
    private Date entryCreateTime;
    //审核状态
    private Integer state;
    //审核人
    private String reviewer;
}
