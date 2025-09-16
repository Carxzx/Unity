using UnityEngine;
using System.Collections;

public class UI_Handler : MonoBehaviour
{
    static public bool ActiveUI;

    public GameObject TextBox;
    public GameObject Menu;
    public GameObject TransitionCircle;
    public GameObject Inventory;
    public GameObject HotBar;
    public GameObject Clock;
    public GameObject Icons;

    Vector2 mousePosition;
    RaycastHit2D Raycast;
    int layerMask;

    public bool EventTrigger;

    void Start(){
        layerMask = LayerMask.GetMask("Interactuable");
        ActiveUI = false;
        EventTrigger = false;
    }
    
    void Update(){
        if(!EventTrigger){
            //Obtenemos la posición del ratón en la pantalla
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //Lanzamos un Raycast para comprobar lo que tiene justo debajo
            Raycast = Physics2D.Raycast(mousePosition,Vector2.zero,0f,layerMask);

            if(ActiveUI){
                Desactivar_UI();

                DesactivarHotBar();
            }else{
                ActivarHotBar();

                Activar_UI();
            }
        }
    }

    void Activar_UI(){
        if(Input.GetMouseButtonDown(1)){ //Click derecho
            ClickableUI();
        }else{
            NotClickableUI();
        }
    }

    void Desactivar_UI(){
        if(Menu.activeSelf && Input.GetKeyDown(KeyCode.Escape)){
            OcultarUI(Menu);
        }
        if(Inventory.activeSelf && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E))){
            OcultarUI(Inventory);
        }
    }

    void ClickableUI(){
        if(Raycast.collider != null && Player.InRange(Raycast.collider.transform.position,Player.InteractuableDistance)){
            if(Raycast.collider.CompareTag("NPC")){
                Raycast.collider.GetComponent<EventTrigger>().Dialogo();
            }else if(Raycast.collider.CompareTag("Door")){
                StartCoroutine(Not_ClickableTransicion());
            }
        }
    }

    void NotClickableUI(){
        if(Player.ChangeScenery){
            StartCoroutine(Not_ClickableTransicion());
        }
        if(Input.GetKeyDown(KeyCode.E)){
            ActivarUI(Inventory);
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            ActivarUI(Menu);
        }
    }

    public static void ActivarUI(GameObject objeto){
        Player.CanMove = false;
        ActiveUI = true;
        objeto.SetActive(true);
    }

    public static void OcultarUI(GameObject objeto){
        Player.CanMove = true;
        ActiveUI = false;
        objeto.SetActive(false);
    }

    private IEnumerator Not_ClickableTransicion(){
        Player.ChangeScenery = false;

        Vector2 halfSize = new Vector2(0.25f, 0.25f);
        Vector2 offset = new Vector2(0f,0.25f);

        Vector2 bottomLeft  = (Vector2)Player.tf.position + offset + new Vector2(-halfSize.x, -halfSize.y);
        Vector2 bottomRight = (Vector2)Player.tf.position + offset + new Vector2( halfSize.x, -halfSize.y);
        Vector2 topLeft     = (Vector2)Player.tf.position + offset + new Vector2(-halfSize.x,  halfSize.y);
        Vector2 topRight    = (Vector2)Player.tf.position + offset + new Vector2( halfSize.x,  halfSize.y);

        float rayLength = 2f; // la distancia que quieras
        int layerMask = LayerMask.GetMask("OutOfBounds");

        RaycastHit2D[] hits = new RaycastHit2D[8];
        hits[0] = Physics2D.Raycast(bottomLeft, Vector2.left, rayLength, layerMask);
        hits[1] = Physics2D.Raycast(bottomLeft, Vector2.down, rayLength, layerMask);

        hits[2] = Physics2D.Raycast(bottomRight, Vector2.right, rayLength, layerMask);
        hits[3] = Physics2D.Raycast(bottomRight, Vector2.down, rayLength, layerMask);

        hits[4] = Physics2D.Raycast(topLeft, Vector2.left, rayLength, layerMask);
        hits[5] = Physics2D.Raycast(topLeft, Vector2.up, rayLength, layerMask);

        hits[6] = Physics2D.Raycast(topRight, Vector2.right, rayLength, layerMask);
        hits[7] = Physics2D.Raycast(topRight, Vector2.up, rayLength, layerMask);

        for (int i = 0; i < hits.Length; i++){
            if(hits[i].collider != null && hits[i].collider.CompareTag("OutOfBoundsDoor")){
                Debug.Log(hits[i].collider);
                ActivarUI(TransitionCircle);
                yield return new WaitForSeconds(0.5f); // Espera los primeros 0.5 segundos
                Door.PlayerTP(TransitionCircle, hits[i].collider.GetComponent<Door>());
                yield return new WaitForSeconds(0.5f); // Espera los otros 0.5 segundos
                OcultarUI(TransitionCircle);

                break;
            }
        }

        yield return null;
    }

    private IEnumerator ClickableTransicion(){
        RaycastHit2D AuxRaycast = Raycast;

        ActivarUI(TransitionCircle);
        yield return new WaitForSeconds(0.5f); // Espera los primeros 0.5 segundos
        Door.PlayerTP(TransitionCircle, AuxRaycast.collider.GetComponent<Door>());
        yield return new WaitForSeconds(0.5f); // Espera los otros 0.5 segundos
        OcultarUI(TransitionCircle);
    }

    void DesactivarHotBar(){
        HotBar.SetActive(false);
    }

    void ActivarHotBar(){
        HotBar.SetActive(true);
    }

    public void EventOn(){
        EventTrigger = true;

        TextBox.SetActive(false);
        Menu.SetActive(false);
        TransitionCircle.SetActive(false);
        Inventory.SetActive(false);
        HotBar.SetActive(false);
        Clock.SetActive(false);
        Icons.SetActive(false);

        Player.CanMove = false;
        Player.walking = false;
        Player.BloquearMovimiento = true;
    }

    public void EventOff(){
        EventTrigger = false;

        HotBar.SetActive(true);
        Clock.SetActive(true);
        Icons.SetActive(true);
        Player.CanMove = true;
        Player.BloquearMovimiento = false;
    }
}

