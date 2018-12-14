namespace AngularASPNet.API.Data

{
    using System.Threading.Tasks;
    using AngularASPNet.API.Models;
    public interface IAuthRepository
    {
         Task<Users> Register(Users user, string password);
         Task<Users> Login(string userName, string password);
         Task<bool> UserExists(string userName);
     }
}