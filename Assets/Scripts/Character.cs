using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Estructura que permite guardar un float para cada lado de la pared
struct Wall
{
    public float left, right, up, down;
}

public class Character : MonoBehaviour
{
	// Utilizamos el Rigidbody2D para mover al personaje
    Rigidbody2D body;
	
	// Guarda los datos para apegarnos y desapegarnos de las paredes
    Wall stuckOn;

	// Axis horizontal y vertical (entre 0.0 y 1.0)
    float horizontalMove, verticalMove;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        horizontalMove = 0f;
        verticalMove = 0f;
    }

    // Update is called once per frame
    void Update()
    {
		// Cada update cogemos los axis
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
		// Cada fixed update añadimos fuerza de impulso sumando
		// horizontalMove + pared izq + pared der
		// verticalMove + pared abajo + pared arriba
		
		// La idea es que añadimos negativo si es abajo o izq, y positivo si es arriba o der
		// por ej 1.0 + (-0.5) + 0 = 0.5f de fuerza aplicada, puedes cambiarlo a lo que quieras
		// te recomiendo poner variables para la velocidad y la fuerza q quieres aplicar blabla
        body.AddForce(new Vector2(horizontalMove + stuckOn.left + stuckOn.right, verticalMove + stuckOn.down + stuckOn.up), ForceMode2D.Impulse);
    }
}
