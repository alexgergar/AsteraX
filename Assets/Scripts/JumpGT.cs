using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JumpGT : MonoBehaviour
{
    public Text jumpText;

    // Use this for initialization
    void Start()
    {
        if (jumpText == null)
        {
            jumpText = GetComponent<Text>();
        }
        jumpText.text = AsteraX.JUMPS.ToString() + " Jumps";
        EventBroker.UpdateJumps += UpdateJumpAmount;
    }


    private void UpdateJumpAmount(int jumpsLeft)
    {
        if (jumpsLeft < 0)
        {
            return;
        }
        switch (jumpsLeft)
        {
            case 0:
                jumpText.text = jumpsLeft.ToString() + " Jumps";
                break;
            case 1:
                jumpText.text = jumpsLeft.ToString() + " Jump";
                break;
            default:
                jumpText.text = jumpsLeft.ToString() + " Jump";
                break;
        }

    }
}
