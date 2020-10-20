package com.tuhu.springbootmybatis.service.impl;

import com.github.pagehelper.PageHelper;
import com.github.pagehelper.PageInfo;
import com.tuhu.springbootmybatis.entity.TTrainCourse;
import com.tuhu.springbootmybatis.mapper.TTrainCourseMapper;
import com.tuhu.springbootmybatis.service.TTrainCourseService;
import com.tuhu.springbootmybatis.vo.TTrainCourseReqGetVO;
import com.tuhu.springbootmybatis.vo.TTrainCourseRespGetVO;
import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import java.util.List;

/**
 * @author hujie1
 * @date 2020/9/2915:18
 */
@Service
public class TTrainCourseServiceImpl  implements TTrainCourseService {

    @Autowired
    TTrainCourseMapper ttTrainCourseMapper;

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
}
