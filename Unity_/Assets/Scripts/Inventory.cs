using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Inventory : MonoBehaviour
{
    Vector2 mousePosition;
    Vector2 EsquinaDerecha = new Vector2(25,-25);

    void Update(){
        if(InventoryData.enMano){
            //Obtenemos la posición del ratón en la pantalla
            mousePosition = Input.mousePosition;

            InventoryData.objetoEnMano = GameObject.Find("objetoEnMano").transform.GetChild(0).GetComponent<Objeto>(); //Obtengo el objeto del Hijo del gameobject "objetoEnMano"
            InventoryData.cantidadObjetoEnMano = GameObject.Find("objetoEnMano").transform.GetChild(1).GetComponent<TMP_Text>();

            InventoryData.objetoEnMano.transform.position = mousePosition;
            InventoryData.cantidadObjetoEnMano.transform.position = mousePosition + EsquinaDerecha;
        }
    }
}
