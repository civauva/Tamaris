using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Tamaris.DAL.DbContexts;
using Tamaris.DAL.Interfaces;
using Tamaris.DAL.Interfaces.Admin;
using Tamaris.DAL.Interfaces.Msg;
using Tamaris.DAL.Repositories;
using Tamaris.DAL.Repositories.Admin;
using Tamaris.DAL.Repositories.Msg;


namespace Tamaris.DAL.Repositories
{
	public class TamarisUnitOfWork : ITamarisUnitOfWork
	{
		private readonly TamarisDbContext _context;
		private readonly ILogger<TamarisDbContext> _logger;

		public TamarisUnitOfWork(TamarisDbContext context, ILogger<TamarisDbContext> logger)
		{
			_context = context;
			_logger = logger;


			// Repositories
			RolesRepository = new RoleRepository(context);
			UsersRepository = new UserRepository(context);
			MessagesRepository = new MessageRepository(context);
		}


		public IRoleRepository RolesRepository { get; private set; }
		public IUserRepository UsersRepository { get; private set; }
		public IMessageRepository MessagesRepository { get; private set; }


		public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
		{
			try
			{
				return (await _context.SaveChangesAsync(cancellationToken) >= 0);
			}
			catch (Exception ex)
			{
				if (_logger != null)
					_logger.LogError(ex, "Error when trying to save the changes to database", null);

				return false;
			}
		}



		public bool Save()
		{
			try
			{
				return (_context.SaveChanges() >= 0);
			}
			catch (Exception ex)
			{
				if (_logger != null)
					_logger.LogError(ex, "Error when trying to save the changes to database", null);

				return false;
			}
		}


		public void Dispose()
		{
			_context.Dispose();
		}
	}
}
