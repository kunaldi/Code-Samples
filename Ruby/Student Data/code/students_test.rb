require 'test/unit'
require './students'

class StudentsTest < Test::Unit::TestCase

  def setup
    @students = Students.new
  end

  def test_output_1
    # Output 1
    expected_output = [
      'Kirlin Mckayla Atlanta 5/29/1986 Maroon',
      'Barrows Anika Hong Kong 5/5/1965 Spring Green',
      'Goyette Timmothy London 10/2/1964 Pacific Blue',
      'Nolan Rhiannon Los Angeles 10/4/1974 Vivid Tangerine',
      'Parker Matteo Melbourne 2/14/1962 Burnt Sienna',
      'Bednar Filomena New York City 1/24/1980 Salmon',
      'Cummerata Elliot New York City 4/3/1947 Neon Carrot',
      'Wilkinson Stacy New York City 1/22/1964 Shocking Pink',
      'Bruen Rigoberto San Francisco 12/1/1962 Raw Umber',
    ]

    r = @students.sort :campus, :last_name
    output = Students.serialize_to_string_array r
    assert_equal output, expected_output
  end

  def test_output_2
    # Output 2
    expected_output = [
      'Cummerata Elliot New York City 4/3/1947 Neon Carrot',
      'Parker Matteo Melbourne 2/14/1962 Burnt Sienna',
      'Bruen Rigoberto San Francisco 12/1/1962 Raw Umber',
      'Wilkinson Stacy New York City 1/22/1964 Shocking Pink',
      'Goyette Timmothy London 10/2/1964 Pacific Blue',
      'Barrows Anika Hong Kong 5/5/1965 Spring Green',
      'Nolan Rhiannon Los Angeles 10/4/1974 Vivid Tangerine',
      'Bednar Filomena New York City 1/24/1980 Salmon',
      'Kirlin Mckayla Atlanta 5/29/1986 Maroon',
    ]

    r = @students.sort :date_of_birth
    output = Students.serialize_to_string_array r
    assert_equal output, expected_output
  end

  def test_output_3
    # Output 3
    expected_output = [
      'Wilkinson Stacy New York City 1/22/1964 Shocking Pink',
      'Parker Matteo Melbourne 2/14/1962 Burnt Sienna',
      'Nolan Rhiannon Los Angeles 10/4/1974 Vivid Tangerine',
      'Kirlin Mckayla Atlanta 5/29/1986 Maroon',
      'Goyette Timmothy London 10/2/1964 Pacific Blue',
      'Cummerata Elliot New York City 4/3/1947 Neon Carrot',
      'Bruen Rigoberto San Francisco 12/1/1962 Raw Umber',
      'Bednar Filomena New York City 1/24/1980 Salmon',
      'Barrows Anika Hong Kong 5/5/1965 Spring Green'
    ]

    r = @students.sort :last_name, nil, :desc
    output = Students.serialize_to_string_array r
    assert_equal output, expected_output
  end
end