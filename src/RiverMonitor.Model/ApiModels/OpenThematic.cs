namespace RiverMonitor.Model.ApiModels;

public class OpenThematic
{
    public class RootObject
    {
        public int StatusCode { get; set; }
        public string Error_msg { get; set; }
        public int Total_count { get; set; }
        public int Total_pages { get; set; }
        public int Page { get; set; }
        public DataItem[] Data { get; set; }
    }

    public class DataItem
    {
        public string? ID { get; set; }
        public string? Title { get; set; }
        public string? Organ { get; set; }
        public string? Catalog { get; set; }
        public int? Counter { get; set; }
        public int? Lnkcnt { get; set; }
        public string? LastUpdateTime { get; set; }
        public int? Messages { get; set; }
        public int? Collect { get; set; }
    }
}