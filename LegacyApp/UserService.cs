using LegacyApp.CreditStrategies;
using LegacyApp.DataAccess;
using LegacyApp.Models;
using LegacyApp.Repositories;
using LegacyApp.Services;
using LegacyApp.Validators;
using System;

namespace LegacyApp
{
    public class UserService
    {

        private readonly IClientRepository _clientRepository;
        private readonly IUserDataAccess _userDataAccess;
        private readonly UserValidator _userValidator;
        private readonly CreditLimitStrategyFactory _creditLimitStrategyFactory;

        public UserService() :
            this(
                new ClientRepository(),
                new UserDataAccessProxy(),
                new UserValidator(new DateTimeProvider()),
                new CreditLimitStrategyFactory(new UserCreditServiceClient()))
        {
        }

        public UserService(
            IClientRepository clientRepository,
            IUserDataAccess userDataAccess,
            UserValidator userValidator,
            CreditLimitStrategyFactory creditLimitStrategyFactory)
        {

            _clientRepository = clientRepository;
            _userDataAccess = userDataAccess;
            _userValidator = userValidator;
            _creditLimitStrategyFactory = creditLimitStrategyFactory;
        }

        public bool AddUser(string firname, string surname, string email, DateTime dateOfBirth, int clientId)
        {
            if (!UserProvideDataIsValid(firname, surname, email, dateOfBirth))
            {
                return false;
            }

            var client = _clientRepository.GetById(clientId);

            var user = new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                Firstname = firname,
                Surname = surname
            };

            ApplyCreditLimit(client, user);

            if (_userValidator.HasCreditLimitAndLimitIsLessThan500(user))
            {
                return false;
            }

            _userDataAccess.Add(user);
            return true;
        }

        private void ApplyCreditLimit(Client client, User user)
        {
            var strategy = _creditLimitStrategyFactory.GetCreditLimitStrategy(client.Name);
            var (hasCreditLimit, creditLimit) = strategy.GetCreditLimit(user);

            user.HasCreditLimit = hasCreditLimit;
            user.CreditLimit = creditLimit;
        }

        public bool UserProvideDataIsValid(string firname, string surname, string email, DateTime dateOfBirth)
        {
            if (!_userValidator.HasValidFullName(firname, surname))
            {
                return false;
            }

            if (!_userValidator.HasValidEmail(email))
            {
                return false;
            }

            if (!_userValidator.IsUserAtLeast21YearsOld(dateOfBirth))
            {
                return false;
            }

            return true;
        }


    }
}
