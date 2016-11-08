namespace MySite.Models
{
    public class LangDetail
    {
        public string name { get; set; }
        public string value { get; set; }
    }


    public class FileDetail
    {
        public string path { get; set; }
        public LangDetail[] langs { get; set; }
    }
}