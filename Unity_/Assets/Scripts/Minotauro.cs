using UnityEngine;

public class Minotauro : Customer
{
    Objeto[] vPedidos;
    CompradoresData CompradoresData;

    protected override void Start(){
        base.Start();

        CompradoresData = FindFirstObjectByType<CompradoresData>();

        int sprite = Random.Range(0, CompradoresData.Minotauros.Length);
        GetComponent<SpriteRenderer>().sprite = CompradoresData.Minotauros[sprite];

        //int numPedidos = Random.Range(1, 4); //1-3 (incluidos)
        int numPedidos = 1;
        vPedidos = new Objeto[numPedidos];

        for(int i = 0 ; i < numPedidos ; i++){
            Objeto objeto;

            do{
                int indice = Random.Range(0,CompradoresData.PedidosMinotauros.Length); //0-ultimo elemento vector (incluidos)
                objeto = CompradoresData.PedidosMinotauros[indice];
            }while(Repetido(objeto));

            objeto.OnGround = false;
            objeto.cantidad = Random.Range(1,6); //1-5 (includidos)

            vPedidos[i] = objeto;
        }

        imprimir_pedidos();
    }

    protected override void Update(){
        base.Update();
    }

    bool Repetido(Objeto obj){
        bool repetido = false;
        foreach (Objeto pedido in vPedidos){
            if (pedido != null && pedido.id == obj.id){
                repetido = true;
            }
        }
        return repetido;
    }

    void imprimir_pedidos(){
        foreach (Objeto pedido in vPedidos){
            Debug.Log(pedido.id);
            Debug.Log(pedido.cantidad);
        }
    }
}
