namespace CmdaFunding.ViewModels
{
    public class UserAccessViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public int UserMaster { get; set; }
        public int UserAccessRights { get; set; }
        public int ChangePassword { get; set; }
        public List<GatewayWiseReportsViewModel> GatewaywiseReports { get; set; }
        public List<MISReportViewModel> MISReports { get; set; }

    }
}
