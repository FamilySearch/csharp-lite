# Welcome

This C# project is intended to extend the [Family Search APIs](https://familysearch.org/developers/) using the [GEDCOM X](http://www.gedcomx.org) specifications.
This project shall also replace the original [C# Family Search SDK](https://github.com/FamilySearch/gedcomx-csharp), however several parts of that SDK can still be utilized. The Nuget Packages are:

| NuGet Package Id | Purpose | Notes |
|------------------|---------|-------|
| **[Gedcomx.Api.Lite](TODO)** | C# Lite version to contact the Family Search GEDCOM X APIs. It will help with authentication and media types through HttpClient. | Can be used alone or along with the other Model libraries. |
| **[Gedcomx.Api.Lite.Core](TODO)** | C# Core version of the Gedcomx.Api.Lite package. | |
| [Gedcomx.Model](http://www.nuget.org/packages/Gedcomx.Model/) | Contains the models for GEDCOM X data. | Use this by itself to just work with the GEDCOM X models and data, and not any web services or files. |
| [Gedcomx.Model.Fs](http://www.nuget.org/packages/Gedcomx.Model.Fs/) | Contains FamilySearch specific GEDCOM X model extensions. | Use this by itself to just work with the GEDCOM X FamilySearch extension models and data, and not any web services or files. |
| [Gedcomx.Model.Rs](http://www.nuget.org/packages/Gedcomx.Model.Rs/) | Contains REST specific GEDCOM X model extensions. | Use this by itself to just work with the GEDCOM X REST extension models and data, and not any web services or files. (This project adds atom feed models.) |

## Build

#### Prerequisites:
* Microsoft .NET Framework 4.6. The web installer can be downloaded here: <https://www.microsoft.com/en-us/download/details.aspx?id=48130>
* NuGet 2.8. Instructions for installing can be found here: <http://docs.nuget.org/docs/start-here/installing-nuget>

#### Project Descriptions:
* Example1 - Contains person creation, relationship creation, search etc.. using the Lite SDK with Models
* Example2.Core - Contains a search example using the new .Net Core approach.
* Gedcomx.Api.Lite - Contains a linked copy of **all** the source needed to run the SDK.
* Gedcomx.Api.Lite.Core - Contains **all** the source needed to run the SDK, but targeting .Net Core Implementations.
* Gedcomx.Api.Lite.Tests - Contains basic test to utilize the api code base and do some code testing.

## Example

Here's how you might use this project. 

To obtain a username, password and applicationKey refer to the [Family Search Developers Site](https://familysearch.org/developers/)

Note that the async and await methodology is supported. For more information refer to [Microsoft's documentation on async and await](https://msdn.microsoft.com/en-us/library/hh191443(v=vs.110).aspx).
 
```
var ft = new FamilySearchSDK("username", "password", "applicationKey", "Example2.Core", "1.0.0", Gedcomx.Api.Lite.Environment.Integration);
var response = ft.Get("/platform/tree/persons/" + personId).Result;
Console.WriteLine(response.persons[0].display.name);
```

## Community Samples

* Ancestory Map - Using the C# lite SDK, you can see a person's ancestory on a world map by clicking [coming]. It combines ASP.Net 5 Core, Google API's, d3.js and Asure. Source Code can be found at this repository: <https://www.microsoft.com/en-us/download/details.aspx?id=48130>.

* Please contact Family Search to add more samples.

