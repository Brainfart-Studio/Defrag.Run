using Dan.Models;
using TMPro;
using UnityEngine;

public class LeaderboardEntryView : MonoBehaviour
{
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text scoreText;

    public void SetEntry(Entry entry)
    {
        rankText.text = (entry.Rank + 1).ToString();
        nameText.text = entry.Username;
        scoreText.text = entry.Score.ToString();
    }
}