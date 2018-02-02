require './chess_board'

class ChessPiece

  attr_accessor :name, :pos

  # Only accepts valid positions
  def initialize(name, pos)
    @name = name
    @pos = pos

    if pos.length > 2 || !valid_position?(*self.position)
      raise 'Invalid position'
    end
  end

  # Returns numeric [x, y] presentation of chess notation position
  def position
    [ChessBoard.col_index(pos[0]), pos[1].to_i]
  end

  # Returns boolean whether x, y pos are valid within 8x8 matrix
  def valid_position?(x, y)
    !!(x && y && x > 0 && y > 0 && x <= 8 && y <= 8)
  end

  # Normalize positions from numeric e.g (17, 28) format to chess notation e.g (d3, e8)
  def normalize_positions(pos_array)
    pos_array.map do |x, y|
      "#{ChessBoard::COLUMN_CODES[x-1]}#{y}"
    end
  end

  # Returns boolean whether target piece's position overlaps movable positions of this piece
  def can_attack?(target_piece)
    self.movable_squares.include? target_piece.pos
  end

end