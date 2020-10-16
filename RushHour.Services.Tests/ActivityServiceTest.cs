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

    public class ActivityServiceTest
    {
        [Fact]
        public async Task GetPaginatedActivities_PassPageAndSize_ExpectedCorrectNumber()
        {
            //Arrange
            var activities = new List<Activity>
            {
                new Activity
                {
                    Id = Guid.NewGuid(),
                    Name = "Crossfit",
                    Duration = 30,
                    Price = 15.99m
                }
            }.AsQueryable();

            var activityMock = activities.BuildMock();


            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfiguration>();
            });

            var repositoryMock = new Mock<IBaseRepository<Activity>>();

            repositoryMock
                .Setup(s => s.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>()))
                .Returns(activityMock.Object);

            var mapperMock = new Mock<IMapper>();

            mapperMock
                .Setup(s => s.ConfigurationProvider).Returns(config);
            
            var service = new ActivityService(repositoryMock.Object, mapperMock.Object);

            //Act
            var response = await service.GetPaginatedActivitiesAsync(-24444, -4444);

            //Assert
            Assert.True(response.Count() == 1);
            Assert.IsType<ActivityDto>(response.FirstOrDefault());
        }

        [Fact]
        public async Task GetActivityEntity_PassWrongId_ExpectedNull()
        {
            Activity activity = null;

            var repositoryMock = new Mock<IBaseRepository<Activity>>();

            repositoryMock
                .Setup(s => s.GetEntityAsync(It.IsAny<Expression<Func<Activity, bool>>>()))
                .Returns(Task.FromResult(activity));

            var service = new ActivityService(repositoryMock.Object, null);

            var result = await service.GetActivityByIdAsync("adasdsadsad");

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetActivityEntity_PassRightId_ExpectedActivityDto()
        {
            var activity = new Activity
            {
                Id = Guid.NewGuid(),
                Name = "Crossfit",
                Duration = 30,
                Price = 15.99m
            };

            var repositoryMock = new Mock<IBaseRepository<Activity>>();

            repositoryMock
                .Setup(s => s.GetEntityAsync(It.IsAny<Expression<Func<Activity, bool>>>()))
                .Returns(Task.FromResult(activity));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfiguration>();
            });

            var mapper = new Mapper(config);

            var service = new ActivityService(repositoryMock.Object, mapper);

            var result = await service.GetActivityByIdAsync("adasdsadsad");

            //Assert
            Assert.NotNull(result);
            Assert.IsType<ActivityDto>(result);
        }

        [Fact]
        public async Task CreateActivity_PassDto_ExpectedUpdatedDto()
        {
            var activityDto = new ActivityDto
            {
                Name = "Crossfit",
                Duration = 30,
                Price = 15.99m
            };

            var repositoryMock = new Mock<IBaseRepository<Activity>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfiguration>();
            });

            var mapper = new Mapper(config);

            var service = new ActivityService(repositoryMock.Object, mapper);

            await service.AddActivityAsync(activityDto);

            Assert.NotNull(activityDto.Id);
        }

        [Fact]
        public async Task UpdateActivity_PassDto_ExpectedUpdatedDto()
        {
            var activityDto = new ActivityDto
            {
                Name = "Fitness",
                Duration = 15,
                Price = 10.99m
            };

            var activity = new Activity
            {
                Id = Guid.NewGuid(),
                Name = "Crossfit",
                Duration = 30,
                Price = 15.99m
            };

            var repositoryMock = new Mock<IBaseRepository<Activity>>();

            repositoryMock
                .Setup(s => s.GetEntityAsync(It.IsAny<Expression<Func<Activity, bool>>>()))
                .Returns(Task.FromResult(activity));

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingConfiguration>();
            });

            var mapper = new Mapper(config);

            var service = new ActivityService(repositoryMock.Object, mapper);

            await service.UpdateActivityAsync(activityDto);

            Assert.Equal(activityDto.Name, activity.Name);
            Assert.Equal(activityDto.Duration, activity.Duration);
            Assert.Equal(activityDto.Price, activity.Price);
        }

        [Fact]
        public async Task DeleteActivity_PassId_ExpectedDeletedDto()
        {
            var activity = new Activity
            {
                Id = Guid.NewGuid(),
                Name = "Crossfit",
                Duration = 30,
                Price = 15.99m
            };

            var repositoryMock = new Mock<IBaseRepository<Activity>>();

            repositoryMock
                .Setup(s => s.GetEntityAsync(It.IsAny<Expression<Func<Activity, bool>>>()))
                .Returns(Task.FromResult(activity));

            var service = new ActivityService(repositoryMock.Object, null);

            await service.DeleteActivityAsync("someId");

            Assert.True(activity.IsDeleted);
        }
    }   
}
