# Tingle.AspNetCore.Authorization

Authorization refers to the process that determines what a user is able to do. For example, an administrative user is allowed to create a document library, add documents, edit documents, and delete them. A non-administrative user working with the library is only authorized to read the documents.

Authorization is orthogonal and independent from authentication. However, authorization requires an authentication mechanism. Authentication is the process of ascertaining who a user is. Authentication may create one or more identities for the current user.

Below are some of the functionalities that the library provides to aid with authorization work flows.

## IP Address Based Authorization

### User Defined IPs

It is a common scenario whereby we may require to only allow HTTP requests from certain IPs. 

In appsettings.json...

```json
{
    "WhitelistIPs": [
      "::1/128",
      "127.0.0.1/32"
    ]
}
```

### Sample Usage Dependency Injection

```cs
public void ConfigureServices(IServiceCollection services)
{
    ...

    services.AddAuthorization(options => 
    {
         options.AddPolicy("my_auth_policy", policy =>
        {
            policy.AddAuthenticationSchemes("my_auth_scheme")
                    .RequireAuthenticatedUser()
                    .RequireApprovedNetworks(Configuration.GetSection("WhitelistIPs"));
        });
    });

    // add accessor for HttpContext i.e. implementation of IHttpContextAccessor
    services.AddHttpContextAccessor();

    // add IAuthorizationHandler for approved networks
    services.AddApprovedNetworksHandler();
    ...
}
```

Details of the implementation of `my_auth_scheme` authentication scheme have been omitted here since it is beyod the scope of this discussion. More details on how to handle authentication in ASP.NET Core can be found [here](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-6.0).

The above code section defines `my_auth_policy` authorization policy which ensures the user who has been authenticated via the `my_auth_scheme` has access to the resource they're trying to gain access to. Using `RequireApprovedNetworks` extension method on the `AuthorizationPolicyBuilder` we can then add a comma separated list of IP networks that are approved to access the resource from.

We also have added a call to the `services.AddHttpContextAccessor()` extension method in order to allow us to gain access to the `HttpContext` which contains the details of the IP address that the request is originating from.

Finally, we have a call to the `services.AddApprovedNetworksHandler()` which adds an instance of the `ApprovedIPNetworkHandler`. This authorization handler then makes a decision if authorization is allowed by checking if the request ip is among the whitelist provided in the authorization policy.

Now, we can use this functionality to authorize access to a controller as shown below:

```cs
[Authorize("my_auth_policy")]
public class DummyController : ControllerBase
{
    ...
}
```

Is that it?...Wait there's more!

### Fully Qualified Domain Names

Alternatively, you can provide a list of fully qualified domain names and each of them will be resolved to the list of IP addresses. Let us see how to do this with an example:

In appsettings.json...

```json
{
    "WhitelistDomains": ["contoso.com", "northwind.com"]
}
```

### Sample Usage Dependency Injection

```cs
public void ConfigureServices(IServiceCollection services)
{
    ...

    services.AddAuthorization(options => 
    {
         options.AddPolicy("my_auth_policy", policy =>
        {
            policy.AddAuthenticationSchemes("my_auth_scheme")
                    .RequireAuthenticatedUser()
                    .RequireNetworkFromDns(Configuration.GetSection("WhitelistDomains"));
        });
    });

    // same as the first example
    ...
}
```

### Azure IPs

For developers who are working with Microsoft Azure and they'd wish to whitelist all their ip addresses they can do that easily as demonstrated below:

### Sample Usage Dependency Injection

```cs
public void ConfigureServices(IServiceCollection services)
{
    ...

    services.AddAuthorization(options => 
    {
         options.AddPolicy("my_auth_policy", policy =>
        {
            policy.AddAuthenticationSchemes("my_auth_scheme")
                    .RequireAuthenticatedUser()
                    .RequireAzureIPNetworks();
        });
    });

    // same as the first example
    ...
}
```

If you however do not wish to whitlist the entire range of Azure IPs, you can provide `serviceId` and `region` parameters to `RequireAzureIPNetworks` to scope the range of IPs based on the Azure service id and/or region.