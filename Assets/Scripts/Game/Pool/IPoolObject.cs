﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolObject
{
    void OnSetActive(bool pValue);

    void OnReturnToPool();

    void SetActive(bool pValue);

    void ReturnToPool();

    bool IsMine();

    //PoolObject GetPoolObject();
}
