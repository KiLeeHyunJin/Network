using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    
    // 여기서 골드 관리 및 상점 연계 (골드쓰니까)
    public int Gold { get; set; }
    public TextMeshProUGUI goldText;

    
    private  Item item; //이 부분 p[ublic 참조 해야하나? 아닐 것 같은데 

    // int slotIndex; --> 미사용


    // 획득한 아이템 --> 일단 구매라고 생각하고 하자. 획득은 그냥 땅에 떨군거 주워먹으면 되서...
    // 캐릭터 trigger GUn 체크 --> 이름 가져와서 자기 이름똑같은 Holder의 자식 active 해주면됨. 
    
    //public Image itemImage; --> 사실 이부분은 크게 중요하지는 않은듯함. -> hud에서 스프라이트 바꿔주면되서.. 

    // private List<GameObject> slotInventory= new List<GameObject>(); --> 나중에 list 인벤토리 필요할 때 

    [SerializeField]
   // private Slot[] slots; //슬롯 배열 

    private void Start()
    {
       //slots= gameObject.GetComponentsInChildren<Slot>();

        goldText.text = $"{0}"; //시작 시에 0원으로 초기화 
    }

    public void GetCoin(int coin) //골드 획득 기능 -->text 업데이트 연계
    {
        Gold += coin; //골드 추가. 
        goldText.text = $"{Gold}";

    }
    
    public void LoseCoin(int coin) //상점 아이템 구매 등
    {
        Gold -= coin;
        if (Gold < 0) Gold = 0; //최소값 0으로 제한 
        goldText.text = $"{Gold}"; //골드텍스트 초기화 
    }


    public void AddItem(Item _item,int ID) // 매개변수로 ID 받아서 그 ID에 맞춘 자식 오브젝트 활성화 시키기. 
    {
        item = _item;

        int id = _item.itemPrefab.GetInstanceID();
        PooledObject getPoolObject = Manager.Pool.GetPool(id, Vector3.zero, Quaternion.identity);
        IKWeapon newWeapon = getPoolObject as IKWeapon;
        GetComponent<Controller>().AddWeapon(newWeapon);

        AnimationController animationController=GetComponentInParent<AnimationController>();
        
        if(animationController != null)
        {
            animationController.GetWeapon(newWeapon.weaponType);
        }

        

    }

    public void RemoveItem() //인벤토리에서 아이템을 제거해주는 함수 --> 이거 무기 버리기 함수 가져오자. 어딨더라?
    {
        // 무기 떨구기. --> 기본적으로 id로 체크해서 id가 겹치면 그 프리팹을 생성해줘야하는데 어떻게 생성하지? 



    }

}
