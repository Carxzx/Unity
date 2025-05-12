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
                Activar_UI();

                ActivarHotBar();
            }
        }
    }

    void Activar_UI(){
        if(Input.GetMouseButtonDown(0)){
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
                ActivarUI(TextBox);
            }else if(Raycast.collider.CompareTag("Door")){
                StartCoroutine(Not_ClickableTransicion());
            }else if(Raycast.collider.CompareTag("Other")){
                //Por hacer
            }
        }
    }

    void NotClickableUI(){
        if(Player.ChangeScenery){
            StartCoroutine(ClickableTransicion());
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

    private IEnumerator ClickableTransicion(){
        Player.ChangeScenery = false;

        ActivarUI(TransitionCircle);
        yield return new WaitForSeconds(0.5f); // Espera los primeros 0.5 segundos
        Door.PlayerTP(TransitionCircle, Player.collisionAux.collider.GetComponent<Door>());
        yield return new WaitForSeconds(0.5f); // Espera los otros 0.5 segundos
        OcultarUI(TransitionCircle);
    }

    private IEnumerator Not_ClickableTransicion(){
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

        Player.CanMove = false;
    }

    public void EventOff(){
        EventTrigger = false;

        HotBar.SetActive(true);
        Player.CanMove = true;
    }
}

