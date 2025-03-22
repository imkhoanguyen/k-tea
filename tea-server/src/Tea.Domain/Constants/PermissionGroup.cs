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
                    new PermissionItemResponse {Name = "Tạo quyền", ClaimValue = AppPermission.Role_Create},
                    new PermissionItemResponse {Name = "Cập nhật quyền quyền", ClaimValue = AppPermission.Role_Edit},
                    new PermissionItemResponse {Name = "Xóa quyền", ClaimValue = AppPermission.Role_Delete},
                    new PermissionItemResponse {Name = "Thay đổi chức năng", ClaimValue = AppPermission.Role_ChangePermission}

                }
            },

             new PermissionGroupResponse
            {
                GroupName = "Quyền truy cập",
                Permissions = new List<PermissionItemResponse>
                {
                    new PermissionItemResponse {Name = "Trang Admin", ClaimValue = AppPermission.Access_Admin},
                    new PermissionItemResponse {Name = "Trang Quản lý danh mục", ClaimValue = AppPermission.Category_View},
                    new PermissionItemResponse {Name = "Trang Quản lý sản phẩm", ClaimValue = AppPermission.Item_View},
                }
            },
        };
    }
}
