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
        public string? Annono { get; set; }
        public string? Annodate { get; set; }
        public string? Annotitle { get; set; }
        public string? Annocontent { get; set; }
        public string? Siteid { get; set; }
        public string? County { get; set; }
        public string? Issoil { get; set; }
        public string? Isgw { get; set; }
    }
}