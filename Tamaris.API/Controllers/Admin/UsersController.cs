using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoMapper;

using Tamaris.Domains.Admin;
using Tamaris.Domains.DataShaping;
using Tamaris.Domains.Msg;
using Tamaris.Entities.Admin;
using Tamaris.DAL.Interfaces;
using Tamaris.API.Infrastructure.Attributes;
using Microsoft.AspNetCore.Identity;

namespace Tamaris.API.Controllers.Admin
{
	[TamarisController(Endpoint = "Admin/")]
	[Produces("application/json")]
	public class UsersController : BaseController
	{
		private readonly ITamarisUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly UserManager<User> _userManager;


		private readonly string _defaultGetSingleRoute = "GetAdminsUser";

		public UsersController(ITamarisUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_userManager = userManager;
		}


		#region Getting data
		// GET
		/// <summary>
		/// Gets paging list with all users
		/// </summary>
		/// <remarks>With pagination, you can optimize fetching the users. For example, you can fetch 100 pages with 10 pages per page or 10 pages with 100 users per page.
		/// Additionally, if you opt to use the pagination (you provide the values for pageIndex and pageSize), this method will also include X-Pagination header
		/// with additional information about the result like totalPages, currentPage, next-/previous link, has next-/previous link.
		/// </remarks>
		/// <param name="parameters">Query parameters</param>
		/// <param name="searchString">Search anything in the given users list. Search is performed against all searchable fields.</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>

		/// <returns></returns>
		/// <response code="200">Returns list of selected users</response>
		/// <response code="204">If there are no users for given PageIndex/PageSize/SearchString combination</response>
		/// <response code="401">If the user is not authorized to access this resource</response>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserForSelect))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<IEnumerable<UserForSelect>>> Get([FromQuery] QueryParameters parameters, string searchString, CancellationToken cancellationToken = default)
		{
			LogMethodListGetterEntry(parameters, searchString);

			try
			{
				var users = await _unitOfWork.UsersRepository.GetPaginatedForSelectAsync(parameters, searchString, cancellationToken);

				if (users.Items != null && users.Items.Count > 0)
				{
					LogMethodListGetterData(users.Items.Count, parameters, searchString);

					// Lets create pagination header first (if the pagination was requested at first place)
					CreatePagingHeader(ref users);

					return Ok(users.Items);
				}

				LogMethodListGetterNoData(parameters, searchString);

				return NoContent(); // No users for given index/page combination
			}
			catch(TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}				
		}


		// GET
		/// <summary>
		/// Gets list with all users for messaging
		/// </summary>
		/// <param name="excludeUsername">User to exclude from the list</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <returns>List with all available users for messaging</returns>
		/// <response code="200">Returns list of selected users</response>
		/// <response code="204">If there are no users for given PageIndex/PageSize/SearchString combination</response>
		/// <response code="401">If the user is not authorized to access this resource</response>
		[HttpGet("ForChat/{excludeUsername}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserForSelect))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<IEnumerable<UserForChat>>> GetAllForMessaging(string excludeUsername, CancellationToken cancellationToken = default)
		{
			try
			{
				var users = await _unitOfWork.UsersRepository.GetAllForChatAsync(excludeUsername, cancellationToken);

				if (users != null && users.Any())
				{
					return Ok(users);
				}

				return NoContent(); // No users for given index/page combination
			}
			catch (TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}
		}



		// GET api/UserForSelect/5
		/// <summary>
		/// Gets single user
		/// </summary>
		/// <remarks>This method returns user with given key</remarks>
		/// <param name="id">The key of the user we want to fetch</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="200">Returns found user</response>
		/// <response code="204">If there is no user found for given key</response>   
		[HttpGet("{id}", Name = "GetAdminsUser")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserForSelect))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<ActionResult<UserForSelect>> GetUser(string id, CancellationToken cancellationToken = default)
		{
			LogMethodSingleGetterEntry(id);
			try
			{
				var user = await _unitOfWork.UsersRepository.GetForSelectByIdAsync(id, cancellationToken);

				if (user == null)
				{
					LogMethodSingleGetterNoData(id);
					return NoContent(); // No user found with given key
				}

				var userFromRepo = await _unitOfWork.UsersRepository.GetAsync(user.Id);

				// In order to get the roles we need to fetch the "real" user from the repository
				var roles = await _userManager.GetRolesAsync(userFromRepo);
				user.Roles = roles != null ? roles.ToList() : new List<string>();

				LogMethodSingleGetterData(id);
				return Ok(user);
			}
			catch(TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}					
		}


		// GET api/UserForSelect/5
		/// <summary>
		/// Gets single user by username
		/// </summary>
		/// <remarks>This method returns user with given key</remarks>
		/// <param name="userName">Username of the user we want to fetch</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="200">Returns found user</response>
		/// <response code="204">If there is no user found for given key</response>   
		[HttpGet("ByUsername/{userName}", Name = "GetAdminsUserByUsername")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserForSelect))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<ActionResult<UserForSelect>> GetUserByUsername(string userName, CancellationToken cancellationToken = default)
		{
			LogMethodSingleGetterEntry(userName);
			try
			{
				var user = await _unitOfWork.UsersRepository.GetForSelectByUsernameAsync(userName, cancellationToken);

				if (user == null)
				{
					LogMethodSingleGetterNoData(userName);
					return NoContent(); // No user found with given key
				}

				var userFromRepo = await _unitOfWork.UsersRepository.GetAsync(user.Id);

				// In order to get the roles we need to fetch the "real" user from the repository
				var roles = await _userManager.GetRolesAsync(userFromRepo);
				user.Roles = roles != null ? roles.ToList() : new List<string>();

				LogMethodSingleGetterData(userName);
				return Ok(user);
			}
			catch (TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}
		}



		// GET api/UserForSelect/5
		/// <summary>
		/// Gets single user by Email
		/// </summary>
		/// <remarks>This method returns user with given key</remarks>
		/// <param name="email">Email of the user we want to fetch</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="200">Returns found user</response>
		/// <response code="204">If there is no user found for given key</response>   
		[HttpGet("ByEmail/{Email}", Name = "GetAdminsUserByEmail")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserForSelect))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<ActionResult<UserForSelect>> GetUserByEmail(string email, CancellationToken cancellationToken = default)
		{
			LogMethodSingleGetterEntry(email);
			try
			{
				var user = await _unitOfWork.UsersRepository.GetForSelectByEmailAsync(email, cancellationToken);

				if (user == null)
				{
					LogMethodSingleGetterNoData(email);
					return NoContent(); // No user found with given key
				}

				var userFromRepo = await _unitOfWork.UsersRepository.GetAsync(user.Id);

				// In order to get the roles we need to fetch the "real" user from the repository
				var roles = await _userManager.GetRolesAsync(userFromRepo);
				user.Roles = roles != null ? roles.ToList() : new List<string>();

				LogMethodSingleGetterData(email);
				return Ok(user);
			}
			catch (TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}
		}


		#endregion Getting data


		#region Adding/creating
		// POST api/user
		/// <summary>
		/// Creates user
		/// </summary>
		/// <remarks>This method adds new user.</remarks>
		/// <param name="user">JSON parsed object we want to insert</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <returns>Newly created user, as it is stored in the database.</returns>
		/// <response code="201">If creation of the user was successful.</response>
		/// <response code="400">If passed user was null or the creation of the user was not successful.</response>   
		/// <response code="422">If the validation of passed user failed.</response>   
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserForSelect))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]		
		public async Task<ActionResult<UserForSelect>> CreateUser([FromBody] UserForInsert user, CancellationToken cancellationToken = default)
		{
			LogMethodCreateEntry(user);

			if (user == null)
			{
				LogMethodCreateBadRequest(user);
				return BadRequest("Cannot create an empty user.");
			}


			// Checks the validation in the data annotation of the data model
			//if (!ModelState.IsValid)
			//{
			//	LogMethodCreateInvalid(user);
			//	return new UnprocessableEntityObjectResult(ModelState);
			//}


			try
			{
				// Map the passed objects to database entity/entities
				var userEntity = _mapper.Map<User>(user);

				// If there is a need to manipulate the object, this is the place to do it
				ManipulateOnCreate(userEntity);


                #region User manager part
                // Instead of storing the user using UnitOfWork we are using UserManager first
                // to enforce hashing and the rest of identity stuff
                var resultCreateUser = await _userManager.CreateAsync(userEntity, user.Password);
				if (!resultCreateUser.Succeeded)
				{
					var errors = resultCreateUser.Errors.Select(e => e.Description);
					return BadRequest(errors);
				}

				var resultAssignRole = await _userManager.AddToRolesAsync(userEntity, user.Roles);
				if (!resultAssignRole.Succeeded)
				{
					var errors = resultAssignRole.Errors.Select(e => e.Description);
					return BadRequest(errors);
				}
				#endregion User manager part


				var userFromRepo = await _unitOfWork.UsersRepository.GetByUsernameAsync(user.Username);
				_mapper.Map(userEntity, userFromRepo);

				// Now, try to save it
				if (!await _unitOfWork.SaveAsync(cancellationToken))
				{
					LogMethodCreateSaveFailed(userEntity);
					return BadRequest("Creating an user failed on save.");
					// return StatusCode(500, "A problem happened with handling your request.");
				}

				LogMethodCreateSaveSuccessful(userEntity.Id);


				// Finally, get the object from the database, because this is what we want to return
				var userToReturn  = await _unitOfWork.UsersRepository.GetForSelectByIdAsync(userEntity.Id, cancellationToken);

				return CreatedAtRoute(_defaultGetSingleRoute, // nameof(GetUser),
					new { userEntity.Id },
					userToReturn);
			}
			catch(TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}	
			catch(Exception ex)
            {
				return BadRequest(ex.Message);
            }
		}

		#endregion Adding/creating


		#region Updating
		// PUT - UPDATE
		/// <summary>
		/// "Upserts" user (updates it if user exists or it creates new if it doesn't exist)
		/// </summary>
		/// <remarks>This method updates user with the form data if the user is found in the database, or it creates new user if it is not found.</remarks>
		/// <param name="id">The key of the user we want to fetch</param>
		/// <param name="user">JSON parsed object we want to update/insert</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="201">If user was not found and its creation was successful.</response>
		/// <response code="204">If update of the user was successful.</response>
		/// <response code="400">If upsertion of the user was not successful.</response>   
		/// <response code="422">If the validation of user failed.</response>   
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserForUpdate))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]		
		public async Task<IActionResult> UpdateUser(string id, [FromBody] UserForUpdate user, CancellationToken cancellationToken = default)
		{
			LogMethodUpdateEntry(id, user);

			if (user == null)
			{
				LogMethodUpdateBadRequest(id, user);
				return BadRequest();
			}


			// Checks the validation in the data annotation of the data model
			//if (!ModelState.IsValid)
			//{
			//	LogMethodUpdateInvalid(id, user);
			//	return new UnprocessableEntityObjectResult(ModelState);
			//}

			try
			{
				var userFromRepo = await _unitOfWork.UsersRepository.GetAsync(id);
				if (userFromRepo == null)
				{
					LogMethodUpdateUpserting(id);
					var userToCreate = _mapper.Map<User>(user);
					userToCreate.Id = id;

					// If there is a need to manipulate the object, this is the place to do it
					ManipulateOnCreate(userToCreate);


					#region User manager part
					// Instead of storing the user using UnitOfWork we are using UserManager first
					// to enforce hashing and the rest of identity stuff
					var resultCreateUser = await _userManager.CreateAsync(userToCreate);
					if (!resultCreateUser.Succeeded)
					{
						var errors = resultCreateUser.Errors.Select(e => e.Description);
						return BadRequest(errors);
					}

					var resultAssignRole = await _userManager.AddToRolesAsync(userToCreate, user.Roles);
					if (!resultAssignRole.Succeeded)
					{
						var errors = resultAssignRole.Errors.Select(e => e.Description);
						return BadRequest(errors);
					}
					#endregion User manager part


					userFromRepo = await _unitOfWork.UsersRepository.GetByUsernameAsync(user.Username);
					_mapper.Map(userToCreate, userFromRepo);

					if (!await _unitOfWork.SaveAsync(cancellationToken))
					{
						LogMethodUpdateUpsertFailed(id, userToCreate);
						return BadRequest($"Upserting user id = {id} failed on save.");
					}

					// Finally, get the object from the database, because this is what we want to return
					var userToReturn  = await _unitOfWork.UsersRepository.GetForSelectByIdAsync(userToCreate.Id, cancellationToken);

					LogMethodUpdateUpsertSuccessful(userToCreate.Id);


					return CreatedAtRoute(_defaultGetSingleRoute, // nameof(GetUser),
						new { userToCreate.Id },
						userToReturn);
				}
				else
				{
					user.Id = id;
					_mapper.Map(user, userFromRepo);

					// If there is a need to manipulate the object, this is the place to do it
					ManipulateOnUpdate(userFromRepo);

					#region User manager part
					var existingRoles = await _userManager.GetRolesAsync(userFromRepo);
					var removedRoles = existingRoles.Where(er => !user.Roles.Any(ur => ur == er));
					var newRoles = user.Roles.Where(ur => !existingRoles.Any(er => er == ur));

					// First revoke the removed roles
					await _userManager.RemoveFromRolesAsync(userFromRepo, removedRoles);

					// Then assign the selected roles
					var resultAssignRole = await _userManager.AddToRolesAsync(userFromRepo, newRoles);
					if (!resultAssignRole.Succeeded)
					{
						var errors = resultAssignRole.Errors.Select(e => e.Description);
						return BadRequest(errors);
					}
					#endregion User manager part


					// There is no such method like Update in the repository - we are working directly on the database
					// _unitOfWork.UsersRepository.Update(userFromRepo);

					if (!await _unitOfWork.SaveAsync(cancellationToken))
					{
						LogMethodUpdateSaveFailed(id, userFromRepo);
						return BadRequest($"Updating user id = {id} failed on save.");
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
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}



		// PUT - UPDATE
		/// <summary>
		/// Updates user profile data (no roles changes)
		/// </summary>
		/// <remarks>This method updates user profile with the form data.</remarks>
		/// <param name="id">The key of the user we want to fetch</param>
		/// <param name="user">JSON parsed object we want to update</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="204">If update of the user was successful.</response>
		/// <response code="400">If update of the user was not successful.</response>   
		/// <response code="422">If the validation of user failed.</response>   
		[HttpPut("Profile/{id}")]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserForUpdate))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
		public async Task<IActionResult> UpdateUserProfile(string id, [FromBody] UserForProfileUpdate user, CancellationToken cancellationToken = default)
		{
			LogMethodUpdateEntry(id, user);

			if (user == null)
			{
				LogMethodUpdateBadRequest(id, user);
				return BadRequest();
			}


			// Checks the validation in the data annotation of the data model
			//if (!ModelState.IsValid)
			//{
			//	LogMethodUpdateInvalid(id, user);
			//	return new UnprocessableEntityObjectResult(ModelState);
			//}

			try
			{
				var userFromRepo = await _unitOfWork.UsersRepository.GetAsync(id);
				if (userFromRepo != null)
				{
					user.Id = id;
					_mapper.Map(user, userFromRepo);

					if (!string.IsNullOrEmpty(user.CurrentPassword) && !string.IsNullOrEmpty(user.NewPassword))
						await _userManager.ChangePasswordAsync(userFromRepo, user.CurrentPassword, user.NewPassword);

					// If there is a need to manipulate the object, this is the place to do it
					ManipulateOnUpdate(userFromRepo);

					// There is no such method like Update in the repository - we are working directly on the database
					// _unitOfWork.UsersRepository.Update(userFromRepo);

					if (!await _unitOfWork.SaveAsync(cancellationToken))
					{
						LogMethodUpdateSaveFailed(id, userFromRepo);
						return BadRequest($"Updating user id = {id} failed on save.");
					}

					LogMethodUpdateSaveSuccessful(id);

					return NoContent();
				}
				else
                {
					return BadRequest($"No such user found");
                }
			}
			catch (TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}


		#endregion Updating


		#region Removing
		// DELETE
		/// <summary>
		/// Deletes user
		/// </summary>
		/// <remarks>This method deletes user with specified id.</remarks>
		/// <param name="username">The key of the user we want to fetch</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="200">If deletion of the user was successful.</response>
		/// <response code="400">If deletion of the user was not successful.</response>   
		/// <response code="404">If the user was not found.</response>   
		[HttpDelete("{username}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserForSelect))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<UserForSelect>> DeleteUser(string username, CancellationToken cancellationToken = default)
		{
			LogMethodDeleteEntry(username);

			try
			{
				var userFromRepo = await _unitOfWork.UsersRepository.GetByUsernameAsync(username, cancellationToken);
				if (userFromRepo == null)
				{
					LogMethodDeleteNoEntity(username);
					return NotFound();
				}

				// Lets get this guy before we remove it from the database
				// so that we can return proper object back to the caller of the method.
				var userForSelect = await _unitOfWork.UsersRepository.GetForSelectByUsernameAsync(username, cancellationToken);

				// We are not using Repository for this like here:
				// _unitOfWork.UsersRepository.Remove(userFromRepo);
				// but instead UserManager like here:
				var resultDeletion = await _userManager.DeleteAsync(userFromRepo);

				if (!resultDeletion.Succeeded)
				{
					var errors = resultDeletion.Errors.Select(e => e.Description);
					return BadRequest(errors);
				}

				if (!await _unitOfWork.SaveAsync(cancellationToken))
				{
					LogMethodDeleteFailed(username);
					return BadRequest($"Deleting user with id = {username} failed on save.");
				}

				LogMethodDeleteSuccessful(username);
				return Ok(userForSelect);
			}
			catch(TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}				
		}
		#endregion Removing


		#region Nested actions
		/// <summary>
		/// Gets paging list with all roles (ForSelect) for given user
		/// </summary>
		/// <remarks>With pagination, you can optimize fetching the roles. For example, you can fetch 100 pages with 30 pages per page or 30 pages with 100 roles per page.
		/// Additionally, if you opt to use the pagination (you provide the values for pageIndex and pageSize), this method will also include X-Pagination header
		/// with additional information about the result like totalPages, currentPage, next-/previous link, has next-/previous link.
		/// </remarks>
		/// <param name="id">The key of the user we want to fetch the roles for</param>
		/// <param name="parameters">Query parameters</param>
		/// <param name="searchString">Search anything in the given roles list. Search is performed against all searchable fields.</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <returns></returns>
		/// <response code="200">Returns list of selected roles</response>
		/// <response code="204">If there are no roles for given pageIndex/pageSize/searchString combination</response>   
		/// <response code="401">If the user is not authorized to access this resource</response>
		[HttpGet("{id}/AdminRolesForSelectForUsers", Name = "GetAdminRolesForSelectForUsers")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RoleForSelect))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<IEnumerable<RoleForSelect>>> GetAdminRolesForSelectForUsers(int id, [FromQuery] QueryParameters parameters, string searchString, CancellationToken cancellationToken = default)
		{
			LogMethodListGetterEntry(parameters, searchString);

			try
			{
				var roles = await _unitOfWork.RolesRepository.GetPaginatedForSelect_ForUsersAsync(id, parameters, searchString, cancellationToken);

				if (roles.Items != null && roles.Items.Count > 0)
				{
					LogMethodListGetterData(roles.Items.Count, parameters, searchString);

					// Lets create pagination header first (if the pagination was requested at first place)
					CreatePagingHeader(ref roles);

					return Ok(roles.Items);
				}

				LogMethodListGetterNoData(parameters, searchString);

				return NoContent(); // No roles (ForSelect) for the given user and index/page combination!
			}
			catch(TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}
		}



		/// <summary>
		/// Gets paging list with all messages (ForSelect) for given user
		/// </summary>
		/// <remarks>With pagination, you can optimize fetching the messages. For example, you can fetch 100 pages with 30 pages per page or 30 pages with 100 messages per page.
		/// Additionally, if you opt to use the pagination (you provide the values for pageIndex and pageSize), this method will also include X-Pagination header
		/// with additional information about the result like totalPages, currentPage, next-/previous link, has next-/previous link.
		/// </remarks>
		/// <param name="id">The key of the user we want to fetch the messages for</param>
		/// <param name="parameters">Query parameters</param>
		/// <param name="searchString">Search anything in the given messages list. Search is performed against all searchable fields.</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <returns></returns>
		/// <response code="200">Returns list of selected messages</response>
		/// <response code="204">If there are no messages for given pageIndex/pageSize/searchString combination</response>   
		/// <response code="401">If the user is not authorized to access this resource</response>
		[HttpGet("{id}/MsgMessagesForSelectForReceiverUserForReceiverUser", Name = "GetMsgMessagesForSelectForReceiverUserForReceiverUser")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageForSelect))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<IEnumerable<MessageForSelect>>> GetMsgMessagesForSelectForReceiverUserForReceiverUser(string id, [FromQuery] QueryParameters parameters, string searchString, CancellationToken cancellationToken = default)
		{
			LogMethodListGetterEntry(parameters, searchString);

			try
			{
				var messages = await _unitOfWork.MessagesRepository.GetPaginatedForSelect_ForReceiverUserAsync(id, parameters, searchString, cancellationToken);

				if (messages.Items != null && messages.Items.Count > 0)
				{
					LogMethodListGetterData(messages.Items.Count, parameters, searchString);

					// Lets create pagination header first (if the pagination was requested at first place)
					CreatePagingHeader(ref messages);

					return Ok(messages.Items);
				}

				LogMethodListGetterNoData(parameters, searchString);

				return NoContent(); // No messages (ForSelect) for the given user and index/page combination!
			}
			catch(TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}
		}



		/// <summary>
		/// Gets paging list with all messages (ForSelect) for given user
		/// </summary>
		/// <remarks>With pagination, you can optimize fetching the messages. For example, you can fetch 100 pages with 30 pages per page or 30 pages with 100 messages per page.
		/// Additionally, if you opt to use the pagination (you provide the values for pageIndex and pageSize), this method will also include X-Pagination header
		/// with additional information about the result like totalPages, currentPage, next-/previous link, has next-/previous link.
		/// </remarks>
		/// <param name="id">The key of the user we want to fetch the messages for</param>
		/// <param name="parameters">Query parameters</param>
		/// <param name="searchString">Search anything in the given messages list. Search is performed against all searchable fields.</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <returns></returns>
		/// <response code="200">Returns list of selected messages</response>
		/// <response code="204">If there are no messages for given pageIndex/pageSize/searchString combination</response>   
		/// <response code="401">If the user is not authorized to access this resource</response>
		[HttpGet("{id}/MsgMessagesForSelectForSenderUserForSenderUser", Name = "GetMsgMessagesForSelectForSenderUserForSenderUser")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageForSelect))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<IEnumerable<MessageForSelect>>> GetMsgMessagesForSelectForSenderUserForSenderUser(string id, [FromQuery] QueryParameters parameters, string searchString, CancellationToken cancellationToken = default)
		{
			LogMethodListGetterEntry(parameters, searchString);

			try
			{
				var messages = await _unitOfWork.MessagesRepository.GetPaginatedForSelect_ForSenderUserAsync(id, parameters, searchString, cancellationToken);

				if (messages.Items != null && messages.Items.Count > 0)
				{
					LogMethodListGetterData(messages.Items.Count, parameters, searchString);

					// Lets create pagination header first (if the pagination was requested at first place)
					CreatePagingHeader(ref messages);

					return Ok(messages.Items);
				}

				LogMethodListGetterNoData(parameters, searchString);

				return NoContent(); // No messages (ForSelect) for the given user and index/page combination!
			}
			catch(TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}
		}




		#endregion Nested actions
	}
}
