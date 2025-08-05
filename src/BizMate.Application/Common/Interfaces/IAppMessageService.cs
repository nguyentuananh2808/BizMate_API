namespace BizMate.Application.Common.Interfaces
{
    public interface IAppMessageService
    {
        string NotExist(object value);
        string AlreadyExist(object value);
        string OtpNotExist();
        string OtpInvalid();
        string Invalid(object value);
        string RequiredField(string fieldName);
        string MaxLength(string fieldName, int maxLength);
        string MinLength(string fieldName, int minLength);
        string Unauthorized();
        string Forbidden();
        string UnexpectedError();
        string Conflict(string message);
        string NotEmpty(string fieldName);
        string DuplicateData(string fieldName);
        string ConcurrencyConflict();
    }
}
