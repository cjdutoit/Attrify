# Attrify - Invisible API attibute and middleware
[Back to main page](../../README.md)

The `InvisibleApi` attribute is designed to work with the `InvisibleApiMiddleware` which is a lightweight 
ASP.NET Core middleware implementation designed to control the visibility of specific API endpoints in your application. 
By using the [InvisibleApi] attribute in combination with this middleware, you can make certain endpoints inaccessible 
(returning a 404 Not Found) unless specific conditions are met.

This approach is particularly useful for:

- Securing internal endpoints from general exposure.
- Providing an additional layer of security beyond standard authentication.

## Overview

The `Attrify - Invisible API` is a solution designed to hide specific API endpoints, primarily for acceptance testing purposes. 
Developers can uses the `[invisibleApi]` attribute to mark `Controllers` or `Actions` as invisible, eliminating the need for 
manual endpoint configuration in middleware. It also introduces an additional security layer by requiring that an authenticated 
user must belong to a certain role to access the endpoint. If these conditions are not met, the middleware intercepts the request 
and returns a `404 Not Found` response, making the endpoint invisible to unauthorized users.

This implementation is specifically tailored for testing scenarios where endpoints need to be hidden in production environments.
It is not intended to be used by consumers (internal or otherwise).


(The `Attrify - Invisible API` is an adaptation of the original [InvisibleApi](https://github.com/hassanhabib/invisibleapi) created by [Hassan Habib](https://github.com/hassanhabib))

* * * * *

### Key Benefits

-   **Enhanced Security:** Endpoints are only visible to authenticated users with the appropriate roles and a valid request header.
-   **Ease of Use:** A simple attribute-based implementation to control endpoint visibility.
-   **Scalability:** Works seamlessly with ASP.NET Core's middleware pipeline and authorization policies.

* * * * *

## Purpose / Use Case

The **Invisible API and Middleware** is designed to secure and hide internal endpoints used exclusively for **acceptance testing**, ensuring they remain inaccessible in production environments. These endpoints facilitate tasks such as:

- Setting up test data.
- Running specific test scenarios.
- Tearing down data after testing.

To safeguard these endpoints, the middleware employs a **twofold protection mechanism**:

1. **Request Header Validation:** A dynamically generated key-value pair must be present in the request headers.
2. **Role-Based Authorization:** The user must be authenticated and belong to a dynamically assigned role.


This middleware enables:

- Consistent data entry and test scenario creation via APIs that enforce business rules and validations negating the need for direct database manipulation.
- End-to-end testing by adhering to production-like constraints.
- Complete concealment of internal endpoints from real users in production or non-test scenarios.

---

### Key Features

- **Dynamic Security:**
  - Security values (request header and role) are randomly generated at application startup.
  - These values are not stored in configuration files or exposed to real users.

- **Dependency Injection Integration:**
  - The generated security values are registered within the DI container, enabling controlled access for acceptance testing without external exposure.

---

### How It Works in Your Application

In your application's `Program.cs` or `Startup.cs`, a unique request header key-value pair and a required role name are dynamically generated using the `InternalVisibleKey` class. This instance is registered in the **Dependency Injection (DI)** container within the `ConfigureServices` method and passed to middleware for runtime enforcement.

Endpoints intended to be hidden are decorated with the `[InvisibleApi]` attribute. The middleware evaluates all requests to these endpoints, ensuring:

1. The request contains the correct header.
2. The user is authenticated and belongs to the specified role.

If validation fails, the middleware responds with a **404 Not Found** status, effectively hiding the endpoint from unauthorized users.

---

### How It Works in Your Acceptance Test Project

In the acceptance test project, an API broker class initializes an instance of your web application via `Program.cs` or `Startup.cs` using a **test web application factory**.

The test factory inherits from the standard `WebApplicationFactory`, retaining its setup but allowing you to:

1. **Override default initialization**:
   - Replace real authentication and authorization services with a custom authentication scheme (`TestScheme`) using a `TestAuthHandler` and a permissive authorization policy (`TestPolicy`).

2. **Simulate authentication and authorization**:
   - The `TestAuthHandler` is configured to simulate an authenticated user. 
   - Since the DI container is accessible, it retrieves the registered `InternalVisibleKey` instance. 
   - The `TestAuthHandler` dynamically adds the required role to the user identity for authentication and authorization.

Additionally, the `HttpClient` in the API broker class is configured to include the custom header key-value pair in all requests, satisfying the middleware's validation. This setup enables the acceptance test project to access hidden endpoints securely, using dynamically generated security values.

---

### Example Use Case

Imagine your application has an API that provides product information. In production, only the **GET** endpoint is needed, as customers can only view products. However, during acceptance testing, you also need to **add**, **update**, and **delete** products to verify functionality. Instead of bypassing the API and inserting data directly into the database, you use the Invisible API to:

- Create products via the API, ensuring all validation rules are applied.
- Test scenarios that simulate actual user workflows.
- Cleanly tear down test data after tests are complete.

In production, these endpoints remain completely hidden and inaccessible, ensuring they can only be used during controlled testing environments.

---


Components
----------

### `[InvisibleApi]` Attribute

The `[InvisibleApi]` attribute is used to mark an API endpoint or controller as invisible. If applied, the endpoint will be inaccessible unless:

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

### Step 1: Create a Test Auth Handler

This handler will simulate an authenticated user with the required role.
```csharp
    public class TestAuthHandler : AuthenticationHandler<CustomAuthenticationSchemeOptions>
    {
        public TestAuthHandler(
            IOptionsMonitor<CustomAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            string randomOidGuid = Guid.NewGuid().ToString();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.Role, "Administrators"),
                new Claim("oid", randomOidGuid)
            };

            var invisibleApiKey = Options.InvisibleApiKey;
            if (invisibleApiKey != null && !string.IsNullOrWhiteSpace(invisibleApiKey.Key))
            {
                claims.Add(new Claim(ClaimTypes.Role, invisibleApiKey.Key));
            }

            var identity = new ClaimsIdentity(claims, "TestScheme");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestScheme");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
```

### Step 2: Create a Test Web Application Factory

This will allow you to override the default authentication and authorization configuration applying the `TestAuthHandler` and `TestPolicy` to simulate an authenticated user with the required role.

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
            var invisibleApiKeyDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(InvisibleApiKey));

            InvisibleApiKey invisibleApiKey = null;

            if (invisibleApiKeyDescriptor != null)
            {
                using (var serviceProvider = services.BuildServiceProvider())
                {
                    invisibleApiKey = serviceProvider.GetService<InvisibleApiKey>();
                }
            }

            var authenticationDescriptor = services
                .FirstOrDefault(d => d.ServiceType == typeof(IAuthenticationSchemeProvider));

            if (authenticationDescriptor != null)
            {
                services.Remove(authenticationDescriptor);
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "TestScheme";
                options.DefaultChallengeScheme = "TestScheme";
            })
            .AddScheme<CustomAuthenticationSchemeOptions, TestAuthHandler>("TestScheme", options =>
            {
                options.InvisibleApiKey = invisibleApiKey;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("TestPolicy", policy => policy.RequireAssertion(_ => true));
            });
        }
    }
```

### Step 3: Create an API Broker Class

This class initializes the test web application factory and configures an `HttpClient` to 
automatically include the custom header key-value pair in all requests.

```csharp
public partial class ApiBroker
{
    internal readonly TestWebApplicationFactory<Program> webApplicationFactory;
    internal readonly HttpClient httpClient;
    internal readonly IRESTFulApiFactoryClient apiFactoryClient;
    internal readonly InvisibleApiKey invisibleApiKey;

    public ApiBroker()
    {
        this.webApplicationFactory = new TestWebApplicationFactory<Program>();
        this.httpClient = this.webApplicationFactory.CreateClient();

        this.httpClient.DefaultRequestHeaders
            .Add(this.invisibleApiKey.Key, this.invisibleApiKey.Value);

        this.apiFactoryClient = new RESTFulApiFactoryClient(this.httpClient);
        this.invisibleApiKey = this.webApplicationFactory.Services.GetService<InvisibleApiKey>();
    }
}

**Note:** The `InvisibleApiKey` instance is retrieved from the DI container, providing 
access to the dynamically generated key-value pair.

The `ApiBroker` class allows you to securely access hidden endpoints in your acceptance test project. 
The custom header and required role are automatically included in all requests, so you don't need to 
adapt your tests to include them manually.


### Step 4: Create an acceptance test that will access the invisible endpoints

```csharp
public partial class ProductsApiTests
{
    [Fact]
    public async Task ShouldPostProductAsync()
    {
        // given
        Product randomProduct = CreateRandomProduct();
        Product inputProduct = randomProduct;
        Product expectedProduct = inputProduct;

        // when 
        await this.apiBroker.PostProductAsync(inputProduct);

        Product actualProduct =
            await this.apiBroker.GetProductByIdAsync(inputProduct.Id);

        // then
        actualProduct.Should().BeEquivalentTo(expectedProduct);
        await this.apiBroker.DeleteProductByIdAsync(actualProduct.Id);
    }
}
```

This test will access the `PostProductAsync` and `DeleteProductByIdAsync` endpoints, which is marked as invisible.

---

Acknowledgements
----------------
This version of the Invisible API is inspired by the work of [Hassan Habib](https://github.com/hassanhabib). His original implementation, [InvisibleApi](https://github.com/hassanhabib/invisibleapi), served as the foundation for this adaptation. 

You can find the original code and more details here:

- GitHub: [InvisibleApi](https://github.com/hassanhabib/invisibleapi)
- YouTube: [Invisible API Middleware](https://www.youtube.com/watch?v=qRiXEjbabH4)

---

Conclusion
----------

The `InvisibleApiMiddleware` and `[InvisibleApi]` attribute offer a robust solution for controlling API endpoint visibility. 
This approach enhances your application's security by requiring both valid headers and role-based authentication for access, 
making it an excellent choice for securing sensitive or administrative APIs.

---
