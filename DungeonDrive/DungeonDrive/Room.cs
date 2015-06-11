using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace DungeonDrive
{
    public class Room
    {

        Stopwatch watch = new Stopwatch();

        private GameState state;

        // Not the place you change the initial room. Change state.currentRoom
        public String currentRoom;

        public Dictionary<Item, PointF> droppedItems = new Dictionary<Item, PointF>();

        //////// IF YOU WANT TO DISABLE WALL BOUNDARIES TO TEST OTHER THINGS, SET noBoundaries TO TRUE ////////
        public bool noBoundaries = false;
        public bool noFogOfWar = false;

        public String environment = "dungeon";

        public int borderSize = 23;

        public int grassNum;

        public int width;
        public int height;
        public List<Obstacle> obstacles = new List<Obstacle>();
        public List<Unit> enemies = new List<Unit>();
        public List<Door> doors = new List<Door>();
        public List<Stairs> stairs = new List<Stairs>();
        public List<int> connectedRooms = new List<int>();

        public List<Stairs> stairsNotDrawn = new List<Stairs>();
        public List<Door> doorsNotDrawn = new List<Door>();
        public List<Unit> enemiesNotDrawn = new List<Unit>();
        public List<Obstacle> obstaclesNotDrawn = new List<Obstacle>();

        public bool[,] freeSpace;                      // something can be placed here (not in front of door, not enemy starting spot, not obstacle spot
        public bool[,] walkingSpace;                   // hero can walk here.
        public bool[,] stairSpace;                      // there are stairs in this space.
        public int[,] roomNumSpace;
        public bool[,] hallwaySpace;
        public bool[,] wallSpace;
        public bool[,] doorSpace;
        public bool[,] wallIntersection;
        public bool[,] drawingSpace;
        public bool[,] houseSpace;
        public bool[,] insideHouse;
        public bool[,] grassSpace;

        public int heroStartingX = 0;                           // where the hero is starting in the new room. Might be useless.
        public int heroStartingY = 0;

        public int numEnemies = 0;
        public int numBats = 0;
        public int numSnakes = 0;             // current number of each of these objects
        public int numSkeletons = 0;
        public int numSkeletalMages = 0;
        public int numGhosts = 0;
        public int numObstacles = 0;
        public int numStairs = 0;
        public int numRooms = 0;
        public int numChests = 0;
        public int numNonHallways = 0;
        public int numDoors = 0;

        public int caveSightFrequency = 3;
        public int caveCounter = 3;
        

        public const int minSizeOfInitRoom = 7;
        public const int maxSizeOfInitRoom = 13;


        public const int minSizeHallway = 2;
        public const int maxSizeHallway = 4;

        public int maxEnemies = 15;
        public int maxObstacles = 15;   // max number of these objects to generate in a room.
        public int maxStairs = 100;
        public int maxChests = 2;

        public int[] effectiveRoomNum;
        public bool[] roomDrawn;
        

        public const int minRoomWidth = 35;
        public const int minRoomHeight = 35;
        public const int maxRoomWidth = 300;
        public const int maxRoomHeight = 300;

        public const int safe_distance = 4;   // safe distance for the enemies to spawn from the player's starting position in the room.
        public double temp_sd = safe_distance;

        private Random rand;
        public Random stairsRand;
        private Bitmap floor1;
        private Bitmap wall1;
        private Bitmap floor2;
        private Bitmap wall2;

        public List<Projectile> projectiles = new List<Projectile>();
        public List<Projectile> deletingProj = new List<Projectile>();

        public Room(GameState state, string path)
        {
            this.state = state;
            generateRoom(path);
        }

        public void  generateRoom(string path)
        {

            watch.Start();

            state.pastRoom = state.currentRoom;
            state.currentRoom = path;
            currentRoom = path;

            rand = new Random(path.GetHashCode());  // random numbers based on path seed

            String[] files = Directory.GetFiles(path);  // get files in directory

            String[] dirs = Directory.GetDirectories(path); // get directories in current directory

            String parentDir = Path.GetDirectoryName(path);
            

            // base size of room on number of objects.


            ////////   GENERATE SIZES OF ROOMS    /////////

            int maxItems = Math.Min(maxEnemies + maxObstacles, files.Length);
            int maxMaxDirectories = Math.Min(dirs.Length, maxStairs) * 2;

            int widthBottom = (int) Math.Min(maxRoomWidth, minRoomWidth + (((rand.NextDouble() * .7) + .6) * (maxItems +maxMaxDirectories)));                // find the floor and ceiling of the height and width sizes
            int widthTop = (int) Math.Min(maxRoomWidth, widthBottom + ((rand.NextDouble() * .7) * (maxItems +maxMaxDirectories)));

            int heightBottom = (int) Math.Min(maxRoomHeight, minRoomHeight + (((rand.NextDouble() * .7) + .6) * (maxItems + maxMaxDirectories)));
            int heightTop = (int) Math.Min(maxRoomHeight, heightBottom + ((rand.NextDouble() * .7) * (maxItems + maxMaxDirectories)));

            //Console.WriteLine("Width between {0} and {1}, and Height between {2} and {3}", widthBottom, widthTop, heightBottom, heightTop);
            this.width = rand.Next(widthBottom, widthTop) ; // width is x-axis

            this.height = rand.Next(heightBottom, heightTop) ; // height is y-axis

            //Console.WriteLine("Width = {0} and Height = {1}", width, height);

            ////////   INIT THE ARRAYS   /////////

            freeSpace = new bool [width,height];
            walkingSpace = new bool [width, height];
            stairSpace = new bool[width, height];
            roomNumSpace = new int[width, height];
            hallwaySpace = new bool[width, height];
            wallSpace = new bool[width, height];
            doorSpace = new bool[width, height];
            wallIntersection = new bool[width, height];
            effectiveRoomNum = new int[2 * maxStairs];
            roomDrawn = new bool[2 * maxStairs];
            drawingSpace = new bool[width, height];
            houseSpace = new bool[width, height];
            insideHouse = new bool[width, height];
            grassSpace = new bool[width, height];
            
            /*
            projectiles.Clear();
            deletingProj.Clear();

            obstacles.Clear();
            enemies.Clear();
            doors.Clear();
            stairs.Clear();
            connectedRooms.Clear();

            stairsNotDrawn.Clear();
            doorsNotDrawn.Clear();
            enemiesNotDrawn.Clear();
            obstaclesNotDrawn.Clear();
            */

            //Console.WriteLine("1");

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    freeSpace[i, j] = true;
                    walkingSpace[i,j] = true;         // initalizes all the arrays
                    stairSpace[i, j] = false;
                    roomNumSpace[i, j] = -1;
                    hallwaySpace[i, j] = false;
                    wallSpace[i, j] = false;
                    doorSpace[i, j] = false;
                    wallIntersection[i, j] = false;
                    drawingSpace[i, j] = false;
                    houseSpace[i, j] = false;
                    insideHouse[i, j] = false;
                    grassSpace[i, j] = false;
                }
            }

            for (int i = 0; i < 2 * maxStairs; i++)
            {
                effectiveRoomNum[i] = i;
                roomDrawn[i] = false;
            }

            //Console.WriteLine("2");

                //////////   ADD STAIR UP TO PARENT UNLESS IN C: DIRECTORY ///////

            //Console.WriteLine("Here0");

                environment = state.allLevelInfo.getEnvironmentType(currentRoom);

            /////////   TRAVERSE ALL DIRECTORIES   ///////

                if (environment.Equals("courtyard"))
                {

                    // get directories
                    generateCourtyard();

                    updateFreeSpace();

                    bool heroPlaced = false;
                    
                    foreach (Stairs stair in stairsNotDrawn)
                    {
                        if (stair.path.Equals(state.pastRoom))
                        {
                            state.hero.x = /*state.hero.xNext = */stair.x + stair.xDirection + 0.5;      // place you on the correct side of it
                            state.hero.y = /*state.hero.yNext = */stair.y + stair.yDirection + 0.5;
                            heroPlaced = true;
                            break;
                        }
                    }

                    if (!heroPlaced)
                    {
                        Console.WriteLine("Hero has not been placed");
                        {
                            int x1, y1;
                            while (grassSpace[x1 = rand.Next(0, width - 1), y1 = rand.Next(0, height)] || wallSpace[x1, y1])
                            {
                                //Console.WriteLine("loop3");
                            }
                            
                            state.hero.x = x1 + 0.5;
                            state.hero.y = y1 + 0.5;
                            freeSpace[x1, y1] = false;
                            heroPlaced = true;
                        }
                    }

                    //Console.WriteLine("Here2");

                    recalcRoomNums();

                    addDoorsToCourtyard();

                    updateGrassSpace();


                }
                else
                {

                    for (int i = 0; i < dirs.Length; i++)
                    {
                        if (!((File.GetAttributes(dirs[i]) & FileAttributes.Hidden) == FileAttributes.Hidden))
                        {              // checks to make sure the directory is not hidden

                            try
                            {
                                bool temp2 = Directory.Exists(dirs[i]);
                                String[] tempStrings = Directory.GetDirectories(dirs[i]);                                               // this should throw an error is the directory is inaccessible       
                                //Console.WriteLine(dirs[i] + " found");
                                directoryFound(dirs[i]);
                            }
                            catch (Exception)
                            {
                                //Console.WriteLine("{0}", e.ToString());
                            }
                        }
                        else
                        {
                            //Console.WriteLine("Found Hidden File " + dirs[i]);
                            try
                            {
                                //hiddenFound(dirs[i]);
                            }
                            catch (Exception)
                            {
                                //Console.WriteLine("{0}", e.ToString());
                            }
                            //hiddenFound(dirs[i]);

                            // found hidden file
                        }
                    }

                   // Console.WriteLine("3");

                    //Console.WriteLine("Here1");

                    /////////// IF THIS IS THE INITIAL ROOM, 

                    updateFreeSpace();

                    /////////   FIND STAIRCASE YOU ARE COMING FROM   /////////

                   // Console.WriteLine("4");

                    addStairs(false, parentDir);

                    foreach (Stairs stair in stairsNotDrawn)
                    {
                        if (stair.path.Equals(state.pastRoom))
                        {
                            if (state.pastRoom.Equals("C:\\"))
                            {
                                String s = state.allLevelInfo.getOppositeDir(state.allLevelInfo.getDirection(currentRoom));
                                switch (s)
                                {
                                    case "up":
                                        state.hero.x = stair.x + .5;
                                        state.hero.y = stair.y - .5;
                                        break;
                                    case "down":
                                        state.hero.x = stair.x + .5;
                                        state.hero.y = stair.y + 1.5;
                                        break;
                                    case "left":
                                        state.hero.x = stair.x - .5;
                                        state.hero.y = stair.y + .5;
                                        break;
                                    case "right":
                                        state.hero.x = stair.x + 1.5;
                                        state.hero.y = stair.y + .5;
                                        break;
                                }
                            }
                            else
                            {

                            }
                            state.hero.x = /*state.hero.xNext = */stair.x + stair.xDirection + 0.5;      // place you on the correct side of it
                            state.hero.y = /*state.hero.yNext = */stair.y + stair.yDirection + 0.5;
                            break;
                        }
                    }

                    //Console.WriteLine("5");

                    //Console.WriteLine("Here2");

                    recalcRoomNums();

                    /////////  CONNECT ROOMS WITH HALLWAYS   ////

                    // look through wallIntersection and try to add doors without hallways.

                    connectTouchingRooms();


                    List<Stairs>[] roomStairs = new List<Stairs>[numRooms];

                    for (int i = 0; i < numRooms; i++)
                    {
                        roomStairs[i] = new List<Stairs>();
                    }

                    foreach (Stairs tempStair in stairsNotDrawn)
                    {
                        //Console.WriteLine("Stair room number = " + tempStair.roomNum);
                        roomStairs[tempStair.roomNum].Add(tempStair);                   // have an array of lists of stairs sorted by room numbers
                    }

                    int staticNumRooms = numRooms;

                    int breaker = 0;

                    //Console.WriteLine("6");

                    while (true)
                    {

                        int count = 0;
                        foreach (Stairs stair in roomStairs[staticNumRooms - 1])
                        {
                            count++;
                        }
                        if (count == numStairs || breaker > 100)
                        {
                            //Console.WriteLine("Iteration: "+breaker+" for hallway generation");
                            breaker++;
                            break;
                        }

                        //Console.WriteLine("Iteration: " + breaker + " for hallway generation");

                        for (int i = 0; i < staticNumRooms; i++)              // go through each room
                        {


                            double shortestDistance = width + height;       // want to find the shortest distance between any stair in this room and any stair not in this room.
                            Stairs source = new Stairs(state, -1, -1, -1, -1, -1, false, "", 'u', -1, -1, -1, -1, false);                                // stair that the hallway must start from
                            Stairs closestStair = new Stairs(state, -1, -1, -1, -1, -1, false, "", 'u', -1, -1, -1, -1, false);
                            int roomDest = -1;



                            foreach (Stairs stair in roomStairs[i])
                            {             // evaluate every stair in the current room

                                for (int j = 0; j < staticNumRooms; j++)              // go through each other room and find the closest stair
                                {
                                    if (i == j) continue;                       // skip current room number, because you are trying to find the shortest distance from stair in room i, to stair in room j.

                                    foreach (Stairs stair2 in roomStairs[j])
                                    {
                                        double newDistance;
                                        if ((newDistance = distanceBtwnPts(stair.centerX, stair.centerY, stair2.centerX, stair2.centerY)) < shortestDistance)           // if the distance between the stairs is smaller than any other one found til this point, update new shortestStair and distance
                                        {
                                            roomDest = j;
                                            shortestDistance = newDistance;
                                            source = stair;
                                            closestStair = stair2;
                                        }

                                    }
                                }

                            }


                            // closestStair should be accurate
                            if (source.x != -1 && closestStair.y != -1)         // if a close stair has been found
                            {
                                //Console.WriteLine("Making Hallway");
                                //makeHallway(source.x, source.y, closestStair.x, closestStair.y, Math.Min(source.maxHallwayWidth, closestStair.maxHallwayWidth));
                                makeHallway(source.centerX, source.centerY, closestStair.centerX, closestStair.centerY, Math.Min(source.maxHallwayWidth, closestStair.maxHallwayWidth));
                                // make a hallway between the two.

                                if (roomDest > i)                       // move all the stairs from the lower room, into the higher room
                                {
                                    foreach (Stairs removeStair in roomStairs[i])
                                    {
                                        roomStairs[roomDest].Add(removeStair);
                                    }

                                    roomStairs[i].Clear();

                                }
                                else
                                {
                                    foreach (Stairs removeStair in roomStairs[roomDest])
                                    {
                                        roomStairs[i].Add(removeStair);
                                    }

                                    roomStairs[roomDest].Clear();

                                }


                            }



                        }

                        //Console.WriteLine(breaker);
                        breaker++;
                    }

                }

            //Console.WriteLine("Out1");

                if (currentRoom.Equals(state.pastRoom))
                {

                    //Console.WriteLine("InitialRoom");
                    int x1, y1;
                    while (roomNumSpace[x1 = rand.Next(0, width - 1), y1 = rand.Next(0, height)] == -1 || wallSpace[x1, y1])
                    {
                        //Console.WriteLine("Hanging");
                    }

                    state.hero.x = x1 + 0.5;
                    state.hero.y = y1 + 0.5;
                    freeSpace[x1, y1] = false;
                }

                //Console.WriteLine("7");
            updateFreeSpace();

                //////////   TRAVERSE ALL FILES   //////////

            //Console.WriteLine("Number of files = " + files.Length);

                for (int i = 0; i < files.Length; i++)
                {
                    //Console.WriteLine("File - " + i);
                    matchExtension(Path.GetExtension(files[i]), Path.GetFileName(files[i]));     // match each file extension and spawn the corresponding object
                }

            // determine hero starting point
            // find stair that matches the pastRoom

                //Console.WriteLine("Out2");

            if (!noBoundaries)
            {
                addBoundaries();
            }
            recalcRoomNums();

/*            
            // load the levelInfo
            if (state.allLevelInfo.levelAlreadyExists(currentRoom))
            {
                Console.WriteLine("Trying to load file "+currentRoom);
                state.allLevelInfo.loadLevel(currentRoom);
            }
            else
            {
                Console.WriteLine(currentRoom + " does not exist yet");
                state.allLevelInfo.addLevel(new LevelInfo(state, currentRoom, false));
            }
*/
            //Console.WriteLine("8");

            if (noFogOfWar)
            {
                eliminateFogOfWar();
            }

            if (environment.Equals("dungeon"))
            {
                floor1 = new Bitmap(Properties.Resources.floor);
                wall1 = new Bitmap(Properties.Resources.wall);
                floor2 = new Bitmap(Properties.Resources.grass);
                if (!state.loadingGame)
                {
                    updateDrawingGrid(roomNumSpace[(int)state.hero.x, (int)state.hero.y]);
                }
            }
            else if (environment.Equals("cave"))
            {
                removeDoors();
                floor1 = new Bitmap(Properties.Resources.caveFloor);
                wall1 = new Bitmap(Properties.Resources.caveWall);
                floor2 = new Bitmap(Properties.Resources.grass);
                makeAllObjectsEligible();
                if (!state.loadingGame)
                {
                    updateHeroVisibility();
                }
            }
            else if (environment.Equals("courtyard"))
            {
                floor1 = new Bitmap(Properties.Resources.grass);
                wall1 = new Bitmap(Properties.Resources.caveTop);
                floor2 = new Bitmap(Properties.Resources.floor);
                wall2 = new Bitmap(Properties.Resources.wall);
                if (!state.loadingGame)
                {
                    updateDrawingGrid(roomNumSpace[(int) state.hero.x,(int)state.hero.y]);
                }
               
            }

            //Console.WriteLine("9");

            watch.Stop();
            Console.WriteLine("Time it took to generate level = "+ (watch.ElapsedMilliseconds / 1000.0 ));

            // Console.WriteLine("Out3");
        }

        public void matchExtension(String extension, String filename)
        {
            switch(extension){

             //* Text Files
                    case ".txt":
                    case ".rtf":
                    case ".doc":
                    case ".docx":
                        textFound();
                        break; //*/

                    //* audio file
                    case ".mp3":
                    case ".m4a":
                    case ".wav":
                    case ".wma":
                        audioFound();
                        break; //*/

                    //* video files
                    case ".avi":
                    case ".m4v":
                    case ".mov":
                    case ".mp4":
                    case ".mpg":
                    case ".wmv":
                        videoFound();
                        break; //*/

                    //*Image files
                    case ".jpg":
                    case ".jpeg":
                    case ".bmp":
                    case ".gif":
                    case ".png":
                    case ".pdf":
                        imageFound();
                        break; //*/   
/*
                    // Powerpoint
                    case ".ppt":
                    case ".pptx":
                    case ".pps":
                        otherFound();
                        break;
  
                    // Spreadsheet Files
                    case ".xlr":
                    case ".xls":
                    case ".xlsx":
                        otherFound();
                        break;

                    // Executable files
                    case ".exe":
                    case ".j":
                        otherFound();
                        break;

                    // Web files
                    case ".html":
                    case ".htm":
                    case ".css":
                    case ".js":
                    case ".php":
                    case ".xhtml":
                        otherFound();
                        break;

                    // Developer files
                    case ".c":
                    case ".class":
                    case ".cpp":
                    case ".cs":
                    case ".h":
                    case ".java":
                    case ".m":
                    case ".pl":
                    case ".py":
                    case ".sh":
                    case ".sln":
                        otherFound();
                        break;
 
                    // Torrent files
                    case ".torrent":
                        otherFound();
                        break;
 //*/           
                    /*//Compressed files
                    case ".7z":
                    case ".gz":
                    case ".rar":
                    case ".tar.gz":
                    case ".zip":
                    case ".zipx":
                        otherFound(filename);
                        break;*/

                    // Other file
                    default:
                        otherFound(filename, extension);
                        break;
                }
        }

        public void updateGrassSpace(){
            int minX = borderSize + 1;
            int maxX = width - borderSize - 1;
            int minY = borderSize + 1;
            int maxY = height - borderSize - 1;

            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if(roomNumSpace[i,j] == 0)
                        grassSpace[i, j] = true;

                }
            }
        }

        public void generateCourtyard()
        {

            updateHouseSpace();
            int minX = borderSize;
            int maxX = width - borderSize;
            int minY = borderSize;
            int maxY = height - borderSize;

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i > minX && i < maxX && j > minY && j < maxY)
                    {
                        roomNumSpace[i, j] = numRooms;
                        grassSpace[i, j] = true;
                    }
                    else
                    {
                        walkingSpace[i, j] = false;
                        houseSpace[i, j] = false;
                        //freeSpace[i, j] = false;
                    }
                }
            }

            grassNum = numRooms;

            numRooms++;

            String[] dirs = Directory.GetDirectories("C:\\"); // get directories in current directory

            for (int a = 0; a < dirs.Length; a++)
            {
                if (!((File.GetAttributes(dirs[a]) & FileAttributes.Hidden) == FileAttributes.Hidden))
                {              // checks to make sure the directory is not hidden

                    try
                    {
                        bool temp2 = Directory.Exists(dirs[a]);
                        String[] tempStrings = Directory.GetDirectories(dirs[a]);                                               // this should throw an error is the directory is inaccessible       
                        //Console.WriteLine(dirs[i] + " found");

                        String env = state.allLevelInfo.getEnvironmentType(dirs[a]);
                        if (env.Equals("cave"))
                        {
                            String direction = state.allLevelInfo.getDirection(dirs[a]);
                            Stairs newStairs;

                            int x, y;

                            switch (direction)
                            {

                                case "up":


                                    do
                                    {
                                        //Console.WriteLine("Stuck please help1");
                                        x = (int)rand.Next(minX + 2, maxX - 2);
                                    } while (!freeSpace[x, minY]);

                                    newStairs = new Stairs(state, x, minY, 2, 1, grassNum, true, dirs[a], 's', 0, 0, 0, numStairs, true);
                                    newStairs.setCaveDoor();
                                    stairSpace[newStairs.x + 1, newStairs.y] = true;
                                    stairSpace[newStairs.x, newStairs.y] = true;
                                    roomNumSpace[newStairs.x, newStairs.y] = grassNum;
                                    stairsNotDrawn.Add(newStairs);

                                    break;
                                case "down":

                                    do
                                    {
                                        //Console.WriteLine("Stuck please help2");
                                        x = (int)rand.Next(minX + 2, maxX - 2);

                                    } while (!freeSpace[x, maxY]);

                                    newStairs = new Stairs(state, x, maxY, 2, 1, grassNum, true, dirs[a], 'w', 0, 0, 0, numStairs, true);
                                    newStairs.setCaveDoor();
                                    stairSpace[newStairs.x, newStairs.y] = true;
                                    stairSpace[newStairs.x + 1, newStairs.y] = true;
                                    roomNumSpace[newStairs.x, newStairs.y] = grassNum;
                                    stairsNotDrawn.Add(newStairs);
                                    break;

                                case "left":

                                    do
                                    {
                                        //Console.WriteLine("Stuck please help3");
                                        y = (int)rand.Next(minY + 2, maxY - 2);
                                    } while (!freeSpace[minX, y]);

                                    newStairs = new Stairs(state, minX, y, 1, 2, grassNum, true, dirs[a], 'd', 0, 0, 0, numStairs, true);
                                    newStairs.setCaveDoor();
                                    stairSpace[newStairs.x, newStairs.y] = true;
                                    stairSpace[newStairs.x, newStairs.y + 1] = true;
                                    roomNumSpace[newStairs.x, newStairs.y] = grassNum;
                                    stairsNotDrawn.Add(newStairs);
                                    break;

                                case "right":

                                    do
                                    {
                                        //Console.WriteLine("Stuck please help4");
                                        y = (int)rand.Next(minY + 2, maxY - 2);
                                    } while (!freeSpace[maxX, y]);

                                    newStairs = new Stairs(state, maxX, y, 1, 2, grassNum, true, dirs[a], 'a', 0, 0, 0, numStairs, true);
                                    newStairs.setCaveDoor();
                                    stairSpace[newStairs.x, newStairs.y + 1] = true;
                                    stairSpace[newStairs.x, newStairs.y] = true;
                                    roomNumSpace[newStairs.x, newStairs.y] = grassNum;
                                    stairsNotDrawn.Add(newStairs);
                                    break;
                            }

                            numStairs++;
                            updateStairSpace();
                        }
                        else
                        {
                            //Console.WriteLine("Making an internal room");

                            //find a point on the map, that is within houseSpace

                            int x, y;

                            do
                            {
                                x = rand.Next(borderSize + 6, width - (borderSize + 6));
                                y = rand.Next(borderSize + 6, height - (borderSize + 6));
                            } while (!houseSpace[x, y]);

                            int roomHeight = minSizeOfInitRoom;
                            int roomWidth = minSizeOfInitRoom;

                            int maxRandom = (int)roomWidth * roomHeight - (2 * (roomWidth + roomHeight)) + 2;   // indicates how many non-border cells there are. We don't want the stairs to be on a border to avoid adjacent stair tiles.

                            int stairLocation = rand.Next(0, maxRandom);
                            int counter = 0;
                            int stairX = x;
                            int stairY = y;


                            //Console.WriteLine("Here1.5");

                            for (int i = -1; i <= roomWidth; i++)
                            {
                                for (int j = -1; j <= roomHeight; j++)
                                {

                                    int cX = x + i - roomWidth;
                                    int cY = y + j - roomHeight;

                                    if (i == -1 || i == roomWidth || j == -1 || j == roomHeight)
                                    {
                                        //*
                                        if (roomNumSpace[cX, cY] != -1 && roomNumSpace[cX,cY] != grassNum && effectiveRoomNum[roomNumSpace[cX, cY]] != effectiveRoomNum[numRooms] && !wallSpace[cX, cY])
                                        {
                                            //int highVal = Math.Max(effectiveRoomNum[roomNumSpace[cX, cY]], effectiveRoomNum[numRooms]);
                                            //int lowVal = Math.Min(effectiveRoomNum[roomNumSpace[cX, cY]], effectiveRoomNum[numRooms]);

                                            int highVal = numRooms;
                                            int lowVal = effectiveRoomNum[roomNumSpace[cX, cY]];

                                            for (int k = 0; k < numRooms; k++)
                                            {
                                                if (effectiveRoomNum[k] == lowVal)
                                                {
                                                    effectiveRoomNum[k] = highVal;
                                                }

                                            }
                                        }
                                        //*/
                                        if (roomNumSpace[cX, cY] == -1 || roomNumSpace[cX,cY] == grassNum)
                                        {
                                            wallSpace[cX, cY] = true;

                                        }

                                        roomNumSpace[cX, cY] = numRooms;

                                    }
                                    else
                                    {
                                        insideHouse[cX, cY] = true;
                                        if (wallSpace[cX, cY])
                                        {
                                            wallSpace[cX, cY] = false;
                                        }

                                        if (i != 0 && i != roomWidth - 1 && j != 0 && j != roomHeight - 1)
                                        {
                                            // a non-border cell.
                                            if (counter == stairLocation)
                                            {

                                                stairX = cX;
                                                stairY = cY;

                                                //wallSpace[stairX, stairY] = true;

                                                int minimumDistToWall = 10;
                                                minimumDistToWall = Math.Min(minimumDistToWall, i);
                                                minimumDistToWall = Math.Min(minimumDistToWall, j);
                                                minimumDistToWall = Math.Min(minimumDistToWall, (roomWidth - 1) - i);
                                                minimumDistToWall = Math.Min(minimumDistToWall, (roomHeight - 1) - j);

                                            }
                                            counter++;
                                        }
                                        roomNumSpace[cX, cY] = numRooms;
                                        //mergeRoom((x - widthRadiusInitRoom) + i, (y - heightRadiusInitRoom) + j, numRooms); 

                                    }


                                }

                            }

                            stairsNotDrawn.Add(new Stairs(state, stairX, stairY, 1, 1, roomNumSpace[stairX, stairY], true, dirs[a], 'd', 2, (x + 1) - (roomWidth / 4), (y + 1) - (roomHeight / 4), numStairs, false));

                            //Console.WriteLine("StairX = " + stairX + " stairY = " + stairY + " x = " + x + " y = " + y + " width = " + widthOfInitStairRoom + " height = " + heightOfInitStairRoom);

                            //Console.WriteLine("Difference of x and centerX = " + (stairX - (x - (widthOfInitStairRoom / 2))) + "Difference of y and centerY = " + (stairY - (y - (heightOfInitStairRoom / 2))));

                            stairSpace[stairX, stairY] = true;

                            for (int i = -1; i <= 1; i++)
                            {
                                for (int j = -1; j <= 1; j++)
                                {
                                    freeSpace[stairX + i, stairY + j] = false;
                                }
                            }

                            //freeSpace[stairX, stairY] = false;
                            //freeSpace[stairX + xDirFromChar(direction), stairY + yDirFromChar(direction)] = false;

                            numRooms++;
                            numStairs++;
                            numNonHallways++;

                            // add stairs?
                        }
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine("{0}", e.ToString());
                    }
                }
                else
                {
                    //Console.WriteLine("Found Hidden File " + dirs[i]);
                    try
                    {
                        //hiddenFound(dirs[i]);
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine("{0}", e.ToString());
                    }
                    //hiddenFound(dirs[i]);

                    // found hidden file
                }
            }

            // place cave entrances around perimeter


            //place grassy field
            
            //place dungeon rooms, make them different room number
            // add doors to the rooms

            // place graveyard


        }

        public void updateHouseSpace()
        {
            int count = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i > (borderSize + maxSizeOfInitRoom) && i < (width - (borderSize)) && j > (borderSize + maxSizeOfInitRoom) && j < (height - (borderSize)))
                    {
                        count++;
                        houseSpace[i, j] = true;
                    }
                }
            }

            //Console.WriteLine("percentage of non=house space = " + (double) (count / (width * height)));
        }

        public void updateStairSpace()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (stairSpace[i, j])
                    {
                        for (int k = -3; k <= 3; k++)
                        {
                            for (int l = -3; l <= 3; l++)
                            {
                                freeSpace[i + k, j + l] = false;
                            }
                        }
                    }
                }
            }
        }

        public void directoryFound(String path)
        {
            // WORK IN PROGRESS

            addStairs(true, path) ;
        }

        public void textFound()
        {
            addObstacle("pillar");
        }

        public void audioFound()
        {
            addObstacle("chest");
        }

        public void videoFound()
        {
            addObstacle("chest");
        }

        public void imageFound()
        {
            addObstacle("chest");
        }

        public void hiddenFound(String filename)
        {
            temp_sd = safe_distance;
            while (!addEnemy(new Ghost(state, rand.Next(0, width - 1) + 0.5, rand.Next(0, height - 1) + 0.5), filename)) ;
            numGhosts++;

        }

        public void otherFound(String filename, String extension)
        {
            temp_sd = safe_distance;
            int random = (int)rand.Next(0, 100);
            if(random <= 20){
                while (!addEnemy(new Bat(state, rand.Next(0, width - 1) + 0.5, rand.Next(0, height - 1) + 0.5), filename)) ;
                numBats++;

            }
            else if (random <= 40)
            {
                while (!addEnemy(new Skeleton(state, rand.Next(0, width - 1) + 0.5, rand.Next(0, height - 1) + 0.5, false), filename)) ;
                numSkeletons++;
            }
            else if (random <= 60)
            {
                while (!addEnemy(new Snake(state, rand.Next(0, width - 1) + 0.5, rand.Next(0, height - 1) + 0.5), filename)) ;
                numSnakes++;
            }
            else if (random <= 80)
            {
                while (!addEnemy(new Ghost(state, rand.Next(0, width - 1) + 0.5, rand.Next(0, height - 1) + 0.5), filename)) ;
                numGhosts++;
            }
            else
            {
                while (!addEnemy(new SkeletalMage(state, rand.Next(0, width - 1) + 0.5, rand.Next(0, height - 1) + 0.5, false), filename)) ;
                numSkeletalMages++;
            }
            if (extension.Equals(".exe"))
            {
                while (!addEnemy(new Boss(state, rand.Next(0, width - 1) + 0.5, rand.Next(0, height - 1) + 0.5), filename)) ;
            }
            
        }

        public void addDoor()
        {

        }

        public void addDoorsToCourtyard()
        {
            bool[] roomExists = new bool[numRooms];

            foreach (Stairs stair in stairsNotDrawn)
            {
                roomExists[roomNumSpace[stair.x, stair.y]] = true;
            }

            for (int i = 0; i < roomExists.Length; i++)
            {
                bool doorFound = false;
                if (roomExists[i])
                {
                    for (int j = 0; j < width; j++)
                    {
                        for (int k = 0; k < height; k++)
                        {
                            if (!doorFound && roomNumSpace[j, k] == i && wallSpace[j,k])
                            {
                                if(roomNumSpace[j + 1,k] == i && wallSpace[j + 1,k]){
                                    //horizontal door
                                    if(wallSpace[j,k - 1] || wallSpace[j,k+1] || wallSpace[j+1,k+1] || wallSpace[j+1,k-1]){

                                    } else {
                                        //Console.WriteLine("Adding door1");
                                        doorFound = true;
                                        doorsNotDrawn.Add(new Door(state,j,k,2,1,roomNumSpace[j,k],false,0,0,false,numDoors++));
                                        doorSpace[j, k] = true;
                                        doorSpace[j + 1, k] = true;
                                        wallSpace[j, k] = false;
                                        wallSpace[j + 1, k] = false;
                                        // good horizontal door location.
                                    }
                                } else if(roomNumSpace[j,k+1] == i && wallSpace[j,k+1]){

                                    // vertical door
                                    if (wallSpace[j + 1, k] || wallSpace[j - 1, k] || wallSpace[j + 1, k + 1] || wallSpace[j - 1, k + 1])
                                    {

                                    }
                                    else
                                    {
                                        //Console.WriteLine("Adding door1");
                                        doorFound = true;
                                        doorsNotDrawn.Add(new Door(state, j, k, 1, 2, roomNumSpace[j, k], true, 0, 0, false, numDoors++));
                                        doorSpace[j, k] = true;
                                        doorSpace[j, k + 1] = true;
                                        wallSpace[j, k] = false;
                                        wallSpace[j, k + 1] = false;
                                    }
                                }
                              
                            }

                        }
                    }

                }
            }
        }

        public void addStairs(bool down, String path)
        {

            //Console.WriteLine("Adding stairs for " + path);

            if (numStairs >= (maxStairs - 1))
            {
                return;
            }

            char direction;

                if (down){
                    direction = 'd';
                } else {
                    direction = 'a';
                }

                if (down)
                {
                    stairsRand = new Random(string.Concat(currentRoom, path).GetHashCode());
                }
                else
                {
                    stairsRand = new Random(string.Concat(path, currentRoom).GetHashCode());
                }

                switch ((int)stairsRand.Next(0, 4))
                {
                    case 0:
                        if (down) {
                            direction = 'w';
                        } else {
                            direction = 's';
                        }
                        break;
                    case 1:
                        if (down) {
                            direction = 'd';
                        } else {
                            direction = 'a';
                        }
                        break;
                    case 2:
                        if (down) {
                            direction = 's';
                        } else {
                            direction = 'w';
                        }
                        break;
                    case 3:
                        if (down) {
                            direction = 'a';
                        } else {
                            direction = 'd';
                        }
                        break;
                }

            int x;
            int y; 
            int tHeight = 1;
            int tWidth = 1;

            int maxHallwayWidth = 2;

            int heightOfInitStairRoom = rand.Next(minSizeOfInitRoom, maxSizeOfInitRoom + 1);
            int widthOfInitStairRoom = rand.Next(minSizeOfInitRoom, maxSizeOfInitRoom + 1);
            int heightRadiusInitRoom = (int)((heightOfInitStairRoom) / 2);
            int widthRadiusInitRoom = (int)((widthOfInitStairRoom) / 2);

            //Console.WriteLine("Here1");

            do
            {
                x = rand.Next(2 + widthRadiusInitRoom, width - 4 - widthRadiusInitRoom);
                y = rand.Next(2 + heightRadiusInitRoom, height - 4 - heightRadiusInitRoom);
            } while (!freeSpace[x, y] || !freeSpace[x + xDirFromChar(direction), y + yDirFromChar(direction)] || roomNumSpace[x, y] != -1);


            int maxRandom = (int) widthOfInitStairRoom * heightOfInitStairRoom - (2 * (widthOfInitStairRoom + heightOfInitStairRoom)) + 2;   // indicates how many non-border cells there are. We don't want the stairs to be on a border to avoid adjacent stair tiles.

            int stairLocation = rand.Next(0, maxRandom);
            int counter = 0;
            int stairX  = x;
            int stairY = y;


            //Console.WriteLine("Here1.5");

            for (int i = -1; i <= widthOfInitStairRoom; i++)
            {
                for (int j = -1; j <= heightOfInitStairRoom; j++)
                {

                    int cX = x + i - widthRadiusInitRoom;
                    int cY = y + j - heightRadiusInitRoom;

                    if (i == -1 || i == widthOfInitStairRoom || j == -1 || j == heightOfInitStairRoom)
                    {
                        //*
                        if (roomNumSpace[cX, cY] != -1 && effectiveRoomNum[roomNumSpace[cX, cY]] != effectiveRoomNum[numRooms] && !wallSpace[cX,cY])
                        {
                            //int highVal = Math.Max(effectiveRoomNum[roomNumSpace[cX, cY]], effectiveRoomNum[numRooms]);
                            //int lowVal = Math.Min(effectiveRoomNum[roomNumSpace[cX, cY]], effectiveRoomNum[numRooms]);

                            int highVal = numRooms;
                            int lowVal = effectiveRoomNum[roomNumSpace[cX,cY]];

                            for (int k = 0; k < numRooms; k++)
                            {
                                if (effectiveRoomNum[k] == lowVal)
                                {
                                    effectiveRoomNum[k] = highVal;
                                }

                            }
                        }
                        //*/
                        if (roomNumSpace[cX, cY] == -1)
                        {
                            wallSpace[cX, cY] = true;
                            
                        }

                        roomNumSpace[cX, cY] = numRooms;

                    } else{

                        if (wallSpace[cX, cY])
                        {
                            wallSpace[cX, cY] = false;
                        }

                        if (i != 0 && i != widthOfInitStairRoom - 1 && j != 0 && j != heightOfInitStairRoom - 1)
                        {
                            // a non-border cell.
                            if (counter == stairLocation)
                            {

                                stairX = cX;
                                stairY = cY;

                                //wallSpace[stairX, stairY] = true;

                                int minimumDistToWall = 10;
                                minimumDistToWall = Math.Min(minimumDistToWall, i);
                                minimumDistToWall = Math.Min(minimumDistToWall, j);
                                minimumDistToWall = Math.Min(minimumDistToWall, (widthOfInitStairRoom - 1) - i);
                                minimumDistToWall = Math.Min(minimumDistToWall, (heightOfInitStairRoom - 1) - j);

                                maxHallwayWidth = 2 * minimumDistToWall;

                            }
                            counter++;
                        }
                        roomNumSpace[cX, cY] = numRooms;
                        //mergeRoom((x - widthRadiusInitRoom) + i, (y - heightRadiusInitRoom) + j, numRooms); 
                        
                    }
                    
                   
                }
            }

            //Console.WriteLine("Here2");


            bool ladder = false;

            if(state.allLevelInfo.getEnvironmentType(currentRoom).Equals("cave")){
                ladder = true;
            }
            if (!state.startTutorial)
            {
                stairsNotDrawn.Add(new Stairs(state, stairX, stairY, tWidth, tHeight, roomNumSpace[stairX, stairY], down, path, direction, maxHallwayWidth, (x + 1) - (widthOfInitStairRoom / 4), (y + 1) - (heightOfInitStairRoom / 4), numStairs, ladder));
            }

            //Console.WriteLine("StairX = " + stairX + " stairY = " + stairY + " x = " + x + " y = " + y + " width = " + widthOfInitStairRoom + " height = " + heightOfInitStairRoom);

            //Console.WriteLine("Difference of x and centerX = " + (stairX - (x - (widthOfInitStairRoom / 2))) + "Difference of y and centerY = " + (stairY - (y - (heightOfInitStairRoom / 2))));

            stairSpace[stairX, stairY] = true;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    freeSpace[stairX + i,stairY + j] = false;
                }
            }

            //freeSpace[stairX, stairY] = false;
            //freeSpace[stairX + xDirFromChar(direction), stairY + yDirFromChar(direction)] = false;

            numRooms++;
            numStairs++;
            numNonHallways++;

            //Console.WriteLine("ENd of add stairs");

            return;

        }

        public void addObstacle(String type)
        {
           
            if (numObstacles >= (maxObstacles - 1))
            {
                return;

            }

            int oWidth = 1;
            int oHeight = 1;

            switch (type)
            {
                case "pillar":
                    oWidth = 1;
                    oHeight = 1;
                    break;

                case "chest":
                    if (numChests >= maxChests)
                    {
                        if (numObstacles >= (maxObstacles - 1))
                        {
                            return;
                        }
                        else
                        {
                            type = "pillar";
                        }
                        
                    }
                    oWidth = 1;
                    oHeight = 1;
                    break;
            }

            int x = 0;
            int y = 0;
            bool intersect = true;

            while (intersect)
            {
                intersect = false;

                x = rand.Next(0, width - 4);
                y = rand.Next(0, height - 3);

                // check to make sure the entire obstacle can be placed on the map without interference.
                for (int i = x; i < (x + oWidth); i++)
                {
                    for (int j = y; j < (y + oHeight); j++)
                    {
                        if (!freeSpace[i, j] || roomNumSpace[i, j] == -1)
                            intersect = true;
                    }
                }
            }

            Obstacle newObs;

            switch (type)
            {
                case "pillar":
                    newObs = new Pillar(state, x, y, oWidth, oHeight, roomNumSpace[x, y],numObstacles);
                    break;
                case "chest":
                    newObs = new Chest(state, x, y, oWidth, oHeight, roomNumSpace[x, y], false, numObstacles);
                    numChests++;
                    break;
                default:
                    newObs = new Pillar(state, x, y, oWidth, oHeight, roomNumSpace[x, y], numObstacles);
                    break;
            }


            obstaclesNotDrawn.Add(newObs);
            for(int i = x; i < (x + oWidth); i++)
            {
                for(int j = y; j < (y + oHeight); j++)
                {
                    walkingSpace[i,j] = false;
                    freeSpace[i, j] = false;
                }
            }

            numObstacles++;

        }

        public bool addEnemy(Unit e, String filename)
        {

            if (numEnemies >= (maxEnemies - 1))
            {
                return true;
            }

            if(Math.Sqrt(Math.Pow(e.x - state.hero.x, 2) + Math.Pow(e.y - state.hero.y, 2)) <= temp_sd || !freeSpace[(int)e.x, (int) e.y] || roomNumSpace[(int)e.x, (int) e.y] == -1)
            {
                temp_sd *= .9;
                return false;
            }

            e.addName(filename);
            enemiesNotDrawn.Add(e);
            freeSpace[(int)e.x, (int)e.y] = false;

            numEnemies++;

            return true;
        }

        public int xDirFromChar(char c)
        {

            switch (c)
            {
                case 'w':
                case 's':
                    return 0;
                case 'a':
                    return -1;
                case 'd':
                    return 1;
            }
            return 0;
        }

        public int yDirFromChar(char c)
        {
            switch (c)
            {
                case 'w':
                    return -1;
                case 's':
                    return 1;
                case 'a':
                case 'd':
                    return 0;
            }
            return 0;
        }


        public void recalcRoomNums()
        {

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (roomNumSpace[i, j] != -1)
                        {
                            //Console.WriteLine("");
                            roomNumSpace[i, j] = effectiveRoomNum[roomNumSpace[i, j]];
                        }


                    }
                }

            foreach (Stairs stair in stairsNotDrawn)
            {
                stair.roomNum = roomNumSpace[stair.x,stair.y];
                //Console.WriteLine("Changed Stairs at "+ stair.x+","+stair.y+" to num "+ roomNumSpace[stair.x,stair.y]);
            }

            foreach (Obstacle obstacle in obstaclesNotDrawn)
            {
                obstacle.roomNum = roomNumSpace[(int)obstacle.x, (int)obstacle.y];
            }

            foreach (Door door in doorsNotDrawn)
            {
                door.roomNum = roomNumSpace[(int)door.x, (int)door.y];
            }

            foreach (Unit unit in enemiesNotDrawn)
            {
                unit.roomNum = roomNumSpace[(int) unit.x, (int) unit.y];
            }
        }

        public void addBoundaries(){

            for(int i = 0; i < width; i++){
                for (int j = 0; j < height; j++){
                    if(roomNumSpace[i,j] == -1 || wallSpace[i,j] || doorSpace[i,j]){
                        walkingSpace[i, j] = false;
                    }
                    if (stairSpace[i, j])
                    {
                        walkingSpace[i, j] = true;
                    }
                }
            }

            List<Door> duplicateDoors = new List<Door>();

            if (!environment.Equals("courtyard"))
            {

                foreach (Door door in doorsNotDrawn)
                {
                    for (int i = 0; i < door.width; i++)
                    {
                        for (int j = 0; j < door.height; j++)
                        {
                            if (doorSpace[door.x + i, door.y + j])
                            {
                                duplicateDoors.Add(door);
                                // duplicate door
                            }
                            wallSpace[door.x + i, door.y + j] = false;
                            doorSpace[door.x + i, door.y + j] = true;
                            //walkingSpace[door.x + i, door.y + j] = true;
                        }
                    }
                }

                foreach (Door door in duplicateDoors)
                {
                    Console.WriteLine("removing door");
                    doorsNotDrawn.Remove(door);
                }
            }

        }

        public void updateFreeSpace()
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (wallSpace[i, j])
                    {
                        freeSpace[i, j] = false;
                    }


                }
            }
        }

        public void makeHallway(int x1, int y1, int x2, int y2, int maxHSize)
        {

            // find two door locations. door[x,y] = x point, 
            int upperHallwaySize = Math.Min(maxHSize, maxSizeHallway);
            int wide = (int)rand.Next(minSizeHallway, upperHallwaySize + 1);

            if (wide == 3)
            {
                wide++;
            }

            int hallwayNum = numRooms;

            //*
            bool door1Found = false;
            bool door2Found = false;
            
            /// CALCULATE WHERE DOORS SHOULD GO

            int xInc = 1;
            int yInc = 1;

            int firstRoom = roomNumSpace[x1,y1];
            int secondRoom = roomNumSpace[x2,y2];

            if (y1 <= y2)
            {
                yInc = 1;
            }
            else
            {
                yInc = -1;
            }

            Door door1 = new Door(state, 0, 0, 1, 2, 0, true, -1, -1, false, -1);
            Door door2 = new Door(state, 0, 0, 1, 2, 0, true, -1, -1, false, -1);


            // CHECK X DIfference for door locations

            if (x1 <= x2)
            {
              xInc = 1;
                for (int i = x1; i < x2; i++)
                {
                    if (wallSpace[i, y1])
                    {
                        if (roomNumSpace[i, y1] == firstRoom && !door1Found)
                        {
                            door1 = findDoorLocation(i, y1, x2, y2, true);
                            if (door1.x != -1)
                            {
                                door1Found = true;
                                x1 = door1.x;
                                y1 = door1.y;
                            }
                            else
                            {
                                door1 = new Door(state, -1, -1, 1, 2, 0, true, -1, -1, false, -1);
                                
                            }
                           
                        }
                        else if (roomNumSpace[i, y1] == secondRoom && !door2Found)
                        {
                            door2 = findDoorLocation(i, y1, x2, y2, true);
                            if (door2.x != -1)
                            {
                                door2Found = true;
                            }
                            else
                            {
                                door2 = new Door(state, -1, -1, 1, 2, 0, true, -1, -1, false, -1);
                            }
                        }

                    }
                }
            } else {

                xInc = -1;
                for(int i = x1; i > x2; i--)
                {
                    if(wallSpace[i,y1])
                    {
                        if (roomNumSpace[i, y1] == firstRoom && !door1Found)
                        {
                            door1 = findDoorLocation(i, y1, x2, y2, true);
                            if (door1.x != -1)
                            {
                                door1Found = true;
                                x1 = door1.x;
                                y1 = door1.y;
                            }
                            else
                            {
                                door1 = new Door(state, -1, -1, 1, 2, 0, true, -1 ,-1, false, -1);
                            }

                        }
                        else if (roomNumSpace[i, y1] == secondRoom && !door2Found)
                        {
                            door2 = findDoorLocation(i, y1, x2, y2, true);
                            if (door2.x != -1)
                            {
                                door2Found = true;
                            }
                            else
                            {
                                door2 = new Door(state, -1, -1, 1, 2, 0, true,-1,-1, false, -1);
                            }

                        }



                    }
                }
            }


            // CHECK Y Difference

            if (!door1Found || !door2Found)
            {

                if (y2 <= y1)
                {
                    yInc = 1;
                    for (int i = y2; i < y1; i++)
                    {
                        if (wallSpace[x2, i])
                        {
                            if (roomNumSpace[x2, i] == firstRoom && !door1Found)
                            {
                                door1 = findDoorLocation(x2, i, x1, y1, false);
                                if (door1.x != -1)
                                {
                                    door1Found = true;
                                    x1 = door1.x;
                                    y1 = door1.y;
                                }
                                else
                                {
                                    door1 = new Door(state, -1, -1, 1, 2, 0, true,-1,-1, false, -1);
                                }
                            }
                            else if (roomNumSpace[x2, i] == secondRoom && !door2Found)
                            {
                                door2 = findDoorLocation(x2, i, x1, y1, false);
                                if (door2.x != -1)
                                {
                                    door2Found = true;
                                }
                                else
                                {
                                    door2 = new Door(state, -1, -1, 1, 2, 0, true,-1,-1, false, -1);
                                }

                            }

                        }
                    }
                }
                else
                {

                    yInc = -1;
                    for (int i = y2; i > y1; i--)
                    {
                        if (wallSpace[x2, i])
                        {
                            if (roomNumSpace[x2, i] == firstRoom)
                            {
                                door1 = findDoorLocation(x2, i, x1, y1, false);
                                if (door1.x != -1)
                                {
                                    door1Found = true;
                                    x1 = door1.x;
                                    y1 = door1.y;
                                }
                                else
                                {
                                    door1 = new Door(state, -1, -1, 1, 2, 0, true,-1,-1, false, -1);
                                }

                            }
                            else if (roomNumSpace[x2, i] == secondRoom)
                            {
                                door2 = findDoorLocation(x2, i, x1, y1, false);
                                if (door2.x != -1)
                                {
                                    door2Found = true;
                                }
                                else
                                {
                                    door2 = new Door(state, -1, -1, 1, 2, 0, true,-1,-1, false, -1);
                                }

                            }

                        }
                    }
                }

            }


            if (!door1Found || !door2Found)
            {
                if (door1Found)
                {
                    door1.id = numDoors++;
                    doorsNotDrawn.Add(door1);
                }
                else
                {
                    door2.id = numDoors++;
                    doorsNotDrawn.Add(door2);
                }
                // add door and return;
                return;
            }



            int door1High, door1Low, door2High, door2Low;

            if (door1.vertical)
            {       // door 1 is vertical
                door1Low = door1.y - (wide / 2);
                door1High = door1.y + (door1.height / 2) + (wide / 2);
            }
            else
            {       // door 1 is horizontal
                door1Low = door1.x - (wide / 2);
                door1High = door1.x + (door1.width / 2) + (wide / 2);
            }

            if (door2.vertical)
            {       // door 2 is vertical
                door2Low = door2.y - (wide / 2);
                door2High = door2.y + (door2.height / 2) + (wide / 2);
            }
            else
            {       // door 2 is horizontal
                door2Low = door2.x - (wide / 2);
                door2High = door2.x + (door2.width / 2) + (wide / 2);
            }





            if (door1.vertical != door2.vertical)
            {

                // make sure that there is enough room

                if (door1.vertical)
                {
                   
                    if (door1.x < door2High && door1.x > door2Low)
                    {
                        // shift door2 towards whichever way door1 is facing
                        if (roomNumSpace[door1.x + 1, door1.y] == -1)
                        {
                            // move door2 so that door2Low = door1.x
                            if (door1.x + (wide / 2) <= door2.maxPositive)
                            {
                                door2.x = door1.x + (wide / 2);
                            }
                            else
                            {
                                Console.WriteLine("Not allowing readjustment of doors");
                            }
                        }
                        else
                        {
                            // move door2 so that door2High = door1.x
                            if (door1.x - (door2.width / 2) - (wide / 2) >= door2.maxNegative)
                            {
                                door2.x = door1.x - (door2.width / 2) - (wide / 2);
                            }
                            else
                            {
                                Console.WriteLine("Not allowing readjustment of doors");
                            }
                        }

                    }

                    if (door2.y < door1High && door2.y > door1Low)
                    {

                        if (roomNumSpace[door2.x, door2.y + 1] == -1)
                        {
                            if (door2.y + (wide / 2) <= door1.maxPositive)
                            {
                                door1.y = door2.y + (wide / 2);
                            }
                            else
                            {
                                Console.WriteLine("Not allowing readjustment of doors");
                            }
                            // move door1 so that door1Low = door2.y
                        }
                        else
                        {
                            
                            if (door2.y - (door1.height / 2) - (wide / 2) >= door1.maxNegative)
                            {
                                door1.y = door2.y - (door1.height / 2) - (wide / 2);
                            }
                            else
                            {
                                Console.WriteLine("Not allowing readjustment of doors");
                            }
                            // move door1 so that door1High = door2.y
                        }
                    }


                }
                else
                {
                    if (door2.x < door1High && door2.x > door1Low)
                    {

                        if (roomNumSpace[door2.x + 1, door2.y] == -1)
                        {
                            // move door 1 so that door1Low = door2.x
                            if (door2.x + (wide / 2) <= door1.maxPositive)
                            {
                                door1.x = door2.x + (wide / 2);
                            }
                            else
                            {
                                Console.WriteLine("Not allowing readjustment of doors");
                            }
                        }
                        else
                        {
                            // move door 1 so that door1High = door2.x
                            if (door2.x - (door1.width / 2) - (wide / 2) >= door1.maxNegative)
                            {
                                door1.x = door2.x - (door1.width / 2) - (wide / 2);
                            }
                            else
                            {
                                Console.WriteLine("Not allowing readjustment of doors");
                            }
                        }
                    }

                    if (door1.y < door2High && door1.y > door2Low)
                    {

                        if (roomNumSpace[door1.x, door1.y + 1] == -1)
                        {
                            // move door 2 so that door2Low = door1.y
                            if (door1.y + (wide / 2) <= door2.maxPositive)
                            {
                                door2.y = door1.y + (wide / 2);
                            }
                            else
                            {
                                Console.WriteLine("Not allowing readjustment of doors");
                            }
                            // move door1 so that door1Low = door2.y
                        }
                        else
                        {
                            // move door2 so that door2High = door1.y
                            if (door1.y - (door2.height / 2) - (wide / 2) >= door2.maxNegative)
                            {
                                door2.y = door1.y - (door2.height / 2) - (wide / 2);
                            }
                            else
                            {
                                Console.WriteLine("Not allowing readjustment of doors");
                            }
                        }
                    }



                }

            }
            


            // covered the x difference between both.


            if (door1Found)
            {
                door1.id = numDoors++;
                doorsNotDrawn.Add(door1);
            }

            if (door2Found)
            {
                door2.id = numDoors++;
                doorsNotDrawn.Add(door2);
            }


            if (door1.vertical)
            {
                door1Low = door1.y - (wide / 2);
                door1High = door1.y + (door1.height / 2) + (wide / 2);
            }
            else
            {
                door1Low = door1.x - (wide / 2);
                door1High = door1.x + (door1.width / 2) + (wide / 2);
            }

            if (door2.vertical)
            {
                door2Low = door2.y - (wide / 2);
                door2High = door2.y + (door2.height / 2) + (wide / 2);
            }
            else
            {
                door2Low = door2.x - (wide / 2);
                door2High = door2.x + (door2.width / 2) + (wide / 2);
            }

            // DONE CALCULATING WHERE DOORS SHOULD GO


            bool xChange = true;
            bool yChange = true;

            if (door1.vertical == door2.vertical)
            {
                if (door1.vertical)
                {
                    // both doors are vertical
                    yChange = false;
                }
                else
                {
                    // both doors are horizontal
                    xChange = false;
                }
            }


            if (xChange)
            {
                //Console.WriteLine("Making horizontal hallway");
                int otherX;

                if (door1.vertical)
                {

                    if (door2.vertical)
                    {
                        otherX = door2.x;
                    }
                    else
                    {
                        if (xInc == -1)
                        {
                            otherX = door2Low;
                        }
                        else
                        {
                            otherX = door2High;
                        }
                    }
                    // make box with bounds from (x) door1.x - door2Low and (y) door1High to door1

                    makeBox(door1.x, otherX, door1Low, door1High, hallwayNum);
                }


                else if (door2.vertical)
                {


                    if (door1.vertical)
                    {
                        otherX = door1.x;
                    }
                    else
                    {
                        if (xInc == -1)
                        {
                            otherX = door1Low;
                        }
                        else
                        {
                            otherX = door1High;
                        }
                    }
                    // make box with bounds from (x) door1.x - door2Low and (y) door1High to door1
                    makeBox(door2.x, otherX, door2Low, door2High, hallwayNum);

                }
            }


            //yChange = false;
            if (yChange)
            {
                //Console.WriteLine("Making vertical hallway");
                int otherY;

                if (!door1.vertical)
                {


                    if (!door2.vertical)
                    {
                        otherY = door2.y;
                    }
                    else
                    {
                        if (yInc == -1)
                        {
                            otherY = door2Low;
                        }
                        else
                        {
                            otherY = door2High;
                        }
                    }

                    //Console.WriteLine("Making vertical box");
                    // make box with bounds from (x) door1.x - door2Low and (y) door1High to door1
                    makeBox(door1Low, door1High, door1.y, otherY, hallwayNum);

                }


                //*
                else if (!door2.vertical)
                {


                    if (!door1.vertical)
                    {
                        otherY = door1.y;
                    }
                    else
                    {
                        if (yInc == -1)
                        {
                            otherY = door1Low;
                        }
                        else
                        {
                            otherY = door1High;
                        }
                    }
                    // make box with bounds from (x) door1.x - door2Low and (y) door1High to door1
                    makeBox(door2Low, door2High, door2.y, otherY, hallwayNum);

                }
                  //*/
            }


            //*/
///// OLD WAY   //////

            /*
            int roomNum1 = roomNumSpace[x1, y1];
            int roomNum2 = roomNumSpace[x2, y2];




            //int upperHallwaySize = Math.Min(maxHSize, maxSizeHallway);
            //int wide = (int) rand.Next(minSizeHallway, upperHallwaySize + 1);

            if (wide == 3)
            {
                wide++;
            }


            int deltaX = x1 - x2;
            int deltaY = y1 - y2;

            //int hallwayNum = numRooms;

            int halfwayInc = (int) (wide - 1) / 2;

            int door1Loc = rand.Next(0, wide - 1);
            int door2Loc = rand.Next(0, wide - 1);

            door1Loc = door2Loc = 0;

            // add an if statement here saying if it is a short hallway to just tear down all walls in it's path

            int doorCounter = 0;
            bool door1ShouldBePlaced = false;
            bool door2ShouldBePlaced = false;

            bool door1Placed = false;
            bool door2Placed = false;

            if (deltaX <= 0)
            {
                // x1 < x2
                for (; x1 < x2 + (wide - halfwayInc); x1++)
                {
                    if (!door1Placed)
                    {
                        if (wallSpace[x1, y1] && roomNumSpace[x1, y1] != hallwayNum && roomNumSpace[x1, y1] != -1)
                        {
                            door1ShouldBePlaced = true;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (!door2Placed)
                    {
                        if (wallSpace[x1, y1] && roomNumSpace[x1, y1] != hallwayNum && roomNumSpace[x1, y1] != -1)
                        {
                            door2ShouldBePlaced = true;

                        }
                        

                    }

                    doorCounter = 0;

                    for (int i = -1; i <= wide; i++)
                    {
                        if (i == -1 || i == wide)
                        {
                            if (roomNumSpace[x1, y1 - halfwayInc + i] == -1)
                            {
                                wallSpace[x1, y1 - halfwayInc + i] = true;
                            }
                        }
                        else
                        {
                            if (door1ShouldBePlaced && doorCounter == door1Loc){ 
                                doors.Add(new Door(state, x1, y1 - halfwayInc + i, 1, 2, roomNumSpace[x1, y1 - halfwayInc], false));
                                doorSpace[x1, y1 - halfwayInc + i] = true;
                                wallSpace[x1, y1 - halfwayInc + i] = false;
                                door1Placed = true;
                                door1ShouldBePlaced = false;
                                
                            } else if(door2ShouldBePlaced && doorCounter == door2Loc){
                                doors.Add(new Door(state, x1, y1 - halfwayInc + i, 1, 2, roomNumSpace[x1, y1 - halfwayInc], false));
                                doorSpace[x1, y1 - halfwayInc + i] = true;
                                wallSpace[x1, y1 - halfwayInc + i] = false;
                                door2Placed = true;
                                door2ShouldBePlaced = false;
                            }
                            doorCounter++;

                            walkingSpace[x1, y1 - halfwayInc + i] = true;

                            
                        }

                        if (roomNumSpace[x1, y1 - halfwayInc + i] == -1)
                        {
                            roomNumSpace[x1, y1 - halfwayInc + i] = hallwayNum;
                            hallwaySpace[x1, y1 - halfwayInc + i] = true;
                        }
                        
                        
                    }
                }
               // x1 -= halfwayInc;
            }
            else
            {
                for (; x1 >= x2 - (halfwayInc); x1--)
                {

                    if (!door1Placed)
                    {
                        if (wallSpace[x1, y1] && roomNumSpace[x1, y1] != hallwayNum && roomNumSpace[x1, y1] != -1)
                        {
                            door1ShouldBePlaced = true;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (!door2Placed)
                    {
                        if (wallSpace[x1, y1] && roomNumSpace[x1, y1] != hallwayNum && roomNumSpace[x1,y1] != -1)
                        {
                            door2ShouldBePlaced = true;

                        }
                        

                    }

                    for (int i = -1; i <= wide; i++)
                    {
                        if (i == -1 || i == wide)
                        {
                            if (roomNumSpace[x1, y1 - halfwayInc + i] == -1)
                            {
                                wallSpace[x1, y1 - halfwayInc + i] = true;
                            }
                        }
                        else
                        {
                            if (door1ShouldBePlaced && doorCounter == door1Loc)
                            {
                                doors.Add(new Door(state, x1, y1 - halfwayInc + i, 1, 2, roomNumSpace[x1, y1 - halfwayInc], false));
                                doorSpace[x1, y1 - halfwayInc + i] = true;
                                wallSpace[x1, y1 - halfwayInc + i] = false;
                                door1Placed = true;
                                door1ShouldBePlaced = false;

                            }
                            else if (door2ShouldBePlaced && doorCounter == door2Loc)
                            {
                                doors.Add(new Door(state, x1, y1 - halfwayInc + i, 1, 2, roomNumSpace[x1, y1 - halfwayInc], false));
                                doorSpace[x1, y1 - halfwayInc + i] = true;
                                wallSpace[x1, y1 - halfwayInc + i] = false;
                                door2Placed = true;
                                door2ShouldBePlaced = false;
                            }
                            doorCounter++;

                            walkingSpace[x1, y1 - halfwayInc + i] = true;

                            
                        }

                        if (roomNumSpace[x1, y1 - halfwayInc + i] == -1)
                        {
                            roomNumSpace[x1, y1 - halfwayInc + i] = hallwayNum;
                            hallwaySpace[x1, y1 - halfwayInc + i] = true;
                        }
                        
                    }
                }
                //x1 += halfwayInc;
            }

            


            if(deltaY < 0){

                //y1 -= halfwayInc;
               
                for (; y1 <= y2; y1++)
                {

                    if (!door1Placed)
                    {
                        if (wallSpace[x2, y1] && roomNumSpace[x2, y1] != hallwayNum && roomNumSpace[x2, y1] != -1)
                        {
                            door1ShouldBePlaced = true;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (!door2Placed)
                    {
                        if (wallSpace[x2, y1] && roomNumSpace[x2, y1] != hallwayNum && roomNumSpace[x2, y1] != -1)
                        {
                            door2ShouldBePlaced = true;

                        }


                    }

                    doorCounter = 0;

                    for (int i = -1; i <= wide; i++)
                    {
                        if (i == -1 || i == wide)
                        {
                            if (roomNumSpace[x2 - halfwayInc + i, y1] == -1)
                            {
                                wallSpace[x2 - halfwayInc + i, y1] = true;

                            }
                        }
                        else
                        {

                            if (door1ShouldBePlaced && doorCounter == door1Loc)
                            {
                                doors.Add(new Door(state, x2 - halfwayInc + i, y1, 2, 1, roomNumSpace[x2 - halfwayInc + i, y1], true));
                                doorSpace[x2 - halfwayInc + i, y1] = true;
                                wallSpace[x2 - halfwayInc + i, y1] = false;
                                door1Placed = true;
                                door1ShouldBePlaced = false;

                            }
                            else if (door2ShouldBePlaced && doorCounter == door2Loc)
                            {
                                doors.Add(new Door(state, x2 - halfwayInc + i, y1, 2, 1, roomNumSpace[x2 - halfwayInc + i, y1], true));
                                doorSpace[x2 - halfwayInc + i, y1] = true;
                                wallSpace[x2 - halfwayInc + i, y1] = false;
                                door2Placed = true;
                                door2ShouldBePlaced = false;
                            }
                            doorCounter++;

                            walkingSpace[x2 - halfwayInc + i, y1] = true;

                            if (wallSpace[x2 - halfwayInc + i, y1] && roomNumSpace[x2 - halfwayInc + i, y1] == hallwayNum)
                            {
                                wallSpace[x2 - halfwayInc + i, y1] = false;
                            }
                        }

                        

                        walkingSpace[x2 - halfwayInc + i, y1] = true;
                        if (roomNumSpace[x2 - halfwayInc + i, y1] == -1)
                        {
                            roomNumSpace[x2 - halfwayInc + i, y1] = hallwayNum;
                            hallwaySpace[x2 - halfwayInc + i, y1] = true;
                        }
                    }
                }
            }
            else
            {
                //y1 +=  wide - halfwayInc;

                for (; y1 >= y2; y1--)
                {

                    if (!door1Placed)
                    {
                        if (wallSpace[x2, y1] && roomNumSpace[x2, y1] != hallwayNum && roomNumSpace[x2, y1] != -1)
                        {
                            door1ShouldBePlaced = true;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if (!door2Placed)
                    {
                        if (wallSpace[x2, y1] && roomNumSpace[x2, y1] != hallwayNum && roomNumSpace[x2, y1] != -1)
                        {
                            door2ShouldBePlaced = true;

                        }


                    }
                    doorCounter = 0;

                    for (int i = -1; i <= wide; i++)
                    {
                        if (i == -1 || i == wide)
                        {
                            if (roomNumSpace[x2 - halfwayInc + i, y1] == -1)
                            {
                                wallSpace[x2 - halfwayInc + i, y1] = true;
                            }
                        }
                        else
                        {
                            if (door1ShouldBePlaced && doorCounter == door1Loc)
                            {
                                doors.Add(new Door(state, x2 - halfwayInc + i, y1, 2, 1, roomNumSpace[x2 - halfwayInc + i, y1], true));
                                doorSpace[x2 - halfwayInc + i, y1] = true;
                                wallSpace[x2 - halfwayInc + i, y1] = false;
                                door1Placed = true;
                                door1ShouldBePlaced = false;

                            }
                            else if (door2ShouldBePlaced && doorCounter == door2Loc)
                            {
                                doors.Add(new Door(state, x2 - halfwayInc + i, y1, 2, 1, roomNumSpace[x2 - halfwayInc + i, y1], true));
                                doorSpace[x2 - halfwayInc + i, y1] = true;
                                wallSpace[x2 - halfwayInc + i, y1] = false;
                                door2Placed = true;
                                door2ShouldBePlaced = false;
                            }
                            doorCounter++;


                            walkingSpace[x2 - halfwayInc + i, y1] = true;

                            if (wallSpace[x2 - halfwayInc + i, y1] && roomNumSpace[x2 - halfwayInc + i, y1] == hallwayNum)
                            {
                                wallSpace[x2 - halfwayInc + i, y1] = false;
                            }
                        }

                        

                        walkingSpace[x2 - halfwayInc + i, y1] = true;
                        if (roomNumSpace[x2 - halfwayInc + i, y1] == -1)
                        {
                            roomNumSpace[x2 - halfwayInc + i, y1] = hallwayNum;
                            hallwaySpace[x2 - halfwayInc + i, y1] = true;
                        }
                    }
                }
            }
              //*/

            numRooms++;
        }


        public Door findDoorLocation(int x1, int y1,int x2,int y2, bool vertical){

            int xInc = 0;
            int yInc = 0;
            
            if(x1 < x2){
                xInc = 1;
            } else if(x1 > x2) {
                xInc = -1;
            }

            if(y1 < y2){
                yInc = 1;
            } else if(y1 > y2) {
                yInc = -1;
            }





            if (!vertical)
            {
                // calculating horizontal door location
                int xTemp = x1;
                while (xTemp > 0 && xTemp < width && wallSpace[xTemp, y1] && roomNumSpace[xTemp++, y1] == roomNumSpace[x1, y1]) ;
                int xMax = xTemp - 1;
                
                
                xTemp = x1;
                while (xTemp > 0 && xTemp < width && wallSpace[xTemp, y1] && roomNumSpace[xTemp--, y1] == roomNumSpace[x1, y1]) ;
                int xMin = xTemp + 1;

                Door retDoor =  new Door(state, x1,y1,2,1,roomNumSpace[x1,y1+yInc],false,xMin, xMax, false, numDoors);

                
                int originalX = retDoor.x;

                bool good = true;

                /*
                while (!isDoorGood(retDoor))
                {
                    if (retDoor.x + 1 > xMax)
                    {
                        good = false;
                        break;
                    }
                    else
                    {
                        retDoor.x++;
                    }
                }

                if (good)
                {
                    return retDoor;
                }

                retDoor.x = originalX;

                while (!isDoorGood(retDoor))
                {
                    if (retDoor.x - 1 < xMin)
                    {
                        good = false;
                        break;
                    }
                    else
                    {
                        retDoor.x--;
                    }
                }

                //*/
                if (good)
                {
                    return retDoor;
                }
                else
                {
                    return new Door(state, -1, -1, 1, 2, -1, true, -1, -1, false, -1);
                }

            }
            else
            {
                // calculating vertical door location

                int yTemp = x1;
                while (yTemp > 0 && yTemp < height && wallSpace[x1, yTemp] && roomNumSpace[x1,yTemp++] == roomNumSpace[x1, y1]) ;
                int yMax = yTemp - 1;

                yTemp = x1;
                while (yTemp > 0 && yTemp < height && wallSpace[x1, yTemp] && roomNumSpace[x1, yTemp--] == roomNumSpace[x1, y1]) ;
                int yMin = yTemp + 1;


                Door retDoor = new Door(state, x1, y1, 1, 2, roomNumSpace[x1 + xInc, y1], true, yMin, yMax, false,numDoors);
                int originalY = retDoor.y;

                bool good = true;

                /*
                while (!isDoorGood(retDoor))
                {
                    if ((retDoor.y + 1) > yMax)
                    {
                        good = false;
                        break;
                    }
                    else
                    {
                        retDoor.y = retDoor.y + 1;
                    }
                }

                if (good)
                {
                    return retDoor;
                }

                retDoor.y = originalY;

                while (!isDoorGood(retDoor))
                {
                    if ((retDoor.y - 1) < yMin)
                    {
                        good = false;
                        break;
                    }
                    else
                    {
                        retDoor.y--;
                    }
                }
                */
                if (good)
                {
                    return retDoor;
                }
                else
                {
                    return new Door(state, -1, -1, 1, 2, -1, true, -1, -1, false, -1);
                }


            }



        }


        public bool isDoorGood(Door door)
        {

            if (door.vertical)
            {
                int negativeRoom = roomNumSpace[door.x - 1, door.y];
                int positiveRoom = roomNumSpace[door.x + 1, door.y];

                Console.WriteLine("negative = " + negativeRoom + " positive = " + positiveRoom);

                for (int i = 0; i < door.height; i++)
                {


                    if (roomNumSpace[door.x - 1, door.y + i] != negativeRoom || wallSpace[door.x - 1, door.y + i])
                    {
                        return false;
                    }

                    if (roomNumSpace[door.x + 1, door.y + i] != positiveRoom || wallSpace[door.x + 1, door.y + i])
                    {
                        return false;
                    }


                }

            }
            else
            {


                int negativeRoom = roomNumSpace[door.x, door.y - 1];
                int positiveRoom = roomNumSpace[door.x, door.y + 1];

                Console.WriteLine("negative = " + negativeRoom + " positive = " + positiveRoom);

                for (int i = 0; i < door.width; i++)
                {


                    if (roomNumSpace[door.x + i, door.y - 1] != negativeRoom || wallSpace[door.x + i, door.y - 1])
                    {
                        return false;
                    }

                    if (roomNumSpace[door.x + i, door.y + 1] != positiveRoom || wallSpace[door.x + i, door.y + 1])
                    {
                        return false;
                    }


                }
            }

            return true;
        }


        public void makeBox(int x1,int x2,int y1,int y2, int roomNum){
            
            int minX = Math.Min(x1, x2);
            int maxX = Math.Max(x1, x2);
            int minY = Math.Min(y1, y2);
            int maxY = Math.Max(y1, y2);

            //Console.WriteLine("Making Hallway #"+roomNum+" that (x) is " + (maxX - minX) + " wide and (y) is " + (maxY - minY));

            for(int i = minX; i <= maxX; i++){
                for (int j = minY; j <= maxY; j++)
                {

                    if ((i == minX || i == maxX || j == minY || j == maxY))
                    {
                         if(roomNumSpace[i, j] == -1){
                             wallSpace[i, j] = true;
                         }

                    }
                    else
                    {
                        
                        if (roomNumSpace[i, j] != -1 && effectiveRoomNum[roomNumSpace[i, j]] != effectiveRoomNum[roomNum] && !wallSpace[i,j])
                        {
                            if (roomNumSpace[i, j] < numNonHallways)
                            {
                                //Console.WriteLine("COULD HAVE SOME UNINTENDED SIDE EFFECTS IN ROOM NUMBERS");
                            }

                            //int highVal = Math.Max(effectiveRoomNum[roomNumSpace[cX, cY]], effectiveRoomNum[numRooms]);
                            //int lowVal = Math.Min(effectiveRoomNum[roomNumSpace[cX, cY]], effectiveRoomNum[numRooms]);

                            int highVal = roomNum;
                            int lowVal = effectiveRoomNum[roomNumSpace[i, j]];

                            for (int k = 0; k < numRooms; k++)
                            {
                                if (effectiveRoomNum[k] == lowVal)
                                {
                                    effectiveRoomNum[k] = highVal;
                                }

                            }
                        }
                        
                        if (roomNumSpace[i, j] >= numNonHallways)
                        {
                            wallSpace[i, j] = false;
                        }
                    }


                    if (roomNumSpace[i, j] == -1)
                    {
                        roomNumSpace[i, j] = roomNum;
                    }

                    

                }
            }


        }

        public double distanceBtwnPts(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        public void connectTouchingRooms(){
            for(int i = 0; i < width; i++){
                for(int j = 0; j < height; j++){

                    if (wallIntersection[i, j])
                    {
                        // found an index where two rooms are potentially touching. 
                        // look at each side of the wall.

                        


                    }
                    
                }
            }
        }

        public void updateHeroVisibility()
        {
            if (caveCounter < caveSightFrequency)
            {
                caveCounter++;
                return;
            }
            caveCounter = 0;

            int radius = 7;

            int currentHeroRoom = roomNumSpace[(int)state.hero.x, (int)state.hero.y];

            int startX, endX, startY, endY;
            startX = Math.Max(0,(int)state.hero.x - radius);
            endX = Math.Min(width - 1, (int)state.hero.x + radius);
            startY = Math.Max(0, (int)state.hero.y - radius);
            endY = Math.Min(height - 1, (int)state.hero.y + radius);

            for (int i = startX; i < endX; i++)
            {
                for (int j = startY; j < endY; j++)
                {
                    if (!drawingSpace[i, j])
                    {
                        if (roomNumSpace[i,j] != -1 && distanceBtwnPts((int)state.hero.x, (int)state.hero.y, i, j) < radius)
                        {
                            drawingSpace[i, j] = true;

                            /*
                            if (!wallSpace[i, j])
                            {
                                for (int k = -1; k <= 1; k++)
                                {
                                    for (int q = -1; q <= 1; q++)
                                    {
                                        if (wallSpace[i + k, j + q] || doorSpace[i+k,j+q])
                                        {
                                            drawingSpace[i + k, j + q] = true;
                                        }
                                    }
                                }
                            }
                            */
                        }
                    }
                }
            }

        }

        public void updateDrawingGrid(int newRoomNum)
        {
            //*
            if (roomDrawn[newRoomNum] || environment.Equals("cave"))
            {
                return;
            }
            //*/

            bool outsideWalls = newRoomNum == 0 && environment.Equals("courtyard");

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (roomNumSpace[i, j] == newRoomNum)
                    {
                        
                        drawingSpace[i, j] = true;

                        if (!wallSpace[i, j] && !doorSpace[i,j])
                        {
                            for (int k = -1; k <= 1; k++)
                            {
                                for (int l = -1; l <= 1; l++)
                                {
                                    if (wallSpace[i + k, j + l])
                                    {
                                        drawingSpace[i + k, j + l] = true;
                                    }
                                    else if (doorSpace[i + k, j + l])
                                    {
                                        //Console.WriteLine("found door space");
                                        Door doorToRemove = new Door(state, -1, -1, -1, -1, -1, true, -1, -1, false, -1);
                                        foreach (Door door in doorsNotDrawn)
                                        {
                                            //Console.WriteLine("Checking door");
                                            if (door.x == (i + k) && door.y == (j + l))
                                            {
                                               // Console.WriteLine("door added");
                                                doorToRemove = door;
                                                doors.Add(door);
                                            }
                                        }
                                        if (doorToRemove.x != -1)
                                        {
                                            doorsNotDrawn.Remove(doorToRemove);
                                        }
                                    }
                                }
                            }
                        }

                        // needs to also include the bordering walls that aren't the same roomNum
                    }
                    else if (outsideWalls && roomNumSpace[i,j] == -1)
                    {
                        drawingSpace[i, j] = true;
                    }



                }
            }
            
            
            foreach(Stairs stair in stairsNotDrawn)
            {
                if(stair.roomNum == newRoomNum){
                    stairs.Add(stair);
                    //stairs.Remove(stair);
                }
            }

            

            foreach (Obstacle obstacle in obstaclesNotDrawn)
            {
                if(obstacle.roomNum == newRoomNum){
                    obstacles.Add(obstacle);
                    //obstacles.Remove(obstacle);
                }
            }

            foreach (Obstacle obstacle in obstacles)
            {
                obstaclesNotDrawn.Remove(obstacle);
                
            }

            foreach (Unit enemy in enemiesNotDrawn){
                if(enemy.roomNum == newRoomNum){
                    enemies.Add(enemy);
                    //enemies.Remove(enemy);
                }
            }

            foreach (Unit enemy in enemies)
            {
                enemiesNotDrawn.Remove(enemy);
            }

            roomDrawn[newRoomNum] = true;
            
        }

        public void updateDoorCollisions()
        {
            foreach (Door door in doors)
            {
                if (door.closed)
                {
                    walkingSpace[(int)door.x,(int)door.y] = false;
                    walkingSpace[(int)(door.x + (door.width - 1)), (int)(door.y + (door.height - 1))] = false;
                }
                else
                {
                    walkingSpace[(int)door.x, (int)door.y] = true;
                    walkingSpace[(int)(door.x + (door.width - 1)), (int)(door.y + (door.height - 1))] = true;

                }
            }
        }

        public void eliminateFogOfWar()
        {

             for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (roomNumSpace[i, j] != -1)
                    {
                        drawingSpace[i, j] = true;
                    }
                }
             }

             for (int i = 0; i < 2 * maxStairs;i++ )
             {
                 roomDrawn[i] = true;
             }

             makeAllObjectsEligible();
            

        }


        public void makeAllObjectsEligible()
        {
            foreach (Unit enemy in enemiesNotDrawn)
            {
                enemies.Add(enemy);
            }

            foreach (Obstacle obstacle in obstaclesNotDrawn)
            {
                obstacles.Add(obstacle);
            }

            foreach (Stairs stair in stairsNotDrawn)
            {
                stairs.Add(stair);
            }

            foreach (Door door in doorsNotDrawn)
            {
                doors.Add(door);
            }
        }

        public void removeProj(Projectile proj)
        {
            state.room.deletingProj.Add(proj);
        }

        public void saveState()
        {
            state.allLevelInfo.updateLevel();
        }

        public void removeDoors()
        {
            doorsNotDrawn = new List<Door>();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (doorSpace[i, j])
                    {
                        doorSpace[i, j] = false;
                        wallSpace[i, j] = false;
                        walkingSpace[i, j] = true;
                    }
                }
            }
        }

        public void updateHeroStaringPosition()
        {
            if (environment.Equals("cave"))
            {
                updateHeroVisibility();
            }
            else if (environment.Equals("dungeon"))
            {
                updateDrawingGrid(roomNumSpace[(int)state.hero.x, (int)state.hero.y]);
            }
            else
            {
                updateDrawingGrid(roomNumSpace[(int)state.hero.x, (int)state.hero.y]);
            }
        }

        public void draw(Graphics g)
        {
            if (environment.Equals("dungeon"))
            {
                int minX = Math.Max(0, (int)state.hero.x - 2 - ((state.form.ClientSize.Width / 2) / state.size));
                int maxX = Math.Min(width, (int)state.hero.x + 2 + ((state.form.ClientSize.Width / 2) / state.size) + 1);

                int minY = Math.Max(0, (int)state.hero.y - 2 - ((state.form.ClientSize.Height / 2) / state.size) + 1);
                int maxY = Math.Min(height, (int)state.hero.y + 2 + ((state.form.ClientSize.Height / 2) / state.size) + 1);

                for (int i = minX; i < maxX; i++)
                    for (int j = minY; j < maxY; j++)
                        if (drawingSpace[i, j])
                        {
                            if (wallSpace[i, j])
                                g.DrawImage(wall1, (int)(i * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size), (int)(j * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size), state.size, state.size);
                            else
                                g.DrawImage(floor1, (int)(i * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size), (int)(j * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size), state.size, state.size);
                        }

                foreach (Obstacle obstacle in obstacles)
                    obstacle.draw(g);

                foreach (Door door in doors)
                    door.draw(g);

                foreach (Stairs stair in stairs)
                {
                    /*if (stair.path.Equals("C:\\"))
                    {
                        g.DrawImage(floor2, (int)(stair.x * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size), (int)(stair.y * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size), state.size, state.size);
                    }
                    else
                    {*/
                        stair.draw(g);
                    //}
                }

                foreach (KeyValuePair<Item, PointF> item in droppedItems)
                    g.DrawImage(item.Key.img, (int)(item.Value.X * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size - state.size / 2), (int)(item.Value.Y * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size - state.size / 2), state.size, state.size);

                foreach (Projectile proj in projectiles)
                    proj.draw(g);

                foreach (Unit enemy in enemies)
                    enemy.draw(g);
            }
            else if (environment.Equals("cave"))
            {
                updateHeroVisibility();

                int minX = Math.Max(0, (int)state.hero.x - 2 - ((state.form.ClientSize.Width / 2) / state.size));
                int maxX = Math.Min(width, (int)state.hero.x + 2 + ((state.form.ClientSize.Width / 2) / state.size) + 1);

                int minY = Math.Max(0, (int)state.hero.y - 2 - ((state.form.ClientSize.Height / 2) / state.size) + 1);
                int maxY = Math.Min(height, (int)state.hero.y + 2 + ((state.form.ClientSize.Height / 2) / state.size) + 1);

                for (int i = minX; i < maxX; i++)
                {
                    for (int j = minY; j < maxY; j++)
                    {
                        if (drawingSpace[i, j])
                        {

                            if (wallSpace[i, j])
                            {
                                g.DrawImage(wall1, (int)(i * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size), (int)(j * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size), state.size, state.size);
                                continue;
                            }
                            else if (roomNumSpace[i, j] != -1)
                            {

                                g.DrawImage(floor1, (int)(i * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size), (int)(j * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size), state.size, state.size);

                            }
                        }

                    }
                }

                foreach (Obstacle obstacle in obstacles)
                {
                    if (drawingSpace[obstacle.x, obstacle.y])
                    {
                        obstacle.draw(g);
                    }
                }

                foreach (Door door in doors)
                {
                    if (drawingSpace[door.x, door.y] || drawingSpace[door.x + (door.width - 1), door.y + (door.height - 1)])
                    {
                        door.draw(g);
                    }
                }

                foreach (Stairs stair in stairs)
                {
                    if (drawingSpace[stair.x, stair.y])
                    {
                        //if (stair.path.Equals("C:\\"))
                        //{
                        //    g.DrawImage(floor2, (int)(stair.x * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size), (int)(stair.y * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size), state.size, state.size);
                        //}
                        //else
                        //{
                            stair.draw(g);
                        //}
                    }
                }

                foreach (KeyValuePair<Item, PointF> item in droppedItems)
                {
                    if (drawingSpace[(int)item.Value.X, (int)item.Value.Y])
                    {
                        g.DrawImage(item.Key.img, (int)(item.Value.X * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size - state.size / 2), (int)(item.Value.Y * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size - state.size / 2), state.size, state.size);
                        /*  
        +                if (item.Key.showDes)
        +                {
        +                    Console.WriteLine("showing");
        +                    g.DrawString(item.Key.description, state.font, Brushes.White, new PointF(item.Value.X, item.Value.Y - state.size));
        +                }
        +               * */
                    }
                }


                foreach (Projectile proj in projectiles)
                    proj.draw(g);

                foreach (Unit enemy in enemies)
                    if (drawingSpace[(int)enemy.x,(int) enemy.y])
                    {
                        enemy.draw(g);
                    }


            }
            else if (environment.Equals("courtyard"))
            {
                int minX = Math.Max(0, (int)state.hero.x - 2 - ((state.form.ClientSize.Width / 2) / state.size));
                int maxX = Math.Min(width, (int)state.hero.x + 2 + ((state.form.ClientSize.Width / 2) / state.size) + 1);

                int minY = Math.Max(0, (int)state.hero.y - 2 - ((state.form.ClientSize.Height / 2) / state.size) + 1);
                int maxY = Math.Min(height, (int)state.hero.y + 2 + ((state.form.ClientSize.Height / 2) / state.size) + 1);

                for (int i = minX; i < maxX; i++)
                {
                    for (int j = minY; j < maxY; j++)
                    {
                        if (drawingSpace[i, j])
                        {
                            if (grassSpace[i, j])
                            {
                                g.DrawImage(floor1, (int)(i * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size), (int)(j * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size), state.size, state.size);
                            }
                            if (wallSpace[i, j])
                                g.DrawImage(wall2, (int)(i * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size), (int)(j * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size), state.size, state.size);
                            else if (insideHouse[i, j])
                            {
                                g.DrawImage(floor2, (int)(i * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size), (int)(j * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size), state.size, state.size);
                            }
                            else if (roomNumSpace[i, j] == -1 && !stairSpace[i, j])
                            {
                                g.DrawImage(wall1, (int)(i * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size), (int)(j * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size), state.size, state.size);
                            }

                        }
                        
                    }
                }

                foreach (Obstacle obstacle in obstacles)
                    obstacle.draw(g);

                foreach (Door door in doors)
                    door.draw(g);

                foreach (Stairs stair in stairs)
                {
                        stair.draw(g);

                }

                foreach (KeyValuePair<Item, PointF> item in droppedItems)
                    g.DrawImage(item.Key.img, (int)(item.Value.X * state.size + state.form.ClientSize.Width / 2 - state.hero.x * state.size - state.size / 2), (int)(item.Value.Y * state.size + state.form.ClientSize.Height / 2 - state.hero.y * state.size - state.size / 2), state.size, state.size);

                foreach (Projectile proj in projectiles)
                    proj.draw(g);

                foreach (Unit enemy in enemies)
                    enemy.draw(g);
            }




        }

    }
}
