using System;
using System.Collections.Generic;

namespace DungeonDrive
{
    public class LevelInfo
    {

        public GameState state;
        public String dirName;
        public bool[] chestsOpened;
        public bool[] roomsDrawn;
        public bool[,] drawingSpace;
        public bool[] doorsOpened;
        public String type;

        public bool initialized = false;


        public LevelInfo(GameState state, String dirName, bool levelShouldBeSet)
        {
            this.state = state;
            this.dirName = dirName;
            if (levelShouldBeSet)
            {
                setLevel();
            }
        }

        public void setLevel()
        {
            type = state.room.environment;
            //setDoors();
            setRooms();
            setChests();
            setDrawingSpace();
            initialized = true;
        }

        public void setDoors()
        {
            doorsOpened = new bool[state.room.numDoors];


            for (int i = 0; i < state.room.numDoors; i++)
            {
                doorsOpened[i] = true;
            }

            foreach(Door door in state.room.doors)
            {
                doorsOpened[door.id] = door.closed;
            }

            foreach(Door door in state.room.doorsNotDrawn)
            {
                doorsOpened[door.id] = door.closed;
            }

        }

        public void setRooms()
        {
            
            roomsDrawn = new bool[state.room.maxStairs * 2];

            for (int i = 0; i < roomsDrawn.Length; i++)
            {
                roomsDrawn[i] = false;
            }

            for (int i = 0; i < state.room.numRooms; i++)
            {
                roomsDrawn[i] = state.room.roomDrawn[i];
            }


        }

        public void setChests()
        {
            chestsOpened = new bool[state.room.maxObstacles];

            for (int i = 0; i < state.room.maxObstacles; i++)
            {
                chestsOpened[i] = true;
            }

            foreach(Obstacle obs in state.room.obstacles)
            {
                if (obs is Chest)
                {
                    Chest chest = (Chest) obs;
                    chestsOpened[obs.id] = chest.closed;
                }
            }

            foreach (Obstacle obs in state.room.obstaclesNotDrawn)
            {
                if (obs is Chest)
                {
                    Chest chest = (Chest)obs;
                    chestsOpened[obs.id] = chest.closed;
                }
            }
        }

        public void setDrawingSpace()
        {
            drawingSpace = new bool[state.room.width, state.room.height];
            for (int i = 0; i < state.room.width; i++)
            {
                for (int j = 0; j < state.room.height; j++)
                {
                    drawingSpace[i, j] = state.room.drawingSpace[i, j];
                }
            }
        }

    }
}
