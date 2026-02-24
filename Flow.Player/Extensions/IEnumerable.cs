using System;
using System.Collections.Generic;
using System.Linq;

namespace Flow.Player.Extensions;

public static class IEnumerable
{
	extension<T>(IEnumerable<T>? list)
	{
		public string ToPrintableString<TResult>(Func<T, TResult> selector)
		{
			if (list is null || !list.Any())
				return "[]";
			
			return $"[{string.Join(", ", list.Select(selector))}]";
		}
	}
}