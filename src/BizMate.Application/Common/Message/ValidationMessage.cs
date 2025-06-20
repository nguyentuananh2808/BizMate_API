namespace BizMate.Public.Message
{
    public static class ValidationMessage
    {
        public static class Strings
        {
            public const string PropertyName = "PropertyName";

            public const string ComparisonValue = "ComparisonValue";

            public const string MaxLength = "MaxLength";

            public const string PropertyValue = "PropertyValue";

            public const string CommaSeparator = ",";

            public const string MinLength = "MinLength";

            public const string PropertyName1 = "PropertyName1";

            public const string PropertyValue1 = "PropertyValue1";

            public const string MustNotEmpty = "{PropertyName} must not be empty";

            public const string MustBeValid = "{PropertyValue} is not valid";

            public const string OperatorInvalid = "This opertor {0} is not support.";

            public const string OperatorNull = "Operator should not be empty.";

            public const string DataNotExist = "{0} with {1} doesn't exist";

            public const string DataNotExist2 = "Key {0}: {1} with {2} doesn't exist";

            public const string UserNameMustBeEqualEmail = "{PropertyValue} must be equal to email {ComparisonValue}";

            public const string NotBelongTo = "{PropertyName}: {PropertyValue} is not belong to {PropertyName1}: {PropertyValue1}";

            public const string BackendLookupValueUnHandled = "{0}: {1} had not been handled by {2}";

            public const string BackgroundJobRequestHasBeenProcessed = "{0}: Request has been processed already {1}";

            public const string MissingInfo = "There is missing info for {0} with Ids: {1}";

            public const string NotJsonString = "The given object {0} is not of Json type";

            public const string GetUserError = "Get users error";
        }

        public static class LocalizedStrings
        {
            public const string MustNotEmpty = "BACKEND.VALIDATION.MESSAGE.MUST_NOT_EMPTY";

            public const string MustContainUppercase = "BACKEND.VALIDATION.MESSAGE.PASSWORD_MUST_CONTAIN_UPPERCASE";

            public const string MustContainLowercase = "BACKEND.VALIDATION.MESSAGE.PASSWORD_MUST_CONTAIN_LOWERCASE";

            public const string MustContainDigit = "BACKEND.VALIDATION.MESSAGE.PASSWORD_MUST_CONTAIN_DIGIT";

            public const string MustContainSpecialChar = "BACKEND.VALIDATION.MESSAGE.PASSWORD_MUST_CONTAIN_SPECIAL_CHAR";

            public const string MustEqualThan = "BACKEND.VALIDATION.MESSAGE.MUST_EQUAL_TO";

            public const string MustGreaterThan = "BACKEND.VALIDATION.MESSAGE.MUST_GREATER_THAN";

            public const string MustGreaterThan2 = "BACKEND.VALIDATION.MESSAGE.MUST_GREATER_THAN2";

            public const string MustGreaterThanOrEqualTo = "BACKEND.VALIDATION.MESSAGE.MUST_GREATER_THAN_OR_EQUAL_TO";

            public const string MustLessThanKeyValue = "BACKEND.VALIDATION.MESSAGE.MUST_LESS_THAN_KEY_VALUE";

            public const string MustLessThanKey = "BACKEND.VALIDATION.MESSAGE.MUST_LESS_THAN_KEY";

            public const string KeyPropertyMustGreaterThanOrEqualTo = "BACKEND.VALIDATION.MESSAGE.PROPERTY_KEY_MUST_LESS_THAN_OR_EQUAL_TO";

            public const string MustHaveLength = "BACKEND.VALIDATION.MESSAGE.MUST_HAVE_LENGTH";

            public const string MustHaveMinLength = "BACKEND.VALIDATION.MESSAGE.MUST_HAVE_MIN_LENGTH";

            public const string MustHaveLengthBetween = "BACKEND.VALIDATION.MESSAGE.MUST_HAVE_LENGTH_BETWEEN";

            public const string MustBeValid = "BACKEND.VALIDATION.MESSAGE.MUST_BE_VALID";

            public const string MustUnique = "BACKEND.VALIDATION.MESSAGE.MUST_UNIQUE";

            public const string CannotDelete = "BACKEND.VALIDATION.MESSAGE.CAN_NOT_DELETE";

            public const string CannotUpdate = "BACKEND.VALIDATION.MESSAGE.CAN_NOT_UPDATE";

            public const string CannotChangeStatus = "BACKEND.VALIDATION.MESSAGE.CAN_NOT_CHANGE_STATUS";

            public const string DataNotNull = "BACKEND.VALIDATION.MESSAGE.DATA_NOT_NULL";

            public const string MultipleValues = "BACKEND.VALIDATION.MESSAGE.MULTIPLE_VALUES";

            public const string InvalidStatusTransition = "BACKEND.VALIDATION.MESSAGE.INVALID_STATUS_TRANSITION";

            public const string IllegalStatusTransition = "BACKEND.VALIDATION.MESSAGE.ILLEGAL_STATUS_TRANSITION";

            public const string InvalidEnumValue = "BACKEND.VALIDATION.MESSAGE.INVALID_ENUM_VALUE";

            public const string ExpiredDateNotValid = "BACKEND.VALIDATION.MESSAGE.EXPIRED_DATE_NOT_VALID";

            public const string RecordBeingUsed2 = "BACKEND.VALIDATION.MESSAGE.RECORD_BEING_USED_2";

            public const string RecordBeingUsed3 = "BACKEND.VALIDATION.MESSAGE.RECORD_BEING_USED_3";

            public const string MustBeLessThanOrEqualTo = "BACKEND.VALIDATION.MESSAGE.MUST_LESS_THAN_OR_EQUAL_TO";

            public const string TotalInvoiceAmountGreaterThanOrder = "BACKEND.VALIDATION.MESSAGE.TOTAL_INVOICE_AMT_GREATER_THAN_ORDER";

            public const string TotalInvoiceSubUnitIsGreaterThanOrder = "BACKEND.VALIDATION.MESSAGE.TOTAL_INVOICE_SUB_UNIT_GREATER_THAN_ORDER";

            public const string DoesNotSupportInvoiceWithDiffTaxRate = "BACKEND.VALIDATION.MESSAGE.DOES_NOT_SUPPORT_INVOICE_WITH_DIFFERENCE_TAX_RATE";

            public const string ShipmentUnitGreaterThanSourceLine = "BACKEND.VALIDATION.MESSAGE.SHIPMENT_UNIT_GREATER_THAN_SOURCE_LINE";

            public const string IllegalStatus = "BACKEND.VALIDATION.MESSAGE.ILLEGAL_STATUS";

            public const string MultipleMatchingResults = "BACKEND.VALIDATION.MESSAGE.MULTIPLE_MATCHING_RESULTS";

            public const string CustomerMainAddressEmpty = "BACKEND.VALIDATION.MESSAGE.CUSTOMER_MAIN_ADDRESS_EMPTY";

            public const string SourceNotSupportType = "BACKEND.VALIDATION.MESSAGE.SOURCE_TYPE_NOT_SUPPORT";

            public const string MustHaveRightFormat = "BACKEND.VALIDATION.MESSAGE.MUST_HAVE_RIGHT_FORMAT";

            public const string AppMustGreaterThanOrEqualTo = "BACKEND.VALIDATION.APP.MESSAGE.MUST_GREATER_THAN_OR_EQUAL_TO";

            public const string OrderReturnItemsNotBelongToSource = "BACKEND.VALIDATION.MESSAGE.ORDER_RETURN_ITEMS_NOT_BELONG_TO_SOURCE";

            public const string OrderReplacementItemsNotBelongToSource = "BACKEND.VALIDATION.MESSAGE.ORDER_REPLACEMENT_ITEMS_NOT_BELONG_TO_SOURCE";

            public const string OrderAllowChangeOrderWarehouseStatus = "BACKEND.VALIDATION.MESSAGE.ORDER_ALLOW_TO_CHANGE_ORDER_WAREHOUSE_STATUS";

            public const string CanNotDeleteOrderReturnWithExistReplacement = "BACKEND.VALIDATION.MESSAGE.ORDER_CAN_NOT_DELETE_ORDER_RETURN_WITH_EXIST_REPLACEMENT";

            public const string InventoryJournalNotFoundBySource = "BACKEND.VALIDATION.MESSAGE.INVENTORY_JOURNAL_NOT_FOUND_BY_SOURCE";

            public const string InventoryWarehouseNotOccurAnyTransaction = "BACKEND.VALIDATION.MESSAGE.WAREHOUSE_NOT_OCCUR_ANY_TRANSACTION";

            public const string InvalidRequest = "COMMON.VALIDATION.INVALID_REQUEST";

            public const string DateMustHaveSpecifiedKind = "COMMON.VALIDATION.DATE_MUST_HAVE_SPECIFIED_KIND";

            public const string AlreadyAssociatedWithExtRefNumber = "BACKEND.VALIDATION.MESSAGE.ALREADY_ASSOCIATED_WITH_EXT_REF_NUMBER";

            public const string PropertyMustLessThanOrEqualTo = "BACKEND.VALIDATION.MESSAGE.PROPERTY_MUST_LESS_THAN_OR_EQUAL_TO";

            public const string PropertyMustEqualThan = "BACKEND.VALIDATION.MESSAGE.PROPERTY_MUST_EQUAL_TO";

            public const string NotJsonString = "BACKEND.VALIDATION.MESSAGE.NOT_JSON_TYPE";

            public const string CancelUnitsNotMatch = "BACKEND.VALIDATION.MESSAGE.CANCEL_UNITS_NOT_MATCH";

            public const string DeliveredUnitsNotMatch = "BACKEND.VALIDATION.MESSAGE.DELIVERED_UNITS_NOT_MATCH";

            public const string InvalidStatusCancelUnit = "BACKEND.VALIDATION.MESSAGE.INVALID_STATUS_CANCEL_UNIT";

            public const string NotExistOrderItemInOrder = "BACKEND.VALIDATION.MESSAGE.NOT_EXIST_ORDER_ITEM_IN_ORDER";

            public const string ShipmentExistInDelivery = "BACKEND.VALIDATION.MESSAGE.SHIPMENT_EXIST_IN_DELIVERY";

            public const string CarryNotExist = "BACKEND.VALIDATION.MESSAGE.CARRY_NOT_EXIST";

            public const string NotSupportImportInventoryDocument = "BACKEND.VALIDATION.MESSAGE.NOT_SUPPORT_IMPORT_INVENTORY_DOCUMENT";

            public const string WarehouseNotContainAnyItem = "BACKEND.VALIDATION.MESSAGE.WAREHOUSE_NOT_CONTAIN_ANY_ITEM";

            public const string PleaseChooseProduct = "MESSAGE_CHOOSE_ORDER_RETURN_ITEM";

            public const string CannotFindFileResource = "BACKEND.VALIDATION.MESSAGE.CANNOT_FIND_FILE_RESOURCE";

            public const string PleaseFilterDataByRequestedDate = "BACKEND.APP_MESSAGE.PLEASE_FILTER_REQUIRED_DATE";

            public const string PleaseFilterRequestedDateIn90Day = "BACKEND.APP_MESSAGE.PLEASE_FILTER_REQUIRED_DATE_IN_90_DAY";

            public const string BusinessUnitNotMatchDepartment = "BACKEND.APP_MESSAGE.BUSINESS_UNIT_NOT_MATCH_DEPARTMENT";

            public const string RequestedDateInValid = "BACKEND.APP_MESSAGE.REQUESTED_DATE_INVALID";

            public const string CustomerDoNotHaveCustomerAddress = "BACKEND.APP_MESSAGE.CUSTOMER_DO_NOT_HAVE_CUSTOMER_ADDRESS";

            public const string ExistCustomerAddressType = "BACKEND.APP_MESSAGE.EXIST_CUSTOMER_ADDRESS_TYPE";

            public const string ExistInvoiceCannotCancelOrder = "BACKEND.EXIST_INVOICE_CANNOT_CANCEL_ORDER";

            public const string ExistShipmentCannotCancelOrder = "BACKEND.EXIST_SHIPMENT_CANNOT_CANCEL_ORDER";

            public const string CannotUpdateCustomerWithStatus = "BACKEND.CANNOT_UPDATE_CUSTOMER_WITH_STATUS";

            public const string NotAllowCreatePriceRequestWithCustomerStatus = "BACKEND.NOT_ALLOW_CREATE_PRICE_REQUEST_WITH_CUSTOMER_STATUS";

            public const string NotAllowCreateOrderWithCustomerStatus = "BACKEND.NOT_ALLOW_CREATE_ORDER_WITH_CUSTOMER_STATUS";

            public const string PropertyComparesMustNotNull = "BACKEND.PROPERTY_COMPARES_MUST_NOT_NULL";

            public const string CannotCancelManualInvoice = "BACKEND.INVOICE.MESSAGE.CAN_NOT_CANCEL_MANUAL";

            public const string CannotDeleteCustomerIfExistShipment = "BACKEND.APP_MESSAGE.NOT_DELETE_CUSTOMER_EXIST_SHIPMENT";

            public const string MustApplyLatLong = "BACKEND.DATA_CHANGE.MESSAGE.MUST_APPLY_LAT_LONG";

            public const string TagIsInUse = "BACKEND.STORAGE.MESSAGE.TAG_IS_IN_USE";

            public const string CannotFindTemplateExcel = "IMPORT_EXPORT.CANNOT_FIND_TEMPLATE_FILE_EXCEL";

            public const string ValidateDateFromToLessThan = "BACKEND.APP_MESSAGE.VALIDATE_DATE_FROM_TO_LESS_THAN";

            public const string MustHavePricing = "BACKEND.VALIDATION.MESSAGE.X_MUST_HAVE_PRICING";

            public const string CannotChangeStatusPricingDefault = "BACKEND.PRICING.MESSAGE.INVALID_CHANGE_STATUS_DEFAULT_PRICING";

            public const string BuMustHaveDefaultPricingActive = "BACKEND.APP_MESSAGE.BU_MUST_HAVE_DEFAULT_PRICING_ACTIVE";

            public const string ProductInActive = "BACKEND.PRICING_ITEM.MESSAGE.INVALID_PRODUCT_INACTIVE";

            public const string PricingInActive = "BACKEND.PRICING_ITEM.MESSAGE.INVALID_PRICING_INACTIVE";

            public const string BusinessUnitAlreadyExistProduct = "BACKEND.PRODUCT_BUSINESS_UNIT.MESSAGE.BUSINESS_UNIT_ALREADY_EXIST_PRODUCT";

            public const string BusinessUnitNotExistProduct = "BACKEND.PRODUCT_BUSINESS_UNIT.MESSAGE.BUSINESS_UNIT_NOT_EXIST_PRODUCT";

            public const string ChildProductNotExist = "BACKEND.PRODUCT.MESSAGE.NOT_EXIST_CHILD_PRODUCT";

            public const string IncorrectTax = "BACKEND.VALIDATION.MESSAGE.INCORRECT_TAX";

            public const string SettingNotFound = "BACKEND.VALIDATION.MESSAGE.SETTING_NOT_FOUND";

            public const string AlreadyExist = "BACKEND.VALIDATION.MESSAGE.ALREADY_EXIST";

            public const string ObjectAlreadyExist = "BACKEND.VALIDATION.MESSAGE_OBJECT_ALREADY_EXIST";

            public const string NotAllowToDoXWithCustomerStatus = "BACKEND.VALIDATION.MESSAGE_NOT_ALLOW_TO_DO_X_WITH_CUSTOMER_STATUS";

            public const string ObjectInActive = "BACKEND.OBJECT.MESSAGE.INVALID_OBJECT_INACTIVE";

            public const string SettingNotSupport = "BACKEND.VALIDATION.MESSAGE_SETTING_NOT_SUPPORT";

            public const string BiilNoDuplicate = "BACKEND.VALIDATION.MESSAGE_BILL_NO_DUPLICATE";

            public const string PricingExpired = "BACKEND.VALIDATION.MESSAGE_PRICING_EXPIRED";

            public const string CannotCreateXForNonInvProduct = "BACKEND.VALIDATION.MESSAGE_CAN_NOT_CREATE_X_FOR_NON_INV_PRODUCT";

            public const string SettingInvalidFormatDecimal = "BACKEND.VALIDATION.MESSAGE.SETTING_INVALID_FORMAT_DECIMAL";

            public const string OrderNotAllowToAddOtherItemToPromoGroup = "BACKEND.VALIDATION.MESSAGE.NOT_ALLOW_TO_ADD_OTHER_ITEM_TO_PROMO_GROUP";

            public const string OrderDoesNotDeliveredAnyItems = "BACKEND.VALIDATION.MESSAGE.ORDER_DOES_NOT_DELIVED_ANY_ITEMS";

            public const string OrderItemDoesNotReturnAll = "BACKEND.VALIDATION.MESSAGE.ORDER_ITEM_DOSE_NOT_RETURN_ALL";

            public const string OrderItemDoesNotDelivered = "BACKEND.VALIDATION.MESSAGE.ORDER_ITEM_DOSE_NOT_DELIVERED";

            public const string OrderItemRequestToReturnUnitsGreaterThanTheRemainUnits = "BACKEND.VALIDATION.MESSAGE.ORDER_ITEM_REQUEST_TO_RETURN_UNITS_GREATER_THAN_THE_REMAIN_UNITS";

            public const string OrderReplaceItemUnitsGreaterThanOrderReturnItemUnits = "BACKEND.VALIDATION.MESSAGE.ORDER_REPLACE_ITEM_UNITS_GREATER_THAN_ORDER_RETURN_ITEM_UNITS";

            public const string OrderItemRequestToReplaceUnitsGreaterThanTheRemainUnits = "BACKEND.VALIDATION.MESSAGE.ORDER_ITEM_REQUEST_TO_REPLACE_UNITS_GREATER_THAN_THE_REMAIN_UNITS";

            public const string InvoiceMessageSyncInvoiceAppliedBalance = "BACKEND.INVOICE.MESSAGE.SYNC_INVOICE_APPLIED_BALANCE";

            public const string NotFoundEInvoiceGatewayUrlForBusinessUnit = "EINVOICE_GATEWAY.MESSAGE_NOT_FOUND_GATEWAY_URL_FOR_BUSINESS_UNIT";

            public const string NotFoundEInvoiceTemplateForBusinessUnit = "EINVOICE_TEMPLATE.MESSAGE_NOT_FOUND_TEMPLATE_FOR_BUSINESS_UNIT";

            public const string NotFoundEInvoiceCertificateForBusinessUnit = "EINVOICE_CERTIFICATE.MESSAGE_NOT_FOUND_CERTIFICATE_FOR_BUSINESS_UNIT";

            public const string InvoiceDoNotCreateEInvoice = "INVOICE.MESSAGE_NOT_FOUND_EINVOICE";

            public const string BusinessUnitHaveBeenEInvoiceCertificate = "EINVOICE_CERTIFICATE.MESSAGE_EXISTS_IN_BUSINESS_UNIT";

            public const string InvoiceHasBeenHaveEInvoice = "INVOICE.MESSAGE_EXISTS_EINVOICE";

            public const string EInvoiceInvalidCancelWithStatus = "BACKEND.EINVOICE.MESSAGE.INVALID_CANCEL_WITH_STATUS";

            public const string EInvoiceInvalidSaveFilesWithStatus = "BACKEND.EINVOICE.MESSAGE.INVALID_SAVE_FILE_WITH_STATUS";

            public const string CannotCancelEInvoice = "BACKEND.EINVOICE.MESSAGE.CAN_NOT_CANCEL_EINVOICE";

            public const string CannotDeleteEInvoice = "BACKEND.EINVOICE.MESSAGE.CAN_NOT_DELETE_EINVOICE";

            public const string CannotDeleteEInvoiceDueToEzInvoice = "BACKEND.EINVOICE.MESSAGE.CAN_NOT_DELETE_EINVOICE_DUE_TO_EZ_INVOICE";

            public const string CannotIssuanceManualEInvoiceDueToEzInvoice = "BACKEND.EINVOICE.MESSAGE.CAN_NOT_ISSUANCE_MANUAL_EINVOICE_DUE_TO_EZ_INVOICE";

            public const string CannotDeleteEInvoiceFromCoreEInvoice = "BACKEND.EINVOICE.MESSAGE.CAN_NOT_DELETE_EINVOICE_FROM_CORE_EINVOICE";

            public const string NumberDeleteProductBuSuccess = "BACKEND.APP_MESSAGE.NUMBER_DELETE_PRODUCT_BUSINESS_UNIT_SUCCESS";

            public const string InfoOfKatgoAndEzInvoiceDoesNotMatch = "BACKEND.APP_MESSAGE.INFO_OF_KATGO_AND_EZ_INVOICE_DOES_NOT_MATCH";

            public const string InvoiceStatusInValidToSendEInvoice = "BACKEND.EINVOICE.MESSAGE.INVOICE_STATUS_INVALID_TO_SEND_E_INVOICE";

            public const string InvoiceAmountInWordsIsNotNull = "BACKEND.EINVOICE.MESSAGE.INVOICE_AMOUNT_IN_WORDS_IS_NOT_NULL";

            public const string InvoiceDoNotIntegrateWithManyBusinessUnits = "BACKEND.EINVOICE.MESSAGE.DO_NOT_INTEGRATE_WITH_MANY_BUSINESS_UNITS";

            public const string InvoiceExceedTheMaximumQuantityIssuance = "BACKEND.EINVOICE.MESSAGE.EXCEED_THE_MAXIMUM_QUANTITY_ISSUANCE";

            public const string CannotCancelEInvoiceDueToReceiptApply = "BACKEND.EINVOICE.MESSAGE.CAN_NOT_CANCEL_DUE_TO_RECEIPT_APPLY";

            public const string NotFoundDispatchNoteInDelivery = "BACKEND.DISPATCH_NOTE.MESSAGE_NOT_FOUND_IN_DELIVERY";

            public const string DispatchNoteNotSentToFbs = "BACKEND.DISPATCH_NOTE.MESSAGE_NOT_SENT_TO_FBS";

            public const string DispatchNoteNotSentToEInvoice = "BACKEND.DISPATCH_NOTE.MESSAGE_NOT_SENT_TO_EINVOICE";

            public const string DispatchNoteSentToEInvoiceCannotDoX = "BACKEND.DISPATCH_NOTE.MESSAGE_SENT_TO_EINVOICE_CAN_NOT_DO_X";

            public const string DispatchNoteSentToFbsCannotDoX = "BACKEND.DISPATCH_NOTE.MESSAGE_SENT_TO_FBS_CAN_NOT_DO_X";

            public const string DispatchNoteHaveDeliveryItemInvalidUnit = "BACKEND.DISPATCH_NOTE.MESSAGE_DELIVERY_ITEM_INVALID_UNIT";

            public const string DispatchNoteItemNotExistDeliveryItem = "BACKEND.DISPATCH_NOTE.MESSAGE_DELIVERY_ITEM_NOT_EXIST";

            public const string NotFoundGatewayForThirdParty = "BACKEND.MESSAGE_NOT_FOUND_GATEWAY_FOR_THIRD_PARTY";

            public const string IntegrationAmisNotSupportType = "BACKEND.MESSAGE_INTEGRATION_AMIS_NOT_SUPPORT_TYPE";

            public const string SalesIntegrationNotSupportObjectType = "BACKEND.MESSAGE_SALES_INTEGRATION_NOT_SUPPORT_OBJECT_TYPE";

            public const string SettingIntegrationNotSupportThirdParty = "BACKEND.MESSAGE_SETTING_INTEGRATION_NOT_SUPPORT_THIRD_PARTY";

            public const string NotSupportIntegrationCommonGiftWithOtherTypes = "BACKEND.MESSAGE_NOT_SUPPORT_COMMON_GIFT_WITH_OTHER_TYPES";

            public const string ValidateItemKindPromotion = "BACKEND.VALIDATION.MESSAGE.ITEM_KIND_PROMOTION_MUST_EQUAL_TO";

            public const string PleaseInputDiscountAccount = "BACKEND.VALIDATION.MESSAGE.PLEASE_INPUT_DISCOUNT_ACCOUNT";

            public const string IntegrateInvDocIssueToThirdPartySuccessfully = "BACKEND.VALIDATION.MESSAGE.INTEGREATE_INV_DOC_ISSUE_TO_THIRD_PARTY_SUCCESSFULLY";

            public const string IntegrateInvoiceToThirdPartySuccessfully = "BACKEND.VALIDATION.MESSAGE.INTEGREATE_INVOICE_TO_THIRD_PARTY_SUCCESSFULLY";

            public const string IntegrateOtherDocToThirdPartySuccessfully = "BACKEND.VALIDATION.MESSAGE.INTEGREATE_OTHER_DOC_TO_THIRD_PARTY_SUCCESSFULLY";

            public const string InfoGetAccessToken = "BACKEND.MESSAGE_GET_ACCESS_TOKEN_INFO";

            public const string ReasonTypeIdOfCaReceiptNotMatch = "BACKEND.MESSAGE_REASON_TYPE_ID_OF_CA_RECEIPT_NOT_MATCH";

            public const string ReasonTypeIdOfBaDepositNotMatch = "BACKEND.MESSAGE_REASON_TYPE_ID_OF_BA_DEPOSIT_NOT_MATCH";

            public const string ThirdPartyResponseNotFound = "BACKEND.VALIDATION.MESSAGE.THIRD_PARTY_RESPONSE_NOT_FOUND";

            public const string DocumentNotSentToThirdParty = "BACKEND.VALIDATION.MESSAGE.DOCUMENT_NOT_SENT_TO_THIRD_PARTY";

            public const string DocumentSendingToThirdParty = "BACKEND.VALIDATION.MESSAGE.DOCUMENT_SENDING_TO_THIRD_PARTY";

            public const string DataDuplicate = "BACKEND.APP_MESSAGE.DATA_DUPLICATE";

            public const string DataDuplicate2 = "BACKEND.APP_MESSAGE.DATA_DUPLICATE2";

            public const string DataDuplicate3 = "BACKEND.APP_MESSAGE.DATA_DUPLICATE3";

            public const string DataNotExist = "BACKEND.APP_MESSAGE.DATA_NOT_EXIST";

            public const string DataNotExist2 = "BACKEND.APP_MESSAGE.DATA_NOT_EXIST2";

            public const string NotAuthorize = "BACKEND.APP_MESSAGE.NOT_AUTHORIZE";

            public const string Concurrency = "BACKEND.APP_MESSAGE.NOT_CONCURRENCY";

            public const string FoundMoreThanOneWithValue = "BACKEND.APP_MESSAGE.FOUND_MORE_THAN_ONE_WITH_VALUE";

            public const string FoundXRecordWithValue = "BACKEND.APP_MESSAGE.FOUND_X_RECORD_WITH_VALUE";

            public const string CannotUpdateCustomerOwner = "BACKEND.APP_MESSAGE.NOT_ALLOW_UPDATE_CUSTOMER_OWNER";

            public const string OnlyOneOfValueCanExist = "BACKEND.APP_MESSAGE.ONLY_ONE_OF_VALUE_CAN_EXIST";

            public const string ManyValueExist = "BACKEND.APP_MESSAGE.MANY_VALUES_EXIST";

            public const string ManyValueExist2 = "BACKEND.APP_MESSAGE.MANY_VALUES_EXIST2";

            public const string CurrentLoginUserInvalid = "BACKEND.APP_MESSAGE.CURRENT_USER_ID_ISNULL";

            public const string StatusNotAllowToBeUpdated = "BACKEND.APP_MESSAGE.STATUS_NOT_ALLOW_TO_BE_UPDATED";

            public const string FailToX = "Key {0}: User {1} failed to {2} {3}";

            public const string RecordBeingUsed = "BACKEND.APP_MESSAGE.RECORD_BEING_USED";

            public const string ValuesDoNotMatch = "BACKEND.APP_MESSAGE.VALUES_DO_NOT_MATCH";

            public const string TotalArGreaterThanOrderTotalAmount = "BACKEND.APP_MESSAGE.TOTAL_AR_GREATER_THAN_ORDER_TOTALAMOUNT";

            public const string BackendOneSignalWithdraw = "BACKEND.ONESIGNAL.MESSAGE.WITHDRAW";

            public const string BackendOneSignalSubmit = "BACKEND.ONESIGNAL.MESSAGE.SUBMIT";

            public const string BackendOneSignalApprove = "BACKEND.ONESIGNAL.MESSAGE.APPROVE";

            public const string BackendOneSignalDeny = "BACKEND.ONESIGNAL.MESSAGE.DENY";

            public const string BackendOneSignalError = "BACKEND.ONESIGNAL.MESSAGE.ERROR";

            public const string BackendOneSignalStatusChange = "BACKEND.ONESIGNAL.MESSAGE.STATUS_CHANGE";

            public const string BackendOneSignalTitle = "BACKEND.ONESIGNAL.MESSAGE.TITLE";

            public const string BackendOneSignalCancel = "BACKEND.ONESIGNAL.MESSAGE.CANCEL";

            public const string BackendOneSignalPending = "BACKEND.ONESIGNAL.MESSAGE.PENDING";

            public const string BackendOneSignalDeleted = "BACKEND.ONESIGNAL.MESSAGE.DELETED";

            public const string BackendOneSignalUpdated = "BACKEND.ONESIGNAL.MESSAGE.UPDATED";

            public const string BackendOneSignalCreated = "BACKEND.ONESIGNAL.MESSAGE.CREATED";

            public const string BackendOneSignalSysValTitle = "BACKEND.ONESIGNAL.MESSAGE.SYS_VAL_TITLE";

            public const string BackendOneSignalOrderSysStatusValid = "BACKEND.ONESIGNAL.MESSAGE.ORDER_SYS_STATUS_VALID";

            public const string BackendOneSignalOrderSysStatusInvalid = "BACKEND.ONESIGNAL.MESSAGE.ORDER_SYS_STATUS_INVALID";

            public const string JobTitle = "BACKEND.MESSAGE.JOB.TITLE";

            public const string JobTitleWithObj = "BACKEND.MESSAGE.JOB.TITLE_WITH_OBJ";

            public const string JobBody = "BACKEND.MESSAGE.JOB.BODY";

            public const string JobSuccessMessage = "BACKEND.MESSAGE.JOB.SUCCESS";

            public const string JobFailMessage = "BACKEND.MESSAGE.JOB.FAIL";

            public const string JobPostInventoryDailyPeriodTitle = "BACKEND.MESSAGE.JOB.POST_INVENTORY_DAILY_PERIOD_TITLE";

            public const string ValueNotCompatible = "BACKEND.APP_MESSAGE.VALUE_NOT_COMPATIBLE";

            public const string InvalidPhoneNumber = "BACKEND.APP_MESSAGE.INVALID_PHONE_NUMBER";

            public const string ValuesAllNullOrHaveValue = "BACKEND.APP_MESSAGE.PROPERTIES_MUST_BE_ALL_NULL_OR_HAVE_VALUE";

            public const string OneOfTheseMustHaveValue = "BACKEND.APP_MESSAGE.ONE_OF_THESE_MUST_HAVE_VALUE";

            public const string MustUnique2 = "BACKEND.APP_MESSAGE.MUST_UNIQUE2";

            public const string ConfirmShipmentWhileCreatingDelivery = "BACKEND.APP_MESSAGE.CONFIRM_DELIVERY_DURING_CREATE_DELIVERY";

            public const string NotAssignedToAValidDelivery = "BACKEND.APP_MESSAGE.NOT_ASSIGN_TO_A_VALID_DELIVERY";

            public const string ShipmentNotReadyForCompleted = "BACKEND.APP_MESSAGE.SHIPMENT_NOT_READY_FOR_COMPLETED";

            public const string DeliveryCannotCompleted = "BACKEND.APP_MESSAGE.DELIVERY_CANNOT_COMPLETED";

            public const string RushOrderNotReadyForCompleted = "BACKEND.APP_MESSAGE.RUSH_ORDER_NOT_READY_FOR_COMPLETED";

            public const string DeliveryNotInStatus = "BACKEND.APP_MESSAGE.DELIVERY_NOT_IN_STATUS";

            public const string ArDocNotInStatus = "BACKEND.APP_MESSAGE.AR_DOC_NOT_IN_STATUS";

            public const string BackendOneSignalCreateDelivery = "BACKEND.ONESIGNAL.MESSAGE.CREATE_DELIVERY";

            public const string CreateOrderForInvalidStatusDelivery = "BACKEND.APP_MESSAGE.CREATE_ORDER_FOR_INVALID_STATUS_DELIVERY";

            public const string ExistOrderItemNotHaveWarehouse = "BACKEND.EXIST_ORDER_NOT_HAVE_WAREHOUSE";

            public const string ExistDeliveryExpressNotComplete = "BACKEND.EXIST_DELIVERY_EXPRESS_COMPLETE";

            public const string InventoryDocumentPostItemNoneValid = "BACKEND.INVENTORY_DOCUMENT.POST.ITEM_NONE_VALID";

            public const string ExistShipmentWithOrder = "EXIST_SHIPMENT_WITH_ORDER";

            public const string ExistWarehouseInBusinessUnit = "EXIST_WAREHOUSE_IN_BUSINESSUNIT";

            public const string ReferenceNotExist = "BACKEND.APP_MESSAGE.REFERENCE_NOT_EXIST";

            public const string NoMatchingRoleForProductGroups = "BACKEND.APP_MESSAGE.NO_MATCHING_ROLE_FOR_PRODUCT_GROUPS";

            public const string InventoryNotExistRootWarehouseOfBusinessUnit = "BACKEND.WAREHOUSE.NOT_EXIST_ROOT_WAREHOUSE_OF_BUSINESS_UNIT";

            public const string NotExistProductWarehouseForecast = "BACKEND.PRODUCT_WAREHOUSE_FORECAST.NOT_EXIST_WITH_ORDER_ID_PRODUCT_ID";

            public const string CreatedBySystem = "BACKEND.APP_MESSAGE.CREATED_BY_SYSTEM";

            public const string IllegalStatusOrder = "BACKEND.APP_MESSAGE.ILLEGAL_STATUS_ORDER";

            public const string IllegalOrderType = "BACKEND.APP_MESSAGE.ILLEGAL_ORDER_TYPE";

            public const string MissingInfo = "BACKEND.APP_MESSAGE.MISSING_INFO";

            public const string ApproveOrderCustomerCodeNotNull = "BACKEND.APPROVE_ORDER_CUSTOMER_CODE_NOT_NULL";

            public const string ExistInvoiceWithOrder = "EXIST_INVOICE_WITH_ORDER";

            public const string StatusAllowToBeUpdated = "BACKEND.APP_MESSAGE.STATUS_ALLOW_TO_BE_UPDATED";

            public const string UnAllowToBeReConfirmed = "BACKEND.APP_MESSAGE.UN_ALLOW_TO_BE_RE_CONFIRMED";

            public const string NotCreateOrderFromAlreadyUsedOneShotPriceRequest = "BACKEND.APP_MESSAGE.NOT_ALLOW_CREATE_ORDER_FROM_ALREADY_USED_ONE_SHOT_PRICE_REQUEST";

            public const string NotInStatus = "BACKEND.APP_MESSAGE.NOT_IN_STATUS";

            public const string CannotRemoveCarriers = "BACKEND.APP_MESSAGE.CANNOT_REMOVE_CARRIERS";

            public const string NotExistActiveDeliveryOfUser = "BACKEND.APP_MESSAGE.NOT_EXIST_ACTIVE_DELIVERY_OF_USER";

            public const string CannotDeleteDeliveryIfExistShipmentLoad = "BACKEND.APP_MESSAGE.NOT_DELETE_DELIVERY_EXIST_SHIPMENT_LOAD";

            public const string CustomerInPriceRequestDoNotMatchingToSelected = "BACKEND.APP_MESSAGE.CUSTOMER_IN_PRICE_REQUEST_NOT_MATCHING_TO_SELECTED";

            public const string ExistShipmentsNeedToCompleteToCompleteDelivery = "BACKEND.APP_MESSAGE.EXIST_SHIPMENTS_NEED_TO_COMPLETE_TO_COMPLETE_DELIVERY";

            public const string ExistRushOrdersNeedToDeliverToCompleteDelivery = "BACKEND.APP_MESSAGE.EXIST_ORDERS_NEED_TO_DELIVER_TO_COMPLETE_DELIVERY";

            public const string InventoryDocumentDescriptionOnDeliveryShipmentLoaded = "BACKEND.SHIPMENT_DELIVERY_LOADED.INVENTORY_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnPickupShipmentDelivered = "BACKEND.SHIPMENT_PICKUP_DELIVERED.INVENTORY_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnSelfShipmentDelivered = "BACKEND.SHIPMENT_SELF_DELIVERED.INVENTORY_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnPickupOrderReturnShipmentDelivered = "BACKEND.SHIPMENT_PICKUP_ORDER_RETURN_DELIVERED.INVENTORY_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnDeliveryShipmentDelivered = "BACKEND.SHIPMENT_DELIVERY_DELIVERED.INVENTORY_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnDeliveryShipmentConfDelivered = "BACKEND.SHIPMENT_DELIVERY_CONF_DELIVERED.INVENTORY_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnDeliveryShipmentPickUpAdjustConfDelivered = "BACKEND.SHIPMENT_DELIVERY_ADJ_CONF_DELIVERED.INVENTORY_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnDeliveryShipmentPickUpTransConfDelivered = "BACKEND.SHIPMENT_DELIVERY_TRANS_CONF_DELIVERED.INVENTORY_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnDeliveryShipmentCancel = "BACKEND.SHIPMENT_DELIVERY_CANCEL.INVENTORY_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnDeliveryShipmentOrderReturnCancel = "BACKEND.SHIPMENT_DELIVERY_ORDER_RETURN_CANCEL.INVENTORY_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnPickupShipmentLoaded = "BACKEND.SHIPMENT_PICKUP_LOADED.INVENTORY_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnPickupShipmentCancel = "BACKEND.SHIPMENT_PICKUP_CANCEL.INVENTORY_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnShipmentDeliveryTransferred = "BACKEND.SHIPMENT_TRANSFER_DELIVERY.INVENTORY_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnStockTakeDocumentPosted = "BACKEND.STOCK_TAKE_POSTED.INVENTORY_DOCUMENT_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnStockTakeDocumentPostedIncrease = "BACKEND.STOCK_TAKE_POSTED.INVENTORY_DOCUMENT_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnStockTakeDocumentPostedDecrease = "BACKEND.STOCK_TAKE_POSTED.INVENTORY_DOCUMENT_DESCRIPTION";

            public const string InventoryDocumentDescriptionOnDeliveryTransferToMainWarehouse = "BACKEND.DELIVERY_TRANSFER_TO_MAIN_WAREHOUSE.INVENTORY_DESCRIPTION";

            public const string InvDocDescOnShipmentInternalTransferDelivered = "BACKEND.SHIPMENT_INTERNAL_TRANSFER_DELIVERED.INVENTORY_DESCRIPTION";

            public const string InvDocDescOnShipmentECommerceDelivered = "BACKEND.SHIPMENT_ECOMMERCE_DELIVERED.INVENTORY_DESCRIPTION";

            public const string InvDocDescOnShipmentConsignmentDelivered = "BACKEND.SHIPMENT_CONSIGNMENT_DELIVERED.INVENTORY_DESCRIPTION";

            public const string ExistUserInDelivery = "BACKEND.APP_MESSAGE.EXIST_USER_IN_OTHER_DELIVERY";

            public const string OrderOutOfStock = "BACKEND.APP_MESSAGE.ORDER_OUT_OF_STOCK";

            public const string VehicleNotEHaveWarehouse = "BACKEND.APP_MESSAGE.VEHICLE_NOT_HAVE_WAREHOUSE";

            public const string VehicleWarehouseNotActive = "BACKEND.APP_MESSAGE.VEHICLE_WAREHOUSE_NOT_ACTIVE";

            public const string ShipmentItemNotHaveWarehouse = "BACKEND.APP_MESSAGE.SHIPMENT_ITEM_NOT_HAVE_WAREHOUSE";

            public const string ShipmentContainShipmentItemWithoutWarehouseId = "BACKEND.APP_MESSAGE.SHIPMENT_CONTAIN_SHIPMENT_WITHOUT_WAREHOUSE_ID";

            public const string DeliveryReceiptInventoryForSaleUnits = "BACKEND.FOR_SALE_UNITS_TRANSFER_DELIVERY.INVENTORY_DESCRIPTION";

            public const string DeliveryReceiptInventoryShipmentsNotFullDelivered = "BACKEND.SHIPMENT_NOT_FULL_DELIVERED.INVENTORY_DESCRIPTION";

            public const string DeliveryReceiptInventoryShipmentsPickup = "BACKEND.SHIPMENT_PICK_UP.INVENTORY_DESCRIPTION";

            public const string DeliveryIssueInventoryShipmentsPickup = "BACKEND.SHIPMENT_PICK_UP.ISSUE_INVENTORY_DESCRIPTION";

            public const string CannotDeliverPickupShipmentWithOtherTypes = "BACKEND.DELIVERY.CAN_NOT_DELIVER_PICKUP_SHIPMENT_WITH_OTHER_TYPES";

            public const string CannotDeliverReceiveShipmentWithOtherTypes = "BACKEND.DELIVERY.CAN_NOT_DELIVER_RECEIVE_SHIPMENT_WITH_OTHER_TYPES";

            public const string DeliverShipmentAtCustomerVehicleMustBeInSystem = "BACKEND.DELIVERY.DELIVERY_SHIPMENT_VEHICLE_MUST_BE_IN_SYSTEM";

            public const string DeliveryCanNotConfirmWhileExistUnCompleteDelivery = "BACKEND.DELIVERY.CAN_NOT_LOAD_WHILE_EXIST_UN_COMPLETE_DELIVERY";

            public const string DeliveryCanNotConfirmWhileExistUnCompleteDeliveryByObj = "BACKEND.DELIVERY.CAN_NOT_LOAD_WHILE_EXIST_UN_COMPLETE_DELIVERY_BY_OBJ";

            public const string DeliveryCanTransferInventoryInStatuses = "BACKEND.DELIVERY.CAN_TRANSFER_INVENTORY_IN_STATUSES";

            public const string DeliveryCantTransferInventoryInThePast = "BACKEND.DELIVERY.CANT_TRANSFER_INVENTORY_IN_THE_PAST";

            public const string DeliveryAlreadyTransferred = "BACKEND.DELIVERY.CANT_TRANSFER_INVENTORY_TRANSFERRED";

            public const string DeliveryExtAttrDescription = "BACKEND.DELIVERY.DELIVERY_EXT_ATTR_DESCRIPTION";

            public const string ProductNotExistInPricing = "BACKEND.PRODUCT_NOT_EXIST_IN_PRICING";

            public const string DataChangeNotFoundAnyChanges = "BACKEND.DATA_CHANGE.NOT_FOUND_ANY_CHANGES";

            public const string DataChangeAutoCreateForCustomerCheckin = "BACKEND.DATA_CHANGE.CUSTOMER_CHECKIN_AUTO_CREATE";

            public const string CannotApprovePriceRequestForUnverifiedCustomer = "BACKEND.VALIDATION.MESSAGE.PRICE_REQUEST_CAN_NOT_APPROVE_FOR_UNVERIFIED_CUSTOMER";

            public const string CannotApproveOrderForUnverifiedCustomer = "BACKEND.VALIDATION.MESSAGE.ORDER_CAN_NOT_APPROVE_FOR_UNVERIFIED_CUSTOMER";

            public const string NewDataChangePending = "COMMON.MESSAGE.NEW_DATA_CHANGE_PENDING";

            public const string MessageNewCustomerSubmitted = "COMMON.MESSAGE.NEW_CUSTOMER_SUBMITTED";

            public const string MessageNewCustomerSubmittedApproved = "COMMON.MESSAGE.NEW_CUSTOMER_APPROVED";

            public const string MessageUpdateCustomerSubmitted = "COMMON.MESSAGE.UPDATE_CUSTOMER_SUBMITTED";

            public const string MessageUpdateCustomerSubmittedApproved = "COMMON.MESSAGE.UPDATE_CUSTOMER_APPROVED";

            public const string MessageNewCustomerAddressSubmitted = "COMMON.MESSAGE.NEW_CUSTOMER_ADDRESS_SUBMITTED";

            public const string MessageNewCustomerAddressSubmittedApproved = "COMMON.MESSAGE.NEW_CUSTOMER_ADDRESS_APPROVED";

            public const string MessageUpdateCustomerAddressSubmitted = "COMMON.MESSAGE.UPDATE_CUSTOMER_ADDRESS_SUBMITTED";

            public const string MessageUpdateCustomerAddressSubmittedApproved = "COMMON.MESSAGE.UPDATE_CUSTOMER_ADDRESS_APPROVED";

            public const string MessageDeleteCustomerAddressSubmitted = "COMMON.MESSAGE.DELETE_CUSTOMER_ADDRESS_SUBMITTED";

            public const string MessageDeleteCustomerAddressSubmittedApproved = "COMMON.MESSAGE.DELETE_CUSTOMER_ADDRESS_APPROVED";

            public const string MessageAutoCancelPendingOrderOnInvalidCustomer = "COMMON.MESSAGE.AUTO_CANCEL_PENDING_ORDER_ON_INVALID_CUSTOMER";

            public const string MessageAutoCancelPendingPriceRequestOnInvalidCustomer = "COMMON.MESSAGE.AUTO_CANCEL_PENDING_PRICE_REQUEST_ON_INVALID_CUSTOMER";

            public const string InventoryDailyPeriodAutoPostDescription = "COMMON.MESSAGE.INVENTORY_DAILY_PERIOD_AUTO_POST_DESCRIPTION";

            public const string InventoryMonthlyPeriodAutoPostDescription = "COMMON.MESSAGE.INVENTORY_MONTHLY_PERIOD_AUTO_POST_DESCRIPTION";

            public const string GatewayTimeout = "BACKEND.APP_MESSAGE.GATEWAY_TIMEOUT";

            public const string SettingNotEnable = "BACKEND.APP_MESSAGE.SETTING_NOT_ENABLE";

            public const string BackendMessageDataChangeTitle = "BACKEND.MESSAGE_DATA_CHANGE_TITLE";

            public const string BackendMessageDataChangeStatusChange = "BACKEND.MESSAGE_DATA_CHANGE_CONTENT";

            public const string BackendOneSignalNewCustomerTitle = "BACKEND.ONESIGNAL.MESSAGE.NEW_CUSTOMER_TITLE";

            public const string BackendOneSignalUpdateCustomerTitle = "BACKEND.ONESIGNAL.MESSAGE.UPDATE_CUSTOMER_TITLE";

            public const string BackendOneSignalDeleteCustomerTitle = "BACKEND.ONESIGNAL.MESSAGE.DELETE_CUSTOMER_TITLE";

            public const string BackendOneSignalNewCustomerMessage = "BACKEND.ONESIGNAL.MESSAGE.NEW_CUSTOMER_MESSAGE";

            public const string BackendOneSignalUpdateCustomerMessage = "BACKEND.ONESIGNAL.MESSAGE.UPDATE_CUSTOMER_MESSAGE";

            public const string BackendOneSignalDeleteCustomerMessage = "BACKEND.ONESIGNAL.MESSAGE.DELETE_CUSTOMER_MESSAGE";

            public const string BackendOneSignalNewCustomerAddressMessage = "BACKEND.ONESIGNAL.MESSAGE.NEW_CUSTOMER_ADDRESS_MESSAGE";

            public const string BackendOneSignalUpdateCustomerAddressMessage = "BACKEND.ONESIGNAL.MESSAGE.UPDATE_CUSTOMER_ADDRESS_MESSAGE";

            public const string BackendOneSignalDeleteCustomerAddressMessage = "BACKEND.ONESIGNAL.MESSAGE.DELETE_CUSTOMER_ADDRESS_MESSAGE";

            public const string BackendDataChangeAutoApproveBy = "BACKEND.DATA_CHANGE.AUTO_APPROVE_BY";

            public const string ExistAutoNumber = "BACKEND.VALIDATION.MESSAGE.EXIST_AUTO_NUMBER";

            public const string AutoNumberV2HasLinking = "BACKEND.VALIDATION.MESSAGE.EXIST_AUTO_NUMBER_V2_HAS_LINKING";

            public const string ExistAutoNumberV2 = "BACKEND.VALIDATION.MESSAGE.EXIST_AUTO_NUMBER_V2";

            public const string BackendOneSignalUserEventFormatMessage = "BACKEND.ONESIGNAL.MESSAGE.USER_EVENT.FORMAT_MESSAGE";

            public const string BackendOneSignalUserEventAttendanceClockIn = "BACKEND.ONESIGNAL.MESSAGE.USER_EVENT.ATTENDANCE_CLOCK_IN";

            public const string BackendOneSignalUserEventAttendanceClockOut = "BACKEND.ONESIGNAL.MESSAGE.USER_EVENT.ATTENDANCE_CLOCK_OUT";

            public const string BackendOneSignalNotification = "BACKEND.ONESIGNAL.NOTICATION";

            public const string BackendOneSignalDeliveryEventFormatMessage = "BACKEND.ONESIGNAL.MESSAGE.DELIVERY_EVENT.FORMAT_MESSAGE";

            public const string BackendOneSignalDeliveryEventInTransit = "BACKEND.ONESIGNAL.MESSAGE.DELIVERY_EVENT.IN_TRANSIT";

            public const string BackendOneSignalDeliveryEventComplete = "BACKEND.ONESIGNAL.MESSAGE.DELIVERY_EVENT.COMPLETE";

            public const string BackendOneSignalDeliveryEventView = "BACKEND.ONESIGNAL.MESSAGE.DELIVERY_EVENT.VIEW";

            public const string PricingItemDeletedMessage = "BACKEND.APP_MESSAGE.DELETED_PRICING_ITEM_SUCCESS";

            public const string CannotDeleteStockoutFromFbs = "BACKEND.APP_MESSAGE.CAN_NOT_DELETE_STOCKOUT_FROM_FBS";

            public const string CannotCreateStockoutFromFbs = "BACKEND.APP_MESSAGE.CAN_NOT_CREATE_STOCKOUT_FROM_FBS";

            public const string CannotAddShipmentToDeliveryWithOtherTypes = "BACKEND.SHIPMENT.CAN_NOT_ADD_SHIPMENT_TO_DELIVERY_WITH_OTHER_TYPES";

            public const string AutoNumberNotConfig = "BACKEND.VALIDATION.MESSAGE.AUTO_NUMBER_NOT_CONFIG";

            public const string ShipmentDiffBu = "BACKEND.MESSAGE_SHIPMENT_DIFF_BU";

            public const string DataDoesNotMatch = "BACKEND.MESSAGE_DATA_DOES_NOT_MATCH";

            public const string ExceededMaximumApprovalAttempts = "BACKEND.APP_MESSAGE.TICKET_EXCEEDED_MAXIMUM_APPROVAL_ATTEMPTS";

            public const string AutoNumberTransactionMapNotConfig = "BACKEND.VALIDATION.MESSAGE.AUTO_NUMBER_TRANSACTION_MAP_NOT_CONFIG";

            public const string CustomerCreatedMessage = "CUSTOMER.MESSAGE.CREATED";

            public const string CustomerCreatedDraftStatusMessage = "CUSTOMER.MESSAGE.CREATED_DRAFT_STATUS";

            public const string CustomerCreatedSubmittedStatusMessage = "CUSTOMER.MESSAGE.CREATED_SUBMITTED_STATUS";

            public const string CustomerCreatedVerifiedStatusMessage = "CUSTOMER.MESSAGE.CREATED_VERIFIED_STATUS";

            public const string CustomerUpdatedMessage = "CUSTOMER.MESSAGE.UPDATED";

            public const string CustomerUpdatingSubmittedChangeMessage = "CUSTOMER.MESSAGE.UPDATING_SUBMITTED_DATA_CHANGE";

            public const string CustomerUpdatedApprovedChangeMessage = "CUSTOMER.MESSAGE.UPDATED_APPROVED_DATA_CHANGE";

            public const string UpdateSuccessLatLongOfCustomer = "BACKEND.CUSTOMER_ADDRESS.UPDATE_SUCCESS_LAT_LONG_OF_CUSTOMER";

            public const string CustomerMainAddressIdEmpty = "BACKEND.APP_MESSAGE.CUSTOMER_MAIN_ADDRESS_EMPTY";

            public const string CustomerPrimaryContactNameEmpty = "BACKEND.APP_MESSAGE.CUSTOMER_PRIMARY_CONTACT_NAME_EMPTY";

            public const string CustomerMainAddressIdInvalid = "BACKEND.APP_MESSAGE.CUSTOMER_MAIN_ADDRESS_INVALID";

            public const string CustomerPrimaryContactPhoneEmpty = "BACKEND.APP_MESSAGE.CUSTOMER_PRIMARY_CONTACT_PHONE_EMPTY";

            public const string CustomerAddressCreatedMessage = "CUSTOMER_ADDRESS.MESSAGE.CREATED";

            public const string CustomerAddressCreatingSubmittedStatusMessage = "CUSTOMER_ADDRESS.MESSAGE.CREATING_SUBMITTED_DATA_CHANGE";

            public const string CustomerAddressCreatedVerifiedStatusMessage = "CUSTOMER_ADDRESS.MESSAGE.CREATED_APPROVED_DATA_CHANGE";

            public const string CustomerAddressUpdatedMessage = "CUSTOMER_ADDRESS.MESSAGE.UPDATED";

            public const string CustomerAddressUpdatingSubmittedChangeMessage = "CUSTOMER_ADDRESS.MESSAGE.UPDATING_SUBMITTED_DATA_CHANGE";

            public const string CustomerAddressUpdatedApprovedChangeMessage = "CUSTOMER_ADDRESS.MESSAGE.UPDATED_APPROVED_DATA_CHANGE";

            public const string CustomerAddressDeletedMessage = "CUSTOMER_ADDRESS.MESSAGE.DELETED";

            public const string CustomerAddressDeletingSubmittedChangeMessage = "CUSTOMER_ADDRESS.MESSAGE.DELETING_SUBMITTED_DATA_CHANGE";

            public const string CustomerAddressDeletedApprovedChangeMessage = "CUSTOMER_ADDRESS.MESSAGE.DELETED_APPROVED_DATA_CHANGE";

            public const string CannotFindMainAddressOfCustomer = "BACKEND.CUSTOMER_ADDRESS.CANNOT_FIND_MAIN_ADDRESS_OF_CUSTOMER";

            public const string CannotFindLatLongOfMainAddress = "BACKEND.CUSTOMER_ADDRESS.CANNOT_FIND_LAT_LONG_OF_MAIN_ADDRESS";

            public const string CantDeleteMainAddress = "BACKEND.APP_MESSAGE.CANT_DELETE_MAIN_ADDRESS";

            public const string CantUpdateMainAddress = "BACKEND.APP_MESSAGE.CANT_UPDATE_MAIN_ADDRESS";

            public const string NumberDeleteSuccess = "BACKEND.MESSAGE_NUMBER_DELETE_SUCCESS";

            public const string PriceRequestDuplicatedRequestMessage = "PRICE_REQUEST.MESSAGE_DUPLICATED_REQUEST";

            public const string PriceRequestItemWithoutPrices = "PRICE_REQUEST_ITEM.MESSAGE_WITHOUT_PRICES";

            public const string PriceRequestExistItemWithoutPrices = "PRICE_REQUEST.MESSAGE_EXISTS_ITEM_WITHOUT_PRICES";

            public const string PriceRequestItemNegativePrice = "PRICE_REQUEST_ITEM.MESSAGE_NEGATIVE_PRICE";

            public const string PriceRequestExistItemNegativePrice = "PRICE_REQUEST.MESSAGE_EXISTS_ITEM_NEGATIVE_PRICES";

            public const string PriceRequestOverAllowableExpiryDate = "PRICE_REQUEST.MESSAGE_OVER_ALLOWABLE_EXPIRY_DATE";

            public const string PriceRequestPromotionProductMustHaveSellProduct = "PRICE_REQUEST.MESSAGE_PROMOTION_PRODUCT_MUST_HAVE_SELL_PRODUCT";

            public const string PriceRequestDuplicateSortInGroup = "PRICE_REQUEST.MESSAGE_DUPLICATE_SORT_IN_GROUP";

            public const string StatusOrderReturnInvalidWhenCreateOrderReplace = "ORDER.STATUS_ORDER_RETURN_INVALID_WHEN_CREATE_ORDER_REPLACE";

            public const string NotFoundOrderWithNumber = "BACKEND.MESSAGE_NOT_FOUND_ORDER_WITH_NUMBER";

            public const string NotFoundShipmentWithNumber = "BACKEND.MESSAGE_NOT_FOUND_SHIPMENT_WITH_NUMBER";

            public const string OrderItemWithoutPrices = "ORDER_ITEM.MESSAGE_WITHOUT_PRICES";

            public const string OrderItemUnderBlockPrice = "ORDER_ITEM.MESSAGE_UNDER_BLOCK_PRICES";

            public const string OrderItemUnderSalePrice = "ORDER_ITEM.MESSAGE_UNDER_SALE_PRICE";

            public const string OrderItemNegativePrice = "ORDER_ITEM.MESSAGE_NEGATIVE_PRICE";

            public const string OrderItemWithoutBlockPrice = "ORDER_ITEM.MESSAGE_WITHOUT_BLOCK_PRICE";

            public const string OrderItemWithoutPromoPrice = "ORDER_ITEM.MESSAGE_WITHOUT_PROMO_PRICE";

            public const string OrderItemNegativeBlockPrice = "ORDER_ITEM.MESSAGE_NEGATIVE_BLOCK_PRICE";

            public const string OrderItemNegativePromoPrice = "ORDER_ITEM.MESSAGE_NEGATIVE_PROMO_PRICE";

            public const string OrderItemNegativeInternalPrice = "ORDER_ITEM.MESSAGE_NEGATIVE_INTERNAL_PRICE";

            public const string OrderExistItemWithoutPrices = "ORDER.MESSAGE_EXISTS_ITEM_WITHOUT_PRICES";

            public const string OrderWithDeliveredUnitsGreaterThanUnits = "ORDER.MESSAGE_DELIVERY_UNITS_GREATER_THAN_UNITS";

            public const string OrderContainsItemWithoutPrices = "ORDER.MESSAGE_ORDER_CONTAINS_ITEM_WITHOUT_PRICES";

            public const string OrderExistItemWithoutBlockPrice = "ORDER.MESSAGE_EXISTS_ITEM_WITHOUT_BLOCK_PRICE";

            public const string OrderExistItemWithoutPromoPrice = "ORDER.MESSAGE_EXISTS_ITEM_WITHOUT_PROMO_PRICE";

            public const string OrderExistItemUnderBlockPrice = "ORDER.MESSAGE_EXISTS_ITEM_UNDER_BLOCK_PRICES";

            public const string OrderExistItemNegativePrice = "ORDER.MESSAGE_EXISTS_ITEM_NEGATIVE_PRICES";

            public const string OrderGroupAndLine = "ORDER.MESSAGE_GROUP_AND_LINES";

            public const string CanNotGetProductPrices = "ORDER.MESSAGE_CAN_NOT_GET_PRODUCT_PRICES";

            public const string CanNotGetPromotions = "ORDER.MESSAGE_CAN_NOT_GET_PROMOTIONS";

            public const string OrderNotFoundLinkedPriceRequestItem = "ORDER.MESSAGE_NOT_FOUND_LINKED_PRICE_REQUEST_ITEM";

            public const string OrderRecommendSuitableRepriceRequests = "ORDER.MESSAGE_RECOMMEND_SUITABLE_PRICE_REQUESTS";

            public const string OrderItemUnderRequestPrice = "ORDER_ITEM.MESSAGE_UNDER_PRICE_REQUEST";

            public const string OrderSysValStatusValid = "ORDER.MESSAGE_SYS_VAL_STATUS_VALID";

            public const string OrderWasNotValidatedBySystem = "ORDER.MESSAGE_ORDER_WAS_NOT_VALIDATED_BY_SYSTEM";

            public const string OrderNotEnoughConditionToApplyPromotion = "ORDER.MESSAGE_NOT_ENOUGH_CONDITION_TO_APPLY_PROMOTION";

            public const string PriceRequestInvalidStatus = "ORDER.MESSAGE_PRICE_REQUEST_INVALID_STATUS";

            public const string PriceRequestNotApproved = "ORDER.MESSAGE_PRICE_REQUEST_NOT_APPROVED";

            public const string PriceRequestExpired = "ORDER.MESSAGE_PRICE_REQUEST_EXPIRED";

            public const string PriceRequestUnApplicable = "ORDER.MESSAGE_PRICE_REQUEST_UN_APPLICABLE";

            public const string PriceRequestNotFound = "ORDER.MESSAGE_PRICE_REQUEST_NOT_FOUND";

            public const string PriceRequestNotMatchProduct = "ORDER.MESSAGE_PRICE_REQUEST_NOT_MATCH_PRODUCT";

            public const string PriceRequestOneShotCanNotApplyMultiple = "ORDER.MESSAGE_PRICE_REQUEST_ONE_SHOT_CAN_NOT_APPLY_MULTIPLE";

            public const string RushOrderCannotCreateForCustomerWithoutCode = "BACKEND.RUSH_ORDER_CANNOT_CREATE_FOR_CUSTOMER_WITHOUT_CODE";

            public const string OrderReturnCannotChangeStatusWhenExistOrderReplace = "BACKEND.ORDER_RETURN.CANNOT_CHANGE_STATUS_WHEN_EXIST_ORDER_REPLACE";

            public const string HoldOrderCantNotTransferInventory = "BACKEND.HOLD_ORDER.CANT_NOT_TRANSFER_INVENTORY";

            public const string CustomerBusinessUnitDoNotFindBusinessUnitOfCustomer = "BACKEND.CUSTOMER_BUSINESS_UNIT.DO_NOT_FIND_BUSINESS_UNIT_OF_CUSTOMER";

            public const string UserBusinessUnitDoNotFindBusinessUnitOfUser = "BACKEND.USER_BUSINESS_UNIT.DO_NOT_FIND_BUSINESS_UNIT_OF_USER";

            public const string BusinessUnitMustHaveDefaultPricing = "BACKEND.APP_MESSAGE.BUSINESS_UNIT_MUST_HAVE_DEFAULT_PRICING";

            public const string BusinessUnitBeingUsed = "BACKEND.VALIDATION.MESSAGE.BUSINESS_UNIT_BEING_USED";

            public const string ArTotalAppliedAmtGreaterThanApplyingDocBalance = "BACKEND.AR_TOTAL_APPLIED_AMT_GREATER_THAN_APPLYING_DOC_BALANCE";

            public const string ArAppliedAmtGreaterThanAppliedDocBalance = "BACKEND.AR_APPLIED_AMT_GREATER_THAN_APPLIED_DOC_BALANCE";

            public const string ArApplyingDocBalancedCanNotApplyToOther = "BACKEND.AR_APPLYING_DOC_BALANCED_CAN_NOT_APPLY_TO_OTHER";

            public const string ArApplyInvalidDocType = "BACKEND.AR_APPLY_INVALID_DOC_TYPE";

            public const string ArObjectNotBelongToCustomer = "BACKEND.AR_OBJECT_NOT_BELONG_TO_CUSTOMER";

            public const string ArDocAndOriginalDocTypeNotMatch = "BACKEND.AR_DOC_AND_ORIGINAL_DOC_TYPE_NOT_MATCH";

            public const string ArSyncBusinessUnitAffectedRow = "BACKEND.AR_SYNC_BUSINESS_UNIT_AFFECTED_ROW";

            public const string ArSyncBusinessUnitNotFoundDiff = "BACKEND.AR_SYNC_BUSINESS_UNIT_NOT_FOUND_DIFF";

            public const string ArBlockOrderDraft = "BACKEND.AR_BLOCK_ORDER_DRAFT";

            public const string ArBlockOrderSubmit = "BACKEND.AR_BLOCK_ORDER_SUBMIT";

            public const string ArBlockOrderSold = "BACKEND.AR_BLOCK_ORDER_SOLD";

            public const string ArSalesOrderBlock = "BACKEND.AR_SALES_BLOCK_ORDER_SOLD";

            public const string ArSalesNotSetupAr = "BACKEND.AR_SALES_NOT_SETUP_AR";

            public const string ArSalesCurrentBalanceGreaterThanLimit = "BACKEND.AR_SALES_CURRENT_BALANCE_GREATER_THAN_LIMIT";

            public const string ArBlockOrderSubmitLevelHighRisk = "BACKEND.AR_BLOCK_ORDER_SUBMIT_LEVEL_HIGH_RISK";

            public const string ArBlockOrderSubmitLevelLowRisk = "BACKEND.AR_BLOCK_ORDER_SUBMIT_LEVEL_LOW_RISK";

            public const string ArBlockRushOrderLevelLowRiskTotalAmtEqualReceiptAmt = "BACKEND.AR_BLOCK_RUSH_ORDER_LEVEL_LOW_RISK_TOTAL_AMT_EQUAL_RECEIPT_AMT";

            public const string ArBlockRushOrderLevelLowRiskTotalAmtLessThanReceiptAmt = "BACKEND.AR_BLOCK_RUSH_ORDER_LEVEL_LOW_RISK_TOTAL_AMT_LESS_THAN_RECEIPT_AMT";

            public const string ArBlockRushOrderLevelLowRiskTotalAmtGreaterThanReceiptAmt = "BACKEND.AR_BLOCK_RUSH_ORDER_LEVEL_LOW_RISK_TOTAL_AMT_GREATER_THAN_RECEIPT_AMT";

            public const string ArBlockRushOrderLevelHighRisk = "BACKEND.AR_BLOCK_RUSH_ORDER_LEVEL_HIGH_RISK";

            public const string ArBlockRushOrderLevelHighRiskTotalAmtEqualReceiptAmt = "BACKEND.AR_BLOCK_RUSH_ORDER_LEVEL_HIGH_RISK_TOTAL_AMT_EQUAL_RECEIPT_AMT";

            public const string ArBlockRushOrderLevelHighRiskTotalAmtLessThanReceiptAmt = "BACKEND.AR_BLOCK_RUSH_ORDER_LEVEL_HIGH_RISK_TOTAL_AMT_LESS_THAN_RECEIPT_AMT";

            public const string ArBlockRushOrderLevelHighRiskTotalAmtGreaterThanReceiptAmt = "BACKEND.AR_BLOCK_RUSH_ORDER_LEVEL_HIGH_RISK_TOTAL_AMT_GREATER_THAN_RECEIPT_AMT";

            public const string ArBlockRushOrderLevelLowRisk = "BACKEND.AR_BLOCK_RUSH_ORDER_LEVEL_LOW_RISK";

            public const string ArBlockRushOrderLevelNoDebt = "BACKEND.AR_BLOCK_RUSH_ORDER_LEVEL_NO_DEBT";

            public const string ArBlockRushOrderLevelNoDebtOverBucketAmt = "BACKEND.AR_BLOCK_RUSH_ORDER_LEVEL_NO_DEBT_OVER_BUCKET_AMT";

            public const string ArBlockRushOrderLevelNoDebtTotalAmtEqualReceiptAmt = "BACKEND.AR_BLOCK_RUSH_ORDER_LEVEL_NO_DEBT_TOTAL_AMT_EQUAL_RECEIPT_AMT";

            public const string ArBlockRushOrderLevelNoDebtTotalAmtLessThanReceiptAmt = "BACKEND.AR_BLOCK_RUSH_ORDER_LEVEL_NO_DEBT_TOTAL_AMT_LESS_THAN_RECEIPT_AMT";

            public const string ArBlockRushOrderLevelNoDebtTotalAmtGreaterThanReceiptAmt = "BACKEND.AR_BLOCK_RUSH_ORDER_LEVEL_NO_DEBT_TOTAL_AMT_GREATER_THAN_RECEIPT_AMT";

            public const string ArBlockRushOrderLevelPrestige = "BACKEND.AR_BLOCK_RUSH_ORDER_LEVEL_PRESTIGE";

            public const string ArBlockOrderSubmitLevelNoDebt = "BACKEND.AR_BLOCK_ORDER_SUBMIT_LEVEL_NO_DEBT";

            public const string ArBlockOrderApproveLevelNoDebt = "BACKEND.AR_BLOCK_ORDER_APPROVE_LEVEL_NO_DEBT";

            public const string ArCustomerBalanceCannotGetInfo = "BACKEND.AR_CUSTOMER_BALANCE_CAN_NOT_GET_INFO";

            public const string ArSalesBalanceCannotGetInfo = "BACKEND.AR_SALES_BALANCE_CAN_NOT_GET_INFO";

            public const string ArCustomerClassifyUpsertSuccess = "BACKEND.APP_MESSAGE.UPSERT_AR_CUSTOMER_CLASSIFY_SUCCESS";

            public const string ArDebtLimitBucketNotSequence = "AR.AR_DEBT_LIMIT.BUCKET_NOT_SEQUENCE";

            public const string ArDebtLimitBucketNotValid = "AR.AR_DEBT_LIMIT.BUCKET_NOT_VALID";

            public const string ArDebtLimitOverlapDate = "AR.AR_DEBT_LIMIT.OVERLAP_DATE";

            public const string ArDebtLimitExpired = "AR.AR_DEBT_LIMIT.EXPIRED";

            public const string NotAllowCreateArDebtLimitWithCustomerStatus = "BACKEND.NOT_ALLOW_CREATE_AR_DEBT_LIMIT_WITH_CUSTOMER_STATUS";

            public const string NotAllowCreateArDebtLimitWithUserInactive = "BACKEND.NOT_ALLOW_CREATE_AR_DEBT_LIMIT_WITH_USER_INACTIVE";

            public const string CannotCreateArDebtLimitWithCustomerCodeNull = "BACKEND.CANNOT_CREATE_AR_DEBT_LIMIT_WITH_CUSTOMER_CODE_NULL";

            public const string ArDocAlreadyAssociatedWithOtherReceipt = "BACKEND.VALIDATION.AR_DOC_ALREADY_ASSOCIATED_WITH_OTHER_RECEIPT";

            public const string CannotUpdateExpiredSalesTarget = "BACKEND.VALIDATION.CAN_NOT_UPDATE_EXPIRED_SALES_TARGET";

            public const string CannotChangeActiveTargetDate = "BACKEND.VALIDATION.CAN_NOT_CHANGE_ACTIVE_TARGET_DATE";

            public const string LocationOldOwnerDeviceHistory = "BACKEND.APP_MESSAGE.LOCATION_OLD_OWNER_DEVICE_HISTORY";

            public const string LocationOldIdentityDeviceHistory = "BACKEND.APP_MESSAGE.LOCATION_OLD_IDENTITY_DEVICE_HISTORY";

            public const string ExistShipmentItemNotConfDeliveredUnit = "BACKEND.APP_MESSAGE.EXIST_SHIPMENT_ITEM_NOT_CONF_DELIVERED_UNIT";

            public const string ShipmentNotConfRcptAmt = "BACKEND.APP_MESSAGE.SHIPMENT_NOT_CONF_RCPT_AMT";

            public const string ShipmentNotConfRcptAmtAndDeliveredUnits = "BACKEND.APP_MESSAGE.SHIPMENT_NOT_CONF_RCPT_AMT_AND_DELIVERED_UNITS";

            public const string ShipmentInvalidConfirmDeliveredUnits = "SHIPMENT.MESSAGE_INVALID_CONFIRM_DELIVERED_UNITS";

            public const string ShipmentHavingReceiptApply = "SHIPMENT.MESSAGE_SHIPMENT_HAVING_RECEIPT_APPLY";

            public const string ShipmentInvalidStatusForInTransitDelivery = "SHIPMENT.MESSAGE_INVALID_STATUS_FOR_INTRANSIT_DELIVERY";

            public const string ShipmentInvalidStatusForAssignToDelivery = "SHIPMENT.MESSAGE_INVALID_STATUS_FOR_ASSIGN_TO_DELIVERY";

            public const string ShipmentAssignToOtherDelivery = "SHIPMENT.MESSAGE_SHIPMENT_ASSIGN_TO_OTHER_DELIVERY";

            public const string ShipmentWithStatusCanNotCreateDelivery = "SHIPMENT.MESSAGE_SHIPMENT_WITH_STATUS_CAN_NOT_CREATE_DELIVERY";

            public const string ShipmentNotAllowToLoadWithNotEmptyWarehouse = "BACKEND.APP_MESSAGE.SHIPMENT_NOT_ALLOW_TO_LOAD_WITH_NOT_EMPTY_WAREHOUSE";

            public const string ShipmentTypeInvalidToCreateOrderReturn = "SHIPMENT.MESSAGE_INVALID_TYPE_TO_CREATE_ORDER_RETURN";

            public const string ShipmentStatusInvalidToCreateOrderReturn = "SHIPMENT.MESSAGE_INVALID_STATUS_TO_CREATE_ORDER_RETURN";

            public const string ShipmentAlreadyDeliveredCannotTransfer = "SHIPMENT.MESSAGE_CAN_NOT_TRANSFER_AFTER_DELIVERED";

            public const string PleaseConfirmRcptAmtOnModuleReceipt = "SHIPMENT.MESSAGE_PLEASE_CONFIRM_RCPT_AMT_ON_MODULE_RECEIPT";

            public const string DeliveryMustInStatusToConfDeliveredUnits = "BACKEND.APP_MESSAGE.DELIVERY_MUST_IN_STATUS_TO_CONF_DELIVERED_UNITS";

            public const string DeliveryNotFoundShipmentToConfDeliveredUnits = "BACKEND.APP_MESSAGE.DELIVERY_NOT_FOUND_SHIPMENT_TO_CONF_DELIVERED_UNITS";

            public const string DeliveryNotAllowToDeleteWithExistingLoadedShipment = "BACKEND.APP_MESSAGE.DELIVERY_NOT_ALLOW_TO_DELETE_WITH_EXISTING_LOADED_SHIPMENT";

            public const string DeliveryNotAllowToChangeVehicleWithExistingLoadedShipment = "BACKEND.APP_MESSAGE.DELIVERY_NOT_ALLOW_TO_CHANGE_VEHICLE_WITH_EXISTING_LOADED_SHIPMENT";

            public const string DeliveryNotAllowToRemoveLoadedShipment = "BACKEND.APP_MESSAGE.DELIVERY_NOT_ALLOW_TO_REMOVE_LOADED_SHIPMENT";

            public const string DeliveryNotAllowToTransferFromXToType = "BACKEND.APP_MESSAGE.DELIVERY_NOT_ALLOW_TO_TRANSFER_FROM_X_TO_Y_TYPE";

            public const string DeliveryNotAllowToCancelWithAllDeliveredShipments = "BACKEND.APP_MESSAGE.DELIVERY_NOT_ALLOW_TO_CANCEL_WITH_ALL_DELIVERED_SHIPMENTS";

            public const string DeliveryNotAllowToConfirmWithNotEmptyWarehouse = "BACKEND.APP_MESSAGE.DELIVERY_NOT_ALLOW_TO_CONFIRM_WITH_NOT_EMPTY_WAREHOUSE";

            public const string DeliveryExpressCanNotCreateWithNotEmptyVehicleWarehouse = "BACKEND.APP_MESSAGE.DELIVERY_EXPRESS_CAN_NOT_CREATE_WITH_NOT_EMPTY_VEHICLE_WAREHOUSE";

            public const string DeliveryTotalUnitsGreaterThanInTransitUnits = "BACKEND.APP_MESSAGE.TOTAL_DELIVERED_UNITS_IS_GREATER_THAN_INTRANSIT_UNITS";

            public const string DeliveryNotAllowToTransferPortionPickup = "BACKEND.APP_MESSAGE.DELIVERY_NOT_ALLOW_TO_TRANSFER_PORTION_PICKUP";

            public const string DeliveryNotAllowXWhenCompleted = "BACKEND.APP_MESSAGE.DELIVERY_NOT_ALLOW_X_WHEN_COMPLETED";

            public const string DispatchNoteExistAndDeleteToContinue = "BACKEND.APP_MESSAGE.EXISTS_DISPATCH_DELETE_TO_CONTINUE";

            public const string BackendEndOutOfStockMessage = "BACKEND.APP_MESSAGE.INVENTORY_OUT_OF_STOCK";

            public const string ImportErrorInFileImport = "IMPORT.ERROR_IN_FILE_IMPOR";

            public const string ImportErrorFileExtension = "IMPORT.ERROR_FILE_EXTENSION";

            public const string ImportHaveMoreSheetInFileExcel = "IMPORT.HAVE_MORE_SHEET_IN_FILE_EXCEL";

            public const string ImportCustomerPriceTempDesciption = "BACKEND.CUSTOMER_PRICE_TEMP.IMPORT_CUSTOMER_PRICE_TEMP_DESC";

            public const string ImportCustomerPriceTempName = "BACKEND.CUSTOMER_PRICE_TEMP.IMPORT_CUSTOMER_PRICE_TEMP_NAME";

            public const string ImportCannotDownloadFile = "IMPORT.APP_MESSAGE.CAN_NOT_DOWNLOAD_FILE";

            public const string ImportIsInProcess = "IMPORT.MESSAGE_IS_IN_PROCESS";

            public const string ImportInValidTemplate = "IMPORT.MESSAGE_INVALID_TEMPLATE";

            public const string ImportInValidUserSession = "IMPORT.MESSAGE_INVALID_USER_SESSION";

            public const string ImportSaveFileSuccess = "IMPORT.MESSAGE_SAVE_FILE_SUCCESS";

            public const string ImportCreateJobSuccess = "IMPORT.MESSAGE_CREATE_JOB_SUCCESS";

            public const string ImportDuplicateDataInFileImportMessage = "IMPORT.MESSAGE_DUPLICATE_DATA_IN_FILE_IMPORT";

            public const string ImportNotExistInSystemMessage = "IMPORT.MESSAGE_NOT_EXIST_IN_SYSTEM";

            public const string ImportExistInSystemMessage = "IMPORT.MESSAGE_EXIST_IN_SYSTEM";

            public const string ImportNotUniformMessage = "IMPORT.MESSAGE_NOT_UNIFORM";

            public const string ImportInvalidFormatMessage = "IMPORT.MESSAGE_INVALID_FORMAT";

            public const string ImportReceiveFileMessage = "IMPORT.MESSAGE_RECEIVE_FILE";

            public const string ImportCheckLoginSessionMessage = "IMPORT.MESSAGE_CHECK_LOGIN_SESSION";

            public const string ImportCompleteImportToTableTempMessage = "IMPORT.MESSAGE_COMPLETE_IMPORT_TO_TABLE_TEMP";

            public const string CreateJobInstanceName = "BACKEND.JOB_INSTANCE.AUTO_CREATE_JOB_INSTANCE_NAME";

            public const string CreateJobInstanceDesc = "BACKEND.JOB_INSTANCE.AUTO_CREATE_JOB_INSTANCE_DESC";

            public const string CalculatePriceStepValidate = "BACKEND.STEP_EXECUTION.CALCULATE_PRICE_STEP_VALIDATE";

            public const string CalculatePriceStepProduceProductSet = "BACKEND.STEP_EXECUTION.CALCULATE_PRICE_STEP_PRODUCE_PRODUCT_SET";

            public const string CalculatePriceStepProduceQueryCustomer = "BACKEND.STEP_EXECUTION.CALCULATE_PRICE_STEP_PRODUCE_QUERY_CUSTOMER";

            public const string CalculatePriceStepCustomerInIAsyncEnumerable = "BACKEND.STEP_EXECUTION.CALCULATE_PRICE_STEP_CUSTOMER_IN_IASYNCENUMERABLE";

            public const string CalculatePriceStepCreateDimCustomerPriceTemp = "BACKEND.STEP_EXECUTION.CALCULATE_PRICE_STEP_CREATE_DIM_CUSTOMER_PRICE_TEMP";

            public const string CalculatePriceStepGetFinalProductOfCustomer = "BACKEND.STEP_EXECUTION.CALCULATE_PRICE_STEP_GET_FINAL_PRODUCT_OF_CUSTOMER";

            public const string CalculatePriceStepGetPriceOfCustomerAndProduct = "BACKEND.STEP_EXECUTION.CALCULATE_PRICE_STEP_GET_PRICE_OF_CUSTOMER_AND_PRORDUCT";

            public const string QuantityBaseProductMessage = "BACKEND.STEP_EXECUTION.QUANTITY_BASE_PRODUCT_MESSAGE";

            public const string QuantityProductMessage = "BACKEND.STEP_EXECUTION.QUANTITY_PRODUCT_MESSAGE";

            public const string FinalProductEmptyMessage = "BACKEND.STEP_EXECUTION.FINAL_PRODUCT_EMPTY_MESSAGE";

            public const string CustomerAndProductHasNoPriceMessage = "BACKEND.STEP_EXECUTION.CUSTOMER_AND_PRODUCT_HAS_NO_PRICE_MESSAGE";

            public const string CustomerInIAsyncEnumerableMessage = "BACKEND.STEP_EXECUTION.CUSTOMER_IN_IASYNCENUMERABLE_MESSAGE";

            public const string GetPriceByCustomerAndXMessage = "BACKEND.STEP_EXECUTION.GET_PRICE_BY_CUSTOMER_AND_X_MESSAGE";

            public const string ExistsJobExecutionIsRunning = "BACKEND.JOB_EXECUTION.EXISTS_JOB_EXECUTION_IS_RUNNING";

            public const string ExistsJobExecutionIsStopped = "BACKEND.JOB_EXECUTION.EXISTS_JOB_EXECUTION_IS_STOPPED";

            public const string NotHaveBackgroundJobId = "JOB_EXECUTION.NOT_HAVE_BACK_GROUND_JOB";

            public const string BusinessUnitNotEnableNetPriceApi = "JOB_INSTANCE.VALIDATION.CUSTOMER_BELONG_BU_NOT_ENABLE_NET_PRICE_API";

            public const string CalcPriceCustomerDiffBu = "CALC_PRICE.MESSAGE_CUSTOMER_DIFF_BU";

            public const string DuplicatedProductListMessage = "BACKEND.MKT_PRODUCT_LIST.DUPLICATED_MESSAGE";

            public const string PromoProductListNotContainSellProductMessage = "BACKEND.MKT_PROMO_PRODUCT_LIST_NOT_CONTAIN_SELL_PRODUCT_MESSAGE";

            public const string ProductListItemCanNotContainBothParentChildProducts = "BACKEND.MKT_PRODUCT_LIST_ITEMS_CAN_NOT_CONTAIN_BOTH_PARENT_CHILD_PRODUCTS_MESSAGE";

            public const string PromotionNotAllowToExtend = "BACKEND.MESSAGE.MKT_PROMOTION_NOT_ALLOW_TO_EXTEND";

            public const string PromotionItemCode = "PROMOTION.FIELD_PROMOTION_ITEM_CODE";

            public const string EInvoiceDescriptionIssuance = "BACKEND.EINVOICE.ACTION_DESC_ISSUANCE";

            public const string MessageCreatedObject = "COMMON.MESSAGE.CREATED_OBJECT";

            public const string MessageUpdatedObject = "COMMON.MESSAGE.UPDATED_OBJECT";

            public const string MessageDeletedObject = "COMMON.MESSAGE.DELETED_OBJECT";

            public const string SentThirdPartySuccessfully = "BACKEND.APP_MESSAGE.SENT_THIRD_PARTY_SUCCESSFULLY";

            public const string SentThirdPartyFailed = "BACKEND.APP_MESSAGE.SENT_THIRD_PARTY_FAILED";

            public const string CancelThirdPartySuccessfully = "BACKEND.APP_MESSAGE.CANCEL_THIRD_PARTY_SUCCESSFULLY";

            public const string CancelThirdPartyFailed = "BACKEND.APP_MESSAGE.CANCEL_THIRD_PARTY_FAILED";

            public const string SendingThirdParty = "BACKEND.NOTICATION.SENDING_THIRD_PARTY";

            public const string SentThirdParty = "BACKEND.NOTICATION.SENT_THIRD_PARTY";

            public const string SendThirdPartyError = "BACKEND.NOTICATION.SEND_THIRD_PARTY_ERROR";

            public const string CancelingThirdParty = "BACKEND.NOTICATION.CANCELING_THIRD_PARTY";

            public const string CancelThirdParty = "BACKEND.NOTICATION.CANCEL_THIRD_PARTY";

            public const string CancelThirdPartyError = "BACKEND.NOTICATION.CANCEL_THIRD_PARTY_ERROR";

            public const string InfoThirdPartyNotExist = "BACKEND.NOTICATION.INFO_THIRD_PARTY_NOT_EXIST";

            public const string NotSupportThirdParty = "BACKEND.APP_MESSAGE.NOT_SUPPORT_THIRD_PARTY";

            public const string NotExistDataOnThirdParty = "BACKEND.APP_MESSAGE.NOT_EXIST_DATA_ON_THIRD_PARTY";

            public const string NotExistDataOnDictionaryMap = "BACKEND.APP_MESSAGE.NOT_EXIST_DATA_ON_DICTIONARY_MAP";

            public const string NotExistChartOfAccount = "BACKEND.APP_MESSAGE.NOT_EXIST_CHART_OF_ACCOUNT";

            public const string ThirdPartyNumberMissingSuffixOp = "BACKEND.APP_MESSAGE.THIRD_PARTY_NUMBER_MISSING_SUFFIX_OP";

            public const string GLVoucherEnvNo = "INVOICE.FIELD_GL_VOUCHER_ENV_NO";

            public const string GLVoucherVatNo = "INVOICE.FIELD_GL_VOUCHER_VAT_NO";

            public const string SalesNo = "INVOICE.FIELD_SALES_NO";

            public const string InOutWardNo = "INVOICE.FIELD_IN_OUT_WARD_NO";

            public const string InInWardNo = "INVOICE.FIELD_IN_IN_WARD_NO";

            public const string DefaultVatDescForInvoice = "BACKEND.APP_MESSAGE.DEFAULT_VAT_DESC_FOR_INVOICE";

            public const string DefaultVatDescForInvoiceItem = "BACKEND.APP_MESSAGE.DEFAULT_VAT_DESC_FOR_INVOICE_ITEM";

            public const string DefaultEnvTaxDescForInvoice = "BACKEND.APP_MESSAGE.DEFAULT_ENV_DESC_FOR_INVOICE";

            public const string DefaultEnvTaxDescForInvoiceItem = "BACKEND.APP_MESSAGE.DEFAULT_ENV_DESC_FOR_INVOICE_ITEM";

            public const string DefaultEnvTaxDescForPuInvoice = "BACKEND.APP_MESSAGE.DEFAULT_ENV_DESC_FOR_PU_INVOICE";

            public const string ReceiptDescForShipmentDelivered = "BACKEND.APP_MESSAGE.RECEIPT_DESC_FOR_SHIPMENT_DELIVERED";

            public const string ReceiptDescForShipmentConfRptAmt = "BACKEND.APP_MESSAGE.RECEIPT_DESC_FOR_SHIPMENT_CONF_RPT_AMT";

            public const string ReceiptDescForShipmentReConfRptAmt = "BACKEND.APP_MESSAGE.RECEIPT_DESC_FOR_SHIPMENT_RE_CONF_RPT_AMT";

            public const string ReceiptJobCreatedSuccessMessage = "BACKEND.APP_MESSAGE.RECEIPT_JOB_CREATED_SUCCESS_MESSAGE";

            public const string ReceiptJobUpdatedSuccessMessage = "BACKEND.APP_MESSAGE.RECEIPT_JOB_UPDATED_SUCCESS_MESSAGE";

            public const string ReceiptJobHaveNoReceiptCreatedMessage = "BACKEND.APP_MESSAGE.RECEIPT_JOB_HAVE_NO_RECEIPT_CREATED_MESSAGE";

            public const string ReceiptDescForImportConfRptAmt = "BACKEND.APP_MESSAGE.RECEIPT_DESC_FOR_IMPORT_CONF_RPT_AMT";

            public const string ReceiptAlreadyHaveReceiptApply = "BACKEND.APP_MESSAGE.RECEIPT_ALREADY_HAVE_RECEIPT_APPLY";

            public const string ArDocCreatedOnConfirmReceiptDesc = "BACKEND.APP_MESSAGE.AR_DOC_CREATED_ON_CONFIRM_RECEIPT_DESC";

            public const string ArDocAdjustIncreaseCreatedOnUpdateReceiptDesc = "BACKEND.APP_MESSAGE.AR_DOC_ADJUST_INCREASE_CREATED_ON_UPDATE_RECEIPT_DESC";

            public const string ArDocAdjustDecreaseCreatedOnUpdateReceiptDesc = "BACKEND.APP_MESSAGE.AR_DOC_ADJUST_DECREASE_CREATED_ON_UPDATE_RECEIPT_DESC";

            public const string TaxCustomerDocCreatedOnSendReceiptApply = "BACKEND.APP_MESSAGE.TAX_CUSTOMER_DOC_CREATED_ON_SEND_RECEIPT_APPLY";

            public const string TaxCustomerDocCreatedOnCancelReceiptApply = "BACKEND.APP_MESSAGE.TAX_CUSTOMER_DOC_CREATED_ON_CANCEL_RECEIPT_APPLY";

            public const string AppliedAmtGreaterThanReceiptRemainTaxAmt = "BACKEND.APP_MESSAGE.APPLIED_AMT_GREATER_THAN_RECEIPT_REMAIN_TAX_AMT";

            public const string AppliedAmtGreaterThanInvoiceAppliedRemainTaxAmt = "BACKEND.APP_MESSAGE.APPLIED_AMT_GREATER_THAN_INVOICE_APPLIED_REMAIN_TAX_AMT";

            public const string ReasonForPaymentReceiptDefaultValue = "RECEIPT_APPLY.REASON_FOR_PAYMENT_RECEIPT_DEFAULT_VALUE";

            public const string ExplanationForReasonDefaultValue = "RECEIPT_APPLY.EXPLANATION_FOR_REASON_DEFAULT_VALUE";

            public const string MessageUserSentSaVoucherAmis = "BACKEND.MESSAGE_USER_SENT_SAVOUCHER_AMIS";

            public const string MessageUserSentSaReturnAmis = "BACKEND.MESSAGE_USER_SENT_SARETURN_AMIS";

            public const string MessageUserDeleteVoucherAmis = "BACKEND.MESSAGE_USER_DELETE_VOUCHER_AMIS";

            public const string MessageCannotAddReceiptDebtReqToDeliveryWithStatus = "BACKEND.MESSAGE_CANNOT_ADD_RECEIPT_DEBT_REQ_TO_DELIVERY_WITH_STATUS";

            public const string MessageCannotCompleteDeliveryWithExistingReceiptDebtReqNotComplete = "BACKEND.MESSAGE_CANNOT_COMPLETE_DELIVERY_WITH_EXISTING_RECEIPT_DEBT_REQ_NOT_COMPLETE";

            public const string MessageCannotDeleteTeamTaskIsInUse = "BACKEND.STORAGE.MESSAGE_CANNOT_DELETE_TEAM_TASK_IS_IN_USE";

            public const string MessageCannotCompleteDeliveryWithExistingParcelNotDelivered = "BACKEND.MESSAGE_CANNOT_COMPLETE_DELIVERY_WITH_EXISTING_PARCEL_NOT_DELIVERED";

            public const string MessageCannotIntransitDeliveryWithInvalidStatusParcel = "BACKEND.MESSAGE_CANNOT_INTRANSIT_DELIVERY_WITH_INVALID_STATUS_PARCEL";

            public const string Latitude = "COMMON.FIELD_LATITUDE";

            public const string Longitude = "COMMON.FIELD_LONGITUDE";

            public const string BeginDate = "COMMON.FIELD_BEGIN_DATE";

            public const string EndDate = "COMMON.FIELD_END_DATE";

            public const string BlockPrice = "CUSTOMER_PRICE.FIELD_BLOCK_PRICE";

            public const string MinSalePrice = "CUSTOMER_PRICE.FIELD_MIN_SALE_PRICE";

            public const string MaxSalePrice = "CUSTOMER_PRICE.FIELD_MAX_SALE_PRICE";

            public const string PromoPrice = "CUSTOMER_PRICE.FIELD_PROMO_PRICE";

            public const string Or = "COMMON.FIELD_OR";

            public const string EInvoiceTypePXK = "EINVOICE.FIELD_TYPE_PXK";

            public const string EInvoiceTypeEInvoice = "EINVOICE.FIELD_TYPE_EINVOICE";

            public const string Code = "COMMON.FIELD_CODE";

            public const string Name = "COMMON.FIELD_NAME";

            public const string ShortName = "COMMON.FIELD_SHORT_NAME";

            public const string Address = "COMMON.FIELD_ADDRESS";

            public const string TypeCode = "COMMON.FIELD_TYPE_CODE";

            public const string Description = "COMMON.FIELD_DESCRIPTION";

            public const string ReferenceCode = "COMMON.FIELD_REFERENCE_CODE";

            public const string PrimaryContact = "CUSTOMER.FIELD_PRIMARY_CONTACT";

            public const string ParentWarehouseCode = "WAREHOUSE.FIELD_PARENT_WAREHOUSE_CODE";

            public const string BusinessUnitCode = "DATA_CHANGE.BUSINESS_UNIT_CODE";

            public const string BusinessUnitName = "DATA_CHANGE.BUSINESS_UNIT_NAME";

            public const string Status = "COMMON.FIELD_STATUS";

            public const string ParentGroup = "PRODUCT_GROUP.FORM_PARENT_GROUP";

            public const string ProductCode = "PRODUCT.PRODUCT_CODE";

            public const string ProductName = "PRODUCT.PRODUCT_NAME";

            public const string ProductGroupCode = "CUSTOMER_PRICING.FIELD_PRODUCTGROUPCODE";

            public const string ProductCategoryCode = "ORDER_QUANTITY.FIELD_PRODUCT_CATEGORY_CODE";

            public const string ProductGroupCode1 = "ORDER_QUANTITY.FIELD_PRODUCT_GROUP_CODE_1";

            public const string ProductGroupName1 = "ORDER_QUANTITY.FIELD_PRODUCT_GROUP_NAME_1";

            public const string ProductGroupCode2 = "ORDER_QUANTITY.FIELD_PRODUCT_GROUP_CODE_2";

            public const string ProductGroupName2 = "ORDER_QUANTITY.FIELD_PRODUCT_GROUP_NAME_2";

            public const string ProductGroupCode3 = "ORDER_QUANTITY.FIELD_PRODUCT_GROUP_CODE_3";

            public const string ProductGroupName3 = "ORDER_QUANTITY.FIELD_PRODUCT_GROUP_NAME_3";

            public const string EcoProtectionTax = "COMMON.ECO_PROTECT_TAX";

            public const string Type = "IMPORT.TYPE";

            public const string DescriptionError = "IMPORT.DESCRIPTION_ERROR";

            public const string PricingCode = "CUSTOMER_PRICING.FIELD_PRICINGCODE";

            public const string PricingName = "CUSTOMER_PRICING.FIELD_PRICINGNAME";

            public const string RoleTeam = "PRICING.ROLETEAM_ROLE";

            public const string UomCode = "PRODUCT.FIELD_UOMCODE";

            public const string UomName = "PRODUCT.FIELD_UOMNAME";

            public const string UomCode2 = "PRODUCT.FIELD_UOMCODE2";

            public const string WarehouseCode = "COMMON.WAREHOUSE_CODE";

            public const string OnHandUnits = "PRODUCT_WAREHOUSE.FIELD_ON_HAND_UNITS";

            public const string CurrentUnitCost = "PRODUCT_WAREHOUSE.FIELD_CURRENT_UNIT_COST";

            public const string Year = "COMMON.FIELD_YEAR";

            public const string Month = "COMMON.FIELD_MONTH";

            public const string Day = "COMMON.FIELD_DATE";

            public const string ObjectTypeCode = "DATA_CHANGE.OBJECT_TYPE_CODE";

            public const string SubObjectTypeCode = "COMMON.SUB_OBJECT_TYPE_CODE";

            public const string SubObjectCode = "COMMON.SUB_OBJECT_CODE";

            public const string Prefix = "AUTO_NUMBER.PREFIX";

            public const string DigitRange = "AUTO_NUMBER.DIGIT_RANGE";

            public const string FormatExpression = "AUTO_NUMBER.FORMAT_EXPRESSION";

            public const string PeriodTypeCode = "AUTO_NUMBER.PERIOD_TYPE_CODE";

            public const string NextValue = "AUTO_NUMBER.NEXT_VALUE";

            public const string WarehouseName = "COMMON.WAREHOUSE_NAME";

            public const string AvailableToPromiseUnits = "ORDER.FIELD_AVAILABLE_TO_PROMISE_UNITS";

            public const string ConvertRate2 = "PRODUCT.FIELD_CONVERRATE2";

            public const string Weight = "PRODUCT.FIELD_WEIGHT";

            public const string WeightUomCode = "COMMON.WEIGHT_UOM_CODE";

            public const string Volume = "PRODUCT.FIELD_VOLUME";

            public const string VolumeUomCode = "COMMON.VOLUME_UOM_CODE";

            public const string NumberOfSubProduct = "PRODUCT.FIELD_NUMBEROFSUBPRODUCT";

            public const string SubunitId = "PRODUCT.FIELD_SUBUNIT_ID";

            public const string SubunitCode = "PRODUCT.FIELD_SUBUNIT_CODE";

            public const string PackagingCode = "PRODUCT.FIELD_PACKAGINGCODE";

            public const string ExpectUnits = "DOCUMENT.FIELD_EXPECT_UNITS";

            public const string ActualUnits = "DOCUMENT.FIELD_ACTUAL_UNITS";

            public const string OrderNumber = "COMMON.ORDER_NUMBER";

            public const string OrderCode = "ORDER.FIELD_CODE";

            public const string OrderType = "COMMON.FIELD_ORDER_TYPE";

            public const string CustomerNumber = "PRICEREQUEST.FIELD_CUSTOMERNUMBER";

            public const string CustomerCode = "COMMON.FIELD_CUSTOMER_CODE";

            public const string CustomerName = "COMMON.CUSTOMER_NAME";

            public const string Units = "COMMON.FIELD_UNITS";

            public const string ToShipUnits = "COMMON.FIELD_TO_SHIP_UNITS";

            public const string RequestedUnits = "COMMON.FIELD_REQUESTEDUNITS";

            public const string IntransitUnits = "COMMON.FIELD_INTRANSITUNITS";

            public const string DeliveredUnits = "COMMON.FIELD_DELIVERED_UNITS";

            public const string CancelUnits = "COMMON.FIELD_CANCEL_UNITS";

            public const string DeliveredVolume = "COMMON.FIELD_DELIVERED_VOLUME";

            public const string UnitPriceBeforePromotion = "COMMON.UNIT_PRICE_BEFORE_PROMOTION";

            public const string UnitPriceAfterPromotion = "COMMON.UNIT_PRICE_AFTER_PROMOTION";

            public const string TotalAmtAfterPromotion = "COMMON.TOTAL_AMT_AFTER_PROMOTION";

            public const string NetPriceReport = "REPORT.FIELD_NET_PRICE";

            public const string MinSalePriceReport = "REPORT.FIELD_MIN_SALE_PRICE";

            public const string ShipmentNumber = "SHIPMENT.SHIPMENT_NUMBER";

            public const string Owner = "COMMON.FIELD_OWNER";

            public const string CustomerOwner = "CUSTOMER.FIELD_OWNER";

            public const string InvoiceSendDate = "INVOICE.FIELD_SEND_DATE";

            public const string CarriedBy = "COMMON.FIELD_CARRIED_BY";

            public const string DeliveryDate = "SHIPMENT.DELIVERY_DATE";

            public const string OrderGroup = "REPORT.FIELD_ORDER_GROUP";

            public const string RequestNumber = "REPORT.FIELD_REQUEST_NUMBER";

            public const string Number = "COMMON.FIELD_NUMBER";

            public const string SourceNumber = "COMMON.FIELD_SOURCE_NUMBER";

            public const string SourceType = "COMMON.FIELD_SOURCE_TYPE_CODE";

            public const string RefSourceNumber = "COMMON.FIELD_REF_SOURCE_NUMBER";

            public const string RefSourceType = "COMMON.FIELD_REF_SOURCE_TYPE_CODE";

            public const string Phone = "COMMON.FIELD_PHONE";

            public const string CurrentLiability = "CUSTOMER.FIELD_CURRENTLIABILITY";

            public const string MaximumLiability = "CUSTOMER.FIELD_MAXIMUMLIABILITY";

            public const string OwnerUserName = "";

            public const string CurrentBalance = "FLOW.BUDGET.FIELD_CURRENT_BALANCE";

            public const string TotalSubmittedOrders = "COMMON.FIELD_TOTAL_SUBMITTED_ORDERS";

            public const string TotalApprovedOrders = "COMMON.FIELD_TOTAL_APPROVED_ORDERS";

            public const string TotalShipped = "COMMON.FIELD_TOTAL_SHIPPED";

            public const string CreatedDate = "COMMON.FIELD_CREATED_DATE";

            public const string UserName = "USER.FIELD_USERNAME";

            public const string Gender = "COMMON.FIELD_GENDER";

            public const string DateOfBirth = "COMMON.FIELD_DOB";

            public const string UserFullName = "USER.PROFILER.FULLNAME";

            public const string PermanentAddress = "USER.FIELD_PERMANENT_ADDRESS";

            public const string CurrentAddress = "USER.FIELD_CURRENT_ADDRESS";

            public const string Role = "ROLE.PLURAL_NAME";

            public const string ApprovedBy = "COMMON.APPROVED_BY";

            public const string ApprovedDate = "COMMON.APPROVED_DATE";

            public const string ExpiredDate = "COMMON.EXPIRED_DATE";

            public const string UnitPrice = "COMMON.FIELD_UNIT_PRICE";

            public const string UnitPriceNet = "PRICEREQUEST.FIELD_UNITPRICE_NET";

            public const string LatestOdo = "COMMON.FIELD_LATEST_ODO";

            public const string AuthorizedPayload = "VEHICLE_AUTHORIZED_PAY_LOAD";

            public const string CargoVolume = "VEHICLE_CARGO_VOLUME";

            public const string ManufacturedDate = "VEHICLE_MANUFACTURED_DATE";

            public const string Manufactured = "VEHICLE_MANUFACTURER";

            public const string RegistrationNumber = "VEHICLE_REGISTRATION_NUMBER";

            public const string ChassisNumber = "VEHICLE_CHASSIS_NUMBER";

            public const string EngineNumber = "VEHICLE_ENGINE_NUMBER";

            public const string OwnershipTypeCode = "COMMON.FIELD_OWNERSHIP";

            public const string TotalVolume = "PRODUCT.FIELD_TOTAL_VOLUME";

            public const string CreatedBy = "COMMON.FIELD_CREATED_BY";

            public const string EndBy = "COMMON.FIELD_END_BY";

            public const string CancelDate = "COMMON.FIELD_CANCEL_DATE";

            public const string CancelBy = "COMMON.FIELD_CANCEL_BY";

            public const string CurrentDate = "COMMON.VALIDATION.FIELD_CURRENT_DATE";

            public const string SendBy = "INVOICE.FIELD_SEND_BY";

            public const string UnitPriceNotDiscount = "INVOICE.UNIT_PRICE.NOT_DISCOUNT";

            public const string AmountNotDiscount = "INVOICE.AMOUNT.NOT_DISCOUNT";

            public const string Discount = "INVOICE.DISCOUNT";

            public const string DiscountRate = "INVOICE.DISCOUNT_RATE";

            public const string AmountAfterDiscount = "INVOICE.AMOUNT.AFTER_DISCOUNT";

            public const string NetAmount = "INVOICE.NET_AMOUNT";

            public const string TaxAmount = "INVOICE.TAX_AMOUNT";

            public const string TaxRate = "INVOICE.TAXRATE";

            public const string UnitPriceAfterDiscount = "INVOICE.UNIT_PRICE.AFTER_DISCOUNT";

            public const string ExtRefNumber = "INVOICE.FIELD_EXT_REF_NUMBER";

            public const string InvoiceDate = "INVOICE.INVOICE_DATE";

            public const string VehicleWarehouse = "DISPATCH_NOTE.FIELD_VEHICLE_WAREHOUSE";

            public const string TaxCode = "COMMON.FIELD_TAXCODE";

            public const string ParentCustomer = "CUSTOMER.FIELD_PARENT_CUSTOMER";

            public const string PrimaryPhone = "CUSTOMER.FIELD_PRIMARY_PHONE";

            public const string Tag = "TAG";

            public const string Dimension = "COMMON.FIELD_DIMENSION";

            public const string Dimension1 = "COMMON.FIELD_DIMENSION_1";

            public const string Dimension2 = "COMMON.FIELD_DIMENSION_2";

            public const string DimensionCode2 = "COMMON.FIELD_DIMENSION_CODE_2";

            public const string Code2 = "COMMON.FIELD_CODE2";

            public const string CustomerState = "CUSTOMER.FIELD_RELATIONSHIP_STATE";

            public const string Yes = "COMMON.BTN_YES";

            public const string No = "COMMON.BTN_NO";

            public const string Other = "COMMON.FIELD_OTHER";

            public const string PartyRelationshipCode = "INVOICE.PARTY_RELATIONSHIP_CODE";

            public const string OfficialName = "COMMON.FIELD_OFFICIAL_NAME";

            public const string ObjectCode = "COMMON.FIELD_OBJ_CODE";

            public const string ObjectName = "COMMON.FIELD_OBJ_NAME";

            public const string DocNumber = "AR.FIELD_DOC_NUMBER";

            public const string SalesPersonUserName = "AR.FIELD_SALES_PERSON_USERNAME";

            public const string BeginBalance = "AR.FIELD_BEGIN_BALANCE";

            public const string BeginDr = "AR.FIELD_BEGIN_DR";

            public const string BeginCr = "AR.FIELD_BEGIN_CR";

            public const string NetDr = "AR.FIELD_NET_DR";

            public const string NetCr = "AR.FIELD_NET_CR";

            public const string EndDr = "AR.FIELD_END_DR";

            public const string EndCr = "AR.FIELD_END_CR";

            public const string EndBalance = "AR.FIELD_END_BALANCE";

            public const string CurrentDr = "AR.FIELD_CURRENT_DR";

            public const string CurrentCr = "AR.FIELD_CURRENT_CR";

            public const string PtdSales = "AR.FIELD_PTD_SALES";

            public const string PtdFinCharges = "AR.FIELD_PTD_FIN_CHARGE";

            public const string PtdPayments = "AR.FIELD_PTD_PAYMENT";

            public const string PtdAdjustDr = "AR.FIELD_PTD_ADJUST_DR";

            public const string PtdAdjustCr = "AR.FIELD_PTD_ADJUST_CR";

            public const string PtdRefund = "AR.FIELD_PTD_REFUND";

            public const string TotalAmt = "AR.FIELD_TOTAL_AMT";

            public const string AmtWithTax = "AR.FIELD_AMT_WITH_TAX";

            public const string DiscountAmt = "AR.FIELD_DISCOUNT_AMT";

            public const string FreightAmtWithTax = "AR.FIELD_FREIGHT_AMOUNT_WITH_TAX";

            public const string TaxAmt = "AR.FIELD_TAX_AMT";

            public const string NetAmt = "AR.FIELD_NET_AMT";

            public const string DueDate = "AR.FIELD_DUE_DATE";

            public const string PostedDate = "AR.FIELD_POSTED_DATE";

            public const string PostedName = "AR.FIELD_POSTED_NAME";

            public const string OrigDocNumber = "AR.FIELD_ORG_DOC_NUMBER";

            public const string OrigDocType = "AR.FIELD_ORG_DOC_TYPE_CODE";

            public const string OrigDocAmount = "AR.FIELD_ORG_DOC_AMOUNT";

            public const string DocType = "AR.FIELD_DOC_TYPE_CODE";

            public const string SalesPerson = "AR.FIELD_SALES_PERSON";

            public const string Amount = "CUSTOMERLIABILITY.FIELD_AMOUNT";

            public const string BeginUnits = "INVENTORY_JOURNAL.FIELD_BEGIN_UNITS";

            public const string BeginUnitCost = "INVENTORY_JOURNAL.FIELD_BEGIN_UNIT_COST";

            public const string BeginTotalCost = "INVENTORY_JOURNAL.FIELD_BEGIN_TOTAL_COST";

            public const string DebitUnits = "INVENTORY_JOURNAL.FIELD_DEBIT_UNITS";

            public const string DebitUnitCode = "INVENTORY_JOURNAL.FIELD_DEBIT_UNIT_COST";

            public const string DebitTotalCost = "INVENTORY_JOURNAL.FIELD_DEBIT_TOTAL_COST";

            public const string DebitUnitPrice = "INVENTORY_JOURNAL.FIELD_DEBIT_UNIT_PRICE";

            public const string DebitTotalPrice = "INVENTORY_JOURNAL.FIELD_DEBIT_TOTAL_PRICE";

            public const string CreditUnits = "INVENTORY_JOURNAL.FIELD_CREDIT_UNITS";

            public const string CreditUnitCost = "INVENTORY_JOURNAL.FIELD_CREDIT_UNIT_COST";

            public const string CreditTotalCost = "INVENTORY_JOURNAL.FIELD_CREDIT_TOTAL_COST";

            public const string CreditUnitPrice = "INVENTORY_JOURNAL.FIELD_CREDIT_UNIT_PRICE";

            public const string CreditTotalPrice = "INVENTORY_JOURNAL.FIELD_CREDIT_TOTAL_PRICE";

            public const string EndUnits = "INVENTORY_JOURNAL.FIELD_END_UNITS";

            public const string EndUnitCost = "INVENTORY_JOURNAL.FIELD_END_UNIT_COST";

            public const string EndTotalCost = "INVENTORY_JOURNAL.FIELD_END_TOTAL_COST";

            public const string ReceiptUnits = "INVENTORY_JOURNAL.FIELD_RECEIPT_UNITS";

            public const string ReceiptReason = "INVENTORY_JOURNAL.FIELD_RECEIPT_REASON";

            public const string TransferUnits = "INVENTORY_JOURNAL.FIELD_TRANSFER_UNITS";

            public const string AdjustIncreaseUnits = "INVENTORY_JOURNAL.FIELD_ADJUST_INCREASE_UNITS";

            public const string ReturnUnits = "INVENTORY_JOURNAL.FIELD_RETURN_UNITS";

            public const string SaleUnits = "INVENTORY_JOURNAL.FIELD_SALE_UNITS";

            public const string TransferOutUnits = "INVENTORY_JOURNAL.FIELD_TRANSFER_OUT_UNITS";

            public const string AdjustDecreaseUnits = "INVENTORY_JOURNAL.FIELD_ADJUST_DECREASE_UNITS";

            public const string InventoryDocumentNumber = "INVENTORY_JOURNAL.FIELD_INVENTORY_DOCUMENT_NUMBER";

            public const string InventoryDocumentItemNumber = "INVENTORY_JOURNAL.FIELD_INVENTORY_DOCUMENT_ITEM_NUMBER";

            public const string InventoryDocumentType = "INVENTORY_FORECAST_JOURNAL.FIELD_INVENTORY_DOCUMENT_TYPE";

            public const string IssueReason = "INVENTORY_JOURNAL.FIELD_ISSUE_REASON";

            public const string TransferReason = "INVENTORY_JOURNAL.FIELD_TRANSFER_REASON";

            public const string VarianceReason = "INVENTORY_JOURNAL.FIELD_VARIANCE_REASON";

            public const string OnDemandUnits = "INVENTORY_FORECAST_JOURNAL.FIELD_ON_DEMAND_UNITS";

            public const string OnScheduledUnits = "INVENTORY_FORECAST_JOURNAL.FIELD_ON_SCHEDULED_UNITS";

            public const string InvDocReceipt = "INVENTORY_DOCUMENT.DOCUMENT_TYPE_RECEIPT";

            public const string InvDocIssue = "INVENTORY_DOCUMENT.DOCUMENT_TYPE_ISSUE";

            public const string InvDocTransfer = "INVENTORY_DOCUMENT.DOCUMENT_TYPE_TRANSFER";

            public const string InvDocAdjustment = "INVENTORY_DOCUMENT.DOCUMENT_TYPE_ADJUSTMENT";

            public const string DispatchNoteStatus = "DISPATCH_NOTE.FIELD_STATUS_CODE";

            public const string RequestDate = "FBS.FIELD_REQUEST_DATE";

            public const string RequestNo = "FBS.FIELD_REQUEST_NO";

            public const string FbsRefDate = "FBS.FIELD_EXT_REF_DATE";

            public const string FbsRefNumber = "FBS.FIELD_EXT_REF_NUMBER";

            public const string DocumentWarehouse = "FBS.FIELD_DOCUMENT_WAREHOUSE";

            public const string ReceiptWarehouse = "INVENTORY.RECEIPT.WAREHOUSE";

            public const string Country = "COUNTRY.PLURAL_NAME";

            public const string ProvinceCode = "PROVINCE.FIELD_CODE";

            public const string ProvinceName = "PROVINCE.FIELD_NAME";

            public const string DistrictCode = "DISTRICT.FIELD_CODE";

            public const string DistrictName = "DISTRICT.FIELD_NAME";

            public const string RelationshipType = "PARTY_RELATIONSHIP.FIELD_RELATIONSHIP_TYPE";

            public const string SubPartyCode = "PARTY_RELATIONSHIP.FIELD_SUB_PARTY_CODE";

            public const string SubPartyName = "PARTY_RELATIONSHIP.FIELD_SUB_PARTY_NAME";

            public const string SubPartyTaxCode = "PARTY_RELATIONSHIP.FIELD_SUB_PARTY_TAX_CODE";

            public const string PartyMessageDuplicatedAtAddress = "PARTY.MESSAGE_DUPLIDATED_AT_ADDRESS";

            public const string PartyMessageDuplicatedAtAddressAndPhone = "PARTY.MESSAGE_DUPLIDATED_AT_ADDRESS_AND_PHONE";

            public const string PartyMessageDuplicatedWithTaxCode = "PARTY.MESSAGE_DUPLIDATED_AT_WITH_TAX_CODE";

            public const string IssuanceNumber = "EINVOICE.FIELD_ISSUANCE_NUMBER";

            public const string IssuanceEInvoiceDate = "EINVOICE.FIELD_ISSUANCE_EINVOICE_DATE";

            public const string IssuanceEInvoiceBy = "EINVOICE.FIELD_ISSUANCE_EINVOICE_BY";

            public const string ProductDesc = "INVOICE.FIELD_PRODUCT_DESC";

            public const string StockAccount = "DEFAULT_ACCT.FIELD_STOCK_ACCOUNT";

            public const string DebitAccountStock = "DEFAULT_ACCT.FIELD_DEBIT_ACCOUNT_STOCK";

            public const string CreditAccountStock = "DEFAULT_ACCT.FIELD_CREDIT_ACCOUNT_STOCK";

            public const string DebitAccountGLVoucherEnv = "DEFAULT_ACCT.FIELD_DEBIT_ACCOUNT_GL";

            public const string CreditAccountGLVoucherEnv = "DEFAULT_ACCT.FIELD_CREDIT_ACCOUNT_GL";

            public const string DebitAccountGLVoucherVat = "DEFAULT_ACCT.FIELD_DEBIT_ACCOUNT_VAT";

            public const string CreditAccountGLVoucherVat = "DEFAULT_ACCT.FIELD_CREDIT_ACCOUNT_VAT";

            public const string DebitAccount = "DEFAULT_ACCT.FIELD_DEBIT_ACCOUNT";

            public const string CreditAccount = "DEFAULT_ACCT.FIELD_CREDIT_ACCOUNT";

            public const string VatAccount = "DEFAULT_ACCT.FIELD_VAT_ACCOUNT";

            public const string DiscountAccount = "DEFAULT_ACCT.FIELD_DISCOUNT_ACCOUNT";

            public const string CostAccount = "DEFAULT_ACCT.FIELD_COST_ACCOUNT";

            public const string SalesAccount = "DEFAULT_ACCT.FIELD_SALES_ACCOUNT";

            public const string LiabilityAccount = "DEFAULT_ACCT.FIELD_LIABILITY_ACCOUNT";

            public const string AmisInvoiceNo = "INVOICE.AMIS.INVOICE_NO";

            public const string AmisInOutWardType = "INVOICE.AMIS.IN_OUT_WARD_TYPE";

            public const string AmisSalesType = "INVOICE.AMIS.SALES_TYPE";

            public const string AmisPaymentMode = "INVOICE.AMIS.PAYMENT_MODE";

            public const string AmisAccountingStatus = "INVOICE.AMIS.ACCOUNTING_STATUS";

            public const string AmisPaymentMethod = "INVOICE.AMIS.PAYMENT_METHOD";

            public const string AmisIncludingWarehouseDeliveryNote = "INVOICE.AMIS.INCLUDING_WAREHOUSE_DELIVERY_NOTE";

            public const string AmisInvoiceAttached = "INVOICE.AMIS.INVOICE_ATTACHED";

            public const string AmisInvoiceCreated = "INVOICE.AMIS.INVOICE_CREATED";

            public const string AmisAccountingDate = "INVOICE.AMIS.ACCOUNTING_DATE";

            public const string AmisDocumentDate = "INVOICE.AMIS.DOCUMENT_DATE";

            public const string AmisInvoiceDate = "INVOICE.AMIS.INVOICE_DATE";

            public const string AmisDocumentNumber = "INVOICE.AMIS.DOCUMENT_NUMBER";

            public const string AmisDeliveryNoteNumber = "INVOICE.AMIS.DELIVERY_NOTE_NUMBER";

            public const string AmisInvoiceTemplate = "INVOICE.AMIS.INVOICE_TEMPLATE";

            public const string AmisInvoicePattern = "INVOICE.AMIS.INVOICE_PATTERN";

            public const string AmisInvoiceNumber = "INVOICE.AMIS.INVOICE_NUMBER";

            public const string AmisCustomerCode = "INVOICE.AMIS.CUSTOMER_CODE";

            public const string AmisObjectCode = "INVOICE.AMIS.OBJECT_CODE";

            public const string AmisDebitObjectCode = "INVOICE.AMIS.DEBIT_OBJECT_CODE";

            public const string AmisCreditObjectCode = "INVOICE.AMIS.CREDIT_OBJECT_CODE";

            public const string AmisExplanationOrReason = "INVOICE.AMIS.EXPLANATION_OR_REASON";

            public const string AmisExplanation = "INVOICE.AMIS.EXPLANATION";

            public const string AmisBusinessType = "INVOICE.AMIS.BUSINESS_TYPE";

            public const string AmisBusiness = "INVOICE.AMIS.BUSINESS";

            public const string AmisExplanationForAccounting = "INVOICE.AMIS.EXPLANATION_FOR_ACCOUNTING";

            public const string AmisReasonForExport = "INVOICE.AMIS.REASON_FOR_EXPORT";

            public const string AmisProductCode = "INVOICE.AMIS.PRODUCT_CODE";

            public const string AmisIsNoteLine = "INVOICE.AMIS.IS_NOTE_LINE";

            public const string AmisPromotionalItem = "INVOICE.AMIS.PROMOTIONAL_ITEM";

            public const string AmisCommercialDiscount = "INVOICE.AMIS.COMMERCIAL_DISCOUNT";

            public const string AmisMoneyOrCostOrDebitAccount = "INVOICE.AMIS.MONEY_OR_COST_OR_DEBIT_ACCOUNT";

            public const string AmisRevenueOrCreditAccount = "INVOICE.AMIS.REVENUE_OR_CREDIT_ACCOUNT";

            public const string AmisUnits = "INVOICE.AMIS.UNITS";

            public const string AmisNetAmount = "INVOICE.AMIS.NET_AMOUNT";

            public const string AmisUnitPrice = "INVOICE.AMIS.UNIT_PRICE";

            public const string AmisTotalAmount = "INVOICE.AMIS.TOTAL_AMOUNT";

            public const string AmisDiscountRate = "INVOICE.AMIS.DISCOUNT_RATE";

            public const string AmisDiscountAmount = "INVOICE.AMIS.DISCOUNT_AMOUNT";

            public const string AmisDiscountAccount = "INVOICE.AMIS.DISCOUNT_ACCOUNT";

            public const string AmisTaxRate = "INVOICE.AMIS.TAX_RATE";

            public const string AmisTotalTax = "INVOICE.AMIS.TOTAL_TAX";

            public const string AmisVatAccount = "INVOICE.AMIS.VAT_ACCOUNT";

            public const string AmisWarehouseCode = "INVOICE.AMIS.WAREHOUSE_CODE";

            public const string AmisCostAccount = "INVOICE.AMIS.COST_ACCOUNT";

            public const string AmisStockAccount = "INVOICE.AMIS.STOCK_ACCOUNT";

            public const string AmisDebitAccount = "INVOICE.AMIS.DEBIT_ACCOUNT";

            public const string AmisCreditAccount = "INVOICE.AMIS.CREDIT_ACCOUNT";

            public const string AmisDomesticSalesOfGoods = "INVOICE.AMIS.DOMESTIC_SALES_OF_GOODS";

            public const string AmisPaymentNotReceived = "INVOICE.AMIS.PAYMENT_NOT_RECEIVED";

            public const string AmisAlreadyCreated = "INVOICE.AMIS.ALREADY_CREATED";

            public const string AmisCashOrBankTransfer = "INVOICE.AMIS.CASH_OR_BANK_TRANSFER";

            public const string AmisAlreadyAccounted = "INVOICE.AMIS.ALREADY_ACCOUNTED";

            public const string AmisOtherWarehouseExport = "INVOICE.AMIS.OTHER_WAREHOUSE_EXPORT";

            public const string AmisVatAccordingToInvoiceNumber = "INVOICE.AMIS.VAT_ACCORDING_TO_INVOICE_NUMBER";

            public const string AmisDate = "INVOICE.AMIS.DATE_WITH_WHITESPACE";

            public const string AmisVatHyphen = "INVOICE.AMIS.VAT_HYPHEN";

            public const string AmisTaxDeductionsForBusinessUnitActivities = "INVOICE.AMIS.TAX_DEDUCTIONS_FOR_BUSINESS_ACTIVITIES";

            public const string AmisECoTaxAccordingToInvoiceNumber = "INVOICE.AMIS.ECO_TAX_ACCORDING_TO_INVOICE_NUMBER";

            public const string AmisECoTaxHyphen = "INVOICE.AMIS.ECO_TAX_HYPHEN";

            public const string AmisECoTaxAmt = "INVOICE.AMIS.ECO_TAX_AMOUNT";

            public const string AmisProductGrossWeight = "INVOICE.AMIS.PRODUCT_GROSS_WEIGHT";

            public const string AmisTaxRateDoNotDeclare = "INVOICE.AMIS.TAX_RATE_DO_NOT_DECLARE";

            public const string AmisTaxRateNotTaxable = "INVOICE.AMIS.TAX_RATE_NOT_TAXABLE";

            public const string Customer = "CUSTOMER.ENTITY_DISPLAYNAME";

            public const string CustomerAddress = "CUSTOMERADDRESS.ENTITY_DISPLAYNAME";

            public const string CustomerPricing = "CUSTOMER_PRICING.ENTITY_DISPLAYNAME";

            public const string Product = "PRODUCT.ENTITY_DISPLAYNAME";

            public const string ProductGroup = "PRODUCT_GROUP.ENTITY_DISPLAYNAME";

            public const string ProductCategory = "PRODUCT_CATEGORY.ENTITY_DISPLAYNAME";

            public const string ProductBu = "PRODUCT_BU.ENTITY_DISPLAY_NAME";

            public const string ProductGroupBu = "PRODUCT_GROUP_BU.ENTITY_DISPLAY_NAME";

            public const string Pricing = "PRICING.ENTITY_DISPLAYNAME";

            public const string BusinessUnit = "BUSINESSUNIT.ENTITY_DISPLAYNAME";

            public const string PricingItem = "PRICING_ITEM.ENTITY_DISPLAY_NAME";

            public const string CustomerPrice = "CUSTOMER_PRICE.ENTITY_DISPLAYNAME";

            public const string CustomerPriceTemp = "CUSTOMER_PRICE_TEMP.ENTITY_DISPLAYNAME";

            public const string MktPromotionImport = "IMPORT.MKT_PROMOTION";

            public const string MktProductListImport = "IMPORT.MKT_PRODUCT_LIST";

            public const string Invoice = "INVOICE.PLURAL_NAME";

            public const string EInvoice = "EINVOICE.ENTITY_DISPLAY_NAME";

            public const string EInvoiceTemplate = "EINVOICE_TEMPLATE.ENTITY_DISPLAY_NAME";

            public const string EInvoiceGateway = "EINVOICE_GATEWAY.ENTITY_DISPLAY_NAME";

            public const string EInvoiceCertificate = "EINVOICE_CERTIFICATE.ENTITY_DISPLAY_NAME";

            public const string EInvoiceEvent = "EINVOICE_EVENT.ENTITY_DISPLAY_NAME";

            public const string Delivery = "DELIVERY.ENTITY_DISPLAY_NAME";

            public const string Promotion = "MKT.PROMOTION.ENTITY_DISPLAYNAME";

            public const string PromotionItem = "MKT.PROMOTION_ITEM.ENTITY_DISPLAYNAME";

            public const string Order = "ORDER.ENTITY_DISPLAYNAME";

            public const string Shipment = "SHIPMENT.ENTITY_DISPLAYNAME";

            public const string DispatchNote = "DISPATCH_NOTE.ENTITY_DISPLAY_NAME";

            public const string EAcctEvent = "EACCT_EVENT.ENTITY_DISPLAY_NAME";

            public const string EAcctGateway = "EACCT_GATEWAY.ENTITY_DISPLAY_NAME";

            public const string Department = "DEPARTMENT.ENTITY_DISPLAY_NAME";

            public const string Vehicle = "VEHICLE.ENTITY_DISPLAYNAME";

            public const string User = "USER.ENTITY_DISPLAYNAME";

            public const string Warehouse = "WAREHOUSE.ENTITY_DISPLAYNAME";

            public const string Bank = "BANK.ENTITY_DISPLAYNAME";

            public const string BankAccount = "BANK_ACCOUNT.ENTITY_DISPLAYNAME";

            public const string InventoryDocument = "DOCUMENT.ENTITY_DISPLAYNAME";

            public const string Party = "PARTY.ENTITY_DISPLAYNAME";

            public const string PartyRelationship = "PARTY_RELATIONSHIP.ENTITY_DISPLAYNAME";

            public const string ArDoc = "AR_DOC.ENTITY_DISPLAYNAME";

            public const string Receipt = "AR.RECEIPT.ENTITY_DISPLAYNAME";

            public const string ReceiptApply = "RECEIPT_APPLY.ENTITY_DISPLAY_NAME";

            public const string AutoNumber = "AUTO_NUMBER.ENTITY_DISPLAYNAME";

            public const string Lookup = "LOOKUP.ENTITY_DISPLAYNAME";

            public const string ProductExt = "PRODUCT_EXT.ENTITY_DISPLAYNAME";

            public const string Province = "PROVINCE.ENTITY_DISPLAYNAME";

            public const string District = "DISTRICT.ENTITY_DISPLAYNAME";

            public const string Ward = "WARD.ENTITY_DISPLAYNAME";

            public const string PriceRequest = "PRICEREQUEST.ENTITY_DISPLAYNAME";

            public const string DeliveryTimetable = "DELIVERY_TIMETABLE.ENTITY_DISPLAYNAME";

            public const string DailyBusinessExpense = "DAILY_BUSINESS_EXPENSE.ENTITY_DISPLAYNAME";

            public const string UserNotSetupCorrectly = "BACKEND.USER.NOT_SETUP_CORRECTLY";

            public const string BudgetDocAutoCreate = "BACKEND.FLOW.BUDGET_DOC_AUTO_CREATE";

            public const string CashTranAutoCreate = "BACKEND.FLOW.CASH_TRAN_AUTO_CREATE";

            public const string CustomerPricingMassDelete = "BACKEND.ACTION.CUSTOMER_PRICING_MASS_DELETE";

            public const string SendBackgroundJobSuccessed = "BACKEND.APP_MESSAGE.SEND_BACKGROUND_JOB_SUCCESSED";

            public const string DeliveryCancel = "DEVICE_LOCATION.EVENT.DELIVERY_CANCEL";

            public const string AddShipmentToDelivery = "DEVICE_LOCATION.EVENT.SHIPMENT_ADDTOEXISTDELIVERY";

            public const string ShipmentTransfer = "DEVICE_LOCATION.EVENT.SHIPMENT_TRANSFER";

            public const string ShipmentCancel = "DEVICE_LOCATION.EVENT.SHIPMENT_CANCEL";

            public const string TransferInventory = "BACKEND.ACTION.TRANSFER_INVENTORY";

            public const string CreateTransferInventoryDocument = "INVENTORY_DOCUMENT.CREATE_TRANSFER_TITLE";

            public const string CreateReceiptInventoryDocument = "INVENTORY_DOCUMENT.CREATE_RECEIPT_TITLE";

            public const string CreateIssueInventoryDocument = "INVENTORY_DOCUMENT.CREATE_ISSUE_TITLE";

            public const string CreateAdjustmentInventoryDocument = "INVENTORY_DOCUMENT.CREATE_ADJUSTMENT_TITLE";

            public const string CreateStockTakeInventoryDocument = "INVENTORY_DOCUMENT.CREATE_STOCK_TAKE_TITLE";

            public const string DispatchNoteSendEStockout = "DISPATCH_NOTE.SEND_ESTOCKOUT";

            public const string DispatchNotePreviewEStockout = "DISPATCH_NOTE.PREVIEW_ESTOCKOUT";

            public const string ShipmentConfReciptAmount = "SHIPMENT.CONFIRM_RECEIPT_AMOUNT";

            public const string Update = "COMMON.BTN_UPDATE";

            public const string CreateReceipt = "RECEIPT.FORM_ADD_TITLE";

            public const string UpdateReceipt = "RECEIPT.FORM_EDIT_TITLE";

            public const string CannotDeleteSelfRefPartyRelationship = "BACKEND.VALIDATION.MESSAGE.SELF_PARTY_RELATIONSHIP_CANNOT_DELETE";

            public const string CannotUpdateSelfRefPartyRelationship = "BACKEND.VALIDATION.MESSAGE.SELF_PARTY_RELATIONSHIP_CANNOT_UPDATE";

            public const string ExistPartyRelationshipAddress = "BACKEND.VALIDATION.MESSAGE.EXIST_PARTY_RELATIONSHIP_ADDRESS";

            public const string PartyRelationshipRequestCannotCreateForCustomerWithoutCode = "BACKEND.PARTY_RELATIONSHIP_REQUEST_CANNOT_CREATE_FOR_CUSTOMER_WITHOUT_CODE";

            public const string SubPartyAlreadyRelationshipWithAnotherCustomer = "BACKEND.VALIDATION.MESSAGE.ALREADY_RELATIONSHIP_WITH_ANOTHER_CUSTOMER";

            public const string PublisherDescription = "PRICEREQUEST.FIELD_DESCRIPTION";

            public const string ApprovedByDescription = "PRICEREQUEST.FIELD_DESCRIPTION2";

            public const string MessageCannotAssignTransportOrgWithParcelStatus = "BACKEND.MESSAGE_CANNOT_ASSIGN_TRANSPORT_ORG_WITH_PARCEL_STATUS";

            public const string MessageCannotReAssignParcelPostOrganization = "BACKEND.MESSAGE_CANNOT_REASSIGN_PARCEL_ORGANIZATION";
        }
    }
}
