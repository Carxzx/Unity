using UnityEngine;

public class HotBar : MonoBehaviour
{
    const int SlotsPerRow = 10;
    int RowActual;
    const int MaxRows = 4;
    GameObject CuadroSeleccion;
    public int seleccionado;
    const int posBase = -180;
    const int tamCuadro = 40;

    bool bandera = false;

    void Start(){
        RowActual = 0;
        seleccionado = 0;
        CuadroSeleccion = GameObject.Find("Selected");
    }

    void OnEnable(){
        if(bandera){
            ActualizarHotBar();
        }else{
            bandera = true;
        }
    }

    public void ActualizarHotBar(){
        InventoryData InventoryData = FindFirstObjectByType<InventoryData>();
        for (int i = 0; i < SlotsPerRow; i++){
            Slot slot = GameObject.Find("S" + i).GetComponent<Slot>();
            slot.objeto = slot.transform.GetChild(0).GetComponent<Objeto>();

            Objeto objetoCopiar = InventoryData.vSlots[i+RowActual*SlotsPerRow].objeto;

            slot.objeto.id = objetoCopiar.id;
            slot.objeto.cantidad = objetoCopiar.cantidad;

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
        if(Input.mouseScrollDelta.y != 0){
            float input = Input.mouseScrollDelta.y;
            MouseScrollWheel(input);
        }
        if(Input.GetKeyDown(KeyCode.Tab)){
            ManejarRow();
        }
    }

    void ManejarRow(){
        RowActual++;
        if(RowActual > MaxRows-1){
            RowActual = 0;
        }
        ActualizarHotBar();
    }
}
