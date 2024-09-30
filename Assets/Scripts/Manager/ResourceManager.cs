using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public Dictionary<string, Sprite> _sprites = new Dictionary<string, Sprite>();

    public T Load<T>(string path) where T : Object
    {
        if (typeof(T) == typeof(Sprite))
        {
            if (_sprites.TryGetValue(path, out Sprite sprite))
                return sprite as T;

            Sprite sp = Resources.Load<Sprite>(path);
            _sprites.Add(path, sp);
            return sp as T;
        }

        return Resources.Load<T>(path);
    }

    // 외부 파일 로드 처리 (bms UI, 썸네일 이미지 등 처리)
    //public T LoadExternalFile<T>(string path) where T : Object
    //{
    //    if (typeof(T) == typeof(Sprite))
    //    {
    //        if (_sprites.TryGetValue(path, out Sprite sprite))
    //            return sprite as T;

    //        Sprite sp = 
    //    }
    //}

    public GameObject Instantiate(string path, Transform parent = null)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");
        if (prefab == null)
        {
            Debug.Log($"Failed to load prefab : {path}");
            return null;
        }

        return Instantiate(prefab, parent);
    }

    public GameObject Instantiate(GameObject prefab, Transform parent = null)
    {
        GameObject go = Object.Instantiate(prefab, parent);
        go.name = prefab.name;
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        Object.Destroy(go);
    }
}
