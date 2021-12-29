using LegacyApp.Models;

namespace LegacyApp.CreditStrategies
{
    public class ImportantClientCreditLimitStrategy : ICreditLimitStrategy
    {
        public string NameRequirement => "ImportantClient";
        private readonly IUserCreditService _userCreditService;

        public ImportantClientCreditLimitStrategy(IUserCreditService userCreditService)
        {
            _userCreditService = userCreditService;
        }

        public (bool HasCreditLimit, int CreditLimit) GetCreditLimit(User user)
        {
            user.HasCreditLimit = true;

            var creditLimit = _userCreditService.GetCreditLimit(user.Firstname, user.Surname, user.DateOfBirth);

            return (true, creditLimit * 2);
        }
    }
}
