using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using AutoMapper;

using Tamaris.Domains.DataShaping;
using Tamaris.Domains.Msg;
using Tamaris.Entities.Msg;
using Tamaris.DAL.Interfaces;
using Tamaris.API.Infrastructure.Attributes;


namespace Tamaris.API.Controllers.Msg
{
	[TamarisController(Endpoint = "Msg/")]
	[Produces("application/json")]
	public class MessagesController : BaseController
	{
		private readonly ITamarisUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		private readonly string _defaultGetSingleRoute = "GetMsgsMessage";

		public MessagesController(ITamarisUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;

		}


		#region Getting data
		// GET
		/// <summary>
		/// Gets paging list with all messages
		/// </summary>
		/// <remarks>With pagination, you can optimize fetching the messages. For example, you can fetch 100 pages with 10 pages per page or 10 pages with 100 messages per page.
		/// Additionally, if you opt to use the pagination (you provide the values for pageIndex and pageSize), this method will also include X-Pagination header
		/// with additional information about the result like totalPages, currentPage, next-/previous link, has next-/previous link.
		/// </remarks>
		/// <param name="parameters">Query parameters</param>
		/// <param name="searchString">Search anything in the given messages list. Search is performed against all searchable fields.</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>

		/// <returns></returns>
		/// <response code="200">Returns list of selected messages</response>
		/// <response code="204">If there are no messages for given PageIndex/PageSize/SearchString combination</response>
		/// <response code="401">If the user is not authorized to access this resource</response>
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageForSelect))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<IEnumerable<MessageForSelect>>> Get([FromQuery] QueryParameters parameters, string searchString, CancellationToken cancellationToken = default)
		{
			LogMethodListGetterEntry(parameters, searchString);

			try
			{
				var messages = await _unitOfWork.MessagesRepository.GetPaginatedForSelectAsync(parameters, searchString, cancellationToken);

				if (messages.Items != null && messages.Items.Count > 0)
				{
					LogMethodListGetterData(messages.Items.Count, parameters, searchString);

					// Lets create pagination header first (if the pagination was requested at first place)
					CreatePagingHeader(ref messages);

					return Ok(messages.Items);
				}

				LogMethodListGetterNoData(parameters, searchString);

				return NoContent(); // No messages for given index/page combination
			}
			catch(TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}
			catch (Exception ex)
			{
				var a = ex;
				return NoContent();
			}
		}



		// GET api/MessageForSelect/5
		/// <summary>
		/// Gets single message
		/// </summary>
		/// <remarks>This method returns message with given key</remarks>
		/// <param name="id">The key of the message we want to fetch</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="200">Returns found message</response>
		/// <response code="204">If there is no message found for given key</response>   
		[HttpGet("{id}", Name = "GetMsgsMessage")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageForSelect))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<ActionResult<MessageForSelect>> GetMessage(int id, CancellationToken cancellationToken = default)
		{
			LogMethodSingleGetterEntry(id);
			try
			{
				var message = await _unitOfWork.MessagesRepository.GetForSelectAsync(id, cancellationToken);

				if (message == null)
				{
					LogMethodSingleGetterNoData(id);
					return NoContent(); // No message found with given key
				}

				LogMethodSingleGetterData(id);
				return Ok(message);
			}
			catch(TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}					
		}


		#endregion Getting data


		#region Adding/creating
		// POST api/message
		/// <summary>
		/// Creates message
		/// </summary>
		/// <remarks>This method adds new message.</remarks>
		/// <param name="message">JSON parsed object we want to insert</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <returns>Newly created message, as it is stored in the database.</returns>
		/// <response code="201">If creation of the message was successful.</response>
		/// <response code="400">If passed message was null or the creation of the message was not successful.</response>   
		/// <response code="422">If the validation of passed message failed.</response>   
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MessageForSelect))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]		
		public async Task<ActionResult<MessageForSelect>> CreateMessage([FromBody] MessageForInsert message, CancellationToken cancellationToken = default)
		{
			LogMethodCreateEntry(message);

			if (message == null)
			{
				LogMethodCreateBadRequest(message);
				return BadRequest("Cannot create an empty message.");
			}


			// Checks the validation in the data annotation of the data model
			if (!ModelState.IsValid)
			{
				LogMethodCreateInvalid(message);
				return new UnprocessableEntityObjectResult(ModelState);
			}


			try
			{
				// Map the passed objects to database entity/entities
				var messageEntity = _mapper.Map<Message>(message);

				// If there is a need to manipulate the object, this is the place to do it
				ManipulateOnCreate(messageEntity);

				// Add it to UnitOfWork
				_unitOfWork.MessagesRepository.Add(messageEntity);

				// Try to save it
				if (!await _unitOfWork.SaveAsync(cancellationToken))
				{
					LogMethodCreateSaveFailed(messageEntity);
					return BadRequest("Creating an message failed on save.");
					// return StatusCode(500, "A problem happened with handling your request.");
				}

				LogMethodCreateSaveSuccessful(messageEntity.Id);


				// Finally, get the object from the database, because this is what we want to return
				var messageToReturn  = await _unitOfWork.MessagesRepository.GetForSelectAsync(messageEntity.Id, cancellationToken);

				return CreatedAtRoute(_defaultGetSingleRoute, // nameof(GetMessage),
					new { messageEntity.Id },
					messageToReturn);
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
		/// "Upserts" message (updates it if message exists or it creates new if it doesn't exist)
		/// </summary>
		/// <remarks>This method updates message with the form data if the message is found in the database, or it creates new message if it is not found.</remarks>
		/// <param name="id">The key of the message we want to fetch</param>
		/// <param name="message">JSON parsed object we want to update/insert</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="201">If message was not found and its creation was successful.</response>
		/// <response code="204">If update of the message was successful.</response>
		/// <response code="400">If upsertion of the message was not successful.</response>   
		/// <response code="422">If the validation of message failed.</response>   
		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(MessageForUpdate))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]		
		public async Task<IActionResult> UpdateMessage(int id, [FromBody] MessageForUpdate message, CancellationToken cancellationToken = default)
		{
			LogMethodUpdateEntry(id, message);

			if (message == null)
			{
				LogMethodUpdateBadRequest(id, message);
				return BadRequest();
			}


			// Checks the validation in the data annotation of the data model
			if (!ModelState.IsValid)
			{
				LogMethodUpdateInvalid(id, message);
				return new UnprocessableEntityObjectResult(ModelState);
			}

			try
			{
				var messageFromRepo = await _unitOfWork.MessagesRepository.GetAsync(id);
				if (messageFromRepo == null)
				{
					LogMethodUpdateUpserting(id);
					var messageToCreate = _mapper.Map<Message>(message);
					messageToCreate.Id = id;

					// If there is a need to manipulate the object, this is the place to do it
					ManipulateOnCreate(messageToCreate);

					_unitOfWork.MessagesRepository.Add(messageToCreate);

					if (!await _unitOfWork.SaveAsync(cancellationToken))
					{
						LogMethodUpdateUpsertFailed(id, messageToCreate);
						return BadRequest($"Upserting message id = {id} failed on save.");
					}

					// Finally, get the object from the database, because this is what we want to return
					var messageToReturn  = await _unitOfWork.MessagesRepository.GetForSelectAsync(messageToCreate.Id, cancellationToken);

					LogMethodUpdateUpsertSuccessful(messageToCreate.Id);


					return CreatedAtRoute(_defaultGetSingleRoute, // nameof(GetMessage),
						new { messageToCreate.Id },
						messageToReturn);
				}
				else
				{
					message.Id = id;
					_mapper.Map(message, messageFromRepo);

					// If there is a need to manipulate the object, this is the place to do it
					ManipulateOnUpdate(messageFromRepo);

					// There is no such method like Update in the repository - we are working directly on the database
					// _unitOfWork.MessagesRepository.Update(messageFromRepo);

					if (!await _unitOfWork.SaveAsync(cancellationToken))
					{
						LogMethodUpdateSaveFailed(id, messageFromRepo);
						return BadRequest($"Updating message id = {id} failed on save.");
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
		/// Deletes message
		/// </summary>
		/// <remarks>This method deletes message with specified id.</remarks>
		/// <param name="id">The key of the message we want to fetch</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="200">If deletion of the message was successful.</response>
		/// <response code="400">If deletion of the message was not successful.</response>   
		/// <response code="404">If the message was not found.</response>   
		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageForSelect))]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<ActionResult<MessageForSelect>> DeleteMessage(int id, CancellationToken cancellationToken = default)
		{
			LogMethodDeleteEntry(id);

			try
			{
				var messageFromRepo = await _unitOfWork.MessagesRepository.GetAsync(id, cancellationToken);
				if (messageFromRepo == null)
				{
					LogMethodDeleteNoEntity(id);
					return NotFound();
				}

				// Lets get this guy before we remove it from the database
				// so that we can return proper object back to the caller of the method.
				var messageForSelect = await _unitOfWork.MessagesRepository.GetForSelectAsync(id, cancellationToken);


				_unitOfWork.MessagesRepository.Remove(messageFromRepo);

				if (!await _unitOfWork.SaveAsync(cancellationToken))
				{
					LogMethodDeleteFailed(id);
					return BadRequest($"Deleting message with id = {id} failed on save.");
				}

				LogMethodDeleteSuccessful(id);
				return Ok(messageForSelect);
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
