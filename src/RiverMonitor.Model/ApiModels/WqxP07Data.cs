namespace RiverMonitor.Model.ApiModels;

public class WqxP07Data
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

    /// <summary>
    /// 代表從 API 獲取的一筆地下水監測站基本資料紀錄。
    /// 屬性名稱與來源 JSON 欄位直接對應，以確保資料綁定成功。
    /// </summary>
    public class RecordItem
    {
        /// <summary>
        /// 測站編號 (Site ID)
        /// </summary>
        public string? Siteid { get; set; }

        /// <summary>
        /// 測站名稱 (Site Name)
        /// </summary>
        public string? Sitename { get; set; }

        /// <summary>
        /// 測站英文名稱 (Site English Name)
        /// </summary>
        public string? Siteengname { get; set; }

        /// <summary>
        /// 縣市 (County)
        /// </summary>
        public string? County { get; set; }

        /// <summary>
        /// 鄉鎮市區 (Township)
        /// </summary>
        public string? Township { get; set; }

        /// <summary>
        /// 地下水分區名稱 (Underground Water District Name)
        /// </summary>
        public string? Ugwdistname { get; set; }

        /// <summary>
        /// TWD97 經度 (TWD97 Longitude)
        /// </summary>
        public string? Twd97Lon { get; set; }

        /// <summary>
        /// TWD97 緯度 (TWD97 Latitude)
        /// </summary>
        public string? Twd97Lat { get; set; }

        /// <summary>
        /// TWD97 TM2 X座標 (TWD97 TM2 X-coordinate)
        /// </summary>
        public string? Twd97Tm2X { get; set; }

        /// <summary>
        /// TWD97 TM2 Y座標 (TWD97 TM2 Y-coordinate)
        /// </summary>
        public string? Twd97Tm2Y { get; set; }

        /// <summary>
        /// 測站地址 (Site Address)
        /// </summary>
        public string? Siteaddress { get; set; }

        /// <summary>
        /// 使用狀態 (Status of Use)
        /// </summary>
        public string? Statusofuse { get; set; }
    }
}