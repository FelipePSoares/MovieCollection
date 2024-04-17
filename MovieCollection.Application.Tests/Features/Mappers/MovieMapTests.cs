using AutoFixture;
using FluentAssertions;
using MovieCollection.Application.Features.Mappers;
using MovieCollection.Common.Tests;
using MovieCollection.Domain;

namespace MovieCollection.Application.Tests.Features.Mappers
{
    public class MovieMapTests : BaseTests
    {
        [Fact]
        public void ToMovieResponse_MapData_ShouldBeMappedCorrectly()
        {
            // Arrange
            var movie = fixture.Create<Movie>();

            // Act
            var movieResponse = movie.ToMovieResponse();

            // Assert
            movieResponse.Id.Should().Be(movie.Id);
            movieResponse.Title.Should().Be(movie.Title);
            movieResponse.Description.Should().Be(movie.Description);
            movieResponse.ReleaseDate.Should().Be(movie.ReleaseDate);
            movieResponse.Duration.Should().Be(movie.Duration);
            movieResponse.Genres.Should().HaveCount(movie.Genres.Count);
        }

        [Fact]
        public void ToMovieResponse_MapListOfData_ShouldBeMappedCorrectly()
        {
            // Arrange
            var movies = fixture.Create<List<Movie>>();

            // Act
            var moviesResponse = movies.ToMovieResponse();

            // Assert

            // Assert
            moviesResponse.Should().HaveCount(movies.Count);
        }
    }
}
