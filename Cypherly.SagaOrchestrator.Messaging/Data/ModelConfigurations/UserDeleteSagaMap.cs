﻿using Cypherly.SagaOrchestrator.Messaging.Saga.User.Delete;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cypherly.SagaOrchestrator.Messaging.Data.ModelConfigurations;

public sealed class UserDeleteSagaMap : SagaClassMap<UserDeleteSagaState>
{
    protected override void Configure(EntityTypeBuilder<UserDeleteSagaState> entity, ModelBuilder model)
    {
        entity.ToTable("UserDeleteSaga");

    }
}