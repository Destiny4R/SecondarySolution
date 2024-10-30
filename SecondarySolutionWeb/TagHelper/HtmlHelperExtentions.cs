using Microsoft.AspNetCore.Mvc.Rendering;

namespace SecondarySolutionWeb.TagHelper
{
	public static class HtmlHelperExtentions
	{
		public static string ActiveClass(this IHtmlHelper htmlHelper, string route)
		{
			var routeData = htmlHelper.ViewContext.RouteData;

			var pageRoute = routeData.Values["page"].ToString();

			return route == pageRoute ? "active text-dark" : "";
		}
	}
}
