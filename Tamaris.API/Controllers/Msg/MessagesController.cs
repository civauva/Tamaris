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
using Microsoft.AspNetCore.SignalR;
using Tamaris.API.Hubs;

namespace Tamaris.API.Controllers.Msg
{
	[TamarisController(Endpoint = "Msg/")]
	[Produces("application/json")]
	public class MessagesController : BaseController
	{
		private readonly ITamarisUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		private readonly string _defaultGetSingleRoute = "GetMsgsMessage";

		public MessagesController(ITamarisUnitOfWork unitOfWork, IMapper mapper, IHubContext<MessageHub, IMessageHub> messageHub)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			hubContext = messageHub;
		}


		#region SignalR emitting events
		private readonly IHubContext<MessageHub, IMessageHub> hubContext;
		#endregion SignalR emitting events


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
		[HttpGet("ForChat")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageForChat))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<IEnumerable<MessageForChat>>> GetForChat(CancellationToken cancellationToken = default)
		{
			try
			{
				var messages = await _unitOfWork.MessagesRepository.GetAllForChatAsync(cancellationToken);

				if (messages != null && messages.Any())
				{
					return Ok(messages);
				}

				return NoContent(); // No messages for given index/page combination
			}
			catch (TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}
			catch (Exception ex)
			{
				return NoContent();
			}
		}


		// GET
		/// <summary>
		/// Gets conversation between those two users
		/// </summary>
		/// <param name="username1">User 1</param>
		/// <param name="username2">User 2</param>
		/// <param name="countLastMessages">How many last messages we would like to fetch? (by default 10)</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <returns></returns>
		/// <response code="200">Returns list of selected messages</response>
		/// <response code="204">If there are no messages for given PageIndex/PageSize/SearchString combination</response>
		/// <response code="401">If the user is not authorized to access this resource</response>
		[HttpGet("Conversation")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(MessageForChat))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		public async Task<ActionResult<IEnumerable<MessageForChat>>> GetConversation(string username1, string username2, int countLastMessages = 10, CancellationToken cancellationToken = default)
		{
			try
			{
				var messages = await _unitOfWork.MessagesRepository.GetAllBetweenAsync(username1, username2, countLastMessages, cancellationToken);

				if (messages != null && messages.Any())
				{
					return Ok(messages);
				}

				return NoContent(); // No messages for given index/page combination
			}
			catch (TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}
			catch (Exception ex)
			{
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



		// GET api/MessageForSelect/5
		/// <summary>
		/// Gets count of unread messages
		/// </summary>
		/// <remarks>This method returns message with given key</remarks>
		/// <param name="receiverUsername">We are obtaining the count for this user as receiver</param>
		/// <param name="senderUsername">If passed, this parameter is used to filter only the messages of this particular sender</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="200">Returns found message</response>
		/// <response code="204">If there is no message found for given key</response>   
		[HttpGet("CountUnread")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<ActionResult<int>> GetCountUnreadMessages(string receiverUsername, string senderUsername, CancellationToken cancellationToken = default)
		{
			try
			{
				var messagesCount = await _unitOfWork.MessagesRepository.GetCountUnreadMessagesAsync(receiverUsername, senderUsername, cancellationToken);

				return Ok(messagesCount);
			}
			catch (TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}
		}



		// GET api/MessageForSelect/5
		/// <summary>
		/// Gets count of unread messages
		/// </summary>
		/// <remarks>This method returns message with given key</remarks>
		/// <param name="receiverEmail">We are obtaining the count for this user as receiver</param>
		/// <param name="senderEmail">If passed, this parameter is used to filter only the messages of this particular sender</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="200">Returns found message</response>
		/// <response code="204">If there is no message found for given key</response>   
		[HttpGet("CountUnread/ByEmail")]
		[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<ActionResult<int>> GetCountUnreadMessagesByEmail(string receiverEmail, string senderEmail, CancellationToken cancellationToken = default)
		{
			try
			{
				var messagesCount = await _unitOfWork.MessagesRepository.GetCountUnreadMessagesByEmailAsync(receiverEmail, senderEmail, cancellationToken);

				return Ok(messagesCount);
			}
			catch (TaskCanceledException)
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
		public async Task<ActionResult<MessageForSelect>> SendMessage([FromBody] MessageForInsert message, CancellationToken cancellationToken = default)
		{
			LogMethodCreateEntry(message);

			if (message == null)
			{
				LogMethodCreateBadRequest(message);
				return BadRequest("Cannot create an empty message.");
			}


			// Checks the validation in the data annotation of the data model
			//if (!ModelState.IsValid)
			//{
			//	LogMethodCreateInvalid(message);
			//	return new UnprocessableEntityObjectResult(ModelState);
			//}


			try
			{
				// TODO: This is not the most optimal way of obtaining the ID's
				var userSender = await _unitOfWork.UsersRepository.GetByUsernameAsync(message.SenderUsername);
				var userReceiver = await _unitOfWork.UsersRepository.GetByUsernameAsync(message.ReceiverUsername);

				// Map the passed objects to database entity/entities
				var messageEntity = new Message
                {
					ReceiverUserId = userReceiver.Id,
					SenderUserId = userSender.Id,
					Subject = message.Subject,
					MessageText = message.MessageText,
					SentOn = DateTime.Now,
					IsRead = false,
                };

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

				// And then emit it to the SignalR clients
				await this.hubContext.Clients.All.MessageSent(messageToReturn.Id);

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
		// POST api/message
		/// <summary>
		/// Updates selected messages as read
		/// </summary>
		/// <remarks>This method marks selected messages as read.</remarks>
		/// <param name="receiverEmail">Messages where this is receiver e-mail...</param>
		/// <param name="senderEmail">and messages where this is sender e-mail</param>
		/// <param name="cancellationToken">Token used to explicitly cancel the request.</param>
		/// <response code="201">If creation of the message was successful.</response>
		/// <response code="400">If passed message was null or the creation of the message was not successful.</response>   
		/// <response code="422">If the validation of passed message failed.</response>   
		[HttpPost("MarkRead")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
		public async Task<ActionResult> MarkRead(string receiverEmail, string senderEmail, CancellationToken cancellationToken = default)
		{
			if (string.IsNullOrEmpty(receiverEmail) || string.IsNullOrEmpty(senderEmail))
				return BadRequest("No email provided.");

			try
			{
				// First mark it
				await _unitOfWork.MessagesRepository.MarkReadAsync(receiverEmail, senderEmail);

				// Then try to save it
				if (!await _unitOfWork.SaveAsync(cancellationToken))
					return BadRequest("Updating messages read-status failed on save.");

				return Ok();
			}
			catch (TaskCanceledException)
			{
				LogVerbose("User cancelled action.");
				return NoContent();
			}
			catch(Exception ex)
            {
				return BadRequest(ex);
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
