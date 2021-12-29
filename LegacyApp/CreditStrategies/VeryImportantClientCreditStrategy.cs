using LegacyApp.Models;

namespace LegacyApp.CreditStrategies
{
    public class VeryImportantClientCreditStrategy : ICreditLimitStrategy
    {
        public string NameRequirement => "VeryImportantClient";

        public (bool HasCreditLimit, int CreditLimit) GetCreditLimit(User user)
        {
            return (false, 0);
        }
    }
}
