using System.Security.Claims;
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
using MovieCollection.Domain.AccessControl;
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

        [Theory]
        [MemberData(nameof(SearchData))]
        public async Task SearchAsync_MovieFound_ShoundReturnSucceededTrueAndMovieData(MovieFilters filter, List<Movie> moviesDb, List<MovieResponse> resultExpected)
        {
            // Arrange
            this.movieRepositoryMock.Setup(movieRepository => movieRepository.NoTrackable())
                .Returns(moviesDb.BuildMock());

            // Act
            var response = await this.movieService.SearchAsync(filter);

            // Assert
            response.Should().NotBeNull();
            response.Succeeded.Should().BeTrue();
            response.Data.Should().BeEquivalentTo(resultExpected);
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
            Movie movieUpdated = default!;
            var movie = Fixture.Create<Movie>();

            var expected = new Movie()
            {
                Id = movie.Id,
                Title = "Test Title",
                Description = "Test Description",
                ReleaseYear = 2022,
                Duration = new TimeSpan(2, 10, 0),
                Genres = new List<Genre>()
                {
                    movie.Genres[0],
                    movie.Genres[2],
                    new Genre()
                    {
                        Id = Guid.NewGuid(),
                        Name = "Genre"
                    }
                }
            };

            var jsonPatch = new JsonPatchDocument<MovieUpdateRequest>();
            jsonPatch.Replace(t => t.Title, expected.Title);
            jsonPatch.Replace(t => t.Description, expected.Description);
            jsonPatch.Replace(t => t.ReleaseYear, expected.ReleaseYear);
            jsonPatch.Replace(t => t.Duration, expected.Duration);
            jsonPatch.Remove(t => t.Genres, 1);
            jsonPatch.Add(t => t.Genres, expected.Genres.Last().ToGenreUpdate());


            this.movieRepositoryMock.Setup(movieRepository => movieRepository.NoTrackable())
                .Returns(new List<Movie>() { movie }.BuildMock());

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.InsertOrUpdate(It.IsAny<Movie>()))
                .Callback((Movie m) => movieUpdated = m)
                .Returns(AppResponse<Movie>.Success(movie));

            // Act
            var response = await this.movieService.UpdateAsync(movie.Id, jsonPatch);

            // Assert
            response.Succeeded.Should().BeTrue();
            response.Messages.Should().BeEmpty();
            response.Data.Should().NotBeNull();
            movieUpdated.Should().BeEquivalentTo(expected, config
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
        public async Task UpdateAsync_InvalidMovie_ShouldReturnSucceededFalseAndErrorMessage()
        {
            // Arrange
            var movie = Fixture.Create<Movie>();

            var jsonPatch = new JsonPatchDocument<MovieUpdateRequest>();
            jsonPatch.Replace(t => t.Title, string.Empty);

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.NoTrackable())
                .Returns(new List<Movie>() { movie }.BuildMock());

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.InsertOrUpdate(It.IsAny<Movie>()))
                .Verifiable();

            // Act
            var response = await this.movieService.UpdateAsync(movie.Id, jsonPatch);

            // Assert
            response.Succeeded.Should().BeFalse();
            response.Data.Should().BeNull();
            response.Messages.Should().Contain("Title", string.Format(ValidationMessages.PropertyCantBeNullOrEmpty, "Title"));
            this.movieRepositoryMock.Verify(movieRepository => movieRepository.InsertOrUpdate(It.IsAny<Movie>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_MovieNotExist_ShouldReturnSucceededTrue()
        {
            // Arrange
            var jsonPatch = new JsonPatchDocument<MovieUpdateRequest>();

            this.movieRepositoryMock.Setup(movieRepository => movieRepository.NoTrackable())
                .Returns(new List<Movie>().BuildMock());

            // Act
            var response = await this.movieService.UpdateAsync(Guid.NewGuid(), jsonPatch);

            // Assert
            response.Should().NotBeNull();
            response.Succeeded.Should().BeFalse();
            response.Messages.Should().ContainKey(MessageKey.NotFound);
        }

        public static IEnumerable<object[]> SearchData()
        {
            var movies = Fixture.Create<List<Movie>>();

            movies[0].ReleaseYear = DateTime.UtcNow.AddYears(-1).Year;
            movies[1].ReleaseYear = DateTime.UtcNow.Year;
            movies[2].ReleaseYear = DateTime.UtcNow.AddYears(1).Year;

            var filter = new MovieFilters()
            {
                Title = movies[1].Title
            };
            yield return new object[] { filter, movies, new List<MovieResponse>() { movies[1].ToMovieResponse() } };

            filter = new MovieFilters()
            {
                Title = string.Concat(movies[1].Title.Skip(5).Take(5))
            };
            yield return new object[] { filter, movies, new List<MovieResponse>() { movies[1].ToMovieResponse() } };

            filter = new MovieFilters()
            {
                Genres = new List<Guid>() { movies[0].Genres.First().Id, movies[2].Genres.Last().Id }
            };
            yield return new object[] { filter, movies, new List<MovieResponse>() { movies[0].ToMovieResponse(), movies[2].ToMovieResponse() } };

            filter = new MovieFilters()
            {
                ReleaseYearStart = DateTime.UtcNow.Year,
                ReleaseYearEnd = DateTime.UtcNow.Year,
            };
            yield return new object[] { filter, movies, new List<MovieResponse>() { movies[1].ToMovieResponse() } };

            filter = new MovieFilters()
            {
                ReleaseYearStart = DateTime.UtcNow.AddYears(-1).Year,
                ReleaseYearEnd = DateTime.UtcNow.AddYears(1).Year,
            };
            yield return new object[] { filter, movies, movies.ToMovieResponse() };

            filter = new MovieFilters()
            {
                ReleaseYearStart = DateTime.UtcNow.AddYears(-1).Year,
                ReleaseYearEnd = DateTime.UtcNow.Year,
            };
            yield return new object[] { filter, movies, new List<MovieResponse>() { movies[0].ToMovieResponse(), movies[1].ToMovieResponse() } };
        }
    }
}
