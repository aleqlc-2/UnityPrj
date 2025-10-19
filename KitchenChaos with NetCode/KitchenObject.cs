using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private IKitchenObjectParent kitchenObjectParent;

    private FollowTransform followTransform;

	// ���ÿ� KitchenObject��ũ��Ʈ�� ���������ʾƼ� followTransform�� ���� ��ġ�� ���� �� �����Ƿ�
	// PlateKitchenObject��ũ��Ʈ�� Awake���� override �� base.Awake();�Ͽ� SetTargetTransform�Լ��� ����ɶ� �������� ��üȭ�Ѵ�
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

	// ������Ʈ�� ��ġ��ȭ ������ �޾Ƽ� ��ε�ĳ����
	[ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
	}

	// ��� Ŭ���̾�Ʈ�� �� �ڵ带 ����(������Ʈ�� ��ġ��ȭ Ŭ���̾�Ʈ���� ���̵��� ����ȭ)
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
			Debug.LogError("IKitchenObjectParent�� KitchenObjectParent�� �̹� ����������");
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
        if (this is PlateKitchenObject) // �� ������Ʈ�� ������ ��ũ��Ʈ�� PlateKitchenObject���(���ö��)
		{
            plateKitchenObject = this as PlateKitchenObject; // PlateKitchenObject�� KitchenObject�� ��ӹ޾����Ƿ� as�� �ڷ��� ��üȭ
			return true; // �����ϱ����� out �Ű����� �Ҵ��ؾ���
		}
        else
        {
            plateKitchenObject = null;
			return false; // �����ϱ����� out �Ű����� �Ҵ��ؾ���
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
