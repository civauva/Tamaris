using System;


namespace Tamaris.API.Infrastructure.Logging
{
	public static class LogFrog
	{
		public static void Error(string message)
		{
			Serilog.Log.Error(message);
		}

		public static void Error(Exception ex, string message)
		{
			Serilog.Log.Error(message, ex);
		}

		public static void Fatal(Exception ex, string message)
		{
			Serilog.Log.Fatal(ex, message);
		}

		public static void Information(string message)
		{
			Serilog.Log.Information(message);
		}

		public static void Verbose(string messageTemplate, params object[] propertyValues)
		{
            try
            {
				Serilog.Log.Verbose(messageTemplate, propertyValues);
			}
			catch(Exception ex)
            {
				var a = ex;
            }
		}
	}
}
