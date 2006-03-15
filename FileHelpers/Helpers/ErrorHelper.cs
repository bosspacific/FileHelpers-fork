#region "  � Copyright 2005-06 to Marcos Meli - http://www.marcosmeli.com.ar" 

// Errors, suggestions, contributions, send a mail to: marcosdotnet[at]yahoo.com.ar.

#endregion

using System;

namespace FileHelpers
{
	internal sealed class ErrorHelper
	{
		private ErrorHelper()
		{
		}

		public static void CheckNullParam(string param, string paramName)
		{
			if (param == null || param.Length == 0)
				throw new ArgumentException(paramName + " can�t be neither null nor empty", paramName);
		}

		public static void CheckNullParam(object param, string paramName)
		{
			if (param == null)
				throw new ArgumentException(paramName + " can�t be null", paramName);
		}

		public static void CheckDifferentsParams(object param1, string param1Name, object param2, string param2Name)
		{
			if (param1 == param2)
				throw new ArgumentException(param1Name + " can�t be the same that " + param2Name, param1Name + " and " + param2Name);
		}

	}
}