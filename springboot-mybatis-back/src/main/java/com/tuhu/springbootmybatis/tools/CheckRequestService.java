package com.tuhu.springbootmybatis.tools;

import org.springframework.validation.BindingResult;
import org.springframework.validation.FieldError;

/**
 * @author hujie1
 * @date 2020/10/1716:13
 */
public class CheckRequestService {

    public static StringBuffer AnalyseBindingResult(BindingResult result){
        StringBuffer stringBuffer = new StringBuffer();
        result.getAllErrors().forEach((error) -> {
            FieldError fieldError = (FieldError) error;
            String field = fieldError.getField();
            String message = fieldError.getDefaultMessage();
            String str = (field + ":" + message);
            stringBuffer.append(str);
        });
        return  stringBuffer;
    }
}
