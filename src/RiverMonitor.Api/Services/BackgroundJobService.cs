using Hangfire;
using RiverMonitor.Bll.Services;

namespace RiverMonitor.Api.Services;

public class BackgroundJobService
{
    public static void ConfigureRecurringJobs()
    {
        // 每天凌晨 2:00 同步廢水排放數據
        RecurringJob.AddOrUpdate<ISyncService>(
            "sync-wastewater-emission",
            service => service.SyncWastewaterEmissionAsync(),
            "0 2 * * *", // Cron expression: 每天凌晨 2:00
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei")
        );

        // 每天凌晨 3:00 同步污染場址及公告數據
        RecurringJob.AddOrUpdate<ISyncService>(
            "sync-pollution-site-announcement",
            service => service.SyncPollutionSiteAndAnnouncementAsync(),
            "0 3 * * *", // Cron expression: 每天凌晨 3:00
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei")
        );

        // 每天凌晨 4:00 同步河川水質監測數據
        RecurringJob.AddOrUpdate<ISyncService>(
            "sync-monitoring-site",
            service => service.SyncMonitoringSiteAsync(),
            "0 4 * * *", // Cron expression: 每天凌晨 4:00
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei")
        );

        // 每天凌晨 5:00 同步地下水監測數據
        RecurringJob.AddOrUpdate<ISyncService>(
            "sync-groundwater-site",
            service => service.SyncGroundwaterSiteAsync(),
            "0 5 * * *", // Cron expression: 每天凌晨 5:00
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei")
        );

        // 每週日凌晨 1:00 同步農田水利署基本資料（頻率較低）
        RecurringJob.AddOrUpdate<ISyncService>(
            "sync-irrigation-agency",
            service => service.SyncIrrigationAgencyAsync(),
            "0 1 * * SUN", // Cron expression: 每週日凌晨 1:00
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei")
        );

        // 每週日凌晨 1:30 同步農田水利署工作站資料
        RecurringJob.AddOrUpdate<ISyncService>(
            "sync-irrigation-agency-station",
            service => service.SyncIrrigationAgencyStationAsync(),
            "30 1 * * SUN", // Cron expression: 每週日凌晨 1:30
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei")
        );

        // 每天凌晨 6:00 同步農田水利署監測數據
        RecurringJob.AddOrUpdate<ISyncService>(
            "sync-irrigation-agency-monitoring-data",
            service => service.SyncIrrigationAgencyStationMonitoringDataAsync(),
            "0 6 * * *", // Cron expression: 每天凌晨 6:00
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Taipei")
        );
    }

    public static void TriggerImmediateJobs()
    {
        // 提供手動觸發任務的方法
        BackgroundJob.Enqueue<ISyncService>(service => service.SyncWastewaterEmissionAsync());
        BackgroundJob.Enqueue<ISyncService>(service => service.SyncPollutionSiteAndAnnouncementAsync());
        BackgroundJob.Enqueue<ISyncService>(service => service.SyncMonitoringSiteAsync());
        BackgroundJob.Enqueue<ISyncService>(service => service.SyncGroundwaterSiteAsync());
        BackgroundJob.Enqueue<ISyncService>(service => service.SyncIrrigationAgencyAsync());
        BackgroundJob.Enqueue<ISyncService>(service => service.SyncIrrigationAgencyStationAsync());
        BackgroundJob.Enqueue<ISyncService>(service => service.SyncIrrigationAgencyStationMonitoringDataAsync());
    }
}