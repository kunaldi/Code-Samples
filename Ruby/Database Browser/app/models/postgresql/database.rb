class PostgreSQL::Database
  include Virtus.model

  attribute :deployment, PostgreSQL::Deployment
  attribute :name, String
  attribute :size, Integer

  def client
    deployment.client(name)
  end

  def tables
    client.exec(<<-eos
      SELECT
        nspname AS schemaname,relname,reltuples
      FROM pg_class C
      LEFT JOIN pg_namespace N ON (N.oid = C.relnamespace)
      WHERE
        nspname NOT IN ('pg_catalog', 'information_schema') AND
        relkind='r'
      ORDER BY reltuples DESC;
    eos
    ).map do |row|
      PostgreSQL::DatabaseTable.new(name: row['relname'], schema: row['schemaname'], rows_count: row['reltuples'], database: self, deployment: deployment)
    end
  end

  def queries(start_timestamp)
    qry_start = start_timestamp# && Time.at(start_timestamp.to_i).to_s(:db) rescue nil
    qry_start_crit = qry_start ? "WHERE query_start > '#{qry_start}'" : ''

    client.exec(<<-eos
      SELECT pid, usename, application_name, query_start, query
      FROM pg_stat_activity
      #{qry_start_crit}
      ORDER BY query_start DESC;
    eos
    ).map do |row|
      PostgreSQL::QueryStat.new(pid: row['pid'], app_name: row['application_name'], query_start: row['query_start'], query: row['query'])
    end
  end


end