namespace Halite.Serialization.JsonNet;

using System;
using Newtonsoft.Json;

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
            writer.WriteValue(uri.PercentEncodedUrl());
        }
    }

    public override Uri? ReadJson(JsonReader reader, Type objectType, Uri? existingUri, bool hasExistingValue, JsonSerializer serializer)
    {
        var uriString = reader.Value as string;
        try
        {
            return new Uri(uriString, UriKind.RelativeOrAbsolute);
        }
        catch (Exception error)
        {
            throw new JsonReaderException($"Unable to create {nameof(System.Uri)} from {uriString}", error);
        }
    }
}