using System;
using System.Linq;
using System.Reflection;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Tamaris.Domains.DataShaping;
using Tamaris.API.Infrastructure;
using Tamaris.API.Infrastructure.Logging;


namespace Tamaris.API.Controllers
{
	public class BaseController: Controller
	{
		protected IActionResult HandleException(Exception ex, string msg)
		{
			IActionResult ret;

			// Create new exception with generic message        
			ret = StatusCode(StatusCodes.Status500InternalServerError, new Exception(msg, ex));
			LogFrog.Fatal(ex, msg);

			return ret;
		}


		/// <summary>
		/// Creates pagination header that the client can use to navigate through paginated list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="paginatedList">Parameter that contains all neccessary information</param>
		protected void CreatePagingHeader<T>(ref PaginatedList<T> paginatedList)
		{
			if (paginatedList.PageSize > 0)
			{
				var urlPath = this.ControllerContext.HttpContext.Request.Path.HasValue ? this.ControllerContext.HttpContext.Request.Path.Value : "";
				var template = urlPath + "?pageIndex={0}&pageSize={1}";

				var paginationMetadata = new
				{
					totalCount = paginatedList.TotalCount,
					pageSize = paginatedList.PageSize,
					currentPage = paginatedList.CurrentPage,
					totalPages = paginatedList.TotalPages,
					hasPreviousPage = paginatedList.HasPreviousPage,
					hasNextPage = paginatedList.HasNextPage,
					previousPageLink = paginatedList.HasPreviousPage ? string.Format(template, paginatedList.CurrentPage - 1, paginatedList.PageSize) : "",
					nextPageLink = paginatedList.HasNextPage ? string.Format(template, paginatedList.CurrentPage + 1, paginatedList.PageSize) : ""
				};

				Response.Headers.Add("X-Pagination",
					Newtonsoft.Json.JsonConvert.SerializeObject(paginationMetadata));
			}
		}		


		/// <summary>
		/// Used only in conjuction with Authorization and Policy
		/// </summary>
		internal int? UserId => this.User?.Claims?.FirstOrDefault(i => i.Type == "UserId") != null ? int.Parse(this.User.Claims.FirstOrDefault(i => i.Type == "UserId").Value) : null;

		internal string UserIp
		{
			get
			{
				if (Request.Headers.ContainsKey("X-Forwarded-For"))
					return Request.Headers["X-Forwarded-For"];
				else
					return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
			}
		}


		/// <summary>
		/// This method is certainly not the speed champion, but it provides us with the possibility to write something like this:
		/// TrySetProperty(contact, "CreatedOn", DateTime.Now);
		/// </summary>
		private bool TrySetProperty(object obj, string property, object value) 
		{
			var prop = obj.GetType().GetProperty(property, BindingFlags.Public | BindingFlags.Instance);
			if(prop != null && prop.CanWrite) 
			{
				prop.SetValue(obj, value, null);
				return true;
			}

			return false;
		}


		/// <summary>
		/// </summary>
		internal void ManipulateOnCreate(object obj)
		{

		}


		/// <summary>
		/// </summary>
		internal void ManipulateOnUpdate(object obj)
		{

		}



		#region Header variables
		// No header variables in the project		
		#endregion Header variables



		#region Logging
		private string CallerMethodName => ReflectionHelper.GetCurrentMethod(3);

		protected void LogVerbose(string logMessage)
		{
			LogFrog.Verbose(logMessage);
		}

		protected void LogInformation(string logMessage)
		{
			LogFrog.Information(logMessage);
		}

		protected void LogError(string logMessage, Exception ex)
		{
			LogFrog.Error(ex, logMessage);
		}

		private const string LogTemplateGetSingle = "Method: {RequestMethod} // Step: {MethodStep} // User id: {UserId} // IP Address: {UserIp} // Entity id: {EntityId}.";
		private const string LogTemplateGetList = "Method: {RequestMethod} // Step: {MethodStep} // User id: {UserId} // IP Address: {UserIp} // Page index: {pageIndex}, page size: {pageSize}, search string \"{searchString}\".";
		private const string LogTemplateCreate = "Method: {RequestMethod} // Step: {MethodStep} // User id: {UserId} // IP Address: {UserIp} // Entity: {@Entity}.";
		private const string LogTemplateCreateId = "Method: {RequestMethod} // Step: {MethodStep} // User id: {UserId} // IP Address: {UserIp} // Entity id: {EntityId}.";
		private const string LogTemplateUpdate = "Method: {RequestMethod} // Step: {MethodStep} // User id: {UserId} // IP Address: {UserIp} // Entity id: {EntityId} // Entity: {@Entity}.";
		private const string LogTemplateUpdateId = "Method: {RequestMethod} // Step: {MethodStep} // User id: {UserId} // IP Address: {UserIp} // Entity id: {EntityId}.";
		private const string LogTemplateDelete = "Method: {RequestMethod} // Step: {MethodStep} // User id: {UserId} // IP Address: {UserIp} // Entity id: {EntityId}.";



		#region Single primary key
		#region Getter - single
		protected void LogMethodSingleGetterEntry(object EntityId)
		{
			LogFrog.Verbose(LogTemplateGetSingle, "GET SINGLE", "Entry", UserId, UserIp, EntityId);
		}

		protected void LogMethodSingleGetterData(object EntityId)
		{
			LogFrog.Verbose(LogTemplateGetSingle, "GET SINGLE", "Delivers data", UserId, UserIp, EntityId);
		}

		protected void LogMethodSingleGetterNoData(object EntityId)
		{
			LogFrog.Verbose(LogTemplateGetSingle, "GET SINGLE", "No data", UserId, UserIp, EntityId);
		}
		#endregion Getter - single


		#region Getter - list
		protected void LogMethodListGetterEntry(QueryParameters parameters, string searchString)
		{
			LogFrog.Verbose(LogTemplateGetList, "GET LIST", "Entry", UserId, UserIp, parameters.PageIndex, parameters.PageSize, searchString);
		}

		protected void LogMethodListGetterData(int count, QueryParameters parameters, string searchString)
		{
			LogFrog.Verbose(LogTemplateGetList, "GET LIST", $"Delivers data ({count} items)", UserId, UserIp, parameters.PageIndex, parameters.PageSize, searchString);
		}

		protected void LogMethodListGetterNoData(QueryParameters parameters, string searchString)
		{
			LogFrog.Verbose(LogTemplateGetList, "GET LIST", "No data", UserId, UserIp, parameters.PageIndex, parameters.PageSize, searchString);
		}

		protected void LogMethodListGetterEntry(int pageIndex, int pageSize, string searchString)
		{
			LogFrog.Verbose(LogTemplateGetList, "GET LIST", "Entry", UserId, UserIp, pageIndex, pageSize, searchString);
		}

		protected void LogMethodListGetterData(int count, int pageIndex, int pageSize, string searchString)
		{
			LogFrog.Verbose(LogTemplateGetList, "GET LIST", $"Delivers data ({count} items)", UserId, UserIp, pageIndex, pageSize, searchString);
		}

		protected void LogMethodListGetterNoData(int pageIndex, int pageSize, string searchString)
		{
			LogFrog.Verbose(LogTemplateGetList, "GET LIST", "No data", UserId, UserIp, pageIndex, pageSize, searchString);
		}
		#endregion Getter - list


		#region Create
		protected void LogMethodCreateEntry(object Entity)
		{
			LogFrog.Verbose(LogTemplateCreate, "CREATE", "Entry", UserId, UserIp, Entity);
		}

		protected void LogMethodCreateBadRequest(object Entity)
		{
			LogFrog.Verbose(LogTemplateCreate, "CREATE", "Emtpy entity", UserId, UserIp, Entity);
		}

		protected void LogMethodCreateInvalid(object Entity)
		{
			LogFrog.Verbose(LogTemplateCreate, "CREATE", "Validation failed", UserId, UserIp, Entity);
		}

		protected void LogMethodCreateSaveSuccessful(object EntityId)
		{
			LogFrog.Verbose(LogTemplateCreateId, "CREATE", "Successful", UserId, UserIp, EntityId);
		}

		protected void LogMethodCreateSaveSuccessful(params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateCreateId, "CREATE", "Successful", UserId, UserIp, EntityId);
		}

		protected void LogMethodCreateSaveFailed(object Entity)
		{
			LogFrog.Verbose(LogTemplateCreate, "CREATE", "Save failed", UserId, UserIp, Entity);
		}
		#endregion Create


		#region Update
		protected void LogMethodUpdateEntry(object EntityId, object Entity)
		{
			LogFrog.Verbose(LogTemplateUpdate, "UPDATE", "Entry", UserId, UserIp, EntityId, Entity);
		}

		protected void LogMethodUpdateBadRequest(object EntityId, object Entity)
		{
			LogFrog.Verbose(LogTemplateUpdate, "UPDATE", "Empty entity", UserId, UserIp, EntityId, Entity);
		}

		protected void LogMethodUpdateInvalid(object EntityId, object Entity)
		{
			LogFrog.Verbose(LogTemplateUpdate, "UPDATE", "Validation failed", UserId, UserIp, EntityId, Entity);
		}

		protected void LogMethodUpdateUpserting(object EntityId)
		{
			LogFrog.Verbose(LogTemplateUpdateId, "UPSERT", "Entry", UserId, UserIp, EntityId);
		}

		protected void LogMethodUpdateUpsertSuccessful(object EntityId)
		{
			LogFrog.Verbose(LogTemplateUpdateId, "UPSERT", "Successful", UserId, UserIp, EntityId);
		}

		protected void LogMethodUpdateSaveSuccessful(object EntityId)
		{
			LogFrog.Verbose(LogTemplateUpdateId, "UPDATE", "Successful", UserId, UserIp, EntityId);
		}

		protected void LogMethodUpdateSaveFailed(object EntityId, object Entity)
		{
			LogFrog.Verbose(LogTemplateUpdate, "UPDATE", "Save failed", UserId, UserIp, EntityId, Entity);
		}

		protected void LogMethodUpdateUpsertFailed(object EntityId, object Entity)
		{
			LogFrog.Verbose(LogTemplateUpdate, "UPSERT", "Save failed", UserId, UserIp, EntityId, Entity);
		}
		#endregion Update


		#region Delete
		protected void LogMethodDeleteEntry(object EntityId)
		{
			LogFrog.Verbose(LogTemplateDelete, "DELETE", "Entry", UserId, UserIp, EntityId);
		}

		protected void LogMethodDeleteNoEntity(object EntityId)
		{
			LogFrog.Verbose(LogTemplateDelete, "DELETE", "No entity", UserId, UserIp, EntityId);
		}

		protected void LogMethodDeleteSuccessful(object EntityId)
		{
			LogFrog.Verbose(LogTemplateDelete, "DELETE", "Successfull", UserId, UserIp, EntityId);
		}

		protected void LogMethodDeleteFailed(object EntityId)
		{
			LogFrog.Verbose(LogTemplateDelete, "DELETE", "Failed", UserId, UserIp, EntityId);
		}
		#endregion Delete
		#endregion Single primary key


		#region Array of primary keys
		#region Getter - single
		protected void LogMethodSingleGetterEntry(params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateGetSingle, "GET SINGLE", "Entry", UserId, UserIp, EntityId);
		}

		protected void LogMethodSingleGetterData(params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateGetSingle, "GET SINGLE", "Delivers data", UserId, UserIp, EntityId);
		}

		protected void LogMethodSingleGetterNoData(params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateGetSingle, "GET SINGLE", "No data", UserId, UserIp, EntityId);
		}
		#endregion Getter - single



		#region Update
		protected void LogMethodUpdateEntry(object Entity, params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateUpdate, "UPDATE", "Entry", UserId, UserIp, EntityId, Entity);
		}

		protected void LogMethodUpdateBadRequest(object Entity, params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateUpdate, "UPDATE", "Empty entity", UserId, UserIp, EntityId, Entity);
		}

		protected void LogMethodUpdateInvalid(object Entity, params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateUpdate, "UPDATE", "Validation failed", UserId, UserIp, EntityId, Entity);
		}

		protected void LogMethodUpdateSaveFailed(object Entity, params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateUpdate, "UPDATE", "Save failed", UserId, UserIp, EntityId, Entity);
		}

		protected void LogMethodUpdateUpsertFailed(object Entity, params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateUpdate, "UPSERT", "Save failed", UserId, UserIp, EntityId, Entity);
		}

		protected void LogMethodUpdateUpserting(params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateUpdateId, "UPSERT", "Entry", UserId, UserIp, EntityId);
		}

		protected void LogMethodUpdateUpsertSuccessful(params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateUpdateId, "UPSERT", "Successful", UserId, UserIp, EntityId);
		}

		protected void LogMethodUpdateSaveSuccessful(params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateUpdateId, "UPDATE", "Successful", UserId, UserIp, EntityId);
		}

		#endregion Update


		#region Delete
		protected void LogMethodDeleteEntry(params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateDelete, "DELETE", "Entry", UserId, UserIp, EntityId);
		}

		protected void LogMethodDeleteNoEntity(params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateDelete, "DELETE", "No entity", UserId, UserIp, EntityId);
		}

		protected void LogMethodDeleteSuccessful(params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateDelete, "DELETE", "Successfull", UserId, UserIp, EntityId);
		}

		protected void LogMethodDeleteFailed(params object[] EntityId)
		{
			LogFrog.Verbose(LogTemplateDelete, "DELETE", "Failed", UserId, UserIp, EntityId);
		}
		#endregion Delete
		#endregion Array of primary keys

		#endregion Logging

	}
}
