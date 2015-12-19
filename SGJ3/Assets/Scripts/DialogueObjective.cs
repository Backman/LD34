using UnityEngine;

[CreateAssetMenuAttribute(menuName = "Objectives/Dialogue")]
public class DialogueObjective : QuestObjective
{
    public Dialog Dialog;

    DialogID _id;
    protected override void InnerStartObjective()
    {
        if (Dialog != null)
        {
            _id = GameManager.Instance.Dialog.PlayDialog(Dialog);
        }
    }

    protected override void InnerEndObjective()
    {
        
    }

    public override void UpdateObjective()
    {
		
    }

    public override bool IsDone()
    {
        if (Dialog == null)
        {
            return true;
        }
        return GameManager.Instance.Dialog.IsAlive(_id) == false;
    }
}