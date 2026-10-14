namespace Nemetscheck.UserDataAccessService.Domain.Common;

public abstract class AggregateRoot<TId> : Entity<TId>
    where TId : struct;
