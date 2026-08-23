using TMPro;
using UnityEngine;

public class CreditsEntryView : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text titleText;

    public void SetEntry(CreditsEntry entry)
    {
        nameText.text = entry.name;
        titleText.text = entry.title;
    }
}