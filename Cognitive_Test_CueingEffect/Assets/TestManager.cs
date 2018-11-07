using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JsonFx.Json;

using System.Linq;
using System.IO;
using UnityEditor;
using UnityEngine.UI;
using System;

public class TestManager : MonoBehaviour {

	public GameObject[] Box;
	public GameObject[] Cue;
	public GameObject BlindPanel;

	public IEnumerator TestLooper;

	private void Start()
	{
		TestLooper = TextProcess();
		StartCoroutine(TestLooper);
	}
	public int Session = 0;

	public int LastType_1, LastType_2, LastType_3;

	public ConditionType CurConditionType;

	public void SetBlind (bool active)
	{
		BlindPanel.SetActive(active);
	}
	public IEnumerator TextProcess()
	{
		SetBlind(true);
        TurnOffAll();
        TurnOffAllCue();
		Debug.Log("Start test");
		LastType_1 = 20;
		LastType_2 = 20;
		LastType_3 = 20;

		while (Session < 60)
		{
			
			//시작하는 스페이스를 기다린다 . 
			yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
			Debug.Log(Session + ": Started session");
			Session++;

			SetBlind(false);

			// 이번회의 타입?
			bool isReadyCondition = false;
			while(!isReadyCondition)
			{
				int ranIntTemp = UnityEngine.Random.Range(0, 3);

				switch (ranIntTemp)
				{
					case 0:
						if(LastType_1 != 0)
						{
							LastType_1--;
							CurConditionType = (ConditionType)ranIntTemp;
							isReadyCondition = true;
						}
						break;

                    case 1:
						if (LastType_2 != 0)
                        {
                            LastType_2--;
                            CurConditionType = (ConditionType)ranIntTemp;
							isReadyCondition = true;
                        }
                        break;

                    case 2:
						if (LastType_3 != 0)
                        {
                            LastType_3--;
                            CurConditionType = (ConditionType)ranIntTemp;
							isReadyCondition = true;
                        }
                        break;
				}
			}
			isReadyCondition = false;
			// 암막치우기 

			int answerIndex = UnityEngine.Random.Range(0, 4);


			// 사전 단서 제공하거나 말거나 세팅  . 
            switch (CurConditionType)
            {
                case ConditionType.None:

                    break;
                case ConditionType.Cueing:
					TurOnCue(Cue[answerIndex]);
                    break;
                case ConditionType.FakeCueing:
                    int fakeCueIdnex = answerIndex;

                    while (fakeCueIdnex == answerIndex)
                    {
                        fakeCueIdnex = UnityEngine.Random.Range(0, 4);
                    }
					TurOnCue(Cue[fakeCueIdnex]);
                    break;
            }
			float ranWaitTime = UnityEngine.Random.Range(3.00f, 6.00f);
			// 3초에서 10초 기다림 

			yield return new WaitForSecondsRealtime(ranWaitTime);
			// 인풋 기다리기 ! 

			TurnOn(Box[answerIndex]);
			DateTime startTime = DateTime.Now;

			yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            
			DateTime endTime = DateTime.Now;

			TimeSpan time = endTime - startTime;

			string timeStr = String.Format("{0}.{1}", time.Seconds, time.Milliseconds.ToString().PadLeft(3, '0'));

			DataTested newData = new DataTested() {  conditionType = (int)CurConditionType , indexNumber = Session, responseTime = timeStr};
			//박스 켰고 ! 기다림 

			SaveNewTestData(newData);
			// -> 인풋 받으면 데이터 저장하고 다시 암막 생성하고 모든거 초기화 하고 다시 루프 ! 
			SetBlind(true);
			TurnOffAll();
			TurnOffAllCue();
		}
		//끝났다고 알려줌 
	}
    
	public void TurnOffAllCue()
	{
		foreach (var c in Cue)
        {
			c.SetActive(false);
        }
	}
	public void TurOnCue(GameObject cue)
	{
		cue.SetActive(true);
	}
	public void TurnOffAll()
	{
		foreach(var box in Box)
		{
			box.GetComponent<Image>().color = Color.white;
		}
	}

	public  void TurnOn(GameObject box)
	{
		box.GetComponent<Image>().color = Color.yellow;
	}

    
	public void SaveNewTestData(DataTested testData)
	{
		string dataString = JsonWriter.Serialize(testData);
		Debug.Log(testData);
		string path = Application.dataPath + "/Resources/Data/TestData_" + Session +".json";
		System.IO.File.WriteAllText(path, dataString);
		AssetDatabase.ImportAsset("/Resources/Data/TestData_" + Session + ".json");
	}
    

	DataTested[] LoadTestData()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Data/TestData");
		if(textAsset == null)
		{
			return null;
		}
		string data = Resources.Load<TextAsset>("Data/TestData").ToString();
		if (data == null)
			return null;
		return JsonReader.Deserialize<DataTested[]>(data);
	}

}


public enum Step
{
	Rest , WaitForSign , SignOn , End 
}