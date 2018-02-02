require './chess_board'
require './chess_piece'

class Queen < ChessPiece

  # Specific movement ability of Queen piece
  # calculates up-down, left-right and each diagonals
  def movable_squares
    x, y = self.position

    squares = (1..8).inject([]) do |m, o|
      m << [
        [x, y - o],          # down offset
        [x, y + o],          # up
        [x + o, y - o],      # down right
        [x + o, y + o],      # up right
        [x + o, y],          # right
        [x - o, y],          # left
        [x - o, y - o],      # down left
        [x - o, y + o]       # up left
      ].keep_if do |i|
        valid_position? *i
      end
    end.flatten(1)

    normalize_positions squares
  end



end