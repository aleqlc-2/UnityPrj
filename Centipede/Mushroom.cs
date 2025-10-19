using UnityEngine;

public class Mushroom : MonoBehaviour
{
    public Sprite[] states;
    private SpriteRenderer spriteRenderer;
    private int health;

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		health = states.Length;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Dart"))
		{
			Damage(1);
		}
	}

	private void Damage(int amount)
	{
		health -= amount;

		if (health > 0)
			spriteRenderer.sprite = states[states.Length - health];
		else
			Destroy(this.gameObject);
	}
}
