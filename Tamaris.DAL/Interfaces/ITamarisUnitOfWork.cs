using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Tamaris.Entities;
using Tamaris.DAL.Interfaces.Admin;
using Tamaris.DAL.Interfaces.Msg;



namespace Tamaris.DAL.Interfaces
{
	public interface ITamarisUnitOfWork : IDisposable
	{
		IRoleRepository RolesRepository { get; }
		IUserRepository UsersRepository { get; }
		IMessageRepository MessagesRepository { get; }

		Task<bool> SaveAsync(CancellationToken cancellationToken = default);
		bool Save();
	}
}
