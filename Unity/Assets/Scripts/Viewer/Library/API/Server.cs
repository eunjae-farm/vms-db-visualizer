using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class Server
{
    private static readonly HttpClient client = new();

    private static object _lock = new object();
    private static Server _instance = null;
    public static Server Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new Server();
                        Server.client.Timeout = TimeSpan.FromSeconds(3);
                    }
                }
            }

            return _instance;
        }
    }
    private string IP;
    private int Port;

    LoginObject LoginData;

    private string Token = "NULL";

    public void Setup(string ip, int port)
    {
        IP = ip;
        Port = port;
    }

    public (bool, string) HealthyCheck(string ip, int port)
    {
        try
        {
            var response = client.GetAsync($"http://{ip}:{port}/healthy").Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            var json = JsonConvert.DeserializeObject<JObject>(responseString);

            var ver = json["version"].Value<string>();
            Debug.Log(json);
            return (true, ver);
        }
        catch
        {
            return (false, "");
        }
    }


    public bool Login(LoginObject data)
    {
        LoginData = data;

        var values = new Dictionary<string, string>
        {
            { "id", LoginData.DatabaseId },
            { "pw", LoginData.DatabasePw },
            { "name", LoginData.DatabaseName },
            { "ip", LoginData.DatabaseIP },
        };
        try
        {
            var send = JsonConvert.SerializeObject(values);
            var content = new StringContent(send, Encoding.UTF8, "application/json");
            //Debug.Log(send);
            var response = client.PostAsync($"http://{IP}:{Port}/login", content).Result;
            var responseString = response.Content.ReadAsStringAsync().Result;
            var json = JsonConvert.DeserializeObject<JObject>(responseString);
            Token = json["token"].Value<string>();
            Debug.Log(json);
            return true;
        }catch
        {
            return false;
        }
    }

    public void Logout()
    {
        var values = new Dictionary<string, string>
        {
            { "token", Token },
        };

        var send = JsonConvert.SerializeObject(values);
        var content = new StringContent(send, Encoding.UTF8, "application/json");
        var response = client.PostAsync($"http://{IP}:{Port}/logout", content).Result;
        //Debug.Log(send);
        var responseString = response.Content.ReadAsStringAsync().Result;
        var json = JsonConvert.DeserializeObject<JObject>(responseString);
        //Token = json["token"].Value<string>();
        Debug.Log(json);
    }

    public List<VMSAlarm> Alarm(int size, int offset, int node = -1)
    {
        var values = new Dictionary<object, object>
        {
            { "token", Token },
            { "size", size },
            { "offset", offset },
        };
        if (node != -1)
        {
            values["node"] = node;
        }

        var send = JsonConvert.SerializeObject(values);
        var content = new StringContent(send, Encoding.UTF8, "application/json");
        var response = client.PostAsync($"http://{IP}:{Port}/alarm", content).Result;
        //Debug.Log(send);
        var responseString = response.Content.ReadAsStringAsync().Result;
        var json = JsonConvert.DeserializeObject<List<VMSAlarm>>(responseString);
        //Token = json["token"].Value<string>();
        //Debug.Log(Token);
        return json;
    }

    public VMSFFT fft(int id, int timeline, int end_freq)
    {
        var values = new Dictionary<object, object>
        {
            { "token", Token },
            { "id", id },
            { "timeline", timeline },
            { "freq", end_freq },
        };
        try
        {
            var send = JsonConvert.SerializeObject(values);
            var content = new StringContent(send, Encoding.UTF8, "application/json");
            var response = client.PostAsync($"http://{IP}:{Port}/fft", content).Result;
            //Debug.Log(send);
            var responseString = response.Content.ReadAsStringAsync().Result;
            //Debug.Log(responseString);
            var json = JsonConvert.DeserializeObject<VMSFFT>(responseString);
            //Token = json["token"].Value<string>();
            //Debug.Log(Token);
            return json;
        }catch
        {
            return null;
        }
    }

    public VMSCharts Charts(int id, int sample_rate)
    {
        var values = new Dictionary<object, object>
        {
            { "token", Token },
            { "id", id },
            { "sample_rate", sample_rate },
        };
        try
        {
            var send = JsonConvert.SerializeObject(values);
            var content = new StringContent(send, Encoding.UTF8, "application/json");
            var response = client.PostAsync($"http://{IP}:{Port}/charts", content).Result;
            //Debug.Log(send);
            var responseString = response.Content.ReadAsStringAsync().Result;
            var json = JsonConvert.DeserializeObject<VMSCharts>(responseString);
            //Debug.Log(Token);
            return json;
        }catch
        {
            return null;
        }
    }
    public List<VMSNode> Node()
    {
        var values = new Dictionary<object, object>
        {
            { "token", Token },
        };
        try
        {
            var send = JsonConvert.SerializeObject(values);
            var content = new StringContent(send, Encoding.UTF8, "application/json");
            var response = client.PostAsync($"http://{IP}:{Port}/node", content).Result;
            //Debug.Log(send);
            var responseString = response.Content.ReadAsStringAsync().Result;
            var json = JsonConvert.DeserializeObject<List<VMSNode>>(responseString);
            //Token = json["token"].Value<string>();
            //Debug.Log(Token);
            return json;
        }catch
        {
            return null;
        }
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
        var response = client.PostAsync($"http://{IP}:{Port}/search", content).Result;
        //Debug.Log(send);
        var responseString = response.Content.ReadAsStringAsync().Result;
        var json = JsonConvert.DeserializeObject<List<VMSNodeData>>(responseString);
        //Token = json["token"].Value<string>();
        //Debug.Log(Token);
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
