using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ProjectNest.Domain.Stores;
using ProjectNest.Infrastructure.Persistence;

namespace ProjectNest.Api.Endpoints;

public static class StoreEndpoints
{
    public static void MapStoreEndpoints(this WebApplication app)
    {
        app.MapGet("/api/stores", async (
            ProjectNestDbContext dbContext,
            CancellationToken cancellationToken) =>
                await dbContext.Stores
                    .AsNoTracking()
                    .ToListAsync(cancellationToken))
            .WithName("GetStores")
            .WithSummary("Retrieve stores")
            .WithDescription("Returns all stores.")
            .Produces<List<Store>>(StatusCodes.Status200OK);

        app.MapPost("/api/stores", async Task<Results<Created<Store>, ValidationProblem>> (
            CreateStoreRequest request,
            ProjectNestDbContext dbContext,
            CancellationToken cancellationToken) =>
        {
            Store store;
            try
            {
                store = request.Status is { } status
                    ? new Store(request.BrandId, request.MallId, status)
                    : new Store(request.BrandId, request.MallId);
            }
            catch (ArgumentException exception)
            {
                return CreateValidationProblem(
                    exception.ParamName ?? "request",
                    exception.Message);
            }

            var validationErrors = new Dictionary<string, string[]>();

            if (!await dbContext.Brands.AnyAsync(
                    brand => brand.Id == request.BrandId,
                    cancellationToken))
            {
                validationErrors["brandId"] = ["Brand does not exist."];
            }

            if (!await dbContext.Malls.AnyAsync(
                    mall => mall.Id == request.MallId,
                    cancellationToken))
            {
                validationErrors["mallId"] = ["Mall does not exist."];
            }

            if (validationErrors.Count > 0)
            {
                return TypedResults.ValidationProblem(validationErrors);
            }

            dbContext.Stores.Add(store);
            await dbContext.SaveChangesAsync(cancellationToken);

            return TypedResults.Created("/api/stores", store);
        })
            .WithName("CreateStore")
            .WithSummary("Create a store")
            .WithDescription("Creates a store for an existing brand and mall.")
            .Produces<Store>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }

    private static ValidationProblem CreateValidationProblem(string field, string message) =>
        TypedResults.ValidationProblem(new Dictionary<string, string[]>
        {
            [field] = [message]
        });
}

/// <summary>Request data for creating a Store.</summary>
public sealed record CreateStoreRequest(
    Guid BrandId,
    Guid MallId,
    StoreStatus? Status = null);
