using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBoard : MonoBehaviour
{
    [Header("Elements")]
    public GameObject[] ElementPrefabs;
    public GameObject ElementParent;
    public GameObject ElementBoardGO;
    [SerializeField] private Element selectedElement;

    [Header("BoardSettings")]
    public int Width;
    public int Height;

    public float SpacingX;
    public float SpacingY;

    private Tile[,] elementBoard;

    [Space(10)]

    public bool isProcessingMove;
    public int maxIndex = 8;
    public float durationSpeed;
    public float processedDelay = 0.2f;

    [Header("Boom")]
    public int boomScore;
    public float boomThreshold;

    [Space(10)]

    public List<GameObject> elementsToDestroy = new();
    List<Element> elementsToRemove = new List<Element>();

    public static ElementBoard Instance;

    public void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));

            if (hit.collider != null && hit.collider.gameObject.GetComponent<Element>())
            {
                if (isProcessingMove)
                    return;

                Element element = hit.collider.gameObject.GetComponent<Element>();

                SelectElement(element);
            }
        }
    }

    public void InitializeBoard(int rows, int columns)
    {
        Width = rows;
        Height = columns;
        maxIndex = Height;
        DestroyElements();
        elementBoard = new Tile[Width, Height];

        //What position will the board be on the camera?
        SpacingX = (float)(Width - 1) / 2;
        SpacingY = (float)(Height - 1) / 2-1;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Vector2 pos = new Vector2(x - SpacingX, y - SpacingY);

                elementBoard[x, y] = new Tile(false, null);

                int randomIndex;
                float randomValue = Random.value;

                if (randomValue < boomThreshold)
                {
                    randomIndex = ElementPrefabs.Length - 1; // index of last element(boom)
                }
                else
                {
                    randomIndex = Random.Range(0, ElementPrefabs.Length - 1); // except last element
                }

                GameObject element = Instantiate(ElementPrefabs[randomIndex], pos, Quaternion.identity);
                element.transform.SetParent(ElementParent.transform);
                element.GetComponent<Element>().SetIndex(x, y);

                elementBoard[x, y] = new Tile(true, element);
                elementsToDestroy.Add(element);
            }
        }

        if (CheckBoard())
        {
            InitializeBoard(Width, Height);
        }
        else
        {
            Debug.Log("There are no matches");
            GameManager1.Instance.score = 0;
        }
    }

    private void DestroyElements()
    {
        if (elementsToDestroy != null)
        {
            foreach (GameObject element in elementsToDestroy)
            {
                Destroy(element);
            }
            elementsToDestroy.Clear();
        }
    }

    public bool CheckBoard()
    {
        bool hasMatched = false;

        elementsToRemove.Clear();

        foreach (Tile tileElement in elementBoard)
        {
            if (tileElement.element != null)
            {
                tileElement.element.GetComponent<Element>().isMatched = false;
            }
        }


        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (elementBoard[x, y].isUsable)
                {
                    GameObject elementObj = elementBoard[x, y].element;

                    if (elementObj != null)
                    {
                        Element element = elementObj.GetComponent<Element>();

                        if (!element.isMatched && element != null)
                        {
                            MatchResult matchedElements = IsConnected(element);

                            if (matchedElements.connectedElements.Count >= 3)
                            {
                                MatchResult superMatchedPotions = SuperMatch(matchedElements);

                                elementsToRemove.AddRange(superMatchedPotions.connectedElements);

                                foreach (Element e in matchedElements.connectedElements)
                                    e.isMatched = true;

                                hasMatched = true;
                            }
                        }
                    }
                }
            }
        }
        return hasMatched;
    }

    public void BoardCleaning()
    {
        foreach  (Transform child in ElementParent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public IEnumerator ProcessTurnOnMatchedBoard(bool _subtractMoves)
    {
        foreach (Element potionToRemove in elementsToRemove)
        {
            potionToRemove.isMatched = false;
        }

        RemoveAndRefill(elementsToRemove);
        yield return new WaitForSeconds(0.4f);

        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchedBoard(false));
        }
    }

    private void RemoveAndRefill(List<Element> elementsToRemove)
    {
        foreach (Element element in elementsToRemove)
        {
            int _xIndex = element.xIndex;
            int _yIndex = element.yIndex;

            Destroy(element.gameObject);

            elementBoard[_xIndex, _yIndex] = new Tile(true, null);

        }

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (elementBoard[x, y].element == null)
                {
                    RefillElement(x, y);
                }
            }
        }
    }

    private void MoveElementToEmptySpace(int x, int y)
    {
        if (elementBoard[x, y].element != null)
        {
            int newX = x;
            int newY = y;

            while (elementBoard[newX, newY].element != null)
            {
                newX += 1;
                if (newX >= Width)
                {
                    newX = 0;
                    newY += 1;
                    if (newY >= Height)
                    {
                        newY = 0;
                    }
                }
            }

            elementBoard[newX, newY].element = elementBoard[x, y].element;
            elementBoard[x, y].element = null;

            elementBoard[newX, newY].element.transform.position = new Vector3(newX - SpacingX, newY - SpacingY, elementBoard[newX, newY].element.transform.position.z);
        }
    }

    private void RefillElement(int x, int y)
    {
        if (elementBoard[x, y].element != null)
        {
            MoveElementToEmptySpace(x, y);
            return;
        }

        int yOffset = 1;

        while (y + yOffset < Height && elementBoard[x, y + yOffset].element == null)
        {
            yOffset++;
        }

        if (y + yOffset < Height && elementBoard[x, y + yOffset].element != null)
        {
            //we've found a potion
            Element elementAbove = elementBoard[x, y + yOffset].element.GetComponent<Element>();

            //Move it to the correct location
            Vector3 targetPos = new Vector3(x - SpacingX, y - SpacingY, elementAbove.transform.position.z);

            elementAbove.MoveToTarget(targetPos, elementAbove, durationSpeed);

            elementAbove.SetIndex(x, y);

            //update our potionBoard
            elementBoard[x, y] = elementBoard[x, y + yOffset];

            elementBoard[x, y + yOffset] = new Tile(transform, null);
        }

        if (y + yOffset == Height)
        {
            SpawnElementAtTop(x);
        }
    }

    private void SpawnElementAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        int locationMoveTo = maxIndex - index;

        if (elementBoard[x, index].element == null)
        {
            int randomIndex;
            float randomValue = Random.value;

            if (randomValue < boomThreshold)
            {
                randomIndex = ElementPrefabs.Length - 1; // index of last element(boom)
            }
            else
            {
                randomIndex = Random.Range(0, ElementPrefabs.Length - 1); // except last element
            }

            GameObject newElementGO = Instantiate(ElementPrefabs[randomIndex], new Vector2(x - SpacingX, Height - SpacingY), Quaternion.identity);
            newElementGO.transform.SetParent(ElementParent.transform);
            Element newElement = newElementGO.GetComponent<Element>();
            newElement.SetIndex(x, index);

            elementBoard[x, index] = new Tile(true, newElementGO);

            Vector3 targetPos = new Vector3(newElementGO.transform.position.x, newElementGO.transform.position.y - locationMoveTo, newElementGO.transform.position.z);
            newElement.MoveToTarget(targetPos, newElement, durationSpeed);
        }
        else
        {
            Debug.LogWarning("SpawnElementAtTop: Tile at the top is already filled!");
        }
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;
        for (int y = Height - 1; y >= 0; y--)
        {
            if (elementBoard[x, y].element == null)
            {
                lowestNull = y;
            }
        }
        return lowestNull;
    }

    #region Matching Logic

    private MatchResult SuperMatch(MatchResult _matchedResults)
    {
        if (_matchedResults.direction == MatchDirection.Horizontal || _matchedResults.direction == MatchDirection.Vertical)
        {
            foreach (Element element in _matchedResults.connectedElements)
            {
                List<Element> extraConnectedElements = new();

                CheckDirection(element, new Vector2Int(0, 1), extraConnectedElements);
                CheckDirection(element, new Vector2Int(0, -1), extraConnectedElements);

                if (extraConnectedElements.Count >= 2)
                {
                    extraConnectedElements.AddRange(_matchedResults.connectedElements);

                    return new MatchResult
                    {
                        connectedElements = extraConnectedElements,
                        direction = MatchDirection.Super
                    };
                }
            }
            return new MatchResult
            {
                connectedElements = _matchedResults.connectedElements,
                direction = MatchDirection.Super
            };
        }
        else if (_matchedResults.direction == MatchDirection.LongHorizontal || _matchedResults.direction == MatchDirection.LongVertical)
        {
            foreach (Element element in _matchedResults.connectedElements)
            {
                List<Element> extraConnectedElements = new();

                CheckDirection(element, new Vector2Int(1, 0), extraConnectedElements);
                CheckDirection(element, new Vector2Int(-1, 0), extraConnectedElements);

                if (extraConnectedElements.Count >= 2)
                {
                    extraConnectedElements.AddRange(_matchedResults.connectedElements);

                    return new MatchResult
                    {
                        connectedElements = extraConnectedElements,
                        direction = MatchDirection.Super
                    };
                }
                return new MatchResult
                {
                    connectedElements = _matchedResults.connectedElements,
                    direction = _matchedResults.direction
                };
            }
        }
        return null;

    }

    MatchResult IsConnected(Element element)//Checking the number of neighbors and taking action accordingly
    {
        List<Element> connectedElements = new();
        ElementType elementType = element.elementType;
        int score = element.elementScore * LevelManager1.Instance.currentLevel;
        connectedElements.Add(element);

        CheckDirection(element, new Vector2Int(1, 0), connectedElements);
        CheckDirection(element, new Vector2Int(-1, 0), connectedElements);

        if (connectedElements.Count == 3)
        {
            Debug.Log("Match Horizontal " + connectedElements[0].elementType);
            GameManager1.Instance.score += score;
            GameManager1.Instance.ScoreSave();
            return new MatchResult
            {
                connectedElements = connectedElements,
                direction = MatchDirection.Horizontal
            };
        }
        else if (connectedElements.Count > 3)
        {
            Debug.Log("Match Long Horizontal " + connectedElements[0].elementType);
            GameManager1.Instance.score += score * 2;
            return new MatchResult
            {
                connectedElements = connectedElements,
                direction = MatchDirection.LongHorizontal
            };
        }

        connectedElements.Clear();
        connectedElements.Add(element);

        CheckDirection(element, new Vector2Int(0, 1), connectedElements);
        CheckDirection(element, new Vector2Int(0, -1), connectedElements);

        if (connectedElements.Count == 3)
        {
            Debug.Log("Match Vertical " + connectedElements[0].elementType);
            GameManager1.Instance.score += score;
            return new MatchResult
            {
                connectedElements = connectedElements,
                direction = MatchDirection.Vertical
            };
        }
        else if (connectedElements.Count > 3)
        {
            Debug.Log("Match Long Vertical " + connectedElements[0].elementType);
            GameManager1.Instance.score += score * 2;
            return new MatchResult
            {
                connectedElements = connectedElements,
                direction = MatchDirection.LongVertical
            };
        }
        else
        {
            return new MatchResult
            {
                connectedElements = connectedElements,
                direction = MatchDirection.None
            };
        }

    }

    void CheckDirection(Element element, Vector2Int direction, List<Element> connectedElements)
    {
        ElementType elementType = element.elementType;

        int x = element.xIndex + direction.x;
        int y = element.yIndex + direction.y;

        while (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            if (elementBoard[x, y].isUsable)
            {
                GameObject elementGameObject = elementBoard[x, y].element;
                if (elementGameObject != null)
                {
                    Element neighbourPos = elementGameObject.GetComponent<Element>();
                    if (neighbourPos != null && !neighbourPos.isMatched && neighbourPos.elementType == elementType)
                    {
                        connectedElements.Add(neighbourPos);
                        x += direction.x;
                        y += direction.y;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }
    }

    #endregion

    #region Swapping Elements

    public void SelectElement(Element _element)
    {
        if (selectedElement == null)
        {
            selectedElement = _element;
        }
        else if (selectedElement == _element)
        {
            selectedElement = null;
        }
        else if (selectedElement != null)
        {
            SwapElement(selectedElement, _element);
            selectedElement = null;
        }
    }

    private void SwapElement(Element _currentElement, Element _targetElement)
    {
        if (_currentElement == null || _targetElement == null || !IsAdjacent(_currentElement, _targetElement))
        {
            return;
        }

        DoSwap(_currentElement, _targetElement);

        isProcessingMove = true;

        StartCoroutine(GameManager1.Instance.SwapRightAmount());

        StartCoroutine(ProcessMatches(_currentElement, _targetElement));

        StartCoroutine(ExplodeAfterSwap(_currentElement, _targetElement));
    }

    private void DoSwap(Element _currentElement, Element _targetElement)
    {
        if (_currentElement == null || _targetElement == null)
        {
            Debug.LogWarning("DoSwap: _currentElement or _targetElement is null!");
            return;
        }
        if (_currentElement.xIndex == _targetElement.xIndex && _currentElement.yIndex == _targetElement.yIndex)
        {
            return;
        }

        GameObject temp = elementBoard[_currentElement.xIndex, _currentElement.yIndex].element;

        elementBoard[_currentElement.xIndex, _currentElement.yIndex].element = elementBoard[_targetElement.xIndex, _targetElement.yIndex].element;
        elementBoard[_targetElement.xIndex, _targetElement.yIndex].element = temp;

        //update indicies.
        int tempXIndex = _currentElement.xIndex;
        int tempYIndex = _currentElement.yIndex;
        _currentElement.xIndex = _targetElement.xIndex;
        _currentElement.yIndex = _targetElement.yIndex;
        _targetElement.xIndex = tempXIndex;
        _targetElement.yIndex = tempYIndex;

        _currentElement.MoveToTarget(elementBoard[_targetElement.xIndex, _targetElement.yIndex].element.transform.position, _currentElement, durationSpeed);
        _targetElement.MoveToTarget(elementBoard[_currentElement.xIndex, _currentElement.yIndex].element.transform.position, _targetElement, durationSpeed);
    }

    private IEnumerator ProcessMatches(Element _currentElement, Element _targetElement)
    {
        yield return new WaitForSeconds(processedDelay);

        if (CheckBoard())
        {
            StartCoroutine(ProcessTurnOnMatchedBoard(true));
        }
        else
        {
            DoSwap(_currentElement, _targetElement);
        }
        isProcessingMove = false;
    }

    private bool IsAdjacent(Element _currentElement, Element _targetElement)
    {
        return Mathf.Abs(_currentElement.xIndex - _targetElement.xIndex) + Mathf.Abs(_currentElement.yIndex - _targetElement.yIndex) == 1;
    }
    #endregion

    #region BOOM

    private void BoomExplode(int xIndex, int yIndex)
    {
        GameManager1.Instance.score += boomScore;

        // Explode elements in the same row
        for (int x = 0; x < Width; x++)
        {
            Element elementToExplode = elementBoard[x, yIndex].element?.GetComponent<Element>();
            if (elementToExplode != null && !elementToExplode.isMatched)
            {
                elementsToRemove.Add(elementToExplode);
            }
        }

        // Explode elements in the same column
        for (int y = 0; y < Height; y++)
        {
            Element elementToExplode = elementBoard[xIndex, y].element?.GetComponent<Element>();
            if (elementToExplode != null && !elementToExplode.isMatched)
            {
                elementsToRemove.Add(elementToExplode);
            }
        }

        StartCoroutine(ProcessTurnOnMatchedBoard(false));
    }

    private IEnumerator ExplodeAfterSwap(Element _currentElement, Element _targetElement)
    {
        yield return new WaitForSeconds(processedDelay); // Wait for a while after the swap process is completed

        // Explode rows and columns containing boom elements
        if (_currentElement.elementType == ElementType.Boom)
        {
            BoomExplode(_targetElement.xIndex, _targetElement.yIndex);
        }
        else if (_targetElement.elementType == ElementType.Boom)
        {
            BoomExplode(_currentElement.xIndex, _currentElement.yIndex);
        }
    }

    #endregion
}


public class MatchResult
{
    public List<Element> connectedElements;
    public MatchDirection direction;
}

public enum MatchDirection
{
    Vertical,
    Horizontal,
    LongVertical,
    LongHorizontal,
    Super,
    Boom,
    None
}
