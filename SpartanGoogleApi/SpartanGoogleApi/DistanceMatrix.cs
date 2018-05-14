using System.Collections.Generic;
namespace SpartanGoogleApi
{
   
    public class DistanceMatrix
    {
        public List<string> destination_addresses { get; set; } = new List<string>();
        public List<string> origin_addresses { get; set; } = new List<string>();
        public List<Row> rows { get; set; } = new List<Row>();
        public string status { get; set; }
    }

    public class Row
    {
        public List<Element> elements { get; set; } = new List<Element>();
    }

    public class Element
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public string status { get; set; }
    }

    public class Distance
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Duration
    {
        public string text { get; set; }
        public int value { get; set; }
    }

}
