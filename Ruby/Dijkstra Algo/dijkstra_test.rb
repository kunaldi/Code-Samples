require 'test/unit'
require './dijkstra'

class DijkstraTest < Test::Unit::TestCase

  def setup
    @graph = Dijkstra.new(
       [
           [1, 2, 7],
           [1, 3, 9],
           [1, 6, 14],
           [2, 3, 10],
           [2, 4, 15],
           [3, 6, 2],
           [3, 4, 11],
           [4, 5, 6],
           [5, 6, 9]
       ]
    )
  end

  def test_calc_shortest
    r = @graph.calc_shortest 1, 5
    assert_equal r[:shortest], 20
    assert_equal r[:distance], {1 => 0, 2 => 7, 3 => 9, 6 => 11, 4 => 20, 5 => 20}
    assert_equal r[:traverse], {2 => 1, 3 => 1, 6 => 3, 4 => 3, 5 => 6}

    # with different numbers (source, dest)
    r = @graph.calc_shortest 2, 6
    assert_equal r[:shortest], 12
    assert_equal r[:distance][6], 12 # alternative pull from distances

    r = @graph.calc_shortest 4, 5
    assert_equal r[:shortest], 6

    r = @graph.calc_shortest 5, 1
    assert_equal r[:shortest], 20

    r = @graph.calc_shortest 4, 6
    assert_equal r[:shortest], 13
  end

end