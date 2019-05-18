using System;

using UnityEngine;


namespace Tochikuru
{
#pragma warning disable 649
    [Serializable]
    public class Prefab
    {
        public GameObject gameObject { get { return _gameObject; } }
        // This field is assigned from Editor.
        [SerializeField] GameObject _gameObject;

        public void SetGameObject(GameObject gameObject)
        {
            _gameObject = gameObject;
        }

        public static implicit operator UnityEngine.Object(Prefab prefab)
        {
            return prefab.gameObject;
        }

        public T InstantiateTo<T>(Transform parent) where T : Component
        {
            return Util.InstantiateTo<T>(parent.gameObject, gameObject);
        }
    }
}
