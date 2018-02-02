Ruby Programming Exercise

You are to write a solution to the “Queen’s Attack” problem.
Your program should model a standard 8x8 chess board.
Input should be accepted via command-line and/or input file as pairs of coordinates on the board (either numeric or some other standard chess notation, your choice). Having selected two positions on the board, the program will report whether chess queens in those positions can attack one another.
In the game of chess, a queen can attack pieces that are on the same row, column, or diagonal.


Usage:
 ruby queen_attack.rb -h
 ruby queen_attack.rb -p pos1 pos2 (e.g d1, e3)

Classes
  ChessBoard
    - An instance for a chessboard that holds square, column names (a, b, c.. etc.) and pieces information
    - test_chess_board.rb includes unit tests for this class

  ChessPiece
    - Base class for any chess piece that holds common attributes and methods
    - test_chess_piece.rb includes unit tests for this class

  Queen < ChessPiece
    - Specific Queen piece. Subclass of ChessPiece class.
    - test_chess_queen.rb includes unit tests for this class


Main File
  queen_attack.rb
