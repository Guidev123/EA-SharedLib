<p align="center">
  <a href="https://dotnet.microsoft.com/" target="blank"><img src="https://upload.wikimedia.org/wikipedia/commons/e/ee/.NET_Core_Logo.svg" width="120" alt=".NET Logo" /></a>
</p>


# SharedLib

This repository contains two common libraries to support the implementation of microservices, focusing on validation, domain modeling, event-driven architecture, asynchronous communication, and RabbitMQ integration.

## Packages

### 1. `SharedLib.Domain`
`SharedLib.Domain` is a domain library that provides resources for:

- **Validation and Domain Modeling**: Simplifies the creation of entities and the application of validation rules.
- **Event-Driven Architecture (EDA)**: Implements essential concepts of event-driven architecture, such as domain events and integration events.
- **CQRS**: Support for Command Query Responsibility Segregation (CQRS), separating read and write responsibilities.
- **Mediator**: Contains its own abstraction of MediatR to facilitate communication between application components without direct dependencies between them.

### 2. `SharedLib.MessageBus`
`SharedLib.MessageBus` is a library that abstracts communication with RabbitMQ, offering:

- **RabbitMQ Abstraction**: Provides a simplified way to integrate with RabbitMQ, allowing you to send and receive messages and events easily.
- **Dependency Injection**: The library includes its own implementation of dependency injection to configure and manage communication with RabbitMQ.
- **Ease of Use**: Simplifies the publishing and subscribing of events and messages in an intuitive way, without the need to manually manage complex communication with RabbitMQ.

Program.cs:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Configures the RabbitMQ connection using the settings provided
    services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"));
}

```
appsettings.json:
```json
"MessageBus": {
  "MessageQueueConnection": "host=localhost:5672;publisherConfirms=true;timeout=10"
},
```

### 3. `SharedLib.Tokens`
`SharedLib.Tokens` is a library that facilitates working with JWT and JWKS using asymmetric keys.

```cli
dotnet add package SharedLib.Tokens --version 1.0.0
```

https://www.nuget.org/packages/SharedLib.Tokens/

## Client-Side Configuration (Token Validation)

Program.cs:
```csharp
builder.Services.AddJwtConfiguration(builder.Configuration);
app.UseAuthConfiguration();
```
appsettings.json:
```json
"JwksSettings": {
  "JwksEndpoint": "https://localhost:3000/jwks"
},
```

## Token Issuer-Side Configuration (Token generation)

Program.cs:
```csharp
builder.Services.AddJwksManager(x => x.Jws = Algorithm.Create(DigitalSignaturesAlgorithm.EcdsaSha256))
    .PersistKeysToDatabaseStore<AuthenticationDbContext>()
    .UseJwtValidation();

app.UseAuthConfiguration();
app.UseJwksDiscovery();
```

AuthenticationDbContext.cs:
```csharp
public class AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options)
           : IdentityDbContext(options), ISecurityKeyContext
{
    public DbSet<KeyMaterial> SecurityKeys { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
```
