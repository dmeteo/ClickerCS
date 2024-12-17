﻿using CSharpClicker.Domain;
using CSharpClicker.DomainServices;
using CSharpClicker.Infrastructure.Abstractions;
using CSharpClicker.UseCases.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CSharpClicker.UseCases.BuyBoost;

public class BuyBoostCommandHandler : IRequestHandler<BuyBoostCommand, ScoreDto>
{
    private readonly ICurrentUserAccessor currentUserAccessor;
    private readonly IAppDbContext appDbContext;

    public BuyBoostCommandHandler(ICurrentUserAccessor currentUserAccessor, IAppDbContext appDbContext)
    {
        this.currentUserAccessor = currentUserAccessor;
        this.appDbContext = appDbContext;
    }

    public async Task<ScoreDto> Handle(BuyBoostCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUserAccessor.GetCurrentUserId();
        var user = await appDbContext.ApplicationUsers
            .Include(user => user.UserBoosts)
            .ThenInclude(ub => ub.Boost)
            .FirstAsync(user => user.Id == userId);

        var boost = await appDbContext.Boosts
            .FirstAsync(b => b.Id == request.BoostId);

        var userBoost = user.UserBoosts.FirstOrDefault(ub => ub.BoostId == request.BoostId);


        var price = 0L;
        if (userBoost != null)
        {
            price = userBoost.CurrentPrice;
            userBoost.Quantity++;
            userBoost.CurrentPrice = Convert.ToInt64(userBoost.CurrentPrice * Constants.BoostCostModifier);
        }
        else
        {
            price = boost.Price;
            var newUserBoost = new UserBoost()
            {
                Boost = boost,
                CurrentPrice = Convert.ToInt64(boost.Price * Constants.BoostCostModifier),
                Quantity = 1,
                User = user,
            };
            await appDbContext.UserBoosts.AddAsync(newUserBoost);
        }

        if (price > user.CurrentScore)
        {
            throw new InvalidCastException("Not enough score to buy a boost");
        }

        user.CurrentScore -= price;

        await appDbContext.SaveChangesAsync();

        return new ScoreDto
        {
            CurrentScore = user.CurrentScore,
            RecordScore = user.RecordScore,
            ProfitPerClick = user.UserBoosts.GetProfit(),
            ProfitPerSecond = user.UserBoosts.GetProfit(shouldCalculateAutoBoosts: true)
        };
    }
}
