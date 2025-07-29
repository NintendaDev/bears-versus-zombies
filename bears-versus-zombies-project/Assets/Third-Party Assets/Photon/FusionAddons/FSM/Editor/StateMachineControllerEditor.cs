using Fusion.Editor;
using UnityEditor;
using UnityEngine;

namespace Fusion.Addons.FSM.Editor
{
	[CustomEditor(typeof(StateMachineController), true)]
	public class StateMachineControllerEditor : NetworkBehaviourEditor
	{
		// Editor INTERFACE

		public override bool RequiresConstantRepaint()
		{
			return true;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (Application.isPlaying == false)
				return;

			var controller = target as StateMachineController;

			for (int i = 0; i < controller.StateMachines.Count; i++)
			{
				var machine = controller.StateMachines[i];

				DrawStateMachine("Machine", machine, true);
				DrawChildMachines(machine);
			}
		}

		// PRIVATE METHODS

		private static void DrawStateMachine(string header, IStateMachine stateMachine, bool isActive)
		{
			EditorGUILayout.Space(10f);
			EditorGUILayout.LabelField($"{header}: {stateMachine.Name}", EditorStyles.boldLabel);

			var color = GUI.color;

			GUI.color = isActive == true ? Color.green : Color.gray;
			var activeStateName = stateMachine.ActiveState != null ? stateMachine.ActiveState.Name : "None";
			EditorGUILayout.LabelField("Active State", activeStateName);

			GUI.color = Color.gray;
			var previousStateName = stateMachine.PreviousState != null ? stateMachine.PreviousState.Name : "None";
			EditorGUILayout.LabelField("Previous State", previousStateName);

			GUI.color = color;
		}

		private static void DrawChildMachines(IStateMachine stateMachine)
		{
			EditorGUI.indentLevel++;

			for (int i = 0; i < stateMachine.States.Length; i++)
			{
				var state = stateMachine.States[i];

				for (int j = 0; j < state.ChildMachines.Length; j++)
				{
					DrawStateMachine("Child Machine", state.ChildMachines[j], stateMachine.ActiveState == state);

					DrawChildMachines(state.ChildMachines[j]);
				}
			}

			EditorGUI.indentLevel--;
		}
	}
}
