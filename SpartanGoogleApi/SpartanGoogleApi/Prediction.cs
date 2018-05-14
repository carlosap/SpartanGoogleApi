using System.Collections.Generic;

namespace SpartanGoogleApi
{
    public class Prediction
    {
        public string description { get; set; }
        public string id { get; set; }
        public Matched_Substrings[] matched_substrings { get; set; }
        public string place_id { get; set; }
        public string reference { get; set; }
        public Structured_Formatting structured_formatting { get; set; }
        public List<Term> terms { get; set; }
        public List<string> types { get; set; }
    }

}



