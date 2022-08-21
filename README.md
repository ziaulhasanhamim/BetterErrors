<div align="center">

# BetterErrors
### A very simple discriminated union of success or error

`dotnet add package BetterErrors`

</div>

- [Getting Started](#getting-started)
- [A more practical example](#a-more-practical-example)
- [Usage](#usage)
    - [Creating `Result<T>`](#creating-resultt)
        - [Creating `Result<T>` from `IError`](#creating-resultt-from-ierror)
    - [Checking if the `Result<T>` is an error](#checking-if-the-resultt-is-an-error)
    - [Accessing the `Result<T>` result](#accessing-the-resultt-result)
        - [Accessing the Value](#accessing-the-value)
        - [Accessing the Error](#accessing-the-error)
    - [Performing actions based on the `Result<T>` result](#performing-actions-based-on-the-resultt-result)
        - [Match](#match)
        - [Switch](#switch)
        - [Map](#map)
- [Result without a type](#result-without-a-type)
- [Error Types](#error-types)
    - [Built-in Error Types](#built-in-error-types)
    - [Custom error](#custom-error)
        - [Benefit of implementing `IError` interface](#benefit-of-implementing-ierror-interface)
        - [Benefit of inheriting `Error` record](#benefit-of-inheriting-error-record)
        - [Examples](#examples)
- [Suggestions and Changes](#suggestions-and-changes)
- [Credits](#credits)

# Getting Started


```cs
Result<User> GetUser(Guid id = default)
{
    if (id == default)
    {
        return new Error("Id is required");
    }

    return new User(Name: "Amichai");
}
```

```cs
GetUser(...).Switch(
    user => Console.WriteLine(user),
    error => Console.WriteLine(error.Message));
```

# A more practical example

```csharp
public Result<User> CreateUser(string name, string email)
{
    List<FieldErrorInfo> fieldErrors = new();

    if (name.Length < 2)
    {
        fieldErrors.Add(new("Name", "Name is too short"));
    }

    if (name.Length > 100)
    {
        fieldErrors.Add(new("Name", "Name is too long"));
    }

    if (!validateEmail(email))
    {
        fieldErrors.Add(new("Email", "Email is invalid"));
    }

    if (fieldErrors.Count > 0)
    {
        return new ValidationError("Provided data is not valid", fieldErrors);
    }
    ......User creation logic
}
```

```csharp
public Task<ErrorOr<User>> AddUserAsync(string name, string email)
{
    return CreateUser(name, email).Map<User>(async user => // Note: You have to pass the generic type parameter explicitly for map method
    {
        await _dbContext.AddAsync(user);
        return user;
    });
}
```

```csharp
[HttpGet("/users/create")]
public async Task<IActionResult> CreateUser(CreateUserRequest req)
{
    Result<User> userResult = await _repo.AddUserAsync(req.Name, req.Email);

    return userResult.Match(
        user => Ok(user),
        error => MapToActionResult(error));
}

public IActionResult MapToActionResult(IError err) => err switch
{
    ValidationError vErr => BadRequest(new { Errors = vErr.ErrorInfos, Message = vErr.Message }),
    NotFoundError nErr => NotFound(new { Message = nErr.Message }),
    _ => Problem()
}
```

# Usage

## Creating `Result<T>`

There are implicit converters from `T`, `Error`, `List<Error>`, `Error[]` to `Result<T>`

### Creating `Result<T>` from `IError`

`IError` is an interface and C# doesn't support implicit conversion from/to interface. So you have to call a Method `ToResult` on `IError`

```csharp
IError err = ...;
Result<T> result = Result.FromErr<T>(err);
// or
Result<T> result = err.ToResult<T>();
```

## Checking if the `Result<T>` is an error

```csharp
if (result.IsFailure)
{
    // result is an error
}
```

## Accessing the `Result<T>` result

### Accessing the Value

```csharp
Result<int> result = Result.From(5);

var value = result.Value;
```

### Accessing the Error

```csharp
Result<int> result = new NotFoundError(...);

IError value = result.Error;
```


## Performing actions based on the `Result<T>` result

### `Match`

```csharp
string foo = result.Match(
    value => value,
    error => $"err: {error.Message}");
```

### `Switch`

Actions that don't return a value on the value or list of errors

```csharp
result.Switch(
    value => Console.WriteLine(value),
    error => Console.WriteLine($"err: {error.Message}"));
```

### `Map`

Map method takes a delegate which will transform success result to `Result<TMap>`. The delegate will only be called if `Result<T>` contains a success result. If `Result<T>` is contains a failure result then a `Result<TMap>` will be constructed with the errors inside `Result<T>`


```csharp
Result<string> userNameResult = userResult.Map<string>(user => user.Username);
```

**Note: Pass the generic type parameter explicitly for Map method**

You can even return error in Map method

```csharp
Result<string> userNameResult = userResult.Map<string>(user => 
{
    if(user.HasName)
    {
        return user.Username;
    }
    return new Error("user doesn't have username");
});
```

## Result without a type

```csharp
Result DeleteUser(Guid id)
{
    var user = await _userRepository.GetByIdAsync(id);
    if (user is null)
    {
        return new NotFoundError("User not found");
    }

    await _userRepository.DeleteAsync(user);
    return Result.Success;
}
```

## Error Types

### Built-in Error Types

- Error
- ValidationError,
- AggregateError,
- NotFoundError


### Custom error

You can create your own error type. You can either implement the `IError` interface or inherit the `Error` record. 

#### Benefit of implementing `IError` interface

You get more control on your type. But you can't implicitly convert it to Result<T>

#### Benefit of inheriting `Error` record

Can be converted Implicitly to Result<T>

#### Examples
```csharp
FileNotFoundError notFoundErr = new("file.txt");
Result<T> result = Result.FromErr<T>(notFoundErr); // Ok
Result<T> result = notFoundErr.ToResult<T>(); // Ok
Result<T> result = notFoundErr; // Error

public class FileNotFoundError : IError
{
    public FileNotFoundError(string fileName)
    {
        FileName = fileName;
    }

    public string FileName { get; }

    public string Message => $"{FileName} was not found";

    public string Code => nameof(FileNotFoundError);
}
```

```csharp
FileNotFoundError notFoundErr = new("file.txt");
Result<T> result = Result.FromErr<T>(notFoundErr); // Ok
Result<T> result = notFoundErr.ToResult<T>(); // Ok
Result<T> result = notFoundErr; // Ok too

public record FileNotFoundError(string FileName) : Error($"{FileName} was not found", nameof(FileNotFoundError));
```

## Suggestions and Changes

Any suggestion on improving this project will be helpful

## Credits

- [ErrorOr](https://github.com/amantinband/error-or) - A simple, fluent discriminated union of an error or a result. BetterErrors package is directly inspired from ErrorOr package with the benefits of error customization

