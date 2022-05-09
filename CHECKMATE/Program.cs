using System;
namespace CHECKMATE
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new GamePlay().Play();
        }
    }
    class GamePlay
    {
        Board boardGame = new Board();
        bool color = true;
        static int counterUpTo50Moves;
        public void Play()
        {
            string input;
            Console.WriteLine(boardGame);
            do
            {
                Console.WriteLine((color ? "White" : "Black") + " player please enter a move or enter draw to offer the {0} player draw:", color ? "Black" : "White");
                Console.WriteLine("start by the current locaition and then the destination, letter first then the number");
                input = Console.ReadLine();
                input = input.Replace(" ", "").ToUpper();
                //draw
                if (input == "DRAW")
                {
                    Console.WriteLine("{0} player would you like a draw too ? YES/NO", color ? "Black" : "White");
                    input = Console.ReadLine();
                    input = input.Replace(" ", "").ToUpper();
                    while (input != "YES" && input != "NO")
                    {
                        Console.WriteLine("wrong input please try again :");
                        Console.WriteLine("{0} player would you like a draw too ? YES/NO", color ? "Black" : "White");
                        input = Console.ReadLine();
                        input = input.Replace(" ", "").ToUpper();
                    }
                    if (input == "YES")
                    {
                        Console.WriteLine("It's a draw !");
                        return;
                    }
                    if (input == "NO")
                    {
                        Console.WriteLine((color ? "White" : "Black") + " player please enter a move:");
                        input = Console.ReadLine();
                        input = input.Replace(" ", "").ToUpper();
                    }
                }
                //input check
                while (!boardGame.checkInput(input))
                {
                    Console.WriteLine("wrong input please try again :");
                    Console.WriteLine((color ? "White" : "Black") + " player please enter a move:");
                    input = Console.ReadLine();
                    input = input.Replace(" ", "").ToUpper();
                }

                //making a move if legal/ trying again
                while (!boardGame.moveTool(boardGame, input[0], int.Parse(input[1] + ""), input[2], int.Parse(input[3] + ""), color))
                {
                    Console.WriteLine("illegal move please try again :");
                    Console.WriteLine((color ? "White" : "Black") + " player please enter a move:");
                    input = Console.ReadLine();
                    input = input.Replace(" ", "").ToUpper();
                    //input check
                    while (!boardGame.checkInput(input))
                    {
                        Console.WriteLine("wrong input please try again :");
                        Console.WriteLine((color ? "White" : "Black") + " player please enter a move:");
                        input = Console.ReadLine();
                        input = input.Replace(" ", "").ToUpper();
                    }
                }
                if (isPat() || isLackOfMoves() || getCounter() >= 50)
                {
                    Console.WriteLine("It's a draw !");
                    return;
                }
                if (isChess(boardGame, !color))
                    Console.WriteLine("It's {0} chess", color ? "White" : "Black");
                color = !color;
            } while (!isWin(boardGame, color));
            Console.WriteLine("Congrats the {0} player win!", color ? "Black" : "White");
            return;
        }
        //check if there is check and returns the location of the tool that threat the king
        public static bool isChess(Board board, bool color)
        {
            King king = findKing(board, color);
            string location = king.getLocation();
            char letter = location[0];
            int digit = int.Parse(location[1] + "");
            for (int i = 1; i <= 8; i++)
            {
                for (char j = 'A'; j <= 'H'; j++)
                {
                    if (board.getToolInCell(j, i) != null)
                    {
                        if (board.getToolInCell(j, i).getColor() != color)
                        {
                            if (board.getToolInCell(j, i).isMoveLegal(board, j, i, letter, digit))
                                return true;
                        }
                    }
                }
            }
            return false;
        }
        public static King findKing(Board board, bool color)
        {
            for (int i = 1; i <= 8; i++)
            {
                for (char j = 'A'; j <= 'H'; j++)
                {
                    if (board.getToolInCell(j, i) is King)
                    {
                        if (board.getToolInCell(j, i).getColor() == color)
                        {
                            return (King)board.getToolInCell(j, i);
                        }
                    }

                }
            }
            return null;
        }
        public static bool isWin(Board board, bool color)
        {
            if (isChess(board, color))
            {
                for (int x = 1; x <= 8; x++)
                {
                    for (char y = 'A'; y <= 'H'; y++)
                    {
                        for (int i = 1; i <= 8; i++)
                        {
                            for (char j = 'A'; j <= 'H'; j++)
                            {
                                if (board.getToolInCell(y, x) != null)
                                {
                                    if (board.getToolInCell(y, x).isMoveLegal(board, y, x, j, i))
                                    {
                                        if (board.getToolInCell(y, x).getColor() == color)
                                        {
                                            if (!board.isStillInChessMode(board, y, x, j, i, color))
                                                return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }
            return false;
        }
        public bool isPat()
        {
            if (!isChess(boardGame, color) && !isChess(boardGame, !color))
            {
                if (!isWin(boardGame, color))
                {
                    for (int i = 1; i <= 8; i++)
                    {
                        for (char j = 'A'; j <= 'H'; j++)
                        {
                            for (int x = 1; x <= 8; x++)
                            {
                                for (char y = 'A'; y <= 'H'; y++)
                                {
                                    if (boardGame.getToolInCell(j, i) != null)
                                    {
                                        if (boardGame.getToolInCell(j, i).isMoveLegal(boardGame, j, i, y, x))
                                        {
                                            if (boardGame.isStillInChessMode(boardGame, j, i, y, x, color))
                                                return false;
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public bool isLackOfMoves()
        {
            int counter = 0;
            for (int i = 1; i <= 8; i++)
            {
                for (char j = 'A'; j <= 'H'; j++)
                {
                    if (boardGame.getToolInCell(j, i) != null)
                        counter++;
                }
            }
            if (counter == 2)
                return true;
            if (counter > 2 && counter <= 4)
            {
                for (int i = 1; i <= 8; i++)
                {
                    for (char j = 'A'; j <= 'H'; j++)
                    {
                        if (boardGame.getToolInCell(j, i) != null)
                        {
                            if (!(boardGame.getToolInCell(j, i) is Bishop || boardGame.getToolInCell(j, i) is Knight))
                                return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }
        public static int getCounter() { return counterUpTo50Moves; }
        public static void setCounter(int counter) { counterUpTo50Moves = counter; }
        public static bool isEaten(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            if (board.getToolInCell(currentLetter, currentDigit).isMoveLegal(board, currentLetter, currentDigit, nextLetter, nextDigit))
            {
                if (board.getToolInCell(nextLetter, nextDigit) != null)
                    return true;
            }
            return false;
        }
    }
    class Board
    {
        Tool[,] board;
        Tool lastToolMoved = new Tool();
        public Board()
        {
            board = new Tool[,] {{new Rook(false,"A1"),new Knight(false,"B1"),new Bishop(false,"C1"),new Queen(false,"D1"),new King(false,"E1"),new Bishop(false,"F1"),new Knight(false,"G1"),new Rook(false,"H1")},
                                 {new Pawn(false,"A2"),new Pawn(false,"B2"),new Pawn(false,"C2"),new Pawn(false,"D2"),new Pawn(false,"E2"),new Pawn(false,"F2"),new Pawn(false,"G2"),new Pawn(false,"H2")},
                                 {null,null,null,null,null,null,null,null},
                                 {null,null,null,null,null,null,null,null},
                                 {null,null,null,null,null,null,null,null},
                                 {null,null,null,null,null,null,null,null},
                                 {new Pawn(true,"A7"),new Pawn(true,"B7"),new Pawn(true,"C7"),new Pawn(true,"D7"),new Pawn(true,"E7"),new Pawn(true,"F7"),new Pawn(true,"G7"),new Pawn(true,"H7")},
                                 {new Rook(true,"A8"),new Knight(true,"B8"),new Bishop(true,"C8"),new Queen(true,"D8"),new King(true,"E8"),new Bishop(true,"F8"),new Knight(true,"G8"),new Rook(true,"H8")}
                                };
        }
        public Tool[,] getBoard() { return board; }
        public override string ToString()
        {
            string output = "X\t" + " A\t" + " B\t" + " C\t" + " D\t" + " E\t" + " F\t" + " G\t" + " H\t" + "\n";
            for (int i = 0; i < 8; i++)
            {
                output += i + 1 + "\t";
                for (int j = 0; j < 8; j++)
                {
                    if (board[i, j] != null)
                        output += ("|" + board[i, j].ToString() + "|\t");
                    else
                        output += "| |\t";
                }
                output += "\n\n";
            }
            return output;
        }
        public Tool getLastToolMoved() { return lastToolMoved; }
        public void setLastToolMoved(Tool tool) { lastToolMoved = tool; }
        public Tool getToolInCell(char currentLetter, int currentDigit) { return board[currentDigit - 1, currentLetter - 'A']; }
        public void setToolInCell(char letter, int digit, Tool tool) { board[digit - 1, letter - 'A'] = tool; }
        // Return false on failure
        public bool moveTool(Board boardGame, char currentLetter, int currentDigit, char nextLetter, int nextDigit, bool color)
        {
            bool tempIsMoved = false;
            if (currentLetter < 'A' || currentLetter > 'H' || nextLetter < 'A' || nextLetter > 'H')
                return false;
            if (currentDigit < 0 || currentDigit > 8 || nextDigit < 0 || nextDigit > 8)
                return false;
            Tool currentTool = this.getToolInCell(currentLetter, currentDigit);
            if (currentTool == null)
                return false;
            if (boardGame.getToolInCell(nextLetter, nextDigit) != null)
            {
                if (boardGame.getToolInCell(currentLetter, currentDigit).getColor() == boardGame.getToolInCell(nextLetter, nextDigit).getColor())
                    return false;
            }
            if (boardGame.getToolInCell(currentLetter, currentDigit).getColor() != color)
                return false;
            if (!currentTool.isMoveLegal(boardGame, currentLetter, currentDigit, nextLetter, nextDigit))
                return false;
            if (isStillInChessMode(boardGame, currentLetter, currentDigit, nextLetter, nextDigit, color))
                return false;
            Tool lastToolSave = lastToolMoved;
            if (currentTool is Rook)
                tempIsMoved = ((Rook)currentTool).getIsMoved();
            if (currentTool is King)
                tempIsMoved = ((King)currentTool).getIsMoved();
            currentTool.MoveTool(boardGame, currentLetter, currentDigit, nextLetter, nextDigit);
            lastToolMoved = currentTool;
            if (GamePlay.isChess(boardGame, color))
            {
                currentTool.MoveTool(boardGame, nextLetter, nextDigit, currentLetter, currentDigit);
                lastToolMoved = lastToolSave;
                if (currentTool is Rook)
                    ((Rook)currentTool).setIsMoved(tempIsMoved);
                if (currentTool is King)
                    ((King)currentTool).setIsMoved(tempIsMoved);
                if (currentTool is Pawn)
                    ((Pawn)currentTool).setNumberOfTimesMoved(((Pawn)currentTool).getNumberOfTimesMoved() - 1);
                return false;
            }
            Console.WriteLine(boardGame);
            return true;
        }
        public bool checkInput(string input)
        {
            if (input == null)
                return false;
            if (input.Length != 4)
                return false;
            if (!(input[1] >= '1' && input[1] <= '8' && input[3] >= '1' && input[3] <= '8'))
                return false;
            if (!(input[0] >= 'A' && input[0] <= 'H' && input[2] >= 'A' && input[2] <= 'H'))
                return false;
            return true;
        }
        public bool isStillInChessMode(Board boardGame, char currentLetter, int currentDigit, char nextLetter, int nextDigit, bool color)
        {
            Tool lastToolSave, toolEaten;
            bool tempIsMoved = false;
            bool result = false;
            string tempLocation = "";
            int tempCounter;
            //hold all data to restore and move the tool
            lastToolSave = getLastToolMoved();
            tempLocation = getToolInCell(currentLetter, currentDigit).getLocation();
            if (getToolInCell(currentLetter, currentDigit) is Rook)
                tempIsMoved = ((Rook)getToolInCell(currentLetter, currentDigit)).getIsMoved();
            if (getToolInCell(currentLetter, currentDigit) is King)
                tempIsMoved = ((King)getToolInCell(currentLetter, currentDigit)).getIsMoved();
            toolEaten = getToolInCell(nextLetter, nextDigit);
            tempCounter = GamePlay.getCounter();
            getToolInCell(currentLetter, currentDigit).MoveTool(boardGame, currentLetter, currentDigit, nextLetter, nextDigit);
            //check if still chess
            result = GamePlay.isChess(boardGame, color);
            //bringing everything back
            getToolInCell(nextLetter, nextDigit).MoveTool(boardGame, nextLetter, nextDigit, currentLetter, currentDigit);
            GamePlay.setCounter(tempCounter);
            setToolInCell(nextLetter, nextDigit, toolEaten);
            setLastToolMoved(lastToolSave);
            getToolInCell(currentLetter, currentDigit).setLocation(tempLocation);
            if (getToolInCell(currentLetter, currentDigit) is Rook)
                ((Rook)getToolInCell(currentLetter, currentDigit)).setIsMoved(tempIsMoved);
            if (getToolInCell(currentLetter, currentDigit) is King)
                ((King)getToolInCell(currentLetter, currentDigit)).setIsMoved(tempIsMoved);
            if (getToolInCell(currentLetter, currentDigit) is Pawn)
                ((Pawn)getToolInCell(currentLetter, currentDigit)).setNumberOfTimesMoved(((Pawn)getToolInCell(currentLetter, currentDigit)).getNumberOfTimesMoved() - 2);
            return result;
        }
    }
    class Tool
    {
        protected bool color;
        string location;
        public Tool(bool color, string location)
        {
            setLocation(location);
            setColor(color);
        }
        public Tool() { }
        public void setLocation(string location) { this.location = location; }
        public string getLocation() { return location; }
        public void setColor(bool color) { this.color = color; }
        public bool getColor() { return color; }
        public override string ToString() { return color ? "W" : "B"; }
        public virtual bool isMoveLegal(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit) { return true; }
        public virtual void MoveTool(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            board.getToolInCell(currentLetter, currentDigit).setLocation(nextLetter + "" + nextDigit);
            if (board.getToolInCell(currentLetter, currentDigit) is Pawn)
                GamePlay.setCounter(GamePlay.getCounter() + 1);
            else
            {
                if (GamePlay.isEaten(board, currentLetter, currentDigit, nextLetter, nextDigit))
                    GamePlay.setCounter(0);
                else
                    GamePlay.setCounter(GamePlay.getCounter() + 1);
            }
        }
        public void changeCells(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit, Tool currentTool)
        {
            board.setToolInCell(nextLetter, nextDigit, currentTool);
            board.setToolInCell(currentLetter, currentDigit, null);
        }
    }
    class Pawn : Tool
    {
        int numberOfTimesMoved = 0;
        public Pawn(bool color, string location) : base(color, location) { }
        public int getNumberOfTimesMoved() { return numberOfTimesMoved; }
        public void setNumberOfTimesMoved(int numberOfTimesMoved) { this.numberOfTimesMoved = numberOfTimesMoved; }
        public override string ToString() { return base.ToString() + "P"; }
        public override void MoveTool(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            Tool tempTool = new Tool();
            //promotion check/move
            tempTool = promotion(nextLetter, nextDigit);
            if (numberOfTimesMoved != 0 && tempTool != null)
            {
                base.MoveTool(board, currentLetter, currentDigit, nextLetter, nextDigit);
                changeCells(board, currentLetter, currentDigit, nextLetter, nextDigit, tempTool);
                numberOfTimesMoved++;
                return;
            }
            if (board.getToolInCell(nextLetter, nextDigit) != null)
            {
                if ((color && nextDigit < currentDigit) || (!color && nextDigit > currentDigit))
                {
                    if (Math.Abs(currentDigit - nextDigit) == Math.Abs(currentLetter - nextLetter) && Math.Abs(currentDigit - nextDigit) == 1)
                    {
                        if (tempTool != null)
                        {
                            base.MoveTool(board, currentLetter, currentDigit, nextLetter, nextDigit);
                            numberOfTimesMoved++;
                            changeCells(board, currentLetter, currentDigit, nextLetter, nextDigit, tempTool);
                            return;
                        }
                    }
                }
            }
            base.MoveTool(board, currentLetter, currentDigit, nextLetter, nextDigit);
            changeCells(board, currentLetter, currentDigit, nextLetter, nextDigit, this);
            numberOfTimesMoved++;
            return;
        }
        public override bool isMoveLegal(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            if (board.getToolInCell(nextLetter, nextDigit) == null)
            {
                if (currentLetter == nextLetter)
                {
                    if (numberOfTimesMoved != 0)
                    {
                        if ((color && (nextDigit - currentDigit == -1)) || (!color && (nextDigit - currentDigit == 1)))
                            return true;
                    }
                    else
                    {
                        if (Math.Abs(nextDigit - currentDigit) == 2)
                        {
                            if (board.getToolInCell(currentLetter, currentDigit).getColor())
                            {
                                if (board.getToolInCell(currentLetter, currentDigit - 1) == null)
                                    return true;
                            }
                            else
                            {
                                if (board.getToolInCell(currentLetter, currentDigit + 1) == null)
                                    return true;
                            }
                        }
                        if ((!color && (nextDigit - currentDigit == 1)) || (color && (nextDigit - currentDigit == -1)))
                            return true;
                        else
                            return false;
                    }
                }
                else
                {
                    if (enPassant(board, currentLetter, currentDigit, nextLetter, nextDigit))
                        return true;
                }
                return false;
            }
            if (board.getToolInCell(nextLetter, nextDigit).getColor() != this.getColor())
            {
                if ((color && nextDigit < currentDigit) || (!color && nextDigit > currentDigit))
                {
                    if (Math.Abs(currentDigit - nextDigit) == Math.Abs(currentLetter - nextLetter) && Math.Abs(currentDigit - nextDigit) == 1)
                        return true;
                }
            }
            return false;
        }
        public bool enPassant(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            Tool lastToolMoved = board.getLastToolMoved();
            string location = lastToolMoved.getLocation();
            if (location == null)
                return false;
            char lastToolLetter = location[0];
            int lastToolDigit = int.Parse(location[1] + "");
            if (!(Math.Abs(currentDigit - nextDigit) == Math.Abs(currentLetter - nextLetter) && Math.Abs(currentDigit - nextDigit) == 1))
                return false;
            if (lastToolMoved.getColor() == color)
                return false;
            if (!(lastToolMoved is Pawn))
                return false;
            if (((Pawn)lastToolMoved).numberOfTimesMoved != 1)
                return false;
            if (lastToolDigit != currentDigit || lastToolLetter != nextLetter)
                return false;
            if ((color && currentDigit != 4) || (!color && currentDigit != 5))
                return false;
            board.setToolInCell(lastToolLetter, lastToolDigit, null);
            return true;
        }
        public Tool promotion(int nextLetter, int nextDigit)
        {
            string input;
            Tool tool;
            if ((color && nextDigit == 1) || (!color && nextDigit == 8))
            {
                do
                {
                    Console.WriteLine("Congrats your pawn can be promoted, please enter the tool you want : (Q for queen,N for knight,R for rook,B for bishop,P for pawn)");
                    input = Console.ReadLine();
                    input = input.Trim().ToUpper();
                    if (input.Length == 1)
                    {
                        switch (input[0])
                        {
                            case 'P':
                                tool = new Pawn(color, nextLetter + "" + nextDigit);
                                return tool;
                            case 'B':
                                tool = new Bishop(color, nextLetter + "" + nextDigit);
                                return tool;
                            case 'Q':
                                tool = new Queen(color, nextLetter + "" + nextDigit);
                                return tool;
                            case 'N':
                                tool = new Knight(color, nextLetter + "" + nextDigit);
                                return tool;
                            case 'R':
                                tool = new Rook(color, nextLetter + "" + nextDigit);
                                return tool;
                            default:
                                Console.WriteLine("wrong input, please try again :");
                                break;
                        }
                    }
                    else
                        Console.WriteLine("wrong input, please try again :");
                } while (true);
            }
            return null;
        }
    }
    class Bishop : Tool
    {
        public Bishop(bool color, string location) : base(color, location) { }
        public override string ToString() { return base.ToString() + "B"; }
        public override void MoveTool(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            base.MoveTool(board, currentLetter, currentDigit, nextLetter, nextDigit);
            changeCells(board, currentLetter, currentDigit, nextLetter, nextDigit, this);
        }
        public override bool isMoveLegal(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            if ((board.getToolInCell(nextLetter, nextDigit) == null) || (board.getToolInCell(nextLetter, nextDigit).getColor() != this.getColor()))
            {
                if (Math.Abs(currentDigit - nextDigit) == Math.Abs(currentLetter - nextLetter))
                {
                    if (currentDigit < nextDigit && currentLetter < nextLetter)//right and down
                    {
                        for (int i = currentDigit + 1, j = currentLetter + 1; i < nextDigit && j < nextLetter; i++, j++)
                        {
                            if (board.getToolInCell((char)j, i) != null)
                                return false;
                        }
                    }
                    if (nextDigit < currentDigit && currentLetter < nextLetter)//right and up
                    {
                        for (int i = currentDigit - 1, j = currentLetter + 1; i > nextDigit && j < nextLetter; i--, j++)
                        {
                            if (board.getToolInCell((char)j, i) != null)
                                return false;
                        }
                    }
                    if (currentDigit < nextDigit && nextLetter < currentLetter)//left and down
                    {
                        for (int i = currentDigit + 1, j = currentLetter - 1; i < nextDigit && j > nextLetter; i++, j--)
                        {
                            if (board.getToolInCell((char)j, i) != null)
                                return false;
                        }
                    }
                    if (nextDigit < currentDigit && nextLetter < currentLetter)//left and up
                    {
                        for (int i = nextDigit + 1, j = nextLetter + 1; i < currentDigit && j < currentLetter; i++, j++)
                        {
                            if (board.getToolInCell((char)j, i) != null)
                                return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
    class Knight : Tool
    {
        public Knight(bool color, string location) : base(color, location) { }
        public override string ToString() { return base.ToString() + "N"; }
        public override void MoveTool(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            base.MoveTool(board, currentLetter, currentDigit, nextLetter, nextDigit);
            changeCells(board, currentLetter, currentDigit, nextLetter, nextDigit, this);
        }
        public override bool isMoveLegal(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            if ((board.getToolInCell(nextLetter, nextDigit) == null) || (board.getToolInCell(nextLetter, nextDigit).getColor() != this.getColor()))
            {
                if (((Math.Abs(nextDigit - currentDigit) == 1) && (Math.Abs(nextLetter - currentLetter) == 2)) || ((Math.Abs(nextDigit - currentDigit) == 2) && (Math.Abs(nextLetter - currentLetter) == 1)))
                    return true;
            }
            return false;
        }
    }
    class Rook : Tool
    {
        bool isMoved = false;
        public Rook(bool color, string location) : base(color, location) { }
        public override string ToString() { return base.ToString() + "R"; }
        public bool getIsMoved() { return isMoved; }
        public void setIsMoved(bool isMoved) { this.isMoved = isMoved; }
        public override void MoveTool(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            base.MoveTool(board, currentLetter, currentDigit, nextLetter, nextDigit);
            isMoved = true;
            changeCells(board, currentLetter, currentDigit, nextLetter, nextDigit, this);
        }
        public override bool isMoveLegal(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            if ((board.getToolInCell(nextLetter, nextDigit) == null) || (board.getToolInCell(nextLetter, nextDigit).getColor() != this.getColor()))
            {
                if (nextDigit == currentDigit && nextLetter != currentLetter)
                {
                    if (nextLetter > currentLetter)
                    {
                        for (int i = currentLetter + 1; i < nextLetter; i++)
                        {
                            if (board.getToolInCell((char)i, nextDigit) != null)
                                return false;
                        }
                    }
                    else
                    {
                        for (int i = nextLetter + 1; i < currentLetter; i++)
                        {
                            if (board.getToolInCell((char)i, nextDigit) != null)
                                return false;
                        }
                    }
                    return true;
                }
                if (nextDigit != currentDigit && nextLetter == currentLetter)
                {
                    if (nextDigit > currentDigit)
                    {
                        for (int i = currentDigit + 1; i < nextDigit; i++)
                        {
                            if (board.getToolInCell(nextLetter, i) != null)
                                return false;
                        }
                    }
                    else
                    {
                        for (int i = nextDigit + 1; i < currentDigit; i++)
                        {
                            if (board.getToolInCell(nextLetter, i) != null)
                                return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
    }
    class King : Tool
    {
        bool isMoved = false;
        public King(bool color, string location) : base(color, location) { }
        public bool getIsMoved() { return isMoved; }
        public void setIsMoved(bool isMoved) { this.isMoved = isMoved; }
        public override string ToString() { return base.ToString() + "K"; }
        public override void MoveTool(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            base.MoveTool(board, currentLetter, currentDigit, nextLetter, nextDigit);
            isMoved = true;
            changeCells(board, currentLetter, currentDigit, nextLetter, nextDigit, this);
        }
        public override bool isMoveLegal(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            if ((board.getToolInCell(nextLetter, nextDigit) == null) || (board.getToolInCell(nextLetter, nextDigit).getColor() != this.getColor()))
            {
                if ((Math.Abs(nextDigit - currentDigit) == 0 || Math.Abs(nextDigit - currentDigit) == 1) && (Math.Abs(nextLetter - currentLetter) == 0 || Math.Abs(nextLetter - currentLetter) == 1))
                    return true;
            }
            if (this.castling(board, currentLetter, currentDigit, nextLetter, nextDigit))
                return true;
            return false;
        }
        public bool castling(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            //getting the rook that doing the castling
            Tool tool = board.getToolInCell(currentLetter < nextLetter ? 'H' : 'A', color ? 8 : 1);
            if (currentDigit != nextDigit || currentLetter == nextLetter)
                return false;
            if (Math.Abs(currentLetter - nextLetter) != 2)
                return false;
            if (this.isMoved)
                return false;
            if (!(tool is Rook))
                return false;
            if (((Rook)tool).getIsMoved())
                return false;
            int whichWayToGO = currentLetter < nextLetter ? 1 : -1;//+1 to the right, -1 to the left
            if (board.getToolInCell((char)(currentLetter + whichWayToGO), currentDigit) != null)
                return false;
            //if it's the big castling check the ['B',digit] location
            if (whichWayToGO == -1)
            {
                if (board.getToolInCell('B', nextDigit) != null)
                    return false;
            }
            //check if not entering into chess in the castling

            this.MoveTool(board, currentLetter, currentDigit, (char)(currentLetter + whichWayToGO), nextDigit);
            if (GamePlay.isChess(board, color))
            {
                this.MoveTool(board, (char)(currentLetter + whichWayToGO), nextDigit, currentLetter, currentDigit);
                isMoved = false;
                return false;
            }
            this.MoveTool(board, (char)(currentLetter + whichWayToGO), nextDigit, currentLetter, currentDigit);
            this.MoveTool(board, currentLetter, currentDigit, nextLetter, nextDigit);
            if (GamePlay.isChess(board, color))
            {
                this.MoveTool(board, nextLetter, nextDigit, currentLetter, currentDigit);
                isMoved = false;
                return false;
            }
            //return the tool back
            this.MoveTool(board, nextLetter, nextDigit, currentLetter, currentDigit);
            isMoved = false;
            ((Rook)tool).setLocation((currentLetter + whichWayToGO) + "" + nextDigit);
            board.setToolInCell((char)(currentLetter + whichWayToGO), nextDigit, tool);
            board.setToolInCell(currentLetter < nextLetter ? 'H' : 'A', currentDigit, null);
            ((Rook)tool).setIsMoved(false);
            return true;
        }
    }
    class Queen : Bishop
    {
        public Queen(bool color, string location) : base(color, location) { }
        public override string ToString() { return (color ? "W" : "B") + "Q"; }
        public override void MoveTool(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            base.MoveTool(board, currentLetter, currentDigit, nextLetter, nextDigit);
            changeCells(board, currentLetter, currentDigit, nextLetter, nextDigit, this);
        }
        public override bool isMoveLegal(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            bool result = new Rook(color, currentLetter + "" + currentDigit).isMoveLegal(board, currentLetter, currentDigit, nextLetter, nextDigit);
            if (board.getToolInCell(nextLetter, nextDigit) == null || (board.getToolInCell(nextLetter, nextDigit).getColor() != this.getColor()))
            {
                if (base.isMoveLegal(board, currentLetter, currentDigit, nextLetter, nextDigit) || result)
                    return true;
            }
            return false;
        }
    }
}
/* board = new Tool[,] {
                                {null, null, new King(false, "C1"), null, null, null, null, null},
                                {null, null, null, null, null, null, null, null},
                                {null,null,null,null,null,null,null,null},
                                {null,null,null,null,new Bishop(true, "E4"),null,null,null},
                                {null,null,null,new Queen(true, "D5"),null,null,null,null},
                                {null,null,null,null,new King(true, "E6"),null,null,null},
                                {null, null, null, null, null, null, null, null},
                                {null,null,null,null,null,null,null,null},
                                };*/