namespace Halite.Serialization.JsonNet;

using System;
using System.Text.RegularExpressions;

public static class UriExtension
{
    private static Uri FakeBaseUri = new Uri("https://host", UriKind.Absolute);

    private static Regex TemplatedUrlPattern = new Regex(@"[{][a-zæøå]*[}]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public static string PercentEncodedUrl(this Uri uri)
    {
        if (uri.IsAbsoluteUri)
        {
            return uri.AbsoluteUri;
        }
        else if (TemplatedUrlPattern.IsMatch(uri.OriginalString))
        {
            return uri.OriginalString;
        }
        else
        {
            var absoluteUri = new Uri(FakeBaseUri, uri);
            return absoluteUri.PathAndQuery;
        }
    }
}