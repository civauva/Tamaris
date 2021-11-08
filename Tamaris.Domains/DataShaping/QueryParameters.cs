using System.ComponentModel;

namespace Tamaris.Domains.DataShaping
{
	public class QueryParameters
	{
		[DefaultValue("1")]
		[Description("<b>Page index</b>. What page you want to render.")]
		public int PageIndex { get; set; }

		[DefaultValue("20")]
		[Description("<b>Page size</b>. How many calls do you want to fetch per page.")]
		public int PageSize { get; set; }

		[Description("Fields used for sorting. Usage: FieldName [desc]. You can specify additional fields and divide them with comma.")]
		public string? OrderBy { get; set; }

		// [Description("Search anything in the given list. Search is performed against all searchable fields.")]
		// public string? SearchString { get; set; }
	}
}