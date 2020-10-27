# Introduction
`Trace` is a trace management for logging operations in .NET.

Its purpose is to encapsulate trace operations for different locations (e.g. console, file etc.). In addition trace messages can be categorized to be filtered at application level. The following sections show the details

# Trace Service Management
The core component is the trace management service which controls registered tracers and handles also the categories. Only messages matching the wanted categories will be processed by each known tracer

# Register Tracers
Initially no tracers are known to the service. To associate a tracer it has to be registered. This is a task that is done at the very beginning of each application. A tracer has to implement the `ITracer` interface.
In the current release there are only a few build-in tracers available. This will grow in the future.

# Build-In Tracers
For the current release there are the following build-in tracers available:
* ConsoleTracer
* FileTracer
* DelegateTracer

# Tracer-Adapters
NLog is supported via an adapter, check out NuGet package NTrace.Adapters.NLog for more information

# Trace Types
A message can be traced of a specific type, like:

* Error
* Warning
* Information

# Trace Categories
To control the purpose of a traced message, it can be assigned a specific category. The following categories are available right now:

* Application
* Connection
* Method
* Data
* Query
* Debug

At application level, these categories can be combined (e.g. Data | Query) where you only want to see those data in you log to keep it clean.
But if writing an error, the message will be traced, regardless of the filtered category.

# IoC Support
`NTrace` is designed for the use in an IoC (Inversion of Control scenario) environment. For that it provides an interface `ITraceService`.
