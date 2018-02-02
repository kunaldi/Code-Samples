require 'test/unit'
require './chess_board'
require './queen'

class TestChessBoard < Test::Unit::TestCase

  def setup
    @board = ChessBoard.new
  end

  def test_init
    assert @board.squares.length == 8
    assert @board.squares[0] == %w(a8 b8 c8 d8 e8 f8 g8 h8)
  end

  def test_add_piece
    @wq = @board.add_piece(Queen, 'White Queen', 'b4')
    @bq = @board.add_piece(Queen, 'Black Queen', 'a6')

    assert @board.pieces.length == 2
    assert_equal @board.pieces[0], @wq
  end

  def test_col_index
    assert ChessBoard.col_index('c') == 3
    assert_nil ChessBoard.col_index('m')
  end

end