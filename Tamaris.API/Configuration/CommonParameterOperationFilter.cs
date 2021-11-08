using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace Tamaris.API.Configuration
{
	public class CommonParameterOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			if (operation.Parameters == null) 
				operation.Parameters = new List<OpenApiParameter>();

			var isControllerQualified = IsControllerQualified(context.ApiDescription.RelativePath);

			if (isControllerQualified)
			{
;
			}
		}

		/// <summary>
		/// We decide if the controller is qualified by the group it belongs to.
		/// If another logic is reqired, this is the place to adopt it.
		/// </summary>
		private bool IsControllerQualified(string relativePath)
		{
			if (string.IsNullOrEmpty(relativePath))
				return false;

			var routeElements = relativePath.Split('/');

			if (routeElements.Length < 4)
				return false;

			// Fourth part of the route is the group name
			var groupName = routeElements[3].ToLower();

			string[] groups = { "admin", "msg" };
			if (groups.Any(g => g.Equals(groupName)))
				return true;

			return false;
		}
	}
}