using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Slot : MonoBehaviour
{
    const int nulo = 0;
    public Objeto objeto;
    InventoryData InventoryData;
    UI_Handler UI_Handler;

    void Start(){
        InventoryData = FindFirstObjectByType<InventoryData>();
        UI_Handler = FindFirstObjectByType<UI_Handler>();
    }

    public void OnClick(){
        if(UI_Handler.Inventory.activeSelf){
            if(objeto.id == 0){
                if(InventoryData.enMano){
                    InventoryData.enMano = false;

                    InventoryData.Intercambiar(objeto,InventoryData.objetoEnMano);
                }
            }else{
                if(InventoryData.enMano){
                    if(InventoryData.SameItem(objeto,InventoryData.objetoEnMano)){
                        InventoryData.enMano = false;

                        objeto = InventoryData.Sumar(objeto,InventoryData.objetoEnMano);

                        //ActualizarSprite(InventoryData.objetoEnMano);
                    }else{
                        InventoryData.Intercambiar(objeto,InventoryData.objetoEnMano);
                    }
                }else{
                    InventoryData.enMano = true;

                    InventoryData.Intercambiar(objeto,InventoryData.objetoEnMano);
                }
            }
        }else{
            HotBar HotBar = FindFirstObjectByType<HotBar>();
            string buttonName = EventSystem.current.currentSelectedGameObject.name;
            HotBar.seleccionado = int.Parse(buttonName.Replace("S", ""));
            HotBar.MoverCuadroSeleccion();
        }
    }
}