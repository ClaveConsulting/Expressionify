# Expressionify

[![Nuget](https://img.shields.io/nuget/v/Clave.Expressionify)][1] [![Nuget](https://img.shields.io/nuget/dt/Clave.Expressionify)][1] [![Build Status](https://claveconsulting.visualstudio.com/Nugets/_apis/build/status/ClaveConsulting.Expressionify?branchName=master)][2] [![Azure DevOps tests](https://img.shields.io/azure-devops/tests/ClaveConsulting/Nugets/14)][2]

> Use extension methods in Entity Framework Core queries

## Installing

Install these two nuget packages:

* `Clave.Expressionify`
* `Clave.Expressionify.Generator`

Make sure to install the second one properly:

```xml
  <ItemGroup>
    <PackageReference Include="Clave.Expressionify" Version="5.0.0" />
    <PackageReference Include="Clave.Expressionify.Generator" Version="5.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
  </ItemGroup>
```

## How to use

1) Mark the `public static` expression method with the `[Expressionify]` attribute.
1) Mark the class with the method as `partial`.
2) Call `.Expressionify()` in your Entity Framework query chain, before using any extension method.
3) Use the extension method in the query

## Example

Lets say you have this code:

```csharp
var users = await db.Users
    .Where(user => user.DateOfBirth < DateTime.Now.AddYears(-18))
    .ToListAsync();
```

That second line is a bit long, so it would be nice to pull it out as a reusable extension method:

```csharp
public static Extensions
{
    public static bool IsOver18(this User user)
        => user.DateOfBirth < DateTime.Now.AddYears(-18);
}

// ...

var users = await db.Users
    .Where(user => user.IsOver18())
    .ToListAsync();

```

Unfortunately this forces Entity Framework to run the query in memory, rather than in the database. That's not very efficient...

But, with just two additional lines of code we can get Entity Framework to understand how translate our extension method to SQL

```diff
public static partial Extensions
{
+   [Expressionify]
    public static bool IsOver18(this User user)
        => user.DateOfBirth < DateTime.Now.AddYears(-18);
}

// ...

// create a query
var users = await db.Users
+   .Expressionify()
    .Where(user => user.IsOver18())
    .ToListAsync();
```

## Upgrading from 3.1 to 5.0

Version 5 works with net 5.0, and has a few other changes. It relies on [Source generators](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/) and Roslyn Analyzers for generating the code, instead of some very clumpsy msbuild code. This means that you will get help if you forget to mark the methods correctly. 

A big change from version 3.1 to 5.0 is that the class no longer needs to be static, but it has to be a partial class.

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
public static int ToInt(this string value) => Convert.ToInt32(value);

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



[1]: https://www.nuget.org/packages/Clave.Expressionify/
[2]: https://claveconsulting.visualstudio.com/Nugets/_build/latest?definitionId=14
