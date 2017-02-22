using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTMaker
{
    static class DateTimeExtension
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1);

        public static long ToUnixTime(this DateTime dateTime)
        {
            return (dateTime - UnixEpoch).Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static DateTime FromUnixTime(this long milliseconds)
        {
            return UnixEpoch.AddTicks(milliseconds*TimeSpan.TicksPerMillisecond);
        }
    }
}
