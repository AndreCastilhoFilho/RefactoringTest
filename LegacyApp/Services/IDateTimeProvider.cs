using System;

namespace LegacyApp.Services
{
    public interface IDateTimeProvider
    {
        public DateTime Now { get; }

    }
}
