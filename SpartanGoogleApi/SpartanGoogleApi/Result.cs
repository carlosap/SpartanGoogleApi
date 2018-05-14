using System.Collections.Generic;

namespace SpartanGoogleApi
{
    public class Result
    {
        public List<Address_Components> address_components { get; set; }
        public string adr_address { get; set; }
        public string formatted_address { get; set; }
        public string formatted_phone_number { get; set; }
        public Geometry geometry { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public string international_phone_number { get; set; }
        public string name { get; set; }
        public Opening_Hours opening_hours { get; set; }
        public List<Photo> photos { get; set; }
        public string place_id { get; set; }
        public int price_level { get; set; }
        public float rating { get; set; }
        public string reference { get; set; }
        public List<Review> reviews { get; set; } = new List<Review>();
        public string scope { get; set; }
        public List<string> types { get; set; }
        public string url { get; set; }
        public int utc_offset { get; set; }
        public string vicinity { get; set; }
        public string website { get; set; }
    }
}



