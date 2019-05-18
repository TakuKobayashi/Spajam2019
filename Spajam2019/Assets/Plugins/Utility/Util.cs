using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography;

namespace Tochikuru
{
	public class Util
	{
		#region Utilities
		/// <summary>
		/// <para>初期化</para>
		/// <para>【第1引数】初期化したいTransform</para>
		/// </summary>
		public static void Normalize(Transform t)
		{
			t.localPosition = Vector3.zero;
			t.localEulerAngles = Vector3.zero;
			t.localScale = Vector3.one;
		}

		/// <summary>
		/// <para>GameObjectを生成</para>
		/// <para>【第1引数】親となるGameObject</para>
		/// <para>【第2引数】生成したいGameObject(外部から持ってきた物)</para>
		/// <para>【戻り値】生成されたGameObject</para>
		/// </summary>
		public static GameObject InstantiateTo(GameObject parent, GameObject go)
		{
			GameObject ins = (GameObject)GameObject.Instantiate(
				                 go, 
				                 Vector3.zero,
				                 Quaternion.identity
			                 );
		
			if(parent != null)
				ins.transform.SetParent(parent.transform, false);
			ins.transform.localPosition = Vector3.zero;

			//Normalize(ins.transform);
			return ins;
		}

		/// <summary>
		/// <para>GameObjectを生成</para>
		/// <para>【第1引数】親となるGameObject</para>
		/// <para>【第2引数】生成したいPrefab(外部から持ってきた物)</para>
		/// <para>【戻り値】生成されたGameObject</para>
		/// </summary>
		public static GameObject InstantiateTo(GameObject parent, Prefab prefab)
		{
			return InstantiateTo(parent, prefab.gameObject);
		}

		/// <summary>
		/// <para>GameObjectを生成し、そのGameObjectに張り付いている、Componentクラスを取得する</para>
		/// <para>【第1引数】親となるGameObject</para>
		/// <para>【第2引数】生成したいGameObject(外部から持ってきた物)</para>
		/// <para>【戻り値】生成されたGameObjectに張り付いてる指定したComponentクラス</para>
		/// </summary>
		public static T InstantiateTo<T>(GameObject parent, GameObject go)
			where T : Component
		{
			return InstantiateTo(parent, go).GetComponent<T>();
		}

		/// <summary>
		/// <para>GameObjectを生成し、そのGameObjectに張り付いている、Componentクラスを取得する</para>
		/// <para>【第1引数】親となるGameObject</para>
		/// <para>【第2引数】生成したいPrefab(外部から持ってきた物)</para>
		/// <para>【戻り値】生成されたGameObjectに張り付いてる指定したComponentクラス</para>
		/// </summary>
		public static T InstantiateTo<T>(GameObject parent, Prefab prefab)
			where T : Component
		{
			return InstantiateTo(parent, prefab.gameObject).GetComponent<T>();
		}
			
		/// <summary>
		/// <para> Editor上では、iosとAndroidのshaderが表示できないので、shaderをreattachする</para>
		/// </summary>
		public static void ReloadShaderForEditor(GameObject modelObject)
		{
			foreach(Transform charaObject in modelObject.gameObject.GetComponentsInChildren<Transform>())
			{
				if(charaObject.GetComponent<SkinnedMeshRenderer>()!=null)
				{
					foreach(Material mat in charaObject.GetComponent<SkinnedMeshRenderer>().materials)
					{
						mat.shader = Shader.Find(mat.shader.name);
					}
				}
			}
		}

		/// <summary>
		/// <para>Transform以下に紐付いている子のGameObjectを全て削除する</para>
		/// <para>【第1引数】削除したいTransform</para>
		/// </summary>
		public static void DeleteAllChildren(Transform parent)
		{
			List<Transform> transformList = new List<Transform>();
			foreach(Transform child in parent)
				transformList.Add(child);
			parent.DetachChildren();
			foreach(Transform child in transformList)
				GameObject.Destroy(child.gameObject);
		}

		/// <summary>
		/// <para>指定した秒数だけ遅らせて処理を実行させる</para>
		/// <para>【第1引数】遅らせたい秒数</para>
		/// <para>【第2引数】遅らせた後に実行する処理</para>
		/// </summary>
		public static IEnumerator DelayedAction(float delaySecond, Action action)
		{
			yield return new WaitForSeconds(delaySecond);
			action();
		}
		#endregion

		#region Animation
		public static AnimationClip LoadAnimationClip(string path)
		{
			AnimationClip clip = Resources.Load<AnimationClip>(path);
			if(clip != null)
			{
				return AnimationClip.Instantiate<AnimationClip>(clip);
			}
			return null;
		}

		/// <summary>
		/// <para>Animatorについている、該当のAnimationの実行時間を取得する</para>
		/// <para>【第1引数】該当のAnimator</para>
		/// <para>【第2引数】AnimationClipの名前</para>
		/// <para>【戻り値】時間(秒)</para>
		/// </summary>
		public static float LoadAnimationSecond(Animator animator, string name)
		{
			RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;
			AnimationClip[] clips = runtimeAnimatorController.animationClips;
			// 全てのレイヤーを取り出す
			for(int i = 0; i < clips.Length; ++i)
			{
				if(clips[i].name == name)
				{
					return clips[i].length;
				}
			}
			return 0f;
		}

		public static AnimationClip PullAnimationClip(Animator animator, string name)
		{
			RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;
			AnimationClip[] clips = runtimeAnimatorController.animationClips;
			// 全てのレイヤーを取り出す
			for(int i = 0; i < clips.Length; ++i)
			{
				if(clips[i].name == name)
				{
					return clips[i];
				}
			}
			return null;
		}

		/// <summary>
		/// controllerのコピーとAnimationClipの挿入.
		/// </summary>
		/// <param name="ani"> 対象Animator.</param>
		/// <param name="controller"> 対象に紐づくRuntimeAnimatorController.</param>
		/// <param name="clipList"> key = 変更対象のClip名 , value = 入れ替えたいClip.</param>
		public static void OverrideAnimatorController (Animator ani, RuntimeAnimatorController controller, Dictionary<string, AnimationClip> clipList) {
			// アニメーションクリップの上書き.
			ani.runtimeAnimatorController = UnityEngine.Object.Instantiate (controller) as RuntimeAnimatorController;
			AnimationClip[] clips = ani.runtimeAnimatorController.animationClips;
			AnimatorOverrideController overrideAnimetorController = new AnimatorOverrideController ();
			overrideAnimetorController.runtimeAnimatorController = ani.runtimeAnimatorController;
			// ClipPairに代入し、差し替え.
			AnimationClipPair[] clipPairs = overrideAnimetorController.clips;
			for (int i = 0; i < clips.Length; ++i) {
				foreach(KeyValuePair<string, AnimationClip> pair in clipList){
					if (clips [i].name == pair.Key) {
						clipPairs [i].overrideClip = pair.Value;
						break;
					}
				}
			}
			overrideAnimetorController.clips = clipPairs;
			// 差し替えたOverrideControllerをAnimatorに代入して完了.
			ani.runtimeAnimatorController = overrideAnimetorController;
		}
		#endregion

		#region Animator
		/// <summary>
		/// Animatorに指定した名称のParameterが存在するか
		/// </summary>
		/// <param name="put_error">エラーをコンソールに出力するか</param>
		public static bool HasParameter(Animator _anim, string _param, bool put_error = true)
		{
			bool found = false;
			for (int i = 0; i < _anim.parameterCount; i++) {
				if (_anim.GetParameter (i).name == _param) {
					found = true;
					break;
				}
			}

			if (put_error && !found) {
				Debug.LogError ("The animator has not the parameter. : " + _anim.gameObject.name + " - " + _param);
			}
				
			return found;
		}

		/// <summary>
		/// Animatorに指定した名称のStateが存在するか
		/// </summary>
		/// <param name="put_error">エラーをコンソールに出力するか</param>
		public static bool HasState(Animator _anim, string _state, bool put_error = true)
		{
			bool found = false;
			for (int i = 0; i < _anim.layerCount; i++) {
				found |= _anim.HasState (i, Animator.StringToHash (_state));
			}

			if (put_error && !found) {
				Debug.LogError ("The animator has not the parameter. : " + _anim.gameObject.name + " - " + _state);
			}

			return found;
		}

		public static Color ToColor(string colorStr)
		{
			Color color = Color.white;
			if (ColorUtility.TryParseHtmlString (colorStr,out color)) {
				return color;
			}else{
				switch (colorStr) {
				case "white": color = Color.white; break;
				case "black": color = Color.black; break;
				case "gray": case "grey": color = Color.gray; break;
				case "red": color = Color.red; break;
				case "blue": color = Color.blue; break;
				case "yellow": color = Color.yellow; break;
				case "green": color = Color.green; break;
				default: Debug.Log("Cannot Find Color : " + colorStr);break;
				}
				return color;
			}
		}

        public static TimeSpan ExecuteBenchmark(Action process){
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            process();
            sw.Stop();
            TimeSpan ts = sw.Elapsed;
            Debug.Log(string.Format("Execute: {0} ms", ts.TotalMilliseconds));
            return ts;
        }

        public static IEnumerator ExecuteBenchmarkCoroutine(Func<IEnumerator> process)
		{
			System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			yield return process();
			sw.Stop();
			TimeSpan ts = sw.Elapsed;
            Debug.Log(string.Format("Execute: {0} ms, Before Frame: {1} ms", ts.TotalMilliseconds, ts.TotalMilliseconds - (Time.deltaTime * 1000)));
			yield return ts;
		}
		#endregion
	}
}