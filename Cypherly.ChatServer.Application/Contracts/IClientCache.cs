using Cypherly.Application.Contracts.Cache;
using Cypherly.ChatServer.Domain.Aggregates;

namespace Cypherly.ChatServer.Application.Contracts;

public interface IClientCache : ICache<Client> 
{
}