/*****************************************************************************
* Project: GoL3D
* File   : Cursor.cs
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

using UnityEngine;

public class Cursor : MonoBehaviour
{
    [SerializeField] GameObject cursorObj;
    GameObject cursor;

    Vector3 cPos;
    int mapDim;
    float cursTickRate;
    float cursTick = 0f;

    void Start()
    {
        RefLibrary.sCursor = this.GetComponent<Cursor>();
        cursor = Instantiate(cursorObj, transform);
        SetCursor();
    }
    public void SetCursor()
    {
        mapDim = Lib.sMapDim;
        cursTickRate = Lib.sCursorTickRate;
        cPos = new Vector3Int(mapDim / 2, mapDim / 2, mapDim / 2);
        cursor.transform.position = cPos;
        cursor.SetActive(true);
    }
    public void DeactivateCursor()
    {
        cursor.SetActive(false);
    }

    void Update()
    {
        DoAction();
        cursTick += 0.016f;
        if (cursTick > cursTickRate)
        {
            DoMovement();
            cursTick = 0f;
        }
    }

    void DoMovement()
    {
        Vector3 tempPos = Vector3.zero;
        if (Input.GetKey(KeyCode.A)) tempPos += Vector3.forward;
        if (Input.GetKey(KeyCode.D)) tempPos -= Vector3.forward;
        if (Input.GetKey(KeyCode.W)) tempPos -= Vector3.left;
        if (Input.GetKey(KeyCode.S)) tempPos += Vector3.left;
        if (Input.GetKey(KeyCode.G)) tempPos += Vector3.up;
        if (Input.GetKey(KeyCode.B)) tempPos -= Vector3.up;

        tempPos += cPos;
        if (tempPos.x < 0.1f || tempPos.x > mapDim - 0.1f ||
           tempPos.y < 0.1f || tempPos.y > mapDim - 0.1f ||
           tempPos.z < 0.1f || tempPos.z > mapDim - 0.1f)
            return;
        cPos = tempPos;
        cursor.transform.position = cPos;
    }
    void DoAction()
    {
        if (Input.GetKeyDown(KeyCode.Return)) Place();
        else if (Input.GetKeyDown(KeyCode.Backspace)) Remove();
    }
    void Place()
    {
        RefLibrary.sGoLBoard.PlaceCell(cPos, true);
    }
    void Remove()
    {
        RefLibrary.sGoLBoard.PlaceCell(cPos, false);
    }
}