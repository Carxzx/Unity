using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    public bool SameItem(Objeto a, Objeto b){
        Sprite sprite1 = a.GetComponent<SpriteRenderer>() == null ? a.GetComponent<Image>().sprite : a.GetComponent<SpriteRenderer>().sprite;
        Sprite sprite2 = b.GetComponent<SpriteRenderer>() == null ? b.GetComponent<Image>().sprite : b.GetComponent<SpriteRenderer>().sprite;
        return sprite1 == sprite2;
    }

    //Copia en el primer objeto el segundo
    public void CopiarObjeto(Objeto obj, Objeto objetoACopiar){
        obj.id = objetoACopiar.id;
        obj.cantidad = objetoACopiar.cantidad;
        obj.GetComponent<Image>().sprite = objetoACopiar.GetComponent<Image>() == null ? objetoACopiar.GetComponent<SpriteRenderer>().sprite : objetoACopiar.GetComponent<Image>().sprite;
        obj.GetComponent<Image>().enabled = obj.id != 0;
        obj.transform.parent.GetChild(1).GetComponent<TMP_Text>().text = obj.cantidad == 0 ? "" : obj.cantidad.ToString();
    }

    public void Borrar(Objeto obj){
        TMP_Text cantidadMostrada = obj.gameObject.transform.parent.GetChild(1).GetComponent<TMP_Text>();
        obj.id = 0;
        obj.cantidad = 0;
        cantidadMostrada.text = "";
        obj.GetComponent<SpriteRenderer>().sprite = null;
        obj.GetComponent<Image>().enabled = false;
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

    public Objeto Sumar(Objeto o1, Objeto o2){
        TMP_Text cantidadMostrada1 = o1.gameObject.transform.parent.GetChild(1).GetComponent<TMP_Text>();

        o1.cantidad += o2.cantidad;
        cantidadMostrada1.text = o1.cantidad.ToString();
        Borrar(o2);
        return o1;
    }

    public void Intercambiar(Objeto o1, Objeto o2){
        TMP_Text cantidadMostrada1 = o1.gameObject.transform.parent.GetChild(1).GetComponent<TMP_Text>();
        TMP_Text cantidadMostrada2 = o2.gameObject.transform.parent.GetChild(1).GetComponent<TMP_Text>();

        int tempID = o1.id;
        int tempCantidad = o1.cantidad;
        Sprite tempSprite = o1.GetComponent<Image>().sprite;
        
        o1.id = o2.id;
        o1.cantidad = o2.cantidad;
        o1.GetComponent<Image>().sprite = o2.GetComponent<Image>().sprite;
        o1.GetComponent<Image>().enabled = o1.id != 0;
        
        o2.id = tempID;
        o2.cantidad = tempCantidad;
        o2.GetComponent<Image>().sprite = tempSprite;
        o2.GetComponent<Image>().enabled = o2.id != 0;

        cantidadMostrada1.text = o1.cantidad == 0 ? "" : o1.cantidad.ToString();
        cantidadMostrada2.text = o2.cantidad == 0 ? "" : o2.cantidad.ToString();

        Debug.Log("Intercambio");
    }
}
