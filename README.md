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

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Configures the RabbitMQ connection using the settings provided
    services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"));
}

