using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Name : MonoBehaviour
{
    string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
    string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
    char[] TRIM_CHARS = { '\"' };

    public List<string> studentNames;
    public List<string> teamNumber;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI teamText;

    void Start()
    {
        List<Dictionary<string, object>> data = Read("eerstejaars");

        studentNames = new List<string>();
        teamNumber = new List<string>();

        for (int i = 0; i < data.Count; i++)
        {
            studentNames.Add(data[i]["naam"].ToString());
            teamNumber.Add(data[i]["team"].ToString());
        }

        StartCoroutine(GenerateCards());
    }

    public List<Dictionary<string, object>> Read(string file)
    {
        var list = new List<Dictionary<string, object>>();
        TextAsset data = Resources.Load<TextAsset>(file);

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);

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

    IEnumerator GenerateCards()
    {
        for (int i = 0; i < studentNames.Count; i++)
        {
            nameText.text = studentNames[i];
            teamText.text = "#"+teamNumber[i];

            int width = 1000;
            int height = 550;
            int startX = 0;
            int startY = 0;
            Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);

            Rect rex = new Rect(startX, startY, width, height);
            yield return new WaitForEndOfFrame();

            tex.ReadPixels(rex, 0, 0);
            tex.Apply();

            byte[] bytes = tex.EncodeToPNG();
            Destroy(tex);

            System.IO.File.WriteAllBytes(Application.dataPath + "/huilen/NamePlate" + studentNames[i] + ".png", bytes);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
