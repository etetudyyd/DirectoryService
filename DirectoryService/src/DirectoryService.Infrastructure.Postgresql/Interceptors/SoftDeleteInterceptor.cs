using DevQuestions.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace DirectoryService.Infrastructure.Postgresql.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        if(eventData.Context is null)
            return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var entries = eventData.Context.ChangeTracker
            .Entries<ISoftDeletable>()
            .Where(e => e.State == EntityState.Deleted);
        foreach (var entry in entries)
        {
            entry.State = EntityState.Modified;
            entry.Entity.Delete();

            foreach (var nav in entry.Navigations)
                nav.IsModified = false;
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}