using System;
using System.Collections.Generic;
using System.IO;

namespace DungeonDrive
{
    public class AllLevelInfo
    {
        GameState state;
        public List<LevelInfo> allLevels = new List<LevelInfo>();
        public List<CDriveSubDirs> cDriveSubDirs = new List<CDriveSubDirs>();

        public AllLevelInfo(GameState state, String currentRoom)
        {
            this.state = state;
            createSubDirEnvs();
            addLevel(new LevelInfo(state, currentRoom, false));
        }

        public void addLevel(LevelInfo newLevel)
        {
            allLevels.Add(newLevel);
        }

        public void loadLevel(String path)
        {

            foreach (LevelInfo tempLevelInfo in allLevels)
            {
                if (tempLevelInfo.dirName.Equals(path))
                {
                    //Console.WriteLine("Found the file");
                    //update doorsss
                    /*
                    foreach (Door door in state.room.doorsNotDrawn)
                    {
                        if (door.id < tempLevelInfo.doorsOpened.Length)
                        {
                            door.closed = tempLevelInfo.doorsOpened[door.id];
                        }
                    }
                    */

                    // update chests

                    foreach (Obstacle obs in state.room.obstacles)
                    {
                        //Console.WriteLine("Obs");
                    }

                    for (int i = 0; i < state.room.obstacles.Count; i++)
                    {
                        //Console.WriteLine("Checking obstacles " + i);
                        if (state.room.obstacles[i] is Chest)
                        {
                            //Console.WriteLine("Found a chest");
                            if (state.room.obstacles[i].id < tempLevelInfo.chestsOpened.Length)
                            {
                                //Console.WriteLine("Changed chest");
                                Chest chest = (Chest)state.room.obstacles[i];
                                chest.closed = tempLevelInfo.chestsOpened[chest.id];
                                state.room.obstacles[i] = chest;
                            }
                        }
                    }

                    // update room

                    //Console.WriteLine("numRooms = " + state.room.numRooms);
                    if (tempLevelInfo.type.Equals("dungeon"))
                    {
                        for (int i = 0; i < state.room.numRooms; i++)
                        {
                            //if (i < state.room.roomDrawn.Length && i < tempLevelInfo.roomsDrawn.Length)
                            //{
                            //Console.WriteLine("Checking room " + i);

                            if (tempLevelInfo.roomsDrawn[i])
                            {
                                //Console.WriteLine("TRYING TO DRAW ROOM " + i);
                                state.room.updateDrawingGrid(i);
                            }
                            //}
                        }

                        state.room.updateDoorCollisions();
                    }
                    else if (tempLevelInfo.type.Equals("cave"))
                    {
                        for (int i = 0; i < state.room.width; i++)
                        {
                            for (int j = 0; j < state.room.height; j++)
                            {
                                state.room.drawingSpace[i, j] = tempLevelInfo.drawingSpace[i, j];
                            }
                        }
                    }
                    else if (tempLevelInfo.type.Equals("courtyard"))
                    {
                        for (int i = 0; i < state.room.numRooms; i++)
                        {
                            //if (i < state.room.roomDrawn.Length && i < tempLevelInfo.roomsDrawn.Length)
                            //{
                            //Console.WriteLine("Checking room " + i);

                            if (tempLevelInfo.roomsDrawn[i])
                            {
                                //Console.WriteLine("TRYING TO DRAW ROOM " + i);
                                state.room.updateDrawingGrid(i);
                            }
                            //}
                        }
                    }

                }
            }

        }

        public bool levelAlreadyExists(String path)
        {
            foreach (LevelInfo levelInfo in allLevels)
            {
                if(levelInfo.dirName.Equals(path)){
                    return true;
                }
            }

            return false;
        }

        public void updateLevel()
        {
            LevelInfo tempLevelInfo = new LevelInfo(state,"C:\\",false);
            bool levelSet = false;

            foreach (LevelInfo levelInfo in allLevels)
            {
                if (levelInfo.dirName.Equals(state.room.currentRoom))
                {
                    levelSet = true;
                    tempLevelInfo = levelInfo;
                }
            }

            if (levelSet)
            {
                allLevels.Remove(tempLevelInfo);

                allLevels.Add(new LevelInfo(state, state.room.currentRoom, true));

            }
        }

        public void createSubDirEnvs()
        {
            String[] dirs = Directory.GetDirectories("C:\\");

            String[] directions = { "up", "down", "left", "right" };

            Random rand = new Random("C:\\".GetHashCode());

            for (int i = 0; i < dirs.Length; i++)
            {
                String extractedDir = dirs[i].Split('\\')[1];
                if (i % 2 == 0)
                {
                    cDriveSubDirs.Add(new CDriveSubDirs(extractedDir, "dungeon", directions[rand.Next(0,4)]));
                }
                else
                {
                    cDriveSubDirs.Add(new CDriveSubDirs(extractedDir, "cave", directions[rand.Next(0,4)]));
                }


            }
        }

        public String getOppositeDir(String dir)
        {
            switch (dir)
            {
                case "up":
                    return "down";
                case "down":
                    return "up";
                case "left":
                    return "right";
                case "right":
                    return "left";

            }

            return "up";
        }

        public String getEnvironmentType(String path)
        {
            if(path.Equals("C:\\")){
                return "courtyard";
            }

            String [] splitPath;
            splitPath = path.Split('\\');
            String dirName = splitPath[1];

            //Console.WriteLine("count = " + cDriveSubDirs.Count);

            foreach(CDriveSubDirs cSubDirs in cDriveSubDirs){
                //Console.WriteLine(cSubDirs.dirName + " != " + dirName);
                if(cSubDirs.dirName.Equals(dirName)){
                    return cSubDirs.environment;
                }
            }

            return "cave";
        }

        public String getDirection(String path)
        {
            if (path.Equals("C:\\"))
            {
                return "up";
            }

            String[] splitPath;
            splitPath = path.Split('\\');
            String dirName = splitPath[1];

            //Console.WriteLine("count = " + cDriveSubDirs.Count);

            foreach (CDriveSubDirs cSubDirs in cDriveSubDirs)
            {
                //Console.WriteLine(cSubDirs.dirName + " != " + dirName);
                if (cSubDirs.dirName.Equals(dirName))
                {
                    return cSubDirs.direction;
                }
            }

            return "down";
        }


    }
}
