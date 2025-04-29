using Microsoft.AspNetCore.Identity;
using Tea.Domain.Entities;

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

        public const string Discount_View = nameof(Discount_View);
        public const string Discount_Create = nameof(Discount_Create);
        public const string Discount_Edit = nameof(Discount_Edit);
        public const string Discount_Delete = nameof(Discount_Delete);


        public const string Order_View = nameof(Order_View);
        public const string Order_Create = nameof(Order_Create);
        public const string Order_Edit = nameof(Order_Edit);
        public const string Order_Delete = nameof(Order_Delete);

        public const string User_View = nameof(User_View);
        public const string User_Create = nameof(User_Create);
        public const string User_Edit = nameof(User_Edit);
        public const string User_Unlock = nameof(User_Unlock);

        public const string Report_View = nameof(Report_View);

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


            // discount
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Discount_Create},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Discount_Delete},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Discount_Edit},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Discount_View},

            // order
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Order_Create},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Order_Delete},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Order_View},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Order_Edit},

            // order
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=User_Create},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=User_Edit},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=User_View},
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=User_Unlock},

            //report
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Report_View},

            // access admin page
            new IdentityRoleClaim<string> {ClaimType=Auth.Permission, ClaimValue=Access_Admin},
        };
    }
}
