namespace Halite.Serialization.JsonNet.Tests;

using System;
using Shouldly;
using Xunit;

public class UriExtensionTests
{
    [Theory]
    [InlineData("https://www.nrk.no/øl", UriKind.RelativeOrAbsolute, "https://www.nrk.no/%C3%B8l")]
    [InlineData("/øl", UriKind.RelativeOrAbsolute, "/%C3%B8l")]
    [InlineData("/øl", UriKind.Relative, "/%C3%B8l")]
    [InlineData("https://www.nrk.no/drikke?litt=øl", UriKind.RelativeOrAbsolute, "https://www.nrk.no/drikke?litt=%C3%B8l")]
    [InlineData("/drikke?litt=øl", UriKind.Relative, "/drikke?litt=%C3%B8l")]
    [InlineData("/en/{templated}/lenke", UriKind.Relative, "/en/{templated}/lenke")]
    public void ShouldPercentEncodeUri(string givenUrl, UriKind withKind, string expectUrl) =>
        (new Uri(givenUrl, withKind)).PercentEncodedUrl().ShouldBe(expectUrl);
}