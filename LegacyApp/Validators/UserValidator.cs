using LegacyApp.Models;
using LegacyApp.Services;
using System;

namespace LegacyApp.Validators
{
    public class UserValidator
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        public UserValidator(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        public bool HasValidEmail(string email)
        {
            return email.Contains("@") || email.Contains(".");
        }

        public bool HasValidFullName(string firname, string surname)
        {
            return !string.IsNullOrEmpty(firname) && !string.IsNullOrEmpty(surname);
        }

        public bool IsUserAtLeast21YearsOld(DateTime dateOfBirth)
        {
            var now = _dateTimeProvider.Now;
            var age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;

            return age >= 21;
        }

        public bool HasCreditLimitAndLimitIsLessThan500(User user)
        {
            return user.HasCreditLimit && user.CreditLimit < 500;
        }

    }
}
