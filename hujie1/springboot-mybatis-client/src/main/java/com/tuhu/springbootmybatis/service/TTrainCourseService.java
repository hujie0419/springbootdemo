package com.tuhu.springbootmybatis.service;

import com.github.pagehelper.PageInfo;
import com.tuhu.springbootmybatis.vo.TTrainCourseReqGetVO;
import com.tuhu.springbootmybatis.vo.TTrainCourseRespGetVO;

/**
 * @author hujie1
 * @date 2020/9/2915:18
 */
public interface TTrainCourseService {

    /**
     * 用户获取所有课程信息
     * @return
     */
    /**
     * 运营获取所有课程信息
     * @return
     */
    PageInfo<TTrainCourseRespGetVO> getCourse(TTrainCourseReqGetVO tTrainCourseReqGetVO, int pageNo, int pageSize);
}
