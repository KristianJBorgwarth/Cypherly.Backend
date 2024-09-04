using Cypherly.Domain.Common;
using MediatR;

namespace Cypherly.Application.Abstractions;

public interface ICommand : IRequest<Result> { }
public interface ICommand<TResponse> : IRequest<Result<TResponse>> { }