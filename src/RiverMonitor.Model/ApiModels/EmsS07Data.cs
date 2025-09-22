namespace RiverMonitor.Model.ApiModels;

public class EmsS07Data
{
    public class RootObject
    {
        public FieldItem[]? Fields { get; set; }
        public string? ResourceId { get; set; }
        public bool? IncludeTotal { get; set; }
        public string? Total { get; set; }
        public string? ResourceFormat { get; set; }
        public string? Limit { get; set; }
        public string? Offset { get; set; }
        public RecordItem[]? Records { get; set; }
    }

    public class FieldItem
    {
        public string? Id { get; set; }
        public string? Type { get; set; }
        public Info? Info { get; set; }
    }

    public class Info
    {
        public string? Label { get; set; }
    }

    public class RecordItem
    {
        /// <summary>
        /// 場址代碼
        /// </summary>
        public string? SiteId { get; set; }

        /// <summary>
        /// 場址名稱
        /// </summary>
        public string? SiteName { get; set; }

        /// <summary>
        /// 公告管制文號
        /// </summary>
        public string? AnnoNo { get; set; }

        /// <summary>
        /// 公告解除文號
        /// </summary>
        public string? DeannoNo { get; set; }

        /// <summary>
        /// 縣市
        /// </summary>
        public string? County { get; set; }

        /// <summary>
        /// 場址類型
        /// </summary>
        public string? SiteType { get; set; }

        /// <summary>
        /// 場址使用狀態
        /// </summary>
        public string? SiteUse { get; set; }

        /// <summary>
        /// 污染物質及濃度
        /// </summary>
        public string? Pollutant { get; set; }

        /// <summary>
        /// 場址地址
        /// </summary>
        public string? Pollutantaddress { get; set; }

        /// <summary>
        /// TM2X座標
        /// </summary>
        public string? Dtmx { get; set; }

        /// <summary>
        /// TM2Y座標
        /// </summary>
        public string? Dtmy { get; set; }

        /// <summary>
        /// 場址管制類型
        /// </summary>
        public string? Controltype { get; set; }

        /// <summary>
        /// 公告管制日期
        /// </summary>
        public string? AnnoDate { get; set; }

        /// <summary>
        /// 公告解除日期
        /// </summary>
        public string? DeannoDate { get; set; }

        /// <summary>
        /// 場址面積
        /// </summary>
        public string? Sitearea { get; set; }

        /// <summary>
        /// 地段地號
        /// </summary>
        public string? Landno { get; set; }

        /// <summary>
        /// 鄉鎮市區
        /// </summary>
        public string? Township { get; set; }

        /// <summary>
        /// WGS84經度
        /// </summary>
        public string? Wgs84Lng { get; set; }

        /// <summary>
        /// WGS84緯度
        /// </summary>
        public string? Wgs84Lat { get; set; }
    }
}