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
        public string? EmsNo { get; set; }
        public string? FacName { get; set; }
        public string? Address { get; set; }
        public string? Unino { get; set; }
        public string? PerNo { get; set; }
        public string? PerSdate { get; set; }
        public string? PerEdate { get; set; }
        public string? PerType { get; set; }
        public string? PigTot { get; set; }
        public string? PerItem { get; set; }
        public string? PerWater { get; set; }
        public string? PerRecycle { get; set; }
        public string? PerStay { get; set; }
        public string? PerTrustee { get; set; }
        public string? PerDelegate { get; set; }
        public string? Let { get; set; }
        public string? LetTm2X { get; set; }
        public string? LetTm2Y { get; set; }
        public string? LetEmi { get; set; }
        public string? LetWatertype { get; set; }
        public string? EmiSdate { get; set; }
        public string? EmiEdate { get; set; }
        public string? EmiWater { get; set; }
        public string? EmiWaterunit { get; set; }
        public string? EmiItem { get; set; }
        public string? EmiValue { get; set; }
        public string? EmiUnits { get; set; }
        public string? EmiScalar { get; set; }
        public string? ItemValue { get; set; }
        public string? ItemUnits { get; set; }
        public string? ItemScalar { get; set; }
        public string? LetEast { get; set; }
        public string? LetNorth { get; set; }
    }
}