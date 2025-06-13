namespace SmartTemplateCore.Models.COMMON
{
    public class SideBarContent
    {
            public int ID_ACL_RESOURCE { get; set; }
            public int RESOURCE_PARENT_ID { get; set; }
            public string RESOURCE_NAME { get; set; }
            public string RESOURCE_VIEW { get; set; }
            public string RESOURCE_CONTROLLER { get; set; }
            public string VIEW_RIGHT { get; set; }
            public string ADD_RIGHT { get; set; }
            public string EDIT_RIGHT { get; set; }
            public string DELETE_RIGHT { get; set; }
            public int LAYER { get; set; }
            public int ACTION { get; set; }
    }
}
