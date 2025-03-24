using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameObject objetoSlot;
    public static GameObject objetoEnMano;
    public static bool enMano;

    Vector2 mousePosition;

    void Start(){
        enMano = false;
    }

    void Update(){
        if(enMano){
            //Obtenemos la posición del ratón en la pantalla
            mousePosition = Input.mousePosition;

            objetoEnMano.transform.position = mousePosition;
        }
    }

    public void OnClick(){
        if(objetoSlot != null){
            if(enMano){
                GameObject aux = objetoEnMano;

                objetoEnMano = objetoSlot;
                objetoEnMano.transform.SetParent(GameObject.Find("objetoEnMano").transform);

                objetoSlot = aux;
                objetoSlot.transform.SetParent(gameObject.transform);
                objetoSlot.transform.localPosition = new Vector2(0f,0f);
            }else{
                enMano = true;
                objetoEnMano = objetoSlot;
                objetoEnMano.transform.SetParent(GameObject.Find("objetoEnMano").transform);

                objetoSlot = null;
            }
        }else{
            if(enMano){
                enMano = false;

                objetoSlot = objetoEnMano;
                objetoSlot.transform.SetParent(gameObject.transform);
                objetoSlot.transform.localPosition = new Vector2(0f,0f);

                objetoEnMano = null;
            }
        }
    }
}
