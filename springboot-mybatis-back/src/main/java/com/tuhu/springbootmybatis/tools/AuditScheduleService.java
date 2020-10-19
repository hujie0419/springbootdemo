package com.tuhu.springbootmybatis.tools;

import com.tuhu.springbootmybatis.service.TTrainEntryService;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.scheduling.annotation.EnableScheduling;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

import java.text.SimpleDateFormat;
import java.util.Calendar;

/**
 * @author hujie1
 * @date 2020/9/3014:18
 */

@Slf4j
@Component
//@EnableScheduling
public class AuditScheduleService {

    @Autowired
    TTrainEntryService tTrainEntryService;

    SimpleDateFormat dateFormat= new SimpleDateFormat("yyyy-MM-dd :hh:mm:ss");
    /**
     * 每天凌晨1点自动审核能过的用户的报名状态为已通过
     * 0 0 1 * * ?
     */
    @Scheduled(cron = "0/5 * * * * *")
    public void scheduled(){
        Calendar calendar= Calendar.getInstance();
        log.info("定时任务审核用户报名课程开始时间：  {}",dateFormat.format(calendar.getTime()));
        tTrainEntryService.updateEntrySchedule();
        log.info("定时任务审核用户报名课程结束时间：  {}",dateFormat.format(calendar.getTime()));
    }
}
