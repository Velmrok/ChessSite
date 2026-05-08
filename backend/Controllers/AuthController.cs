using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using backend.Data;
using backend.DTO.Auth;
using backend.Extensions;
using backend.Services.Helpers.Auth;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;


[Route("/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IPresenceService _presenceService;
    private readonly IQueueService _queueService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AppDbContext dbContext,
        IPresenceService presenceService, IQueueService queueService, ILogger<AuthController> logger)
    {
        _dbContext = dbContext;
        _presenceService = presenceService;
        _queueService = queueService;
        _logger = logger;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var allClaims = User.Claims.Select(c => $"{c.Type}={c.Value}");
        _logger.LogWarning($"GetMe claims: {string.Join(", ", allClaims)}");

        var sub = User.FindFirst("sub")?.Value;
        _logger.LogWarning($"GetMe sub: {sub}");
        var userId = User.FindFirst("sub")?.Value;
        if (userId == null) return Unauthorized();

        var user = await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id.ToString() == userId);
        if (user == null) return NotFound();

        await _presenceService.SetOnlineAsync(user.Id);
        var queueData = _queueService.GetUserQueueData(userId);

        return Ok(new GetMeResponse(
            user.Nickname,
            user.ProfilePictureUrl,
            user.CreatedAt,
            user.LastActive,
            user.Rating,
            new List<string>(),
            queueData
        ));
    }
}