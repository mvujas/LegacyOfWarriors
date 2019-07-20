using GameServer.Repositories;
using System.Text.RegularExpressions;

namespace GameServer.Logic.Validation
{
    public static class UserValidator
    {
        #region CONSTS
        private const int MINIMUM_PASSWORD_LENGTH = 8;
        #endregion

        #region REGEX PATTERNS
        private static Regex usernamePattern = new Regex(
            @"^[-._\w\d]{4,32}$", 
            RegexOptions.Compiled);
        #endregion

        private static UserRepository userRepository = new UserRepository();

        public static void ValidateUsername(string username)
        {
            if(!usernamePattern.IsMatch(username))
            {
                throw new InvalidLogicDataException("Nevalidno korisnicko ime!");
            }
            if(userRepository.DoesUserExist(username))
            {
                throw new InvalidLogicDataException(
                    "Vec postoji korisnik sa datim korisnickim imenom");
            }
        }

        public static void ValidatePassword(string password)
        {
            if(password.Length < MINIMUM_PASSWORD_LENGTH)
            {
                throw new InvalidLogicDataException(
                    "Lozinka mora biti duzine barem " + MINIMUM_PASSWORD_LENGTH);
            }
            bool digit = false, 
                bigLetter = false,
                smallLetter = false;
            foreach(char c in password)
            {
                if(c == ' ')
                {
                    throw new InvalidLogicDataException("Lozinka ne sme imati razmake");
                }
                digit |= (c >= '0' && c <= '9');
                bigLetter |= (c >= 'A' && c <= 'Z');
                smallLetter |= (c >= 'a' && c <= 'z');
            }
            if((digit ? 1 : 0) + (bigLetter ? 1 : 0) + (smallLetter ? 1 : 0) < 2)
            {
                throw new InvalidLogicDataException(
                    "Lozinka mora imati bar dva od ponudjenog: malo slovo, veliko slovo, cifra");
            }
        }

        public static void ValidateUserData(string username, string password)
        {
            ValidateUsername(username);
            ValidatePassword(password);
        }
    }
}
