using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTrans : MonoBehaviour
{
    public Transform trans;

    //(Gab) Vraiment weird comme script mais a une utilit�e!
    //Bug dans Rig system de Unity. Faire cela contourne le probl�me.
    void LateUpdate()
    {
        transform.position = trans.position;
    }
}
