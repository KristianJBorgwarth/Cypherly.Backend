using System.Text.RegularExpressions;
using Cypherly.Domain.Common;

namespace Cypherly.Domain.ValueObjects
{
    public class Password : ValueObject
    {
        public string HashedPassword { get; private set; } = null!;

        public Password() { } //For EF Core

        private Password(string hashedPassword)
        {
            HashedPassword = hashedPassword;
        }

        public static Result<Password> Create(string plainPassword)
        {
            try
            {
                IsPasswordValidFormat(plainPassword);
                return Result.Ok(new Password(BCrypt.Net.BCrypt.HashPassword(plainPassword)));
            }
            catch(Exception ex)
            {
                return Result.Fail<Password>(Errors.General.UnspecifiedError(ex.Message));
            }
        }
        public bool Verify(string plainPassword)
        {
            return BCrypt.Net.BCrypt.Verify(plainPassword, HashedPassword);
        }

        /// <summary>
        /// Validates the password format:
        /// Must be between 8 and 18 characters.
        /// Must contain at least one uppercase letter
        /// Must contain at least one lowercase letter
        /// Must contain at least one special character
        /// Must not contain any spaces
        /// </summary>
        /// <param name="pw">the password being validated</param>
        /// <returns>boolean value representing the validity of password</returns>
        private static void IsPasswordValidFormat(string pw)
        {
            if (!Regex.IsMatch(pw, @"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[\W_]).{8,18}$"))
            {
                throw new ArgumentException(
                    "Incorrect password: Must be between 8 and 18 characters, contain one uppercase letter, one lowercase letter, one special character, and no spaces.");
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return HashedPassword;
        }
    }
}