using Microsoft.AspNetCore.Identity;

namespace Tea.Domain.Constants
{
    public class AppPermission
    {
        public const string Category_View = nameof(Category_View);
        public const string Category_Create = nameof(Category_Create);
        public const string Category_Edit = nameof(Category_Edit);
        public const string Category_Delete = nameof(Category_Delete);

        public const string Item_View = nameof(Item_View);
        public const string Item_Create = nameof(Item_Create);
        public const string Item_Edit = nameof(Item_Edit);
        public const string Item_Delete = nameof(Item_Delete);

        public const string Role_Create = nameof(Role_Create);
        public const string Role_Delete = nameof(Role_Delete);
        public const string Role_Edit = nameof(Role_Edit);
        public const string Role_View = nameof(Role_View);
        public const string Role_ChangePermission = nameof(Role_ChangePermission);

        public const string Access_Admin = nameof(Access_Admin);

        public static List<IdentityRoleClaim<string>> adminClaims = new List<IdentityRoleClaim<string>>()
        {
            //Category
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Category_View},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Category_Create},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Category_Edit},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Category_Delete},

            //Item
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Item_View},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Item_Create},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Item_Edit},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Item_Delete},

            // role
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Role_View},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Role_Create},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Role_Edit},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Role_Delete},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Role_ChangePermission},

            // access admin page
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Access_Admin},
        };
    }
}
