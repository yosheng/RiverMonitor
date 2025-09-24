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

    public class RecordItem
    {
        public string? Siteid { get; set; }
        public string? Sitename { get; set; }
        public string? Siteengname { get; set; }
        public string? Ugwdistname { get; set; }
        public string? County { get; set; }
        public string? Township { get; set; }
        public string? Twd97Lon { get; set; }
        public string? Twd97Lat { get; set; }
        public string? Twd97Tm2X { get; set; }
        public string? Twd97Tm2Y { get; set; }
        public string? Sampledate { get; set; }
        public string? Itemname { get; set; }
        public string? Itemengname { get; set; }
        public string? Itemengabbreviation { get; set; }
        public string? Itemvalue { get; set; }
        public string? Itemunit { get; set; }
        public string? Note { get; set; }
    }
}