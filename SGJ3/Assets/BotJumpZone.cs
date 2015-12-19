using UnityEngine;
using System.Collections;

public class BotJumpZone : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D collision)
    {
        BotControl control = collision.GetComponent<BotControl>(); 
        if (control != null)
        {
            control.InJumpZone();
        }
    }
}
