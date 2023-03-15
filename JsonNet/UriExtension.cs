namespace Halite.Serialization.JsonNet;

using System;

public static class UriExtension
{
    private static Uri FakeBaseUri = new Uri("https://host", UriKind.Absolute);

    public static string PercentEncodedUrl(this Uri uri)
    {
        if (uri.IsAbsoluteUri)
        {
            return uri.AbsoluteUri;
        }
        else if (uri.IsTemplated())
        {
            /*
            * Denne løsningen fungerer ikke for lenker som er templated
            * og som innholder tegn som må prosent-enkodes.
            */
            return uri.OriginalString;
        }
        else
        {
            var absoluteUri = new Uri(FakeBaseUri, uri);
            return absoluteUri.PathAndQuery;
        }
    }

    /*
     * Dette er en håpløst dårlig måte å sjekke om en URI er en del
     * av en lenker som er templated.
     * Denne informasjonen finnes egentlig i HalLinkObject,
     * og burde hentes derfra, i stedet for å gjenoppdages her.
     */
    private static bool IsTemplated(this Uri uri) =>
        uri.OriginalString.Contains("{");
}