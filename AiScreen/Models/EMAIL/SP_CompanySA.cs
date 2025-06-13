namespace MaxSystemWebSite.Models.EMAIL
{
    public class SP_CompanySA
    {
        public SP_CompanySA()
        {
        }

        public SP_CompanySA(string sA_NAME, string sA_EMAIL)
        {
            SA_NAME = sA_NAME;
            SA_EMAIL = sA_EMAIL;
        }

        public string SA_NAME { get; set; }
        public string SA_EMAIL { get; set; }
    }
}
