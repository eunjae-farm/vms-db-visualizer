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

    public List<VMSAlarm> Alarm(int size, int offset, List<int> node = null)
    {
        var values = new Dictionary<object, object>
        {
            { "token", Token },
            { "size", size },
            { "offset", offset },
        };
        if (node != null)
        {
            values["node"] = node;
        }

        try
        {
            var send = JsonConvert.SerializeObject(values);
            var content = new StringContent(send, Encoding.UTF8, "application/json");
            var response = client.PostAsync($"http://{IP}:{Port}/alarm", content).Result;
            //Debug.Log(send);
            var responseString = response.Content.ReadAsStringAsync().Result;
            var json = JsonConvert.DeserializeObject<List<VMSAlarm>>(responseString);
            //Token = json["token"].Value<string>();
            //Debug.Log(Token);
            return json;
        }catch
        {
            return null;
        }
    }
    
    public List<VMSAlarm> Alarm(DateTime start, DateTime end, List<int> node)
    {
        if (start == DateTime.MinValue)
        {
            start = new DateTime(1980, 01, 01);
        }
        
        if (end == DateTime.MinValue)
        {
            end = DateTime.Now.AddHours(9);
        }

        start = start.AddHours(-9);
        end = end.AddHours(-9);
        
        var values = new Dictionary<object, object>
        {
            { "token", Token },
            { "start", start.ToString("yyyy-MM-dd HH:mm:ss") },
            { "end", end.ToString("yyyy-MM-dd HH:mm:ss") },
            { "node", node },
        };

        try
        {
            var send = JsonConvert.SerializeObject(values);
            var content = new StringContent(send, Encoding.UTF8, "application/json");
            var response = client.PostAsync($"http://{IP}:{Port}/alarm_date", content).Result;
            //Debug.Log(send);
            var responseString = response.Content.ReadAsStringAsync().Result;
            var json = JsonConvert.DeserializeObject<List<VMSAlarm>>(responseString);
            //Token = json["token"].Value<string>();
            //Debug.Log(Token);
            return json;
        }catch
        {
            return null;
        }
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

    public VMSMonth AvailableMonthData(List<int> nodes, int year, int month, int next_year, int next_month)
    {
        var values = new Dictionary<object, object>
        {
            { "token", Token },
            { "node", nodes },
            { "year", year },
            { "month", month },
            { "ny", next_year },
            { "nm", next_month },
        };
        
        try
        {
            var send = JsonConvert.SerializeObject(values);
            var content = new StringContent(send, Encoding.UTF8, "application/json");
            var response = client.PostAsync($"http://{IP}:{Port}/month", content).Result;
            //Debug.Log(send);
            var responseString = response.Content.ReadAsStringAsync().Result;
            var json = JsonConvert.DeserializeObject<VMSMonth>(responseString);
            //Debug.Log(Token);
            return json;
        }catch
        {
            return null;
        }
    }
    
    public VMSHour AvailableHourData(List<int> nodes, int year, int month, int day)
    {
        var values = new Dictionary<object, object>
        {
            { "token", Token },
            { "node", nodes },
            { "year", year },
            { "month", month },
            { "day", day },
        };
        
        try
        {
            var send = JsonConvert.SerializeObject(values);
            var content = new StringContent(send, Encoding.UTF8, "application/json");
            var response = client.PostAsync($"http://{IP}:{Port}/date", content).Result;
            //Debug.Log(send);
            var responseString = response.Content.ReadAsStringAsync().Result;
            var json = JsonConvert.DeserializeObject<VMSHour>(responseString);
            //Debug.Log(Token);
            return json;
        }catch
        {
            return null;
        }
    }
    
    
    // i need some more idea.
    // how to implement algorithm for get a child node using node id?
    // 1. tree
    // 2. running many loop for each node.
    // number 2 seems good idea to me.
    // So,,,, I implement number 2. Trees annoy me and make me work hard.
    // Writing date : 2023. 12. 15. by J.H Cha
    private List<VMSNode> ChildNodes(List<VMSNode> node, int nodeId)
    {
        List<VMSNode> result = new List<VMSNode>();

        // find a node id
        var root = node.First(n => n.NodeId == nodeId);

        List<VMSNode> temp = new List<VMSNode>();
        temp.Add(root);

        int count = 0;
        do
        {
            count = 0;
            var t = temp.SelectMany(item =>
            {
                return node.Where(n => n.Parent == item.NodeId);
            }).Distinct().ToList();

            result.AddRange(temp);
            temp.Clear();

            t.ForEach(item =>
            {
                node.Remove(item);
                count += 1;
            });
            temp = t;
        } while (count != 0);

        return result;
    }

    public List<VMSNode> Node(int nodeId = -1)
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

            if (nodeId == -1)
            {
                return json;
            }else
            {
                return ChildNodes(json, nodeId);
            }
        }catch
        {
            return null;
        }
    }

    public List<VMSNodeData> Search(int id, int size, int offset)
    {
        try
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
        }catch
        {
            return null;
        }
    }
    
    public List<VMSNodeData> Search(int id, DateTime start, DateTime end)
    {
        try
        {
            var values = new Dictionary<object, object>
            {
                { "token", Token },
                {  "node", id } ,
                { "start", start.AddHours(-9).ToString("yyyy-MM-dd HH:mm:ss") },
                { "end", end.AddHours(-9).ToString("yyyy-MM-dd HH:mm:ss") }
            };

            var send = JsonConvert.SerializeObject(values);
            var content = new StringContent(send, Encoding.UTF8, "application/json");
            var response = client.PostAsync($"http://{IP}:{Port}/find_search", content).Result;
            //Debug.Log(send);
            var responseString = response.Content.ReadAsStringAsync().Result;
            var json = JsonConvert.DeserializeObject<List<VMSNodeData>>(responseString);
            //Token = json["token"].Value<string>();
            //Debug.Log(Token);
            return json;
        }catch
        {
            return null;
        }
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
