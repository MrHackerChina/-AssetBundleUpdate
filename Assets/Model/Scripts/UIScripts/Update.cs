using UnityEngine;

public class Update : MonoBehaviour{

    public void LoadSeverAsset()
    {
        gameObject.SetActive(false);
        LoadAB.Instance.LoadAsset();
    }

}
