using Examination_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Data.Repositories
{
    public class UsersRepo:IService<User>
    {
        public Exam_sysContext _dbContext;

        public UsersRepo(Exam_sysContext _dbContext)
        {
            this._dbContext = _dbContext;
        }

        public async Task<User> Create(User entity)
        {         
            await _dbContext.Users.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return await _dbContext.Users
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User> GetById(int id)
        {
            return await _dbContext.Users
                .FirstOrDefaultAsync(u => u.id == id);
        }

        public async Task<User> Update(int id, User entity)
        {
            var existingUser = await _dbContext.Users.FindAsync(id);
            if (existingUser == null)
                return null;

            _dbContext.Entry(existingUser).CurrentValues.SetValues(entity);
            await _dbContext.SaveChangesAsync();
            return existingUser;
        }

        public async Task<bool> Delete(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
                return false;

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}
