package com.tuhu.springbootmybatis.service.impl;

import com.github.pagehelper.PageHelper;
import com.github.pagehelper.PageInfo;
import com.tuhu.springbootmybatis.entity.TTrainCourse;
import com.tuhu.springbootmybatis.mapper.TTrainCourseMapper;
import com.tuhu.springbootmybatis.service.TTrainCourseService;
import com.tuhu.springbootmybatis.vo.TTrainCourseReqGetVO;
import com.tuhu.springbootmybatis.vo.TTrainCourseReqSaveVO;
import com.tuhu.springbootmybatis.vo.TTrainCourseReqUpdateVO;
import com.tuhu.springbootmybatis.vo.TTrainCourseRespGetVO;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;

import java.util.Date;
import java.util.List;

/**
 * @author hujie1
 * @date 2020/10/1711:32
 */
@Service
public class TTrainCourseServiceImpl implements TTrainCourseService {



    @Autowired
    TTrainCourseMapper ttTrainCourseMapper;

    @Override
    public int selectCourseByCourseName(TTrainCourseReqSaveVO tTrainCourseReqSaveVO) {
        TTrainCourse tTrainCourse = new TTrainCourse();
        BeanUtils.copyProperties(tTrainCourseReqSaveVO, tTrainCourse);
        return ttTrainCourseMapper.selectCourseByCourseName(tTrainCourse);
    }

    /**
     * 运营保存课程信息
     * @return
     */
    public int saveCourse(TTrainCourseReqSaveVO tTrainCourseReqSaveVO) {
        TTrainCourse tTrainCourse = new TTrainCourse();
        BeanUtils.copyProperties(tTrainCourseReqSaveVO, tTrainCourse);
        tTrainCourse.setIsDelete(0);
        tTrainCourse.setCreateTime(new Date());
        tTrainCourse.setUpdateTime(new Date());
        return ttTrainCourseMapper.insert(tTrainCourse);
    }

    /**
     * 用户获取所有课程信息
     * @return
     */
    public PageInfo<TTrainCourseRespGetVO> getCourse(TTrainCourseReqGetVO tTrainCourseReqGetVO, int pageNo, int pageSize) {
        TTrainCourse tTrainCourse = new TTrainCourse();
        BeanUtils.copyProperties(tTrainCourseReqGetVO, tTrainCourse);
        PageHelper.startPage(pageNo,pageSize);
        List<TTrainCourseRespGetVO> list =  ttTrainCourseMapper.selectTrainCourseBySelective(tTrainCourse);
        PageInfo<TTrainCourseRespGetVO> pageInfo = new PageInfo<>(list);
        return pageInfo;
    }

    /**
     * 运营修改更新课程信息
     * @return
     */
    public int updateCourseById(TTrainCourseReqUpdateVO tTrainCourseReqUpdateVO) {
        TTrainCourse tTrainCourse = new TTrainCourse();
        BeanUtils.copyProperties(tTrainCourseReqUpdateVO, tTrainCourse);
        tTrainCourse.setUpdateTime(new Date());
        return ttTrainCourseMapper.updateByPrimaryKeySelective(tTrainCourse);
    }

    /**
     * 运营删除课程信息 逻辑删除
     * @return
     */
    public int deleteCourseById(long id) {
        return ttTrainCourseMapper.deleteLogicByPrimaryKey(id);
    }
}
