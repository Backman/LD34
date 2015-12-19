using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Objectives/Close To Tree")]
public class CloseToTreeObjective : QuestObjective
{
    public float Distance;

    private Transform _treeTransform;
    private Transform _ellisTransform;

    private static bool _textEnabled;

    public override bool IsDone()
    {
        var done = Mathf.Abs(_ellisTransform.position.x - _treeTransform.position.x) <= Distance;
        if (done && !_textEnabled)
        {
            HUD.Instance.EnableTexts();
        }
        return done;
    }

    public override void UpdateObjective()
    {
		
    }

    protected override void InnerEndObjective()
    {
		
    }

    protected override void InnerStartObjective()
    {
        _treeTransform = FindObjectOfType<TreeOfLife>().transform;
        _ellisTransform = FindObjectOfType<GardenerController>().transform;
    }
}