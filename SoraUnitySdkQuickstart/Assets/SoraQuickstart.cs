using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Sora;


public class SoraQuickstart : MonoBehaviour
{
    Sora sora;
    void DumpDeviceInfo(string name, Sora.DeviceInfo[] infos)
    {
        Debug.LogFormat("------------ {0} --------------", name);
        foreach (var info in infos)
        {
            Debug.LogFormat("DeviceName={0} UniqueName={1}", info.DeviceName, info.UniqueName);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DumpDeviceInfo("video capturer devices", Sora.GetVideoCapturerDevices());
        DumpDeviceInfo("audio recording devices", Sora.GetAudioRecordingDevices());
        DumpDeviceInfo("audio playout devices", Sora.GetAudioPlayoutDevices());

        // Sora の生成、イベント受信の準備はアプリ開始時に一回だけやる
        InitSora();

    }

    // Sora の生成、イベント受信の準備をする。イベント発生時は何もしない。
    void InitSora()
    {
        sora = new Sora();
        Debug.LogFormat("Sora is Created");

        sora.OnAddTrack = (trackId, connectionId) =>
        {
            Debug.LogFormat("OnAddTrack: trackId={0}, connectionId={1}", trackId, connectionId);
/*            
            var obj = GameObject.Instantiate(baseContent, Vector3.zero, Quaternion.identity);
            obj.name = string.Format("track {0}", trackId);
            obj.transform.SetParent(scrollViewContent.transform);
            obj.SetActive(true);
            var image = obj.GetComponent<UnityEngine.UI.RawImage>();
            image.texture = new Texture2D(320, 240, TextureFormat.RGBA32, false);
            tracks.Add(trackId, obj);
*/
        };
        sora.OnRemoveTrack = (trackId, connectionId) =>
        {
            Debug.LogFormat("OnRemoveTrack: trackId={0}, connectionId={1}", trackId, connectionId);
/*            
            if (tracks.ContainsKey(trackId))
            {
                GameObject.Destroy(tracks[trackId]);
                tracks.Remove(trackId);
            }
*/
        };
        sora.OnNotify = (json) =>
        {
            Debug.LogFormat("OnNotify: {0}", json);
        };
        sora.OnPush = (json) =>
        {
            Debug.LogFormat("OnPush: {0}", json);
        };
        // これは別スレッドからやってくるので注意すること
        sora.OnHandleAudio = (buf, samples, channels) =>
        {
            Debug.LogFormat("OnHandleAudio");
            /*
            lock (audioBuffer)
            {
                audioBuffer.Enqueue(buf);
                audioBufferSamples += samples;
            }
            */
        };
        sora.OnMessage = (label, data) =>
        {
            Debug.LogFormat("OnMessage: label={0} data={1}", label, System.Text.Encoding.UTF8.GetString(data));
        };
        sora.OnDisconnect = (code, message) =>
        {
            Debug.LogFormat("OnDisconnect: code={0} message={1}", code.ToString(), message);
            //DisposeSora();
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (sora == null)
        {
            //Debug.LogFormat("Sora is Null");
            return;
        }
        sora.DispatchEvents();
    }

    // OnDestroy is called when terminate app
    void OnDestroy()
    {
        if (sora == null)
        {
            return;
        }
        sora.Dispose();
        sora = null;
        Debug.Log("Sora is Disposed");
    }

    // Connect ボタンの押下
    public void OnClickConnect()
    {
        // Sora に接続をする
        var config = new Sora.Config()
        {
            SignalingUrl = "wss://sora.example.com/signaling",
            ChannelId = "sora",
            Role = Sora.Role.Recvonly,
        };
        sora.Connect(config);
        Debug.LogFormat("Sora is Connected");

    }

    public void OnClickDisconnect()
    {
        // Sora から切断をする
        sora.Disconnect();
        Debug.LogFormat("Sora is Disconnected");
    }

}
