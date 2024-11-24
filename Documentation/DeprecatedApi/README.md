# Attrify - Deprecated Api Attribute and Middleware
[Back to main page](../../README.md)

As an API evolves, some endpoints may become outdated or need to be decommissioned. 
However, removing an API without adequate notice can disrupt your users and cause 
significant issues for consumers relying on that API. It's essential to communicate 
upcoming changes to the consumers well in advance.

The `Deprecated API Attribute` and accompanying `Middleware` provide developers an elegant solution 
for marking API endpoints as deprecated and notifying consumers about the upcoming deprecation,  
advance notice of when an API will be decommissioned and timeline to take 
appropriate action before the deprecation date (the "Sunset" date) has passed. 

Once the sunset date is reached, the middleware will automatically return a 410 Gone HTTP status 
code to indicate that the resource is no longer available, while simultaneously providing clear information 
on where to find an alternative endpoint or what migration steps to follow.

By integrating this solution into your project, you can ensure that your users are well-informed 
about deprecated APIs and have sufficient time to transition to newer versions, minimizing disruption 
and improving your API's lifecycle management.

## Overview
The Deprecated API Attribute is a custom attribute that can be applied to any API endpoint, signaling 
that it is deprecated and will eventually be retired. This attribute allows developers to specify 
important details, including:

- Sunset Date: The date after which the API will be removed, effectively marking it as unavailable.

- Warning Message: A message that provides information about the deprecation and encourages clients to migrate to a new version of the API.

- Link to Additional Information: An optional URL that leads to a migration guide or other helpful resources for users transitioning away from the deprecated API.

The Deprecated API Middleware works alongside this attribute to ensure that deprecation notifications 
are properly handled at runtime. When a request is made to a deprecated endpoint, the middleware 
automatically sets the appropriate HTTP response headers (such as Sunset, Warning, and Link). 
It is important that consumers of the API's always check for these headers to ensure they get early
notification of the deprecation.

When the sunset date is reached, the middleware will immediately return a 410 Gone HTTP status code. This status 
code signals that the requested resource is no longer available, giving clients that have missed the deprecation warning
clear and immediate feedback about next steps.

## Key Benefits:
- Advance Notice: Developers can provide clients with a clear timeline for when the API will be removed, helping consumers plan for migration ahead of time.

- Automated Handling: Once the sunset date arrives, the middleware automatically returns the appropriate HTTP status (410 Gone), making it easier to manage deprecated APIs without needing manual intervention.

- Consistent Communication: Automatically sends deprecation-related headers and messages to clients, ensuring consistency and clarity about the API's status.

By using this combination of attribute and middleware, your project will be able to manage the 
lifecycle of your APIs efficiently, ensuring both transparency and smooth transitions when APIs need to be phased out.

Components
----------

### 1\. Deprecated API Attribute

The `DeprecatedApiAttribute` is a custom attribute used to mark API endpoints that are deprecated. This attribute can be applied to controller actions or routes that are being sunset.

#### Properties:

-   **Sunset**: The date when the API will be deprecated. This is a required field, and it indicates when the resource will no longer be available.
-   **Warning**: A custom warning message that explains the deprecation and provides any relevant details. This is optional but recommended to help the API consumer understand the situation.
-   **Link**: A URL providing more information about the deprecation. This can link to documentation, alternative APIs, or migration guides.

#### Example Usage:

```csharp
[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    [HttpGet]
    [DeprecatedApi(Sunset = "2024-12-31", Warning = "This API is deprecated. Please migrate to v2.", Link = "https://example.com/deprecation-info")]
    public IActionResult GetSampleData()
    {
        // Logic for the API endpoint
        return Ok(new { message = "Sample data" });
    }
}
```

In this example, the `GetSampleData` action is marked as deprecated, with a sunset date of `2024-12-31`, a warning message, and a link to more information.

### 2\. Deprecated API Middleware

The `DeprecatedApiMiddleware` is responsible for inspecting the request and checking whether the requested endpoint is marked with the `DeprecatedApiAttribute`. 
If the endpoint is deprecated, the middleware automatically modifies the response to include the necessary headers (Sunset, Warning, and Link) and sets the status code to HTTP 410 Gone when the sunset date is reached.

#### How It Works:

-   The middleware inspects the endpoint metadata for the `DeprecatedApiAttribute`.
-   If the attribute is found the middleware:
    -   Sets the **Sunset** header to the `Sunset` date from the attribute.
    -   Sets the **Warning** header with the deprecation message.
    -   Optionally, sets the **Link** header with the deprecation link if provided.
    -   If the current date is NOT past the `Sunset` date.
        -   Allows the request to proceed to the endpoint.
    -   If the current date is past the `Sunset` date.
        -   Sets the HTTP status code to `410 Gone` to indicate that the resource is no longer available.
        -   Writes an appropriate error message to the response body.

#### Middleware Configuration

To use the `DeprecatedApiMiddleware`, it must be added to the request pipeline in the `Startup` or `Program` class of your application.

#### Example Configuration:

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder, builder.Configuration);
        var app = builder.Build();
        ConfigurePipeline(app);
        app.Run();
    }

    public static void ConfigurePipeline(WebApplication app, InvisibleApiKey invisibleApiKey)
    {
        ...

        // Use the middleware to intercept deprecated API calls
        app.UseDeprecatedApiMiddleware();

        ...
    }
}
```

In this example, the `DeprecatedApiMiddleware` is added to the pipeline, ensuring that every incoming request is inspected for deprecated APIs.

How to Implement in Your Project
--------------------------------

1.  **Add the Attribute**: Decorate any API endpoints you want to mark as deprecated with the `DeprecatedApiAttribute`. Specify the `Sunset` date, provide a warning message and optionally a link to more information providing migration / next step instructions.

2.  **Configure the Middleware**: Add the `DeprecatedApiMiddleware` to your request pipeline by modifying the `Configure` method in your `Startup.cs` or `Program.cs` file. This middleware will automatically handle deprecated endpoints.

## Best Practices

- **Graceful Deprecation**:  
  When marking APIs as deprecated, provide ample time for consumers to migrate to newer versions. Use the `Sunset` date to specify when the API will no longer be available, and provide alternative endpoints or new versions in the `Link` header.

- **Testing**:  
  Ensure that the middleware is properly tested, especially with different sunset dates and warning messages. Verify that the correct HTTP status code (**410 Gone**) and headers are set, and that the response body contains the expected deprecation message.

- **Consistency**:  
  Use the same pattern across your project for deprecating APIs to ensure a consistent experience for clients and users.

- **Consumer Communications**:  
  In addition to implementing the **Deprecated API Attribute** and middleware, developers should notify API consumers via any/all communication channels, such as:
  - Sending emails to registered support contacts.
  - Updating status or uptime dashboards with deprecation notices.
  - Posting updates in developer forums or other communication platforms.  
  This ensures that all consumers are aware of the deprecation, even if they miss the deprecation headers.

- **Update API Documentation**:  
  If this deprecated API approach is adopted, all API documentation must be updated to:
  - Clearly explain that your APIs follow a standardized deprecation process. Document the meaning and purpose of the deprecation headers (Sunset, Warning, Link) included in the response.
  - Encourage API consumers to implement monitoring and alerting processes for these deprecation headers. This proactive approach will help them handle deprecations smoothly and plan migrations without disruptions to their workflows.

Conclusion
----------

The **Deprecated API Attribute** and **Middleware** provide a streamlined way to handle deprecated APIs in your project. By leveraging these components, you can ensure that your API consumers are informed about deprecated functionality and are given clear guidance about migration or alternative resources.

[Back to main page](../../README.md)