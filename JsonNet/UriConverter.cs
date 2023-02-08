namespace Halite.Serialization.JsonNet;

using System;
using Newtonsoft.Json;

/*
 * This converter is currently not working properly.
 */
public class UriConverter : JsonConverter<Uri>
{
    public override void WriteJson(JsonWriter writer, Uri? uri, JsonSerializer serializer)
    {
        if (uri is null)
        {
            writer.WriteValue(string.Empty);
        }
        else
        {
            /*
             * System.Text.Json is able to encode System.Uri properly, but how?
             * The source seems to be doing exactly the same as we're doing here:
             * https://github.com/dotnet/runtime/blob/main/src/libraries/System.Text.Json/src/System/Text/Json/Serialization/Converters/Value/UriConverter.cs
             */
            writer.WriteValue(uri.OriginalString);
        }
    }

    public override Uri? ReadJson(JsonReader reader, Type objectType, Uri? existingUri, bool hasExistingValue, JsonSerializer serializer) =>
        reader.Value switch
        {
            string uri => new Uri(uri, UriKind.RelativeOrAbsolute),
            _ => null,
        };
}