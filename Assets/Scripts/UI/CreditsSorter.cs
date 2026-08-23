using System.Collections.Generic;
using System.Linq;

public static class CreditsSorter
{
    public static List<CreditsEntry> SortByTitle(CreditsConfig config)
    {
        return config.entries.OrderBy(e => e.title).ToList();
    }
}