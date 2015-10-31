using UnityEngine;
using System.Collections;
using System.Text;
using System.IO;
using System;

public class FileReader : MonoBehaviour {

	private static FileInfo sourceFile = null;
	private static StreamReader reader = null;
	private static string text = " ";

	public static string[,] readFile(string s) {

		int length = 0;
		int width = 0;
		sourceFile = new FileInfo(s);
		reader = sourceFile.OpenText ();

		while (true) {
			text = reader.ReadLine();
			if (text != null) {
				int ticker = 0;

				for (int i = 1; i <= text.Length; i++) {

					if (text.Substring(i-1, 1) == ",") {
						ticker++;
					} else if (text.Substring(i-1,1) == ";") {
						length++;
						ticker++;
					}
				}
				if (ticker > width) {

					width = ticker;

				}

			} else {
				break;
			}
		}
		string[,] grid = new string[length,width];

		reader.Close();

		sourceFile = new FileInfo(s);

		reader = sourceFile.OpenText ();

		for (int i = 0; i < length; i++) {

			text = reader.ReadLine();

			if (text != null) {

				int ticker = 0;
				int prevStop = 0;

				for (int k = 1; k <= text.Length; k++) {

					if (ticker == width) {
						continue;
					}

					if (text.Substring(k-1, 1) == "," || text.Substring(k-1, 1) == ";") {
						/*
						int number;
						bool success = Int32.TryParse((string)text.Substring(prevStop, 1), out number);
						if (success) {
							grid[i,ticker] = number;
						} else {
							grid[i,ticker] = 0;
						}
						*/
						if (k - prevStop > 0) {
							grid[i,ticker] = text.Substring(prevStop,k-prevStop);
						} else {
							grid[i,ticker] = "0";
						}

						prevStop = k;
						ticker++;
					}
				}
			}

		}

		reader.Close();

		return grid;

	}

}
