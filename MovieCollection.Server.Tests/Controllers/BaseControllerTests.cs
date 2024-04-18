using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MovieCollection.Infrastructure.DTOs;
using MovieCollection.Server.Controllers;
using MovieCollection.Common.Tests.Extensions;

namespace MovieCollection.Server.Tests.Controllers
{
    public class BaseControllerTests : BaseController
    {
        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.NoContent)]
        public void ValidateResponse_AppRespondeSuccessWithoutData_ShouldReturnStatusCodeResultWithCorrectStatusCode(HttpStatusCode statusCode)
        {
            // Arrange
            var appResponse = AppResponse.Success();

            // Act
            var result = this.ValidateResponse(appResponse, statusCode);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Which.StatusCode.Should().Be((int)statusCode);
        }

        [Fact]
        public void ValidateResponse_AppRespondeFailWithoutData_ShouldReturnBadRequestObjectResultWithCorrectMessage()
        {
            // Arrange
            var appResponse = AppResponse.Error("teste", "teste");
            var statusCode = HttpStatusCode.OK;

            // Act
            var result = this.ValidateResponse(appResponse, statusCode);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>()
                .Which.Value.Should().BeAssignableTo<List<AppMessage>>()
                .Which.Should().Contain("teste", "teste");
        }

        [Theory]
        [InlineData("NotFound", typeof(NotFoundResult))]
        [InlineData("Blocked", typeof(ForbidResult))]
        public void ValidateResponse_AppRespondeFailWithoutData_ShoudReturnCorrectResultType(
            string messageKey,
            Type expectedResultType)
        {
            // Arrange
            var appResponse = AppResponse.Error(messageKey, "teste");
            var statusCode = HttpStatusCode.OK;

            // Act
            var result = this.ValidateResponse(appResponse, statusCode);

            // Assert
            result.Should().BeAssignableTo(expectedResultType);
        }

        [Theory]
        [InlineData(HttpStatusCode.OK)]
        [InlineData(HttpStatusCode.NoContent)]
        public void ValidateResponseWithData_AppRespondeSuccessWithData_ShouldReturnStatusCodeResultWithCorrectStatusCodeAndData(HttpStatusCode statusCode)
        {
            // Arrange
            var appResponse = AppResponse<string>.Success("teste");

            // Act
            var result = this.ValidateResponse(appResponse, statusCode);

            // Assert
            result.Should().BeAssignableTo<ObjectResult>()
                .Which.StatusCode.Should().Be((int)statusCode);
            result.Should().BeAssignableTo<ObjectResult>()
                .Which.Value.Should().Be("teste");
        }

        [Fact]
        public void ValidateResponseWithData_AppRespondeFail_ShouldReturnStatusCodeResultWithStatusCodeBadRequest()
        {
            // Arrange
            var appResponse = AppResponse<string>.Error("teste", "teste");
            var statusCode = HttpStatusCode.OK;

            // Act
            var result = this.ValidateResponse(appResponse, statusCode);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>()
                .Which.Value.Should().BeAssignableTo<List<AppMessage>>()
                .Which.Should().Contain("teste", "teste");
        }

        [Theory]
        [InlineData("NotFound", typeof(NotFoundResult))]
        [InlineData("Blocked", typeof(ForbidResult))]
        public void ValidateResponse_AppRespondeFailWithData_ShoudReturnCorrectResultType(
            string messageKey,
            Type expectedResultType)
        {
            // Arrange
            var appResponse = AppResponse<string>.Error(messageKey, "teste");
            var statusCode = HttpStatusCode.OK;

            // Act
            var result = this.ValidateResponse(appResponse, statusCode);

            // Assert
            result.Should().BeAssignableTo(expectedResultType);
        }
    }
}
