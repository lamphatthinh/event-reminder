using System;
using EventReminder.Application.Abstractions.Common;

namespace EventReminder.Infrastructure.Common
{
    /// <summary>
    /// Represents the machine date time service.
    /// </summary>
    internal sealed class MachineDateTime : IDateTime
    {
        private static readonly TimeZoneInfo ApplicationTimeZone = GetApplicationTimeZone();

        /// <inheritdoc />
        public DateTime UtcNow => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, ApplicationTimeZone);

        private static TimeZoneInfo GetApplicationTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
            }
        }
    }
}
