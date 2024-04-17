using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MovieCollection.Infrastructure;
using MovieCollection.Server.Controllers;

namespace MovieCollection.Server.Tests.Controllers
{
    public class BaseControllerTests : BaseController
    {
        [Fact]
        public void ValidateResponse_AppRespondeSuccessWithoutData_ShouldReturnStatusCodeResultWithStatusCodeOk()
        {
            // Arrange
            var appResponse = AppResponse.Success();
            var statusCode = HttpStatusCode.OK;

            // Act
            var result = this.ValidateResponse(appResponse, statusCode);

            // Assert
            result.Should().BeAssignableTo<StatusCodeResult>()
                .Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Fact]
        public void ValidateResponse_AppRespondeFailWithoutData_ShouldReturnStatusCodeResultWithStatusCodeBadRequest()
        {
            // Arrange
            var appResponse = AppResponse.Error("teste", "teste");
            var statusCode = HttpStatusCode.OK;

            // Act
            var result = this.ValidateResponse(appResponse, statusCode);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>()
                .Which.Value.Should().BeAssignableTo<Dictionary<string, string>>()
                .Which.Should().Contain("teste", "teste");
        }

        [Fact]
        public void ValidateResponseWithData_AppRespondeSuccessWithoutData_ShouldReturnStatusCodeResultWithStatusCodeOk()
        {
            // Arrange
            var appResponse = AppResponse<string>.Success("teste");
            var statusCode = HttpStatusCode.OK;

            // Act
            var result = this.ValidateResponse(appResponse, statusCode);

            // Assert
            result.Should().BeAssignableTo<ObjectResult>()
                .Which.StatusCode.Should().Be((int)HttpStatusCode.OK);
            result.Should().BeAssignableTo<ObjectResult>()
                .Which.Value.Should().Be("teste");
        }

        [Fact]
        public void ValidateResponseWithData_AppRespondeFailWithoutData_ShouldReturnStatusCodeResultWithStatusCodeBadRequest()
        {
            // Arrange
            var appResponse = AppResponse<string>.Error("teste", "teste");
            var statusCode = HttpStatusCode.OK;

            // Act
            var result = this.ValidateResponse(appResponse, statusCode);

            // Assert
            result.Should().BeAssignableTo<BadRequestObjectResult>()
                .Which.Value.Should().BeAssignableTo<Dictionary<string, string>>()
                .Which.Should().Contain("teste", "teste");
        }
    }
}
