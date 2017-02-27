using UnityEngine;
using System.Collections;

//Inhert from this to get a static game manager singleton
namespace RL_Helpers {      //Default namespace for helper code

    abstract public class Singleton : MonoBehaviour {
        static public bool ShowDebug = true;    //Show Debug messages

        protected bool CreateSingleton<T>(ref T sGM) where T : Singleton {  //Set Up singleton for a Type
            if (sGM == null) {
                sGM = (T)this;
                DontDestroyOnLoad(gameObject);
                DebugMsg("First time creation of:" + this.GetType().Name);
                return true;        //Signal back if this is the first time this has been created
            } else if (sGM != this) {
                Destroy(gameObject);
                DebugMsg("Subsequent creation of:" + this.GetType().Name + " ignored");
            }
            return false;   //Don't do it twice
        }

        //Allows debug string to be output, but allows this to turned off anythere in code by clearing ShowDebug 
        public static void DebugMsg(string vMessage) {
            if (ShowDebug) {
                Debug.Log(vMessage);
            }
        }
    }
}