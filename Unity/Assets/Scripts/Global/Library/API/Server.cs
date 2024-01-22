using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour
{
    private static Server _instance = null;

    public static Server Instance => _instance;

    public void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);
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

    (string, UnityWebRequest.Result) Perform(UnityWebRequest uwr)
    {
        uwr.SendWebRequest();
        while (!uwr.isDone)
        {
        }

        return (uwr.downloadHandler.text, uwr.result);
    }

    public (bool, string) HealthyCheck(string ip, int port)
    {
        using var client = UnityWebRequest.Get($"http://{ip}:{port}/healthy");
        client.timeout = 3;
        var (response, status) = Perform(client);

        if (status == UnityWebRequest.Result.Success)
        {
            var json = JsonConvert.DeserializeObject<JObject>(response);
            var ver = json["version"].Value<string>();
            Debug.Log(json);
            return (true, ver);
        }

        return (false, "");
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

        var send = JsonConvert.SerializeObject(values);
        // var content = new StringContent(send, Encoding.UTF8, "application/json");
        Debug.Log(send);
        UploadHandler raw = new UploadHandlerRaw(Encoding.UTF8.GetBytes(send));
        raw.contentType = "application/json";

        using var client = new UnityWebRequest($"http://{IP}:{Port}/login", "POST");
        client.timeout = 3;
        client.uploadHandler = raw;

        var (res, status) = Perform(client);
        if (status == UnityWebRequest.Result.Success)
        {
            var json = JsonConvert.DeserializeObject<JObject>(res);
            Token = json["token"].Value<string>();
            Debug.Log(json);
            return true;
        }

        return false;
    }

    public void Logout()
    {
        var values = new Dictionary<string, string>
        {
            { "token", Token },
        };

        var send = JsonConvert.SerializeObject(values);
        UploadHandler raw = new UploadHandlerRaw(Encoding.UTF8.GetBytes(send));
        raw.contentType = "application/json";

        using var client = new UnityWebRequest($"http://{IP}:{Port}/logout", "POST");
        client.timeout = 3;
        client.uploadHandler = raw;

        var (res, status) = Perform(client);
        var json = JsonConvert.DeserializeObject<JObject>(res);
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

        var send = JsonConvert.SerializeObject(values);

        // var content = new StringContent(send, Encoding.UTF8, "application/json");
        Debug.Log(send);
        UploadHandler raw = new UploadHandlerRaw(Encoding.UTF8.GetBytes(send));
        raw.contentType = "application/json";

        using var client = new UnityWebRequest($"http://{IP}:{Port}/alarm", "POST");
        client.timeout = 3;
        client.uploadHandler = raw;

        var (res, status) = Perform(client);
        if (status == UnityWebRequest.Result.Success)
        {
            var json = JsonConvert.DeserializeObject<List<VMSAlarm>>(res);
            Debug.Log(json);
            return json;
        }

        return null;
    }

    public List<VMSAlarm> Alarm(DateTime start, DateTime end, List<int> node)
    {
        if (start == DateTime.MinValue)
        {
            start = new DateTime(1980, 01, 01);
        }

        if (end == DateTime.MinValue)
        {
            end = DateTime.UtcNow;
        }

        var values = new Dictionary<object, object>
        {
            { "token", Token },
            { "start", start.ToString("yyyy-MM-dd HH:mm:ss") },
            { "end", end.ToString("yyyy-MM-dd HH:mm:ss") },
            { "node", node },
        };

        var send = JsonConvert.SerializeObject(values);

        // var content = new StringContent(send, Encoding.UTF8, "application/json");
        Debug.Log(send);
        UploadHandler raw = new UploadHandlerRaw(Encoding.UTF8.GetBytes(send));
        raw.contentType = "application/json";

        using var client = new UnityWebRequest($"http://{IP}:{Port}/alarm_date", "POST");
        client.timeout = 3;
        client.uploadHandler = raw;

        var (res, status) = Perform(client);

        if (status == UnityWebRequest.Result.Success)
        {
            var json = JsonConvert.DeserializeObject<List<VMSAlarm>>(res);
            Debug.Log(json);
            return json;
        }

        return null;
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

        var send = JsonConvert.SerializeObject(values);

        // var content = new StringContent(send, Encoding.UTF8, "application/json");
        Debug.Log(send);
        UploadHandler raw = new UploadHandlerRaw(Encoding.UTF8.GetBytes(send));
        raw.contentType = "application/json";

        using var client = new UnityWebRequest($"http://{IP}:{Port}/fft", "POST");
        client.timeout = 3;
        client.uploadHandler = raw;
        var (res, status) = Perform(client);
        if (status == UnityWebRequest.Result.Success)
        {
            var json = JsonConvert.DeserializeObject<VMSFFT>(res);
            Debug.Log(json);
            return json;
        }

        return null;
    }

    public VMSCharts Charts(int id, int sample_rate)
    {
        var values = new Dictionary<object, object>
        {
            { "token", Token },
            { "id", id },
            { "sample_rate", sample_rate },
        };
        var send = JsonConvert.SerializeObject(values);

        // var content = new StringContent(send, Encoding.UTF8, "application/json");
        Debug.Log(send);
        UploadHandler raw = new UploadHandlerRaw(Encoding.UTF8.GetBytes(send));
        raw.contentType = "application/json";

        using var client = new UnityWebRequest($"http://{IP}:{Port}/charts", "POST");
        client.timeout = 3;
        client.uploadHandler = raw;
        var (res, status) = Perform(client);
        if (status == UnityWebRequest.Result.Success)
        {
            var json = JsonConvert.DeserializeObject<VMSCharts>(res);
            Debug.Log(json);
            return json;
        }

        return null;
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
        var send = JsonConvert.SerializeObject(values);

        // var content = new StringContent(send, Encoding.UTF8, "application/json");
        Debug.Log(send);
        UploadHandler raw = new UploadHandlerRaw(Encoding.UTF8.GetBytes(send));
        raw.contentType = "application/json";

        using var client = new UnityWebRequest($"http://{IP}:{Port}/month", "POST");
        client.timeout = 3;
        client.uploadHandler = raw;

        var (res, status) = Perform(client);
        if (status == UnityWebRequest.Result.Success)
        {
            var json = JsonConvert.DeserializeObject<VMSMonth>(res);
            Debug.Log(json);
            return json;
        }

        return null;
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

        var send = JsonConvert.SerializeObject(values);

        // var content = new StringContent(send, Encoding.UTF8, "application/json");
        Debug.Log(send);
        UploadHandler raw = new UploadHandlerRaw(Encoding.UTF8.GetBytes(send));
        raw.contentType = "application/json";

        using var client = new UnityWebRequest($"http://{IP}:{Port}/date", "POST");
        client.timeout = 3;
        client.uploadHandler = raw;
        var (res, status) = Perform(client);
        if (status == UnityWebRequest.Result.Success)
        {
            var json = JsonConvert.DeserializeObject<VMSHour>(res);
            Debug.Log(json);
            return json;
        }

        return null;
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
            var t = temp.SelectMany(item => { return node.Where(n => n.Parent == item.NodeId); }).Distinct().ToList();

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
        var send = JsonConvert.SerializeObject(values);

        // var content = new StringContent(send, Encoding.UTF8, "application/json");
        Debug.Log(send);
        UploadHandler raw = new UploadHandlerRaw(Encoding.UTF8.GetBytes(send));
        raw.contentType = "application/json";

        using var client = new UnityWebRequest($"http://{IP}:{Port}/node", "POST");
        client.timeout = 3;
        client.uploadHandler = raw;
        var (res, status) = Perform(client);
        if (status == UnityWebRequest.Result.Success)
        {
            var json = JsonConvert.DeserializeObject<List<VMSNode>>(res);
            Debug.Log(json);
            if (nodeId == -1)
            {
                return (json);
            }
            else
            {
                return (ChildNodes(json, nodeId));
            }
        }

        return (null);
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

            // var content = new StringContent(send, Encoding.UTF8, "application/json");
            Debug.Log(send);
            UploadHandler raw = new UploadHandlerRaw(Encoding.UTF8.GetBytes(send));
            raw.contentType = "application/json";

            using var client = new UnityWebRequest($"http://{IP}:{Port}/search", "POST");
            client.timeout = 3;
            client.uploadHandler = raw;
            var (res, status) = Perform(client);
            if (status == UnityWebRequest.Result.Success)
            {
                var json = JsonConvert.DeserializeObject<List<VMSNodeData>>(res);
                Debug.Log(json);
                return (json);
            }

            return (null);
        }
        catch
        {
            return (null);
        }
    }

    public List<VMSNodeData> Search(int id, DateTime start, DateTime end)
    {
        try
        {
            var values = new Dictionary<object, object>
            {
                { "token", Token },
                { "node", id },
                { "start", start.ToString("yyyy-MM-dd HH:mm:ss") },
                { "end", end.ToString("yyyy-MM-dd HH:mm:ss") }
            };
            var send = JsonConvert.SerializeObject(values);

            // var content = new StringContent(send, Encoding.UTF8, "application/json");
            Debug.Log(send);
            UploadHandler raw = new UploadHandlerRaw(Encoding.UTF8.GetBytes(send));
            raw.contentType = "application/json";

            using var client = new UnityWebRequest($"http://{IP}:{Port}/find_search", "POST");
            client.timeout = 3;
            client.uploadHandler = raw;

            var (res, status) = Perform(client);
            if (status == UnityWebRequest.Result.Success)
            {
                var json = JsonConvert.DeserializeObject<List<VMSNodeData>>(res);
                Debug.Log(json);
                return (json);
            }

            return (null);
        }
        catch
        {
            return (null);
        }
    }
}