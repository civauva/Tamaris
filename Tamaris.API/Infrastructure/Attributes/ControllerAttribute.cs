using System;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Tamaris.API.Infrastructure.Attributes
{
	public class TamarisControllerAttribute : Attribute, IRouteTemplateProvider
    {
        public string Endpoint
        {
            get { return ""; }
            set 
            {
                var template = $"/api/v1/{value}[controller]";
                Template = template;
            }
        }

        public string Template { get; set; }
        public int? Order => 1;
        public string Name { get; set; }
    }
}
