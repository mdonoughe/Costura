using System.Collections.Generic;
using System.Linq;

static class Extensions
{
    public static IEnumerable<string> NonEmpty(this IEnumerable<string> list)
        => list.Select(x => x.Trim()).Where(x => x != string.Empty);
}