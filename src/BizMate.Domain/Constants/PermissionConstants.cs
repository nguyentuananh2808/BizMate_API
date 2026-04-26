namespace BizMate.Domain.Constants
{
    /// <summary>
    /// Định nghĩa tất cả Permission string trong hệ thống.
    /// - Được seed vào DB 1 lần khi migrate.
    /// - Không bao giờ sửa giá trị string sau khi đã seed (sẽ mất mapping với DB).
    /// </summary>
    public static class PermissionConstants
    {
        public static class Order
        {
            public const string View = "order.view";
            public const string Create = "order.create";
            public const string Edit = "order.edit";
            public const string Cancel = "order.cancel";
            public const string Approve = "order.approve";
        }

        public static class Product
        {
            public const string View = "product.view";
            public const string Create = "product.create";
            public const string Edit = "product.edit";
            public const string Delete = "product.delete";
        }

        public static class ProductCategory
        {
            public const string View = "productcategory.view";
            public const string Create = "productcategory.create";
            public const string Edit = "productcategory.edit";
            public const string Delete = "productcategory.delete";
        }

        public static class Stock
        {
            public const string View = "stock.view";
            public const string Adjust = "stock.adjust";
        }

        public static class Customer
        {
            public const string View = "customer.view";
            public const string Create = "customer.create";
            public const string Edit = "customer.edit";
            public const string Delete = "customer.delete";
        }

        public static class Supplier
        {
            public const string View = "supplier.view";
            public const string Create = "supplier.create";
            public const string Edit = "supplier.edit";
            public const string Delete = "supplier.delete";
        }

        public static class ImportReceipt
        {
            public const string View = "importreceipt.view";
            public const string Create = "importreceipt.create";
            public const string Edit = "importreceipt.edit";
            public const string Cancel = "importreceipt.cancel";
        }

        public static class ExportReceipt
        {
            public const string View = "exportreceipt.view";
            public const string Create = "exportreceipt.create";
            public const string Edit = "exportreceipt.edit";
            public const string Cancel = "exportreceipt.cancel";
        }

        public static class DealerLevel
        {
            public const string View = "dealerlevel.view";
            public const string Create = "dealerlevel.create";
            public const string Edit = "dealerlevel.edit";
            public const string Delete = "dealerlevel.delete";
        }

        public static class Report
        {
            public const string View = "report.view";
            public const string Export = "report.export";
        }

        public static class User
        {
            public const string View = "user.view";
            public const string Create = "user.create";
            public const string Edit = "user.edit";
            public const string Delete = "user.delete";
        }

        public static class Role
        {
            public const string View = "role.view";
            public const string Create = "role.create";
            public const string Edit = "role.edit";
            public const string Delete = "role.delete";
            public const string Assign = "role.assign";
        }

        /// <summary>
        /// Trả về toàn bộ danh sách permission dạng (Name, DisplayName, Group)
        /// để dùng khi Seed vào DB.
        /// </summary>
        public static IEnumerable<(string Name, string DisplayName, string Group)> GetAll()
        {
            return new[]
            {
                // Order
                (Order.View,    "Xem đơn hàng",          "order"),
                (Order.Create,  "Tạo đơn hàng",           "order"),
                (Order.Edit,    "Sửa đơn hàng",           "order"),
                (Order.Cancel,  "Hủy đơn hàng",           "order"),
                (Order.Approve, "Duyệt đơn hàng",         "order"),
                // Product
                (Product.View,   "Xem sản phẩm",          "product"),
                (Product.Create, "Thêm sản phẩm",         "product"),
                (Product.Edit,   "Sửa sản phẩm",          "product"),
                (Product.Delete, "Xóa sản phẩm",          "product"),
                // ProductCategory
                (ProductCategory.View,   "Xem danh mục",  "productcategory"),
                (ProductCategory.Create, "Thêm danh mục", "productcategory"),
                (ProductCategory.Edit,   "Sửa danh mục",  "productcategory"),
                (ProductCategory.Delete, "Xóa danh mục",  "productcategory"),
                // Stock
                (Stock.View,   "Xem tồn kho",             "stock"),
                (Stock.Adjust, "Điều chỉnh tồn kho",      "stock"),
                // Customer
                (Customer.View,   "Xem khách hàng",       "customer"),
                (Customer.Create, "Thêm khách hàng",      "customer"),
                (Customer.Edit,   "Sửa khách hàng",       "customer"),
                (Customer.Delete, "Xóa khách hàng",       "customer"),
                // Supplier
                (Supplier.View,   "Xem nhà cung cấp",     "supplier"),
                (Supplier.Create, "Thêm nhà cung cấp",    "supplier"),
                (Supplier.Edit,   "Sửa nhà cung cấp",     "supplier"),
                (Supplier.Delete, "Xóa nhà cung cấp",     "supplier"),
                // ImportReceipt
                (ImportReceipt.View,   "Xem phiếu nhập",  "importreceipt"),
                (ImportReceipt.Create, "Tạo phiếu nhập",  "importreceipt"),
                (ImportReceipt.Edit,   "Sửa phiếu nhập",  "importreceipt"),
                (ImportReceipt.Cancel, "Hủy phiếu nhập",  "importreceipt"),
                // ExportReceipt
                (ExportReceipt.View,   "Xem phiếu xuất",  "exportreceipt"),
                (ExportReceipt.Create, "Tạo phiếu xuất",  "exportreceipt"),
                (ExportReceipt.Edit,   "Sửa phiếu xuất",  "exportreceipt"),
                (ExportReceipt.Cancel, "Hủy phiếu xuất",  "exportreceipt"),
                // DealerLevel
                (DealerLevel.View,   "Xem cấp đại lý",    "dealerlevel"),
                (DealerLevel.Create, "Thêm cấp đại lý",   "dealerlevel"),
                (DealerLevel.Edit,   "Sửa cấp đại lý",    "dealerlevel"),
                (DealerLevel.Delete, "Xóa cấp đại lý",    "dealerlevel"),
                // Report
                (Report.View,   "Xem báo cáo",            "report"),
                (Report.Export, "Xuất báo cáo",           "report"),
                // User
                (User.View,   "Xem nhân viên",            "user"),
                (User.Create, "Thêm nhân viên",           "user"),
                (User.Edit,   "Sửa nhân viên",            "user"),
                (User.Delete, "Xóa nhân viên",            "user"),
                // Role
                (Role.View,   "Xem phân quyền",           "role"),
                (Role.Create, "Tạo vai trò",              "role"),
                (Role.Edit,   "Sửa vai trò",              "role"),
                (Role.Delete, "Xóa vai trò",              "role"),
                (Role.Assign, "Gán vai trò cho nhân viên","role"),
            };
        }
    }
}