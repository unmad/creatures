using UnityEngine;

namespace Islands {

/// <summary>
/// Be aware this will not prevent a non singleton constructor
///   such as `T myT = new T();`
/// To prevent that, add `protected T () {}` to your singleton class.
/// 
/// As a note, this is made as MonoBehaviour because we need Coroutines.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;
	private bool global = false;
	
	private static object _lock = new object();
	
	public static T Instance
	{
		get
		{
//			if (objectWasCreated && applicationIsQuitting) {
//				Debug.LogWarning("[Singleton] Instance '"+ typeof(T) +
//				                 "' already destroyed on application quit." +
//				                 " Won't create again - returning null.");
//				return null;
//			}
			
			lock(_lock)
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType(typeof(T)) as T;

					var found = FindObjectsOfType(typeof(T));
					if ( found.Length > 1 )
					{
						//Debug.LogError(found.Join(", "));
						//Debug.LogError("[Singleton] Something went really wrong " +
						//               " - there should never be more than 1 singleton!" +
						//               " Reopenning the scene might fix it. on " + typeof(T));
						return _instance;
					}
					
					if (_instance == null)
					{
						GameObject singleton = new GameObject();
						_instance = singleton.AddComponent<T>();
						singleton.name = "(singleton) "+ typeof(T).ToString();
//						objectWasCreated = true;

						//DontDestroyOnLoad(singleton);

//						Debug.Log("[Singleton] An instance of " + typeof(T) + 
//						          " is needed in the scene, so '" + singleton +
//						          "' was created with DontDestroyOnLoad.");
					} else {
//						Debug.Log("[Singleton] Using instance already created: " +
//						          _instance.gameObject.name);
					}
				}
				
				return _instance;
			}
		}
	}

	public static bool HasInstance() {
		return _instance != null;
	}
		

//	private static bool objectWasCreated = false;
//	private static bool applicationIsQuitting = false;

	/// <summary>
	/// When Unity quits, it destroys objects in a random order.
	/// In principle, a Singleton is only destroyed when application quits.
	/// If any script calls Instance after it have been destroyed, 
	///   it will create a buggy ghost object that will stay on the Editor scene
	///   even after stopping playing the Application. Really bad!
	/// So, this was made to be sure we're not creating that buggy ghost object.
	/// </summary>
	void OnDestroy () {
		_instance = null;
//		applicationIsQuitting = true;
	}

	protected bool BecomeChosen() {	
		// find all globals in scene. if we come from other scene - stay as main object
		bool isMain = true;
		T[] globals = FindObjectsOfType<T>();
		
		foreach (var g in globals) {
			Singleton<T> singleton = g as Singleton<T>;
			if (g != this && singleton.global)
				isMain = false;
		}
		
		global = isMain;
		
		if (isMain)
			DontDestroyOnLoad(gameObject);
		else
			DestroyImmediate(gameObject);

		return isMain;
	}
}

	public interface ISingletonComponent {
		object CreateInstance();
	}

	public class Singleton<T, R> : MonoBehaviour where T : MonoBehaviour, ISingletonComponent where R : class
	{
		private static R _instance;
		
		private static object _lock = new object();
		
		public static R Instance
		{
			get
			{
				//			if (objectWasCreated && applicationIsQuitting) {
				//				Debug.LogWarning("[Singleton] Instance '"+ typeof(T) +
				//				                 "' already destroyed on application quit." +
				//				                 " Won't create again - returning null.");
				//				return null;
				//			}
				
				lock(_lock)
				{
					if (_instance == null)
					{
						var component = FindObjectOfType(typeof(T)) as T;
						if (component != null)
							_instance = component.CreateInstance() as R;
						
						var found = FindObjectsOfType(typeof(T));
						if ( found.Length > 1 )
						{
							//Debug.LogError(found.Join(", "));
							//Debug.LogError("[Singleton] Something went really wrong " +
							//               " - there should never be more than 1 singleton!" +
							//               " Reopenning the scene might fix it. on " + typeof(T));
							return _instance;
						}
						
						if (_instance == null)
						{
							GameObject singleton = new GameObject();
							component = singleton.AddComponent<T>();
							_instance = component.CreateInstance() as R;
							singleton.name = "(singleton) "+ typeof(T).ToString();
							//						objectWasCreated = true;
							
							//DontDestroyOnLoad(singleton);
							
							//						Debug.Log("[Singleton] An instance of " + typeof(T) + 
							//						          " is needed in the scene, so '" + singleton +
							//						          "' was created with DontDestroyOnLoad.");
						} else {
							//						Debug.Log("[Singleton] Using instance already created: " +
							//						          _instance.gameObject.name);
						}
					}
					
					return _instance;
				}
			}
		}
		//	private static bool objectWasCreated = false;
		//	private static bool applicationIsQuitting = false;
		
		/// <summary>
		/// When Unity quits, it destroys objects in a random order.
		/// In principle, a Singleton is only destroyed when application quits.
		/// If any script calls Instance after it have been destroyed, 
		///   it will create a buggy ghost object that will stay on the Editor scene
		///   even after stopping playing the Application. Really bad!
		/// So, this was made to be sure we're not creating that buggy ghost object.
		/// </summary>
		void OnDestroy () {
			//		applicationIsQuitting = true;
		}
	}

}