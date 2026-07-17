using BizMate.Application.UserCases.InventoryChat;

namespace BizMate.Application.Common.Interfaces
{
    public interface IInventoryQuestionParser
    {
        ParsedInventoryQuestion Parse(string question);
    }
}
