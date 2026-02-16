using System;
using System.Collections.Generic;
using Cosmos.System;
using Cosmos.Core;
using Sys = Cosmos.System;

namespace TextyOS
{
    public class Kernel : Sys.Kernel
    {
        private string userName = "User";
        private string currentDirectory = @"C:\";
        private string computerName = "TEXTYOS-PC";

        private Dictionary<string, string> environmentVariables = new Dictionary<string, string>();
        private List<string> commandHistory = new List<string>();
        private int historyIndex = -1;

        // File system simulation
        private Dictionary<string, List<FileEntry>> fileSystem = new Dictionary<string, List<FileEntry>>();

        class FileEntry
        {
            public string Name;
            public string Attributes;
            public string Size;
            public string Date;
            public bool IsDirectory;
        }

        protected override void BeforeRun()
        {
            System.Console.Clear();
            System.Console.ForegroundColor = System.ConsoleColor.White;

            InitializeEnvironmentVariables();
            InitializeFileSystem();

            System.Console.WriteLine($"Microsoft Windows [Version 10.0.19045.6456]");
            System.Console.WriteLine("(c) Microsoft Corporation. All rights reserved.");
            System.Console.WriteLine("");
        }

        private void InitializeEnvironmentVariables()
        {
            environmentVariables["PATH"] = @"C:\Windows\system32;C:\Windows";
            environmentVariables["USERNAME"] = userName;
            environmentVariables["COMPUTERNAME"] = computerName;
            environmentVariables["OS"] = "Windows_NT";
            environmentVariables["PROCESSOR_ARCHITECTURE"] = "x86";
            environmentVariables["NUMBER_OF_PROCESSORS"] = "1";
            environmentVariables["WINDIR"] = @"C:\Windows";
            environmentVariables["SYSTEMROOT"] = @"C:\Windows";
            environmentVariables["TEMP"] = @"C:\Users\" + userName + @"\AppData\Local\Temp";
            environmentVariables["TMP"] = @"C:\Users\" + userName + @"\AppData\Local\Temp";
        }

        private void InitializeFileSystem()
        {
            // Create root directory
            var rootDir = new List<FileEntry>();
            rootDir.Add(new FileEntry { Name = "Windows", Attributes = "<DIR>", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = true });
            rootDir.Add(new FileEntry { Name = "Users", Attributes = "<DIR>", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = true });
            rootDir.Add(new FileEntry { Name = "Program Files", Attributes = "<DIR>", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = true });
            rootDir.Add(new FileEntry { Name = "autoexec.bat", Attributes = "", Size = "256", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = false });
            rootDir.Add(new FileEntry { Name = "config.sys", Attributes = "", Size = "128", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = false });
            fileSystem[@"C:\"] = rootDir;

            // Create Windows directory
            var windowsDir = new List<FileEntry>();
            windowsDir.Add(new FileEntry { Name = "system32", Attributes = "<DIR>", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = true });
            windowsDir.Add(new FileEntry { Name = "System", Attributes = "<DIR>", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = true });
            windowsDir.Add(new FileEntry { Name = "notepad.exe", Attributes = "", Size = "187,456", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = false });
            windowsDir.Add(new FileEntry { Name = "calc.exe", Attributes = "", Size = "98,304", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = false });
            windowsDir.Add(new FileEntry { Name = "cmd.exe", Attributes = "", Size = "245,760", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = false });
            fileSystem[@"C:\Windows"] = windowsDir;

            // Create Users directory
            var usersDir = new List<FileEntry>();
            usersDir.Add(new FileEntry { Name = userName, Attributes = "<DIR>", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = true });
            usersDir.Add(new FileEntry { Name = "Public", Attributes = "<DIR>", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = true });
            fileSystem[@"C:\Users"] = usersDir;

            // Create user directory
            var userDir = new List<FileEntry>();
            userDir.Add(new FileEntry { Name = "Desktop", Attributes = "<DIR>", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = true });
            userDir.Add(new FileEntry { Name = "Documents", Attributes = "<DIR>", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = true });
            userDir.Add(new FileEntry { Name = "Downloads", Attributes = "<DIR>", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = true });
            userDir.Add(new FileEntry { Name = "Favorites", Attributes = "<DIR>", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = true });
            userDir.Add(new FileEntry { Name = "Music", Attributes = "<DIR>", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = true });
            userDir.Add(new FileEntry { Name = "Pictures", Attributes = "<DIR>", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = true });
            userDir.Add(new FileEntry { Name = "Videos", Attributes = "<DIR>", Date = DateTime.Now.ToString("MM/dd/yyyy  hh:mm tt"), IsDirectory = true });
            fileSystem[@"C:\Users\" + userName] = userDir;
        }

        protected override void Run()
        {
            try
            {
                environmentVariables["USERNAME"] = userName;

                System.Console.Write(currentDirectory + ">");

                string input = ReadLineWithHistory();

                if (!string.IsNullOrEmpty(input))
                {
                    if (commandHistory.Count == 0 || commandHistory[commandHistory.Count - 1] != input)
                    {
                        commandHistory.Add(input);
                    }
                    historyIndex = commandHistory.Count;
                }

                ProcessCommand(input);
            }
            catch (Exception ex)
            {
                System.Console.ForegroundColor = System.ConsoleColor.Red;
                System.Console.WriteLine($"Error: {ex.Message}");
                System.Console.ForegroundColor = System.ConsoleColor.White;
            }
        }

        private string ReadLineWithHistory()
        {
            string input = "";
            int cursorPos = 0;

            while (true)
            {
                var keyInfo = System.Console.ReadKey(true);

                if (keyInfo.Key == System.ConsoleKey.Enter)
                {
                    System.Console.WriteLine();
                    break;
                }
                else if (keyInfo.Key == System.ConsoleKey.UpArrow)
                {
                    if (commandHistory.Count > 0 && historyIndex > 0)
                    {
                        historyIndex--;
                        input = commandHistory[historyIndex];
                        cursorPos = input.Length;
                        RedrawInputLine(input, cursorPos);
                    }
                }
                else if (keyInfo.Key == System.ConsoleKey.DownArrow)
                {
                    if (historyIndex < commandHistory.Count - 1)
                    {
                        historyIndex++;
                        input = commandHistory[historyIndex];
                        cursorPos = input.Length;
                        RedrawInputLine(input, cursorPos);
                    }
                    else if (historyIndex == commandHistory.Count - 1)
                    {
                        historyIndex++;
                        input = "";
                        cursorPos = 0;
                        RedrawInputLine(input, cursorPos);
                    }
                }
                else if (keyInfo.Key == System.ConsoleKey.Backspace && cursorPos > 0)
                {
                    input = input.Remove(cursorPos - 1, 1);
                    cursorPos--;
                    RedrawInputLine(input, cursorPos);
                }
                else if (keyInfo.Key == System.ConsoleKey.Delete && cursorPos < input.Length)
                {
                    input = input.Remove(cursorPos, 1);
                    RedrawInputLine(input, cursorPos);
                }
                else if (keyInfo.Key == System.ConsoleKey.LeftArrow && cursorPos > 0)
                {
                    cursorPos--;
                    int currentLineCursor = System.Console.CursorTop;
                    System.Console.SetCursorPosition(currentDirectory.Length + 1 + cursorPos, currentLineCursor);
                }
                else if (keyInfo.Key == System.ConsoleKey.RightArrow && cursorPos < input.Length)
                {
                    cursorPos++;
                    int currentLineCursor = System.Console.CursorTop;
                    System.Console.SetCursorPosition(currentDirectory.Length + 1 + cursorPos, currentLineCursor);
                }
                else if (keyInfo.Key == System.ConsoleKey.Tab)
                {
                    // Simple tab completion
                    string[] commonCommands = { "help", "dir", "cd", "cls", "echo", "set", "path", "ver", "time", "date",
                                               "exit", "shutdown", "reboot", "color", "title", "prompt", "copy", "del",
                                               "rename", "type", "find", "sort", "attrib", "xcopy", "mkdir", "rmdir" };

                    foreach (string cmd in commonCommands)
                    {
                        if (cmd.StartsWith(input, StringComparison.OrdinalIgnoreCase))
                        {
                            input = cmd;
                            cursorPos = input.Length;
                            RedrawInputLine(input, cursorPos);
                            break;
                        }
                    }
                }
                else if (keyInfo.KeyChar != 0 && !char.IsControl(keyInfo.KeyChar))
                {
                    input = input.Insert(cursorPos, keyInfo.KeyChar.ToString());
                    cursorPos++;
                    RedrawInputLine(input, cursorPos);
                }
            }

            return input.Trim();
        }

        private void RedrawInputLine(string input, int cursorPos)
        {
            int currentLineCursor = System.Console.CursorTop;
            System.Console.SetCursorPosition(0, currentLineCursor);
            System.Console.Write(new string(' ', System.Console.WindowWidth));
            System.Console.SetCursorPosition(0, currentLineCursor);
            System.Console.Write(currentDirectory + ">" + input);
            System.Console.SetCursorPosition(currentDirectory.Length + 1 + cursorPos, currentLineCursor);
        }

        private void ProcessCommand(string cmd)
        {
            if (string.IsNullOrEmpty(cmd))
                return;

            // Echo command
            if (cmd.StartsWith("echo ", StringComparison.OrdinalIgnoreCase))
            {
                string echoText = cmd.Substring(5);

                // Check for environment variable
                if (echoText.StartsWith("%") && echoText.EndsWith("%"))
                {
                    string varName = echoText.Trim('%');
                    if (environmentVariables.ContainsKey(varName))
                    {
                        System.Console.WriteLine(environmentVariables[varName]);
                    }
                    else
                    {
                        System.Console.WriteLine("%" + varName + "%");
                    }
                }
                else if (string.IsNullOrEmpty(echoText))
                {
                    System.Console.WriteLine("ECHO is on.");
                }
                else
                {
                    System.Console.WriteLine(echoText);
                }
                return;
            }

            // Set environment variable
            if (cmd.Contains("=") && !cmd.StartsWith("echo ") && !cmd.StartsWith("set "))
            {
                string[] parts = cmd.Split(new[] { '=' }, 2);
                if (parts.Length == 2 && !string.IsNullOrEmpty(parts[0]))
                {
                    string varName = parts[0].Trim();
                    string varValue = parts[1].Trim();
                    environmentVariables[varName] = varValue;
                }
                return;
            }

            string[] parts2 = cmd.Split(' ');
            string command = parts2[0].ToLower();
            string arguments = parts2.Length > 1 ? cmd.Substring(command.Length).Trim() : "";

            switch (command)
            {
                case "help":
                    ShowHelp(arguments);
                    break;

                case "dir":
                    DirCommand(arguments);
                    break;

                case "cd":
                case "chdir":
                    CdCommand(arguments);
                    break;

                case "cls":
                    System.Console.Clear();
                    break;

                case "ver":
                    System.Console.WriteLine("Microsoft Windows [Version 10.0.19045.6456]");
                    break;

                case "time":
                    TimeCommand();
                    break;

                case "date":
                    DateCommand();
                    break;

                case "set":
                    SetCommand(arguments);
                    break;

                case "path":
                    if (environmentVariables.ContainsKey("PATH"))
                    {
                        System.Console.WriteLine("PATH=" + environmentVariables["PATH"]);
                    }
                    break;

                case "exit":
                    System.Console.WriteLine("Exiting...");
                    break;

                case "shutdown":
                    System.Console.WriteLine("Shutting down...");
                    Cosmos.System.Power.Shutdown();
                    break;

                case "reboot":
                    System.Console.WriteLine("Rebooting...");
                    Cosmos.System.Power.Reboot();
                    break;

                case "color":
                    ColorCommand(arguments);
                    break;

                case "title":
                    if (!string.IsNullOrEmpty(arguments))
                    {
                        System.Console.Title = arguments;
                    }
                    break;

                case "prompt":
                    PromptCommand(arguments);
                    break;

                case "copy":
                    CopyCommand(arguments);
                    break;

                case "del":
                case "erase":
                    DeleteCommand(arguments);
                    break;

                case "rename":
                case "ren":
                    RenameCommand(arguments);
                    break;

                case "type":
                    TypeCommand(arguments);
                    break;

                case "find":
                    FindCommand(arguments);
                    break;

                case "sort":
                    SortCommand(arguments);
                    break;

                case "attrib":
                    AttribCommand(arguments);
                    break;

                case "xcopy":
                    XCopyCommand(arguments);
                    break;

                case "mkdir":
                case "md":
                    MkdirCommand(arguments);
                    break;

                case "rmdir":
                case "rd":
                    RmdirCommand(arguments);
                    break;

                case "vol":
                    System.Console.WriteLine(" Volume in drive C has no label.");
                    System.Console.WriteLine(" Volume Serial Number is 1234-5678");
                    break;

                case "label":
                    System.Console.WriteLine("Label command not fully implemented in this version.");
                    break;

                case "chkdsk":
                    System.Console.WriteLine("Chkdsk not implemented in this version.");
                    break;

                case "tree":
                    TreeCommand(arguments);
                    break;

                case "move":
                    MoveCommand(arguments);
                    break;

                case "fc":
                    System.Console.WriteLine("FC command not fully implemented in this version.");
                    break;

                case "comp":
                    System.Console.WriteLine("COMP command not fully implemented in this version.");
                    break;

                case "diskpart":
                    System.Console.WriteLine("DiskPart is not available in this version.");
                    break;

                case "systeminfo":
                    SystemInfoCommand();
                    break;

                case "tasklist":
                    TasklistCommand();
                    break;

                case "taskkill":
                    System.Console.WriteLine("TASKKILL not implemented in this version.");
                    break;

                case "sc":
                    System.Console.WriteLine("SC command not implemented in this version.");
                    break;

                case "schtasks":
                    System.Console.WriteLine("Schtasks not implemented in this version.");
                    break;

                case "wmic":
                    System.Console.WriteLine("WMIC is not available in this version.");
                    break;

                case "where":
                    WhereCommand(arguments);
                    break;

                case "whoami":
                    System.Console.WriteLine(computerName + "\\" + userName);
                    break;

                case "hostname":
                    System.Console.WriteLine(computerName);
                    break;

                case "ping":
                    System.Console.WriteLine("Ping not implemented in this version.");
                    break;

                case "ipconfig":
                    System.Console.WriteLine("Windows IP Configuration");
                    System.Console.WriteLine("");
                    System.Console.WriteLine("Ethernet adapter Ethernet:");
                    System.Console.WriteLine("   Connection-specific DNS Suffix  . : ");
                    System.Console.WriteLine("   Link-local IPv6 Address . . . . . : fe80::1%1");
                    System.Console.WriteLine("   IPv4 Address. . . . . . . . . . . : 192.168.1.100");
                    System.Console.WriteLine("   Subnet Mask . . . . . . . . . . . : 255.255.255.0");
                    System.Console.WriteLine("   Default Gateway . . . . . . . . . : 192.168.1.1");
                    break;

                case "netstat":
                    System.Console.WriteLine("Netstat not implemented in this version.");
                    break;

                case "tracert":
                    System.Console.WriteLine("Tracert not implemented in this version.");
                    break;

                case "nslookup":
                    System.Console.WriteLine("NSLookup not implemented in this version.");
                    break;

                case "ftp":
                    System.Console.WriteLine("FTP not implemented in this version.");
                    break;

                case "telnet":
                    System.Console.WriteLine("Telnet not implemented in this version.");
                    break;

                case "assoc":
                    System.Console.WriteLine(".txt=txtfile");
                    System.Console.WriteLine(".exe=exefile");
                    System.Console.WriteLine(".bat=batfile");
                    System.Console.WriteLine(".cmd=cmdfile");
                    break;

                case "ftype":
                    System.Console.WriteLine("txtfile=%SystemRoot%\\system32\\NOTEPAD.EXE %1");
                    System.Console.WriteLine("exefile=\"%1\" %*");
                    System.Console.WriteLine("batfile=\"%1\" %*");
                    break;

                case "pause":
                    System.Console.WriteLine("Press any key to continue . . .");
                    System.Console.ReadKey(true);
                    break;

                case "rem":
                    // Remark/comment - do nothing
                    break;

                case "":
                    break;

                default:
                    System.Console.WriteLine($"'{command}' is not recognized as an internal or external command, operable program or batch file.");
                    break;
            }
        }

        private void DirCommand(string arguments)
        {
            string dirPath = currentDirectory;
            bool showAll = false;
            bool bareFormat = false;

            if (!string.IsNullOrEmpty(arguments))
            {
                string[] args = arguments.Split(' ');
                foreach (string arg in args)
                {
                    if (arg == "/a" || arg == "/a:h" || arg == "/a:-h" || arg == "/a:s" || arg == "/a:d")
                        showAll = true;
                    else if (arg == "/b")
                        bareFormat = true;
                    else if (arg.StartsWith(@"\"))
                        dirPath = @"C:" + arg;
                    else if (arg.Contains(":"))
                        dirPath = arg;
                }
            }

            if (!fileSystem.ContainsKey(dirPath))
            {
                System.Console.WriteLine(" File Not Found");
                return;
            }

            if (!bareFormat)
            {
                System.Console.WriteLine(" Volume in drive C has no label.");
                System.Console.WriteLine(" Volume Serial Number is 1234-5678");
                System.Console.WriteLine("");
                System.Console.WriteLine(" Directory of " + dirPath);
                System.Console.WriteLine("");
            }

            var files = fileSystem[dirPath];
            int fileCount = 0;
            int dirCount = 0;
            long totalSize = 0;

            foreach (var file in files)
            {
                if (bareFormat)
                {
                    System.Console.WriteLine(file.Name);
                }
                else
                {
                    if (file.IsDirectory)
                    {
                        System.Console.WriteLine(file.Date + "    " + file.Attributes.PadRight(8) + " " + file.Name);
                        dirCount++;
                    }
                    else
                    {
                        string sizeStr = file.Size.PadLeft(15);
                        System.Console.WriteLine(file.Date + "    " + sizeStr + " " + file.Name);
                        fileCount++;
                        if (long.TryParse(file.Size.Replace(",", ""), out long size))
                            totalSize += size;
                    }
                }
            }

            if (!bareFormat)
            {
                System.Console.WriteLine("               " + fileCount + " File(s)     " + totalSize.ToString("N0") + " bytes");
                System.Console.WriteLine("               " + dirCount + " Dir(s)      " + "unlimited bytes free");
                System.Console.WriteLine("");
            }
        }

        private void CdCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                System.Console.WriteLine(currentDirectory);
                return;
            }

            string newPath = arguments;

            // Handle cd..
            if (arguments == ".." || arguments == "cd..")
            {
                if (currentDirectory != @"C:\")
                {
                    int lastSlash = currentDirectory.LastIndexOf('\\');
                    if (lastSlash > 0)
                        currentDirectory = currentDirectory.Substring(0, lastSlash);
                    else
                        currentDirectory = @"C:\";
                }
                return;
            }

            // Handle cd \
            if (arguments == @"\" || arguments == @"\")
            {
                currentDirectory = @"C:\";
                return;
            }

            // Handle relative paths
            if (!newPath.Contains(":"))
            {
                if (newPath.StartsWith(@"\"))
                    newPath = @"C:" + newPath;
                else if (currentDirectory.EndsWith(@"\"))
                    newPath = currentDirectory + newPath;
                else
                    newPath = currentDirectory + @"\" + newPath;
            }

            // Normalize path
            newPath = newPath.Replace('/', '\\');

            // Check if directory exists in our simulated filesystem
            if (fileSystem.ContainsKey(newPath))
            {
                currentDirectory = newPath;
            }
            else
            {
                // Try to find by partial match
                bool found = false;
                foreach (var dir in fileSystem.Keys)
                {
                    if (dir.Equals(newPath, StringComparison.OrdinalIgnoreCase))
                    {
                        currentDirectory = dir;
                        found = true;
                        break;
                    }
                }
                if (!found)
                    System.Console.WriteLine("The system cannot find the path specified.");
            }
        }

        private void SetCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                foreach (var env in environmentVariables)
                {
                    System.Console.WriteLine($"{env.Key}={env.Value}");
                }
            }
            else if (arguments.Contains("="))
            {
                string[] parts = arguments.Split(new[] { '=' }, 2);
                string varName = parts[0].Trim();
                string varValue = parts.Length > 1 ? parts[1].Trim() : "";
                environmentVariables[varName] = varValue;
            }
            else
            {
                // Show specific variable
                if (environmentVariables.ContainsKey(arguments))
                {
                    System.Console.WriteLine($"{arguments}={environmentVariables[arguments]}");
                }
                else
                {
                    System.Console.WriteLine($"Environment variable {arguments} not defined");
                }
            }
        }

        private void ColorCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                System.Console.WriteLine("Sets the default console foreground and background colors.");
                System.Console.WriteLine("");
                System.Console.WriteLine("COLOR [attr]");
                System.Console.WriteLine("");
                System.Console.WriteLine("  attr        Specifies color attribute of console output");
                System.Console.WriteLine("");
                System.Console.WriteLine("Color attributes are specified as TWO hex digits -- the first");
                System.Console.WriteLine("corresponds to the background; the second the foreground. Each");
                System.Console.WriteLine("digit can be any of the following values:");
                System.Console.WriteLine("    0 = Black       8 = Gray");
                System.Console.WriteLine("    1 = Blue        9 = Light Blue");
                System.Console.WriteLine("    2 = Green       A = Light Green");
                System.Console.WriteLine("    3 = Aqua        B = Light Aqua");
                System.Console.WriteLine("    4 = Red         C = Light Red");
                System.Console.WriteLine("    5 = Purple      D = Light Purple");
                System.Console.WriteLine("    6 = Yellow      E = Light Yellow");
                System.Console.WriteLine("    7 = White       F = Bright White");
                return;
            }

            if (arguments.Length >= 2)
            {
                char bgChar = arguments[0];
                char fgChar = arguments[1];

                System.ConsoleColor bg = CharToColor(bgChar);
                System.ConsoleColor fg = CharToColor(fgChar);

                System.Console.BackgroundColor = bg;
                System.Console.ForegroundColor = fg;
                System.Console.Clear();
            }
        }

        private System.ConsoleColor CharToColor(char c)
        {
            return c switch
            {
                '0' => System.ConsoleColor.Black,
                '1' => System.ConsoleColor.DarkBlue,
                '2' => System.ConsoleColor.DarkGreen,
                '3' => System.ConsoleColor.DarkCyan,
                '4' => System.ConsoleColor.DarkRed,
                '5' => System.ConsoleColor.DarkMagenta,
                '6' => System.ConsoleColor.DarkYellow,
                '7' => System.ConsoleColor.Gray,
                '8' => System.ConsoleColor.DarkGray,
                '9' => System.ConsoleColor.Blue,
                'A' or 'a' => System.ConsoleColor.Green,
                'B' or 'b' => System.ConsoleColor.Cyan,
                'C' or 'c' => System.ConsoleColor.Red,
                'D' or 'd' => System.ConsoleColor.Magenta,
                'E' or 'e' => System.ConsoleColor.Yellow,
                'F' or 'f' => System.ConsoleColor.White,
                _ => System.ConsoleColor.White,
            };
        }

        private void PromptCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                System.Console.WriteLine("Changes the command prompt.");
                System.Console.WriteLine("");
                System.Console.WriteLine("PROMPT [text]");
                System.Console.WriteLine("");
                System.Console.WriteLine("  text    Specifies a new command prompt.");
                System.Console.WriteLine("");
                System.Console.WriteLine("Prompt can be made up of normal characters and the");
                System.Console.WriteLine("following special codes:");
                System.Console.WriteLine("  $A & (Ampersand)");
                System.Console.WriteLine("  $B | (pipe)");
                System.Console.WriteLine("  $C ( (Left parenthesis)");
                System.Console.WriteLine("  $D Current date");
                System.Console.WriteLine("  $E Escape code (ASCII code 27)");
                System.Console.WriteLine("  $F ) (Right parenthesis)");
                System.Console.WriteLine("  $G > (greater-than sign)");
                System.Console.WriteLine("  $H Backspace (erases previous character)");
                System.Console.WriteLine("  $L < (less-than sign)");
                System.Console.WriteLine("  $N Current drive");
                System.Console.WriteLine("  $P Current drive and path");
                System.Console.WriteLine("  $Q = (equals sign)");
                System.Console.WriteLine("  $T Current time");
                System.Console.WriteLine("  $V Windows version number");
                System.Console.WriteLine("  $_ Carriage return and linefeed");
                System.Console.WriteLine("  $$ $ (dollar sign)");
                return;
            }

            // For simplicity, we'll just show that prompt changes are not fully supported
            System.Console.WriteLine("Prompt customization is not fully supported in this version.");
        }

        private void CopyCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                System.Console.WriteLine("Copies one or more files to another location.");
                System.Console.WriteLine("");
                System.Console.WriteLine("COPY source destination");
                return;
            }

            System.Console.WriteLine("        1 file(s) copied.");
        }

        private void DeleteCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                System.Console.WriteLine("Deletes one or more files.");
                System.Console.WriteLine("");
                System.Console.WriteLine("DEL [/P] [/F] [/S] [/Q] [/A[[:]attributes]] names");
                return;
            }

            System.Console.WriteLine("        1 file(s) deleted.");
        }

        private void RenameCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                System.Console.WriteLine("Renames a file or files.");
                System.Console.WriteLine("");
                System.Console.WriteLine("RENAME [drive:][path]filename1 filename2");
                return;
            }

            System.Console.WriteLine("File renamed successfully.");
        }

        private void TypeCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                System.Console.WriteLine("Displays the contents of a text file.");
                System.Console.WriteLine("");
                System.Console.WriteLine("TYPE [drive:][path]filename");
                return;
            }

            System.Console.WriteLine("This is a sample text file.");
            System.Console.WriteLine("TYPE command displays the contents.");
            System.Console.WriteLine("Line 3 of the file.");
        }

        private void FindCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments) || !arguments.Contains("\""))
            {
                System.Console.WriteLine("Searches for a text string in a file or files.");
                System.Console.WriteLine("");
                System.Console.WriteLine("FIND [/V] [/C] [/N] [/I] [/OFF[LINE]] \"string\" [[drive:][path]filename[ ...]]");
                return;
            }

            System.Console.WriteLine("---------- FILENAME.TXT");
            System.Console.WriteLine("    1:  This line contains the search string.");
            System.Console.WriteLine("    2:  Another line with the string.");
        }

        private void SortCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                System.Console.WriteLine("Sorts input.");
                System.Console.WriteLine("");
                System.Console.WriteLine("SORT [/R] [/+n] [/M kilobytes] [/L locale] [/REC recordbytes]");
                System.Console.WriteLine("    [[drive1:][path1]filename1] [/T [drive2:][path2]]");
                System.Console.WriteLine("    [/O [drive3:][path3]filename3]");
                return;
            }

            System.Console.WriteLine("Apple");
            System.Console.WriteLine("Banana");
            System.Console.WriteLine("Cherry");
            System.Console.WriteLine("Date");
        }

        private void AttribCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                System.Console.WriteLine("Displays or changes file attributes.");
                System.Console.WriteLine("");
                System.Console.WriteLine("ATTRIB [+R | -R] [+A | -A] [+S | -S] [+H | -H] [+I | -I]");
                System.Console.WriteLine("       [drive:][path][filename] [/S [/D] [/L]]");
                return;
            }

            System.Console.WriteLine("     A    R    H    " + arguments);
        }

        private void XCopyCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                System.Console.WriteLine("Copies files and directory trees.");
                System.Console.WriteLine("");
                System.Console.WriteLine("XCOPY source [destination] [/A | /M] [/D[:date]] [/P] [/S [/E]] [/V] [/W]");
                return;
            }

            System.Console.WriteLine("        5 file(s) copied.");
        }

        private void MkdirCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                System.Console.WriteLine("Creates a directory.");
                System.Console.WriteLine("");
                System.Console.WriteLine("MKDIR [drive:]path");
                return;
            }

            System.Console.WriteLine("Directory created successfully.");
        }

        private void RmdirCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                System.Console.WriteLine("Removes (deletes) a directory.");
                System.Console.WriteLine("");
                System.Console.WriteLine("RMDIR [/S] [/Q] [drive:]path");
                return;
            }

            System.Console.WriteLine("Directory removed successfully.");
        }

        private void TreeCommand(string arguments)
        {
            string path = currentDirectory;

            System.Console.WriteLine("Folder PATH listing for volume Windows");
            System.Console.WriteLine("Volume serial number is 1234-5678");
            System.Console.WriteLine(path + "");
            System.Console.WriteLine("├───Windows");
            System.Console.WriteLine("│   ├───System32");
            System.Console.WriteLine("│   ├───System");
            System.Console.WriteLine("│   └───Temp");
            System.Console.WriteLine("├───Users");
            System.Console.WriteLine("│   ├───" + userName);
            System.Console.WriteLine("│   │   ├───Desktop");
            System.Console.WriteLine("│   │   ├───Documents");
            System.Console.WriteLine("│   │   ├───Downloads");
            System.Console.WriteLine("│   │   ├───Music");
            System.Console.WriteLine("│   │   ├───Pictures");
            System.Console.WriteLine("│   │   └───Videos");
            System.Console.WriteLine("│   └───Public");
            System.Console.WriteLine("└───Program Files");
        }

        private void MoveCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                System.Console.WriteLine("Moves files and renames files and directories.");
                System.Console.WriteLine("");
                System.Console.WriteLine("To move one or more files:");
                System.Console.WriteLine("MOVE [/Y | /-Y] [drive:][path]filename1[,...] destination");
                return;
            }

            System.Console.WriteLine("        1 file(s) moved.");
        }

        private void SystemInfoCommand()
        {
            System.Console.WriteLine("Host Name:                 " + computerName);
            System.Console.WriteLine("OS Name:                   Microsoft Windows 10 Pro");
            System.Console.WriteLine("OS Version:                10.0.19045 N/A Build 19045");
            System.Console.WriteLine("OS Manufacturer:           Microsoft Corporation");
            System.Console.WriteLine("OS Configuration:          Standalone Workstation");
            System.Console.WriteLine("OS Build Type:             Multiprocessor Free");
            System.Console.WriteLine("Registered Owner:          " + userName);
            System.Console.WriteLine("Registered Organization:   ");
            System.Console.WriteLine("Product ID:                12345-67890-12345-67890");
            System.Console.WriteLine("Original Install Date:     1/1/2023, 12:00:00 AM");
            System.Console.WriteLine("System Boot Time:          " + DateTime.Now.ToString("M/d/yyyy, h:mm:ss tt"));
            System.Console.WriteLine("System Manufacturer:       TextyOS Virtual Machine");
            System.Console.WriteLine("System Model:              TextyOS PC");
            System.Console.WriteLine("System Type:               x86-based PC");
            System.Console.WriteLine("Processor(s):              1 Processor(s) Installed.");
            System.Console.WriteLine("                           [01]: x86 Family 0 Model 0 Stepping 0 GenuineIntel ~1000 Mhz");
            System.Console.WriteLine("BIOS Version:              TextyOS BIOS 1.0, 1/1/2023");
            System.Console.WriteLine("Windows Directory:         C:\\Windows");
            System.Console.WriteLine("System Directory:          C:\\Windows\\system32");
            System.Console.WriteLine("Boot Device:               \\Device\\HarddiskVolume1");
            System.Console.WriteLine("System Locale:             en-us;English (United States)");
            System.Console.WriteLine("Input Locale:              en-us;English (United States)");
            System.Console.WriteLine("Time Zone:                 (UTC-05:00) Eastern Time (US & Canada)");
            System.Console.WriteLine("Total Physical Memory:     1,024 MB");
            System.Console.WriteLine("Available Physical Memory: 512 MB");
            System.Console.WriteLine("Virtual Memory: Max Size:  2,048 MB");
            System.Console.WriteLine("Virtual Memory: Available: 1,024 MB");
            System.Console.WriteLine("Virtual Memory: In Use:    1,024 MB");
            System.Console.WriteLine("Page File Location(s):     C:\\pagefile.sys");
            System.Console.WriteLine("Domain:                    WORKGROUP");
            System.Console.WriteLine("Logon Server:              \\\\" + computerName);
            System.Console.WriteLine("Hotfix(s):                 N/A");
            System.Console.WriteLine("Network Card(s):           1 NIC(s) installed.");
            System.Console.WriteLine("                           [01]: Intel PRO/1000 MT Desktop Adapter");
            System.Console.WriteLine("                                 Connection Name: Ethernet");
            System.Console.WriteLine("                                 DHCP Enabled:    Yes");
            System.Console.WriteLine("                                 IP Address(es)");
            System.Console.WriteLine("                                 [01]: 192.168.1.100");
        }

        private void TasklistCommand()
        {
            System.Console.WriteLine("");
            System.Console.WriteLine("Image Name                     PID Session Name        Session#    Mem Usage");
            System.Console.WriteLine("========================= ======== ================ =========== ============");
            System.Console.WriteLine("System Idle Process              0 Services                   0         24 K");
            System.Console.WriteLine("System                           4 Services                   0      1,024 K");
            System.Console.WriteLine("smss.exe                       248 Services                   0        452 K");
            System.Console.WriteLine("csrss.exe                      332 Services                   0      3,456 K");
            System.Console.WriteLine("wininit.exe                    396 Services                   0      2,104 K");
            System.Console.WriteLine("csrss.exe                      404 Console                    1     42,108 K");
            System.Console.WriteLine("winlogon.exe                   468 Console                    1      5,672 K");
            System.Console.WriteLine("services.exe                   508 Services                   0      6,540 K");
            System.Console.WriteLine("lsass.exe                      524 Services                   0     10,236 K");
            System.Console.WriteLine("svchost.exe                    672 Services                   0      8,432 K");
            System.Console.WriteLine("svchost.exe                    744 Services                   0      6,124 K");
            System.Console.WriteLine("dwm.exe                        852 Console                    1     98,304 K");
            System.Console.WriteLine("explorer.exe                   912 Console                    1    124,560 K");
            System.Console.WriteLine("cmd.exe                       1234 Console                    1      3,456 K");
            System.Console.WriteLine("TextyOS.exe                   1300 Console                    1     65,536 K");
            System.Console.WriteLine("");
        }

        private void WhereCommand(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                System.Console.WriteLine("Displays the location of files that match the search pattern.");
                System.Console.WriteLine("");
                System.Console.WriteLine("WHERE [/R dir] [/Q] [/F] [/T] pattern...");
                return;
            }

            System.Console.WriteLine(@"C:\Windows\System32\" + arguments + ".exe");
            System.Console.WriteLine(@"C:\Windows\" + arguments + ".exe");
        }

        private void TimeCommand()
        {
            System.Console.WriteLine("The current time is: " + DateTime.Now.ToString("HH:mm:ss.ff"));
            System.Console.Write("Enter the new time: ");
            string input = System.Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                System.Console.WriteLine("Invalid time format. (Use hours:minutes:seconds)");
            }
        }

        private void DateCommand()
        {
            System.Console.WriteLine("The current date is: " + DateTime.Now.ToString("ddd MM/dd/yyyy"));
            System.Console.Write("Enter the new date (mm-dd-yy): ");
            string input = System.Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                System.Console.WriteLine("Invalid date format. (Use mm-dd-yy format)");
            }
        }

        private void ShowHelp(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                System.Console.WriteLine("");
                System.Console.WriteLine("For more information on a specific command, type HELP command-name");
                System.Console.WriteLine("");
                System.Console.WriteLine("ASSOC          Displays or modifies file extension associations.");
                System.Console.WriteLine("ATTRIB         Displays or changes file attributes.");
                System.Console.WriteLine("BREAK          Sets or clears extended CTRL+C checking.");
                System.Console.WriteLine("BCDEDIT        Sets properties in boot database to control boot loading.");
                System.Console.WriteLine("CACLS          Displays or modifies access control lists (ACLs) of files.");
                System.Console.WriteLine("CALL           Calls one batch program from another.");
                System.Console.WriteLine("CD             Displays the name of or changes the current directory.");
                System.Console.WriteLine("CHCP           Displays or sets the active code page number.");
                System.Console.WriteLine("CHDIR          Displays the name of or changes the current directory.");
                System.Console.WriteLine("CHKDSK         Checks a disk and displays a status report.");
                System.Console.WriteLine("CHKNTFS        Displays or modifies the checking of disk at boot time.");
                System.Console.WriteLine("CLS            Clears the screen.");
                System.Console.WriteLine("CMD            Starts a new instance of the Windows command interpreter.");
                System.Console.WriteLine("COLOR          Sets the default console foreground and background colors.");
                System.Console.WriteLine("COMP           Compares the contents of two files or sets of files.");
                System.Console.WriteLine("COMPACT        Displays or alters the compression of files on NTFS partitions.");
                System.Console.WriteLine("CONVERT        Converts FAT volumes to NTFS.");
                System.Console.WriteLine("COPY           Copies one or more files to another location.");
                System.Console.WriteLine("DATE           Displays or sets the date.");
                System.Console.WriteLine("DEL            Deletes one or more files.");
                System.Console.WriteLine("DIR            Displays a list of files and subdirectories in a directory.");
                System.Console.WriteLine("DISKPART       Displays or configures Disk Partition properties.");
                System.Console.WriteLine("DOSKEY         Edits command lines, recalls Windows commands, and creates macros.");
                System.Console.WriteLine("DRIVERQUERY    Displays current device driver status and properties.");
                System.Console.WriteLine("ECHO           Displays messages, or turns command echoing on or off.");
                System.Console.WriteLine("ENDLOCAL       Ends localization of environment changes in a batch file.");
                System.Console.WriteLine("ERASE          Deletes one or more files.");
                System.Console.WriteLine("EXIT           Quits the CMD.EXE program (command interpreter).");
                System.Console.WriteLine("FC             Compares two files or sets of files, and displays the differences.");
                System.Console.WriteLine("FIND           Searches for a text string in a file or files.");
                System.Console.WriteLine("FINDSTR        Searches for strings in files.");
                System.Console.WriteLine("FOR            Runs a specified command for each file in a set of files.");
                System.Console.WriteLine("FORMAT         Formats a disk for use with Windows.");
                System.Console.WriteLine("FSUTIL         Displays or configures the file system properties.");
                System.Console.WriteLine("FTYPE          Displays or modifies file types used in file extension associations.");
                System.Console.WriteLine("GOTO           Directs the Windows command interpreter to a labeled line in a batch program.");
                System.Console.WriteLine("GPRESULT       Displays Group Policy information for machine or user.");
                System.Console.WriteLine("GRAFTABL       Enables Windows to display an extended character set in graphics mode.");
                System.Console.WriteLine("HELP           Provides Help information for Windows commands.");
                System.Console.WriteLine("ICACLS         Display, modify, backup, or restore ACLs for files and directories.");
                System.Console.WriteLine("IF             Performs conditional processing in batch programs.");
                System.Console.WriteLine("LABEL          Creates, changes, or deletes the volume label of a disk.");
                System.Console.WriteLine("MD             Creates a directory.");
                System.Console.WriteLine("MKDIR          Creates a directory.");
                System.Console.WriteLine("MKLINK         Creates symbolic links and hard links.");
                System.Console.WriteLine("MODE           Configures a system device.");
                System.Console.WriteLine("MORE           Displays output one screen at a time.");
                System.Console.WriteLine("MOVE           Moves one or more files from one directory to another directory.");
                System.Console.WriteLine("OPENFILES      Displays files opened by remote users for a file share.");
                System.Console.WriteLine("PATH           Displays or sets a search path for executable files.");
                System.Console.WriteLine("PAUSE          Suspends processing of a batch file and displays a message.");
                System.Console.WriteLine("POPD           Restores the previous value of the current directory saved by PUSHD.");
                System.Console.WriteLine("PRINT          Prints a text file.");
                System.Console.WriteLine("PROMPT         Changes the Windows command prompt.");
                System.Console.WriteLine("PUSHD          Saves the current directory then changes it.");
                System.Console.WriteLine("RD             Removes a directory.");
                System.Console.WriteLine("RECOVER        Recovers readable information from a bad or defective disk.");
                System.Console.WriteLine("REM            Records comments (remarks) in batch files or CONFIG.SYS.");
                System.Console.WriteLine("REN            Renames a file or files.");
                System.Console.WriteLine("RENAME         Renames a file or files.");
                System.Console.WriteLine("REPLACE        Replaces files.");
                System.Console.WriteLine("RMDIR          Removes a directory.");
                System.Console.WriteLine("ROBOCOPY       Advanced utility to copy file trees.");
                System.Console.WriteLine("SET            Displays, sets, or removes Windows environment variables.");
                System.Console.WriteLine("SETLOCAL       Begins localization of environment changes in a batch file.");
                System.Console.WriteLine("SC             Displays or configures services (background processes).");
                System.Console.WriteLine("SCHTASKS       Schedules commands and programs to run on a computer.");
                System.Console.WriteLine("SHIFT          Shifts the position of replaceable parameters in batch files.");
                System.Console.WriteLine("SHUTDOWN       Allows proper local or remote shutdown of machine.");
                System.Console.WriteLine("SORT           Sorts input.");
                System.Console.WriteLine("START          Starts a separate window to run a specified program or command.");
                System.Console.WriteLine("SUBST          Associates a path with a drive letter.");
                System.Console.WriteLine("SYSTEMINFO     Displays machine specific properties and configuration.");
                System.Console.WriteLine("TASKLIST       Displays all currently running tasks including services.");
                System.Console.WriteLine("TASKKILL       Kill or stop a running process or application.");
                System.Console.WriteLine("TIME           Displays or sets the system time.");
                System.Console.WriteLine("TITLE          Sets the window title for a CMD.EXE session.");
                System.Console.WriteLine("TREE           Graphically displays the directory structure of a drive or path.");
                System.Console.WriteLine("TYPE           Displays the contents of a text file.");
                System.Console.WriteLine("VER            Displays the Windows version.");
                System.Console.WriteLine("VERIFY         Tells Windows whether to verify that your files are written correctly to a disk.");
                System.Console.WriteLine("VOL            Displays a disk volume label and serial number.");
                System.Console.WriteLine("XCOPY          Copies files and directory trees.");
                System.Console.WriteLine("WMIC           Displays WMI information inside interactive command shell.");
                System.Console.WriteLine("");
                System.Console.WriteLine("For more information on tools see the command-line reference in the online help.");
                System.Console.WriteLine("");
            }
            else
            {
                // Show help for specific command
                switch (command.ToLower())
                {
                    case "dir":
                        System.Console.WriteLine("Displays a list of files and subdirectories in a directory.");
                        System.Console.WriteLine("");
                        System.Console.WriteLine("DIR [drive:][path][filename] [/A[[:]attributes]] [/B] [/C] [/D] [/L] [/N]");
                        System.Console.WriteLine("  [/O[[:]sortorder]] [/P] [/Q] [/R] [/S] [/T[[:]timefield]] [/W] [/X] [/4]");
                        break;
                    case "cd":
                        System.Console.WriteLine("Displays the name of or changes the current directory.");
                        System.Console.WriteLine("");
                        System.Console.WriteLine("CD [/D] [drive:][path]");
                        System.Console.WriteLine("CD [..]");
                        break;
                    // Add more specific help as needed
                    default:
                        System.Console.WriteLine($"Help for {command} is not available in this version.");
                        break;
                }
            }
        }
    }
}