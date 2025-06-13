using BaseModel.Models.Component;
using static MaxSys.Enum.Enum;

namespace MaxSys.Models
{
    public class COM_TABLE_CARD
    {
        public int COM_TABLE_CARD_ID { get; set; }
        public string NAME { get; set; }
        public string CLASS { get; set; }
        public string TITLE { get; set; }
        public bool ALLOW_FILTER { get; set; } = true;
        public bool ALLOW_CLICK { get; set; } = true;
        public List<COM_TABLE_CARD_D> COM_TABLE_CARD_D { get; set; }
    }
    public class COM_TABLE_CARD_D
    {
        public COM_TABLE_CARD_D(int cOM_TABLE_CARD_D_ID,int cOM_TABLE_CARD_ID, string iMAGE, COM_TABLE_CARD_D_IMAGE_TYPE iMAGE_TYPE, 
            bool tITLE_VISIBLE, string tITLE, bool bODY_VISIBLE, string bODY, bool fOOTER_VISIBLE, string fOOTER, string cLICK_URL)
        {
            COM_TABLE_CARD_D_ID = cOM_TABLE_CARD_D_ID;
            COM_TABLE_CARD_ID = cOM_TABLE_CARD_ID;
            IMAGE = iMAGE;
            IMAGE_TYPE = iMAGE_TYPE;
            TITLE_VISIBLE = tITLE_VISIBLE;
            TITLE = tITLE;
            BODY_VISIBLE = bODY_VISIBLE;
            BODY = bODY;
            FOOTER_VISIBLE = fOOTER_VISIBLE;
            FOOTER = fOOTER;
            CLICK_URL = cLICK_URL;
        }

        public COM_TABLE_CARD_D()
        {
                
        }
        public int COM_TABLE_CARD_D_ID { get; set; }
        public int COM_TABLE_CARD_ID { get; set; }
        public bool IMAGE_VISIBLE { get; set; }
        public string IMAGE { get; set; }
        public COM_TABLE_CARD_D_IMAGE_TYPE IMAGE_TYPE { get; set; } = 0; //0 URL, 1 BASE 64
        public bool TITLE_VISIBLE { get; set; }
        public string TITLE { get; set; }

        public bool BODY_VISIBLE { get; set; }
        public string BODY { get; set; }

        public bool FOOTER_VISIBLE { get; set; }
        public string FOOTER { get; set; }

        public string CLICK_URL { get; set; }
    }
}
