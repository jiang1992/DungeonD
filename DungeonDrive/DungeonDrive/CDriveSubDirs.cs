using System;
using System.Collections.Generic;

namespace DungeonDrive
{
    public class CDriveSubDirs
    {
        public String dirName;
        public String environment;
        public String direction;

        public CDriveSubDirs(String dirName, String environment, String direction)
        {
            this.dirName = dirName;
            this.environment = environment;
            this.direction = direction;
        }
    }
}
