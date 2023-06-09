using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{

    public Animator anim;
    public GameObject levelSelect;

    void Start(){
      levelSelect.SetActive( false );
    }

    public void EasyGameStart(){
      sound();
      Gomoku.changeLevel( 3 );
      SceneManager.LoadScene( "Gomoku" );
    }

    public void NormalGameStart(){
      sound();
      Gomoku.changeLevel( 5 );
      SceneManager.LoadScene( "Gomoku" );
    }

    public void HardGameStart(){
      sound();
      Gomoku.changeLevel( 10 );
      SceneManager.LoadScene( "Gomoku" );
    }

    public void openLevelSelect(){
      sound();
      levelSelect.SetActive( true );
    }

    public void closeLevelSelect(){
      sound();
      levelSelect.SetActive( false );
    }

    void sound(){
      AudioSource audio = GetComponent<AudioSource>();
      if( audio != null )
        audio.PlayOneShot( audio.clip );
    }

    public void Shake(){
      anim.SetTrigger( "Shake" );
    }
}
