using System;
using System.ComponentModel;

namespace CmdaFunding.Commons.Enums
{
    public enum RoleType : byte
    {
        [Description("None")]
        None = 0,
        [Description("User")]
        User = 1,
        [Description("Admin")]
        Admin = 2
    }
}
