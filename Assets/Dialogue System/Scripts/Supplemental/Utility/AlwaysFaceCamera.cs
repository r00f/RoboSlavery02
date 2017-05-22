using UnityEngine;

namespace PixelCrushers.DialogueSystem.Examples {

	/// <summary>
	/// Component that keeps its game object always facing the main camera.
	/// </summary>
	[AddComponentMenu("Dialogue System/Actor/Always Face Camera")]
	public class AlwaysFaceCamera : MonoBehaviour {
		
		public bool yAxisOnly = false;
        [SerializeField]
        Camera cam;
		
		private Transform myTransform = null;
		
		void Awake() {
			myTransform = transform;
		}
	
		void Update() {
			if ((myTransform != null) && (cam != null)) {
				if (yAxisOnly) {
					myTransform.LookAt(new Vector3(cam.transform.position.x, myTransform.position.y, cam.transform.position.z));
				} else {
					myTransform.LookAt(cam.transform);
				}
			}
		}
		
	}

}
