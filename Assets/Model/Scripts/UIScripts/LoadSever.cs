using UnityEngine;

public class LoadSever : MonoBehaviour {


    public GameObject loading;
    public GameObject butAssets;

    private void OnEnable()
    {
        loading.SetActive(false);
        butAssets.SetActive(true);
    }

    public void OpenUrlAssets()
    {
        loading.SetActive(true);
        butAssets.SetActive(false);
        UpdateSeverAssets.Instance.Init();
        StartCoroutine(LoadAB.Instance.LoadUIName("load",CallBack));
    }

    private void CallBack(GameObject obj)
    {
       if(obj!=null)
        {
            gameObject.SetActive(false);
            obj.SetActive(true);
        }
    }
}
