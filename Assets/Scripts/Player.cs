﻿using UnityEngine;
using System.Collections;

public class Player : Humanoid {

	//Declaraçao de variaveis
	public Transform groundCheck; // Objeto para checar colisao com o  chao
	public LayerMask whatIsGround; // Layer para indicar o que e chao
	public float jumpForce = 700f; // Força do pulo
    public Rigidbody2D body2D; // Armazena o componente Rigidbody2D do gameObject
    public BoxCollider2D boxCollider;
    public float eixoX; // Float que recebe o valor do Input.GetAxis("Horizontal") para movimentacao
    public float eixoY;
    public Lifebar lifebar;

    private bool olhandoDireita = true;
	private bool grounded = false; // Bool para indicar se o personagem esta tocando o chao
    private Animator animator; // Armazena o componente Animator do gameObject
	private float groundRadius = 0.2f; // Raio produzido pelo groundCheck
	private bool isAndando = false; // Bool para dizer se o personagem esta andando ou parado
    private bool canClimb;

//----------------------------- MÉTODOS DO SISTEMA 

	// Use this for initialization
	void Awake () {
        // Atributos
        this.HP = 10;
        this.Dano = 2;
	}

    void Start()
    {
        animator = GetComponent<Animator>();    //Recebe o componente Animator do gameObject
        body2D = GetComponent<Rigidbody2D>();   // Recebe o componente Rigidbody2D do gameObject
        boxCollider = GetComponent<BoxCollider2D>(); // Recebe o BoxCollider2D do player
        this.velocidadeMax = 5.0f;              // Velocidade maxima do personagem
    }

	// FixedUpdate tem resultados melhores para uso da fisica
	void FixedUpdate () 
    {
        Mover();
        Pulo();
        Escalar();
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Ladder")
        {
            canClimb = true;
            animator.SetBool("canClimb", true);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Ladder")
        {
            canClimb = false;
            animator.SetBool("canClimb", false);
        }
    }

//----------------------------- MÉTODOS PLAYER

    void Pulo()
    {
        if (grounded && Input.GetButtonDown("Jump")) // Se estiver tocando o chao e o botao Jump for pressionado 1 vez
        {
            animator.SetBool("ground", false);          // animator ground se torna falso
            body2D.AddForce(new Vector2(0, jumpForce)); // Adiciona força de pulo no eixo Y
        }
    }

    // Método reescrito da classe Humanoid 
    public override void Mover()
    {
        eixoX = Input.GetAxis("Horizontal");    // Recebe o valor float dos controles no eixo x
        eixoY = Input.GetAxis("Vertical");      // Recebe o valor float dos controles no eixo y

        // ground recebe true se o personagem tocar o chao ou false se nao 
        // Physics2D.OverlapCircle cria um circulo que detecta colisao com algum LayerMask
        // Parametros (Posicao, Raio, Layer)
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

        animator.SetFloat("speed", Mathf.Abs(eixoX));   // Passa o valor exato dos controles para o animator do gameObject
        animator.SetBool("ground", grounded);   // Muda o Bool ground do animator
        animator.SetFloat("vSpeed", body2D.velocity.y); // Muda o Float vSpeed do animator passando a velocidade vertical
        animator.SetBool("isCrouch", false);    // Muda o Bool isCrouch para false
        isAndando = false;  // Muda o Bool isAndando para false, indicando que o player não está andando

        // Movimentação principal
        body2D.gravityScale = 6;    // Muda a gravidade do rigidbody para 6
        body2D.velocity = new Vector2(eixoX * velocidadeMax, body2D.velocity.y);   // Move na direcao recebida do eixo x

        // Se o player estiver tocando o chão, estiver parado e pressionar a tecla S...
        if( (grounded) && (eixoX == 0) && (Input.GetKey(KeyCode.S)) ) 
        {
            Agachar();
        }
        else
        {
            boxCollider.size = new Vector2(boxCollider.size.x, 1.20f);  // Ajusta o tamanho do boxCollider para o tamanho inicial
            boxCollider.offset = new Vector2(boxCollider.offset.x, 0.3f); // Ajuda a posição do boxCollider para a posição inicial
        }

        if (eixoX != 0) // Se alguma força estiver sendo aplicada nos eixos horizontais
        {
            isAndando = true;
            velocidadeMax = 5f;
        }

        if (isAndando && Input.GetButton("Run"))    // Run = left shift
        {
            velocidadeMax = 10f;
        }

        // Se estiver movendo para a direita e nao estiver olhando para a direita
        if (eixoX > 0 && !olhandoDireita)
        {
            // Inverte a posicao
            Flip();

            // Caso contratio se estiver movendo para a esquerda e estiver olhando para a direita
        }
        else if (eixoX < 0 && olhandoDireita)
        {
            // Inverte a posicao
            Flip();

        }
    }

    // Inverte o personagem
    void Flip()
    {
        olhandoDireita = !olhandoDireita;   // Muda a direcao para qual o gameObject esta olhando
        Vector3 theScale = transform.localScale;    // Armazena a escala local do gameObject
        theScale.x *= -1;   // Inverte a escala
        transform.localScale = theScale;    // Aplica a inversao
    }

    void Agachar()
    {
        animator.SetBool("isCrouch", true);
        transform.position = new Vector2(transform.position.x, transform.position.y - 0.1f);
        boxCollider.size = new Vector2(boxCollider.size.x, 1.20f);
        boxCollider.offset = new Vector2(boxCollider.offset.x, 0.20f);
    }

    void Escalar()
    {
        if(canClimb){
            body2D.gravityScale = 0;
            body2D.velocity = new Vector2(body2D.velocity.x, eixoY * velocidadeMax);
        }

        if ((canClimb) && ((eixoY > 0) || (eixoY < 0)))
        {
            animator.SetBool("isClimbing", true);
        }
        else
        {
            animator.SetBool("isClimbing", false);
        }



    }

}
