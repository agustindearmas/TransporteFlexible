using System.ComponentModel;

namespace Common.Enums.Seguridad
{
    public enum SV
    {
        [Description("LoggedUserName")]
        LoggedUserName,
        [Description("Permissions")]
        Permissions,
        [Description("LoggedUserId")]
        LoggedUserId,
        [Description("EditingUserId")]
        EditingUserId,
        [Description("AssignedPermissions")]
        AssignedPermissions,
        [Description("DeallocatedPermissions")]
        DeallocatedPermissions,
        [Description("Emails")]
        Emails,
        [Description("Phones")]
        Phones,
        [Description("EditingPersonId")]
        EditingPersonId,
        [Description("Addresses")]
        Addresses,
        [Description("Provinces")]
        Provinces,
        [Description("Locations")]
        Locations,
        [Description("ModifyRoleId")]
        EdittingRoleId
    }
}
