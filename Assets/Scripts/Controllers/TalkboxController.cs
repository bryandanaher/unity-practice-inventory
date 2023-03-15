using UnityEngine;

public class TalkboxController : MonoBehaviour {
    [SerializeField] private GameObject displayGraphic;
    private GraphicDisplayer graphicDisplayer;

    private void Awake() {
        PlayerInteractHandler.InteractEvent += PlayDisplayGraphicAnimation;
        graphicDisplayer = displayGraphic.GetComponent<GraphicDisplayer>();
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.name == "GroundCheck") {
            graphicDisplayer.ShowDisplayGraphic();
        }
    }
    
    private void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.name == "GroundCheck") {
            graphicDisplayer.HideDisplayGraphic();
        }
    }

    private void PlayDisplayGraphicAnimation(GameObject go) {
        if (go.GetComponent<PlayerInteractHandler>().CurrentInteractTarget != PlayerInteractTarget.Carnage) {
            return;
        }
        graphicDisplayer.PlayDisplayGraphicAnimation();
    }
}