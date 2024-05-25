using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyManager;

public class ItemManager : MonoBehaviour
{
    public ItemKind itemKind;
    public enum ItemKind
    {
        Sake,
        Tsumami,
        Takoyaki,
        Ring,
        Figure
    }

    public bool Treasure;

    [Header("スプライトの種類")]
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Sprite[] itemSprite;

    [Header("回復アイテムステータス")]
    [Header("0はつまみの回復 \n 1は酒の回復 \n 2は酒のダメージ \n 3はたこやきの回復 \n")]
    [SerializeField] private int[] _healAmount;
    [SerializeField] private int[] _getScore;
    [SerializeField] private float DestroyTime;

    void Start()
    {
        if (Treasure)
        {
            System.Array values = System.Enum.GetValues(typeof(ItemKind));
            int randomIndex = Random.Range(3, 5);
            itemKind = (ItemKind)values.GetValue(randomIndex);
        } else
        {
            System.Array values = System.Enum.GetValues(typeof(ItemKind));
            int randomIndex = Random.Range(0, 3);
            itemKind = (ItemKind)values.GetValue(randomIndex);
        }




        if (itemKind == ItemKind.Sake)
        {
            renderer.sprite = itemSprite[0];
        }
        else if (itemKind == ItemKind.Tsumami)
        {
            renderer.sprite = itemSprite[1];
        } 
        else if(itemKind == ItemKind.Takoyaki)
        {
            renderer.sprite = itemSprite[2];
        }
        else if(itemKind == ItemKind.Ring)
        {
            renderer.sprite = itemSprite[3];
        }
        else if( itemKind == ItemKind.Figure)
        {
            renderer.sprite = itemSprite[4];
        }

        Invoke("Destroy", DestroyTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("アイテムがプレイヤーを認識");

            if (itemKind == ItemKind.Tsumami)
            {
                GameManager.Health += _healAmount[0];
                Debug.Log("つまみを取得");
            }
            else if (itemKind == ItemKind.Sake)
            {
                Debug.Log("酒を取得");

                int index = Random.Range(1, 101);
                if (index <= 80)
                {
                    GameManager.Health += _healAmount[1];
                }
                else
                {
                    GameManager.Health -= _healAmount[2];
                }

                GameManager.Drink += 50;
            }
            else if(itemKind == ItemKind.Takoyaki)
            {
                Debug.Log("たこやきを取得");
                GameManager.Health += _healAmount[3];
            }
            else if(itemKind == ItemKind.Ring)
            {
                Debug.Log("指輪を取得");
                GameManager.Score += _getScore[0];
            }
            else if(itemKind == ItemKind.Figure)
            {
                Debug.Log("フィギュアを取得");
                GameManager.Score += _getScore[1];
            }

            Destroy(gameObject);
        }
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }


    void Update()
    {
        
    }
}
