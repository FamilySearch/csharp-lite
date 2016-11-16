using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gedcomx.Api.Lite.Tests.Properties;
using Newtonsoft.Json;
using System.Net;
using System.Linq;

namespace Gedcomx.Api.Lite.Tests
{
	[TestClass]
	public class PortraitTests
	{
		[TestMethod]
		public void GetAPortrait()
		{
			var ft = new FamilySearchSDK(Settings.Default.UserName, Settings.Default.Password, Settings.Default.ApplicationKey,
				TestBacking.AppName, TestBacking.AppVersion, Gedcomx.Api.Lite.Environment.Integration);

			// Create a Person
			var gedcomx = new Gx.Gedcomx();
			gedcomx.AddPerson(TestBacking.GetCreateMalePerson());
			var postResults = ft.Post("/platform/tree/persons", JsonConvert.SerializeObject(gedcomx), MediaType.X_GEDCOMX_v1_JSON).Result;

			// Now get the new person.
			string personId = ((string[])postResults.Headers.Location.ToString().Split('/')).Last();
			var portraitResponse = ft.Get($"/platform/tree/persons/{personId}/portrait", MediaType.X_FS_v1_JSON);
			var portrait = portraitResponse.Result;

			Assert.IsTrue(postResults.StatusCode == HttpStatusCode.Created, "Portrait should flag the reply as created");
		}
	}
}
