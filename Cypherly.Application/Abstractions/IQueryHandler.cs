using Cypherly.Domain.Common;
using MediatR;
// ReSharper disable TypeParameterCanBeVariant

namespace Cypherly.Application.Abstractions;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>> where TQuery : IQuery<TResponse> { }