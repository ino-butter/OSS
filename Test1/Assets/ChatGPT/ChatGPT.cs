using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

public class ChatGPT : MonoBehaviour
{
    private string apiKey = "sk-XoTMKf8LmzmC2r2JaTMvD593EDnVtYGOKRbz_pcQIrT3BlbkFJF-pJENxv6otr83NfYfv9lok-1TUoD-g13VUiUCWvoA"; // Replace with an actual API key
    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    [SerializeField]
    private GameObject waitText;

    [SerializeField, TextArea(3, 5)] private string promptField;
    [SerializeField] public Texture2D textureSample;
    public void Send2DTexture()
    {
        if (textureSample == null)
            return;
        StartCoroutine(GetImageGPTResponse(textureSample, OnResponseReceived));
    }
    void OnResponseReceived(string response)
    {
        Debug.Log("ChatGPT Response: " + response);
        promptField = response;
    }
    public IEnumerator GetImageGPTResponse(Texture2D _texture, System.Action<string> callback)
    {
        byte[] imageBytes = _texture.EncodeToPNG();
        string base64Image = System.Convert.ToBase64String(imageBytes);

        // Setting OpenAI API Request Data
        waitText.gameObject.SetActive(true);
        var jsonData = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new
                {
                    role = "system",
                    content = new object[]
                    {
                        new
                        {
                            type = "text",
                            text = "무조건 단어로만 알려줘 예시로는 들어서 책이면 책 제목만, 약이면 약 이름만, 상품이면 상품명만 물건 당 한개만 표현해"
                        }
                    }
                },
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = "이 이미지에 뭐가 있어?" },
                        new
                        {
                            type = "image_url",
                            image_url = new { url = $"data:image/jpeg;base64,{base64Image}" }
                        }
                    }
                }
            },
            max_tokens = 50
        };
        //var jsonData = new
        //{
        //    model = "gpt-4o-mini",
        //    messages = new[]
        //    { 
        //        new
        //        {
        //            role = "user",
        //            content = new object[]
        //            {
        //                new { type = "text", text = "이 이미지에 대해서 설명해줘" },
        //                new
        //                {
        //                    type = "image_url",
        //                    image_url = new { url = $"data:image/jpeg;base64,{base64Image}" }
        //                }
        //            }
        //        }
        //    },
        //    max_tokens = 10
        //};

        string jsonString = JsonConvert.SerializeObject(jsonData);

        // HTTP request settings
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            var responseText = request.downloadHandler.text;
            Debug.Log("Response: " + responseText);
            // Parse the JSON response to extract the required parts
            var response = JsonConvert.DeserializeObject<OpenAIResponse>(responseText);
            callback(response.choices[0].message.content.Trim());
        }
        waitText.gameObject.SetActive(false);
    }
    public IEnumerator GetChatGPTResponse(string _prompt, System.Action<string> callback)
    {
        // Setting OpenAI API Request Data
        var jsonData = new
        {
            model = "gpt-4o-mini",
            messages = new[]
            {
                new { role = "user", content = _prompt }
            },
            max_tokens = 50
        };

        string jsonString = JsonConvert.SerializeObject(jsonData);

        // HTTP request settings
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonString);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            var responseText = request.downloadHandler.text;
            Debug.Log("Response: " + responseText);
            // Parse the JSON response to extract the required parts
            var response = JsonConvert.DeserializeObject<OpenAIResponse>(responseText);
            callback(response.choices[0].message.content.Trim());
        }
    }

    public class OpenAIResponse
    {
        public Choice[] choices { get; set; }
    }

    public class Choice
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public string role { get; set; }
        public string content { get; set; }
    }

    //
    [Serializable]
    public class ImageMessage
    {
        public string Role { get; set; }
        public Content[] Content { get; set; }
    }
    [Serializable]
    public class Content
    {
        public string Type { get; set; }
        public string Text { get; set; }  // 텍스트 콘텐츠용
        public ImageUrl ImageUrl { get; set; }  // 이미지 콘텐츠용
    }
    [Serializable]
    public class ImageUrl
    {
        public string Url { get; set; }
    }
    [Serializable]
    public class ChatRequest
    {
        public string Model { get; set; }
        public ImageMessage[] Messages { get; set; }
        public int MaxTokens { get; set; }
    }
}
