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
        Board[] threeLastBoards = new Board[3];
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
                while (!makeAMoveIfPossible(boardGame, input[0], int.Parse(input[1] + ""), input[2], int.Parse(input[3] + ""), color))
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
                if (isPat() || isLackOfMoves() || getCounterUpTo50Moves() >= 50 || isThreefoldRepetition())
                {
                    Console.WriteLine("It's a draw !");
                    return;
                }
                if (isChess(boardGame, !color))
                    Console.WriteLine("It's {0} chess", color ? "White" : "Black");
                color = !color;
            } while (!isWin());
            Console.WriteLine("Congrats the {0} player win!", color ? "Black" : "White");
            return;
        }
        public bool isThreefoldRepetition()
        {
            if(threeLastBoards[2]==null || threeLastBoards[1]==null)
                return false;
            threeLastBoards[2] = threeLastBoards[1].copy();
            threeLastBoards[1] = threeLastBoards[0].copy();
            threeLastBoards[0] = boardGame.copy();
            if (threeLastBoards[1] == null || threeLastBoards[2] == null)
                return false;
            for (int i = 1; i <= 8; i++)
            {
                for (char j = 'A'; j <= 'H'; j++)
                {
                    if (threeLastBoards[0].getToolInCell(j, i) != threeLastBoards[1].getToolInCell(j, i) || threeLastBoards[0].getToolInCell(j, i) != threeLastBoards[2].getToolInCell(j, i))
                        return false;
                    if (threeLastBoards[0].getToolInCell(j, i) is Rook)
                    {
                        if (((Rook)threeLastBoards[0].getToolInCell(j, i)).getIsMoved() != ((Rook)threeLastBoards[1].getToolInCell(j, i)).getIsMoved() || ((Rook)threeLastBoards[0].getToolInCell(j, i)).getIsMoved() != ((Rook)threeLastBoards[2].getToolInCell(j, i)).getIsMoved())
                          return false;
                    }
                    if (threeLastBoards[0].getToolInCell(j, i) is King)
                    {
                        if (((King)threeLastBoards[0].getToolInCell(j, i)).getIsMoved() != ((King)threeLastBoards[1].getToolInCell(j, i)).getIsMoved() || ((King)threeLastBoards[0].getToolInCell(j, i)).getIsMoved() != ((King)threeLastBoards[2].getToolInCell(j, i)).getIsMoved())
                            return false;
                    }
                    if (threeLastBoards[0].getToolInCell(j, i) is Pawn)
                    {
                        for (int x = 1; x <= 8; x++)
                        {
                            for (char y = 'A'; y <= 'H'; y++)
                            {
                                if(((Pawn)threeLastBoards[0].getToolInCell(j, i)).enPassant(boardGame,j,i,y,x)!=((Pawn)threeLastBoards[1].getToolInCell(j, i)).enPassant(boardGame, j, i, y, x)|| ((Pawn)threeLastBoards[0].getToolInCell(j, i)).enPassant(boardGame, j, i, y, x) != ((Pawn)threeLastBoards[2].getToolInCell(j, i)).enPassant(boardGame, j, i, y, x))
                                    return false;
                            }
                        }
                    }
                }
            }
            return true;
        }
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
        public bool isWin()
        {
            if (isChess(boardGame, color))
                return runThroughAllBoard();
            return false;
        }
        public bool isPat()
        {
            if (!isChess(boardGame, color) && !isChess(boardGame, !color))
            {
                if (!isWin())
                    return runThroughAllBoard();
            }
            return false;
        }
        public bool runThroughAllBoard()
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
                                    if (boardGame.getToolInCell(j, i).getColor() == color)
                                    {
                                        if (!isStillInChessMode(boardGame, j, i, y, x, color))
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
        public static int getCounterUpTo50Moves() { return counterUpTo50Moves; }
        public static void setCounterUpTo50Moves(int counter) { counterUpTo50Moves = counter; }
        public static bool isEaten(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit)
        {
            if (board.getToolInCell(currentLetter, currentDigit).isMoveLegal(board, currentLetter, currentDigit, nextLetter, nextDigit))
            {
                if (board.getToolInCell(nextLetter, nextDigit) != null)
                    return true;
            }
            return false;
        }
        public bool isStillInChessMode(Board boardGame, char currentLetter, int currentDigit, char nextLetter, int nextDigit, bool color)
        {
            Tool lastToolSave, toolEaten;
            bool holdPreviousIsMoved = false, result;
            string tempLocation;
            int tempCounter;
            //hold all data to restore and move the tool
            lastToolSave = boardGame.getLastToolMoved();
            tempLocation = boardGame.getToolInCell(currentLetter, currentDigit).getLocation();
            if (boardGame.getToolInCell(currentLetter, currentDigit) is Rook)
                holdPreviousIsMoved = ((Rook)boardGame.getToolInCell(currentLetter, currentDigit)).getIsMoved();
            if (boardGame.getToolInCell(currentLetter, currentDigit) is King)
                holdPreviousIsMoved = ((King)boardGame.getToolInCell(currentLetter, currentDigit)).getIsMoved();
            toolEaten = boardGame.getToolInCell(nextLetter, nextDigit);
            tempCounter = getCounterUpTo50Moves();
            boardGame.getToolInCell(currentLetter, currentDigit).MoveTool(boardGame, currentLetter, currentDigit, nextLetter, nextDigit);
            //check if still chess
            result = isChess(boardGame, color);
            //bringing everything back
            boardGame.getToolInCell(nextLetter, nextDigit).MoveTool(boardGame, nextLetter, nextDigit, currentLetter, currentDigit);
            setCounterUpTo50Moves(tempCounter);
            boardGame.setToolInCell(nextLetter, nextDigit, toolEaten);
            boardGame.setLastToolMoved(lastToolSave);
            boardGame.getToolInCell(currentLetter, currentDigit).setLocation(tempLocation);
            if (boardGame.getToolInCell(currentLetter, currentDigit) is Rook)
                ((Rook)boardGame.getToolInCell(currentLetter, currentDigit)).setIsMoved(holdPreviousIsMoved);
            if (boardGame.getToolInCell(currentLetter, currentDigit) is King)
                ((King)boardGame.getToolInCell(currentLetter, currentDigit)).setIsMoved(holdPreviousIsMoved);
            if (boardGame.getToolInCell(currentLetter, currentDigit) is Pawn)
                ((Pawn)boardGame.getToolInCell(currentLetter, currentDigit)).setNumberOfTimesMoved(((Pawn)boardGame.getToolInCell(currentLetter, currentDigit)).getNumberOfTimesMoved() - 2);
            return result;
        }
        // Return false on failure
        public bool makeAMoveIfPossible(Board boardGame, char currentLetter, int currentDigit, char nextLetter, int nextDigit, bool color)
        {
            bool tempIsMoved = false;
            if (currentLetter < 'A' || currentLetter > 'H' || nextLetter < 'A' || nextLetter > 'H')
                return false;
            if (currentDigit < 0 || currentDigit > 8 || nextDigit < 0 || nextDigit > 8)
                return false;
            Tool currentTool = boardGame.getToolInCell(currentLetter, currentDigit);
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
            Tool lastToolSave = boardGame.getLastToolMoved();
            if (currentTool is Rook)
                tempIsMoved = ((Rook)currentTool).getIsMoved();
            if (currentTool is King)
                tempIsMoved = ((King)currentTool).getIsMoved();
            currentTool.MoveTool(boardGame, currentLetter, currentDigit, nextLetter, nextDigit);
            boardGame.setLastToolMoved(currentTool);
            if (isChess(boardGame, color))
            {
                currentTool.MoveTool(boardGame, nextLetter, nextDigit, currentLetter, currentDigit);
                boardGame.setLastToolMoved(lastToolSave);
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
        public Board copy()
        {
            Board result = new Board();
            for(int i=0;i<8;i++)
                for(int j=0;j<8;j++)
                      result.board[i,j] = this.board[i,j].copy();
            result.lastToolMoved=this.lastToolMoved;
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
                GamePlay.setCounterUpTo50Moves(GamePlay.getCounterUpTo50Moves() + 1);
            else
            {
                if (GamePlay.isEaten(board, currentLetter, currentDigit, nextLetter, nextDigit))
                    GamePlay.setCounterUpTo50Moves(0);
                else
                    GamePlay.setCounterUpTo50Moves(GamePlay.getCounterUpTo50Moves() + 1);
            }
        }
        public void changeCells(Board board, char currentLetter, int currentDigit, char nextLetter, int nextDigit, Tool currentTool)
        {
            board.setToolInCell(nextLetter, nextDigit, currentTool);
            board.setToolInCell(currentLetter, currentDigit, null);
        }
        public Tool copy()
        {
            Tool result=new Tool();
            result.color=this.color;
            result.location=this.location;
            return result;
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
            Tool tempTool;
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
                    {
                        Tool lastToolMoved = board.getLastToolMoved();
                        string location = lastToolMoved.getLocation();
                        if (location == null)
                            return false;
                        char lastToolLetter = location[0];
                        int lastToolDigit = int.Parse(location[1] + "");
                        board.setToolInCell(lastToolLetter, lastToolDigit, null);
                        return true;
                    }
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
            return true;
        }
        public Tool promotion(int nextLetter, int nextDigit)
        {
            string input;
            Tool tool;
            if ((color && nextDigit == 1) || (!color && nextDigit == 8))
            {
                while(true)
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
                }
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
/* a board for checking stalemate
 * board = new Tool[,] {
                                {null, null, new King(false, "C1"), null, null, null, null, null},
                                {null, null, null, null, null, null, null, null},
                                {null,null,null,null,null,null,null,null},
                                {null,null,null,null,new Bishop(true, "E4"),null,null,null},
                                {null,null,null,new Queen(true, "D5"),null,null,null,null},
                                {null,null,null,null,new King(true, "E6"),null,null,null},
                                {null, null, null, null, null, null, null, null},
                                {null,null,null,null,null,null,null,null},
                                };*/