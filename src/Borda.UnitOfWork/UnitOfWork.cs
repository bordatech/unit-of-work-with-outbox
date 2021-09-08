using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Borda.UnitOfWork
{
    public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly ICapPublisher _publisher;
        private IDbContextTransaction _transaction;
        private readonly List<EventBase> _events;

        public UnitOfWork(TContext context, ICapPublisher publisher)
        {
            _context = context;
            _publisher = publisher;
            _events = new List<EventBase>();
        }

        public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = _context.Database.BeginTransaction(_publisher);

            return Task.CompletedTask;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            await _transaction.CommitAsync(cancellationToken);
        }

        public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        {
            int result = 0;

            if (_transaction is null && _events.Any())
            {
                await BeginTransactionAsync(cancellationToken);
                result = await _context.SaveChangesAsync(cancellationToken);
                PublishEvents();
                await CommitTransactionAsync(cancellationToken);
            }
            else
            {
                result = await _context.SaveChangesAsync(cancellationToken);
            }

            return result;
        }

        public void AddEvent(EventBase @event)
        {
            _events.Add(@event);
        }

        public virtual void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
            _events.Clear();
        }

        private void PublishEvents()
        {
            foreach (var @event in _events)
            {
                var attr = (EventKeyAttribute) Attribute.GetCustomAttributes(@event.GetType())
                    .FirstOrDefault(i => i is EventKeyAttribute);

                if (attr is not null)
                {
                    _publisher.Publish(attr.Key, @event);    
                }
            }

            _events.Clear();
        }
    }
}