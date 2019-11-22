using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
	const string privateCode = "EpItyRWFY0OryLfDZ3OvrgFBBCiAytHkmVuPOFWS5RJg";
	const string publicCode = "584e5fea8af60302f8f9553e";
	const string webURL = "http://dreamlo.com/lb/";

	public int currentLevel;
	public Text showLevelTxt;
	public int heart;
	public GameObject[] heartsImg = new GameObject[10];
	public int eggs;
	public Text countEggs;
	public Animator countEggsAnimator;
	public bool isDead = true;
	public GameObject player;
	public Transform startPoint;
	public GameObject mainCamera;
	public GameObject gameOverTxt;
	public Text timeTxt;
	public int levelTime;
	public int currentTime;
	public Animator timeAnimator;
	public Animator addHeartAnimator;
	public GameObject audioController;
	public AudioMixer audioMixer;
	public float isEndLevel = 0f;
	public Text playTxt;
	public GameObject menuImg;
	public InputField playerNameTxt;
	public Scrollbar volumeGame;
	public GameObject exitNoBtn, exitYesBtn;
	public GameObject playMenu;
	public GameObject shareScoreMenu;
	public Text showMsgOnlineTxt;
	public GameObject[] playerOnlineTxtGO = new GameObject[5];
	public GameObject darkImg;
	public GameObject currentPlayer;
	public Button leftBtn, rightBtn, shootBtn, jumpBtn;

	int isAddedHeart = 0;
	int isUploaded = 0;
	int isDownloaded = 0;
	string[] playersOnList;
	int minIndex, maxIndex;
	float alpha = 0f;
	bool startLevel = false;
	AudioSource themeAudio;
	AudioSource addHeartAudio;
	AudioSource endLevelAudio;
	AudioSource buttonAudio;
	Text[,] playerOnlineTxt = new Text[5, 3];
	PlayerController playerC;
	Image darImgColor;
	int countEggsEndLevel;

	void Start ()
	{
		themeAudio = audioController.GetComponent <AudioManagement> ().theme.GetComponent <AudioSource> ();
		addHeartAudio = audioController.GetComponent <AudioManagement> ().addHeart.GetComponent <AudioSource> ();
		endLevelAudio = audioController.GetComponent <AudioManagement> ().endLevel.GetComponent <AudioSource> ();
		buttonAudio = audioController.GetComponent <AudioManagement> ().buttonClick.GetComponent <AudioSource> ();
		darImgColor = darkImg.GetComponent <Image> ();

		currentLevel = SceneManager.GetActiveScene ().buildIndex;
		showLevelTxt.text += ((currentLevel / 3 + 1).ToString () + "-" + (currentLevel % 3 + 1).ToString ());
		if (currentLevel == 9)
			showLevelTxt.text = "Finish";
		volumeGame.value = PlayerPrefs.GetFloat ("Volume", 1f);
		audioMixer.SetFloat ("VolumeGame", ((volumeGame.value * 80f) - 80f));
		playerNameTxt.text = PlayerPrefs.GetString ("PlayerName", "Player");
		string[] tempPlayerName = playerNameTxt.text.Split ("♣" [0]);
		playerNameTxt.text = tempPlayerName [0];
		if (currentLevel == 0)
		{
			PlayerPrefs.SetInt ("Eggs", 0);
			PlayerPrefs.SetInt ("Hearts", 4);
			menuImg.SetActive (true);
			Time.timeScale = 0;
			playTxt.text = "Play";
			playTxt.fontSize = 50;
			themeAudio.volume = 0.2f;
		}
		else
		{
			PlayerPrefs.SetInt ("Hearts", PlayerPrefs.GetInt ("Hearts") + 1);
			darImgColor.color = new Color (0f, 0f, 0f, 1f);
			darkImg.SetActive (true);
			alpha = 1;
			startLevel = true;
		}
		heart = PlayerPrefs.GetInt ("Hearts", 4);
		eggs = PlayerPrefs.GetInt ("Eggs", 0);
		for (int i = 0; i < heart; i++)
			heartsImg [i].SetActive (true);
			
		isAddedHeart = eggs / 100;
		currentTime = levelTime;
		if (currentLevel < 9)
			InvokeRepeating ("SetTime", 1f, 1f);
		else
			countEggsEndLevel = eggs;
	}

	void SetTime ()
	{
		if (currentTime <= 0 || isEndLevel > 0)
			return;
		currentTime--;
		timeTxt.text = string.Empty;
		if (currentTime / 60 < 10)
			timeTxt.text = "0";
		timeTxt.text += (currentTime / 60).ToString () + ":";
		if (currentTime % 60 < 10)
			timeTxt.text += "0";
		timeTxt.text += (currentTime % 60).ToString ();
		timeAnimator.SetInteger ("Time", currentTime);
		if (currentTime < 20)
			themeAudio.pitch = (1f + (0.3f - (currentTime / 66.7f)));
		else
			themeAudio.pitch = 1f;
	}

	void Update ()
	{
		audioMixer.SetFloat ("VolumeGame", ((volumeGame.value * 80f) - 80f));
		countEggs.text = eggs.ToString ();

		if (currentLevel == 9 && eggs == countEggsEndLevel + 58)
		{
			gameOverTxt.SetActive (true);
			if (!menuImg.activeSelf) {
				gameOverTxt.transform.FindChild ("GameOverTxt").GetComponent <Text> ().color = Color.green;
				gameOverTxt.transform.FindChild ("GameOverTxt").GetComponent <Text> ().text = "YOU WON!!!";
				playTxt.text = "Play\nAgain";
				Invoke ("SetPauseBtn", 2f);
			}
		}
		if (isDead) {
			if (heart > 0)
			{
				if (Time.timeScale > 0)
				{
					heart--;
					heartsImg [heart].SetActive (false);
					isDead = false;
					currentPlayer = (GameObject)Instantiate (player, startPoint.position, new Quaternion (0, 0, 0, 0));
					currentPlayer.transform.parent = gameObject.transform;
					if (currentLevel < 9)
						mainCamera.GetComponent <CameraController> ().player = currentPlayer.transform;
					currentTime = levelTime;
					PlayerPrefs.SetInt ("Hearts", heart);
					playerC = currentPlayer.GetComponent <PlayerController> ();
				}
			}
			else
			{
				if (themeAudio.pitch > 0.4f)
					themeAudio.pitch -= 0.05f;
				gameOverTxt.SetActive (true);
				playTxt.text = "Restart\nGame";
				Invoke ("SetPauseBtn", 2f);
				isEndLevel = 0.5f;
			}
		}

		if (eggs / 100 > isAddedHeart && heart < 10)
		{
			heartsImg [heart].SetActive (true);
			heart++;
			isAddedHeart++;
			PlayerPrefs.SetInt ("Hearts", heart);
			addHeartAnimator.SetBool ("AddHeart", true);
			if (!addHeartAudio.isPlaying)
				addHeartAudio.Play ();
		}

		if (isEndLevel > 0.5f && isEndLevel < 4f)
		{
			if (themeAudio.volume > 0.1f)
				themeAudio.volume -= 0.05f;
			themeAudio.pitch = 1f;
			if (!endLevelAudio.isPlaying)
			{
				endLevelAudio.Play ();
				isEndLevel++;
			}
		}

		if (isEndLevel >= 4)
		{
			NextLevel (currentLevel + 2, 1f);
		}

		if (Input.GetKeyDown (KeyCode.Escape))
		{
			if (shareScoreMenu.activeSelf)
				SetDoneBtn ();
			else if (menuImg.activeSelf)
				SetPlayBtn ();
			else
				SetPauseBtn ();
		}

		if (darkImg.activeSelf)
		{
			if (startLevel)
			{
				if (alpha > 0f)
				{
					alpha -= 0.05f;
					darImgColor.color = new Color (0f, 0f, 0f, alpha);
				}
				else
				{
					startLevel = false;
					darkImg.SetActive (false);
				}
			}
			else
			{
				if (alpha <= 1f)
					alpha += 0.05f;
				darImgColor.color = new Color (0f, 0f, 0f, alpha);
			}
		}
		if (!menuImg.activeSelf && !gameOverTxt.activeSelf)
			SetTouchInput ();
	}

	void SetTouchInput ()
	{
		if (currentPlayer == null)
			return;
		bool jump = false;
		bool shoot = false;
		int mov = 0;
		foreach (Touch touch in Input.touches)
		{
			if (touch.phase == TouchPhase.Began)
			{
				if ((touch.position.x >= (Camera.main.WorldToScreenPoint (jumpBtn.transform.position).x - 75f) && touch.position.x <= (Camera.main.WorldToScreenPoint (jumpBtn.transform.position).x + 75f)) &&
				    (touch.position.y >= (Camera.main.WorldToScreenPoint (jumpBtn.transform.position).y - 75f) && touch.position.y <= (Camera.main.WorldToScreenPoint (jumpBtn.transform.position).y + 75f)))
				{
					jump = true;
					break;
				}
				if ((touch.position.x >= (Camera.main.WorldToScreenPoint (shootBtn.transform.position).x - 75f) && touch.position.x <= (Camera.main.WorldToScreenPoint (shootBtn.transform.position).x + 75f)) &&
				    (touch.position.y >= (Camera.main.WorldToScreenPoint (shootBtn.transform.position).y - 75f) && touch.position.y <= (Camera.main.WorldToScreenPoint (shootBtn.transform.position).y + 75f)))
				{
					shoot = true;
					break;
				}
			}
		}
		if (jump)
		{
			SetJumpBtn (jump);
			jump = false;
		}
		if (shoot)
		{
			SetShootBtn (shoot);
			shoot = false;
		}

		foreach (Touch touch in Input.touches)
		{
			if ((touch.position.x >= (Camera.main.WorldToScreenPoint (leftBtn.transform.position).x - 85f) && touch.position.x <= (Camera.main.WorldToScreenPoint (leftBtn.transform.position).x + 85f)) &&
			    (touch.position.y >= (Camera.main.WorldToScreenPoint (leftBtn.transform.position).y - 70f) && touch.position.y <= (Camera.main.WorldToScreenPoint (leftBtn.transform.position).y + 70f))) {
				mov = -1;
				break;
			}
			if ((touch.position.x >= (Camera.main.WorldToScreenPoint (rightBtn.transform.position).x - 85f) && touch.position.x <= (Camera.main.WorldToScreenPoint (rightBtn.transform.position).x + 85f)) &&
			    (touch.position.y >= (Camera.main.WorldToScreenPoint (rightBtn.transform.position).y - 70f) && touch.position.y <= (Camera.main.WorldToScreenPoint (rightBtn.transform.position).y + 70f))) {
				mov = 1;
				break;
			}
		}
		SetMoveBtn (mov);
		mov = 0;
	}

	public void SetMoveBtn (int mvb)
	{
		if (isEndLevel < 1)
			playerC.moveBtn = mvb;
	}

	public void SetShootBtn (bool trigun)
	{
		if (isEndLevel < 1)
			playerC.ShootCarrot (trigun);
	}

	public void SetJumpBtn (bool jumpBtn)
	{
		if (isEndLevel < 1)
			playerC.Jump (jumpBtn);
	}

	public void SetRestartGame ()
	{
		StartCoroutine (StartNewGame (1, 0f));
	}

	void NextLevel (int levelNumber, float second)
	{
		StartCoroutine (StartNewGame (levelNumber, second));
	}

	IEnumerator StartNewGame (int levelNumber, float second)
	{
		yield return new WaitForSeconds (second);
		darkImg.SetActive (true);
		yield return new WaitForSeconds (0.3f);
		SceneManager.LoadScene ("Level" + levelNumber.ToString ());
	}

	public void SetPauseBtn ()
	{
		if (isEndLevel < 1)
		{
			if (!menuImg.activeSelf)
			{
				buttonAudio.Play ();
				menuImg.SetActive (true);
			}
			if (playTxt.text == "Play\nAgain")
			{
				gameOverTxt.transform.FindChild ("GameOverTxt").GetComponent <Text> ().enabled = false;
				gameOverTxt.transform.FindChild ("NoteTxt").GetComponent <Text> ().text = "YOU WON!!!\nShare Your Score and Play Agian!";
				return;
			}
			if (playTxt.text != "Restart\nGame")
				Time.timeScale = 0;
			else
			{
				gameOverTxt.transform.FindChild ("GameOverTxt").GetComponent <Text> ().text = "";
				gameOverTxt.transform.FindChild ("NoteTxt").GetComponent <Text> ().text = "Game Over!!!\nShare Your Score and Try Agian!";
			}
		}
	}

	public void SetPlayBtn ()
	{
		buttonAudio.Play ();
		Time.timeScale = 1;
		if (playTxt.text == "Restart\nGame" || playTxt.text == "Play\nAgain")
		{
			SetRestartGame ();
			return;
		}
		themeAudio.volume = 1f;
		playTxt.text = "Resume";
		playTxt.fontSize = 35;
		exitNoBtn.SetActive (false);
		exitYesBtn.SetActive (false);
		menuImg.SetActive (false);
	}

	public void SetExitBtn ()
	{
		buttonAudio.Play ();
		exitNoBtn.SetActive (true);
		exitYesBtn.SetActive (true);
	}

	public void SetExitNoBtn ()
	{
		buttonAudio.Play ();
		exitNoBtn.SetActive (false);
		exitYesBtn.SetActive (false);
	}

	public void SetExitYesBtn ()
	{
		buttonAudio.Play ();
		Application.Quit ();
	}

	public void SetVolumeGame ()
	{
		audioMixer.SetFloat ("VolumeGame", ((volumeGame.value * 80f) - 80f));
		PlayerPrefs.SetFloat ("Volume", volumeGame.value);
	}

	public void SetCorrectPlayerName ()
	{
		if (playerNameTxt.text == "" || playerNameTxt.text == null)
			return;
		if ((playerNameTxt.text [playerNameTxt.text.Length - 1] >= 'a' && playerNameTxt.text [playerNameTxt.text.Length - 1] <= 'z') ||
		    (playerNameTxt.text [playerNameTxt.text.Length - 1] >= 'A' && playerNameTxt.text [playerNameTxt.text.Length - 1] <= 'Z') ||
		    (playerNameTxt.text [playerNameTxt.text.Length - 1] >= '0' && playerNameTxt.text [playerNameTxt.text.Length - 1] <= '9') ||
			 playerNameTxt.text [playerNameTxt.text.Length - 1] == '-' || playerNameTxt.text [playerNameTxt.text.Length - 1] == '_'  ||
			 playerNameTxt.text [playerNameTxt.text.Length - 1] == '+' || playerNameTxt.text [playerNameTxt.text.Length - 1] == ',')
			return;
		playerNameTxt.text = playerNameTxt.text.Substring (0, playerNameTxt.text.Length - 1);
			
	}

	public void SetPlayerName ()
	{
		playerNameTxt.text = playerNameTxt.text.Trim ();
		if (playerNameTxt.text == null || playerNameTxt.text == "")
			playerNameTxt.text = "Player";

		string [] tempCompareTxt = PlayerPrefs.GetString ("PlayerName").Split ("♣" [0]);
		if (tempCompareTxt [0] != playerNameTxt.text)
		PlayerPrefs.SetString ("PlayerName", playerNameTxt.text + "♣" +
			System.DateTime.Now.DayOfYear.ToString () +
			System.DateTime.Now.Hour.ToString () +
			System.DateTime.Now.Minute.ToString () +
			System.DateTime.Now.Second.ToString ());
	}

	public void SetDoneBtn ()
	{
		buttonAudio.Play ();
		playMenu.SetActive (true);
		shareScoreMenu.SetActive (false);
	}

	public void SetShareScore ()
	{
		buttonAudio.Play ();

		for (int k = 0; k < 5; k++)
		{
			playerOnlineTxt [k, 0] = playerOnlineTxtGO [k].transform.FindChild ("PlayerNameTxt").GetComponent <Text> ();
			playerOnlineTxt [k, 1] = playerOnlineTxtGO [k].transform.FindChild ("EggsTxt").GetComponent <Text> ();
			playerOnlineTxt [k, 2] = playerOnlineTxtGO [k].transform.FindChild ("LevelTxt").GetComponent <Text> ();
		}

		string playerNameOnline = PlayerPrefs.GetString ("PlayerName");
		if (playerNameOnline == "" || playerNameOnline == null)
		{
			SetPlayerName ();
			playerNameOnline = PlayerPrefs.GetString ("PlayerName");
		}

		for (int i = 0; i < 5; i++)
		{
			playerOnlineTxt [i, 0].text = "...";
			playerOnlineTxt [i, 1].text = "...";
			playerOnlineTxt [i, 2].text = "...";
		}

		playMenu.SetActive (false);
		shareScoreMenu.SetActive (true);
		StartCoroutine (UploadNewHighScore (playerNameOnline));
		showMsgOnlineTxt.text = "Please Wait...";
	}

	IEnumerator UploadNewHighScore (string username)
	{
		WWW wwwUpload = new WWW (webURL + privateCode + "/add/" + WWW.EscapeURL (username) + "/" + eggs + "/" + currentLevel);
		yield return wwwUpload;

		if (string.IsNullOrEmpty (wwwUpload.error))
		{
			isUploaded = 1;
			print ("Upload: " + isUploaded.ToString ());
			print ("UploadError: " + wwwUpload.text);
			//showMsgOnlineTxt.text = "Uploaded!";

			WWW wwwDownload = new WWW (webURL + publicCode + "/pipe/");
			yield return wwwDownload;
	
			if (string.IsNullOrEmpty (wwwDownload.error))
			{
				print ("List" + wwwDownload.text + "EndList");
				playersOnList = wwwDownload.text.Split ("\n" [0]);
				isDownloaded = 1;
				print ("Download: " + isDownloaded.ToString ());
				print ("MaxPlayer: " + (playersOnList.Length - 1).ToString ());
				minIndex = 0;
				if ((playersOnList.Length - 1) >= 5)
					maxIndex = 5;
				else
					maxIndex = playersOnList.Length - 1;				
				ScoreFormatedShow ();
			}
			else
			{
				isDownloaded = -1;
				print ("Download: " + isDownloaded.ToString ());
				showMsgOnlineTxt.text = "Error!!!";
			}
		}
		else
		{
			isUploaded = -1;
			print ("Upload: " + isUploaded.ToString ());
			showMsgOnlineTxt.text = "Error!!!";

			/*test
			string testString = "5856|5116|8||12/12/2016 5:04:55 PM|0\n-_,+|1000|90||12/12/2016 7:30:28 PM|1\ncancer_232f23|1000|0||12/12/2016 7:05:05 PM|2\ncan-cer_232f23|1000|1||12/12/2016 7:05:21 PM|3\nfwefwefwe1651515615f15we5we|1000|3||12/12/2016 7:03:15 PM|4\nCarmine|1000|4||12/12/2016 5:02:50 PM|5\nba ba|1000|5||12/12/2016 6:07:50 PM|6\na+b+c♣347213628|597|10||12/12/2016 6:06:33 PM|7\nwwwwwwwwwww♣3472167|597|1||12/12/2016 6:02:19 PM|8\nSex♣347213656|597|1||12/12/2016 6:07:02 PM|9\nFuck+You♣347213258|597|1||12/12/2016 6:05:35 PM|10\neqfwâ™£347213910|597|1||12/12/2016 6:09:17 PM|11\nAFWE♣34723240|597|1||12/12/2016 7:35:24 PM|12\nbetbebrbae♣34720392|595|1||12/12/2016 5:09:08 PM|13\nfweafwf♣34720335|595|1||12/12/2016 5:03:21 PM|14\nzfsdwwe♣34720407|595|1||12/12/2016 5:11:04 PM|15\nwwwwwwwwwww♣347204953|595|1||12/12/2016 5:35:34 PM|16\nymtnhrgb♣347203531|595|1||12/12/2016 5:05:38 PM|17\nqwertyuiopasdfghjklzxcvbn♣347193916|583|1||12/12/2016 5:01:55 PM|18\nCaFwefwee|55|423||12/12/2016 5:04:26 PM|19\n916|19|716||12/12/2016 5:05:21 PM|20\nbaaasjk|14|12||12/12/2016 5:04:42 PM|21\nrwgr|13|1165||12/12/2016 5:05:05 PM|22";
			playersOnList = testString.Split ("\n" [0]);
			isDownloaded = 1;
			minIndex = 0;
			if (playersOnList.Length >= 5)
				maxIndex = 5;
			else
				maxIndex = playersOnList.Length;

			showMsgOnlineTxt.text = "Downloaded!";
			ScoreFormatedShow ();
			*/
		}
	}

	void ScoreFormatedShow ()
	{
		showMsgOnlineTxt.text = (minIndex + 1).ToString () + " - ";
		if (maxIndex <= playersOnList.Length)
			showMsgOnlineTxt.text += maxIndex.ToString ();
		else
			showMsgOnlineTxt.text += (playersOnList.Length - 1).ToString ();
		
		for (int i = minIndex; i < maxIndex; i++)
		{
			if (i < playersOnList.Length - 1)
			{
				string[] featurePlayer = playersOnList [i].Split ("|" [0]);
				string[] tempNamePlayer = featurePlayer [0].Split ("♣" [0]);
				if (tempNamePlayer [0].Length > 8)
					tempNamePlayer [0] = tempNamePlayer [0].Substring (0, 8) + "...";

				if (featurePlayer [0] == PlayerPrefs.GetString ("PlayerName"))
				{
					playerOnlineTxt [i % 5, 0].color = Color.cyan;
					playerOnlineTxt [i % 5, 1].color = Color.cyan;
					playerOnlineTxt [i % 5, 2].color = Color.cyan;
				}
				else
				{
					playerOnlineTxt [i % 5, 0].color = new Color (158f / 255f, 74f / 255f, 7f / 255f, 255f / 255f);
					playerOnlineTxt [i % 5, 1].color = new Color (158f / 255f, 74f / 255f, 7f / 255f, 255f / 255f);
					playerOnlineTxt [i % 5, 2].color = new Color (158f / 255f, 74f / 255f, 7f / 255f, 255f / 255f);
				}
				
				playerOnlineTxt [i % 5, 0].text = tempNamePlayer [0];
				playerOnlineTxt [i % 5, 1].text = featurePlayer [1];
				if (int.Parse (featurePlayer [2]) >= 9)
					playerOnlineTxt [i % 5, 2].text = "Finish";
				else
					playerOnlineTxt [i % 5, 2].text = ((int.Parse (featurePlayer [2]) / 3 + 1).ToString () + "-" + (int.Parse (featurePlayer [2]) % 3 + 1).ToString ());
			}
			else
			{
				playerOnlineTxt [i % 5, 0].text = "";
				playerOnlineTxt [i % 5, 1].text = "";
				playerOnlineTxt [i % 5, 2].text = "";
			}
		}
	}

	public void SetLeft_RightBtn (int i)
	{
		if (isDownloaded > 0)
		{
			if ((i < 0 && minIndex >= 5) || (i > 0 && (maxIndex < playersOnList.Length - 1)))
			{
				buttonAudio.Play ();
				minIndex += i;
				maxIndex += i;
				ScoreFormatedShow ();
			}
		}
	}
}
