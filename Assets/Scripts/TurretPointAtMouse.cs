using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretPointAtMouse : MonoBehaviour
{
    private void Update()
    {
        AimTurret();
    }

    private void AimTurret()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        // Need z position and back it up to the correct plane
        Vector3 zPos = Vector3.back * Camera.main.transform.position.z;
        // Add z position to mouseScreenPosition
        mouseScreenPosition += zPos;

        // this could have been written in one line ---- Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.back * Camera.main.transform.position.z);

        transform.LookAt(Camera.main.ScreenToWorldPoint(mouseScreenPosition), Vector3.back);

    }

}
