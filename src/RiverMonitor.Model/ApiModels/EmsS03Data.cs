namespace RiverMonitor.Model.ApiModels;

public class EmsS03Data
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
        /// 管制事業編號
        /// </summary>
        public string? EmsNo { get; set; }

        /// <summary>
        /// 事業名稱
        /// </summary>
        public string? FacName { get; set; }

        /// <summary>
        /// 實際廠（場）地址
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// 營利事業統一編號
        /// </summary>
        public string? Unino { get; set; }

        /// <summary>
        /// 許可證號
        /// </summary>
        public string? PerNo { get; set; }

        /// <summary>
        /// 許可證起始日
        /// </summary>
        public string? PerSdate { get; set; }

        /// <summary>
        /// 許可證截止日
        /// </summary>
        public string? PerEdate { get; set; }

        /// <summary>
        /// 水污染防治許可種類
        /// </summary>
        public string? PerType { get; set; }

        /// <summary>
        /// 養豬頭數
        /// </summary>
        public string? PigTot { get; set; }

        /// <summary>
        /// 廢（污）水處理設施單元名稱
        /// </summary>
        public string? PerItem { get; set; }

        /// <summary>
        /// 廢（污）水（前）處理設施處理水量
        /// </summary>
        public string? PerWater { get; set; }

        /// <summary>
        /// 回收量
        /// </summary>
        public string? PerRecycle { get; set; }

        /// <summary>
        /// 貯留水量
        /// </summary>
        public string? PerStay { get; set; }

        /// <summary>
        /// 受託處理水量
        /// </summary>
        public string? PerTrustee { get; set; }

        /// <summary>
        /// 委託處理水量
        /// </summary>
        public string? PerDelegate { get; set; }

        /// <summary>
        /// 放流口別
        /// </summary>
        public string? Let { get; set; }

        /// <summary>
        /// 放流口X座標
        /// </summary>
        public string? LetTm2x { get; set; }

        /// <summary>
        /// 放流口Y座標
        /// </summary>
        public string? LetTm2y { get; set; }

        /// <summary>
        /// 核准排放量
        /// </summary>
        public string? LetEmi { get; set; }

        /// <summary>
        /// 承受水體
        /// </summary>
        public string? LetWatertype { get; set; }

        /// <summary>
        /// 申報區間（起）
        /// </summary>
        public string? EmiSdate { get; set; }

        /// <summary>
        /// 申報區間（迄）
        /// </summary>
        public string? EmiEdate { get; set; }

        /// <summary>
        /// 排放水量
        /// </summary>
        public string? EmiWater { get; set; }

        /// <summary>
        /// 排放水量度量單位
        /// </summary>
        public string? EmiWaterunit { get; set; }

        /// <summary>
        /// 排放水質
        /// </summary>
        public string? EmiItem { get; set; }

        /// <summary>
        /// 排放濃度
        /// </summary>
        public string? EmiValue { get; set; }

        /// <summary>
        /// 排放度量單位
        /// </summary>
        public string? EmiUnits { get; set; }

        /// <summary>
        /// 數據小數點展示位數
        /// </summary>
        public string? EmiScalar { get; set; }

        /// <summary>
        /// 污染量
        /// </summary>
        public string? ItemValue { get; set; }

        /// <summary>
        /// 污染量單位
        /// </summary>
        public string? ItemUnits { get; set; }

        /// <summary>
        /// 污染量數據小數點展示位數
        /// </summary>
        public string? ItemScalar { get; set; }

        /// <summary>
        /// 東經
        /// </summary>
        public string? LetEast { get; set; }

        /// <summary>
        /// 北緯
        /// </summary>
        public string? LetNorth { get; set; }
    }
}