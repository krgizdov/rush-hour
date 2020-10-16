namespace RushHour.Services.Tests
{
    using AutoMapper;
    using MockQueryable.Moq;
    using Moq;
    using RushHour.Data.Entities;
    using RushHour.Domain.Entities;
    using RushHour.Domain.Repositories;
    using RushHour.Domain.Services;
    using RushHour.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Xunit;

    public class AppointmentServiceTest
    {
        [Fact]
        public async Task GetPaginatedAppointments_PassPageAndSizeIsDeletedTrue_ExpectedCorrectNumber()
        {
            //Arrange
            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMinutes(60)
                },
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMinutes(60),
                    IsDeleted = true
                }
            }.AsQueryable();

            var appointmentMock = appointments.BuildMock();


            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfiguration>();
            });

            var repositoryMock = new Mock<IBaseRepository<Appointment>>();

            repositoryMock
                .Setup(s => s.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(appointmentMock.Object);

            var mapperMock = new Mock<IMapper>();

            mapperMock
                .Setup(s => s.ConfigurationProvider).Returns(config);

            var service = new AppointmentService(repositoryMock.Object, mapperMock.Object, null);

            //Act
            var response = await service.GetPaginatedAppointmentsAsync(1, 10);

            //Assert
            Assert.True(response.Count() == 1);
            Assert.IsType<AppointmentDto>(response.FirstOrDefault());
        }

        [Fact]
        public async Task GetPaginatedAppointmentsForUser_PassPageAndSizeIsDeletedTrueUserId_ExpectedCorrectNumber()
        {
            //Arrange
            var appointments = new List<Appointment>
            {
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMinutes(60),
                    ApplicationUserId = "1"
                },
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMinutes(60),
                    ApplicationUserId = "2",
                },
                new Appointment
                {
                    Id = Guid.NewGuid(),
                    StartDate = DateTime.UtcNow,
                    EndDate = DateTime.UtcNow.AddMinutes(60),
                    ApplicationUserId = "3",
                    IsDeleted = true
                }
            }.AsQueryable();

            var appointmentMock = appointments.BuildMock();


            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfiguration>();
            });

            var repositoryMock = new Mock<IBaseRepository<Appointment>>();

            repositoryMock
                .Setup(s => s.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(appointmentMock.Object);

            var mapperMock = new Mock<IMapper>();

            mapperMock
                .Setup(s => s.ConfigurationProvider).Returns(config);

            var service = new AppointmentService(repositoryMock.Object, mapperMock.Object, null);

            //Act
            var response = await service.GetPaginatedAppointmentsAsync("1", 1, 10);

            //Assert
            Assert.True(response.Count() == 1);
            Assert.IsType<AppointmentDto>(response.FirstOrDefault());
        }

        [Fact]
        public async Task GetAppointmentEntity_PassWrongId_ExpectedNull()
        {
            Appointment appointment = null;

            var repositoryMock = new Mock<IBaseRepository<Appointment>>();

            repositoryMock
                .Setup(s => s.GetEntityAsync(It.IsAny<Expression<Func<Appointment, bool>>>()))
                .Returns(Task.FromResult(appointment));

            var service = new AppointmentService(repositoryMock.Object, null, null);

            var result = await service.GetAppointmentByIdAsync("1");

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAppointmentEntity_PassRightId_ExpectedAppointmentDto()
        {
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMinutes(60)
            };

            var repositoryMock = new Mock<IBaseRepository<Appointment>>();

            repositoryMock
                .Setup(s => s.GetEntityAsync(It.IsAny<Expression<Func<Appointment, bool>>>()))
                .Returns(Task.FromResult(appointment));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfiguration>();
            });

            var mapper = new Mapper(config);

            var service = new AppointmentService(repositoryMock.Object, mapper, null);

            var result = await service.GetAppointmentByIdAsync("1");

            //Assert
            Assert.NotNull(result);
            Assert.IsType<AppointmentDto>(result);
        }

        [Fact]
        public async Task GetAppointmentEntityForUser_PassWrongId_ExpectedNull()
        {
            Appointment appointment = null;

            var repositoryMock = new Mock<IBaseRepository<Appointment>>();

            repositoryMock
                .Setup(s => s.GetEntityAsync(It.IsAny<Expression<Func<Appointment, bool>>>()))
                .Returns(Task.FromResult(appointment));

            var service = new AppointmentService(repositoryMock.Object, null, null);

            var result = await service.GetAppointmentByIdAsync("1", "2");

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAppointmentEntityForUser_PassRightId_ExpectedAppointmentDto()
        {
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddMinutes(60)
            };

            var repositoryMock = new Mock<IBaseRepository<Appointment>>();

            repositoryMock
                .Setup(s => s.GetEntityAsync(It.IsAny<Expression<Func<Appointment, bool>>>()))
                .Returns(Task.FromResult(appointment));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfiguration>();
            });

            var mapper = new Mapper(config);

            var service = new AppointmentService(repositoryMock.Object, mapper, null);

            var result = await service.GetAppointmentByIdAsync("1", "2");

            //Assert
            Assert.NotNull(result);
            Assert.IsType<AppointmentDto>(result);
        }

        [Fact]
        public async Task AddAppointment_ActivityNotFound_ExpectedException()
        {
            ActivityDto activityDto = null;

            var serviceMock = new Mock<IActivityService>();

            serviceMock
                .Setup(s => s.GetActivityByIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(activityDto));

            var appointmentService = new AppointmentService(null, null, serviceMock.Object);

            var activityIds = new string[] { "1", "2" };

            Func<Task> task = async() => await appointmentService.AddAppointmentAsync(activityIds, null);

            await Assert.ThrowsAsync<ArgumentException>(task);
        }
    }
}
