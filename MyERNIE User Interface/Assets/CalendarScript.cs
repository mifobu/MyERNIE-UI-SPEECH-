using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;  //needed for TMP
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Threading;


namespace Calendar
{
    public class CalendarScript : MonoBehaviour
    {
        public class Day
        {
            public int dayNum;
            public Color dayColor;
            public GameObject obj;

            public Day(int dayNum, Color dayColor, GameObject obj)
            {
                this.dayNum = dayNum;
                this.dayColor = dayColor;
                this.obj = obj;
                UpdateColor(dayColor);
                UpdateDay(dayNum);
            }

            public void UpdateColor(Color newColor)
            {
                obj.GetComponent<Image>().color = newColor;
                dayColor = newColor;
            }

            public void UpdateDay(int newDayNum)
            {
                //print("update day for " + newDayNum);
                this.dayNum = newDayNum;
                if (dayColor == Color.white || dayColor == Color.green)
                {
                    obj.GetComponentInChildren<TextMeshProUGUI>().text = (dayNum + 1).ToString();
                    //return dayNum + 1; //debugging
                }
                else
                {
                    obj.GetComponentInChildren<TextMeshProUGUI>().text = "";
                    //return 0; //debugging
                }
            }
        }//end of Day class

        public List<Day> days = new List<Day>();
        public static List<DateTime> specialDates = new List<DateTime>();
        //Day newDay; //debugging

        public Transform[] weeks; //stores CalendarWeeks

        public TextMeshProUGUI MonthAndYear; //public Text MonthAndYear;
        public static DateTime currDate = DateTime.Now;

        private void Start()
        {
            UpdateCalendar(DateTime.Now.Year, DateTime.Now.Month);
        }

        void UpdateCalendar(int year, int month)
        {
            DateTime temp = new DateTime(year, month, 1);
            currDate = temp;
            MonthAndYear.text = temp.ToString("MMMM") + " " + temp.Year.ToString();
            int startDay = GetMonthStartDay(year, month);
            int endDay = GetTotalNumberOfDays(year, month);

            days.Clear();

            //print("****startdate " + startDay + " endDay" + endDay);
            for (int w = 0; w < 6; w++)
            {
                for (int i = 0; i < 7; i++)
                {
                    Day newDay;
                    int currDay = (w * 7) + i;
                    //print("currDay " + currDay);
                    if (currDay < startDay || ((currDay - startDay) >= endDay))
                    {
                        //print("gray!!!!");
                        newDay = new Day(currDay - startDay, Color.grey, weeks[w].GetChild(i).gameObject);
                    }
                    else
                    {
                        //print("white");
                        newDay = new Day(currDay - startDay, Color.white, weeks[w].GetChild(i).gameObject);
                    }
                    days.Add(newDay);

                    // FOR DEBUGGING
                    //Debug.Log("Current Day Number: ");
                    //Debug.Log(newDay.dayNum);


                }
            }

            if (DateTime.Now.Year == year && DateTime.Now.Month == month)
            {
                days[(DateTime.Now.Day - 1) + startDay].UpdateColor(Color.yellow);
            }

            foreach (DateTime sd in specialDates)
            {
                if (sd.Month == month)
                {
                    Color lightBlue = new Color(0.4f, 0.5f, 0.9f);
                    days[(sd.Day - 1) + startDay].UpdateColor(lightBlue);
                }
            }
        }

        int GetMonthStartDay(int year, int month)
        {
            DateTime temp = new DateTime(year, month, 1);
            return (int)temp.DayOfWeek;
        }

        int GetTotalNumberOfDays(int year, int month)
        {
            return DateTime.DaysInMonth(year, month);
        }

        public void SwitchMonth(int direction)
        {
            if (direction < 0)
            {
                currDate = currDate.AddMonths(-1);
            }
            else
            {
                currDate = currDate.AddMonths(1);
            }
            UpdateCalendar(currDate.Year, currDate.Month);
        }






        // Script for Pop Up Events page
        public GameObject help;
        public TextMeshProUGUI eventDesc;
        public TextMeshProUGUI date;

        public void ToggleOn(int day)
        {
            //Debug.Log("you clicked on day " + days[day].dayNum);

            int dayNum = days[day].dayNum + 1;
            Color dayColor = days[day].dayColor;
            //Debug.Log("you clicked on color " + dayColor);

            if (dayColor != Color.grey)
            {
                help.SetActive(true);
                //Debug.Log("Color is NOT gray.");
                date.GetComponent<TextMeshProUGUI>().text = "";
                eventDesc.GetComponent<TextMeshProUGUI>().text = "";
                StartCoroutine(GetRequest("https://prescott.erau.edu/campus-life/academic-calendar", dayNum));
            }
        }

        public void ToggleOff()
        {
            help.SetActive(false);
        }

        IEnumerator GetRequest(string uri, int dayNumber)
        {
            //Debug.Log("In GetRequest with dayNumber " + dayNumber);
            UnityWebRequest uwr = UnityWebRequest.Get(uri);
            yield return uwr.SendWebRequest();

            if (uwr.isNetworkError)
            {
                Debug.Log("Error While Sending: " + uwr.error);
            }

            else
            {
                // FOR DEBUGGING
                //Debug.Log("Received: " + uwr.downloadHandler.text);
                //string testDay = newDay.dayNum.ToString();
                //int test = newDay.UpdateDay(newDay.dayNum);
                //Debug.Log("Current Day Number: ");
                //Debug.Log(test);



                //To get the month: currDate.ToString("MMMM")
                //To get the day: dayNumber

                string webpage = uwr.downloadHandler.text;
                var lines = webpage.Split('\n');
                bool found = false;
                int printedHours = 0;
                string searchDate = currDate.ToString("MMMM") + " " + dayNumber;

                date.GetComponent<TextMeshProUGUI>().text = searchDate;

                foreach (var line in lines)
                {
                    if (line.IndexOf(searchDate) > 0)
                    {
                        found = true;
                    }

                    else if (found)
                    {

                        //DateTime d = new DateTime(2023, 3, 13);
                        //Debug.Log("Year: " + currDate.ToString("yyyy"));
                        var text = line.Split('>', '<');
                        //Debug.Log("Line: " + line);
                        //Debug.Log("text[2] is " + text[2]);

                        //fixing double quote issue from HTML text
                        if (text[2].ToLower().Contains('&'))
                        {
                            //Debug.Log("This is inside the if loop.");
                            string correctText = text[2].Replace("&ldquo;", "\"").Replace("&rdquo;", "\"");
                            eventDesc.GetComponent<TextMeshProUGUI>().text = correctText;

                        }
                        else
                        {
                            eventDesc.GetComponent<TextMeshProUGUI>().text = text[2];
                        }

                        printedHours++;


                        if (printedHours >= 1)
                        {
                            break;
                        }
                    }
                }

                if (!found)
                {
                    eventDesc.GetComponent<TextMeshProUGUI>().text = "There are no events happening today.";
                }
            }
        }
    }
}