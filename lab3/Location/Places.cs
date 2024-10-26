

namespace HttpPlaces
{
    public class Places
    {
        public Place[]? hits { get; set; }
    }

    public class Place
    {
        public string? name { get; set; }
        public string? country { get; set; }
        public long osm_id { get; set; }
        public string? osm_type { get; set; }
        public string? osm_key { get; set; }
        public string? city { get; set; }
        public string? osm_value { get; set; }
        public string? postcode { get; set; }
        public Point? point { get; set; }

    }
    public class Point 
    {
        public double lng { get; set;}
        public double lat { get; set; }
    }
}