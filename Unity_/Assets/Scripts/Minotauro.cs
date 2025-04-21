using UnityEngine;

public class Minotauro : Customer
{
    Objeto[] vPedidos;
    CompradoresData CompradoresData;
    Rigidbody2D Rb;
    Animator anim;
    public int fotograma;
    Vector3 pos_ant;

    public Sprite[] lado;
    public Sprite[] arriba;
    public Sprite[] abajo;
    bool walking;
    int direccion;

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

        direccion = 0;
        walking = false;
        fotograma = 0;
        anim = GetComponent<Animator>();
        Rb = GetComponent<Rigidbody2D>();
        pos_ant = transform.position;
    }

    protected override void Update(){
        base.Update();
        if(pos_ant == transform.position){
            walking = false;
            anim.SetBool("Walk",walking);
        }else{
            walking = true;
            anim.SetBool("Walk",walking);

            direccion = ComprobarDireccion();
        }

        AsignarSprite(direccion,fotograma);
        pos_ant = transform.position;
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

    int ComprobarDireccion(){
        if(pos_ant.y < transform.position.y){
            return 1;
        }else if(pos_ant.y > transform.position.y){
            return 2;
        }else if(pos_ant.x < transform.position.x){
            return 3;
        }else{
            return 4;
        }
    }

    void AsignarSprite(int dir, int fot){
        switch(dir){
            case 1:
                GetComponent<SpriteRenderer>().sprite = arriba[fot];
                break;
            case 2:
                GetComponent<SpriteRenderer>().sprite = abajo[fot];
                break;
            case 3:
            case 4:
                GetComponent<SpriteRenderer>().flipX = direccion == 3 ? false : true;
                GetComponent<SpriteRenderer>().sprite = lado[fot];
                break;
        }
    }
}
