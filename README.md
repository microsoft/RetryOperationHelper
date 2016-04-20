# RetryOperationHelper
Retry Operation Helper is a simple class which encapsulates the retry of any given piece of C# .NET logic a given number of times before failing, as well as running an optional function on failure. It is useful for any call where transient exception are possible, for example, making a call to a remote database or service.
