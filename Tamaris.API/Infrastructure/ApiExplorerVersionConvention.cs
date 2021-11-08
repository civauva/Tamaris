using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Tamaris.API.Infrastructure
{
	/// <summary>
	/// This class is used to tell the swagger what class(es) should be grouped within the 
	/// choosen Swagger document. Used in conjuction with SwaggerConfiguration.cs class.
	/// </summary>
	public class ApiExplorerVersionConvention : IControllerModelConvention
	{
		public void Apply(ControllerModel controller)
		{
			var aha = controller.Selectors?.FirstOrDefault()?.AttributeRouteModel?.Template;
			var oldStyle = false; // If we still decide to use the old style of grouping, set this to true

			// Old way of constructing groups
			if (oldStyle || string.IsNullOrEmpty(aha))
			{
				var controllerNamespace = controller.ControllerType.Namespace; // e.g. "Controllers.v1"  
				if (controllerNamespace == null) return;
				var apiVersion = controllerNamespace.Split('.').Last().ToLower();

				controller.ApiExplorer.GroupName = apiVersion;
			}
			// New way of constructing groups
			else
			{
				var apiVersion = aha.Replace("/api/", "").Replace("/[controller]", "").Replace("/", ".").ToLower();
				controller.ApiExplorer.GroupName = apiVersion;
			}
		}
	}
}