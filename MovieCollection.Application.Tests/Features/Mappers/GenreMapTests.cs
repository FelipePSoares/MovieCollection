using AutoFixture;
using FluentAssertions;
using MovieCollection.Application.Features.Mappers;
using MovieCollection.Common.Tests;
using MovieCollection.Domain;

namespace MovieCollection.Application.Tests.Features.Mappers
{
    public class GenreMapTests : BaseTests
    {
        [Fact]
        public void ToGenreResponse_MapData_ShouldBeMappedCorrectly()
        {
            // Arrange
            var genre = Fixture.Create<Genre>();

            // Act
            var genreResponse = genre.ToGenreResponse();

            // Assert
            genreResponse.Id.Should().Be(genre.Id);
            genreResponse.Name.Should().Be(genre.Name);
        }

        [Fact]
        public void ToGenreResponse_MapListOfData_ShouldBeMappedCorrectly()
        {
            // Arrange
            var genres = Fixture.Create<List<Genre>>();

            // Act
            var genresResponse = genres.ToGenreResponse();

            // Assert
            genresResponse.Should().HaveCount(genres.Count);
        }
    }
}
