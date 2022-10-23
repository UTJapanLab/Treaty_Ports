using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//Classes
    //Player class
    public class Player {
        public GameObject token;    //image or main representing object
        public string name;         //name of player
        public int id;              //player order id

        public int curr_pos = 0;    //current board position
        public HashSet<int> places_visited = new HashSet<int>();    //board spaces already visited
        public bool reverse_path = false;   //if player must go backwards
        public bool lose_a_turn = false;    //if player lost a turn

        //Player Constructor
        public Player(string player_name, int id, Canvas canvas) {
            name = player_name;
            this.id = id;

            //create token
            GameObject tok = new GameObject("player_" + id + "_token");
            //handle sizing and position
            RectTransform trans = tok.AddComponent<RectTransform>();
            trans.transform.SetParent(canvas.transform);
            //move position
            trans.localScale = Vector3.one;
            //size of token
            trans.sizeDelta = new Vector2(25, 25);
            //handle image
            Image image = tok.AddComponent<Image>();
            Texture2D tex = Resources.Load<Texture2D>("coin_" + id);
            image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            tok.transform.SetParent(canvas.transform);
            token = tok;
        }
    }

    //Boardspace class
    public class BoardSpace {
        public float x;     //center x value
        public float y;     //center y value
        public double w;    //width
        public double h;    //height
        public Queue<Player> players_on_me = new Queue<Player>(); //players on this space
        public bool rest_square = false; //if rest space or not
        public int[] fast_travels;       //fast travel spots
        //add which transition scene to use?

        //Boardspace Constructor
        public BoardSpace(float center_x, float center_y, double width, double height, bool rest, int[] fast) {
            x = center_x;
            y = center_y;
            w = width;
            h = height;
            rest_square = rest;
            fast_travels = fast;
        }
    }

public class main : MonoBehaviour
{

    /* Online References
      2D Dice Roll:
        https://www.youtube.com/watch?v=JgbJZdXDNtg&ab_channel=AlexanderZotov
    */

    //Unity Variables
    [SerializeField] Canvas canvas;
    [SerializeField] Scrollbar long_map_slider;

    //this is really jank but it works now
    //Fast Travel Arrays
    private static int[] no_fast = {};
    private static int[] space_two_fast = {0, 25, 0, 0, 0, 0};
    private static int[] space_four_fast = {0, 0, 0, 22, 0, 0};
    private static int[] space_six_fast = {7, 0, 10, 0, 27, 0};
    private static int[] space_seven_fast = {0, 10, 0, 15, 0, 27};
    private static int[] space_thirteen_fast = {22, 0, 0, 0, 0, 0};


    //Every single board space
    private static BoardSpace[] board = {
        new BoardSpace(220, -112, 112, 92, false, no_fast),
        new BoardSpace(133, -112, 88, 92, false, space_two_fast),
        new BoardSpace(44, -112, 115.5, 92, false, no_fast),
        new BoardSpace(-43, -112, 105, 92, false, space_four_fast),
        new BoardSpace(-127, -112, 85, 92, false, no_fast),
        new BoardSpace(-220, -112, 142, 92, false, space_six_fast),
        new BoardSpace(-241, -17, 93.5, 132, false, space_seven_fast),
        new BoardSpace(-241, 67, 83.5, 54.5, true, no_fast),
        new BoardSpace(-241, 114, 83.5, 84, false, no_fast),
        new BoardSpace(-241, 195, 74.5, 84, false, no_fast),
        new BoardSpace(-163, 195, 65, 84, true, no_fast),
        new BoardSpace(-95, 195, 65, 84, false, no_fast),
        new BoardSpace(-32, 195, 79, 84, false, space_thirteen_fast),
        new BoardSpace(39, 195, 88, 84, true, no_fast),
        new BoardSpace(116, 195, 82, 84, false, no_fast),
        new BoardSpace(199, 195, 82, 54.5, false, no_fast),
        new BoardSpace(199, 128, 82, 58, false, no_fast),
        new BoardSpace(199, 82, 82, 54, false, no_fast),
        new BoardSpace(199, 20, 82, 82.5, false, no_fast),
        new BoardSpace(199, -45, 82, 82.5, false, no_fast),
        new BoardSpace(119, -45, 70, 82.5, false, no_fast),
        new BoardSpace(44, -45, 64, 82.5, false, no_fast),
        new BoardSpace(-18, -45, 84, 84, false, no_fast),
        new BoardSpace(-93, -45, 82.5, 84, false, no_fast),
        new BoardSpace(-175, -45, 82.5, 51, false, no_fast),
        new BoardSpace(-175, 16, 82.5, 132, false, no_fast),
        new BoardSpace(-175, 108, 64, 86, false, no_fast),
        new BoardSpace(-102, 129, 82, 86, false, no_fast),
        new BoardSpace(-28, 129, 106.5, 83, false, no_fast),
        new BoardSpace(56, 129, 81, 113.5, false, no_fast),
        new BoardSpace(134, 101, 81, 70, false, no_fast),
        new BoardSpace(134, 17, 80, 70, false, no_fast),
        new BoardSpace(-20, 43, 308, 135, false, no_fast)
    };

    //Player Order and Information
    private static Player[] order = new Player[4];
    private static int curr_turn = 0;
    private Player player_one;
    private Player player_two;
    private Player player_three;
    private Player player_four;
    public static Player curr_player; 

    //flags
    public static bool dice_exit = false;
    public static bool next_scene = false;
    public static bool space_exit = false;
    private bool move = false;
    private bool fast_travel = false;

    //stuff for move
    int old_pos = 0;
    int next_pos = 0;
    int new_pos = 0;
    int square_pos = 0;
   

    // Start is called before the first frame update
    void Start() {

        long_map_slider.value = 1;

        //TODO temporary, take this out
        player_one = new Player("test_name", 1, canvas);
        player_two = new Player("test_name", 2, canvas);
        player_three = new Player("test_name", 3, canvas);
        player_four = new Player("test_name", 4, canvas);

        BoardSpace curr_square = board[0];

        //TODO ADD RANDOMIZE ORDER LATER
        order[0] = player_one;
        order[1] = player_two;
        order[2] = player_three;
        order[3] = player_four;

        curr_player = order[0];

        for (int i = 0; i < order.Length; i++) {
            curr_square.players_on_me.Enqueue(order[i]);
        }
       
        update_player_pos(curr_square);

        //load dice scene
        SceneManager.LoadSceneAsync(32, LoadSceneMode.Additive);
        
    }


    // Update is called once per frame
    void Update() {
        //once the dice scene is exited
        if (dice_exit) {
            dice_exit = false;
            int curr_roll = dice.get_roll();
            if (fast_travel) { //handles fast travel
                int curr_pos = curr_player.curr_pos;
                int space = board[curr_pos].fast_travels[curr_roll - 1];
                if (space != 0) {
                    curr_roll = (space - curr_pos) - 1;
                } else {
                    curr_roll = 0;
                }
                fast_travel = false;
            } 
            if (curr_roll != 0) {
                setup_move(curr_player, curr_roll);
                move = true;
            } else {
                next_roll();
            }
        }

        //once a token is able to move
        if (move) {
            move_func();
        }


        //once a board space scene has exited
        if (space_exit) {
            space_exit = false;
            if (fast_travel) {
                load_dice();
            } else {
                next_roll();
            }
        }
    }

    private void move_func() {
        Vector2 next_target;
        if (next_pos == new_pos) { //if next position is final, adjust the movement to the specific corner depending on number of players
            next_target = get_next_vector(board[next_pos], square_pos);
        } else { // else move to center of each space
            next_target = new Vector2(board[next_pos].x, board[next_pos].y);
        }
        curr_player.token.transform.localPosition = Vector2.MoveTowards(curr_player.token.transform.localPosition, 
            next_target, 200f * Time.deltaTime);
        if ((Vector2) curr_player.token.transform.localPosition == next_target) {
            move = false;
            if (next_pos == new_pos) { //reached target, load board space
                SceneManager.LoadSceneAsync(curr_player.curr_pos + 1, LoadSceneMode.Additive);
            } else { //reached an internal board space, delay the next move and update next and old position 
                old_pos = next_pos;
                next_pos = curr_player.reverse_path ? next_pos - 1 : next_pos + 1;
                Invoke("delay_move", .2f);
            }
        }
    }

    //Obtains the next vector given the next board space's coordinates and the current player's position on the target space.
    private Vector2 get_next_vector(BoardSpace space, int pos) {
        if (pos == 1) {
            return new Vector2(space.x - (float) (space.w / 5) , space.y + (float) (space.h / 5));
        } else if (pos == 2) {
            return new Vector2(space.x + (float) (space.w / 5) , space.y + (float) (space.h / 5));
        } else if (pos == 3) {
            return new Vector2(space.x - (float) (space.w / 5) , space.y - (float) (space.h / 5));
        } else if (pos == 4) {
            return new Vector2(space.x + (float) (space.w / 5) , space.y - (float) (space.h / 5));
        }
        return new Vector2(0, 0);
    }

    //Allow player to move again.
    private void delay_move() {
        move = true;
    }



    /*
        Given a board space, update the positions of every player on that board space.
    */
    static void update_player_pos(BoardSpace curr_square) {
        //TODO really gacky try and fix this
        Queue<Player> player_q = curr_square.players_on_me;
        int pos_count = 0;
        foreach (Player player in player_q) {
            GameObject player_token = player.token;
            if (pos_count == 0) {
                player_token.GetComponent<RectTransform>().localPosition = new Vector2(curr_square.x - (float) (curr_square.w / 5) , curr_square.y + (float) (curr_square.h / 5));
            } else if (pos_count == 1) {
                player_token.GetComponent<RectTransform>().localPosition = new Vector2(curr_square.x + (float) (curr_square.w / 5) , curr_square.y + (float) (curr_square.h / 5));
            } else if (pos_count == 2) {
                player_token.GetComponent<RectTransform>().localPosition = new Vector2(curr_square.x - (float) (curr_square.w / 5) , curr_square.y - (float) (curr_square.h / 5));
            } else if (pos_count == 3) {
                 player_token.GetComponent<RectTransform>().localPosition = new Vector2(curr_square.x + (float) (curr_square.w / 5) , curr_square.y - (float) (curr_square.h / 5));
            }
            pos_count++;
        }
    }

    //Sets up neccesary information about the player moving to the next location.
    private void setup_move(Player player, int move) {
        board[player.curr_pos].players_on_me.Dequeue(); //remove player off of current board space
        old_pos = player.curr_pos;
        next_pos = player.reverse_path ? player.curr_pos - 1 : player.curr_pos + 1;
        new_pos = player.reverse_path ? player.curr_pos -= move : player.curr_pos += move;
        if (new_pos >= 33) { //player reached center, must allow player to move back to the start 
            player.curr_pos = 32;
            player.reverse_path = true;
        } else if (new_pos <= 0) { //player has reached the end goal
            player.curr_pos = 0;
            //END GAME
        }
        Debug.Log("Player's Current Position On Board: " + (new_pos + 1));
        BoardSpace curr_square = board[new_pos];
        player.places_visited.Add(new_pos); //add board space to player's visited list
        if (curr_square.fast_travels.Length > 0 && !player.reverse_path) { //only allow fast travel on the way to Yokohama, not on the way back.
            Debug.Log("Fast Travel!");
            fast_travel = true;
        }
        curr_square.players_on_me.Enqueue(player); //add player onto new board space
        square_pos = curr_square.players_on_me.Count; //which corner of the new board space the player will land on
        if (curr_square.rest_square) { //check board space if player must lose turn 
            player.lose_a_turn = true;
        }
        player.curr_pos = new_pos;
    }

    //Gets the next player from the turn order and loads the dice.
    private void next_roll() {
        curr_turn++;
        if (curr_turn >= 4) {
            curr_turn = 0;
        }
        curr_player = order[curr_turn];     
         if (curr_player.lose_a_turn) { //Skip a player
            while(curr_player.lose_a_turn) { //Find a player that isn't resting.
                Debug.Log("You Rested!");
                curr_player.lose_a_turn = false; //If find a player that is resting, make them unrested.
                curr_turn++;
                if (curr_turn >= 4) {
                    curr_turn = 0;
                }
                curr_player = order[curr_turn];
            }
        }
        // Invoke("load_dice", .5f);   //Delays the loading of the dice by .5 seconds.
        load_dice();
    }

    //Loads the dice scene.
     private void load_dice() {
        SceneManager.LoadSceneAsync(32, LoadSceneMode.Additive);
    }

    //Set when the dice scene is exited in the dice class (dice.cs)
    public static void set_dice_exit(bool val) {
        dice_exit = val;
    }

    //Set when the board space scene is exited in the board space class (board_space.cs)
    //I just realized I named a class board_space and another class BoardSpace. Oops.
    public static void set_space_exit(bool val) {
        space_exit = val;
    }

}