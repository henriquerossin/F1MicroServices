using F1.EngineeringAPI.Controllers;
using F1.EngineeringAPI.Services;
using F1.Models.DTOs.EngineeringDTOs;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Testcontainers.RabbitMq;

namespace F1.Tests.EngineeringTests
{
    public class EngineeringServiceTest
    {
        [Fact]
        public async Task ProcessMessage_ShouldUpdateCoefficientsCorrectly()
        {
            // Arrange
            var message = new EngineeringResponseDTO
            {
                CarId = 1,
                AerodynamicCoefficent = 10m,
                PowerCoefficient = 8m,
                EngineerAerodynamicExperience = 0.5m,
                EngineerPowerExperience = 0.4m,
                PilotExperience = 2m,
                Handicap = 5m,
                Type = 4 
            };

            // Act 
            var result = await ProcessMessageLogic(message);

            // Assert
            Assert.Equal(1, result.CarId);
            Assert.True(result.AerodynamicCoefficent != 10m); 
            Assert.True(result.Handicap < 5m); 
            Assert.True(result.PD > 0); 
        }

        private async Task<EngineeringResponseDTO> ProcessMessageLogic(EngineeringResponseDTO info)
        {
            
            decimal newCa, newCp, newHandicap, randomCa, randomCp;
            Random random = new Random();

            randomCa = (decimal)((random.NextDouble() * 2) - 1);
            randomCp = (decimal)((random.NextDouble() * 2) - 1);

            newCa = info.AerodynamicCoefficent + (info.EngineerAerodynamicExperience * randomCa);
            newCp = info.PowerCoefficient + (info.EngineerPowerExperience * randomCp);
            newHandicap = info.Handicap - (info.PilotExperience * 0.5m);

            decimal pd = 0m, randomPd;
            if (info.Type == 4 || info.Type == 5)
            {
                randomPd = (decimal)(random.Next(1, 11));
                pd = (info.AerodynamicCoefficent * 0.4m) + (info.PowerCoefficient * 0.4m)
                     - info.Handicap + randomPd;
            }

            return new EngineeringResponseDTO
            {
                CarId = info.CarId,
                AerodynamicCoefficent = newCa,
                PowerCoefficient = newCp,
                //resto dos campos iguais
                Handicap = newHandicap,
                Type = info.Type,
                PD = pd
            };
        }
    }
}
