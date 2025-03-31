using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        int tempID = o1.id;
        int tempCantidad = o1.cantidad;
        
        o1.id = o2.id;
        o1.cantidad = o2.cantidad;
        
        o2.id = tempID;
        o2.cantidad = tempCantidad;

        ActualizarSprite(o1);
        ActualizarSprite(o2);
    }

    Objeto Sumar(Objeto o1, Objeto o2){
        o1.cantidad += o2.cantidad;
        Borrar(o2);
        return o1;
    }

    void Borrar(Objeto obj){
        obj.id = 0;
        obj.cantidad = 0;
        obj.GetComponent<Image>().enabled = false;
    }

    public void ActualizarSprite(Objeto obj){
        Image img = obj.GetComponent<Image>();
        SpriteManager SpriteManager = FindFirstObjectByType<SpriteManager>();

        img.sprite = SpriteManager.vSprite[obj.id];
        img.enabled = obj.id != 0;
    }
}