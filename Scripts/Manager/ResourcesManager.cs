using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager
{
    private Dictionary<string, BaseUI> uiDictionary = new Dictionary<string, BaseUI>();
    private Dictionary<string, BaseObject> objectDictionary = new Dictionary<string, BaseObject>();

    private BaseUI ui;
    private BaseObject obj;

    public void Clear()
    {
        uiDictionary.Clear();
        objectDictionary.Clear();
    }

    public BaseUI GetUIInDic(string uiName)
    {
        return uiDictionary[uiName];
    }

    public BaseObject GetObjectInDic(string objName)
    {
        return objectDictionary[objName];
    }

    public void AddUIInDic(UI type, string uiName)
    {
        ui = GameManager.Instance.InstantiateObject(Resources.Load<BaseUI>($"UI/{type.ToString()}/{uiName}"));
        uiDictionary.Add(uiName, ui);
    }

    public void AddObjectInDic(ObjectType type, string objName)
    {
        obj = GameManager.Instance.InstantiateObject(Resources.Load<BaseObject>($"Object/{type.ToString()}/{objName}"));
        objectDictionary.Add(objName, obj);
    }

    public bool CheckUIInDic(string uiName)
    {
        return uiDictionary.ContainsKey(uiName);
    }

    public bool CheckObjectInDic(string objName)
    {
        return objectDictionary.ContainsKey(objName);
    }
}