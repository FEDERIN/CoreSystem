using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace Core.Idempotency.IntegrationTests.ExceptionHandling;

public abstract class ExceptionHandlingTestsBase
{
    protected abstract HttpClient Client { get; }

    [Fact]
    public async Task Should_ReturnConflict_When_RequestFingerprintDoesNotMatch()
    {
        // Arrange
        const string idempotencyKey = "integration-fingerprint";

        var firstRequest = new CreateOrderRequest("Apple");
        var secondRequest = new CreateOrderRequest("Orange");

        // First request
        using var request1 = new HttpRequestMessage(HttpMethod.Post, "/orders")
        {
            Content = JsonContent.Create(firstRequest)
        };

        request1.Headers.Add("Idempotency-Key", idempotencyKey);

        var firstResponse = await Client.SendAsync(request1, TestContext.Current.CancellationToken);

        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Second request (same key, different payload)
        using var request2 = new HttpRequestMessage(HttpMethod.Post, "/orders")
        {
            Content = JsonContent.Create(secondRequest)
        };

        request2.Headers.Add("Idempotency-Key", idempotencyKey);

        // Act
        var response = await Client.SendAsync(request2, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(TestContext.Current.CancellationToken);

        problem.Should().NotBeNull();
        problem!.Status.Should().Be(StatusCodes.Status409Conflict);

        problem.Title.Should().Be("Idempotency fingerprint mismatch");

        problem.Detail.Should().Be(
            "The request does not match the original request associated with this idempotency key.");

        problem.Extensions.Should().ContainKey("idempotencyKey");

        problem.Extensions["idempotencyKey"]
            .Should()
            .BeOfType<JsonElement>()
            .Which
            .GetString()
            .Should()
            .Be(idempotencyKey);
    }

    private sealed record CreateOrderRequest(string Name);
}