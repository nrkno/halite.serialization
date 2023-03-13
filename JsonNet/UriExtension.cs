namespace Halite.Serialization.JsonNet;

using System;

/*
 * System.Text.Json is able to encode System.Uri properly, but how?
 * The source seems to be doing exactly the same as we're doing here:
 * https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/Converters/Value/UriConverter.cs
 */
public static class UriExtension
{
    public static string PercentEncodedUrl(this Uri uri)
    {
        return uri.OriginalString;
    }
}