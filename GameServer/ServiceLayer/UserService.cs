using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Model;
using GameServer.DataAccessLayer;

namespace GameServer.ServiceLayer
{
    public static class UserService
    {
        public static User VerifyUser(string username, string password)
        {
            User user = UserDao.GetInstance().GetUserByUsername(username);
            if(user == null)
            {
                throw new UserLoginRegistrationException("Ne postoji korisnik pod datim username-om");
            }
            
            if(!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new UserLoginRegistrationException("Pogresna lozinka");
            }

            return user;
        }

        private static int MINIMUM_PASSWORD_LENGTH = 8;
        private static void CheckPasswordValidity(string password)
        {
            string exceptionMessage = null;
            if (password.Length < MINIMUM_PASSWORD_LENGTH)
            {
                exceptionMessage = String.Format("Sifra mora biti duzine barem {0}", MINIMUM_PASSWORD_LENGTH);
            }
            else if (password.Contains(" "))
            {
                exceptionMessage = "Sifra ne sme sadrzati razmake";
            }

            if(exceptionMessage != null)
            {
                throw new UserLoginRegistrationException(exceptionMessage);
            }
        }

        private static int MINIMUM_USERNAME_LENGTH = 5;
        private static void CheckUsernameValidity(string username)
        {
            string exceptionMessage = null;
            if (username.Length < MINIMUM_USERNAME_LENGTH)
            {
                exceptionMessage = String.Format("Username mora biti duzine barem {0}", MINIMUM_USERNAME_LENGTH);
            }
            else if (username.Contains(" "))
            {
                exceptionMessage = "Username ne sme sadrzati razmake";
            }
            else if (UserDao.GetInstance().GetUserByUsername(username) != null)
            {
                exceptionMessage = "Vec postoji korisnik sa unetim username-om";
            }

            if (exceptionMessage != null)
            {
                throw new UserLoginRegistrationException(exceptionMessage);
            }
        }

        public static void RegisterUser(string username, string password)
        {
            if(username == null || password == null)
            {
                throw new ArgumentException("Username and password cannot be null");
            }

            username = username.Trim();
            CheckUsernameValidity(username);
            password = password.Trim();
            CheckPasswordValidity(password);

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            Boolean ok = UserDao.GetInstance().AddUser(username, passwordHash);
            if(!ok)
            {
                throw new UserLoginRegistrationException(
                    "Doslo je do greske prilikom unosa korisnika u bazu, pokusajte kasnije");
            }
        }
    }
}
