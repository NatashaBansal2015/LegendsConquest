using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public int damage;
    public int armor;
    public int healthMax;
    public Transform healthUI;

    public float moveRange;
    public float attackRange;

    private GameManager gameManager;


    public List<Transform> attack_piece = new List<Transform>();// record attackable piece
    public List<GameObject> attackSelectBtn = new List<GameObject>();// attack option

    public GameObject attackBtn;// attack button

    float reduceValue;// record how healthUI gonna change

    public GameObject timerSlider;//record time

    void Start()
    {
        reduceValue = transform.localScale.x / healthMax;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    private void Update()
    {
        if(healthMax <= 0)
        {
            Destroy(gameObject);// destroy piece when healthUI = 0
            gameManager.enemyPiece.Remove(GetComponent<Piece>());// remove piece from gamemanager
        }
        if (transform.position.x < -5.2f)//if arrive base
        {
            timerSlider.SetActive(true);//Show the capture time
        }
    }
    public void OnMouseDown()
    {
        for (int i = 0; i < gameManager.attackBtn.Count; i++)//close attack botton
        {
            gameManager.attackBtn[i].SetActive(false);
        }
        attack_piece.Clear();
        gameManager.selected = this.gameObject;
        gameManager.ShowMoveRange();
        for (int i = 0; i < gameManager.enemyPiece.Count; i++)//check all enemyPiece
        {
            if (Vector2.Distance(transform.position, gameManager.enemyPiece[i].transform.position) < attackRange)// if it's position inside attacke range
            {
                attack_piece.Add(gameManager.enemyPiece[i].transform);//record this piece
                attackBtn.SetActive(true);//attack UI active
            }
        }
    }
    public void SelectMethod()//select piece and show attck UI
    {
        for (int i = 0; i < attack_piece.Count; i++)
        {
            attackSelectBtn[i].SetActive(true);
        }
        gameManager.AttackMoveRange();
    }
    private GameObject piece;
    public void SelectAttackTarget(int index)
    {
        piece = attack_piece[index].gameObject;
        Attack();
    }
    public void Attack()
    {
        attackBtn.SetActive(false);//default is don't show the attack option
        for (int i = 0; i < attack_piece.Count; i++)
        {
            attackSelectBtn[i].SetActive(false);//default is don't show the attack button
        }
        piece.GetComponent<Piece>().Injured(damage);// be attcked adn reduce health
        gameManager.CloseMoveRange();
        piece = null;
    }
    public void Injured(int damage)
    {
        healthMax-= damage;
        if (healthMax <= 0)
        {
            return;
        }
        healthUI.localScale = new Vector3(healthUI.localScale.x - reduceValue*damage, 0.2f, 1);//change the health bar
    }
}
