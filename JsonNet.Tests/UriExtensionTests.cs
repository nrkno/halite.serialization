namespace Halite.Serialization.JsonNet.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

public class UriExtensionTests
{
    public static (string, Uri, string)[] Scenarios = new (string, Uri, string)[]
    {
        ("Bokstaven ø i banen /øl skal kodes som %C3%B8 for en relativ eller absolutt URI.",
            new Uri("https://www.nrk.no/øl", UriKind.RelativeOrAbsolute),
            "https://www.nrk.no/%C3%B8l"),

        ("Bokstaven ø i parameteren litt=øl skal kodes som %C3%B8 for en relativ eller absolutt URI.",
            new Uri("https://www.nrk.no/drikke?litt=øl", UriKind.RelativeOrAbsolute),
            "https://www.nrk.no/drikke?litt=%C3%B8l"),
    };

    public static IEnumerable<object[]> MemberDataScenarios = Scenarios.Select(tuple =>
    {
        var (because, uri, expectedUrl) = tuple;
        return new object[] { uri, expectedUrl, because };
    });

    [Theory]
    [MemberData(nameof(MemberDataScenarios))]
    public void ShouldPercentEncodeUri(Uri uri, string expectedUrl, string because) =>
        uri.PercentEncodedUrl().ShouldBe(expectedUrl, because);
}