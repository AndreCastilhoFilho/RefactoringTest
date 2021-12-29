using AutoFixture;
using FluentAssertions;
using LegacyApp;
using LegacyApp.CreditStrategies;
using LegacyApp.DataAccess;
using LegacyApp.Models;
using LegacyApp.Repositories;
using LegacyApp.Services;
using LegacyApp.Validators;
using NSubstitute;
using System;
using Xunit;

namespace RefactoringTest.UnitTests
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly IDateTimeProvider _dateTimeProvider = Substitute.For<IDateTimeProvider>();
        private readonly IClientRepository _clientRepository = Substitute.For<IClientRepository>();
        private readonly IUserDataAccess _userDataAccess = Substitute.For<IUserDataAccess>();
        private readonly IUserCreditService _userCreditService = Substitute.For<IUserCreditService>();
        private readonly IFixture _autoFixture = new Fixture();


        public UserServiceTests()
        {
            _userService = new UserService(
                _clientRepository,
                _userDataAccess,
                new UserValidator(_dateTimeProvider),
                new CreditLimitStrategyFactory(_userCreditService));
        }

        [Fact]
        public void AddUser_AllParametersAreValid_ShouldCreateUser()
        {
            // Arrange         
            const int clientId = 1;
            const string firstName = "Andre";
            const string lastName = "Castilho";
            var dateOfBirth = new DateTime(1988, 10, 18);

            var client = _autoFixture.Build<Client>()
                .With(c => c.Id, clientId)
                .Create();

            _dateTimeProvider.Now.Returns(new DateTime(2021, 12, 23));
            _clientRepository.GetById(clientId).Returns(client);
            _userCreditService.GetCreditLimit(firstName, lastName, dateOfBirth).Returns(600);

            // Act
            var result = _userService.AddUser(firstName, lastName, "andre.castilho@br.ey.com", dateOfBirth, clientId);

            // Assert
            result.Should().BeTrue();
            _userDataAccess.Received(1).Add(Arg.Any<User>());

        }

        [Theory]
        [InlineData("", "Castilho", "andre.castilho@br.ey.com", 1993)]
        [InlineData("Andre", "", "andre.castilho@br.ey.com", 1993)]
        [InlineData("Andre", "Castilho", "andrecom", 1993)]
        [InlineData("Andre", "Castilho", "andre.castilho@br.ey.com", 2002)]
        public void AddUser_DetailsAreInvalid_ShouldNotCreateUser(
           string firstName, string lastName, string email, int yearOfBirth)
        {
            // Arrange
            const int clientId = 1;
            var dateOfBirth = new DateTime(yearOfBirth, 1, 1);
            var client = _autoFixture.Build<Client>()
                .With(c => c.Id, () => clientId)
                .Create();

            _dateTimeProvider.Now.Returns(new DateTime(2021, 2, 16));
            _clientRepository.GetById(Arg.Is(clientId)).Returns(client);
            _userCreditService.GetCreditLimit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(600);

            // Act
            var result = _userService.AddUser(firstName, lastName, email, dateOfBirth, 1);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("RandomClientName", true, 600, 600)]
        [InlineData("ImportantClient", true, 600, 1200)]
        [InlineData("VeryImportantClient", false, 0, 0)]
        public void AddUser_NameIndicatesDifferentClassification_ShouldCreateUserWithCorrectCreditLimit(
            string clientName, bool hasCreditLimit, int initialCreditLimit, int finalCreditLimit)
        {
            // Arrange
            const int clientId = 1;
            const string firstName = "Nick";
            const string lastName = "Chapsas";
            var dateOfBirth = new DateTime(1993, 10, 10);
            var client = _autoFixture.Build<Client>()
                .With(c => c.Id, clientId)
                .With(c => c.Name, clientName)
                .Create();

            _dateTimeProvider.Now.Returns(new DateTime(2021, 2, 16));
            _clientRepository.GetById(Arg.Is(clientId)).Returns(client);
            _userCreditService.GetCreditLimit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(initialCreditLimit);

            // Act
            var result = _userService.AddUser(firstName, lastName, "nick.chapsas@gmail.com", dateOfBirth, 1);

            // Assert
            result.Should().BeTrue();
            _userDataAccess.Received()
                .Add(Arg.Is<User>(user => user.HasCreditLimit == hasCreditLimit && user.CreditLimit == finalCreditLimit));
        }

        [Fact]
        public void AddUser_UserHasCreditLimitAndCreditLimitIsLessThan500_ShouldNotCreateUser()
        {
            // Arrange
            const int clientId = 1;
            const string firstName = "Andre";
            const string lastName = "Castilho";
            var dateOfBirth = new DateTime(1993, 10, 10);
            var client = _autoFixture.Build<Client>()
                .With(c => c.Id, () => clientId)
                .Create();

            _dateTimeProvider.Now.Returns(new DateTime(2021, 2, 16));
            _clientRepository.GetById(Arg.Is(clientId)).Returns(client);
            _userCreditService.GetCreditLimit(Arg.Is(firstName), Arg.Is(lastName), Arg.Is(dateOfBirth)).Returns(499);

            // Act
            var result = _userService.AddUser(firstName, lastName, "andre.castilho@br.ey.com", dateOfBirth, 1);

            // Assert
            result.Should().BeFalse();
        }
    }
}

