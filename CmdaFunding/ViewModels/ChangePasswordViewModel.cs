namespace CmdaFunding.ViewModels
{
    public class ChangePasswordViewModel
    {
        public int userId {  get; set; }
        public string username { get; set; }
        public string oldpassword { get; set; }
        public string password { get; set; }
    }
}
