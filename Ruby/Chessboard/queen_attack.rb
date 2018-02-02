require 'optparse'
require './chess_board'
require './queen'

options = {}

OptionParser.new do |opts|
  opts.banner = 'Usage: queen_attack [options]'

  # opts.on( '-f', '--file', 'Select positions file that includes x, y pos like "b4 d2"' ) do |pos_file|
  #   options[:pos_file] = pos_file
  # end

  opts.on( '-p', '--positions', 'or set two positions separated by space e.g "b4 d2"' ) do |positions|
    options[:positions] = positions
  end

  opts.on( '-h', '--help', 'Display this screen' ) do
    puts opts
    exit
  end
end.parse!

if options[:positions]
  begin
    # Create board and two queens
    board = ChessBoard.new
    wq = board.add_piece Queen, 'White Queen', ARGV[0]
    bq = board.add_piece Queen, 'Black Queen', ARGV[1]

    # Test if they can attack each other
    [[wq, bq], [bq, wq]].each do |q1, q2|
      attack = q1.can_attack?(q2) ? '' : 'not'
      puts "#{q1.name} [#{q1.pos}] can#{attack} attack #{q2.name} [#{q2.pos}]"
    end
  rescue StandardError => e
    puts e.message
  end

end