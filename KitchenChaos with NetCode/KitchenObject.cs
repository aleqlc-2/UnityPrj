using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private IKitchenObjectParent kitchenObjectParent;

    private FollowTransform followTransform;

	// 접시에 KitchenObject스크립트가 부착되지않아서 followTransform을 통해 위치를 정할 수 없으므로
	// PlateKitchenObject스크립트의 Awake에서 override 및 base.Awake();하여 SetTargetTransform함수가 실행될때 접시임을 구체화한다
	protected virtual void Awake()
	{
		followTransform = GetComponent<FollowTransform>();
	}

	public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }
    
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
    {
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
	}

	// 오브젝트의 위치변화 서버가 받아서 브로드캐스팅
	[ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
	}

	// 모든 클라이언트가 이 코드를 실행(오브젝트의 위치변화 클라이언트에도 보이도록 동기화)
	[ClientRpc]
	private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
		kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
		IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

		if (this.kitchenObjectParent != null)
		{
			this.kitchenObjectParent.ClearKitchenObject();
		}

		this.kitchenObjectParent = kitchenObjectParent;

		if (kitchenObjectParent.HasKitchenObject())
		{
			Debug.LogError("IKitchenObjectParent가 KitchenObjectParent를 이미 가지고있음");
		}

		kitchenObjectParent.SetKitchenObject(this);

		followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowTransform());
	}

    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

    public void ClearKitchenObjectOnParent()
    {
		kitchenObjectParent.ClearKitchenObject();
	}

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if (this is PlateKitchenObject) // 그 오브젝트에 부착된 스크립트가 PlateKitchenObject라면(접시라면)
		{
            plateKitchenObject = this as PlateKitchenObject; // PlateKitchenObject가 KitchenObject를 상속받았으므로 as로 자료형 구체화
			return true; // 리턴하기전에 out 매개변수 할당해야함
		}
        else
        {
            plateKitchenObject = null;
			return false; // 리턴하기전에 out 매개변수 할당해야함
		}
    }

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        KitchenGameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);
	}

    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        KitchenGameMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
    }
}
