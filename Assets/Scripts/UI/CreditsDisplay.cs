using System.Collections.Generic;
using UnityEngine;

public class CreditsDisplay : MonoBehaviour
{
    [SerializeField] private CreditsConfig config;
    [SerializeField] private CreditsEntryView entryPrefab;
    [SerializeField] private Transform contentParent;

    private void Awake()
    {
        Populate();
    }

    public void Populate()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        List<CreditsEntry> sortedEntries = CreditsSorter.SortByTitle(config);
        foreach (CreditsEntry entry in sortedEntries)
        {
            CreditsEntryView view = Instantiate(entryPrefab, contentParent);
            view.SetEntry(entry);
        }
    }
}