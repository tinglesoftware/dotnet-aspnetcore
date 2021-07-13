# ASP.NET Core convenience functionality

![GitHub Workflow Status](https://img.shields.io/github/workflow/status/tinglesoftware/dotnet-aspnetcore/Build%20and%20Publish?style=flat-square)

This repository contains projects/libraries for adding useful functionality to ASP.NET Core when running real world applications in production. The packages depend on [ASP.NET Core](https://github.com/dotnet/aspnetcore). You can find samples, documentation and getting started instructions for ASP.NET Core in the [AspNetCore](https://github.com/dotnet/aspnetcore) repository. We have been using this packages at [Tingle](https://tingle.software) for years and thought it is better if we shared them.

## Packages

|Package|Version|Description|
|--|--|--|
|`Tingle.AspNetCore.Authorization`|[![NuGet](https://img.shields.io/nuget/v/Tingle.AspNetCore.Authorization.svg)](https://www.nuget.org/packages/Tingle.AspNetCore.Authorization/)|Additional authorization functionality such as handlers and requirements. See [docs](./src/Tingle.AspNetCore.Authorization/README.md) and [sample](./samples/AuthorizationSample)|
|`Tingle.AspNetCore.DataProtectionMongoDB`|[![NuGet](https://img.shields.io/nuget/v/Tingle.AspNetCore.DataProtectionMongoDB.svg)](https://www.nuget.org/packages/Tingle.AspNetCore.DataProtectionMongoDB/)|Data Protection store in [MongoDB](https://mongodb.com) for ASP.NET Core. See [docs](./src/Tingle.AspNetCore.DataProtectionMongoDB/README.md) and [sample](./samples/DataProtectionMongoDBSample).|
|`Tingle.AspNetCore.JsonPatch.NewtonsoftJson`|[![NuGet](https://img.shields.io/nuget/v/Tingle.AspNetCore.JsonPatch.NewtonsoftJson.svg)](https://www.nuget.org/packages/Tingle.AspNetCore.JsonPatch.NewtonsoftJson/)|Helpers for validation when working with JsonPatch in ASP.NET Core. See [docs](./src/Tingle.AspNetCore.JsonPatch.NewtonsoftJson/README.md) and [blog](https://medium.com/swlh/immutable-properties-with-json-patch-in-aspnet-core-25185f493ea8).|
|`Tingle.AspNetCore.Tokens`|[![NuGet](https://img.shields.io/nuget/v/Tingle.AspNetCore.Tokens.svg)](https://www.nuget.org/packages/Tingle.AspNetCore.Tokens/)|Support for generation of coninuation tokens in ASP.NET Core with optional expiry. Useful for pagination, user invite tokens, expiring operation tokens, etc. This is availed through the `ContinuationToken<T>` and `TimedContinuationToken<T>` types. See [docs](./src/Tingle.AspNetCore.Tokens/README.md) and [sample](./samples/TokensSample).|

### Issues &amp; Comments

Please leave all comments, bugs, requests, and issues on the Issues page. We'll respond to your request ASAP!

### License

The Library is licensed under the [MIT](http://www.opensource.org/licenses/mit-license.php "Read more about the MIT license form") license. Refer to the [LICENSE](./LICENSE) file for more information.
