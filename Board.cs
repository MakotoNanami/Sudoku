using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    private int[,] solvedGrid = new int[9, 9];
    public int shuffleNum;
    private string s;

    private int[,] riddleGrid = new int[9, 9];
    public int piecesToErase;

    public Transform A1, A2, A3, B1, B2, B3, C1, C2, C3;
    public GameObject buttonPrefab;

    public GameObject menu;
    public GameObject trigger;
    public Animator doorOpen;
    public GameObject puzzleCompletedUI;

    public AudioSource confirm;
    public AudioSource errorBeep;
    public AudioSource doorSound;
    public GameManager gm;


    //DIFFICULTY
    public enum Difficulties
    {
        DEBUG,
        EASY,
        MEDIUM,
        HARD,
        PLEASENO
    }

    public Difficulties difficulty;

    void Start()
    {
        InitGrid(ref solvedGrid);

        ShuffleGrid(ref solvedGrid, shuffleNum);
        CreateRiddleGrid();

        CreateButtons();
        gm = FindObjectOfType<GameManager>();    
    }

    public void InitGrid(ref int[,] grid)
    {
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                grid[i, j] = (i * 3 + i / 3 + j) % 9 + 1;
            }
        }

        int n1 = 8 * 3; //24
        int n2 = 8 / 3; //2
        int n = (n1 + n2 + 0) % 9 + 1;
        //print(n1 + "+" + n2 + "+" + 0);
        //print(n);
    }

    public void DebugGrid(ref int[,] grid)
    {
        s = "";
        int sep = 0;
        for (int i = 0; i < 9; i++)
        {
            s += "|";
            for(int j = 0; j < 9; j++)
            {
                s += grid[i, j].ToString();

                sep = j % 3;
                if(sep == 2)
                {
                    s += "|";
                }
            }
            s += "\n";
        }
        //print(s);
    }

    public void ShuffleGrid(ref int[,] grid, int shuffleAmount)
    {
        for(int i = 0; i < shuffleAmount; i++)
        {
            int value1 = Random.Range(1, 10);
            int value2 = Random.Range(1, 10);
            //MIX 2 CELLS
            MixTwoGridCells(ref grid, value1, value2);
        }
        DebugGrid(ref grid);
    }

    public void MixTwoGridCells(ref int[,] grid, int value1, int value2)
    {
        int x1 = 0;
        int x2 = 0;
        int y1 = 0;
        int y2 = 0;

        for(int i = 0; i < 9; i += 3)
        {
            for (int k = 0; k < 9; k += 3)
            {
                for(int j = 0; j < 3; j++)
                {
                    for(int l = 0; l < 3; l++)
                    {
                        if(grid[i+j, k+l] == value1)
                        {
                            x1 = i + j;
                            y1 = k + l;
                        }
                        if (grid[i + j, k + l] == value2)
                        {
                            x2 = i + j;
                            y2 = k + l;
                        }
                    }
                }
                grid[x1, y1] = value2;
                grid[x2, y2] = value1;
            }
        }
    }

    public void CreateRiddleGrid()
    {
        //COPY THE SOLVED GRID
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                riddleGrid[i, j] = solvedGrid[i, j];
            }
        }

        //SET DIFFICULTY
        SetDifficulty();

        //ERASE FROM RIDDLE GRID
        for (int i = 0; i < piecesToErase; i++)
        {
            int x1 = Random.Range(0, 9);
            int y1 = Random.Range(0, 9);
            //REROLL UNTIL WE FIND ONE WITHOUT A 0
            while (riddleGrid[x1, y1] == 0)
            {
                x1 = Random.Range(0, 9);
                y1 = Random.Range(0, 9);
            }
            //ONCE WE FOUND ONE WITH NO 0
            riddleGrid[x1, y1] = 0;
        }
        DebugGrid(ref riddleGrid);
    }

    public void CreateButtons()
    {
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                GameObject newButton = Instantiate(buttonPrefab);

                //SET ALL VALUE
                NumberField numField = newButton.GetComponent<NumberField>();
                numField.SetValues(i, j, riddleGrid[i, j], i + "," + j, this);
                newButton.name = i + "," + j;

                //PARENT THE BUTTON
                //A1
                if(i < 3 && j < 3)
                {
                    newButton.transform.SetParent(A1, false);
                }
                //A2
                if(i < 3 && j > 2 && j < 6)
                {
                    newButton.transform.SetParent(A2, false);
                }
                //A3
                if (i < 3 && j > 5)
                {
                    newButton.transform.SetParent(A3, false);
                }
                //B1
                if (i > 2 && i < 6 && j < 3)
                {
                    newButton.transform.SetParent(B1, false);
                }
                //B2
                if (i > 2 && i < 6 && j > 2 && j < 6)
                {
                    newButton.transform.SetParent(B2, false);
                }
                //B3
                if (i > 2 && i < 6 && j > 5)
                {
                    newButton.transform.SetParent(B3, false);
                }
                //C1
                if (i > 5 && j < 3)
                {
                    newButton.transform.SetParent(C1, false);
                }
                //C2
                if (i > 5 && j > 2 && j < 6)
                {
                    newButton.transform.SetParent(C2, false);
                }
                //C3
                if (i > 5 && j > 5)
                {
                    newButton.transform.SetParent(C3, false);
                }
            }
        }
    }

    public void SetInputInRiddleGrid(int x, int y, int value)
    {
        riddleGrid[x, y] = value;
    }

    private void SetDifficulty()
    {
        switch (difficulty)
        {
            case Difficulties.DEBUG:
                piecesToErase = 5;
                break;
            case Difficulties.EASY:
                piecesToErase = 10;
                break;
            case Difficulties.MEDIUM:
                piecesToErase = 15;
                break;
            case Difficulties.HARD:
                piecesToErase = 18;
                break;
            case Difficulties.PLEASENO:
                piecesToErase = 20;
                break;
        }
    }

    public void ConfirmComplete()
    {
        if (CheckIfCorrect())
        {
            //print("Yay!");
            confirm.Play();
            StartCoroutine(PuzzleCompletion());
        }
        else
        {
            print("No, try again.");
            errorBeep.Play();
        }
    }

    private bool CheckIfCorrect()
    {
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                if(riddleGrid[i, j] != solvedGrid[i, j])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void DisablePuzzle()
    {
        this.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        PlayerStats.ps.canMove = true;
    }

    public IEnumerator PuzzleCompletion()
    {
        gm.PuzzleEnd();
        puzzleCompletedUI.SetActive(true);
        yield return new WaitForSeconds(1f);
        puzzleCompletedUI.SetActive(false);
        menu.SetActive(false);
        trigger.SetActive(false);
        doorSound.Play();
        doorOpen.SetBool("Open", true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        PlayerStats.ps.canMove = true;
    }
}
