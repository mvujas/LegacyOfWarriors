using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Repositories;
using GameServer.Logic.Validation;
using GameServer.Model;
using DevOne.Security.Cryptography.BCrypt;
using MySql.Data.MySqlClient;

namespace GameServer.Logic
{
    public static class UserLogic
    {
        private static UserRepository userRepo = new UserRepository();

        /// <exception cref="GameServer.Logic.Validation.InvalidLogicDataException">
        /// Baca se ako su podaci nevalidni
        /// </exception>
        public static void RegisterUser(string username, string password)
        {
            UserValidator.ValidateUserData(username, password);
            string salt = BCryptHelper.GenerateSalt();
            User user = new User
            {
                Username = username,
                PasswordHash = BCryptHelper.HashPassword(password, salt)
            };
            try
            {
                userRepo.Add(user);
            }
            catch (MySqlException e)
            {
                if (e.IsDuplicateEntry())
                {
                    throw new InvalidLogicDataException(
                        "Vec postoji korisnik sa datim korisnickim imenom");
                }
            }
        }

        /// <exception cref="GameServer.Logic.Validation.InvalidLogicDataException">
        /// Baca se ako podaci za prijavljivanje nisu tacni
        /// </exception>
        public static User GetUserByLoginInfo(string username, string password)
        {
            var user = userRepo.GetByUsername(username);
            if(user == null)
            {
                throw new InvalidLogicDataException(
                    "Ne postoji korisnik pod datim korisnickim imenom");
            }
            if(!BCryptHelper.CheckPassword(password, user.PasswordHash))
            {
                throw new InvalidLogicDataException("Pogresna lozinka");
            }
            return user;
        }

        public static User GetUserById(long id)
        {
            return userRepo.GetById(id);
        }
    }
}
