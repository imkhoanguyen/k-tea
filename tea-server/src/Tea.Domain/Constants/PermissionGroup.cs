using Tea.Application.DTOs.Permissions;

namespace Tea.Domain.Constants
{
    public class PermissionGroup
    {
        public static List<PermissionGroupResponse> AllPermissionGroups = new List<PermissionGroupResponse>() {
            new PermissionGroupResponse
            {
                GroupName = "Quản lý danh mục",
                Permissions = new List<PermissionItemResponse>
                {
                    new PermissionItemResponse {Name = "Xem danh mục", ClaimValue = AppPermission.Category_View},
                    new PermissionItemResponse {Name = "Tạo danh mục", ClaimValue = AppPermission.Category_Create},
                    new PermissionItemResponse {Name = "Cập nhật danh mục", ClaimValue = AppPermission.Category_Edit},
                    new PermissionItemResponse {Name = "Xóa danh mục", ClaimValue = AppPermission.Category_Delete},
                }
            },

            new PermissionGroupResponse
            {
                GroupName = "Quản lý sản phẩm",
                Permissions = new List<PermissionItemResponse>
                {
                    new PermissionItemResponse {Name = "Xem sản phẩm", ClaimValue = AppPermission.Item_View},
                    new PermissionItemResponse {Name = "Tạo sản phẩm", ClaimValue = AppPermission.Item_Create},
                    new PermissionItemResponse {Name = "Cập nhật sản phẩm", ClaimValue = AppPermission.Item_Edit},
                    new PermissionItemResponse {Name = "Xóa sản phẩm", ClaimValue = AppPermission.Item_Delete},
                }
            },

            new PermissionGroupResponse
            {
                GroupName = "Quản lý quyền",
                Permissions = new List<PermissionItemResponse>
                {
                    new PermissionItemResponse {Name = "Xem quyền", ClaimValue = AppPermission.Role_View},
                    new PermissionItemResponse {Name = "Tạo quyền", ClaimValue = AppPermission.Role_Create},
                    new PermissionItemResponse {Name = "Cập nhật quyền quyền", ClaimValue = AppPermission.Role_Edit},
                    new PermissionItemResponse {Name = "Xóa quyền", ClaimValue = AppPermission.Role_Delete},
                    new PermissionItemResponse {Name = "Thay đổi chức năng", ClaimValue = AppPermission.Role_ChangePermission}
                }
            },

            new PermissionGroupResponse
            {
                GroupName = "Quản lý phiếu giảm giá",
                Permissions = new List<PermissionItemResponse>
                {
                    new PermissionItemResponse {Name = "Xem phiếu giảm giá", ClaimValue = AppPermission.Discount_View},
                    new PermissionItemResponse {Name = "Tạo phiếu giảm giá", ClaimValue = AppPermission.Discount_Create},
                    new PermissionItemResponse {Name = "Cập nhật phiếu giảm giá", ClaimValue = AppPermission.Discount_Edit},
                    new PermissionItemResponse {Name = "Xóa phiếu giảm giá", ClaimValue = AppPermission.Discount_Delete},
                }
            },

            new PermissionGroupResponse
            {
                GroupName = "Quản lý đơn hàng",
                Permissions = new List<PermissionItemResponse>
                {
                    new PermissionItemResponse {Name = "Xem đơn hàng", ClaimValue = AppPermission.Order_View},
                    new PermissionItemResponse {Name = "Tạo đơn hàng", ClaimValue = AppPermission.Order_Create},
                    new PermissionItemResponse {Name = "Cập nhật đơn hàng", ClaimValue = AppPermission.Order_Edit},
                    new PermissionItemResponse {Name = "Xóa đơn hàng", ClaimValue = AppPermission.Order_Delete},
                }
            },


            new PermissionGroupResponse
            {
                GroupName = "Quản lý người dùng",
                Permissions = new List<PermissionItemResponse>
                {
                    new PermissionItemResponse {Name = "Xem người dùng", ClaimValue = AppPermission.User_View},
                    new PermissionItemResponse {Name = "Tạo người dùng", ClaimValue = AppPermission.User_Edit},
                    new PermissionItemResponse {Name = "Cập nhật người dùng", ClaimValue = AppPermission.User_Create},
                    new PermissionItemResponse {Name = "Mở khóa người dùng", ClaimValue = AppPermission.User_Unlock},
                }
            },


             new PermissionGroupResponse
            {
                GroupName = "Quyền truy cập",
                Permissions = new List<PermissionItemResponse>
                {
                    new PermissionItemResponse {Name = "Trang Admin", ClaimValue = AppPermission.Access_Admin},
                }
            },
        };
    }
}
