

namespace http
{
    public class DescriptionJson
    {
        public string? xid { get; set; }
        public string? name { get; set; }
        public Address? address { get; set; }
        public string? kinds { get; set; }
        public Info? info { get; set; }
        public Wiki? wikipedia_extracts { get; set; }
    }

    public class Address 
    {
        public string? city { get; set; }
        public string? road { get; set; }
        public string? house { get; set; }
        public string? state { get; set; }
        public string? suburb { get; set; }
        public string? country { get; set; }
        public string? house_number { get; set; }
        public string? city_district { get; set; }
    }

    public class Info
    {
        public string? descr { get; set; }
    }

    public class Wiki
    {
        public string? text { get; set; }
    }
}