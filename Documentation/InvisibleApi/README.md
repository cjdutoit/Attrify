# Attrify - Invisible API attibute and middleware
[Back to main page](../../README.md)

The InvisibleApiMiddleware is a lightweight ASP.NET Core middleware designed to control the 
visibility of specific API endpoints in your application. By using the [InvisibleApi] attribute 
in combination with this middleware, you can make certain endpoints inaccessible 
(returning a 404 Not Found) unless specific conditions are met.

This approach is particularly useful for:

- Securing internal or administrative endpoints from general exposure.
- Providing an additional layer of security beyond standard authentication.

## Overview

The `InvisibleApiMiddleware` works seamlessly with the `[InvisibleApi]` attribute to evaluate requests based on:

1.  A custom request header that must match a preconfigured key-value pair.
2.  User authentication and role validation.

If both conditions are not met, the middleware intercepts the request and returns a `404 Not Found` response, making the endpoint invisible to unauthorized users.

* * * * *

### Key Benefits

-   **Enhanced Security:** Endpoints are only visible to authenticated users with the appropriate roles and a valid request header.
-   **Ease of Use:** A simple attribute-based implementation to control endpoint visibility.
-   **Flexible Configuration:** Define visibility rules through custom headers and roles tailored to your application's needs.
-   **Scalability:** Works seamlessly with ASP.NET Core's middleware pipeline and authorization policies.

* * * * *

## Purpose

The **Invisible API and Middleware** is designed to secure and hide administrative endpoints that are necessary 
for acceptance testing but must never be exposed in production environments. These endpoints often facilitate 
tasks like setting up test data, running specific test scenarios, and tearing down data after testing.

To ensure that these endpoints remain completely hidden and inaccessible to real users, this middleware 
applies a **twofold protection mechanism**:

1.  **Validation of a Request Header:** A dynamically generated key-value pair must be present in the request headers.
2.  **Role-Based Authorization:** The user must be authenticated and belong to a dynamically generated role.


### Key Characteristics

-   **Dynamic Security:**
    -   The request header key-value pair and the required role are randomly generated during application startup.
    -   These values are **not stored in configuration files** and are inaccessible to actual users.
-   **Integration with Dependency Injection:**
    -   The generated security values are registered with the DI container, allowing them to be 
        programmatically retrieved during testing without exposing them externally.

* * * * *

Use Case: Acceptance Testing
----------------------------

The primary use case of the Invisible API is to provide access to administrative endpoints during **acceptance testing**, enabling:

-   Consistent data entry via APIs that adhere to business rules and validations.
-   Controlled creation of test scenarios by leveraging APIs that applies business rules and validation consistently, ensuring end-to-end testing integrity.
-   Hiding and protecting these endpoints from real users in production environments / non test scenarios.

### How It Works In Your Application

In your application's Program.cs or Startup.cs file, a unique request header key-value pair 
and a required role name are dynamically generated.  These values are registered in the 
Dependency Injection (DI) container within the ConfigureServices method and passed 
to middleware for runtime enforcement.

Endpoints intended to be hidden are decorated with the [InvisibleApi] attribute. 
The middleware evaluates all requests to these endpoints, ensuring that the 
request contains the correct header and that the user is both authenticated and 
belongs to the specified role. If the validation fails, the middleware responds 
with a 404 Not Found status, effectively hiding the endpoint from unauthorized users.

### How It Works In Your Acceptance Test Project

In the acceptance test project, an API broker class initializes your application 
(from Program.cs or Startup.cs) using a web application factory for testing purposes. 
A custom test web application factory overrides the default initialization by removing 
real authentication and authorization services. Instead, it replaces them with a custom 
authentication scheme (TestScheme) using a TestAuthHandler and a permissive 
authorization policy (TestPolicy).

The TestAuthHandler is configured to simulate an authenticated user with the generated role. 
Additionally, the HttpClient in the API broker class is set up to include the custom header 
key-value pair in all requests, satisfying the middleware's validation. This setup allows the 
acceptance test project to access hidden endpoints and validate their functionality 
using the dynamically generated security values.
 

### Example Use Case

Imagine your application has an API that presents product information. In procution you only 
need the GET endpoint as customers can only view products.  However, during acceptance testing, 
you want to add, update, and delete products to verify functionality. Rather than bypassing 
the API and inserting data directly into the database, you use the Invisible API to: 

-   Create products via the API, ensuring validation rules are applied.
-   Test scenarios that mimic actual user workflows.
-   Tear down the test data cleanly after tests run.

In production, these endpoints are completely hidden and inaccessible to anyone, 
ensuring they can only ever be used during controlled testing.



Components
----------

### `[InvisibleApi]` Attribute

The `[InvisibleApi]` attribute is used to mark an API endpoint or controller as conditionally invisible. If applied, the endpoint will be inaccessible unless:

-   A custom header with the correct key-value pair is included in the request.
-   The user is authenticated and belongs to a specific role.

#### Example:

```csharp
[InvisibleApi]
[HttpPost]
public async ValueTask<ActionResult<Product>> PostProductAsync(Product product)
{
    ...

    // POST logic

    ...
}
```

* * * * *

### `InvisibleApiMiddleware`

The middleware evaluates each incoming request:

1.  It checks if the target endpoint is decorated with `[InvisibleApi]`.
2.  It validates the presence of the custom header and its value.
3.  It ensures the user is authenticated and belongs to the specified role.
4.  If any of these checks fail, the middleware returns a `404 Not Found` response, making the endpoint inaccessible.

* * * * *

# Implementation

## Setup the Web Application

### Step 1: Create the key and value for the custom header

1.  Define a custom visibility header by creating an `InvisibleApiKey`:

    ```csharp

        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);
                var invisibleApiKey = new InvisibleApiKey();
                ConfigureServices(builder, builder.Configuration, invisibleApiKey);
                var app = builder.Build();
                ConfigurePipeline(app, invisibleApiKey);
                app.Run();
            }

            public static void ConfigurePipeline(WebApplication app, InvisibleApiKey invisibleApiKey)
            {
                ...
            }
        }
    ```

2.  Add the middleware to your application's pipeline:

    #### In `Program.cs` or `Startup.cs`:

    ```csharp

        public class Program
        {
            public static void Main(string[] args)
            {
                var builder = WebApplication.CreateBuilder(args);
                var invisibleApiKey = new InvisibleApiKey();
                ConfigureServices(builder, builder.Configuration, invisibleApiKey);
                var app = builder.Build();
                ConfigurePipeline(app, invisibleApiKey);
                app.Run();
            }

            public static void ConfigurePipeline(WebApplication app, InvisibleApiKey invisibleApiKey)
            {
                ...

                app.UseInvisibleApiMiddleware(invisibleApiKey);

                ...

            }
        }
    ```

* * * * *

### Step 2: Use the `[InvisibleApi]` Attribute

Mark the endpoints or controllers you want to make invisible with the `[InvisibleApi]` attribute:

```csharp
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : RESTFulController
    {
        private readonly IProductService productService;

        public ProductsController(IProductService productService) =>
            this.productService = productService;

        [InvisibleApi]
        [HttpPost]
        public async ValueTask<ActionResult<Product>> PostProductAsync(Product product)
        {
            ...
        }

        [HttpGet]
        public async ValueTask<ActionResult<IQueryable<Product>>> GetAllProductsAsync()
        {
            ...
        }

        [HttpGet("{productId}")]
        public async ValueTask<ActionResult<Product>> GetProductByIdAsync(Guid productId)
        {
            ...
        }

        [InvisibleApi]
        [HttpPut]
        public async ValueTask<ActionResult<Product>> PutProductAsync(Product product)
        {
            ...
        }

        [InvisibleApi]
        [HttpDelete("{productId}")]
        public async ValueTask<ActionResult<Product>> DeleteProductByIdAsync(Guid productId)
        {
            ...
        }
    }
````

In the above sample, the `PostProductAsync`, `PutProductAsync`, and `DeleteProductByIdAsync` 
endpoints are marked as invisible.  These endpoints will only be accessible if the custom 
header is present in the request and the user is authenticated with the required role.

* * * * *

### Step 3: Test the Configuration

1.  **Without Header or Incorrect Role:**

    -   Send a request to an endpoint marked with `[InvisibleApi]`.
    -   The response will be `404 Not Found` if the header is missing or the user does not belong to the required role.

2.  **With Correct Header and Role:**
    
    -   Setup your acceptance test project as per the next section to send a request with the custom header and the correct role.
    -   The response will be the expected API data.

* * * * *

## Setup the Web Application

### Step 1: Create a Test Web Application Factory

This will allow you to override the default authentication and authorization configuration.

```csharp
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                OverrideSecurityForTesting(services);
            });
        }

        private static void OverrideSecurityForTesting(IServiceCollection services)
        {
            // Find the first instance of InvisibleApiKey
            var invisibleApiKeyDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(InvisibleApiKey));

            InvisibleApiKey invisibleApiKey = null;

            if (invisibleApiKeyDescriptor != null)
            {
                // Resolve the InvisibleApiKey instance
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    invisibleApiKey = serviceProvider.GetService<InvisibleApiKey>();
                }
            }

            // Remove existing authentication and authorization
            var authenticationDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(IAuthenticationSchemeProvider));

            if (authenticationDescriptor != null)
            {
                services.Remove(authenticationDescriptor);
            }

            // Override authentication and authorization
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestScheme";
                options.DefaultChallengeScheme = "TestScheme";
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options =>
            {
                // Pass the InvisibleApiKey value to TestAuthHandler through Options
                options.Events = new AuthenticationEvents
                {
                    OnTicketReceived = context =>
                    {
                        context.Principal?.AddIdentity(new ClaimsIdentity(new[]
                        {
                            new Claim(ClaimTypes.Role, invisibleApiKeyValue)
                        }));
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("TestPolicy", policy => policy.RequireAssertion(_ => true));
            });
        }
    }
```

### Step 2: Create a Test Auth Handler
```csharp

```


Conclusion
----------

The `InvisibleApiMiddleware` and `[InvisibleApi]` attribute offer a robust solution for controlling API endpoint visibility. This approach enhances your application's security by requiring both valid headers and role-based authentication for access, making it an excellent choice for securing sensitive or administrative APIs.