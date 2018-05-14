using System.Collections.Generic;

namespace SpartanGoogleApi
{
    public class AutoCompleteResponse
    {
        public string Id { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public List<string> PredictionTypes { get; set; }
    }
}



