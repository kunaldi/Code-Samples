class PostgreSQL::DatabaseTable
  include Virtus.model

  attribute :database, PostgreSQL::Database
  attribute :schema, String
  attribute :name, String
  attribute :rows_count, String

  def columns
    database.client.exec_params('SELECT column_name, data_type FROM information_schema.columns WHERE table_schema = $1 AND table_name = $2;',
                                [schema, name]).map do |row|
      PostgreSQL::TableColumn.new(name: row['column_name'], data_type: row['data_type'], table: self)
    end
  end

  def schema_name
    schema == 'public' ? name : "#{schema}.#{name}"
  end

  def default_schema
    schema == 'public' ? nil : schema
  end


end