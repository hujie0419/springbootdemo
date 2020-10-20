package com.tuhu.springbootmybatis.tools;

import lombok.Data;
import lombok.ToString;

/**
 * @author hujie1
 * @date 2020/9/2817:36
 */
@Data
@ToString
public class APIResponse<T> {

    //0-成功  1-失败
    private String code;
    private T data;
    private String msg;

    /**
     * 默认无参 返回成功
     */
    public APIResponse() {
        this.code = "0";
        this.msg = "操作成功";
    }

    /**
     * 自定义
     * @param code
     * @param data
     * @param msg
     */
    public APIResponse(String code, T data, String msg) {
        this.code = code;
        this.data = data;
        this.msg = msg;
    }

}
