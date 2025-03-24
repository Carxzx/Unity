using UnityEngine;

public class Icons : MonoBehaviour
{
    public GameObject Balloon;

    Vector2 mousePosition;
    int layerMask;
    RaycastHit2D Raycast;


    void Start(){
        gameObject.SetActive(true);
        layerMask = LayerMask.GetMask("Interactuable");
    }

    void Update(){
        IconsHandler();
    }


    //Manejador de Iconos
    void IconsHandler(){
        //Obtenemos la posición del ratón en la pantalla
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        //Lanzamos un Raycast para comprobar lo que tiene justo debajo
        Raycast = Physics2D.Raycast(mousePosition,Vector2.zero,0f,layerMask);
        
        ManejarBalloon();
        ShowTile();
    }


    //Activa el globo de texto si el cursor está encima de un NPC y lo muestra en una posición concreta. Desactiva el globo de texto en caso contrario
    void ManejarBalloon(){
        //Controlan la posición del Bocadillo respecto del Objeto
        const float offsetX = -0.3f;
        const float offsetY = 1.4f;

        bool active = Raycast.collider != null && Raycast.collider.CompareTag("NPC") && !UI_Handler.ActiveUI;
        Balloon.SetActive(active);

        if(active){
            Balloon.transform.position = new Vector2(Raycast.collider.transform.position.x + offsetX, Raycast.collider.transform.position.y + offsetY);
        }
    }


    //Muestra con un cuadrado rojo la casilla sobre la que se encuentra el cursor
    void ShowTile(){ //MODIFICAR --> LAS FUNCIONES DEBUG NO PUEDEN USARSE PARA EL JUEGO
        const float RaycastDistance = 1f;
        Vector2 coords;

        if(Raycast.collider == null && !UI_Handler.ActiveUI){
            coords = new Vector2(Mathf.FloorToInt(mousePosition.x),Mathf.FloorToInt(mousePosition.y));
            Debug.DrawRay(coords,Vector2.up*RaycastDistance,Color.red); //Pintar Raycast
            Debug.DrawRay(coords,Vector2.right*RaycastDistance,Color.red); //Pintar Raycast

            coords = new Vector2(coords.x+1f,coords.y+1f);
            Debug.DrawRay(coords,Vector2.left*RaycastDistance,Color.red); //Pintar Raycast
            Debug.DrawRay(coords,Vector2.down*RaycastDistance,Color.red); //Pintar Raycast
        }
    }
}
