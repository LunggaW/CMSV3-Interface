using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using Quartz;
using Quartz.Impl;
using KBS.KBS.CMSV3.INTERFACE.FUNCTION;
using KBS.KBS.CMSV3.INTERFACE.DATAMODEL;
using NLog;
using License = KBS.KBS.CMSV3.INTERFACE.DATAMODEL.License;

namespace KBS.KBS.CMSV3.INTERFACE.SERVICES.TRANS
{
    public partial class ServiceOracle : ServiceBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Function CMSV3Function = new Function();

        public ServiceOracle()
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

                if (CMSV3Function.GetIsDaily())
                {
                    logger.Debug("Start Daily");
                    trigger = TriggerBuilder.Create()
                        .WithIdentity("myTrigger")
                        .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(CMSV3Function.GetHour(),
                            CMSV3Function.GetMinutes())) // execute job daily at
                        //.ModifiedByCalendar("myHolidays") // but not on holidays
                        .Build();
                }
                else if (CMSV3Function.GetIsWeekly())
                {
                    logger.Debug("Start Weekly");
                    logger.Debug("Day of week is : " + CMSV3Function.GetDayofWeek());
                    trigger = TriggerBuilder.Create()
                        .WithIdentity("myTrigger")
                        .WithSchedule(CronScheduleBuilder.WeeklyOnDayAndHourAndMinute(CMSV3Function.GetDayofWeek(),
                            CMSV3Function.GetHour(),
                            CMSV3Function.GetMinutes())) // execute job daily at
                        //.ModifiedByCalendar("myHolidays") // but not on holidays
                        .Build();
                }
                else if (CMSV3Function.GetIsMonthly())
                {
                    logger.Debug("Start Monthly");
                    logger.Debug("Day of month is : " + CMSV3Function.GetDayofMonth());
                    trigger = TriggerBuilder.Create()
                        .WithIdentity("myTrigger")
                        .WithSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute(CMSV3Function.GetDayofMonth(),
                            CMSV3Function.GetHour(),
                            CMSV3Function.GetMinutes())) // execute job daily at
                        //.ModifiedByCalendar("myHolidays") // but not on holidays
                        .Build();
                }
                else
                {
                    trigger = TriggerBuilder.Create()
                        .WithIdentity("myTrigger", "group1")
                        .WithSimpleSchedule(x => x
                            .WithIntervalInSeconds(CMSV3Function.GetIntervalinSeconds())
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
        private Function CMSV3Function = new Function();
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
            //CSMV3Function.ExecExportDeliveryNote();
            //CSMV3Function.ExecExportBarcodeMaster();
            //CSMV3Function.ExecExportSalesPriceMaster();
            //CSMV3Function.ExecExportItemMaster();
            //CSMV3Function.ExecExportStoreMaster();
            LicenseCheck();
        }

        private void sales_int()
        {
            try
            {
                DTSales = CMSV3Function.SelectSales();


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
                File.WriteAllText(CMSV3Function.getCSVLocationOracle() + "test.csv", sb.ToString());
            }
            catch (Exception ex)
            {
                logger.Error("Messsage : " + ex.Message);
                logger.Error("Inner Exception : " + ex.InnerException);
                throw;
            }
        }

        private void LicenseCheck()
        {
            try
            {
                String licenseText;
                License license = new License();

                CMSV3Function.DisableAllStoreFlagandStatus();

                licenseText = CMSV3Function.GetLicense();
                licenseText = CMSV3Function.Decrypt(licenseText);

                license = CMSV3Function.ParseLicenseText(licenseText);

                CMSV3Function.ValidateLicenseEndDate(license.EndDate);
                CMSV3Function.ValidateLicenseStore(Int32.Parse(license.StoreTotal));
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
