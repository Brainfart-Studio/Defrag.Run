using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CreditsEntry
{
    public string name;
    public string title;
}

[CreateAssetMenu(fileName = "CreditsConfig", menuName = "UI/CreditsConfig")]
public class CreditsConfig : ScriptableObject
{
    public List<CreditsEntry> entries = new();
}