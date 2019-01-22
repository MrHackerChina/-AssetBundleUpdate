using UnityEngine;

public class Load : MonoBehaviour {

	public void OnClickButton()
    {
        StartCoroutine(LoadAB.Instance.LoadUIName("show", Callback));
    }

    private void Callback(GameObject obj)
    {
        if(obj!=null)
        {
            obj.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
