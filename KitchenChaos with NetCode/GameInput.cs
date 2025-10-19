using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
	public static GameInput Instance { get; private set; }

	private InputSystem_Actions playerInputActions;

	// EventHandler�� ��������Ʈ
	public event EventHandler OnInteractAction; // E������ Invoke�� �̺�Ʈ���� ����
	public event EventHandler OnInteractAlternateAction; // F������ Invoke�� �̺�Ʈ���� ����
	public event EventHandler OnPauseAction; // ESC������ Invoke�� �̺�Ʈ���� ����
	public event EventHandler OnBindingRebind; // ���� ���ε��� Ű�� �������� Invoke�� �̺�Ʈ���� ����

	private const string PLAYER_PREFS_BINDINGS = "InputBindings";

	public enum Binding
	{
		Move_Up,
		Move_Down,
		Move_Left,
		Move_Right,
		Interact,
		InteractAlternate,
		Pause
	}

	private void Awake()
	{
		Instance = this;

		playerInputActions = new InputSystem_Actions();

		// ���� ���ӿ��� ���� ���ε��� ������ ������ �ҷ��´�
		if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
		{
			playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
		}

		playerInputActions.Player.Enable();

		// +=���� �Ǵ����� �Լ��ڵ�����
		playerInputActions.Player.Interact.performed += Interact_performed; // ��ǲ�ý��ۿ��� Interact�� EŰ �������� ���ε� 
		playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed; // ��ǲ�ý��ۿ��� InteractAlternate�� FŰ �������� ���ε�
		playerInputActions.Player.Pause.performed += Pause_performed; // ��ǲ�ý��ۿ��� Pause�� ESCŰ �������� ���ε�

		
	}

	private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
	}

	private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		OnInteractAction?.Invoke(this, EventArgs.Empty); // OnInteractAction�� null�� �ƴҶ��� Invoke
	}

	private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		OnPauseAction?.Invoke(this, EventArgs.Empty);
	}

	public Vector2 GetMovementVectorNormalized()
	{
		Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
		inputVector = inputVector.normalized;

		return inputVector;
	}

	// GameScene���� ����ߴ��� �ٸ������� ���� ���� �����ָ� �ٽ� GameScene ������ ������
	private void OnDestroy()
	{
		playerInputActions.Player.Interact.performed -= Interact_performed;
		playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
		playerInputActions.Player.Pause.performed -= Pause_performed;

		playerInputActions.Dispose();
	}

	public string GetBindingText(Binding binding)
	{
		// �޼��尡 string�� ��ȯ�ϹǷ� default���� ��� ���̽����� return �ؾ���
		switch (binding)
		{
			// return�̹Ƿ� break�ᵵ �ǹ̾���
			// bindings[0]�� �����Ϳ��� �ش��ο��� ���ε��Ѱ��� ù��°��. Move�� ����ī�װ� WASD���� 0���ε�����
			// ToDisplayString�� ��λ��� ���ε��� Ű���尪�� �̾Ƽ� string���� ��ȯ(E,F,W,A,S,D,ESCAPE)
			case Binding.Move_Up:
				return playerInputActions.Player.Move.bindings[1].ToDisplayString();

			case Binding.Move_Down:
				return playerInputActions.Player.Move.bindings[2].ToDisplayString();

			case Binding.Move_Left:
				return playerInputActions.Player.Move.bindings[3].ToDisplayString();

			case Binding.Move_Right:
				return playerInputActions.Player.Move.bindings[4].ToDisplayString();

			case Binding.Interact:
				return playerInputActions.Player.Interact.bindings[0].ToDisplayString();

			case Binding.InteractAlternate:
				return playerInputActions.Player.InteractAlternate.bindings[0].ToDisplayString();

			case Binding.Pause:
				return playerInputActions.Player.Pause.bindings[0].ToDisplayString();

			// �� ���� default: �� ����ֵ� �ڵ�� ������ �ȶ�
			default:
				Debug.Log("�ش� ���̽� ����");
				return "";
		}
	}

	public void RebindBinding(Binding binding, Action onActionRebound) // Action ��������Ʈ�� �ݹ�޼��带 �޴´�
	{
		playerInputActions.Player.Disable(); // �����ε��Ҷ� �÷��̾� �������� �ʵ���

		InputAction inputAction;
		int bindingIndex;

		switch (binding)
		{
			default: // default:�� break;���� �Ǿ��ٿ� �־�� �������� ���Ҵ翡�� �ȶ�
			
			case Binding.Move_Up:
				inputAction = playerInputActions.Player.Move;
				bindingIndex = 1;
				break;

			case Binding.Move_Down:
				inputAction = playerInputActions.Player.Move;
				bindingIndex = 2;
				break;

			case Binding.Move_Left:
				inputAction = playerInputActions.Player.Move;
				bindingIndex = 3;
				break;

			case Binding.Move_Right:
				inputAction = playerInputActions.Player.Move;
				bindingIndex = 4;
				break;

			case Binding.Interact:
				inputAction = playerInputActions.Player.Interact;
				bindingIndex = 0;
				break;

			case Binding.InteractAlternate:
				inputAction = playerInputActions.Player.InteractAlternate;
				bindingIndex = 0;
				break;

			case Binding.Pause:
				inputAction = playerInputActions.Player.Pause;
				bindingIndex = 0;
				break;
		}

		inputAction.PerformInteractiveRebinding(bindingIndex)//.WithControlsExcluding("<Mouse>") // �����ε��� ���콺�Է� �ȹޱ�
			.OnComplete(callback => // ���� ���ε��� Ű�� ������ �������� ����
			{
				//Debug.Log(callback.action.bindings[1].path); // ���� ���ε��� Ű
				//Debug.Log(callback.action.bindings[1].overridePath); // ���� ���� ���ε� Ű
				callback.Dispose(); // �ݹ� �޸� ����
				playerInputActions.Player.Enable();
				onActionRebound(); // �����޼������

				// ���� ���ε��� ������ PlayerPrefs�� ����
				PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
				PlayerPrefs.Save();

				OnBindingRebind?.Invoke(this, EventArgs.Empty);
			})
			.Start();
	}
}
