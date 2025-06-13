namespace Base.Model
{
    public class AclResource
    {
        public int ID_ACL_RESOURCE { get; set; }
        public int RESOURCE_PARENT_ID { get; set; }
        public int ACCESS_LEVEL { get; set; }
        public string RESOURCE_NAME { get; set; }
        public string RESOURCE_VIEW { get; set; }
        public string RESOURCE_CONTROLLER { get; set; }
        public int VIEW_RIGHT { get; set; }
        public int ADD_RIGHT { get; set; }
        public int EDIT_RIGHT { get; set; }
        public int DELETE_RIGHT { get; set; }
        public int LAYER { get; set; }
        public int ACTION { get; set; }
        public string ICON { get; set; }
        public int SEQ { get; set; }
        public List<AclResource> ListChild { get; set; }
    }
    public class AclResource_Access 
    {
        public int ID_ACL_RESOURCE { get; set; }
        public int ACCESS_LEVEL { get; set; }
        public int VIEW_RIGHT { get; set; }
        public int ADD_RIGHT { get; set; }
        public int EDIT_RIGHT { get; set; }
        public int DELETE_RIGHT { get; set; }
    }
}
