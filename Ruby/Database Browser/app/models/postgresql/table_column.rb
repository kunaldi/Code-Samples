class PostgreSQL::TableColumn
  include Virtus.model

  attribute :table, PostgreSQL::DatabaseTable
  attribute :name, String
  attribute :data_type, String

end