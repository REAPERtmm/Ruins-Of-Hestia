using UnityEngine;
using UnityEngine.SceneManagement;

public class Campfire : MonoBehaviour
{
    [SerializeField] PlayerController Player;
    [SerializeField] RectTransform PannelEndOfMission;

    public void LoadScene(string scene_name)
    {
        // tout doux : utiliser le scene manager de ethan
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        SceneManager.LoadScene(scene_name, LoadSceneMode.Single);
    }


    void OpenPannel()
    {
        PannelEndOfMission.gameObject.SetActive(true);
    }

    void ClosePannel()
    {
        PannelEndOfMission.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            OpenPannel();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            ClosePannel();
        }
    }

}
