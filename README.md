dotMailer .NET API client
=========

Provides a simple .NET wrapper for the dotMailer REST API that encapsulates some of the lower-level HTTP protocol code leaving consumers with clean methods and simple objects with which to interact with dotMailer.

The solution consists of 3 projects:

### dotMailer.Api

This is the main project for interacting with the dotMailer API.  All the classes within are auto-generated from the dotMailer.Api.WadlParser (see below) so don't modify them individually here.

This contains the main Client class which is used to call methods on the API. Here's an example of retrieving address books with the Client:

```csharp
    var client = new Client("demo@apiconnector.com", "demo");
    var result = client.GetAddressBooks();
    foreach (var addressBook in result.Data)
      Console.WriteLine(addressBook.Name);
```

All of the API methods return a `ServiceResult` which will let you know whether the call was successful or not.  If the call was unsuccessful you'll also be given a simple, human-readable message to explain the problem.

```csharp
    var client = new Client("demo@apiconnector.com", "demo");
    var result = client.GetAddressBooks();
    if (!result.Success)
      Console.WriteLine("Something went wrong: " + result.Message);
```

If you're expecting some data back then you'll be given a `ServiceResult` with a correctly typed `Data` property which you can use.

```csharp
    var client = new Client("demo@apiconnector.com", "demo");
    var result = client.GetContacts();
    foreach (var contact in result.Data)
      Console.WriteLine(contact.Email);
```

### dotMailer.Api.WadlParser

This console application is responsible for retrieving the latest API definition (WADL) from http://api.dotmailer.com/v2/help/wadl and generating CLR methods and classes for rich interaction with the dotMailer API.

This application will generate C# class files on the local file system which can then be copied directly into the dotMailer.Api project root.  This was specifically built for the current dotMailer API WADL but could potentially be used on other definitions with some minor modifications.

### dotMailer.Api.Tests

Tests covering every method of the generated client code to ensure code coverage and completeness.

