using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManageCartas : MonoBehaviour
{
    public GameObject carta;                                            
    private bool primeiraCartaSelecionada, segundaCartaSelecionada;     
    private GameObject carta1, carta2;                                  
    private string linhaCarta1, linhaCarta2, linhaCarta3, LinhaCarta4;

    bool timerPausado, timerAcionado;
    float timer;

    int numTentativas = 0;
    int numAcertos = 0;

    public AudioClip somOk;
    public AudioClip somWin;

    AudioSource myAudioSource1;
    AudioSource myAudioSource2;

    int ultimoJogo = 0;
    int record = 0;
    int numeroJogos = 0;

    // Start is called before the first frame update
    void Start()
    {

        myAudioSource1 = AddAudio(false, false, 0.2f);
        myAudioSource2 = AddAudio(false, false, 0.3f);
        numeroJogos = PlayerPrefs.GetInt("numJogos", 0);

        if (numeroJogos == 0)
        {
            PlayerPrefs.SetInt("record", 0);
        }
        else if (numeroJogos >= 1)
        {
            int quebrouRecord = PlayerPrefs.GetInt("quebrouRecord", 0);
            if (quebrouRecord == 1){
                myAudioSource2.clip = somWin;
                myAudioSource2.Play();
                PlayerPrefs.SetInt("quebrouRecord", 0);
            }
            
        }
        MostraCartas();
        updateNumTentativas();
        ultimoJogo = PlayerPrefs.GetInt("Jogadas",0);
        record = PlayerPrefs.GetInt("record", 0);
        GameObject.Find("numJogadas").GetComponent<Text>().text = "Ultimo jogo = " + ultimoJogo;
        GameObject.Find("record").GetComponent<Text>().text = "Record = " + record;
    }

    public AudioSource AddAudio(bool loop, bool playAwake, float vol){
        AudioSource newAudio = gameObject.AddComponent<AudioSource>();
        newAudio.loop = loop;
        newAudio.playOnAwake = playAwake;
        newAudio.volume = vol;
        return newAudio;
    }

    // Update is called once per frame
    void Update()
    {
        if (timerAcionado)
        {
            timer += Time.deltaTime;
            print(timer);
            if (timer > 1)
            {
                timerPausado = true;
                timerAcionado = false;
                if(carta1.tag == carta2.tag)
                {
                    Destroy(carta1);
                    Destroy(carta2);
                    numAcertos++;
                    myAudioSource1.clip = somOk;
                    myAudioSource1.Play();
                    if (numAcertos == 13)
                    {
                        PlayerPrefs.SetInt("Jogadas", numTentativas);
                        if(numeroJogos == 0)
                        {
                            PlayerPrefs.SetInt("record", numTentativas);
                        }
                        else if (numTentativas < record)
                        {
                            PlayerPrefs.SetInt("quebrouRecord", 1);
                            PlayerPrefs.SetInt("record", numTentativas);
                        }
                        PlayerPrefs.SetInt("numJogos", +1);
                        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                    }
                }
                else
                {
                    carta1.GetComponent<Tile>().EscondeCarta();
                    carta2.GetComponent<Tile>().EscondeCarta();
                }
                primeiraCartaSelecionada = false;
                segundaCartaSelecionada = false;
                carta1 = null;
                carta2 = null;
                linhaCarta1 = "";
                linhaCarta2 = "";
                linhaCarta3 = "";
                LinhaCarta4 = "";
                timer = 0;
            }
        }
    }

    void MostraCartas()
    {
        int[] arrayEmbaralhado = criaArrayEmbaralhado();
        int[] arrayEmbaralhado2 = criaArrayEmbaralhado();
        int[] arrayEmbaralhado3 = criaArrayEmbaralhado();
        int[] arrayEmbaralhado4 = criaArrayEmbaralhado();
        for (int i = 0; i < 13; i++)
        {
            AddUmaCarta(0,i, arrayEmbaralhado[i]);
            AddUmaCarta(1, i, arrayEmbaralhado2[i]);
            AddUmaCarta(2, i, arrayEmbaralhado3[i]);
            AddUmaCarta(3, i, arrayEmbaralhado4[i]);
        }
    }

    void AddUmaCarta(int linha,int rank, int valor)
    {
        GameObject centro = GameObject.Find("CentroDaTela");
        float escalaCartaOriginal = carta.transform.localScale.x;
        float fatorEscalaX = ((650 * escalaCartaOriginal) / 110.0f);
        float fatorEscalaY = ((945 * escalaCartaOriginal) / 110.0f);
        Vector3 novaPosicao = new Vector3(centro.transform.position.x + ((rank - 13 / 2) * fatorEscalaX), centro.transform.position.y + ((linha-1) * fatorEscalaY), centro.transform.position.z);
        GameObject c = (GameObject)(Instantiate(carta, novaPosicao, Quaternion.identity));
        c.tag = "" + (valor );
        c.name = ""+linha+ "_" + valor;
        string nomeDaCarta = "";
        string numeroCarta = "";
        if (valor == 0)
        {
            numeroCarta = "ace";
        }
        else if (valor == 10)
        {
            numeroCarta = "jack";
        }
        else if (valor == 11)
        {
            numeroCarta = "queen";
        }
        else if (valor == 12)
        {
            numeroCarta = "king";
        }
        else
        {
            numeroCarta = "" + (valor + 1);
        }
        if (linha == 0)
        {
            nomeDaCarta = numeroCarta + "_of_hearts";
        }
        else if(linha == 1)
        {
            nomeDaCarta = numeroCarta + "_of_clubs";
        }
        else if(linha == 2)
        {
            nomeDaCarta = numeroCarta + "_of_diamonds";
        }
        else if(linha == 3)
        {
            nomeDaCarta = numeroCarta + "_of_spades";
        }
 
        Sprite s1 = (Sprite)(Resources.Load<Sprite>(nomeDaCarta));
        print("S1: " + s1);
        print(nomeDaCarta);
        GameObject.Find(""+linha+"_"+ valor).GetComponent<Tile>().setCartaOriginal(s1);
    }

    public int[] criaArrayEmbaralhado()
    {
        int[] novoArray = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
        int temp;
        for (int t = 0; t < 13; t++)
        {
            temp = novoArray[t];
            int r = Random.Range(t, 13);
            novoArray[t] = novoArray[r];
            novoArray[r] = temp;
        }
        return novoArray;
    }

    public void CartaSelecionada(GameObject carta)
    {
        if (!primeiraCartaSelecionada)
        {
            string linha = carta.name.Substring(0, 1);
            linhaCarta1 = linha;
            primeiraCartaSelecionada = true;
            carta1 = carta;
            carta1.GetComponent<Tile>().RevelaCarta();
        }
        else if(primeiraCartaSelecionada && !segundaCartaSelecionada)
        {
            string linha = carta.name.Substring(0, 1);
            linhaCarta2 = linha;
            segundaCartaSelecionada = true;
            carta2 = carta;
            carta2.GetComponent<Tile>().RevelaCarta();
            VerificaCartas();
        }
    }

    public void VerificaCartas()
    {
        DisparaTimer();
        numTentativas++;
        updateNumTentativas();
    }

    public void DisparaTimer()
    {
        timerPausado = false;
        timerAcionado = true;
    }

    void updateNumTentativas()
    {
        GameObject.Find("numTentativas").GetComponent<Text>().text = "Tentativas = " + numTentativas;
    }

}
