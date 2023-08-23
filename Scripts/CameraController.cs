/*****************************************************************************
* Project: GoL3D
* File   : CameraController.cs
* Date   : 25.11.2021
* Author : Jan Apsel (JA)
*
* These coded instructions, statements, and computer programs contain
* proprietary information of the author and are protected by Federal
* copyright law. They may not be disclosed to third parties or copied
* or duplicated in any form, in whole or in part, without the prior
* written consent of the author.
*
* History:
*   26.11.2021	JA	Created
******************************************************************************/
using System;
using System.Collections.Generic;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    Vector3 mTarget;

    private void FixedUpdate()
    {
        transform.LookAt(mTarget);
        transform.Translate(Vector3.right * Lib.sCameraSpeed * Time.deltaTime);
    }

    public void SetCameraCenter(Vector3 _target, int maxWide)
    {
        if (maxWide >= Lib.sMapDim - 1) return;
        mTarget = _target;
        transform.position = mTarget + new Vector3(-1.4f, 0, 0) * (maxWide + 1);
    }
}

