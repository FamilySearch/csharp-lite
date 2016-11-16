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

Most of the code exists in a single file [FamilySearchSDK.cs](https://github.com/FamilySearch/csharp-lite/blob/master/Gedcomx.Api.Lite.Core/FamilySearchSDK.cs) with supporting enumeration files Environment.cs and MediaType.cs

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

Here is how you might use this project. 

#### Step 1
Obtain a *username*, *password* and *applicationKey* refer to the [Family Search Developers Site](https://familysearch.org/developers/)

#### Step 2
Instantiate a ``FamilySearchSDK`` Class supplying the following parameters:
* username from step 1.
* password from step 1.
* application key, sometimes refered to as client key, from step 1.
* application name,  preferably from the dev registration site, but could be anything header friendly
* version likely a string such as 1.0.0
* environment Enumeration such as integration (was sandbox), beta and production
```
var ft = new FamilySearchSDK("username", "password", "applicationKey", "Example2.Core", "1.0.0", Gedcomx.Api.Lite.Environment.Integration);
var response = ft.Get("/platform/tree/persons/" + personId).Result;
Console.WriteLine(response.persons[0].display.name);
```

#### Step 3
Make an api call such as Get, Post, Put, Delete which returns a Task by default. Adding ``.Result`` will wait for the response. Note that the async and await methodology is encouraged. For more information refer to [Microsoft's documentation on async and await](https://msdn.microsoft.com/en-us/library/hh191443(v=vs.110).aspx).

```
var ft = new FamilySearchSDK("username", "password", "applicationKey", "Example2.Core", "1.0.0", Gedcomx.Api.Lite.Environment.Integration);
var response = ft.Get("/platform/tree/persons/" + personId).Result;
Console.WriteLine(response.persons[0].display.name);
```
#### Step 4
Visit the [Family Search Resources](https://familysearch.org/developers/docs/api/resources) site for details on how to get, post, put and delete content from Family Search.

## Community Samples

* *Ancestory Map* - Using the C# lite SDK, you can see a person's ancestory on a world map by clicking [coming]. It combines ASP.Net 5 Core, Google API's, d3.js and Asure. Source Code can be found at this repository: <https://www.microsoft.com/en-us/download/details.aspx?id=48130>.

* Please contact Family Search to add more samples.

