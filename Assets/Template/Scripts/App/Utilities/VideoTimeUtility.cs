using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public static class VideoTimeUtility 
{
	/// <summary>
	/// Converts a given video time to MS using a long.
	/// </summary>
	public static long ConvertTimeToMs(long duration) 
	{
		double durationDouble = (double)duration;
		return (long)durationDouble;
	}

	/// <summary>
	/// Converts a given video time to MS given a percentage;
	/// </summary>
	public static long ConvertTimeToMSFromPercentage(long duration, float percentage)
	{
		double durationDouble = (double)duration;
		double gazePointTime = durationDouble * ((double)percentage);
		return (long)gazePointTime;
	}

	/// <summary>
	/// Returns a string format for a given video time
	/// </summary>
	public static string FormatTimespanToString(TimeSpan convertTimespan)
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

	/// <summary>
	/// Writes out the video time string;
	/// </summary>
	public static void WriteVideoTime(TextMeshProUGUI textMesh, TimeSpan videoTimespan)
	{
		string format = null;

		if (videoTimespan.Hours > 0) 
		{
			textMesh.text = string.Format ("{0:00}:{1:00}:{2:00}", videoTimespan.Hours, videoTimespan.Minutes, videoTimespan.Seconds);
		} 
		else 
		{
			if (videoTimespan.Minutes < 10) 
			{
				textMesh.text = string.Format("{0}:{1:00}", videoTimespan.Minutes, videoTimespan.Seconds);
			} 
			else 
			{
				textMesh.text = string.Format ("{0:00}:{1:00}", videoTimespan.Minutes, videoTimespan.Seconds);
			}
		}
	}

}
