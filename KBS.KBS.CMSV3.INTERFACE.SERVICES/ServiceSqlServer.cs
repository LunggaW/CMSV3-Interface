﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using KBS.KBS.CMSV3.INTERFACE.FUNCTION;
using NLog;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Calendar;

namespace KBS.KBS.CMSV3.INTERFACE.SERVICES
{
    public partial class ServiceSqlServer : ServiceBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Function CSMV3Function = new Function();

        public ServiceSqlServer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {


            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();

            // get a scheduler
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();

            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<HelloJob>()
                .WithIdentity("myJob", "group1")
                .Build();

            // Trigger the job to run now, and then every 40 seconds
            //ITrigger trigger = TriggerBuilder.Create()
            //  .WithIdentity("myTrigger", "group1")
            //  .StartNow()
            //  .WithSimpleSchedule(x => x
            //      .WithIntervalInSeconds(40)
            //      .RepeatForever())
            //  .Build();

            ITrigger trigger;


            //holiday calendar
            //HolidayCalendar cal = new HolidayCalendar();
            //cal.AddExcludedDate(DateTime.Now.AddDays(1));

            //sched.AddCalendar("myHolidays", cal, false,false);

            if (CSMV3Function.GetIsDaily())
            {
                logger.Debug("Start Daily");
                trigger = TriggerBuilder.Create()
                    .WithIdentity("myTrigger")
                    .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(CSMV3Function.GetHour(),
                        CSMV3Function.GetMinutes())) // execute job daily at
                    //.ModifiedByCalendar("myHolidays") // but not on holidays
                    .Build();
            }
            else if (CSMV3Function.GetIsWeekly())
            {
               logger.Debug("Start Weekly");
               logger.Debug("Day of week is : " + CSMV3Function.GetDayofWeek());
                trigger = TriggerBuilder.Create()
                    .WithIdentity("myTrigger")
                    .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(CSMV3Function.GetDayofWeek(),
                        CSMV3Function.GetHour(),
                        CSMV3Function.GetMinutes())) // execute job daily at
                    //.ModifiedByCalendar("myHolidays") // but not on holidays
                    .Build();
            }
            else if (CSMV3Function.GetIsMonthly())
            {
                logger.Debug("Start Monthly");
                logger.Debug("Day of month is : " + CSMV3Function.GetDayofMonth());
                trigger = TriggerBuilder.Create()
                    .WithIdentity("myTrigger")
                    .WithSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(CSMV3Function.GetDayofMonth(),
                        CSMV3Function.GetHour(),
                        CSMV3Function.GetMinutes())) // execute job daily at
                    //.ModifiedByCalendar("myHolidays") // but not on holidays
                    .Build();
            }
            else
            {
                trigger = TriggerBuilder.Create()
                    .WithIdentity("myTrigger", "group1")
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(CSMV3Function.GetIntervalinSeconds())
                        .RepeatForever())
                    .EndAt(DateBuilder.DateOf(22, 0, 0))
                    .Build();
            }



        

            sched.ScheduleJob(job, trigger);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                logger.Error(ex.InnerException);
                throw;
            }
        }

        protected override void OnStop()
        {
        }

        

    }

    public class HelloJob : IJob
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Function CSMV3Function = new Function();
        private DataTable DTSales = new DataTable();


        public void Execute(IJobExecutionContext context)
        {
            logger.Debug("Start Job");
            StartJob();
        }

        public void StartJob()
        {
            // TODO: Insert monitoring activities here.
            //eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);StringBuilder sb = new StringBuilder(); 
            //sales_int();
            //CSMV3Function.SelectSalesSql();
            CSMV3Function.ExecExportDeliveryNote();
            CSMV3Function.ExecExportBarcodeMaster();
            CSMV3Function.ExecExportSalesPriceMaster();
            CSMV3Function.ExecExportItemMaster();
            CSMV3Function.ExecExportStoreMaster();
        }

        private void sales_int()
        {
            try
            {
                DTSales = CSMV3Function.SelectSales();


                StringBuilder sb = new StringBuilder();


                IEnumerable<string> columnNames = DTSales.Columns.Cast<DataColumn>().
                                                  Select(column => column.ColumnName);
                sb.AppendLine(string.Join("|", columnNames));

                foreach (DataRow row in DTSales.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                    sb.AppendLine(string.Join("|", fields));

                }
                logger.Debug("DATA : " + sb.ToString());
                File.WriteAllText(CSMV3Function.getCSVLocationOracle() + "test.csv", sb.ToString());
            }
            catch (Exception ex)
            {
                logger.Error("Messsage : " + ex.Message);
                logger.Error("Inner Exception : " + ex.InnerException);
                throw;
            } 
        }
    }
}
