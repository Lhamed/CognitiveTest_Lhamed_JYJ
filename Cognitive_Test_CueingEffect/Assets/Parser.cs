using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using JsonFx.Json;
public class Parser : MonoBehaviour {

	// Use this for initialization
	void Start () {

		TextAsset[] assets= Resources.LoadAll<TextAsset>("Data");
		string jsonTemp = "";
		List<DataTested> testListTemp = new List<DataTested>();

		foreach(var asset in assets)
		{
			DataTested temp = 	JsonReader.Deserialize<DataTested>(asset.ToString());
			testListTemp.Add(temp);
		
		}
		jsonTemp = JsonWriter.Serialize(testListTemp.ToArray());
		string path = Application.dataPath + "/Resources/Data/TestDataTotal"  + ".json";
		System.IO.File.WriteAllText(path, jsonTemp);
		AssetDatabase.ImportAsset("/Resources/Data/TestDataTotal" + ".json");
		Debug.Log("Saved"); 
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
