package com.tuhu.springbootmybatis.config;

import lombok.Data;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

/**
 * @author hujie1
 * @date 2020/10/1014:04
 */
@Component
@Data
public class ApolloConfigProperties {

    @Value("${name}")
    private String name;

}
