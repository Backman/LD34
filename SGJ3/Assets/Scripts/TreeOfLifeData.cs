using UnityEngine;

[CreateAssetMenu]
public class TreeOfLifeData : ScriptableObject
{
    [System.Serializable]
    public struct TreeOfLifeSegment
    {
        public float GrowthPercentage;
        public float StartAtGrowthValue;
        public AudioClip StartSegmentClip;
        public GameObject Prefab;
        // public float GrowthMultiplier;
    }
    public Gradient EvolveBlinkGradient;
    public int EvolveBlinkCount;
    public float EvolveBlinkWaitTime;
    public float Health = 100f;
    public float MaxGrowth = 1000f;
    public float StartGrowthPercentage = 10f;
    public GameObject TakeDamagePrefab;
    public float GrowthSpeed;
    public TreeOfLifeSegment[] Segments;
}