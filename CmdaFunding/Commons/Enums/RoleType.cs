using System;
using System.ComponentModel;

namespace CmdaFunding.Commons.Enums
{
    public enum MenuType : byte
    {
        [Description("UserMaster")]
        UserMaster = 1,
        [Description("UserAccessRights")]
        UserAccessRights = 2,

        //Gateway-Reports
        [Description("Transaction")]
        Transaction = 4,
        [Description("Header")]
        Header = 5,
        [Description("LocalBody")]
        LocalBody = 6,
        [Description("District")]
        District = 7,
        [Description("Applicant")]
        Applicant = 8,

        //MISReports
        [Description("MISHeader")]
        MISHeader = 10,
        [Description("MISLocalBody")]
        MISLocalBody = 11,
        [Description("MISDistrict")]
        MISDistrict = 12,
        [Description("MISApplicant")]
        MISApplicant = 13,
        [Description("FileList")]
        FileList = 14,
        [Description("FileDetails")]
        FileDetails = 15,
        [Description("ChangePassword")]
        ChangePassword = 16
    }
}



/*
 UserMaster
UserAccessRights
GatewayReports
Transaction
Header
LocalBody
District
Applicant
MISReports
Header
LocalBody
District
Applicant
FileList
FileDetails
ChangePassword
*/
