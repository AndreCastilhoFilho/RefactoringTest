using LegacyApp.Models;

namespace LegacyApp.CreditStrategies
{
    public class DefaultCreditLimitStrategy : ICreditLimitStrategy
    {
        public string NameRequirement => string.Empty;
        private readonly IUserCreditService _userCreditService;

        public DefaultCreditLimitStrategy(IUserCreditService userCreditService)
        {
            _userCreditService = userCreditService;
        }

        public (bool HasCreditLimit, int CreditLimit) GetCreditLimit(User user)
        {
            user.HasCreditLimit = true;
            var creditLimit = _userCreditService.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);

            return (true, creditLimit);
        }
    }
}
