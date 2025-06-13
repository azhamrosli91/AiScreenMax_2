namespace MaxSys.Interface
{
    public interface IButton
    {
        public string CreateForm_ButtonID { get; set; }
        public string CreateForm_ButtonName { get; set; }
        public string CreateForm_ButtonText { get; set; }
        public string CreateForm_ButtonIcon { get; set; }
        public string CreateForm_ButtonIcon_Color { get; set; }
        public string CreateForm_ButtonColor { get; set; }
        public string CreateForm_ButtonTextColor { get; set; }
        public string CreateForm_ButtonType { get; set; }
        public string CreateForm_ButtonPath { get; set; }
        public string CreateForm_ButtonFunctionName { get; set; }
        public bool CreateForm_ButtonVisible { get; set; }

    }
}
