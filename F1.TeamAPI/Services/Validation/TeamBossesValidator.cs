using F1.Models.Enums;
using F1.TeamAPI.Repositories.Interfaces;

public class TeamBossesValidator
{
    private readonly IBossRepository _bossRepository;

    public TeamBossesValidator(IBossRepository bossRepository)
    {
        _bossRepository = bossRepository;
    }

    public async Task<bool> ValidateAsync(int teamId)
    {
        var bosses = await _bossRepository.GetBossesByTeamAsync(teamId);

        // Deve ter exatamente 2 chefes
        if (bosses.Count != 2)
            return false;

        // Deve ter exatamente dois tipos diferentes
        var types = bosses
            .Select(b => b.Type)
            .Distinct()
            .ToList();

        if (types.Count != 2)
            return false;

        // Deve conter BC (1) e SC (2)
        if (!types.Contains((int)BossType.BB))
            return false;

        if (!types.Contains((int)BossType.SB))
            return false;

        return true;
    }
}
