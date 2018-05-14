using System.Collections.Generic;

namespace SpartanGoogleApi
{
    public class PlaceDetailResponse
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string Website { get; set; }
        public Location Location { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> LocationTypes { get; set; } = new List<string>();
        public bool isOpen { get; set; }

    }
}



