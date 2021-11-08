using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Tamaris.API.Infrastructure
{
	public static class ReflectionHelper
	{
		/// <summary>
		/// Delivers fully qualified name of the calling method based upon the stackFrameIndex
		/// </summary>
		/// <param name="stackFrameIndex">Depth of the stack. For example if you expect that the caller is on the 3rd level from this method, you should pass here number 3.</param>
		/// <returns>Name of the caller method.</returns>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static string GetCurrentMethod(int stackFrameIndex)
		{
			var st = new StackTrace();
			var sf = st.GetFrame(stackFrameIndex);
			var method = sf.GetMethod();
			var methodName = method.DeclaringType.FullName;

			return methodName;
		}
	}
}