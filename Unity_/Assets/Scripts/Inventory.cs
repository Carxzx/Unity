using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    Vector2 mousePosition;
    GameObject objetoEnMano;

    void Start(){
        objetoEnMano = GameObject.Find("objetoEnMano");
    }

    void Update(){
        if(InventoryData.enMano){
            //Obtenemos la posición del ratón en la pantalla
            mousePosition = Input.mousePosition;

            objetoEnMano.transform.position = mousePosition;
        }
    }
}
