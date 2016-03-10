class LetterOrder
  def initialize(file_path)
    @letters = []
    
    File.open(file_path, 'r').each_line do |w|
      @letters << w
    end
  end
  
  def letters
    result
  end
end