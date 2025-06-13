using Base.Model;

namespace MaxSys.Models
{
    public class MM_EMAIL : BaseStandardModel
    {
        public int ID_MM_EMAIL { get; set; }
        public string CATEGORY { get; set; }
        public string SUBJECT { get; set; }
        public string BODY { get; set; }

    }

}
