package com.tuhu.springbootmybatis.vo;

import lombok.Data;
import javax.validation.constraints.NotBlank;
import javax.validation.constraints.NotNull;
import java.io.Serializable;

/**
 * @author hujie1
 * @date 2020/10/1718:31
 */
@Data
public class TTrainEntryReqPutVO implements Serializable {

    @NotBlank(message = "用户名不能为空")
    private String userName;
    @NotBlank(message = "用户身份证不能为空")
    private String userCard;
    @NotBlank(message = "用户电话不能为空")
    private String phone;
    @NotBlank(message = "用户国籍不能为空")
    private String nationality;
    @NotNull(message = "用户年龄不能为空")
    private Integer age;
    @NotNull(message = "用户健康状态不能为空")
    private Integer bodyState;
    @NotNull(message = "课程编号不能为空")
    private Long courseId;
}
