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
            Objeto objeto = GameObject.Find("S" + i).transform.GetChild(0).GetComponent<Objeto>();
            Objeto objetoCopiar = InventoryData.vSlots[i+RowActual*SlotsPerRow].objeto;

            InventoryData.CopiarObjeto(objeto,objetoCopiar);
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
        GameObject prefab;
        switch(objeto.id){
            case 2: //Hacha
                prefab = Resources.Load<GameObject>("Prefabs/Herramientas/Hacha");
                Instantiate(prefab);
                break;
            case 3: //Pico
                prefab = Resources.Load<GameObject>("Prefabs/Herramientas/Pico");
                Instantiate(prefab);
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