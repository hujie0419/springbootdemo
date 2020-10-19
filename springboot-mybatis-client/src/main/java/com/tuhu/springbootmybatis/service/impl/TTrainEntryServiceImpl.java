package com.tuhu.springbootmybatis.service.impl;

import com.tuhu.springbootmybatis.entity.TTrainEntry;
import com.tuhu.springbootmybatis.mapper.TTrainEntryMapper;
import com.tuhu.springbootmybatis.service.TTrainEntryService;
import com.tuhu.springbootmybatis.vo.TTrainEntryReqPutVO;
import com.tuhu.springbootmybatis.vo.TTrainEntryRespGetVO;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.Date;
import java.util.List;

/**
 * @author hujie1
 * @date 2020/10/1711:26
 */
@Service
public class TTrainEntryServiceImpl implements TTrainEntryService {


    @Autowired
    TTrainEntryMapper tTrainEntryMapper;

    /**
     * 用户课程报名
     * @return
     */
    public int insertEntry(TTrainEntryReqPutVO tTrainEntryReqPutVO) {
        TTrainEntry tTrainEntry = new TTrainEntry();
        BeanUtils.copyProperties(tTrainEntryReqPutVO, tTrainEntry);
        tTrainEntry.setState(0);
        tTrainEntry.setIsDelete(0);
        tTrainEntry.setCreateTime(new Date());
        tTrainEntry.setUpdateTime(new Date());
        return tTrainEntryMapper.insert(tTrainEntry);
    }


    /**
     * 用户课程报名限制5人
     * @return
     */
    public int selectEntryCountByCourseId(TTrainEntryReqPutVO tTrainEntryReqPutVO) {
        TTrainEntry tTrainEntry = new TTrainEntry();
        BeanUtils.copyProperties(tTrainEntryReqPutVO, tTrainEntry);
        return tTrainEntryMapper.selectEntryCountByCourseId(tTrainEntry);
    }


    /**
     * 校验是否重复报名 身份证编号+课程名称
     * @return
     */
    public int selectEntryByUserCard(TTrainEntryReqPutVO tTrainEntryReqPutVO) {
        TTrainEntry tTrainEntry = new TTrainEntry();
        BeanUtils.copyProperties(tTrainEntryReqPutVO, tTrainEntry);
        return tTrainEntryMapper.selectEntryByUserCard(tTrainEntry);
    }


    /**
     * 用户查看自己的报名信息
     * @return
     */
    public List<TTrainEntryRespGetVO> getEntryByUserCard(String  userCard) {
        return tTrainEntryMapper.getEntryByUserCard(userCard);
    }


}
