using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System;

public class GridMaker : MonoBehaviour {

    public static void CreateGridFromString(string s)
    {
        // FileReader isn't very commented but I reckon the idea is to:
        // Find the length, i.e. the amount of rows
        // Set width to match the widest (longest) row, counting the amount of text units as separated by commas and semicolons
        // Then create the grid as a string array, filling in the blanks of the shorter rows with zeros

        int length = 0;
        int width = 0;
        Debug.Log("input string " + s);

        string[] stringList = s.Split('\n');
        length = stringList.Length; // This should get the length right away
        Debug.Log(stringList);
        Debug.Log("len " + length);

        if (stringList != null)
        {

            for (int l = 0; l < length; l++)
            {
                int ticker = 0;

                for (int c = 0; c < stringList[l].Length; c++)
                {
                    if (stringList[l].Substring(c, 1) == "," || stringList[l].Substring(c, 1) == ";")
                    {
                        ticker++;
                    }
                }
                if (ticker > width)
                {
                    width = ticker;
                }

            }
        }
        Debug.Log("len, wid: " + length + " " + width);
        // Initialise grid
        string[,] grid = new string[length, width];

        // Fill grid
        for (int i = 0; i < length; i++)
        {

            int ticker = 0;
            int prevStop = 0;

            for (int k = 1; k <= stringList[i].Length; k++)
            {

                if (ticker == width)
                {
                    continue;
                }

                if (stringList[i].Substring(k - 1, 1) == "," || stringList[i].Substring(k - 1, 1) == ";")
                {

                    int number;
                    bool success = Int32.TryParse((string)stringList[i].Substring(prevStop, 1), out number);
                    if (success)
                    {
                        grid[i, ticker] = number.ToString();
                    }
                    else
                    {
                        grid[i, ticker] = "0";
                    }

                    if (k - prevStop > 0)
                    {
                        grid[i, ticker] = stringList[i].Substring(prevStop, k - prevStop);
                    }
                    else
                    {
                        grid[i, ticker] = "0";
                    }

                    prevStop = k;
                    ticker++;
                }

            }
           
        }
        Debug.Log(grid);
    }
}
