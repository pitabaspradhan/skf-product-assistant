using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Skf.ProductAssistant.Models;
using Skf.ProductAssistant.Orchestration;
using System.Net;
using System.Text.Json;

namespace Skf.ProductAssistant.FunctionApp;

public class ProductAssistantFunction
{
    private readonly IntentOrchestrator _orchestrator;
    private readonly ILogger<ProductAssistantFunction> _logger;

    public ProductAssistantFunction(
        IntentOrchestrator orchestrator,
        ILogger<ProductAssistantFunction> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    // 🔹 Main Function Endpoint
    [Function("product-assistant")]

    [OpenApiOperation(
        operationId: "Run",
        tags: new[] { "Product Assistant" },
        Summary = "SKF Product Assistant Endpoint",
        Description = "Handles product Q&A and feedback with conversational state.")]

    [OpenApiRequestBody(
        contentType: "application/json",
        bodyType: typeof(UserMessage),
        Required = true,
        Description = "User query payload")]

    [OpenApiResponseWithBody(
        statusCode: HttpStatusCode.OK,
        contentType: "text/plain",
        bodyType: typeof(string),
        Description = "Assistant response")]

    [OpenApiResponseWithoutBody(
        statusCode: HttpStatusCode.BadRequest,
        Description = "Invalid request")]

    public async Task<HttpResponseData> Run(
        [HttpTrigger(
            AuthorizationLevel.Function,
            "get",
            "post")]
        HttpRequestData req)
    {
        _logger.LogInformation(
            "Product Assistant function invoked.");

        // ✅ GET → Health + Usage Info
        if (req.Method.Equals(
            "GET",
            StringComparison.OrdinalIgnoreCase))
        {
            var getResponse =
                req.CreateResponse(HttpStatusCode.OK);

            await getResponse.WriteStringAsync(
                """
                SKF Product Assistant is running.

                Send a POST request with JSON body:

                {
                  "conversationId": "<any-unique-id>",
                  "message": "Width of 6205?"
                }

                Example conversationId values:
                - user-123
                - session-abc
                - guid-value

                This ID maintains conversational state
                across follow-up questions and feedback.
                """);

            return getResponse;
        }

        // ✅ Read request body safely
        string body;

        using (var reader =
               new StreamReader(req.Body))
        {
            body = await reader.ReadToEndAsync();
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            return await CreateErrorResponse(
                req,
                "Request body cannot be empty.",
                HttpStatusCode.BadRequest);
        }

        UserMessage? request;

        try
        {
            request =
                JsonSerializer.Deserialize<UserMessage>(
                    body,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Invalid JSON format.");

            return await CreateErrorResponse(
                req,
                "Invalid JSON format.",
                HttpStatusCode.BadRequest);
        }

        if (request == null ||
            string.IsNullOrWhiteSpace(
                request.ConversationId) ||
            string.IsNullOrWhiteSpace(
                request.Message))
        {
            return await CreateErrorResponse(
                req,
                "conversationId and message are required.",
                HttpStatusCode.BadRequest);
        }

        // ✅ Route via Orchestrator
        var result =
            await _orchestrator.HandleAsync(
                request.ConversationId,
                request.Message);

        var response =
            req.CreateResponse(HttpStatusCode.OK);

        await response.WriteStringAsync(result);

        return response;
    }

    // 🔧 Standardized Error Helper
    private static async Task<HttpResponseData>
        CreateErrorResponse(
            HttpRequestData req,
            string message,
            HttpStatusCode statusCode)
    {
        var response =
            req.CreateResponse(statusCode);

        await response.WriteStringAsync(message);

        return response;
    }
}
