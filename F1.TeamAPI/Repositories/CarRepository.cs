using Dapper;
using F1.Models.DTOs.TeamDTOs.CarDTOs;
using F1.TeamAPI.Data;
using F1.TeamAPI.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace F1.TeamAPI.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly SqlConnection _connection;
        private readonly ILogger<CarRepository> _logger;

        public CarRepository(ConnectionDB c, ILogger<CarRepository> logger)
        {
            _connection = c.GetSlqConnection();
            _logger = logger;
        }

        public async Task CreateCarAsync(CarRequestDTO dto)
        {
            try
            {
                const string sql = @"
                    INSERT INTO Car
                    (
                        AerodynamicCoefficent,
                        PowerCoefficient,
                        Weight,
                        Model,
                        PilotId
                    )
                    VALUES
                    (
                        @AerodynamicCoefficent,
                        @PowerCoefficient,
                        @Weight,
                        @Model,
                        @PilotId
                    );
                ";

                await _connection.ExecuteAsync(sql, new
                {
                    dto.AerodynamicCoefficent,
                    dto.PowerCoefficient,
                    dto.Weight,
                    dto.Model,
                    dto.PilotId
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao registrar carro: " + ex.Message);
            }
        }

        public async Task DeleteCarAsync(int id)
        {
            try
            {
                const string sql = @"
                    UPDATE Car
                    SET IsActive = 0
                    WHERE Id = @Id;
                ";

                await _connection.ExecuteAsync(sql, new { Id = id });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao deletar carro: " + ex.Message);
            }
        }

        public async Task<List<CarResponseDTO>> GetAllCarsAsync()
        {
            try
            {
                const string sql = @"
                    SELECT
                        Id,
                        AerodynamicCoefficent,
                        PowerCoefficient,
                        Weight,
                        Model,
                        PilotId
                    FROM Car
                    WHERE IsActive = 1;
                ";

                return (await _connection.QueryAsync<CarResponseDTO>(sql)).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter carros: " + ex.Message);
            }
        }

        public async Task<List<CarResponseDTO>> GetCarsByTeamAsync(int teamId)
        {
            try
            {
                const string sql = @"
                    SELECT
                        c.Id,
                        c.AerodynamicCoefficent,
                        c.PowerCoefficient,
                        c.Weight,
                        c.Model,
                        c.PilotId
                    FROM Car c
                    INNER JOIN Pilot p ON p.Id = c.PilotId
                    INNER JOIN Team t ON t.Id = p.TeamId
                    WHERE
                        t.Id = @TeamId
                        AND c.IsActive = 1
                        AND p.Status = 1
                        AND t.IsActive = 1;
                ";

                return (await _connection.QueryAsync<CarResponseDTO>(
                    sql,
                    new { TeamId = teamId }
                )).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter carros do time: " + ex.Message);
            }
        }

        public async Task UpdateCarAsync(int id, CarRequestDTO dto)
        {
            try
            {
                const string sql = @"
                    UPDATE Car
                    SET
                        AerodynamicCoefficent = @AerodynamicCoefficent,
                        PowerCoefficient = @PowerCoefficient,
                        Weight = @Weight,
                        Model = @Model,
                        PilotId = @PilotId
                    WHERE
                        Id = @Id
                        AND IsActive = 1;
                ";

                await _connection.ExecuteAsync(sql, new
                {
                    Id = id,
                    dto.AerodynamicCoefficent,
                    dto.PowerCoefficient,
                    dto.Weight,
                    dto.Model,
                    dto.PilotId
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar carro: " + ex.Message);
            }
        }

        public async Task UpdateCACPByPilotIdAsync(int pilotId, decimal ca, decimal cp)
        {
            try
            {
                const string sql = @"
                    UPDATE Car
                    SET
                        AerodynamicCoefficent = @AerodynamicCoefficent,
                        PowerCoefficient = @PowerCoefficient
                    WHERE
                        PilotId = @PilotId
                        AND IsActive = 1;
                ";

                await _connection.ExecuteAsync(sql, new
                {
                    PilotId = pilotId,
                    AerodynamicCoefficent = ca,
                    PowerCoefficient = cp
                });
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar CA e CP: " + ex.Message);
            }
        }

        public async Task<List<CarHistoryResponseDTO>> GetAllCarsHistoryAsync()
        {
            var sql =
                @"SELECT Id as CarId, Model as CarModel, AerodynamicCoefficent as CarAerodynamicCoefficent, PowerCoefficient as CarPowerCoefficient, PilotId 
                    FROM Car";

            var cars = await _connection.QueryAsync<CarHistoryResponseDTO>(sql);
            return cars.ToList();
        }
    }
}
