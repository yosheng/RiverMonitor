namespace RiverMonitor.Model.ApiModels;

public class WqxP02Data
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
    /// 代表從 API 獲取的一筆原始地下水水質監測紀錄。
    /// 屬性名稱與來源 JSON 欄位直接對應，以確保資料綁定成功。
    /// </summary>
    public class RecordItem
    {
        /// <summary>
        /// 測站代碼
        /// </summary>
        public string? Siteid { get; set; }

        /// <summary>
        /// 測站名稱
        /// </summary>
        public string? Sitename { get; set; }

        /// <summary>
        /// 測站英文名稱
        /// </summary>
        public string? Siteengname { get; set; }

        /// <summary>
        /// 地下水分區名稱
        /// </summary>
        public string? Ugwdistname { get; set; }

        /// <summary>
        /// 縣市
        /// </summary>
        public string? County { get; set; }

        /// <summary>
        /// 鄉鎮市區
        /// </summary>
        public string? Township { get; set; }

        /// <summary>
        /// TWD97經度
        /// </summary>
        public string? Twd97Lon { get; set; }

        /// <summary>
        /// TWD97緯度
        /// </summary>
        public string? Twd97Lat { get; set; }

        /// <summary>
        /// TWD97二度分帶X
        /// </summary>
        public string? Twd97Tm2X { get; set; }

        /// <summary>
        /// TWD97二度分帶Y
        /// </summary>
        public string? Twd97Tm2Y { get; set; }

        /// <summary>
        /// 採樣日期
        /// </summary>
        public string? Sampledate { get; set; }

        /// <summary>
        /// 測項名稱
        /// </summary>
        public string? Itemname { get; set; }

        /// <summary>
        /// 測項英文名稱
        /// </summary>
        public string? Itemengname { get; set; }

        /// <summary>
        /// 測項英文簡稱
        /// </summary>
        public string? Itemengabbreviation { get; set; }

        /// <summary>
        /// 監測值
        /// </summary>
        public string? Itemvalue { get; set; }

        /// <summary>
        /// 測項單位
        /// </summary>
        public string? Itemunit { get; set; }

        /// <summary>
        /// 備註
        /// </summary>
        public string? Note { get; set; }
    }
}