// FILE: src/BizMate.Domain/Constants/ApiNameConstants.cs
// Thêm ProductItem vào danh sách hiện có

namespace BizMate.Domain.Constants
{
    public class ApiNameConstants
    {
        // api name — hiện có
        public const string ApiV1            = "v1/";
        public const string ApiV2            = "v2/";
        public const string Authentication   = "auth";
        public const string Product          = "product";
        public const string ProductCategory  = "product-category";
        public const string Image            = "image";
        public const string InventoryReceipt = "inventory-receipt";
        public const string User             = "user";
        public const string Customer         = "customer";
        public const string DealerLevel      = "dealer-level";
        public const string DealerPrice      = "dealer-price";
        public const string ImportReceipt    = "import-receipt";
        public const string ExportReceipt    = "export-receipt";
        public const string Notification     = "notification";
        public const string Status           = "status";
        public const string ExportOrder      = "export-order";
        public const string Order            = "order";
        public const string HandlerFile      = "handler-file";

        // phân quyền
        public const string Role             = "role";
        public const string Permission       = "permission";

        // serial tracking (MỚI)
        public const string ProductItem      = "product-item";

        // action
        public const string RefreshToken     = "refreshtoken";
        public const string Login            = "login";
        public const string Upload           = "upload";
        public const string UserRegister     = "register";
        public const string Verify           = "verify";
        public const string ForgotPassword   = "forgot-password";
        public const string ResetPassword    = "reset-password";
    }
}
