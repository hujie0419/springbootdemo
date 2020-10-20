package com.tuhu.springbootmybatis.service;

import com.github.pagehelper.PageInfo;
import com.tuhu.springbootmybatis.vo.TTrainCourseReqGetVO;
import com.tuhu.springbootmybatis.vo.TTrainCourseReqSaveVO;
import com.tuhu.springbootmybatis.vo.TTrainCourseReqUpdateVO;
import com.tuhu.springbootmybatis.vo.TTrainCourseRespGetVO;

/**
 * @author hujie1
 * @date 2020/9/2915:18
 */
public interface TTrainCourseService {



    /**
     * 运营根据课程名称查询课程信息
     *
     * @return
     */
    int selectCourseByCourseName(TTrainCourseReqSaveVO tTrainCourseReqSaveVO);

    /**
     * 运营保存课程信息
     *
     * @return
     */
    int saveCourse(TTrainCourseReqSaveVO tTrainCourse);

    /**
     * 运营获取所有课程信息
     * @return
     */
    PageInfo<TTrainCourseRespGetVO> getCourse(TTrainCourseReqGetVO tTrainCourseReqGetVO, int pageNo, int pageSize);

    /**
     * 运营修改更新课程信息
     *
     * @return
     */
    int updateCourseById(TTrainCourseReqUpdateVO tTrainCourseReqUpdateVO);

    /**
     * 运营删除课程信息
     *
     * @return
     */
    int deleteCourseById(long id);

}
