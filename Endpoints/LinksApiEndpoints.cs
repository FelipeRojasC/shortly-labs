using Shortly.Application.DTOs;
using Shortly.Application.Interfaces;

namespace Shortly.Endpoints;

public static class LinksApiEndpoints
{
    public static void MapLinksApi(this WebApplication app)
    {
        var group = app.MapGroup("/api");

        group.MapPost("/urls", async (HttpRequest request, ILinkService linkService, IUserRepository userRepository) =>
        {
            var body = await request.ReadNegotiatedBody<CreateLinkRequest>();
            if (body is null || string.IsNullOrWhiteSpace(body.Url))
                return new ErrorResponse { Error = "Url is required" }.Negotiated(400);

            long userId = body.UserId ?? 0;
            if (userId <= 0)
            {
                var admin = await userRepository.GetByEmailAsync("admin@shortly.disc.cl");
                if (admin is null)
                    return new ErrorResponse { Error = "No default user available" }.Negotiated(400);
                userId = admin.Id;
            }

            try
            {
                var created = await linkService.CreateLink(body.Url, userId);
                return created.Negotiated(201);
            }
            catch (ArgumentException ex)
            {
                return new ErrorResponse { Error = ex.Message }.Negotiated(400);
            }
        });

        group.MapGet("/urls", async (ILinkService linkService) =>
        {
            var links = await linkService.GetAllLinks();
            return links.Negotiated(200);
        });

        group.MapGet("/urls/{id:long}", async (long id, ILinkService linkService) =>
        {
            try
            {
                var link = await linkService.GetLinkById(id);
                return link.Negotiated(200);
            }
            catch (KeyNotFoundException)
            {
                return new ErrorResponse { Error = "Link not found" }.Negotiated(404);
            }
        });

        group.MapDelete("/urls/{id:long}", async (long id, ILinkService linkService) =>
        {
            try
            {
                await linkService.DeleteLink(id);
                return Results.StatusCode(204);
            }
            catch (KeyNotFoundException)
            {
                return new ErrorResponse { Error = "Link not found" }.Negotiated(404);
            }
        });

        group.MapGet("/stats", async (ILinkService linkService) =>
        {
            var links = await linkService.GetAllLinks();

            var stats = new StatsResponse
            {
                TotalLinks = links.Count,
                TotalClicks = links.Sum(l => l.Clicks),
                TopLinks = links.OrderByDescending(l => l.Clicks).Take(5).ToList()
            };

            return stats.Negotiated(200);
        });
    }
}