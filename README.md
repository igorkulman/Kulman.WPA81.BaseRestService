Base REST service for Universal Apps
============================

[![Build status](https://ci.appveyor.com/api/projects/status/tyk3ff6jamxondgh?svg=true)](https://ci.appveyor.com/project/igorkulman/kulman-wpa81-baserestservice)

Base class for a Windows Phone 8.1 (Silverlight), Windows Phone 8.1 XAML and Windows 8.1 REST service implementation.

## Installation

[![NuGet version](http://img.shields.io/nuget/v/Kulman.WPA81.BaseRestService.svg?style=flat)](https://nuget.org/packages/Kulman.WPA81.BaseRestService)  [![NuGet downloads](http://img.shields.io/nuget/dt/Kulman.WPA81.BaseRestService.svg?style=flat)](https://nuget.org/packages/Kulman.WPA81.BaseRestService)

	PM> Install-Package Kulman.WPA81.BaseRestService
	
## Usage

Create your service class and inherit from BaseRestService. The minimum you need to do to make it work is to override the GetBaseUrl() method to set the base url for all the requests.

```csharp
public class MyDataService: BaseRestService
{
  protected override string GetBaseUrl()
  {
    return "my base url";
  }
}
```  
  
You can (but do not have to) also override the GetRequestHeaders() method to set the default request headers.
  
```csharp  
protected override Dictionary<string, string> GetRequestHeaders()
{
    return new Dictionary<string, string>
    {
        {"Accept-Encoding", "gzip, deflate"},
        {"Accept", "application/json"},
    };
}
```

Now you can use the following methods in your class

```csharp
Task<T> Get<T>(string url);
Task<T> Put<T>(string url, object request);
Task<T> Post<T>(string url, object request);
Task<T> Patch<T>(string url, object request);
Task Delete(string url);
Task<Dictionary<string, string>> Head(string url);
```

If you need to get the raw request, there are overloads returning `HttpResponseMessage`

```csharp
Task<HttpResponseMessage> Get(string url);
Task<HttpResponseMessage> Put(string url, object request);
Task<HttpResponseMessage> Post(string url, object request);
Task<HttpResponseMessage> Patch(string url, object request);
```

Methods in your service may then look like this

```csharp
public Task<List<Account>> GetAccounts()
{
  return Get<List<Account>>("/accounts");
}
 
public Task<Account> UpdateAccount(Account account)
{
  return Patch<Account>("/accounts",account);
}
```

For more information, see my blog post [REST service base class for Windows Phone 8.1 XAML apps](http://blog.kulman.sk/rest-service-base-class-for-windows-phone-8-1-xaml-apps/).

### Changelog

**Version 1.2**

- adding support for passing cancellation tokens
- removed Jetbrains.Annotations binary depedency
