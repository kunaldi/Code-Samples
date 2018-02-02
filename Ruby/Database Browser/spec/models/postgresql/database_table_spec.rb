require 'rails_helper'

RSpec.describe PostgreSQL::DatabaseTable, type: :model do

  before :context do
    @db = PostgreSQL::Database.new(name: 'booktown', deployment:
        PostgreSQL::Deployment.new(
            type: 'postgresql',
            host: 'localhost',
            port: 5432,
            name: 'funky-deployment-9',
            account_id: 1337
        ))

    @table = PostgreSQL::DatabaseTable.new(name: 'books', database: @db, schema: 'public')
  end


  describe '#columns' do

    it 'should return table column information in Array' do
      expect(@table.columns).to be_an Array
    end

  end


  describe '#schema and #default_schema' do

    it 'should have a prefix schema name if other than default' do
      @table.schema = 'myschema'
      expect(@table.schema_name).to eq 'myschema.books'
    end

    it 'hides default schema name' do
      @table.schema = 'public'
      expect(@table.default_schema).to be_nil

      @table.schema = 'myschema'
      expect(@table.default_schema).to eq 'myschema'
    end

  end

end
