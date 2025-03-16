using UnityEngine;

public class Balloon : MonoBehaviour
{
    //Controlan la posición del Bocadillo respecto del Objeto
    const float offsetX = -0.3f;
    const float offsetY = 1.4f;

    public Transform tf;
    public SpriteRenderer SR;
    public GameObject TextBox;
    Vector2 mousePosition;
    int layerMask;
    RaycastHit2D Raycast;


    void Start(){
        gameObject.SetActive(true);
        SR.enabled = false;
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

        if(Raycast.collider != null){
            if(Raycast.collider.CompareTag("NPC")){
                if(Input.GetMouseButtonDown(0)){
                    ActivarTextBox();
                }
            }else if(Raycast.collider.CompareTag("Other")){
                //Por hacer
            }
        }
        ManejarBalloon();
    }


    //Activa el globo de texto si el cursor está encima de un NPC y lo muestra en una posición concreta. Desactiva el globo de texto en caso contrario
    void ManejarBalloon(){
        SR.enabled = Raycast.collider != null && Raycast.collider.CompareTag("NPC") && !TextBox.activeSelf;
        if(SR.enabled){
            tf.position = new Vector2(Raycast.collider.transform.position.x + offsetX, Raycast.collider.transform.position.y + offsetY);
        }
    }


    //Activa el Cuadro de Texto
    void ActivarTextBox(){
        TextBox.SetActive(true);
    }
}
