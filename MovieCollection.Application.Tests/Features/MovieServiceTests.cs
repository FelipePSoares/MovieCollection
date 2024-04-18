using AutoFixture;
using FluentAssertions;
using Moq;
using MovieCollection.Application.Contracts.Persistence;
using MovieCollection.Application.Features;
using MovieCollection.Application.Features.DTOs;
using MovieCollection.Common.Tests;
using MovieCollection.Common.Tests.Extensions;
using MovieCollection.Domain;
using MovieCollection.Infrastructure;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Application.Tests.Features
{
    public class MovieServiceTests : BaseTests
    {
        private readonly IMovieService movieService = default!;
        private readonly Mock<IGenericRepository<Movie>> movieRepositoryMock = default!;

        [Fact]
        public async Task RegisterAsync_SucessRegistration_ShouldReturnIsSucceedTrue()
        {
            // Arrange
            var movie = Fixture.Create<MovieRegisterRequest>();

            // Act
            var response = await this.movieService.RegisterAsync(movie);

            // Assert
            response.Succeeded.Should().BeTrue();
            response.Messages.Should().BeEmpty();
            response.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task RegisterAsync_FailedRegistration_ShouldReturnIsSucceedFalseAndTheMessage()
        {
            // Arrange
            var movie = Fixture.Create<MovieRegisterRequest>();

            movie.Title = default!;

            var error = new AppMessage("title", string.Format(ValidationMessages.PropertyCantBeNullOrEmpty, "title"));

            // Act
            var response = await this.movieService.RegisterAsync(movie);

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().Contain(error.Code, error.Description);
        }

        [Fact]
        public async Task RemoveAsync_SucceedRemoved_ShouldReturnIsSucceedFalseAndErrorMessage()
        {
            // Arrange
            var movieId = Guid.NewGuid();
            var movie = Fixture.Create<Movie>();

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.Trackable())
                .Returns(new List<Movie>() { new() }.AsQueryable());

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.Delete(It.IsAny<Movie>()))
                .Returns(AppResponse.Success);

            // Act
            var response = await this.movieService.RemoveAsync(movieId);

            // Assert
            response.Succeeded.Should().BeTrue();
            response.Messages.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveAsync_ErrorOnRemove_ShouldReturnIsSucceedFalseAndErrorMessage()
        {
            // Arrange
            var movieId = Guid.NewGuid();
            var movie = Fixture.Create<Movie>();

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.Trackable())
                .Returns(new List<Movie>() { new() }.AsQueryable());

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.Delete(It.IsAny<Movie>()))
                .Returns(AppResponse.Error("code", "description"));

            // Act
            var response = await this.movieService.RemoveAsync(movieId);

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().Contain("code", "description");
        }

        [Fact]
        public async Task RemoveAsync_MovieNotExist_ShouldReturnIsSucceedTrue()
        {
            // Arrange
            var movieId = Guid.NewGuid();

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.Delete(It.IsAny<Movie>()))
                .Verifiable();

            // Act
            var response = await this.movieService.RemoveAsync(movieId);

            // Assert
            response.Succeeded.Should().BeTrue();
            this.movieRepositoryMock.Verify(movieRepository => movieRepository.Delete(It.IsAny<Movie>()), Times.Never);
        }
    }
}
