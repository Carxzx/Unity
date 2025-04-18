using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.SceneManagement;

public class HotBar : MonoBehaviour
{
    const int SlotsPerRow = 10;
    public int RowActual;
    const int MaxRows = 4;
    GameObject CuadroSeleccion;
    public int seleccionado;
    const int posBase = -180;
    const int tamCuadro = 40;
    int layerMask;

    bool bandera = false;

    InventoryData InventoryData;

    void Start(){
        RowActual = 0;
        seleccionado = 0;
        CuadroSeleccion = GameObject.Find("Selected");
        InventoryData = FindFirstObjectByType<InventoryData>();
        layerMask = LayerMask.GetMask("UI");
    }

    void OnEnable(){
        if(bandera){
            ActualizarHotBar();
        }else{
            bandera = true;
        }
        MoverHotBar();
    }

    public void ActualizarHotBar(){
        for (int i = 0; i < SlotsPerRow; i++){
            Slot slot = GameObject.Find("S" + i).GetComponent<Slot>();
            slot.objeto = slot.transform.GetChild(0).GetComponent<Objeto>();
            TMP_Text cantidad  = slot.transform.GetChild(1).GetComponent<TMP_Text>();

            Objeto objetoCopiar = InventoryData.vSlots[i+RowActual*SlotsPerRow].objeto;

            slot.objeto.id = objetoCopiar.id;
            slot.objeto.cantidad = objetoCopiar.cantidad;
            cantidad.text = slot.objeto.cantidad == 0 ? "" : slot.objeto.cantidad.ToString();

            slot.ActualizarSprite(slot.objeto);
        }
    }

    void MouseScrollWheel(float input){
        if(input > 0){
            seleccionado++;
            if(seleccionado == 10){
                seleccionado = 0;
            }
        }else{
            seleccionado--;
            if(seleccionado == -1){
                seleccionado = 9;
            }
        }
        MoverCuadroSeleccion();
    }

    public void MoverCuadroSeleccion(){
        CuadroSeleccion.transform.localPosition = new Vector2(posBase+tamCuadro*seleccionado,CuadroSeleccion.transform.localPosition.y);
    }

    void Update(){
        //Rodar Rueda del raton
        if(Input.mouseScrollDelta.y != 0){
            float input = Input.mouseScrollDelta.y;
            MouseScrollWheel(input);
        }

        //Pulsar Tabulador
        if(Input.GetKeyDown(KeyCode.Tab)){
            ManejarRow();
        }

        //Pulsar Click Izquierdo
        if(Input.GetKeyDown(KeyCode.Mouse0) && Player.CanMove){
            if(!EventSystem.current.IsPointerOverGameObject()){
                AccionObjeto();
            }
        }

        //Pulsar un numero
        PulsarNum();

        MoverHotBar();

        if(Input.GetKeyDown(KeyCode.H)){
            SceneManager.LoadScene("CarroScene");
        }
    }

    void ManejarRow(){
        RowActual++;
        if(RowActual > MaxRows-1){
            RowActual = 0;
        }
        ActualizarHotBar();
    }

    void MoverHotBar(){
        if(PlayerCamera.Chocando_abajo){
            gameObject.transform.localPosition = new Vector2(gameObject.transform.localPosition.x, 190);
        }else{
            gameObject.transform.localPosition = new Vector2(gameObject.transform.localPosition.x, -190);
        }
    }


    void AccionObjeto(){
        Objeto objeto = GameObject.Find("S" + seleccionado).transform.GetChild(0).GetComponent<Objeto>(); 
        switch(objeto.id){
            case 1: //Hacha
                objeto.Hacha();
            break;
        }
    }

    void PulsarNum(){
        for (int i = 0; i <= 9; i++){
            KeyCode alphaKey = KeyCode.Alpha0 + i;
            KeyCode keypadKey = KeyCode.Keypad0 + i;

            if (Input.GetKeyDown(alphaKey) || Input.GetKeyDown(keypadKey)){
                seleccionado = i;
                MoverCuadroSeleccion();
            }
        }
    }
}