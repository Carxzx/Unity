using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot : MonoBehaviour
{
    const int nulo = 0;
    public Objeto objeto;

    /*void Start(){
        objeto = gameObject.transform.GetChild(0).GetComponent<Objeto>();
        ActualizarSprite(objeto);
    }*/

    public void OnClick(){
        UI_Handler UI_Handler = FindFirstObjectByType<UI_Handler>();
        if(UI_Handler.Inventory.activeSelf){
            if(objeto.id == 0){
                if(InventoryData.enMano){
                    InventoryData.enMano = false;

                    Intercambiar(objeto,InventoryData.objetoEnMano);
                }
            }else{
                if(InventoryData.enMano){
                    if(InventoryData.SameID(objeto.id,InventoryData.objetoEnMano.id)){
                        InventoryData.enMano = false;

                        objeto = Sumar(objeto,InventoryData.objetoEnMano);

                        ActualizarSprite(InventoryData.objetoEnMano);
                    }else{
                        Intercambiar(objeto,InventoryData.objetoEnMano);
                    }
                }else{
                    InventoryData.enMano = true;

                    Intercambiar(objeto,InventoryData.objetoEnMano);
                }
            }
        }else{
            HotBar HotBar = FindFirstObjectByType<HotBar>();
            string buttonName = EventSystem.current.currentSelectedGameObject.name;
            HotBar.seleccionado = int.Parse(buttonName.Replace("S", ""));
            HotBar.MoverCuadroSeleccion();
        }
    }

    void Intercambiar(Objeto o1, Objeto o2){
        TMP_Text cantidadMostrada1 = o1.gameObject.transform.parent.GetChild(1).GetComponent<TMP_Text>();
        TMP_Text cantidadMostrada2 = o2.gameObject.transform.parent.GetChild(1).GetComponent<TMP_Text>();

        int tempID = o1.id;
        int tempCantidad = o1.cantidad;
        
        o1.id = o2.id;
        o1.cantidad = o2.cantidad;
        
        o2.id = tempID;
        o2.cantidad = tempCantidad;

        cantidadMostrada1.text = o1.cantidad == 0 ? "" : o1.cantidad.ToString();
        cantidadMostrada2.text = o2.cantidad == 0 ? "" : o2.cantidad.ToString();

        ActualizarSprite(o1);
        ActualizarSprite(o2);
    }

    Objeto Sumar(Objeto o1, Objeto o2){
        TMP_Text cantidadMostrada1 = o1.gameObject.transform.parent.GetChild(1).GetComponent<TMP_Text>();

        o1.cantidad += o2.cantidad;
        cantidadMostrada1.text = o1.cantidad.ToString();
        Borrar(o2);
        return o1;
    }

    void Borrar(Objeto obj){
        TMP_Text cantidadMostrada = obj.gameObject.transform.parent.GetChild(1).GetComponent<TMP_Text>();
        obj.id = 0;
        obj.cantidad = 0;
        cantidadMostrada.text = "";
        obj.GetComponent<Image>().enabled = false;
    }

    public void ActualizarSprite(Objeto obj){
        Image img = obj.GetComponent<Image>();
        SpriteManager SpriteManager = FindFirstObjectByType<SpriteManager>();

        img.sprite = SpriteManager.vSprite[obj.id];
        img.enabled = obj.id != 0;
    }
}