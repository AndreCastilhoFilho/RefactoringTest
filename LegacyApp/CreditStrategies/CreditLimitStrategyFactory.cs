using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LegacyApp.CreditStrategies
{
    public class CreditLimitStrategyFactory
    {
        private readonly IReadOnlyDictionary<string, ICreditLimitStrategy> _creditLimitStrategies;

        public CreditLimitStrategyFactory(IUserCreditService userCreditService)
        {
            var creditLimitStrategyType = typeof(ICreditLimitStrategy);
            _creditLimitStrategies = creditLimitStrategyType.Assembly.ExportedTypes
                .Where(x => creditLimitStrategyType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x =>
                {
                    var parameterlessCtor = x.GetConstructors().SingleOrDefault(c => c.GetParameters().Length == 0);
                    return parameterlessCtor is not null
                    ? Activator.CreateInstance(x)
                    : Activator.CreateInstance(x, userCreditService);
                })
                .Cast<ICreditLimitStrategy>()
                .ToImmutableDictionary(x => x.NameRequirement, x => x);

        }

        public ICreditLimitStrategy GetCreditLimitStrategy(string clientName)
        {
            var factory = _creditLimitStrategies.GetValueOrDefault(clientName);

            return factory ?? _creditLimitStrategies[String.Empty];
        }

    }
}
