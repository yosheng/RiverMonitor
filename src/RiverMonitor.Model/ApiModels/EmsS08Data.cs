namespace RiverMonitor.Model.ApiModels;

public class EmsS08Data
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
        /// 公告公文
        /// </summary>
        public string? Annono { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public string? Annodate { get; set; }
        
        /// <summary>
        /// 主旨
        /// </summary>
        public string? Annotitle { get; set; }

        /// <summary>
        /// 公告事項
        /// </summary>
        public string? Annocontent { get; set; }

        /// <summary>
        /// 場址代碼
        /// </summary>
        public string? Siteid { get; set; }

        /// <summary>
        /// 縣市
        /// </summary>
        public string? County { get; set; }

        /// <summary>
        /// 土壤污染管制區
        /// </summary>
        public string? Issoil { get; set; }

        /// <summary>
        /// 地下水污染管制區
        /// </summary>
        public string? Isgw { get; set; }
    }
}