namespace CmdaFunding.ViewModels
{
    public class SubMenuRecordViewModel
    {
        public int MenuId { get; set; }
        public string MenuName { get; set; }
        public string Type { get; set; }
        public string Url { get; set; }
        public int? ParentMenuId { get; set; }
        
    }
}
