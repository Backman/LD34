using UnityEngine;
using System.Collections;

[CreateAssetMenu]
public class Dialog : ScriptableObject
{
    [System.Serializable]
    public struct DialogEntry
    {
        public Person Person;
        [Multiline]
        public string Text;
        public float StartDelay;
        public float ScrollForwardTime;
        public float LingerTime;
    }

    public DialogEntry[] Entries;
}
