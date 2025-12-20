using Dapper;
using F1.Models.DTOs.HistoryDTOs;
using F1.Models.DTOs.TeamDTOs.TeamDTOs;
using F1.TeamAPI.Data;
using F1.TeamAPI.DTOs.TeamCreation;
using F1.TeamAPI.Repositories.Interfaces;
using F1.TeamAPI.Services.Generators;
using Microsoft.Data.SqlClient;

namespace F1.TeamAPI.Repositories
{
    public class TeamRepository : ITeamRepository
    {
        public readonly SqlConnection _connection;
        public readonly ILogger _logger;

        public TeamRepository(ConnectionDB c, ILogger logger)
        {
            _connection = c.GetSlqConnection();
            _logger = logger;
        }

        public async Task<List<TeamResponseDTO>> GetAllTeamsAsync()
        {
            try
            {
                var sql = @"SELECT Id, Name, Points, Placement 
                           FROM Team 
                           WHERE IsActive = 1;";
                var teams = (await _connection.QueryAsync<TeamResponseDTO>(sql)).ToList();

                return teams;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter times: " + ex.Message);
            }
        }

        public async Task<List<TeamResponseDTO>> GetAllTeamsFinalAsync()
        {
            try
            {
                var sql = @"SELECT Id, Name, Points, Placement 
                           FROM Team 
                           WHERE IsActive = 1
                           ORDER BY Placement;";
                var teams = (await _connection.QueryAsync<TeamResponseDTO>(sql)).ToList();

                return teams;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter times: " + ex.Message);
            }

        }

        public async Task CreateTeamAsync(TeamRequestDTO dto)
        {
            try
            {

                const string sql = @"INSERT INTO Team (Name)
                                 VALUES (@Name);";

                await _connection.ExecuteAsync(sql, new
                {
                    dto.Name
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao criar equipe: " + ex.Message);
            }
        }

        public async Task DeleteTeamAsync(int id)
        {
            try
            {
                var sql = "UPDATE Team Set IsActive = 0 WHERE Id = @Id";

                await _connection.ExecuteAsync(sql, new { id });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar time" + ex.Message);
            }
        }

        public async Task UpdateTeamPlacementAndPointsAsync(int teamId, int placement, int points)
        {
            const string sql = @"
                               UPDATE Team
                               SET 
                               Placement = @Placement,
                               Points = @Points
                               WHERE Id = @TeamId;";

            await _connection.ExecuteAsync(sql, new
            {
                TeamId = teamId,
                Placement = placement,
                Points = points
            });
        }

        public async Task CreateFullTeamAsync(CreateFullTeamRequestDTO dto)
        {
            using var transaction = _connection.BeginTransaction();

            try
            {
                //TEAM
                var teamId = await _connection.ExecuteScalarAsync<int>(
                    @"INSERT INTO Team (Name)
                      VALUES (@Name);
                      SELECT CAST(SCOPE_IDENTITY() AS INT);",
                    new { dto.Team.Name },
                    transaction
                );

                //PILOTS
                var pilotIds = new List<int>();

                foreach (var pilot in dto.Pilots)
                {
                    var pilotId = await _connection.ExecuteScalarAsync<int>(
                        @"INSERT INTO Pilot
                        (Name, Surname, Weight, Age, IdentificationNumber,
                         Experience, Handicap, TeamId)
                        VALUES
                        (@Name, @Surname, @Weight, @Age, @IdentificationNumber, 
                         @Experience, @Handicap, @TeamId);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);",
                        new
                        {
                            pilot.Name,
                            pilot.Surname,
                            pilot.Weight,
                            pilot.Age,
                            pilot.IdentificationNumber,
                            pilot.Experience,
                            pilot.Handicap,
                            TeamId = teamId
                        },
                        transaction
                    );

                    pilotIds.Add(pilotId);
                }

                //CARS 
                var carIds = new List<int>();

                for (int i = 0; i < dto.Cars.Count; i++)
                {
                    var car = dto.Cars[i];

                    var carId = await _connection.ExecuteScalarAsync<int>(
                        @"INSERT INTO Car
                        (AerodynamicCoefficent, PowerCoefficient, Weight, Model, PilotId)
                        VALUES
                        (@AerodynamicCoefficent, @PowerCoefficient, @Weight, @Model, @PilotId);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);",
                        new
                        {
                            car.AerodynamicCoefficent,
                            car.PowerCoefficient,
                            car.Weight,
                            car.Model,
                            PilotId = pilotIds[i]
                        },
                        transaction
                    );

                    carIds.Add(carId);
                }

                //ENGINEERS
                foreach (var engineer in dto.Engineers)
                {
                    await _connection.ExecuteAsync(
                        @"INSERT INTO Engineer
                        (Name, Surname, Age, Experience, Type, TeamId, CarId)
                        VALUES
                        (@Name, @Surname, @Age, @Experience, @Type, @TeamId, @CarId);",
                        new
                        {
                            engineer.Name,
                            engineer.Surname,
                            engineer.Age,
                            engineer.Experience,
                            engineer.Type,
                            TeamId = teamId,
                            engineer.CarId
                        },
                        transaction
                    );
                }

                //BOSSES
                foreach (var boss in dto.Bosses)
                {
                    await _connection.ExecuteAsync(
                        @"INSERT INTO Boss
                        (Name, Surname, Age, Type, TeamId)
                        VALUES
                        (@Name, @Surname, @Age, @Type, @TeamId);",
                        new
                        {
                            boss.Name,
                            boss.Surname,
                            boss.Age,
                            boss.Type,
                            TeamId = teamId
                        },
                        transaction
                    );
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task CreateFullTeamRandomAsync(CreateFullTeamRequestDTO dto)
        {
            using var transaction = _connection.BeginTransaction();

            try
            {
                var TeamName = TeamGenerator.NameGenerator();
                //TEAM
                var teamId = await _connection.ExecuteScalarAsync<int>(
                    @"INSERT INTO Team (Name)
                      VALUES (@Name);
                      SELECT CAST(SCOPE_IDENTITY() AS INT);",
                    new { TeamName },
                    transaction
                );

                //PILOTS
                var pilotIds = new List<int>();

                foreach (var pilot in dto.Pilots)
                {
                    var pilotName = PilotGenerator.PilotName();
                    var pilotSurname = PilotGenerator.PilotSurname();
                    var pilotWeight = PilotGenerator.PilotWeight();
                    var pilotAge = PilotGenerator.PilotAge();

                    var pilotId = await _connection.ExecuteScalarAsync<int>(
                        @"INSERT INTO Pilot
                        (Name, Surname, Weight, Age, IdentificationNumber,
                         Experience, Handicap, TeamId)
                        VALUES
                        (@Name, @Surname, @Weight, @Age, @IdentificationNumber, 
                         @Experience, @Handicap, @TeamId);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);",
                        new
                        {
                            pilotName,
                            pilotSurname,
                            pilot.Weight,
                            pilot.Age,
                            pilot.IdentificationNumber,
                            pilot.Experience,
                            pilot.Handicap,
                            TeamId = teamId
                        },
                        transaction
                    );

                    pilotIds.Add(pilotId);
                }

                //CARS 
                var carIds = new List<int>();

                for (int i = 0; i < dto.Cars.Count; i++)
                {
                    var car = dto.Cars[i];

                    var carId = await _connection.ExecuteScalarAsync<int>(
                        @"INSERT INTO Car
                        (AerodynamicCoefficent, PowerCoefficient, Weight, Model, PilotId)
                        VALUES
                        (@AerodynamicCoefficent, @PowerCoefficient, @Weight, @Model, @PilotId);
                        SELECT CAST(SCOPE_IDENTITY() AS INT);",
                        new
                        {
                            car.AerodynamicCoefficent,
                            car.PowerCoefficient,
                            car.Weight,
                            car.Model,
                            PilotId = pilotIds[i]
                        },
                        transaction
                    );

                    carIds.Add(carId);
                }

                //ENGINEERS
                foreach (var engineer in dto.Engineers)
                {
                    await _connection.ExecuteAsync(
                        @"INSERT INTO Engineer
                        (Name, Surname, Age, Experience, Type, TeamId, CarId)
                        VALUES
                        (@Name, @Surname, @Age, @Experience, @Type, @TeamId, @CarId);",
                        new
                        {
                            engineer.Name,
                            engineer.Surname,
                            engineer.Age,
                            engineer.Experience,
                            engineer.Type,
                            TeamId = teamId,
                            engineer.CarId
                        },
                        transaction
                    );
                }

                //BOSSES
                foreach (var boss in dto.Bosses)
                {
                    await _connection.ExecuteAsync(
                        @"INSERT INTO Boss
                        (Name, Surname, Age, Type, TeamId)
                        VALUES
                        (@Name, @Surname, @Age, @Type, @TeamId);",
                        new
                        {
                            boss.Name,
                            boss.Surname,
                            boss.Age,
                            boss.Type,
                            TeamId = teamId
                        },
                        transaction
                    );
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<List<TeamHistoryResponseDTO>> GetAllTeamsHistoryAsync()
        {
            var sql =
                @"SELECT Id, Name, Points, Placement, IsActive, TeamId
                    FROM Team";

            var teams = await _connection.QueryAsync<TeamHistoryResponseDTO>(sql);
            return teams.ToList();
        }
    }
}
