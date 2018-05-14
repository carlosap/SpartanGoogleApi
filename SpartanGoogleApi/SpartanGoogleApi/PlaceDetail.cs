using System.Collections.Generic;

namespace SpartanGoogleApi
{
    public class PlaceDetail
    {
        public List<object> html_attributions { get; set; } = new List<object>();
        public Result result { get; set; } = new Result();
        public string status { get; set; } = string.Empty;
        
    }
}



