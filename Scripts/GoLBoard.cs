/*****************************************************************************
* Project: GoL3D
* File   : GoLBoard.cs
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using nPool;

public class GoLBoard : MonoBehaviour
{
    [SerializeField] GameObject mObject;
    [SerializeField] Material mMaterial;

    [SerializeField] CameraController mCamera;
    int minX, maxX, minY, maxY, minZ, maxZ;

    int mPoolSizeInit;
    int mMapDim;
    int mCurrColor = 0;
    Options mOptions;

    ObjectPool mObjPool;
    GameObject[,,] mObjectMap;
    bool[,,] mMap, mTempMap;
    Material[] mColors;
    int mUpdateCountdown;
    bool isStepDone = true,
         isObjectsPresent = false;

    void Awake()
    {
        mUpdateCountdown = Lib.sUpdateDelay;
        RefLibrary.sGoLBoard = this.GetComponent<GoLBoard>();
    }
    void Start()
    {
        mOptions = RefLibrary.sOptions;
        Initialize();
        mColors = CreateMaterials();
    }

    private void Update()
    {
        if (Lib.sIsConstantUpdate && isStepDone)
        {
            if (Lib.sUpdateDelay != 0)
            {
                if (mUpdateCountdown > 0)
                    mUpdateCountdown -= 1;
                else
                {
                    mUpdateCountdown = Lib.sUpdateDelay;
                    GameStep();
                }
            }
            else GameStep();
        }
    }
    public void Initialize()
    {

        if (isObjectsPresent) ClearMap();
        mMapDim = Lib.sMapDim;
        mPoolSizeInit = Lib.sPoolSizeInit;
        mObjPool = new ObjectPool(mObject, mPoolSizeInit);
        mMap = new bool[mMapDim, mMapDim, mMapDim];
        mObjectMap = new GameObject[mMapDim, mMapDim, mMapDim];
        GameStep();
        isObjectsPresent = true;

        ResetOutlines();
        AdjustCamera();
    }

    public void DoSingleStep()
    {
        if (Lib.sIsConstantUpdate)
            Lib.sIsConstantUpdate = false;
        GameStep();
    }
    public void PlaceCell(Vector3 _position, bool place)
    {
        Vector3Int pos = Vector3Int.FloorToInt(_position);

        mMap[pos.x, pos.y, pos.z] = place;

        if (place)
            PlaceObject(pos.x, pos.y, pos.z);
        else RemoveObject(pos.x, pos.y, pos.z);
    }

    void ResetOutlines()
    {
        minX = mMapDim / 2;
        maxX = minX +1;
        minY = minX -1;
        maxY = minX +1;
        minZ = minX -1;
        maxZ = minX +1;
        minX = minX -1;
    }
    void GameStep()
    {
        isStepDone = false;
        UpdateMap();
        UpdateObjectMap();
        isStepDone = true;
    }

    void ClearMap()
    {
        for (int x = 0; x < mMapDim; x++)
        {
            for (int y = 0; y < mMapDim; y++)
            {
                for (int z = 0; z < mMapDim; z++)
                {
                    RemoveObject(x, y, z);
                }
            }
        }
    }

    int ActiveCells()
    {
        int count = 0;
        for (int x = 0; x < mMapDim; x++)
        {
            for (int y = 0; y < mMapDim; y++)
            {
                for (int z = 0; z < mMapDim; z++)
                {
                    if (mMap[x, y, z]) count++;
                }
            }
        }
        return count;
    }

    void UpdateObjectMap()
    {
        mCurrColor++;
        if (mCurrColor > 5) mCurrColor = 0;

        for (int x = 0; x < mMapDim; x++)
        {
            for (int y = 0; y < mMapDim; y++)
            {
                for (int z = 0; z < mMapDim; z++)
                {
                    if (mMap[x, y, z] == mTempMap[x, y, z]) continue;

                    if (mTempMap[x, y, z]) PlaceObject(x, y, z);
                    else RemoveObject(x, y, z);
                }
            }
        }
        mMap = mTempMap;
    }


    void UpdateMap()
    {
        mTempMap = new bool[mMapDim, mMapDim, mMapDim];
        for (int x = 0; x < mMapDim; x++)
        {
            for (int y = 0; y < mMapDim; y++)
            {
                for (int z = 0; z < mMapDim; z++)
                {
                    //if (!map[x, y, z]) continue;
                    mTempMap[x, y, z] = CheckNeighbours(x, y, z);
                }
            }
        }
    }
    bool CheckNeighbours(int _x, int _y, int _z)
    {
        int count = 0;
        //count += mMap[_x, _y, _z] ? -1 : 0;

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int z = -1; z < 2; z++)
                {
                    if (x == 0 && y == 0 && z == 0) continue;

                    int xTmp = _x + x,
                        yTmp = _y + y,
                        zTmp = _z + z;

                    if (xTmp < 0) xTmp = mMapDim - 1;
                    if (yTmp < 0) yTmp = mMapDim - 1;
                    if (zTmp < 0) zTmp = mMapDim - 1;

                    if (xTmp >= mMapDim) xTmp = 0;
                    if (yTmp >= mMapDim) yTmp = 0;
                    if (zTmp >= mMapDim) zTmp = 0;

                    if (mMap[xTmp, yTmp, zTmp]) count++;
                }
            }
        }

        return IsAlive(mMap[_x, _y, _z], count);

    }
    void UpdateOutlines(int _x, int _y, int _z)
    {
        if (_x < minX) minX = _x;
        if (_x > maxX) maxX = _x;
        if (_y < minY) minY = _y;
        if (_y > maxY) maxY = _y;
        if (_z < minZ) minZ = _z;
        if (_z > maxY) maxZ = _z;
    }

    void AdjustCamera()
    {
        Vector3 outlineCenter = new Vector3((minX + maxX) / 2,
                                            (minY + maxY) / 2,
                                            (minZ + maxZ) / 2);

        mCamera.SetCameraCenter(outlineCenter,
            Mathf.Max(-minX + maxX, -minY + maxY, -minZ + maxZ));

    }

    bool IsAlive(bool _alive, int _neighbours)
    {
        if (_alive)
        {
            if (_neighbours < 2 || _neighbours > 3)
                return false;
            return true;
        }

        if (_neighbours != 3) return false;
        return true;

    }

    void PlaceObject(int _x, int _y, int _z)
    {
        UpdateOutlines(_x, _y, _z);
        AdjustCamera();

        GameObject obj = mObjPool.GetObject();
        obj.transform.position = new Vector3(_x, _y, _z);
        obj.GetComponent<Renderer>().material = mColors[mCurrColor];
        obj.SetActive(true);
        mObjectMap[_x, _y, _z] = obj;
    }
    void RemoveObject(int _x, int _y, int _z)
    {
        if (mObjectMap[_x, _y, _z] == null) return;
        GameObject temp = mObjectMap[_x, _y, _z];
        mObjectMap[_x, _y, _z] = null;
        mObjPool.ReturnObject(temp);
    }
    Color PickColor(int _color)
    {
        switch (_color)
        {
            case 0: return Color.white;
            case 1: return Color.cyan;
            case 2: return Color.blue;
            case 3: return Color.green;
            case 4: return Color.red;
            case 5: return Color.yellow;
            default: return Color.black;
        }
    }
    Material[] CreateMaterials()
    {
        Material[] tempMatArr = new Material[7];
        for (int i = 0; i < 6; i++)
        {
            Material tempMat = new Material(mMaterial);
            tempMat.color = PickColor(i);
            tempMatArr[i] = tempMat;
        }
        return tempMatArr;
    }
}

