using UnityEngine;
using System.Collections;
using NCMB;
using System.Collections.Generic;

public class NCMBPushSample : MonoBehaviour
{
	//[SerializeField]
	//string m_AndroidPushAction = "com.teapea.testiab.RECEIVE_PUSH";//action
	[SerializeField]
	GUIText m_GuiText;
	//ログ表示
	//ログ
	string m_Log = string.Empty;
	int logCount = 0;
	public static readonly string LINE = "----------------------------------------\n";
	//ダイアログ
	Vector2 scrollPosition = Vector2.zero;
	//フォント
	GUIStyle titleStyle;
	GUIStyle targetStyle;
	GUIStyle bottonStyle;
	int fontSize;

	/// <summary>
	///初回起動時にフォント設定を行う
	/// </summary>
	void Start ()
	{
		//フォント設定
		FontResize (m_GuiText.pixelOffset, m_GuiText.fontSize);
	}

	/// <summary>
	///スタイルを設定する
	/// </summary>
	void StyleSettings ()
	{
		
		//タイトル
		titleStyle = new GUIStyle ();
		titleStyle.fontSize = fontSize + 20;
		titleStyle.normal.textColor = Color.red;
		titleStyle.alignment = TextAnchor.MiddleCenter;
		
		//OS
		targetStyle = new GUIStyle ();
		targetStyle.fontSize = fontSize + 10;
		targetStyle.normal.textColor = Color.green;
		targetStyle.alignment = TextAnchor.MiddleCenter;
		
		//ボタン文字
		bottonStyle = new GUIStyle (GUI.skin.button);
		bottonStyle.fontSize = fontSize - 5;
	}

	/// <summary>
	///端末に合わせてリサイズする
	/// </summary>
	/// <param name="pixel">座標</param>
	/// <param name="defaultFontSize">初期フォントサイズ</param>
	void FontResize (Vector2 pixel, float defaultFontSize)
	{
		//端末のスクリーンサイズ取得
		float screenWidth = Screen.width;
		float screenHeigt = Screen.height;
		
		//縦、横対応
		float wdRatio = 100.0f / (640.0f / screenWidth);
		float heRatio = 100.0f / (1136.0f / screenHeigt);
		float ratio;
		if (screenWidth < screenHeigt) {
			ratio = wdRatio;
		} else {
			ratio = heRatio;
		}
		
		//フォント設定
		fontSize = (int)(defaultFontSize * (ratio / 100));
		float rePixOffsetX = pixel.x * (ratio / 100);
		float rePixOffsetY = pixel.y * (ratio / 100);
		m_GuiText.fontSize = fontSize;
		Vector2 position = new Vector2 (rePixOffsetX, rePixOffsetY);
		m_GuiText.pixelOffset = position;
	}

	/// <summary>
	///イベントリスナーの登録
	/// </summary>
	void OnEnable ()
	{
		NCMBManager.onSendPush += OnSendPush;
		NCMBManager.onRegistration += OnRegistration;
		NCMBManager.onNotificationReceived += OnNotificationReceived;
	}

	/// <summary>
	///イベントリスナーの削除
	/// </summary>
	void OnDisable ()
	{
		NCMBManager.onSendPush -= OnSendPush;
		NCMBManager.onRegistration -= OnRegistration;
		NCMBManager.onNotificationReceived -= OnNotificationReceived;
	}

	/// <summary>
	///端末登録後
	/// </summary>
	void OnRegistration (string errorMessage)
	{
		if (errorMessage == null) {
			Log ("OnRegistrationSucceeded");
		} else {
			Log ("OnRegistrationFailed", errorMessage);
		}
		RemoveLog ();
	}

	/// <summary>
	///メッセージ受信
	/// </summary>
	void OnNotificationReceived (NCMBPushPayload payload)
	{
		Log (LINE + "OnNotificationReceived");
		
		// Common iOS and Android:
		Log ("PushId", payload.PushId);
		Log ("Message", payload.Message);
		//Log("RichUrl", payload.RichUrl);
		
		#if UNITY_ANDROID || UNITY_EDITOR
		Log ("Title", payload.Title);
		//Log("Data", payload.Data);
		//Log("Channel", payload.Channel);
		//Log("Dialog", payload.Dialog);
		#elif UNITY_IOS
		//if (payload.UserInfo != null)
		//{
		//	if (payload.UserInfo.Contains("user"))
		//	{
		//		Log("User", payload.UserInfo["user"].ToString());
		//	}
		//}
		#endif
		
		RemoveLog ();
	}

	/// <summary>
	///プッシュ送信後
	/// </summary>
	void OnSendPush (string errorMessage)
	{
		if (errorMessage == null) {
			Log (LINE + "OnSendPushSucceeded");
		} else {
			Log (LINE + "OnSendPushFailed", errorMessage);
		}
		RemoveLog ();
	}

	/// <summary>
	/// 位置情報成功
	/// </summary>
	/// <param name="geo">位置情報</param>
	void OnGetLocationSucceeded (NCMBGeoPoint geo)
	{
		Log (LINE + "OnGetLocationSucceeded");
		Log ("Latitude", geo.Latitude);
		Log ("Longitude", geo.Longitude);
		RemoveLog ();
	}

	/// <summary>
	/// 位置情報失敗
	/// </summary>
	/// <param name="errorMessage">エラーメッセージ</param>
	void OnGetLocationFailed (string errorMessage)
	{
		Log (LINE + "OnGetLocationFailed", errorMessage);
		RemoveLog ();
	}

	/// <summary>
	/// OS確認
	/// </summary>
	void CheckEditor ()
	{
		#if UNITY_EDITOR
		Log (LINE + "Error", "Please running from the terminal.");
		RemoveLog ();
		#endif

	}

	/// <summary>
	/// GUIを表示する
	/// </summary>
	void OnGUI ()
	{
		
		//スタイル設定
		StyleSettings ();
		GUILayout.Label ("PushSample", titleStyle);
		scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.Width (Screen.width - 5), GUILayout.Height (Screen.height / 2));
		GUILayout.BeginVertical ("box");
		
		/* iOS */
		GUILayout.Space (20);
		GUILayout.Label ("- iOS - ", targetStyle);
		if (GUILayout.Button ("\nNo.1 : プッシュ送信\n", bottonStyle)) {
			this.CheckEditor ();
			NCMBPush push = new NCMBPush () {
				Message = "No.1 テストメッセージiOS",
				PushToIOS = true,
				DelayByMilliseconds = 0,
			};
			push.SendPush ();
		}
		if (GUILayout.Button ("\nNo.2 : プッシュ送信(1分後)\n", bottonStyle)) {
			this.CheckEditor ();
			NCMBPush push = new NCMBPush () {
				Message = "No.2 テストメッセージiOS(1分後)",
				PushToIOS = true,
				DelayByMilliseconds = 1 * 60 * 1000,
			};
			push.SendPush ();
		}
		if (GUILayout.Button ("\nNo.3 : iOS,Androidプッシュ送信\n", bottonStyle)) {
			this.CheckEditor ();
			NCMBPush push = new NCMBPush () {
				Message = "No.3 テストメッセージiOS,Android",
				DelayByMilliseconds = 0,
			};
			push.SendPush ();
		}
		
		
		/* Android */
		GUILayout.Space (20);
		GUILayout.Label ("- Android - ", targetStyle);
		if (GUILayout.Button ("\nNo.1 : プッシュ送信\n", bottonStyle)) {
			this.CheckEditor ();
			NCMBPush push = new NCMBPush () {
				Title = "No.1 テストタイトル",
				Message = "No.1 テストメッセージAndroid",
				PushToAndroid = true,
				DelayByMilliseconds = 0,
			};
			push.SendPush ();
		}
		if (GUILayout.Button ("\nNo.2 : プッシュ送信(1分後)\n", bottonStyle)) {
			this.CheckEditor ();
			NCMBPush push = new NCMBPush () {
				Title = "No.2 テストタイトル",
				Message = "No.2 テストメッセージAndroid(1分後)",
				PushToAndroid = true,
				DelayByMilliseconds = 1 * 60 * 1000,
			};
			push.SendPush ();
		}
		if (GUILayout.Button ("\nNo.3 : Android,iOSプッシュ送信\n", bottonStyle)) {
			this.CheckEditor ();
			NCMBPush push = new NCMBPush () {
				Message = "No.3 テストメッセージAndroid,iOS",
				DelayByMilliseconds = 0,
			};
			push.SendPush ();
		}
		GUILayout.Space (20);
		GUILayout.EndVertical ();
		GUILayout.EndScrollView ();
	}

	/// <summary>
	/// タイトル、メッセージを表示する
	/// </summary>
	/// <param name="title">タイトル</param>
	/// <param name="message">メッセージ</param>
	void Log (string title, object message)
	{
		m_Log += title + ":" + message.ToString () + "\n";
		m_GuiText.text = m_Log;
	}

	/// <summary>
	/// メッセージを表示する
	/// </summary>
	/// <param name="message">メッセージ</param>
	void Log (string message)
	{
		m_Log += message + "\n";
		m_GuiText.text = m_Log;
	}

	/// <summary>
	/// ログを削除する
	/// </summary>
	/// <param name="message">メッセージ</param>
	void RemoveLog ()
	{
		logCount++;
		if (logCount >= 3) {
			string[] deleteStr = m_Log.Split ("-" [0]);
			m_Log = m_Log.Remove (0, deleteStr [0].Length + 40);
		}
	}
}