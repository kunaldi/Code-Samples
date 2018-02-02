require 'test/unit'
require './chess_board'
require './chess_piece'

class TestChessPiece < Test::Unit::TestCase

  def setup
    @queen = ChessPiece.new 'Queen', 'c6'
  end

  def test_init
    assert @queen.pos = 'c6'
    assert @queen.name = 'Queen'
    assert_raise_message('Invalid position') { ChessPiece.new 'Queen', 'z90'}
  end

  def test_position
    assert_true @queen.valid_position?(3, 6), true
    assert_false @queen.valid_position?(0, 9), false
  end

  def test_valid_position
    assert @queen.position == [3, 6]

    @queen.pos = 'm9'
    assert @queen.position == [nil, 9]
  end

end