namespace RiverMonitor.Model.ApiModels;

public class WqxP01Data
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
    /// 代表從 API 獲取的一筆原始河川水質監測紀錄。
    /// </summary>
    public class RecordItem
    {
        /// <summary>
        /// 測站編號 (SiteId)
        /// </summary>
        public string? Siteid { get; set; }
        
        /// <summary>
        /// 測站名稱 (SiteName)
        /// </summary>
        public string? Sitename { get; set; }
        
        /// <summary>
        /// 測站英文名稱 (SiteEngName)
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
        /// 流域 (Basin)
        /// </summary>
        public string? Basin { get; set; }
        
        /// <summary>
        /// 河川 (River)
        /// </summary>
        public string? River { get; set; }
        
        /// <summary>
        /// TWD97經度 (TWD97Lon)
        /// </summary>
        public string? Twd97Lon { get; set; }
        
        /// <summary>
        /// TWD97緯度 (TWD97Lat)
        /// </summary>
        public string? Twd97Lat { get; set; }
        
        /// <summary>
        /// TWD97TM2X座標 (TWD97TM2X)
        /// </summary>
        public string? Twd97Tm2X { get; set; }
        
        /// <summary>
        /// TWD97TM2Y座標 (TWD97TM2Y)
        /// </summary>
        public string? Twd97Tm2Y { get; set; }
        
        /// <summary>
        /// 採樣日期 (SampleDate)
        /// </summary>
        public string? Sampledate { get; set; }
        
        /// <summary>
        /// 檢測項目 (ItemName)
        /// </summary>
        public string? Itemname { get; set; }
        
        /// <summary>
        /// 檢測項目英文名稱 (ItemEngName)
        /// </summary>
        public string? Itemengname { get; set; }
        
        /// <summary>
        /// 檢測項目英文縮寫 (ItemEngAbbreviation)
        /// </summary>
        public string? Itemengabbreviation { get; set; }
        
        /// <summary>
        /// 檢測值 (ItemValue)
        /// </summary>
        public string? Itemvalue { get; set; }
        
        /// <summary>
        /// 檢測項目單位 (ItemUnit)
        /// </summary>
        public string? Itemunit { get; set; }
        
        /// <summary>
        /// 備註 (Note)
        /// </summary>
        public string? Note { get; set; }
    }
}