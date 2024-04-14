using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementBoard : MonoBehaviour
{
    public int Width;
    public int Height;

    public float SpacingX;
    public float SpacingY;

    public GameObject[] ElementPrefabs;

    private Tile[,] elementBoard;
    public GameObject ElementBoardGO;
    public GameObject ElementParent;

    [SerializeField] private Element selectedElement;
    [SerializeField] private bool isProcessingMove;
    public float processedDelay = 0.2f;
    public int maxIndex = 8;

    public List<GameObject> elementsToDestroy = new();
    public List<Element> connectedElements = new();
    public static ElementBoard Instance;

    public void Awake()
    {
        Instance = this;
    }

    public void Start()
    {
        InitializeBoard();
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

    private void InitializeBoard()
    {
        DestroyElements();
        elementBoard = new Tile[Width, Height];

        //What position will the board be on the camera?
        SpacingX = (float)(Width - 1) / 2;
        SpacingY = (float)(Height - 1) / 2;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                Vector2 pos = new Vector2(x - SpacingX, y - SpacingY);

                // elementBoard[x, y] = new Tile(false, null);
                int randomIndex = Random.Range(0, ElementPrefabs.Length);

                //After creating a random element, we set its position and make it usable.
                GameObject element = Instantiate(ElementPrefabs[randomIndex], pos, Quaternion.identity);
                element.transform.SetParent(ElementParent.transform);
                element.GetComponent<Element>().SetIndex(x, y);
                elementBoard[x, y] = new Tile(true, element);
                elementsToDestroy.Add(element);
            }
        }
        if (CheckBoard(false))
        {
            InitializeBoard();
        }
        else
        {
            Debug.Log("There are no matches");
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

    public bool CheckBoard(bool _takeAction)
    {
        bool hasMatched = false;

        List<Element> elementsToRemove = new();

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
                    Element element = elementBoard[x, y].element.GetComponent<Element>();

                    if (!element.isMatched)
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

        if (_takeAction)
        {
            foreach (Element elementToRemove in elementsToRemove)
            {
                elementToRemove.isMatched = false;
            }

            RemoveAndRefill(elementsToRemove);

            if (CheckBoard(false))
            {
                CheckBoard(true);
            }
        }


        return hasMatched;
    }

    private void RemoveAndRefill(List<Element> elementsToRemove)
    {
        foreach (Element element in elementsToRemove)
        {
            int _xIndex = element.xIndex;
            int _yIndex = element.yIndex;

            Destroy(element.gameObject);
            elementsToDestroy.Remove(element.gameObject);

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

    private void RefillElement(int x, int y)
    {
        int yOffset = 1;

        while (y + yOffset < Height && elementBoard[x, y + yOffset].element == null)
        {
            yOffset++;
        }

        if (y + yOffset < Height && elementBoard[x, y + yOffset].element != null)
        {
            Element elementAbove = elementBoard[x, y + yOffset].element.GetComponent<Element>();

            Vector3 targetPos = new Vector3(x - SpacingX, y - SpacingY, elementAbove.transform.position.z);

            elementAbove.MoveToTarget(targetPos);

            elementAbove.SetIndex(x, y);

            elementBoard[x, y] = elementBoard[x, y + yOffset];

            elementBoard[x, y + yOffset] = new Tile(transform, null);
        }

        if (y + yOffset == Height)
        {
            SpawnElementAtTop(x);
            Debug.Log("SpawnAtTop");
        }
    }

    private void SpawnElementAtTop(int x)
    {
        int index = FindIndexOfLowestNull(x);
        int locationMoveTo = maxIndex - index;

        int randomIndex = Random.Range(0, ElementPrefabs.Length);
        GameObject newElement = Instantiate(ElementPrefabs[randomIndex], new Vector2(x - SpacingX, Height - SpacingY), Quaternion.identity);
        newElement.transform.SetParent(ElementParent.transform);
        newElement.GetComponent<Element>().SetIndex(x, index);

        elementBoard[x, index] = new Tile(true, newElement);

        Vector3 targetPos = new Vector3(newElement.transform.position.x, newElement.transform.position.y - locationMoveTo, newElement.transform.position.z);
        newElement.GetComponent<Element>().MoveToTarget(targetPos);
    }

    private int FindIndexOfLowestNull(int x)
    {
        int lowestNull = 99;
        for (int y = 7; y >= 0; y--)
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
        
        ElementType elementType = element.elementType;

        connectedElements.Add(element);

        CheckDirection(element, new Vector2Int(1, 0), connectedElements);
        CheckDirection(element, new Vector2Int(-1, 0), connectedElements);

        if (connectedElements.Count == 3)
        {
            Debug.Log("Match Horizontal " + connectedElements[0].elementType);

            return new MatchResult
            {
                connectedElements = connectedElements,
                direction = MatchDirection.Horizontal
            };
        }
        else if (connectedElements.Count > 3)
        {
            Debug.Log("Match Long Horizontal " + connectedElements[0].elementType);

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

            return new MatchResult
            {
                connectedElements = connectedElements,
                direction = MatchDirection.Vertical
            };
        }
        else if (connectedElements.Count > 3)
        {
            Debug.Log("Match Long Vertical " + connectedElements[0].elementType);

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

    void CheckDirection(Element element, Vector2Int direction, List<Element> connectedElements)//If there is a match, find their positions and add them to the list.
    {
        ElementType elementType = element.elementType;

        int x = element.xIndex + direction.x;
        int y = element.yIndex + direction.y;

        while (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            if (elementBoard[x, y].isUsable)
            {
                Element neighbourPos = elementBoard[x, y].element.GetComponent<Element>();

                if (!neighbourPos.isMatched && neighbourPos.elementType == elementType)
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
    }

    #endregion

    #region Swapping Elements

    public void SelectElement(Element _element)
    {
        if (_element != null)
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
    }

    private void SwapElement(Element _currentElement, Element _targetElement)
    {
        if (!IsAdjacent(_currentElement, _targetElement))
        {
            return;
        }

        DoSwap(_currentElement, _targetElement);

        isProcessingMove = true;

        StartCoroutine(ProcessMatches(_currentElement, _targetElement));
    }

    private void DoSwap(Element _currentElement, Element _targetElement)
    {
        GameObject temp = elementBoard[_currentElement.xIndex, _currentElement.yIndex].element;

        elementBoard[_currentElement.xIndex, _currentElement.yIndex].element = elementBoard[_targetElement.xIndex, _targetElement.yIndex].element;
        elementBoard[_targetElement.xIndex, _targetElement.yIndex].element = temp;

        int tempXIndex = _currentElement.xIndex;
        int tempYIndex = _currentElement.yIndex;
        _currentElement.xIndex = _targetElement.xIndex;
        _currentElement.yIndex = _targetElement.yIndex;
        _targetElement.xIndex = tempXIndex;
        _targetElement.yIndex = tempYIndex;

        Debug.Log(_currentElement.xIndex + " " + _currentElement.yIndex
            + " " + _targetElement.xIndex + " " + _targetElement.yIndex);

        _currentElement.MoveToTarget(elementBoard[_targetElement.xIndex, _targetElement.yIndex].element.transform.position);
        _targetElement.MoveToTarget(elementBoard[_currentElement.xIndex, _currentElement.yIndex].element.transform.position);
    }

    private IEnumerator ProcessMatches(Element _currentElement, Element _targetElement)
    {
        yield return new WaitForSeconds(processedDelay);

        bool hasMatch = CheckBoard(false);

        if (!hasMatch)
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
    None
}

