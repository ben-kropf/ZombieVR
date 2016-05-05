using UnityEngine;

public class MazeDoor : MazePassage {

	private static Quaternion
		normalRotation = Quaternion.Euler(0f, -90f, 0f),
		mirroredRotation = Quaternion.Euler(0f, 90f, 0f);

	public Transform hinge;

	private bool isMirrored;
    private bool enter;
    private bool open;
    GameObject player;

    public GUISkin mySkin;

	private MazeDoor OtherSideOfDoor {
		get {
			return otherCell.GetEdge(direction.GetOpposite()) as MazeDoor;
		}
	}
   

    public void Start()
    {
        
        



    }
    public void Update()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if (distance < 2)
        {
            enter = true;
        }
        else
        {
            enter = false;
        }
        
        if(Input.GetKeyDown("e") && enter){
            open = !open;
        }
        if (open)
        {
            OtherSideOfDoor.hinge.localRotation = hinge.localRotation =
                isMirrored ? mirroredRotation : normalRotation;
        }
    }
    void OnGUI()
    {
        if (enter)
        {
            GUI.skin = mySkin;
            GUI.depth = 2;
            GUI.Label(new Rect(Screen.width / 2 - 75, Screen.height - 100, 150, 30), "Press 'E' to open the door");
            //GUI.Label(new Rect(Screen.width / 2 - 400, Screen.height - (Screen.height / 1.4f), 800, 100), "Press key <color=#88FF6AFF> << E >> </color> to open the door", mySkin.customStyles[1]);
        }
    }
    
	public override void Initialize (MazeCell primary, MazeCell other, MazeDirection direction) {
        GameObject[] temp;
        temp = GameObject.FindGameObjectsWithTag("Player");

        if (temp[0] != null)
        {
            player = temp[0];
        }
		base.Initialize(primary, other, direction);
		if (OtherSideOfDoor != null) {
			isMirrored = true;
			hinge.localScale = new Vector3(-1f, 1f, 1f);
			Vector3 p = hinge.localPosition;
			p.x = -p.x;
			hinge.localPosition = p;
		}
		for (int i = 0; i < transform.childCount; i++) {
			Transform child = transform.GetChild(i);
			if (child != hinge) {
				child.GetComponent<Renderer>().material = cell.room.settings.wallMaterial;
			}
		}
	}

	public override void OnPlayerEntered () {
		OtherSideOfDoor.hinge.localRotation = hinge.localRotation = isMirrored ? mirroredRotation : normalRotation;
	}
	
	public override void OnPlayerExited () {
		OtherSideOfDoor.hinge.localRotation = hinge.localRotation = Quaternion.identity;
	}
}