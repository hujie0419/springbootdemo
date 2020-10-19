package com.tuhu.springbootmybatis.vo;

import com.fasterxml.jackson.annotation.JsonFormat;
import lombok.Data;
import lombok.ToString;

import java.util.Date;

/**
 * @author hujie1
 * @date 2020/10/1717:49
 */
@Data
@ToString
public class TTrainCourseRespGetVO {

    private Long id;
    private String courseName;
    private String courseContent;
    private String lecturer;
    @JsonFormat(locale = "zh", timezone = "GMT+8", pattern = "yyyy-MM-dd HH:mm:ss")
    private Date openingTime;
}
