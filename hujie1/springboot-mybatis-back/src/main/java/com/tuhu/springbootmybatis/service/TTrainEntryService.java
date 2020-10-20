package com.tuhu.springbootmybatis.service;

import com.github.pagehelper.PageInfo;
import com.tuhu.springbootmybatis.entity.TTrainEntry;
import com.tuhu.springbootmybatis.vo.TTrainCourseReqUpdateVO;
import com.tuhu.springbootmybatis.vo.TTrainEntryRespGetVO;

import java.util.List;

/**
 * @author hujie1
 * @date 2020/9/2915:25
 */
public interface TTrainEntryService {


    /**
     * 运营查询所有报名课程信息
     *
     * @return
     */
    PageInfo<TTrainEntryRespGetVO> selectEntry(int pageNo, int PageSize);

    /**
     * 运营查询课程是否被报名占用
     *
     * @return
     */
    int selectEntryByCourseId(TTrainCourseReqUpdateVO tTrainCourseReqUpdateVO);

    /**
     * 运营删除报名信息
     *
     * @return
     */
    int deleteLogicByPrimaryKey(long id);

    /**
     * 运营审核报名信息状态
     *
     * @return
     */
    int updateEntryStateById(long id, int state);

    /**
     * 定时任务审核报名信息状态
     *
     * @return
     */
    int updateEntrySchedule();

}
