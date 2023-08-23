/*****************************************************************************
* Project: GoL3D
* File   : ObjectPool.cs
* Date   : 07.11.2021
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
using nPrefabFac;

namespace nPool
{
    public class ObjectPool
    {
        public ObjectPool(GameObject _object, int _poolSize)
        { 
            mObject = _object;
            pfabCreator = RefLibrary.sPrefabCreator;

            for (int i = 0; i < _poolSize; i++)
                mIdleObjects.Enqueue(GetNewPrefab());
        } 

        PrefabCreator pfabCreator;
        GameObject mObject;
        Queue<GameObject> mIdleObjects = new Queue<GameObject>();

        public void ReturnObject(GameObject _object)
        {
            _object.SetActive(false);
            mIdleObjects.Enqueue(_object);
        }

        public GameObject GetObject()
        {
            if (mIdleObjects.Count > 0) return mIdleObjects.Dequeue();
            else 
            {
                return GetNewPrefab();
            }
        }

        GameObject GetNewPrefab()
        {
            return pfabCreator.CreatePrefab(mObject);
        }
    }
}
