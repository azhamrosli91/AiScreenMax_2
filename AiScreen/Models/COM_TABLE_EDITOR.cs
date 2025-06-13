namespace BaseModel.Models.Component
{
    public class COM_TABLE_EDITOR 
    {
        public int COM_TABLE_EDITOR_ID { get; set; }
        public string NAME { get; set; }
        public string CLASS { get; set; }
        public string TITLE { get; set; }
        public bool IS_DRAGGABLE { get; set; }
        public bool DISABLE { get; set; } = false;
        public bool TITLE_VISIBLE { get; set; } = true;
        public bool IS_ADD { get; set; } = true;
        public bool IS_SHOW_INMODAL { get; set; } = false;
        public string MODAL_SIZE { get; set; } = "modal-md";

        public bool MODAL_BTN_CLOSE_VISIBLE { get; set; } = true;
        public string MODAL_BTN_CLOSE_TEXT { get; set; } = "Tutup";

        public bool MODAL_BTN_ADD_UPDATE_VISIBLE { get; set; } = true;
        public string MODAL_BTN_ADD_TEXT { get; set; } = "Tambah";
        public string MODAL_BTN_UPDATE_TEXT { get; set; } = "Kemaskini";

        public bool MODAL_BTN_DELETE_VISIBLE { get; set; } = true;
        public string MODAL_BTN_DELETE_TEXT { get; set; } = "Buang";


        public string BTN_ADD_TEXT { get; set; } = "Add";
        public bool IS_REMOVE { get; set; } = true;
        public string BTN_REMOVE_TEXT { get; set; } = "Remove";
        public List<COM_TABLE_EDITOR_D> COM_TABLE_D { get; set; }
        public List<(int,List<COM_TABLE_EDITOR_D>)> LIST_DATA { get; set; }

    }

    public class COM_TABLE_EDITOR_D
    {
        public COM_TABLE_EDITOR_D()
        {

        }

        public COM_TABLE_EDITOR_D(int cOM_TABLE_EDITOR_D_ID, int cOM_TABLE_EDITOR_ID, string cOLUMN_NAME, string cOLUMN_TITLE, string cOLUMN_DATATYPE, string cOLUMN_OBJECT, int wIDTH, int? sEQ)
        {
            COM_TABLE_EDITOR_D_ID = cOM_TABLE_EDITOR_D_ID;
            COM_TABLE_EDITOR_ID = cOM_TABLE_EDITOR_ID;
            COLUMN_NAME = cOLUMN_NAME;
            COLUMN_TITLE = cOLUMN_TITLE;
            COLUMN_DATATYPE = cOLUMN_DATATYPE;
            COLUMN_OBJECT = cOLUMN_OBJECT;
            WIDTH = wIDTH;
            SEQ = sEQ;
        }

        public int COM_TABLE_EDITOR_D_ID { get; set; }
        public int COM_TABLE_EDITOR_ID { get; set; }
        public string COLUMN_NAME { get; set; } //Field naem
        public string COLUMN_TITLE { get; set; } //Table Header Name
        public string COLUMN_DATATYPE { get; set; } //data type int,varchar,nvarchar,decimal(18,2)
        public string COLUMN_OBJECT { get; set; } //input,select,button,radio,checkbox,inputgroup with button

        public int WIDTH { get; set; }
        public int? SEQ { get; set; }

    }
}
