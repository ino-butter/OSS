﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using UnityEngine;

public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r<br>";
    static char[] TRIM_CHARS = { '\"' };

    public static List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        //TextAsset data = Resources.Load (file) as TextAsset;

        string source;
        StreamReader sr = new StreamReader(@"..\..\" + file);
        source = sr.ReadToEnd();
        sr.Close();

        //var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        var lines = Regex.Split(source, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }

            list.Add(entry);
        }
        return list;
    }
    public static List<Dictionary<string, object>> Read(TextAsset file)
    {
        var list = new List<Dictionary<string, object>>();
        //TextAsset data = Resources.Load (file) as TextAsset;

        string source = file.text;

        //var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        var lines = Regex.Split(source, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
            }

            list.Add(entry);
        }
        return list;
    }
    public static Dictionary<string, Dictionary<string, object>> ReadStringIndex(string file)
    {
        var list = new Dictionary<string, Dictionary<string, object>>();
        //TextAsset data = Resources.Load (file) as TextAsset;

        string source;
        StreamReader sr = new StreamReader(@"..\..\" + file);
        source = sr.ReadToEnd();
        sr.Close();

        //var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        var lines = Regex.Split(source, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            string firstKey = null;
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
                if (j == 0)
                    firstKey = finalvalue.ToString();
            }
            list.Add(firstKey, entry);
        }
        return list;
    }
    public static Dictionary<string, Dictionary<string, object>> ReadStringIndex(TextAsset file)
    {
        var list = new Dictionary<string, Dictionary<string, object>>();
        //TextAsset data = Resources.Load (file) as TextAsset;

        string source = file.text;

        //var lines = Regex.Split(data.text, LINE_SPLIT_RE);

        var lines = Regex.Split(source, LINE_SPLIT_RE);

        if (lines.Length <= 1) return list;

        var header = Regex.Split(lines[0], SPLIT_RE);
        for (var i = 1; i < lines.Length; i++)
        {

            var values = Regex.Split(lines[i], SPLIT_RE);
            if (values.Length == 0 || values[0] == "") continue;

            var entry = new Dictionary<string, object>();
            string firstKey = null;
            for (var j = 0; j < header.Length && j < values.Length; j++)
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }
                entry[header[j]] = finalvalue;
                if (j == 0)
                    firstKey = finalvalue.ToString();
            }
            list.Add(firstKey, entry);
        }
        return list;
    }
}