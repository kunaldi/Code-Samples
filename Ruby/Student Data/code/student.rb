class Student
  require 'date'

  DEFAULT_INPUT_DATA = {
      ',' => {
          filename: 'comma.txt',
          columns: %w(last_name first_name campus favorite_color date_of_birth) },
      '$' => {
          filename: 'dollar.txt',
          columns: %w(last_name first_name middle_initial campus date_of_birth favorite_color),
          date_format: '%m-%d-%Y' },
      '|' => {
          filename: 'pipe.txt',
          columns: %w(last_name first_name middle_initial campus favorite_color date_of_birth),
          date_format: '%m-%d-%Y' },
  }

  FIELDS = %i(last_name first_name middle_initial campus date_of_birth favorite_color)

  FIELDS.each do |f|
    attr_accessor f
  end

  def initialize(attr={}, date_format=nil)
    # set given attributes to model
    attr.each do |k, v|
      self.send("#{k}=", v) if FIELDS.include? k
    end

    normalize_date date_format
    normalize_state if self.campus.length <= 3
  end

  def to_s
    %i(last_name first_name campus date_of_birth favorite_color).map do |f|
      v = self.send f
      v.is_a?(DateTime) ? v.strftime('%-m/%-d/%Y') : v
    end.join(' ')
  end

  protected


  def normalize_date(format)
    self.date_of_birth = DateTime.strptime(self.date_of_birth, format || '%m/%d/%Y')
  end

  def normalize_state
    states = ['LA', 'Los Angeles', 'NYC', 'New York City', 'SF', 'San Francisco']
    i = states.index(self.campus)
    self.campus = states[i+1] if i
  end
end