# Auth Toolbox

Fork of the [Digipolis Auth Toolbox](https://github.com/digipolisantwerp/auth_aspnetcore)

Adds authentication based on an internal API key passed in the request headers.

## Installation
```
  <ItemGroup>
    <PackageReference Include="Narato.Digipolis.Auth" Version="1.0.0" />
  </ItemGroup>
```
 
## Configuration
Some additional configuration options are available:

```
{
  "AUTH": {
    ...
    "ENABLEINTERNALAPIKEYHEADERAUTH": true, // flag to enable the authentication based on an internal API key
    "INTERNALAPIKEY": {
      "HEADERVALUE": "super-secret-apikey" // expected value of the internal API key
    } 
  }
}
```

## Startup

Add the following line to Startup.ConfigureServices

```
  services.Configure<InternalApikeyHeaderAuthenticationOptions>(Configuration.GetSection("AUTH:INTERNALAPIKEY"));
  
  service.AddAuth(...);
```
