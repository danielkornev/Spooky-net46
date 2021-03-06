﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Spooky
{
	internal static class GuardExtensions
	{

		internal static void GuardNull(this object value, string argumentName)
		{
			if (value == null) throw new ArgumentNullException(argumentName);
		}

		internal static void GuardNullOrEmpty(this string value, string argumentName)
		{
			if (value == null) throw new ArgumentNullException(argumentName);
			if (value.Length == 0) throw new ArgumentException(argumentName);
		}
	}
}
