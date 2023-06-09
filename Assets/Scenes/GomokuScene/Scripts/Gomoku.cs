using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using Cinemachine;


public class Gomoku : MonoBehaviour
{

    public GameObject blackStone;
    public GameObject whiteStone;
    public GameObject rotateGimmick;
    public GameObject upGimmick;
    public GameObject downGimmick;
    public GameObject leftGimmick;
    public GameObject rightGimmick;
    public Animator blackHand;
    public GameObject blackHandRoot;
    public Animator whiteHand;
    public GameObject whiteHandRoot;
    public GameObject Menu;
    public GameObject EnterMenu;
    public CinemachineDollyCart CameraPath;
    public GameObject tebanBlack;
    public GameObject tebanWhite;
    public GameObject Win;
    public Text Winner;
    public AudioClip putClip;
    public AudioClip finishClip;
    public AudioClip soundClip;

    bool BLACK = true;
    bool WHITE = false;

    enum State{ BLACK, WHITE, EMPTY, ROTATE, UP, DOWN, LEFT, RIGHT }

    State[,] field;
    GameObject[,] stones;
    GameObject[,] gimmicks;
    GameObject dammyBlackStone;
    GameObject dammyWhiteStone;

    int FIELD_SIZE = 15;
    static int GIMMICK_LEVEL = 10;
    int[] dx = { 1, 1, 0, -1, -1, -1, 0, 1 };
    int[] dz = { 0, 1, 1, 1, 0, -1, -1, -1 };

    bool turn;
    bool flg;

    float x, z;
    float time;

    PlayableDirector pd;

    public static void changeLevel( int lv ){
        GIMMICK_LEVEL = lv;
    }

    void Start()
    {
      field = new State[ FIELD_SIZE, FIELD_SIZE ];
      stones = new GameObject[ FIELD_SIZE, FIELD_SIZE ];
      gimmicks = new GameObject[ FIELD_SIZE, FIELD_SIZE ];
      pd = GetComponent<PlayableDirector>();
      NullInit();
      Init();
    }

    public void Restart()
    {
      sound();
      pd.time = 0.0f;
      CameraPath.m_Position = 0.0f;
      Win.SetActive( false );
      Init();
    }

    public void Init()
    {
      BGM();
      turn = BLACK;
      tebanBlack.SetActive( true );
      tebanWhite.SetActive( false );
      flg = true;
      x = 0f;
      z = 0f;
      time = 0f;
      for( int i = 0 ; i < FIELD_SIZE ; i++ ){
        for( int j = 0 ; j < FIELD_SIZE ; j++ ){
          if( stones[i,j] != null )
            Destroy( stones[i,j] );
          if( gimmicks[i,j] != null )
            Destroy( gimmicks[i,j] );
        }
      }
      NullInit();
      Destroy( dammyBlackStone );
      Destroy( dammyWhiteStone );
      dammyBlackStone = Instantiate( blackStone, new Vector3( 0f, 0f, 0f ), Quaternion.identity );
      dammyBlackStone.SetActive( false );
      dammyWhiteStone = Instantiate( whiteStone, new Vector3( 0f, 0f, 0f ), Quaternion.identity );
      dammyWhiteStone.SetActive( false );
      PlayableDirector pd = GetComponent<PlayableDirector>();
      pd.stopped += OnPlayableDirectorStopped;
      pd.Play();
    }

    void NullInit()
    {
      for( int i = 0 ; i < FIELD_SIZE ; i++ ){
        for( int j = 0 ; j < FIELD_SIZE ; j++ ){
          field[i,j] = State.EMPTY;
          stones[i,j] = null;
          gimmicks[i,j] = null;
        }
      }
      for( int i = 0 ; i < GIMMICK_LEVEL ; i++ ){
        int id = UnityEngine.Random.Range( 0, FIELD_SIZE*FIELD_SIZE );
        int _x = id % FIELD_SIZE;
        int _z = id / FIELD_SIZE;
        if( gimmicks[_x,_z] != null )
          Destroy( gimmicks[_x,_z] );
        gimmicks[_x,_z] = Instantiate( rotateGimmick );
        gimmicks[_x,_z].transform.position = new Vector3( _x, gimmicks[_x,_z].transform.position.y, _z );
        field[_x,_z] = State.ROTATE;
      }
      for( int i = 0 ; i < GIMMICK_LEVEL ; i++ ){
        int id = UnityEngine.Random.Range( 0, FIELD_SIZE*FIELD_SIZE );
        int _x = id % FIELD_SIZE;
        int _z = id / FIELD_SIZE;
        if( gimmicks[_x,_z] != null )
          Destroy( gimmicks[_x,_z] );
        gimmicks[_x,_z] = Instantiate( upGimmick );
        gimmicks[_x,_z].transform.position = new Vector3( _x, gimmicks[_x,_z].transform.position.y, _z );
        field[_x,_z] = State.UP;
      }
      for( int i = 0 ; i < GIMMICK_LEVEL ; i++ ){
        int id = UnityEngine.Random.Range( 0, FIELD_SIZE*FIELD_SIZE );
        int _x = id % FIELD_SIZE;
        int _z = id / FIELD_SIZE;
        if( gimmicks[_x,_z] != null )
          Destroy( gimmicks[_x,_z] );
        gimmicks[_x,_z] = Instantiate( downGimmick );
        gimmicks[_x,_z].transform.position = new Vector3( _x, gimmicks[_x,_z].transform.position.y, _z );
        field[_x,_z] = State.DOWN;
      }
      for( int i = 0 ; i < GIMMICK_LEVEL ; i++ ){
        int id = UnityEngine.Random.Range( 0, FIELD_SIZE*FIELD_SIZE );
        int _x = id % FIELD_SIZE;
        int _z = id / FIELD_SIZE;
        if( gimmicks[_x,_z] != null )
          Destroy( gimmicks[_x,_z] );
        gimmicks[_x,_z] = Instantiate( leftGimmick );
        gimmicks[_x,_z].transform.position = new Vector3( _x, gimmicks[_x,_z].transform.position.y, _z );
        field[_x,_z] = State.LEFT;
      }
      for( int i = 0 ; i < GIMMICK_LEVEL ; i++ ){
        int id = UnityEngine.Random.Range( 0, FIELD_SIZE*FIELD_SIZE );
        int _x = id % FIELD_SIZE;
        int _z = id / FIELD_SIZE;
        if( gimmicks[_x,_z] != null )
          Destroy( gimmicks[_x,_z] );
        gimmicks[_x,_z] = Instantiate( rightGimmick );
        gimmicks[_x,_z].transform.position = new Vector3( _x, gimmicks[_x,_z].transform.position.y, _z );
        field[_x,_z] = State.RIGHT;
      }
    }

    void dammyStoneDisp( bool blink = false ){
      if( turn == BLACK ){
        if( blink )
          dammyBlackStone.SetActive( true );
        else
          dammyBlackStone.SetActive( false );
      }
      if( turn == WHITE ){
        if( blink )
          dammyWhiteStone.SetActive( true );
        else
          dammyWhiteStone.SetActive( false );
      }
    }

    void dammyStoneMove(){
      if( turn == BLACK )
        dammyBlackStone.transform.position = new Vector3( x, 0f, z );
      if( turn == WHITE )
        dammyWhiteStone.transform.position = new Vector3( x, 0f, z );
    }

    public void realPut(){

      if( turn == BLACK )
        stones[(int)x,(int)z] = Instantiate( blackStone, new Vector3( x, 0f, z ), Quaternion.identity );
      if( turn == WHITE )
        stones[(int)x,(int)z] = Instantiate( whiteStone, new Vector3( x, 0f, z ), Quaternion.identity );
      if( gimmicks[(int)x,(int)z] != null ){
        Destroy( gimmicks[(int)x,(int)z] );
        switch( field[(int)x,(int)z] ){
          case State.ROTATE: field[(int)x,(int)z] = ( turn == BLACK ? State.BLACK : State.WHITE ); rotate(); break;
          case State.UP: field[(int)x,(int)z] = ( turn == BLACK ? State.BLACK : State.WHITE ); up(); break;
          case State.DOWN: field[(int)x,(int)z] = ( turn == BLACK ? State.BLACK : State.WHITE ); down(); break;
          case State.LEFT: field[(int)x,(int)z] = ( turn == BLACK ? State.BLACK : State.WHITE ); left(); break;
          case State.RIGHT: field[(int)x,(int)z] = ( turn == BLACK ? State.BLACK : State.WHITE ); right(); break;
        }
      }
      else
        field[(int)x,(int)z] = ( turn == BLACK ? State.BLACK : State.WHITE );
      putSound();
      lineCheck();
      flg = false;
      turn = !turn;
      tebanBlack.SetActive( turn == BLACK );
      tebanWhite.SetActive( turn == WHITE );

    }

    public void rotate(){

      int dist = Math.Max( Math.Abs( (int)x - FIELD_SIZE / 2 ), Math.Abs( (int)z - FIELD_SIZE / 2 ) );
      if( dist == 0 ){
        if( field[(int)x,(int)z] == State.BLACK )
          stones[(int)x,(int)z] = Instantiate( blackStone, new Vector3( x, 0f, z ), Quaternion.identity );
        if( field[(int)x,(int)z] == State.WHITE )
          stones[(int)x,(int)z] = Instantiate( whiteStone, new Vector3( x, 0f, z ), Quaternion.identity );
      }
      else{
        for( int i = 1 ; i < 2 * dist + 1 ; i++ ){
          if( stones[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist] != null )
            Destroy( stones[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist] );
          if( gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist] != null )
            Destroy( gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist] );
        }
        for( int i = 1 ; i < 2 * dist + 1 ; i++ ){
          if( stones[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i] != null )
            Destroy( stones[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i] );
          if( gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i] != null )
            Destroy( gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i] );
        }
        for( int i = 1 ; i < 2 * dist + 1 ; i++ ){
          if( stones[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist] != null )
            Destroy( stones[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist] );
          if( gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist] != null )
            Destroy( gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist] );
        }
        for( int i = 1 ; i < 2 * dist + 1 ; i++ ){
          if( stones[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i] != null )
            Destroy( stones[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i] );
          if( gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i] != null )
            Destroy( gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i] );
        }
        State s = field[FIELD_SIZE/2-dist,FIELD_SIZE/2-dist];
        for( int i = 0 ; i < 2 * dist ; i++ )
          field[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist] = field[FIELD_SIZE/2-dist+i+1,FIELD_SIZE/2-dist];
        for( int i = 0 ; i < 2 * dist ; i++ )
          field[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i] = field[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i+1];
        for( int i = 0 ; i < 2 * dist ; i++ )
          field[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist] = field[FIELD_SIZE/2+dist-i-1,FIELD_SIZE/2+dist];
        for( int i = 0 ; i < 2 * dist ; i++ )
          field[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i] = field[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i-1];
        field[FIELD_SIZE/2-dist,FIELD_SIZE/2-dist+1] = s;
        for( int i = 1 ; i < 2 * dist + 1 ; i++ ){
          switch( field[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist] ){
            case State.BLACK:  stones[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist] = Instantiate( blackStone, new Vector3( FIELD_SIZE/2-dist+i, 0f, FIELD_SIZE/2-dist ), Quaternion.identity ); break;
            case State.WHITE:  stones[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist] = Instantiate( whiteStone, new Vector3( FIELD_SIZE/2-dist+i, 0f, FIELD_SIZE/2-dist ), Quaternion.identity ); break;
            case State.UP:     gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist] = Instantiate( upGimmick );
                               gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist].transform.position = new Vector3( FIELD_SIZE/2-dist+i, gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist].transform.position.y, FIELD_SIZE/2-dist ); break;
            case State.DOWN:   gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist] = Instantiate( downGimmick );
                               gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist].transform.position = new Vector3( FIELD_SIZE/2-dist+i, gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist].transform.position.y, FIELD_SIZE/2-dist ); break;
            case State.LEFT:   gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist] = Instantiate( leftGimmick );
                               gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist].transform.position = new Vector3( FIELD_SIZE/2-dist+i, gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist].transform.position.y, FIELD_SIZE/2-dist ); break;
            case State.RIGHT:  gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist] = Instantiate( rightGimmick );
                               gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist].transform.position = new Vector3( FIELD_SIZE/2-dist+i, gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist].transform.position.y, FIELD_SIZE/2-dist ); break;
            case State.ROTATE: gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist] = Instantiate( rotateGimmick );
                               gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist].transform.position = new Vector3( FIELD_SIZE/2-dist+i, gimmicks[FIELD_SIZE/2-dist+i,FIELD_SIZE/2-dist].transform.position.y, FIELD_SIZE/2-dist ); break;
          }
        }
        for( int i = 1 ; i < 2 * dist + 1 ; i++ ){
          switch( field[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i] ){
            case State.BLACK:  stones[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i] = Instantiate( blackStone, new Vector3( FIELD_SIZE/2+dist, 0f, FIELD_SIZE/2-dist+i ), Quaternion.identity ); break;
            case State.WHITE:  stones[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i] = Instantiate( whiteStone, new Vector3( FIELD_SIZE/2+dist, 0f, FIELD_SIZE/2-dist+i ), Quaternion.identity ); break;
            case State.UP:     gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i] = Instantiate( upGimmick );
                               gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i].transform.position = new Vector3( FIELD_SIZE/2+dist, gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i].transform.position.y, FIELD_SIZE/2-dist+i ); break;
            case State.DOWN:   gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i] = Instantiate( downGimmick );
                               gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i].transform.position = new Vector3( FIELD_SIZE/2+dist, gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i].transform.position.y, FIELD_SIZE/2-dist+i ); break;
            case State.LEFT:   gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i] = Instantiate( leftGimmick );
                               gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i].transform.position = new Vector3( FIELD_SIZE/2+dist, gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i].transform.position.y, FIELD_SIZE/2-dist+i ); break;
            case State.RIGHT:  gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i] = Instantiate( rightGimmick );
                               gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i].transform.position = new Vector3( FIELD_SIZE/2+dist, gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i].transform.position.y, FIELD_SIZE/2-dist+i ); break;
            case State.ROTATE: gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i] = Instantiate( rotateGimmick );
                               gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i].transform.position = new Vector3( FIELD_SIZE/2+dist, gimmicks[FIELD_SIZE/2+dist,FIELD_SIZE/2-dist+i].transform.position.y, FIELD_SIZE/2-dist+i ); break;
          }
        }
        for( int i = 1 ; i < 2 * dist + 1 ; i++ ){
          switch( field[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist] ){
            case State.BLACK:  stones[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist] = Instantiate( blackStone, new Vector3( FIELD_SIZE/2+dist-i, 0f, FIELD_SIZE/2+dist ), Quaternion.identity ); break;
            case State.WHITE:  stones[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist] = Instantiate( whiteStone, new Vector3( FIELD_SIZE/2+dist-i, 0f, FIELD_SIZE/2+dist ), Quaternion.identity ); break;
            case State.UP:     gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist] = Instantiate( upGimmick );
                               gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist].transform.position = new Vector3( FIELD_SIZE/2+dist-i, gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist].transform.position.y, FIELD_SIZE/2+dist ); break;
            case State.DOWN:   gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist] = Instantiate( downGimmick );
                               gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist].transform.position = new Vector3( FIELD_SIZE/2+dist-i, gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist].transform.position.y, FIELD_SIZE/2+dist ); break;
            case State.LEFT:   gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist] = Instantiate( leftGimmick );
                               gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist].transform.position = new Vector3( FIELD_SIZE/2+dist-i, gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist].transform.position.y, FIELD_SIZE/2+dist ); break;
            case State.RIGHT:  gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist] = Instantiate( rightGimmick );
                               gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist].transform.position = new Vector3( FIELD_SIZE/2+dist-i, gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist].transform.position.y, FIELD_SIZE/2+dist ); break;
            case State.ROTATE: gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist] = Instantiate( rotateGimmick );
                               gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist].transform.position = new Vector3( FIELD_SIZE/2+dist-i, gimmicks[FIELD_SIZE/2+dist-i,FIELD_SIZE/2+dist].transform.position.y, FIELD_SIZE/2+dist ); break;
          }
        }
        for( int i = 1 ; i < 2 * dist + 1 ; i++ ){
          switch( field[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i] ){
            case State.BLACK:  stones[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i] = Instantiate( blackStone, new Vector3( FIELD_SIZE/2-dist, 0f, FIELD_SIZE/2+dist-i ), Quaternion.identity ); break;
            case State.WHITE:  stones[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i] = Instantiate( whiteStone, new Vector3( FIELD_SIZE/2-dist, 0f, FIELD_SIZE/2+dist-i ), Quaternion.identity ); break;
            case State.UP:     gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i] = Instantiate( upGimmick );
                               gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i].transform.position = new Vector3( FIELD_SIZE/2-dist, gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i].transform.position.y, FIELD_SIZE/2+dist-i ); break;
            case State.DOWN:   gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i] = Instantiate( downGimmick );
                               gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i].transform.position = new Vector3( FIELD_SIZE/2-dist, gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i].transform.position.y, FIELD_SIZE/2+dist-i ); break;
            case State.LEFT:   gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i] = Instantiate( leftGimmick );
                               gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i].transform.position = new Vector3( FIELD_SIZE/2-dist, gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i].transform.position.y, FIELD_SIZE/2+dist-i ); break;
            case State.RIGHT:  gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i] = Instantiate( rightGimmick );
                               gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i].transform.position = new Vector3( FIELD_SIZE/2-dist, gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i].transform.position.y, FIELD_SIZE/2+dist-i ); break;
            case State.ROTATE: gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i] = Instantiate( rotateGimmick );
                               gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i].transform.position = new Vector3( FIELD_SIZE/2-dist, gimmicks[FIELD_SIZE/2-dist,FIELD_SIZE/2+dist-i].transform.position.y, FIELD_SIZE/2+dist-i ); break;
          }
        }
      }

    }

    public void up(){

      for( int i = 0 ; i < FIELD_SIZE ; i++ ){
        if( stones[(int)x,i] != null )
          Destroy( stones[(int)x,i] );
        if( gimmicks[(int)x,i] != null )
          Destroy( gimmicks[(int)x,i] );
      }

      State s = field[(int)x,0];
      for( int i = 0 ; i < FIELD_SIZE-1 ; i++ )
        field[(int)x,i] = field[(int)x,i+1];
      field[(int)x,FIELD_SIZE-1] = s;

      for( int i = 0 ; i < FIELD_SIZE ; i++ ){
        switch( field[(int)x,i] ){
          case State.BLACK:  stones[(int)x,i] = Instantiate( blackStone, new Vector3( x, 0f, i ), Quaternion.identity ); break;
          case State.WHITE:  stones[(int)x,i] = Instantiate( whiteStone, new Vector3( x, 0f, i ), Quaternion.identity ); break;
          case State.UP:     gimmicks[(int)x,i] = Instantiate( upGimmick ); gimmicks[(int)x,i].transform.position = new Vector3( x, gimmicks[(int)x,i].transform.position.y, i ); break;
          case State.DOWN:   gimmicks[(int)x,i] = Instantiate( downGimmick ); gimmicks[(int)x,i].transform.position = new Vector3( x, gimmicks[(int)x,i].transform.position.y, i ); break;
          case State.LEFT:   gimmicks[(int)x,i] = Instantiate( leftGimmick ); gimmicks[(int)x,i].transform.position = new Vector3( x, gimmicks[(int)x,i].transform.position.y, i ); break;
          case State.RIGHT:  gimmicks[(int)x,i] = Instantiate( rightGimmick ); gimmicks[(int)x,i].transform.position = new Vector3( x, gimmicks[(int)x,i].transform.position.y, i ); break;
          case State.ROTATE: gimmicks[(int)x,i] = Instantiate( rotateGimmick ); gimmicks[(int)x,i].transform.position = new Vector3( x, gimmicks[(int)x,i].transform.position.y, i ); break;
        }
      }

    }

    public void down(){

      for( int i = 0 ; i < FIELD_SIZE ; i++ ){
        if( stones[(int)x,i] != null )
          Destroy( stones[(int)x,i] );
        if( gimmicks[(int)x,i] != null )
          Destroy( gimmicks[(int)x,i] );
      }

      State s = field[(int)x,FIELD_SIZE-1];
      for( int i = FIELD_SIZE-1 ; i > 0 ; i-- )
        field[(int)x,i] = field[(int)x,i-1];
      field[(int)x,0] = s;

      for( int i = 0 ; i < FIELD_SIZE ; i++ ){
        switch( field[(int)x,i] ){
          case State.BLACK:  stones[(int)x,i] = Instantiate( blackStone, new Vector3( x, 0f, i ), Quaternion.identity ); break;
          case State.WHITE:  stones[(int)x,i] = Instantiate( whiteStone, new Vector3( x, 0f, i ), Quaternion.identity ); break;
          case State.UP:     gimmicks[(int)x,i] = Instantiate( upGimmick ); gimmicks[(int)x,i].transform.position = new Vector3( x, gimmicks[(int)x,i].transform.position.y, i ); break;
          case State.DOWN:   gimmicks[(int)x,i] = Instantiate( downGimmick ); gimmicks[(int)x,i].transform.position = new Vector3( x, gimmicks[(int)x,i].transform.position.y, i ); break;
          case State.LEFT:   gimmicks[(int)x,i] = Instantiate( leftGimmick ); gimmicks[(int)x,i].transform.position = new Vector3( x, gimmicks[(int)x,i].transform.position.y, i ); break;
          case State.RIGHT:  gimmicks[(int)x,i] = Instantiate( rightGimmick ); gimmicks[(int)x,i].transform.position = new Vector3( x, gimmicks[(int)x,i].transform.position.y, i ); break;
          case State.ROTATE: gimmicks[(int)x,i] = Instantiate( rotateGimmick ); gimmicks[(int)x,i].transform.position = new Vector3( x, gimmicks[(int)x,i].transform.position.y, i ); break;
        }
      }

    }

    public void left(){

      for( int i = 0 ; i < FIELD_SIZE ; i++ ){
        if( stones[i,(int)z] != null )
          Destroy( stones[i,(int)z] );
        if( gimmicks[i,(int)z] != null )
          Destroy( gimmicks[i,(int)z]  );
      }

      State s = field[FIELD_SIZE-1,(int)x];
      for( int i = FIELD_SIZE-1 ; i > 0 ; i-- )
        field[i,(int)z] = field[i-1,(int)z];
      field[0,(int)z] = s;

      for( int i = 0 ; i < FIELD_SIZE ; i++ ){
        switch( field[i,(int)z] ){
          case State.BLACK:  stones[i,(int)z]  = Instantiate( blackStone, new Vector3( i, 0f, z ), Quaternion.identity ); break;
          case State.WHITE:  stones[i,(int)z]  = Instantiate( whiteStone, new Vector3( i, 0f, z ), Quaternion.identity ); break;
          case State.UP:     gimmicks[i,(int)z] = Instantiate( upGimmick ); gimmicks[i,(int)z].transform.position = new Vector3( i, gimmicks[i,(int)z].transform.position.y, z ); break;
          case State.DOWN:   gimmicks[i,(int)z] = Instantiate( downGimmick ); gimmicks[i,(int)z].transform.position = new Vector3( i, gimmicks[i,(int)z].transform.position.y, z ); break;
          case State.LEFT:   gimmicks[i,(int)z] = Instantiate( leftGimmick ); gimmicks[i,(int)z].transform.position = new Vector3( i, gimmicks[i,(int)z].transform.position.y, z ); break;
          case State.RIGHT:  gimmicks[i,(int)z] = Instantiate( rightGimmick ); gimmicks[i,(int)z].transform.position = new Vector3( i, gimmicks[i,(int)z].transform.position.y, z ); break;
          case State.ROTATE: gimmicks[i,(int)z] = Instantiate( rotateGimmick ); gimmicks[i,(int)z].transform.position = new Vector3( i, gimmicks[i,(int)z].transform.position.y, z ); break;
        }
      }

    }

    public void right(){

      for( int i = 0 ; i < FIELD_SIZE ; i++ ){
        if( stones[i,(int)z] != null )
          Destroy( stones[i,(int)z] );
        if( gimmicks[i,(int)z] != null )
          Destroy( gimmicks[i,(int)z]  );
      }

      State s = field[0,(int)x];
      for( int i = 0 ; i < FIELD_SIZE - 1 ; i++ )
        field[i,(int)z] = field[i+1,(int)z];
      field[FIELD_SIZE-1,(int)z] = s;

      for( int i = 0 ; i < FIELD_SIZE ; i++ ){
        switch( field[i,(int)z] ){
          case State.BLACK:  stones[i,(int)z]  = Instantiate( blackStone, new Vector3( i, 0f, z ), Quaternion.identity ); break;
          case State.WHITE:  stones[i,(int)z]  = Instantiate( whiteStone, new Vector3( i, 0f, z ), Quaternion.identity ); break;
          case State.UP:     gimmicks[i,(int)z] = Instantiate( upGimmick ); gimmicks[i,(int)z].transform.position = new Vector3( i, gimmicks[i,(int)z].transform.position.y, z ); break;
          case State.DOWN:   gimmicks[i,(int)z] = Instantiate( downGimmick ); gimmicks[i,(int)z].transform.position = new Vector3( i, gimmicks[i,(int)z].transform.position.y, z ); break;
          case State.LEFT:   gimmicks[i,(int)z] = Instantiate( leftGimmick ); gimmicks[i,(int)z].transform.position = new Vector3( i, gimmicks[i,(int)z].transform.position.y, z ); break;
          case State.RIGHT:  gimmicks[i,(int)z] = Instantiate( rightGimmick ); gimmicks[i,(int)z].transform.position = new Vector3( i, gimmicks[i,(int)z].transform.position.y, z ); break;
          case State.ROTATE: gimmicks[i,(int)z] = Instantiate( rotateGimmick ); gimmicks[i,(int)z].transform.position = new Vector3( i, gimmicks[i,(int)z].transform.position.y, z ); break;
        }
      }

    }

    public void openMenu(){
      sound();
      EnterMenu.SetActive( false );
      Menu.SetActive( true );
      flg = true;
    }

    public void closeMenu(){
      sound();
      EnterMenu.SetActive( true );
      Menu.SetActive( false );
      flg = false;
    }

    void put( int px, int pz ){
      flg = true;
      if( turn == BLACK ){
        Vector3 pos = blackHandRoot.transform.position;
        blackHandRoot.transform.position = new Vector3( -FIELD_SIZE + 1 - pos.x + px, pos.y, -FIELD_SIZE + 1 - pos.z + pz );
        blackHand.SetTrigger("Put");
      }
      if( turn == WHITE ){
        Vector3 pos = whiteHandRoot.transform.position;
        whiteHandRoot.transform.position = new Vector3( pos.x + px, pos.y, pos.z + pz );
        whiteHand.SetTrigger("Put");
      }
    }

    void lineCheck(){
      State s = State.BLACK;
      for( int i = 0 ; i < FIELD_SIZE ; i++ ){
        for( int j = 0 ; j < FIELD_SIZE ; j++ ){
          if( field[i,j] == s ){
            for( int k = 0 ; k < 8 ; k++ ){
              int _x = i, _z = j;
              for( int l = 0 ; l < 4 ; l++ ){
                _x += dx[k];
                _z += dz[k];
                if( _x < 0 || _x >= FIELD_SIZE || _z < 0 || _z >= FIELD_SIZE || field[_x,_z] != s )
                  break;
                if( l == 3 ){
                  Finish( BLACK );
                  return;
                }
              }
            }
          }
        }
      }
      s = State.WHITE;
      for( int i = 0 ; i < FIELD_SIZE ; i++ ){
        for( int j = 0 ; j < FIELD_SIZE ; j++ ){
          if( field[i,j] == s ){
            for( int k = 0 ; k < 8 ; k++ ){
              int _x = i, _z = j;
              for( int l = 0 ; l < 4 ; l++ ){
                _x += dx[k];
                _z += dz[k];
                if( _x < 0 || _x >= FIELD_SIZE || _z < 0 || _z >= FIELD_SIZE || field[_x,_z] != s )
                  break;
                if( l == 3 ){
                  Finish( WHITE );
                  return;
                }
              }
            }
          }
        }
      }
    }

    void Finish( bool winner ){
      BGMStop();
      finishSound();
      flg = true;
      Winner.text = ( winner == BLACK ? "BLACK" : "WHITE" );
      Win.SetActive( true );
    }

    void OnPlayableDirectorStopped( PlayableDirector director ){
      flg = false;
    }

    public void ToTitle(){
      sound();
      SceneManager.LoadScene( "Title" );
    }

    bool CanPut( int px, int pz ){
      return field[px,pz] != State.BLACK && field[px,pz] != State.WHITE;
    }

    void Update()
    {
      if( flg ){ dammyStoneDisp(); return; }
      time += Time.deltaTime * 5.0f;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      RaycastHit hit;
      if (Physics.Raycast(ray, out hit, 20.0f))
      {
        if( hit.collider.gameObject.CompareTag( "Field" ) ){
          x = Mathf.Round( hit.point.x );
          z = Mathf.Round( hit.point.z );
          if( (int)x >= 0 && (int)x < FIELD_SIZE && (int)z >= 0 && (int)z < FIELD_SIZE && CanPut( (int)x, (int)z ) ){
            dammyStoneDisp( true );
            if( Input.GetMouseButtonDown( 0 ) && CanPut( (int)x, (int)z ) )
              put( (int)x, (int)z );
          }
          else
            dammyStoneDisp();
        }
        else
          dammyStoneDisp();
      }
      dammyStoneMove();
    }

    void sound(){
      AudioSource audio = GetComponent<AudioSource>();
      if( audio != null )
        audio.PlayOneShot( soundClip );
    }

    void putSound(){
      AudioSource audio = GetComponent<AudioSource>();
      if( audio != null )
        audio.PlayOneShot( putClip );
    }

    void finishSound(){
      AudioSource audio = GetComponent<AudioSource>();
      if( audio != null )
        audio.PlayOneShot( finishClip );
    }

    void BGM(){
      AudioSource audio = GetComponent<AudioSource>();
      if( audio != null )
        audio.Play();
    }

    void BGMStop(){
      AudioSource audio = GetComponent<AudioSource>();
      if( audio != null )
        audio.Stop();
    }

}
