package com.tuhu.springbootmybatis.controller;

import com.github.pagehelper.PageInfo;
import com.tuhu.springbootmybatis.service.TTrainCourseService;
import com.tuhu.springbootmybatis.service.TTrainEntryService;
import com.tuhu.springbootmybatis.tools.APIResponse;
import com.tuhu.springbootmybatis.vo.TTrainCourseReqGetVO;
import com.tuhu.springbootmybatis.vo.TTrainCourseRespGetVO;
import com.tuhu.springbootmybatis.vo.TTrainEntryReqPutVO;
import io.swagger.annotations.Api;
import io.swagger.annotations.ApiOperation;
import lombok.extern.slf4j.Slf4j;
import org.redisson.api.RedissonClient;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.util.StringUtils;
import org.springframework.web.bind.annotation.*;

import javax.validation.Valid;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.concurrent.locks.Lock;

/**
 * @author hujie1
 * @date 2020/9/2810:01
 */
@Slf4j
@Api(tags = "用户模块")
@RestController
public class UserController {

    @Autowired
    TTrainEntryService tTrainEntryService;
    @Autowired
    TTrainCourseService tTrainCourseService;
    @Autowired
    private RedissonClient redissonClient;

    /**
     * 用户获取所有课程信息
     */
    @ApiOperation(value = "用户获取课所有程信息")
    @RequestMapping(value = "/course", method = RequestMethod.GET)
    public APIResponse getCourse(@RequestParam(value = "courseName") String courseName,
                                 @RequestParam(value = "lecturer") String lecturer,
                                 @RequestParam(value = "openingTime") String openingTime,
                                 @RequestParam(value = "pageNo", defaultValue = "1") int pageNo,
                                 @RequestParam(value = "pageSize", defaultValue = "10") int pageSize) {
        TTrainCourseReqGetVO tTrainCourseReqGetVO = new TTrainCourseReqGetVO();
        tTrainCourseReqGetVO.setCourseName(courseName);
        tTrainCourseReqGetVO.setLecturer(lecturer);
        if (!StringUtils.isEmpty(openingTime)) {
            SimpleDateFormat simpleDateFormat = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss");
            try {
                tTrainCourseReqGetVO.setOpeningTime(simpleDateFormat.parse(openingTime));
            } catch (ParseException e) {
                log.info("异常: ", e.getMessage());
                return new APIResponse("1", "时间转化异常", "操作失败");
            }
        }
        PageInfo<TTrainCourseRespGetVO> pageInfo = tTrainCourseService.getCourse(tTrainCourseReqGetVO, pageNo, pageSize);
        return new APIResponse("0", pageInfo, "操作成功");
    }

    /**
     * 用户课程报名
     */
    @RequestMapping(value = "/entry", method = RequestMethod.PUT)
    public APIResponse saveEntry(@RequestBody @Valid TTrainEntryReqPutVO tTrainEntryReqPutVO) {
        Lock lock = redissonClient.getLock("saveEntry");
        try {
            lock.tryLock();
            int countCourse = tTrainEntryService.selectEntryCountByCourseId(tTrainEntryReqPutVO);
            if (countCourse < 5) {
                if (tTrainEntryService.selectEntryByUserCard(tTrainEntryReqPutVO) > 0) {
                    return new APIResponse("1", "", "课程已经报名过了");
                }
                tTrainEntryService.insertEntry(tTrainEntryReqPutVO);
                return new APIResponse();
            } else
                return new APIResponse("1", "", "课程报名人数已超过5人限制，请选择其他的课程");
        } catch (Exception e) {
            log.info("异常: " + e.getMessage());
            return new APIResponse("1", e.getStackTrace(), "操作失败");
        } finally {
            lock.unlock();
        }

    }

    /**
     * 用户查看自己的报名信息
     */
    @ApiOperation(value = "用户查看自己的报名信息")
    @RequestMapping(value = "/entry/{user_card}", method = RequestMethod.GET)
    public APIResponse getEntryByUserCard(@PathVariable("user_card") String userCard) {
        return new APIResponse("0", tTrainEntryService.getEntryByUserCard(userCard), "操作成功");
    }
}
