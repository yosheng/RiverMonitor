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
        public string? SiteId { get; set; }
        public string? SiteName { get; set; }
        public string? AnnoNo { get; set; }
        public string? DeannoNo { get; set; }
        public string? County { get; set; }
        public string? SiteType { get; set; }
        public string? SiteUse { get; set; }
        public string? Pollutant { get; set; }
        public string? Pollutantaddress { get; set; }
        public string? Dtmx { get; set; }
        public string? Dtmy { get; set; }
        public string? Controltype { get; set; }
        public string? AnnoDate { get; set; }
        public string? DeannoDate { get; set; }
        public string? Sitearea { get; set; }
        public string? Landno { get; set; }
        public string? Township { get; set; }
        public string? Wgs84Lng { get; set; }
        public string? Wgs84Lat { get; set; }
    }
}