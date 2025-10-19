using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
	public static GameInput Instance { get; private set; }

	private InputSystem_Actions playerInputActions;

	// EventHandler가 델리게이트
	public event EventHandler OnInteractAction; // E누를때 Invoke할 이벤트변수 선언
	public event EventHandler OnInteractAlternateAction; // F누를때 Invoke할 이벤트변수 선언
	public event EventHandler OnPauseAction; // ESC누를때 Invoke할 이벤트변수 선언
	public event EventHandler OnBindingRebind; // 새로 바인딩할 키를 눌렀을때 Invoke할 이벤트변수 선언

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

		// 이전 게임에서 새로 바인딩한 정보가 있으면 불러온다
		if (PlayerPrefs.HasKey(PLAYER_PREFS_BINDINGS))
		{
			playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREFS_BINDINGS));
		}

		playerInputActions.Player.Enable();

		// +=에서 탭누르면 함수자동생성
		playerInputActions.Player.Interact.performed += Interact_performed; // 인풋시스템에서 Interact에 E키 누를때로 바인딩 
		playerInputActions.Player.InteractAlternate.performed += InteractAlternate_performed; // 인풋시스템에서 InteractAlternate에 F키 누를때로 바인딩
		playerInputActions.Player.Pause.performed += Pause_performed; // 인풋시스템에서 Pause에 ESC키 누를때로 바인딩

		
	}

	private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
	}

	private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
	{
		OnInteractAction?.Invoke(this, EventArgs.Empty); // OnInteractAction가 null이 아닐때만 Invoke
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

	// GameScene에서 등록했던거 다른씬으로 갈때 해제 안해주면 다시 GameScene 왔을때 에러뜸
	private void OnDestroy()
	{
		playerInputActions.Player.Interact.performed -= Interact_performed;
		playerInputActions.Player.InteractAlternate.performed -= InteractAlternate_performed;
		playerInputActions.Player.Pause.performed -= Pause_performed;

		playerInputActions.Dispose();
	}

	public string GetBindingText(Binding binding)
	{
		// 메서드가 string을 반환하므로 default포함 모든 케이스에서 return 해야함
		switch (binding)
		{
			// return이므로 break써도 의미없음
			// bindings[0]는 에디터에서 해당경로에서 바인딩한것중 첫번째거. Move의 하위카테고리 WASD부터 0번인덱스임
			// ToDisplayString은 경로빼고 바인딩된 키보드값만 뽑아서 string으로 반환(E,F,W,A,S,D,ESCAPE)
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

			// 맨 위에 default: 만 적어둬도 코드상 에러는 안뜸
			default:
				Debug.Log("해당 케이스 없음");
				return "";
		}
	}

	public void RebindBinding(Binding binding, Action onActionRebound) // Action 델리게이트가 콜백메서드를 받는다
	{
		playerInputActions.Player.Disable(); // 리바인딩할때 플레이어 움직이지 않도록

		InputAction inputAction;
		int bindingIndex;

		switch (binding)
		{
			default: // default:가 break;없이 맨앞줄에 있어야 지역변수 비할당에러 안뜸
			
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

		inputAction.PerformInteractiveRebinding(bindingIndex)//.WithControlsExcluding("<Mouse>") // 리바인딩에 마우스입력 안받기
			.OnComplete(callback => // 새로 바인딩할 키를 유저가 눌렀을때 실행
			{
				//Debug.Log(callback.action.bindings[1].path); // 원래 바인딩된 키
				//Debug.Log(callback.action.bindings[1].overridePath); // 새로 누른 바인딩 키
				callback.Dispose(); // 콜백 메모리 해제
				playerInputActions.Player.Enable();
				onActionRebound(); // 받은메서드실행

				// 새로 바인딩된 정보를 PlayerPrefs에 저장
				PlayerPrefs.SetString(PLAYER_PREFS_BINDINGS, playerInputActions.SaveBindingOverridesAsJson());
				PlayerPrefs.Save();

				OnBindingRebind?.Invoke(this, EventArgs.Empty);
			})
			.Start();
	}
}
