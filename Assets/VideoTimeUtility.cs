using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoTimeUtility 
{
	/// <summary>
	/// Converts a given video time to MS given a percentage;
	/// </summary>

	public long ConvertTimeToMS(long duration, float percentage)
	{
		double durationDouble = (double)duration;
		double gazePointTime = durationDouble * ((double)percentage);
		return (long)gazePointTime;
	}

	public string FormatTimespanToString(TimeSpan convertTimespan)
	{
		string format = null;

		if (convertTimespan.Hours > 0) 
		{
			return string.Format("{0:00}:{1:00}:{2:00}", convertTimespan.Hours, convertTimespan.Minutes, convertTimespan.Seconds);
		} 
		else 
		{
			if (convertTimespan.Minutes < 10) 
			{
				return string.Format("{0}:{1:00}", convertTimespan.Minutes, convertTimespan.Seconds);
			} 
			else 
			{
				return string.Format("{0:00}:{1:00}", convertTimespan.Minutes, convertTimespan.Seconds);
			}
		}
	}

}
