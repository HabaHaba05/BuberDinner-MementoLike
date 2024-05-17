namespace BuberDinner.Application.Common.Interfaces.Persistence;
public interface IUnitOfWork : IDisposable
{
    IAdminRepository AdminRepository { get; }
    IUserRepository UserRepository { get; }
    IBecomeHostRequestRepository BecomeHostRequestRepository { get; }
    IGuestRepository GuestRepository { get; }
    IHostRepository HostRepository { get; }
    IDinnerRepository DinnerRepository { get; }
    IMenuRepository MenuRepository { get; }
    IMenuReviewRepository MenuReviewRepository { get; }
    IBillRepository BillRepository { get; }

    void Begin();
    void Commit();
    void Rollback();
}