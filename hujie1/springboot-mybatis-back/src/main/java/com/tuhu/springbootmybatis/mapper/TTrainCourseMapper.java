package com.tuhu.springbootmybatis.mapper;

import com.tuhu.springbootmybatis.entity.TTrainCourse;
import com.tuhu.springbootmybatis.vo.TTrainCourseRespGetVO;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface TTrainCourseMapper {
    /**
     * This method was generated by MyBatis Generator.
     * This method corresponds to the database table t_train_course
     *
     * @mbggenerated Tue Sep 29 15:05:54 CST 2020
     */
    int deleteByPrimaryKey(Long id);

    int deleteLogicByPrimaryKey(Long id);

    /**
     * This method was generated by MyBatis Generator.
     * This method corresponds to the database table t_train_course
     *
     * @mbggenerated Tue Sep 29 15:05:54 CST 2020
     */
    int insert(TTrainCourse record);

    /**
     * This method was generated by MyBatis Generator.
     * This method corresponds to the database table t_train_course
     *
     * @mbggenerated Tue Sep 29 15:05:54 CST 2020
     */
    int insertSelective(TTrainCourse record);

    /**
     * This method was generated by MyBatis Generator.
     * This method corresponds to the database table t_train_course
     *
     * @mbggenerated Tue Sep 29 15:05:54 CST 2020
     */
    TTrainCourse selectByPrimaryKey(Long id);

    /**
     * This method was generated by MyBatis Generator.
     * This method corresponds to the database table t_train_course
     *
     * @mbggenerated Tue Sep 29 15:05:54 CST 2020
     */
    int updateByPrimaryKeySelective(TTrainCourse record);

    /**
     * This method was generated by MyBatis Generator.
     * This method corresponds to the database table t_train_course
     *
     * @mbggenerated Tue Sep 29 15:05:54 CST 2020
     */
    int updateByPrimaryKey(TTrainCourse record);

    /**
     * 获取全量的课程
     */
    List<TTrainCourseRespGetVO> getCourse();

    /**
     * 运营根据课程名称查询课程信息
     */
    int selectCourseByCourseName(TTrainCourse tTrainCourse);

    /**
     * 运营根据课程名称查询课程信息
     */
    List<TTrainCourseRespGetVO> selectTrainCourseBySelective(TTrainCourse tTrainCourse);

}