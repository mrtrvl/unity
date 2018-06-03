using System;
#if UNITY_EDITOR
//using UnityEditor;
#endif
using UnityEngine;


namespace UnityStandardAssets.CrossPlatformInput
{
    [ExecuteInEditMode]
    public class MobileControlRig : MonoBehaviour
#if UNITY_STANDALONE || UNITY_WEBPLAYER
//        , UnityEditor.Build.IActiveBuildTargetChanged
#endif
    {
        // this script enables or disables the child objects of a control rig
        // depending on whether the USE_MOBILE_INPUT define is declared.

        // This define is set or unset by a menu item that is included with
        // the Cross Platform Input package.


#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
        void OnEnable()
	{
		CheckEnableControlRig();
	}
#else
        public int callbackOrder
        {
            get
            {
                return 1;
            }
        }
#endif

        private void Start()
        {
#if UNITY_STANDALONE || UNITY_WEBPLAYER
            if (Application.isPlaying) //if in the editor, need to check if we are playing, as start is also called just after exiting play
#endif
            {
                UnityEngine.EventSystems.EventSystem system = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();

                if (system == null)
                {//the scene have no event system, spawn one
                    GameObject o = new GameObject("EventSystem");

                    o.AddComponent<UnityEngine.EventSystems.EventSystem>();
                    o.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
                }
            }
        }

#if UNITY_STANDALONE || UNITY_WEBPLAYER

        private void OnEnable()
        {
            //EditorApplication.update += Update;
        }


        private void OnDisable()
        {
            //EditorApplication.update -= Update;
        }


        private void Update()
        {
            CheckEnableControlRig();
        }
#endif


        private void CheckEnableControlRig()
        {
#if UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
            EnableControlRig(true);
#else
            EnableControlRig(false);
#endif
        }


        private void EnableControlRig(bool enabled)
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(enabled);
            }
        }

#if UNITY_STANDALONE || UNITY_WEBPLAYER
        //public void OnActiveBuildTargetChanged(BuildTarget previousTarget, BuildTarget newTarget)
        //{
         //   CheckEnableControlRig();
        //}
#endif
    }
}