using LibHub.UserService.Domain.Entities;

namespace LibHub.UserService.Data.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> FindByEmailAsync(string email);
    Task<User> AddAsync(User user);
}
