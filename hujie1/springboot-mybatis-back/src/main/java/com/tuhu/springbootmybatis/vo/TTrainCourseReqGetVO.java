package com.tuhu.springbootmybatis.vo;

import com.fasterxml.jackson.annotation.JsonFormat;
import lombok.Data;
import lombok.ToString;

import javax.validation.constraints.NotBlank;
import javax.validation.constraints.NotNull;
import java.util.Date;

/**
 * @author hujie1
 * @date 2020/10/1910:11
 */
@Data
@ToString
public class TTrainCourseReqGetVO {

    @NotBlank(message="课程名称不能为空")
    private String courseName;
    @NotBlank(message="授课人不能为空")
    private String lecturer;
    @NotNull(message="日期不能为空")
    @JsonFormat(locale = "zh", timezone = "GMT+8", pattern = "yyyy-MM-dd HH:mm:ss")
    private Date openingTime;
}
