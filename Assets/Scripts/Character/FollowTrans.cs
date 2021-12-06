using UnityEngine;

public class FollowTrans : MonoBehaviour
{
    public Transform trans;
    public bool dontDestroy = false;

    private void Start()
    {
        if (dontDestroy)
            DontDestroyOnLoad(gameObject);
    }

    //(Gab) Vraiment weird comme script mais a une utilit�e!
    //Bug dans Rig system de Unity. Faire cela contourne le probl�me.
    void LateUpdate()
    {
        transform.position = trans.position;
    }
}
