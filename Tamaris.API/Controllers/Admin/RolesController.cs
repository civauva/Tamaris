using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using AutoMapper;

using Tamaris.Domains.Admin;
using Tamaris.Domains.DataShaping;
using Tamaris.Entities.Admin;
using Tamaris.DAL.Interfaces;
using Tamaris.API.Infrastructure.Attributes;
using Microsoft.AspNetCore.Identity;

namespace Tamaris.API.Controllers.Admin
{
	[TamarisController(Endpoint = "Admin/")]
	[Produces("application/json")]
	public class RolesController : BaseController
	{
		private readonly ITamarisUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
        private readonly RoleManager<Role> _roleManager;
        private readonly string _defaultGetSingleRoute = "GetAdminsRole";

		public RolesController(ITamarisUnitOfWork unitOfWork, IMapper mapper, RoleManager<Role> roleManager)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_roleManager = roleManager;
		}

 
        #region Getting data
        // GET
        /// <summary>
        /// Gets paging list with all roles
        /// </summary>
        /// <remarks>With pagination, you can optimize fetching the roles. For example, you can fetch 100 pages with 10 pages per page or 10 pages with 100 roles per page.
        /// Additionally, if you opt to use the pagination (you provide the values for pageIndex and pageSize), this method will also include X-Pagination header
        /// with additional information about the result like totalPages, currentPage, next-/previous link, has next-/previous link.
        /// </remarks>
        /// <param name="parameters">Query parameters</param>
        /// <param name="searchString">Search anything in the given roles list. Search is performed against all searchable fields.</param>
        /// <param name="cancellationToken">Token used to explicitly cancel the request.</param>

        /// <returns></returns>
        /// <response code="200">Returns list of selected roles</response>
        /// <response code="204">If there are no roles for given PageIndex/PageSize/SearchString combination</response>
        /// <response code="401">If the user is not authorized to access this resource</response>
        [HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoleForSelect))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<IEnumerable<RoleForSelect>>> Get([FromQuery] QueryParameters parameters, string searchString, CancellationToken cancellationToken = default)
		{
			LogMethodListGetterEntry(parameters, searchString);

			try
			{
				var roles = await _unitOfWork.RolesRepository.GetPaginatedForSelectAsync(parameters, searchString, cancellationToken);

				if (roles.Items != null && roles.Items.Count > 0)
				{
					LogMethodListGetterData(roles.Items.Count, parameters, searchString);

					// Lets create pagination header first (if the pagination was requested at first place)
					CreatePagingHeader(ref roles);

					return Ok(roles.Items);
				}

				LogMethodListGetterNoData(parameters, searchString);

				return NoContent(); // No roles for given index/page combination
			}
			catch(TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}				
		}


        
		// GET api/RoleForSelect/5
		/// <summary>
		/// Gets single role
		/// </summary>
		/// <remarks>This method returns role with given key</remarks>
		/// <param name="id">The key of the role we want to fetch</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="200">Returns found role</response>
		/// <response code="204">If there is no role found for given key</response>   
		[HttpGet("{id}", Name = "GetAdminsRole")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoleForSelect))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<ActionResult<RoleForSelect>> GetRole(string id, CancellationToken cancellationToken = default)
		{
			LogMethodSingleGetterEntry(id);
			try
			{
				var role = await _unitOfWork.RolesRepository.GetForSelectAsync(id, cancellationToken);

				if (role == null)
				{
					LogMethodSingleGetterNoData(id);
					return NoContent(); // No role found with given key
				}

				LogMethodSingleGetterData(id);
				return Ok(role);
			}
			catch(TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}					
		}


		#endregion Getting data


		#region Adding/creating
		// POST api/role
		/// <summary>
		/// Creates role
		/// </summary>
		/// <remarks>This method adds new role.</remarks>
		/// <param name="role">JSON parsed object we want to insert</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <returns>Newly created role, as it is stored in the database.</returns>
		/// <response code="201">If creation of the role was successful.</response>
		/// <response code="400">If passed role was null or the creation of the role was not successful.</response>   
		/// <response code="422">If the validation of passed role failed.</response>   
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RoleForSelect))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]		
		public async Task<ActionResult<RoleForSelect>> CreateRole([FromBody] RoleForInsert role, CancellationToken cancellationToken = default)
		{
			LogMethodCreateEntry(role);

			if (role == null)
			{
				LogMethodCreateBadRequest(role);
				return BadRequest("Cannot create an empty role.");
			}


			// Checks the validation in the data annotation of the data model
			if (!ModelState.IsValid)
			{
				LogMethodCreateInvalid(role);
				return new UnprocessableEntityObjectResult(ModelState);
			}


			try
			{
				// Map the passed objects to database entity/entities
				var roleEntity = _mapper.Map<Role>(role);

				// If there is a need to manipulate the object, this is the place to do it
				ManipulateOnCreate(roleEntity);

				// Add it to UnitOfWork
				_unitOfWork.RolesRepository.Add(roleEntity);

				// Try to save it
				if (!await _unitOfWork.SaveAsync(cancellationToken))
				{
					LogMethodCreateSaveFailed(roleEntity);
					return BadRequest("Creating an role failed on save.");
					// return StatusCode(500, "A problem happened with handling your request.");
				}

				LogMethodCreateSaveSuccessful(roleEntity.Id);


				// Finally, get the object from the database, because this is what we want to return
				var roleToReturn  = await _unitOfWork.RolesRepository.GetForSelectAsync(roleEntity.Id, cancellationToken);

				return CreatedAtRoute(_defaultGetSingleRoute, // nameof(GetRole),
					new { roleEntity.Id },
					roleToReturn);
			}
			catch(TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}					
		}

		#endregion Adding/creating


		#region Updating
		// PUT - UPDATE
		/// <summary>
		/// "Upserts" role (updates it if role exists or it creates new if it doesn't exist)
		/// </summary>
		/// <remarks>This method updates role with the form data if the role is found in the database, or it creates new role if it is not found.</remarks>
		/// <param name="id">The key of the role we want to fetch</param>
		/// <param name="role">JSON parsed object we want to update/insert</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="201">If role was not found and its creation was successful.</response>
		/// <response code="204">If update of the role was successful.</response>
		/// <response code="400">If upsertion of the role was not successful.</response>   
		/// <response code="422">If the validation of role failed.</response>   
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RoleForUpdate))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]		
		public async Task<IActionResult> UpdateRole(string id, [FromBody] RoleForUpdate role, CancellationToken cancellationToken = default)
		{
			LogMethodUpdateEntry(id, role);

			if (role == null)
			{
				LogMethodUpdateBadRequest(id, role);
				return BadRequest();
			}


			// Checks the validation in the data annotation of the data model
			if (!ModelState.IsValid)
			{
				LogMethodUpdateInvalid(id, role);
				return new UnprocessableEntityObjectResult(ModelState);
			}

			try
			{
				var roleFromRepo = await _unitOfWork.RolesRepository.GetAsync(id);
				if (roleFromRepo == null)
				{
					LogMethodUpdateUpserting(id);
					var roleToCreate = _mapper.Map<Role>(role);
					roleToCreate.Id = id;

					// If there is a need to manipulate the object, this is the place to do it
					ManipulateOnCreate(roleToCreate);

					_unitOfWork.RolesRepository.Add(roleToCreate);

					if (!await _unitOfWork.SaveAsync(cancellationToken))
					{
						LogMethodUpdateUpsertFailed(id, roleToCreate);
						return BadRequest($"Upserting role id = {id} failed on save.");
					}

					// Finally, get the object from the database, because this is what we want to return
					var roleToReturn  = await _unitOfWork.RolesRepository.GetForSelectAsync(roleToCreate.Id, cancellationToken);

					LogMethodUpdateUpsertSuccessful(roleToCreate.Id);


					return CreatedAtRoute(_defaultGetSingleRoute, // nameof(GetRole),
						new { roleToCreate.Id },
						roleToReturn);
				}
				else
				{
					role.Id = id;
					_mapper.Map(role, roleFromRepo);

					// If there is a need to manipulate the object, this is the place to do it
					ManipulateOnUpdate(roleFromRepo);

					// There is no such method like Update in the repository - we are working directly on the database
					// _unitOfWork.RolesRepository.Update(roleFromRepo);

					if (!await _unitOfWork.SaveAsync(cancellationToken))
					{
						LogMethodUpdateSaveFailed(id, roleFromRepo);
						return BadRequest($"Updating role id = {id} failed on save.");
					}

					LogMethodUpdateSaveSuccessful(id);

					return NoContent();
				}
			}
			catch(TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}
		}


		#endregion Updating


		#region Removing
		// DELETE
		/// <summary>
		/// Deletes role
		/// </summary>
		/// <remarks>This method deletes role with specified id.</remarks>
		/// <param name="id">The key of the role we want to fetch</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="200">If deletion of the role was successful.</response>
		/// <response code="400">If deletion of the role was not successful.</response>   
		/// <response code="404">If the role was not found.</response>   
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoleForSelect))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<RoleForSelect>> DeleteRole(string id, CancellationToken cancellationToken = default)
		{
			LogMethodDeleteEntry(id);

			try
			{
				var roleFromRepo = await _unitOfWork.RolesRepository.GetAsync(id, cancellationToken);
				if (roleFromRepo == null)
				{
					LogMethodDeleteNoEntity(id);
					return NotFound();
				}

				// Lets get this guy before we remove it from the database
				// so that we can return proper object back to the caller of the method.
				var roleForSelect = await _unitOfWork.RolesRepository.GetForSelectAsync(id, cancellationToken);


				_unitOfWork.RolesRepository.Remove(roleFromRepo);

				if (!await _unitOfWork.SaveAsync(cancellationToken))
				{
					LogMethodDeleteFailed(id);
					return BadRequest($"Deleting role with id = {id} failed on save.");
				}

				LogMethodDeleteSuccessful(id);
				return Ok(roleForSelect);
			}
			catch(TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}				
		}
		#endregion Removing
	}
}
