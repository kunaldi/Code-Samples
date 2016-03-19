require './student'

class Students

  def initialize(input_data=nil)
    # Parse/merge data files into a Student model array

    @data = []

    (input_data || Student::DEFAULT_INPUT_DATA).each do |delimeter, h|
      File.open(File.expand_path(h[:filename], '../data'), 'r').each_line do |row|
        cols = row.split(delimeter)

        attr = h[:columns].each_with_index.inject({}) do |m, (i, idx)|
          m[i.to_sym] = cols[idx].strip; m
        end

        @data << { student: Student.new(attr, h[:date_format])}
      end
    end
  end

  def sort(sort_col1, sort_col2=nil, sort_order1=:asc, sort_order2=:asc)
    # or sort_by
    @data.sort do |x, y|
      a = x[:student].send(sort_col1)
      b = y[:student].send(sort_col1)

      r = (sort_order1 == :asc ? a <=> b : b <=> a)

      if sort_col2
        c = x[:student].send(sort_col2)
        d = y[:student].send(sort_col2)
        r.nonzero? || (sort_order2 == :asc ? c <=> d : d <=> c)
      else
        r
      end
    end
  end

  def self.serialize_to_string_array(arr)
    arr.map {|i| i[:student].to_s}
  end


end