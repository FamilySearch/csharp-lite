using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gedcomx.Api.Lite.Tests.Properties;

namespace Gedcomx.Api.Lite.Tests
{
	[TestClass]
	public class AuthenticationTests
	{
		[TestMethod]
		public void AuthenticateTest()
		{
			var ft = new FamilySearchSDK(Settings.Default.UserName, Settings.Default.Password, Settings.Default.ApplicationKey,
				TestBacking.AppName, TestBacking.AppVersion, Environment.Integration);

			Assert.IsNotNull(ft.AccessToken, "Authentication needs an access token. Check username and password");
		}
	}
}
