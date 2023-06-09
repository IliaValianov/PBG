using General;
using Ordering;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickUpItem : MonoBehaviour
{
    [SerializeField] Transform Point;
    public Transform PositionWithCamera;
    public Transform PositionWithPlayer;
    public Transform PositionWithHand;
    public Transform PositionWithHandAnother;
    public GameObject GameObject;
    public GameObject TakeBoxBtn;
    private Box _currentBox; 
    private List<Box> ListOfBoxesInPlayer = new();
    private Transform _lastposBox;
    public int RangeOfGetting;
    public Tool SampleOfWrench;
    public Tool SampleOfFlash;

    public bool IsEmpty => ListOfBoxesInPlayer.Count == 0;

    private void OnEnable()
    {
        GameEvents.Instance.Subscribe(GameEventType.OrderCompleted, ListOfBoxesInPlayer.Clear);
    }

    void Update()
    {
        
        Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mRay, out RaycastHit hitInfo))
        {
            //if (hitbox.GetComponent<Box>())
            //        hitbox.GetComponent<Outline>().OutlineWidth = 2;
            //if (!hitInfo.transform.GetComponent<Box>())
            //        hitbox.GetComponent<Outline>().OutlineWidth = 0;
            //hitbox = hitInfo.transform;
            if (Input.GetMouseButtonDown(0))
            {
                if (PlayerState.Instance.CurrentState == PlayerStates.Default)
                {
                    if (hitInfo.transform.TryGetComponent(out Box box))
                    {
                        if (UnityEngine.Vector3.Distance(hitInfo.transform.position, GetComponent<Transform>().position) < RangeOfGetting)
                        {
                            PlayerState.Instance.CurrentState = PlayerStates.PickedUpItem;
                            _currentBox = box;
                            _currentBox.GetComponent<Rigidbody>().isKinematic = true;
                            TpToCameraObjetc(hitInfo.transform);
                        }
                    }

                    if (hitInfo.transform.TryGetComponent(out Tool tool))
                    {
                        if (UnityEngine.Vector3.Distance(hitInfo.transform.position, GetComponent<Transform>().position) < RangeOfGetting && tool != GetComponent<Inventory>().InHands)
                        {
                            if (GetComponent<Inventory>().InHands != null)
                            {
                                try
                                {
                                    GetComponent<Inventory>().ListOfToolsInInventory.Remove(tool);
                                }
                                catch (System.Exception)
                                {
                                    
                                    throw;
                                }
                                GetComponent<Inventory>().ListOfToolsInInventory.Add(GetComponent<Inventory>().InHands);
                                GetComponent<Inventory>().InHands = tool;
                                hitInfo.transform.position = PositionWithHand.position;
                                hitInfo.transform.rotation = PositionWithHand.rotation;
                                hitInfo.transform.SetParent(GetComponent<Transform>());
                                //if(tool == SampleOfFlash)
                                //{
                                //    tool.Light.enabled = false;
                                //}
                                GetComponent<Inventory>().UpdateSlots();
                            }
                            else
                            {
                                GetComponent<Inventory>().InHands = tool;
                                hitInfo.transform.position = PositionWithHand.position;
                                hitInfo.transform.rotation = PositionWithHand.rotation;
                                hitInfo.transform.SetParent(GetComponent<Transform>());
                                //if(tool == SampleOfFlash)
                                //{
                                //    tool.Light.enabled = false;
                                //}
                                GetComponent<Inventory>().UpdateSlots();
                            }

                            //if (SampleOfWrench == tool)
                            //{
                            //    UnityEngine.Debug.Log("7");
                            //    GetComponent<Inventory>().InHands = tool;
                            //    hitInfo.transform.position = PositionWithHand.position;
                            //    hitInfo.transform.rotation = PositionWithHand.rotation;
                            //    hitInfo.transform.SetParent(GetComponent<Transform>());
                            //}
                            //UnityEngine.Debug.Log(SampleOfFlash == tool);
                            //if(SampleOfFlash == tool)
                            //{
                            //    UnityEngine.Debug.Log("8");
                            //    GetComponent<Inventory>().InHandsAnother = tool;
                            //    hitInfo.transform.position = PositionWithHandAnother.position;
                            //    hitInfo.transform.rotation = PositionWithHandAnother.rotation;
                            //    hitInfo.transform.SetParent(GetComponent<Transform>());
                            //}


                        }
                    }
                }
            }
        }
    }

    public void InHand()
    {
        return;
    }
    public void TpToCameraObjetc(Transform obj)
    {
        _lastposBox = _currentBox.transform;
        _currentBox.transform.SetParent(Camera.main.transform);
        _currentBox.transform.position = PositionWithCamera.position;
        TakeBoxBtn.SetActive(true);
        _currentBox.GetComponent<RotateObject>().enabled = true;
    } 
    public void TakeBox()
    {
        if(ListOfBoxesInPlayer.Count < Terminal.Instance.OrderBoxesCount)   
        {
            PlayerState.Instance.CurrentState = PlayerStates.Default;
            TakeBoxBtn.SetActive(false);
            _currentBox.transform.SetParent(GetComponent<Transform>());
            UnityEngine.Vector3 PlayerRotationn = new UnityEngine.Vector3(GetComponent<Transform>().rotation.x, GetComponent<Transform>().rotation.y, GetComponent<Transform>().rotation.z) + new UnityEngine.Vector3(-90, 90, 0);
            float hight = 0;
            foreach (var box in ListOfBoxesInPlayer)
            {
                hight += box.transform.GetComponent<Transform>().localScale.y / 50;
            }
            hight += _currentBox.transform.GetComponent<Transform>().localScale.y  / 50;
            _currentBox.transform.position = PositionWithPlayer.position + new UnityEngine.Vector3(0, hight, 0);
            _currentBox.transform.rotation =  PositionWithPlayer.rotation; //UnityEngine.Quaternion.Euler(PlayerRotationn.x, PlayerRotationn.y, PlayerRotationn.z);
            bool Wasnt = true;
            foreach (var boxx in ListOfBoxesInPlayer)
            {
                if (boxx == _currentBox)
                    Wasnt = false;
            }
            if (Wasnt)
            {
                ListOfBoxesInPlayer.Add(_currentBox);
                GetComponent<Inventory>().ListOfBoxesInPlayerIN.Add(_currentBox);
            }
        }
    }

    public void OutBox()
    {
        PlayerState.Instance.CurrentState = PlayerStates.Default;
        _currentBox.transform.SetParent(null);
        _currentBox.GetComponent<Rigidbody>().isKinematic = false;
        TakeBoxBtn.SetActive(false);
        Box box = ListOfBoxesInPlayer.SingleOrDefault(box => box == _currentBox);
        if (box != null)
        {
            
            ListOfBoxesInPlayer.Remove(box);
            GetComponent<Inventory>().ListOfBoxesInPlayerIN.Remove(box);
            float height = 0;
            foreach (var boxx in ListOfBoxesInPlayer)
            {
                height += boxx.transform.localScale.y / 50;
                boxx.transform.position = PositionWithPlayer.position + new Vector3(0, height, 0);
            }
        }
        _currentBox.transform.position = GetComponent<Transform>().position + new Vector3(1, 1, 1);
        _currentBox.transform.rotation = PositionWithPlayer.rotation;
        

        
    }

    public void GetOrderedBoxes(IEnumerable<BoxInfo> orderedBoxes, out IReadOnlyCollection<BoxInfo> givenBoxes)
    {
        List<BoxInfo> findedBoxes = new();

        foreach (BoxInfo orderedBox in orderedBoxes)
        {
            Box findedBox = ListOfBoxesInPlayer.FirstOrDefault(box => box.Info == orderedBox);

            if (findedBox != null)
            {
                findedBoxes.Add(findedBox.Info);
                findedBox.transform.SetParent(null);
                findedBox.gameObject.SetActive(false);
                ListOfBoxesInPlayer.Remove(findedBox);
            }
        }

        givenBoxes = findedBoxes;
    }

    private void OnDisable()
    {
        GameEvents.Instance.UnSubscribe(GameEventType.OrderCompleted, ListOfBoxesInPlayer.Clear);
    }
}
