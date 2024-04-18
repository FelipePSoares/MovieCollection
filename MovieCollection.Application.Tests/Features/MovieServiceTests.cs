using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.JsonPatch;
using MockQueryable.Moq;
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
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MovieCollection.Application.Tests.Features
{
    public class MovieServiceTests : BaseTests
    {
        private readonly IMovieService movieService = default!;
        private readonly Mock<IGenericRepository<Movie>> movieRepositoryMock = default!;

        public MovieServiceTests()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            this.movieRepositoryMock = new Mock<IGenericRepository<Movie>>();
            unitOfWork.Setup(uw => uw.MovieRepository).Returns(this.movieRepositoryMock.Object);

            this.movieService = new MovieService(unitOfWork.Object);
        }

        [Fact]
        public async Task GetByIdAsync_MovieFound_ShoundReturnSucceededTrueAndMovieData()
        {
            // Arrange
            var movie = Fixture.Create<Movie>();

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.NoTrackable())
                .Returns(new List<Movie>() { movie }.BuildMock());

            // Act
            var response = await this.movieService.GetByIdAsync(movie.Id);

            // Assert
            response.Should().NotBeNull();
            response.Succeeded.Should().BeTrue();
            response.Data.Should().BeEquivalentTo(movie, config 
                => config
                .Excluding(movie => movie.Users)
                .Excluding(movie => movie.CreatedDate)
                .Excluding(movie => movie.ModifiedAt)
                .For(movie => movie.Genres)
                    .Exclude(genre => genre.CreatedDate)
                .For(movie => movie.Genres)
                    .Exclude(genre => genre.ModifiedAt));
        }

        [Fact]
        public async Task GetByIdAsync_MovieNotExist_ShoundReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            this.movieRepositoryMock.Setup(movieRepository => movieRepository.NoTrackable())
                .Returns(new List<Movie>() { }.BuildMock());

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
                .Returns(movies.BuildMock());

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
                .Returns(new List<Movie>().BuildMock());

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

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.InsertOrUpdate(It.IsAny<Movie>()))
                .Returns(AppResponse<Movie>.Success(movie.FromDTO()));

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

            // Act
            var response = await this.movieService.RegisterAsync(movie);

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().Contain("Title", string.Format(ValidationMessages.PropertyCantBeNullOrEmpty, "Title"));
        }

        [Fact]
        public async Task RemoveAsync_SucceedRemoved_ShouldReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            var movies = Fixture.Create<List<Movie>>();

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.Trackable())
                .Returns(movies.BuildMock());

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.Delete(It.IsAny<Movie>()))
                .Returns(AppResponse.Success);

            // Act
            var response = await this.movieService.RemoveAsync(movies.First().Id);

            // Assert
            response.Succeeded.Should().BeTrue();
            response.Messages.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveAsync_ErrorOnRemove_ShouldReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            var movies = Fixture.Create<List<Movie>>();

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.Trackable())
                .Returns(movies.BuildMock());

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.Delete(It.IsAny<Movie>()))
                .Returns(AppResponse.Error("code", "description"));

            // Act
            var response = await this.movieService.RemoveAsync(movies.First().Id);

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().Contain("code", "description");
        }

        [Fact]
        public async Task RemoveAsync_MovieNotExist_ShouldReturnSucceededTrue()
        {
            // Arrange
            this.movieRepositoryMock.Setup(movieRepository => movieRepository.Trackable())
                .Returns(new List<Movie>().BuildMock());

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.Delete(It.IsAny<Movie>()))
                .Verifiable();

            // Act
            var response = await this.movieService.RemoveAsync(Guid.NewGuid());

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
                .Returns(new List<Movie>() { movie }.BuildMock());

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
                .Returns(new List<Movie>() { }.BuildMock());

            // Act
            var response = await this.movieService.UpdateAsync(Guid.NewGuid(), jsonPatch);

            // Assert
            response.Should().NotBeNull();
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().ContainKey(MessageKey.NotFound);
        }

    }
}
