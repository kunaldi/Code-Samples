require 'test/unit'
require './letter_order'

class LetterOrderTest < Test::Unit::TestCase
  def setup
    @alphabet = LetterOrderTest.new("./alphabet.txt")
  end
 
  def test_matches
    result = @alphabet.letters
    assert_equal('abc', result)
  end
end
