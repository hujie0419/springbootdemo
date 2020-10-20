package com.tuhu.springbootmybatis.controller;

import com.github.pagehelper.PageInfo;
import com.tuhu.springbootmybatis.config.ApolloConfigProperties;
import com.tuhu.springbootmybatis.service.TTrainCourseService;
import com.tuhu.springbootmybatis.service.TTrainEntryService;
import com.tuhu.springbootmybatis.tools.APIResponse;
import com.tuhu.springbootmybatis.tools.CheckRequestService;
import com.tuhu.springbootmybatis.vo.TTrainCourseReqGetVO;
import com.tuhu.springbootmybatis.vo.TTrainCourseReqSaveVO;
import com.tuhu.springbootmybatis.vo.TTrainCourseReqUpdateVO;
import com.tuhu.springbootmybatis.vo.TTrainCourseRespGetVO;
import io.swagger.annotations.Api;
import io.swagger.annotations.ApiOperation;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.util.StringUtils;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.*;
import javax.validation.Valid;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Date;

/**
 * @author hujie1
 * @date 2020/9/2810:01
 */
@Api(tags = "运营管理模块")
@Slf4j
@RestController
public class OperateController {

    @Autowired
    TTrainCourseService tTrainCourseService;
    @Autowired
    TTrainEntryService tTrainEntryService;
    @Autowired
    ApolloConfigProperties apolloConfigProperties;

    @Value("${name}")
    private String name;

    /**
     * 运营保存课程信息
     */
    @ApiOperation(value = "运营保存课程信息")
    @RequestMapping(value = "/course", method = RequestMethod.PUT)
    public APIResponse saveCourse(@RequestBody @Valid TTrainCourseReqSaveVO tTrainCourseReqSaveVO, BindingResult result) {
         try{
             if (result.hasErrors()) {
                 return new APIResponse("1", CheckRequestService.AnalyseBindingResult(result),"参数校验失败");
             }
             if(tTrainCourseReqSaveVO.getOpeningTime().before(new Date())){
                 return new APIResponse("1","","开课日期不能小于当前时间");
             }
             //课程校验重复
             if(tTrainCourseService.selectCourseByCourseName(tTrainCourseReqSaveVO)>0){
                 return new APIResponse("1","","课程名称已存在");
             }
             tTrainCourseService.saveCourse(tTrainCourseReqSaveVO);
             return new APIResponse();
        } catch (Exception e) {
             log.info("异常",e.getMessage());
            return new APIResponse("1",e.getStackTrace(),"保存课程失败");
        }

    }

    /**
     * 运维获取所有课程信息
     */
    @ApiOperation(value="运维获取所有课程信息")
    @RequestMapping(value = "/course",method = RequestMethod.GET)
    public APIResponse getCourse(@RequestParam(value="courseName")String  courseName,
                                 @RequestParam(value="lecturer")String lecturer,
                                 @RequestParam(value="openingTime")String openingTime,
                                 @RequestParam(value="pageNo",defaultValue="1")int pageNo,
                                 @RequestParam(value="pageSize",defaultValue="10")int pageSize) {
        TTrainCourseReqGetVO tTrainCourseReqGetVO = new TTrainCourseReqGetVO();
        tTrainCourseReqGetVO.setCourseName(courseName);
        tTrainCourseReqGetVO.setLecturer(lecturer);
        if(!StringUtils.isEmpty(openingTime)){
            SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
            try {
                tTrainCourseReqGetVO.setOpeningTime(simpleDateFormat.parse(openingTime));
            } catch (ParseException e) {
                log.info("异常",e.getMessage());
                return new APIResponse("1","时间转化异常","操作失败");
            }
        }
        PageInfo<TTrainCourseRespGetVO> pageInfo = tTrainCourseService.getCourse(tTrainCourseReqGetVO,pageNo,pageSize);
        return new APIResponse("0",pageInfo,"操作成功");
    }


    /**
     * 运营修改更新课程信息
     */
    @ApiOperation(value = "运营修改更新课程信息")
    @RequestMapping(value = "/course", method = RequestMethod.POST)
    public APIResponse updateCourseById(@RequestBody @Valid TTrainCourseReqUpdateVO tTrainCourseReqUpdateVO, BindingResult result) {
        try {
            if (result.hasErrors()) {
                return new APIResponse("1", CheckRequestService.AnalyseBindingResult(result),"参数校验失败");
            }
            //课程被报名与否
            int retCount = tTrainEntryService.selectEntryByCourseId(tTrainCourseReqUpdateVO);
            if(retCount>0){
                return  new APIResponse("1", "","课程被报名,不能被修改");
            }
            if(tTrainCourseReqUpdateVO.getOpeningTime().before(new Date())){
                return new APIResponse("1","","开课日期不能小于当前时间");
            }
            tTrainCourseService.updateCourseById(tTrainCourseReqUpdateVO);
            return  new APIResponse();
        } catch (Exception e) {
            log.info("异常: "+e.getMessage());
            return new APIResponse("1",e.getStackTrace(),"操作失败");
        }

    }

    /**
     * 运营删除课程信息
     */
    @ApiOperation(value = "运营删除课程信息")
    @RequestMapping(value = "/course/{id}", method = RequestMethod.DELETE)
    public APIResponse deleteCourseById(@PathVariable("id") Long id) {
        try {
            tTrainCourseService.deleteCourseById(id);
            return  new APIResponse();
        } catch (Exception e) {
            log.info("异常: "+e.getMessage());
            return new APIResponse("1",e.getStackTrace(),"操作失败");
        }

    }

    /**
     * 运营查看报名信息
     */
    @ApiOperation(value = "运营查看报名信息")
    @RequestMapping(value = "/entry", method = RequestMethod.GET)
    public APIResponse selectEntry(@RequestParam(value="pageNo",defaultValue="1")int pageNo,
                                   @RequestParam(value="pageSize",defaultValue="10")int pageSize) {
        return new APIResponse("0",tTrainEntryService.selectEntry(pageNo,pageSize),"操作成功");
    }

    /**
     * 运营审核报名信息状态
     */
    @ApiOperation(value = "运营审核报名信息状态")
    @RequestMapping(value = "/entry/{state}/{id}", method = RequestMethod.POST)
    public APIResponse updateEntryStateById(@PathVariable("state") Integer state, @PathVariable("id") Long id) {
        try {
            tTrainEntryService.updateEntryStateById(id, state);
            return new APIResponse();
        } catch (Exception e) {
            log.info("异常: "+e.getMessage());
            return new APIResponse("1",e.getStackTrace(),"操作失败");
        }
    }

    /**
     * 运营删除报名信息
     */
    @ApiOperation(value = "运营删除报名信息")
    @RequestMapping(value = "/entry/{id}", method = RequestMethod.DELETE)
    public APIResponse deleteEntryById(@PathVariable("id") Long id) {
        try {
            tTrainEntryService.deleteLogicByPrimaryKey(id);
            return new APIResponse();
        } catch (Exception e) {
            log.info("异常: "+e.getMessage());
            return new APIResponse("1",e.getStackTrace(),"操作失败");
        }
    }

    /**
     * 获取配置的信息
     */
    @ApiOperation(value = "获取配置的信息")
    @RequestMapping(value = "/name", method = RequestMethod.GET)
    public APIResponse getApolloProperties() {
        return new APIResponse("0",name,"操作成功");
    }
}
