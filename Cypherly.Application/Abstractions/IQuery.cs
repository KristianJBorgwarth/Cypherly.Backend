using Cypherly.Domain.Common;
using MediatR;

namespace Cypherly.Application.Abstractions;

public interface IQuery<TResponse> : IRequest<Result<TResponse>> { }