using System;

namespace Mono.Unix.Native
{
	[Map]
	[CLSCompliant(false)]
	public enum SyslogLevel
	{
		LOG_EMERG = 0,
		LOG_ALERT = 1,
		LOG_CRIT = 2,
		LOG_ERR = 3,
		LOG_WARNING = 4,
		LOG_NOTICE = 5,
		LOG_INFO = 6,
		LOG_DEBUG = 7
	}
}
