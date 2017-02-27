using UnityEngine;
using System.Collections;
using RL_Helpers;

public class GM : Singleton {

    #region
    public enum AsteroidSize {
        Big=0
        ,Medium
        ,Small
        ,None
    }

    public GameObject[] AsteroidPrefabs;

    #endregion


    #region Singleton       //Deal with once only creation
    //We keep the actual variable hidden
    static GM sGM;

    void Awake() {
        if (CreateSingleton<GM>(ref sGM)) {    //Do one time setup inside
        }
    }
    #endregion


    #region Player

    PlayerShip  mPlayerShip;    //Keep variable hidden

    public static PlayerShip PlayerShip {       //Allow access to Player ship globally
        get {
            return sGM.mPlayerShip;
        }
    }

    public static void    RegisterPlayerShip(PlayerShip vPS) {
        sGM.mPlayerShip = vPS;
    }

    #endregion

    #region Asteroids

    public  static  void    CreateAsteroid(Vector2 tPosition, ) {
    }
    #endregion

}
