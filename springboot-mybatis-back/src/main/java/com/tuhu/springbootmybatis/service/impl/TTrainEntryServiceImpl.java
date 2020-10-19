package com.tuhu.springbootmybatis.service.impl;

import com.github.pagehelper.PageHelper;
import com.github.pagehelper.PageInfo;
import com.tuhu.springbootmybatis.entity.TTrainEntry;
import com.tuhu.springbootmybatis.mapper.TTrainEntryMapper;
import com.tuhu.springbootmybatis.service.TTrainEntryService;
import com.tuhu.springbootmybatis.vo.TTrainCourseReqUpdateVO;
import com.tuhu.springbootmybatis.vo.TTrainCourseRespGetVO;
import com.tuhu.springbootmybatis.vo.TTrainEntryRespGetVO;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.Date;
import java.util.List;

/**
 * @author hujie1
 * @date 2020/10/1711:33
 */
@Service
public class TTrainEntryServiceImpl implements TTrainEntryService {



    @Autowired
    TTrainEntryMapper tTrainEntryMapper;


    /**
     * 运营查询所有报名课程信息
     * @return
     */
    public PageInfo<TTrainEntryRespGetVO> selectEntry(int pageNo , int pageSize) {
        PageHelper.startPage(pageNo,pageSize);
        List<TTrainEntryRespGetVO> list =  tTrainEntryMapper.selectEntry();
        PageInfo<TTrainEntryRespGetVO> pageInfo = new PageInfo<>(list);
        return pageInfo;
    }

    @Override
    public int selectEntryByCourseId(TTrainCourseReqUpdateVO tTrainCourseReqUpdateVO) {
        TTrainEntry tTrainEntry = new TTrainEntry();
        tTrainEntry.setCourseId(tTrainCourseReqUpdateVO.getId());
        return tTrainEntryMapper.selectEntryByCourseId(tTrainEntry);
    }

    /**
     * 运营删除报名信息
     * @return
     */
    public int  deleteLogicByPrimaryKey(long id) {
        return tTrainEntryMapper.deleteByPrimaryKey(id);
    }

    /**
     * 运营审核报名信息状态
     * @return
     */
    public int  updateEntryStateById(long id,int state) {
        TTrainEntry tTrainEntry = new TTrainEntry();
        tTrainEntry.setState(state);
        tTrainEntry.setId(id);
        tTrainEntry.setUpdateTime(new Date());
        tTrainEntry.setReviewer("运营手动");
        return tTrainEntryMapper.updateByPrimaryKeySelective(tTrainEntry);
    }

    /**
     * 定时任务审核报名信息状态
     * @return
     */
    public int  updateEntrySchedule() {
        TTrainEntry tTrainEntry = new TTrainEntry();
        tTrainEntry.setState(1);
        tTrainEntry.setUpdateTime(new Date());
        tTrainEntry.setReviewer("自动审核");
        return tTrainEntryMapper.updateEntrySchedule(tTrainEntry);
    }

}
