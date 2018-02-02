require 'test/unit'
require './chess_board'
require './queen'

class TestQueen < Test::Unit::TestCase

  def setup
    @board = ChessBoard.new
    @wq = @board.add_piece Queen, 'White Queen', 'd2'
    @bq = @board.add_piece Queen, 'Black Queen', 'g5'
  end

  def test_movable_squares
    assert @wq.movable_squares == %w(b3 b5 c3 c5 c4 a4 a3 a5 b2 b6 d2 d6 d4 b1 b7 e1 e7 e4 b8 f8 f4 g4 h4)

    @wq.pos = 'a1'
    assert @wq.movable_squares == %w(a2 b2 b1 a3 c3 c1 a4 d4 d1 a5 e5 e1 a6 f6 f1 a7 g7 g1 a8 h8 h1)
  end

  def test_can_attack
    assert_true @wq.can_attack?(@bq)
    assert_true @bq.can_attack?(@wq)

    @another_queen = @board.add_piece Queen, 'Another Queen', 'b4'

    assert_false @another_queen.can_attack?(@bq)
    assert_true @wq.can_attack?(@another_queen)
  end

end