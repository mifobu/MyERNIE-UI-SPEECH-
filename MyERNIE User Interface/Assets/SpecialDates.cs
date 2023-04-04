using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;  //needed for TMP
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Calendar;
using System.Text.RegularExpressions;

public class SpecialDates : MonoBehaviour
{
    //public DateTime currDate = Calendar.CalendarScript.currDate;
    enum month
    {
        January, February, March, April, May, June, July, August, September, October, November, December
    }


    void Start()
    {
        StartCoroutine(GetRequest("https://prescott.erau.edu/campus-life/academic-calendar"));

        //This is how you add a date to the specialDates list
        //DateTime d = new DateTime(2023, 4, 13);
        //Calendar.CalendarScript.specialDates.Add(d);
    }


    IEnumerator GetRequest(string uri)
    {
        Debug.Log("This is inside the function");
        //Debug.Log("Search: " + currDate.ToString("MMMM"));
        UnityWebRequest uwr = UnityWebRequest.Get(uri);
        yield return uwr.SendWebRequest();

        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }

        else
        {
            string webpage = uwr.downloadHandler.text;
            var lines = webpage.Split('\n');
            bool found = false;
            int datesToStore = 0;
            string year = string.Empty;
            //string search = currDate.ToString("MMMM");



            foreach (var line in lines)
            {
                if (line.Contains("Semester"))
                {
                    string yearLine = line.Replace("h2", "  ");
                    Debug.Log("Year Line: " + line);
                    for (int i = 0; i < yearLine.Length; i++)
                    {
                        if (Char.IsDigit(yearLine[i]))
                        {
                            year += yearLine[i];
                        }
                    }
                    Debug.Log("Year: " + year);
                }

                foreach (string mon in Enum.GetNames(typeof(month)))
                {
                    var dateText = line.Split('>', '<');
                    foreach (var date in dateText)
                    {
                        if (date.Contains(mon))
                        {
                            for (int day = 31; day > 0; day--)
                            {
                                if (date.Contains(day.ToString()))
                                {
                                    //Debug.Log("Date: " + mon + " " + day);
                                    //Debug.Log("Date Line: " + date);

                                    //This is the one I was working with
                                    Debug.Log("Month: " + mon + "   Day: " + day);
                                    int yearInt = Int32.Parse(year);
                                    int month = mon.GetHashCode();
                                    DateTime d = new DateTime(2023, 4, 29);
                                    Calendar.CalendarScript.specialDates.Add(d);


                                    /*
                                    if ((date.Contains("-")))
                                    {
                                        var dateLineDash = date.Split('-', ' ');
                                        foreach (var temp in dateLineDash)
                                        {
                                            //Debug.Log("            DateLine: " + temp);
                                            if (temp.Contains(day.ToString()))
                                            {
                                                Debug.Log("Month: " + mon + "   Day: " + day);
                                                datesToStore++;
                                                break;
                                            }
                                        }

                                        if (datesToStore > 1)
                                        {
                                            break;
                                        }
                                    }
                                    else if ((date.Contains("&")))
                                    {
                                        var dateLineDash = date.Split('&', ' ');
                                        foreach (var temp in dateLineDash)
                                        {
                                            //Debug.Log("            DateLine: " + temp);
                                            if (temp.Contains(day.ToString()))
                                            {
                                                Debug.Log("Month: " + mon + "   Day: " + day);
                                                datesToStore++;
                                                break;
                                            }
                                        }

                                        if (datesToStore > 4)
                                        {
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }*/
                                    break;
                                }
                            }
                                //Debug.Log("Line: " + line);
                            
                        }
                    }
                }

            }
        }
    }
}
