using UnityEngine;

public class Show : MonoBehaviour {

	public void OpenUILoad()
    {
        StartCoroutine(LoadAB.Instance.LoadUIName("load",CallBack));

    }

    public void OpenUIUpdate()
    {
        StartCoroutine(LoadAB.Instance.LoadUIName("update",CallBack));

    }

    private void CallBack(GameObject obj)
    {
        if (obj != null)
            obj.SetActive(true);
            gameObject.SetActive(false);
    }

}

