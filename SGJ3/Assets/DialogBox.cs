using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogBox : MonoBehaviour
{
    public Text Text;
    struct GraphicState
    {
        public Graphic Graphic;
        public float Alpha;
    }
    [HideInInspector]
    GraphicState[] Graphics;


    public void SetAlpha(float alpha)
    {
        if (Graphics == null)
        {
            var graphics = GetComponentsInChildren<Graphic>();
            Graphics = new GraphicState[graphics.Length];
            for (int i = 0; i < graphics.Length; i++)
            {
                Graphics[i].Graphic = graphics[i];
                Graphics[i].Alpha = graphics[i].color.a;
            }
        }
        for (int i = 0; i < Graphics.Length; i++)
        {
            var g = Graphics[i];
            var color = g.Graphic.color;
            color.a = alpha * g.Alpha;
            g.Graphic.color = color;
        }
    }
}
