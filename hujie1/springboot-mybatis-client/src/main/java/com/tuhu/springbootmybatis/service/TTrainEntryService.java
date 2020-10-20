package com.tuhu.springbootmybatis.service;

import com.tuhu.springbootmybatis.entity.TTrainEntry;
import com.tuhu.springbootmybatis.vo.TTrainEntryReqPutVO;
import com.tuhu.springbootmybatis.vo.TTrainEntryRespGetVO;

import java.util.List;

/**
 * @author hujie1
 * @date 2020/9/2915:25
 */
public interface TTrainEntryService {


    /**
     * 用户课程报名
     *
     * @return
     */
     int insertEntry(TTrainEntryReqPutVO tTrainEntryReqPutVO);


    /**
     * 用户课程报名限制5人
     *
     * @return
     */
    int selectEntryCountByCourseId(TTrainEntryReqPutVO tTrainEntryReqPutVO);


    /**
     * 校验是否重复报名 身份证编号+课程名称
     *
     * @return
     */
    int selectEntryByUserCard(TTrainEntryReqPutVO tTrainEntryReqPutVO);


    /**
     * 用户查看自己的报名信息
     *
     * @return
     */
    List<TTrainEntryRespGetVO> getEntryByUserCard(String userCard);

}
