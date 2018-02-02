class PostgreSQL::QueryStat
  include Virtus.model

  attribute :pid, String
  attribute :app_name, String
  attribute :query_start, String
  attribute :query, String

end