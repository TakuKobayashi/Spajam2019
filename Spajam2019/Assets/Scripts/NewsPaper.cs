﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class NewsPaper : MonoBehaviour
{
    [SerializeField] private TextMesh newsPaperText;
    [SerializeField] private Vector3 defaultScale;
    [SerializeField] private int splitByteCount;

    public void Scaleing()
    {
        this.transform.localScale = defaultScale;
        SplitText();
    }

    public void SplitText()
    {
        string text = newsPaperText.text;
        StringBuilder builder = new StringBuilder();
        int textByte = text.Length;
        Encoding utf8 = Encoding.UTF8;
        for (int i = 0;i < text.Length; ++i)
        {
            string word = text.Substring(i, 1);
            byte[] textBytes = utf8.GetBytes(word);
            textByte += textBytes.Length;
            if (textByte > splitByteCount)
            {
                builder.Append("\n");
                textByte = 0;
            }
            builder.Append(word);
        }
        newsPaperText.text = builder.ToString();
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
