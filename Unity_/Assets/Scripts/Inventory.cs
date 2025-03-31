using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    Vector2 mousePosition;

    void Update(){
        if(InventoryData.enMano){
            //Obtenemos la posición del ratón en la pantalla
            mousePosition = Input.mousePosition;

            InventoryData.objetoEnMano = GameObject.Find("objetoEnMano").transform.GetChild(0).GetComponent<Objeto>(); //Obtengo el objeto del Hijo del gameobject "objetoEnMano"
            InventoryData.objetoEnMano.transform.position = mousePosition;
        }
    }
}
