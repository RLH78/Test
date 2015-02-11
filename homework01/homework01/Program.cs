//Rebecca Hoerner
//Homework01 CPTS 323

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;

namespace homework01
{
    class Program
    {
        static void Main(string[] args)
        {
            string line; // the full string that is read in
            int targetCount = 0; // counter for index of Target
            int dataCount = 0; // counter for how many data items have been input
            char[] delimiters = { '=', '#' };
            Target[] targets = new Target[50]; // array of targets
            bool commentLine = false; // checks if the line is comment line

            try
            {
                var fileCheck = File.Exists(args[0]);
                if (fileCheck == false)
                    throw new Exception("This file doesn't appear to exist. Please check the path you entered.");
            }
            
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
                return;
            }
            
            using (TextReader reader = File.OpenText(args[0]))
            {             
                while ((line = reader.ReadLine()) != null) 
                {
                    string[] words = line.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                    commentLine = false;
                    if (line.StartsWith("#")) // checking if line is a comment
                    {
                        commentLine = true;
                    }

                    if (line.ToUpper() == "[TARGET]") // checking for [Target] header
                    {
                        if(dataCount < 9 && dataCount != 0) // check for missing data
                        {
                            Console.WriteLine("WARNING: There was an error reading the file.\nAre you missing some data?");
                        }

                        if (dataCount == 9) // have all the data. Restart count for next target
                        {
                            dataCount = 0;
                            targetCount++;
                        } 
                        
                        targets[targetCount] = new Target();                       
                    }

                    else if (!string.IsNullOrEmpty(line) && commentLine == false) // if not a comment line and not a space, then found data
                    {
                        targets = TargetClassSetUp(words, targets, targetCount);
                        dataCount++;
                    }                        
                }
            }

            Console.WriteLine("Target File Loaded");

            string command = "start"; // string that holds the user's command
            int exit = 0; // exit flag
            while (exit == 0)
            {
                Console.WriteLine("\nList of Commands: ");
                Console.WriteLine("1. Print");
                Console.WriteLine("2. Print <target name>");
                Console.WriteLine("3. Convert <file name>");
                Console.WriteLine("4. Isfriend <target name>");
                Console.WriteLine("5. Exit");
                Console.WriteLine("Please type in one of the commands");
                Console.WriteLine("Example --> Command: Print targetOne");
                Console.Write("Command: ");
                command = Console.ReadLine();

                int caseNum = determineCaseNumber(command); // determine which case to switch to

                switch (caseNum)
                {
                    case 1: // print
                        printTargetNames(targets, targetCount);
                        break;
                    case 2: // print <target name>
                        printTargetData(targets, targetCount, command);
                        break;
                    case 3: // convert <file name>
                        string path = determinePath(command);
                        convertToPig(path);
                        break;
                    case 4: // isfriend <file name>
                        areYouMyFriend(targets, targetCount, command);
                        break;
                    case 5: //exit
                        exit = 1;
                        break;
                    default:
                        Console.WriteLine("Make sure to enter a correct command");
                        break;
                }
            }  
        }
        //Function: convertToPig
        //Input: the path name
        //Opens the file and converts the target label and name to pig latin. Writes the file back to path name.
        public static void convertToPig(string pathName)
        {
            string line = ""; // a full line read from file
            List<string> lineList = new List<string>(); // list of lines
            string oneChar; // the first character in the target's name

            try
            {
                using (TextReader reader = File.OpenText(pathName))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.ToUpper() == "[TARGET]")
                        {
                            line = "[ARGETTAY]";
                        }
                        if (line.Length > 5 && line.ToUpper().Substring(0, 5) == "NAME=")
                        {
                            oneChar = line.Substring(5, 1);

                            if (oneChar.ToUpper() == "A" || oneChar.ToUpper() == "E" || oneChar.ToUpper() == "I" || oneChar.ToUpper() == "O" || oneChar.ToUpper() == "U")
                            {
                                line = string.Concat(line, "way");
                            }
                            else
                            {
                                line = line.Remove(5, 1);
                                line = string.Concat(line, oneChar);
                                line = string.Concat(line, "ay");
                            }
                        }
                        lineList.Add(line);
                    }
                }
                updateTargetFile(lineList, pathName);
            }
            catch
            {
                Console.WriteLine("File not found");
            }           
       }
        //Function: determinePath
        //Input: the user's command string
        //Output: the path name
        //Function stores and returns the new path name for the file to be converted to pig latin
        public static string determinePath(string commandString)
        {
            string pathName = "temp";
            if (commandString.Length > 8) // assumes user entered "convert "
            {
                pathName = commandString.Substring(8);
            }
            else
            {
                Console.WriteLine("Please enter a proper path name");
            }
            return pathName;
        }
        //Function: updateTargetFile
        //Input: a list of strings (file lines) and a path name
        //Function writes pig latin file to target file
        public static void updateTargetFile(List<string> aList, string path)
        {
            using (StreamWriter file = new StreamWriter(path))
            {
                foreach (string line in aList)
                {
                    file.WriteLine(line);
                }               
            }
        }
        //Function: areYouMyFriend
        //Input: array of targets, counter for targets, and command string
        //Function checks if a specific target is a friend
        public static void areYouMyFriend(Target[] target, int count, string commandString)
        {
            bool found = false;
            for (int i = 0; i <= count; i++)
            {
                if (commandString.Length > 9 && target[i].name.ToUpper() == commandString.Substring(9).ToUpper())  // finding target that user wanted information about
                {
                    if(target[i].friend == true)
                    {
                        Console.WriteLine("Aye Captain!");
                    }
                    else if(target[i].friend == false)
                    {
                        Console.WriteLine("Nay, Scallywag!");
                    }
                    else
                    {
                        Console.WriteLine("Something is wrong with this target's friend data");
                    }
                   
                    found = true;
                }
            }
            if (found == false)
            {
                Console.WriteLine("This target doesn't seem to exist");
            }

        }
        //Function: printTargetData
        //Input: target array, counter for targets, command string
        //Function outputs all data about a specific target
        public static void printTargetData(Target[] target, int count, string commandString)
        {
            bool found = false;
            for (int i = 0; i <= count; i++)
            {
                if(target[i].name.ToUpper() == commandString.Substring(6).ToUpper())
                {
                    Console.WriteLine("Name: {0}", target[i].name);
                    Console.WriteLine("X: {0}", target[i].xCoord);
                    Console.WriteLine("Y: {0}", target[i].yCoord);
                    Console.WriteLine("Z: {0}", target[i].zCoord);
                    Console.WriteLine("Friend: {0}", target[i].friend);              
                    Console.WriteLine("Points: {0}", target[i].points);
                    Console.WriteLine("Flash Rate: {0}", target[i].flashRate);
                    Console.WriteLine("Spawn Rate: {0}", target[i].spawnRate);
                    Console.WriteLine("Can Swap Sides When Hit: {0}", target[i].swapSides);                    
                    found = true;
                }
            }
            if(found == false)
            {
                Console.WriteLine("Target does not exist.\nPlease check your spelling and make sure to leave a space after typing 'print'");
            }
        }
        //Function: printTargetNames
        //Input: target array, counter for targets
        //Function prints the name of every available target and asks the user if the names should be sorted.
        public static void printTargetNames(Target[] target, int count)
        {
            Console.WriteLine("Do you want the target names sorted? (Yes/No)");
            string answer = Console.ReadLine();
            answer = answer.ToUpper();

            switch (answer)
            {
                case "YES":
                    List<string> nameList = new List<string>();
                    List<string> orderedList = new List<string>();
                    for (int i = 0; i <= count; i++)
                    {
                        nameList.Add(target[i].name);
                    }
                    orderedList = nameList.OrderBy(x => x).ToList();                   
                    foreach(var x in orderedList)
                    {
                        Console.WriteLine(x);
                    }
                    break;
                case "NO":
                    for(int i = 0; i <= count; i++)
                    {
                        Console.WriteLine(target[i].name);
                    }
                    break;
                default:
                    Console.WriteLine("Make sure to enter yes or no");
                    break;
            }          
        }
        //Function: determineCaseNumber
        //Input: user command string
        //Return: int, the case number to use in a switch statement 
        //Determines which command the user entered and assigns it a case number
        public static int determineCaseNumber(string userCommand)
        {
            int num = 0;
            userCommand = userCommand.ToUpper();

            if(userCommand == "PRINT" && userCommand.Length == 5)
            {
                num = 1;
            }
            else if (userCommand.Length > 5 && userCommand.Substring(0, 5) == "PRINT")
            {
                num = 2;
            }
            else if (userCommand.Length >= 7 && userCommand.Substring(0, 7) == "CONVERT")
            {
                num = 3;
            }
            else if (userCommand.Length >= 8 && userCommand.Substring(0, 8) == "ISFRIEND")
            {
                num = 4;
            }
            else if(userCommand.Length == 4 && userCommand.Substring(0,4) == "EXIT")
            {
                num = 5;
            }
            else
            {
                num = 0;
            }          
            return num;
        }
        //Function: TargetClassSetUp
        //Input: string array holding delimited file line contents, array of targets, counter for targets
        //Return: target array
        //Function determines which label has appeared from the file and sets the current target's proper data member to the correct value
        public static Target[] TargetClassSetUp(string[] data, Target[] target, int counter)
        {
            if (data.ElementAt(0).ToUpper() == "NAME")
            {
                target[counter].name = data.ElementAt(1);         
            }
            else if (data.ElementAt(0).ToUpper() == "X")
            {
                target[counter].xCoord = Convert.ToDouble(data.ElementAt(1));
            }
            else if (data.ElementAt(0).ToUpper() == "Y")
            {
                target[counter].yCoord = Convert.ToDouble(data.ElementAt(1));
            }
            else if (data.ElementAt(0).ToUpper() == "Z")
            {
                target[counter].zCoord = Convert.ToDouble(data.ElementAt(1));
            }
            else if (data.ElementAt(0).ToUpper() == "FRIEND")
            {
                target[counter].friend = Convert.ToBoolean(data.ElementAt(1));
            }
            else if (data.ElementAt(0).ToUpper() == "POINTS")
            {
                target[counter].points = Convert.ToInt32(data.ElementAt(1));
            }
            else if (data.ElementAt(0).ToUpper() == "FLASHRATE")
            {
                target[counter].flashRate = Convert.ToInt32(data.ElementAt(1));
            }
            else if (data.ElementAt(0).ToUpper() == "SPAWNRATE")
            {
                target[counter].spawnRate = Convert.ToInt32(data.ElementAt(1));
            }
            else if (data.ElementAt(0).ToUpper() == "CANSWAPSIDESWHENHIT")
            {
                target[counter].swapSides = Convert.ToBoolean(data.ElementAt(1));
            }
            else
            {
                Console.WriteLine("WARNING: There was an error in a data label");
                Console.WriteLine("Please exit the program and fix your target file");
            }     
            return target;
        }
    }
    //Class Target
    //Holds the data for a single Target
    public class Target
    {
        public Target()
        {
            this.name = "ERROR";
            this.xCoord = -99.0;
            this.yCoord = -99.0;
            this.zCoord = -99.0;
            this.friend = true;
            this.points = -99;
            this.flashRate = -99;
            this.spawnRate = -99;
            this.swapSides = true;
        }
        public string name { get; set; }
        public double xCoord { get; set; }
        public double yCoord { get; set; }
        public double zCoord { get; set; }
        public bool friend { get; set; }
        public int points { get; set; }
        public int flashRate { get; set; }
        public int spawnRate { get; set; }
        public bool swapSides { get; set; }
    }    
}
