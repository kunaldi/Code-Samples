class PostgreSQL::DatabasesController < PostgreSQL::BaseController

  def index
    deployment = current_deployment or return head :not_found

    render locals: {databases: deployment.databases}
  end

  def live_queries
    new_queries = current_database.queries(params[:lq_timestamp])
    new_timestamp = new_queries[0].query_start#.to_time.to_i

    if request.xhr?
      res = render_to_string({template: 'postgresql/databases/_queries', locals: {queries: new_queries}, layout: false})

      render json: {queries: res, lq_timestamp: new_timestamp}
    else
      render locals: {queries: new_queries, lq_timestamp: new_timestamp}
    end

  rescue PG::ConnectionBad
    head :not_found
  end

end