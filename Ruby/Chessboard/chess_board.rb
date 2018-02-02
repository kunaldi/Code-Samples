require './chess_piece'

class ChessBoard

  attr_reader :squares, :pieces

  COLUMN_CODES = ('a'..'h').to_a

  def initialize
    @pieces = []
    # This is not used yet
    @squares =
      8.downto(1).inject([]) do |r, y|
        r << ('a'..'h').inject([]) do |m, x|
          m << "#{x}#{y}"
        end
      end
  end

  # Public: Adds a new chess piece
  #
  # klass  - Class of the actual piece e.g Queen
  # name   - String name of the piece.
  # pos    - String position of the piece e.g d6
  #
  # Examples
  #
  #   add_piece(Queen, 'White Queen', 'd6')
  #
  # Returns new instance of klass piece.
  def add_piece(klass, name, pos)
    klass.new(name, pos).tap { |p| @pieces << p }
  end

  # Looks up for numeric value of letter based board column
  def self.col_index(col_code)
    COLUMN_CODES.index(col_code)&.next
  end

end