require 'rails_helper'

RSpec.describe PostgreSQL::Database, type: :model do

  before :context do
    @db = PostgreSQL::Database.new(name: 'booktown', deployment:
      PostgreSQL::Deployment.new(
          type: 'postgresql',
          host: 'localhost',
          port: 5432,
          name: 'funky-deployment-9',
          account_id: 1337
    ))
  end


  describe '#tables' do

    before :context do
      @tables = @db.tables
    end

    it 'should return table information of a database in Array' do
      expect(@tables).to be_an Array
    end

    it 'should include books table for the booktown sample db' do
      expect(@tables.map(&:name)).to include 'books'
    end

  end


  describe '#queries' do

    it 'should return all query stats of a database in Array' do
      expect(@db.queries(nil)).to be_an Array
    end

    it 'should return only new query stats if a timestamp is given' do
      start_time = 10.minutes.ago
      stats = @db.queries(start_time)

      expect(stats).to be_an Array
      expect(DateTime.parse(stats.first.query_start).to_f).to be > start_time.to_f
    end
  end

end
