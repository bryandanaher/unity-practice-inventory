using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChanger : MonoBehaviour {
    public AudioSource buttonSound;

    private void Awake() {
        PlayerInteractHandler.InteractEvent += ChangeScene;
    }
    
    public void ChangeScene(GameObject go) {
        if (go.GetComponent<PlayerInteractHandler>().CurrentInteractTarget == PlayerInteractTarget.Doorway) {
            StartCoroutine(ActuallyChangeTheScene("DanceParty"));
        }
    }

    private IEnumerator ActuallyChangeTheScene(string sceneName) {
        buttonSound.Play();
        yield return new WaitWhile(() => buttonSound.isPlaying);
        SceneManager.LoadScene(sceneName);
    }

    public void Exit() {
        Application.Quit();
    }
}