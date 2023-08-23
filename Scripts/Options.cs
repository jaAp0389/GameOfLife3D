/*****************************************************************************
* Project: GoL3D
* File   : Options.cs
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
*   07.11.2021	JA	Created
******************************************************************************/
using UnityEditor;
using UnityEngine;

public class Options : MonoBehaviour
{
    [SerializeField]
    int poolSizeInit = 10,
                         mapDimensions = 10,
                         updateDelay = 0;
    [SerializeField]
    [Range(0, 5)]
    float cursorTickRate = 1f,
                           cameraSpeed = 1f;
    //[SerializeField] bool isConstantUpdate = false;


    void Awake()
    {
        RefLibrary.sOptions = this.GetComponent<Options>();
    }
    void OnValidate()
    {
        Lib.sPoolSizeInit = poolSizeInit;
        Lib.sCursorTickRate = cursorTickRate;
        Lib.sMapDim = mapDimensions;
        Lib.sUpdateDelay = updateDelay;
        Lib.sCameraSpeed = cameraSpeed;
        //Lib.sIsConstantUpdate = isConstantUpdate;
    }
}

[CustomEditor(typeof(Options))]
class DecalMeshHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Reset Map"))
        {
            RefLibrary.sGoLBoard.Initialize();
            RefLibrary.sCursor.SetCursor();
        }
        if (GUILayout.Button("Do Single Step"))
        {
            RefLibrary.sGoLBoard.DoSingleStep();
            RefLibrary.sCursor.DeactivateCursor();
        }
        if (GUILayout.Button("Constant Update"))
        {
            Lib.sIsConstantUpdate = !Lib.sIsConstantUpdate;
            RefLibrary.sCursor.DeactivateCursor();
        }
    }
}
public static class Lib
{
    public static int sUpdateDelay,
                      sPoolSizeInit,
                      sMapDim;
    public static bool sIsConstantUpdate = false;
    public static float sCursorTickRate,
                        sCameraSpeed;
}

