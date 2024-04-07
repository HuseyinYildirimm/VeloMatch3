using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width = 6;
    public int height = 8;

    public float spacingX;
    public float spacingY;

    public GameObject[] mineralPrefabs;

    Tile[,] board;
    public GameObject boardObject;

    //public ArrayLayout array;
    public static Board Instance;
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        InitiializeBoard();
    }

    void InitiializeBoard()
    {
        board = new Tile[width,height];
        spacingX = (float)(width - 1) / 2;
        spacingY = (float)((height - 1) / 2) + 1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector2 position = new Vector2(x - spacingX, y - spacingY);

                int random = Random.Range(0, mineralPrefabs.Length);

                GameObject mineral = Instantiate(mineralPrefabs[random], position, Quaternion.identity);
                mineral.GetComponent<Mineral>().SetIndicies(x, y);
                board[x, y] = new Tile(true, mineral);
            
            }
        }

    }

}
