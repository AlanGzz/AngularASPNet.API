using System;
using System.Threading.Tasks;
using AngularASPNet.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AngularASPNet.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext context;
        public AuthRepository(DataContext context)
        {
            this.context = context;

        }
        public async Task<Users> Login(string userName, string password)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            if(user == null)
                return null;

            if(!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using  (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                    var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                    for(int i = 0; i < computedHash.Length; i++)
                    {
                        if(computedHash[i]!= passwordHash[i]) return false;
                    }
            }
            return true;
        }

        public async Task<Users> Register(Users user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using  (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                     passwordSalt = hmac.Key;
                    passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            
        }

        public async Task<bool> UserExists(string userName)
        {
            if(await context.Users.AnyAsync(x=> x.UserName == userName))
                return true;

            return false;
        }
    }
}