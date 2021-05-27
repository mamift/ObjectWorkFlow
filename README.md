# Port in-progress!

This is an in-progrss port of the existing code from: https://github.com/tkellogg/objectflow to .NET Core 3.1 (for the test projects) and .NET Standard for the libraries themselves.
A few breaking changes are underway!

# Objectflow: Workflows for busy developers

Objectflow is a framework for declaratively creating workflows in C# with a fluent syntax. Objectflow allows you to consolidate your business logic in the usual places (the model) so the workflow only has to focus on how those pieces fit together. 
```C#
// Define a workflow
var workflow = new StatefulWorflow<SiteVisit>()

// Configure security
.Configure(config => config.Security.AsStrict.UsingMethod(
	() => _factory.GetAllowedTransitions())

// Declare a starting point
.Yield("Scheduled")

.Do(x => x.Open())
.Yield("In Progress")

.Unless(x => x.IsReadyToBeClosed()).Fail("Site visit was not ready to be closed")
.Do(x => x.Close());
```

Objectflow provides several services that can optionally be plugged in; including security, persistence, error handling, and UI integration. Each workflow is built inside plain C# classes, making them completely compatible with dependency injection frameworks (unlike Workflow Foundation).

We also provide other facilities for interacting with workflows to provide persistance and security frameworks.
See the [Quickstart guide](https://github.com/tkellogg/objectflow/wiki/Home) for more information.

## Installation

Objectflow is available via NuGet under the code `StatefulObjectflow`. (http://www.nuget.org/List/Packages/StatefulObjectflow)


## Contributing

If you would like to suggest a feature or report a bug, open an issue. If you want to contribute code, fork the repository, make changes, and send a pull request through github.