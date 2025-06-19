namespace BizMate.Application.Common.Interfaces
{
    /// <summary>
    /// OutputPort interface: Cho phép UseCase gửi kết quả ra Presenter.
    /// </summary>
    /// <typeparam name="TUseCaseResponse">Kiểu dữ liệu trả ra từ UseCase</typeparam>
    public interface IOutputPort<in TUseCaseResponse>
    {
        void Handle(TUseCaseResponse response);
    }
}
