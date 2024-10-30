using Microsoft.EntityFrameworkCore;
using Ratbags.Accounts.API.Interfaces;
using Ratbags.Accounts.API.Models.API.Tokens;
using Ratbags.Accounts.API.Models.DB;

namespace Ratbags.Accounts.API.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RefreshTokenRepository> _logger;

    public RefreshTokenRepository(
        ApplicationDbContext context,
        ILogger<RefreshTokenRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> CreateAsync(RefreshTokenCreate model)
    {
        var refreshToken = new RefreshToken
        {
            Id = model.Id,
            UserId = model.UserId,
            Created = model.Created,
            Expires = model.Expires,
            Token = model.Token,
        };

        try
        {
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException e)
        {
            _logger.LogError($"error creating refresh token for user {model.Id}: {e.Message}");
            throw;
        }
    }

    public async Task<string?> GetAsync(Guid userId)
    {
        var record = await _context.RefreshTokens
            .OrderByDescending(x => x.Created)
            .Where(x => x.UserId == userId && x.Expires >= DateTime.UtcNow)
            .Select(x=>x.Token)
            .FirstOrDefaultAsync();

        return record;
    }

    public async Task<bool> RevokeAsync(Guid userId, bool all = false)
    {
        var latestRefreshToken = await _context.RefreshTokens
            .OrderByDescending(x => x.Expires)
            .Where(x => x.UserId == userId && !x.Revoked)
            .Select(x => x.Token)
            .FirstOrDefaultAsync();

        var records = await _context.RefreshTokens
            .Where(x => x.UserId == userId
                    && (all == true || (x.Token != latestRefreshToken && !x.Revoked))
                    )
            .ToListAsync();

        if (records.Count > 0)
        {
            try
            {
                records.ForEach(x => x.Revoked = true);

                _context.RefreshTokens.UpdateRange(records);

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                _logger.LogError($"error revoking refresh token for user {userId}: {e.Message}");
                throw;
            }

            return true;
        }

        return false;
    }

    public async Task<bool> ValidateAsync(string token)
    {
        var record = await _context.RefreshTokens
            .OrderByDescending(x => x.Created)
            .Where(x => x.Token == token && x.Expires >= DateTime.UtcNow)
            .FirstOrDefaultAsync();

        return record != null;
    }
}