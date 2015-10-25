using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

static class Extensions
{
    public static IEnumerable<string> NonEmpty(this IEnumerable<string> list)
        => list.Select(x => x.Trim()).Where(x => x != string.Empty);

    public static byte[] FixedGetResourceData(this EmbeddedResource resource)
    {
        // There's a bug in Mono.Cecil so when you access a resources data
        // the stream is not reset after use.
        var data = resource.GetResourceData();
        resource.GetResourceStream().Position = 0;
        return data;
    }
}