using System.Collections.Generic;

namespace SpartanGoogleApi
{
    public class Opening_Hours
    {
        public bool open_now { get; set; }
        public List<Period> periods { get; set; }
        public List<string> weekday_text { get; set; }
    }
}



