using UnityEngine;
using UnityEngine.Events;

public class BossHealth : MonoBehaviour
{
    public delegate void Callback();
    private Callback callback = null;

    [SerializeField] private Boss boss = null;

    private int bossHealth = 500;
    public int Bosshealth
	{
        get { return bossHealth; }
        set { bossHealth = value; }
	}

    [SerializeField] private SpriteRenderer render = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            collision.gameObject.SetActive(false);

            bossHealth -= 2;

            if (bossHealth <= 0)
			{
                render.enabled = false;
                UIManager.instance.StopHighlight();
                GameManager.Instance.SetState(GameManager.State.GAMEOVERUI);
            }
            else
			{
                 
               if (callback != null) callback.Invoke();
               //callback?.Invoke();
            }
        }
    }

    public void SetCallback(Callback cal)
	{
        callback += cal;
	}

}
