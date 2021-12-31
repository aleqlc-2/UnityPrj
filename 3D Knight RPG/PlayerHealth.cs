using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    private Animator anim;

    private Image health_Img;

    public float health = 100f;

    private bool isShielded;
    public bool Shielded
    {
        get { return isShielded; }
        set { isShielded = value; }
    }

    void Awake()
    {
        anim = GetComponent<Animator>();

        health_Img = GameObject.Find("Health Icon").GetComponent<Image>();
    }

    public void TakeDamage(float amount)
    {
        if (!isShielded)
        {
            health -= amount;

            health_Img.fillAmount = health / 100f;

            if (health <= 0f)
            {
                anim.SetBool("Death", true);

                // Death애니메이션이 95% 이상 진행되었다면
                if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Death")
                    && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f)
                {
                    Destroy(gameObject, 2f);
                }
            }
        }
    }

    public void HealPlayer(float healAmount)
    {
        health += healAmount;

        if (health > 100) health = 100f;

        health_Img.fillAmount = health / 100f;
    }
}
