using LegacyApp.Models;

namespace LegacyApp.CreditStrategies
{
    public interface ICreditLimitStrategy
    {
        public string NameRequirement { get; }

        (bool HasCreditLimit, int CreditLimit) GetCreditLimit(User user);

    }
}
