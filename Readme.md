# Epressionify

> Use extension methods in Entity Framework Core queries

## How to use

1) Install the two nuget packages `Clave.Expressionify` and `Clave.Expressionify.CodeGen`
2) Mark the `public static` extension method with `[Expressionify]`.
3) Call `.Expressionify()` at the beginning of the EF query.
4) Use the extension method in the query

## Example

```csharp
public static Extensions
{
    // This extension method will be expressionified
    [Expressionify]
    public static bool IsOver18(this User user)
        => user.DateOfBirth < DateTime.Now.AddYears(-18);
}

// create a query
var users = await db.Users
    .Expressionify() // This adds the magic...
    .Where(user => user.IsOver18()) // ...so we can use the method
    .ToListAsync();
```

## Limitations

Expressionify uses the Roslyn code analyzer and generator to look for `public` `static` methods with expression bodies tagged with the `[Expressionify]` attribute.

```csharp
// ✔ OK
[Expressionify]
public static int ToInt(this string value) => Convert.ToInt32(value);

// ❌ Not ok (it's not static)
[Expressionify]
public int ToInt(this string value) => Convert.ToInt32(value);

// ❌ Not ok (it's missing the attribute)
public int ToInt(this string value) => Convert.ToInt32(value);

// ❌ Not ok (it's not public)
[Expressionify]
private static int ToInt(this string value) => Convert.ToInt32(value);

// ❌ Not ok (it doesn't have an expression body)
[Expressionify]
public static int ToInt(this string value)
{
    return Convert.ToInt32(value);
}
```


## Inspiration and help

The first part of this project relies heavily on the work done by [Luke McGregor](https://twitter.com/staticv0id) in his [LinqExpander](https://github.com/lukemcgregor/LinqExpander) project, as described in his article on [composable repositories - nesting expressions](https://blog.staticvoid.co.nz/2016/composable_repositories_-_nesting_extensions/), and on the updated code by [Ben Cull](https://twitter.com/BenWhoLikesBeer) in his article [Expression and Projection Magic for Entity Framework Core ](https://benjii.me/2018/01/expression-projection-magic-entity-framework-core/).

The second part of this project uses Roslyn to analyze and generate code, and part of it is built directly on code by [Carlos Mendible](https://twitter.com/cmendibl3) from his article [Create a class with .NET Core and Roslyn](https://carlos.mendible.com/2017/03/02/create-a-class-with-net-core-and-roslyn/).

The rest is stitched together from various Stack Overflow answers and code snippets found on GitHub.