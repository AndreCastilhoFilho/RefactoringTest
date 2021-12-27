using LegacyApp.Models;

namespace LegacyApp.DataAccess
{
    public class UserDataAccessProxy : IUserDataAccess
    {
        public void Add(User user)
        {
            UserDataAccess.AddUser(user);
        }
    }
}
