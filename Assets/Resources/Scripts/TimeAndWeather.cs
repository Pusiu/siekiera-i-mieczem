using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeAndWeather : MonoBehaviour
{
	public int currentTime = 1200;
	public float updateFrequency = 1;

	public Light sun;
	public Light moon;

	public void StartCycle()
	{
		UpdateGameTime();
	}

	public void SetTime(int time)
	{
		currentTime = time;
	}



	int sunrise = 400;
	int sunset = 2000;
	Color ambientColor = new Color(199 / 255.0f, 199 / 255.0f, 199 / 255.0f);
	public void ProcessTime()
	{
		if (currentTime - (int)(currentTime / 100) * 100 >= 60)
		{
			currentTime = ((int)(currentTime / 100) + 1) * 100;
		}

		if (currentTime >= 2400)
			currentTime = 0;

		float xrot;
		if (currentTime >= sunrise && currentTime <= sunset)
		{
			if (currentTime >= sunrise)
			{
				float progress = (float)(GetDifferenceInSeconds(currentTime, sunrise)) / GetDifferenceInSeconds(sunrise + 100, sunrise);
				sun.shadowStrength = Mathf.Lerp(0, 1, progress);
				sun.intensity = Mathf.Lerp(0, 1, progress);
				moon.shadowStrength = Mathf.Lerp(1, 0, progress);
				moon.intensity = Mathf.Lerp(0.1f, 0, progress);
				float intensity = Mathf.Lerp(0.2f, 1, progress);
				RenderSettings.ambientLight = ambientColor * intensity;
			}

			float tdiff = GetDifferenceInSeconds(currentTime, sunrise);
			float duration = TimeToSeconds(sunset) - TimeToSeconds(sunrise);
			xrot = Mathf.LerpAngle(0, 180, tdiff / duration);
		}
		else
		{
			if (currentTime >= sunset || currentTime < sunrise)
			{
				float progress = (GetDifferenceInSeconds(currentTime, sunset)) / (float)GetDifferenceInSeconds(sunset + 100, sunset);
				sun.shadowStrength = Mathf.Lerp(1, 0, progress);
				sun.intensity = Mathf.Lerp(1, 0, progress);
				moon.shadowStrength = Mathf.Lerp(0, 1, progress);
				moon.intensity = Mathf.Lerp(0, 0.1f, progress);
				float intensity = Mathf.Lerp(1, 0.2f, progress);
				RenderSettings.ambientLight = ambientColor * intensity;
			}

			int nightDuration = GetDifferenceInSeconds(sunrise, sunset);
			float passedTime = (currentTime >= sunset) ? GetDifferenceInSeconds(currentTime, sunset) : TimeToSeconds(currentTime) + GetDifferenceInSeconds(2400, sunset);

			xrot = Mathf.LerpAngle(180, 360, (passedTime / nightDuration));
		}

		sun.transform.rotation = Quaternion.Euler(new Vector3(xrot, 30, 0));
		GameUI.instance.timeDial.rectTransform.rotation = Quaternion.Euler(0, 0, xrot-90);
		//GameUI.instance.timeDial.rectTransform.rotation = Quaternion.Euler(0, 0, 90 - xrot);
		//sun.transform.rotation = Quaternion.Euler(xrot, sun.transform.rotation.eulerAngles.y, sun.transform.rotation.eulerAngles.z);


	}

	private void UpdateGameTime()
	{
		currentTime++;
		ProcessTime();
		Invoke("UpdateGameTime", 1 / updateFrequency);
	}


	int TimeToSeconds(int t)
	{
		int th = (t / 100) * 60;
		int tm = t - (t / 100) * 100;
		return (th*60)+(tm*60);
	}

	/// <summary>
	/// Returns a-b in seconds
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <returns></returns>
	int GetDifferenceInSeconds(int a, int b)
	{
		int ah = (a / 100);
		int amin = a - ah*100;
		int bh = b / 100;
		int bmin = b - bh*100;

		ah = ah * 60 * 60;
		amin = amin * 60;
		bh = bh * 60 * 60;
		bmin = bmin * 60;

		if (a > b)
			return (ah + amin) - (bh + bmin);
		else
			return (ah+ amin) - (bh + bmin) + (24*60*60);
	}
}
