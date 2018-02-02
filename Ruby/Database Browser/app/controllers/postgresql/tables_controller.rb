class PostgreSQL::TablesController < PostgreSQL::BaseController

  helper_method :current_table

  def index
    render locals: {tables: current_database.tables}

  rescue PG::ConnectionBad
    head :not_found
  end

  def show
    @current_table = PostgreSQL::DatabaseTable.new(name: params[:table_name],
                                                   schema: params[:schema] || 'public',
                                                   database: current_database)

    render locals: {table: current_table,
                    columns: current_table.columns}
  end

  private

  def current_table
    @current_table
  end

end