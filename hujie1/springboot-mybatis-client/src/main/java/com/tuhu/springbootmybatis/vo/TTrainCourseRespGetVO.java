package com.tuhu.springbootmybatis.vo;

import com.fasterxml.jackson.annotation.JsonFormat;
import lombok.Data;
import lombok.ToString;

import java.util.Date;

/**
 * @author hujie1
 * @date 2020/10/1711:20
 */
@Data
@ToString
public class TTrainCourseRespGetVO {
    //课程id
    private Long id;
    //课程名称
    private String courseName;
    //课程内容
    private String courseContent;
    //授课人
    private String lecturer;
    //开课日期
    @JsonFormat(locale = "zh", timezone = "GMT+8", pattern = "yyyy-MM-dd HH:mm:ss")
    private Date openingTime;
}
