using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using Moq;
using MovieCollection.Application.Contracts.Persistence;
using MovieCollection.Application.Features;
using MovieCollection.Application.Features.DTOs;
using MovieCollection.Application.Features.Mappers;
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
        public async Task GetByIdAsync_MovieFound_ShoundReturnSucceededTrueAndMovieData()
        {
            // Arrange
            var movie = Fixture.Create<Movie>();

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.NoTrackable())
                .Returns(new List<Movie>() { movie }.AsQueryable());

            // Act
            var response = await this.movieService.GetByIdAsync(movie.Id);

            // Assert
            response.Should().NotBeNull();
            response.Succeeded.Should().BeTrue();
            response.Data.Should().BeEquivalentTo(movie);
        }

        [Fact]
        public async Task GetByIdAsync_MovieNotExist_ShoundReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            this.movieRepositoryMock.Setup(movieRepository => movieRepository.NoTrackable())
                .Returns(new List<Movie>() { }.AsQueryable());

            // Act
            var response = await this.movieService.GetByIdAsync(Guid.NewGuid());

            // Assert
            response.Should().NotBeNull();
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().ContainKey(MessageKey.NotFound);
        }

        [Fact]
        public async Task SearchAsync_MovieFound_ShoundReturnSucceededTrueAndMovieData()
        {
            // Arrange
            var movies = Fixture.Create<List<Movie>>();

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.NoTrackable())
                .Returns(movies.AsQueryable());

            // Act
            var response = await this.movieService.SearchAsync(new MovieFilters());

            // Assert
            response.Should().NotBeNull();
            response.Succeeded.Should().BeTrue();
            response.Data.Should().BeEquivalentTo(movies);
        }

        [Fact]
        public async Task SearchAsync_MovieNotExist_ShoundReturnSucceededTrueAndEmptyData()
        {
            // Arrange
            this.movieRepositoryMock.Setup(movieRepository => movieRepository.NoTrackable())
                .Returns(new List<Movie>().AsQueryable());

            // Act
            var response = await this.movieService.SearchAsync(new MovieFilters());

            // Assert
            response.Should().NotBeNull();
            response.Succeeded.Should().BeTrue();
            response.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task RegisterAsync_SucessRegistration_ShouldReturnSucceededTrue()
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
        public async Task RegisterAsync_FailedRegistration_ShouldReturnSucceededFalseAndTheMessage()
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
        public async Task RemoveAsync_SucceedRemoved_ShouldReturnSucceededFalseAndErrorMessage()
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
        public async Task RemoveAsync_ErrorOnRemove_ShouldReturnSucceededFalseAndErrorMessage()
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
        public async Task RemoveAsync_MovieNotExist_ShouldReturnSucceededTrue()
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

        [Fact]
        public async Task UpdateAsync_SucessUpdate_ShouldReturnSucceededTrue()
        {
            // Arrange
            var movieRequest = Fixture.Create<MovieRegisterRequest>();
            var movie = movieRequest.FromDTO();

            var jsonPatch = new JsonPatchDocument<MovieRegisterRequest>();
            jsonPatch.Add(t => t.Title, "Test Title");

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.NoTrackable())
                .Returns(new List<Movie>() { movie }.AsQueryable());

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.InsertOrUpdate(It.IsAny<Movie>()))
                .Returns(AppResponse<Movie>.Success(movie));

            // Act
            var response = await this.movieService.UpdateAsync(Guid.NewGuid(), jsonPatch);

            // Assert
            response.Succeeded.Should().BeTrue();
            response.Messages.Should().BeEmpty();
            response.Data.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateAsync_MovieNotExist_ShouldReturnSucceededTrue()
        {
            // Arrange
            var jsonPatch = new JsonPatchDocument<MovieRegisterRequest>();

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.NoTrackable())
                .Returns(new List<Movie>() { }.AsQueryable());

            // Act
            var response = await this.movieService.UpdateAsync(Guid.NewGuid(), jsonPatch);

            // Assert
            response.Should().NotBeNull();
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().ContainKey(MessageKey.NotFound);
        }

    }
}
