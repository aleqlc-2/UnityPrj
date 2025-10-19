using UnityEngine.Scripting;

namespace Quantum
{
	using Photon.Deterministic;

	[Preserve]
	public unsafe class CharacterMovableSystem : SystemMainThreadFilter<CharacterMovableSystem.Filter>, ISignalOnTriggerEnter2D, ISignalOnTriggerExit2D
	{
		public override void Update(Frame f, ref Filter filter)
		{
			var gameManager = f.GetSingleton<GameManager>();
			if (gameManager.CurrentGameState == GameState.GameOver) return;

			Log.Info("Character Movable System");

			var input = f.GetPlayerInput(filter.PlayerLink->Player);
			MovePlayer(f, filter, input);
			RotatePlayer(f, filter, input);
		}

		private static void RotatePlayer(Frame frame, Filter filter, Input* input)
		{
			var direction = input->MousePosition - filter.Transform->Position;
			filter.Transform->Rotation = FPVector2.RadiansSigned(FPVector2.Up, direction);
		}

		private static void MovePlayer(Frame f, Filter filter, Input* input)
		{
			var direction = input->Direction;
			if (direction.Magnitude > 1)
			{
				direction = direction.Normalized;
			}

			var kccSettings = f.FindAsset(filter.KCC->Settings);
			kccSettings.Move(f, filter.Entity, direction);
		}


		public struct Filter
		{
			public EntityRef Entity;
			public KCC* KCC;
			public PlayerLink* PlayerLink;
			public Transform2D* Transform;
		}

		public void OnTriggerEnter2D(Frame f, TriggerInfo2D info)
		{
			if (!f.TryGet(info.Entity, out PlayerLink playerLink)) return;
			if (!f.TryGet<Grass>(info.Other, out _)) return;

			f.Events.OnPlayerEnteredGrass(playerLink.Player);

			Log.Info("Player entered the grass");
		}

		public void OnTriggerExit2D(Frame f, ExitInfo2D info)
		{
			if (!f.TryGet(info.Entity, out PlayerLink playerLink)) return;
			if (!f.TryGet<Grass>(info.Other, out _)) return;

			f.Events.OnPlayerExitGrass(playerLink.Player);
		}
	}
}
