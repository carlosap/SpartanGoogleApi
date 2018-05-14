using System.Collections.Generic;

namespace SpartanGoogleApi
{
    public class Near
    {
        public List<object> html_attributions { get; set; }
        public List<Result> results { get; set; }
        public string status { get; set; }
    }

}
