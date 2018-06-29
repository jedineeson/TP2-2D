using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour 
{
	
	[SerializeField]
	private float m_LoadingTimer = 5f;
	[SerializeField]
	private GameObject m_LoadingScreen;
	private static LevelManager m_Instance;
	public static LevelManager Instance
	{		
		get { return m_Instance; }
	}

	private void Awake()
	{
		if(m_Instance != null)
		{
			Debug.Log("Je ne suis pas le seul je dois me détruire.");
			Destroy(gameObject);
		}
		else
		{
			Debug.Log("Je suis le premier, je reste.");
			m_Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		m_LoadingScreen.SetActive(false);
	}

	private void OnLoadingDone(Scene aScene, LoadSceneMode aMode)
	{
		SceneManager.sceneLoaded -= OnLoadingDone;
		m_LoadingScreen.SetActive(false);
	}

	public void ChangeLevel(string aScene)
	{
		StartCoroutine(Loading(aScene));
	}

	private IEnumerator Loading(string aScene)
	{
		m_LoadingScreen.SetActive(true);
		yield return new WaitForSeconds(m_LoadingTimer);
		SceneManager.LoadScene(aScene);
		SceneManager.sceneLoaded += OnLoadingDone;
	}

}