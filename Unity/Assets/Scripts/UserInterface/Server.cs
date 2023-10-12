using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Server : MonoBehaviour
{
    public string IP;
    public int Port;

    public string DatabaseIP;
    public string DatabasePort;
    public string DatabaseName;
    public string DatabaseId;
    public string DatabasePw;

    private string Token = "NULL";

    private static readonly HttpClient client = new HttpClient();

    public void Login()
    {
        var values = new Dictionary<string, string>
        {
            { "id", DatabaseId },
            { "pw", DatabasePw },
            { "name", DatabaseName },
            { "ip", DatabaseIP },
        };

        var send = JsonConvert.SerializeObject(values);
        var content = new StringContent(send, Encoding.UTF8, "application/json");
        Debug.Log(send);
        var response = client.PostAsync($"http://{IP}:{Port}/login", content).Result;
        var responseString = response.Content.ReadAsStringAsync().Result;
        var json = JsonConvert.DeserializeObject<JObject>(responseString);
        Token = json["token"].Value<string>();
        Debug.Log(json);
    }

    public void Logout()
    {
        var values = new Dictionary<string, string>
        {
            { "token", Token },
        };

        var send = JsonConvert.SerializeObject(values);
        var content = new StringContent(send, Encoding.UTF8, "application/json");
        var response = client.PostAsync($"http://{DatabaseIP}:{DatabasePort}/logout", content).Result;
        Debug.Log(send);
        var responseString = response.Content.ReadAsStringAsync().Result;
        var json = JsonConvert.DeserializeObject<JObject>(responseString);
        Token = json["token"].Value<string>();
        Debug.Log(Token);
    }

    public List<VMSAlarm> Alarm(int node, int size, int offset)
    {
        var values = new Dictionary<object, object>
        {
            { "token", Token },
            { "node", node },
            { "size", size },
            { "offset", offset },
        };

        var send = JsonConvert.SerializeObject(values);
        var content = new StringContent(send, Encoding.UTF8, "application/json");
        var response = client.PostAsync($"http://{DatabaseIP}:{DatabasePort}/alarm", content).Result;
        Debug.Log(send);
        var responseString = response.Content.ReadAsStringAsync().Result;
        var json = JsonConvert.DeserializeObject<List<VMSAlarm>>(responseString);
        //Token = json["token"].Value<string>();
        Debug.Log(Token);
        return json;
    }

    public List<VMSFFT> fft(int id, int timeline, int end_freq)
    {
        var values = new Dictionary<object, object>
        {
            { "token", Token },
            { "id", id },
            { "timeline", timeline },
            { "freq", end_freq },
        };

        var send = JsonConvert.SerializeObject(values);
        var content = new StringContent(send, Encoding.UTF8, "application/json");
        var response = client.PostAsync($"http://{DatabaseIP}:{DatabasePort}/fft", content).Result;
        Debug.Log(send);
        var responseString = response.Content.ReadAsStringAsync().Result;
        var json = JsonConvert.DeserializeObject<List<VMSFFT>>(responseString);
        //Token = json["token"].Value<string>();
        Debug.Log(Token);
        return json;
    }

    public List<VMSCharts> Charts(int id, int sample_rate)
    {
        var values = new Dictionary<object, object>
        {
            { "token", Token },
            { "id", id },
            { "sample_rate", sample_rate },
        };

        var send = JsonConvert.SerializeObject(values);
        var content = new StringContent(send, Encoding.UTF8, "application/json");
        var response = client.PostAsync($"http://{DatabaseIP}:{DatabasePort}/charts", content).Result;
        Debug.Log(send);
        var responseString = response.Content.ReadAsStringAsync().Result;
        var json = JsonConvert.DeserializeObject<List<VMSCharts>>(responseString);
        Debug.Log(Token);
        return json;
    }

    public List<VMSNode> Node()
    {
        var values = new Dictionary<object, object>
        {
            { "token", Token },
        };

        var send = JsonConvert.SerializeObject(values);
        var content = new StringContent(send, Encoding.UTF8, "application/json");
        var response = client.PostAsync($"http://{DatabaseIP}:{DatabasePort}/node", content).Result;
        Debug.Log(send);
        var responseString = response.Content.ReadAsStringAsync().Result;
        var json = JsonConvert.DeserializeObject<List<VMSNode>>(responseString);
        //Token = json["token"].Value<string>();
        Debug.Log(Token);
        return json;
    }

    public List<VMSNodeData> Search(int id, int size, int offset)
    {
        var values = new Dictionary<object, object>
        {
            { "token", Token },
            { "id", id },
            { "size", size },
            { "offset", offset },
        };

        var send = JsonConvert.SerializeObject(values);
        var content = new StringContent(send, Encoding.UTF8, "application/json");
        var response = client.PostAsync($"http://{DatabaseIP}:{DatabasePort}/search", content).Result;
        Debug.Log(send);
        var responseString = response.Content.ReadAsStringAsync().Result;
        var json = JsonConvert.DeserializeObject<List<VMSNodeData>>(responseString);
        //Token = json["token"].Value<string>();
        Debug.Log(Token);
        return json;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
