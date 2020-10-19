package com.tuhu.springbootmybatis.vo;

import com.fasterxml.jackson.annotation.JsonFormat;
import lombok.Data;

import javax.validation.constraints.NotBlank;
import javax.validation.constraints.NotNull;
import java.io.Serializable;
import java.util.Date;

/**
 * @author hujie1
 * @date 2020/10/1713:30
 */
@Data
public class TTrainCourseReqSaveVO implements Serializable {

    private static final long serialVersionUID = 1L;
    @NotBlank(message="课程名称不能为空")
    private String courseName;
    @NotBlank(message="课程内容不能为空")
    private String courseContent;
    @NotBlank(message="授课人不能为空")
    private String lecturer;
    @NotNull(message="日期不能为空")
    @JsonFormat(locale="zh", timezone="GMT+8", pattern="yyyy-MM-dd HH:mm:ss")
    private Date openingTime;
}
