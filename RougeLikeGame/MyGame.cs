using RogueLib.Dungeon;
using RogueLib.Engine;
using RogueLib.Utilities;

namespace RlGameNS;


public class MyGame : Game
{

    private void init()
    {
        // To create a new game just need to 
        // 'inject' an IRenderWindow to draw the game one
        // 'inject' a Player, the player lives outside or the Scene's because the 
        // player visits all the scenes and takes their inventory with them. 
        // you must load the first leveel, and your level or your game must manage 
        // the level switching. 

        _window = new ScreenBuff(width, height);
        _player = new Rogue();

        var level1 = new Level(_player, map1, this);
        var level2 = new Level2(_player, map2, this);

        level1.Linked = level2;
        level2.Linked = level1;

        level1.InitPlayer(new Vector2(4, 12));

        _currentLevel = level1;

    }

    public MyGame()
    {
        // init level on construction 
        init();
    }


    // ----------------------------------------------------------------
    // string to use as the backgound on our first level
    // ----------------------------------------------------------------

    public const string map1 =
       """

               ┌──────┐          ┌─────────────┐
               │......│        ##+.............│            ┌───────┐
               │......│        # │.............+##          │......%│
               │......+######### └──────────+──┘ ###########+.......│                                                    
               │......│                     #               └───────┘
               └──+───┘                     #
           ########                 #########
      ┌────+┐                     ┌─+───────┐              ┌──────────────────┐
      │.....│                     │.........│              │..................│
      │.....+#####################+.........│              │..................│
      │.....│                     │.........│              │..................│
      │.....│                     │.........│              │..................│
      │....>│                     │.........+##############+..................│
      └─+───┘                     └───+─────┘              └───────────────+──┘
        #                             #                                    #
        ######               ┌────────+──────────────┐                     #
             #             ##+.......................|                     #
             #             # |.......................|   ###################
             #             # |.......................|   #
             #             # |.......................+####
             #             # └───────────────────────┘
             ###############
             
             
      """;

    public const string map2 =
         """

                           ┌─────────────┐
           ┌──────┐      ##+.............│                  ┌───────┐
           │......│  ##### │.............│     #############+......%│
           │......│###     │.............│     #            │.......│
           │......│        │.............│     #            └───────┘
           └──+───┘        │.............+######
             ##            │.............│     #
             #             │.............│     #
             #             │.............│     # ┌──────────┐
             #             └───+─────────┘     ##+..........│
             #                 #                 │..........│
           ###             #####                 │..........│
             ###     #######                     │..........│
             # #######     #                     └────+─────┘
           ###             #                          #
           #             ┌─+────────┐                 #
      ┌────+─────┐       │..........│                 #
      │..........│       │..........+##################
      │..........│       └──────────┘
      │..........│
      └──────────┘

      """;
}