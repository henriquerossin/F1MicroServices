using F1.Models.Enums;
using F1.TeamAPI.Repositories.Interfaces;
using MongoDB.Bson;

namespace F1.TeamAPI.Services.Validation
{
    public class TeamBossesValidator
    {
        private readonly IBossRepository _bossRepository;

        public TeamBossesValidator(IBossRepository bossRepository)
        {
            _bossRepository = bossRepository;
        }

        public async Task ValidateAsync(int teamId)
        {
            var bosses = await _bossRepository.GetBossesByTeamAsync(teamId);

            if (bosses.Count != 2)
                throw new Exception($"Equipe {teamId} não possui 2 chefes");

            var types = bosses.Select(b => b.Type).Distinct().ToList();

            //if (types.Count != 2 ||
            //    !types.Contains(((int)BossType.BC).ToString()) ||
            //    !types.Contains(((int)BossType.SC).ToString()))
            //{
            //    throw new Exception($"Equipe {teamId} deve ter chefes BC e SC");
            //}
        }
    }

}
