using UnityEngine;
using TMPro;

public class InventoryData : MonoBehaviour
{
    public GameObject Inventory;

    public const int numSlots = 40;
    public Slot[] vSlots = new Slot[numSlots];

    public static Objeto objetoEnMano;
    public static TMP_Text cantidadObjetoEnMano;
    public static bool enMano;

    void Start(){
        enMano = false;
        ObtenerSlots();
    }

    public bool InventoryFull(){
        int i = 0;
        bool bandera = true;
        while(i < numSlots && bandera){
            if(vSlots[i].objeto.id == 0){
                bandera = false;
            }
            i++;
        }
        return bandera;
    }

    static public bool SameID(int id1, int id2){
        return id1 == id2;
    }

    public void ObtenerSlots(){
        Inventory.SetActive(true);
        for (int i = 0; i < numSlots; i++){
            vSlots[i] = GameObject.Find("Slot" + i).GetComponent<Slot>();
            vSlots[i].objeto = vSlots[i].transform.GetChild(0).GetComponent<Objeto>();
            TMP_Text cantidad  = vSlots[i].transform.GetChild(1).GetComponent<TMP_Text>();
            cantidad.text = vSlots[i].objeto.cantidad == 0 ? "" : vSlots[i].objeto.cantidad.ToString();
        }
        objetoEnMano = GameObject.Find("objetoEnMano").transform.GetChild(0).GetComponent<Objeto>(); //Obtengo el objeto del Hijo del gameobject "objetoEnMano"
        cantidadObjetoEnMano = GameObject.Find("objetoEnMano").transform.GetChild(1).GetComponent<TMP_Text>();
        cantidadObjetoEnMano.text = objetoEnMano.cantidad == 0 ? "" : objetoEnMano.cantidad.ToString();

        Inventory.SetActive(false);

        HotBar HotBar = FindFirstObjectByType<HotBar>();
        HotBar.ActualizarHotBar();
    }
}
