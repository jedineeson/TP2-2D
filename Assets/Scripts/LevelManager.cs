using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour 
{
	public GameObject m_LoadingScreen;
	private static LevelManager m_Instance;
	//variable de moi même(on peux y accéder de partout avec LevelManager.Instance)
	public static LevelManager Instance
	{
		//on veux jamais le re-set		
		get { return m_Instance; }
	}

	private void Awake()
	{
		//Si ma variable instance = null, je suis le premier(le script levelManager) "LevelManager script" dans la hierarchy.
		//Je m'assigne moi même(le script levelManager) à m_Instance pour m'assurer d'être le seul. 
		//Si instance != null, ça veux dire que je ne suis pas le premier, je me détruis car il ne peut y en avoir qu'un.
		if(m_Instance != null)
		{
			Debug.Log("Je ne suis pas le seul je dois me détruire.");
			//pour pas en avoir d'autre
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


	private void StartLoading()
	{
		m_LoadingScreen.SetActive(true);
		//Play animation
	}

	private void OnLoadingDone(Scene aScene, LoadSceneMode aMode)
	{
		//Stop animation
		SceneManager.sceneLoaded -= OnLoadingDone;
		m_LoadingScreen.SetActive(false);
	}

	public void ChangeLevel(string aScene)
	{
		StartLoading();
		SceneManager.LoadScene(aScene);
		//Action/Event qui trigger automatiquement la fonction donné
		SceneManager.sceneLoaded += OnLoadingDone;
	}

}